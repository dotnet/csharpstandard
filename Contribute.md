# Contribute fixes to the C# Standard text

This article classifies three types of changes to the standard:

- *Copyedits*: These changes do not modify the meaning of the standard, but fix (English) grammar issues, style and formatting issues. Importantly, changing text from informative to normative and vice-versa is not a copy edit.
- *Issue fixes*: These changes fix an identified or discovered issue in the standard text. They have semantic impact. However, they are small in scope and address a single error in the current text.
- *Language features*: These are larger changes for a new language feature. In general, these are made by the committee.

***This repository does not accept proposals to change the behavior of the C# language. This repository holds the C# standard text. The work here is to update the standard text based on proposals accepted and implemented following the process in the [dotnet/csharplang](https://github.com/dotnet/csharplang) repository.*** PRs or issues for new features will be closed without action.

## Repository use

- ***C# 5.0 standard in markdown***: This is a faithful version of the C# 5.0 standard, converted to markdown and copyedited. Use the `version-5` branch for this version.
- ***Initial C# 6.0 draft, based on the C# 5.0 standard:*** The C# 6 draft in the [dotnet/csharplang](https://github.com/dotnet/charplang) repository was created from the Microsoft version of the C# 5.0 spec. This text was created by applying the C# 6 feature text to the C# 5.0 standard. Use the `version-6` draft for this version. (Currently `v6-draft` in this repo).
- ***C# 7.3 draft:*** This version is where we have incorporated the features from C# 7.0 through C# 7.3. Use the `version-7-3` branch for this version.

## Branch management

Our default branch name matches the version of C# being worked on. This means the default branch name will change over time, but will always represent the "current" version for the standards committee. At the time of this writing, the `6-draft` is the default branch.

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

You may want to add links to a new clause in the same PR that adds the new clause. In that case, add a marker before the section header text that you can use as an anchor:

```markdown
### §feature-added-new-clause New feature added
```

The `§` character indicates that the token should be an anchor to that clause, rather than part of the clause header. You can add a link to that section as follows:

```markdown
See §feature-added-new-clause for more details on how this feature affects everything.
```

The tool will replace the text with the following:

```markdown
See [§19.23.42](features.md#19-23-42-new-feature-added) for more details on how this feature affects everything.
```
