using CSharp2Colorized;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownConverter.Grammar
{
    internal static class Antlr
    {
        // This is used to replace "\u" in the grammar, so that we can turn it back into "\u" later.
        // (By default, the backslash would be unescaped...)
        private const string UnicodeEscapeSequencePlaceholder = "\u016f";

        public static string ToString(EbnfGrammar grammar)
        {
            var r = $"grammar {grammar.Name};\r\n";
            foreach (var p in grammar.Productions)
            {
                r += ToString(p);
            }

            return r;
        }

        public static string ToString(Production p)
        {
            if (p.Ebnf == null && string.IsNullOrEmpty(p.Comment))
            {
                return "\r\n";
            }
            else if (p.Ebnf == null)
            {
                return $"//{p.Comment}\r\n";
            }
            else
            {
                var r = $"{p.Name}:";
                if (p.RuleStartsOnNewLine)
                {
                    r += "\r\n";
                }

                r += "\t";
                if (p.RuleStartsOnNewLine)
                {
                    r += "| ";
                }

                r += $"{ToString(p.Ebnf)};";
                if (!string.IsNullOrEmpty(p.Comment))
                {
                    r += $"  //{p.Comment}";
                }

                r += "\r\n";
                return r;
            }
        }

        public static string ToString(EbnfNode node)
        {
            var r = "";
            EbnfNode prevElement = null;
            switch (node.Kind)
            {
                case EbnfKind.Terminal:
                    r = $"'{node.Text.Replace("\\", "\\\\").Replace("'", "\\'")}'";
                    break;
                case EbnfKind.ExtendedTerminal:
                    r = $"'<{node.Text.Replace("\\", "\\\\").Replace("'", "\\'")}>'";
                    break;
                case EbnfKind.Reference:
                    r = node.Text;
                    break;
                case EbnfKind.OneOrMoreOf:
                case EbnfKind.ZeroOrMoreOf:
                case EbnfKind.ZeroOrOneOf:
                    var op = (node.Kind == EbnfKind.OneOrMoreOf ? "+" : (node.Kind == EbnfKind.ZeroOrMoreOf ? "*" : "?"));
                    if (node.Children[0].Kind == EbnfKind.Choice || node.Children[0].Kind == EbnfKind.Sequence)
                    {
                        r = $"( {ToString(node.Children[0])} ){op}";
                    }
                    else
                    {
                        r = $"{ToString(node.Children[0])}{op}";
                    }

                    break;
                case EbnfKind.Choice:
                    foreach (var c in node.Children)
                    {
                        if (prevElement != null)
                        {
                            r += (r.Last() == '\t' ? "| " : " | ");
                        }

                        r += ToString(c);
                        prevElement = c;
                    }
                    break;
                case EbnfKind.Sequence:
                    foreach (var c in node.Children)
                    {
                        if (prevElement != null)
                        {
                            r += (r == "" || r.Last() == '\t' ? "" : " ");
                        }

                        if (c.Kind == EbnfKind.Choice)
                        {
                            r += "( " + ToString(c) + " )";
                        }
                        else
                        {
                            r += ToString(c);
                        }

                        prevElement = c;
                    }
                    break;
                default:
                    r = "???";
                    break;
            }
            if (!string.IsNullOrEmpty(node.FollowingComment))
            {
                r += " //" + node.FollowingComment;
            }

            if (node.FollowingNewline)
            {
                r += "\r\n\t";
            }

            return r;
        }


        public static IEnumerable<ColorizedLine> ColorizeAntlr(string antlr)
        {
            var grammar = ReadString(antlr, "dummyGrammarName");
            return Colorize.Words2Lines(ColorizeAntlr(grammar));
        }

        private static IEnumerable<ColorizedWord> ColorizeAntlr(EbnfGrammar grammar)
        {
            foreach (var p in grammar.Productions)
            {
                foreach (var word in ColorizeAntlr(p))
                {
                    yield return word;
                }
            }
        }

        private static IEnumerable<ColorizedWord> ColorizeAntlr(Production p)
        {
            if (p.Ebnf == null && string.IsNullOrEmpty(p.Comment))
            {
                yield return null;
            }
            else if (p.Ebnf == null)
            {
                yield return Col("// " + p.Comment, "Comment");
                yield return null;
            }
            else
            {
                yield return Col(p.Name, "Production");
                yield return Col(":", "PlainText");
                if (p.RuleStartsOnNewLine) { yield return null; yield return Col("\t| ", "PlainText"); }
                else
                {
                    yield return Col(" ", "PlainText");
                }

                foreach (var word in ColorizeAntlr(p.Ebnf))
                {
                    yield return word;
                }

                yield return Col(";", "PlainText");
                if (!string.IsNullOrEmpty(p.Comment))
                {
                    yield return Col("  //" + p.Comment, "Comment");
                }

                yield return null;
            }
        }

        public static IEnumerable<ColorizedWord> ColorizeAntlr(EbnfNode node)
        {
            var lastWasTab = false;
            EbnfNode prevElement = null;
            switch (node.Kind)
            {
                case EbnfKind.Terminal:
                    yield return Col("'" + node.Text.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\\\"", "\"").Replace(UnicodeEscapeSequencePlaceholder, "\\u") + "'", "Terminal");
                    break;
                case EbnfKind.ExtendedTerminal:
                    yield return Col(node.Text, "ExtendedTerminal");
                    break;
                case EbnfKind.Reference:
                    yield return Col(node.Text, "Production");
                    break;
                case EbnfKind.OneOrMoreOf:
                case EbnfKind.ZeroOrMoreOf:
                case EbnfKind.ZeroOrOneOf:
                    var op = (node.Kind == EbnfKind.OneOrMoreOf ? "+" : (node.Kind == EbnfKind.ZeroOrMoreOf ? "*" : "?"));
                    if (node.Children[0].Kind == EbnfKind.Choice || node.Children[0].Kind == EbnfKind.Sequence)
                    {
                        yield return Col("( ", "PlainText");
                        foreach (var word in ColorizeAntlr(node.Children[0]))
                        {
                            yield return word;
                        }

                        yield return Col(" )", "PlainText");
                        yield return Col(op, "PlainText");
                    }
                    else
                    {
                        foreach (var word in ColorizeAntlr(node.Children[0]))
                        {
                            yield return word;
                        }

                        yield return Col(op, "PlainText");
                    }
                    break;
                case EbnfKind.Choice:
                    foreach (var c in node.Children)
                    {
                        if (prevElement != null)
                        {
                            yield return Col(lastWasTab ? "| " : "| ", "PlainText");
                        }

                        foreach (var word in ColorizeAntlr(c)) { yield return word; lastWasTab = (word?.Text == "\t"); }
                        prevElement = c;
                    }
                    break;
                case EbnfKind.Sequence:
                    foreach (var c in node.Children)
                    {
                        if (lastWasTab)
                        {
                            yield return Col("  ", "PlainText");
                        }

                        if (c.Kind == EbnfKind.Choice)
                        {
                            yield return Col("( ", "PlainText");
                            foreach (var word in ColorizeAntlr(c))
                            {
                                yield return word;
                            }

                            yield return Col(" )", "PlainText");
                            lastWasTab = false;
                        }
                        else
                        {
                            foreach (var word in ColorizeAntlr(c)) { yield return word; lastWasTab = (word?.Text == "\t"); }
                        }
                        prevElement = c;
                    }
                    break;
                default:
                    throw new NotSupportedException("Unrecognized Ebnf");
            }
            if (!string.IsNullOrEmpty(node.FollowingWhitespace))
            {
                yield return Col(node.FollowingWhitespace, "Comment");
            }

            if (!string.IsNullOrEmpty(node.FollowingComment))
            {
                yield return Col(" //" + node.FollowingComment, "Comment");
            }

            if (node.FollowingNewline) { yield return null; yield return Col("\t", "PlainText"); }
        }

        private static ColorizedWord Col(string token, string color)
        {
            switch (color)
            {
                case "PlainText": return new ColorizedWord { Text = token };
                case "Production": return new ColorizedWord { Text = token, Red = 106, Green = 90, Blue = 205 };
                case "Comment": return new ColorizedWord { Text = token, Green = 128 };
                case "Terminal": return new ColorizedWord { Text = token, Red = 163, Green = 21, Blue = 21 };
                case "ExtendedTerminal": return new ColorizedWord { Text = token, IsItalic = true };
                default: throw new Exception("bad color name");
            }
        }

        public static EbnfGrammar ReadFile(string fn)
        {
            return ReadString(File.ReadAllText(fn), Path.GetFileNameWithoutExtension(fn));
        }

        public static EbnfGrammar ReadString(string src, string grammarName)
        {
            return new EbnfGrammar { Productions = ReadInternal(src).ToList(), Name = grammarName };
        }

        private static IEnumerable<Production> ReadInternal(string src)
        {
            var tokens = Tokenize(src);
            while (tokens.Any())
            {
                var t = tokens.First.Value; tokens.RemoveFirst();
                if (t == "grammar")
                {
                    while (tokens.Any() && tokens.First.Value != ";")
                    {
                        tokens.RemoveFirst();
                    }

                    if (tokens.Any() && tokens.First.Value == ";")
                    {
                        tokens.RemoveFirst();
                    }

                    if (tokens.Any() && tokens.First.Value == "\r\n")
                    {
                        tokens.RemoveFirst();
                    }
                }
                else if (t.StartsWith("//"))
                {
                    yield return new Production { Comment = t.Substring(2) };
                    if (tokens.Any() && tokens.First.Value == "\r\n")
                    {
                        tokens.RemoveFirst();
                    }
                }
                else if (t == "\r\n")
                {
                    yield return new Production();
                }
                else if (string.IsNullOrWhiteSpace(t))
                {
                    // skip
                }
                else
                {
                    var whitespace = "";
                    var comment = "";
                    var newline = false;
                    while (tokens.Any() && string.IsNullOrWhiteSpace(tokens.First.Value))
                    {
                        if (tokens.First.Value == "\r\n")
                        {
                            newline = true;
                        }

                        tokens.RemoveFirst();

                    }
                    if (tokens.First.Value != ":")
                    {
                        throw new Exception($"After '{t}' expected ':' not {tokens.First.Value}");
                    }

                    tokens.RemoveFirst();
                    GobbleUpComments(tokens, ref whitespace, ref comment, ref newline);
                    var p = ParseProduction(tokens, ref whitespace, ref comment);
                    GobbleUpComments(tokens, p);
                    if (tokens.Any() && tokens.First.Value == ";")
                    {
                        tokens.RemoveFirst();
                    }

                    if (tokens.Any() && tokens.First.Value == "\r\n")
                    {
                        tokens.RemoveFirst();
                    }

                    var production = new Production { Comment = comment, Ebnf = p, Name = t, RuleStartsOnNewLine = newline };
                    while (tokens.Any() && tokens.First.Value.StartsWith("//"))
                    {
                        production.Comment += tokens.First.Value.Substring(2); tokens.RemoveFirst();
                        if (tokens.First.Value == "\r\n")
                        {
                            tokens.RemoveFirst();
                        }
                    }
                    yield return production;
                }
            }
        }

        private static LinkedList<string> Tokenize(string s)
        {
            s = s.Trim();
            var tokens = new LinkedList<String>();
            var pos = 0;

            while (pos < s.Length)
            {
                if (pos + 1 < s.Length && s.Substring(pos, 2) == "\r\n")
                {
                    tokens.AddLast("\r\n"); pos += 2;
                }
                else if (s.Substring(pos, 1) == "\r" || s.Substring(pos, 1) == "\n")
                {
                    tokens.AddLast("\r\n"); pos += 1;
                }
                else if (":*?|+;()".Contains(s[pos]))
                {
                    tokens.AddLast(s[pos].ToString()); pos++;
                }
                else if (pos + 1 < s.Length && s.Substring(pos, 2) == "//")
                {
                    pos += 2;
                    var t = "";
                    while (pos < s.Length && !"\r\n".Contains(s[pos])) { t += s[pos]; pos += 1; }
                    if (t.Contains("*)"))
                    {
                        throw new Exception("Comments may not include *)");
                    }

                    tokens.AddLast("//" + t);
                }
                else if (s[pos] == '\'')
                {
                    var t = ""; pos++;
                    while (pos < s.Length && s.Substring(pos, 1) != "'")
                    {
                        if (pos == s.Length - 1)
                        {
                            throw new Exception($"Ran out of text parsing terminal {t}");
                        }
                        if (s.Substring(pos, 2) == "\\\\") { t += "\\"; pos += 2; }
                        else if (s.Substring(pos, 2) == "\\'") { t += "'"; pos += 2; }
                        else if (s.Substring(pos, 2) == "\\\"") { t += "\""; pos += 2; }
                        // Replace "\u" with a placeholder so we can get back to just "\u" later.
                        else if (s.Substring(pos, 2) == "\\u" && pos + 6 <= s.Length) { t += UnicodeEscapeSequencePlaceholder + s.Substring(pos + 2, 4);  pos += 6; }
                        else if (s.Substring(pos, 1) == "\\")
                        {
                            throw new Exception($@"Terminals may not include \ except in \\ or \' or \"". Error at {t}\ while tokenizing {s}");
                        }
                        else { t += s[pos]; pos++; }
                    }
                    if (t.Contains("\r") || t.Contains("\n"))
                    {
                        throw new Exception($"Terminals must be single-line. Actual terminal: {t}");
                    }

                    tokens.AddLast("'" + t + "'"); pos++;
                }
                else
                {
                    var t = "";
                    while (pos < s.Length && !string.IsNullOrWhiteSpace(s[pos].ToString())
                        && !":*?;\r\n'()+".Contains(s[pos]) && (pos + 1 >= s.Length || s.Substring(pos, 2) != "//"))
                    {
                        t += s[pos]; pos++;
                    }
                    // Allow a trailing () for Antlr functions
                    if (pos + 2 <= s.Length && s.Substring(pos, 2) == "()")
                    {
                        t += "()";
                        pos += 2;
                    }
                    tokens.AddLast(t);
                }
                // Bump up to the next non-whitespace character:
                var whitespace = "";
                while (pos < s.Length && !"\r\n".Contains(s[pos]) && string.IsNullOrWhiteSpace(s[pos].ToString()))
                {
                    whitespace += s[pos]; pos++;
                }
                if (whitespace != "")
                {
                    tokens.AddLast(whitespace);
                }
            }

            return tokens;
        }

        private static void GobbleUpComments(LinkedList<string> tokens, EbnfNode node)
        {
            string followingWhitespace = node.FollowingWhitespace;
            string followingComment = node.FollowingComment;
            bool followingNewline = node.FollowingNewline;
            GobbleUpComments(tokens, ref followingWhitespace, ref followingComment, ref followingNewline);
            node.FollowingWhitespace = followingWhitespace;
            node.FollowingComment = followingComment;
            node.FollowingNewline = followingNewline;
        }

        private static void GobbleUpComments(LinkedList<string> tokens, ref string extraWhitespace, ref string extraComments, ref bool hasNewline)
        {
            if (tokens.Count == 0)
            {
                return;
            }

            while (true)
            {
                if (tokens.First.Value.StartsWith("//"))
                {
                    extraComments += tokens.First.Value.Substring(2); tokens.RemoveFirst();
                }
                else if (tokens.First.Value == "\r\n")
                {
                    hasNewline = true;
                    tokens.RemoveFirst();
                    if (extraComments.Length > 0)
                    {
                        extraComments += " ";
                    }
                }
                else if (string.IsNullOrWhiteSpace(tokens.First.Value))
                {
                    extraWhitespace += tokens.First.Value; tokens.RemoveFirst();
                }
                else
                {
                    break;
                }
            }
            extraComments = extraComments.TrimEnd();
        }

        static bool dummy;

        private static EbnfNode ParseProduction(LinkedList<string> tokens, ref string ExtraWhitespace, ref string ExtraComments)
        {
            if (tokens.Count == 0)
            {
                throw new Exception("empty input stream");
            }

            GobbleUpComments(tokens, ref ExtraWhitespace, ref ExtraComments, ref dummy);
            return ParsePar(tokens, ref ExtraWhitespace, ref ExtraComments);
        }

        private static EbnfNode ParsePar(LinkedList<string> tokens, ref string extraWhitespace, ref string extraComments)
        {
            var pp = new LinkedList<EbnfNode>();
            if (tokens.First.Value == "|") { tokens.RemoveFirst(); GobbleUpComments(tokens, ref extraWhitespace, ref extraComments, ref dummy); }
            pp.AddLast(ParseSeq(tokens, ref extraWhitespace, ref extraComments));
            while (tokens.Any() && tokens.First.Value == "|")
            {
                tokens.RemoveFirst();
                var node = pp.Last.Value;
                bool followingNewline = node.FollowingNewline;
                GobbleUpComments(tokens, ref extraWhitespace, ref extraComments, ref followingNewline);
                node.FollowingNewline = followingNewline;
                pp.AddLast(ParseSeq(tokens, ref extraWhitespace, ref extraComments));
            }
            if (pp.Count == 1)
            {
                return pp.First.Value;
            }

            return new EbnfNode { Kind = EbnfKind.Choice, Children = pp.ToList() };
        }

        private static EbnfNode ParseSeq(LinkedList<string> tokens, ref string extraWhitespace, ref string extraComments)
        {
            var pp = new LinkedList<EbnfNode>();
            pp.AddLast(ParseUnary(tokens, ref extraWhitespace, ref extraComments));
            while (tokens.Any() && tokens.First.Value != "|" && tokens.First.Value != ";" && tokens.First.Value != ")")
            {
                var node = pp.Last.Value;
                bool followingNewline = node.FollowingNewline;
                GobbleUpComments(tokens, ref extraWhitespace, ref extraComments, ref followingNewline);
                node.FollowingNewline = followingNewline;
                pp.AddLast(ParseUnary(tokens, ref extraWhitespace, ref extraComments));
            }
            if (pp.Count == 1)
            {
                return pp.First.Value;
            }

            return new EbnfNode { Kind = EbnfKind.Sequence, Children = pp.ToList() };
        }

        private static EbnfNode ParseUnary(LinkedList<string> tokens, ref string extraWhitespace, ref string extraComments)
        {
            var p = ParseAtom(tokens, ref extraWhitespace, ref extraComments);
            while (tokens.Any())
            {
                if (tokens.First.Value == "+")
                {
                    tokens.RemoveFirst();
                    p = new EbnfNode { Kind = EbnfKind.OneOrMoreOf, Children = new[] { p }.ToList() };
                    GobbleUpComments(tokens, p);
                }
                else if (tokens.First.Value == "*")
                {
                    tokens.RemoveFirst();
                    p = new EbnfNode { Kind = EbnfKind.ZeroOrMoreOf, Children = new[] { p }.ToList() };
                    GobbleUpComments(tokens, p);
                }
                else if (tokens.First.Value == "?")
                {
                    tokens.RemoveFirst();
                    p = new EbnfNode { Kind = EbnfKind.ZeroOrOneOf, Children = new[] { p }.ToList() };
                    GobbleUpComments(tokens, p);
                }
                else
                {
                    break;
                }
            }
            return p;
        }

        private static EbnfNode ParseAtom(LinkedList<string> tokens, ref string ExtraWhitespace, ref string ExtraComments)
        {
            if (tokens.First.Value == "(")
            {
                tokens.RemoveFirst();
                var p = ParseProduction(tokens, ref ExtraWhitespace, ref ExtraComments);
                if (tokens.Count == 0 || tokens.First.Value != ")")
                {
                    throw new Exception($"mismatched parentheses in {string.Join(", ", tokens)}");
                }

                tokens.RemoveFirst();
                GobbleUpComments(tokens, p);
                return p;
            }
            else if (tokens.First.Value.StartsWith("'"))
            {
                var t = tokens.First.Value; tokens.RemoveFirst();
                t = t.Substring(1, t.Length - 2);
                var p = new EbnfNode { Kind = EbnfKind.Terminal, Text = t };
                if (t.StartsWith("<") && t.EndsWith(">"))
                {
                    p.Kind = EbnfKind.ExtendedTerminal;
                    p.Text = t.Substring(1, t.Length - 2);
                    if (p.Text.Contains("?"))
                    {
                        throw new Exception("A special-terminal may not contain a question-mark '?'");
                    }
                    if (p.Text == "")
                    {
                        throw new Exception("A terminal may not be '<>'");
                    }
                }
                else
                {
                    if (t.Contains("'") && t.Contains("\""))
                    {
                        throw new Exception("A terminal must either contain no ' or no \"");
                    }
                }
                GobbleUpComments(tokens, p);
                return p;
            }
            else
            {
                var t = tokens.First.Value; tokens.RemoveFirst();
                var p = new EbnfNode { Kind = EbnfKind.Reference, Text = t };
                GobbleUpComments(tokens, p);
                return p;
            }
        }

    }
}