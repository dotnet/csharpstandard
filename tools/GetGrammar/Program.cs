/*************************************************************************
 * 
 * Rex Jaeschke, 2020-06-25
 * 
 * This is a VERY simple-minded tool that extracts ANTLR grammar productions from standard input
 * (typically a C# spec markdown file) and writes them to standard output, preceeding each one with a 
 * blank line.
 * 
 * By running this over all C# spec markdown files, each time appending standard output to the same file,
 * that file can then be used as the content for grammar.md.
 * 
 * The program is looking for sets of input lines having the following pattern:
 * 
 * ```ANTLR
 * ...
 * ```
 * 
 * where the opening fence can also be spelled in all lowercase (```antlr).
 * 
 * Any characters on an input line beyond the opening or closing fence characters (typically spaces) are ignored.
 * Opening and closing fences MUST begin in the first character position.
 * 
 */

using System;
using System.IO;
using System.Threading.Tasks;

namespace ExtractGrammar
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: GetGrammar <filename>");
                Environment.Exit(1);
            }
            using var inputFile = new StreamReader(args[0]);
            string section = "";

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
                Console.WriteLine();    // write out blank line before each new production
                Console.WriteLine($"// Source: §{section}");

                // This loop might be a candidate for a bit of refactoring.
                while (true)
                {
                    string? nextLine = await inputFile.ReadLineAsync();
                    if (nextLine == null)
                    {
                        Console.WriteLine("Unexpected EOF; no closing grammar fence");
                        Environment.Exit(1);
                    }
                    if (nextLine.Length < 3)   // Is it long enough to contain a closing fence?
                    {
                        Console.WriteLine(nextLine);
                    }
                    else if (nextLine.Substring(0, 3) == "```")    // If line starts with ```
                    {
                        //			Console.WriteLine("------ End of a production");
                        break;
                    }
                    else
                    {
                        Console.WriteLine(nextLine);
                    }
                }
            }
        }
    }
}
