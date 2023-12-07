namespace MarkdownConverter.Spec;

// TODO: Now that there's a Span type in the .NET runtime, this should be renamed.
// I'll suggest TextSpan.
internal class Span
{
    public int Start { get; }
    public int Length { get; }
    public int End => Start + Length;

    public Span(int start, int length) =>
        (Start, Length) = (start, length);
}
