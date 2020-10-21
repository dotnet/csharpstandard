using MarkdownConverter.Grammar;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownConverter.Spec
{
    internal class ProductionRef
    {
        /// <summary>
        /// The complete antlr code block in which it's found
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// The names of all the productions. (TODO: Work out what this means...)
        /// </summary>
        public List<string> Names { get; }

        /// <summary>
        /// The bookmark name for this production reference, e.g. _Grm00023.
        /// </summary>
        public string BookmarkName { get; }

        /// <summary>
        /// Counter for bookmark generation.
        /// </summary>
        private static int count = 1;

        public ProductionRef(string code, IEnumerable<Production> productions)
        {
            Code = code;
            Names = productions.Select(p => p.Name).Where(name => name != null).ToList();
            BookmarkName = $"_Grm{count:00000}";
            count++;
        }
    }
}
