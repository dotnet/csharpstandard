using Utilities;

namespace Utilities.Tests;

public class StatusCheckLoggerTests
{
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
        });
    }
}
