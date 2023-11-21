using FSharp.Markdown;
using MarkdownConverter.Spec;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MarkdownConverter.Converter
{
    /// <summary>
    /// Maintains conversion context across multiple Markdown files.
    /// </summary>
    public sealed class ConversionContext
    {
        internal Dictionary<string, TermRef> Terms { get; } = new Dictionary<string, TermRef>();
        internal List<string> TermKeys { get; } = new List<string>();
        internal List<ItalicUse> Italics { get; } = new List<ItalicUse>();
        internal StrongBox<int> MaxBookmarkId { get; } = new StrongBox<int>();

        private readonly List<int> needleCounts = new List<int>(200);

        private int sectionRefCount = 0;

        internal SectionRef CreateSectionRef(MarkdownParagraph.Heading mdh, string filename)
        {
            string bookmarkName = $"_Toc{++sectionRefCount:00000}";
            return new SectionRef(mdh, filename, bookmarkName);
        }

        // TODO: Work out what this actually does. It's very confusing...
        internal IEnumerable<Needle> FindNeedles(IEnumerable<string> needles0, string haystack)
        {
            IList<string> needles = (needles0 as IList<string>) ?? new List<string>(needles0);
            for (int i = 0; i < Math.Min(needleCounts.Count, needles.Count); i++)
            {
                needleCounts[i] = 0;
            }

            while (needleCounts.Count < needles.Count)
            {
                needleCounts.Add(0);
            }

            var xcount = 0;
            for (int ic = 0; ic < haystack.Length; ic++)
            {
                var c = haystack[ic];
                xcount++;
                for (int i = 0; i < needles.Count; i++)
                {
                    if (needles[i][needleCounts[i]] == c)
                    {
                        needleCounts[i]++;
                        if (needleCounts[i] == needles[i].Length)
                        {
                            if (xcount > needleCounts[i])
                            {
                                yield return new Needle(-1, ic + 1 - xcount, xcount - needleCounts[i]);
                            }
                            yield return new Needle(i, ic + 1 - needleCounts[i], needleCounts[i]);
                            xcount = 0;
                            for (int j = 0; j < needles.Count; j++)
                            {
                                needleCounts[j] = 0;
                            }
                            break;
                        }
                    }
                    else
                    {
                        needleCounts[i] = 0;
                    }
                }
            }
            if (xcount > 0)
            {
                yield return new Needle(-1, haystack.Length - xcount, xcount);
            }
        }
    }
}
