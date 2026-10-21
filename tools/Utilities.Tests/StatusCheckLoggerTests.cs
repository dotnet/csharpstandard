using Utilities;

namespace Utilities.Tests;

[NonParallelizable]
public class StatusCheckLoggerTests
{
    private string? originalGitHubActions;

    [SetUp]
    public void SetUp()
    {
        originalGitHubActions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS");
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", null);
    }

    [TearDown]
    public void TearDown() =>
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", originalGitHubActions);

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
    public void AllDiagnosticsAreCollectedAndEmittedBeyondTheLegacyAnnotationLimit()
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
    public void LegacyCheckRunPreservesLevelsLocationsMessagesAndFailure()
    {
        var logger = new StatusCheckLogger(new StringWriter(), ".", "Test tool");
        const string message = "Raw%,:\r\nmessage";

        logger.LogNotice(new("file.md", 1, 2, message, "N001"));
        logger.LogWarning(new("file.md", 3, 4, message, "W001"));
        logger.LogFailure(new("file.md", 5, 6, message, "E001"));

        var checkRun = logger.CreateCheckRun("head-sha");

        Assert.Multiple(() =>
        {
            Assert.That(checkRun.Conclusion?.Value, Is.EqualTo(Octokit.CheckConclusion.Failure));
            Assert.That(checkRun.Output.Annotations.Select(a => a.AnnotationLevel?.Value),
                Is.EqualTo(new[]
                {
                    Octokit.CheckAnnotationLevel.Notice,
                    Octokit.CheckAnnotationLevel.Warning,
                    Octokit.CheckAnnotationLevel.Failure
                }));
            Assert.That(checkRun.Output.Annotations.Select(a => a.Path),
                Is.EqualTo(new[] { "file.md", "file.md", "file.md" }));
            Assert.That(checkRun.Output.Annotations.Select(a => a.StartLine),
                Is.EqualTo(new[] { 1, 3, 5 }));
            Assert.That(checkRun.Output.Annotations.Select(a => a.EndLine),
                Is.EqualTo(new[] { 2, 4, 6 }));
            Assert.That(checkRun.Output.Annotations.Select(a => a.Message),
                Is.EqualTo(new[] { $"N001::{message}", $"W001::{message}", $"E001::{message}" }));
        });
    }

    [Test]
    public void CheckRunLimitsAnnotationsAndSummarizesOmittedDiagnostics()
    {
        var output = new StringWriter();
        var logger = new StatusCheckLogger(output, ".", "Test tool");

        for (int i = 1; i <= 55; i++)
        {
            logger.LogWarning(new StatusCheckMessage("file.md", i, i, $"Warning {i}", "TEST001"));
        }

        var checkRun = logger.CreateCheckRun("head-sha");

        Assert.Multiple(() =>
        {
            Assert.That(checkRun.Output.Annotations, Has.Count.EqualTo(StatusCheckLogger.MaximumAnnotations));
            Assert.That(checkRun.Output.Summary, Does.Contain("55 diagnostics"));
            Assert.That(checkRun.Output.Summary, Does.Contain("5 additional diagnostics"));
            Assert.That(output.ToString(), Does.Contain("Warning 55"));
            Assert.That(logger.Diagnostics, Has.Count.EqualTo(55));
        });
    }

    private static string[] WorkflowCommands(StringWriter output) =>
        output.ToString().Split(["\r\n", "\n", "\r"], StringSplitOptions.RemoveEmptyEntries)
            .Where(line => line.StartsWith("::", StringComparison.Ordinal)).ToArray();
}
