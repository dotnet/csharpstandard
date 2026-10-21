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

    private async Task<(int ExitCode, string Error)> RunTesterAsync(string? reportPath, string[] args)
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
            await output;
            return (process.ExitCode, await error);
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
