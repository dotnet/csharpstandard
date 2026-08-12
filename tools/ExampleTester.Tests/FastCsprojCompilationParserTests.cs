using System.Collections.Immutable;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Shouldly;

namespace ExampleTester.Tests;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

public static class FastCsprojCompilationParserTests
{
    private const string CsprojFileName = "Test.csproj";

    private static CsprojParseResult ParseCsproj(string csprojContents)
    {
        var result = FastCsprojCompilationParser.ParseCsproj(XDocument.Parse(csprojContents), CsprojFileName);
        CompareMSBuildWorkspaceCompilation(csprojContents, result);
        return result;
    }

    private static void CompareMSBuildWorkspaceCompilation(string csprojContents, CsprojParseResult result)
    {
        var msbuildCompilation = GetMSBuildWorkspaceCompilation(csprojContents);

        result.AssemblyName.ShouldBe(msbuildCompilation.AssemblyName);

        var sanitizedCompilationOptions = msbuildCompilation.Options
            .WithAssemblyIdentityComparer(result.CompilationOptions.AssemblyIdentityComparer)
            .WithMetadataReferenceResolver(null)
            .WithSourceReferenceResolver(null)
            .WithStrongNameProvider(null)
            .WithSyntaxTreeOptionsProvider(null)
            .WithXmlReferenceResolver(null);

        result.CompilationOptions.Equals(sanitizedCompilationOptions).ShouldBeTrue();

        var sanitizedParseOptions = msbuildCompilation.SyntaxTrees.First().Options;

        result.ParseOptions.Equals(sanitizedParseOptions).ShouldBeTrue();

        var sanitizedSyntaxTrees = msbuildCompilation.SyntaxTrees
            .Where(tree => !new[] { ".AssemblyAttributes.cs", ".AssemblyInfo.cs" }.Any(ending =>
                Path.GetFileName(tree.FilePath).EndsWith(ending, StringComparison.OrdinalIgnoreCase)))
            .ToImmutableArray();

        // Assert that the generated files have the same names in the same order
        result.GeneratedSources.SequenceEqual(sanitizedSyntaxTrees, (a, b) =>
            Path.GetFileName(a.FilePath).Equals(Path.GetFileName(b.FilePath), StringComparison.OrdinalIgnoreCase));

        // Assert that the files have the same parse options and contents
        foreach (var (fastGenerated, msbuildGenerated) in result.GeneratedSources.Zip(sanitizedSyntaxTrees))
        {
            fastGenerated.Options.ShouldBe(msbuildGenerated.Options);
            fastGenerated.GetText().ToString().ShouldBe(msbuildGenerated.GetText().ToString());
        }
    }

    private static Compilation GetMSBuildWorkspaceCompilation(string csprojContents)
    {
        using var workspace = MSBuildWorkspace.Create(new Dictionary<string, string> { ["Configuration"] = "Release" });

        var tempFolder = Path.Join(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempFolder);
        try
        {
            var csprojPath = Path.Join(tempFolder, CsprojFileName);
            File.WriteAllText(csprojPath, csprojContents);
            var project = workspace.OpenProjectAsync(csprojPath).GetAwaiter().GetResult();
            return project.GetCompilationAsync().GetAwaiter().GetResult().ShouldNotBeNull();
        }
        finally
        {
            Directory.Delete(tempFolder, recursive: true);
        }
    }

    [Test]
    public static void Defaults()
    {
        var result = ParseCsproj("""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
              </PropertyGroup>

            </Project>
            """);

        result.CompilationOptions.OutputKind.ShouldBe(OutputKind.DynamicallyLinkedLibrary);
        result.CompilationOptions.NullableContextOptions.ShouldBe(NullableContextOptions.Disable);
        result.AssemblyName.ShouldBe(Path.GetFileNameWithoutExtension(CsprojFileName));
        result.CompilationOptions.AllowUnsafe.ShouldBeFalse();
        result.ParseOptions.LanguageVersion.ShouldBe(LanguageVersion.CSharp10); // Due to net6.0
        result.CompilationOptions.WarningLevel.ShouldBe(9); // Due to net9.0
        result.GeneratedSources.ShouldBeEmpty();
    }

