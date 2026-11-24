# 11 Patterns and pattern matching

## 11.1 General

A ***pattern*** may be used with the `is` operator ([§12.14.12](expressions.md#121412-the-is-operator)), in a *switch_statement* ([§13.8.3](statements.md#1383-the-switch-statement)), and in a *switch_expression* ([§12.11](expressions.md#1211-switch-expression)) to describe the shape of data against which incoming data is to be compared. Patterns may be nested, with parts of the data being matched against ***sub-patterns***.

A pattern is tested against a value in a number of contexts:

- In a *switch_statement*, the *pattern* of a *switch_label* is tested against the *selector_expression* of the *switch_statement*.
- With an *is-pattern* operator, the *pattern* on the right-hand-side is tested against the expression on the left.
- In a *switch_expression*, the *pattern* of a *switch_expression_arm* is tested against the expression on the *switch_expression*’s left-hand-side.
- In nested contexts, the *sub-pattern* is tested against values retrieved from properties, fields, or indexed from other input values, depending on the pattern form.

The value against which a pattern is tested is called the ***pattern input value***.

A pattern `P` is *subsumed* by set of unguarded patterns `Q` if any input value matched by `P` is matched by one of the members of `Q`.

In a switch statement ([§13.8.3](statements.md#1383-the-switch-statement)), it is an error if a case’s pattern is *subsumed* by the preceding set of *unguarded* ([§13.8.3](statements.md#1383-the-switch-statement)) cases. In a switch expression ([§12.11](expressions.md#1211-switch-expression)), it is an error if a *switch_expression_arm*’s pattern is *subsumed* by the preceding set of *unguarded* *switch_expression_arm*s’ patterns.

A set of patterns is exhaustive if, for every possible input value, some pattern in the set is applicable. When an implementation detects that a set of patterns is not exhaustive, it shall issue a warning.

## 11.2 Pattern forms

### 11.2.1 General

A pattern may have one of the following forms:

```ANTLR
pattern
    : logical_pattern
    ;

primary_pattern
    : parenthesized_pattern
    | declaration_pattern
    | constant_pattern
    | var_pattern
    | positional_pattern
    | property_pattern
    | discard_pattern
    | type_pattern
    | relational_pattern
    ;

parenthesized_pattern
    : '(' pattern ')'
    ;
```

For a *parenthesized_pattern* `(P)`, the *input type* of `P` is the input type of the parenthesized pattern. The parenthesized pattern is applicable to a type if and only if `P` is applicable to that type, matches a value if and only if `P` matches that value, and has the same *narrowed type* as `P`. Its definite-assignment effects are specified in [§9.4.4.34](variables.md#94434-is-pattern-expressions).

When recognising the *type* of a *declaration_pattern*, *type_pattern*, *positional_pattern*, or *property_pattern*, a token sequence beginning with `(` is recognised as a *type* only if its initial *tuple_type* is immediately followed by `[` starting an array rank specifier. This restriction also applies to the *type* following `is` ([§12.14.1](expressions.md#12141-general)).

A form consisting of `(`, exactly one *subpattern* of the form *pattern* (without an *identifier* `:` prefix), and `)`, with no preceding *type*, following *property_subpattern*, or following *simple_designation*, is recognised as a *parenthesized_pattern* rather than a *positional_pattern*. If its enclosed pattern is a *constant_pattern*, the form is instead recognised as a *constant_pattern* whose *constant_expression* is parenthesized.

If the input can be syntactically recognised as both a *constant_pattern* and a *positional_pattern* then the *constant_pattern* shall be chosen.

> *Note*: These restrictions concern syntactic recognition, not whether the type denoted by a name is a tuple type. Thus `ValueTuple<int, int> pair`, an eligible alias to that named type followed by a designation, `(int, int)[] pairs`, and `List<(int, int)> pairs` can be declaration patterns. The form `(1)` is a constant pattern, whereas `(var item)` is a parenthesized pattern. The forms `(var item) whole`, `(value: var item)`, and `(var item) {}` are positional patterns. *end note*

The `'(' pattern ')'` production allows a pattern to be enclosed in parentheses to enforce the order of evaluation among patterns combined using one of the *logical_pattern*s.

> *Example*: A parenthesized pattern preserves narrowing to the right-hand pattern of a conjunction. In the following example, `text` has type `string`, so its `Length` property can be accessed:
>
> <!-- Example: {template:"standalone-console", name:"ParenthesizedPatternNarrowing", inferOutput:true} -->
> ```csharp
> object value = "abc";
> Console.WriteLine(value is (string) and { Length: 3 });
> if (value is ((string)) and var text)
> {
>     Console.WriteLine(text.Length);
> }
> value = 42;
> Console.WriteLine(value is (string));
> ```
>
> The output produced is:
>
> ```console
> True
> 3
> False
> ```
>
> *end example*

Some *pattern*s can result in the declaration of a local variable.

Each pattern form defines the set of types for input values that the pattern may be applied to. A pattern `P` is *applicable to* a type `T` if `T` is among the types whose values the pattern may match. It is a compile-time error if a pattern `P` appears in a program to match a pattern input value ([§11.1](patterns.md#111-general)) of type `T` if `P` is not applicable to `T`.

> *Example*: The following example generates a compile-time error because the compile-time type of `v` is `TextReader`. A variable of type `TextReader` can never have a value that is reference-compatible with `string`:
>
> <!-- Example: {template:"standalone-console", name:"PatternFormGen1", expectedWarnings:["CS0184"]} -->
> ```csharp
> TextReader v = Console.In; // compile-time type of 'v' is 'TextReader'
> if (v is string) // compile-time error
> {
>     // code assuming v is a string
> }
> ```
>
> However, the following does not generate a compile-time error because the compile-time type of `v` is `object`. A variable of type `object` could have a value that is reference-compatible with `string`:
>
> <!-- Example: {template:"standalone-console", name:"PatternFormGen2"} -->
> ```csharp
> object v = Console.In;
> if (v is string s)
> {
>     // code assuming v is a string
> }
> ```
>
> *end example*

Each pattern form defines the set of values for which the pattern *matches* the value at runtime.

The order of evaluation of operations and side effects during pattern-matching (calls to `Deconstruct`, property accesses, and invocations of members of `System.Runtime.CompilerServices.ITuple`) is not specified.

### 11.2.2 Constant pattern

A *constant_pattern* is used to test the value of a pattern input value ([§11.1](patterns.md#111-general)) against the given constant value.

```ANTLR
constant_pattern
    : constant_expression
    ;
```

A constant pattern `P` is *applicable to* a type `T` if there is an implicit conversion from the constant expression of `P` to the type `T`.

For a constant pattern `P`, its *converted value* is

- if the pattern input value’s type is an integral type or an enum type, the pattern’s constant value converted to that type; otherwise
- if the pattern input value’s type is the nullable version of an integral type or an enum type, the pattern’s constant value converted to its underlying type; otherwise
- the value of the pattern’s constant value.

Given a pattern input value *e* and a constant pattern `P` with converted value *v*,

- if *e* has integral type or enum type, or a nullable form of one of those, and *v* has integral type, the pattern `P` *matches* the value *e* if result of the expression `e == v` is `true`; otherwise
- the pattern `P` *matches* the value *e* if `object.Equals(e, v)` returns `true`.

> *Example*: The `switch` statement in the following method uses five constant patterns in its case labels.
>
> <!-- Example: {template:"standalone-console", name:"ConstantPattern1", replaceEllipsis:true, customEllipsisReplacements: ["\"xxx\""], ignoredWarnings:["CS8321"]} -->
> ```csharp
> static decimal GetGroupTicketPrice(int visitorCount)
> {
>     switch (visitorCount) 
>     {
>         case 1: return 12.0m;
>         case 2: return 20.0m;
>         case 3: return 27.0m;
>         case 4: return 32.0m;
>         case 0: return 0.0m;
>         default: throw new ArgumentException(...);
>     }
> }
> ```
>
> *end example*

### 11.2.3 Declaration pattern

A *declaration_pattern* is used to test that a value has a given type and, if the test succeeds, to optionally provide the value in a variable of that type.

```ANTLR
declaration_pattern
    : type simple_designation
    ;
simple_designation
    : discard_designation
    | single_variable_designation
    ;
discard_designation
    : '_'
    ;
single_variable_designation
    : identifier
    ;
```

The recognition of *type* is subject to [§11.2.1](patterns.md#1121-general).

When recognising a *simple_designation* if both the *discard_designation* and *single_variable_designation* alternatives are applicable then the former shall be chosen.

> *Note*: ANTLR makes the specified choice automatically due to the ordering of the alternatives of *simple_designation*. *end note*

A *declaration_pattern* cannot be used to test that a value has a type named `var` unless that type is referenced using an identifier containing a unicode character escape sequence ([§6.4.2](lexical-structure.md#642-unicode-character-escape-sequences)) or represented by an *Escaped_Identifier* ([§6.4.3](lexical-structure.md#643-identifiers)).

> *Example*: Given a type named `var`, the following illustrates the distinction between is-type syntax, an erroneous pattern, and valid *declaration_pattern* spellings.
>
> The `is var` construct (no designation) is the is-type operator ([§12.14.12.1](expressions.md#1214121-the-is-type-operator)) and tests whether the operand’s runtime type is `var`:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"VarTypeName1", ignoredWarnings:["CS8981"]} -->
> ```csharp
> #pragma warning disable CS8981
> class var { }
> class C
> {
>     static bool M(object o) => o is var; // is-type check; o is var (the type)
> }
> ```
>
> Writing `is var y` as a pattern is a compile-time error when a type named `var` is in scope:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"VarTypeName2", expectedErrors:["CS8508"], ignoredWarnings:["CS8981"]} -->
> ```csharp
> #pragma warning disable CS8981
> class var { }
> class C
> {
>     static void M(object o)
>     {
>         if (o is var y) { } // error CS8508: 'var' refers to the in-scope type
>     }
> }
> ```
>
> Using an *Escaped_Identifier* (`@var`) or a unicode-escape spelling (`v\u0061r`) as the type in a *declaration_pattern* is valid and matches only instances of the type named `var`:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"VarTypeName3", ignoredWarnings:["CS8981"]} -->
> ```csharp
> #pragma warning disable CS8981
> class var { }
> class C
> {
>     static void M(object o)
>     {
>         // declaration_pattern; matches instances of type 'var'
>         if (o is @var y) { }
>
>         // declaration_pattern; same type, unicode-escape spelling
>         if (o is v\u0061r z) { }
>     }
> }
> ```
>
> *end example*

The *type* of a *declaration_pattern* cannot be `dynamic`.

It is a compile-time error if the *type* is a nullable value type ([§8.3.12](types.md#8312-nullable-value-types)) or a nullable reference type ([§8.9.3](types.md#893-nullable-reference-types)).

The runtime type of the value is tested against the *type* in the pattern using the same rules specified in the is-type operator ([§12.14.12.1](expressions.md#1214121-the-is-type-operator)). If the test succeeds, the pattern *matches* that value.

> *Note*: The is-type expression `e is T` and the declaration pattern `e is T _` are equivalent when both are valid. *end note*

Given a pattern input value ([§11.1](patterns.md#111-general)) *e*, if the *simple_designation* is a *discard_designation*, denoting a discard ([§9.2.9.2](variables.md#9292-discards)), the value of *e* is not bound to anything. Otherwise, if the *simple_designation* is a *single_variable_designation*, a local variable ([§9.2.9](variables.md#929-local-variables)) of the given type named by the given identifier is introduced. That local variable is assigned the value of the pattern input value when the pattern *matches* the value.

> *Note*: This treatment of `_` within a *declaration_pattern* differs from that of a standalone `_` written as a *pattern* ([§11.2.7](patterns.md#1127-discard-pattern)): in the latter case, an in-scope constant or type named `_`, if any, is *not* hidden. *end note*

A type `E` is said to be ***pattern compatible*** with the type `T` if there exists an identity conversion, an implicit or explicit reference conversion, a boxing conversion, an unboxing conversion, or an implicit or explicit nullable value type conversion from `E` to `T`, or if either `E` or `T` is an open type ([§8.4.3](types.md#843-open-and-closed-types)). A declaration pattern naming a type `T` is *applicable to* ([§11.2.1](patterns.md#1121-general)) every type `E` for which `E` is pattern compatible with `T`. It is a compile-time error if a declaration pattern naming a type `T` is used to match a pattern input value ([§11.1](patterns.md#111-general)) whose static type `E` is not pattern compatible with `T`.

> *Note*: The support for open types can be most useful when checking types that may be either struct or class types, and boxing is to be avoided. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The declaration pattern is useful for performing run-time type tests of reference types, and replaces the idiom
>
> ```csharp
> var v = expr as Type;
> if (v != null) { /* code using v */ }
> ```
>
> with the slightly more concise
>
> ```csharp
> if (expr is Type v) { /* code using v */ }
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The declaration pattern can be used to test values of nullable types: a value of type `Nullable<T>` (or a boxed `T`) matches a type pattern `T2 id` if the value is non-null and `T2` is `T`, or some base type or interface of `T`. For example, in the code fragment
>
> <!-- Example: {template:"standalone-console-without-using", name:"DeclarationPattern1"} -->
> ```csharp
> int? x = 3;
> if (x is int v) { /* code using v */ }
> ```
>
> The condition of the `if` statement is `true` at runtime and the variable `v` holds the value `3` of type `int` inside the block. After the block the variable `v` is in scope, but not definitely assigned. *end example*

### 11.2.4 Var pattern

A *var_pattern* *matches* every value. That is, a pattern-matching operation with a *var_pattern* always succeeds.

A *var_pattern* is *applicable to* every type.

A *var_pattern* cannot be used when there is an in-scope type named `var`.

> *Note*: An escaped identifier such as `@var` or a unicode-escape spelling such as `v\u0061r` names the in-scope type in a *declaration_pattern* but does not spell the contextual `var` token of a *var_pattern*. The patterns `@var y` and `v\u0061r y` are therefore *declaration_pattern*s ([§11.2.3](patterns.md#1123-declaration-pattern)) that match only values whose runtime type is compatible with the in-scope type named `var`. *end note*

```ANTLR
var_pattern
    : 'var' designation
    ;
designation
    : simple_designation
    | tuple_designation
    ;
tuple_designation
    : '(' designations? ')'
    ;
designations
    : designation (',' designation)*
    ;
```

Given a pattern input value ([§11.1](patterns.md#111-general)) *e*, if *designation* is *discard_designation*, it denotes a discard ([§9.2.9.2](variables.md#9292-discards)), and the value of *e* is not bound to anything. (Although a declared variable with that name may be in scope at that point, that named variable is not seen in this context.) Otherwise, if *designation* is *single_variable_designation*, at runtime the value of *e* is bound to a newly introduced local variable ([§9.2.9](variables.md#929-local-variables)) of that name whose type is the static type of *e*, and the pattern input value is assigned to that local variable.

If *designation* is a *tuple_designation*, the pattern is equivalent to a *positional_pattern* ([§11.2.5](patterns.md#1125-positional-pattern)) of the form `(var` *designation*, … `)` where the *designation*s are those found within the *tuple_designation*.  For example, the pattern `var (x, (y, z))` is equivalent to `(var x, (var y, var z))`.

### 11.2.5 Positional pattern

A *positional_pattern* checks that the input value is not `null`, extracts a sequence of values from it, and matches each extracted value against a corresponding *subpattern*. The values are extracted in one of three ways: by treating the input as a tuple, by invoking a `Deconstruct` method, or by indexing the input through `System.Runtime.CompilerServices.ITuple`.

> *Note*: The use of `Deconstruct` here is distinct from the source-level deconstruction transformation defined in [§12.7](expressions.md#127-deconstruction). *end note*

```ANTLR
positional_pattern
    : type? '(' subpatterns? ')' property_subpattern? simple_designation?
    ;
subpatterns
    : subpattern (',' subpattern)*
    ;
subpattern
    : pattern
    | identifier ':' pattern
    ;
```

The recognition of *type* and of a *positional_pattern* is subject to [§11.2.1](patterns.md#1121-general).

Given a match of an input value to the pattern *type* `(` *subpatterns* `)`, a method is selected by searching in *type* for accessible declarations of `Deconstruct` and selecting one among them using the same rules as for the deconstructing assignment ([§12.23.3](expressions.md#12233-deconstructing-assignment)).

> *Note*: A tuple literal can be matched by patterns of several different forms, which are not interchangeable:
>
> - `(int, int) x` is a *positional_pattern* with two *type_pattern* subpatterns and the outer *simple_designation* `x`.
> - `ValueTuple<int, int> x` is a *declaration_pattern*.
> - `(int A, int B) x` is a *positional_pattern* with two *declaration_pattern* subpatterns introducing locals `A` and `B`, and the outer *simple_designation* `x`.
> - `var (x, y)` is a *var_pattern* with a *tuple_designation*.
> - `(int x, int y)` is a *positional_pattern*.
> - `(int, int) (x, y)` is not a valid pattern.
>
> *end note*

In order to extract the values to match against the patterns in the list, let *D* be *type* if present; otherwise let *D* be the static type of the input value, with a nullable value type replaced by its underlying type.

1. **Tuple form.** If *type* is omitted and *D* is a tuple type ([§8.3.11](types.md#8311-tuple-types)) or if the input value is a tuple literal ([§12.8.6](expressions.md#1286-tuple-literals)), then this case applies. It is a compile-time error if *n* is not equal to the arity of that tuple type. At runtime, each tuple element of the non-null input value, unwrapped if necessary, is matched against the corresponding *subpattern*; the match succeeds if all of these succeed. If any *subpattern* has an *identifier*, that *identifier* shall name the tuple element at the corresponding position in the tuple type.
2. **Deconstruct form.** Otherwise, if either *type* is present, or *type* is omitted and *D* contains an accessible `Deconstruct` method ([§12.7](expressions.md#127-deconstruction)), then this case applies. A `Deconstruct` method is selected from *D* using the same overload-resolution rules as for a deconstructing assignment ([§12.23.3](expressions.md#12233-deconstructing-assignment)), with the additional requirement that its number of `out` parameters is equal to *n*; it is a compile-time error if no such method exists. If *type* is present, it is a compile-time error if the static type of the input value is not pattern compatible ([§11.2.3](patterns.md#1123-declaration-pattern)) with *type*; at runtime the input value is tested against *type* and, if that test fails, the positional pattern match fails. Otherwise, the input value is converted to *D* and the selected `Deconstruct` method is invoked with fresh variables receiving its `out` parameters. Each received value is matched against the corresponding *subpattern*, and the match succeeds if all of these succeed. If any *subpattern* has an *identifier*, that *identifier* shall name the parameter at the corresponding position of `Deconstruct`.
3. **ITuple form.** Otherwise, if *type* is omitted, no *subpattern* has an *identifier*, and the static type of the input value is `object`, `System.Runtime.CompilerServices.ITuple`, or a type that has an implicit reference conversion to `System.Runtime.CompilerServices.ITuple`, then this case applies. At runtime, the input value is tested for being a non-`null` instance of `System.Runtime.CompilerServices.ITuple`; if that test fails, the positional pattern match fails. Otherwise, the value’s `Length` property is read and, if it is not equal to *n*, the positional pattern match fails. Otherwise, for each *i* from 1 to *n*, the value obtained by indexing the input value with *i* − 1 is matched against the *i*-th *subpattern*, and the match succeeds if all of these succeed.
4. Otherwise, no case applies and the *positional_pattern* is a compile-time error.

In the tuple and Deconstruct forms, if *simple_designation* is a *single_variable_designation*, it introduces a local variable ([§9.2.9](variables.md#929-local-variables)) of type *D*. When the pattern matches, the variable is assigned the matched input value, converted to *D*. Tuple element names are those of *D*, rather than the names of variables declared in subpatterns. A *discard_designation* introduces no variable and does not bind the input value. Definite-assignment effects are specified in [§9.4.4.34](variables.md#94434-is-pattern-expressions).

The order in which subpatterns are matched at runtime is unspecified, and a failed match might not attempt to match all subpatterns.

> *Example*: Bare tuple-looking syntax matches the elements, rather than testing for the corresponding tuple type. An outer designation preserves the input tuple's names and unwraps a nullable tuple:
>
> <!-- Example: {template:"standalone-console", name:"TuplePatternRecognition", inferOutput:true} -->
> ```csharp
> object value = ((object)1, (object)"text");
> Console.WriteLine(value is (int, string));
> Console.WriteLine(value is ValueTuple<int, string>);
>
> (int Left, int Right)? pair = (67, 69);
> if (pair is (int A, int B) captured)
> {
>     Console.WriteLine($"{A},{B};{captured.Left},{captured.Right}");
> }
> ```
>
> The output produced is:
>
> ```console
> True
> False
> 67,69;67,69
> ```
>
> *end example*
>
> *Example*: An explicit pattern type determines the outer variable's type; deconstruction does not construct a tuple to assign to it:
>
> <!-- Example: {template:"standalone-console", name:"PositionalPatternOuterType", inferOutput:true} -->
> ```csharp
> object value = new Point(1, 2);
> if (value is Point(var x, var y) captured)
> {
>     Console.WriteLine($"{ReferenceEquals(value, captured)}:{x + y}:{captured.X + captured.Y}");
> }
>
> class Point
> {
>     public int X { get; }
>     public int Y { get; }
>     public Point(int x, int y) => (X, Y) = (x, y);
>     public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
> }
> ```
>
> The output produced is:
>
> ```console
> True:3:3
> ```
>
> *end example*
>
> *Example*: Here, we deconstruct an expression result and match the resulting values against the corresponding nested patterns:
>
> <!-- Example: {template:"standalone-console-without-using", name:"PositionalPattern1", ignoredWarnings:["CS8321"]} -->
> ```csharp
> static string Classify(Point point) => point switch
> {
>     (0, 0) => "Origin",
>     (1, 0) => "positive X basis end",
>     (0, 1) => "positive Y basis end",
>     _ => "Just a point",
> };
> 
> public readonly struct Point
> {
>     public int X { get; }
>     public int Y { get; }
>     public Point(int x, int y) => (X, Y) = (x, y);
>     public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
> }
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The names of tuple elements and Deconstruct parameters can be used in a positional pattern, as follows:
>
> <!-- Example: {template:"standalone-console", name:"PositionalPattern2", ignoredWarnings:["CS8321"], inferOutput:true} -->
> ```csharp
> var numbers = new List<int> { 10, 20, 30 };
> if (SumAndAverage(numbers) is (Sum: var sum, Average: var average))
> {
>     Console.WriteLine(
>         $"Sum of [{string.Join(" ", numbers)}] is {sum}; average is {average}");
> }
> else
> {
>     // Note: sum and average are in scope here, but not definitely assigned
>     Console.WriteLine("No numbers provided to compute sum and average.");   
> }
>
> static (double Sum, double Average)? SumAndAverage(IEnumerable<int> numbers)
> {
>     int sum = 0;
>     int count = 0;
>     foreach (int number in numbers)
>     {
>         sum += number;
>         count++;
>     }
>     return count == 0 ? null : (sum, sum / count);
> }
> ```
>
> The output produced is
>
> ```console
> Sum of [10 20 30] is 60; average is 20
> ```
>
> *end example*

### 11.2.6 Property pattern

A *property_pattern* checks that the input value is not `null`, and recursively matches values extracted by the use of accessible properties or fields.

```ANTLR
property_pattern
    : type? property_subpattern simple_designation?
    ;
property_subpattern
    : '{' '}'
    | '{' subpatterns ','? '}'
    ;
```

The recognition of the optional *type* is subject to [§11.2.1](patterns.md#1121-general).

It is an error if any *subpattern* of a *property_pattern* does not contain an *identifier*.

It is a compile-time error if the *type* is a nullable value type ([§8.3.12](types.md#8312-nullable-value-types)) or a nullable reference type ([§8.9.3](types.md#893-nullable-reference-types)).

> *Note*: A null-checking pattern falls out of a trivial property pattern. To check if the string `s` is non-null, one can write any of the following forms:
>
> <!-- Example: {template:"standalone-console", name:"PropertyPattern1", replaceEllipsis:true, customEllipsisReplacements: [";", ";", ";", ";"], ignoredWarnings:["CS0642"]} -->
> ```csharp
> #nullable enable
> string s = "abc";
> if (s is object o) ...  // o is of type object
> if (s is string x1) ... // x1 is of type string
> if (s is {} x2) ...     // x2 is of type string
> if (s is {}) ...
> ```
>
> The example declaring `x2` is similar to `if (s is var x2)` in terms of inferring the variable type, but the property pattern guarantees that `x2` is non-null.
> *end note*

Given a match of an expression *e* to the pattern *type* `{` *subpatterns* `}`, it is a compile-time error if the expression *e* is not pattern compatible ([§11.2.3](patterns.md#1123-declaration-pattern)) with the type *T* designated by *type*. If the type is absent, the type is assumed to be the static type of *e*. Each of the identifiers appearing on the left-hand-side of its *subpatterns* shall designate an accessible readable property or field of *T*. If the *simple_designation* of the *property_pattern* is present, it declares a pattern variable of type *T*.

At runtime, the expression is tested against *T*. If this fails then the property pattern match fails, and the result is `false`. If it succeeds, then each *property_subpattern* field or property is read, and its value matched against its corresponding pattern. The result of the whole match is `false` only if the result of any of these is `false`. The order in which subpatterns are matched is not specified, and a failed match may not test all subpatterns at runtime. If the match succeeds and the *simple_designation* of the *property_pattern* is a *single_variable_designation*, the declared variable is assigned the matched value.

The *property_pattern* may be used to pattern-match with anonymous types.

> *Example*:
>
> <!-- Example: {template:"standalone-console", name:"PropertyPattern2", replaceEllipsis:true, customEllipsisReplacements: ["new object()", ";"], ignoredWarnings:["CS0642"]} -->
> ```csharp
> var o = ...;
> if (o is string { Length: 5 } s) ...
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: A run-time type check and a variable declaration can be added to a property pattern, as follows:
>
> <!-- Example: {template:"standalone-console", name:"PropertyPattern3", inferOutput:true} -->
> ```csharp
> Console.WriteLine(TakeFive("Hello, world!"));
> Console.WriteLine(TakeFive("Hi!"));
> Console.WriteLine(TakeFive(new[] { '1', '2', '3', '4', '5', '6', '7' }));
> Console.WriteLine(TakeFive(new[] { 'a', 'b', 'c' }));
> 
> static string TakeFive(object input) => input switch
> {
>     string { Length: >= 5 } s => s.Substring(0, 5),
>     string s => s,
>     ICollection<char> { Count: >= 5 } chars => new string(chars.Take(5).ToArray()),
>     ICollection<char> chars => new string(chars.ToArray()),
>     null => throw new ArgumentNullException(nameof(input)),
>     _ => throw new ArgumentException("Not supported input type."),
> };
> ```
>
> The output produced is
>
> ```console
> Hello
> Hi!
> 12345
> abc
> ```
>
> *end example*

### 11.2.7 Discard pattern

Every expression matches the discard pattern, which results in the value of the expression being discarded.

```ANTLR
discard_pattern
    : '_'
    ;
```

Where the syntactic context permits a *pattern*, if the token `_` would resolve as a *simple_name* ([§12.8.4](expressions.md#1284-simple-names)) to an accessible constant or to a type, then `_` is *not* treated as a *discard_pattern*. Instead:

- If `_` resolves to an accessible constant, the `_` is interpreted as a *constant_pattern* ([§11.2.2](patterns.md#1122-constant-pattern)) whose constant expression is that constant.
- If `_` resolves to a type, then in the right-hand side of an `is` operator the construct *relational_expression* `is _` is interpreted as the is-type operator ([§12.14.12.1](expressions.md#1214121-the-is-type-operator)) testing against that type. In any other syntactic context that admits a *pattern*, a bare `_` resolving to a type is not by itself a valid *pattern*; however, `_` may appear as the *type* of a *declaration_pattern* (e.g., `_ x`) or in other pattern forms that explicitly name a type.

This rule preserves backward compatibility with code that defined `_` as a type or identifier prior to the introduction of the discard pattern. If `_` resolves to anything other than an accessible constant or type (for example, a local variable, parameter, field, or method), the rule does not apply and `_` remains a *discard_pattern*.

> *Note*: This is analogous to the rule for `var` in [§11.2.4](patterns.md#1124-var-pattern), except that for `_` an in-scope constant or type causes `_` to be interpreted as a reference to that declaration rather than producing an error. *end note*

If, after applying the preceding rule, the token `_` is still a *discard_pattern*, it is a compile-time error for that *discard_pattern* to appear as the entire *pattern* of a *relational_expression* of the form *relational_expression* `is` *pattern*, or as the entire *pattern* of a *switch_label*. A *discard_pattern* may, however, appear as a *subpattern* of an enclosing pattern (for example, as a *subpattern* of a *positional_pattern* or *property_pattern*).

> *Note*: In those cases, to match any expression, use a *var_pattern* with a discard `var _`. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*:
>
> <!-- Example: {template:"standalone-console", name:"DiscardPattern1", inferOutput:true} -->
> ```csharp
> Console.WriteLine(GetDiscountInPercent(DayOfWeek.Friday));
> Console.WriteLine(GetDiscountInPercent(null));
> Console.WriteLine(GetDiscountInPercent((DayOfWeek)10));
>
> static decimal GetDiscountInPercent(DayOfWeek? dayOfWeek) => dayOfWeek switch
> {
>     DayOfWeek.Monday => 0.5m,
>     DayOfWeek.Tuesday => 12.5m,
>     DayOfWeek.Wednesday => 7.5m,
>     DayOfWeek.Thursday => 12.5m,
>     DayOfWeek.Friday => 5.0m,
>     DayOfWeek.Saturday => 2.5m,
>     DayOfWeek.Sunday => 2.0m,
>     _ => 0.0m,
> };
> ```
>
> The output produced is
>
> ```console
> 5.0
> 0.0
> 0.0
> ```
>
> Here, a discard pattern is used to handle `null` and any integer value that does not have the corresponding member of the `DayOfWeek` enumeration. That guarantees that the `switch` expression handles all possible input values.
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The following illustrates how an in-scope constant named `_` changes the interpretation of an `_` arm in a `switch` expression. In `WithoutUnderscore`, the `_` arm is a *discard_pattern* and matches any value. In `WithUnderscore`, the in-scope constant `_` causes the `_` arm to be interpreted as a *constant_pattern* that matches only the value `0`.
>
> ```csharp
> static string WithoutUnderscore(int n) => n switch
> {
>     1 => "one",
>     _ => "other",
> };
>
> static string WithUnderscore(int n)
> {
>     const int _ = 0;
>     return n switch
>     {
>         1 => "one",
>         _ => "zero",
>         var x => "other: " + x,
>     };
> }
> ```
>
> *end example*

### §type-pattern-new-clause Type pattern

A *type_pattern* is used to test that the pattern input value ([§11.1](patterns.md#111-general)) has a given type.

```ANTLR
type_pattern
    : type
    ;
```

A type pattern naming a type `T` is *applicable to* every type `E` for which `E` is *pattern compatible* with `T` (§11.2.3).

The runtime type of the value is tested against *type* using the same rules specified in the is-type operator ([§12.14.12.1](expressions.md#1214121-the-is-type-operator)). If the test succeeds, the pattern matches that value. It is a compile-time error if the *type* is a nullable type. This pattern form never matches a `null` value.

### §relational-pattern-new-clause Relational pattern

A *relational_pattern* is used to relationally test the pattern input value ([§11.1](patterns.md#111-general)) against a constant value.

```ANTLR
relational_pattern
    : '<'  relational_expression
    | '<=' relational_expression
    | '>'  relational_expression
    | '>=' relational_expression
    ;
```

The *relational_expression* `E` in a *relational_pattern* is used to determine a converted constant value and a comparison type. A *default_literal* is not permitted as `E`, ignoring enclosing parentheses and null-forgiving operators.

For an input type `T`, determine an expression `R` as follows. For these rules, parentheses are ignored and user-defined conversions are not considered.

- If `T` is an open type ([§8.4.3](types.md#843-open-and-closed-types)), `R` is `E`, without a conversion to `T`.
- Otherwise, implicitly convert `E` to `T` as if it were the right operand of a simple assignment to a variable of type `T` ([§12.23.2](expressions.md#12232-simple-assignment)), and let `A` be the resulting expression. An implicit identity conversion between identical types leaves `E` unchanged.
  - If `T` is a nullable value type with underlying type `U`, and `A` is the result of an identity, enumeration, or nullable value type conversion and is not a null constant, `R` is the operand of that conversion converted to `U` by a standard conversion ([§10.4](conversions.md#104-standard-conversions)).
  - Otherwise, if `A` is the result of a boxing conversion ([§10.2.9](conversions.md#1029-boxing-conversions)) or an implicit reference conversion ([§10.2.8](conversions.md#1028-implicit-reference-conversions)), its operand has a constant value, and `A` does not have a constant value, `R` is that operand.
  - Otherwise, `R` is `A`.

The ***comparison type*** `C` is the type of `R`, and the converted constant value `v` is the constant value of `R`.

A *relational_pattern* is *applicable to* its input type `T` if all of the following conditions hold:

- The conversions required above exist, and `R` has a type and a constant value.
- `C` is one of `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `float`, `double`, `decimal`, `nint`, `nuint`, or an enum type.
- `T` is pattern compatible with `C` ([§11.2.3](patterns.md#1123-declaration-pattern)) and is neither an open class type nor an open ref struct type ([§16.2.3](structs.md#1623-ref-modifier)).
- If `T` is an enum type, `C` is an enum type.
- If an explicit nullable conversion ([§10.3.4](conversions.md#1034-explicit-nullable-conversions)) exists from `T` to `C`, the underlying type of `T` is identity convertible with `C`.

It is a compile-time error if `v` is `double.NaN`, `float.NaN`, or `null`.

At runtime, the input value is tested against `C` using the rules of the is-type operator ([§12.14.12.1](expressions.md#1214121-the-is-type-operator)). If the test fails, including when the input is `null`, the pattern does not match. Otherwise, let `e` be the input value, unwrapped or unboxed as necessary to obtain a value of type `C`. The pattern matches if the predefined comparison `e «op» v` evaluates to `true`, where «op» is the relational operator of the pattern. The comparison uses the predefined numeric or enumeration comparison operators ([§12.14](expressions.md#1214-relational-and-type-testing-operators)), including binary numeric promotion ([§12.4.7.3](expressions.md#12473-binary-numeric-promotions)) where required.

> *Example*: The constant is converted before comparing values of the comparison type. This differs from selecting a binary operator for the original input and constant types:
>
> <!-- Example: {template:"standalone-console", name:"RelationalPatternConversions", inferOutput:true} -->
> ```csharp
> int integer = 1;
> Console.WriteLine(integer < 2L);
> double number = 1;
> Console.WriteLine(number is < 2);
> int? optional = 1;
> Console.WriteLine(optional is < 2);
> optional = null;
> Console.WriteLine(optional is < 2);
> object boxed = (short)1;
> Console.WriteLine(boxed is < 3);
> Console.WriteLine(boxed is not < 3);
> Console.WriteLine(boxed is >= 3);
> ```
>
> The output produced is:
>
> ```console
> True
> True
> True
> False
> False
> True
> False
> ```
>
> The boxed `short` does not pass the type test for the comparison type `int`. Negating that failed match produces `true`; using `>=` instead still requires the type test and produces `false`.
>
> In contrast, the following relational patterns generate compile-time errors because their constants cannot be implicitly converted to the comparison types `int`, `byte`, and `float`, respectively:
>
> <!-- Example: {template:"standalone-console", name:"RelationalPatternConstantConversions", expectedErrors:["CS0266", "CS0031", "CS0664"], ignoredWarnings:["CS8794"]} -->
> ```csharp
> int integer = 1;
> Console.WriteLine(integer is < 2L);
> byte small = 1;
> Console.WriteLine(small is < 300);
> float single = 1;
> Console.WriteLine(single is < 2D);
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->

> *Example*:
>
> <!-- Example: {template:"standalone-console", name:"RelationalPattern1", inferOutput:true} -->
> ```csharp
> Console.WriteLine(Classify(13));
> Console.WriteLine(Classify(double.NaN));
> Console.WriteLine(Classify(2.4));
>
> static string Classify(double measurement) => measurement switch
> {
>     < -4.0 => "Too low",
>     > 10.0 => "Too high",
>     double.NaN => "Unknown",
>     _ => "Acceptable",
> };
> ```
>
> The output produced is
>
> ```console
> Too high
> Unknown
> Acceptable
> ```
>
> *end example*

### §logical-pattern-new-clause Logical pattern

A *logical_pattern* is used to negate the result of a pattern match, or to combine the results of multiple pattern matches using conjunction (`and`) or disjunction (`or`).

```ANTLR
logical_pattern
    : disjunctive_pattern
    ;

disjunctive_pattern
    : disjunctive_pattern 'or' conjunctive_pattern
    | conjunctive_pattern
    ;

conjunctive_pattern
    : conjunctive_pattern 'and' negated_pattern
    | negated_pattern
    ;

negated_pattern
    : 'not' negated_pattern
    | primary_pattern
    ;
```

`not`, `and`, and `or` are collectively called ***pattern operators***.

A *negated_pattern* matches if the pattern being negated does not match, and vice versa. A *conjunctive_pattern* requires both patterns to match. A *disjunctive_pattern* requires either pattern to match. Unlike their language operator counterparts, `&&` and `||`, `and` and `or` are *not* short-circuiting operators.

A declaration of a pattern variable shall satisfy both of the following conditions:

- The declaration is not enclosed in a *disjunctive_pattern* of the form `P or Q`.
- Each *negated_pattern* of the form `not P` that encloses the declaration is either the entire pattern of an is-pattern expression ([§12.14.12.2](expressions.md#1214122-the-is-pattern-operator)), or is nested within that pattern through only parenthesized patterns and other negated patterns.

The definite-assignment effects of patterns are specified in [§9.4.4.34](variables.md#94434-is-pattern-expressions).

> *Example*: A variable declared within a negated pattern can be definitely assigned when the pattern does not match. Parentheses and repeated negation are also permitted:
>
> <!-- Example: {template:"standalone-console", name:"IsNotPatternDeclarations", inferOutput:true} -->
> ```csharp
> PrintIfInt(42);
> PrintIfInt("other");
>
> static void PrintIfInt(object value)
> {
>     if (value is not (int number))
>     {
>         return;
>     }
>     Console.WriteLine(number);
>
>     if (value is (not not int repeated) && repeated > 0)
>     {
>         Console.WriteLine(repeated);
>     }
>
>     if (value is not (int positive and > 0) || positive > 100)
>     {
>         return;
>     }
>     Console.WriteLine(positive);
> }
> ```
>
> The output produced is:
>
> ```console
> 42
> 42
> 42
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: Declarations within disjunctions, within a negation nested beneath a conjunction, and within a negation in a switch expression are rejected:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"PatternDeclarationRestrictions", expectedErrors:["CS8780", "CS8780", "CS8780"]} -->
> ```csharp
> class C
> {
>     static bool Disjunction(object value) => value is int number or string;
>     static bool NestedNegation(object value) => value is {} and not int number;
>     static int SwitchNegation(object value) => value switch
>     {
>         not int number => 0,
>         _ => 1,
>     };
> }
> ```
>
> *end example*

In a *conjunctive_pattern*, the *input type* of the second pattern is narrowed by the *type narrowing* requirements of first pattern of the `and`. The *narrowed type* of a pattern `P` is defined as follows:

- If `P` is a type pattern, the *narrowed type* is the type of the type pattern's type.
- Otherwise, if `P` is a declaration pattern, the *narrowed type* is the type of the declaration pattern's type.
- Otherwise, if `P` specifies a *type* and is either a *positional_pattern* or a *property_pattern*, the *narrowed type* is that type.
- Otherwise, if `P` is matched via the rules for `ITuple` in a *positional_pattern* (§11.2.5), the *narrowed type* is the type `System.Runtime.CompilerServices.ITuple`.
- Otherwise, if `P` is a constant pattern where the constant is not the null constant, the *narrowed type* is the type of expression `R` obtained using the expression-conversion steps in [Relational pattern](patterns.md#relational-pattern-new-clause-relational-pattern), with the constant pattern's expression as `E` and its input type as `T`. Only the steps determining `R` are used here.
- Otherwise, if `P` is a relational pattern, the *narrowed type* is its comparison type ([Relational pattern](patterns.md#relational-pattern-new-clause-relational-pattern)).
- Otherwise, if `P` is an `or` pattern, the *narrowed type* is the common type of the *narrowed type* of the subpatterns if such a common type exists. For this purpose, the common type algorithm considers only identity, boxing, and implicit reference conversions, and it considers all subpatterns of a sequence of `or` patterns (ignoring parenthesized patterns).
- Otherwise, if `P` is an `and` pattern, the *narrowed type* is the *narrowed type* of the right pattern. Moreover, the *narrowed type* of the left pattern is the *input type* of the right pattern.
- Otherwise, if `P` is a parenthesized pattern, its *narrowed type* is determined as specified in [§11.2.1](patterns.md#1121-general).
- Otherwise the *narrowed type* of `P` is `P`'s input type.

> *Example*: Narrowing accounts for numeric conversion of a constant and retains its type before boxing. The generic method below prints the inferred type of its argument, rather than its runtime type:
>
> <!-- Example: {template:"standalone-console", name:"ConvertedConstantPatternNarrowing", inferOutput:true} -->
> ```csharp
> double number = 1;
> if (number is 1 and var constant)
> {
>     PrintType(constant);
> }
> if (number is < 2 and var relational)
> {
>     PrintType(relational);
> }
> object boxed = 1;
> if (boxed is 1 and var unboxed)
> {
>     PrintType(unboxed);
> }
>
> static void PrintType<T>(T value) => Console.WriteLine(typeof(T));
> ```
>
> The output produced is:
>
> ```console
> System.Double
> System.Double
> System.Int32
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: As indicated by the grammar, `not` has precedence over `and`, which has precedence over `or`. This can be explicitly indicated or overridden by using parentheses. *end note*

When a *pattern* appears on the right-hand-side of `is`, the extent of the pattern is determined by the grammar; as a result, the pattern operators `and`, `or`, and `not` within the pattern bind more tightly than the logical operators `&&`, `||`, and `!` outside the pattern.

> *Example*:
>
> <!-- Example: {template:"standalone-console", name:"LogicalPattern1", inferOutput:true} -->
> ```csharp
> Console.WriteLine(Classify(13));
> Console.WriteLine(Classify(-100));
> Console.WriteLine(Classify(5.7));
>
> static string Classify(double measurement) => measurement switch
> {
>     < -40.0 => "Too low",
>     >= -40.0 and < 0 => "Low",
>     >= 0 and < 10.0 => "Acceptable",
>     >= 10.0 and < 20.0 => "High",
>     >= 20.0 => "Too high",
>     double.NaN => "Unknown",
> };
> ```
>
> The output produced is
>
> ```console
> High
> Too low
> Acceptable
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*:
>
> <!-- Example: {template:"standalone-console", name:"LogicalPattern2", inferOutput:true} -->
> ```csharp
> Console.WriteLine(GetCalendarSeason(new DateTime(2021, 1, 19)));
> Console.WriteLine(GetCalendarSeason(new DateTime(2021, 10, 9)));
> Console.WriteLine(GetCalendarSeason(new DateTime(2021, 5, 11)));
>
> static string GetCalendarSeason(DateTime date) => date.Month switch
> {
>     3 or 4 or 5 => "spring",
>     6 or 7 or 8 => "summer",
>     9 or 10 or 11 => "autumn",
>     12 or 1 or 2 => "winter",
>     _ => throw new ArgumentOutOfRangeException(nameof(date),
>       $"Date with unexpected month: {date.Month}."),
> };
> ```
>
> The output produced is
>
> ```console
> winter
> autumn
> spring
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*:
>
> <!-- Example: {template:"standalone-console", name:"LogicalPattern3", inferOutput:true} -->
> ```csharp
> object msg = "msg";
> object obj = 5;
> bool flag = true;
>
> // This is parsed as: (msg is (not int) or string)
> bool result = msg is not int or string;
> Console.WriteLine($"msg (\"msg\"): msg is not int or string: {result}");
>
> // This is parsed as: (obj is (int or string)) && flag
> result = obj is int or string && flag;
> Console.WriteLine($"obj (5), flag (true): obj is int or string && flag: {result}");
>
> // This is parsed as: (obj is int) || ((obj is string) && flag)
> result = obj is int || obj is string && flag;
> Console.WriteLine($"obj (5), flag (true): obj is int || obj is string && flag: {result}");
>
> flag = false;
> // This is parsed as: (obj is (int or string)) && flag
> result = obj is int or string && flag;
> Console.WriteLine($"obj (5), flag (false): obj is int or string && flag: {result}");
>
> // This is parsed as: (obj is int) || ((obj is string) && flag)
> result = obj is int || obj is string && flag;
> Console.WriteLine($"obj (5), flag (false): obj is int || obj is string && flag: {result}");
> ```
>
> The output produced is
>
> ```console
> msg ("msg"): msg is not int or string: True
> obj (5), flag (true): obj is int or string && flag: True
> obj (5), flag (true): obj is int || obj is string && flag: True
> obj (5), flag (false): obj is int or string && flag: False
> obj (5), flag (false): obj is int || obj is string && flag: True
> ```
>
> *end example*
