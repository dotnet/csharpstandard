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
}
