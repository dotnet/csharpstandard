# 16 Structs

## 16.1 General

Structs are similar to classes in that they represent data structures that can contain data members and function members. However, unlike classes, structs are value types and do not require heap allocation. A variable of a `struct` type directly contains the data of the `struct`, whereas a variable of a class type contains a reference to the data, the latter known as an object.

> *Note*: Structs are particularly useful for small data structures that have value semantics. Complex numbers, points in a coordinate system, or key-value pairs in a dictionary are all good examples of structs. Key to these data structures is that they have few data members, that they do not require use of inheritance or reference semantics, rather they can be conveniently implemented using value semantics where assignment copies the value instead of the reference. *end note*

As described in [§8.3.5](types.md#835-simple-types), the simple types provided by C#, such as `int`, `double`, and `bool`, are, in fact, all struct types.

## 16.2 Struct declarations

### 16.2.1 General

A *struct_declaration* is a *type_declaration* ([§14.8](namespaces.md#148-type-declarations)) that declares a new struct:

```ANTLR
struct_declaration
    : non_record_struct_declaration
    | record_struct_declaration
    ;

non_record_struct_declaration
    : attributes? struct_modifier* 'ref'? 'partial'? 'struct'
      identifier type_parameter_list? struct_interfaces?
      type_parameter_constraints_clause* struct_body ';'?
    ;

record_struct_declaration
    : attributes? struct_modifier* 'partial'? 'record' 'struct'
      identifier type_parameter_list? delimited_parameter_list? struct_interfaces?
      type_parameter_constraints_clause* record_struct_body
    ;

record_struct_body
    : struct_body ';'?
    | ';'
    ;
```

A *struct_declaration* is for either a ***non-record struct*** or a ***record struct***.

A *non_record_struct_declaration* consists of an optional set of *attributes* ([§23](attributes.md#23-attributes)), followed by an optional set of *struct_modifier*s ([§16.2.2](structs.md#1622-struct-modifiers)), followed by an optional `ref` modifier ([§16.2.3](structs.md#1623-ref-modifier)), followed by an optional partial modifier ([§15.2.7](classes.md#1527-partial-type-declarations)), followed by the keyword `struct` and an *identifier* that names the struct, followed by an optional *type_parameter_list* specification ([§15.2.3](classes.md#1523-type-parameters)), followed by an optional *struct_interfaces* specification ([§16.2.5](structs.md#1625-struct-interfaces)), followed by an optional *type_parameter_constraints-clauses* specification ([§15.2.5](classes.md#1525-type-parameter-constraints)), followed by a *struct_body* ([§16.2.6](structs.md#1626-struct-body)), optionally followed by a semicolon.

A *record_struct_declaration* consists of an optional set of *attributes* ([§23](attributes.md#23-attributes)), followed by an optional set of *struct_modifier*s ([§16.2.2](structs.md#1622-struct-modifiers)), followed by an optional partial modifier ([§15.2.7](classes.md#1527-partial-type-declarations)), followed by the keyword `record`, followed by the keyword `struct` and an *identifier* that names the struct, followed by an optional *type_parameter_list* specification ([§15.2.3](classes.md#1523-type-parameters)), followed by an optional *delimited_parameter_list* specification ([§15.2.1](classes.md#1521-general)), followed by an optional *struct_interfaces* specification ([§16.2.5](structs.md#1625-struct-interfaces)), followed by an optional *type_parameter_constraints-clauses* specification ([§15.2.5](classes.md#1525-type-parameter-constraints)), followed by a *record_struct_body*.

A *struct_declaration* shall not supply *type_parameter_constraints_clause*s unless it also supplies a *type_parameter_list*.

A *struct_declaration* that supplies a *type_parameter_list* is a generic struct declaration. Additionally, any struct nested inside a generic class declaration or a generic struct declaration is itself a generic struct declaration, since type arguments for the containing type shall be supplied to create a constructed type ([§8.4](types.md#84-constructed-types)).

A *non_record_struct_declaration* that includes a `ref` modifier shall not have a *struct_interfaces* part.

A *record_struct_declaration* having a *delimited_parameter_list* declares a ***positional record struct***.

At most only one *record_struct_declaration* containing `partial` may provide a *delimited_parameter_list*.

The parameters in *delimited_parameter_list* shall not have `ref`, `out` or `this` modifiers; however, `in` and `params` modifiers are permitted.
For a *record_struct_declaration*, the *record_struct_body*s `{}`, `{};`, and `;` are equivalent. They all indicate that the only members are those synthesized by the compiler ([§16.4](structs.md#164-synthesized-record-struct-members)).

### 16.2.2 Struct modifiers

A *struct_declaration* may optionally include a sequence of *struct_modifier*s:

```ANTLR
struct_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | 'readonly'
    | unsafe_modifier   // unsafe code support
    ;
```

*unsafe_modifier* ([§24.2](unsafe-code.md#242-unsafe-contexts)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).

It is a compile-time error for the same modifier to appear multiple times in a struct declaration.

Except for `readonly`, the modifiers of a struct declaration have the same meaning as those of a class declaration ([§15.2.2](classes.md#1522-class-modifiers)).

The `readonly` modifier indicates that the *struct_declaration* declares a type whose instances are immutable.

A readonly struct has the following constraints:

- Each of its instance fields shall also be declared `readonly`.
- It shall not declare any field-like events ([§15.8.2](classes.md#1582-field-like-events)).

When an instance of a readonly struct is passed to a method, its `this` is treated like an input argument/parameter, which disallows write access to any instance fields (except by constructors).

### 16.2.3 Ref modifier

The `ref` modifier indicates that the *non_record_struct_declaration* declares a type whose instances are allocated on the execution stack. These types are called ***ref struct*** types. The `ref` modifier declares that instances may contain ref-like fields, and shall not be copied out of its safe-context ([§16.5.15](structs.md#16515-safe-context-constraint)). The rules for determining the safe context of a ref struct are described in [§16.5.15](structs.md#16515-safe-context-constraint).

It is a compile-time error if a ref struct type is used in any of the following contexts:

- As the element type of an array.
- As the declared type of a field of a class or a struct that does not have the `ref` modifier.
- As a type argument.
- As the type of a tuple element.
- In an async method.
- In an iterator.
- As the receiver type for a method group conversion from an instance method to a delegate type.
- As a captured variable in a lambda expression or a local function.

In addition, the following restrictions apply to a `ref struct` type:

- A `ref struct` type shall not be boxed to `System.ValueType` or `System.Object`.
- A `ref struct` type shall not be declared to implement any interface.
- An instance method declared in `object` or in `System.ValueType` but not overridden in a `ref struct` type shall not be called with a receiver of that `ref struct` type.

> *Note*: A `ref struct` shall not declare `async` instance methods nor use a `yield return` or `yield break` statement within an instance method, because the implicit `this` parameter cannot be used in those contexts. *end note*

These constraints ensure that a variable of `ref struct` type does not refer to stack memory that is no longer valid, or to variables that are no longer valid.

### 16.2.4 Partial modifier

The `partial` modifier indicates that this *struct_declaration* is a partial type declaration. Multiple partial struct declarations with the same name within an enclosing namespace or type declaration combine to form one struct declaration, following the rules specified in [§15.2.7](classes.md#1527-partial-type-declarations).

### 16.2.5 Struct interfaces

A struct declaration may include a *struct_interfaces* specification, in which case the struct is said to directly implement the given interface types. For a constructed struct type, including a nested type declared within a generic type declaration ([§15.3.9.7](classes.md#15397-nested-types-in-generic-classes)), each implemented interface type is obtained by substituting, for each *type_parameter* in the given interface, the corresponding *type_argument* of the constructed type.

```ANTLR
struct_interfaces
    : ':' interface_type_list
    ;
```

The handling of interfaces on multiple parts of a partial struct declaration ([§15.2.7](classes.md#1527-partial-type-declarations)) are discussed further in [§15.2.4.3](classes.md#15243-interface-implementations).

Interface implementations are discussed further in [§19.6](interfaces.md#196-interface-implementations).

### 16.2.6 Struct body

The *struct_body* of a struct defines the members of the struct.

```ANTLR
struct_body
    : '{' struct_member_declaration* '}'
    ;
```

## 16.3 Struct members

### 16.3.1 General

The members of a struct consist of the members introduced by its *struct_member_declaration*s and the members inherited from the type `System.ValueType`. For a record struct, the member set also includes the synthesized members generated by the compiler ([§16.4](structs.md#164-synthesized-record-struct-members)).

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

*fixed_size_buffer_declaration* ([§24.8.2](unsafe-code.md#2482-fixed-size-buffer-declarations)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).

> *Note*: All kinds of *class_member_declaration*s except *finalizer_declaration* are also *struct_member_declaration*s. *end note*

Except for the differences noted in [§16.5](structs.md#165-class-and-struct-differences), the descriptions of class members provided in [§15.3](classes.md#153-class-members) through [§15.12](classes.md#1512-static-constructors) apply to struct members as well.

It is an error for an instance field of a record struct to have an unsafe type.

### 16.3.2 Readonly members

An instance member definition or accessor of an instance property, indexer, or event that includes the `readonly` modifier has the following restrictions:

- The `this` parameter is a `ref readonly` reference.
- The member shall not reassign the value of `this` or an instance field of the receiver.
- The member shall not reassign the value of an instance field-like event ([§15.8.2](classes.md#1582-field-like-events)) of the receiver.
- If a readonly member invokes a non-readonly member, the structure referred to by `this` must be copied to use a writable reference for the `this` argument.

> *Note:* Instance fields include the hidden backing field used for automatically implemented properties ([§15.7.4](classes.md#1574-automatically-implemented-properties)). *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: A readonly member can modify the state of an object referred to by an instance field, even though the readonly member can’t reassign that instance member. The following code demonstrates the reassigning and modifying an instance field:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ReadonlyMember" } -->
> ```csharp
> public struct S
> {
>     private List<string> messages;
>
>     public S(IEnumerable<string> messages) =>
>         this.messages = new List<string>(messages);
>
>     public void InitializeMessages() =>
>         messages = new List<string>();
>
>     public readonly void AddMessage(string message)
>     {
>         if (messages == null)
>         {
>             throw new InvalidOperationException("Messages collection is not initialized.");
>         }
>         messages.Add(message);
>     }
> }
> ```
>
> The `readonly` method `AddMessage` can change the state of a message list. The `InitializeMessages` member can clear and re-initialize the list of messages. In the case of `AddMessage`, the `readonly` modifier is valid. In the case of `InitializeMessages`, adding the `readonly` modifier is invalid. *end example*

## 16.4 Synthesized record struct members

### 16.4.1 General

In the case of a record struct, members are synthesized unless a member with a “matching” signature is declared in the *record_struct_body* or an accessible concrete non-virtual member with a “matching” signature is inherited. Two members are considered matching if they have the same signature or would be considered “hiding” in an inheritance scenario. (See Signatures and overloading [§7.6](basic-concepts.md#76-signatures-and-overloading).)

The synthesized members are described in the following subclauses.

### 16.4.2 Equality members

The synthesized equality members are similar to those for a record class ([§15.16.2](classes.md#15162-equality-members)), except for the lack of `EqualityContract`, null checks, or inheritance.

A record struct `R` implements `System.IEquatable<R>` and includes a synthesized strongly-typed overload of `Equals(R other)`, which is public, as follows:

```csharp
public readonly bool Equals(R other);
```

This method can be declared explicitly. However, it is an error if the explicit declaration does not match the expected signature or accessibility.

If `Equals(R other)` is user-defined (that is, not synthesized) but `GetHashCode` is not, a warning shall be produced.

The synthesized `Equals(R)` shall return `true` if and only if for each instance field `fieldN` in the record struct the value of `System.Collections.Generic.EqualityComparer<TN>.Default.Equals(fieldN, other.fieldN)`,  where `TN` is the field type, is `true`.

The record struct includes synthesized `==` and `!=` operators equivalent to operators declared as follows:

```csharp
public static bool operator==(R r1, R r2) => r1.Equals(r2);
public static bool operator!=(R r1, R r2) => !(r1 == r2);
```

The `Equals` method called by the `==` operator is the `Equals(R other)` method specified above. The `!=` operator delegates to the `==` operator. It is an error if the operators are declared explicitly.

The record struct includes a synthesized override equivalent to a method declared as follows:

```csharp
public override readonly bool Equals(object? obj);
```

It is an error if the override is declared explicitly. The synthesized override shall return `other is R temp && Equals(temp)` where `R` is the record struct.

The record struct includes a synthesized override equivalent to a method declared as follows:

```csharp
public override readonly int GetHashCode();
```

This method may be declared explicitly.

A warning shall be reported if one of `Equals(R)` and `GetHashCode()` is explicitly declared but the other method is not.

The synthesized override of `GetHashCode()` shall return an `int` result of combining the values of `System.Collections.Generic.EqualityComparer<TN>.Default.GetHashCode(fieldN)` for each instance field `fieldN` with `TN` being the type of `fieldN`.

> *Example*: Consider the following record struct:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"RecordStructEqualityMembers1", additionalFiles:["T1T2T3.cs"]} -->
> ```csharp
> record struct R1(T1 P1, T2 P2);
> ```
>
> For this, the synthesized equality members would be something like:
>
> <!-- Example: {template:"standalone-lib", name:"RecordStructEqualityMembers2", additionalFiles:["T1T2T3.cs"]} -->
> ```csharp
> struct R1 : IEquatable<R1>
> {
>     public T1 P1 { get; set; }
>     public T2 P2 { get; set; }
>     public override bool Equals(object? obj) => obj is R1 temp && Equals(temp);
>     public bool Equals(R1 other)
>     {
>         return
>             EqualityComparer<T1>.Default.Equals(P1, other.P1) &&
>             EqualityComparer<T2>.Default.Equals(P2, other.P2);
>     }
>     public static bool operator==(R1 r1, R1 r2) => r1.Equals(r2);
>     public static bool operator!=(R1 r1, R1 r2) => !(r1 == r2);    
>     public override int GetHashCode()
>     {
>         return HashCode.Combine(
>             EqualityComparer<T1>.Default.GetHashCode(P1),
>             EqualityComparer<T2>.Default.GetHashCode(P2));
> ```
>
> *end example*

### 16.4.3 Printing members

A record struct includes a synthesized method equivalent to the following:

```csharp
private bool PrintMembers(System.Text.StringBuilder builder);
```

This method performs the following tasks:

1. For each of the record struct’s printable members (non-static public field and readable property members), appends that member’s name followed by “` = `“ followed by the member’s value separated with “`, “`,
2. Returns true if the record struct has printable members.

For a member that has a value type, its value shall be converted to a string representation.

If the record’s printable members do not include a readable property with a non-`readonly` `get` accessor, then the synthesized `PrintMembers` is `readonly`. There is no requirement for the record’s fields to be `readonly` for the `PrintMembers` method to be `readonly`.

The `PrintMembers` method can be declared explicitly. However, it is an error if the explicit declaration does not match the expected signature or accessibility.

The record struct includes a synthesized method equivalent to the following:

```csharp
public override string ToString();
```

If the record struct’s `PrintMembers` method is `readonly`, then the synthesized `ToString()` method shall be `readonly`.

This method can be declared explicitly. It is an error if the explicit declaration does not match the expected signature or accessibility.

This method performs the following tasks:

1. Creates a `StringBuilder` instance,
2. Appends the record struct name to the builder, followed by “` { `“,
3. Invokes the record struct’s `PrintMembers` method giving it the builder, followed by “` `” if it returned true,
4. Appends “`}`”,
5. Returns the builder’s contents with `builder.ToString()`.

> *Example*: Consider the following record struct:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"RecordStructPrintingMembers1", additionalFiles:["T1T2T3.cs"]} -->
> ```csharp
> record struct R1(T1 P1, T2 P2);
> ```
>
> For this record struct, the synthesized printing members would be something like:
>
> <!-- Example: {template:"standalone-lib", name:"RecordStructPrintingMembers2", additionalFiles:["T1T2T3.cs"], expectedErrors:["CS0535"]} -->
> <!-- NOTE: In reality, class R1 will also have a synthesized implementation of interface member 'IEquatable<R1>.Equals(R1?)', but as that is not relevant to this printing-member example, error CS0535 re this omission has been ignored. -->
> ```csharp
> struct R1 : IEquatable<R1>
> {
>     public T1 P1 { get; set; }
>     public T2 P2 { get; set; }
>
>     private bool PrintMembers(StringBuilder builder)
>     {
>         builder.Append(nameof(P1));
>         builder.Append(" = ");
>         builder.Append(this.P1); // or builder.Append(this.P1.ToString());
>                                  // if P1 has a value type
>         builder.Append(", ");
>
>         builder.Append(nameof(P2));
>         builder.Append(" = ");
>         builder.Append(this.P2); // or builder.Append(this.P2.ToString());
>                                  // if P2 has a value type
>
>         return true;
>     }
>
>     public override string ToString()
>     {
>         var builder = new StringBuilder();
>         builder.Append(nameof(R1));
>         builder.Append(" { ");
>
>         if (PrintMembers(builder))
>             builder.Append(" ");
>
>         builder.Append("}");
>         return builder.ToString();
>     }
> }
> ```
>
> *end example*

### 16.4.4 Positional record struct members

#### 16.4.4.1 General

As well as providing the members described in the preceding subclauses, positional record structs ([§16.2.1](structs.md#1621-general)) synthesize additional members with the same conditions as the other members, as described in the following subclauses.

#### 16.4.4.2 Primary constructor

A record struct has a public constructor whose signature corresponds to the value parameters of the type declaration. This is called the primary constructor for the type. It is an error to have a primary constructor and a constructor with the same signature already present in the struct. If the type declaration does not include a *delimited_parameter_list*, no primary constructor is generated.

> <!-- Example: {template:"standalone-lib", name:"RecordStructPrimaryConstructor1", expectedErrors:["CS0111","CS8862"]} -->
> ```csharp
> record struct R1
> {
>     public R1() { } // OK
> }
>
> record struct R2()
> {
>     public R2() { } // error: 'R2' already defines
>                     // a constructor with the same parameter types
> }
> ```

Instance field declarations for a record struct are permitted to include variable initializers. If there is no primary constructor, the instance initializers execute as part of the parameterless constructor. Otherwise, at runtime the primary constructor executes the instance initializers appearing in the record-struct-body.

If a record struct has a primary constructor, any user-defined constructor shall have an explicit `this` constructor initializer that calls the primary constructor or an explicitly declared constructor.

Parameters of the primary constructor as well as members of the record struct are in scope within initializers of instance fields or properties. Instance members would be an error in these locations, but the parameters of the primary constructor would be in scope and useable and would shadow members. Static members would also be useable.

A warning shall be produced if a parameter of the primary constructor is not read.

The definite assignment rules for struct instance constructors ([§16.5.9](structs.md#1659-constructors), [§12.8.14](expressions.md#12814-this-access)) apply to the primary constructor of record structs. As for any other struct instance constructor without a `this()` initializer, any instance field that is not definitely assigned by the primary constructor is implicitly initialized to its default value in the initialization phase that runs before the body of the primary constructor.

#### 16.4.4.3 Properties

For each parameter of a *delimited_parameter_list* that has the same name and type as an explicitly declared instance field, the remainder of this subclause does not apply.

For each record struct parameter of a *delimited_parameter_list* there is a corresponding public property member whose name and type are taken from the value parameter declaration.

For a record struct:

- a public `get` and `init` auto-property is created if the record struct has a `readonly` modifier, `get` and `set` otherwise. Both kinds of set accessors (`set` and `init`) are considered “matching.” So, the user may declare an init-only property in place of a synthesized mutable one.

- An inherited `abstract` property with matching type is overridden.

- No auto-property is created if the record struct has an instance field with expected name and type.

- It is an error if the inherited property does not have `public` `get` and `set`/`init` accessors.

- It is an error if the inherited property or field is hidden.

- The auto-property is initialized to the value of the corresponding primary constructor parameter.

- Attributes may be applied to the synthesized auto-property and its backing field by using `property:` or `field:` targets for attributes syntactically applied to the corresponding record struct parameter.

#### 16.4.4.4 Deconstruct

A positional record struct with at least one parameter synthesizes a public `void`-returning instance method called `Deconstruct` with an out parameter declaration for each parameter of the primary constructor declaration. Each parameter of `Deconstruct` has the same type as the corresponding parameter of the primary
constructor declaration. The body of the method assigns each parameter of the Deconstruct method to the value from an instance member access to a member of the same name.
If the instance members accessed in the body do not include a property with a non-`readonly` `get` accessor, then the synthesized `Deconstruct` method is `readonly`.
The method can be declared explicitly. It is an error if the explicit declaration does not match the expected signature or accessibility, or is static.

## 16.5 Class and struct differences

### 16.5.1 General

Structs differ from classes in several important ways:

- Structs are value types ([§16.5.2](structs.md#1652-value-semantics)).
- All struct types implicitly inherit from the class `System.ValueType` ([§16.5.3](structs.md#1653-inheritance)).
- Assignment to a variable of a struct type creates a *copy* of the value being assigned ([§16.5.4](structs.md#1654-assignment)).
- The default value of a struct is the value produced by setting all fields to their default value ([§16.5.5](structs.md#1655-default-values)).
- Boxing and unboxing operations are used to convert between a struct type and certain reference types ([§16.5.6](structs.md#1656-boxing-and-unboxing)).
- The meaning of `this` is different within struct members ([§16.5.7](structs.md#1657-meaning-of-this)).
- A struct is not permitted to declare a finalizer.
- Event declarations, property declarations, property accessors, indexer declarations, and method declarations are permitted to have the modifier `readonly` while that is not generally permitted for those same member kinds in classes.

### 16.5.2 Value semantics

Structs are value types ([§8.3](types.md#83-value-types)) and are said to have value semantics. Classes, on the other hand, are reference types ([§8.2](types.md#82-reference-types)) and are said to have reference semantics.

A variable of a struct type directly contains the data of the struct, whereas a variable of a class type contains a reference to an object that contains the data. When a struct `B` contains an instance field of type `A` and `A` is a struct type, it is a compile-time error for `A` to depend on `B` or a type constructed from `B`. A struct `X` *directly depends on* a struct `Y` if `X` contains an instance field of type `Y`. Given this definition, the complete set of structs upon which a struct depends is the transitive closure of the *directly depends on* relationship.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ValueSemantics1", expectedErrors:["CS0523"], ignoredWarnings:["CS0169"]} -->
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
> <!-- Example: {template:"standalone-lib-without-using", name:"ValueSemantics2", expectedErrors:["CS0523","CS0523","CS0523"], ignoredWarnings:["CS0169"]} -->
> ```csharp
> struct A { B b; }
> struct B { C c; }
> struct C { A a; }
> ```
>
> is an error because each of the types `A`, `B`, and `C` depend on each other.
>
> *end example*

With classes, it is possible for two variables to reference the same object, and thus possible for operations on one variable to affect the object referenced by the other variable. With structs, the variables each have their own copy of the data (except in the case of by-reference parameters), and it is not possible for operations on one to affect the other. Furthermore, except when explicitly nullable ([§8.3.12](types.md#8312-nullable-value-types)), it is not possible for values of a struct type to be `null`.

> *Note*: If a struct contains a field of reference type then the contents of the object referenced can be altered by other operations. However the value of the field itself, i.e., which object it references, cannot be changed through a mutation of a different struct value. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: Given the following
>
> <!-- Example: {template:"standalone-console", name:"ValueSemantics3", expectedOutput:["10"]} -->
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
>
> class A
> {
>     static void Main()
>     {
>         Point a = new Point(10, 10);
>         Point b = a;
>         a.x = 100;
>         Console.WriteLine(b.x);
>     }
> }
>  ```
>
> the output is `10`. The assignment of `a` to `b` creates a copy of the value, and `b` is thus unaffected by the assignment to `a.x`. Had `Point` instead been declared as a class, the output would be `100` because `a` and `b` would reference the same object.
>
> *end example*

### 16.5.3 Inheritance

All struct types implicitly inherit from the class `System.ValueType`, which, in turn, inherits from class `object`. A struct declaration may specify a list of implemented interfaces, but it is not possible for a struct declaration to specify a base class.

Struct types are never abstract and are always implicitly sealed. The `abstract` and `sealed` modifiers are therefore not permitted in a struct declaration.

Since inheritance is not supported for structs, the declared accessibility of a struct member cannot be `protected`, `private protected`, or `protected internal`.

Function members in a struct cannot be abstract or virtual, and the `override` modifier is allowed only to override methods inherited from `System.ValueType`.

### 16.5.4 Assignment

Assignment to a variable of a struct type creates a *copy* of the value being assigned. This differs from assignment to a variable of a class type, which copies the reference but not the object identified by the reference.

Similar to an assignment, when a struct is passed as a value parameter or returned as the result of a function member, a copy of the struct is created. A struct may be passed by reference to a function member using a by-reference parameter.

When a property or indexer of a struct is the target of an assignment, the instance expression associated with the property or indexer access shall be classified as a variable. If the instance expression is classified as a value, a compile-time error occurs. This is described in further detail in [§12.24.2](expressions.md#12242-simple-assignment).

### 16.5.5 Default values

As described in [§9.3](variables.md#93-default-values), several kinds of variables are automatically initialized to their default value when they are created. For variables of class types and other reference types, this default value is `null`. However, since structs are value types that cannot be `null`, the default value of a struct is the value produced by setting all value type fields to their default value and all reference type fields to `null`.

> *Example*: Referring to the `Point` struct declared above, the example
>
> <!-- Example: {template:"code-in-main-without-using", name:"DefaultValues1", additionalFiles:["Point.cs"]} -->
> ```csharp
> Point[] a = new Point[100];
> ```
>
> initializes each `Point` in the array to the value produced by setting the `x` and `y` fields to zero.
>
> *end example*

The default value of a struct corresponds to the value returned by the default constructor of the struct ([§8.3.3](types.md#833-default-constructors)). When a struct does not declare an explicit parameterless instance constructor, the default constructor is synthesized and always returns the value that results from setting all fields to their default values. The `default` expression always produces the zero-initialized default value, even when a struct declares an explicit parameterless instance constructor ([§16.5.9](structs.md#1659-constructors)).

> *Note*: Structs should be designed to consider the default initialization state a valid state. In the example
>
> <!-- Example: {template:"standalone-lib", name:"DefaultValues2", ignoredWarnings:["CS0649"]} -->
> ```csharp
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

### 16.5.6 Boxing and unboxing

A value of a class type can be converted to type `object` or to an interface type that is implemented by the class simply by treating the reference as another type at compile-time. Likewise, a value of type `object` or a value of an interface type can be converted back to a class type without changing the reference (but, of course, a run-time type check is required in this case).

Since structs are not reference types, these operations are implemented differently for struct types. When a value of a struct type is converted to certain reference types (as defined in [§10.2.9](conversions.md#1029-boxing-conversions)), a boxing operation takes place. Likewise, when a value of certain reference types (as defined in [§10.3.7](conversions.md#1037-unboxing-conversions)) is converted back to a struct type, an unboxing operation takes place. A key difference from the same operations on class types is that boxing and unboxing *copies* the struct value either into or out of the boxed instance.

> *Note*: Thus, following a boxing or unboxing operation, changes made to the unboxed `struct` are not reflected in the boxed `struct`. *end note*

For further details on boxing and unboxing, see [§10.2.9](conversions.md#1029-boxing-conversions) and [§10.3.7](conversions.md#1037-unboxing-conversions).

### 16.5.7 Meaning of this

The meaning of `this` in a struct differs from the meaning of `this` in a class, as described in [§12.8.14](expressions.md#12814-this-access). When a struct type overrides a virtual method inherited from `System.ValueType` (such as `Equals`, `GetHashCode`, or `ToString`), invocation of the virtual method through an instance of the struct type does not cause boxing to occur. This is true even when the struct is used as a type parameter and the invocation occurs through an instance of the type parameter type.

> *Example*:
>
> <!-- Example: {template:"standalone-console", name:"MeaningOfThis1", inferOutput:true} -->
> ```csharp
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
>         Console.WriteLine(x.ToString());
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
> <!-- Example: {template:"standalone-console", name:"MeaningOfThis2", inferOutput:true} -->
> ```csharp
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

### 16.5.8 Field initializers

As described in [§16.5.5](structs.md#1655-default-values), the default value of a struct consists of the value that results from setting all value type fields to their default value and all reference type fields to `null`. Static and instance fields of a struct are permitted to include variable initializers; however, in the case of an instance field initializer, at least one instance constructor shall also be declared, or for a record struct, a *delimited_parameter_list* shall be present.

> *Example*:
>
> <!-- Example: {template:"standalone-console", name:"FieldInitializers", inferOutput:true} -->
> ```csharp
> Console.WriteLine($"Point is {new Point()}");
>
> struct Point
> {
>     public int x = 1;
>     public int y = 1;
>
>     public Point() { }
>
>     public override string ToString()
>     {
>         return "(" + x + ", " + y + ")";
>     }
> }
> ```
>
> ```console
> Point is (1, 1)
> ```
>
> *end example*

When a struct instance constructor has no constructor initializer, that constructor implicitly performs the initializations specified by the *variable_initializer*s of the instance fields declared in its struct. This corresponds to a sequence of assignments that are executed immediately upon entry to the constructor.

When a struct instance constructor has a `this()` constructor initializer that represents the default parameterless constructor, the declared constructor implicitly clears all instance fields and performs the initializations specified by the *variable_initializer*s of the instance fields declared in its struct. Immediately upon entry to the constructor, all value type fields are set to their default value and all reference type fields are set to `null`. Immediately after that, a sequence of assignments corresponding to the *variable_initializer*s are executed.

A *field_declaration* declared directly inside a *struct_declaration* having the *struct_modifier* `readonly` shall have the *field_modifier* `readonly`.

### 16.5.9 Constructors

A struct can declare instance constructors, with zero or more parameters. If a struct has no explicitly declared parameterless instance constructor, one is synthesized, with public accessibility, which always returns the value that results from setting all value type fields to their default value and all reference type fields to `null` ([§8.3.3](types.md#833-default-constructors)). In such a case, any instance field initializers are ignored when that constructor executes.

An explicitly declared parameterless instance constructor shall have public accessibility.

> *Example*: Given the following:
>
> <!-- Example: {template:"standalone-console", name:"Constructors1", ignoredWarnings:["CS0219"], inferOutput:true} -->
> ```csharp
> using System;
> struct Point
> {
>     int x = -1, y = -2;
>
>     public Point(int x, int y) 
>     {
>         this.x = x;
>         this.y = y;
>     }
>
>     public override string ToString()
>     {
>         return "(" + x + ", " + y + ")";
>     }
> }
>
> class A
> {
>     static void Main()
>     {
>         Console.WriteLine($"Point is {new Point()}");
>         Console.WriteLine($"Point is {new Point(0,0)}");
>     }
> }
> ```
>
> ```console
> Point is (0, 0)
> Point is (0, 0)
> ```
>
> the statements both create a `Point` with `x` and `y` initialized to zero, which in the case of the call to the parameterless instance constructor, may be surprising, as both instance fields have initializers, but they are *not* executed.
>
> *end example*

A struct instance constructor is not permitted to include a constructor initializer of the form `base(`*argument_list*`)`, where *argument_list* is optional. The execution of an instance constructor shall not result in the execution of a constructor in the struct’s base type `System.ValueType`.

The `this` parameter of a struct instance constructor behaves similarly to an output parameter of the struct type, except that when the definite assignment requirements ([§9.4.1](variables.md#941-general)) for `this` (or for instance variables within `this`) are not met at a location where they would otherwise be required, that does not result in a compile-time error. Instead, the unassigned variables are implicitly initialized to the default value ([§9.3](variables.md#93-default-values)) in an *initialization* phase before any other code in the constructor runs, as described in [§12.8.14](expressions.md#12814-this-access).

If the struct instance constructor specifies a constructor initializer, that initializer is considered a definite assignment to `this` that occurs prior to the body of the constructor. Therefore, the body itself has no initialization requirements.

For a struct instance constructor that does not have a `this()` initializer, any instance field (other than a `fixed` field) that is not definitely assigned at every location where the constructor returns, or that has not yet been definitely assigned at a location where the value of `this` or of that field is read, is implicitly initialized to its default value in the *initialization* phase described in [§12.8.14](expressions.md#12814-this-access).

> *Example*: Consider the instance constructor implementation below:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Constructors2"} -->
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
>         X = x; // ok; x is implicitly initialized to its default value
>                // before the body runs, so calling the X setter is allowed
>         Y = y; // ok, for the same reason
>     }
> }
> ```
>
> Because the instance fields `x` and `y` are not definitely assigned before the calls to the property setters, they are implicitly initialized to their default values in the initialization phase before the constructor body runs. The set accessors for `X` and `Y` may therefore be invoked even though the constructor body itself does not first assign `x` and `y`.
>
> Automatically implemented properties ([§15.7.4](classes.md#1574-automatically-implemented-properties)) interact with these rules via the hidden backing field. The definite assignment rules ([§12.24.2](expressions.md#12242-simple-assignment)) specifically treat assignment to an auto-property of a struct type within an instance constructor of that struct type as a definite assignment of the hidden backing field of the auto-property. Thus, the following is also allowed:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Constructors3"} -->
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

### 16.5.10 Static constructors

Static constructors for structs follow most of the same rules as for classes. The execution of a static constructor for a struct type is triggered by the first of the following events to occur within an application domain:

- A static member of the struct type is referenced.
- An explicitly declared constructor of the struct type is called.

> *Note*: The creation of default values ([§16.5.5](structs.md#1655-default-values)) of struct types does not trigger the static constructor. (An example of this is the initial value of elements in an array.) *end note*

### 16.5.11 Properties

A *property_declaration* ([§15.7.1](classes.md#1571-general)) for an instance property in a *struct_declaration* may contain the *property_modifier* `readonly`. However, a static property shall not contain that modifier.

It is a compile-time error to attempt to modify the state of an instance struct variable via a readonly property declared in that struct.

It is a compile-time error for an automatically implemented property having a `readonly` modifier, to also have a `set` accessor.

It is a compile-time error for an automatically implemented property in a `readonly` struct to have a `set` accessor.

An automatically implemented property declared inside a `readonly` struct need not have a `readonly` modifier, as its `get` accessor is implicitly assumed to be readonly.

It is a compile-time error to have a `readonly` modifier on a property itself as well as on either of its `get` and `set` accessors.

It is a compile-time error for a property to have a readonly modifier on all of its accessors.

> *Note*: To correct the error, move the modifier from the accessors to the property itself. *end note*

For a property accessor expression, `s.P`:

- It is a compile-time error if `s.P` invokes the set accessor `M` of type `T` when the process in [§12.6.6.1](expressions.md#12661-general) would create a temporary copy of `s`.
- If `s.P` invokes the get accessor of type `T`, the process in [§12.6.6.1](expressions.md#12661-general) is followed, including creating a temporary copy of `s` if required.

Automatically implemented properties ([§15.7.4](classes.md#1574-automatically-implemented-properties)) use hidden backing fields, which are only accessible to the property accessors.

> *Note*: Because the backing field of an auto-property of a struct type is implicitly initialized to its default value in the initialization phase of an instance constructor that does not assign it ([§12.8.14](expressions.md#12814-this-access)), an explicit constructor initializer is not required in order to satisfy the definite-assignment rules for that backing field. *end note*

### 16.5.12 Methods

A *method_declaration* ([§15.6.1](classes.md#1561-general)) for an instance method in a *struct_declaration* may contain the *method_modifier* `readonly`. However, a static method shall not contain that modifier.

It is a compile-time error to attempt to modify the state of an instance struct variable via a readonly method declared in that struct.

Although a readonly method may call a sibling, non-readonly method, or property or indexer get accessor, doing so results in the creation of an implicit copy of `this` as a defensive measure.

A readonly method may call a sibling property or indexer set accessor that is readonly. If a sibling member’s accessor is not explicitly or implicitly readonly, a compile-error occurs.

All *method_declaration*s of a partial method shall have a `readonly` modifier, or none of them shall have it.

### 16.5.13 Indexers

An *indexer_declaration* ([§15.9](classes.md#159-indexers)) for an instance indexer in a *struct_declaration* may contain the *indexer_modifier* `readonly`.

It is a compile-time error to attempt to modify the state of an instance struct variable via a readonly indexer declared in that struct.

It is a compile-time error to have a `readonly` modifier on an indexer itself as well as on either of its `get` or `set` accessors.

It is a compile-time error for an indexer to have a readonly modifier on all of its accessors.

> *Note*: To correct the error, move the modifier from the accessors to the indexer itself. *end note*

### 16.5.14 Events

An *event_declaration* ([§15.8.1](classes.md#1581-general)) for an instance, non-field-like event in a *struct_declaration* may contain the *event_modifier* `readonly`. However, a static event shall not contain that modifier.

### 16.5.15 Safe context constraint

#### 16.5.15.1 General

At compile-time, each expression is associated with a context where that instance and all its fields can be safely accessed, its ***safe-context***. The safe-context is a context, enclosing an expression, which it is safe for the value to escape to.

Any expression whose compile-time type is not a ref struct has a safe-context of caller-context.

A `default` expression, for any type, has safe-context of caller-context.

For any non-default expression whose compile-time type is a ref struct has a safe-context defined by the following sections.

The safe-context records which context a value may be copied into. Given an assignment from an expression `E1` with a safe-context `S1`, to an expression `E2` with safe-context `S2`, it is an error if `S2` is a wider context than `S1`.

There are three different safe-context values, the same as the ref-safe-context values defined for reference variables ([§9.7.2](variables.md#972-ref-safe-contexts)): **declaration-block**, **function-member**, and **caller-context**. The safe-context of an expression constrains its use as follows:

- For a return statement `return e1`, the safe-context of `e1` shall be caller-context.
- For an assignment `e1 = e2` the safe-context of `e2` shall be at least as wide a context as the safe-context of `e1`.

For a method invocation if there is a `ref` or `out` argument of a `ref struct` type (including the receiver unless the type is `readonly`), with safe-context `S1`, then no argument (including the receiver) may have a narrower safe-context than `S1`.

#### 16.5.15.2 Parameter safe context

A parameter of a ref struct type, including the `this` parameter of an instance method, has a safe-context of caller-context.

#### 16.5.15.3 Local variable safe context

A local variable of a ref struct type has a safe-context as follows:

- If the variable is an iteration variable of a `foreach` loop, then the variable’s safe-context is the same as the safe-context of the `foreach` loop’s expression.
- Otherwise if the variable’s declaration has an initializer then the variable’s safe-context is the same as the safe-context of that initializer.
- Otherwise the variable is uninitialized at the point of declaration and has a safe-context of caller-context.

#### 16.5.15.4 Field safe context

A reference to a field `e.F`, where the type of `F` is a ref struct type, has a safe-context that is the same as the safe-context of `e`.

#### 16.5.15.5 Operators

The application of a user-defined operator is treated as a method invocation ([§16.5.15.6](structs.md#165156-method-and-property-invocation)).

For an operator that yields a value, such as `e1 + e2` or `c ? e1 : e2`, the safe-context of the result is the narrowest context among the safe-contexts of the operands of the operator. As a consequence, for a unary operator that yields a value, such as `+e`, the safe-context of the result is the safe-context of the operand.

> *Note*: The first operand of a conditional operator is a `bool`, so its safe-context is caller-context. It follows that the resulting safe-context is the narrowest safe-context of the second and third operand. *end note*

#### 16.5.15.6 Method and property invocation

A value resulting from a method invocation `e1.M(e2, ...)` or property invocation `e.P` has safe-context of the smallest of the following contexts:

- caller-context.
- The safe-context of all argument expressions (including the receiver).

A property invocation (either `get` or `set`) is treated as a method invocation of the underlying method by the above rules.

#### 16.5.15.7 stackalloc

The result of a stackalloc expression has safe-context of function-member.

#### 16.5.15.8 Constructor invocations

A `new` expression that invokes a constructor obeys the same rules as a method invocation that is considered to return the type being constructed.

In addition the safe-context is the smallest of the safe-contexts of all arguments and operands of all object initializer expressions, recursively, if any initializer is present.

> *Note*: These rules rely on `Span<T>` not having a constructor of the following form:
>
> ```csharp
> public Span<T>(ref T p)
> ```
>
> Such a constructor makes instances of `Span<T>` used as fields indistinguishable from a `ref` field. The safety rules described in this document depend on `ref` fields not being a valid construct in C# or .NET. *end note*
