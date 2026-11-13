using ExampleTester;
using System.CommandLine;
using Utilities;

var logger = new StatusCheckLogger(Console.Out, "..", "Example tester");
var reportPath = Environment.GetEnvironmentVariable("DIAGNOSTICS_REPORT_PATH");
int exitCode = 1;

try
{
    var rootCommand = new RootCommand();
    new TesterConfigurationBinder().ConfigureCommand(rootCommand, ExecuteAsync);
    exitCode = rootCommand.Parse(args).Invoke();
    return exitCode;
}
finally
{
    if (reportPath is not null)
    {
        await logger.WriteReportAsync(reportPath, exitCode == 0);
    }
}

async Task<int> ExecuteAsync(TesterConfiguration configuration)
{
    var parentDirectory = configuration.ExtractedOutputDirectory;

    if (!Directory.Exists(parentDirectory))
    {
        Console.WriteLine($"Error: '{parentDirectory}' does not exist or is not a directory");
        return 1;
    }

    int failures = 0;
    var examples = GeneratedExample.LoadAllExamples(parentDirectory)
        .OrderBy(e => e.Metadata.MarkdownFile).ThenBy(e => e.Metadata.StartLine)
        .Where(e => configuration.SourceFile is not string sourceFile || e.Metadata.MarkdownFile == sourceFile)
        .Where(e => configuration.ExampleName is not string exampleName || e.Metadata.Name == exampleName)
        .ToList();

    if (examples.Count == 0)
    {
        Console.WriteLine("Error: no examples to run. Check source/example options.");
        return 1;
    }

    foreach (var example in examples)
    {
        // The Run method explains any failures, we just need to count them.
        if (!await example.Test(configuration, logger))
        {
            failures++;
        }
    }

    Console.WriteLine();
    Console.WriteLine($"Tests: {examples.Count}");
    Console.WriteLine($"Failures: {failures}");

    return failures;
}
