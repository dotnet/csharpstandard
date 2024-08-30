using System.Text;

namespace StandardAnchorTags;

/// <summary>
/// The data storage for the headers of the grammar file
/// </summary>
/// <param name="LexicalHeader">The text for the lexical section header</param>
/// <param name="SyntacticHeader">The text for the syntactic section header</param>
/// <param name="UnsafeExtensionsHeader">The text for the unsafe extensions header</param>
/// <param name="GrammarFooter">The footer</param>
public record GrammarHeaders(string LexicalHeader, 
    string SyntacticHeader, 
    string UnsafeExtensionsHeader,
    string GrammarFooter);

/// <summary>
/// This class generates a grammar file from the ANTLR blocks in the standard
/// </summary>
/// <remarks>
/// The full grammar is in small pieces in each clause of the standard. Then,
/// its duplicated in an Annex. Rather than copy and paste by hand, generate that
/// Annex from the other smaller parts.
/// </remarks>
public class GenerateGrammar : IDisposable
{
    /// <summary>
    /// Read the existing headers from a grammar file
    /// </summary>
    /// <param name="pathToStandard">Path to the standard files (likely ../standard)</param>
    /// <param name="grammarFile">The filename for the grammar annex</param>
    /// <returns>The task that will return the headers when complete.</returns>
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

    /// <summary>
    /// Construct a new grammar generator
    /// </summary>
    /// <param name="grammarPath">The path to the file</param>
    /// <param name="pathToStandardFiles">The path to the files in the standard</param>
    /// <param name="headers">The header text</param>
    public GenerateGrammar(string grammarPath, string pathToStandardFiles, GrammarHeaders headers)
    {
        grammarStream = new StreamWriter(Path.Combine(pathToStandardFiles, grammarPath), false);
        this.pathToStandardFiles = pathToStandardFiles;
        informativeTextBlocks = headers;
    }

    /// <summary>
    /// Write the header text that appears before the grammar output.
    /// </summary>
    /// <returns>The task</returns>
    public async Task WriteHeader() => await grammarStream.WriteAsync(informativeTextBlocks.LexicalHeader);

    /// <summary>
    /// Write the header text for the syntactic section
    /// </summary>
    /// <returns>The task</returns>
    public async Task WriteSyntaxHeader() => await grammarStream.WriteAsync(informativeTextBlocks.SyntacticHeader);

    /// <summary>
    /// Write the header text for the unsafe extensions section
    /// </summary>
    /// <returns>The task</returns>
    public async Task WriteUnsafeExtensionHeader() => await grammarStream.WriteAsync(informativeTextBlocks.UnsafeExtensionsHeader);

    /// <summary>
    /// Write the footer text that appears after the grammar output.
    /// </summary>
    /// <returns>The task</returns>
    public async Task WriteGrammarFooter()
    {
        await grammarStream.WriteAsync(informativeTextBlocks.GrammarFooter);
        await grammarStream.FlushAsync();
        grammarStream.Close();
    }

    /// <summary>
    /// Extract the grammar from one file in the standard
    /// </summary>
    /// <param name="inputFileName">The input file from the standard</param>
    /// <returns>The task</returns>
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

    /// <summary>
    /// Dispose of the stream
    /// </summary>
    public void Dispose() => grammarStream.Dispose();
}
