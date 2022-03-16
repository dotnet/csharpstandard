using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using MarkdownConverter.Converter;
using MarkdownConverter.Spec;
using Org.XmlUnit;
using Org.XmlUnit.Builder;
using Org.XmlUnit.Diff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace MarkdownConverter.Tests
{
    public class MarkdownSourceConverterTests
    {
        [Theory]
        [InlineData("table-with-pipe")]
        [InlineData("antlr-with-line-comment")]
        [InlineData("note")]
        [InlineData("code-block-in-list")]
        public void SingleResourceConversion(string name)
        {
            var reporter = new Reporter(TextWriter.Null);
            var expectedXml = ReadResource($"{name}.xml");
            var spec = MarkdownSpec.ReadFiles(new[] { $"{name}.md" }, reporter, name => new StreamReader(new MemoryStream(ReadResource(name))));

            var resultDoc = WordprocessingDocument.Create(new MemoryStream(), WordprocessingDocumentType.Document);
            resultDoc.AddMainDocumentPart();
            var source = spec.Sources.Single();
            var converter = new MarkdownSourceConverter(source.Item2, wordDocument: resultDoc,
                spec: spec,
                filename: $"{name}.md",
                reporter);

            // Gather all the paragraphs together, but remove all namespaces aliases so our test documents can be simpler.
            // (While a single declaration of the namespace in the root element works as a default for element names,
            // it doesn't help with attribute names.)
            var paragraphs = converter.Paragraphs.ToList();
            XDocument actualXDocument = XDocument.Parse($@"<doc>{string.Join("\r\n", paragraphs.Select(p => p.OuterXml))}</doc>");
            // Remove attributes
            foreach (var element in actualXDocument.Root!.Descendants())
            {
                element.Name = element.Name.LocalName;
                element.Attributes().Where(attr => attr.Name.Namespace == XNamespace.Xmlns).Remove();
                element.ReplaceAttributes(element.Attributes().Select(attr => new XAttribute(attr.Name.LocalName, attr.Value)));
            }

            ISource expectedDoc = Input.FromByteArray(expectedXml).Build();
            ISource actualDoc = Input.FromDocument(actualXDocument).Build();
            IDifferenceEngine diff = new DOMDifferenceEngine();
            var differences = new List<Comparison>();
            diff.DifferenceListener += (comparison, outcome) => differences.Add(comparison);
            diff.Compare(expectedDoc, actualDoc);
            Assert.Empty(differences);
        }

        [Theory]
        [InlineData(MarkdownSourceConverter.MaximumCodeLineLength, true, 0)]
        [InlineData(MarkdownSourceConverter.MaximumCodeLineLength + 1, false, 0)]
        [InlineData(MarkdownSourceConverter.MaximumCodeLineLength + 1, true, 1)]
        public void LongLineWarnings(int lineLength, bool code, int expectedWarningCount)
        {
            string prefix = code ? "```csharp\r\n" : "";
            string line = new string('x', lineLength);
            string suffix = code ? "```\r\n" : "";
            string text = $"# 1 Heading\r\n{prefix}{line}\r\n{suffix}";
            var reporter = new Reporter(TextWriter.Null);
            var spec = MarkdownSpec.ReadFiles(new[] { "test.md" }, reporter, _ => new StringReader(text));
            var resultDoc = WordprocessingDocument.Create(new MemoryStream(), WordprocessingDocumentType.Document);
            var source = spec.Sources.Single();
            var converter = new MarkdownSourceConverter(source.Item2, wordDocument: resultDoc,
                spec: spec,
                filename: "test.md",
                reporter);

            Assert.Equal(expectedWarningCount, reporter.Warnings);
        }

        private static byte[] ReadResource(string name)
        {
            var fullName = $"MarkdownConverter.Tests.{name}";
            var asm = typeof(MarkdownSourceConverterTests).Assembly;
            using var resource = asm.GetManifestResourceStream(fullName);
            if (resource is null)
            {
                throw new ArgumentException($"Can't find resource '{fullName}'. Available resources: {string.Join(", ", asm.GetManifestResourceNames())}");
            }
            using var memory = new MemoryStream();
            resource.CopyTo(memory);
            return memory.ToArray();
        }
    }
}
