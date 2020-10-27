## V6 Feature List and Current Status

To find out what was new/different in v6, Rex used two main sources:

- The [What's New in V6](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6) web page
- The diffs that resulted when he compared the v5std md with the v6spec md.

In the latter case, he located those features that required new or changed grammar. He then found all uses of the names of grammar productions that were added/removed, so he could make adjustments to the text. As he progressed, he discovered some new features that did *not* involve grammar changes. Others have reported (small) new features as well.

This file identifies the known V6 features and tracks their status.

Feature | PR | Notes
-------- | --- | ----------
initialization of an accessible indexer  | [??](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/87), [??](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/114) | This also handled the initialization of associative collections using indexers
expression-bodied function members | [4](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/4) | The "What's New" page only mentions methods and properties; however, the final edits also included indexers and operators as well. As a by-product, this PR also took care of adding support for automatically implemented property initializers
read-only automatically implemented properties | [11](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/11) | 
using static |  [9](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/9) |
nameof | [10](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/10) |
exception filter |  [2](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/2) |
null-conditional operator | [7](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/7) |
interpolated strings | [12](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/12) |
enum base type tweak |  [??](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/111) | See [here](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-6.0/enum-base-type.md)
relaxed rules for auto-properties in structs |  [??](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/112) | See [here](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-6.0/struct-autoprop-init.md)
await in catch and finally blocks | [3](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/3) |
extension Add methods in collection initializers | [8](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/8) |
improved overload resolution | [6](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/6) | See Issue #995 
empty array for params parameter | | See Issue #1004  
