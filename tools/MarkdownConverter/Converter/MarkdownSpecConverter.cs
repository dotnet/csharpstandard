using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MarkdownConverter.Spec;
using System;
using System.IO;
using System.Linq;

namespace MarkdownConverter.Converter
{
    internal static class MarkdownSpecConverter
    {
        public static void ConvertToWord(MarkdownSpec spec, string templateFile, string outputFile, Reporter reporter)
        {
            using (var templateDoc = WordprocessingDocument.Open(templateFile, false))
            using (var resultDoc = WordprocessingDocument.Create(outputFile, WordprocessingDocumentType.Document))
            {
                foreach (var part in templateDoc.Parts)
                {
                    resultDoc.AddPart(part.OpenXmlPart, part.RelationshipId);
                }

                var body = resultDoc.MainDocumentPart.Document.Body;

                ReplaceTableOfContents(spec, body);

                var context = new ConversionContext();
                context.MaxBookmarkId.Value = 1 + body.Descendants<BookmarkStart>().Max(bookmark => int.Parse(bookmark.Id));

                foreach (var src in spec.Sources)
                {
                    string fileName = Path.GetFileName(src.Item1);
                    var converter = new MarkdownSourceConverter(
                        markdownDocument: src.Item2,
                        wordDocument: resultDoc,
                        spec: spec,
                        context: context,
                        filename: fileName,
                        reporter.WithFileName(fileName));
                    foreach (var p in converter.Paragraphs())
                    {
                        body.AppendChild(p);
                    }
                }
            }
        }

        private static void ReplaceTableOfContents(MarkdownSpec spec, Body body)
        {
            // We have to find the TOC, if one exists, and replace it...
            if (FindToc(body, out var tocFirst, out var tocLast, out var tocInstr, out var tocSec))
            {
                for (int i = tocLast; i >= tocFirst; i--)
                {
                    body.RemoveChild(body.ChildElements[i]);
                }
                var afterToc = body.ChildElements[tocFirst];
                for (int i = 0; i < spec.Sections.Count; i++)
                {
                    var section = spec.Sections[i];
                    if (section.Level > 2)
                    {
                        continue;
                    }

                    var p = new Paragraph();
                    p.AppendChild(new Hyperlink(new Run(new Text(section.Title))) { Anchor = section.BookmarkName });

                    p.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = $"TOC{section.Level}" });
                    body.InsertBefore(p, afterToc);
                }
                if (tocSec != null)
                {
                    body.InsertBefore(tocSec, afterToc);
                }
            }
        }

        private static bool FindToc(Body body, out int ifirst, out int iLast, out string instr, out Paragraph secBreak)
        {
            ifirst = -1; iLast = -1; instr = null; secBreak = null;

            for (int i = 0; i < body.ChildElements.Count; i++)
            {
                var p = body.ChildElements.GetItem(i) as Paragraph;
                if (p == null)
                {
                    continue;
                }

                // The TOC might be a simple field
                var sf = p.OfType<SimpleField>().FirstOrDefault();
                if (sf != null && sf.Instruction.Value.Contains("TOC"))
                {
                    if (ifirst != -1)
                    {
                        throw new Exception("Found start of TOC and then another simple TOC");
                    }

                    ifirst = i; iLast = i; instr = sf.Instruction.Value;
                    break;
                }

                // or it might be a complex field
                var runElements = (from r in p.OfType<Run>() from e in r select e).ToList();
                var f1 = runElements.FindIndex(f => f is FieldChar && (f as FieldChar).FieldCharType.Value == FieldCharValues.Begin);
                var f2 = runElements.FindIndex(f => f is FieldCode && (f as FieldCode).Text.Contains("TOC"));
                var f3 = runElements.FindIndex(f => f is FieldChar && (f as FieldChar).FieldCharType.Value == FieldCharValues.Separate);
                var f4 = runElements.FindIndex(f => f is FieldChar && (f as FieldChar).FieldCharType.Value == FieldCharValues.End);

                if (f1 != -1 && f2 != -1 && f3 != -1 && f2 > f1 && f3 > f2)
                {
                    if (ifirst != -1)
                    {
                        throw new Exception("Found start of TOC and then another start of TOC");
                    }

                    ifirst = i; instr = (runElements[f2] as FieldCode).Text;
                }
                if (f4 != -1 && f4 > f1 && f4 > f2 && f4 > f3)
                {
                    iLast = i;
                    if (ifirst != -1)
                    {
                        break;
                    }
                }
            }

            if (ifirst == -1)
            {
                return false;
            }

            if (iLast == -1)
            {
                throw new Exception("Found start of TOC field, but not end");
            }

            for (int i = ifirst; i <= iLast; i++)
            {
                var p = body.ChildElements.GetItem(i) as Paragraph;
                if (p == null)
                {
                    continue;
                }

                var sp = p.ParagraphProperties.OfType<SectionProperties>().FirstOrDefault();
                if (sp == null)
                {
                    continue;
                }

                if (i != iLast)
                {
                    throw new Exception("Found section break within TOC field");
                }

                secBreak = new Paragraph(new Run(new Text(""))) { ParagraphProperties = new ParagraphProperties(sp.CloneNode(true)) };
            }
            return true;
        }
    }
}
