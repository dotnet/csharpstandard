namespace MarkdownConverter.Converter
{
    internal readonly struct Needle
    {
        public int NeedleId { get; }
        public int Start { get; }
        public int Length { get; }

        public Needle(int needleId, int start, int length)
        {
            NeedleId = needleId;
            Start = start;
            Length = length;
        }
    }
}
