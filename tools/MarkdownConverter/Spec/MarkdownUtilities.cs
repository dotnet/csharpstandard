using FSharp.Markdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownConverter.Spec
{
    internal static class MarkdownUtilities
    {
        internal static string UnescapeLiteral(MarkdownSpan.Literal literal) =>
            literal.text.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&reg;", "®");
    }
}
