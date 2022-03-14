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
using Xunit;

namespace MarkdownConverter.Tests
{
    public class MarkdownSourceConverterTests
    {
        [Theory]
        [InlineData("table-with-pipe")]
        public void SingleResourceConversion(string name)
        {
            var expectedXml = ReadResource($"{name}.xml");
            var spec = MarkdownSpec.ReadFiles(new[] { $"{name}.md" }, name => new StreamReader(new MemoryStream(ReadResource(name))));

            var resultDoc = WordprocessingDocument.Create(new MemoryStream(), WordprocessingDocumentType.Document);
            var source = spec.Sources.Single();
            var converter = new MarkdownSourceConverter(source.Item2, wordDocument: resultDoc,
                spec: spec,
                context: new ConversionContext(),
                filename: $"{name}.md");
            var paragraphs = converter.Paragraphs().ToList();
            var actualXmlDoc = $"<doc>{string.Join("\r\n", paragraphs.Select(p => p.OuterXml))}</doc>";
            ISource expectedDoc = Input.FromByteArray(expectedXml).Build();
            ISource actualDoc = Input.FromString(actualXmlDoc).Build();
            IDifferenceEngine diff = new DOMDifferenceEngine();
            var differences = new List<Comparison>();
            diff.DifferenceListener += (comparison, outcome) => differences.Add(comparison);
            diff.Compare(expectedDoc, actualDoc);
            Assert.Empty(differences);
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
