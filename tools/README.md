# Tools for the C# Standard

This directory contains tools used in the process of preparing and
validating the C# Standard.

## Diagnostic output

Tools that use the shared diagnostic logger retain their human-readable console
output. When `GITHUB_ACTIONS` is `true`, errors, warnings, and notices also emit
workflow commands for runner-generated annotations, using repository-relative
file paths. Console-only messages do not generate annotations.

Set `DIAGNOSTICS_REPORT_PATH` to a JSON file path to save the tool name, success
state, and all collected diagnostics. For example, in PowerShell:

```powershell
$env:DIAGNOSTICS_REPORT_PATH = Join-Path $PWD 'reports\word-converter.json'
```

Run the tool normally after setting the variable. Parent directories are created
automatically. Reports retain every collected diagnostic, even when GitHub limits
displayed annotations, and preserve message text without workflow-command
escaping. Console-only messages are not included.

Once the logger is initialized, the tool writes its report even on early failure
or an exception. A failed tool still returns a nonzero exit code; report-writing
errors also fail the invocation. Failures before the tool starts, such as a
compilation failure, may produce no report.

Without `DIAGNOSTICS_REPORT_PATH`, execution remains console-only locally.
Reporting requires no GitHub authentication and no longer uses `GH_TOKEN` or
`HEAD_SHA`.

## GetGrammar

This is used to extract the ANTLR grammar from all the clauses in the Standard
(in order) and creates a new `grammar.md` file with the complete
grammar.

## Standard Anchor Tags

This automatically renumbers sections within the Standard. See
the [contributing guide](../CONTRIBUTING.md) for details.

The legacy `--owner` and `--repo` options are accepted for compatibility with
existing scripts but are no longer required or used.

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
