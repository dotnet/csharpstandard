using CSharp2Colorized;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FSharp.Formatting.Common;
using FSharp.Markdown;
using MarkdownConverter.Spec;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MarkdownConverter.Converter
{
    public class MarkdownSourceConverter
    {
        /// <summary>
        /// The maximum code line length that's allowed without generating a warning.
        /// </summary>
        public const int MaximumCodeLineLength = 80;

        private const int InitialIndentation = 540;
        private const int ListLevelIndentation = 360;
        private const int TableIndentation = 360;

        private static readonly Dictionary<char, char> SubscriptUnicodeToAscii = new Dictionary<char, char>
        {
            { '\u1d62', 'i' },
            { '\u1d65', 'v' },
            { '\u2080', '0' },
            { '\u2081', '1' },
            { '\u2082', '2' },
            { '\u2083', '3' },
            { '\u2084', '4' },
            { '\u2085', '5' },
            { '\u2086', '6' },
            { '\u2087', '7' },
            { '\u2088', '8' },
            { '\u2089', '9' },
            { '\u208a', '+' },
            { '\u208b', '-' },
            { '\u2091', 'e' },
            { '\u2093', 'x' },
        };

        private static readonly Dictionary<char, char> SuperscriptUnicodeToAscii = new Dictionary<char, char>
        {
            { '\u00aa', 'a' },
            { '\u207f', 'n' },
            { '\u00b9', '1' },
        };

        private readonly MarkdownDocument markdownDocument;
        private readonly WordprocessingDocument wordDocument;
        private readonly Dictionary<string, SectionRef> sections;
        private readonly ConversionContext context;
        private readonly string filename;
        private readonly Reporter reporter;
        
        public IReadOnlyList<OpenXmlCompositeElement> Paragraphs { get; }

        public MarkdownSourceConverter(
            MarkdownDocument markdownDocument,
            WordprocessingDocument wordDocument,
            MarkdownSpec spec,
            string filename,
            Reporter reporter)
        {
            this.markdownDocument = markdownDocument;
            this.wordDocument = wordDocument;
            sections = spec.Sections.ToDictionary(sr => sr.Url);
            this.filename = filename;
            this.reporter = reporter;
            context = spec.Context;
            Paragraphs = Paragraphs2Paragraphs(markdownDocument.Paragraphs).ToList();
        }

        IEnumerable<OpenXmlCompositeElement> Paragraphs2Paragraphs(IEnumerable<MarkdownParagraph> pars) =>
            pars.SelectMany(md => Paragraph2Paragraphs(md));

        IEnumerable<OpenXmlCompositeElement> Paragraph2Paragraphs(MarkdownParagraph md)
        {
            reporter.CurrentParagraph = md;
            if (md.IsHeading)
            {
                var mdh = md as MarkdownParagraph.Heading;
                var level = mdh.size;
                var spans = mdh.body;
                var sr = sections[context.CreateSectionRef(mdh, filename).Url];
                reporter.CurrentSection = sr;
                var properties = new List<OpenXmlElement>
                {
                    new ParagraphStyleId { Val = $"Heading{level}" }
                };
                if (sr.Number is null)
                {
                    properties.Add(new NumberingProperties(new NumberingLevelReference { Val = 0 }, new NumberingId { Val = 0 }));
                }
                var props = new ParagraphProperties(properties);
                var p = new Paragraph { ParagraphProperties = props };
                context.MaxBookmarkId.Value += 1;
                p.AppendChild(new BookmarkStart { Name = sr.BookmarkName, Id = context.MaxBookmarkId.Value.ToString() });
                p.Append(Span2Elements(MarkdownSpan.NewLiteral(sr.TitleWithoutNumber, FSharpOption<MarkdownRange>.None)));
                p.AppendChild(new BookmarkEnd { Id = context.MaxBookmarkId.Value.ToString() });
                yield return p;

                var i = sr.Url.IndexOf("#");
                string currentSection = $"{sr.Url.Substring(0, i)} {new string('#', level)} {sr.Title} [{sr.Number}]";
                reporter.Log(currentSection);
                yield break;
            }

            else if (md.IsParagraph)
            {
                var mdp = md as MarkdownParagraph.Paragraph;
                var spans = mdp.body;
                yield return new Paragraph(Spans2Elements(spans));
                yield break;
            }

            else if (md.IsQuotedBlock)
            {
                // Keep track of which list numbering schemes we've already indented.
                // Lists are flattened into multiple paragraphs, but all paragraphs within one list
                // keep the same numbering scheme, and we only want to increase the indentation level once.
                var indentedLists = new HashSet<int>();

                var mdq = md as MarkdownParagraph.QuotedBlock;
                // TODO: Actually make this a block quote.
                // We're now indenting, which is a start... a proper block would be nicer though.
                foreach (var element in mdq.paragraphs.SelectMany(Paragraph2Paragraphs))
                {
                    if (element is Paragraph paragraph)
                    {
                        paragraph.ParagraphProperties ??= new ParagraphProperties();

                        // Indentation in lists is controlled by numbering properties.
                        // Each list creates its own numbering, with a set of properties for each numbering level.
                        // If there's a list within a note, we need to increase the indentation of each numbering level.
                        if (paragraph.ParagraphProperties.NumberingProperties?.NumberingId?.Val?.Value is int numberingId)
                        {
                            if (indentedLists.Add(numberingId))
                            {
                                var numbering = wordDocument.MainDocumentPart.NumberingDefinitionsPart.Numbering.OfType<NumberingInstance>().First(ni => ni.NumberID.Value == numberingId);
                                var abstractNumberingId = numbering.AbstractNumId.Val;
                                var abstractNumbering = wordDocument.MainDocumentPart.NumberingDefinitionsPart.Numbering.OfType<AbstractNum>().FirstOrDefault(ani => ani.AbstractNumberId.Value == abstractNumberingId);
                                foreach (var level in abstractNumbering.OfType<Level>())
                                {
                                    var paragraphProperties = level.GetFirstChild<ParagraphProperties>();
                                    int indentation = int.Parse(paragraphProperties.Indentation.Left.Value);
                                    paragraphProperties.Indentation.Left.Value = (indentation + InitialIndentation).ToString();
                                }
                            }
                        }
                        else
                        {
                            paragraph.ParagraphProperties.Indentation = new Indentation { Left = InitialIndentation.ToString() };
                        }
                        yield return paragraph;
                    }
                    else if (element is Table table)
                    {
                        if (table.ElementAt(0) is TableProperties tableProperties)
                        {
                            tableProperties.TableIndentation ??= new TableIndentation();
                            // TODO: This will be incorrect if we ever have a table in a list in a note.
                            // Let's just try not to do that.
                            tableProperties.TableIndentation.Width = InitialIndentation;
                            yield return table;
                        }
                        else
                        {
                            reporter.Error("MD31", $"Table in quoted block does not start with table properties");
                        }
                    }
                    else
                    {
                        reporter.Error("MD30", $"Unhandled element type in quoted block: {element.GetType()}");
                    }
                }
                yield break;
            }

            else if (md is MarkdownParagraph.ListBlock mdl)
            {
                mdl = MaybeRewriteListBlock(mdl);
                var flat = FlattenList(mdl);

                // Let's figure out what kind of list it is - ordered or unordered? nested?
                var format0 = new[] { "1", "1", "1", "1" };
                foreach (var item in flat)
                {
                    format0[item.Level] = (item.IsBulletOrdered ? "1" : "o");
                }

                var format = string.Join("", format0);

                var numberingPart = wordDocument.MainDocumentPart.NumberingDefinitionsPart ?? wordDocument.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>("NumberingDefinitionsPart001");
                if (numberingPart.Numbering == null)
                {
                    numberingPart.Numbering = new Numbering();
                }

                Func<int, bool, Level> createLevel = (level, isOrdered) =>
                {
                    var numformat = NumberFormatValues.Bullet;
                    var levelText = new[] { "·", "o", "·", "o" }[level];
                    if (isOrdered && level == 0) { numformat = NumberFormatValues.Decimal; levelText = "%1."; }
                    if (isOrdered && level == 1) { numformat = NumberFormatValues.LowerLetter; levelText = "%2."; }
                    if (isOrdered && level == 2) { numformat = NumberFormatValues.LowerRoman; levelText = "%3."; }
                    if (isOrdered && level == 3) { numformat = NumberFormatValues.LowerRoman; levelText = "%4."; }
                    var r = new Level { LevelIndex = level };
                    r.Append(new StartNumberingValue { Val = 1 });
                    r.Append(new NumberingFormat { Val = numformat });
                    r.Append(new LevelText { Val = levelText });
                    r.Append(new ParagraphProperties(new Indentation { Left = (InitialIndentation + ListLevelIndentation * level).ToString(), Hanging = ListLevelIndentation.ToString() }));
                    if (levelText == "·")
                    {
                        r.Append(new NumberingSymbolRunProperties(new RunFonts { Hint = FontTypeHintValues.Default, Ascii = "Symbol", HighAnsi = "Symbol", EastAsia = "Times new Roman", ComplexScript = "Times new Roman" }));
                    }

                    if (levelText == "o")
                    {
                        r.Append(new NumberingSymbolRunProperties(new RunFonts { Hint = FontTypeHintValues.Default, Ascii = "Courier New", HighAnsi = "Courier New", ComplexScript = "Courier New" }));
                    }

                    return r;
                };
                var level0 = createLevel(0, format[0] == '1');
                var level1 = createLevel(1, format[1] == '1');
                var level2 = createLevel(2, format[2] == '1');
                var level3 = createLevel(3, format[3] == '1');

                var abstracts = numberingPart.Numbering.OfType<AbstractNum>().Select(an => an.AbstractNumberId.Value).ToList();
                var aid = (abstracts.Count == 0 ? 1 : abstracts.Max() + 1);
                var aabstract = new AbstractNum(new MultiLevelType() { Val = MultiLevelValues.Multilevel }, level0, level1, level2, level3) { AbstractNumberId = aid };
                numberingPart.Numbering.InsertAt(aabstract, 0);

                var instances = numberingPart.Numbering.OfType<NumberingInstance>().Select(ni => ni.NumberID.Value);
                var nid = (instances.Count() == 0 ? 1 : instances.Max() + 1);
                var numInstance = new NumberingInstance(new AbstractNumId { Val = aid }) { NumberID = nid };
                numberingPart.Numbering.AppendChild(numInstance);

                // We'll also figure out the indentation(for the benefit of those paragraphs that should be
                // indented with the list but aren't numbered). The indentation is generated by the createLevel delegate.
                Func<int, string> calcIndent = level => (InitialIndentation + level * ListLevelIndentation).ToString();

                foreach (var item in flat)
                {
                    var content = item.Paragraph;
                    if (content.IsParagraph || content.IsSpan)
                    {
                        var spans = (content.IsParagraph ? (content as MarkdownParagraph.Paragraph).body : (content as MarkdownParagraph.Span).body);
                        if (item.HasBullet)
                        {
                            yield return new Paragraph(Spans2Elements(spans, inList: true)) { ParagraphProperties = new ParagraphProperties(new NumberingProperties(new ParagraphStyleId { Val = "ListParagraph" }, new NumberingLevelReference { Val = item.Level }, new NumberingId { Val = nid })) };
                        }
                        else
                        {
                            yield return new Paragraph(Spans2Elements(spans, inList: true)) { ParagraphProperties = new ParagraphProperties(new Indentation { Left = calcIndent(item.Level) }) };
                        }
                    }
                    else if (content.IsQuotedBlock || content.IsCodeBlock)
                    {
                        foreach (var p in Paragraph2Paragraphs(content))
                        {
                            var props = p.GetFirstChild<ParagraphProperties>();
                            if (props == null)
                            {
                                props = new ParagraphProperties();
                                p.InsertAt(props, 0);
                            }
                            var indent = props?.GetFirstChild<Indentation>();
                            if (indent == null)
                            {
                                indent = new Indentation();
                                props.Append(indent);
                            }
                            indent.Left = calcIndent(item.Level);
                            yield return p;
                        }
                    }
                    else if (content.IsTableBlock)
                    {
                        foreach (var p in Paragraph2Paragraphs(content))
                        {
                            var table = p as Table;
                            if (table == null)
                            {
                                yield return p;
                                continue;
                            }
                            var tprops = table.GetFirstChild<TableProperties>();
                            var tindent = tprops?.GetFirstChild<TableIndentation>();
                            if (tindent == null)
                            {
                                throw new Exception("Ooops! Table is missing indentation");
                            }

                            tindent.Width = int.Parse(calcIndent(item.Level));
                            yield return table;
                        }
                    }
                    else if (content is MarkdownParagraph.InlineBlock inlineBlock && GetCustomBlockId(inlineBlock) is string customBlockId)
                    {
                        foreach (var element in GenerateCustomBlockElements(customBlockId, inlineBlock))
                        {
                            yield return element;
                        }
                    }
                    else
                    {
                        reporter.Error("MD08", $"Unexpected item in list '{content.GetType().Name}'");
                    }
                }
            }

            else if (md.IsCodeBlock)
            {
                var mdc = md as MarkdownParagraph.CodeBlock;
                var code = mdc.code;
                var lang = mdc.language;
                code = BugWorkaroundDecode(code);
                var runs = new List<Run>();
                var onFirstLine = true;
                IEnumerable<ColorizedLine> lines;
                switch (lang)
                {
                    case "csharp":
                    case "c#":
                    case "cs":
                        lines = Colorize.CSharp(code);
                        break;
                    case "vb":
                    case "vbnet":
                    case "vb.net":
                        lines = Colorize.VB(code);
                        break;
                    case "":
                    case "console":
                    case "xml":
                        lines = Colorize.PlainText(code);
                        break;
                    case "ANTLR":
                        lines = Colorize.PlainText(code);
                        break;
                    default:
                        reporter.Error("MD09", $"unrecognized language {lang}");
                        lines = Colorize.PlainText(code);
                        break;
                }

                foreach (var line in lines)
                {
                    int lineLength = line.Words.Sum(w => w.Text.Length);
                    if (lineLength > MaximumCodeLineLength)
                    {
                        reporter.Warning("MD32", $"Line length {lineLength} > maximum {MaximumCodeLineLength}");
                    }

                    if (onFirstLine)
                    {
                        onFirstLine = false;
                    }
                    else
                    {
                        runs.Add(new Run(new Break()));
                    }

                    foreach (var word in line.Words)
                    {
                        var run = new Run();
                        var props = new RunProperties();
                        if (word.Red != 0 || word.Green != 0 || word.Blue != 0)
                        {
                            props.Append(new Color { Val = $"{word.Red:X2}{word.Green:X2}{word.Blue:X2}" });
                        }

                        if (word.IsItalic)
                        {
                            props.Append(new Italic());
                        }

                        if (props.HasChildren)
                        {
                            run.Append(props);
                        }

                        run.Append(new Text(word.Text) { Space = SpaceProcessingModeValues.Preserve });
                        runs.Add(run);
                    }
                }
                var p = new Paragraph() { ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Code" }) };
                p.Append(runs);
                yield return p;
            }

            else if (md.IsTableBlock)
            {
                var mdt = md as MarkdownParagraph.TableBlock;
                var header = mdt.headers.Option();
                var align = mdt.alignments;
                var rows = mdt.rows;
                var table = TableHelpers.CreateTable();
                if (header == null)
                {
                    reporter.Error("MD10", "Github requires all tables to have header rows");
                }

                if (!header.Any(cell => cell.Length > 0))
                {
                    header = null; // even if Github requires an empty header, we can at least cull it from Docx
                }

                var ncols = align.Length;
                for (int irow = -1; irow < rows.Length; irow++)
                {
                    if (irow == -1 && header == null)
                    {
                        continue;
                    }

                    var mdrow = (irow == -1 ? header : rows[irow]);
                    var row = new TableRow();
                    for (int icol = 0; icol < Math.Min(ncols, mdrow.Length); icol++)
                    {
                        var mdcell = mdrow[icol];
                        var cell = new TableCell();
                        var pars = Paragraphs2Paragraphs(mdcell).ToList();
                        for (int ip = 0; ip < pars.Count; ip++)
                        {
                            var p = pars[ip] as Paragraph;
                            if (p == null)
                            {
                                cell.Append(pars[ip]);
                                continue;
                            }
                            var props = new ParagraphProperties(new ParagraphStyleId { Val = "TableCellNormal" });
                            if (align[icol].IsAlignCenter)
                            {
                                props.Append(new Justification { Val = JustificationValues.Center });
                            }

                            if (align[icol].IsAlignRight)
                            {
                                props.Append(new Justification { Val = JustificationValues.Right });
                            }

                            p.InsertAt(props, 0);
                            cell.Append(pars[ip]);
                        }
                        if (pars.Count == 0)
                        {
                            cell.Append(new Paragraph(new ParagraphProperties(new SpacingBetweenLines { After = "0" }), new Run(new Text(""))));
                        }

                        row.Append(cell);
                    }
                    table.Append(row);
                }
                foreach (var element in TableHelpers.CreateTableElements(table))
                {
                    yield return element;
                }
            }
            // Special handling for elements (typically tables) we can't represent nicely in Markdown
            else if (md is MarkdownParagraph.InlineBlock block && GetCustomBlockId(block) is string customBlockId)
            {
                foreach (var element in GenerateCustomBlockElements(customBlockId, block))
                {
                    yield return element;
                }
            }
            // Ignore any other HTML comments entirely
            else if (md is MarkdownParagraph.InlineBlock inlineBlock && inlineBlock.code.StartsWith("<!--"))
            {
                yield break;
            }
            else
            {
                reporter.Error("MD11", $"Unrecognized markdown element {md.GetType().Name}");
                yield return new Paragraph(new Run(new Text($"[{md.GetType().Name}]")));
            }
        }

        static string GetCustomBlockId(MarkdownParagraph.InlineBlock block)
        {
            Regex customBlockComment = new Regex(@"^<!-- Custom Word conversion: ([a-z0-9_]+) -->");
            var match = customBlockComment.Match(block.code);
            return match.Success ? match.Groups[1].Value : null;
        }

        IEnumerable<FlatItem> FlattenList(MarkdownParagraph.ListBlock md)
        {
            var flat = FlattenList(md, 0).ToList();
            var isOrdered = new Dictionary<int, bool>();
            foreach (var item in flat)
            {
                var level = item.Level;
                var isItemOrdered = item.IsBulletOrdered;
                var content = item.Paragraph;
                if (isOrdered.ContainsKey(level) && isOrdered[level] != isItemOrdered)
                {
                    reporter.Error("MD12", "List can't mix ordered and unordered items at same level");
                }

                isOrdered[level] = isItemOrdered;
                if (level > 3)
                {
                    reporter.Error("MD13", "Can't have more than 4 levels in a list");
                }
            }
            return flat;
        }

        // Workaround for https://github.com/dotnet/csharpstandard/issues/440
        // Code blocks in list items are parsed as InlineCode in a span instead of CodeBlock,
        // so we detect that and rewrite it.
        MarkdownParagraph.ListBlock MaybeRewriteListBlock(MarkdownParagraph.ListBlock listBlock)
        {
            // Regardless of the source, the Markdown parser rewrites the inline code to use the environment newline.
            string csharpPrefix = "csharp" + Environment.NewLine;

            var items = listBlock.items.Select(paragraphList => paragraphList.SelectMany(MaybeSplitParagraph));
            var fsharpItems = ListModule.OfSeq(items.Select(item => ListModule.OfSeq(item)));
            return (MarkdownParagraph.ListBlock) MarkdownParagraph.NewListBlock(listBlock.kind, fsharpItems, listBlock.range);

            IEnumerable<MarkdownParagraph> MaybeSplitParagraph(MarkdownParagraph paragraph)
            {
                if (paragraph is not MarkdownParagraph.Span span)
                {
                    yield return paragraph;
                    yield break;
                }
                var currentSpanBody = new List<MarkdownSpan>();
                // Note: the ranges in these paragraphs will be messed up, but that will rarely matter.
                // TODO: Maybe trim whitespace from the end of a literal before the block, and from the start of a literal
                // after the block? Otherwise they include blank lines. This looks okay, but may not be "strictly" ideal.
                foreach (var item in span.body)
                {
                    if (item is MarkdownSpan.InlineCode code && code.code.StartsWith(csharpPrefix))
                    {
                        if (currentSpanBody.Count > 0)
                        {
                            yield return MarkdownParagraph.NewSpan(ListModule.OfSeq(currentSpanBody), span.range);
                            currentSpanBody.Clear();
                        }
                        yield return MarkdownParagraph.NewCodeBlock(code.code.Substring(csharpPrefix.Length), "csharp", "", code.range);
                    }
                    else
                    {
                        currentSpanBody.Add(item);
                    }
                }
                if (currentSpanBody.Count > 0)
                {
                    yield return MarkdownParagraph.NewSpan(ListModule.OfSeq(currentSpanBody), span.range);
                }
            }
        }

        IEnumerable<FlatItem> FlattenList(MarkdownParagraph.ListBlock md, int level)
        {
            var isOrdered = md.kind.IsOrdered;
            var items = md.items;
            foreach (var mdpars in items)
            {
                var isFirstParagraph = true;
                foreach (var mdp in mdpars)
                {
                    var wasFirstParagraph = isFirstParagraph; isFirstParagraph = false;

                    if (mdp.IsParagraph || mdp.IsSpan)
                    {
                        var mdp1 = mdp;
                        var buglevel = BugWorkaroundIndent(ref mdp1, level);
                        yield return new FlatItem(buglevel, wasFirstParagraph, isOrdered, mdp1);
                    }
                    else if (mdp.IsQuotedBlock || mdp.IsCodeBlock)
                    {
                        yield return new FlatItem(level, false, isOrdered, mdp);
                    }
                    else if (mdp.IsListBlock)
                    {
                        foreach (var subitem in FlattenList(mdp as MarkdownParagraph.ListBlock, level + 1))
                        {
                            yield return subitem;
                        }
                    }
                    else if (mdp.IsTableBlock || mdp is MarkdownParagraph.InlineBlock inline && GetCustomBlockId(inline) is not null)
                    {
                        yield return new FlatItem(level, false, isOrdered, mdp);
                    }
                    else
                    {
                        reporter.Error("MD14", $"nothing fancy allowed in lists - specifically not '{mdp.GetType().Name}'");
                    }
                }
            }
        }


        IEnumerable<OpenXmlElement> Spans2Elements(IEnumerable<MarkdownSpan> mds, bool nestedSpan = false, bool inList = false)
        {
            // This is more longwinded than it might be, because we want to avoid ending with a break.
            // (That would occur naturally with a bullet point ending in a note, for example; the break
            // at the end adds too much space.)
            OpenXmlElement previous = null;
            foreach (var md in mds)
            {
                foreach (var e in Span2Elements(md, nestedSpan, inList))
                {
                    if (previous is object)
                    {
                        yield return previous;
                    }
                    previous = e;
                }
            }
            if (previous is object && !(previous is Break))
            {
                yield return previous;
            }
        }

        IEnumerable<OpenXmlElement> Span2Elements(MarkdownSpan md, bool nestedSpan = false, bool inList = false)
        {
            // Handle the end of a note or example in a list. Add a break at the end.
            if (inList && md.IsEmphasis)
            {
                var emphasis = (MarkdownSpan.Emphasis) md;
                if (emphasis.body.Length == 1 && emphasis.body[0] is MarkdownSpan.Literal { text: string literalText } &&
                    (literalText == "end example" || literalText == "end note"))
                {
                    foreach (var element in Span2Elements(md, nestedSpan, inList: false))
                    {
                        yield return element;
                    }
                    yield return new Break();
                    yield break;
                }
            }

            reporter.CurrentSpan = md;
            if (md.IsLiteral)
            {
                var mdl = md as MarkdownSpan.Literal;
                var s = MarkdownUtilities.UnescapeLiteral(mdl);

                // We have lines containing just "<!-- markdownlint-disable MD028 -->" which end up being
                // reported as literals after the note/example they end (rather than as InlineBlocks). Just ignore them.
                // Note that Environment.NewLine is needed as the Markdown parser replaces line breaks with that, regardless of the source.
                if (s.StartsWith($"{Environment.NewLine}<!--"))
                {
                    yield break;
                }

                foreach (var r in Literal2Elements(s, nestedSpan, inList))
                {
                    yield return r;
                }
            }

            else if (md.IsStrong || md.IsEmphasis)
            {
                IEnumerable<MarkdownSpan> spans = (md.IsStrong ? (md as MarkdownSpan.Strong).body : (md as MarkdownSpan.Emphasis).body);

                // Workaround for https://github.com/tpetricek/FSharp.formatting/issues/389 - the markdown parser
                // turns *this_is_it* into a nested Emphasis["this", Emphasis["is"], "it"] instead of Emphasis["this_is_it"]
                // What we'll do is preprocess it into Emphasis["this_is_it"]
                if (md.IsEmphasis)
                {
                    var spans2 = spans.Select(s =>
                    {
                        var _ = "";
                        if (s is MarkdownSpan.Emphasis emphasis)
                        {
                            if (emphasis.body.Count() != 1)
                            {
                                throw new Exception($"Got {emphasis.body.Count()} elements in {md}");
                            }
                            s = emphasis.body.Single();
                            _ = "_";
                        }
                        if (s.IsLiteral)
                        {
                            return _ + (s as MarkdownSpan.Literal).text + _;
                        }

                        reporter.Error("MD15", $"something odd inside emphasis '{s.GetType().Name}' - only allowed emphasis and literal"); return "";
                    });
                    spans = new List<MarkdownSpan>() { MarkdownSpan.NewLiteral(string.Join("", spans2), FSharpOption<MarkdownRange>.None) };
                }

                // Convention is that ***term*** is used to define a term.
                // That's parsed as Strong, which contains Emphasis, which contains one Literal
                string literal = null;
                TermRef termdef = null;
                if (!nestedSpan && md.IsStrong && spans.Count() == 1 && spans.First().IsEmphasis)
                {
                    var spans2 = (spans.First() as MarkdownSpan.Emphasis).body;
                    if (spans2.Count() == 1 && spans2.First().IsLiteral)
                    {
                        literal = (spans2.First() as MarkdownSpan.Literal).text;
                        termdef = new TermRef(literal, reporter.Location);
                        if (context.Terms.ContainsKey(literal))
                        {
                            var def = context.Terms[literal];
                            reporter.Warning("MD16", $"Term '{literal}' defined a second time");
                            reporter.Warning("MD16b", $"Here was the previous definition of term '{literal}'", def.Loc);
                        }
                        else
                        {
                            context.Terms.Add(literal, termdef);
                            context.TermKeys.Clear();
                        }
                    }
                }

                // Convention inside our specs is that emphasis only ever contains literals,
                // either to emphasis some human-text or to refer to an ANTLR-production
                if (!nestedSpan && md.IsEmphasis && (spans.Count() != 1 || !spans.First().IsLiteral))
                {
                    reporter.Error("MD17", $"something odd inside emphasis");
                }

                if (!nestedSpan && md.IsEmphasis && spans.Count() == 1 && spans.First().IsLiteral)
                {
                    literal = (spans.First() as MarkdownSpan.Literal).text;
                    // TODO: Maybe remove ItalicUse entirely, now we're not parsing the grammar.
                    context.Italics.Add(new ItalicUse(literal, ItalicUse.ItalicUseKind.Italic, reporter.Location));
                }

                if (termdef != null)
                {
                    context.MaxBookmarkId.Value += 1;
                    yield return new BookmarkStart { Name = termdef.BookmarkName, Id = context.MaxBookmarkId.Value.ToString() };
                    var props = new RunProperties(new Italic(), new Bold());
                    yield return new Run(new Text(literal) { Space = SpaceProcessingModeValues.Preserve }) { RunProperties = props };
                    yield return new BookmarkEnd { Id = context.MaxBookmarkId.Value.ToString() };
                }
                else
                {
                    foreach (var e in Spans2Elements(spans, true))
                    {
                        var style = (md.IsStrong ? new Bold() as OpenXmlElement : new Italic());
                        var run = e as Run;
                        if (run != null)
                        {
                            run.InsertAt(new RunProperties(style), 0);
                        }

                        yield return e;
                    }
                }
            }

            else if (md.IsInlineCode)
            {
                var mdi = md as MarkdownSpan.InlineCode;
                var code = BugWorkaroundDecode(mdi.code);

                foreach (var run in SplitLiteralByVerticalPosition().Select(CreateRun))
                {
                    yield return run;
                }

                Run CreateRun((string text, VerticalPositionValues position) part)
                {
                    var txt = new Text(part.text) { Space = SpaceProcessingModeValues.Preserve };
                    var props = part.position == VerticalPositionValues.Baseline
                        ? new RunProperties(new RunStyle { Val = "CodeEmbedded" })
                        : new RunProperties(new RunStyle { Val = "CodeEmbedded" }, new VerticalTextAlignment { Val = part.position });
                    return new Run(txt) { RunProperties = props };
                }

                // Splits the code into pieces by vertical position.
                // TODO: Use this more widely, for italics and even normal text potentially.
                // (Doing it everywhere would be potentially quite slow, and risks some correctness.)
                IEnumerable<(string text, VerticalPositionValues position)> SplitLiteralByVerticalPosition()
                {
                    StringBuilder builder = new StringBuilder();
                    VerticalPositionValues position = VerticalPositionValues.Baseline;
                    foreach (var c in code)
                    {
                        VerticalPositionValues nextPosition =
                            SubscriptUnicodeToAscii.TryGetValue(c, out var ascii) ? VerticalPositionValues.Subscript
                            : SuperscriptUnicodeToAscii.TryGetValue(c, out ascii) ? VerticalPositionValues.Superscript
                            : VerticalPositionValues.Baseline;
                        if (nextPosition != position && builder.Length > 0)
                        {
                            yield return (builder.ToString(), position);
                            builder.Clear();
                        }
                        position = nextPosition;
                        builder.Append(position == VerticalPositionValues.Baseline ? c : ascii);
                    }
                    yield return (builder.ToString(), position);
                }
            }

            else if (md.IsLatexInlineMath)
            {
                var latex = md as MarkdownSpan.LatexInlineMath;
                var code = latex.code;

                // TODO: Make this look nice - if we actually need it. It's possible that it's only present
                // before subscripts are replaced.
                var txt = new Text(BugWorkaroundDecode(code)) { Space = SpaceProcessingModeValues.Preserve };
                var props = new RunProperties(new RunStyle { Val = "CodeEmbedded" });
                var run = new Run(txt) { RunProperties = props };
                yield return run;
            }

            else if (md.IsDirectLink || md.IsIndirectLink)
            {
                IEnumerable<MarkdownSpan> spans;
                string url = "", alt = "";
                if (md.IsDirectLink)
                {
                    var mddl = md as MarkdownSpan.DirectLink;
                    spans = mddl.body;
                    url = mddl.link;
                    alt = mddl.title.Option();
                }
                else
                {
                    var mdil = md as MarkdownSpan.IndirectLink;
                    var original = mdil.original;
                    var id = mdil.key;
                    spans = mdil.body;
                    if (markdownDocument.DefinedLinks.ContainsKey(id))
                    {
                        url = markdownDocument.DefinedLinks[id].Item1;
                        alt = markdownDocument.DefinedLinks[id].Item2.Option();
                    }
                }

                var anchor = "";
                if (spans.Count() == 1 && spans.First().IsLiteral)
                {
                    anchor = MarkdownUtilities.UnescapeLiteral(spans.First() as MarkdownSpan.Literal);
                }
                else if (spans.Count() == 1 && spans.First().IsInlineCode)
                {
                    anchor = (spans.First() as MarkdownSpan.InlineCode).code;
                }
                else
                {
                    reporter.Error("MD18", $"Link anchor must be Literal or InlineCode, not '{md.GetType().Name}'");
                    yield break;
                }

                if (sections.ContainsKey(url))
                {
                    var section = sections[url];
                    // If we're linking to something with a section number, we know what the link text should be.
                    // (There are a few links that aren't to numbered sections, e.g. to "Annex C".)
                    if (section.Number is object)
                    {
                        var expectedAnchor = "§" + section.Number;
                        if (anchor != expectedAnchor)
                        {
                            reporter.Warning("MD19", $"Mismatch: link anchor is '{anchor}', should be '{expectedAnchor}'");
                        }
                    }

                    var txt = new Text(anchor) { Space = SpaceProcessingModeValues.Preserve };
                    var run = new Hyperlink(new Run(txt)) { Anchor = section.BookmarkName };
                    yield return run;
                }
                else if (url.StartsWith("http:") || url.StartsWith("https:"))
                {
                    var style = new RunStyle { Val = "Hyperlink" };
                    var hyperlink = new Hyperlink { DocLocation = url, Tooltip = alt };
                    foreach (var element in Spans2Elements(spans))
                    {
                        var run = element as Run;
                        if (run != null)
                        {
                            run.InsertAt(new RunProperties(style), 0);
                        }

                        hyperlink.AppendChild(run);
                    }
                    yield return hyperlink;
                }
                else
                {
                    // TODO: Make this report an error unconditionally once the subscript "latex-like" Markdown is removed.
                    if (url != "")
                    {
                        reporter.Error("MD28", $"Hyperlink url '{url}' unrecognized - not a recognized heading, and not http");
                    }
                }
            }

            else if (md.IsHardLineBreak)
            {
                // I've only ever seen this arise from dodgy markdown parsing, so I'll ignore it...
            }

            else
            {
                reporter.Error("MD20", $"Unrecognized markdown element {md.GetType().Name}");
                yield return new Run(new Text($"[{md.GetType().Name}]"));
            }
        }


        IEnumerable<OpenXmlElement> Literal2Elements(string literal, bool isNested, bool inList)
        {
            // Handle notes and examples embedded within bullet points. These are always
            // introduced by a literal either of just "> " or a line break followed by "> ".
            if (inList)
            {
                if (literal == "> ")
                {
                    yield return new Break();
                    yield break;
                }
                if (literal.EndsWith("\r\n> "))
                {
                    yield return new Run(new Text(literal.Substring(0, literal.Length - 4)) { Space = SpaceProcessingModeValues.Preserve });
                    yield return new Break();
                    yield break;
                }
            }

            // Otherwise, handle the literal normally.
            if (isNested || context.Terms.Count == 0)
            {
                yield return new Run(new Text(literal) { Space = SpaceProcessingModeValues.Preserve });
                yield break;
            }

            if (context.TermKeys.Count == 0)
            {
                context.TermKeys.AddRange(context.Terms.Keys);
            }

            foreach (var needle in context.FindNeedles(context.TermKeys, literal))
            {
                var s = literal.Substring(needle.Start, needle.Length);
                if (needle.NeedleId == -1)
                {
                    yield return new Run(new Text(s) { Space = SpaceProcessingModeValues.Preserve });
                    continue;
                }
                var termref = context.Terms[s];
                context.Italics.Add(new ItalicUse(s, ItalicUse.ItalicUseKind.Term, reporter.Location));
                var props = new RunProperties(new Underline { Val = UnderlineValues.Dotted, Color = "4BACC6" });
                var run = new Run(new Text(s) { Space = SpaceProcessingModeValues.Preserve }) { RunProperties = props };
                var link = new Hyperlink(run) { Anchor = termref.BookmarkName };
                yield return link;
            }
        }

        IEnumerable<OpenXmlCompositeElement> GenerateCustomBlockElements(string customBlockId, MarkdownParagraph.InlineBlock block) => customBlockId switch
        {
            "multiplication" => TableHelpers.CreateMultiplicationTable(),
            "division" => TableHelpers.CreateDivisionTable(),
            "remainder" => TableHelpers.CreateRemainderTable(),
            "addition" => TableHelpers.CreateAdditionTable(),
            "subtraction" => TableHelpers.CreateSubtractionTable(),
            "function_members" => TableHelpers.CreateFunctionMembersTable(block.code),
            "format_strings_1" => new[] { new Paragraph(new Run(new Text("FIXME: Replace with first format strings table"))) },
            "format_strings_2" => new[] { new Paragraph(new Run(new Text("FIXME: Replace with second format strings table"))) },
            // This is for the sake of a simple unit test. It's a single-row table with two cells.
            "test" => TableHelpers.CreateTestTable(),
            _ => HandleInvalidCustomBlock(customBlockId)
        };

        private static string BugWorkaroundDecode(string s)
        {
            // This function should be called on all inline-code and code blocks
            s = s.Replace("ceci_n'est_pas_une_pipe", "|");
            s = s.Replace("ceci_n'est_pas_une_", "");
            // When a pipe is needed within a table cell, it is escaped with a backslash.
            // We never actually want the backslash in the resulting text, so unescape it here.
            // (This is somewhat ugly and could cause problems if we ever want a backslash followed
            // by a pipe, but that's not the case at the moment.)
            s = s.Replace("\\|", "|");
            return s;
        }

        private static int BugWorkaroundIndent(ref MarkdownParagraph mdp, int level)
        {
            if (!mdp.IsParagraph)
            {
                return level;
            }

            var p = mdp as MarkdownParagraph.Paragraph;
            var spans = p.body;
            if (spans.Count() == 0 || !spans[0].IsLiteral)
            {
                return level;
            }

            var literal = spans[0] as MarkdownSpan.Literal;
            if (!literal.text.StartsWith("ceci-n'est-pas-une-indent"))
            {
                return level;
            }
            //
            var literal2 = MarkdownSpan.NewLiteral(literal.text.Substring(25), FSharpOption<MarkdownRange>.None);
            var spans2 = Microsoft.FSharp.Collections.FSharpList<MarkdownSpan>.Cons(literal2, spans.Tail);
            var p2 = MarkdownParagraph.NewParagraph(spans2, FSharpOption<MarkdownRange>.None);
            mdp = p2;
            return 0;
        }

        private IEnumerable<OpenXmlCompositeElement> HandleInvalidCustomBlock(string customBlockId)
        {
            reporter.Error("MD29", $"Invalid custom block ID: {customBlockId}");
            yield return new Paragraph(new Run(new Text($"Custom block {customBlockId}")));
        }

        internal static class TableHelpers
        {
            /// <summary>
            /// Generates the function members table, which has a rather more complex structure than the numeric tables,
            /// and has lots of text that might change over time. We parse the HTML (as XML) and go from there, handling
            /// a limited set of HTML elements.
            /// </summary>
            internal static IEnumerable<OpenXmlCompositeElement> CreateFunctionMembersTable(string xml)
            {
                XDocument doc = XDocument.Parse(xml);
                Table table = CreateTable(width: 9000);
                int rowsLeftToMerge = 0;
                foreach (var row in doc.Root.Elements("tr"))
                {
                    // Convert all the cells we *do* have...
                    var cells = row.Elements().Select(CreateCell).ToList();

                    // ... and then potentially amend or add a first cell in order to get the row span right.
                    int? rowSpan = (int?) row.Elements("td").FirstOrDefault()?.Attribute("rowspan");
                    if (rowSpan is object)
                    {
                        rowsLeftToMerge = rowSpan.Value - 1;
                        cells[0].TableCellProperties = new TableCellProperties
                        {
                            VerticalMerge = new VerticalMerge { Val = MergedCellValues.Restart }
                        };
                    }
                    else if (rowsLeftToMerge > 0)
                    {
                        var cell = CreateTableCell(new Paragraph());
                        cell.TableCellProperties = new TableCellProperties
                        {
                            VerticalMerge = new VerticalMerge { Val = MergedCellValues.Continue }
                        };
                        cells.Insert(0, cell);
                        rowsLeftToMerge--;
                    }
                    table.Append(new TableRow(cells));
                }

                return CreateTableElements(table);

                TableCell CreateCell(XElement xmlCell)
                {
                    if (xmlCell.Name.LocalName == "th")
                    {
                        var para = new Paragraph(new Run(new Text(xmlCell.Value)) { RunProperties = new RunProperties(new Bold()) });
                        return CreateTableCell(para, JustificationValues.Left);
                    }
                    var runs = xmlCell.Nodes().Select(CreateRun);
                    return CreateTableCell(new Paragraph(runs), JustificationValues.Left);
                }

                Run CreateRun(XNode node)
                {
                    if (node is XElement element && element.Name.LocalName == "code")
                    {
                        var txt = new Text(BugWorkaroundDecode(element.Value)) { Space = SpaceProcessingModeValues.Preserve };
                        var props = new RunProperties(new RunStyle { Val = "CodeEmbedded" });
                        return new Run(txt) { RunProperties = props };
                    }
                    else if (node is XText text)
                    {
                        return new Run(new Text(text.Value) { Space = SpaceProcessingModeValues.Preserve });
                    }
                    else
                    {
                        throw new Exception($"Unexpected node {node.NodeType} in function members table");
                    }
                }
            }

            internal static IEnumerable<OpenXmlCompositeElement> CreateMultiplicationTable()
            {
                Table table = CreateTable(indentation: TableIndentation + InitialIndentation, width: 8000);
                table.Append(CreateTableRow(Empty, PlusY, MinusY, PlusZero, MinusZero, PlusInfinity, MinusInfinity, NaN));
                table.Append(CreateTableRow(PlusX, PlusZ, MinusZ, PlusZero, MinusZero, PlusInfinity, MinusInfinity, NaN));
                table.Append(CreateTableRow(MinusX, MinusZ, PlusZ, MinusZero, PlusZero, MinusInfinity, PlusInfinity, NaN));
                table.Append(CreateTableRow(PlusZero, PlusZero, MinusZero, PlusZero, MinusZero, NaN, NaN, NaN));
                table.Append(CreateTableRow(MinusZero, MinusZero, PlusZero, MinusZero, PlusZero, NaN, NaN, NaN));
                table.Append(CreateTableRow(PlusInfinity, PlusInfinity, MinusInfinity, NaN, NaN, PlusInfinity, MinusInfinity, NaN));
                table.Append(CreateTableRow(MinusInfinity, MinusInfinity, PlusInfinity, NaN, NaN, MinusInfinity, PlusInfinity, NaN));
                table.Append(CreateTableRow(NaN, NaN, NaN, NaN, NaN, NaN, NaN, NaN));
                return CreateTableElements(table);
            }

            internal static IEnumerable<OpenXmlCompositeElement> CreateDivisionTable()
            {
                Table table = CreateTable(indentation: TableIndentation + InitialIndentation, width: 8000);
                table.Append(CreateTableRow(Empty, PlusY, MinusY, PlusZero, MinusZero, PlusInfinity, MinusInfinity, NaN));
                table.Append(CreateTableRow(PlusX, PlusZ, MinusZ, PlusInfinity, MinusInfinity, PlusZero, MinusZero, NaN));
                table.Append(CreateTableRow(MinusX, MinusZ, PlusZ, MinusInfinity, PlusInfinity, MinusZero, PlusZero, NaN));
                table.Append(CreateTableRow(PlusZero, PlusZero, MinusZero, NaN, NaN, PlusZero, MinusZero, NaN));
                table.Append(CreateTableRow(MinusZero, MinusZero, PlusZero, NaN, NaN, MinusZero, PlusZero, NaN));
                table.Append(CreateTableRow(PlusInfinity, PlusInfinity, MinusInfinity, PlusInfinity, MinusInfinity, NaN, NaN, NaN));
                table.Append(CreateTableRow(MinusInfinity, MinusInfinity, PlusInfinity, MinusInfinity, PlusInfinity, NaN, NaN, NaN));
                table.Append(CreateTableRow(NaN, NaN, NaN, NaN, NaN, NaN, NaN, NaN));
                return CreateTableElements(table);
            }

            internal static IEnumerable<OpenXmlCompositeElement> CreateRemainderTable()
            {
                Table table = CreateTable(indentation: TableIndentation + InitialIndentation, width: 8000);
                table.Append(CreateTableRow(Empty, PlusY, MinusY, PlusZero, MinusZero, PlusInfinity, MinusInfinity, NaN));
                table.Append(CreateTableRow(PlusX, PlusZ, PlusZ, NaN, NaN, PlusX, PlusX, NaN));
                table.Append(CreateTableRow(MinusX, MinusZ, MinusZ, NaN, NaN, MinusX, MinusX, NaN));
                table.Append(CreateTableRow(PlusZero, PlusZero, PlusZero, NaN, NaN, PlusZero, PlusZero, NaN));
                table.Append(CreateTableRow(MinusZero, MinusZero, MinusZero, NaN, NaN, MinusZero, MinusZero, NaN));
                table.Append(CreateTableRow(PlusInfinity, NaN, NaN, NaN, NaN, NaN, NaN, NaN));
                table.Append(CreateTableRow(MinusInfinity, NaN, NaN, NaN, NaN, NaN, NaN, NaN));
                table.Append(CreateTableRow(NaN, NaN, NaN, NaN, NaN, NaN, NaN, NaN));
                return CreateTableElements(table);
            }

            internal static IEnumerable<OpenXmlCompositeElement> CreateAdditionTable()
            {
                Table table = CreateTable(indentation: TableIndentation + InitialIndentation, width: 8000);
                table.Append(CreateTableRow(Empty, Y, PlusZero, MinusZero, PlusInfinity, MinusInfinity, NaN));
                table.Append(CreateTableRow(X, Z, X, X, PlusInfinity, MinusInfinity, NaN));
                table.Append(CreateTableRow(PlusZero, Y, PlusZero, PlusZero, PlusInfinity, MinusInfinity, NaN));
                table.Append(CreateTableRow(MinusZero, Y, PlusZero, MinusZero, PlusInfinity, MinusInfinity, NaN));
                table.Append(CreateTableRow(PlusInfinity, PlusInfinity, PlusInfinity, PlusInfinity, PlusInfinity, NaN, NaN));
                table.Append(CreateTableRow(MinusInfinity, MinusInfinity, MinusInfinity, MinusInfinity, NaN, MinusInfinity, NaN));
                table.Append(CreateTableRow(NaN, NaN, NaN, NaN, NaN, NaN, NaN));
                return CreateTableElements(table);
            }
           
            internal static IEnumerable<OpenXmlCompositeElement> CreateSubtractionTable()
            {
                Table table = CreateTable(indentation: TableIndentation + InitialIndentation, width: 8000);
                table.Append(CreateTableRow(Empty, Y, PlusZero, MinusZero, PlusInfinity, MinusInfinity, NaN));
                table.Append(CreateTableRow(X, Z, X, X, MinusInfinity, PlusInfinity, NaN));
                table.Append(CreateTableRow(PlusZero, MinusY, PlusZero, PlusZero, MinusInfinity, PlusInfinity, NaN));
                table.Append(CreateTableRow(MinusZero, MinusY, MinusZero, PlusZero, MinusInfinity, PlusInfinity, NaN));
                table.Append(CreateTableRow(PlusInfinity, PlusInfinity, PlusInfinity, PlusInfinity, NaN, PlusInfinity, NaN));
                table.Append(CreateTableRow(MinusInfinity, MinusInfinity, MinusInfinity, MinusInfinity, MinusInfinity, NaN, NaN));
                table.Append(CreateTableRow(NaN, NaN, NaN, NaN, NaN, NaN, NaN));
                return CreateTableElements(table);
            }

            internal static IEnumerable<OpenXmlCompositeElement> CreateTestTable()
            {
                Table table = CreateTable(indentation: 900, width: 8000);
                table.Append(CreateTableRow(CreateNormalTableCell("Normal cell"), CreateCodeTableCell("Code cell")));
                return CreateTableElements(table);
            }
            
            private static TableRow CreateTableRow(params TableCell[] cells) => new TableRow(cells);

            internal static Table CreateTable(int indentation = TableIndentation, int? width = null)
            {                
                var props = new TableProperties
                {
                    TableStyle = new TableStyle { Val = "TableGrid" },
                    TableIndentation = new TableIndentation { Width = indentation, Type = TableWidthUnitValues.Dxa },
                    TableBorders = new TableBorders
                    {
                        TopBorder = new TopBorder { Val = BorderValues.Single },
                        BottomBorder = new BottomBorder { Val = BorderValues.Single },
                        LeftBorder = new LeftBorder { Val = BorderValues.Single },
                        RightBorder = new RightBorder { Val = BorderValues.Single },
                        InsideHorizontalBorder = new InsideHorizontalBorder { Val = BorderValues.Single },
                        InsideVerticalBorder = new InsideVerticalBorder { Val = BorderValues.Single }
                    },
                    TableCellMarginDefault = new TableCellMarginDefault()
                };
                if (width is object)
                {
                    props.TableWidth = new TableWidth { Width = width.ToString() };
                }
                return new Table(props);
            }

            internal static IEnumerable<OpenXmlCompositeElement> CreateTableElements(Table table)
            {
                yield return new Paragraph(new Run(new Text(""))) { ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "TableLineBefore" }) };
                yield return table;
                yield return new Paragraph(new Run(new Text(""))) { ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "TableLineAfter" }) };
            }

            // Properties to create well-known table cells.
            // Each call creates a new object, which is unconventional
            // but required as any one cell can't be added more than once.
            // These could be methods, but that would add clutter in the calling code.
            private static TableCell Empty => CreateNormalTableCell("");
            private static TableCell X => CreateCodeTableCell("x");
            private static TableCell Y => CreateCodeTableCell("y");
            private static TableCell Z => CreateCodeTableCell("z");
            private static TableCell PlusX => CreateCodeTableCell("+x");
            private static TableCell PlusY => CreateCodeTableCell("+y");
            private static TableCell PlusZ => CreateCodeTableCell("+z");
            private static TableCell MinusX => CreateCodeTableCell("-x");
            private static TableCell MinusY => CreateCodeTableCell("-y");
            private static TableCell MinusZ => CreateCodeTableCell("-z");
            private static TableCell PlusInfinity => CreateCodeTableCell("+\u221E");
            private static TableCell MinusInfinity => CreateCodeTableCell("-\u221E");
            private static TableCell PlusZero => CreateCodeTableCell("+0");
            private static TableCell MinusZero => CreateCodeTableCell("-0");
            private static TableCell NaN => CreateCodeTableCell("NaN");

            private static TableCell CreateNormalTableCell(string text)
            {
                var p = new Paragraph(new Run(new Text(text)));
                return CreateTableCell(p);
            }

            private static TableCell CreateCodeTableCell(string text)
            {
                var p = new Paragraph(new Run(new Text(text)))
                {
                    ParagraphProperties = new ParagraphProperties
                    {
                        ParagraphStyleId = new ParagraphStyleId { Val = "Code" },
                        // It's unclear why we need this indentation, but without it we don't get centering.
                        Indentation = new Indentation { Left = "0" }
                    }
                };
                return CreateTableCell(p);
            }

            private static TableCell CreateTableCell(Paragraph paragraph, JustificationValues? justification = null)
            {
                var cell = new TableCell();

                var props = new ParagraphProperties
                {
                    ParagraphStyleId = new ParagraphStyleId { Val = "TableCellNormal" },
                    Justification = new Justification { Val = justification ?? JustificationValues.Center }
                };
                cell.Append(props);
                cell.Append(paragraph);
                return cell;
            }
        }
    }
}
