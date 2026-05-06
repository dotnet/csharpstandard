using System.CommandLine;
using System.CommandLine.Invocation;

namespace ExampleTester;

public record TesterConfiguration(
    string ExtractedOutputDirectory,
    bool Quiet = false,
    string? SourceFile = null,
    string? ExampleName = null);

public class TesterConfigurationBinder
{
    private readonly Argument<string> extractedOutputDirectory =
        new Argument<string>("extractedExampleDirectory") { Description = "The directory containing the extracted examples" };

    private readonly Option<bool> quiet =
        new Option<bool>("--quiet") { Description = "If set, only failures are displayed" };

    private readonly Option<string?> sourceFile =
        new Option<string?>("--source") { Description = "If set, only examples from the given source file are tested" };

    private readonly Option<string?> exampleName =
        new Option<string?>("--example") { Description = "If set, only the specified example is tested" };

    public void ConfigureCommand(Command command, Func<TesterConfiguration, Task<int>> action)
    {
        command.Add(extractedOutputDirectory);
        command.Add(quiet);
        command.Add(sourceFile);
        command.Add(exampleName);

        command.Action = new CustomAction(async parseResult =>
        {
            var directory = parseResult.GetValue(extractedOutputDirectory)!;
            var quietFlag = parseResult.GetValue(quiet);
            var source = parseResult.GetValue(sourceFile);
            var example = parseResult.GetValue(exampleName);
            var config = new TesterConfiguration(directory, quietFlag, source, example);
            return await action(config);
        });
    }

    private class CustomAction : AsynchronousCommandLineAction
    {
        private readonly Func<ParseResult, Task<int>> _action;

        public CustomAction(Func<ParseResult, Task<int>> action)
        {
            _action = action;
        }

        public override async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
        {
            return await _action(parseResult);
        }
    }
}
