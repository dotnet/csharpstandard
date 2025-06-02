using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;

namespace ExampleTester.Tests;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

public static class FastCsprojCompilationParserTests
{
    private static CsprojParseResult ParseCsproj(string csprojContents)
    {
        return FastCsprojCompilationParser.ParseCsproj(XDocument.Parse(csprojContents), "Test.csproj");
    }

    [Test]
    public static void Defaults()
    {
        var result = ParseCsproj("""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFramework>net6.0</TargetFramework>
              </PropertyGroup>

            </Project>
            """);

        result.CompilationOptions.OutputKind.ShouldBe(OutputKind.DynamicallyLinkedLibrary);
        result.CompilationOptions.NullableContextOptions.ShouldBe(NullableContextOptions.Disable);
        result.AssemblyName.ShouldBeNull();
        result.CompilationOptions.AllowUnsafe.ShouldBeFalse();
        result.ParseOptions.LanguageVersion.ShouldBe(LanguageVersion.CSharp10); // Due to net6.0
        result.CompilationOptions.WarningLevel.ShouldBe(6); // Due to net6.0
        result.GeneratedSources.ShouldBeEmpty();
    }

    [Test]
    public static void ParsesTargetFramework()
    {
        ParseCsproj("""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFramework>net6.0</TargetFramework>
              </PropertyGroup>

            </Project>
            """).TargetFramework.ShouldBe("net6.0");
    }

    [Test]
    public static void ParsesOutputType([Values("Library", "Exe", "WinExe")] string outputType)
    {
        ParseCsproj($"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFramework>net6.0</TargetFramework>
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
                <TargetFramework>net6.0</TargetFramework>
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
                <TargetFramework>net6.0</TargetFramework>
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
                <TargetFramework>net6.0</TargetFramework>
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
                <TargetFramework>net6.0</TargetFramework>
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
