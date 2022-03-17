using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

namespace StandardAnchorTags
{
    /// <summary>
    /// This builds the TOC numbers, and a mapping of all anchors.
    /// </summary>
    /// <remarks>
    /// In addition, the creation of the TOC Section map creates two side-effects:
    /// 1. Updates all headers in the source markdown to have the proper updated section numbers.
    /// 2. Create the toc.md file, containing updated section numbers.
    /// </remarks>
    public class TocSectionNumberBuilder
    {
        private struct SectionHeader
        {
            public int level;
            public string sectionHeaderText;
            public string title;

        }

        private const string MainSectionPattern = @"^\d+(\.\d+)*$";
        private const string AnnexPattern = @"^[A-Z](\.\d+)*$";

        private readonly string PathToStandardFiles;
        private readonly bool dryRun;

        // String builder to store the full TOC for the standard.
        private readonly StringBuilder tocContent = new();
        private readonly Dictionary<string, SectionLink> sectionLinkMap = new();
        private bool isAnnexes;

        // Running array of entries for the current headings. 
        // Starting with H1 is headings[0], H2 is headings[1] etc.
        private readonly int[] headings = new int[8];

        /// <summary>
        /// Construct the map Builder.
        /// </summary>
        public TocSectionNumberBuilder(string pathFromToolToStandard, bool dryRun)
        {
            PathToStandardFiles = pathFromToolToStandard;
            this.dryRun = dryRun;
        }

        /// <summary>
        /// Add the front matter entries to the TOC
        /// </summary>
        /// <param name="fileName">The front matter file name.</param>
        /// <remarks>
        /// The front matter entries don't add section numbers in the headers.
        /// Otherwise, this is the same logic for the main file.
        /// </remarks>
        public async Task AddFrontMatterTocEntries(string fileName)
        {
            using var stream = File.OpenText(Path.Combine(PathToStandardFiles,fileName));
            string? line = await stream.ReadLineAsync();
            {
                if (line?.StartsWith("# ") == true)
                {
                    var linkText = line[2..];
                    tocContent.AppendLine($"- [{linkText}]({fileName})");
                    // Done: return.
                    return;
                }
            }
            // Getting here means this file doesn't have an H1. That's an error:
            throw new InvalidOperationException($"File {fileName} doesn't have an H1 tag as its first line.");
        }

        public async Task AddContentsToTOC(string filename)
        {
            string pathToFile = $"{PathToStandardFiles}/{filename}";
            string tmpFileName = $"{filename}-updated.md";
            string? line;
            int lineNumber = 0;
            using (var stream = new StreamReader(pathToFile))
            {
                using var writeStream = new StreamWriter(tmpFileName);
                while ((line = await stream.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    if (FindHeader(line) is SectionHeader header)
                    {
                        SectionLink link = BuildSectionLink(header,filename);
                        var linkDestinationUrl = $"{filename}#{link.AnchorText}";
                        // Add to collection, if the header has link text:
                        if (!string.IsNullOrWhiteSpace(header.sectionHeaderText))
                        {
                            if (sectionLinkMap.ContainsKey(header.sectionHeaderText))
                                throw new InvalidOperationException($"Duplicate section header [{header.sectionHeaderText}] found at line {lineNumber} in {filename}");
                            sectionLinkMap.Add(header.sectionHeaderText, link);
                        }
                        // Build the new header line
                        var atxHeader = new string('#', header.level);
                        // Write TOC line
                        tocContent.AppendLine($"{new string(' ', (header.level - 1) * 2)}- {link.TOCMarkdownLink()}  {header.title}");
                        line = $"{atxHeader} {(isAnnexes && (header.level == 1) ? "Annex " : "")}{link.NewLinkText} {header.title}";
                    }
                    await writeStream.WriteLineAsync(line);
                }
            }
            if (dryRun)
            {
                File.Delete(tmpFileName);
            }
            else
            {
                File.Move(tmpFileName, pathToFile, true);
            }
        }

        /// <summary>
        /// This method signals that all main sections have been processed. 
        /// </summary>
        /// <remarks>
        /// After this method is called, all sections are interpreted as annexes.
        /// </remarks>
        public void FinishMainSection()
        {
            isAnnexes = true;
            for (int i = 0; i < headings.Length; i++) headings[i] = 0;
        }

        /// <summary>
        /// Retrieve the text of the TOC
        /// </summary>
        public string Toc => tocContent.ToString();

        /// <summary>
        /// Retrieve a readonly map of existing link text to updated link.
        /// </summary>
        public IReadOnlyDictionary<string, SectionLink> LinkMap => sectionLinkMap;

        private SectionLink BuildSectionLink(SectionHeader header, string filename)
        {
            // Now, construct the mapping, add TOC, construct output line.
            // Set all headings:
            headings[header.level - 1]++;
            for (int index = header.level; index < headings.Length; index++)
                headings[index] = 0;

            // Generate the correct clause name:
            string newSectionNumber = isAnnexes
                ? string.Join('.', headings.Take(header.level).Select((n, index) => (index == 0) ? ((char)(n + 64)).ToString() : n.ToString()))
                : string.Join('.', headings.Take(header.level).Select(n => n.ToString()));
            string anchor = $"{newSectionNumber} {header.title}"
                .Replace(' ', '-').Replace(".", "").Replace(",", "").Replace("`", "")
                .Replace("/", "").Replace(":", "").Replace("?", "").Replace("&", "")
                .Replace("|", "").Replace("!", "").Replace("\\<", "").Replace("\\>", "").Replace("\\#", "")
                .Replace("…", "")
                .ToLower();

            // Top-level annex references (e.g. just to "Annex D") need a leading "annex-" as that's
            // in the title of the page.
            if (isAnnexes && (header.level == 1))
            {
                anchor = $"annex-{anchor}";
            }
            return new SectionLink(header.sectionHeaderText, newSectionNumber, $"{filename}#{anchor}");
        }


        // A line in the standard is either a paragraph of text or
        // a header. this method determines which and returns one 
        // of the two types.
        private SectionHeader? FindHeader(string lineRead)
        {
            // Blank line:
            if (string.IsNullOrWhiteSpace(lineRead))
            {
                return null;
            }

            var fields = lineRead.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

            int level = fields[0].All(c => c == '#') ? fields[0].Length : 0;
            // input line is a paragraph of text.
            if (level == 0)
            {
                return null;
            }
            SectionHeader header = new()
            {
                level = level
            };

            // A few cases for section number:
            // 1. Starts with §: represents a newly added section
            if (fields[1].StartsWith("§"))
            {
                header.sectionHeaderText = fields[1];
                header.title = fields[2];
                return header;
            }
            (header.sectionHeaderText, header.title) =
                (isAnnexes, level, fields[1]) switch
                {
                    // Annex H1: "Annex B" (A-Z)
                    (true, 1, "Annex") => ("§" + fields[2][..1], fields[2][2..]),
                    // Annex H1, no section header.
                    (true, 1, _) => ("", fields[1] + " " + fields[2]),
                    // Annex, Hn: "D.1.2", or no section header:
                    // Main section, "12.7.2", or no section header text:
                    (_, _, _) => Regex.IsMatch(fields[1], (isAnnexes ? AnnexPattern : MainSectionPattern))
                        ? ("§"+fields[1], fields[2])
                        : ("", fields[1] + " " + fields[2]),
                };
            return header;
        }
    }
}
