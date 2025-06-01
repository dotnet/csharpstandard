using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ExampleTester;

internal sealed record CsprojParseResult(
    string? AssemblyName,
    string TargetFramework,
    CSharpParseOptions ParseOptions,
    CSharpCompilationOptions CompilationOptions,
    ImmutableArray<SyntaxTree> GeneratedSources);
