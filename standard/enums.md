# 19 Enums

## 19.1 General

An ***enum type*** is a distinct value type ([§9.3](types.md#93-value-types)) that declares a set of named constants.

> *Example:* The example
> 
> ```csharp
> enum Color
> {
> 	Red,
> 	Green,
> 	Blue
> }
> ```
> declares an enum type named `Color` with members `Red`, `Green`, and `Blue`. *end example*

## 19.2 Enum declarations

An enum declaration declares a new enum type. An enum declaration begins with the keyword `enum`, and defines the name, accessibility, underlying type, and members of the enum.

```ANTLR
enum_declaration
    : attributes? enum_modifier* 'enum' identifier enum_base? enum_body ';'?
    ;

enum_base
    : ':' integral_type
    | ':' integral_type_name
    ;

integral_type_name
    : type_name // restricted to one of System.{SByte,Byte,Int16,UInt16,Int32,UInt32,Int64,UInt64}
    ;

enum_body
    : '{' enum_member_declarations? '}'
    | '{' enum_member_declarations ',' '}'
    ;
```

Each enum type has a corresponding integral type called the ***underlying type*** of the enum type. This underlying type shall be able to represent all the enumerator values defined in the enumeration. If the *enum_base* is present, it explicitly declares the underlying type. The underlying type shall be one of the *integral types* ([§9.3.6](types.md#936-integral-types)) other than `char`; specified either by keyword (*integral_type*), or by one of the full type names that the integral types alias ([§9.3.5](types.md#935-simple-types)) (*integral_type_name*).

> *Note*: Neither `char` nor `System.Char` can be used as an underlying type. *end note*

An enum declaration that does not explicitly declare an underlying type has an underlying type of `int`.

> *Example*: The example
> 
> ```csharp
> enum Color: long
> {
> 	Red,
> 	Green,
> 	Blue
> }
> ```
> declares an enum with an underlying type of `long`. *end example*

> *Note*: A developer might choose to use an underlying type of `long`, as in the example, to enable the use of values that are in the range of `long` but not in the range of `int`, or to preserve this option for the future. *end note*

> *Note*: C# allows a trailing comma in an *enum_body*, just like it allows one in an *array_initializer* ([§17.7](arrays.md#177-array-initializers)). *end note*

## 19.3 Enum modifiers

An *enum_declaration* may optionally include a sequence of enum modifiers:

```ANTLR
enum_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    ;
```

It is a compile-time error for the same modifier to appear multiple times in an enum declaration.

The modifiers of an enum declaration have the same meaning as those of a class declaration ([§15.2.2](classes.md#1522-class-modifiers)). However, the `abstract`, and `sealed`, and `static` modifiers are not permitted in an enum declaration. Enums cannot be abstract and do not permit derivation.

## 19.4 Enum members

The body of an enum type declaration defines zero or more enum members, which are the named constants of the enum type. No two enum members can have the same name.

```ANTLR
enum_member_declarations
    : enum_member_declaration (',' enum_member_declaration)*
    ;
```

```ANTLR
enum_member_declaration
    : attributes? identifier ('=' constant_expression)?
    ;
```

Each enum member has an associated constant value. The type of this value is the underlying type for the containing enum. The constant value for each enum member shall be in the range of the underlying type for the enum.

> *Example*: The example
> ```csharp
> enum Color: uint
> {
> 	Red = -1,
> 	Green = -2,
> 	Blue = -3
> }
> ```
> results in a compile-time error because the constant values `-1`, `-2`, and `-3` are not in the range of the underlying integral type `uint`. *end example*

Multiple enum members may share the same associated value.

> *Example*: The example
> ```csharp
> enum Color
> {
> 	Red,
> 	Green,
> 	Blue,
> 	Max = Blue
> }
> ```
> shows an enum in which two enum members—`Blue` and `Max`—have the same associated value. *end example*

The associated value of an enum member is assigned either implicitly or explicitly. If the declaration of the enum member has a *constant_expression* initializer, the value of that constant expression, implicitly converted to the underlying type of the enum, is the associated value of the enum member. If the declaration of the enum member has no initializer, its associated value is set implicitly, as follows:

-   If the enum member is the first enum member declared in the enum type, its associated value is zero.
-   Otherwise, the associated value of the enum member is obtained by increasing the associated value of the textually preceding enum member by one. This increased value shall be within the range of values that can be represented by the underlying type, otherwise a compile-time error occurs.

> *Example*: The example
> ```csharp
> using System;
> enum Color
> {
>    Red,
>    Green = 10,
>    Blue
> }
> class Test
> {
>    static void Main() {
>       Console.WriteLine(StringFromColor(Color.Red));
>       Console.WriteLine(StringFromColor(Color.Green));
>       Console.WriteLine(StringFromColor(Color.Blue));
>    }
>    static string StringFromColor(Color c) {
>       switch (c) {
>          case Color.Red:
>             return String.Format("Red = {0}", (int) c);
>          case Color.Green:
>             return String.Format("Green = {0}", (int) c);
>          case Color.Blue:
>             return String.Format("Blue = {0}", (int) c);
>          default:
>             return "Invalid color";
>       }
>    }
> }
> ```
> prints out the enum member names and their associated values. The output is:
> ```console
> Red = 0
> Green = 10
> Blue = 11
> ```
> for the following reasons:
> -   the enum member `Red` is automatically assigned the value zero (since it has no initializer and is the first enum member);
> -   the enum member `Green` is explicitly given the value `10`;
> -   and the enum member `Blue` is automatically assigned the value one greater than the member that textually precedes it.
> *end example*

The associated value of an enum member may not, directly or indirectly, use the value of its own associated enum member. Other than this circularity restriction, enum member initializers may freely refer to other enum member initializers, regardless of their textual position. Within an enum member initializer, values of other enum members are always treated as having the type of their underlying type, so that casts are not necessary when referring to other enum members.

> *Example*: The example
> ```csharp
> enum Circular
> {
>     A = B,
>     B
> }
> ```
> results in a compile-time error because the declarations of `A` and `B` are circular. `A` depends on `B` explicitly, and `B` depends on `A` implicitly. *end example*

Enum members are named and scoped in a manner exactly analogous to fields within classes. The scope of an enum member is the body of its containing enum type. Within that scope, enum members can be referred to by their simple name. From all other code, the name of an enum member shall be qualified with the name of its enum type. Enum members do not have any declared accessibility—an enum member is accessible if its containing enum type is accessible.

## 19.5 The System.Enum type

The type `System.Enum` is the abstract base class of all enum types (this is distinct and different from the underlying type of the enum type), and the members inherited from `System.Enum` are available in any enum type. A boxing conversion ([§11.2.8](conversions.md#1128-boxing-conversions)) exists from any enum type to `System.Enum`, and an unboxing conversion ([§11.3.6](conversions.md#1136-unboxing-conversions)) exists from `System.Enum` to any enum type.

Note that `System.Enum` is not itself an *enum_type*. Rather, it is a *class_type* from which all *enum_type*s are derived. The type `System.Enum` inherits from the type `System.ValueType` ([§9.3.2](types.md#932-the-systemvaluetype-type)), which, in turn, inherits from type `object`. At run-time, a value of type `System.Enum` can be `null` or a reference to a boxed value of any enum type.

## 19.6 Enum values and operations

Each enum type defines a distinct type; an explicit enumeration conversion ([§11.3.3](conversions.md#1133-explicit-enumeration-conversions)) is required to convert between an enum type and an integral type, or between two enum types. The set of values of the enum type is the same as the set of values of the underlying type and is not restricted to the values of the named constants. Any value of the underlying type of an enum can be cast to the enum type, and is a distinct valid value of that enum type.

Enum members have the type of their containing enum type (except within other enum member initializers: see [§19.4](enums.md#194-enum-members)). The value of an enum member declared in enum type `E` with associated value `v` is `(E)v`.

The following operators can be used on values of enum types:

-   `==`, `!=`, `<`, `>`, `<=`, `>=` ([§12.11.6](expressions.md#12116-enumeration-comparison-operators))
-   binary `+` ([§12.9.5](expressions.md#1295-addition-operator))
-   binary `-` ([§12.9.6](expressions.md#1296-subtraction-operator))
-   `^`, `&`, `|` ([§12.12.3](expressions.md#12123-enumeration-logical-operators))
-   `~` ([§12.8.5](expressions.md#1285-bitwise-complement-operator))
-   `++`, `--` ([§12.7.10](expressions.md#12710-postfix-increment-and-decrement-operators) and [§12.8.6](expressions.md#1286-prefix-increment-and-decrement-operators))
-   sizeof ([§23.6.9](unsafe-code.md#2369-the-sizeof-operator))

Every enum type automatically derives from the class `System.Enum` (which, in turn, derives from `System.ValueType` and `object`). Thus, inherited methods and properties of this class can be used on values of an enum type.
