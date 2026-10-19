## V14 Feature List and Current Status

This file identifies the known V14 features, and tracks their status.

Dependencies between feature specs are noted. The *Effort* column is an attempt to show the size/complexity of the proposal, such as *trivial*, *small*, *medium*, or *large*, allowing TG2 to pick-and-chose the ones they'll work on next.

Rex started with a set of [MS proposals](https://github.com/dotnet/csharplang/tree/main/proposals/csharp-14.0). He wrote tests, looked at MS (and other) tutorial pages. **It is quite possible that not everything in any given MS proposal was in fact implemented in that version, and it is also possible that things implemented in a version later on were not spec'd back into the proposal.** 

Feature | PR | Status | Effort | Annotation | Notes
------- | -- | ------ | ------ | ---------- | ------
Extension operators ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-14.0/extension-operators.md)) | -- | | | | Combined with "Extension members" PR
Extension members ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-14.0/extensions.md)) | [1833](https://github.com/dotnet/csharpstandard/pull/1833) | SPEC'D | medium | N/A | 
`field` keyword ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-14.0/field-keyword.md)) | | | | | 
First-class span types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-14.0/first-class-span-types.md)) | | | | | 
Ignored directives ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-14.0/ignored-directives.md)) | [1826](https://github.com/dotnet/csharpstandard/pull/1826) | SPEC'D | small | N/A | 
null conditional assigmment ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-14.0/null-conditional-assignment.md)) | [1838](https://github.com/dotnet/csharpstandard/pull/1838) | SPEC'D | small | N/A |
Optional and named parameters in expression trees ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-14.0/optional-and-named-parameters-in-expression-trees.md)) | | | | | See [Issue 1799](https://github.com/dotnet/csharpstandard/issues/1799); might be a no-op!
Partial events and constructors ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-14.0/partial-events-and-constructors.md)) | [1795](https://github.com/dotnet/csharpstandard/pull/1795) | SPEC'D | medium | N/A |
Simple lambda parameters with modifiers ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-14.0/simple-lambda-parameters-with-modifiers.md)) | | | | | 
Unbound generic types in nameof ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-14.0/unbound-generic-types-in-nameof.md)) | [1841](https://github.com/dotnet/csharpstandard/pull/1841) | SPEC'D | small | N/A |
User-defined compound assignment ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-14.0/user-defined-compound-assignment.md)) | | | | | 
