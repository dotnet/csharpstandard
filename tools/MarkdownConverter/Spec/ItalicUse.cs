namespace MarkdownConverter.Spec
{
    internal sealed class ItalicUse
    {
        public string Literal { get; }
        public ItalicUseKind Kind { get; }
        public SourceLocation Loc { get; }
        public enum ItalicUseKind { Production, Italic, Term };

        public ItalicUse(string literal, ItalicUseKind kind, SourceLocation loc)
        {
            Literal = literal;
            Kind = kind;
            Loc = loc;
        }
    }
}
