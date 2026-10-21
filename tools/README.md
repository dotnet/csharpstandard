# Tools for the C# Standard

This directory contains tools used in the process of preparing and
validating the C# Standard.

## Diagnostic output

Tools that use the shared diagnostic logger retain their human-readable console
output. When `GITHUB_ACTIONS` is `true`, errors, warnings, and notices also emit
workflow commands for runner-generated annotations, using repository-relative
file paths. Console-only messages do not generate annotations.

The logger retains all collected diagnostics, independently of annotation display
limits. The existing custom check-run publisher still submits only the first
50 diagnostics; its summary identifies any additional diagnostics in the log.

## GetGrammar

This is used to extract the ANTLR grammar from all the clauses in the Standard
(in order) and creates a new `grammar.md` file with the complete
grammar.

## Standard Anchor Tags

This automatically renumbers sections within the Standard. See
the [contributing guide](../CONTRIBUTING.md) for details.

## Word Converter

While the primary output format of the Standard is the Markdown
itself, a Word converter is provided both for personal use and in
order to prepare ECMA submissions.

The [run-converter.sh shell script](run-converter.sh) can be invoked
directly, and is the best starting point to understand the usage.

Note that the converter does *not* need any access to an Office
installation, and can be run in any .NET 5.0 environment.

## Example Extractor and Example Tester

A user's guide to annotating spec examples for extraction and testing can be found [here](./ExampleExtractor/ExtractorAndTesterUsersGuide.md).

The ANTLR grammar for example annotations is [here](./ExampleExtractor/ExampleAnnotationGrammar.g4).

> TODO
