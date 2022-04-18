# 15 Structs

## 15.1 General

Structs are similar to classes in that they represent data structures that can contain data members and function members. However, unlike classes, structs are value types and do not require heap allocation. A variable of a `struct` type directly contains the data of the `struct`, whereas a variable of a class type contains a reference to the data, the latter known as an object.

> *Note*: Structs are particularly useful for small data structures that have value semantics. Complex numbers, points in a coordinate system, or key-value pairs in a dictionary are all good examples of structs. Key to these data structures is that they have few data members, that they do not require use of inheritance or reference semantics, rather they can be conveniently implemented using value semantics where assignment copies the value instead of the reference. *end note*

As described in [§8.3.5](types.md#835-simple-types), the simple types provided by C#, such as `int`, `double`, and `bool`, are, in fact, all struct types.

## 15.2 Struct declarations

### 15.2.1 General

A *struct_declaration* is a *type_declaration* ([§13.7](namespaces.md#137-type-declarations)) that declares a new struct:

```ANTLR
struct_declaration
    : attributes? struct_modifier* 'partial'? 'struct' identifier
      type_parameter_list? struct_interfaces?
      type_parameter_constraints_clause* struct_body ';'?
    ;
```

A *struct_declaration* consists of an optional set of *attributes* ([§21](attributes.md#21-attributes)), followed by an optional set of *struct_modifier*s ([§15.2.2](structs.md#1522-struct-modifiers)), followed by an optional partial modifier ([§14.2.7](classes.md#1427-partial-declarations)), followed by the keyword `struct` and an *identifier* that names the struct, followed by an optional *type_parameter_list* specification ([§14.2.3](classes.md#1423-type-parameters)), followed by an optional *struct_interfaces* specification ([§15.2.4](structs.md#1524-struct-interfaces)), followed by an optional *type_parameter_constraints-clauses* specification ([§14.2.5](classes.md#1425-type-parameter-constraints)), followed by a *struct_body* ([§15.2.5](structs.md#1525-struct-body)), optionally followed by a semicolon.

A struct declaration shall not supply a *type_parameter_constraints_clauses* unless it also supplies a *type_parameter_list*.

A struct declaration that supplies a *type_parameter_list* is a generic struct declaration. Additionally, any struct nested inside a generic class declaration or a generic struct declaration is itself a generic struct declaration, since type arguments for the containing type shall be supplied to create a constructed type ([§8.4](types.md#84-constructed-types)).

### 15.2.2 Struct modifiers

A *struct_declaration* may optionally include a sequence of *struct_modifier*s:

```ANTLR
struct_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | unsafe_modifier   // unsafe code support
    ;
```

*unsafe_modifier* ([§22.2](unsafe-code.md#222-unsafe-contexts)) is only available in unsafe code ([§22](unsafe-code.md#22-unsafe-code)).

It is a compile-time error for the same modifier to appear multiple times in a struct declaration.

The modifiers of a struct declaration have the same meaning as those of a class declaration ([§14.2.2](classes.md#1422-class-modifiers)).

### 15.2.3 Partial modifier

The `partial` modifier indicates that this *struct_declaration* is a partial type declaration. Multiple partial struct declarations with the same name within an enclosing namespace or type declaration combine to form one struct declaration, following the rules specified in [§14.2.7](classes.md#1427-partial-declarations).

### 15.2.4 Struct interfaces

A struct declaration may include a *struct_interfaces* specification, in which case the struct is said to directly implement the given interface types. For a constructed struct type, including a nested type declared within a generic type declaration ([§14.3.9.7](classes.md#14397-nested-types-in-generic-classes)), each implemented interface type is obtained by substituting, for each *type_parameter* in the given interface, the corresponding *type_argument* of the constructed type.

```ANTLR
struct_interfaces
    : ':' interface_type_list
    ;
```

The handling of interfaces on multiple parts of a partial struct declaration ([§14.2.7](classes.md#1427-partial-declarations)) are discussed further in [§14.2.4.3](classes.md#14243-interface-implementations).

Interface implementations are discussed further in [§17.6](interfaces.md#176-interface-implementations).

### 15.2.5 Struct body

The *struct_body* of a struct defines the members of the struct.

```ANTLR
struct_body
    : '{' struct_member_declaration* '}'
    ;
```

## 15.3 Struct members

The members of a struct consist of the members introduced by its *struct_member_declaration*s and the members inherited from the type `System.ValueType`.

```ANTLR
struct_member_declaration
    : constant_declaration
    | field_declaration
    | method_declaration
    | property_declaration
    | event_declaration
    | indexer_declaration
    | operator_declaration
    | constructor_declaration
    | static_constructor_declaration
    | type_declaration
    | fixed_size_buffer_declaration   // unsafe code support
    ;
```

*fixed_size_buffer_declaration* ([§22.8.2](unsafe-code.md#2282-fixed-size-buffer-declarations)) is only available in unsafe code ([§22](unsafe-code.md#22-unsafe-code)).

> *Note*: All kinds of *class_member_declaration*s except *finalizer_declaration* are also *struct_member_declaration*s. *end note*

Except for the differences noted in [§15.4](structs.md#154-class-and-struct-differences), the descriptions of class members provided in [§14.3](classes.md#143-class-members) through [§14.12](classes.md#1412-static-constructors) apply to struct members as well.

## 15.4 Class and struct differences

### 15.4.1 General

Structs differ from classes in several important ways:

- Structs are value types ([§15.4.2](structs.md#1542-value-semantics)).
- All struct types implicitly inherit from the class `System.ValueType` ([§15.4.3](structs.md#1543-inheritance)).
- Assignment to a variable of a struct type creates a *copy* of the value being assigned ([§15.4.4](structs.md#1544-assignment)).
- The default value of a struct is the value produced by setting all fields to their default value ([§15.4.5](structs.md#1545-default-values)).
- Boxing and unboxing operations are used to convert between a struct type and certain reference types ([§15.4.6](structs.md#1546-boxing-and-unboxing)).
- The meaning of `this` is different within struct members ([§15.4.7](structs.md#1547-meaning-of-this)).
- Instance field declarations for a struct are not permitted to include variable initializers ([§15.4.8](structs.md#1548-field-initializers)).
- A struct is not permitted to declare a parameterless instance constructor ([§15.4.9](structs.md#1549-constructors)).
- A struct is not permitted to declare a finalizer.

### 15.4.2 Value semantics

Structs are value types ([§8.3](types.md#83-value-types)) and are said to have value semantics. Classes, on the other hand, are reference types ([§8.2](types.md#82-reference-types)) and are said to have reference semantics.

A variable of a struct type directly contains the data of the struct, whereas a variable of a class type contains a reference to an object that contains the data. When a struct `B` contains an instance field of type `A` and `A` is a struct type, it is a compile-time error for `A` to depend on `B` or a type constructed from `B`. `A struct X` *directly depends on* a struct `Y` if `X` contains an instance field of type `Y`. Given this definition, the complete set of structs upon which a struct depends is the transitive closure of the *directly depends on* relationship.

> *Example*:
>
> ```csharp
> struct Node
> {
>     int data;
>     Node next; // error, Node directly depends on itself
> }
> ```
>
> is an error because `Node` contains an instance field of its own type. Another example
>
> ```csharp
> struct A { B b; }
> struct B { C c; }
> struct C { A a; }
> ```
>
> is an error because each of the types `A`, `B`, and `C` depend on each other.
>
> *end example*

With classes, it is possible for two variables to reference the same object, and thus possible for operations on one variable to affect the object referenced by the other variable. With structs, the variables each have their own copy of the data (except in the case of `ref` and `out` parameter variables), and it is not possible for operations on one to affect the other. Furthermore, except when explicitly nullable ([§8.3.11](types.md#8311-nullable-value-types)), it is not possible for values of a struct type to be `null`.

> *Note*: If a struct contains a field of reference type then the contents of the object referenced can be altered by other operations. However the value of the field itself, i.e., which object it references, cannot be changed through a mutation of a different struct value. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: Given the declaration
>
> ```csharp
> struct Point
> {
>     public int x, y;
>
>     public Point(int x, int y) 
>     {
>         this.x = x;
>         this.y = y;
>     }
> }
> ```
>
> the code fragment
>
> ```csharp
> Point a = new Point(10, 10);
> Point b = a;
> a.x = 100;
> System.Console.WriteLine(b.x);
> ```
>
> outputs the value `10`. The assignment of `a` to `b` creates a copy of the value, and `b` is thus unaffected by the assignment to `a.x`. Had `Point` instead been declared as a class, the output would be `100` because `a` and `b` would reference the same object.
>
> *end example*

### 15.4.3 Inheritance

All struct types implicitly inherit from the class `System.ValueType`, which, in turn, inherits from class `object`. A struct declaration may specify a list of implemented interfaces, but it is not possible for a struct declaration to specify a base class.

Struct types are never abstract and are always implicitly sealed. The `abstract` and `sealed` modifiers are therefore not permitted in a struct declaration.

Since inheritance isn’t supported for structs, the declared accessibility of a struct member cannot be protected or protected internal.

Function members in a struct cannot be abstract or virtual, and the `override` modifier is allowed only to override methods inherited from `System.ValueType`.

### 15.4.4 Assignment

Assignment to a variable of a struct type creates a *copy* of the value being assigned. This differs from assignment to a variable of a class type, which copies the reference but not the object identified by the reference.

Similar to an assignment, when a struct is passed as a value parameter or returned as the result of a function member, a copy of the struct is created. A struct may be passed by reference to a function member using a `ref` or `out` parameter.

When a property or indexer of a struct is the target of an assignment, the instance expression associated with the property or indexer access shall be classified as a variable. If the `instance` expression is classified as a value, a compile-time error occurs. This is described in further detail in [§11.18.2](expressions.md#11182-simple-assignment).

### 15.4.5 Default values

As described in [§9.3](variables.md#93-default-values), several kinds of variables are automatically initialized to their default value when they are created. For variables of class types and other reference types, this default value is `null`. However, since structs are value types that cannot be `null`, the default value of a struct is the value produced by setting all value type fields to their default value and all reference type fields to `null`.

> *Example*: Referring to the `Point` struct declared above, the example
>
> ```csharp
> Point[] a = new Point[100];
> ```
>
> initializes each `Point` in the array to the value produced by setting the `x` and `y` fields to zero.
>
> *end example*

The default value of a struct corresponds to the value returned by the default constructor of the struct ([§8.3.3](types.md#833-default-constructors)). Unlike a class, a struct is not permitted to declare a parameterless instance constructor. Instead, every struct implicitly has a parameterless instance constructor, which always returns the value that results from setting all fields to their default values.

> *Note*: Structs should be designed to consider the default initialization state a valid state. In the example
>
> ```csharp
> using System;
> struct KeyValuePair
> {
>     string key;
>     string value;
>
>     public KeyValuePair(string key, string value)
>     {
>         if (key == null || value == null)
>         {
>             throw new ArgumentException();
>         }
>
>         this.key = key;
>         this.value = value;
>     }
> }
> ```
>
> the user-defined instance constructor protects against `null` values only where it is explicitly called. In cases where a `KeyValuePair` variable is subject to default value initialization, the `key` and `value` fields will be `null`, and the struct should be prepared to handle this state.
>
> *end note*

### 15.4.6 Boxing and unboxing

A value of a class type can be converted to type `object` or to an interface type that is implemented by the class simply by treating the reference as another type at compile-time. Likewise, a value of type `object` or a value of an interface type can be converted back to a class type without changing the reference (but, of course, a run-time type check is required in this case).

Since structs are not reference types, these operations are implemented differently for struct types. When a value of a struct type is converted to certain reference types (as defined in [§10.2.9](conversions.md#1029-boxing-conversions)), a boxing operation takes place. Likewise, when a value of certain reference types (as defined in [§10.3.6](conversions.md#1036-unboxing-conversions)) is converted back to a struct type, an unboxing operation takes place. A key difference from the same operations on class types is that boxing and unboxing *copies* the struct value either into or out of the boxed instance.

> *Note*: Thus, following a boxing or unboxing operation, changes made to the unboxed `struct` are not reflected in the boxed `struct`. *end note*

For further details on boxing and unboxing, see [§10.2.9](conversions.md#1029-boxing-conversions) and [§10.3.6](conversions.md#1036-unboxing-conversions).

### 15.4.7 Meaning of this

The meaning of `this` in a struct differs from the meaning of `this` in a class, as described in [§11.7.12](expressions.md#11712-this-access). When a struct type overrides a virtual method inherited from `System.ValueType` (such as `Equals`, `GetHashCode`, or `ToString`), invocation of the virtual method through an instance of the struct type does not cause boxing to occur. This is true even when the struct is used as a type parameter and the invocation occurs through an instance of the type parameter type.

> *Example*:
>
> ```csharp
> using System;
> struct Counter
> {
>     int value;
>     public override string ToString() 
>     {
>         value++;
>         return value.ToString();
>     }
> }
>
> class Program
> {
>     static void Test<T>() where T : new()
>     {
>         T x = new T();
>         Console.WriteLine(x.ToString());
>         Console.WriteLine(x.ToString());
>         console.WriteLine(x.ToString());
>     }
>
>     static void Main() => Test<Counter>();
> }
> ```
>
> The output of the program is:
>
> ```console
> 1
> 2
> 3
> ```
>
> Although it is bad style for `ToString` to have side effects, the example demonstrates that no boxing occurred for the three invocations of `x.ToString()`.
>
> *end example*

Similarly, boxing never implicitly occurs when accessing a member on a constrained type parameter when the member is implemented within the value type. For example, suppose an interface `ICounter` contains a method `Increment`, which can be used to modify a value. If `ICounter` is used as a constraint, the implementation of the `Increment` method is called with a reference to the variable that `Increment` was called on, never a boxed copy.

> *Example*:
>
> ```csharp
> using System;
>
> interface ICounter
> {
>     void Increment();
> }
>
> struct Counter : ICounter
> {
>     int value;
>
>     public override string ToString() => value.ToString();
>
>     void ICounter.Increment() => value++;
> }
>
> class Program
> {
>     static void Test<T>() where T : ICounter, new()
>     {
>         T x = new T();
>         Console.WriteLine(x);
>         x.Increment();              // Modify x
>         Console.WriteLine(x);
>         ((ICounter)x).Increment();  // Modify boxed copy of x
>         Console.WriteLine(x);
>     }
>
>     static void Main() => Test<Counter>();
> }
> ```
>
> The first call to `Increment` modifies the value in the variable `x`. This is not equivalent to the second call to `Increment`, which modifies the value in a boxed copy of `x`. Thus, the output of the program is:
>
> ```console
> 0
> 1
> 1
> ```
>
> *end example*

### 15.4.8 Field initializers

As described in [§15.4.5](structs.md#1545-default-values), the default value of a struct consists of the value that results from setting all value type fields to their default value and all reference type fields to `null`. For this reason, a struct does not permit instance field declarations to include variable initializers. This restriction applies only to instance fields. Static fields of a struct are permitted to include variable initializers.

> *Example*: The following
>
> ```csharp
> struct Point
> {
>     public int x = 1; // Error, initializer not permitted
>     public int y = 1; // Error, initializer not permitted
> }
> ```
>
> is in error because the instance field declarations include variable initializers.
>
> *end example*

### 15.4.9 Constructors

Unlike a class, a struct is not permitted to declare a parameterless instance constructor. Instead, every struct implicitly has a parameterless instance constructor, which always returns the value that results from setting all value type fields to their default value and all reference type fields to `null` ([§8.3.3](types.md#833-default-constructors)). A struct can declare instance constructors having parameters.

> *Example*:
>
> ```csharp
> struct Point
> {
>     int x, y;
>
>     public Point(int x, int y) 
>     {
>         this.x = x;
>         this.y = y;
>     }
> }
> ```
>
> Given the above declaration, the statements
>
> ```csharp
> Point p1 = new Point();
> Point p2 = new Point(0, 0);
> ```
>
> both create a `Point` with `x` and `y` initialized to zero.
>
> *end example*

A struct instance constructor is not permitted to include a constructor initializer of the form `base(`*argument_list*`)`, where *argument_list* is optional.

The `this` parameter of a struct instance constructor corresponds to an `out` parameter of the struct type. As such, `this` shall be definitely assigned ([§9.4](variables.md#94-definite-assignment)) at every location where the constructor returns. Similarly, it cannot be read (even implicitly) in the constructor body before being definitely assigned.

If the struct instance constructor specifies a constructor initializer, that initializer is considered a definite assignment to this that occurs prior to the body of the constructor. Therefore, the body itself has no initialization requirements.

> *Example*: Consider the instance constructor implementation below:
>
> ```csharp
> struct Point
> {
>     int x, y;
>
>     public int X
>     {
>         set { x = value; }
>     }
>
>     public int Y 
>     {
>         set { y = value; }
>     }
>
>     public Point(int x, int y) 
>     {
>         X = x; // error, this is not yet definitely assigned
>         Y = y; // error, this is not yet definitely assigned
>     }
> }
> ```
>
> No instance function member (including the set accessors for the properties `X` and `Y`) can be called until all fields of the struct being constructed have been definitely assigned. Note, however, that if `Point` were a class instead of a struct, the instance constructor implementation would be permitted.
> There is one exception to this, and that involves automatically implemented properties ([§14.7.4](classes.md#1474-automatically-implemented-properties)). The definite assignment rules ([§11.18.2](expressions.md#11182-simple-assignment)) specifically exempt assignment to an auto-property of a struct type within an instance constructor of that struct type: such an assignment is considered a definite assignment of the hidden backing field of the auto-property. Thus, the following is allowed:
>
> ```csharp
> struct Point
> {
>     public int X { get; set; }
>     public int Y { get; set; }
>
>     public Point(int x, int y)
>     {
>         X = x; // allowed, definitely assigns backing field
>         Y = y; // allowed, definitely assigns backing field
>    }
> }
> ```
>
> *end example*]

### 15.4.10 Static constructors

Static constructors for structs follow most of the same rules as for classes. The execution of a static constructor for a struct type is triggered by the first of the following events to occur within an application domain:

- A static member of the struct type is referenced.
- An explicitly declared constructor of the struct type is called.

> *Note*: The creation of default values ([§15.4.5](structs.md#1545-default-values)) of struct types does not trigger the static constructor. (An example of this is the initial value of elements in an array.) *end note*

### 15.4.11 Automatically implemented properties

Automatically implemented properties ([§14.7.4](classes.md#1474-automatically-implemented-properties)) use hidden backing fields, which are only accessible to the property accessors.

> *Note*: This access restriction means that constructors in structs containing automatically implemented properties often need an explicit constructor initializer where they would not otherwise need one, to satisfy the requirement of all fields being definitely assigned before any function member is invoked or the constructor returns. *end note*