    [Test]
    public static void ParsesTargetFramework()
    {
        ParseCsproj("""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
              </PropertyGroup>

            </Project>
            """).TargetFramework.ShouldBe("net9.0");
    }

    [Test]
    public static void ParsesOutputType([Values("Library", "Exe", "WinExe")] string outputType)
    {
        ParseCsproj($"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
                <OutputType>{outputType}</OutputType>
              </PropertyGroup>

            </Project>
            """).CompilationOptions.OutputKind.ShouldBe(outputType switch
        {
            "Library" => OutputKind.DynamicallyLinkedLibrary,
            "Exe" => OutputKind.ConsoleApplication,
            "WinExe" => OutputKind.WindowsApplication,
        });
    }

    [Test]
    public static void ParsesNullable([Values("enable", "disable", "annotations", "warnings")] string nullable)
    {
        ParseCsproj($"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
                <Nullable>{nullable}</Nullable>
              </PropertyGroup>

            </Project>
            """).CompilationOptions.NullableContextOptions.ShouldBe(nullable switch
        {
            "enable" => NullableContextOptions.Enable,
            "disable" => NullableContextOptions.Disable,
            "annotations" => NullableContextOptions.Annotations,
            "warnings" => NullableContextOptions.Warnings,
        });
    }

    [Test]
    public static void ParsesAssemblyName()
    {
        ParseCsproj($"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
                <AssemblyName>Xyz</AssemblyName>
              </PropertyGroup>

            </Project>
            """).AssemblyName.ShouldBe("Xyz");
    }

    [Test]
    public static void ParsesAllowUnsafeBlocks([Values("true", "false")] string allowUnsafeBlocks)
    {
        ParseCsproj($"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
                <AllowUnsafeBlocks>{allowUnsafeBlocks}</AllowUnsafeBlocks>
              </PropertyGroup>

            </Project>
            """).CompilationOptions.AllowUnsafe.ShouldBe(allowUnsafeBlocks switch
        {
            "true" => true,
            "false" => false,
        });
    }

    [Test]
    public static void ParsesImplicitUsings([Values("true", "enable", "false", "disable")] string implicitUsings)
    {
        var hasImplicitUsings = implicitUsings switch
        {
            "true" or "enable" => true,
            "false" or "disable" => false,
        };

        var result = ParseCsproj($"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
                <ImplicitUsings>{implicitUsings}</ImplicitUsings>
              </PropertyGroup>

            </Project>
            """);

        if (hasImplicitUsings)
        {
            var source = result.GeneratedSources.ShouldHaveSingleItem();
            source.Options.ShouldBe(result.ParseOptions);
            source.FilePath.ShouldBe("Test.GlobalUsings.g.cs");
            source.GetText().ToString().ShouldBe("""
                // <auto-generated/>
                global using System;
                global using System.Collections.Generic;
                global using System.IO;
                global using System.Linq;
                global using System.Net.Http;
                global using System.Threading;
                global using System.Threading.Tasks;

                """);
        }
        else
        {
            result.GeneratedSources.ShouldBeEmpty();
        }
    }

    [Test]
    public static void ThrowsNotImplementedExceptionForUnrecognizedProperty()
    {
        Should.Throw<NotImplementedException>(() => ParseCsproj("""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <Xyz></Xyz>
              </PropertyGroup>

            </Project>
            """));
    }

    [Test]
    public static void ThrowsNotImplementedExceptionForUnrecognizedItem()
    {
        Should.Throw<NotImplementedException>(() => ParseCsproj("""
            <Project Sdk="Microsoft.NET.Sdk">

              <ItemGroup>
                <Xyz Include="" />
              </ItemGroup>

            </Project>
            """));
    }

    [Test]
    public static void ThrowsNotImplementedExceptionForUnrecognizedTopLevelElement()
    {
        Should.Throw<NotImplementedException>(() => ParseCsproj("""
            <Project Sdk="Microsoft.NET.Sdk">

              <Xyz></Xyz>

            </Project>
            """));
    }
}
