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
            bool inProduction = false;

            Console.OutputEncoding = Encoding.UTF8;

            while (await inputFile.ReadLineAsync() is string inputLine)
            {
                if (inProduction)
                {
                    if (inputLine.StartsWith("```"))
                    {
                        inProduction = false;
                    }
                    else
                    {
                        await grammarStream.WriteLineAsync(inputLine);
                    }
                }
                else
                {
                    if (inputLine.StartsWith("#"))
                    {
                        section = inputLine.Trim('#', ' ');
                    }
                    else if (inputLine.StartsWith("```ANTLR", StringComparison.InvariantCultureIgnoreCase))
                    {
                        await grammarStream.WriteLineAsync();    // write out blank line before each new production
                        await grammarStream.WriteLineAsync($"// Source: §{section}");
                        inProduction = true;
                    }
                }
            }
        }

        public void Dispose() => grammarStream.Dispose();
    }
}
