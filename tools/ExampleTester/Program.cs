using ExampleTester;

if (args.Length != 1)
{
    Console.WriteLine("Arguments: <example-output-directory>");
    Console.WriteLine("(This directory is the one containing a subdirectory per example.)");
    return 1;
}

var parentDirectory = args[0];

if (!Directory.Exists(parentDirectory))
{
    Console.WriteLine($"Error: '{parentDirectory}' does not exist or is not a directory");
    return 1;
}

int failures = 0;
var allExamples = GeneratedExample.LoadAllExamples(parentDirectory)
    .OrderBy(e => e.Metadata.MarkdownFile).ThenBy(e => e.Metadata.StartLine)
    .ToList();
foreach (var example in allExamples)
{
    // The Run method explains any failures, we just need to count them.
    if (!await example.Test())
    {
        failures++;
    }
}

Console.WriteLine();
Console.WriteLine($"Tests: {allExamples.Count}");
Console.WriteLine($"Failures: {failures}");

return failures;
