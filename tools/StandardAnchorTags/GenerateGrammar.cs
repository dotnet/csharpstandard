using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StandardAnchorTags;

public record GrammarHeaders(string LexicalHeader, 
    string SyntacticHeader, 
    string UnsafeExtensionsHeader,
    string GrammarFooter);

public class GenerateGrammar : IDisposable
{
    public static async Task<GrammarHeaders> ReadExistingHeaders(string pathToStandard, string grammarFile)
    {
        GrammarHeaders headers = new GrammarHeaders("", "", "", "");
        StringBuilder headerBuffer = new StringBuilder();

        using var reader = new StreamReader(Path.Combine(pathToStandard, grammarFile));

        while (await reader.ReadLineAsync() is string inputLine)
        {
            headerBuffer.AppendLine(inputLine);
            if (inputLine.StartsWith("```ANTLR"))
            {
                if (string.IsNullOrWhiteSpace(headers.LexicalHeader))
                {
                    headers = headers with { LexicalHeader = headerBuffer.ToString() };
                }
                else if (string.IsNullOrWhiteSpace(headers.SyntacticHeader))
                {
                    headers = headers with { SyntacticHeader = headerBuffer.ToString() };
                }
                else if (string.IsNullOrWhiteSpace(headers.UnsafeExtensionsHeader))
                {
                    headers = headers with { UnsafeExtensionsHeader = headerBuffer.ToString() };
                }
            } else if (inputLine.StartsWith("```"))
            {
                headerBuffer.Clear();
                // Put the closing tag back:
                headerBuffer.AppendLine(inputLine);
            }
        }
        headers = headers with { GrammarFooter = headerBuffer.ToString() };
        reader.Close();

        return headers;
    }

    private readonly GrammarHeaders informativeTextBlocks;
    private readonly string pathToStandardFiles;
    private readonly StreamWriter grammarStream;

    public GenerateGrammar(string grammarPath, string pathToStandardFiles, GrammarHeaders headers)
    {
        grammarStream = new StreamWriter(Path.Combine(pathToStandardFiles, grammarPath), false);
        this.pathToStandardFiles = pathToStandardFiles;
        informativeTextBlocks = headers;
    }

    public async Task WriteHeader() => await grammarStream.WriteAsync(informativeTextBlocks.LexicalHeader);
    public async Task WriteSyntaxHeader() => await grammarStream.WriteAsync(informativeTextBlocks.SyntacticHeader);
    public async Task WriteUnsafeExtensionHeader() => await grammarStream.WriteAsync(informativeTextBlocks.UnsafeExtensionsHeader);

    public async Task WriteGrammarFooter()
    {
        await grammarStream.WriteLineAsync(informativeTextBlocks.GrammarFooter);
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
