# Process to create an alpha branch for an upcoming version

Base a new version branch off the previous version's "feature complete alpha branch". That should provide the structure for the content coming in the ealier versions.

> This does increase the likelihood of merge conflicts as the "draft-vN" branch matures, and that work is merged into the earlier versions "feature complete alpha branch". The plan is to merge the draft-vN work into the corresponding "feature complete alpha" branch after each committee meeting. That should pay dividends as we move through upcoming versions.

## Ordering patches

Copilot can do a good job managing conflicts when applying patches, unless the scope of merge conflicts becomes too great. As a result, the order of applying patches is important. After several experiments, the best order is to apply the patches from most files changed to fewest files changes. Where different patches have changed the same number of files, order should work to minimize the potential conflict opportunities.

For example, this order worked best for V9:

1. *Lambda improvements*
   - https://github.com/dotnet/csharpstandard/pull/1566.patch
   - medium - 4 files
     - conversions.md
     - expressions.md
     - lexical-structure.md
     - statements.md
1. *Extended interpolated strings*
   - https://github.com/dotnet/csharpstandard/pull/1552.patch
   - small - 4 files
     - attributes.md
     - conversions.md*
     - expressions.md*
     - standard-library.md

1. *record structs*
   - https://github.com/dotnet/csharpstandard/pull/1556.patch
   - large - 3 files
     - classes.md
     - expressions.md**
     - structs.md
1. *Global using directives*
   - https://github.com/dotnet/csharpstandard/pull/1565.patch
   - medium - 3 files
     - basic-concepts.md
     - expressions.md***
     - namespaces.md
1. *CallerArgumentExpressionAttribute*
   - https://github.com/dotnet/csharpstandard/pull/1535.patch
   - Small - 3 files
     - attributes.md*
     - standard-library.md*
     - test file:  CallerArgumentAttrM.cs

1. *improved definite assignment*
   - https://github.com/dotnet/csharpstandard/pull/1545.patch
   - medium - 2 files
     - expressions.md****
     - variables.md
1. *async method builder attribute*
   - https://github.com/dotnet/csharpstandard/pull/1543.patch
   - small - 2 files
     - classes.md*
     - standard-library.md**
1. *file scoped namespaces*
   - https://github.com/dotnet/csharpstandard/pull/1540.patch 
   - small - 2 files
     - basic-concepts.md*
     - namespaces.md*
1. *parameterless struct constructors*
   - https://github.com/dotnet/csharpstandard/pull/1561.patch
   - medium - 2 files
     - classes.md**
     - structs.md*
1. *combined assignment and declaration*
   - https://github.com/dotnet/csharpstandard/pull/1544.patch
   - small - 1 file
     - expressions.md*****
1. *record class*
   - https://github.com/dotnet/csharpstandard/pull/1536.patch
   - tiny - 1 file
     - classes.md***
1. *sealed ToString in record types*
   - https://github.com/dotnet/csharpstandard/pull/1550.patch
   - small - 1 file
     - classes.md****
1. *Extended line directive*
   - https://github.com/dotnet/csharpstandard/pull/1564.patch
   - medium - 1 files
     - lexical-structure.md*
1. *forward*
   - https://github.com/dotnet/csharpstandard/pull/1537.patch
   - Small - 1 file
     - foreword.md 
1. *Extended property patterns*
   - https://github.com/dotnet/csharpstandard/pull/1551.patch
   - small - 1 files
     - patterns.md
1. *Attributes and extern on local functions*
   - https://patch-diff.githubusercontent.com/raw/dotnet/csharpstandard/pull/1466.patch
   - Small - 2 files, 3 potential conflict opportunities
     - attributes.md: 2
     - statements.md: 1
1. *More better nullable*. **Already merged**
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
