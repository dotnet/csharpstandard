# 17 Arrays

## 17.1 General

An array is a data structure that contains a number of variables that are accessed through computed indices. The variables contained in an array, also called the ***elements*** of the array, are all of the same type, and this type is called the ***element type*** of the array.

An array has a rank that determines the number of indices associated with each array element. The rank of an array is also referred to as the dimensions of the array. An array with a rank of one is called a ***single-dimensional array.*** An array with a rank greater than one is called a ***multi-dimensional array***. Specific sized multi-dimensional arrays are often referred to as two-dimensional arrays, three-dimensional arrays, and so on. Each dimension of an array has an associated length that is an integral number greater than or equal to zero. The dimension lengths are not part of the type of the array, but rather are established when an instance of the array type is created at run-time. The length of a dimension determines the valid range of indices for that dimension: For a dimension of length `N`, indices can range from `0` to `N – 1` inclusive. The total number of elements in an array is the product of the lengths of each dimension in the array. If one or more of the dimensions of an array have a length of zero, the array is said to be empty.

The element type of an array can itself be an array type ([§17.2.1](arrays.md#1721-general)). Such arrays of arrays are distinct from multi-dimensional arrays and can be used to represent "jagged arrays".

> *Example*:
> ```csharp
> int[][] pascals = {
>     new int[] {1},
>     new int[] {1, 1},
>     new int[] {1, 2, 1},
>     new int[] {1, 3, 3, 1}
> };
> ```
> *end example*

Every array type is a reference type ([§9.2](types.md#92-reference-types)). The element type of an array can be any type, including value types and array types.

## 17.2 Array types

### 17.2.1 General

The grammar productions for array types are provided in [§9.2.1](types.md#921-general).

An array type is written as a *non_array_type* followed by one or more *rank_specifier*s.

A *non_array_type* is any *type* that is not itself an *array_type*.

The rank of an array type is given by the leftmost *rank_specifier* in the *array_type*: A *rank_specifier* indicates that the array is an array with a rank of one plus the number of "`,`" tokens in the *rank_specifier*.

The element type of an array type is the type that results from deleting the leftmost *rank_specifier*:

-   An array type of the form `T[R]` is an array with rank `R` and a non-array element type `T`.
-   An array type of the form `T[R][R₁]...[Rₓ]` is an array with rank `R` and an element type `T[R₁]...[Rₓ]`.

In effect, the *rank_specifier*s are read from left to right *before* the final non-array element type.

> *Example*: The type in `T[][,,][,]` is a single-dimensional array of three-dimensional arrays of two-dimensional arrays of `int`. *end example*

At run-time, a value of an array type can be `null` or a reference to an instance of that array type. 

> *Note*: Following the rules of [§17.6](arrays.md#176-array-covariance), the value may also be a reference to a covariant array type. *end note*

### 17.2.2 The System.Array type

The type `System.Array` is the abstract base type of all array types. An implicit reference conversion ([§11.2.8](conversions.md#1128-implicit-reference-conversions)) exists from any array type to `System.Array` and to any interface type implemented by `System.Array`. An explicit reference conversion ([§11.3.5](conversions.md#1135-explicit-reference-conversions)) exists from `System.Array` and any interface type implemented by `System.Array` to any array type. `System.Array` is not itself an *array_type*. Rather, it is a *class_type* from which all *array_type*s are derived.

At run-time, a value of type `System.Array` can be `null` or a reference to an instance of any array type.

### 17.2.3 Arrays and the generic collection interfaces

A single-dimensional array `T[]` implements the interface System.Collections.Generic.`IList<T>` (`IList<T>` for short) and its base interfaces. Accordingly, there is an implicit conversion from `T[]` to `IList<T>` and its base interfaces. In addition, if there is an implicit reference conversion from `S` to `T` then `S[]` implements `IList<T>` and there is an implicit reference conversion from `S[]` to `IList<T>` and its base interfaces ([§11.2.8](conversions.md#1128-implicit-reference-conversions)). If there is an explicit reference conversion from `S` to `T` then there is an explicit reference conversion from `S[]` to `IList<T>` and its base interfaces ([§11.3.5](conversions.md#1135-explicit-reference-conversions)).

Similarly, a single-dimensional array `T[]` also implements the interface `System.Collections.Generic.IReadOnlyList<T>` (`IReadOnlyList<T>` for short) and its base interfaces. Accordingly, there is an implicit conversion from `T[]` to `IReadOnlyList<T>` and its base interfaces. In addition, if there is an implicit reference conversion from `S` to `T` then `S[]` implements `IReadOnlyList<T>` and there is an implicit reference conversion from `S[]` to `IReadOnlyList<T>` and its base interfaces ([§11.2.8](conversions.md#1128-implicit-reference-conversions)). If there is an explicit reference conversion from `S` to `T` then there is an explicit reference conversion from `S[]` to `IReadOnlyList<T>` and its base interfaces ([§11.3.5](conversions.md#1135-explicit-reference-conversions)).

> *Example*: For example:
> ```csharp
> using System.Collections.Generic;
> 
> class Test
> {
>     static void Main()
>     {
>         string[] sa = new string[5];
>         object[] oa1 = new object[5];
>         object[] oa2 = sa;
>         IList<string> lst1 = sa; // Ok
>         IList<string> lst2 = oa1; // Error, cast needed
>         IList<object> lst3 = sa; // Ok
>         IList<object> lst4 = oa1; // Ok
>         IList<string> lst5 = (IList<string>)oa1; // Exception
>         IList<string> lst6 = (IList<string>)oa2; // Ok
>         IReadOnlyList<string> lst7 = sa; // Ok
>         IReadOnlyList<string> lst8 = oa1; // Error, cast needed
>         IReadOnlyList<object> lst9 = sa; // Ok
>         IReadOnlyList<object> lst10 = oa1; // Ok
>         IReadOnlyList<string> lst11 = (IReadOnlyList<string>)oa1; // Exception
>         IReadOnlyList<string> lst12 = (IReadOnlyList<string>)oa2; // Ok
>     }
> }
> ```
> The assignment `lst2 = oa1` generates a compile-time error since the conversion from `object[]` to `IList<string>` is an explicit conversion, not implicit. The cast `(IList<string>)oa1` will cause an exception to be thrown at run-time since `oa1` references an `object[]` and not a `string[]`. However the cast (`IList<string>)oa2` will not cause an exception to be thrown since `oa2` references a `string[]`. *end example*

Whenever there is an implicit or explicit reference conversion from `S[]` to `IList<T>`, there is also an explicit reference conversion from `IList<T>` and its base interfaces to `S[]` ([§11.3.5](conversions.md#1135-explicit-reference-conversions)).

When an array type `S[]` implements `IList<T>`, some of the members of the implemented interface may throw exceptions. The precise behavior of the implementation of the interface is beyond the scope of this specification.

## 17.3 Array creation

Array instances are created by *array_creation_expression*s ([§12.7.15.5](expressions.md#127155-array-creation-expressions)) or by field or local variable declarations that include an *array_initializer* ([§17.7](arrays.md#177-array-initializers)). Array instances can also be created implicitly as part of evaluating an argument list involving a parameter array ([§15.6.2.5](classes.md#15625-parameter-arrays)).

When an array instance is created, the rank and length of each dimension are established and then remain constant for the entire lifetime of the instance. In other words, it is not possible to change the rank of an existing array instance, nor is it possible to resize its dimensions.

An array instance is always of an array type. The `System.Array` type is an abstract type that cannot be instantiated.

Elements of arrays created by *array_creation_expression*s are always initialized to their default value ([§10.3](variables.md#103-default-values)).

## 17.4 Array element access

Array elements are accessed using *element_access* expressions ([§12.7.10.2](expressions.md#127102-array-access)) of the form `A[I₁, I₂, ..., Iₓ]`, where `A` is an expression of an array type and each `Iₑ` is an expression of type `int`, `uint`, `long`, `ulong`, or can be implicitly converted to one or more of these types. The result of an array element access is a variable, namely the array element selected by the indices.

The elements of an array can be enumerated using a `foreach` statement ([§13.9.5](statements.md#1395-the-foreach-statement)).

## 17.5 Array members

Every array type inherits the members declared by the `System.Array` type.

## 17.6 Array covariance

For any two *reference_type*s `A` and `B`, if an implicit reference conversion ([§11.2.8](conversions.md#1128-implicit-reference-conversions)) or explicit reference conversion ([§11.3.5](conversions.md#1135-explicit-reference-conversions)) exists from `A` to `B`, then the same reference conversion also exists from the array type `A[R]` to the array type `B[R]`, where `R` is any given *rank_specifier* (but the same for both array types). This relationship is known as ***array covariance***. Array covariance, in particular, means that a value of an array type `A[R]` might actually be a reference to an instance of an array type `B[R]`, provided an implicit reference conversion exists from `B` to `A`.

Because of array covariance, assignments to elements of reference type arrays include a run-time check which ensures that the value being assigned to the array element is actually of a permitted type ([§12.18.2](expressions.md#12182-simple-assignment)).

> *Example*:
> ```csharp
> class Test
> {
>     static void Fill(object[] array, int index, int count, object value) {
>         for (int i = index; i < index + count; i++) array[i] = value;
>     }
>     static void Main() {
>         string[] strings = new string[100];
>         Fill(strings, 0, 100, "Undefined");
>         Fill(strings, 0, 10, null);
>         Fill(strings, 90, 10, 0);
>     }
> }
> ```
> The assignment to `array[i]` in the `Fill` method implicitly includes a run-time check, which ensures that `value` is either a `null` reference or a reference to an object of a type that is compatible with the actual element type of `array`. In `Main`, the first two invocations of `Fill` succeed, but the third invocation causes a `System.ArrayTypeMismatchException` to be thrown upon executing the first assignment to `array[i]`. The exception occurs because a boxed `int` cannot be stored in a `string` array. *end example*

Array covariance specifically does not extend to arrays of *value_type*s. For example, no conversion exists that permits an `int[]` to be treated as an `object[]`.

## 17.7 Array initializers

Array initializers may be specified in field declarations ([§15.5](classes.md#155-fields)), local variable declarations ([§13.6.2](statements.md#1362-local-variable-declarations)), and array creation expressions ([§12.7.15.5](expressions.md#127155-array-creation-expressions)):

```ANTLR
array_initializer
    : '{' variable_initializer_list? '}'
    | '{' variable_initializer_list ',' '}'
    ;

variable_initializer_list
    : variable_initializer (',' variable_initializer)*
    ;
    
variable_initializer
    : expression
    | array_initializer
    ;
```
An array initializer consists of a sequence of variable initializers, enclosed by "`{`" and "`}`" tokens and separated by "`,`" tokens. Each variable initializer is an expression or, in the case of a multi-dimensional array, a nested array initializer.

The context in which an array initializer is used determines the type of the array being initialized. In an array creation expression, the array type immediately precedes the initializer, or is inferred from the expressions in the array initializer. In a field or variable declaration, the array type is the type of the field or variable being declared. When an array initializer is used in a field or variable declaration,

> *Example*:
> ```csharp
> int[] a = {0, 2, 4, 6, 8};
> ```
> *end example*

> it is simply shorthand for an equivalent array creation expression:

> *Example*:
> ```csharp
> int[] a = new int[] {0, 2, 4, 6, 8};
> ```
> *end example*

For a single-dimensional array, the array initializer shall consist of a sequence of expressions, each having an implicit conversion to the element type of the array ([§11.2](conversions.md#112-implicit-conversions)). The expressions initialize array elements in increasing order, starting with the element at index zero. The number of expressions in the array initializer determines the length of the array instance being created.

> *Example*: The array initializer above creates an `int[]` instance of length 5 and then initializes the instance with the following values:
> ```csharp
> a[0] = 0; a[1] = 2; a[2] = 4; a[3] = 6; a[4] = 8;
> ```
> *end example*

For a multi-dimensional array, the array initializer shall have as many levels of nesting as there are dimensions in the array. The outermost nesting level corresponds to the leftmost dimension and the innermost nesting level corresponds to the rightmost dimension. The length of each dimension of the array is determined by the number of elements at the corresponding nesting level in the array initializer. For each nested array initializer, the number of elements shall be the same as the other array initializers at the same level.

> *Example*: The example:
> ```csharp
> int[,] b = {{0, 1}, {2, 3}, {4, 5}, {6, 7}, {8, 9}};
> ```
> creates a two-dimensional array with a length of five for the leftmost dimension and a length of two for the rightmost dimension:
> ```csharp
> int[,] b = new int[5, 2];
> ```
> and then initializes the array instance with the following values:
> ```csharp
> b[0, 0] = 0; b[0, 1] = 1;
> b[1, 0] = 2; b[1, 1] = 3;
> b[2, 0] = 4; b[2, 1] = 5;
> b[3, 0] = 6; b[3, 1] = 7;
> b[4, 0] = 8; b[4, 1] = 9;
> ```
> *end example*

If a dimension other than the rightmost is given with length zero, the subsequent dimensions are assumed to also have length zero.

> *Example*:
> ```csharp
> int[,] c = {};
> ```
> creates a two-dimensional array with a length of zero for both the leftmost and the rightmost dimension:
> ```csharp
> int[,] c = new int[0, 0];
> ```
> *end example*

When an array creation expression includes both explicit dimension lengths and an array initializer, the lengths shall be constant expressions and the number of elements at each nesting level shall match the corresponding dimension length.

> *Example*: Here are some examples:
> ```csharp
> int i = 3;
> int[] x = new int[3] {0, 1, 2}; // OK
> int[] y = new int[i] {0, 1, 2}; // Error, i not a constant
> int[] z = new int[3] {0, 1, 2, 3}; // Error, length/initializer mismatch
> ```
> Here, the initializer for `y` results in a compile-time error because the dimension length expression is not a constant, and the initializer for `z` results in a compile-time error because the length and the number of elements in the initializer do not agree. *end example*

> *Note*: C# allows a trailing comma at the end of an *array_initializer*. This syntax provides flexibility in adding or deleting members from such a list, and simplifies machine generation of such lists. *end note*
