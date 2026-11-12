using System.Text.Json;
using Utilities;

namespace Utilities.Tests;

[NonParallelizable]
public class StatusCheckLoggerTests
{
    private string? originalGitHubActions;
    private string tempDirectory = null!;

    [SetUp]
    public void SetUp()
    {
        originalGitHubActions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS");
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", null);
        tempDirectory = Directory.CreateTempSubdirectory("StatusCheckLoggerTests-").FullName;
    }

    [TearDown]
    public void TearDown()
    {
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", originalGitHubActions);
        Directory.Delete(tempDirectory, recursive: true);
    }

    [Test]
    public void WorkflowCommandsMapSeveritiesAndPreserveConsoleOutput()
    {
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", "true");
        var output = new StringWriter();
        var logger = new StatusCheckLogger(output, ".", "Test tool");

        logger.LogNotice(new("file.md", 1, 2, "Notice", "N001"));
        logger.LogWarning(new("file.md", 3, 4, "Warning", "W001"));
        logger.LogFailure("file.md", 5, 6, "Failure", "E001");

        Assert.Multiple(() =>
        {
            Assert.That(output.ToString(), Is.EqualTo(string.Join(output.NewLine,
                "Test tool-N001::file=file.md,line=1::Notice",
                "::notice file=file.md,line=1,endLine=2,title=Test tool - N001::Notice",
                "⚠️Test tool-W001::file=file.md,line=3::Warning",
                "::warning file=file.md,line=3,endLine=4,title=Test tool - W001::Warning",
                "❌Test tool-E001::file=file.md,line=5::Failure",
                "::error file=file.md,line=5,endLine=6,title=Test tool - E001::Failure",
                "")));
            Assert.That(logger.Diagnostics.Select(d => d.Severity),
                Is.EqualTo(new[] { "notice", "warning", "error" }));
        });
    }

    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("false", false)]
    [TestCase("1", false)]
    [TestCase("true", true)]
    [TestCase("TRUE", true)]
    public void WorkflowCommandsRequireGitHubActions(string? value, bool expected)
    {
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", value);
        var output = new StringWriter();
        var logger = new StatusCheckLogger(output, ".", "Test tool");

        logger.LogNotice(new("file.md", 1, 1, "Notice", "N001"));
        logger.LogWarning(new("file.md", 2, 2, "Warning", "W001"));
        logger.LogFailure(new("file.md", 3, 3, "Failure", "E001"));

        Assert.Multiple(() =>
        {
            Assert.That(WorkflowCommands(output), Has.Length.EqualTo(expected ? 3 : 0));
            Assert.That(logger.Diagnostics, Has.Count.EqualTo(3));
            Assert.That(output.ToString(), Does.Contain("Test tool-N001::"));
            Assert.That(output.ToString(), Does.Contain("Test tool-W001::"));
            Assert.That(output.ToString(), Does.Contain("Test tool-E001::"));
        });
    }

    [Test]
    public void WorkflowCommandEscapesPropertiesAndMessageWithoutChangingDiagnostics()
    {
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", "true");
        var output = new StringWriter();
        const string tool = "Test%,:\r\ntool";
        const string id = "ID%,:\r\n001";
        const string message = "First%0A%,:\r\nSecond\nThird\rFourth";
        var logger = new StatusCheckLogger(output, ".", tool);

        logger.LogFailure(new("standard/file%,:\r\n.md", 42, 44, message, id));

        Assert.Multiple(() =>
        {
            Assert.That(WorkflowCommands(output), Is.EqualTo(new[]
            {
                "::error file=standard/file%25%2C%3A%0D%0A.md,line=42,endLine=44," +
                "title=Test%25%2C%3A%0D%0Atool - ID%25%2C%3A%0D%0A001::" +
                "First%250A%25,:%0D%0ASecond%0AThird%0DFourth"
            }));
            Assert.That(logger.Diagnostics.Single(), Is.EqualTo(
                new StatusCheckDiagnostic("error", "standard/file%,:\r\n.md",
                    42, 44, id, message)));
            Assert.That(output.ToString(), Does.Contain(message));
        });
    }

    [TestCase("standard/expressions.md")]
    [TestCase(@"standard\expressions.md")]
    public void RelativePathsUseForwardSlashes(string source)
    {
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", "true");
        var output = new StringWriter();
        var logger = new StatusCheckLogger(output, ".", "Test tool");

        logger.LogNotice(new(source, 1, 1, "Notice", "N001"));

        Assert.Multiple(() =>
        {
            Assert.That(logger.Diagnostics.Single().File,
                Is.EqualTo("standard/expressions.md"));
            Assert.That(WorkflowCommands(output).Single(),
                Does.StartWith("::notice file=standard/expressions.md,"));
        });
    }

    [Test]
    public void AbsolutePathsAreRelativeToTheSuppliedRepositoryRoot()
    {
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", "true");
        var output = new StringWriter();
        var root = Path.Combine(Path.GetTempPath(), "status-check-logger-tests");
        var logger = new StatusCheckLogger(output, root, "Test tool");

        logger.LogNotice(new(Path.Combine(root, "standard", "expressions.md"),
            42, 44, "Notice", "N001"));

        Assert.Multiple(() =>
        {
            Assert.That(logger.Diagnostics.Single().File,
                Is.EqualTo("standard/expressions.md"));
            Assert.That(WorkflowCommands(output).Single(), Is.EqualTo(
                "::notice file=standard/expressions.md,line=42,endLine=44," +
                "title=Test tool - N001::Notice"));
        });
    }

    [Test]
    public void RelativeSourcePathsAreResolvedFromTheWorkingDirectory()
    {
        var output = new StringWriter();
        var logger = new StatusCheckLogger(output, "..", "Test tool");

        logger.LogNotice(new(Path.Combine("..", "standard", "expressions.md"),
            1, 1, "Notice", "N001"));

        Assert.That(logger.Diagnostics.Single().File,
            Is.EqualTo("standard/expressions.md"));
    }

    [TestCase(0, 0, "")]
    [TestCase(0, 10, "")]
    [TestCase(-1, 10, "")]
    [TestCase(int.MinValue, int.MaxValue, "")]
    [TestCase(4, 0, ",line=4")]
    [TestCase(4, -1, ",line=4")]
    [TestCase(4, 3, ",line=4")]
    [TestCase(4, 4, ",line=4,endLine=4")]
    [TestCase(4, 6, ",line=4,endLine=6")]
    public void WorkflowCommandsOmitInvalidLineProperties(
        int startLine, int endLine, string lineProperties)
    {
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", "true");
        var output = new StringWriter();
        var logger = new StatusCheckLogger(output, ".", "Test tool");

        logger.LogWarning(new("file.md", startLine, endLine, "Warning", "W001"));

        Assert.Multiple(() =>
        {
            Assert.That(WorkflowCommands(output).Single(), Is.EqualTo(
                $"::warning file=file.md{lineProperties},title=Test tool - W001::Warning"));
            Assert.That(logger.Diagnostics.Single().StartLine, Is.EqualTo(startLine));
            Assert.That(logger.Diagnostics.Single().EndLine, Is.EqualTo(endLine));
        });
    }

    [TestCase(null)]
    [TestCase("true")]
    public void ConsoleOnlyLogsAreNotCollectedOrAnnotated(string? githubActions)
    {
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", githubActions);
        var output = new StringWriter();
        var logger = new StatusCheckLogger(output, ".", "Test tool");

        logger.ConsoleOnlyLog("file.md", 1, 2, "First message", "C001");
        logger.ConsoleOnlyLog("file.md", new("file.md", 3, 4, "Second message", "C002"));

        Assert.Multiple(() =>
        {
            Assert.That(output.ToString(), Is.EqualTo(
                $"Test tool-C001::file=file.md,line=1::First message{output.NewLine}" +
                $"Test tool-C002::file=file.md,line=3::Second message{output.NewLine}"));
            Assert.That(logger.Diagnostics, Is.Empty);
            Assert.That(WorkflowCommands(output), Is.Empty);
            Assert.That(logger.Success, Is.True);
        });
    }

    [Test]
    public void OnlyFailuresChangeSuccessAndLaterDiagnosticsDoNotResetIt()
    {
        var logger = new StatusCheckLogger(new StringWriter(), ".", "Test tool");

        Assert.That(logger.Success, Is.True);
        logger.LogNotice(new("file.md", 1, 1, "Notice", "N001"));
        logger.LogWarning(new("file.md", 1, 1, "Warning", "W001"));
        Assert.That(logger.Success, Is.True);

        logger.LogFailure(new("file.md", 1, 1, "Failure", "E001"));
        Assert.That(logger.Success, Is.False);
        logger.LogNotice(new("file.md", 1, 1, "Notice", "N002"));
        logger.LogWarning(new("file.md", 1, 1, "Warning", "W002"));
        Assert.That(logger.Success, Is.False);
    }

    [Test]
    public void ExitOnFailureRecordsAndAnnotatesFailureBeforeThrowing()
    {
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", "true");
        var output = new StringWriter();
        var logger = new StatusCheckLogger(output, ".", "Test tool");

        Assert.That(() => logger.ExitOnFailure(new("file.md", 1, 1, "Failure", "E001")),
            Throws.InvalidOperationException.With.Message.EqualTo("Failure"));

        Assert.Multiple(() =>
        {
            Assert.That(logger.Success, Is.False);
            Assert.That(logger.Diagnostics.Single().Severity, Is.EqualTo("error"));
            Assert.That(WorkflowCommands(output).Single(), Is.EqualTo(
                "::error file=file.md,line=1,endLine=1,title=Test tool - E001::Failure"));
        });
    }

    [Test]
    public void AllDiagnosticsAreCollectedAndEmittedBeyondAnnotationDisplayLimits()
    {
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", "true");
        var output = new StringWriter();
        var logger = new StatusCheckLogger(output, ".", "Test tool");

        for (int i = 1; i <= 55; i++)
        {
            logger.LogWarning(new("file.md", i, i, $"Warning {i}", "W001"));
        }

        Assert.Multiple(() =>
        {
            Assert.That(logger.Diagnostics, Has.Count.EqualTo(55));
            Assert.That(logger.Diagnostics.Last().Message, Is.EqualTo("Warning 55"));
            Assert.That(WorkflowCommands(output), Has.Length.EqualTo(55));
            Assert.That(WorkflowCommands(output).Last(), Does.EndWith("::Warning 55"));
        });
    }

    [Test]
    public async Task ReportCreatesParentDirectoriesAndPreservesRawDiagnostics()
    {
        var logger = new StatusCheckLogger(new StringWriter(), ".", "Test tool");
        const string message = "Raw%0A%,:\r\n\"message\" <&>";
        var reportPath = Path.Combine(tempDirectory, "nested", "reports", "diagnostics.json");

        logger.ConsoleOnlyLog("file.md", 1, 1, "Console only", "C001");
        logger.LogNotice(new(@"standard\file.md", 1, 2, message, "N001"));
        logger.LogWarning(new(@"standard\file.md", 3, 4, message, "W001"));
        logger.LogFailure(new(@"standard\file.md", 5, 6, message, "E001"));

        await logger.WriteReportAsync(reportPath);
        using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));
        var root = report.RootElement;
        var diagnostics = root.GetProperty("diagnostics").EnumerateArray().ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(root.EnumerateObject().Select(p => p.Name),
                Is.EquivalentTo(new[] { "tool", "success", "diagnostics" }));
            Assert.That(root.GetProperty("tool").GetString(), Is.EqualTo("Test tool"));
            Assert.That(root.GetProperty("success").GetBoolean(), Is.False);
            Assert.That(diagnostics.Select(d => d.GetProperty("severity").GetString()),
                Is.EqualTo(new[] { "notice", "warning", "error" }));
            Assert.That(diagnostics.Select(d => d.GetProperty("file").GetString()),
                Is.EqualTo(new[] { "standard/file.md", "standard/file.md", "standard/file.md" }));
            Assert.That(diagnostics.Select(d => d.GetProperty("startLine").GetInt32()),
                Is.EqualTo(new[] { 1, 3, 5 }));
            Assert.That(diagnostics.Select(d => d.GetProperty("endLine").GetInt32()),
                Is.EqualTo(new[] { 2, 4, 6 }));
            Assert.That(diagnostics.Select(d => d.GetProperty("id").GetString()),
                Is.EqualTo(new[] { "N001", "W001", "E001" }));
            Assert.That(diagnostics.Select(d => d.GetProperty("message").GetString()),
                Is.EqualTo(new[] { message, message, message }));
            Assert.That(diagnostics[0].EnumerateObject().Select(p => p.Name),
                Is.EquivalentTo(new[] { "severity", "file", "startLine", "endLine", "id", "message" }));
        });
    }

    [Test]
    public async Task ReportRetainsEveryDiagnosticBeyondAnnotationDisplayLimits()
    {
        var logger = new StatusCheckLogger(new StringWriter(), ".", "Test tool");
        var reportPath = Path.Combine(tempDirectory, "diagnostics.json");

        for (int i = 1; i <= 55; i++)
        {
            logger.LogWarning(new StatusCheckMessage("file.md", i, i, $"Warning {i}", "TEST001"));
        }

        await logger.WriteReportAsync(reportPath);
        using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));
        var diagnostics = report.RootElement.GetProperty("diagnostics");

        Assert.Multiple(() =>
        {
            Assert.That(diagnostics.GetArrayLength(), Is.EqualTo(55));
            Assert.That(diagnostics[54].GetProperty("message").GetString(), Is.EqualTo("Warning 55"));
            Assert.That(report.RootElement.GetProperty("success").GetBoolean(), Is.True);
        });
    }

    [TestCase(false, true, true)]
    [TestCase(false, false, false)]
    [TestCase(true, true, false)]
    [TestCase(true, false, false)]
    public async Task ReportSuccessRequiresSuccessfulCompletionAndNoFailures(
        bool logFailure, bool completedSuccessfully, bool expectedSuccess)
    {
        var logger = new StatusCheckLogger(new StringWriter(), ".", "Test tool");
        var reportPath = Path.Combine(tempDirectory, "diagnostics.json");
        if (logFailure)
        {
            logger.LogFailure("file.md", 0, -1, "Failure", "E001");
        }

        await logger.WriteReportAsync(reportPath, completedSuccessfully);
        using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));

        Assert.That(report.RootElement.GetProperty("success").GetBoolean(),
            Is.EqualTo(expectedSuccess));
    }

    [TestCase(0, -1)]
    [TestCase(4, 3)]
    public async Task ReportPreservesRawLineNumbers(int startLine, int endLine)
    {
        var logger = new StatusCheckLogger(new StringWriter(), ".", "Test tool");
        var reportPath = Path.Combine(tempDirectory, "diagnostics.json");
        logger.LogWarning(new("file.md", startLine, endLine, "Warning", "W001"));

        await logger.WriteReportAsync(reportPath);
        using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));
        var diagnostic = report.RootElement.GetProperty("diagnostics")[0];

        Assert.Multiple(() =>
        {
            Assert.That(diagnostic.GetProperty("startLine").GetInt32(), Is.EqualTo(startLine));
            Assert.That(diagnostic.GetProperty("endLine").GetInt32(), Is.EqualTo(endLine));
        });
    }

    [Test]
    public async Task ReportOverwritesPreviousRunAndSupportsEmptyDiagnostics()
    {
        var logger = new StatusCheckLogger(new StringWriter(), ".", "Test tool");
        var reportPath = Path.Combine(tempDirectory, "diagnostics.json");
        logger.LogFailure("file.md", 1, 1, "Old failure", "E001");
        await logger.WriteReportAsync(reportPath);

        var nextRun = new StatusCheckLogger(new StringWriter(), ".", "Next tool");
        await nextRun.WriteReportAsync(reportPath);
        using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));

        Assert.Multiple(() =>
        {
            Assert.That(report.RootElement.GetProperty("tool").GetString(), Is.EqualTo("Next tool"));
            Assert.That(report.RootElement.GetProperty("success").GetBoolean(), Is.True);
            Assert.That(report.RootElement.GetProperty("diagnostics").GetArrayLength(), Is.Zero);
        });
    }

    [Test]
    public void ReportWritingErrorsAreNotSuppressed()
    {
        var logger = new StatusCheckLogger(new StringWriter(), ".", "Test tool");
        var blockingFile = Path.Combine(tempDirectory, "not-a-directory");
        File.WriteAllText(blockingFile, "File blocking report directory creation");

        Assert.That(() => logger.WriteReportAsync(Path.Combine(blockingFile, "diagnostics.json")),
            Throws.TypeOf<IOException>());
    }

    private static string[] WorkflowCommands(StringWriter output) =>
        output.ToString().Split(["\r\n", "\n", "\r"], StringSplitOptions.RemoveEmptyEntries)
            .Where(line => line.StartsWith("::", StringComparison.Ordinal)).ToArray();
}
