## V8 Feature List and Current Status

This file identifies the known V8 features, and tracks their status.

The *Effort* column is an attempt to show the size/complexity of the proposal, such as *small*, *medium*, or *large*, allowing TG2 members to pick-and-chose the ones they'll work on next.

Feature | PR | Status | Effort | Notes
------- | -- | ------ | ------ | ------
alternative interpolated verbatim strings ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/alternative-interpolated-verbatim.md)) | [607](https://github.com/dotnet/csharpstandard/pull/607) | Completed | small | 
async streams ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/async-streams.md)) | [606](https://github.com/dotnet/csharpstandard/pull/606) | Completed | small | 
async using declaration ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/async-using.md)) |  | Open | | 
override with constraints ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/constraints-in-overrides.md)) | | Open | 
unmanaged constructed types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/constructed-unmanaged.md)) | [604](https://github.com/dotnet/csharpstandard/pull/604) | Completed | small |
default interface methods ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/default-interface-methods.md)) | | Largely complete | medium | Needs some Q's answered
permit `stackalloc` in nested contexts ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/nested-stackalloc.md)) |  | In-progress | small | wait on the V7 stackalloc PR to be merged
`notnull` constraint ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/notnull-constraint.md)) | | Almost complete | small | 
null coalescing assignment ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/null-coalescing-assignment.md)) | [609](https://github.com/dotnet/csharpstandard/pull/609) | Completed | small | 
nullable reference types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/nullable-reference-types.md)) | | Almost complete | large | Needs many Q's answered
Obsolete on property accessor ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/obsolete-accessor.md)) | **no change needed** | Postponed | | See Issue [#375](https://github.com/dotnet/csharpstandard/issues/375)
recursive pattern matching ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/patterns.md)) | |  | medium |
ranges and indices ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/ranges.md)) | [605](https://github.com/dotnet/csharpstandard/pull/605) | Completed | medium | 
readonly instance members ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/readonly-instance-members.md)) |  | Completed | small | 
name shadowing in nested functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/shadowing-in-nested-functions.md)) | [608](https://github.com/dotnet/csharpstandard/pull/608) | Open | small | 
static local functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/static-local-functions.md)) | | Almost complete | small | Pending final words for V7.0 addition of non-static local functions
unconstrained type parameter in null coalescing operator ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/unconstrained-null-coalescing.md)) | | Completed | |
using declarations ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/using.md)) | | Completed | small | 
disposable ref structs/pattern-based using ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/using.md)) | | Almost complete | small | Q. on Teams re extension method support

