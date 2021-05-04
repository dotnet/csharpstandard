# 23 Unsafe code

## 23.1 General

An implementation that does not support unsafe code is required to diagnose any usage of the keyword `unsafe`.

**The remainder of this clause, including all of its subclauses, is conditionally normative.**

> *Note*: The core C# language, as defined in the preceding clauses, differs notably from C and C++ in its omission of pointers as a data type. Instead, C# provides references and the ability to create objects that are managed by a garbage collector. This design, coupled with other features, makes C# a much safer language than C or C++. In the core C# language, it is simply not possible to have an uninitialized variable, a "dangling" pointer, or an expression that indexes an array beyond its bounds. Whole categories of bugs that routinely plague C and C++ programs are thus eliminated.
> 
> While practically every pointer type construct in C or C++ has a reference type counterpart in C#, nonetheless, there are situations where access to pointer types becomes a necessity. For example, interfacing with the underlying operating system, accessing a memory-mapped device, or implementing a time-critical algorithm might not be possible or practical without access to pointers. To address this need, C# provides the ability to write ***unsafe code***.
> 
> In unsafe code, it is possible to declare and operate on pointers, to perform conversions between pointers and integral types, to take the address of variables, and so forth. In a sense, writing unsafe code is much like writing C code within a C# program.
> 
> Unsafe code is in fact a "safe" feature from the perspective of both developers and users. Unsafe code shall be clearly marked with the modifier `unsafe`, so developers can't possibly use unsafe features accidentally, and the execution engine works to ensure that unsafe code cannot be executed in an untrusted environment. *end note*

## 23.2 Unsafe contexts

The unsafe features of C# are available only in unsafe contexts. An unsafe context is introduced by including an `unsafe` modifier in the declaration of a type or member, or by employing an *unsafe_statement*:

- A declaration of a class, struct, interface, or delegate may include an `unsafe` modifier, in which case, the entire textual extent of that type declaration (including the body of the class, struct, or interface) is considered an unsafe context. 
  > *Note*: If the *type_declaration* is partial, only that part is an unsafe context. *end note*
- A declaration of a field, method, property, event, indexer, operator, instance constructor, finalizer, or static constructor may include an `unsafe` modifier, in which case, the entire textual extent of that member declaration is considered an unsafe context.
- An *unsafe_statement* enables the use of an unsafe context within a *block*. The entire textual extent of the associated *block* is considered an unsafe context.

The associated grammar extensions are shown below. For brevity, ellipses (...) are used to represent productions that appear in preceding clauses.

```ANTLR
class_modifier
    : '...'
    | 'unsafe'
    ;

struct_modifier
    : '...'
    | 'unsafe'
    ;

interface_modifier
    : '...'
    | 'unsafe'
    ;

delegate_modifier
    : '...'
    | 'unsafe'
    ;

field_modifier
    : '...'
    | 'unsafe'
    ;

method_modifier
    : '...'
    | 'unsafe'
    ;

property_modifier
    : '...'
    | 'unsafe'
    ;

event_modifier
    : '...'
    | 'unsafe'
    ;

indexer_modifier
    : '...'
    | 'unsafe'
    ;

operator_modifier
    : '...'
    | 'unsafe'
    ;

constructor_modifier
    : '...'
    | 'unsafe'
    ;

finalizer_declaration
    : attributes? 'extern'? 'unsafe'? '~' identifier '(' ')' finalizer_body
    | attributes? 'unsafe'? 'extern'? '~' identifier '(' ')' finalizer_body
    ;

static_constructor_modifiers
    : 'extern'? 'unsafe'? 'static'
    | 'unsafe'? 'extern'? 'static'
    | 'extern'? 'static' 'unsafe'?
    | 'unsafe'? 'static' 'extern'?
    | 'static' 'extern'? 'unsafe'?
    | 'static' 'unsafe'? 'extern'?
    ;

embedded_statement
    : '...'
    | unsafe_statement
    | fixed_statement
    ;

unsafe_statement
    : 'unsafe' block
    ;
```

> *Example*: In the following code
> ```csharp
> public unsafe struct Node
> {
>     public int Value;
>     public Node* Left;
>     public Node* Right;
> }
> ```
> the `unsafe` modifier specified in the struct declaration causes the entire textual extent of the struct declaration to become an unsafe context. Thus, it is possible to declare the `Left` and `Right` fields to be of a pointer type. The example above could also be written
> ```csharp
> public struct Node
> {
>     public int Value;
>     public unsafe Node* Left;
>     public unsafe Node* Right;
> }
> ```
> Here, the `unsafe` modifiers in the field declarations cause those declarations to be considered unsafe contexts. *end example*

Other than establishing an unsafe context, thus permitting the use of pointer types, the `unsafe` modifier has no effect on a type or a member.

> *Example*: In the following code
> ```csharp
> public class A
> {
>     public unsafe virtual void F() {
>         char* p;
>         ...
>     }
> }
> public class B: A
> {
>     public override void F() {
>         base.F();
>         ...
>     }
> }
> ```
> the unsafe modifier on the `F` method in `A` simply causes the textual extent of `F` to become an unsafe context in which the unsafe features of the language can be used. In the override of `F` in `B`, there is no need to re-specify the `unsafe` modifier—unless, of course, the `F` method in `B` itself needs access to unsafe features.
> 
> The situation is slightly different when a pointer type is part of the method's signature
> ```csharp
> public unsafe class A
> {
>     public virtual void F(char* p) {...}
> }
> public class B: A
> {
>     public unsafe override void F(char* p) {...}
> }
> ```
> Here, because `F`'s signature includes a pointer type, it can only be written in an unsafe context. However, the unsafe context can be introduced by either making the entire class unsafe, as is the case in `A`, or by including an `unsafe` modifier in the method declaration, as is the case in `B`. *end example*

