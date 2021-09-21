# C# language standard

Working space for [ECMA-TC49-TG2](https://www.ecma-international.org/task-groups/tc49-tg2/), the C# standard committee.

- The text is licensed under the [Creative Commons license](LICENSE).
- The code for our tools is licensed under the [MIT license](LICENSE-CODE).

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community. For more information, see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## C# Language Specification

### C# 6.0 draft

The branch `draft-v6` has the draft text for C# 6.0. It has not been submitted as a formal standard to ECMA. This version is a working draft that contains the features for C# 6.0.

### C# 5.0 standard

The branch `standard-v5` has the ECMA C# 5.0 standard text, converted to Markdown. For the official standard, see the [ECMA site](https://www.ecma-international.org/publications-and-standards/standards/ecma-334/).

This version is stored in this branch as a base markdown version to compare with future updated standard texts.

<!--
(This document is also available for download: [csharp.pdf](CSharp%20Language%20Specification.pdf?raw=true) and [csharp.docx](CSharp%20Language%20Specification.docx?raw=true))
-->

## Tools folder

This folder contains tools related to maintaining and converting the ECMA C# spec (ECMA-354).

### GetGrammar

This folder contains an ANTLR grammar-extraction tool and support files.

- ExtractGrammar.exe - the simple-minded grammar-extraction program. It processes only *one* md file.

- GetGrammar.bat - the Windows batch file that invokes ExtractGrammar on each md file of the C# specification that contains ANTLR grammar blocks, in clause order, inserting some md headers and such along the way. The result is a file called grammar.md, **which is a direct replacement for that file in the specification repo**.

> A minor wart: There is an extraneous blank line at the beginning of each of the lexical, syntactic, and unsafe grammars. At a glance, the amount of programming effort probably needed to stop this from happening seems to be *huge* compared with simply deleting those three lines manually. 

### MarkdownConverter

This tool is used by the committee to produce a Word format of the standard for submission to ECMA or ISO. This is run on each PR to ensure we can always produce the correct format when needed.

### StandardAnchorTags

This tool creates the outline using section numbers, and updates all links to the correct section number. Its purpose is to ensure that all references continue to point to the correct section, and that the table of contents shows the correct section numbers for all sections.

Contributors that add sections should follow the guidance in our [contributor guide](CONTRIBUTING.md#how-to-add-or-remove-clauses) to ensure that links to new sections are incorporated correctly. This tool is run on each PR in a `dry-run` mode to ensure that the changes will parse correctly. When a PR is merged, the tool runs to update all section links.

## .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).

## Table of contents - C# standard

The [README.md](standard/README.md) file in the `standard` folder contains a detailed table of contents for the C# standard.