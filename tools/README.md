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

Or, in Bash:

```bash
export DIAGNOSTICS_REPORT_PATH="$PWD/reports/word-converter.json"
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

The PR workflows upload `example-test-diagnostics`, `word-converter-diagnostics`,
and `section-renumber-diagnostics` artifacts. The merge-update workflow uploads
both tool reports as `spec-update-diagnostics`. Download these artifacts from
the workflow run's **Artifacts** section. Uploads run even after a failed tool
step and warn if the tool could not produce a report; they do not change a
failed job's result.

Each Actions job remains a pass/fail check on the PR. Runner-generated
annotations belong to that job check; GitHub may also display them inline when
their locations match the PR diff. These annotations do not recreate the
separately named custom check runs.

### Testing diagnostic reporting

From the `tools` directory, run the focused logger and tool-report suites:

```console
dotnet test tools.sln --filter "FullyQualifiedName~StatusCheckLoggerTests|FullyQualifiedName~DiagnosticReportTests"
```

The integration tests use isolated temporary inputs, including compiling and
running examples. They do not update the checked-out standard.

For end-to-end verification, use a fork PR. Confirm that a deliberately failing
tool still fails its Actions job, the **Upload diagnostics** step succeeds when
a report exists, and runner-generated annotations appear on the job check.
Download and inspect the JSON artifact, and check the job's **GITHUB_TOKEN
Permissions** log section for `Contents: read` with no repository write
permissions. A same-repository PR does not verify fork-specific behavior.

### Required checks and rollout

Before rollout, inspect classic branch protection and repository or inherited
organization rulesets for required checks that use the old custom check names.
Replace any such requirements with the corresponding Actions job checks:

| Old custom check name | Actions job check | Workflow |
| --- | --- | --- |
| `Example tester` | `test-extraction-and-runner` | [Test examples](../.github/workflows/test-examples.yaml) |
| `Markdown to Word Converter` | `word-converter` | [Word Converter](../.github/workflows/word-converter.yaml) |
| `TOC and Anchor updater` | `renumber-sections` | [Renumber standard TOC](../.github/workflows/renumber-sections.yaml) |

Keep existing Actions job requirements and unrelated checks unchanged, and keep
optional jobs optional. Verify that replacement jobs run for the branches and
changes covered by each protection rule, including workflow path filters.
Recheck the effective requirements before merging, and complete the fork-PR
verification described above before rollout.

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
