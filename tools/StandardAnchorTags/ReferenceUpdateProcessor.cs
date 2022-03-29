using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StandardAnchorTags
{
    internal class ReferenceUpdateProcessor
    {
        const char sectionReference = '§';

        private readonly IReadOnlyDictionary<string, SectionLink> linkMap;
        private readonly bool dryRun;
        private readonly string PathToFiles;
        public int ErrorCount { get; private set; }

        public ReferenceUpdateProcessor(string pathToFiles, IReadOnlyDictionary<string, SectionLink> linkMap, bool dryRun)
        {
            PathToFiles = pathToFiles;
            this.linkMap = linkMap;
            this.dryRun = dryRun;
        }

        public async Task ReplaceReferences(string file)
        {
            var inputPath = $"{PathToFiles}/{file}";
            var tmpFileName = $"{file}.tmp";
            int lineNumber = 0;
            using (var readStream = new StreamReader(inputPath))
            {
                using StreamWriter writeStream = new(tmpFileName);
                while (await readStream.ReadLineAsync() is string line)
                {
                    lineNumber++;
                    var updatedLine = line.Contains(sectionReference)
                        ? ProcessSectionLinks(line, lineNumber, file)
                        : line;
                    await writeStream.WriteLineAsync(updatedLine);
                }
                writeStream.Close();
                readStream.Close();
            }
            if (dryRun)
            {
                File.Delete(tmpFileName);
            }
            else
            {
                File.Move(tmpFileName, inputPath, true);
            }
        }

        private string ProcessSectionLinks(string line, int lineNumber, string file)
        {
            var returnedLine = new StringBuilder();
            int index = 0;

            while (FindNextSectionReference(line, index) is Range sectionReferenceRange) // found another section reference.
            {
                // Grab the section text:
                string referenceText = line[sectionReferenceRange];
                string linkText = referenceText;
                if ((referenceText.Length > 1) &&
                    (!linkMap.ContainsKey(referenceText)))
                {
                    var msg = $"Section reference [{referenceText}] not found at line {lineNumber} in {file}";
                    if (dryRun)
                    {
                        ErrorCount++;
                        Console.WriteLine(msg);
                    }
                    else
                        throw new InvalidOperationException(msg);
                } else
                {
                    linkText = linkMap[referenceText].FormattedMarkdownLink;
                }
                // expand the range for any existing link:
                sectionReferenceRange = ExpandToIncludeExistingLink(line, sectionReferenceRange);

                var textRangeToCopyUnchanged = new Range(index, sectionReferenceRange.Start);
                // Copy text up to replacement:
                returnedLine.Append(line[textRangeToCopyUnchanged]);

                returnedLine.Append(linkText);
                index = sectionReferenceRange.End.Value;
            }
            // Copy remaining text
            returnedLine.Append(line[index..]);
            return returnedLine.ToString();
        }

        private static Range? FindNextSectionReference(string line, int startIndex)
        {
            // Find the start:
            startIndex = line.IndexOf(sectionReference, startIndex);

            if (startIndex == -1) return default;
            int endIndex = startIndex + 1;

            // The first character not in the set of:
            // A..Z
            // a..z
            // 0..9
            // .
            // - 
            // indicates the end of the link reference.
            while ((endIndex < line.Length) && (line[endIndex] switch
            {
                >= 'A' and <= 'Z' => true,
                >= 'a' and <= 'z' => true,
                >= '0' and <= '9' => true,
                '.' or '-' => true,
                _ => false,
            })) endIndex++;

            // One final special case: If the last character is '.', it's not 
            // part of the section reference, it's the period at the end of a sentence:
            if (line[endIndex - 1] == '.')
                endIndex--;
            return new Range(startIndex, endIndex);
        }

        private static Range ExpandToIncludeExistingLink(string line, Range range)
        {
            // If the character before the start of the range isn't the '[' character,
            // return => no existing link.
            if (range.Start.Value == 0) return range;
            var previous = range.Start.Value - 1;
            if (line[previous] != '[') return range;

            // Start and the end of the range, look for "](", then ']'. 
            int endIndex = range.End.Value;
            if (line.Substring(endIndex, 2) != "](") throw new InvalidOperationException("Unexpected link text");

            endIndex += 2;
            while (line[endIndex] != ')') endIndex++;

            return new Range(previous, endIndex + 1);
        }
    }
}