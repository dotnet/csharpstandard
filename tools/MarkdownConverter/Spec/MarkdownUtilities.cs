using FSharp.Markdown;

namespace MarkdownConverter.Spec
{
    internal static class MarkdownUtilities
    {
        internal static string UnescapeLiteral(MarkdownSpan.Literal literal) =>
            literal.text
                .Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&reg;", "®")
                .Replace("\\<", "<")
                .Replace("\\>", ">");
    }
}
