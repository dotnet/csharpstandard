## V9 Feature List and Current Status

This file identifies the known V9 features, and tracks their status.

Dependencies between feature specs are noted. The *Effort* column is an attempt to show the size/complexity of the proposal, such as *small*, *medium*, or *large*, allowing TG2 to pick-and-chose the ones they'll work on next.

Rex started with a set of [MS proposals](https://github.com/dotnet/csharplang/tree/main/proposals/csharp-9.0). He wrote tests, looked at MS (and other) tutorial pages. **It is quite possible that not everything in any given MS proposal was in fact implemented in that version, and it is also possible that things implemented in a version later on were not spec'd back into the proposal.** 

[Any work done by Rex that has not yet been turned into a Draft PR is stored in a Dropbox folder to which Bill, Mads, and Jon have access.]

Feature | PR | Branch | Status | Effort | Annotation | Notes
------- | -- | ------ | ------ | ------ | ---------- | ------
Init accessors ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/init.md))         | [#1452](https://github.com/dotnet/csharpstandard/pull/1452) | `v9-init-accessors` | SPEC'D | medium | Done | See review notes in [#978](https://github.com/dotnet/csharpstandard/pull/978)
Top-level statements ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/top-level-statements.md))	| [#1454](https://github.com/dotnet/csharpstandard/pull/1454) | `v9-top-level-statements` | SPEC'D |  small | Done | 
Native sized integers ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/native-integers.md))	| [#1457](https://github.com/dotnet/csharpstandard/pull/1457) | `v9-native-sized-integers`  | SPEC'D | medium | Done | See review notes in [#1060](https://github.com/dotnet/csharpstandard/pull/1060)
Records ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/records.md))	| [#1458](https://github.com/dotnet/csharpstandard/pull/1458)  | `v9-records` | SPEC'D | large | Done | See open issues in the PR intro
Function pointers ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/function-pointers.md))	| [#1459](https://github.com/dotnet/csharpstandard/pull/1459)  |`v9-function-pointers` | SPEC'D | large | Done | See open issues in the PR intro
Pattern matching enhancements ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/patterns3.md))	| [#1460](https://github.com/dotnet/csharpstandard/pull/1460)  | `v9-patterns` | SPEC'D | medium | Done |
Suppress emitting localsinit flag ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/skip-localsinit.md))	| | N/A | | | **This is a compiler feature and not a language feature**
static anonymous functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/static-anonymous-functions.md))	| [#1461](https://github.com/dotnet/csharpstandard/pull/1461)  | `v9-static-anonymous-functions`| SPEC'D | small | N/A |
Target-typed conditional expressions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/target-typed-conditional-expression.md))	| [#1465](https://github.com/dotnet/csharpstandard/pull/1465)  | `v9-target-typed-conditional-expression` | SPEC'D | small | N/A |
Covariant return types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/covariant-returns.md))	| [#1462](https://github.com/dotnet/csharpstandard/pull/1462) | `v9-covariant-return-types` | partially SPEC'D |  small | |
Extension GetEnumerator support for foreach loops ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/extension-getenumerator.md))	| [#1472](https://github.com/dotnet/csharpstandard/pull/1472) | `v9-extension-foreach` | SPEC'D | small | N/A | 
Lambda discard parameters ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/lambda-discard-parameters.md))	| [#1464](https://github.com/dotnet/csharpstandard/pull/1464)  | `v9-lambda-discards` | SPEC'D | small | N/A | 
Attributes and extern on local functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/local-function-attributes.md))	| [#1466](https://github.com/dotnet/csharpstandard/pull/1466) | `v9-attribute-locations` | Merged | small | Done | 
Module initializers ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/module-initializers.md))	| [#1467](https://github.com/dotnet/csharpstandard/pull/1467)| `v9-module-initializers` | SPEC'D | small | Done |
New features for partial methods ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/extending-partial-methods.md))	| [#1468](https://github.com/dotnet/csharpstandard/pull/1468)  | `v9-more-partial-methods` | SPEC'D | small | Done | 
Target-typed new expressions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/target-typed-new.md))	| [#1469](https://github.com/dotnet/csharpstandard/pull/1469) | `v9-target-typed-new`  | SPEC'D | small | N/A |
Unconstrained type parameter annotations ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/unconstrained-type-parameter-annotations.md))	| [#1470](https://github.com/dotnet/csharpstandard/pull/1470) | `v9-unconstrained-type-parameters`  | SPEC'D | small | N/A | 
Variance safety for static interface members ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/variance-safety-for-static-interface-members.md))	| [#1471](https://github.com/dotnet/csharpstandard/pull/1471) | `v9-static-interface-variance-safetys`  | SPEC'D | small | N/A | 
Nullable reference types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/nullable-reference-types-specification.md))	| [#1221](https://github.com/dotnet/csharpstandard/pull/1221) | `add-v9-nullable-attributes` | SPEC'D | small | Done |
Nullable constructor analysis ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/nullable-constructor-analysis.md))	| None | | | | | | Contains implementation details only; no spec changes necessary
Nullable parameter default value analysis ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/nullable-parameter-default-value-analysis.md))	| None | | | | | | Contains implementation details only; no spec changes necessary


