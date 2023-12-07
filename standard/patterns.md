# 11 Patterns and pattern matching

## 11.1 General

A ***pattern*** is a syntactic form that can be used with the `is` operator ([§12.14.12](expressions.md#121412-the-is-operator)), in a *switch_statement* ([§13.8.3](statements.md#1383-the-switch-statement)), and in a *switch_expression* ([§12.11](expressions.md#1211-switch-expression)) to express the shape of data against which incoming data is to be compared. Patterns may be recursive, so that parts of the data may be matched against ***sub-patterns***.

A pattern is tested against a value in a number of contexts:

- In a switch statement, the *pattern* of a case label is tested against the *expression* of the switch statement.  
- In an *is-pattern* operator, the *pattern* on the right-hand-side is tested against the expression on the left.  
- In a switch expression, the *pattern* of a *switch_expression_arm* is tested against the expression on the switch-expression’s left-hand-side.  
- In nested contexts, the *sub-pattern* is tested against values retrieved from properties, fields, or indexed from other input values, depending on the pattern form.  

The value against which a pattern is tested is called the ***pattern input value***. Patterns may be combined using Boolean logic.

## 11.2 Pattern forms

### 11.2.1 General

A pattern may have one of the following forms:

```ANTLR
pattern
    : '(' pattern ')'
    | declaration_pattern
    | constant_pattern
    | var_pattern
    | positional_pattern
    | property_pattern
    | discard_pattern
    | type_pattern
    | relational_pattern
    | logical_pattern
    ;
```

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
> However, the following doesn’t generate a compile-time error because the compile-time type of `v` is `object`. A variable of type `object` could have a value that is reference-compatible with `string`:
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

The order of evaluation of operations and side effects during pattern-matching (calls to `Deconstruct`, property accesses, and invocations of methods in `System.ITuple`) is not specified.

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

A *simple_designation* with the token `_` shall be considered a *discard_designation* rather than a *single_variable_designation*.

The runtime type of the value is tested against the *type* in the pattern using the same rules specified in the is-type operator ([§12.14.12.1](expressions.md#1214121-the-is-type-operator)). If the test succeeds, the pattern *matches* that value. It is a compile-time error if the *type* is a nullable value type ([§8.3.12](types.md#8312-nullable-value-types)) or a nullable reference type ([§8.9.3](types.md#893-nullable-reference-types)). This pattern form never matches a `null` value.

> *Note*: The is-type expression `e is T` and the declaration pattern `e is T _` are equivalent when `T` isn’t a nullable type. *end note*

Given a pattern input value ([§11.1](patterns.md#111-general)) *e*, if the *simple_designation* is *discard_designation*, it denotes a discard ([§9.2.9.2](variables.md#9292-discards)), and the value of *e* is not bound to anything. (Although a declared variable with the name `_` may be in scope at that point, that named variable is not seen in this context.) Otherwise, if the *simple_designation* is *single_variable_designation*, a local variable ([§9.2.9](variables.md#929-local-variables)) of the given type named by the given identifier is introduced. That local variable is assigned the value of the pattern input value when the pattern *matches* the value.  

Certain combinations of static type of the pattern input value and the given type are considered incompatible and result in a compile-time error. A value of static type `E` is said to be ***pattern compatible*** with the type `T` if there exists an identity conversion, an implicit or explicit reference conversion, a boxing conversion, or an unboxing conversion from `E` to `T`, or if either `E` or `T` is an open type ([§8.4.3](types.md#843-open-and-closed-types)). A declaration pattern naming a type `T` is *applicable to* every type `E` for which `E` is pattern compatible with `T`.

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

### 11.2.4 Var pattern

A *var_pattern* *matches* every value. That is, a pattern-matching operation with a *var_pattern* always succeeds.

A *var_pattern* is *applicable to* every type.

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

It is an error if the name `var` would bind to a type where a *var_pattern* is used.

If *designation* is a *tuple_designation*, the pattern is equivalent to a *positional_pattern* ([§11.2.5](patterns.md#1125-positional-pattern)) of the form `(var` *designation*, … `)` where the *designation*s are those found within the *tuple_designation*.  For example, the pattern `var (x, (y, z))` is equivalent to `(var x, (var y, var z))`.

### 11.2.5 Positional pattern

A *positional_pattern* checks that the input value is not `null`, invokes an appropriate `Deconstruct` method ([§12.7](expressions.md#127-deconstruction)), and performs further pattern matching on the resulting values.  It also supports a tuple-like pattern syntax (without the type being provided) when the type of the input value is the same as the type containing `Deconstruct`, or if the type of the input value is a tuple type, or if the type of the input value is `object` or `System.ITuple` and the runtime type of the expression implements `System.ITuple`.

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
It is an error if a *positional_pattern* omits the type, has a single *subpattern* without an *identifier*, has no *property_subpattern* and has no *simple_designation*. This disambiguates between a *constant_pattern* that is parenthesized and a *positional_pattern*.
In order to extract the values to match against the patterns in the list,

- If *type* is omitted and the input expression’s type is a tuple type, then the number of subpatterns shall to be the same as the cardinality of the tuple. Each tuple element is matched against the corresponding *subpattern*, and the match succeeds if all of these succeed. If any *subpattern* has an *identifier*, then that shall name a tuple element at the corresponding position in the tuple type.
- Otherwise, if a suitable `Deconstruct` exists as a member of *type*, it is a compile-time error if the type of the input value is not pattern-compatible with *type*. At runtime the input value is tested against *type*. If this fails, then the positional pattern match fails. If it succeeds, the input value is converted to this type and `Deconstruct` is invoked with fresh compiler-generated variables to receive the output parameters. Each value that was received is matched against the corresponding *subpattern*, and the match succeeds if all of these succeed. If any *subpattern* has an *identifier*, then that shall name a parameter at the corresponding position of `Deconstruct`.
- Otherwise, if *type* is omitted, and the input value is of type `object` or some type that can be converted to `System.ITuple` by an implicit reference conversion, and no *identifier* appears among the subpatterns, then the match uses `System.ITuple`.  
- Otherwise, the pattern is a compile-time error.

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
> if (SumAndCount(numbers) is (Sum: var sum, Count: var count))
> {
>     Console.WriteLine($"Sum of [{string.Join(" ", numbers)}] is {sum}");
> }
>
> static (double Sum, int Count) SumAndCount(IEnumerable<int> numbers)
> {
>     int sum = 0;
>     int count = 0;
>     foreach (int number in numbers)
>     {
>         sum += number;
>         count++;
>     }
>     return (sum, count);
> }
> ```
>
> The output produced is
>
> ```console
> Sum of [10 20 30] is 60
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
> *end note*
Given a match of an expression *e* to the pattern *type* `{` *property_pattern_list* `}`, it is a compile-time error if the expression *e* is not pattern-compatible with the type *T* designated by *type*. If the type is absent, the type is assumed to be the static type of *e*. Each of the identifiers appearing on the left-hand-side of its *property_pattern_list* shall designate an accessible readable property or field of *T*. If the *simple_designation* of the *property_pattern* is present, it declares a pattern variable of type *T*.  

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
> <!-- markdownlint-disable MD028 -->
>
> <!-- markdownlint-enable MD028 -->
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

It is a compile-time error to use a discard pattern in a *relational_expression* of the form *relational_expression* `is` *pattern* or as the pattern of a *switch_label*.  

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
> Here, a discard pattern is used to handle `null` and any integer value that doesn’t have the corresponding member of the `DayOfWeek` enumeration. That guarantees that the `switch` expression handles all possible input values.
> *end example*

### §type-pattern-new-clause Type pattern

A *type_pattern* is used to test that the pattern input value ([§11.1](patterns.md#111-general)) has a given type.

```ANTLR
type_pattern
    : type
    ;
```

The runtime type of the value is tested against *type* using the same rules specified in the is-type operator ([§12.14.12.1](expressions.md#1214121-the-is-type-operator)). If the test succeeds, the pattern matches that value. It is a compile-time error if the *type* is a nullable type. This pattern form never matches a `null` value.

### §relational-pattern-new-clause Relational pattern

A *relational_pattern* is used to relationally test the pattern input value ([§11.1](patterns.md#111-general)) against a constant value.

```ANTLR
relational_pattern
    : '<'  constant_expression
    | '<=' constant_expression
    | '>'  constant_expression
    | '>=' constant_expression
    ;
```

Relational patterns support the relational operators `<`, `<=`, `>`, and `>=` on all of the built-in types that support such binary relational operators with both operands having the same type: `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `float`, `double`, `decimal`, `nint`, `nuint`, and enums.

It is a compile-time error if `constant_expression`is `double.NaN`, `float.NaN`, or `null_literal`.

When the input value has a type for which a suitable built-in binary relational operator is defined, the evaluation of that operator is taken as the meaning of the relational pattern.  Otherwise, the input value is converted to the type of `constant_expression` using an explicit nullable or unboxing conversion.  It is a compile-time error if no such conversion exists.  The pattern is considered to not match if the conversion fails.  If the conversion succeeds, the result of the pattern-matching operation is the result of evaluating the expression `e «op» v` where `e` is the converted input, «op» is the relational operator, and `v` is the `constant_expression`.

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

A *logical_pattern* is used to negate a pattern input value ([§11.1](patterns.md#111-general)) or to combine that value with a pattern using a Boolean operator.

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
    | pattern
    ;
```

`not`, `and`, and `or` are collectively called ***pattern operators***.

A *negated_pattern* matches if the pattern being negated does not match, and vice versa. A *conjunctive_pattern* requires both patterns to match. A *disjunctive_pattern* requires either pattern to match. Unlike their language operator counterparts, `&&` and `||`, `and` and `or` are *not* short-circuiting operators.

> *Note*: As indicated by the grammar, `not` has precedence over `and`, which has precedence over `or`. This can be explicitly indicated or overridden by using parentheses. *end note*

When a *pattern* is used with `is`, any pattern operators in that *pattern* have higher precedence than their logical operator counterparts. Otherwise, those pattern operators have lower precedence.

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

## 11.3 Pattern subsumption

In a switch statement, it is an error if a case’s pattern is *subsumed* by the preceding set of unguarded cases ([§13.8.3](statements.md#1383-the-switch-statement)).
Informally, this means that any input value would have been matched by one of the previous cases.
The following rules define when a set of patterns subsumes a given pattern:

A pattern `P` *would match* a constant `K` if the specification for that pattern’s runtime behavior is that `P` matches `K`.

A set of patterns `Q` *subsumes* a pattern `P` if any of the following conditions hold:

- `P` is a constant pattern and any of the patterns in the set `Q` would match `P`’s *converted value*
- `P` is a var pattern and the set of patterns `Q` is *exhaustive* ([§11.4](patterns.md#114-pattern-exhaustiveness)) for the type of the pattern input value ([§11.1](patterns.md#111-general)), and either the pattern input value is not of a nullable type or some pattern in `Q` would match `null`.
- `P` is a declaration pattern with type `T` and the set of patterns `Q` is *exhaustive* for the type `T` ([§11.4](patterns.md#114-pattern-exhaustiveness)).

## 11.4 Pattern exhaustiveness

Informally, a set of patterns is exhaustive for a type if, for every possible value of that type other than null, some pattern in the set is applicable.
The following rules define when a set of patterns is *exhaustive* for a type:

A set of patterns `Q` is *exhaustive* for a type `T` if any of the following conditions hold:

1. `T` is an integral or enum type, or a nullable version of one of those, and for every possible value of `T`’s non-nullable underlying type, some pattern in `Q` would match that value; or
2. Some pattern in `Q` is a *var pattern*; or
3. Some pattern in `Q` is a *declaration pattern* for type `D`, and there is an identity conversion, an implicit reference conversion, or a boxing conversion from `T` to `D`.

> *Example*:
>
> <!-- Example: {template:"standalone-console-without-using", name:"PatternExhaustiveness1", replaceEllipsis:true, customEllipsisReplacements: [""], ignoredWarnings:["CS8321"]} -->
> ```csharp
> static void M(byte b)
> {
>     switch (b) {
>         case 0: case 1: case 2: ... // handle every specific value of byte
>             break;
>         // error: the pattern 'byte other' is subsumed by the (exhaustive)
>         // previous cases
>         case byte other: 
>             break;
>     }
> }
> ```
>
> *end example*
