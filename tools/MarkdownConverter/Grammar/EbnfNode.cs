using System;
using System.Collections.Generic;

namespace MarkdownConverter.Grammar
{
    public sealed class EbnfNode
    {
        public EbnfKind Kind { get; set; }
        public string Text { get; set; }

        public List<EbnfNode> Children { get; set; }
        public string FollowingWhitespace { get; set; }

        /// <summary>
        /// Does not contain *) or newline
        /// </summary>
        public string FollowingComment { get; set; } = "";

        public bool FollowingNewline { get; set; }

        public override string ToString()
        {
            switch (Kind)
            {
                case EbnfKind.ExtendedTerminal:
                case EbnfKind.Reference:
                case EbnfKind.Terminal:
                    return Text;
                case EbnfKind.OneOrMoreOf:
                    return $"({Children[0]})+";
                case EbnfKind.ZeroOrMoreOf:
                    return $"({Children[0]})*";
                case EbnfKind.ZeroOrOneOf:
                    return $"({Children[0]})?";
                case EbnfKind.Choice:
                    return string.Join(" | ", Children);
                case EbnfKind.Sequence:
                    return string.Join(" ", Children);
                default:
                    throw new InvalidOperationException($"Unknown Kind: {Kind}");
            }
        }
    }
}
