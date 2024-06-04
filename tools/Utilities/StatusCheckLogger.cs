using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities;

public record Diagnostic(string Message, string Id, bool IsWarning);

/// <summary>
/// This class writes the status of the check to the console in the format GitHub supports
/// </summary>
/// <remarks>
/// For all of our tools, if all error and warning messages are formatted correctly, GitHub
/// will show those errors and warnings inline in the files tab for the PR. Let's format
/// them correctly.
/// </remarks>
/// <param name="toolName">The name of the tool that is running the check</param>
public class StatusCheckLogger(string toolName)
{
    /// <summary>
    /// Log a message in the format for GitHub to display it inline
    /// </summary>
    /// <param name="path">The path to the file, relative to the root of the repository</param>
    /// <param name="line">The line in the file (based on 1 as the first line)</param>
    /// <param name="col">The column in the file (based on 1 as the first column)</param>
    /// <param name="d">The diagnostic. Fields include a string ID, a message, and a flag to indicate a warning</param>
    public void LogCheck(string path, int line, int col, Diagnostic d) =>
        Console.WriteLine($"::{(d.IsWarning ? "⚠️" : "❌")} file={path},line={line},col={col}::{toolName}-{d.Id}: {d.Message}");
}
