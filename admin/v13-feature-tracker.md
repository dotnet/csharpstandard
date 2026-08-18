## V13 Feature List and Current Status

This file identifies the known V13 features, and tracks their status.

Dependencies between feature specs are noted. The *Effort* column is an attempt to show the size/complexity of the proposal, such as *trivial*, *small*, *medium*, or *large*, allowing TG2 to pick-and-chose the ones they'll work on next.

Rex started with a set of [MS proposals](https://github.com/dotnet/csharplang/tree/main/proposals/csharp-13.0). He wrote tests, looked at MS (and other) tutorial pages. **It is quite possible that not everything in any given MS proposal was in fact implemented in that version, and it is also possible that things implemented in a version later on were not spec'd back into the proposal.** 

Feature | PR | Status | Effort | Annotation | Notes
------- | -- | ------ | ------ | ---------- | ------
Better conversion from collection expression element ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/collection-expressions-better-conversion.md)) | [1772](https://github.com/dotnet/csharpstandard/pull/1772) | SPEC'D | Small | N/A | 
params collections ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/params-collections.md)) | [1783](https://github.com/dotnet/csharpstandard/pull/1783) | SPEC'D | Medium | done |  
New lock object ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/lock-object.md)) | [1770](https://github.com/dotnet/csharpstandard/pull/1770) | SPEC'D | small | N/A | 
New escape sequence (`\e`) ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/esc-escape-sequence.md)) | [1769](https://github.com/dotnet/csharpstandard/pull/1769) | SPEC'D | Tiny | N/A | 
Method group natural type ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/method-group-natural-type-improvements.md)) | [1773](https://github.com/dotnet/csharpstandard/pull/1773) | SPEC'D | Small | N/A | build's on V10's natural type
Implicit indexer access in object initializers (no MS Proposal) | [1782](https://github.com/dotnet/csharpstandard/pull/1782) | SPEC'D | Tiny | N/A | new context for `^` operator
ref and unsafe in iterators and async methods ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/ref-unsafe-in-iterators-async.md)) | [1781](https://github.com/dotnet/csharpstandard/pull/1781) | SPEC'D | Small | N/A | 
ref struct improvements ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/ref-struct-interfaces.md)) | [1780](https://github.com/dotnet/csharpstandard/pull/1780) | SPEC'D | small | N/A | Involves `allows` anti-constraint as well as the ability to implement interfaces
partial properties and indexers ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/partial-properties.md)) | [1776](https://github.com/dotnet/csharpstandard/pull/1776) | SPEC'D | Small | Done | 
Overload resolution priority ([MS Proposal](Overload Resolution Priority)) | [1774](https://github.com/dotnet/csharpstandard/pull/1774) | SPEC'D | Small | Done | involves recognition of `OverloadResolutionPriorityAttribute`
