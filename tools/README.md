# Tools for the C# Standard

This directory contains tools used in the process of preparing and
validating the C# Standard.

## GetGrammar

This is used to extract the ANTLR grammar from all the clauses in the Standard
(in order) and creates a new `grammar.md` file with the complete
grammar.

## Standard Anchor Tags

This automatically renumbers sections within the Standard. See
the [contributing guide](../Contribute.md) for details.

## Word Converter

While the primary output format of the Standard is the Markdown
itself, a Word converter is provided both for personal use and in
order to prepare ECMA submissions.

The [run-converter.sh shell script](run-converter.sh) can be invoked
directly, and is the best starting point to understand the usage.

Note that the converter does *not* need any access to an Office
installation, and can be run in any .NET 5.0 environment.
