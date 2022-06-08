# 6 Lexical structure

## 6.1 Programs

A C# ***program*** consists of one or more source files, known formally as ***compilation units*** ([§13.2](namespaces.md#132-compilation-units)). Although a compilation unit might have a one-to-one correspondence with a file in a file system, such correspondence is not required.

Conceptually speaking, a program is compiled using three steps:

1. Transformation, which converts a file from a particular character repertoire and encoding scheme into a sequence of Unicode characters.
1. Lexical analysis, which translates a stream of Unicode input characters into a stream of tokens.
1. Syntactic analysis, which translates the stream of tokens into executable code.

Conforming implementations shall accept Unicode compilation units encoded with the UTF-8 encoding form (as defined by the Unicode standard), and transform them into a sequence of Unicode characters. Implementations can choose to accept and transform additional character encoding schemes (such as UTF-16, UTF-32, or non-Unicode character mappings).

> *Note*: The handling of the Unicode NULL character (U+0000) is implementation-specific. It is strongly recommended that developers avoid using this character in their source code, for the sake of both portability and readability. When the character is required within a character or string literal, the escape sequences `\0` or `\u0000` may be used instead. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: It is beyond the scope of this standard to define how a file using a character representation other than Unicode might be transformed into a sequence of Unicode characters. During such transformation, however, it is recommended that the usual line-separating character (or sequence) in the other character set be translated to the two-character sequence consisting of the Unicode carriage-return character (U+000D) followed by Unicode line-feed character (U+000A). For the most part this transformation will have no visible effects; however, it will affect the interpretation of verbatim string literal tokens ([§6.4.5.6](lexical-structure.md#6456-string-literals)). The purpose of this recommendation is to allow a verbatim string literal to produce the same character sequence when its compilation unit is moved between systems that support differing non-Unicode character sets, in particular, those using differing character sequences for line-separation.  *end note*

## 6.2 Grammars

### 6.2.1 General

This specification presents the syntax of the C# programming language using two grammars. The ***lexical grammar*** ([§6.2.3](lexical-structure.md#623-lexical-grammar)) defines how Unicode characters are combined to form line terminators, white space, comments, tokens, and pre-processing directives. The ***syntactic grammar*** ([§6.2.4](lexical-structure.md#624-syntactic-grammar)) defines how the tokens resulting from the lexical grammar are combined to form C# programs.

All terminal characters are to be understood as the appropriate Unicode character from the range U+0020 to U+007F, as opposed to any similar-looking characters from other Unicode character ranges.

### 6.2.2 Grammar notation

The lexical and syntactic grammars are presented in the ANTLR grammar tool’s Extended Backus-Naur form.

While the ANTLR notation is used, this Standard does not present a complete ANTLR-ready “reference grammar” for C#; writing a lexer and parser, either by hand or using a tool such as ANTLR, is outside the scope of a language specification. With that qualification, this Standard attempts to minimize the gap between the specified grammar and that required to build a lexer and parser in ANTLR.

ANTLR distinguishes between lexical and syntactic, termed parser by ANTLR, grammars in its notation by starting lexical rules with an uppercase letter and parser rules with a lowercase letter.

> *Note*: The C# lexical grammar ([§6.2.3](lexical-structure.md#623-lexical-grammar)) and syntactic grammar ([§6.2.4](lexical-structure.md#624-syntactic-grammar)) are not in exact correspondence with the ANTLR division into lexical and parser grammers. This small mismatch means that some ANTLR parser rules are used when specifying the C# lexical grammar. *end note*

### 6.2.3 Lexical grammar

The lexical grammar of C# is presented in [§6.3](lexical-structure.md#63-lexical-analysis), [§6.4](lexical-structure.md#64-tokens), and [§6.5](lexical-structure.md#65-pre-processing-directives). The terminal symbols of the lexical grammar are the characters of the Unicode character set, and the lexical grammar specifies how characters are combined to form tokens ([§6.4](lexical-structure.md#64-tokens)), white space ([§6.3.4](lexical-structure.md#634-white-space)), comments ([§6.3.3](lexical-structure.md#633-comments)), and pre-processing directives ([§6.5](lexical-structure.md#65-pre-processing-directives)).

Many of the terminal symbols of the syntactic grammar are not defined explicitly as tokens in the lexical grammar. Rather, advantage is taken of the ANTLR behavior that literal strings in the grammar are extracted as implicit lexical tokens; this allows keywords, operators, etc. to be represented in the grammar by their literal representation rather than a token name.

Every compilation unit in a C# program shall conform to the *input* production of the lexical grammar ([§6.3.1](lexical-structure.md#631-general)).

### 6.2.4 Syntactic grammar

The syntactic grammar of C# is presented in the clauses, subclauses, and annexes that follow this subclause. The terminal symbols of the syntactic grammar are the tokens defined explicitly by the lexical grammar and implicitly by literal strings in the grammar itself ([§6.2.3](lexical-structure.md#623-lexical-grammar)). The syntactic grammar specifies how tokens are combined to form C# programs.

Every compilation unit in a C# program shall conform to the *compilation_unit* production ([§13.2](namespaces.md#132-compilation-units)) of the syntactic grammar.

### 6.2.5 Grammar ambiguities

The productions for *simple_name* ([§11.7.4](expressions.md#1174-simple-names)) and *member_access* ([§11.7.6](expressions.md#1176-member-access)) can give rise to ambiguities in the grammar for expressions.

> *Example*: The statement:
>
> ```csharp
> F(G<A, B>(7));
> ```
>
> could be interpreted as a call to `F` with two arguments, `G < A` and `B > (7)`. Alternatively, it could be interpreted as a call to `F` with one argument, which is a call to a generic method `G` with two type arguments and one regular argument.
>
> *end example*

If a sequence of tokens can be parsed (in context) as a *simple_name* ([§11.7.4](expressions.md#1174-simple-names)), *member_access* ([§11.7.6](expressions.md#1176-member-access)), or *pointer_member_access* ([§22.6.3](unsafe-code.md#2263-pointer-member-access)) ending with a *type_argument_list* ([§8.4.2](types.md#842-type-arguments)), the token immediately following the closing `>` token is examined. If it is one of

```csharp
( ) ] : ; , . ? == !=
```

then the *type_argument_list* is retained as part of the *simple_name*, *member_access*, or *pointer_member_access* and any other possible parse of the sequence of tokens is discarded. Otherwise, the *type_argument_list* is not considered part of the *simple_name*, *member_access*, or *pointer_member_access*, even if there is no other possible parse of the sequence of tokens.

> *Note*: These rules are not applied when parsing a *type_argument_list* in a *namespace_or_type_name* ([§7.8](basic-concepts.md#78-namespace-and-type-names)). *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The statement:
>
> ```csharp
> F(G<A, B>(7));
> ```
>
> will, according to this rule, be interpreted as a call to `F` with one argument, which is a call to a generic method `G` with two type arguments and one regular argument. The statements
>
> ```csharp
> F(G<A, B>7);
> F(G<A, B>>7);
> ```
>
> will each be interpreted as a call to `F` with two arguments. The statement
>
> ```csharp
> x = F<A> + y;
> ```
>
> will be interpreted as a less-than operator, greater-than operator and unary-plus operator, as if the statement had been written `x = (F < A) > (+y)`, instead of as a *simple_name* with a *type_argument_list* followed by a binary-plus operator. In the statement
>
> ```csharp
> x = y is C<T> && z;
> ```
>
> the tokens `C<T>` are interpreted as a *namespace_or_type_name* with a *type_argument_list* due to being on the right-hand side of the `is` operator ([§11.11.1](expressions.md#11111-general)). Because `C<T>` parses as a *namespace_or_type_name*, not a *simple_name*, *member_access*, or *pointer_member_access*, the above rule does not apply, and it is considered to have a *type_argument_list* regardless of the token that follows.
>
> *end example*

## 6.3 Lexical analysis

### 6.3.1 General

For convenience, the lexical grammar defines and references the following named lexer tokens:

```ANTLR
DEFAULT  : 'default' ;
NULL     : 'null' ;
TRUE     : 'true' ;
FALSE    : 'false' ;
ASTERISK : '*' ;
SLASH    : '/' ;
```

Although these are lexer rules, these names are spelled in all-uppercase letters to distinguish them from ordinary lexer rule names.

> *Note*: These convenience rules are exceptions to the usual practice of not providing explicit token names for tokens defined by literal strings. *end note*

The *input* production defines the lexical structure of a C# compilation unit.

```ANTLR
input
    : input_section?
    ;

input_section
    : input_section_part+
    ;

input_section_part
    : input_element* New_Line
    | PP_Directive
    ;

input_element
    : Whitespace
    | Comment
    | token
    ;
```

> *Note*: The above grammar is described by ANTLR parsing rules, it defines the lexical structure of a C# compilation unit and not lexical tokens. *end note*

Five basic elements make up the lexical structure of a C# compilation unit: Line terminators ([§6.3.2](lexical-structure.md#632-line-terminators)), white space ([§6.3.4](lexical-structure.md#634-white-space)), comments ([§6.3.3](lexical-structure.md#633-comments)), tokens ([§6.4](lexical-structure.md#64-tokens)), and pre-processing directives ([§6.5](lexical-structure.md#65-pre-processing-directives)). Of these basic elements, only tokens are significant in the syntactic grammar of a C# program ([§6.2.4](lexical-structure.md#624-syntactic-grammar)).

The lexical processing of a C# compilation unit consists of reducing the file into a sequence of tokens that becomes the input to the syntactic analysis. Line terminators, white space, and comments can serve to separate tokens, and pre-processing directives can cause sections of the compilation unit to be skipped, but otherwise these lexical elements have no impact on the syntactic structure of a C# program.

When several lexical grammar productions match a sequence of characters in a compilation unit, the lexical processing always forms the longest possible lexical element.

> *Example*: The character sequence `//` is processed as the beginning of a single-line comment because that lexical element is longer than a single `/` token. *end example*

Some tokens are defined by a set of lexical rules; a main rule and one or more sub-rules. The latter are marked in the grammar by `fragment` to indicate the rule defines part of another token. Fragment rules are not considered in the top-to-bottom ordering of lexical rules.

> *Note*: In ANTLR `fragment` is a keyword which produces the same behavior defined here. *end note*

### 6.3.2 Line terminators

Line terminators divide the characters of a C# compilation unit into lines.

```ANTLR
New_Line
    : New_Line_Character
    | '\u000D\u000A'    // carriage return, line feed 
    ;
```

For compatibility with source code editing tools that add end-of-file markers, and to enable a compilation unit to be viewed as a sequence of properly terminated lines, the following transformations are applied, in order, to every compilation unit in a C# program:

- If the last character of the compilation unit is a Control-Z character (U+001A), this character is deleted.
- A carriage-return character (U+000D) is added to the end of the compilation unit if that compilation unit is non-empty and if the last character of the compilation unit is not a carriage return (U+000D), a line feed (U+000A), a next line character (U+0085), a line separator (U+2028), or a paragraph separator (U+2029).

> *Note*: The additional carriage-return allows a program to end in a *PP_Directive* ([§6.5](lexical-structure.md#65-pre-processing-directives)) that does not have a terminating *New_Line*. *end note*

### 6.3.3 Comments

Two forms of comments are supported: delimited comments and single-line comments.

A ***delimited comment*** begins with the characters `/*` and ends with the characters `*/`. Delimited comments can occupy a portion of a line, a single line, or multiple lines.

> *Example*: The example
>
> ```csharp
> /* Hello, world program
>    This program writes "hello, world" to the console
> */
> class Hello
> {
>     static void Main()
>     {
>         System.Console.WriteLine("hello, world");
>     }
> }
> ```
>
> includes a delimited comment.
>
> *end example*

A ***single-line comment*** begins with the characters `//` and extends to the end of the line.

> *Example*: The example
>
> ```csharp
> // Hello, world program
> // This program writes "hello, world" to the console
> //
> class Hello // any name will do for this class
> {
>     static void Main() // this method must be named "Main"
>     {
>         System.Console.WriteLine("hello, world");
>     }
> }
> ```
>
> shows several single-line comments.
>
> *end example*

```ANTLR
Comment
    : Single_Line_Comment
    | Delimited_Comment
    ;

fragment Single_Line_Comment
    : '//' Input_Character*
    ;

fragment Input_Character
    // anything but New_Line_Character
    : ~('\u000D' | '\u000A'   | '\u0085' | '\u2028' | '\u2029')
    ;
    
fragment New_Line_Character
    : '\u000D'  // carriage return
    | '\u000A'  // line feed
    | '\u0085'  // next line
    | '\u2028'  // line separator
    | '\u2029'  // paragraph separator
    ;
    
fragment Delimited_Comment
    : '/*' Delimited_Comment_Section* ASTERISK+ '/'
    ;
    
fragment Delimited_Comment_Section
    : SLASH
    | ASTERISK* Not_Slash_Or_Asterisk
    ;

fragment Not_Slash_Or_Asterisk
    : ~('/' | '*')    // Any except SLASH or ASTERISK
    ;
```

Comments do not nest. The character sequences `/*` and `*/` have no special meaning within a single-line comment, and the character sequences `//` and `/*` have no special meaning within a delimited comment.

Comments are not processed within character and string literals.

> *Note*: These rules must be interpreted carefully. For instance, in the example below, the delimited comment that begins before `A` ends between `B` and `C()`. The reason is that
>
> ```csharp
> // B */ C();
> ```
>
> is not actually a single-line comment, since `//` has no special meaning within a delimited comment, and so `*/` does have its usual special meaning in that line.
>
> Likewise, the delimited comment starting before `D` ends before `E`. The reason is that `"D */ "` is not actually a string literal, since the initial double quote character appears inside a delimited comment.
>
> A useful consequence of `/*` and `*/` having no special meaning within a single-line comment is that a block of source code lines can be commented out by putting `//` at the beginning of each line. In general, it does not work to put `/*` before those lines and `*/` after them, as this does not properly encapsulate delimited comments in the block, and in general may completely change the structure of such delimited comments.
>
> Example code:
>
> ```csharp
> static void Main()
> {
>     /* A
>     // B */ C();
>     Console.WriteLine(/* "D */ "E");
> }
> ```
>
> *end note*

*Single_Line_Comment*s and *Delimited_Comment*s having particular formats can be used as *documentation comments*, as described in [§D](documentation-comments.md#annex-d-documentation-comments).

### 6.3.4 White space

White space is defined as any character with Unicode class Zs (which includes the space character) as well as the horizontal tab character, the vertical tab character, and the form feed character.

```ANTLR
Whitespace
    : [\p{Zs}]  // any character with Unicode class Zs
    | '\u0009'  // horizontal tab
    | '\u000B'  // vertical tab
    | '\u000C'  // form feed
    ;
```

## 6.4 Tokens

### 6.4.1 General

There are several kinds of ***tokens***: identifiers, keywords, literals, operators, and punctuators. White space and comments are not tokens, though they act as separators for tokens.

```ANTLR
token
    : identifier
    | keyword
    | Integer_Literal
    | Real_Literal
    | Character_Literal
    | String_Literal
    | operator_or_punctuator
    ;
```

> *Note*: This is an ANTLR parser rule, it does not define a lexical token but rather the collection of token kinds. *end note*

### 6.4.2 Unicode character escape sequences

A Unicode escape sequence represents a Unicode code point. Unicode escape sequences are processed in identifiers ([§6.4.3](lexical-structure.md#643-identifiers)), character literals ([§6.4.5.5](lexical-structure.md#6455-character-literals)), regular string literals ([§6.4.5.6](lexical-structure.md#6456-string-literals)), and interpolated regular string expressions ([§11.7.3](expressions.md#1173-interpolated-string-expressions)). A Unicode escape sequence is not processed in any other location (for example, to form an operator, punctuator, or keyword).

```ANTLR
fragment Unicode_Escape_Sequence
    : '\\u' Hex_Digit Hex_Digit Hex_Digit Hex_Digit
    | '\\U' Hex_Digit Hex_Digit Hex_Digit Hex_Digit
            Hex_Digit Hex_Digit Hex_Digit Hex_Digit
    ;
```

A Unicode character escape sequence represents the single Unicode code point formed by the hexadecimal number following the “\u” or “\U” characters. Since C# uses a 16-bit encoding of Unicode code points in character and string values, a Unicode code point in the range `U+10000` to `U+10FFFF` is represented using two Unicode surrogate code units. Unicode code points above `U+FFFF` are not permitted in character literals. Unicode code points above `U+10FFFF` are invalid and are not supported.

Multiple translations are not performed. For instance, the string literal `"\u005Cu005C"` is equivalent to `"\u005C"` rather than `"\"`.

> *Note*: The Unicode value `\u005C` is the character “`\`”. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The example
>
> ```csharp
> class Class1
> {
>     static void Test(bool \u0066)
>     {
>         char c = '\u0066';
>         if (\u0066)
>         {
>             System.Console.WriteLine(c.ToString());
>         }
>     }
> }
> ```
>
> shows several uses of `\u0066`, which is the escape sequence for the letter “`f`”. The program is equivalent to
>
> ```csharp
> class Class1
> {
>     static void Test(bool f)
>     {
>         char c = 'f';
>         if (f)
>         {
>             System.Console.WriteLine(c.ToString());
>         }
>     }
> }
> ```
>
> *end example*

### 6.4.3 Identifiers

The rules for identifiers given in this subclause correspond exactly to those recommended by the Unicode Standard Annex 15 except that underscore is allowed as an initial character (as is traditional in the C programming language), Unicode escape sequences are permitted in identifiers, and the “`@`” character is allowed as a prefix to enable keywords to be used as identifiers.

```ANTLR
identifier
    : Simple_Identifier
    | contextual_keyword
    ;

Simple_Identifier
    : Available_Identifier
    | Escaped_Identifier
    ;

fragment Available_Identifier
    // excluding keywords or contextual keywords, see note below
    : Basic_Identifier
    ;

fragment Escaped_Identifier
    // Includes keywords and contextual keywords prefixed by '@'.
    // See note below.
    : '@' Basic_Identifier 
    ;

fragment Basic_Identifier
    : Identifier_Start_Character Identifier_Part_Character*
    ;

fragment Identifier_Start_Character
    : Letter_Character
    | Underscore_Character
    ;

fragment Underscore_Character
    : '_'           // underscore
    | '\\u005' [fF] // Unicode_Escape_Sequence for underscore
    ;

fragment Identifier_Part_Character
    : Letter_Character
    | Decimal_Digit_Character
    | Connecting_Character
    | Combining_Character
    | Formatting_Character
    ;

fragment Letter_Character
    // Category Letter, all subcategories; category Number, subcategory letter.
    : [\p{L}\p{Nl}]
    // Only escapes for categories L & Nl allowed. See note below.
    | Unicode_Escape_Sequence
    ;

fragment Combining_Character
    // Category Mark, subcategories non-spacing and spacing combining.
    : [\p{Mn}\p{Mc}]
    // Only escapes for categories Mn & Mc allowed. See note below.
    | Unicode_Escape_Sequence
    ;

fragment Decimal_Digit_Character
    // Category Number, subcategory decimal digit.
    : [\p{Nd}]
    // Only escapes for category Nd allowed. See note below.
    | Unicode_Escape_Sequence
    ;

fragment Connecting_Character
    // Category Punctuation, subcategory connector.
    : [\p{Pc}]
    // Only escapes for category Pc allowed. See note below.
    | Unicode_Escape_Sequence
    ;

fragment Formatting_Character
    // Category Other, subcategory format.
    : [\p{Cf}]
    // Only escapes for category Cf allowed, see note below.
    | Unicode_Escape_Sequence
    ;
```

> *Note*:
>
> - For information on the Unicode character classes mentioned above, see *The Unicode Standard*.
> - The fragment *Available_Identifier* requires the exclusion of keywords and contextual keywords. If the grammar in this Standard is processed with ANTLR then this exclusion is handled automatically by the semantics of ANTLR:
>   - Keywords and contextual keywords occur in the grammar as literal strings.
>   - ANTLR creates implicit lexical token rules are created from these literal strings.
>   - ANTLR considers these implicit rules before the explicit lexical rules in the grammar.
>   - Therefore fragment *Available_Identifier* will not match keywords or contextual keywords as the lexical rules for those precede it.
> - Fragment *Escaped_Identifier* includes escaped keywords and contextual keywords as they are part of the longer token starting with an `@` and lexical processing always forms the longest possible lexical element ([§6.3.1](lexical-structure.md#631-general)).
> - How an implementation enforces the restrictions on the allowable *Unicode_Escape_Sequence* values is an implementation issue.
>
> *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: Examples of valid identifiers are `identifier1`, `_identifier2`, and `@if`. *end example*

An identifier in a conforming program shall be in the canonical format defined by Unicode Normalization Form C, as defined by Unicode Standard Annex 15. The behavior when encountering an identifier not in Normalization Form C is implementation-defined; however, a diagnostic is not required.

The prefix “`@`” enables the use of keywords as identifiers, which is useful when interfacing with other programming languages. The character `@` is not actually part of the identifier, so the identifier might be seen in other languages as a normal identifier, without the prefix. An identifier with an `@` prefix is called a ***verbatim identifier***.

> *Note*:  Use of the `@` prefix for identifiers that are not keywords is permitted, but strongly discouraged as a matter of style. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The example:
>
> ```csharp
> class @class
> {
>     public static void @static(bool @bool)
>     {
>         if (@bool)
>         {
>             System.Console.WriteLine("true");
>         }
>         else
>         {
>             System.Console.WriteLine("false");
>         }
>     }
> }
>
> class Class1
> {
>     static void M()
>     {
>         cl\u0061ss.st\u0061tic(true);
>     }
> }
> ```
>
> defines a class named “`class`” with a static method named “`static`” that takes a parameter named “`bool`”. Note that since Unicode escapes are not permitted in keywords, the token “`cl\u0061ss`” is an identifier, and is the same identifier as “`@class`”.
>
> *end example*

Two identifiers are considered the same if they are identical after the following transformations are applied, in order:

- The prefix “`@`”, if used, is removed.
- Each *Unicode_Escape_Sequence* is transformed into its corresponding Unicode character.
- Any *Formatting_Character*s are removed.

Identifiers containing two consecutive underscore characters (`U+005F`) are reserved for use by the implementation; however, no diagnostic is required if such an identifier is defined.

> *Note*: For example, an implementation might provide extended keywords that begin with two underscores. *end note*

### 6.4.4 Keywords

A ***keyword*** is an identifier-like sequence of characters that is reserved, and cannot be used as an identifier except when prefaced by the `@` character.

```ANTLR
keyword
    : 'abstract' | 'as'       | 'base'       | 'bool'      | 'break'
    | 'byte'     | 'case'     | 'catch'      | 'char'      | 'checked'
    | 'class'    | 'const'    | 'continue'   | 'decimal'   | DEFAULT
    | 'delegate' | 'do'       | 'double'     | 'else'      | 'enum'
    | 'event'    | 'explicit' | 'extern'     | FALSE       | 'finally'
    | 'fixed'    | 'float'    | 'for'        | 'foreach'   | 'goto'
    | 'if'       | 'implicit' | 'in'         | 'int'       | 'interface'
    | 'internal' | 'is'       | 'lock'       | 'long'      | 'namespace'
    | 'new'      | NULL       | 'object'     | 'operator'  | 'out'
    | 'override' | 'params'   | 'private'    | 'protected' | 'public'
    | 'readonly' | 'ref'      | 'return'     | 'sbyte'     | 'sealed'
    | 'short'    | 'sizeof'   | 'stackalloc' | 'static'    | 'string'
    | 'struct'   | 'switch'   | 'this'       | 'throw'     | TRUE
    | 'try'      | 'typeof'   | 'uint'       | 'ulong'     | 'unchecked'
    | 'unsafe'   | 'ushort'   | 'using'      | 'virtual'   | 'void'
    | 'volatile' | 'while'
    ;
```

A ***contextual keyword*** is an identifier-like sequence of characters that has special meaning in certain contexts, but is not reserved, and can be used as an identifier outside of those contexts as well as when prefaced by the `@` character.

```ANTLR
contextual_keyword
    : 'add'    | 'alias'      | 'ascending' | 'async'   | 'await'
    | 'by'     | 'descending' | 'dynamic'   | 'equals'  | 'from'
    | 'get'    | 'global'     | 'group'     | 'into'    | 'join'
    | 'let'    | 'nameof'     | 'on'        | 'orderby' | 'partial'
    | 'remove' | 'select'     | 'set'       | 'value'   | 'var'
    | 'when'   | 'where'      | 'yield'
    ;
```

> *Note*: The rules *keyword* and *contextual_keyword* are parser rules as they do not introduce new token kinds. All keywords and contextual keywords are defined by implicit lexical rules as they occur as literal strings in the grammar ([§6.2.3](lexical-structure.md#623-lexical-grammar)). *end note*

In most cases, the syntactic location of contextual keywords is such that they can never be confused with ordinary identifier usage. For example, within a property declaration, the `get` and `set` identifiers have special meaning ([§14.7.3](classes.md#1473-accessors)). An identifier other than `get` or `set` is never permitted in these locations, so this use does not conflict with a use of these words as identifiers.

In certain cases the grammar is not enough to distinguish contextual keyword usage from identifiers. In all such cases it will be specified how to disambiguate between the two. For example, the contextual keyword `var` in implicitly typed local variable declarations ([§12.6.2](statements.md#1262-local-variable-declarations)) might conflict with a declared type called `var`, in which case the declared name takes precedence over the use of the identifier as a contextual keyword.

Another example such disambiguation is the contextual keyword `await` ([§11.8.8.1](expressions.md#11881-general)), which is considered a keyword only when inside a method declared `async`, but can be used as an identifier elsewhere.

Just as with keywords, contextual keywords can be used as ordinary identifiers by prefixing them with the `@` character.

> *Note*: When used as contextual keywords, these identifiers cannot contain *Unicode_Escape_Sequence*s. *end note*

### 6.4.5 Literals

#### 6.4.5.1 General

A ***literal*** ([§11.7.2](expressions.md#1172-literals)) is a source-code representation of a value.

```ANTLR
literal
    : boolean_literal
    | Integer_Literal
    | Real_Literal
    | Character_Literal
    | String_Literal
    | null_literal
    ;
```

> *Note*: *literal* is a parser rule as it groups other token kinds and does not introduce a new token kind. *end note*

#### 6.4.5.2 Boolean literals

There are two Boolean literal values: `true` and `false`.

```ANTLR
boolean_literal
    : TRUE
    | FALSE
    ;
```

> *Note*: *boolean_literal* is a parser rule as it groups other token kinds and does not introduce a new token kind. *end note*

The type of a *boolean_literal* is `bool`.

#### 6.4.5.3 Integer literals

Integer literals are used to write values of types `int`, `uint`, `long`, and `ulong`. Integer literals have three possible forms: decimal, hexadecimal, and binary.

```ANTLR
Integer_Literal
    : Decimal_Integer_Literal
    | Hexadecimal_Integer_Literal
    | Binary_Integer_Literal
    ;

fragment Decimal_Integer_Literal
    : Decimal_Digit Decorated_Decimal_Digit* Integer_Type_Suffix?
    ;

fragment Decorated_Decimal_Digit
    : '_'* Decimal_Digit
    ;
       
fragment Decimal_Digit
    : '0'..'9'
    ;
    
fragment Integer_Type_Suffix
    : 'U' | 'u' | 'L' | 'l' |
      'UL' | 'Ul' | 'uL' | 'ul' | 'LU' | 'Lu' | 'lU' | 'lu'
    ;
    
fragment Hexadecimal_Integer_Literal
    : ('0x' | '0X') Decorated_Hex_Digit+ Integer_Type_Suffix?
    ;

fragment Decorated_Hex_Digit
    : '_'* Hex_Digit
    ;
       
fragment Hex_Digit
    : '0'..'9' | 'A'..'F' | 'a'..'f'
    ;
   
fragment Binary_Integer_Literal
    : ('0b' | '0B') Decorated_Binary_Digit+ Integer_Type_Suffix?
    ;

fragment Decorated_Binary_Digit
    : '_'* Binary_Digit
    ;
       
fragment Binary_Digit
    : '0' | '1'
    ;
```

The type of an integer literal is determined as follows:

- If the literal has no suffix, it has the first of these types in which its value can be represented: `int`, `uint`, `long`, `ulong`.
- If the literal is suffixed by `U` or `u`, it has the first of these types in which its value can be represented: `uint`, `ulong`.
- If the literal is suffixed by `L` or `l`, it has the first of these types in which its value can be represented: `long`, `ulong`.
- If the literal is suffixed by `UL`, `Ul`, `uL`, `ul`, `LU`, `Lu`, `lU`, or `lu`, it is of type `ulong`.

If the value represented by an integer literal is outside the range of the `ulong` type, a compile-time error occurs.

> *Note*: As a matter of style, it is suggested that “`L`” be used instead of “`l`” when writing literals of type `long`, since it is easy to confuse the letter “`l`” with the digit “`1`”. *end note*

To permit the smallest possible `int` and `long` values to be written as integer literals, the following two rules exist:

- When an *Integer_Literal* representing the value `2147483648` (2³¹) and no *Integer_Type_Suffix* appears as the token immediately following a unary minus operator token ([§11.8.3](expressions.md#1183-unary-minus-operator)), the result (of both tokens) is a constant of type int with the value `−2147483648` (−2³¹). In all other situations, such an *Integer_Literal* is of type `uint`.
- When an *Integer_Literal* representing the value `9223372036854775808` (2⁶³) and no *Integer_Type_Suffix* or the *Integer_Type_Suffix* `L` or `l` appears as the token immediately following a unary minus operator token ([§11.8.3](expressions.md#1183-unary-minus-operator)), the result (of both tokens) is a constant of type `long` with the value `−9223372036854775808` (−2⁶³). In all other situations, such an *Integer_Literal* is of type `ulong`.

> *Example*:
>
> ```csharp
> 123                  // decimal, int
> 10_543_765Lu         // decimal, ulong
> 1_2__3___4____5      // decimal, int
> _123                 // not a numeric literal; identifier due to leading _
> 123_                 // invalid; no trailing _allowed
> 
> 0xFf                 // hex, int
> 0X1b_a0_44_fEL       // hex, long
> 0x1ade_3FE1_29AaUL   // hex, ulong
> 0x_abc               // hex, int
> _0x123               // not a numeric literal; identifier due to leading _
> 0xabc_               // invalid; no trailing _ allowed
> 
> 0b101                // binary, int
> 0B1001_1010u         // binary, uint
> 0b1111_1111_0000UL   // binary, ulong
> 0B__111              // binary, int
> __0B111              // not a numeric literal; identifier due to leading _
> 0B111__              // invalid; no trailing _ allowed
> ```
>
> *end example*

#### 6.4.5.4 Real literals

Real literals are used to write values of types `float`, `double`, and `decimal`.

```ANTLR
Real_Literal
    : Decimal_Digit Decorated_Decimal_Digit* '.'
      Decimal_Digit Decorated_Decimal_Digit* Exponent_Part? Real_Type_Suffix?
    | '.' Decimal_Digit Decorated_Decimal_Digit* Exponent_Part? Real_Type_Suffix?
    | Decimal_Digit Decorated_Decimal_Digit* Exponent_Part Real_Type_Suffix?
    | Decimal_Digit Decorated_Decimal_Digit* Real_Type_Suffix
    ;

fragment Exponent_Part
    : ('e' | 'E') Sign? Decimal_Digit Decorated_Decimal_Digit*
    ;

fragment Sign
    : '+' | '-'
    ;

fragment Real_Type_Suffix
    : 'F' | 'f' | 'D' | 'd' | 'M' | 'm'
    ;
```

If no *Real_Type_Suffix* is specified, the type of the *Real_Literal* is `double`. Otherwise, the *Real_Type_Suffix* determines the type of the real literal, as follows:

- A real literal suffixed by `F` or `f` is of type `float`.
  > *Example*: The literals `1f`, `1.5f`, `1e10f`, and `123.456F` are all of type `float`. *end example*
- A real literal suffixed by `D` or `d` is of type `double`.
  > *Example*: The literals `1d`, `1.5d`, `1e10d`, and `123.456D` are all of type `double`. *end example*
- A real literal suffixed by `M` or `m` is of type `decimal`.
  > *Example*: The literals `1m`, `1.5m`, `1e10m`, and `123.456M` are all of type `decimal`. *end example*  
  This literal is converted to a `decimal` value by taking the exact value, and, if necessary, rounding to the nearest representable value using banker’s rounding ([§8.3.8](types.md#838-the-decimal-type)). Any scale apparent in the literal is preserved unless the value is rounded.
  > *Note*: Hence, the literal `2.900m` will be parsed to form the `decimal` with sign `0`, coefficient `2900`, and scale `3`. *end note*

If the magnitude of the specified literal is too large to be represented in the indicated type, a compile-time error occurs.

> *Note*: In particular, a *Real_Literal* will never produce a floating-point infinity. A non-zero *Real_Literal* may, however, be rounded to zero. *end note*

The value of a real literal of type `float` or `double` is determined by using the IEC 60559 “round to nearest” mode with ties broken to “even” (a value with the least-significant-bit zero), and all digits considered significant.

> *Note*: In a real literal, decimal digits are always required after the decimal point. For example, `1.3F` is a real literal but `1.F` is not. *end note*
>
> *Example*:
>
> ```csharp
> 1.234_567      // double
> .3e5f          // float
> 2_345E-2_0     // double
> 15D            // double
> 19.73M         // decimal
> 1.F            // parsed as a member access of F due to non-digit after .
> 1_.2F          // invalid; no trailing _ allowed in integer part
> 1._234         // parsed as a member access of _234 due to non-digit after .
> 1.234_         // invalid; no trailing _ allowed in fraction
> .3e_5F         // invalid; no leading _ allowed in exponent
> .3e5_F         // invalid; no trailing _ allowed in exponent
> ```
>
> *end example*

#### 6.4.5.5 Character literals

A character literal represents a single character, and consists of a character in quotes, as in `'a'`.

```ANTLR
Character_Literal
    : '\'' Character '\''
    ;
    
fragment Character
    : Single_Character
    | Simple_Escape_Sequence
    | Hexadecimal_Escape_Sequence
    | Unicode_Escape_Sequence
    ;
    
fragment Single_Character
    // anything but ', \, and New_Line_Character
    : ~['\\\u000D\u000A\u0085\u2028\u2029]
    ;
    
fragment Simple_Escape_Sequence
    : '\\\'' | '\\"' | '\\\\' | '\\0' | '\\a' | '\\b' |
      '\\f' | '\\n' | '\\r' | '\\t' | '\\v'
    ;
    
fragment Hexadecimal_Escape_Sequence
    : '\\x' Hex_Digit Hex_Digit? Hex_Digit? Hex_Digit?
    ;
```

> *Note*: A character that follows a backslash character (`\`) in a *Character* must be one of the following characters: `'`, `"`, `\`, `0`, `a`, `b`, `f`, `n`, `r`, `t`, `u`, `U`, `x`, `v`. Otherwise, a compile-time error occurs. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The use of the `\x` *Hexadecimal_Escape_Sequence* production can be error-prone and hard to read due to the variable number of hexadecimal digits following the `\x`. For example, in the code:
>
> ```csharp
> string good = "x9Good text";
> string bad = "x9Bad text";
> ```
>
> it might appear at first that the leading character is the same (`U+0009`, a tab character) in both strings. In fact the second string starts with `U+9BAD` as all three letters in the word “Bad” are valid hexadecimal digits. As a matter of style, it is recommended that `\x` is avoided in favour of either specific escape sequences (`\t` in this example) or the fixed-length `\u` escape sequence.
>
> *end note*

A hexadecimal escape sequence represents a single Unicode UTF-16 code unit, with the value formed by the hexadecimal number following “`\x`”.

If the value represented by a character literal is greater than `U+FFFF`, a compile-time error occurs.

A Unicode escape sequence ([§6.4.2](lexical-structure.md#642-unicode-character-escape-sequences)) in a character literal shall be in the range `U+0000` to `U+FFFF`.

A simple escape sequence represents a Unicode character, as described in the table below.

| **Escape sequence** | **Character name** | **Unicode code point** |
|---------------------|--------------------|--------------------|
| `\'`                | Single quote       | U+0027             |
| `\"`                | Double quote       | U+0022             |
| `\\`                | Backslash          | U+005C             |
| `\0`                | Null               | U+0000             |
| `\a`                | Alert              | U+0007             |
| `\b`                | Backspace          | U+0008             |
| `\f`                | Form feed          | U+000C             |
| `\n`                | New line           | U+000A             |
| `\r`                | Carriage return    | U+000D             |
| `\t`                | Horizontal tab     | U+0009             |
| `\v`                | Vertical tab       | U+000B             |

The type of a *Character_Literal* is `char`.

#### 6.4.5.6 String literals

C# supports two forms of string literals: ***regular string literals*** and ***verbatim string literals***. A regular string literal consists of zero or more characters enclosed in double quotes, as in `"hello"`, and can include both simple escape sequences (such as `\t` for the tab character), and hexadecimal and Unicode escape sequences.

A verbatim string literal consists of an `@` character followed by a double-quote character, zero or more characters, and a closing double-quote character.

> *Example*: A simple example is `@"hello"`. *end example*

In a verbatim string literal, the characters between the delimiters are interpreted verbatim, with the only exception being a *Quote_Escape_Sequence*, which represents one double-quote character. In particular, simple escape sequences, and hexadecimal and Unicode escape sequences are not processed in verbatim string literals. A verbatim string literal may span multiple lines.

```ANTLR
String_Literal
    : Regular_String_Literal
    | Verbatim_String_Literal
    ;
    
fragment Regular_String_Literal
    : '"' Regular_String_Literal_Character* '"'
    ;
    
fragment Regular_String_Literal_Character
    : Single_Regular_String_Literal_Character
    | Simple_Escape_Sequence
    | Hexadecimal_Escape_Sequence
    | Unicode_Escape_Sequence
    ;

fragment Single_Regular_String_Literal_Character
    // anything but ", \, and New_Line_Character
    : ~["\\\u000D\u000A\u0085\u2028\u2029]
    ;

fragment Verbatim_String_Literal
    : '@"' Verbatim_String_Literal_Character* '"'
    ;
    
fragment Verbatim_String_Literal_Character
    : Single_Verbatim_String_Literal_Character
    | Quote_Escape_Sequence
    ;
    
fragment Single_Verbatim_String_Literal_Character
    : ~["]     // anything but quotation mark (U+0022)
    ;
    
fragment Quote_Escape_Sequence
    : '""'
    ;
```

> *Example*: The example
>
> ```csharp
> string a = "Happy birthday, Joel"; // Happy birthday, Joel
> string b = @"Happy birthday, Joel"; // Happy birthday, Joel
> string c = "hello \t world"; // hello world
> string d = @"hello \t world"; // hello \t world
> string e = "Joe said \"Hello\" to me"; // Joe said "Hello" to me
> string f = @"Joe said ""Hello"" to me"; // Joe said "Hello" to me
> string g = "\\\\server\\share\\file.txt"; // \\server\share\file.txt
> string h = @"\\server\share\file.txt"; // \\server\share\file.txt
> string i = "one\r\ntwo\r\nthree";
> string j = @"one
> two
> three";
> ```
>
> shows a variety of string literals. The last string literal, `j`, is a verbatim string literal that spans multiple lines. The characters between the quotation marks, including white space such as new line characters, are preserved verbatim, and each pair of double-quote characters is replaced by one such character.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: Any line breaks within verbatim string literals are part of the resulting string. If the exact characters used to form line breaks are semantically relevant to an application, any tools that translate line breaks in source code to different formats (between “`\n`” and “`\r\n`”, for example) will change application behavior. Developers should be careful in such situations. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: Since a hexadecimal escape sequence can have a variable number of hex digits, the string literal `"\x123"` contains a single character with hex value `123`. To create a string containing the character with hex value `12` followed by the character `3`, one could write `"\x00123"` or `"\x12"` + `"3"` instead. *end note*

The type of a *String_Literal* is `string`.

Each string literal does not necessarily result in a new string instance. When two or more string literals that are equivalent according to the string equality operator ([§11.11.8](expressions.md#11118-string-equality-operators)), appear in the same assembly, these string literals refer to the same string instance.

> *Example*: For instance, the output produced by
>
> ```csharp
> class Test
> {
>     static void Main()
>     {
>         object a = "hello";
>         object b = "hello";
>         System.Console.WriteLine(a == b);
>     }
> }
> ```
>
> is `True` because the two literals refer to the same string instance.
>
> *end example*

#### 6.4.5.7 The null literal

```ANTLR
null_literal
    : NULL
    ;
```

> *Note*: *null_literal* is a parser rule as it does not introduce a new token kind. *end note*

A *null_literal* represents a `null` value. It does not have a type, but can be converted to any reference type or nullable value type through a null literal conversion ([§10.2.7](conversions.md#1027-null-literal-conversions)).

### 6.4.6 Operators and punctuators

There are several kinds of operators and punctuators. Operators are used in expressions to describe operations involving one or more operands.

> *Example*: The expression `a + b` uses the `+` operator to add the two operands `a` and `b`. *end example*

Punctuators are for grouping and separating.

```ANTLR
operator_or_punctuator
    : '{'  | '}'  | '['  | ']'  | '('   | ')'  | '.'  | ','  | ':'  | ';'
    | '+'  | '-'  | ASTERISK    | SLASH | '%'  | '&'  | '|'  | '^'  | '!' | '~'
    | '='  | '<'  | '>'  | '?'  | '??'  | '::' | '++' | '--' | '&&' | '||'
    | '->' | '==' | '!=' | '<=' | '>='  | '+=' | '-=' | '*=' | '/=' | '%='
    | '&=' | '|=' | '^=' | '<<' | '<<=' | '=>'
    ;

right_shift
    : '>'  '>'
    ;

right_shift_assignment
    : '>' '>='
    ;
```

> *Note*: *right_shift* and *right_shift_assignment* are parser rules as they do not introduce a new token kind but represent a sequence of two tokens. The *operator_or_punctuator* rule exists for descriptive purposes only and is not used elsewhere in the grammar. *end note*

*right_shift* is made up of the two tokens `>` and `>`. Similarly, *right_shift_assignment* is made up of the two tokens `>` and `>=`. Unlike other productions in the syntactic grammar, no characters of any kind (not even whitespace) are allowed between the two tokens in each of these productions. These productions are treated specially in order to enable the correct handling of *type_parameter_lists* ([§14.2.3](classes.md#1423-type-parameters)).

> *Note*: Prior to the addition of generics to C#, `>>` and `>>=` were both single tokens. However, the syntax for generics uses the `<` and `>` characters to delimit type parameters and type arguments. It is often desirable to use nested constructed types, such as `List<Dictionary<string, int>>`. Rather than requiring the programmer to separate the `>` and `>` by a space, the definition of the two *operator_or_punctuator*s was changed. *end note*

## 6.5 Pre-processing directives

### 6.5.1 General

The pre-processing directives provide the ability to skip conditionally sections of compilation units, to report error and warning conditions, and to delineate distinct regions of source code.

> *Note*: The term “pre-processing directives” is used only for consistency with the C and C++ programming languages. In C#, there is no separate pre-processing step; pre-processing directives are processed as part of the lexical analysis phase. *end note*

```ANTLR
PP_Directive
    : PP_Start PP_Kind PP_New_Line
    ;

fragment PP_Kind
    : PP_Declaration
    | PP_Conditional
    | PP_Line
    | PP_Diagnostic
    | PP_Region
    | PP_Pragma
    ;

// Only recognised at the beginning of a line
fragment PP_Start
    // See note below.
    : { getCharPositionInLine() == 0 }? PP_Whitespace? '#' PP_Whitespace?
    ;

fragment PP_Whitespace
    : ( [\p{Zs}]  // any character with Unicode class Zs
      | '\u0009'  // horizontal tab
      | '\u000B'  // vertical tab
      | '\u000C'  // form feed
      )+
    ;

fragment PP_New_Line
    : PP_Whitespace? Single_Line_Comment? New_Line
    ;
```

> *Note*:
>
> - The pre-processor grammar defines a single lexical token `PP_Directive` used for all pre-processing directives. The semantics of each of the pre-processing directives are defined in this language specification but not how to implement them.
> - The `PP_Start` fragment must only be recognised at the start of a line, the `getCharPositionInLine() == 0` ANTLR lexical predicate above suggests one way in which this may be achieved and is informative *only*, an implementation may use a different strategy.
>
> *end note*

The following pre-processing directives are available:

- `#define` and `#undef`, which are used to define and undefine, respectively, conditional compilation symbols ([§6.5.4](lexical-structure.md#654-definition-directives)).
- `#if`, `#elif`, `#else`, and `#endif`, which are used to skip conditionally sections of source code ([§6.5.5](lexical-structure.md#655-conditional-compilation-directives)).
- `#line`, which is used to control line numbers emitted for errors and warnings ([§6.5.8](lexical-structure.md#658-line-directives)).
- `#error`, which is used to issue errors ([§6.5.6](lexical-structure.md#656-diagnostic-directives)).
- `#region` and `#endregion`, which are used to explicitly mark sections of source code ([§6.5.7](lexical-structure.md#657-region-directives)).
- `#pragma`, which is used to specify optional contextual information to a compiler ([§6.5.9](lexical-structure.md#659-pragma-directives)).

A pre-processing directive always occupies a separate line of source code and always begins with a `#` character and a pre-processing directive name. White space may occur before the `#` character and between the `#` character and the directive name.

A source line containing a `#define`, `#undef`, `#if`, `#elif`, `#else`, `#endif`, `#line`, or `#endregion` directive can end with a single-line comment. Delimited comments (the `/* */` style of comments) are not permitted on source lines containing pre-processing directives.

Pre-processing directives are not part of the syntactic grammar of C#. However, pre-processing directives can be used to include or exclude sequences of tokens and can in that way affect the meaning of a C# program.

> *Example*: When compiled, the program
>
> ```csharp
> #define A
> #undef B
> class C
> {
> #if A
>     void F() {}
> #else
>     void G() {}
> #endif
> #if B
>     void H() {}
> #else    
>     void I() {}
> #endif
> }
> ```
>
> results in the exact same sequence of tokens as the program
>
> ```csharp
> class C
> {
>     void F() {}
>     void I() {}
> }
> ```
>
> Thus, whereas lexically, the two programs are quite different, syntactically, they are identical.
>
> *end example*

### 6.5.2 Conditional compilation symbols

The conditional compilation functionality provided by the `#if`, `#elif`, `#else`, and `#endif` directives is controlled through pre-processing expressions ([§6.5.3](lexical-structure.md#653-pre-processing-expressions)) and conditional compilation symbols.

```ANTLR
fragment PP_Conditional_Symbol
    // Must not be equal to tokens TRUE or FALSE. See note below.
    : Basic_Identifier
    ;
```

> *Note* How an implementation enforces the restriction on the allowable *Basic_Identifier* values is an implementation issue. *end note*

Two conditional compilation symbols are considered the same if they are identical after the following transformations are applied, in order:

- Each *Unicode_Escape_Sequence* is transformed into its corresponding Unicode character.
- Any *Formatting_Characters* are removed.

A conditional compilation symbol has two possible states: ***defined*** or ***undefined***. At the beginning of the lexical processing of a compilation unit, a conditional compilation symbol is undefined unless it has been explicitly defined by an external mechanism (such as a command-line compiler option). When a `#define` directive is processed, the conditional compilation symbol named in that directive becomes defined in that compilation unit. The symbol remains defined until a `#undef` directive for that same symbol is processed, or until the end of the compilation unit is reached. An implication of this is that `#define` and `#undef` directives in one compilation unit have no effect on other compilation units in the same program.

When referenced in a pre-processing expression ([§6.5.3](lexical-structure.md#653-pre-processing-expressions)), a defined conditional compilation symbol has the Boolean value `true`, and an undefined conditional compilation symbol has the Boolean value `false`. There is no requirement that conditional compilation symbols be explicitly declared before they are referenced in pre-processing expressions. Instead, undeclared symbols are simply undefined and thus have the value `false`.

The namespace for conditional compilation symbols is distinct and separate from all other named entities in a C# program. Conditional compilation symbols can only be referenced in `#define` and `#undef` directives and in pre-processing expressions.

### 6.5.3 Pre-processing expressions

Pre-processing expressions can occur in `#if` and `#elif` directives. The operators `!`, `==`, `!=`, `&&`, and `||` are permitted in pre-processing expressions, and parentheses may be used for grouping.

```ANTLR
fragment PP_Expression
    : PP_Whitespace? PP_Or_Expression PP_Whitespace?
    ;
    
fragment PP_Or_Expression
    : PP_And_Expression (PP_Whitespace? '||' PP_Whitespace? PP_And_Expression)*
    ;
    
fragment PP_And_Expression
    : PP_Equality_Expression (PP_Whitespace? '&&' PP_Whitespace?
      PP_Equality_Expression)*
    ;

fragment PP_Equality_Expression
    : PP_Unary_Expression (PP_Whitespace? ('==' | '!=') PP_Whitespace?
      PP_Unary_Expression)*
    ;
    
fragment PP_Unary_Expression
    : PP_Primary_Expression
    | '!' PP_Whitespace? PP_Unary_Expression
    ;
    
fragment PP_Primary_Expression
    : TRUE
    | FALSE
    | PP_Conditional_Symbol
    | '(' PP_Whitespace? PP_Expression PP_Whitespace? ')'
    ;
```

When referenced in a pre-processing expression, a defined conditional compilation symbol has the Boolean value `true`, and an undefined conditional compilation symbol has the Boolean value `false`.

Evaluation of a pre-processing expression always yields a Boolean value. The rules of evaluation for a pre-processing expression are the same as those for a constant expression ([§11.20](expressions.md#1120-constant-expressions)), except that the only user-defined entities that can be referenced are conditional compilation symbols.

### 6.5.4 Definition directives

The definition directives are used to define or undefine conditional compilation symbols.

```ANTLR
fragment PP_Declaration
    : 'define' PP_Whitespace PP_Conditional_Symbol
    | 'undef' PP_Whitespace PP_Conditional_Symbol
    ;
```

The processing of a `#define` directive causes the given conditional compilation symbol to become defined, starting with the source line that follows the directive. Likewise, the processing of a `#undef` directive causes the given conditional compilation symbol to become undefined, starting with the source line that follows the directive.

Any `#define` and `#undef` directives in a compilation unit shall occur before the first *token* ([§6.4](lexical-structure.md#64-tokens)) in the compilation unit; otherwise a compile-time error occurs. In intuitive terms, `#define` and `#undef` directives shall precede any “real code” in the compilation unit.

> *Example*: The example:
>
> ```csharp
> #define Enterprise
> #if Professional || Enterprise
> #define Advanced
> #endif
> namespace Megacorp.Data
> {
> #if Advanced
>     class PivotTable {...}
> #endif
> }
> ```
>
> is valid because the `#define` directives precede the first token (the `namespace` keyword) in the compilation unit.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The following example results in a compile-time error because a #define follows real code:
>
> ```csharp
> #define A
> namespace N
> {
> #define B
> #if B
>     class Class1 {}
> #endif
> }
> ```
>
> *end example*

A `#define` may define a conditional compilation symbol that is already defined, without there being any intervening `#undef` for that symbol.

> *Example*: The example below defines a conditional compilation symbol A and then defines it again.
>
> ```csharp
> #define A
> #define A
> ```
>
> For compilers that allow conditional compilation symbols to be defined as compilation options, an alternative way for such redefinition to occur is to define the symbol as a compiler option as well as in the source.
>
> *end example*

A `#undef` may “undefine” a conditional compilation symbol that is not defined.

> *Example*: The example below defines a conditional compilation symbol `A` and then undefines it twice; although the second `#undef` has no effect, it is still valid.
>
> ```csharp
> #define A
> #undef A
> #undef A
> ```
>
> *end example*

### 6.5.5 Conditional compilation directives

The conditional compilation directives are used to conditionally include or exclude portions of a compilation unit.

```ANTLR
fragment PP_Conditional
    : PP_If_Section
    | PP_Elif_Section
    | PP_Else_Section
    | PP_Endif
    ;

fragment PP_If_Section
    : 'if' PP_Whitespace PP_Expression
    ;
    
fragment PP_Elif_Section
    : 'elif' PP_Whitespace PP_Expression
    ;
    
fragment PP_Else_Section
    : 'else'
    ;
    
fragment PP_Endif
    : 'endif'
    ;
```

Conditional compilation directives shall be written in groups consisting of, in order, a `#if` directive, zero or more `#elif` directives, zero or one `#else` directive, and a `#endif` directive. Between the directives are ***conditional sections*** of source code. Each section is controlled by the immediately preceding directive. A conditional section may itself contain nested conditional compilation directives provided these directives form complete groups.

> *Example*: The following example illustrates how conditional compilation directives can nest:
>
> ```csharp
> #define Debug // Debugging on
> #undef Trace // Tracing off
> class PurchaseTransaction
> {
>     void Commit()
>     {
> #if Debug
>         CheckConsistency();
>     #if Trace
>         WriteToLog(this.ToString());
>     #endif
> #endif
>         CommitHelper();
>     }
>     ...
> }
> ```
>
> *end example*

At most one of the contained conditional sections is selected for normal lexical processing:

- The *PP_Expression*s of the `#if` and `#elif` directives are evaluated in order until one yields `true`. If an expression yields `true`, the conditional section following  the corresponding directive is selected.
- If all *PP_Expression*s yield `false`, and if a `#else` directive is present, the conditional section following the `#else` directive is selected.
- Otherwise, no conditional section is selected.

The selected conditional section, if any, is processed as a normal *input_section*: the source code contained in the section shall adhere to the lexical grammar; tokens are generated from the source code in the section; and pre-processing directives in the section have the prescribed effects.

Any remaining conditional sections are skipped and no tokens, except those for pre-processing directives, are generated from the source code. Therefore skipped source code, except pre-processing directives, may be lexically incorrect. Skipped pre-processing directives shall be lexically correct but are not otherwise processed. Within a conditional section that is being skipped any nested conditional sections (contained in nested `#if...#endif` constructs) are also skipped.

> *Note*: The above grammar does not capture the allowance that the conditional sections between the pre-processing directives may be malformed lexically. Therefore the grammar is not ANTLR-ready as it only supports lexically correct input. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The following example illustrates how conditional compilation directives can nest:
>
> ```csharp
> #define Debug // Debugging on
> #undef Trace // Tracing off
> class PurchaseTransaction
> {
>     void Commit()
>     {
> #if Debug
>         CheckConsistency();
>     #if Trace
>         WriteToLog(this.ToString());
>     #endif
> #endif
>         CommitHelper();
>     }
>     ...
> }
> ```
>
> Except for pre-processing directives, skipped source code is not subject to lexical analysis. For example, the following is valid despite the unterminated comment in the `#else` section:
>
> ```csharp
> #define Debug // Debugging on
> class PurchaseTransaction
> {
>     void Commit()
>     {
> #if Debug
>         CheckConsistency();
> #else
>         /* Do something else
> #endif
>     }
>     ...
> }
> ```
>
> Note, however, that pre-processing directives are required to be lexically correct even in skipped sections of source code.
>
> Pre-processing directives are not processed when they appear inside multi-line input elements. For example, the program:
>
> ```csharp
> class Hello
> {
>     static void Main()
>     {
>         System.Console.WriteLine(@"hello,
> #if Debug
>         world
> #else
>         Nebraska
> #endif
>         ");
>     }
> }
> ```
>
> results in the output:
>
> ```console
> hello,
> #if Debug
>     world
> #else
>     Nebraska
> #endif
> ```
>
> In peculiar cases, the set of pre-processing directives that is processed might depend on the evaluation of the *pp_expression*. The example:
>
> ```csharp
> #if X
>     /*
> #else
>     /* */ class Q { }
> #endif
> ```
>
> always produces the same token stream (`class` `Q` `{` `}`), regardless of whether or not `X` is defined. If `X` is defined, the only processed directives are `#if` and `#endif`, due to the multi-line comment. If `X` is undefined, then three directives (`#if`, `#else`, `#endif`) are part of the directive set.
>
> *end example*

### 6.5.6 Diagnostic directives

The diagnostic directives are used to generate explicitly error and warning messages that are reported in the same way as other compile-time errors and warnings.

```ANTLR
fragment PP_Diagnostic
    : 'error' PP_Message?
    | 'warning' PP_Message?
    ;

fragment PP_Message
    : PP_Whitespace Input_Character*
    ;
```

> *Example*: The example
>
> ```csharp
> #if Debug && Retail
>     #error A build can't be both debug and retail
> #endif
> class Test {...}
> ```
>
> produces a compile-time error (“A build can’t be both debug and retail”) if the conditional compilation symbols `Debug` and `Retail` are both defined. Note that a *PP_Message* can contain arbitrary text; specifically, it need not contain well-formed tokens, as shown by the single quote in the word `can't`.
>
> *end example*

### 6.5.7 Region directives

The region directives are used to mark explicitly regions of source code.

```ANTLR
fragment PP_Region
    : PP_Start_Region
    | PP_End_Region
    ;

fragment PP_Start_Region
    : 'region' PP_Message?
    ;

fragment PP_End_Region
    : 'endregion' PP_Message?
    ;
```

No semantic meaning is attached to a region; regions are intended for use by the programmer or by automated tools to mark a section of source code. There must be one `#endregion` directive matching every `#region` directive. The message specified in a `#region` or `#endregion` directive likewise has no semantic meaning; it merely serves to identify the region. Matching `#region` and `#endregion` directives may have different *PP_Message*s.

The lexical processing of a region:

```csharp
#region
...
#endregion
```

corresponds exactly to the lexical processing of a conditional compilation directive of the form:

```csharp
#if true
...
#endif
```

> *Note*: This means that a region can include one or more `#if`/.../`#endif`, or be contained with a conditional section within a  `#if`/.../`#endif`; but a region cannot overlap with an just part of an `#if`/.../`#endif`, or start & end in different conditional sections. *end note*

### 6.5.8 Line directives

Line directives may be used to alter the line numbers and compilation unit names that are reported by the compiler in output such as warnings and errors. These values are also used by caller-info attributes ([§21.5.5](attributes.md#2155-caller-info-attributes)).

> *Note*: Line directives are most commonly used in meta-programming tools that generate C# source code from some other text input. *end note*

```ANTLR
fragment PP_Line
    : 'line' PP_Whitespace PP_Line_Indicator
    ;

fragment PP_Line_Indicator
    : Decimal_Digit+ PP_Whitespace PP_Compilation_Unit_Name
    | Decimal_Digit+
    | DEFAULT
    | 'hidden'
    ;
    
fragment PP_Compilation_Unit_Name
    : '"' PP_Compilation_Unit_Name_Character+ '"'
    ;
    
fragment PP_Compilation_Unit_Name_Character
    // Any Input_Character except "
    : ~('\u000D' | '\u000A'   | '\u0085' | '\u2028' | '\u2029' | '#')
    ;
```

When no `#line` directives are present, the compiler reports true line numbers and compilation unit names in its output. When processing a `#line` directive that includes a *Line_Indicator* that is not `default`, the compiler treats the line *after* the directive as having the given line number (and compilation unit name, if specified).

A `#line default` directive undoes the effect of all preceding `#line` directives. The compiler reports true line information for subsequent lines, precisely as if no `#line` directives had been processed.

A `#line hidden` directive has no effect on the compilation unit and line numbers reported in error messages, or produced by use of `CallerLineNumberAttribute` ([§21.5.5.2](attributes.md#21552-the-callerlinenumber-attribute)). It is intended to affect source-level debugging tools so that, when debugging, all lines between a `#line` hidden directive and the subsequent `#line` directive (that is not `#line hidden`) have no line number information, and are skipped entirely when stepping through code.

> *Note*: Although a *Compilation_Unit_Name* might contain text that looks like an escape sequence, such text is not an escape sequence; in this context a ‘`\`’ character simply designates an ordinary backslash character. *end note*

### 6.5.9 Pragma directives

The `#pragma` preprocessing directive is used to specify contextual information to a compiler.

> *Note*: For example, a compiler might provide `#pragma` directives that
>
> - Enable or disable particular warning messages when compiling subsequent code.
> - Specify which optimizations to apply to subsequent code.
> - Specify information to be used by a debugger.
>
> *end note*

```ANTLR
fragment PP_Pragma
    : 'pragma' PP_Pragma_Text?
    ;

fragment PP_Pragma_Text
    : PP_Whitespace Input_Character*
    ;
```

The *Input_Character*s in the *PP_Pragma_Text* are interpreted by the compiler in an implementation-defined manner. The information supplied in a `#pragma` directive shall not change program semantics. A `#pragma` directive shall only change compiler behavior that is outside the scope of this language specification. If the compiler cannot interpret the *Input_Character*s, the compiler can produce a warning; however, it shall not produce a compile-time error.

> *Note*: *PP_Pragma_Text* can contain arbitrary text; specifically, it need not contain well-formed tokens. *end note*
