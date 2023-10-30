## V9 Feature List and Current Status

This file identifies the known V9 features, and tracks their status.

Dependencies between feature specs are noted. The *Effort* column is an attempt to show the size/complexity of the proposal, such as *small*, *medium*, or *large*, allowing TG2 to pick-and-chose the ones they'll work on next.

Rex started with a set of [MS proposals](https://github.com/dotnet/csharplang/tree/main/proposals/csharp-9.0). He wrote tests, looked at MS (and other) tutorial pages. **It is quite possible that not everything in any given MS proposal was in fact implemented in that version, and it is also possible that things implemented in a version later on were not spec'd back into the proposal.** 

Feature | PR | Status | Effort | Annotation | Notes
------- | -- | ------ | ------ | ---------- | ------
Init accessors ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/init.md))         | | SPEC'D | medium | Done | 
Top-level statements ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/top-level-statements.md))	| | SPEC'D |  small | Done | 
Native sized integers ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/native-integers.md))	| | SPEC'D | medium | Done | have a few small issues
Records ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/records.md))	| | SPEC'D | large | Done | need help to resolve some (smallish) issues
Function pointers ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/function-pointers.md))	| | Almost complete | large | Done | need help to resolve some issues
Pattern matching enhancements ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/patterns3.md))	| | PENDING |  | | Wait to write this up until all the pattern-matching additions have been completed in V8
Suppress emitting localsinit flag ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/skip-localsinit.md))	| | N/A | | | **This is explicitly a compiler feature and not a language feature**
static anonymous functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/static-anonymous-functions.md))	| | SPEC'D | small | N/A |
Target-typed conditional expressions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/target-typed-conditional-expression.md))	| | SPEC'D | small | N/A | need help to resolve one issue
Covariant return types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/covariant-returns.md))	| | partially SPEC'D |  small | | waiting on adoption of V8 "impl. in interfaces"
Extension GetEnumerator support for foreach loops ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/extension-getenumerator.md))	| | SPEC'D | small | N/A | 
Lambda discard parameters ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/lambda-discard-parameters.md))	| | PENDING | small | | 
Attributes and extern on local functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/local-function-attributes.md))	| | SPEC'D | small | Pending | Once ExampleTester operational, test new example annotation
Module initializers ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/module-initializers.md))	| | SPEC'D | small | Pending | need help to resolve some issues
New features for partial methods ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/extending-partial-methods.md))	| | SPEC'D | small | Done | 
Target-typed new expressions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/target-typed-new.md))	| | SPEC'D | small | N/A | several open issues
Unconstrained type parameter annotations ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/unconstrained-type-parameter-annotations.md))	| | PENDING | small | | Wait to write this up until all the nullable reference and notnull constraint additions have been completed in V8
