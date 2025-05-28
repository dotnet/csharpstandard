namespace StandardAnchorTags;

/// <summary>
/// Data stored for generating a link to a section.
/// </summary>
public readonly struct SectionLink
{
    private const char sectionReference = '§';

    /// <summary>
    /// Constructor for a section link.
    /// </summary>
    /// <param name="oldLink">The old link</param>
    /// <param name="newLink">The new link</param>
    /// <param name="anchor">The anchor text</param>
    public SectionLink(string oldLink, string newLink, string anchor)
    {
        ExistingLinkText = oldLink;
        NewLinkText = newLink;
        AnchorText = anchor;
    }

    /// <summary>
    /// The property is the link text currently used.
    /// </summary>
    /// <remarks>Might not be needed.</remarks>
    public string ExistingLinkText { get; }

    /// <summary>
    /// The text following the § character for any link in the updated standard.
    /// </summary>
    public string NewLinkText { get; }

    /// <summary>
    /// The text string for the destination file and anchor.
    /// </summary>
    public string AnchorText { get; }

    /// <summary>
    /// The markdown link for the section.
    /// </summary>
    public string FormattedMarkdownLink => $"[{sectionReference}{NewLinkText}]({AnchorText})";

    /// <summary>
    /// The markdown link and text for the TOC
    /// </summary>
    public string TOCMarkdownLink()
        => $"[{sectionReference}{NewLinkText}]({AnchorText})";
}
