# Process to create an alpha branch for an upcoming version

Base a new version branch off the previous version's "feature complete alpha branch". That should provide the structure for the content coming in the ealier versions.

> This does increase the likelihood of merge conflicts as the "draft-vN" branch matures, and that work is merged into the earlier versions "feature complete alpha branch". The plan is to merge the draft-vN work into the corresponding "feature complete alpha" branch after each committee meeting. That should pay dividends as we move through upcoming versions.

## Ordering patches

Copilot can do a good job managing conflicts when applying patches, unless the scope of merge conflicts becomes too great. As a result, the order of applying patches is important. After several experiments, the best order is to apply the patches from most files changed to fewest files changes. Where different patches have changed the same number of files, order should work to minimize the potential conflict opportunities.

For example, this order worked best for V9:

1. *Native integers*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1457.patch
   - Medium - 9 files, 0 potential conflict opportunities
     - arrays.md
     - classes.md
     - conversions.md
     - enums.md
     - expressions.md
     - lexical-structure.md
     - portability-issues.md
     - statements.md
     - types.md
     - unsafe-code.md
     - variables.md
1. *Init Accessors*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1452.patch
   - Medium 7 files, 4 potential conflict opportunities
     - attributes.md
     - basic-concepts.md
     - classes.md: 1
     - expressions.md: 1
     - interfaces.md
     - lexical-structure.md: 1
     - portability-issues.md: 1
1. *Function pointers*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1459.patch
   - Large - 6 files, 8 potential conflict opportunities
     - expressions.md: 2
     - lexical-structure.md: 2
     - portability-issues.md: 2
     - standard-library.md
     - types.md: 1
     - unsafe-code.md: 1
1. *Static anonymous functions*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1461.patch
   - Small - 4 files, 5 potential conflict opportunities
     - conversions.md: 1
     - expressions.md: 2
     - unsafe-code.md: 1
     - variables.md: 1
1. *Top level statements*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1454.patch
   - Medium - 4 files, 3 potential conflict opportunities
     - attributes.md: 1
     - basic-concepts.md
     - namespaces.md
     - portability-issues.md: 3
1. *Records*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1458.patch
   - Large - 3 files, 7 potential conflict opportunities
     - classes.md: 2
     - expressions.md: 3
     - lexical-structure.md: 2
1. *Covariant return types*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1462.patch
   - Small - 3 files, 8 potential conflict opportuntities (this was the toughest set of conflicts)
     - classes.md: 3
     - expressions.md: 4
     - interfaces.md: 1
1. *Target-typed new*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1469.patch
   - Small - 3 files, 8 potential conflict opportunities
     - conversions.md: 2
     - expressions.md: 5
     - statements.md: 1
1. *Module initializers*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1467.patch
   - Small - 3 files, 5 potential conflict opportunities
     - attributes.md: 2
     - portability-issues.md: 3
     - standard-library.md
1. *Target-typed conditional expressions*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1465.patch
   - Small - 2 files, 8 potential conflict opportunities. (This set was also very difficult conflicts)
     - conversions.md: 3
     - expressions.md: 5
1. *Pattern matching*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1460.patch
   - Medium - 2 files, 3 potential conflict opportunities
     - lexical-structure.md: 3
     - patterns.md
1. *Attributes and extern on local functions*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1466.patch
   - Small - 2 files, 3 potential conflict opportunities
     - attributes.md: 2
     - statements.md: 1
1. *More better nullable*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1221.patch
   - Small - 2 files, 3 potential conflict opportunities
     - attributes.md: 3
     - standard-library
1. *Lambda discard parameters*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1464.patch
   - Small - expressions.md: 6
1. *New features for partial members*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1468.patch
   - Small - classes.md: 3
1. *Unconstrained type parameter annotations*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1470.patch
   - Small - classes.md: 4
1. *Variance safety for static interface members*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1471.patch
   - Small - interfaces.md: 2
1. *Extension GetEnumerator in foreach*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1472.patch
   - Small - statements.md: 1
