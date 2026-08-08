## V12 Feature List and Current Status

This file identifies the known V12 features, and tracks their status.

Dependencies between feature specs are noted. The *Effort* column is an attempt to show the size/complexity of the proposal, such as *trivial*, *small*, *medium*, or *large*, allowing TG2 to pick-and-chose the ones they'll work on next.

Rex started with a set of [MS proposals](https://github.com/dotnet/csharplang/tree/main/proposals/csharp-12.0). He wrote tests, looked at MS (and other) tutorial pages. **It is quite possible that not everything in any given MS proposal was in fact implemented in that version, and it is also possible that things implemented in a version later on were not spec'd back into the proposal.** 

Feature | PR | Status | Effort | Annotation | Notes
------- | -- | ------ | ------ | ---------- | ------
Collection expressions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-12.0/collection-expressions.md)) | [1638](https://github.com/dotnet/csharpstandard/pull/1638) | SPEC'D | medium/large | Done | Some Q's remain
Inline Arrays ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-12.0/inline-arrays.md)) | [1620](https://github.com/dotnet/csharpstandard/pull/1620) | SPEC'D | medium | Done | Some Q's remain
Optional and parameter array parameters for lambdas and method groups ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-12.0/lambda-method-group-defaults.md)) | [1623](https://github.com/dotnet/csharpstandard/pull/1623) | SPEC'D | small | N/A | builds on V10
Primary constructors ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-12.0/primary-constructors.md)) | [1766](https://github.com/dotnet/csharpstandard/pull/1766) | SPEC'D | large | Done| builds on V9's record classes and V10's record structs
ref readonly parameters ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-12.0/ref-readonly-parameters.md)) | [1624](https://github.com/dotnet/csharpstandard/pull/1624) | SPEC'D | medium | Done | 
Allow using alias directive to reference any kind of Type ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-12.0/using-alias-types.md)) | [1622](https://github.com/dotnet/csharpstandard/pull/1622) | SPEC'D | small | Done | 
