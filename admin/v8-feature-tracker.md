## V8 Feature List and Current Status

This file identifies the known V8 features, and tracks their status.

The *Effort* column is an attempt to show the size/complexity of the proposal, such as *small*, *medium*, or *large*, allowing TG2 members to pick-and-chose the ones they'll work on next.

Feature | PR | Status | Effort | Annotation | Notes
------- | -- | ------ | ------ | ---------- | ------
alternative interpolated verbatim strings ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/alternative-interpolated-verbatim.md)) | [607](https://github.com/dotnet/csharpstandard/pull/607) | Merged | small | N/A |
async streams ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/async-streams.md)) | [606](https://github.com/dotnet/csharpstandard/pull/606) | Completed | small | Done | PR [672](https://github.com/dotnet/csharpstandard/pull/672) will be layered on top of this
using declarations and async using ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/using.md)), ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/async-using.md)) | [672](https://github.com/dotnet/csharpstandard/pull/672) | Completed | small |  Done | This PR will be layered on top of PR [606](https://github.com/dotnet/csharpstandard/pull/606); reconcile any overlap
override with constraints ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/constraints-in-overrides.md)) | [671](https://github.com/dotnet/csharpstandard/pull/671) | Completed | small |  Done |
unmanaged constructed types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/constructed-unmanaged.md)) | [604](https://github.com/dotnet/csharpstandard/pull/604) | Completed | small | N/A |
default interface methods ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/default-interface-methods.md)) | [681](https://github.com/dotnet/csharpstandard/pull/681) | Completed | medium |  Done |
permit `stackalloc` in nested contexts ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/nested-stackalloc.md)) |  | In-progress | small | N/A | can be completed once draft-V8 has been rebased on final V7
`notnull` constraint ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/notnull-constraint.md)) | [830](https://github.com/dotnet/csharpstandard/pull/830) | Completed | small |  Done | 
null coalescing assignment ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/null-coalescing-assignment.md)) | [609](https://github.com/dotnet/csharpstandard/pull/609) | In-progress | small |  N/A | See Issue [#737](https://github.com/dotnet/csharpstandard/issues/737)
nullable reference types ([MS Proposal (from V9)](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-9.0/nullable-reference-types-specification.md) which supercedes ([MS Proposal (from V8)](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/nullable-reference-types.md)) |[700](https://github.com/dotnet/csharpstandard/pull/700) | Completed | large  | Done | related to V8 "notnull" feature 
Obsolete on property accessor ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/obsolete-accessor.md)) | **no change needed** | Postponed | | N/A | See Issue [#375](https://github.com/dotnet/csharpstandard/issues/375)
New kinds of pattern matching ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/patterns.md)) | [873](https://github.com/dotnet/csharpstandard/pull/873) | Completed | medium | Done | precedence table assumes "ranges and indices" has been merged
ranges and indices ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/ranges.md)) | [605](https://github.com/dotnet/csharpstandard/pull/605) | Completed | medium | Done |
readonly instance members ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/readonly-instance-members.md)) | [673](https://github.com/dotnet/csharpstandard/pull/673) | Completed | small | N/A  | **Needs a small tweak once draft-v8 rebased with draft-v7**
name shadowing in nested functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/shadowing-in-nested-functions.md)) | [608](https://github.com/dotnet/csharpstandard/pull/608) | Completed | small | N/A  |
static local functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/static-local-functions.md)) | [869](https://github.com/dotnet/csharpstandard/pull/869)| Completed | small | N/A | 
Disposable ref structs [672](https://github.com/dotnet/csharpstandard/pull/672) | | | | | Included in PR [606](https://github.com/dotnet/csharpstandard/pull/606) **Check this**
