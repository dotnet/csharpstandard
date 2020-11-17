// TODO:
//
// * Something goofy is going on with the TOC in the Word document. The field codes are broken, so it
//   isn't recognized as a field. And if I edit the field in Word (e.g. to add page numbers) then all
//   within-spec section links get broken.

using MarkdownConverter.Converter;
using MarkdownConverter.Grammar;
using MarkdownConverter.Spec;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownConverter
{
    static class Program
    {
        static int Main(string[] args)
        {
            // mdspec2docx *.md csharp.g4 template.docx -o spec.docx
            var ifiles = new List<string>();
            var ofiles = new List<string>();
            string argserror = "";
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.StartsWith("-"))
                {
                    if (arg == "-o" && i < args.Length - 1) { i++; ofiles.Add(args[i]); }
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
                    string dir = Path.GetDirectoryName(arg), filename = Path.GetFileName(arg);
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
            string ireadmefile = null, iantlrfile = null, idocxfile = null, odocfile = null;
            foreach (var ifile in ifiles)
            {
                var name = Path.GetFileName(ifile);
                var ext = Path.GetExtension(ifile).ToLower();
                if (ext == ".g4")
                {
                    if (iantlrfile != null)
                    {
                        argserror += "Multiple input .g4 files\n";
                    }
                    iantlrfile = ifile;
                }
                else if (ext == ".docx") { if (idocxfile != null) { argserror += "Multiple input .docx files\n"; } idocxfile = ifile; }
                else if (ext != ".md") { argserror += $"Not .g4 or .docx or .md '{ifile}'\n"; continue; }
                else if (String.Equals(name, "readme.md", StringComparison.InvariantCultureIgnoreCase)) { if (ireadmefile != null) { argserror += "Multiple readme.md files\n"; } ireadmefile = ifile; }
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

            if (ireadmefile == null && ifiles.Count == 0)
            {
                argserror += "No .md files supplied\n";
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
                Console.Error.WriteLine("If a .g4 is given, it verifies 1:1 correspondence with ```antlr blocks.");
                Console.Error.WriteLine("The -td temp directory will enable incremental (faster) runs in future.");
                return 1;
            }

            Console.WriteLine("Reading markdown files");

            // Read input file. If it contains a load of linked filenames, then read them instead.
            List<string> ifiles_in_order = new List<string>();
            List<Tuple<int, string, string, SourceLocation>> urls = null;
            if (ireadmefile == null)
            {
                ifiles_in_order.AddRange(imdfiles);
            }
            else if (ireadmefile != null && ifiles.Count == 0)
            {
                ifiles_in_order.Add(ireadmefile);
            }
            else
            {
                var readme = FSharp.Markdown.Markdown.Parse(File.ReadAllText(ireadmefile));
                urls = new List<Tuple<int, string, string, SourceLocation>>();
                // is there a nicer way to get the URLs of all depth-1 and depth-2 URLs in this list? ...
                foreach (var list in readme.Paragraphs.OfType<FSharp.Markdown.MarkdownParagraph.ListBlock>())
                {
                    var pp = new List<Tuple<int, FSharp.Markdown.MarkdownParagraph>>();
                    foreach (var pars in list.items)
                    {
                        foreach (var par in pars)
                        {
                            pp.Add(Tuple.Create(1, par));
                            var sublist = par as FSharp.Markdown.MarkdownParagraph.ListBlock;
                            if (sublist != null)
                            {
                                pp.AddRange(from subpars in sublist.items
                                            from subpar in subpars
                                            select Tuple.Create(2, subpar));
                            }
                        }
                    }
                    foreach (var tpp in pp)
                    {
                        var level = tpp.Item1;
                        var spanpar = tpp.Item2 as FSharp.Markdown.MarkdownParagraph.Span;
                        if (spanpar == null)
                        {
                            continue;
                        }

                        var links = spanpar.body.OfType<FSharp.Markdown.MarkdownSpan.DirectLink>();
                        urls.AddRange(from link in links
                                      let title = string.Join("", link.body.OfType<FSharp.Markdown.MarkdownSpan.Literal>().Select(l => l.text))
                                      let url = link.link
                                      where url.ToLower().EndsWith(".md") || url.ToLower().Contains(".md#")
                                      let loc = new SourceLocation(ireadmefile, null, list, link)
                                      select Tuple.Create(level, title, url, loc));
                    }
                }
                var filelinks = (from turl in urls
                                 let url = turl.Item3
                                 let i = url.IndexOf('#')
                                 let url2 = (i == -1 ? url : url.Substring(0, i))
                                 select url2).ToList().Distinct();
                foreach (var link in filelinks)
                {
                    var ifile = ifiles.FirstOrDefault(f => Path.GetFileName(f) == link);
                    if (ifile == null) { Console.Error.WriteLine($"readme.md link '{link}' wasn't one of the files passed on the command line"); return 1; }
                    ifiles_in_order.Add(ifile);
                }
            }
            var md = MarkdownSpec.ReadFiles(ifiles_in_order, urls);


            // Now md.Gramar contains the grammar as extracted out of the *.md files, and moreover has
            // correct references to within the spec. We'll check that it has the same productions as
            // in the corresponding ANTLR file
            if (iantlrfile != null)
            {
                Console.WriteLine($"Reading {Path.GetFileName(iantlrfile)}");
                var grammar = Antlr.ReadFile(iantlrfile);
                foreach (var diff in CompareGrammars(grammar, md.Grammar))
                {
                    if (diff.authority == null)
                    {
                        Report("MD21", "error", $"markdown has superfluous production '{diff.productionName}'", "mdspec2docx");
                    }
                    else if (diff.copy == null)
                    {
                        Report("MD22", "error", $"markdown lacks production '{diff.productionName}'", "mdspec2docx");
                    }
                    else
                    {
                        Report("MD23", "error", $"production '{diff.productionName}' differs between markdown and antlr.g4", "mdspec2docx");
                        Report("MD23b", "error", "antlr.g4 says " + diff.authority.Replace("\r", "\\r").Replace("\n", "\\n"), "mdspec2docx");
                        Report("MD23c", "error", "markdown says " + diff.copy.Replace("\r", "\\r").Replace("\n", "\\n"), "mdspec2docx");
                    }
                }
                foreach (var p in grammar.Productions)
                {
                    p.Link = md.Grammar.Productions.FirstOrDefault(mdp => mdp?.Name == p.Name)?.Link;
                    p.LinkName = md.Grammar.Productions.FirstOrDefault(mdp => mdp?.Name == p.Name)?.LinkName;
                }
            }

            // Generate the Specification.docx file
            if (odocfile != null)
            {
                var odocfile2 = odocfile;
                if (odocfile2 != odocfile)
                {
                    Report("MD26", "error", $"File '{odocfile}' was in use", "mdspec2docx");
                }

                Console.WriteLine($"Writing '{Path.GetFileName(odocfile2)}'");
                try
                {
                    MarkdownSpecConverter.ConvertToWord(md, idocxfile, odocfile2);
                }
                catch (Exception ex)
                {
                    Report("MD27", "error", ex.Message, ex.StackTrace);
                    return 1;
                }
                if (odocfile2 != odocfile)
                {
                    return 1;
                }
            }
            return 0;
        }

        static int ireport = 0;
        public static void Report(string code, string severity, string msg, string loc)
        {
            ireport++;
            Console.Error.WriteLine($"{loc}: {severity} {code}: {ireport:D3}. {msg}");
        }

        class ProductionDifference
        {
            public string productionName;
            public string authority, copy;
        }

        static IEnumerable<ProductionDifference> CompareGrammars(EbnfGrammar authority, EbnfGrammar copy)
        {
            Func<EbnfGrammar, Dictionary<string, Production>> ToDictionary;
            ToDictionary = g =>
            {
                var d = new Dictionary<string, Production>();
                foreach (var pp in g.Productions)
                {
                    if (pp.Name != null)
                    {
                        d[pp.Name] = pp;
                    }
                }

                return d;
            };
            var dauthority = ToDictionary(authority);
            var dcopy = ToDictionary(copy);

            foreach (var p in dauthority.Keys)
            {
                if (!dcopy.ContainsKey(p))
                {
                    continue;
                }

                Production pauthority0 = dauthority[p], pcopy0 = dcopy[p];
                string pauthority = Antlr.ToString(pauthority0), pcopy = Antlr.ToString(pcopy0);
                if (pauthority == pcopy)
                {
                    continue;
                }

                yield return new ProductionDifference { productionName = p, authority = pauthority, copy = pcopy };
            }

            foreach (var p in dauthority.Keys)
            {
                if (p == "start")
                {
                    continue;
                }

                if (!dcopy.ContainsKey(p))
                {
                    yield return new ProductionDifference { productionName = p, authority = "<defined>", copy = null };
                }
            }
            foreach (var p in dcopy.Keys)
            {
                if (p == "start")
                {
                    continue;
                }

                if (!dauthority.ContainsKey(p))
                {
                    yield return new ProductionDifference { productionName = p, authority = null, copy = "<defined>" };
                }
            }
        }
    }
}