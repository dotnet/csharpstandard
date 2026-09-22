using StandardAnchorTags;

namespace StandardAnchorTags.Tests;

public class ReferenceUpdateProcessorTests
{
    [TestCase("```csharp")]
    [TestCase("  ```")]
    [TestCase("> ```console")]
    [TestCase(">> ```")]
    public void RecognizesCodeFenceDelimiters(string line)
    {
        Assert.That(ReferenceUpdateProcessor.IsCodeFenceDelimiter(line), Is.True);
    }

    [TestCase("<!-- Maintenance note: use ```console for output. -->")]
    [TestCase("Text containing ``` inline.")]
    public void DoesNotTreatFenceMentionsAsDelimiters(string line)
    {
        Assert.That(ReferenceUpdateProcessor.IsCodeFenceDelimiter(line), Is.False);
    }

    [Test]
    public void RejectsSectionLinkWithoutClosingParenthesis()
    {
        const string line = "[§15.17](classes.md#1517-record-class-and-non-record-class-differences.";

        var result = ReferenceUpdateProcessor.ExpandToIncludeExistingLink(line, new Range(1, 7));

        Assert.That(result, Is.Null);
    }
}
