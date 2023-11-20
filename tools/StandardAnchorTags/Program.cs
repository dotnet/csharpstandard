﻿using System;
using System.IO;
using System.Threading.Tasks;
using Utilities;
using System.Text.Json;

namespace StandardAnchorTags
{
    public class Program
    {
        const string TOCHeader = "<!-- The remaining text is generated by a tool. Do not hand edit -->";
        private const string PathToStandard = "../standard/";
        private const string ReadMePath = "../standard/README.md";
        private const string FilesPath = "../standard/clauses.json";
        private const string GrammarFile = "grammar.md";

        private static Clauses? standardClauses;

        static async Task<int> Main(string[] args)
        {
            using FileStream openStream = File.OpenRead(FilesPath);
            standardClauses = await JsonSerializer.DeserializeAsync<Clauses>(openStream);
            if (standardClauses is null)
            {
                Console.WriteLine("Could not read list of clauses. Exiting");
                return 1;
            }

            bool dryRun = ((args.Length > 0) && (args[0].Contains("dryrun")));
            if (dryRun)
            {
                Console.WriteLine("Doing a dry run");
            }

            try
            {
                Console.WriteLine("=========================== Front Matter ===================================");
                var sectionMap = new TocSectionNumberBuilder(PathToStandard, dryRun);
                foreach (var file in standardClauses.FrontMatter)
                {
                    Console.WriteLine($" -- {file}");
                    await sectionMap.AddFrontMatterTocEntries(file);
                }

                Console.WriteLine("================= GENERATE UPDATED SECTION NUMBERS =========================");
                Console.WriteLine("============================ Scope and Conformance ======================================");
                foreach (var file in standardClauses.ScopeAndConformance)
                {
                    Console.WriteLine($" -- {file}");
                    await sectionMap.AddContentsToTOC(file);

                }
                Console.WriteLine("============================ Lexical Structure ======================================");
                foreach (var file in standardClauses.LexicalStructure)
                {
                    Console.WriteLine($" -- {file}");
                    await sectionMap.AddContentsToTOC(file);
                }
                Console.WriteLine("============================ Main text======================================");
                foreach (var file in standardClauses.MainBody)
                {
                    Console.WriteLine($" -- {file}");
                    await sectionMap.AddContentsToTOC(file);
                }
                Console.WriteLine("============================ Unsafe clauses======================================");
                foreach (var file in standardClauses.UnsafeClauses)
                {
                    Console.WriteLine($" -- {file}");
                    await sectionMap.AddContentsToTOC(file);
                }
                Console.WriteLine("============================= Annexes ======================================");
                sectionMap.FinishMainSection();
                foreach (var file in standardClauses.Annexes)
                {
                    Console.WriteLine($" -- {file}");
                    await sectionMap.AddContentsToTOC(file);
                }
                if (!dryRun)
                {
                    Console.WriteLine("Update TOC");
                    var existingReadMe = await ReadExistingReadMe();
                    using var readme = new StreamWriter(ReadMePath, false);
                    await readme.WriteAsync(existingReadMe);
                    await readme.WriteLineAsync(TOCHeader);
                    await readme.WriteLineAsync();
                    await readme.WriteAsync(sectionMap.Toc);
                }

                Console.WriteLine("======================= UPDATE ALL REFERENCES ==============================");
                var fixup = new ReferenceUpdateProcessor(PathToStandard, sectionMap.LinkMap, dryRun);

                Console.WriteLine("=========================== Front Matter ===================================");
                foreach (var file in standardClauses.FrontMatter)
                {
                    Console.WriteLine($" -- {file}");
                    await fixup.ReplaceReferences(file);
                }
                Console.WriteLine("============================ Scope and Conformance ======================================");
                foreach (var file in standardClauses.ScopeAndConformance)
                {
                    Console.WriteLine($" -- {file}");
                    await fixup.ReplaceReferences(file);

                }
                Console.WriteLine("============================ Lexical Structure ======================================");
                foreach (var file in standardClauses.LexicalStructure)
                {
                    Console.WriteLine($" -- {file}");
                    await fixup.ReplaceReferences(file);
                }
                Console.WriteLine("============================ Main text======================================");
                foreach (var file in standardClauses.MainBody)
                {
                    Console.WriteLine($" -- {file}");
                    await fixup.ReplaceReferences(file);

                }
                Console.WriteLine("============================ Unsafe clauses======================================");
                foreach (var file in standardClauses.UnsafeClauses)
                {
                    Console.WriteLine($" -- {file}");
                    await fixup.ReplaceReferences(file);

                }
                Console.WriteLine("============================= Annexes ======================================");
                foreach (var file in standardClauses.Annexes)
                {
                    Console.WriteLine($" -- {file}");
                    await fixup.ReplaceReferences(file);
                }
                if (!dryRun)
                {
                    Console.WriteLine("======================= READ EXISTING GRAMMAR HEADERS =======================");
                    var headers = await GenerateGrammar.ReadExistingHeaders(PathToStandard, GrammarFile);

                    Console.WriteLine("======================= GENERATE GRAMMAR ANNEX ==============================");
                    using var grammarGenerator = new GenerateGrammar(GrammarFile, PathToStandard, headers);

                    Console.WriteLine("============================ Lexical Structure ======================================");

                    await grammarGenerator.WriteHeader();
                    foreach (var file in standardClauses.LexicalStructure)
                    {
                        Console.WriteLine($" -- {file}");
                        await grammarGenerator.ExtractGrammarFrom(file);
                    }
                    Console.WriteLine("============================ Main text======================================");

                    await grammarGenerator.WriteSyntaxHeader();
                    foreach (var file in standardClauses.MainBody)
                    {
                        Console.WriteLine($" -- {file}");
                        await grammarGenerator.ExtractGrammarFrom(file);
                    }
                    Console.WriteLine("============================ Unsafe clauses======================================");
                    await grammarGenerator.WriteUnsafeExtensionHeader();
                    foreach (var file in standardClauses.UnsafeClauses)
                    {
                        Console.WriteLine($" -- {file}");
                        await grammarGenerator.ExtractGrammarFrom(file);
                    }
                    await grammarGenerator.WriteGrammarFooter();
                }
                return fixup.ErrorCount;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("\tError encountered:");
                Console.WriteLine(e.Message.ToString());
                Console.WriteLine("To recover, do the following:");
                Console.WriteLine("1. Discard all changes from the section numbering tool");
                Console.WriteLine("2. Fix the error noted above.");
                Console.WriteLine("3. Run the tool again.");
                return 1;
            }
        }

        private static async Task<string> ReadExistingReadMe()
        {
            using var reader = new StreamReader(ReadMePath);
            var contents = await reader.ReadToEndAsync();

            // This is the first node in the TOC, so truncate here:
            var index = contents.IndexOf(TOCHeader);

            return contents[..index];
        }
    }
}