# Instructions for cleaning the standard.

General instructions:

1. Target the csharp5-standard branch (in the dotnet/csharplang repo).
1. Each commit should be one header section.
1. Each PR should be an H2 section, no larger.
1. It is essential to have a copy of the standard open in another window.

## Things to fix

- There are blank HTML comments that can be removed.  These are `<-- -->`. (Yes, but first use them to locate nested bullet lists. See below.)
- Nested bullet lists: See detailed discussion following this list.
- Replace curled quotes with straight quotes (both double quotes and single quotes).
- *Remove non-header anchors.* These look like `[#Grammar_type](type)`
- *Update section references with links.* There are several section references that appear like `(§23.3)`. These should be replaced with links to the appropriate section in markdown. See [standard.md](standard.md) for a mapping of standard section numbers to file and anchor. For example, the preceding example should be `([§23.3](unsafe-code.md#pointer-types))`. Note that these anchors were generated from a single markdown file so it is likely that the numeric extensions will need to be updated. I tried to catch most of these in this PR. Re links to "General" headers, see below.
- Notes have a line like this: `> [!NOTE]`. Those should be replaced with `> *Note*:` and closed with `*end note*`. This preserves the existing block quote style.
- Examples should be block quoted. These should start with `*Example*:` and end with `*end example*`.
- Look carefully for C# keywords that are inline in text. Many of these have been formatted as plain text.
- Some C# keywords are occasionally code fenced when they shouldn't be, because of global search and replace.
- Superscripts don't render correctly and should be replaced with code `^` for exponents. (See below re superscripts.)
- Check code samples for trailing backslash characters
- Tables are incorrectly formatted. These will all need to be reformatted for correct rendering.
- *Optional* The markdown standard for list indentation is 2 spaces. Some areas of source have 4 spaces. 2 is preferred. (See below.)
- Em-dash and en-dash appear to get converted to en-dash, which in some cases looks odd when rendered.
- Links to "General" headers. (See below.)

## Nested bullet lists

Rex stumbled on this in Clause 11, "Conversions", which has lists nested to 2 and 3 levels. **The Word-to-md conversion flattens all nested lists to 1 level!** Consider the following in which we have 3 levels, with zzz subordinate to yyy, which in turn is subordinate to xxx. The md that resulted was, as follows:
```
-   xxx
<!-- -->
-   yyy
<!-- -->
-   zzz

```
Specifically, the intro dash for the subordinate list entries is *not indented* but it needs to be (2 and 4 spaces, respectively)! The converter puts the HTML comment "\<!-- --\>" at each change of list level change, so you can use that to find all occurences.

Note also that there are exactly 3 spaces between the dash and the entry text. When editing and rendering using Chrome, this caused problems, but only in some cases. My general solution was to have exactly 1 space between the dash and its following text, at each list level. As a result, what worked, was the following:
```
- xxx
  - yyy
    - zzz
```
Now, experiments show that the required format is not always that rigid, but my approach is simple and consistent for all levels of nesting, and it works!

BTW, when editing in Chrome, using my formatting approach, text for level 3 entries is displayed in red rather than black providing a visual verification that the 3rd level was recognized as such.

## Subscripts

While we have a workaround for the lack of superscripts in md (by using `^`), we don't have one for subscripts, which we use a lot in some clauses. The converter simply makes subscripts "regular" characters, as follows with A1 and AK (from basic-concepts.md):

```
If ...
-   `I`
-   `I<A1, ..., AK>`
-   `N.I`
-   `N.I<A1, ..., AK>`

where `I` is a single identifier, `N` is a *namespace-or-type-name* and `<A1, ..., AK>` is an optional *type-argument-list*. When no *type-argument-list* is specified, consider `K` to be zero.
```
Although I haven't studied this in detail, I have not yet seen a case in which the lack of distinction matters. However, be on the lookout for potential problems with this.

Of course, we could use HTML to get super/subscripts, but then we'd be polluting the text with markup!

## Links to "General" headers

ISO disallows *hanging paragraphs*, which is any text between a header level-x and its subordinate header level-x+1. We had them all over the place in the initial spec, and I added *fake* headers called "General" to avoid them. However, this leads to duplicate headers in an md file. For example, Clause 15, "Classes", has 20 of them (15.1, 15.2.1, 15.2.2.1, ...)! In md, the destination of a link within a file *must* be unique, so how to distinguish between duplicates? Consider the link "§15.15.1". This needs to be changed to the following:

```
[§15.15.1](classes.md#general-20)
```
because that General subclause is lexically the 20th General subclause in that file. Note that the header being referred to is *not* itself numbered and doesn't know it's the 20th one. Of course, this is tedious to figure out. And in future when we start adding new General subclauses for new features, **previous links will get broken!** 

> Is there some way to create named anchors that we can define and reference instead? 

## Grammar productions

The grammar productions have been converted based on the grammar's visual style, not semantics. It's best to assume the grammar productions are incorrect. Compare with the grammar productions in the C# 5.0 standard for correctness. The [C# 6.0 draft standard](https://github.com/dotnet/csharplang/tree/master/spec) can be useful to understand ANTLR syntax for grammar productions. There are a number of things to pay attention to:

- Literal strings are not surrounded by single quotes.
- Some terms were italicized using `*`.
- Where multiple ANTLR constructs are not separated by text, but only anchors, remove the anchors and create a single ANTLR block.
- The `|` characters are missing.
- The `;` terminator for rules is missing.
- The `:` character after a rule name should be on the following line for consistency.

In addition, we are taking this opportunity to replace `-` with `_` as a word separator in rule names. Make sure to check for rule names in the text. Replace the `-` with `_` in the grammar productions as well in any text referring to those grammar productions. Note that rule names in the text are italicized, which can help finding them.
