using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ExampleTester;

public static class FastCsprojCompilationParser
{
    public static CSharpCompilation CreateCompilation(string csprojPath)
    {
        var csproj = ParseCsproj(XDocument.Load(csprojPath), csprojPath);

        var syntaxTrees = Directory.GetFiles(Path.GetDirectoryName(csprojPath)!, "*.cs").AsParallel().Select(path =>
        {
            using var stream = File.OpenRead(path);
            return CSharpSyntaxTree.ParseText(SourceText.From(stream), csproj.ParseOptions, path);
        });

        return CSharpCompilation.Create(
            csproj.AssemblyName,
            csproj.GeneratedSources.AddRange(syntaxTrees),
            SupportedTargetFrameworks[csproj.TargetFramework].References,
            csproj.CompilationOptions);
    }

    private static readonly FrozenDictionary<string, OutputKind> SupportedOutputKinds = new KeyValuePair<string, OutputKind>[]
    {
        new("Exe", OutputKind.ConsoleApplication),
        new("Library", OutputKind.DynamicallyLinkedLibrary),
        new("WinExe", OutputKind.WindowsApplication),
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private sealed record TargetFrameworkInfo(
        ImmutableArray<PortableExecutableReference> References,
        LanguageVersion DefaultLanguageVersion,
        int DefaultWarningLevel);


    // When we add new rows here, we need to add the corresponding NuGet package to ExampleTester.csproj. These packages provide the information used
    // for the reference assemblies.
    private static readonly FrozenDictionary<string, TargetFrameworkInfo> SupportedTargetFrameworks = new KeyValuePair<string, TargetFrameworkInfo>[]
    {
        new("net6.0", new(Basic.Reference.Assemblies.Net60.References.All, LanguageVersion.CSharp10, DefaultWarningLevel: 6)),
        new("net7.0", new(Basic.Reference.Assemblies.Net70.References.All, LanguageVersion.CSharp11, DefaultWarningLevel: 7)),
        new("net8.0", new(Basic.Reference.Assemblies.Net80.References.All, LanguageVersion.CSharp12, DefaultWarningLevel: 8)),
        new("net9.0", new(Basic.Reference.Assemblies.Net90.References.All, LanguageVersion.CSharp13, DefaultWarningLevel: 9)),
        new("net10.0", new(Basic.Reference.Assemblies.Net100.References.All, LanguageVersion.CSharp14, DefaultWarningLevel: 10)),
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    // When we add new versions, this list changes. You'll need to change the current release from "NET9_0",
    // and add necessary "NET?_0_OR_GREATER" symbols.
    private static readonly CSharpParseOptions DefaultParseOptions = new(preprocessorSymbols: [
        "TRACE", "RELEASE", "NET", "NET9_0", "NETCOREAPP", "NET5_0_OR_GREATER", "NET6_0_OR_GREATER",
        "NET7_0_OR_GREATER","NET8_0_OR_GREATER","NET9_0_OR_GREATER",
        "NETCOREAPP1_0_OR_GREATER", "NETCOREAPP1_1_OR_GREATER", "NETCOREAPP2_0_OR_GREATER",
        "NETCOREAPP2_1_OR_GREATER",  "NETCOREAPP2_2_OR_GREATER", "NETCOREAPP3_0_OR_GREATER",
        "NETCOREAPP3_1_OR_GREATER"]);

    private static readonly CSharpCompilationOptions DefaultCompilationOptions = new(
        OutputKind.DynamicallyLinkedLibrary,
        deterministic: true,
        optimizationLevel: OptimizationLevel.Release,
        specificDiagnosticOptions: [
            new("NU1605", ReportDiagnostic.Error),
            new("CS1702", ReportDiagnostic.Suppress),
            new("CS1701", ReportDiagnostic.Suppress),
            new("CS8002", ReportDiagnostic.Suppress),
            new("SYSLIB0011", ReportDiagnostic.Error)]);

    public static CsprojParseResult ParseCsproj(XDocument csprojDocument, string filePath)
    {
        var projectName = Path.GetFileNameWithoutExtension(filePath);
        var assemblyName = projectName;
        var targetFramework = (string?)null;
        var parseOptions = DefaultParseOptions;
        var compilationOptions = DefaultCompilationOptions;
        var implicitUsings = false;

        foreach (var element in csprojDocument.Root!.Elements())
        {
            switch (element.Name.LocalName)
            {
                case "PropertyGroup":
                    foreach (var property in element.Elements())
                    {
                        switch (property.Name.LocalName)
                        {
                            case "OutputType":
                                compilationOptions = compilationOptions.WithOutputKind(SupportedOutputKinds[property.Value]);
                                break;
                            case "TargetFramework":
                                targetFramework = property.Value;
                                break;
                            case "Nullable":
                                compilationOptions = compilationOptions.WithNullableContextOptions(
                                    Enum.Parse<NullableContextOptions>(property.Value, ignoreCase: true));
                                break;
                            case "AssemblyName":
                                assemblyName = property.Value;
                                break;
                            case "AllowUnsafeBlocks":
                                compilationOptions = compilationOptions.WithAllowUnsafe(bool.Parse(property.Value));
                                break;
                            case "LangVersion":
                                var enumMemberName = property.Value;

                                if (decimal.TryParse(
                                    property.Value,
                                    NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint,
                                    CultureInfo.InvariantCulture,
                                    out var decimalValue))
                                {
                                    enumMemberName = "CSharp" + decimalValue.ToString("0.#", CultureInfo.InvariantCulture).Replace('.', '_');
                                }

                                parseOptions = parseOptions.WithLanguageVersion(Enum.Parse<LanguageVersion>(enumMemberName, ignoreCase: true));
                                break;
                            case "ImplicitUsings":
                                implicitUsings = ParseFeatureBool(property.Value);
                                break;
                            default:
                                throw new NotImplementedException($"Review whether support is required for <{property.Name}> ({filePath})");
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException($"Review whether support is required for <{element.Name}> ({filePath})");
            }
        }

        if (parseOptions.SpecifiedLanguageVersion == LanguageVersion.Default)
            parseOptions = parseOptions.WithLanguageVersion(SupportedTargetFrameworks[targetFramework!].DefaultLanguageVersion);

        compilationOptions = compilationOptions
            .WithWarningLevel(SupportedTargetFrameworks[targetFramework!].DefaultWarningLevel)
            .WithModuleName(assemblyName + ".dll");

        var generatedSources = ImmutableArray.CreateBuilder<SyntaxTree>();

        if (implicitUsings)
        {
            generatedSources.Add(SyntaxFactory.ParseSyntaxTree("""
                // <auto-generated/>
                global using System;
                global using System.Collections.Generic;
                global using System.IO;
                global using System.Linq;
                global using System.Net.Http;
                global using System.Threading;
                global using System.Threading.Tasks;

                """,
                parseOptions,
                projectName + ".GlobalUsings.g.cs"));
        }

        return new CsprojParseResult(
            assemblyName,
            targetFramework!,
            parseOptions,
            compilationOptions,
            generatedSources.DrainToImmutable());
    }

    private static bool ParseFeatureBool(string propertyValue)
    {
        return propertyValue.Equals("enable", StringComparison.OrdinalIgnoreCase)
            || propertyValue.Equals("true", StringComparison.OrdinalIgnoreCase);
    }
}
