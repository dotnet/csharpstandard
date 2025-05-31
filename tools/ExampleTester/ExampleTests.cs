using NUnit.Framework;
using Utilities;

[assembly: Parallelizable(ParallelScope.Children)]

namespace ExampleTester;

public static class ExampleTests
{
    private static TesterConfiguration TesterConfiguration { get; } = new(FindTmpDirectory());

    public static IEnumerable<object[]> LoadExamples() =>
        from example in GeneratedExample.LoadAllExamples(TesterConfiguration.ExtractedOutputDirectory)
        select new object[] { example };

    [TestCaseSource(nameof(LoadExamples))]
    public static async Task ExamplePasses(GeneratedExample example)
    {
        var logger = new StatusCheckLogger(TestContext.Out, "..", "Example tester");

        if (!await example.Test(TesterConfiguration, logger))
            Assert.Fail("There were one or more failures. See the logged output for details.");
    }

    private static string FindTmpDirectory()
    {
        for (string? current = AppDomain.CurrentDomain.BaseDirectory; current != null; current = Path.GetDirectoryName(current))
        {
            string testPath = Path.Join(current, "tmp");
            if (Directory.Exists(testPath))
                return testPath;
        }

        throw new InvalidOperationException($"Can't find 'tmp' directory in {AppDomain.CurrentDomain.BaseDirectory} or any parent directories.");
    }
}
