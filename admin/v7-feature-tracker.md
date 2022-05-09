## V7 Feature List and Current Status

This file identifies the known V7 features, and tracks their status. Although there were minor "dot" releases of V7, committee ECMA TC49/TG2 plans to combine all 7.x features into one V7 spec.

For the most part, the features were spec'd in version-number order. (One exception was that a feature from 7.2 was combined with two related features from 7.0 in a single PR.) Dependencies between feature specs are noted. The *Effort* column is an attempt to show the size/complexity of the proposal, such as *small*, *medium*, or *large*, allowing TG2 to pick-and-chose the ones they'll work on next.

Unlike with the V6 work, where we had separate and divergent V5 Ecma and MS specs, for V7, we started with a set of [MS proposals](https://github.com/dotnet/csharplang/tree/main/proposals), some of which were placeholders **for which text was never provided**. Where they existed, Rex took these proposals, wrote tests, looked at MS (and other) tutorial pages, and created a branch, and for most features, a corresponding (Draft) PR, with the edits he thought were needed. **It is quite possible that not everything in any given MS proposal was in fact implemented in that version, and it is also possible that things implemented in a version later on were not spec'd back into the proposal.** 

For any given feature, Rex actually wrote his version of the final proposal, in a Word file, but using md syntax. This allowed him to order the sets of edits by clause and subclause, and to exploit Word's comment, tracked-change, color, and other features, in order to make the proposal more readable and consistant. A Table-of-Contents was also added, so the reader can see at a glance the areas of the standard impacted by the proposal. This Word document was eventually turned into the branch edits and corresponding PR. For at least some features, it likely will be useful for TG2 members to have access to these Word files, as they may be helpful in seeing the actual changes made, along with "Notes to TG2" that Rex made for consideration come processing time. (These Word documents and their associated test currently reside in a DropBox folder to which Bill Wagner has access, as well as on Rex's machine.)

Version | Feature | PR | Status | Effort | Notes
------- | ------- | -- | ------ | ------ | -----
7.0 | binary integer literals | [449](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/449) | Open | Small | Feature Group A.
7.0 | embedded digit separators in numeric literals | [449](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/449) | Open | Small | Feature Group A.
7.0 | `out` variables | [44](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/44) | Open | Small | Feature Group B.
7.0 | Discards | [44](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/44) | Open | Small | Feature Group B.
7.0 | Tuples | [63](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/63) **but needs some work** | Open | Large | Feature Group C.
7.0 | Pattern Matching | [61](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/61) | Open | Medium | Feature Group D.
7.0 | `ref` locals and returns | [213](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/213) **but needs work** | Open | Medium | Feature Group E.
7.0 | Local Functions | [104](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/104) | Open | Small | 
7.0 | More expression-bodied members | [69](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/69) | Open | Small | 
7.0 | `throw` Expressions | [65](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/65) **but needs some work** | Open | Small | 
7.0 | Generalized `async` return types | [556](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/556) **but needs some work** | Open | Small | 
7.1 | `async Main` method | [70](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/70) | Open | Small | 
7.1 | `default` literal expressions | [236](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/236) | Open | Small | 
7.1 | Inferred tuple element names - see [MS Proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-7.1/infer-tuple-names.md) | [63](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/63) **but needs some work** | Open | Large | Feature Group C.
7.1 | Pattern matching on generic type parameters | [61](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/61) | Open | Medium | Feature Group D.
7.2 | leading digit separators in bin/hex integer literals | [449](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/449) | Open | Small | Feature Group A.
7.2 | Non-trailing named arguments | [216](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/216) | Open | Small | 
7.2 | `private protected` access modifier | [215](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/215) | Open | Small |
7.2 | Conditional `ref` expressions | [213](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/213) **but needs work**  | Open | | Medium | Feature Group E.
7.2 | `in` parameter modifier | [219](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/219) | Open | Medium | Feature Group F.
7.2 | `ref` with `this` in extension methods | [219](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/219) | Open | Medium | Feature Group F.
7.2 | `readonly` and `ref` structs | [333](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/333) | Open | Small | 
7.3 | indexing movable fixed buffer without pinning | [239](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/239) | Open | Small |  
7.3 | reassign `ref` local variables | [213](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/213) **but needs work**  | Open | | Medium | Feature Group E.
7.3 | use initializers on `stackalloc` arrays | [238](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/238) | Open | Small | 
7.3 | Support for Pattern-Based `fixed` Statements | [240](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/240) | Open | Small |  
7.3 | use additional generic constraints | [244](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/244) | Open | Small | 
7.3 | test `==` and `!=` with tuple types | [63](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/63) **but needs some work** | Open | Large | Feature Group C.
7.3 | use expression variables in more locations | [44](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/44), [61](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/61) | Open | Medium | Feature Group B and D.
7.3 | attach attributes to the backing field of auto-implemented properties | [262](https://github.com/dotnet/csharpstandard/pull/262) | Open | Small | 
7.3 | overload resolution now has fewer ambiguous cases | [263](https://github.com/dotnet/csharpstandard/pull/263) | Open | Small | 
