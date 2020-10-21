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

namespace ExtractGrammar
{
    class Program
    {
	public static void Main()
	{
	    string inputLine;

	    while ((inputLine = Console.ReadLine()) != null)
	    {
		if (inputLine.Length < 8)	// Is it long enough to contain an opening fence?
                {
		    continue;
                }
		string leader = inputLine.Substring(0, 8);  // grab what might be an opening fence
//		Console.WriteLine(">" + leader + "<");
		if (leader != "```ANTLR" && leader != "```antlr")
		    {
			continue;
                }
//		Console.WriteLine("------ Start of a production");
		Console.WriteLine();	// write out blank line before each new production

		while (true)
		{
		    inputLine = Console.ReadLine();
		    if (inputLine == null)
		    {
			Console.WriteLine("Unexpected EOF; no closing grammar fence");
			Environment.Exit(1);
		    }
		    if (inputLine.Length < 3)   // Is it long enough to contain a closing fence?
		    {
			Console.WriteLine(inputLine);
		    }
		    else if (inputLine.Substring(0, 3) == "```")    // If line starts with ```
		    {
//			Console.WriteLine("------ End of a production");
			break;
		    }
		    else
                    {
			Console.WriteLine(inputLine);
		    }
		}
	    }
	}
    }
}
