To find out what was new/different in v6, Rex used two main sources:

- The [What's New in V6](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6) web page
- The diffs that resulted when he compared the v5std md with the v6spec md.

In the latter case, he located those features that required new or changed grammar. He then found all uses of the names of grammar productions that were added/removed, so he could make adjustments to the text. As he progressed, he discovered some new features that did *not* involve grammar changes. Others have reported (small) new features as well.

This file identifies the known V6 features and tracks their status.

Feature | PR | Notes
-------- | --- | ----------
initialization of an accessible indexer  | [87](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/87), [114](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/114) | This also handled the initialization of associative collections using indexers
expression-bodied function members | [91](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/91) | The "What's New" page only mentions methods and properties; however, the final edits also included indexers and operators as well. As a by-product, this PR also took care of adding support for automatically implemented property initializers
read-only automatically implemented properties | [92](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/92) | 
using static |  [97](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/97) |
nameof | [106](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/106) |
exception filter |  [107](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/107) |
null-conditional operator | [108](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/108) |
interpolated strings | [110](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/110) |
enum base type tweak |  [111](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/111) | See [here](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-6.0/enum-base-type.md)
relaxed rules for auto-properties in structs |  [112](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/112) | See [here](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-6.0/struct-autoprop-init.md)
await in catch and finally blocks | [113](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/113) |
extension Add methods in collection initializers | [115](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/115) |
improved overload resolution | | See Issue #995 
empty array for params parameter | | See Issue #1004  
