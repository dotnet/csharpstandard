using FSharp.Markdown;
using System;
using System.Linq;

namespace MarkdownConverter.Spec
{
    internal class SectionRef
    {
        /// <summary>
        /// Section number, e.g. 10.1.2, or A.3 or null for sections without a number (e.g. Foreword).
        /// </summary>
        public string Number { get;  }

        /// <summary>
        /// Section title, e.g. "10.1.2 Goto Statement"
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Section title not including the number, e.g. "Goto Statement"
        /// </summary>
        public string TitleWithoutNumber { get; }

        /// <summary>
        /// 1-based level, e.g. 3
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// URL for the Markdown source, e.g. statements.md#goto-statement
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Name of generated bookmark, e.g. _Toc00023.
        /// </summary>
        public string BookmarkName { get; }

        /// <summary>
        /// Location in source Markdown.
        /// </summary>
        public SourceLocation Loc { get; }

        /// <summary>
        /// Counter used to generate bookmarks.
        /// </summary>
        private static int count = 1;

        public SectionRef(MarkdownParagraph.Heading mdh, string filename)
        {
            Level = mdh.size;
            var spans = mdh.body;
            if (spans.Length == 1 && spans.First().IsLiteral)
            {
                Title = MarkdownUtilities.UnescapeLiteral(spans.First() as MarkdownSpan.Literal).Trim();
                if (char.IsDigit(Title[0]) || (Title[0] >= 'A' && Title[0] <= 'D' && Title[1] == '.'))
                {
                    var titleParts = Title.Split(new[] { ' ' }, 2);
                    Number = titleParts[0];
                    TitleWithoutNumber = titleParts[1];
                }
                else
                {
                    Number = null;
                    TitleWithoutNumber = Title;
                }
            }
            else
            {
                throw new NotSupportedException($"Heading must be a single literal. Got: {mdh.body}");
            }
            foreach (var c in Title)
            {
                if (c >= 'a' && c <= 'z')
                {
                    Url += c;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    Url += char.ToLowerInvariant(c);
                }
                else if (c >= '0' && c <= '9')
                {
                    Url += c;
                }
                else if (c == '-' || c == '_')
                {
                    Url += c;
                }
                else if (c == ' ')
                {
                    Url += '-';
                }
            }
            Url = filename + "#" + Url;
            BookmarkName = $"_Toc{count:00000}"; count++;
            Loc = new SourceLocation(filename, this, mdh, null);
        }

        public override string ToString() => Url;
    }
}
