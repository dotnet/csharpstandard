namespace MarkdownConverter.Spec
{
    internal class Span
    {
        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;

        public Span(int start, int length) =>
            (Start, Length) = (start, length);
    }
}
