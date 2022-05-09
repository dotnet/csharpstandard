# Contribute fixes to the C# Standard text

This article classifies three types of changes to the standard:

- *Copyedits*: These changes do not modify the meaning of the standard, but fix (English) grammar issues, style and formatting issues. Importantly, changing text from informative to normative and vice-versa is not a copy edit.
- *Issue fixes*: These changes fix an identified or discovered issue in the standard text. They have semantic impact. However, they are small in scope and address a single error in the current text.
- *Language features*: These are larger changes for a new language feature. In general, these are made by the committee.

***This repository does not accept proposals to change the behavior of the C# language. This repository holds the C# standard text. The work here is to update the standard text based on proposals accepted and implemented following the process in the [dotnet/csharplang](https://github.com/dotnet/csharplang) repository.*** PRs or issues for new features will be closed without action.

## Repository use

- ***C# 5.0 standard in markdown***: This is a faithful version of the C# 5.0 standard, converted to markdown and copyedited. Use the `standard-v5` branch for this version.
- ***Initial C# 6.0 draft, based on the C# 5.0 standard:*** The C# 6 draft in the [dotnet/csharplang](https://github.com/dotnet/charplang) repository was created from the Microsoft version of the C# 5.0 spec. This text was created by applying the C# 6 feature text to the C# 5.0 standard. Use the `draft-v6` branch for this version.
- ***C# 7.3 draft:*** This version is where we have incorporated the features from C# 7.0 through C# 7.3. Use the `draft-v7` branch for this version.

## Branch management

Our default branch name matches the version of C# being worked on. This means the default branch name will change over time, but will always represent the "current" version for the standards committee. At the time of this writing, the `draft-v6` is the default branch.

## Filing issues

If you have found a problem in the standard, please let us know by filing an issue. If possible, please let us know what kind of problem you've found:

- Does an existing implementation (e.g. Roslyn / .NET) behave contrary to the standard? If you can provide a complete example program and explain how it actually behaves and how you think it should behave according to the standard (ideally referencing all the relevant sections of the standard) that's enormously helpful
- Is there a relatively simple typo in the standard? Feel free to create a PR to address this directly should you wish, but it's equally fine just to create an issue. Please be as specific as possible about where the typo is and what you'd expect instead.
- Is there a C# feature missing in the standard?
  - If this is with respect to a feature introduced an older version, or the version that is currently the draft specified in the default branch, we definitely want to capture anything missing. The [feature tracker](https://github.com/dotnet/csharpstandard/blob/draft-v6/admin/v6-feature-tracker.md) lists features for the current version. Please check that document and existing issues then file a new one if the feature isn't already covered.
  - If this is with respect to a feature introduced in a version we haven't tackled yet, please hold off for now. It's simplest for us to create a bunch of issues for "all the features we know are in version X" when we create a branch for version X. If we receive an issue about a future version long before we actually standardize that version, it makes it harder to navigate the issues we're trying to address in the current version.

## Copyedits

Copy edits can be submitted as PRs without first creating an issue to discuss the potential fix. These changes follow our simplest workflow:

- The author creates a PR.
- One committee member (except the author) reviews the PR. The reviewer can approve, close, or request changes to the PR.
- The secretary merges approved PRs on a regular cadence, and logs for meeting minutes.

## Issue fixes

These PRs address identified issues. The scope may be larger, based on the scope of the issue. The changes follow a similar workflow, but provide an option for committee members to raise a PR for discussion:

- We recommend discussion take place on the issue. A draft PR may be created to discuss proposed changes.
- The author updates the PR from *draft* to *ready* status once it is ready for review.
- All committee members may vote on the proposed change with one of two votes:
  - *accept*: This may include comments and suggestions that should be addressed before merging, but no discussion is required. Use the :+1: emoji to approve.
  - *discuss*: This means the changes should be discussed in the next committee meeting. The member should include comments to focus the discussion. Use the :-1: emoji.
- If all votes cast are to accept, the secretary merges. Otherwise, the PR is discussed at the next meeting and an action plan is determined.

## Language features

These PRs describe one feature or sub-feature. The workflow allows for collaboration during writing, and a final review when the feature is completely specified.

- The author accepts assignment of the feature issue.
- The author performs work in a branch (or fork).
- The author can tag other committee members for review when needed.
- When complete, the author rebases the work on the current default branch and opens a PR.
- During a 2 week review period, comments are accepted.
- The author responds to comments.
- The PR is given final review and approval at the committee meeting.

## How to add or remove clauses

The automatic section renumbering tool will update all references in any clause of the standard when clauses are added, removed, or moved. Do not update section numbers, either in headers or links, when you add, move, or remove clauses.

When you add a new clause, you do not need to add any numbers. For example, use the following for a new H3 clause:

```markdown
### New feature added
```

You may want to add links to a new clause in the same PR that adds the new clause. In that case, add a marker before the section header text that you can use as an anchor. The marker must start with the '§' character. The text that follows the '§' character is user-defined. The marker you add must be unique in your PR for the standard text. We recommend including the parent header where there may be multiple clauses with the same title, such as **General**.

```markdown
### §feature-added-new-clause New feature added

#### §feature-added-new-clause-general General

```

The `§` character indicates that the token should be an anchor to that clause, rather than part of the clause header. You can add a link to that section as follows:

```markdown
See §feature-added-new-clause for more details on how this feature affects everything. The §feature-added-new-clause-general clause has an overview.
```

The tool will replace the text with the following:

```markdown
See [§19.23.42](features.md#19-23-42-new-feature-added) for more details on how this feature affects everything. The [§19.23.42.1](features.md#19-23-42-1-general) clause has an overview.
```

## Coding style

The code in the standard is optimized for reading the standard. In general, follow the existing code format. We've tried to be consistent throughout the standard.

We've adopted the following non-exhaustive rules:

- Use the latest C# syntax for the branch being edited. This is currently C# 7.3, except where older syntax is required for the example. Readers of the standard should see the latest syntax.
- Use [Allman style braces](https://en.wikipedia.org/wiki/Indentation_style?msclkid=5bd8d8f5cd7b11ec82461327ced4546a#Allman_style) in most of our samples. The exception is when a method or type is empty, where the declaration, the opening and closing brace are on the same line.
- Use a blank line between peer declarations. There are two exceptions to this rule:
  - If a group of peer declarations all have empty bodies (e.g. `class C {}` or `public void M() {}`, then no blank line is added between them.
  - Don't add a blank line where the concept is better illustrated without them. For example, multiple overloads of the same method may read better grouped without a blank line.
- Elements of the same type can be grouped without blank lines. When that's done, there should be a blank line between elements of different types. (e.g. all private fields can be declared without a blank line, but there should be a blank line between the group of private fields and method declarations).
- Indentation is 4 spaces. Use spaces, not tabs for the Word and PDF export formats.
- Where a group of lines all have comments appended, the opening `//` should be on the same column on all lines.
- Where expression bodied members are used, place all code on the same line if it fits. If it's too long, wrap after the `=>` token.
- Line lengths cannot exceed 80 characters.
- Use spaces around most tokens (e.g. `:` for base classes or interface implementation, `+` and all operators). Exceptions are:
  - `<` and `>` to declare generic types and methods
  - `(` and `)`. No spaces around parentheses on method declarations.
  - No space after the opening `(` and no space preceding the closing `)` for other expressions.
  - No spaces on `[]` for array declarations.

ANTLR Grammar productions should not exceed 80 characters.
