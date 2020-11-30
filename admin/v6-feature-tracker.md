## V6 Feature List and Current Status

To find out what was new/different in v6, Rex used two main sources:

- The [What's New in V6](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-6) web page
- The diffs that resulted when he compared the v5std md with the v6spec md.

In the latter case, he located those features that required new or changed grammar. He then found all uses of the names of grammar productions that were added/removed, so he could make adjustments to the text. As he progressed, he discovered some new features that did *not* involve grammar changes. Others have reported (small) new features as well.

This file identifies the known V6 features and tracks their status.

Feature | PR | Notes
-------- | --- | ----------
[#48](https://github.com/ECMA-TC49-TG2/csharpstandard/issues/48) - initialization of an accessible indexer | [#8](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/8) | This also handled the initialization of associative collections using indexers
[#49](https://github.com/ECMA-TC49-TG2/csharpstandard/issues/49) - expression-bodied function members | [#4](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/4) | The "What's New" page only mentions methods and properties; however, the final edits also included indexers and operators as well. As a by-product, this PR also took care of adding support for automatically implemented property initializers
[#50](https://github.com/ECMA-TC49-TG2/csharpstandard/issues/50) - read-only automatically implemented properties | [#11](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/11) | 
[#51](https://github.com/ECMA-TC49-TG2/csharpstandard/issues/51) - using static |  [#9](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/9) |
[#52](https://github.com/ECMA-TC49-TG2/csharpstandard/issues/52) - nameof | [#10](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/10) |
[#53](https://github.com/ECMA-TC49-TG2/csharpstandard/issues/53) - exception filter |  [#2](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/2) |
[#54](https://github.com/ECMA-TC49-TG2/csharpstandard/issues/54) - null-conditional operator | [#7](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/7) |
[#55](https://github.com/ECMA-TC49-TG2/csharpstandard/issues/55) - interpolated strings | [#12](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/12) |
[#56](https://github.com/ECMA-TC49-TG2/csharpstandard/issues/56) - enum base type tweak |  [??](https://github.com/ECMA-TC49-TG2/conversion-to-markdown/pull/111) | See [here](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-6.0/enum-base-type.md). May be completed
[#57](https://github.com/ECMA-TC49-TG2/csharpstandard/issues/57) - relaxed rules for auto-properties in structs |  [#11](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/11). See [diff](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/11/files#diff-db3cda0263120ba604965e231273850f9b60c1ec077cc0098f44b3123be19526R357-R370). | See [here](https://github.com/dotnet/csharplang/blob/master/proposals/csharp-6.0/struct-autoprop-init.md)
[#58](https://github.com/ECMA-TC49-TG2/csharpstandard/issues/58) - await in catch and finally blocks | [3](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/3) |
[#59](https://github.com/ECMA-TC49-TG2/csharpstandard/issues/59) - extension Add methods in collection initializers | [#8](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/8) |
improved overload resolution | [#6](https://github.com/ECMA-TC49-TG2/csharpstandard/pull/6) | See Issue [#995](https://github.com/ECMA-TC49-TG2/spec/issues/995) 
empty array for params parameter | | See Issue [#1004](https://github.com/ECMA-TC49-TG2/spec/issues/1004)  
