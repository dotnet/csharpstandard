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
    : declaration_pattern
    | constant_pattern
    | var_pattern
    | positional_pattern
    | property_pattern
    | discard_pattern
    ;
```

If the input can be syntactically recognised as both a *constant_pattern* and a *positional_pattern* then the *constant_pattern* shall be chosen.

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

### 11.2.3 Constant pattern

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

### 11.2.2 Declaration pattern

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

When recognising a *simple_designation* if both the *discard_designation* and *single_variable_designation* alternatives are applicable then the former shall be chosen.

> *Note*: ANTLR makes the specified choice automatically due to the ordering of the alternatives of *simple_designation*. *end note*

A *declaration_pattern* cannot be used to test that a value has a type named `var` unless that type is referenced using an identifier containing a unicode character escape sequence ([§6.4.2](lexical-structure.md#642-unicode-character-escape-sequences)) or represented by an *Escaped_Identifier* ([§6.4.3](lexical-structure.md#643-identifiers)).

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

Given a match of an input value to the pattern *type* `(` *subpatterns* `)`, a method is selected by searching in *type* for accessible declarations of `Deconstruct` and selecting one among them using the same rules as for the deconstruction declaration.
If the input can be syntactically recognised as both a *constant_pattern* and a *positional_pattern* then the *constant_pattern* shall be chosen.

> *Note*: A tuple literal can be matched by patterns of several different forms, which are not interchangeable:
>
> - `(int, int) x` is a *declaration_pattern* with type `(int, int)` and *simple_designation* `x`.
> - `var (x, y)` is a *var_pattern* with a *tuple_designation*.
> - `(int x, int y)` is a *positional_pattern*.
> - `(int, int) (x, y)` is not a valid pattern.
>
> *end note*

In order to extract the values to match against the patterns in the list,

1. **Tuple form.** If *type* is omitted and the static type of the input value is a tuple type ([§8.3.11](types.md#8311-tuple-types)) or if the input value is a tuple literal ([§12.8.6](expressions.md#1286-tuple-literals)), then this case applies. It is a compile-time error if *n* is not equal to the arity of that tuple type. At runtime, each tuple element is matched against the corresponding *subpattern*; the match succeeds if all of these succeed. If any *subpattern* has an *identifier*, that *identifier* shall name the tuple element at the corresponding position in the tuple type.
2. **Deconstruct form.** Otherwise, if either *type* is present, or *type* is omitted and the static type of the input value contains an accessible `Deconstruct` method ([§12.7](expressions.md#127-deconstruction)), then this case applies. Let *D* be *type* if *type* is present; otherwise let *D* be the static type of the input value. A `Deconstruct` method is selected from *D* using the same overload-resolution rules as for a deconstruction declaration, with the additional requirement that its number of `out` parameters is equal to *n*; it is a compile-time error if no such method exists. If *type* is present, it is a compile-time error if the static type of the input value is not pattern compatible ([§11.2.2](patterns.md#1122-declaration-pattern)) with *type*; at runtime the input value is tested against *type* and, if that test fails, the positional pattern match fails. Otherwise, the input value is converted to *D* and the selected `Deconstruct` method is invoked with fresh variables receiving its `out` parameters. Each received value is matched against the corresponding *subpattern*, and the match succeeds if all of these succeed. If any *subpattern* has an *identifier*, that *identifier* shall name the parameter at the corresponding position of `Deconstruct`.
3. **ITuple form.** Otherwise, if *type* is omitted, no *subpattern* has an *identifier*, and the static type of the input value is `object`, `System.Runtime.CompilerServices.ITuple`, or a type that has an implicit reference conversion to `System.Runtime.CompilerServices.ITuple`, then this case applies. At runtime, the input value is tested for being a non-`null` instance of `System.Runtime.CompilerServices.ITuple`; if that test fails, the positional pattern match fails. Otherwise, the value’s `Length` property is read and, if it is not equal to *n*, the positional pattern match fails. Otherwise, for each *i* from 1 to *n*, the value obtained by indexing the input value with *i* − 1 is matched against the *i*-th *subpattern*, and the match succeeds if all of these succeed.
4. Otherwise, no case applies and the *positional_pattern* is a compile-time error.

The order in which subpatterns are matched at runtime is unspecified, and a failed match might not attempt to match all subpatterns.

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
>     Console.WriteLine($"Sum of [{string.Join(" ", numbers)}] is {sum}; average is {average}");
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

Given a match of an expression *e* to the pattern *type* `{` *subpatterns* `}`, it is a compile-time error if the expression *e* is not pattern compatible ([§11.2.2](patterns.md#1122-declaration-pattern)) with the type *T* designated by *type*. If the type is absent, the type is assumed to be the static type of *e*. Each of the identifiers appearing on the left-hand-side of its *subpatterns* shall designate an accessible readable property or field of *T*. If the *simple_designation* of the *property_pattern* is present, it declares a pattern variable of type *T*.

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
> *Example*: A run-time type check and a variable declaration can be added to a property pattern, as follow:
>
> <!-- Example: {template:"standalone-console", name:"PropertyPattern3", inferOutput:true} -->
> ```csharp
> Console.WriteLine(TakeFive("Hello, world!"));  // output: Hello
> Console.WriteLine(TakeFive("Hi!"));            // output: Hi!
> Console.WriteLine(TakeFive(new[] { '1', '2', '3', '4', '5', '6', '7' }));  // output: 12345
> Console.WriteLine(TakeFive(new[] { 'a', 'b', 'c' }));  // output: abc
> 
> static string TakeFive(object input) => input switch
> {
>     string { Length: >= 5 } s => s.Substring(0, 5),
>     string s => s,
>     ICollection<char> { Count: >= 5 } symbols => new string(symbols.Take(5).ToArray()),
>     ICollection<char> symbols => new string(symbols.ToArray()),
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

- If `_` resolves to an accessible constant, the `_` is interpreted as a *constant_pattern* ([§11.2.3](patterns.md#1123-constant-pattern)) whose constant expression is that constant.
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
