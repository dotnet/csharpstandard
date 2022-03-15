using MarkdownConverter.Spec;
using System;
using Xunit;

namespace MarkdownConverter.Tests
{
    public class MarkdownSpecFileListTests
    {
        [Fact]
        public void EmptyListTest()
        {
            Assert.Throws<ArgumentNullException>(() => MarkdownSpec.ReadFiles(null, new Reporter(TextWriter.Null)));
        }
    }
}