using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StandardAnchorTags
{
    public class GenerateGrammar : IDisposable
    {
        private readonly string pathToStandardFiles;
        private readonly StreamWriter grammarStream;
        private const string LexicalHeader = 
@"# Annex A Grammar

**This clause is informative.**

## A.1 General

This annex contains the grammar productions found in the specification, including the optional ones for unsafe code. Productions appear here in the same order in which they appear in the specification.

## A.2 Lexical grammar

```ANTLR";

        private const string SyntacticHeader =
@"```

## A.3 Syntactic grammar

```ANTLR";

        private const string UnsafeExtensionsHeader =
@"```

## A.4 Grammar extensions for unsafe code

```ANTLR";

        private const string GrammarFooter =
@"```

**End of informative text.**
";

        public GenerateGrammar(string grammarPath, string pathToStandardFiles)
        {
            grammarStream = new StreamWriter(grammarPath, false);
            this.pathToStandardFiles = pathToStandardFiles;
        }

        public async Task WriteHeader() => await grammarStream.WriteLineAsync(GenerateGrammar.LexicalHeader);
        public async Task WriteSyntaxHeader() => await grammarStream.WriteLineAsync(GenerateGrammar.SyntacticHeader);
        public async Task WriteUnsafeExtensionHeader() => await grammarStream.WriteLineAsync(GenerateGrammar.UnsafeExtensionsHeader);

        public async Task WriteGrammarFooter()
        {
            await grammarStream.WriteLineAsync(GenerateGrammar.GrammarFooter);
            await grammarStream.FlushAsync();
            grammarStream.Close();
        }

        public async Task ExtractGrammarFrom(string inputFileName)
        {
            string inputFilePath = $"{pathToStandardFiles}/{inputFileName}";
            using var inputFile = new StreamReader(inputFilePath);
            string section = "";

            Console.OutputEncoding = Encoding.UTF8;

            while (await inputFile.ReadLineAsync() is string inputLine)
            {
                if (inputLine.StartsWith("#"))
                {
                    section = inputLine.Trim('#', ' ');
                    continue;
                }
                if (!inputLine.StartsWith("```ANTLR", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }
                //		Console.WriteLine("------ Start of a production");
                await grammarStream.WriteLineAsync();    // write out blank line before each new production
                await grammarStream.WriteLineAsync($"// Source: §{section}");

                // This loop might be a candidate for a bit of refactoring.
                while (true)
                {
                    string? nextLine = await inputFile.ReadLineAsync();
                    if (nextLine == null)
                    {
                        throw new InvalidOperationException("Unexpected EOF; no closing grammar fence");
                    }
                    if (nextLine.Length < 3)   // Is it long enough to contain a closing fence?
                    {
                        await grammarStream.WriteLineAsync(nextLine);
                    }
                    else if (nextLine.Substring(0, 3) == "```")    // If line starts with ```
                    {
                        //			Console.WriteLine("------ End of a production");
                        break;
                    }
                    else
                    {
                        await grammarStream.WriteLineAsync(nextLine);
                    }
                }
            }
        }

        public void Dispose() => grammarStream.Dispose();
    }
}
