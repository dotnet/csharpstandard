# Plan: Apply 16 Git Patches for C# 11 Features

Sequential application of 16 git patches to the current branch, adding C# 11 language features to the standard specification. After each patch, pause for user validation. Track changes via both git history and detailed notes to resolve conflicts caused by earlier patches in the series.

## Workflow

For each patch (URLs listed below in Patch Application Order):

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
- **Ambiguous merges**: Flag for human review when it's unclear how to combine overlapping edits
- **Section renumbering**: Do NOT manually renumber sections or update cross-reference links during patch application. An automated renumbering tool will be run on the completed branch after all patches are applied

## Change Tracking

Maintain notes for files with high conflict risk:
- **expressions.md** (12 patches) - most heavily modified
- **classes.md** (9 patches)
- **lexical-structure.md** (6 patches)
- **standard-library.md** (4 patches)
- **interfaces.md** (3 patches)
- **attributes.md** (3 patches)
- **patterns.md** (3 patches)

For each successfully applied patch, document:
- Modified sections and approximate line ranges
- Nature of changes (new sections, grammar updates, cross-reference additions)
- Any renumbering or structural changes that might affect subsequent patches

## Patch Application Order

Patches are applied in the order listed below (largest to smallest file count, per the algorithm in [alpha-branch-creation.md](./alpha-branch-creation.md)):

1. *Ref fields and scoped*
   - https://github.com/dotnet/csharpstandard/pull/1614.patch
   - large - 9 files
     - attributes.md
     - basic-concepts.md
     - classes.md
     - expressions.md
     - lexical-structure.md
     - standard-library.md
     - statements.md
     - structs.md
     - variables.md
1. *File local types*
   - https://github.com/dotnet/csharpstandard/pull/1587.patch
   - medium - 8 files (3 potential conflicts)
     - classes.md*
     - delegates.md
     - enums.md
     - expressions.md*
     - interfaces.md
     - lexical-structure.md*
     - namespaces.md
     - structs.md*
1. *Required members*
   - https://github.com/dotnet/csharpstandard/pull/1597.patch
   - large - 7 files (9 potential conflicts)
     - attributes.md*
     - classes.md**
     - expressions.md**
     - interfaces.md*
     - lexical-structure.md**
     - standard-library.md*
     - types.md
1. *Checked user defined conversions*
   - https://github.com/dotnet/csharpstandard/pull/1585.patch
   - medium - 6 files (9 potential conflicts)
     - classes.md***
     - conversions.md
     - documentation-comments.md
     - expressions.md***
     - standard-library.md**
     - statements.md* 
1. *Unsigned right shift operator*
   - https://github.com/dotnet/csharpstandard/pull/1595.patch
   - medium 5 files. (13 potential conflicts)
     - classes.md****
     - documentation-comments.md*
     - expressions.md****
     - lexical-structure.md***
     - variables.md*
1. *Extended nameof scope*
   - https://github.com/dotnet/csharpstandard/pull/1586.patch
   - small - 4 files (12 potential conflicts)
     - basic-concepts.md*
     - classes.md*****
     - delegates.md*
     - expressions.md*****
1. *static abstract members in interfaces*
   - https://github.com/dotnet/csharpstandard/pull/1609.patch
   - medium - 4 files (9 potential conflicts)
     - classes.md******
     - conversions.md*
     - interfaces.md**
     - patterns.md
1. *Numeric int ptr*
  - https://github.com/dotnet/csharpstandard/pull/1598.patch
  - large - 4 files (8 potential conflicts)
    - conversions.md**
    - expressions.md******
    - types.md*
    - unsafe-code.md
1. *UTF8 string literals*
   - https://github.com/dotnet/csharpstandard/pull/1610.patch
   - medium - 3 files (12 potential conflicts)
     - attributes.md**
     - expressions.md******
     - lexical-structure.md****
1. *Relaxing shift operator requirements*
   - https://github.com/dotnet/csharpstandard/pull/1590.patch
   - trivial - 2 files (13 potential conflicts)
     - classes.md******
     - expressions.md*******
1. *Raw string literal*
   - https://github.com/dotnet/csharpstandard/pull/1599.patch
   - medium - 2 files (12 potential conflicts)
     - expressions.md********
     - lexical-structure.md****
1. *Pattern match span on constant string*
   - https://github.com/dotnet/csharpstandard/pull/1594.patch
   - small - 2 files (4 potential conflicts)
     - patterns.md*
     - standard-library.md***
1. *List and slice patterns*
   - https://github.com/dotnet/csharpstandard/pull/1591.patch
   - small - 2 files (2 potential conflicts)
     - patterns.md**
     - portability-issues.md
1. *Auto-default structs*
   - https://github.com/dotnet/csharpstandard/pull/1573.patch
   - trivial - 1 file (9 potential conflicts)
     - expressions.md*********
1. *New lines in interpolations*
   - https://github.com/dotnet/csharpstandard/pull/1589.patch
   - trivial - 1 file (10 potential conflicts)
     - expressions.md**********
1. *Generic attributes*
   - https://github.com/dotnet/csharpstandard/pull/1588.patch
   - trivial - 1 file (6 potential conflicts)
     - classes.md******

## Session Management

- All patches applied in one continuous session to maintain full context and change history
- User validation checkpoint after each patch before proceeding to next
- All patches applied directly to current branch (no feature branch creation)
- Changes to this plan file are expected and will be committed alongside patch work

## Post-Patch Verification

After each patch, perform extra checks inspired by V9 lessons learned:
- **Code example completeness**: Verify that code examples have matching opening/closing braces and no truncated lines (V9 issue: missing `}` lines in record struct examples)
- **Grammar block completeness**: Confirm grammar productions were fully applied, not partially dropped (V9 issue: file-scoped namespace grammar changes were omitted)
- **Diff review**: Summarize the changes for user validation

## Success Criteria

- All 16 patches successfully applied with git commits
- Working directory clean after entire series
- User validates correctness after each patch application
- Change notes maintained for conflict resolution reference
- Automated section renumbering tool run on completed branch (post all patches)
