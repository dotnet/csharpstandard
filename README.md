# standard

Working space for ECMA-TC49-TG2, the C# standard committee.

## Tools folder

This folder contains tools related to maintaining and converting the ECMA C# spec (ECMA-354).

### GetGrammar

This folder contains an ANTLR grammar-extraction tool and support files.

- ExtractGrammar.exe - the simple-minded grammar-extraction program. It processes only *one* md file.

- GetGrammar.bat - the Windows batch file that invokes ExtractGrammar on each md file of the C# specification that contains ANTLR grammar blocks, in clause order, inserting some md headers and such along the way. The result is a file called grammar.md, **which is a direct replacement for that file in the specification repo**.

> A minor wart: There is an extraneous blank line at the beginning of each of the lexical, syntactic, and unsafe grammars. At a glance, the amount of programming effort probably needed to stop this from happening seems to be *huge* compared with simply deleting those three lines manually. 

### MarkdownConverter

> Details yet to be added.

### StandardAnchorTags

> Details yet to be added.