When the `unsafe` modifier is used on a partial type declaration ([§15.2.7](classes.md#1527-partial-declarations)), only that particular part is considered an unsafe context.

## 23.3 Pointer types

In an unsafe context, a *type* ([§9.1](types.md#91-general)) can be a *pointer_type* as well as a *value_type*, a *reference_type*, or a *type_parameter*. In an unsafe context a pointer-type may also be the element type of an array ([§17](arrays.md#17-arrays)). A *pointer-type* may also be used in a typeof expression ([§12.7.12](expressions.md#12712-the-typeof-operator)) outside of an unsafe context (as such usage is not unsafe).

```ANTLR
type
    : '...'
    | pointer_type
    ;

non_array_type
    : '...'
    | pointer_type
    ;
```

A *pointer_type* is written as an *unmanaged_type* or the keyword `void`, followed by a `*` token:

```ANTLR
pointer_type
    : unmanaged_type '*'
    | 'void' '*'
    ;

unmanaged_type
    : type
    ;
```

The type specified before the `*` in a pointer type is called the ***referent type*** of the pointer type. It represents the type of the variable to which a value of the pointer type points.

A *pointer_type* may only be used in an *array_type* in an unsafe context ([§23.2](unsafe-code.md#232-unsafe-contexts)). A *non_array_type* is any type that is not itself an *array_type*.

Unlike references (values of reference types), pointers are not tracked by the garbage collector—the garbage collector has no knowledge of pointers and the data to which they point. For this reason a pointer is not permitted to point to a reference or to a struct that contains references, and the referent type of a pointer shall be an *unmanaged_type*.

An *unmanaged_type* is any type that isn't a *reference_type*, a *type_parameter*, or a constructed type, and contains no fields whose type is not an *unmanaged_type*. In other words, an *unmanaged_type* is one of the following:

- `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `float`, `double`, `decimal`, or `bool`.
- Any *enum_type*.
- Any *pointer_type*.
- Any user-defined *struct_type* that is not a constructed type and contains fields of *unmanaged_type*s only.

The intuitive rule for mixing of pointers and references is that referents of references (objects) are permitted to contain pointers, but referents of pointers are not permitted to contain references.

> *Example*: Some examples of pointer types are given in the table below:
> Example   | Description
> --------- | -----------
> `byte*`   | Pointer to `byte`
> `char*`   | Pointer to `char`
> `int**`   | Pointer to pointer to `int`
> `int*[]`  | Single-dimensional array of pointers to `int`
> `void*`   | Pointer to unknown type
> *end example*

For a given implementation, all pointer types shall have the same size and representation.

> *Note*: Unlike C and C++, when multiple pointers are declared in the same declaration, in C# the `*` is written along with the underlying type only, not as a prefix punctuator on each pointer name. For example:
> ```csharp
> int* pi, pj; // NOT as int *pi, *pj;  
> ```
> *end note*

The value of a pointer having type `T*` represents the address of a variable of type `T`. The pointer indirection operator `*` ([§23.6.2](unsafe-code.md#2362-pointer-indirection)) can be used to access this variable.

> *Example*: Given a variable `P` of type `int*`, the expression `*P` denotes the `int` variable found at the address contained in `P`. *end example*

Like an object reference, a pointer may be `null`. Applying the indirection operator to a `null`-valued pointer results in implementation-defined behavior ([§23.6.2](unsafe-code.md#2362-pointer-indirection)). A pointer with value `null` is represented by all-bits-zero.

The `void*` type represents a pointer to an unknown type. Because the referent type is unknown, the indirection operator cannot be applied to a pointer of type `void*`, nor can any arithmetic be performed on such a pointer. However, a pointer of type `void*` can be cast to any other pointer type (and vice versa) and compared to values of other pointer types ([§23.6.8](unsafe-code.md#2368-pointer-comparison)).

Pointer types are a separate category of types. Unlike reference types and value types, pointer types do not inherit from `object` and no conversions exist between pointer types and `object`. In particular, boxing and unboxing ([§9.3.12](types.md#9312-boxing-and-unboxing)) are not supported for pointers. However, conversions are permitted between different pointer types and between pointer types and the integral types. This is described in [§23.5](unsafe-code.md#235-pointer-conversions).

A *pointer_type* cannot be used as a type argument ([§9.4](types.md#94-constructed-types)), and type inference ([§12.6.3](expressions.md#1263-type-inference)) fails on generic method calls that would have inferred a type argument to be a pointer type.

A *pointer_type* cannot be used as a type of a subexpression of a dynamically bound operation ([§12.3.3](expressions.md#1233-dynamic-binding)).

A *pointer_type* may be used as the type of a volatile field ([§15.5.4](classes.md#1554-volatile-fields)).

> *Note*: Although pointers can be passed as `ref` or `out` parameters, doing so can cause undefined behavior, since the pointer might well be set to point to a local variable that no longer exists when the called method returns, or the fixed object to which it used to point, is no longer fixed. For example:
> ```csharp
> using System;
> class Test
> {
>     static int value = 20;
>     unsafe static void F(out int* pi1, ref int* pi2) {
>         int i = 10;
>         pi1 = &i;
>         fixed (int* pj = &value) {
>             // ...
>             pi2 = pj;
>         }
>     }
>     static void Main() {
>         int i = 10;
>         unsafe {
>             int* px1;
>             int* px2 = &i;
>             F(out px1, ref px2);
>             Console.WriteLine("*px1 = {0}, *px2 = {1}",
>                *px1, *px2); // undefined behavior
>         }
>     }
> }
> ```
> *end note*

A method can return a value of some type, and that type can be a pointer. 

> *Example*: When given a pointer to a contiguous sequence of `int`s, that sequence's element count, and some other `int` value, the following method returns the address of that value in that sequence, if a match occurs; otherwise it returns `null`:
> ```csharp
> unsafe static int* Find(int* pi, int size, int value) {
>     for (int i = 0; i < size; ++i) {
>         if (*pi == value)
>             return pi;
>             ++pi;
>     }
>     return null;
> }
> ```
> *end example*

In an unsafe context, several constructs are available for operating on pointers:

- The unary `*` operator may be used to perform pointer indirection ([§23.6.2](unsafe-code.md#2362-pointer-indirection)).
- The `->` operator may be used to access a member of a struct through a pointer ([§23.6.3](unsafe-code.md#2363-pointer-member-access)).
- The `[]` operator may be used to index a pointer ([§23.6.4](unsafe-code.md#2364-pointer-element-access)).
- The unary `&` operator may be used to obtain the address of a variable ([§23.6.5](unsafe-code.md#2365-the-address-of-operator)).
- The `++` and `--` operators may be used to increment and decrement pointers ([§23.6.6](unsafe-code.md#2366-pointer-increment-and-decrement)).
- The binary `+` and `-` operators may be used to perform pointer arithmetic ([§23.6.7](unsafe-code.md#2367-pointer-arithmetic)).
- The `==`, `!=`, `<`, `>`, `<=`, and `>=` operators may be used to compare pointers ([§23.6.8](unsafe-code.md#2368-pointer-comparison)).
- The `stackalloc` operator may be used to allocate memory from the call stack ([§23.9](unsafe-code.md#239-stack-allocation)).
- The `fixed` statement may be used to temporarily fix a variable so its address can be obtained ([§23.7](unsafe-code.md#237-the-fixed-statement)).

## 23.4 Fixed and moveable variables

The address-of operator ([§23.6.5](unsafe-code.md#2365-the-address-of-operator)) and the `fixed` statement ([§23.7](unsafe-code.md#237-the-fixed-statement)) divide variables into two categories: ***Fixed variables*** and ***moveable variables***.

Fixed variables reside in storage locations that are unaffected by operation of the garbage collector. (Examples of fixed variables include local variables, value parameters, and variables created by dereferencing pointers.) On the other hand, moveable variables reside in storage locations that are subject to relocation or disposal by the garbage collector. (Examples of moveable variables include fields in objects and elements of arrays.)

The `&` operator ([§23.6.5](unsafe-code.md#2365-the-address-of-operator)) permits the address of a fixed variable to be obtained without restrictions. However, because a moveable variable is subject to relocation or disposal by the garbage collector, the address of a moveable variable can only be obtained using a `fixed statement` ([§23.7](unsafe-code.md#237-the-fixed-statement)), and that address remains valid only for the duration of that `fixed` statement.

In precise terms, a fixed variable is one of the following:

- A variable resulting from a *simple_name* ([§12.7.3](expressions.md#1273-simple-names)) that refers to a local variable, value parameter, or parameter array, unless the variable is captured by an anonymous function ([§12.16.6.2](expressions.md#121662-captured-outer-variables)).
- A variable resulting from a *member_access* ([§12.7.5](expressions.md#1275-member-access)) of the form `V.I`, where `V` is a fixed variable of a *struct_type*.
- A variable resulting from a *pointer_indirection_expression* ([§23.6.2](unsafe-code.md#2362-pointer-indirection)) of the form `*P`, a *pointer_member_access* ([§23.6.3](unsafe-code.md#2363-pointer-member-access)) of the form `P->I`, or a *pointer_element_access* ([§23.6.4](unsafe-code.md#2364-pointer-element-access)) of the form `P[E]`.

All other variables are classified as moveable variables.

A static field is classified as a moveable variable. Also, a `ref` or `out` parameter is classified as a moveable variable, even if the argument given for the parameter is a fixed variable. Finally, a variable produced by dereferencing a pointer is always classified as a fixed variable.

## 23.5 Pointer conversions

### 23.5.1 General

In an unsafe context, the set of available implicit conversions ([§11.2](conversions.md#112-implicit-conversions)) is extended to include the following implicit pointer conversions:

- From any *pointer_type* to the type `void*`.
- From the `null` literal ([§7.4.5.7](lexical-structure.md#7457-the-null-literal)) to any *pointer_type*.

Additionally, in an unsafe context, the set of available explicit conversions ([§11.3](conversions.md#113-explicit-conversions)) is extended to include the following explicit pointer conversions:

- From any *pointer_type* to any other *pointer_type*.
- From `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, or `ulong` to any *pointer_type*.
- From any *pointer_type* to `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, or `ulong`.

Finally, in an unsafe context, the set of standard implicit conversions ([§11.4.2](conversions.md#1142-standard-implicit-conversions)) includes the following pointer conversions:

- From any *pointer_type* to the type `void*`.
- From the `null` literal to any *pointer_type*.

Conversions between two pointer types never change the actual pointer value. In other words, a conversion from one pointer type to another has no effect on the underlying address given by the pointer.

When one pointer type is converted to another, if the resulting pointer is not correctly aligned for the pointed-to type, the behavior is undefined if the result is dereferenced. In general, the concept "correctly aligned" is transitive: if a pointer to type `A` is correctly aligned for a pointer to type `B`, which, in turn, is correctly aligned for a pointer to type `C`, then a pointer to type `A` is correctly aligned for a pointer to type `C`. 

> *Example*: Consider the following case in which a variable having one type is accessed via a pointer to a different type:
> ```csharp
> char c = 'A';
> char* pc = &c;
> void* pv = pc;
> int* pi = (int*)pv;
> int i = *pi; // undefined
> *pi = 123456; // undefined
> ```
> *end example*

When a pointer type is converted to a pointer to `byte`, the result points to the lowest addressed `byte` of the variable. Successive increments of the result, up to the size of the variable, yield pointers to the remaining bytes of that variable.

> *Example*: The following method displays each of the eight bytes in a `double` as a hexadecimal value:
> ```csharp
> using System;
> class Test
> {
>     static void Main() {
>         double d = 123.456e23;
>         unsafe {
>             byte* pb = (byte*)&d;
>             for (int i = 0; i < sizeof(double); ++i)
>                 Console.Write("{0:X2} ", *pb++);
>                 Console.WriteLine();
>         }
>     }
> }
> ```
> Of course, the output produced depends on endianness. *end example*

Mappings between pointers and integers are implementation-defined.

> *Note*: However, on 32- and 64-bit CPU architectures with a linear address space, conversions of pointers to or from integral types typically behave exactly like conversions of `uint` or `ulong` values, respectively, to or from those integral types. *end note*

### 23.5.2 Pointer arrays

Arrays of pointers can be constructed using *array_creation_expression* ([§12.7.11.5](expressions.md#127115-array-creation-expressions)) in an usafe context. Only some of the conversions that apply to other array types are allowed on pointer arrays:

- The implicit reference conversion ([§11.2.5](conversions.md#1125-implicit-nullable-conversions)) from any *array_type* to `System.Array` and the interfaces it implements also applies to pointer arrays. However, any attempt to access the array elements through `System.Array` or the interfaces it implements may result in an exception at run-time, as pointer types are not convertible to `object`.
- The implicit and explicit reference conversions ([§11.2.5](conversions.md#1125-implicit-nullable-conversions), [§11.3.4](conversions.md#1134-explicit-nullable-conversions)) from a single-dimensional array type `S[]` to `System.Collections.Generic.IList<T>` and its generic base interfaces never apply to pointer arrays.
- The explicit reference conversion ([§11.3.4](conversions.md#1134-explicit-nullable-conversions)) from `System.Array` and the interfaces it implements to any *array_type* applies to pointer arrays.
- The explicit reference conversions ([§11.3.4](conversions.md#1134-explicit-nullable-conversions)) from `System.Collections.Generic.IList<S>` and its base interfaces to a single-dimensional array type `T[]` never applies to pointer arrays, since pointer types cannot be used as type arguments, and there are no conversions from pointer types to non-pointer types.

These restrictions mean that the expansion for the `foreach` statement over arrays described in [§10.4.4.17](variables.md#104417-foreach-statements) cannot be applied to pointer arrays. Instead, a `foreach` statement of the form

`foreach (V v in x)` *embedded_statement*

where the type of `x` is an array type of the form `T[,,...,]`, *n* is the number of dimensions minus 1 and `T` or `V` is a pointer type, is expanded using nested for-loops as follows:

```csharp
{
    T[,,...,] a = x; for (int i0 = a.GetLowerBound(0); i0 <= a.GetUpperBound(0); i0++)
    for (int i1 = a.GetLowerBound(1); i1 <= a.GetUpperBound(1); i1++)
    ...
    for (int in = a.GetLowerBound(n); in <= a.GetUpperBound(n); in++) {
        V v = (V)a[i0,i1,...,in];
        *embedded_statement*
    }
}
```

The variables `a`, `i0`, `i1`, ... `in` are not visible to or accessible to `x` or the *embedded_statement* or any other source code of the program. The variable `v` is read-only in the embedded statement. If there is not an explicit conversion ([§23.5](unsafe-code.md#235-pointer-conversions)) from `T` (the element type) to `V`, an error is produced and no further steps are taken. If `x` has the value `null`, a `System.NullReferenceException` is thrown at run-time.

> *Note*: Although pointer types are not permitted as type arguments, pointer arrays may be used as type arguments. *end note*

## 23.6 Pointers in expressions

### 23.6.1 General

In an unsafe context, an expression may yield a result of a pointer type, but outside an unsafe context, it is a compile-time error for an expression to be of a pointer type. In precise terms, outside an unsafe context a compile-time error occurs if any *simple_name* ([§12.7.3](expressions.md#1273-simple-names)), *member_access* ([§12.7.5](expressions.md#1275-member-access)), *invocation_expression* ([§12.7.6](expressions.md#1276-invocation-expressions)), or *element_access* ([§12.7.7](expressions.md#1277-element-access)) is of a pointer type.

In an unsafe context, the *primary_no_array_creation_expression* ([§12.7](expressions.md#127-primary-expressions)) and *unary_expression* ([§12.8](expressions.md#128-unary-operators)) productions permit the following additional constructs:

```ANTLR
primary_no_array_creation_expression
    : '...'
    | pointer_member_access
    | pointer_element_access
    ;

unary_expression
    : '...'
    | pointer_indirection_expression
    | addressof_expression
    ;
```

These constructs are described in the following subclauses.

> *Note*: The precedence and associativity of the unsafe operators is implied by the grammar. *end note*

### 23.6.2 Pointer indirection

A *pointer_indirection_expression* consists of an asterisk (`*`) followed by a *unary_expression*.

```ANTLR
pointer_indirection_expression
    : '*' unary_expression
    ;
```

The unary `*` operator denotes pointer indirection and is used to obtain the variable to which a pointer points. The result of evaluating `*P`, where `P` is an expression of a pointer type `T*`, is a variable of type `T`. It is a compile-time error to apply the unary `*` operator to an expression of type `void*` or to an expression that isn't of a pointer type.

The effect of applying the unary `*` operator to a `null`-valued pointer is implementation-defined. In particular, there is no guarantee that this operation throws a `System.NullReferenceException`.

If an invalid value has been assigned to the pointer, the behavior of the unary `*` operator is undefined. 

> *Note*: Among the invalid values for dereferencing a pointer by the unary `*` operator are an address inappropriately aligned for the type pointed to (see example in [§23.5](unsafe-code.md#235-pointer-conversions)), and the address of a variable after the end of its lifetime.

For purposes of definite assignment analysis, a variable produced by evaluating an expression of the form `*P` is considered initially assigned ([§10.4.2](variables.md#1042-initially-assigned-variables)).

### 23.6.3 Pointer member access

A *pointer_member_access* consists of a *primary_expression*, followed by a "`->`" token, followed by an *identifier* and an optional *type_argument_list*.

```ANTLR
pointer_member_access
    : primary_expression '->' identifier type_argument_list?
    ;
```

In a pointer member access of the form `P->I`, `P` shall be an expression of a pointer type, and `I` shall denote an accessible member of the type to which `P` points.

A pointer member access of the form `P->I` is evaluated exactly as `(*P).I`. For a description of the pointer indirection operator (`*`), see [§23.6.2](unsafe-code.md#2362-pointer-indirection). For a description of the member access operator (`.`), see [§12.7.5](expressions.md#1275-member-access).

> *Example*: In the following code
> ```csharp
> using System;
> 
> struct Point
> {
>     public int x;
>     public int y;
>     public override string ToString() {
>         return "(" + x + "," + y + ")";
>     }
> }
> class Test
> {
>     static void Main() {
>         Point point;
>         unsafe {
>             Point* p = &point;
>             p->x = 10;
>             p->y = 20;
>             Console.WriteLine(p->ToString());
>         }
>     }
> }
> ```
> the `->` operator is used to access fields and invoke a method of a struct through a pointer. Because the operation `P->I` is precisely equivalent to `(*P).I`, the `Main` method could equally well have been written:
> ```csharp
> class Test
> {
>     static void Main() {
>         Point point;
>         unsafe {
>             Point* p = &point;
>             (*p).x = 10;
>             (*p).y = 20;
>             Console.WriteLine((*p).ToString());
>         }
>     }
> }
> ```
> *end example*

### 23.6.4 Pointer element access

A *pointer_element_access* consists of a *primary_no_array_creation_expression* followed by an expression enclosed in "`[`" and "`]`".

```ANTLR
pointer_element_access
    : primary_no_array_creation_expression '[' expression ']'
    ;
```

In a pointer element access of the form `P[E]`, `P` shall be an expression of a pointer type other than `void*`, and `E` shall be an expression that can be implicitly converted to `int`, `uint`, `long`, or `ulong`.

A pointer element access of the form `P[E]` is evaluated exactly as `*(P + E)`. For a description of the pointer indirection operator (`*`), see [§23.6.2](unsafe-code.md#2362-pointer-indirection). For a description of the pointer addition operator (`+`), see [§23.6.7](unsafe-code.md#2367-pointer-arithmetic).

> *Example*: In the following code
> ```csharp
> class Test
> {
>     static void Main() {
>         unsafe {
>             char* p = stackalloc char[256];
>             for (int i = 0; i < 256; i++) p[i] = (char)i;
>         }
>     }
> }
> ```
> a pointer element access is used to initialize the character buffer in a `for` loop. Because the operation `P[E]` is precisely equivalent to `*(P + E)`, the example could equally well have been written:
> ```csharp
> class Test
> {
>     static void Main() {
>         unsafe {
>             char* p = stackalloc char[256];
>             for (int i = 0; i < 256; i++) *(p + i) = (char)i;
>         }
>     }
> }
> ```
> *end example*

The pointer element access operator does not check for out-of-bounds errors and the behavior when accessing an out-of-bounds element is undefined. 

> *Note*: This is the same as C and C++. *end note*

### 23.6.5 The address-of operator

An *addressof_expression* consists of an ampersand (`&`) followed by a *unary_expression*.

```ANTLR
addressof_expression
    : '&' unary_expression
    ;
```

Given an expression `E` which is of a type `T` and is classified as a fixed variable ([§23.4](unsafe-code.md#234-fixed-and-moveable-variables)), the construct `&E` computes the address of the variable given by `E`. The type of the result is `T*` and is classified as a value. A compile-time error occurs if `E` is not classified as a variable, if `E` is classified as a read-only local variable, or if `E` denotes a moveable variable. In the last case, a fixed statement ([§23.7](unsafe-code.md#237-the-fixed-statement)) can be used to temporarily "fix" the variable before obtaining its address. 

> *Note*: As stated in [§12.7.5](expressions.md#1275-member-access), outside an instance constructor or static constructor for a struct or class that defines a `readonly` field, that field is considered a value, not a variable. As such, its address cannot be taken. Similarly, the address of a constant cannot be taken.

The `&` operator does not require its argument to be definitely assigned, but following an `&` operation, the variable to which the operator is applied is considered definitely assigned in the execution path in which the operation occurs. It is the responsibility of the programmer to ensure that correct initialization of the variable actually does take place in this situation.

> *Example*: In the following code
> ```csharp
> using System;
> class Test
> {
>     static void Main() {
>         int i;
>         unsafe {
>             int* p = &i;
>             *p = 123;
>         }
>         Console.WriteLine(i);
>     }
> }
> ```
> `i` is considered definitely assigned following the `&i` operation used to initialize `p`. The assignment to `*p` in effect initializes `i`, but the inclusion of this initialization is the responsibility of the programmer, and no compile-time error would occur if the assignment was removed. *end example*

> *Note*: The rules of definite assignment for the `&` operator exist such that redundant initialization of local variables can be avoided. For example, many external APIs take a pointer to a structure which is filled in by the API. Calls to such APIs typically pass the address of a local struct variable, and without the rule, redundant initialization of the struct variable would be required. *end note*

> *Note*: When a local variable, value parameter, or parameter array is captured by an anonymous function ([§12.7.17](expressions.md#12717-anonymous-method-expressions)), that local variable, parameter, or parameter array is no longer considered to be a fixed variable ([§23.7](unsafe-code.md#237-the-fixed-statement)), but is instead considered to be a moveable variable. Thus it is an error for any unsafe code to take the address of a local variable, value parameter, or parameter array that has been captured by an anonymous function. *end note*

### 23.6.6 Pointer increment and decrement

In an unsafe context, the `++` and `--` operators ([§12.7.10](expressions.md#12710-postfix-increment-and-decrement-operators) and [§12.8.6](expressions.md#1286-prefix-increment-and-decrement-operators)) can be applied to pointer variables of all types except `void*`. Thus, for every pointer type `T*`, the following operators are implicitly defined:

```csharp
T* operator ++(T* x);
T* operator --(T* x);
```

The operators produce the same results as `x+1` and `x-1`, respectively ([§23.6.7](unsafe-code.md#2367-pointer-arithmetic)). In other words, for a pointer variable of type `T*`, the `++` operator adds `sizeof(T)` to the address contained in the variable, and the `--` operator subtracts `sizeof(T)` from the address contained in the variable.

If a pointer increment or decrement operation overflows the domain of the pointer type, the result is implementation-defined, but no exceptions are produced.

### 23.6.7 Pointer arithmetic

In an unsafe context, the `+` operator ([§12.9.5](expressions.md#1295-addition-operator)) and `–` operator ([§12.9.6](expressions.md#1296-subtraction-operator)) can be applied to values of all pointer types except `void*`. Thus, for every pointer type `T*`, the following operators are implicitly defined:

```csharp
T* operator +(T* x, int y);
T* operator +(T* x, uint y);
T* operator +(T* x, long y);
T* operator +(T* x, ulong y);
T* operator +(int x, T* y);
T* operator +(uint x, T* y);
T* operator +(long x, T* y);
T* operator +(ulong x, T* y);
T* operator –(T* x, int y);
T* operator –(T* x, uint y);
T* operator –(T* x, long y);
T* operator –(T* x, ulong y);
long operator –(T* x, T* y);
```

Given an expression `P` of a pointer type `T*` and an expression `N` of type `int`, `uint`, `long`, or `ulong`, the expressions `P + N` and `N + P` compute the pointer value of type `T*` that results from adding `N * sizeof(T)` to the address given by `P`. Likewise, the expression `P – N` computes the pointer value of type `T*` that results from subtracting `N * sizeof(T)` from the address given by `P`.

Given two expressions, `P` and `Q`, of a pointer type `T*`, the expression `P – Q` computes the difference between the addresses given by `P` and `Q` and then divides that difference by `sizeof(T)`. The type of the result is always `long`. In effect, `P - Q` is computed as `((long)(P) - (long)(Q)) / sizeof(T)`. 

> *Example*:
> ```csharp
> using System;
> class Test
> {
>     static void Main() {
>         unsafe {
>             int* values = stackalloc int[20];
>             int* p = &values[1];
>             int* q = &values[15];
>             Console.WriteLine("p - q = {0}", p - q);
>             Console.WriteLine("q - p = {0}", q - p);
>         }
>     }
> }
> ```
> which produces the output:
> ```console
> p - q = -14
> q - p = 14
> ```
> *end example*

If a pointer arithmetic operation overflows the domain of the pointer type, the result is truncated in an implementation-defined fashion, but no exceptions are produced.

### 23.6.8 Pointer comparison

In an unsafe context, the `==`, `!=`, `<`, `>`, `<=`, and `>=` operators ([§12.11](expressions.md#1211-relational-and-type-testing-operators)) can be applied to values of all pointer types. The pointer comparison operators are:

```csharp
bool operator ==(void* x, void* y);
bool operator !=(void* x, void* y);
bool operator <(void* x, void* y);
bool operator >(void* x, void* y);
bool operator <=(void* x, void* y);
bool operator >=(void* x, void* y);
```

Because an implicit conversion exists from any pointer type to the `void*` type, operands of any pointer type can be compared using these operators. The comparison operators compare the addresses given by the two operands as if they were unsigned integers.

### 23.6.9 The sizeof operator

For certain predefined types ([§12.7.13](expressions.md#12713-the-sizeof-operator)), the `sizeof` operator yields a constant `int` value. For all other types, the result of the `sizeof` operator is implementation-defined and is classified as a value, not a constant.

The order in which members are packed into a struct is unspecified.

For alignment purposes, there may be unnamed padding at the beginning of a struct, within a struct, and at the end of the struct. The contents of the bits used as padding are indeterminate.

When applied to an operand that has struct type, the result is the total number of bytes in a variable of that type, including any padding.

## 23.7 The fixed statement

In an unsafe context, the *embedded_statement* ([§13.1](statements.md#131-general)) production permits an additional construct, the fixed statement, which is used to "fix" a moveable variable such that its address remains constant for the duration of the statement.

```ANTLR
fixed_statement
    : 'fixed' '(' pointer_type fixed_pointer_declarators ')' embedded_statement
    ;

fixed_pointer_declarators
    : fixed_pointer_declarator (','  fixed_pointer_declarator)*
    ;

fixed_pointer_declarator
    : identifier '=' fixed_pointer_initializer
    ;

fixed_pointer_initializer
    : '&' variable_reference
    | expression
    ;
```

Each *fixed_pointer_declarator* declares a local variable of the given *pointer_type* and initializes that local variable with the address computed by the corresponding *fixed_pointer_initializer*. A local variable declared in a fixed statement is accessible in any *fixed_pointer_initializer*s occurring to the right of that variable's declaration, and in the *embedded_statement* of the fixed statement. A local variable declared by a fixed statement is considered read-only. A compile-time error occurs if the embedded statement attempts to modify this local variable (via assignment or the `++` and `--` operators) or pass it as a `ref` or `out` parameter.

It is an error to use a captured local variable ([§12.16.6.2](expressions.md#121662-captured-outer-variables)), value parameter, or parameter array in a *fixed_pointer_initializer*.A *fixed_pointer_initializer* can be one of the following:

- The token "`&`" followed by a *variable_reference* ([§10.4.4](variables.md#1044-precise-rules-for-determining-definite-assignment)) to a moveable variable ([§23.4](unsafe-code.md#234-fixed-and-moveable-variables)) of an unmanaged type `T`, provided the type `T*` is implicitly convertible to the pointer type given in the `fixed` statement. In this case, the initializer computes the address of the given variable, and the variable is guaranteed to remain at a fixed address for the duration of the fixed statement.
- An expression of an *array_type* with elements of an unmanaged type `T`, provided the type `T*` is implicitly convertible to the pointer type given in the fixed statement. In this case, the initializer computes the address of the first element in the array, and the entire array is guaranteed to remain at a fixed address for the duration of the `fixed` statement. If the array expression is `null` or if the array has zero elements, the initializer computes an address equal to zero.
- An expression of type `string`, provided the type `char*` is implicitly convertible to the pointer type given in the `fixed` statement. In this case, the initializer computes the address of the first character in the string, and the entire string is guaranteed to remain at a fixed address for the duration of the `fixed` statement. The behavior of the `fixed` statement is implementation-defined if the string expression is `null`.
- A *simple_name* or *member_access* that references a fixed-size buffer member of a moveable variable, provided the type of the fixed-size buffer member is implicitly convertible to the pointer type given in the `fixed` statement. In this case, the initializer computes a pointer to the first element of the fixed-size buffer ([§23.8.3](unsafe-code.md#2383-fixed-size-buffers-in-expressions)), and the fixed-size buffer is guaranteed to remain at a fixed address for the duration of the `fixed` statement.

For each address computed by a *fixed_pointer_initializer* the `fixed` statement ensures that the variable referenced by the address is not subject to relocation or disposal by the garbage collector for the duration of the `fixed` statement.

> *Example*: If the address computed by a *fixed_pointer_initializer* references a field of an object or an element of an array instance, the fixed statement guarantees that the containing object instance is not relocated or disposed of during the lifetime of the statement. *end example*

It is the programmer's responsibility to ensure that pointers created by fixed statements do not survive beyond execution of those statements. 

> *Example*: When pointers created by `fixed` statements are passed to external APIs, it is the programmer’s responsibility to ensure that the APIs retain no memory of these pointers. *end example*

Fixed objects can cause fragmentation of the heap (because they can't be moved). For that reason, objects should be fixed only when absolutely necessary and then only for the shortest amount of time possible. 

> *Example*: The example
> ```csharp
> class Test
> {
>     static int x;
>     int y;
>     unsafe static void F(int* p) {
>         *p = 1;
>     }
>     static void Main() {
>         Test t = new Test();
>         int[] a = new int[10];
>         unsafe {
>             fixed (int* p = &x) F(p);
>             fixed (int* p = &t.y) F(p);
>             fixed (int* p = &a[0]) F(p);
>             fixed (int* p = a) F(p);
>         }
>     }
> }
> ```
> demonstrates several uses of the `fixed` statement. The first statement fixes and obtains the address of a static field, the second statement fixes and obtains the address of an instance field, and the third statement fixes and obtains the address of an array element. In each case, it would have been an error to use the regular `&` operator since the variables are all classified as moveable variables.
> 
> The third and fourth `fixed` statements in the example above produce identical results. In general, for an array instance `a`, specifying `a[0]` in a `fixed` statement is the same as simply specifying `a`. *end example*

In an unsafe context, array elements of single-dimensional arrays are stored in increasing index order, starting with index `0` and ending with index `Length – 1`. For multi-dimensional arrays, array elements are stored such that the indices of the rightmost dimension are increased first, then the next left dimension, and so on to the left.

Within a `fixed` statement that obtains a pointer `p` to an array instance `a`, the pointer values ranging from `p` to `p + a.Length - 1` represent addresses of the elements in the array. Likewise, the variables ranging from `p[0]` to `p[a.Length - 1]` represent the actual array elements. Given the way in which arrays are stored, we can treat an array of any dimension as though it were linear.

> *Example*:
> ```csharp
> using System;
> class Test
> {
>     static void Main() {
>         int[,,] a = new int[2,3,4];
>         unsafe {
>             fixed (int* p = a) {
>             for (int i = 0; i < a.Length; ++i) // treat as linear
>                 p[i] = i;
>         }
>     }
>     for (int i = 0; i < 2; ++i)
>         for (int j = 0; j < 3; ++j) {
>             for (int k = 0; k < 4; ++k)
>                 Console.Write("[{0},{1},{2}] = {3,2} ", i, j, k, a[i,j,k]);
>                 Console.WriteLine();
>         }
>     }
> }
> ```
> which produces the output:
> ```console
> [0,0,0] = 0 [0,0,1] = 1 [0,0,2] = 2 [0,0,3] = 3
> [0,1,0] = 4 [0,1,1] = 5 [0,1,2] = 6 [0,1,3] = 7
> [0,2,0] = 8 [0,2,1] = 9 [0,2,2] = 10 [0,2,3] = 11
> [1,0,0] = 12 [1,0,1] = 13 [1,0,2] = 14 [1,0,3] = 15
> [1,1,0] = 16 [1,1,1] = 17 [1,1,2] = 18 [1,1,3] = 19
> [1,2,0] = 20 [1,2,1] = 21 [1,2,2] = 22 [1,2,3] = 23
> ```
> *end example*

> *Example*: In the following code
> ```csharp
> class Test
> {
>     unsafe static void Fill(int* p, int count, int value) {
>         for (; count != 0; count--) *p++ = value;
>     }
>     static void Main() {
>         int[] a = new int[100];
>         unsafe {
>             fixed (int* p = a) Fill(p, 100, -1);
>         }
>     }
> }
> ```
> a `fixed` statement is used to fix an array so its address can be passed to a method that takes a pointer. *end example*

A `char*` value produced by fixing a string instance always points to a null-terminated string. Within a fixed statement that obtains a pointer `p` to a string instance `s`, the pointer values ranging from `p` to `p + s.Length ‑ 1` represent addresses of the characters in the string, and the pointer value `p + s.Length` always points to a null character (the character with value '\0').

> *Example*:
> ```csharp
> class Test
> {
>     static string name = "xx";
> 
>     unsafe static void F(char* p) {
>         for (int i = 0; p[i] != '\\0'; ++i)
>             Console.WriteLine(p[i]);
>     }
>     static void Main() {
>         unsafe {
>             fixed (char* p = name) F(p);
>             fixed (char* p = "xx") F(p);
>         }
>     }
> }
> ```
> *end example*

Modifying objects of managed type through fixed pointers can result in undefined behavior. 

> *Note*: For example, because strings are immutable, it is the programmer's responsibility to ensure that the characters referenced by a pointer to a fixed string are not modified. *end note*

> *Note*: The automatic null-termination of strings is particularly convenient when calling external APIs that expect "C-style" strings. Note, however, that a string instance is permitted to contain null characters. If such null characters are present, the string will appear truncated when treated as a null-terminated `char*`. *end note*

## 23.8 Fixed-size buffers

### 23.8.1 General

Fixed-size buffers are used to declare "C-style" in-line arrays as members of structs, and are primarily useful for interfacing with unmanaged APIs.

### 23.8.2 Fixed-size buffer declarations

A ***fixed-size buffer*** is a member that represents storage for a fixed-length buffer of variables of a given type. A fixed-size buffer declaration introduces one or more fixed-size buffers of a given element type. 

> *Note*: Like an array, a fixed-size buffer can be thought of as containing elements.  As such, the term *element type* as defined for an array is also used with a fixed-size buffer. *end note*

Fixed-size buffers are only permitted in struct declarations and may only occur in unsafe contexts ([§23.2](unsafe-code.md#232-unsafe-contexts)).

```ANTLR
struct_member_declaration
    : '...'
    | fixed_size_buffer_declaration
    ;

fixed_size_buffer_declaration
    : attributes? fixed_size_buffer_modifier* 'fixed' buffer_element_type fixed_size_buffer_declarator+ ';'
    ;

fixed_size_buffer_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | 'unsafe'
    ;

buffer_element_type
    : type
    ;

fixed_size_buffer_declarator
    : identifier '[' constant_expression ']'
    ;
```

A fixed-size buffer declaration may include a set of attributes ([§22](attributes.md#22-attributes)), a `new` modifier ([§15.3.5](classes.md#1535-the-new-modifier)), a valid combination of the four access modifiers ([§15.3.6](classes.md#1536-access-modifiers)) and an `unsafe` modifier ([§23.2](unsafe-code.md#232-unsafe-contexts)). The attributes and modifiers apply to all of the members declared by the fixed-size buffer declaration. It is an error for the same modifier to appear multiple times in a fixed-size buffer declaration.

A fixed-size buffer declaration is not permitted to include the `static` modifier.

The buffer element type of a fixed-size buffer declaration specifies the element type of the buffer(s) introduced by the declaration. The buffer element type shall be one of the predefined types `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `float`, `double`, or `bool`.

The buffer element type is followed by a list of fixed-size buffer declarators, each of which introduces a new member. A fixed-size buffer declarator consists of an identifier that names the member, followed by a constant expression enclosed in `[` and `]` tokens. The constant expression denotes the number of elements in the member introduced by that fixed-size buffer declarator. The type of the constant expression shall be implicitly convertible to type `int`, and the value shall be a non-zero positive integer.

The elements of a fixed-size buffer shall be laid out sequentially in memory.

A fixed-size buffer declaration that declares multiple fixed-size buffers is equivalent to multiple declarations of a single fixed-size buffer declaration with the same attributes, and element types. 

> *Example*:
> ```csharp
> unsafe struct A
> {
>     public fixed int x[5], y[10], z[100];
> }
> ```
> is equivalent to
> ```csharp
> unsafe struct A
> {
>     public fixed int x[5];
>     public fixed int y[10];
>     public fixed int z[100];
> }
> ```
> *end example*

### 23.8.3 Fixed-size buffers in expressions

Member lookup ([§12.5](expressions.md#125-member-lookup)) of a fixed-size buffer member proceeds exactly like member lookup of a field.

A fixed-size buffer can be referenced in an expression using a *simple_name* ([§12.6.3](expressions.md#1263-type-inference)) or a *member_access* ([§12.6.5](expressions.md#1265-compile-time-checking-of-dynamic-member-invocation)).

When a fixed-size buffer member is referenced as a simple name, the effect is the same as a member access of the form `this.I`, where `I` is the fixed-size buffer member.

In a member access of the form `E.I`, if `E` is of a struct type and a member lookup of `I` in that struct type identifies a fixed-size member, then `E.I` is evaluated an classified as follows:

- If the expression `E.I` does not occur in an unsafe context, a compile-time error occurs.
- If `E` is classified as a value, a compile-time error occurs.
- Otherwise, if `E` is a moveable variable ([§23.4](unsafe-code.md#234-fixed-and-moveable-variables)) and the expression `E.I` is not a *fixed_pointer_initializer* ([§23.7](unsafe-code.md#237-the-fixed-statement)), a compile-time error occurs.
- Otherwise, `E` references a fixed variable and the result of the expression is a pointer to the first element of the fixed-size buffer member `I` in `E`. The result is of type `S*`, where S is the element type of `I`, and is classified as a value.

The subsequent elements of the fixed-size buffer can be accessed using pointer operations from the first element. Unlike access to arrays, access to the elements of a fixed-size buffer is an unsafe operation and is not range checked.

> *Example*: The following declares and uses a struct with a fixed-size buffer member.
> ```csharp
> unsafe struct Font
> {
>     public int size;
>     public fixed char name[32];
> }
> class Test
> {
>     unsafe static void PutString(string s, char* buffer, int bufSize) {
>         int len = s.Length;
>         if (len > bufSize) len = bufSize;
>         for (int i = 0; i < len; i++) buffer[i] = s[i];
>         for (int i = len; i < bufSize; i++) buffer[i] = (char)0;
>     }
>     unsafe static void Main()
>     {
>         Font f;
>         f.size = 10;
>         PutString("Times New Roman", f.name, 32);
>     }
> }
> ```
> *end example*

### 23.8.4 Definite assignment checking

Fixed-size buffers are not subject to definite assignment-checking ([§10.4](variables.md#104-definite-assignment)), and fixed-size buffer members are ignored for purposes of definite-assignment checking of struct type variables.

When the outermost containing struct variable of a fixed-size buffer member is a static variable, an instance variable of a class instance, or an array element, the elements of the fixed-size buffer are automatically initialized to their default values ([§10.3](variables.md#103-default-values)). In all other cases, the initial content of a fixed-size buffer is undefined.

## 23.9 Stack allocation

In an unsafe context, a local variable declaration ([§13.6.2](statements.md#1362-local-variable-declarations)) may include a stack allocation initializer, which allocates memory from the call stack.

```ANTLR
local_variable_initializer
    : '...'
    | stackalloc_initializer
    ;

stackalloc_initializer
    : 'stackalloc' unmanaged_type '[' expression ']'
    ;
```

The *unmanaged_type* indicates the type of the items that will be stored in the newly allocated location, and the *expression* indicates the number of these items. Taken together, these specify the required allocation size. Since the size of a stack allocation cannot be negative, it is a compile-time error to specify the number of items as a *constant_expression* that evaluates to a negative value.

A stack allocation initializer of the form stackalloc `T[E]` requires `T` to be an unmanaged type ([§23.3](unsafe-code.md#233-pointer-types)) and `E` to be an expression implicitly convertible to type int. The construct allocates `E * sizeof(T)` bytes from the call stack and returns a pointer, of type `T*`, to the newly allocated block. If `E` is a negative value, then the behavior is undefined. If `E` is zero, then no allocation is made, and the pointer returned is implementation-defined. If there is not enough memory available to allocate a block of the given size, a `System.StackOverflowException` is thrown.

The content of the newly allocated memory is undefined.

Stack allocation initializers are not permitted in `catch` or `finally` blocks ([§13.11](statements.md#1311-the-try-statement)).

> *Note*: There is no way to explicitly free memory allocated using stackalloc. *end note*

All stack-allocated memory blocks created during the execution of a function member are automatically discarded when that function member returns.

> *Note*: This corresponds to the `alloca` function, an extension commonly found in C and C++ implementations. *end note*

> *Example*: In the following code
> ```csharp
> using System;
> class Test
> {
>     static string IntToString(int value) {
>         int n = value >= 0 ? value : -value;
>         unsafe {
>             char* buffer = stackalloc char[16];
>             char* p = buffer + 16;
>             do {
>                 *--p = (char)(n % 10 + '0');
>                 n /= 10;
>             } while (n != 0);
>             if (value < 0) *--p = '-';
>             return new string(p, 0, (int)(buffer + 16 - p));
>         }
>     }
>     static void Main() {
>         Console.WriteLine(IntToString(12345));
>         Console.WriteLine(IntToString(-999));
>     }
> }
> ```
> a `stackalloc` initializer is used in the `IntToString` method to allocate a buffer of 16 characters on the stack. The buffer is automatically discarded when the method returns. *end example*

Except for the `stackalloc` operator, C# provides no predefined constructs for managing non-garbage collected memory. Such services are typically provided by supporting class libraries or imported directly from the underlying operating system.

**End of conditionally normative text.**
