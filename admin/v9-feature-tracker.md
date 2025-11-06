## V9 Feature List and Current Status

This file identifies the known V9 features, and tracks their status.

Dependencies between feature specs are noted. The *Effort* column is an attempt to show the size/complexity of the proposal, such as *small*, *medium*, or *large*, allowing TG2 to pick-and-chose the ones they'll work on next.

Rex started with a set of [MS proposals](https://github.com/dotnet/csharplang/tree/main/proposals/csharp-9.0). He wrote tests, looked at MS (and other) tutorial pages. **It is quite possible that not everything in any given MS proposal was in fact implemented in that version, and it is also possible that things implemented in a version later on were not spec'd back into the proposal.** 

[Any work done by Rex that has not yet been turned into a Draft PR is stored in a Dropbox folder to which Bill, Mads, and Jon have access.]

Feature | PR | Branch | Status | Effort | Annotation | Notes
------- | -- | ------ | ------ | ------ | ---------- | ------
Init accessors ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/init.md))         | [#1452](https://github.com/dotnet/csharpstandard/pull/1492) | `v9-init-accessors` | SPEC'D | medium | Done. See review notes in [#978](https://github.com/dotnet/csharpstandard/pull/978) | 
Top-level statements ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/top-level-statements.md))	| [#1454](https://github.com/dotnet/csharpstandard/pull/1454) | `v9-top-level-statements` | SPEC'D |  small | Done | 
Native sized integers ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/native-integers.md))	| [1060](https://github.com/dotnet/csharpstandard/pull/1060) | N/A  | SPEC'D | medium | Done |
Records ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/records.md))	| [983](https://github.com/dotnet/csharpstandard/pull/983)  | N/A | SPEC'D | large | Done | See open issues in the PR intro
Function pointers ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/function-pointers.md))	| [984](https://github.com/dotnet/csharpstandard/pull/984)  | N/A | SPEC'D | large | Done | See open issues in the PR intro
Pattern matching enhancements ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/patterns3.md))	| [1026](https://github.com/dotnet/csharpstandard/pull/1026)  | N/A | SPEC'D | medium | Done | Might need tweaking after V8 pattern-matching additions merged + open issues addressed
Suppress emitting localsinit flag ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/skip-localsinit.md))	| | N/A | | | **This is a compiler feature and not a language feature**
static anonymous functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/static-anonymous-functions.md))	| [988](https://github.com/dotnet/csharpstandard/pull/988)  | N/A | SPEC'D | small | N/A |
Target-typed conditional expressions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/target-typed-conditional-expression.md))	| [1071](https://github.com/dotnet/csharpstandard/pull/1071)  | N/A | SPEC'D | small | N/A |
Covariant return types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/covariant-returns.md))	| | partially SPEC'D |  small | | waiting on adoption of V8 "impl. in interfaces"
Extension GetEnumerator support for foreach loops ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/extension-getenumerator.md))	| [989](https://github.com/dotnet/csharpstandard/pull/989) | N/A | SPEC'D | small | N/A | 
Lambda discard parameters ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/lambda-discard-parameters.md))	| [995](https://github.com/dotnet/csharpstandard/pull/995)  | N/A | SPEC'D | small | N/A | 
Attributes and extern on local functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/local-function-attributes.md))	| [994](https://github.com/dotnet/csharpstandard/pull/994) | N/A | SPEC'D | small | Done | 
Module initializers ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/module-initializers.md))	| [992](https://github.com/dotnet/csharpstandard/pull/992)| SPEC'D | small | Done |
New features for partial methods ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/extending-partial-methods.md))	| [991](https://github.com/dotnet/csharpstandard/pull/991)  | N/A | SPEC'D | small | Done | 
Target-typed new expressions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/target-typed-new.md))	| [990](https://github.com/dotnet/csharpstandard/pull/990) | N/A  | SPEC'D | small | N/A |
Unconstrained type parameter annotations ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/unconstrained-type-parameter-annotations.md))	| [1326](https://github.com/dotnet/csharpstandard/pull/1326) | N/A  | SPEC'D | small | N/A | 
Variance safety for static interface members ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/variance-safety-for-static-interface-members.md))	| [1343](https://github.com/dotnet/csharpstandard/pull/1343) | N/A  | SPEC'D | small | N/A | 
Nullable reference types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/nullable-reference-types-specification.md))	| [1221](https://github.com/dotnet/csharpstandard/pull/1221) | N/A | SPEC'D | small | Done | The `default` constraint part is handled by PR #[1326](https://github.com/dotnet/csharpstandard/pull/1326)
Nullable constructor analysis ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/nullable-constructor-analysis.md))	| None | | | | | | Contains implementation details only; no spec changes necessary
Nullable parameter default value analysis ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/nullable-parameter-default-value-analysis.md))	| None | | | | | | Contains implementation details only; no spec changes necessary
