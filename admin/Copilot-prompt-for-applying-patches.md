# Plan: Apply 18 Git Patches for C# 9/10 Features

Sequential application of 18 git patches to the current branch, adding C# 9 and 10 language features to the standard specification. After each patch, pause for user validation. Track changes via both git history and detailed notes to resolve conflicts caused by earlier patches in the series.

## Workflow

For each patch URL provided by the user:

1. **Download and preview patch**: Use curl to download patch file, parse to identify affected files, report to user for context preparation
2. **Apply with git am**: Execute `git am <patchfile>` and monitor for success or conflicts
3. **Resolve conflicts using dual tracking**: First adjust patch file line numbers; if unsuccessful, consult both git diff history and maintained notes of section changes to merge updates from earlier patches with current patch
4. **Document and pause**: After successful application, record modified sections and line ranges in notes, summarize feature changes, stop for user validation
5. **Iterate through series**: Once user confirms, delete patch file and repeat for next patch URL

## Conflict Resolution Strategy

- **Primary approach**: Edit patch file to update line numbers and locations when git am fails
- **Secondary approach**: Manually merge changes by consulting:
  - Git history showing what previous patches modified
  - Running notes of key section modifications per file
- **Key principle**: Accept changes from both patches and merge updates, as each patch was originally non-conflicting

## Change Tracking

Maintain notes for files with high conflict risk:
- **expressions.md** (12 patches) - most heavily modified
- **classes.md** (5 patches)
- **lexical-structure.md** (5 patches)
- **conversions.md** (4 patches)
- **portability-issues.md** (4 patches)

For each successfully applied patch, document:
- Modified sections and approximate line ranges
- Nature of changes (new sections, grammar updates, cross-reference additions)
- Any renumbering or structural changes that might affect subsequent patches

## Patch Application Order

The user will provide patch URLs in their chosen order. The planned order from the prompt file is:

***fill in using algorithm form [notes](./alpha-branch-creation.md).***

## Session Management

- All patches applied in one continuous session to maintain full context and change history
- User validation checkpoint after each patch before proceeding to next
- No intermediate validation tools (grammar checks, example extraction) between patches
- All patches applied directly to current branch (no feature branch creation)

## Success Criteria

- All 18 patches successfully applied with git commits
- Working directory clean after entire series
- User validates correctness after each patch application
- Change notes maintained for conflict resolution reference
