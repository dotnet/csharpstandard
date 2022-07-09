## V8 Feature List and Current Status

This file identifies the known V8 features, and tracks their status.

The *Effort* column is an attempt to show the size/complexity of the proposal, such as *small*, *medium*, or *large*, allowing TG2 members to pick-and-chose the ones they'll work on next.

For V7, we started with a set of [MS proposals](https://github.com/dotnet/csharplang/tree/main/proposals), some of which were placeholders **for which text was never provided**. Where they existed, Rex took these proposals, wrote tests, looked at MS (and other) tutorial pages, and created a branch, and for most features, a corresponding (Draft) PR, with the edits he thought were needed. **It is quite possible that not everything in any given MS proposal was in fact implemented in that version, and it is also possible that things implemented in a version later on were not spec'd back into the proposal.** 

For any given feature, Rex actually wrote his version of the final proposal, in a Word file, but using md syntax. This allowed him to order the sets of edits by clause and subclause, and to exploit Word's comment, tracked-change, color, and other features, in order to make the proposal more readable and consistant. A Table-of-Contents was also added, so the reader can see at a glance the areas of the standard impacted by the proposal. This Word document was eventually turned into the branch edits and corresponding PR. For at least some features, it likely will be useful for TG2 members to have access to these Word files, as they may be helpful in seeing the actual changes made, along with "Notes to TG2" that Rex made for consideration come processing time. (These Word documents and their associated test currently reside in a DropBox folder to which Bill Wagner has access, as well as on Rex's machine.)


Feature | PR | Status | Effort | Notes
------- | -- | ------ | ------ | ------
alternative interpolated verbatim strings ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/alternative-interpolated-verbatim.md)) | small | Completed | | 
async streams ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/async-streams.md)) | | Completed | small | 
async using declaration ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/async-using.md)) | | Open | | 
override with constraints ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/constraints-in-overrides.md)) | | Open | 
unmanaged constructed types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/constructed-unmanaged.md)) | small | Completed | |
default interface methods ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/default-interface-methods.md)) | | Largely complete | medium | Needs some Q's answered
permit `stackalloc` in nested contexts ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/nested-stackalloc.md)) | small | In-progress | | 
`notnull` constraint ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/notnull-constraint.md)) | | Almost complete | small | 
null coalescing assignment ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/null-coalescing-assignment.md)) | small | Completed | | 
nullable reference types ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/nullable-reference-types.md)) | | Almost complete | large | Needs many Q's answered
Obsolete on property accessor ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/obsolete-accessor.md)) | no change | Postponed | | See Issue #375
recursive pattern matching ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/patterns.md)) | | medium | |
ranges and indices ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/ranges.md)) | | Completed | medium | 
readonly instance members ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/readonly-instance-members.md)) | small | Completed | | 
name shadowing in nested functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/shadowing-in-nested-functions.md)) | small | Open | | 
static local functions ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/static-local-functions.md)) | | Almost complete | small | Pending final words for V7.0 addition of non-static local functions
unconstrained type parameter in null coalescing operator ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/unconstrained-null-coalescing.md)) | | Completed | |
using declarations ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/using.md)) | | Completed | small | 
disposable ref structs/pattern-based using ([MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/using.md)) | | Almost complete | small | Q. on Teams re extension method support

