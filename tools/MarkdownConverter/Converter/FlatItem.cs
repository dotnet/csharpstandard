using FSharp.Markdown;

namespace MarkdownConverter.Converter
{
    internal sealed class FlatItem
    {
        public int Level { get; }
        public bool HasBullet { get; }
        public bool IsBulletOrdered { get; }
        public MarkdownParagraph Paragraph { get; }

        public FlatItem(int level, bool hasBullet, bool isBulletOrdered, MarkdownParagraph paragraph)
        {
            Level = level;
            HasBullet = hasBullet;
            IsBulletOrdered = isBulletOrdered;
            Paragraph = paragraph;
        }
    }
}
