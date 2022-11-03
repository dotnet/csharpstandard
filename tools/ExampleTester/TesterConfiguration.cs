using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;

namespace ExampleTester;

public record TesterConfiguration(
    string ExtractedOutputDirectory,
    bool Quiet,
    string? SourceFile,
    string? ExampleName)
{
    
}

public class TesterConfigurationBinder : BinderBase<TesterConfiguration>
{
    private static readonly Argument<string> extractedOutputDirectory =
        new Argument<string>("--extractedExampleDirectory", "The directory containing the extracted examples");

    private static readonly Option<bool> quiet =
        new Option<bool>("--quiet", "If set, only failures are displayed");

    private static readonly Option<string?> sourceFile =
        new Option<string?>("--source", "If set, only examples from the given source file are tested");

    private static readonly Option<string?> exampleName =
        new Option<string?>("--example", "If set, only the specified example is tested");

    public void ConfigureCommand(Command command, Func<TesterConfiguration, Task<int>> action)
    {
        command.Add(extractedOutputDirectory);
        command.Add(quiet);
        command.Add(sourceFile);
        command.Add(exampleName);
        command.SetHandler(action, this);
    }

    protected override TesterConfiguration GetBoundValue(BindingContext bindingContext) =>
        new TesterConfiguration(
            bindingContext.ParseResult.GetValueForArgument(extractedOutputDirectory),
            bindingContext.ParseResult.GetValueForOption(quiet),
            bindingContext.ParseResult.GetValueForOption(sourceFile),
            bindingContext.ParseResult.GetValueForOption(exampleName));
}
