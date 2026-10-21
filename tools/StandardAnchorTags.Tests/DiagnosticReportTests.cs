using System.Diagnostics;
using System.Text.Json;

namespace StandardAnchorTags.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class DiagnosticReportTests
{
    private string tempDirectory = null!;

    [SetUp]
    public void SetUp() =>
        tempDirectory = Directory.CreateTempSubdirectory("StandardAnchorTagsReportTests-").FullName;

    [TearDown]
    public void TearDown() => Directory.Delete(tempDirectory, recursive: true);

    [TestCase("missing", false)]
    [TestCase("missing", true)]
    [TestCase("null", false)]
    [TestCase("invalid", false)]
    public async Task ClauseLoadingFailuresWriteReportsWithoutRenumbering(
        string failure, bool legacyOptions)
    {
        var workingDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, "tools"));
        var reportPath = Path.Combine(tempDirectory, "reports", "anchors.json");
        if (failure != "missing")
        {
            var standard = Directory.CreateDirectory(Path.Combine(tempDirectory, "standard"));
            await File.WriteAllTextAsync(Path.Combine(standard.FullName, "clauses.json"),
                failure == "null" ? "null" : "Invalid JSON");
        }

        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = workingDirectory.FullName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(typeof(Program).Assembly.Location);
        if (legacyOptions)
        {
            foreach (var arg in new[] { "--owner", "dotnet", "--repo", "csharpstandard" })
            {
                startInfo.ArgumentList.Add(arg);
            }
        }
        startInfo.Environment.Remove("GH_TOKEN");
        startInfo.Environment.Remove("HEAD_SHA");
        startInfo.Environment["DIAGNOSTICS_REPORT_PATH"] = reportPath;

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start the anchor tool");
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        try
        {
            await process.WaitForExitAsync(timeout.Token);
            await output;
            var errorText = await error;
            using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));

            Assert.Multiple(() =>
            {
                Assert.That(process.ExitCode, Is.Not.Zero, errorText);
                Assert.That(report.RootElement.GetProperty("tool").GetString(),
                    Is.EqualTo("TOC and Anchor updater"));
                Assert.That(report.RootElement.GetProperty("success").GetBoolean(), Is.False);
                Assert.That(report.RootElement.GetProperty("diagnostics").GetArrayLength(), Is.Zero);
                Assert.That(Directory.GetFiles(tempDirectory, "*.md", SearchOption.AllDirectories),
                    Is.Empty);
            });
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
