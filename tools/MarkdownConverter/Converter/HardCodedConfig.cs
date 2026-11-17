using System.Collections.Immutable;

namespace MarkdownConverter.Converter;

/// <summary>
/// Aspects of configuration which are hard-coded for convenience, but could be extracted later.
/// </summary>
public static class HardCodedConfig
{
    // Long lines that are either known to actually be okay in the Word output, or
    // are acceptable and better than trying to break them. These do not issue warnings.
    public static ImmutableList<string> AcceptedLongLines = new List<string>
    {
        // expressions.md and grammar.md
        // These genuinely wrap, but that's better than breaking the lines.
        "    : null_conditional_member_access null_forgiving_operator? '(' argument_list? ')'",
        "    | null_conditional_element_access null_forgiving_operator? '(' argument_list? ')'",
        "    | 'stackalloc' unmanaged_type? '[' constant_expression? ']' stackalloc_initializer",
        // classes.md
        // This doesn't end up wrapping.
        "        // nonnullable base class requirement allows nonnullable class type argument",
    }.ToImmutableList();
}
