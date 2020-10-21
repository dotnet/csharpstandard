namespace MarkdownConverter.Grammar
{
    internal enum EbnfKind
    {
        // has exactly one child   e*  {e}
        ZeroOrMoreOf,
        // has exactly one child   e+  [e]
        OneOrMoreOf,
        // has exactly one child   e?  {e}-
        ZeroOrOneOf,
        // has 2+ children
        Sequence,
        // has 2+ children
        Choice,
        // has 0 children and an unescaped string without linebreaks which is not "<>", which either does not contain ' or does not contain "
        Terminal,
        // has 0 children and a string without linebreaks, which does not itself contain '?'
        ExtendedTerminal,
        // has 0 children and a string
        Reference
    }
}
