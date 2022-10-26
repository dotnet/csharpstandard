using ExampleExtractor;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace ExampleTester;

internal class GeneratedExample
{
    static GeneratedExample()
    {
        MSBuildLocator.RegisterDefaults();
    }

    private readonly string directory;
    internal ExampleMetadata Metadata { get; }

    private GeneratedExample(string directory)
    {
        this.directory = directory;
        string metadataJson = File.ReadAllText(Path.Combine(directory, ExampleMetadata.MetadataFile));
        Metadata = JsonConvert.DeserializeObject<ExampleMetadata>(metadataJson) ?? throw new ArgumentException($"Invalid (null) metadata in {directory}");
    }

    internal static List<GeneratedExample> LoadAllExamples(string parentDirectory) =>
        Directory.GetDirectories(parentDirectory).Select(Load).ToList();

    private static GeneratedExample Load(string directory)
    {
        return new GeneratedExample(directory);
    }

    internal async Task<bool> Test()
    {
        Console.WriteLine($"Testing {Metadata.Name} from {Metadata.Source}");

        using var workspace = MSBuildWorkspace.Create();
        // TODO: Validate this more cleanly.
        var projectFile = Directory.GetFiles(directory, "*.csproj").Single();
        var project = await workspace.OpenProjectAsync(projectFile);
        var compilation = await project.GetCompilationAsync();
        if (compilation is null)
        {
            throw new InvalidOperationException("Project has no Compilation");
        }

        bool ret = true;
        ret &= ValidateDiagnostics("errors", DiagnosticSeverity.Error, Metadata.ExpectedErrors);
        ret &= ValidateDiagnostics("warnings", DiagnosticSeverity.Warning, Metadata.ExpectedWarnings);
        ret &= ValidateOutput();

        return ret;

        bool ValidateDiagnostics(string type, DiagnosticSeverity severity, List<string> expected)
        {
            expected ??= new List<string>();
            var actual = compilation.GetDiagnostics().Where(d => d.Severity == severity).Select(d => d.Id).ToList();
            return ValidateExpectedAgainstActual(type, expected, actual);
        }

        bool ValidateOutput()
        {
            var entryPoint = compilation.GetEntryPoint(cancellationToken: default);
            if (entryPoint is null)
            {
                if (Metadata.ExpectedOutput != null)
                {
                    Console.WriteLine("  Output expected, but project has no entry point.");
                    return false;
                }
                return true;
            }

            string typeName = entryPoint.ContainingType.MetadataName;
            string methodName = entryPoint.MetadataName;

            var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);
            if (!emitResult.Success)
            {
                Console.WriteLine("  Failed to emit assembly");
                return false;
            }

            var generatedAssembly = Assembly.Load(ms.ToArray());
            // TODO: Check for null here and below
            var type = generatedAssembly.GetType(typeName)!;
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!;
            // TODO: Handle async entry points. (Is the entry point the synthesized one, or the user code?)
            var arguments = method.GetParameters().Any() ? new object[] { new string[0] } : new object[0];

            var oldOut = Console.Out;
            List<string> actualLines;
            Exception? actualException = null;
            try
            {
                var builder = new StringBuilder();
                Console.SetOut(new StringWriter(builder));
                try
                {
                    method.Invoke(null, arguments);
                }
                catch (TargetInvocationException outer)
                {
                    actualException = outer.InnerException ?? throw new InvalidOperationException("TargetInvocationException had no nested exception");
                }
                // Skip blank lines, to avoid unnecessary trailing empties.
                actualLines = builder.ToString().Replace("\r\n", "\n").Split('\n').Where(line => line != "").ToList();
            }
            finally
            {
                Console.SetOut(oldOut);
            }
            var expectedLines = Metadata.ExpectedOutput ?? new List<string>();
            return ValidateException(actualException, Metadata.ExpectedException) &&
                ValidateExpectedAgainstActual("output", expectedLines, actualLines);
        }

        bool ValidateException(Exception? actualException, string? expectedExceptionName)
        {
            return (actualException, expectedExceptionName) switch
            {
                (null, null) => true,
                (Exception ex, string name) =>
                    MaybeReportError(ex.GetType().Name == name, $"  Mismatched exception type: Expected {name}; Was {ex.GetType().Name}"),
                (null, string name) =>
                    MaybeReportError(false, $"  Expected exception type {name}; no exception was thrown"),
                (Exception ex, null) =>
                    MaybeReportError(false, $"  Exception type {ex.GetType().Name} was thrown unexpectedly; Message: {ex.Message}")
            };

            bool MaybeReportError(bool result, string message)
            {
                if (!result)
                {
                    Console.WriteLine(message);
                }
                return result;
            }
        }

        bool ValidateExpectedAgainstActual(string type, List<string> expected, List<string> actual)
        {
            if (!expected.SequenceEqual(actual))
            {
                Console.WriteLine($"  Mismatched {type}: Expected {string.Join(", ", expected)}; Was {string.Join(", ", actual)}");
                return false;
            }
            return true;
        }
    }
}
