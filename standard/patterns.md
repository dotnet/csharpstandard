# 11 Patterns and pattern matching

## 11.1 General

A ***pattern*** is a syntactic form that can be used with the `is` operator ([§12.12.12](expressions.md#121212-the-is-operator)) and in a *switch_statement* ([§13.8.3](statements.md#1383-the-switch-statement)) to express the shape of data against which incoming data is to be compared. A pattern is tested against the *expression* of a switch statement, or against a *relational_expression* that is on the left-hand side of an `is` operator, each of which is referred to as a ***pattern input value***.

## 11.2 Pattern forms

### 11.2.1 General

A pattern may have one of the following forms:

```ANTLR
pattern
    : declaration_pattern
    | constant_pattern
    | var_pattern
    ;
```

A *declaration_pattern* and a *var_pattern* can result in the declaration of a local variable.

Each pattern form defines the set of types for input values that the pattern may be applied to. A pattern `P` is *applicable to* a type `T` if `T` is among the types whose values the pattern may match. It is a compile-time error if a pattern `P` appears in a program to match a pattern input value ([§11.1](patterns.md#111-general)) of type `T` if `P` is not applicable to `T`.

Each pattern form defines the set of values for which the pattern *matches* the value at runtime.

### 11.2.2 Declaration pattern

A *declaration_pattern* is used to test that a value has a given type and, if the test succeeds, provide the value in a variable of that type.

```ANTLR
declaration_pattern
    : type simple_designation
    ;
simple_designation
    : single_variable_designation
    ;
single_variable_designation
    : identifier
    ;
```

The runtime type of the value is tested against the *type* in the pattern using the same rules specified in the is-type operator (§12.12.12.1) returns true. If the test succeeds succeeds, the pattern *matches* that value. It is a compile-time error if the *type* is a nullable value type (§8.3.12). This pattern form never matches a `null` value.

> *Note*: The is-type expression `e is T` and the declaration pattern `e is T _` are equivalent when `T` isn't a nullable type. *end note*

Given a pattern input value ([§11.1](patterns.md#111-general)) *e*, if the *simple_designation* is the *identifier* `_`, it denotes a discard ([§9.2.9.1](variables.md#9291-discards)) and the value of *e* is not bound to anything. (Although a declared variable with the name `_` may be in scope at that point, that named variable is not seen in this context.) If *simple_designation* is any other identifier, a local variable ([§9.2.9](variables.md#929-local-variables)) of the given type named by the given identifier is introduced. That local variable is assigned the value of the pattern input value when the pattern *matches* the value.

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

It is an error if *type* is a nullable value type.

> *Example*: The declaration pattern can be used to test values of nullable types: a value of type `Nullable<T>` (or a boxed `T`) matches a type pattern `T2 id` if the value is non-null and `T2` is `T`, or some base type or interface of `T`. For example, in the code fragment
>
> <!-- Example: {template:"standalone-console-without-using", name:"DeclarationPattern1"} -->
> ```csharp
> int? x = 3;
> if (x is int v) { /* code using v */ }
> ```
>
> The condition of the `if` statement is `true` at runtime and the variable `v` holds the value `3` of type `int` inside the block. *end example*

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
    ;
```

Given a pattern input value ([§11.1](patterns.md#111-general)) *e*, if *designation* is the *identifier* `_`, it denotes a discard ([§9.2.9.1](variables.md#9291-discards)), and the value of *e* is not bound to anything. (Although a declared variable with that name may be in scope at that point, that named variable is not seen in this context.) If *designation* is any other identifier, at runtime the value of *e* is bound to a newly introduced local variable ([§9.2.9](variables.md#929-local-variables)) of that name whose type is the static type of *e*, and the pattern input value is assigned to that local variable.

It is an error if the name `var` would bind to a type where a *var_pattern* is used.

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

Informally, a set of patterns is exhaustive for a type if some pattern in the set is applicable to every possible value of that type other than null.
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
>         case 0: case 1: case 2: case 3: ... // handle every specific value of byte
>             break;
>         case byte other: // error: the pattern 'byte other' is subsumed by the (exhaustive) previous cases
>             break;
>     }
> }
> ```
>
> *end example*
