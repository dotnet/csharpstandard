// TODO:
//
// * Something goofy is going on with the TOC in the Word document. The field codes are broken, so it
//   isn't recognized as a field. And if I edit the field in Word (e.g. to add page numbers) then all
//   within-spec section links get broken.

using MarkdownConverter.Converter;
using MarkdownConverter.Spec;

namespace MarkdownConverter;

static class Program
{
    static async Task<int> Main(string[] args)
    {
        // mdspec2docx *.md csharp.g4 template.docx -o spec.docx
        var ifiles = new List<string>();
        var ofiles = new List<string>();
        var head_sha = Environment.GetEnvironmentVariable("HEAD_SHA");
        var token = Environment.GetEnvironmentVariable("GH_TOKEN");
        string argserror = "";
        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg.StartsWith("-"))
            {
                if (arg == "-o" && i < args.Length - 1) { i++; ofiles.Add(args[i]); }
                else if (arg == "-s" && i < args.Length - 1) {  i++; head_sha = args[i]; }
                else if (arg.StartsWith("-")) { argserror += $"Unrecognized '{arg}'\n"; }
                else
                {
                    argserror += $"Unrecognized '{arg}'\n";
                }
            }
            else if (!arg.Contains("*") && !arg.Contains("?"))
            {
                if (!File.Exists(arg)) { Console.Error.WriteLine($"Not found - {arg}"); return 1; }
                ifiles.Add(arg);
            }
            else
            {
                // Windows command-shell doesn't do globbing, so we have to do it ourselves
                string dir = Path.GetDirectoryName(arg) ?? throw new ArgumentException($"'{arg}' is a root directory");
                string filename = Path.GetFileName(arg);
                if (dir.Contains("*") || dir.Contains("?"))
                {
                    Console.Error.WriteLine("Can't match wildcard directory names");
                    return 1;
                }
                if (dir == "")
                {
                    dir = Directory.GetCurrentDirectory();
                }

                if (!Directory.Exists(dir)) { Console.Error.WriteLine($"Not found - \"{dir}\""); return 1; }
                var fns2 = Directory.GetFiles(dir, filename);
                if (fns2.Length == 0) { Console.Error.WriteLine($"Not found - \"{arg}\""); return 1; }
                ifiles.AddRange(fns2);
            }
        }

        var imdfiles = new List<string>();
        string? idocxfile = null;
        string? odocfile = null;
        foreach (var ifile in ifiles)
        {
            var name = Path.GetFileName(ifile);
            // Ignore the README, which is maintained separately and not part of the Standard.
            if (name.Equals("README.md", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            var ext = Path.GetExtension(ifile).ToLower();
            if (ext == ".docx") { if (idocxfile != null) { argserror += "Multiple input .docx files\n"; } idocxfile = ifile; }
            else if (ext != ".md") { argserror += $"Not .g4 or .docx or .md '{ifile}'\n"; continue; }
            else { imdfiles.Add(ifile); }
        }
        foreach (var ofile in ofiles)
        {
            var ext = Path.GetExtension(ofile).ToLower();
            if (ext == ".docx")
            {
                if (odocfile != null)
                {
                    argserror += "Multiple output docx files\n";
                }
                odocfile = ofile;
            }
            else
            {
                argserror += $"Unknown output file extension: {ext}\n";
            }
        }

        if (odocfile == null)
        {
            argserror += "No output .docx file specified\n";
        }

        if (idocxfile == null)
        {
            argserror += "No template.docx supplied\n";
        }

        if (argserror != "")
        {
            Console.Error.WriteLine(argserror);
            Console.Error.WriteLine("mdspec2docx *.md grammar.g4 template.docx -o spec.docx");
            Console.Error.WriteLine();
            Console.Error.WriteLine("Turns the markdown files into a word document based on the template.");
            Console.Error.WriteLine("If readme.md and other files are given, then readme is used solely to");
            Console.Error.WriteLine("   sort the docx based on its list of `* [Link](subfile.md)`.");
            return 1;
        }

        Console.WriteLine("Reading markdown files");

        var reporter = new Reporter();

        // Read input file. If it contains a load of linked filenames, then read them instead.
        var md = MarkdownSpec.ReadFiles(imdfiles, reporter);

        // Generate the Specification.docx file
        if (odocfile != null)
        {
            var odocfile2 = odocfile;
            if (odocfile2 != odocfile)
            {
                reporter.Error("MD26", $"File '{odocfile}' was in use");
            }

            Console.WriteLine($"Writing '{Path.GetFileName(odocfile2)}'");
            try
            {
                MarkdownSpecConverter.ConvertToWord(md, idocxfile!, odocfile2, reporter);
            }
            catch (Exception ex)
            {
                reporter.Error("MD27", ex.ToString());
                return 1;
            }
            if (odocfile2 != odocfile)
            {
                return 1;
            }
        }
        Console.WriteLine($"Errors: {reporter.Errors}");
        Console.WriteLine($"Warnings: {reporter.Warnings}");
        if ((head_sha is not null) &&
            (token is not null))
        {
            Console.WriteLine("Submitting check run");
            await reporter.WriteCheckStatus(token, head_sha);

        }
        return reporter.Errors == 0 ? 0 : 1;
    }
}
