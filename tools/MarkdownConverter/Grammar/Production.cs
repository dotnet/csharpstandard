namespace MarkdownConverter.Grammar
{
    internal class Production
    {
        /// <summary>
        /// Optional.
        /// </summary>
        public EbnfNode Ebnf { get; set; }

        /// <summary>
        /// Whether or not this production is a fragment
        /// </summary>
        public bool Fragment { get; set; }

        /// <summary>
        /// Optional. Contains no whitespace and is not delimited by '
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional. Does not contain *) or newline
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// e.g. "Rule: \n | Choice1"
        /// </summary>
        public bool RuleStartsOnNewLine { get; set; }

        /// <summary>
        /// Optional link to the spec.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Optional name of the spec link.
        /// </summary>
        public string LinkName { get; set; }

        public override string ToString() => $"{(Fragment ? "fragment " : "")}{Name} := {Ebnf}";
    }
}
