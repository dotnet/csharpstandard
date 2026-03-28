## V11 Feature List and Current Status

This file identifies the known V11 features, and tracks their status.

Dependencies between feature specs are noted. The *Effort* column is an attempt to show the size/complexity of the proposal, such as *trivial*, *small*, *medium*, or *large*, allowing TG2 to pick-and-chose the ones they'll work on next.

Rex started with a set of [MS proposals](https://github.com/dotnet/csharplang/tree/main/proposals/csharp-11.0). He wrote tests, looked at MS (and other) tutorial pages. **It is quite possible that not everything in any given MS proposal was in fact implemented in that version, and it is also possible that things implemented in a version later on were not spec'd back into the proposal.** 

Feature | PR | Status | Effort | Annotation | Notes
------- | -- | ------ | ------ | ---------- | ------
Auto-default structs ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/auto-default-structs.md)) | [1573](https://github.com/dotnet/csharpstandard/pull/1573) | SPEC'D | trivial | N/A | 
Checked user-defined operators ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/checked-user-defined-operators.md)) | [1585](https://github.com/dotnet/csharpstandard/pull/1585) | SPEC'D | medium | N/A |
Extended nameof scope ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/extended-nameof-scope.md)) | [1586](https://github.com/dotnet/csharpstandard/pull/1586) | SPEC'D | small | N/A | 
File-local types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/file-local-types.md)) | [1587](https://github.com/dotnet/csharpstandard/pull/1587) | SPEC'D | medium | N/A | 
Generic Attributes ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/generic-attributes.md)) | [1588](https://github.com/dotnet/csharpstandard/pull/1588) | SPEC'D | trivial | N/A | 
List and slice patterns ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/list-patterns.md)) | [1591](https://github.com/dotnet/csharpstandard/pull/1591) | SPEC'D | small | Done |
Ref-Fields and Scoped Variables; Low-Level Struct Improvements ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/low-level-struct-improvements.md)) | [1614](https://github.com/dotnet/csharpstandard/pull/1614) | SPEC'D | large | Done | many issues
Allow new-lines in all interpolations ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/new-line-in-interpolation.md)) | [1589](https://github.com/dotnet/csharpstandard/pull/1589) | SPEC'D | trivial | N/A | 
Numeric IntPtr ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/numeric-intptr.md)) | [1598](https://github.com/dotnet/csharpstandard/pull/1598) | SPEC'D | large | N/A | several issues
Pattern match Span<char> on a constant string ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/pattern-match-span-of-char-on-string.md)) | [1594](https://github.com/dotnet/csharpstandard/pull/1594) | SPEC'D | small | N/A | 
Raw string literal ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/raw-string-literal.md)) | [1599](https://github.com/dotnet/csharpstandard/pull/1599) | SPEC'D | medium | Done | 
Relaxing shift operator requirements ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/relaxing_shift_operator_requirements.md)) | [1590](https://github.com/dotnet/csharpstandard/pull/1590) | SPEC'D | trivial | N/A |
Required Members ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/required-members.md)) | [1597](https://github.com/dotnet/csharpstandard/pull/1597) | SPEC'D | large | Done |
Static abstract members in interfaces ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/static-abstracts-in-interfaces.md)) | [1609](https://github.com/dotnet/csharpstandard/pull/1609) | SPEC'D | medium | N/A | 
Unsigned right shift operator ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/unsigned-right-shift-operator.md)) | [1595](https://github.com/dotnet/csharpstandard/pull/1595) | SPEC'D | medium | N/A |
Utf8 Strings Literals ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-11.0/utf8-string-literals.md)) | [1610](https://github.com/dotnet/csharpstandard/pull/1610) | SPEC'D | medium | N/A |
