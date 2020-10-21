namespace MarkdownConverter.Spec
{
    internal class TermRef
    {
        private static int count = 1;

        public string Term { get; }
        public string BookmarkName { get; }
        public SourceLocation Loc { get; }

        public TermRef(string term, SourceLocation loc)
        {
            Term = term;
            Loc = loc;
            BookmarkName = $"_Trm{count:00000}";
            count++;
        }
    }
}
