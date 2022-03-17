using FSharp.Markdown;
using MarkdownConverter.Converter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownConverter.Spec
{
    public class MarkdownSpec
    {
        public const string NoteAndExampleFakeSeparator = "REMOVE-ME-INSERTED-TO-SEPARATE_NOTES-AND-EXAMPLES";

        public List<SectionRef> Sections { get; } = new List<SectionRef>();
        public IEnumerable<Tuple<string, MarkdownDocument>> Sources { get; }

        /// <summary>
        /// The conversion context, used to keep track of sections etc.
        /// </summary>
        public ConversionContext Context { get; }

        private MarkdownSpec(IEnumerable<Tuple<string, MarkdownDocument>> sources, Reporter reporter)
        {
            Context = new ConversionContext();
            Sources = sources;

            // (1) Add sections into the dictionary
            string url = "", title = "";

            // (2) Turn all the antlr code blocks into a grammar
            var sbantlr = new StringBuilder();

            foreach (var src in sources)
            {
                var fileReporter = reporter.WithFileName(src.Item1);
                var filename = Path.GetFileName(src.Item1);
                var md = src.Item2;

                foreach (var mdp in md.Paragraphs)
                {
                    fileReporter.CurrentParagraph = mdp;
                    fileReporter.CurrentSection = null;
                    if (mdp.IsHeading)
                    {
                        try
                        {
                            var sr = Context.CreateSectionRef(mdp as MarkdownParagraph.Heading, filename);
                            if (Sections.Any(s => s.Url == sr.Url))
                            {
                                fileReporter.Error("MD02", $"Duplicate section title {sr.Url}");
                            }
                            else
                            {
                                Sections.Add(sr);
                                url = sr.Url;
                                title = sr.Title;
                                fileReporter.CurrentSection = sr;
                            }
                        }
                        catch (Exception ex)
                        {
                            fileReporter.Error("MD03", ex.Message); // constructor of SectionRef might throw
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
                    }
                }
            }
        }

        public static MarkdownSpec ReadFiles(IEnumerable<string> files, Reporter reporter, Func<string, TextReader> readerProvider = null)
        {
            if (files is null) throw new ArgumentNullException(nameof(files));

            readerProvider ??= File.OpenText;

            // (0) Read all the markdown docs.
            // We do so in a parallel way, being careful not to block any threadpool threads on IO work;
            // only on CPU work.
            var tasks = new List<Task<Tuple<string, MarkdownDocument>>>();
            foreach (var fn in files)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using (var reader = readerProvider(fn))
                    {
                        var text = await reader.ReadToEndAsync();
                        text = BugWorkaroundEncode(text);
                        text = RemoveBlockComments(text, Path.GetFileName(fn));
                        text = SeparateNotesAndExamples(text);
                        return Tuple.Create(fn, Markdown.Parse(text));
                    }
                }));
            }
            var sources = Task.WhenAll(tasks).GetAwaiter().GetResult().OrderBy(tuple => GetSectionOrderingKey(tuple.Item2)).ToList();
            return new MarkdownSpec(sources, reporter);

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

        /// <summary>
        /// Removes HTML comments *only* when the "start comment" is on a line on its own,
        /// and the "end comment" is also on a line on its own. (We use comments for some other
        /// fiddly parts, such as table conversions.)
        /// </summary>
        /// <param name="text">Markdown text</param>
        /// <returns>The text without the comments</returns>
        private static string RemoveBlockComments(string text, string file)
        {
            // This is probably doable with a multi-line regex, but it's probably simpler not to...
            // Note that we assume CLRF line endings as that's what BugWorkaroundEncode returns.
            while (true)
            {
                int startIndex = text.IndexOf("\r\n<!--\r\n");
                int endIndex = text.IndexOf("\r\n-->\r\n");
                if (endIndex < startIndex)
                {
                    throw new InvalidOperationException($"End comment before start comment in {file}");
                }
                if (startIndex == -1)
                {
                    if (endIndex != -1)
                    {
                        throw new InvalidOperationException($"End comment with no start comment in {file}");
                    }
                    return text;
                }
                // Remove everything from the start of the match (CRLF) to the end of CLRF--> but not including the *trailing* CRLF.
                text = text.Remove(startIndex, endIndex - startIndex + 5);
            }
        }

        /// <summary>
        /// When we have a paragraph ending with "*end note*" or "*end example*", and the next paragraph starts
        /// with "> *" (for either a note or an example), insert fake text to cause the Markdown parser to treat
        /// them as separate spans. We remove this later on when converting Markdown to Word. Note that this
        /// only works at the top level at the moment.
        /// </summary>
        private static string SeparateNotesAndExamples(string text) => text
            .Replace("*end note*\r\n\r\n> *", $"*end note*\r\n\r\n{NoteAndExampleFakeSeparator}\r\n\r\n> *")
            .Replace("*end example*\r\n\r\n> *", $"*end example*\r\n\r\n{NoteAndExampleFakeSeparator}\r\n\r\n> *");
    }
}
