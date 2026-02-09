## V10 Feature List and Current Status

This file identifies the known V10 features, and tracks their status.

Dependencies between feature specs are noted. The *Effort* column is an attempt to show the size/complexity of the proposal, such as *small*, *medium*, or *large*, allowing TG2 to pick-and-chose the ones they'll work on next.

Rex started with a set of [MS proposals](https://github.com/dotnet/csharplang/tree/main/proposals/csharp-10.0). He wrote tests, looked at MS (and other) tutorial pages. **It is quite possible that not everything in any given MS proposal was in fact implemented in that version, and it is also possible that things implemented in a version later on were not spec'd back into the proposal.** 

Feature | PR | Status | Effort | Annotation | Notes
------- | -- | ------ | ------ | ---------- | -----
record class ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/record-structs.md))	| [1536](https://github.com/dotnet/csharpstandard/pull/1536) | SPEC'D | trivial | N/A | |
record struct ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/record-structs.md))	| [1556](https://github.com/dotnet/csharpstandard/pull/1556) | SPEC'D | medium/large | Done | |
Enhanced #line directives ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/enhanced-line-directives.md))	| [1564](https://github.com/dotnet/csharpstandard/pull/1564) | SPEC'D | small/medium | pending | |
CallerArgumentExpression attribute diagnostics ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/caller-argument-expression.md))	| [1535](https://github.com/dotnet/csharpstandard/pull/1535)  | SPEC'D | small | Done ||
Interpolated string handler ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/improved-interpolated-strings.md))	| [1552](https://github.com/dotnet/csharpstandard/pull/1552)  | SPEC'D | small | Done | **Also incorporates Constant interpolated strings feature spec** |
Constant interpolated strings ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/constant_interpolated_strings.md))	| [1552](https://github.com/dotnet/csharpstandard/pull/1552)  | SPEC'D | small | Done | **merged into Interpolated string handler feature spec** |
File Scoped Namespaces ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/file-scoped-namespaces.md))	| [1540](https://github.com/dotnet/csharpstandard/pull/1540)  | SPEC'D | small | Done | |
Allow AsyncMethodBuilder attribute on methods ([MS Proposal](https://github.com/dotnet/csharplang/tree/main/proposals/csharp-10.0))	| [1543](https://github.com/dotnet/csharpstandard/pull/1543)  | SPEC'D | small | N/A ||
Assignment and declaration in same deconstruction ([**NO MS Proposal**]())	| [1544](https://github.com/dotnet/csharpstandard/pull/1544)  | SPEC'D | trivial | N/A ||
Improved definite assignment ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/improved-definite-assignment.md))	| [1545](https://github.com/dotnet/csharpstandard/pull/1545)  | SPEC'D | medium | N/A | |
Record types can seal ToString (**NO MS Proposal**)	| [1550](https://github.com/dotnet/csharpstandard/pull/1550)  | SPEC'D | trivial | N/A |
Lambda expression improvements ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/lambda-improvements.md))	| [1566](https://github.com/dotnet/csharpstandard/pull/1566)  | SPEC'D | medium | Done | |
Extended property patterns ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/extended-property-patterns.md))	| [1551](https://github.com/dotnet/csharpstandard/pull/1551) | SPEC'D | small | N/A |
Global using directives ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/GlobalUsingDirective.md))	| [1565](https://github.com/dotnet/csharpstandard/pull/1565)  | SPEC'D | medium | N/A | |
Parameterless struct constructors ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-10.0/parameterless-struct-constructors.md))	| [1561](https://github.com/dotnet/csharpstandard/pull/1561)  | SPEC'D | medium | Done | |
