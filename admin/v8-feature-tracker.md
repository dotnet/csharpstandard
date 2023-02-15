## V8 Feature List and Current Status

This file identifies the known V8 features, and tracks their status.

The *Effort* column is an attempt to show the size/complexity of the proposal, such as *small*, *medium*, or *large*, allowing TG2 members to pick-and-chose the ones they'll work on next.

Feature | PR | Status | Effort | Annotation | Notes
------- | -- | ------ | ------ | ---------- | ------
alternative interpolated verbatim strings ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/alternative-interpolated-verbatim.md)) | [607](https://github.com/dotnet/csharpstandard/pull/607) | Completed | small | N/A |
async streams ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/async-streams.md)) | [606](https://github.com/dotnet/csharpstandard/pull/606) | Completed | small | Done |
using declarations and async using ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/using.md)), ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/async-using.md)) | [672](https://github.com/dotnet/csharpstandard/pull/672) | Completed | small |  Done |
override with constraints ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/constraints-in-overrides.md)) | [671](https://github.com/dotnet/csharpstandard/pull/671) | Completed | small |  Done |
unmanaged constructed types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/constructed-unmanaged.md)) | [604](https://github.com/dotnet/csharpstandard/pull/604) | Completed | small | N/A |
default interface methods ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/default-interface-methods.md)) | [681](https://github.com/dotnet/csharpstandard/pull/681) | Completed | medium |  Done |
permit `stackalloc` in nested contexts ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/nested-stackalloc.md)) |  | In-progress | small | N/A | Waiting on the [V7 PR #238](https://github.com/dotnet/csharpstandard/pull/238) to be merged
`notnull` constraint ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/notnull-constraint.md)) | | Almost complete | small |  N/A | Waiting on the [V7 PR #244](https://github.com/dotnet/csharpstandard/pull/244) to be merged
null coalescing assignment ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/null-coalescing-assignment.md)) | [609](https://github.com/dotnet/csharpstandard/pull/609) | In-progress | small |  N/A | See Issue [#737](https://github.com/dotnet/csharpstandard/issues/737)
nullable reference types ([MS Proposal (from V9)](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/nullable-reference-types-specification.md) which supercedes ([MS Proposal (from V8)](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/nullable-reference-types.md)) |[700](https://github.com/dotnet/csharpstandard/pull/700) | Almost complete | large  | Done | waiting on V7 "Local function declarations" to be merged; waiting on v8 "notnull" feature to be merged 
Obsolete on property accessor ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/obsolete-accessor.md)) | **no change needed** | Postponed | | N/A | See Issue [#375](https://github.com/dotnet/csharpstandard/issues/375)
recursive pattern matching ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/patterns.md)) | | In-progress | medium | | dependent on V7 and other V8 proposals
ranges and indices ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/ranges.md)) | [605](https://github.com/dotnet/csharpstandard/pull/605) | Completed | medium | Done but not yet applied to Draft PR |
readonly instance members ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/readonly-instance-members.md)) | [673](https://github.com/dotnet/csharpstandard/pull/673) | Completed | small | N/A  | **Needs a small tweak once draft-v8 rebased with draft-v7**
name shadowing in nested functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/shadowing-in-nested-functions.md)) | [608](https://github.com/dotnet/csharpstandard/pull/608) | Open | small | N/A  |
static local functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/static-local-functions.md)) | | Almost complete | small | | Pending final words for V7.0 addition of non-static local functions
disposable ref structs/pattern-based using ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/using.md)) | | Almost complete | small | | Q. on Teams re extension method support

