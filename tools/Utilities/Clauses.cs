namespace Utilities
{
    public class Clauses
    {
        public string[] FrontMatter { get; set; } = Array.Empty<string>();

        public string[] ScopeAndConformance {  get; set; } = Array.Empty<string>();

        public string[] LexicalStructure { get; set; } = Array.Empty<string>();
        public string[] MainBody { get; set; } = Array.Empty<string>();

        // Sure, there's only one, but let's allow for possible expansion.
        public string[] UnsafeClauses { get; set; } = Array.Empty<string>();
        public string[] Annexes { get; set; } = Array.Empty<string>();
    }
}