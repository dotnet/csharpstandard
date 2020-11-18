using FSharp.Markdown;
using MarkdownConverter.Grammar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownConverter.Spec
{
    class MarkdownSpec
    {
        public List<SectionRef> Sections { get; } = new List<SectionRef>();
        public List<ProductionRef> Productions { get; } = new List<ProductionRef>();
        public IEnumerable<Tuple<string, MarkdownDocument>> Sources { get; }

        private MarkdownSpec(IEnumerable<Tuple<string, MarkdownDocument>> sources)
        {
            var grammar = new EbnfGrammar();
            Sources = sources;

            // (1) Add sections into the dictionary
            string url = "", title = "";

            // (2) Turn all the antlr code blocks into a grammar
            var sbantlr = new StringBuilder();

            foreach (var src in sources)
            {
                var reporter = new Reporter(src.Item1);
                var filename = Path.GetFileName(src.Item1);
                var md = src.Item2;

                foreach (var mdp in md.Paragraphs)
                {
                    reporter.CurrentParagraph = mdp;
                    reporter.CurrentSection = null;
                    if (mdp.IsHeading)
                    {
                        try
                        {
                            var sr = new SectionRef(mdp as MarkdownParagraph.Heading, filename);
                            if (Sections.Any(s => s.Url == sr.Url))
                            {
                                reporter.Error("MD02", $"Duplicate section title {sr.Url}");
                            }
                            else
                            {
                                Sections.Add(sr);
                                url = sr.Url;
                                title = sr.Title;
                                reporter.CurrentSection = sr;
                            }
                        }
                        catch (Exception ex)
                        {
                            reporter.Error("MD03", ex.Message); // constructor of SectionRef might throw
                        }
                    }
                    else if (mdp.IsCodeBlock)
                    {
                        var mdc = mdp as MarkdownParagraph.CodeBlock;
                        string code = mdc.code, lang = mdc.language;
                        if (lang != "antlr")
                        {
                            continue;
                        }

                        var g = Antlr.ReadString(code, "");
                        Productions.Add(new ProductionRef(code, g.Productions));
                        foreach (var p in g.Productions)
                        {
                            p.Link = url; p.LinkName = title;
                            if (p.Name != null && grammar.Productions.Any(dupe => dupe.Name == p.Name))
                            {
                                reporter.Warning("MD04", $"Duplicate grammar for {p.Name}");
                            }
                            grammar.Productions.Add(p);
                        }
                    }
                }
            }
        }

        public static MarkdownSpec ReadFiles(IEnumerable<string> files, List<Tuple<int, string, string, SourceLocation>> readme_headings)
        {
            // (0) Read all the markdown docs.
            // We do so in a parallel way, being careful not to block any threadpool threads on IO work;
            // only on CPU work.
            var tasks = new List<Task<Tuple<string, MarkdownDocument>>>();
            foreach (var fn in files)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using (var reader = File.OpenText(fn))
                    {
                        var text = await reader.ReadToEndAsync();
                        text = BugWorkaroundEncode(text);
                        return Tuple.Create(fn, Markdown.Parse(text));
                    }
                }));
            }
            var sources = Task.WhenAll(tasks).GetAwaiter().GetResult().OrderBy(tuple => GetSectionOrderingKey(tuple.Item2)).ToList();

            var md = new MarkdownSpec(sources);
            var md_headings = md.Sections.Where(s => s.Level <= 2).ToList();
            if (readme_headings != null && md_headings.Count > 0)
            {
                var readme_order = (from readme in readme_headings
                                    select new
                                    {
                                        orderInBody = md_headings.FindIndex(mdh => readme.Item1 == mdh.Level && readme.Item3 == mdh.Url),
                                        level = readme.Item1,
                                        title = readme.Item2,
                                        url = readme.Item3,
                                        loc = readme.Item4
                                    }).ToList();

                // The readme order should go "1,2,3,..." up to md_headings.Last()
                int expected = 0;
                foreach (var readme in readme_order)
                {
                    var reporter = new Reporter(readme.loc.File);
                    if (readme.orderInBody == -1)
                    {
                        var link = $"{new string(' ', readme.level * 2 - 2)}* [{readme.title}]({readme.url})";
                        reporter.Error("MD25", $"Remove: {link}", readme.loc);
                    }
                    else if (readme.orderInBody < expected)
                    {
                        continue; // error has already been reported
                    }
                    else if (readme.orderInBody == expected)
                    {
                        expected++; continue;
                    }
                    else if (readme.orderInBody > expected)
                    {
                        for (int missing = expected; missing < readme.orderInBody; missing++)
                        {
                            var s = md_headings[missing];
                            var link = $"{new string(' ', s.Level * 2 - 2)}* [{s.Title}]({s.Url})";
                            reporter.Error("MD24", $"Insert: {link}", readme.loc);
                        }
                        expected = readme.orderInBody + 1;
                    }
                }
            }

            return md;

            static int GetSectionOrderingKey(MarkdownDocument doc)
            {
                if (doc.Paragraphs.FirstOrDefault() is not MarkdownParagraph.Heading heading)
                {
                    throw new ArgumentException("Document does not start with a heading");
                }

                if (heading.body.SingleOrDefault() is not MarkdownSpan.Literal literal)
                {
                    throw new ArgumentException("Heading is not a literal");
                }

                string title = MarkdownUtilities.UnescapeLiteral(literal);
                return title switch
                {
                    "Foreword" => -10,
                    "Introduction" => -5,
                    string mainSection when char.IsDigit(mainSection[0]) => int.Parse(mainSection.Split(' ')[0]),
                    string annex when annex.StartsWith("Annex ") => 1000 + annex[6],
                    _ => throw new ArgumentException($"Unexpected section title: {title}")
                };
            }
        }

        private static string BugWorkaroundEncode(string src)
        {
            var lines = src.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // https://github.com/tpetricek/FSharp.formatting/issues/388
            // The markdown parser doesn't recognize | inside inlinecode inside table
            // To work around that, we'll encode this |, then decode it later
            for (int li = 0; li < lines.Length; li++)
            {
                if (!lines[li].StartsWith("|"))
                {
                    continue;
                }

                var codes = lines[li].Split('`');
                for (int ci = 1; ci < codes.Length; ci += 2)
                {
                    codes[ci] = codes[ci].Replace("|", "ceci_n'est_pas_une_pipe");
                }
                lines[li] = string.Join("`", codes);
            }

            // https://github.com/tpetricek/FSharp.formatting/issues/347
            // The markdown parser overly indents a level1 paragraph if it follows a level2 bullet
            // To work around that, we'll call out now, then unindent it later
            var state = 0; // 1=found.level1, 2=found.level2
            for (int li = 0; li < lines.Length - 1; li++)
            {
                if (lines[li].StartsWith("*  "))
                {
                    state = 1;
                    if (string.IsNullOrWhiteSpace(lines[li + 1]))
                    {
                        li++;
                    }
                }
                else if ((state == 1 || state == 2) && lines[li].StartsWith("   * "))
                {
                    state = 2;
                    if (string.IsNullOrWhiteSpace(lines[li + 1]))
                    {
                        li++;
                    }
                }
                else if (state == 2 && lines[li].StartsWith("      ") && lines[li].Length > 6 && lines[li][6] != ' ')
                {
                    state = 2;
                    if (string.IsNullOrWhiteSpace(lines[li + 1]))
                    {
                        li++;
                    }
                }
                else if (state == 2 && lines[li].StartsWith("   ") && lines[li].Length > 3 && lines[li][3] != ' ')
                {
                    lines[li] = "   ceci-n'est-pas-une-indent" + lines[li].Substring(3);
                    state = 0;
                }
                else
                {
                    state = 0;
                }
            }

            src = string.Join("\r\n", lines);

            // https://github.com/tpetricek/FSharp.formatting/issues/390
            // The markdown parser doesn't recognize bullet-chars inside codeblocks inside lists
            // To work around that, we'll prepend the line with stuff, and remove it later
            var codeblocks = src.Split(new[] { "\r\n    ```" }, StringSplitOptions.None);
            for (int cbi = 1; cbi < codeblocks.Length; cbi += 2)
            {
                var s = codeblocks[cbi];
                s = s.Replace("\r\n    *", "\r\n    ceci_n'est_pas_une_*");
                s = s.Replace("\r\n    +", "\r\n    ceci_n'est_pas_une_+");
                s = s.Replace("\r\n    -", "\r\n    ceci_n'est_pas_une_-");
                codeblocks[cbi] = s;
            }

            return string.Join("\r\n    ```", codeblocks);
        }
    }
}
