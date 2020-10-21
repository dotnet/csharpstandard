using System.Collections.Generic;

namespace MarkdownConverter.Grammar
{
    internal sealed class EbnfGrammar
    {
        public string Name { get; set; }
        public List<Production> Productions { get; set; } = new List<Production>();
    }
}
