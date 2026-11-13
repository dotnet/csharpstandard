using System.Diagnostics;
using System.Text.Json;

namespace ExampleTester.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class DiagnosticReportTests
{
    private string tempDirectory = null!;

    [SetUp]
    public void SetUp() =>
        tempDirectory = Directory.CreateTempSubdirectory("ExampleTesterReportTests-").FullName;

    [TearDown]
    public void TearDown() => Directory.Delete(tempDirectory, recursive: true);

    [TestCase("missing")]
    [TestCase("empty")]
    [TestCase("metadata")]
    public async Task EarlyFailuresAndExceptionsWriteReportsAndKeepNonzeroExitCodes(string failure)
    {
        var reportPath = Path.Combine(tempDirectory, "reports", "examples.json");
        var examples = Path.Combine(tempDirectory, "examples");
        if (failure != "missing")
        {
            Directory.CreateDirectory(examples);
        }
        if (failure == "metadata")
        {
            var example = Directory.CreateDirectory(Path.Combine(examples, "example"));
            await File.WriteAllTextAsync(Path.Combine(example.FullName, "metadata.json"), "Invalid JSON");
        }

        var result = await RunTesterAsync(reportPath, [examples]);
        using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));

        Assert.Multiple(() =>
        {
            Assert.That(result.ExitCode, Is.Not.Zero);
            Assert.That(report.RootElement.GetProperty("tool").GetString(), Is.EqualTo("Example tester"));
            Assert.That(report.RootElement.GetProperty("success").GetBoolean(), Is.False);
            Assert.That(report.RootElement.GetProperty("diagnostics").GetArrayLength(), Is.Zero);
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task SuccessfulExecutionNeedsNoAuthenticationAndReportingIsOptional(bool writeReport)
    {
        var reportPath = Path.Combine(tempDirectory, "reports", "examples.json");

        var result = await RunTesterAsync(writeReport ? reportPath : null, ["--help"]);

        Assert.That(result.ExitCode, Is.Zero, result.Error);
        if (writeReport)
        {
            using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));
            Assert.Multiple(() =>
            {
                Assert.That(report.RootElement.GetProperty("success").GetBoolean(), Is.True);
                Assert.That(report.RootElement.GetProperty("diagnostics").GetArrayLength(), Is.Zero);
            });
        }
        else
        {
            Assert.That(Directory.GetFileSystemEntries(tempDirectory), Is.Empty);
        }
    }

    [Test]
    public async Task ReportWritingErrorFailsOtherwiseSuccessfulInvocation()
    {
        var blockingFile = Path.Combine(tempDirectory, "not-a-directory");
        await File.WriteAllTextAsync(blockingFile, "File blocking report directory creation");

        var result = await RunTesterAsync(Path.Combine(blockingFile, "examples.json"), ["--help"]);

        Assert.Multiple(() =>
        {
            Assert.That(result.ExitCode, Is.Not.Zero);
            Assert.That(result.Error, Does.Contain("IOException"));
        });
    }

    [TestCase(false, 0)]
    [TestCase(true, 2)]
    public async Task ActualExamplesPreserveFailureCountsAndDiagnosticLocations(
        bool fail, int expectedExitCode)
    {
        var examples = Directory.CreateDirectory(Path.Combine(tempDirectory, "examples"));
        var reportPath = Path.Combine(tempDirectory, "reports", "examples.json");
        await CreateExampleAsync(examples.FullName, "first", fail);
        await CreateExampleAsync(examples.FullName, "second", fail);

        var result = await RunTesterAsync(reportPath, [examples.FullName]);
        using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));
        var diagnostics = report.RootElement.GetProperty("diagnostics").EnumerateArray().ToArray();
        var commands = result.Output.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Where(line => line.StartsWith("::error ", StringComparison.Ordinal)).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(result.ExitCode, Is.EqualTo(expectedExitCode), result.Error);
            Assert.That(result.Output, Does.Contain("Tests: 2"));
            Assert.That(result.Output, Does.Contain($"Failures: {expectedExitCode}"));
            Assert.That(report.RootElement.GetProperty("success").GetBoolean(), Is.EqualTo(!fail));
            Assert.That(diagnostics, Has.Length.EqualTo(expectedExitCode));
            Assert.That(commands, Has.Length.EqualTo(expectedExitCode));
            foreach (var diagnostic in diagnostics)
            {
                Assert.That(diagnostic.GetProperty("file").GetString(), Is.EqualTo("standard/expressions.md"));
                Assert.That(diagnostic.GetProperty("startLine").GetInt32(), Is.EqualTo(42));
                Assert.That(diagnostic.GetProperty("endLine").GetInt32(), Is.EqualTo(44));
                Assert.That(diagnostic.GetProperty("severity").GetString(), Is.EqualTo("error"));
                Assert.That(diagnostic.GetProperty("id").GetString(), Is.EqualTo("ExampleTester"));
            }
            foreach (var command in commands)
            {
                Assert.That(command, Does.StartWith(
                    "::error file=standard/expressions.md,line=42,endLine=44,title=Example tester - ExampleTester::"));
            }
        });
    }

    [TestCase("compiler", "Mismatched errors")]
    [TestCase("output", "Mismatched output")]
    [TestCase("exception", "Exception type InvalidOperationException was thrown unexpectedly")]
    public async Task ActualExampleFailuresUseSourceFilesForReportsAndAnnotations(
        string failure, string expectedMessage)
    {
        var examples = Directory.CreateDirectory(Path.Combine(tempDirectory, "examples"));
        var reportPath = Path.Combine(tempDirectory, "reports", "examples.json");
        var program = failure switch
        {
            "compiler" => "public class Example { public static void Main() { Missing(); } }",
            "output" => "public class Example { public static void Main() { System.Console.WriteLine(\"Actual output\"); } }",
            "exception" => "public class Example { public static void Main() { throw new System.InvalidOperationException(\"Example failure\"); } }",
            _ => throw new ArgumentException("Unknown failure scenario", nameof(failure))
        };
        await CreateExampleAsync(examples.FullName, "example", fail: true, program: program);

        var result = await RunTesterAsync(reportPath, [examples.FullName]);
        using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));
        var diagnostics = report.RootElement.GetProperty("diagnostics").EnumerateArray().ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(result.ExitCode, Is.EqualTo(1), result.Error);
            Assert.That(report.RootElement.GetProperty("success").GetBoolean(), Is.False);
            Assert.That(diagnostics.Select(d => d.GetProperty("message").GetString()),
                Has.Some.Contains(expectedMessage));
            Assert.That(diagnostics.Select(d => d.GetProperty("file").GetString()),
                Is.All.EqualTo("standard/expressions.md"));
            Assert.That(result.Output, Does.Contain(
                "::error file=standard/expressions.md,line=42,endLine=44,title=Example tester - ExampleTester::"));
        });
    }

    [Test]
    public async Task LaterExceptionPreservesPreviouslyCollectedDiagnostics()
    {
        var examples = Directory.CreateDirectory(Path.Combine(tempDirectory, "examples"));
        var reportPath = Path.Combine(tempDirectory, "reports", "examples.json");
        await CreateExampleAsync(examples.FullName, "first", fail: true);
        await CreateExampleAsync(examples.FullName, "second", fail: false, startLine: 70);
        File.Delete(Path.Combine(examples.FullName, "second", "Example.csproj"));

        var result = await RunTesterAsync(reportPath, [examples.FullName]);
        using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));
        var diagnostics = report.RootElement.GetProperty("diagnostics").EnumerateArray().ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(result.ExitCode, Is.Not.Zero);
            Assert.That(result.Error, Does.Contain("Sequence contains no elements"));
            Assert.That(report.RootElement.GetProperty("success").GetBoolean(), Is.False);
            Assert.That(diagnostics, Has.Length.EqualTo(1));
            Assert.That(diagnostics[0].GetProperty("file").GetString(),
                Is.EqualTo("standard/expressions.md"));
            Assert.That(diagnostics[0].GetProperty("message").GetString(),
                Does.Contain("Output expected, but project has no entry point."));
        });
    }

    private static async Task CreateExampleAsync(
        string examples, string name, bool fail, int startLine = 42, string? program = null)
    {
        var example = Directory.CreateDirectory(Path.Combine(examples, name));
        await File.WriteAllTextAsync(Path.Combine(example.FullName, "metadata.json"),
            JsonSerializer.Serialize(new
            {
                name,
                markdownFile = Path.Combine("..", "standard", "expressions.md"),
                startLine,
                endLine = startLine + 2,
                expectedOutput = fail ? new[] { "Expected output" } : null
            }));
        await File.WriteAllTextAsync(Path.Combine(example.FullName, "Example.csproj"),
            $"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net6.0</TargetFramework>
                <OutputType>{(program is null ? "Library" : "Exe")}</OutputType>
              </PropertyGroup>
            </Project>
            """);
        await File.WriteAllTextAsync(Path.Combine(example.FullName, "Example.cs"),
            program ?? "public class Example { }");
    }

    private async Task<(int ExitCode, string Output, string Error)> RunTesterAsync(string? reportPath, string[] args)
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = tempDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(typeof(GeneratedExample).Assembly.Location);
        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }
        startInfo.Environment.Remove("GH_TOKEN");
        startInfo.Environment.Remove("HEAD_SHA");
        startInfo.Environment.Remove("DIAGNOSTICS_REPORT_PATH");
        startInfo.Environment["GITHUB_ACTIONS"] = "true";
        if (reportPath is not null)
        {
            startInfo.Environment["DIAGNOSTICS_REPORT_PATH"] = reportPath;
        }

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start the example tester");
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        try
        {
            await process.WaitForExitAsync(timeout.Token);
            return (process.ExitCode, await output, await error);
        }
        finally
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync();
            }
        }
    }
}
