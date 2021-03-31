## V7 Feature List and Current Status

This file identifies the known V7 features and tracks their status. Although there were minor "dot" releases of V7, committee ECMA TC49/TG2 plans to combine all 7.x features into one V7 spec.

Version | Feature | PR | Status | Notes
------- | ------- | -- | ------ | ------
7.0 | binary integer literals | [45](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/45) | Open | Combination #1
7.0 | embedded digit separators in numeric literals | [45](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/45) | Open |  Combination #1
7.0 | `out` variables | [44](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/44) | Open |  Combination #2
7.0 | Discards | [44](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/44) | Open |  Combination #2
7.0 | Tuples | [63](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/63) **but needs some work** | Open |  Combination #3
7.0 | Pattern Matching | [61](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/61) **but needs some work** | Open |  Combination #3
7.0 | `ref` locals and returns | [213](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/213) | Open | Combination #4
7.0 | Local Functions | [104](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/104) **but needs a lot of work** | Open | 
7.0 | More expression-bodied members | [69](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/69) | Open |  
7.0 | `throw` Expressions | [65](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/65) **but needs some work** | Open |  
7.0 | Generalized `async` return types | | Open | 
7.1 | `async Main` method | [70](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/70) | Open |  
7.1 | `default` literal expressions | [236](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/236) | Open |  
7.1 | Inferred tuple element names | [63](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/63) **but needs some work** | Open | 
7.1 | Pattern matching on generic type parameters | [61](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/61) | Open |  Combination #3
7.2 | leading digit separators in bin/hex integer literals | [45](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/45) | Open |  Combination #1
7.2 | Non-trailing named arguments | [216](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/216) | Open | | 
7.2 | `private protected` access modifier | [215](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/215) | Open | 
7.2 | Conditional `ref` expressions | [213](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/213) | Open | Combination #4
7.2 | `in` parameter modifier | [219](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/219) | Open | Combination #5
7.2 | `ref` with `this` in extension methods | [219](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/219) | Open | Combination #5
7.2 | `readonly` and `ref` structs | **In progress** | Open | 
7.3 | indexing movable fixed buffer without pinning | [239](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/239) | Open |  
7.3 | reassign `ref` local variables | [213](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/213) | Open | Combination #4
7.3 | use initializers on `stackalloc` arrays | [238](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/238) | Open |  
7.3 | Support for Pattern-Based `fixed` Statements | [240](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/240) | Open |  
7.3 | use additional generic constraints | [244](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/244) | Open |  
7.3 | test `==` and `!=` with tuple types | [63](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/63) **but needs some work** | Open |  Combination #3
7.3 | use expression variables in more locations | **In progress** | Open |  
7.3 | attach attributes to the backing field of auto-implemented properties | | Open |  
7.3 | Method resolution when arguments differ by `in` has been improved | | Open |  
7.3 | overload resolution now has fewer ambiguous cases | | Open |  
