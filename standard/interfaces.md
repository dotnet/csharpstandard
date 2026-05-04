# 19 Interfaces

## 19.1 General

An interface defines a contract. A class or struct that implements an interface shall adhere to its contract. An interface may inherit from multiple base interfaces, and a class or struct may implement multiple interfaces.

Interfaces may contain various kinds of members, as described in [§19.4](interfaces.md#194-interface-members). The interface itself may provide an implementation for some or all of the function members that it declares. Members for which the interface does not provide an implementation are abstract. Their implementations must be supplied by classes or structs that implement the interface, and may be supplied by derived interfaces through an explicit implementation ([§19.6.2](interfaces.md#1962-explicit-interface-member-implementations)).

<!-- This note needs to be updated in C# 13, when ref struct types can implement interfaces. -->
> *Note*: Historically, adding a new function member to an interface impacted all existing consumers of that interface type; it was a breaking change. The addition of interface function member implementations allowed developers to upgrade an interface while still enabling any implementors to override that implementation. Users of the interface can accept the implementation as a non-breaking change; however, if their requirements are different, they can override the provided implementations. *end note*

## 19.2 Interface declarations

### 19.2.1 General

An *interface_declaration* is a *type_declaration* ([§14.8](namespaces.md#148-type-declarations)) that declares a new interface type.

```ANTLR
interface_declaration
    : attributes? interface_modifier* 'partial'? 'interface'
      identifier variant_type_parameter_list? interface_base?
      type_parameter_constraints_clause* interface_body ';'?
    ;
```

An *interface_declaration* consists of an optional set of *attributes* ([§23](attributes.md#23-attributes)), followed by an optional set of *interface_modifier*s ([§19.2.2](interfaces.md#1922-interface-modifiers)), followed by an optional partial modifier ([§15.2.7](classes.md#1527-partial-type-declarations)), followed by the keyword `interface` and an *identifier* that names the interface, followed by an optional *variant_type_parameter_list* specification ([§19.2.3](interfaces.md#1923-variant-type-parameter-lists)), followed by an optional *interface_base* specification ([§19.2.4](interfaces.md#1924-base-interfaces)), followed by an optional *type_parameter_constraints_clause*s specification ([§15.2.5](classes.md#1525-type-parameter-constraints)), followed by an *interface_body* ([§19.3](interfaces.md#193-interface-body)), optionally followed by a semicolon.

An interface declaration shall not supply *type_parameter_constraints_clause*s unless it also supplies a *variant_type_parameter_list*.

An interface declaration that supplies a *variant_type_parameter_list* is a generic interface declaration. Additionally, any interface nested inside a generic class declaration or a generic struct declaration is itself a generic interface declaration, since type arguments for the containing type shall be supplied to create a constructed type ([§8.4](types.md#84-constructed-types)).

### 19.2.2 Interface modifiers

An *interface_declaration* may optionally include a sequence of interface modifiers:

```ANTLR
interface_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | unsafe_modifier   // unsafe code support
    ;
```

*unsafe_modifier* ([§24.2](unsafe-code.md#242-unsafe-contexts)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).

It is a compile-time error for the same modifier to appear multiple times in an interface declaration.

The `new` modifier is only permitted on interfaces defined within a class. It specifies that the interface hides an inherited member by the same name, as described in [§15.3.5](classes.md#1535-the-new-modifier).

The `public`, `protected`, `internal`, and `private` modifiers control the accessibility of the interface. Depending on the context in which the interface declaration occurs, only some of these modifiers might be permitted ([§7.5.2](basic-concepts.md#752-declared-accessibility)). When a partial type declaration ([§15.2.7](classes.md#1527-partial-type-declarations)) includes an accessibility specification (via the `public`, `protected`, `internal`, and `private` modifiers), the rules in [§15.2.2](classes.md#1522-class-modifiers) apply.

### 19.2.3 Variant type parameter lists

#### 19.2.3.1 General

Variant type parameter lists can only occur on interface and delegate types. The difference from ordinary *type_parameter_list*s is the optional *variance_annotation* on each type parameter.

```ANTLR
variant_type_parameter_list
    : '<' variant_type_parameter (',' variant_type_parameter)* '>'
    ;

variant_type_parameter
    : attributes? variance_annotation? type_parameter
    ;

variance_annotation
    : 'in'
    | 'out'
    ;
```

If the variance annotation is `out`, the type parameter is said to be ***covariant***. If the variance annotation is `in`, the type parameter is said to be ***contravariant***. If there is no variance annotation, the type parameter is said to be ***invariant***.

> *Example*: In the following:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"VariantTypeParameterLists"} -->
> ```csharp
> interface C<out X, in Y, Z>
> {
>     X M(Y y);
>     Z P { get; set; }
> }
> ```
>
> `X` is covariant, `Y` is contravariant and `Z` is invariant.
>
> *end example*

If a generic interface is declared in multiple parts ([§15.2.3](classes.md#1523-type-parameters)), each partial declaration shall specify the same variance for each type parameter.

#### 19.2.3.2 Variance safety

The occurrence of variance annotations in the type parameter list of a type restricts the places where types can occur within the type declaration. However, these restrictions do not apply to occurrences of types within a declaration of a non-virtual, non-abstract static member.

A type `T` is ***output-unsafe*** if one of the following holds:

- `T` is a contravariant type parameter
- `T` is an array type with an output-unsafe element type
- `T` is an interface or delegate type `S<Aᵢ,... Aₑ>` constructed from a generic type `S<Xᵢ, ... Xₑ>` where for at least one `Aᵢ` one of the following holds:
  - `Xᵢ` is covariant or invariant and `Aᵢ` is output-unsafe.
  - `Xᵢ` is contravariant or invariant and `Aᵢ` is input-unsafe.

A type `T` is ***input-unsafe*** if one of the following holds:

- `T` is a covariant type parameter
- `T` is an array type with an input-unsafe element type
- `T` is an interface or delegate type `S<Aᵢ,... Aₑ>` constructed from a generic type `S<Xᵢ, ... Xₑ>` where for at least one `Aᵢ` one of the following holds:
  - `Xᵢ` is covariant or invariant and `Aᵢ` is input-unsafe.
  - `Xᵢ` is contravariant or invariant and `Aᵢ` is output-unsafe.

Intuitively, an output-unsafe type is prohibited in an output position, and an input-unsafe type is prohibited in an input position.

A type is ***output-safe*** if it is not output-unsafe, and ***input-safe*** if it is not input-unsafe.

#### 19.2.3.3 Variance conversion

The purpose of variance annotations is to provide for more lenient (but still type safe) conversions to interface and delegate types. To this end the definitions of implicit ([§10.2](conversions.md#102-implicit-conversions)) and explicit conversions ([§10.3](conversions.md#103-explicit-conversions)) make use of the notion of variance-convertibility, which is defined as follows:

A type `T<Aᵢ, ..., Aᵥ>` is variance-convertible to a type `T<Bᵢ, ..., Bᵥ>` if `T` is either an interface or a delegate type declared with the variant type parameters `T<Xᵢ, ..., Xᵥ>`, and for each variant type parameter `Xᵢ` one of the following holds:

- `Xᵢ` is covariant and an implicit reference or identity conversion exists from `Aᵢ` to `Bᵢ`
- `Xᵢ` is contravariant and an implicit reference or identity conversion exists from `Bᵢ` to `Aᵢ`
- `Xᵢ` is invariant and an identity conversion exists from `Aᵢ` to `Bᵢ`

### 19.2.4 Base interfaces

An interface can inherit from zero or more interface types, which are called the ***explicit base interface***s of the interface. When an interface has one or more explicit base interfaces, then in the declaration of that interface, the interface identifier is followed by a colon and a comma-separated list of base interface types.

A derived interface may declare new members that hide inherited members ([§7.7.2.3](basic-concepts.md#7723-hiding-through-inheritance)) declared in base interfaces or explicitly implement inherited members ([§19.6.2](interfaces.md#1962-explicit-interface-member-implementations)) declared in base interfaces.

```ANTLR
interface_base
    : ':' interface_type_list
    ;
```

The explicit base interfaces can be constructed interface types ([§8.4](types.md#84-constructed-types), [§19.2](interfaces.md#192-interface-declarations)). A base interface cannot be a type parameter on its own, though it can involve the type parameters that are in scope.

For a constructed interface type, the explicit base interfaces are formed by taking the explicit base interface declarations on the generic type declaration, and substituting, for each *type_parameter* in the base interface declaration, the corresponding *type_argument* of the constructed type.

The explicit base interfaces of an interface shall be at least as accessible as the interface itself ([§7.5.5](basic-concepts.md#755-accessibility-constraints)).

> *Note*: For example, it is a compile-time error to specify a `private` or `internal` interface in the *interface_base* of a `public` interface. *end note*

It is a compile-time error for an interface to directly or indirectly inherit from itself.

The ***base interface***s of an interface are the explicit base interfaces and their base interfaces. In other words, the set of base interfaces is the complete transitive closure of the explicit base interfaces, their explicit base interfaces, and so on. An interface inherits all members of its base interfaces.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"BaseInterfaces1"} -->
> ```csharp
> interface IControl
> {
>     void Paint();
> }
> 
> interface ITextBox : IControl
> {
>     void SetText(string text);
> }
> 
> interface IListBox : IControl
> {
>     void SetItems(string[] items);
> }
>
> interface IComboBox: ITextBox, IListBox {}
> ```
>
> the base interfaces of `IComboBox` are `IControl`, `ITextBox`, and `IListBox`. In other words, the `IComboBox` interface above inherits members `SetText` and `SetItems` as well as `Paint`.
>
> *end example*

Members inherited from a constructed generic type are inherited after type substitution. That is, any constituent types in the member have the base class declaration’s type parameters replaced with the corresponding type arguments used in the *class_base* specification.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"BaseInterfaces2"} -->
> ```csharp
> interface IBase<T>
> {
>     T[] Combine(T a, T b);
> }
>
> interface IDerived : IBase<string[,]>
> {
>     // Inherited: string[][,] Combine(string[,] a, string[,] b);
> }
> ```
>
> the interface `IDerived` inherits the `Combine` method after the type parameter `T` is replaced with `string[,]`.
>
> *end example*

A class or struct that implements an interface also implicitly implements all of the interface’s base interfaces.

The handling of interfaces on multiple parts of a partial interface declaration ([§15.2.7](classes.md#1527-partial-type-declarations)) are discussed further in [§15.2.4.3](classes.md#15243-interface-implementations).

Every base interface of an interface shall be output-safe ([§19.2.3.2](interfaces.md#19232-variance-safety)).

## 19.3 Interface body

The *interface_body* of an interface defines the members of the interface.

```ANTLR
interface_body
    : '{' interface_member_declaration* '}'
    ;
```

## 19.4 Interface members

### 19.4.1 General

The members of an interface are the members inherited from the base interfaces and the members declared by the interface itself.

```ANTLR
interface_member_declaration
    : constant_declaration
    | field_declaration
    | method_declaration
    | property_declaration
    | event_declaration
    | indexer_declaration
    | static_constructor_declaration
    | operator_declaration
    | type_declaration
    ;
```

This subclause augments the description of members in classes ([§15.3](classes.md#153-class-members)) with the differences and restrictions for interfaces:

- A *finalizer_declaration* is not allowed.
- Instance constructors, *constructor_declaration*s, are not allowed.
- All interface members implicitly have public access; however, an explicit access modifier ([§7.5.2](basic-concepts.md#752-declared-accessibility)) is permitted except on static constructors ([§15.12](classes.md#1512-static-constructors)).
- The `abstract` modifier is implied for interface instance function members ([§12.6](expressions.md#126-function-members)) without bodies; that modifier may be given explicitly. For interface static function members without bodies the `abstract` modifier shall be present.
- An interface instance function member whose declaration includes a body is an implicitly `virtual` member unless the `sealed` or `private` modifier is used. The `virtual` modifier may be given explicitly. An interface static member whose declaration includes a body may have a `virtual` modifier.
- A `private` or `sealed` function member of an interface shall have a body.
- A `private` instance function member shall not have the modifier `sealed`.
- A derived interface may override an abstract or virtual member declared in a base interface.
- An explicitly implemented function member shall not have the modifier `sealed`.

Some declarations, such as *constant_declaration* ([§15.4](classes.md#154-constants)) have no restrictions in interfaces.

The inherited members of an interface are specifically not part of the declaration space of the interface. Thus, an interface is allowed to declare a member with the same name or signature as an inherited member. When this occurs, the derived interface member is said to *hide* the base interface member. Hiding an inherited member is not considered an error, but it does result in a warning ([§7.7.2.3](basic-concepts.md#7723-hiding-through-inheritance)).

If a `new` modifier is included in a declaration that does not hide an inherited member, a warning is issued to that effect.

> *Note*: The members in class `object` are not, strictly speaking, members of any interface ([§19.4](interfaces.md#194-interface-members)). However, the members in class `object` are available via member lookup in any interface type ([§12.5](expressions.md#125-member-lookup)). *end note*

The set of members of an interface declared in multiple parts ([§15.2.7](classes.md#1527-partial-type-declarations)) is the union of the members declared in each part. The bodies of all parts of the interface declaration share the same declaration space ([§7.3](basic-concepts.md#73-declarations)), and the scope of each member ([§7.7](basic-concepts.md#77-scopes)) extends to the bodies of all the parts.

> *Example*: Consider an interface `IA` with an implementation for a member `M` and a property `P`. An implementing type `C` does not provide an implementation for either `M` or `P`. They must be accessed through a reference whose compile-time type is an interface that is implicitly convertible to `IA` or `IB`. These members are not found through member lookup on a variable of type `C`.
>
> <!-- Example: {template:"standalone-console", name:"InterfaceMember", expectedOutput:["IB.M", "IA.P = 10", "IB.P = 20"]} -->
> ```csharp
> interface IA
> {
>     public int P { get { return 10; } }
>     public void M()
>     {
>         Console.WriteLine("IA.M");
>     }
> }
> 
> interface IB : IA
> {
>     public new int P { get { return 20; } }
>     void IA.M()
>     {
>         Console.WriteLine("IB.M");
>     }
> }
> 
> class C : IB { }
> 
> class Test
> {
>     public static void Main()
>     {
>         C c = new C();
>         ((IA)c).M();                               // cast needed
>         Console.WriteLine($"IA.P = {((IA)c).P}");  // cast needed
>         Console.WriteLine($"IB.P = {((IB)c).P}");  // cast needed
>     }
> }
> ```
>
>Within the interfaces `IA` and `IB`, member `M` is accessible directly by name. However, within method `Main`, we cannot write `c.M()` or `c.P`, as those names are not visible. To find them, casts to the appropriate interface type are needed. The declaration of `M` in `IB` uses explicit interface implementation syntax. This is necessary to make that method override the one in `IA`; the modifier `override` may not be applied to a function member. *end example*

### 19.4.2 Interface fields

This clause augments the description of fields in classes [§15.5](classes.md#155-fields) for fields declared in interfaces.

Interface fields are declared using *field_declaration*s ([§15.5.1](classes.md#1551-general)) with the following additional rules:

- It is a compile-time error for *field_declaration* to declare an instance field.

> *Example*: The following program contains static members of various kinds:
>
> <!-- Example: {template:"standalone-console", name:"InterfaceFields", inferOutput:true} -->
> ```csharp
> public interface IX
> {
>     public const int Constant = 100;
>     protected static int field;
> 
>     static IX()
>     {
>         Console.WriteLine("static members initialized");
>         Console.WriteLine($"constant = {IX.Constant}, field = {IX.field}");
>         field = 50;
>         Console.WriteLine("static constructor has run");
>     }
> }
>
> public class Test: IX
> {
>     public static void Main()
>     {
>         Console.WriteLine($"constant = {IX.Constant}, field = {IX.field}");
>     }
> }
> ```
>
> The output produced is
>
> ```console
> static members initialized
> constant = 100, field = 0
> static constructor has run
> constant = 100, field = 50
> ```
>
> *end example*

See [§19.4.8](interfaces.md#1948-interface-static-constructors) for information regarding the allocation and initialization of static fields.

### 19.4.3 Interface methods

This clause augments the description of methods in classes [§15.6](classes.md#156-methods) for methods declared in interfaces.

Interface methods are declared using *method_declaration*s ([§15.6](classes.md#156-methods))). The *attributes*, *return_type*, *ref_return_type*, *identifier*, and *parameter_list* of an interface method declaration have the same meaning as those of a method declaration in a class. Interface methods have the following additional rules:

- *method_modifier* shall not include `override`.
- An instance method whose body is a semi-colon (`;`) is `abstract`; the `abstract` modifier is not required, but is allowed. A static method whose body is a semi-colon (`;`) shall include the `abstract` modifier.
- An interface instance method declaration that has a block body or expression body as a *method_body* is `virtual`; the `virtual` modifier is not required, but is allowed. For a static method the `virtual` modifier is permitted.
- A *method_declaration* shall not have *type_parameter_constraints_clause*s unless it also has a *type_parameter_list*.
- The list of requirements for valid combinations of modifiers stated for a class method is extended, as follows:
  - A static declaration that is not extern shall have a block body or expression body as a *method_body*, or shall be declared `abstract`.
  - A virtual declaration that is not extern shall have a block body or expression body as a *method_body*.
  - A private declaration that is not extern shall have a block body or expression body as a *method_body*.
  - A sealed declaration that is not extern shall have a block body or expression body as a *method_body*.
  - An async declaration shall have a block body or expression body as a *method_body*.
- All parameter types of an interface method shall be input-safe ([§19.2.3.2](interfaces.md#19232-variance-safety)), and the return type shall be either `void` or output-safe.
- Any output or reference parameter types shall also be output-safe.

  > *Note*: Output parameters are required to be input-safe due to common implementation restrictions. *end note*

- Each class type constraint, interface type constraint and type parameter constraint on any type parameters of the method shall be input-safe.

These rules ensure that any covariant or contravariant usage of the interface remains typesafe.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceMethods1", expectedErrors:["CS1961"]} -->
> ```csharp
> interface I<out T>
> {
>     void M<U>() where U : T;     // Error
> }
> ```
>
> is ill-formed because the usage of `T` as a type parameter constraint on `U` is not input-safe.
>
> Were this restriction not in place it would be possible to violate type safety in the following manner:
>
> <!-- Incomplete$Example: {template:"standalone-lib-without-using", name:"InterfaceMethods2", replaceEllipsis:true, expectedErrors:["x","x"], expectedWarnings:["x","x"]} -->
> ```csharp
> interface I<out T>
> {
>     void M<U>() where U : T;
> }
> class B {}
> class D : B {}
> class E : B {}
> class C : I<D>
> {
>     public void M<D>() {...} 
> }
>
> ...
>
> I<B> b = new C();
> b.M<E>();
> ```
>
> This is actually a call to `C.M<E>`. But that call requires that `E` derive from `D`, so type safety would be violated here.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: See [§19.4.2](interfaces.md#1942-interface-fields) for an example that not only shows a static method with an implementation, but as that method is called `Main` and has the right return type and signature, it is also an entry point. *end note*

A virtual method with implementation declared in an interface may be overridden to be abstract in a derived interface. This is known as ***reabstraction***.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceMethods2"} -->
> ```csharp
> interface IA
> {
>     void M() { Console.WriteLine("IA.M"); }
> }
> 
> interface IB: IA
> {
>     abstract void IA.M();    // reabstraction of M
> }
> ```
>
> This is useful in derived interfaces where the implementation of a method is inappropriate and a more appropriate implementation should be provided by implementing classes. *end example*

### 19.4.4 Interface properties

This subclause augments the description of properties in classes [§15.7](classes.md#157-properties) for properties declared in interfaces.

Interface properties are declared using *property_declaration*s ([§15.7.1](classes.md#1571-general)) with the following additional rules:

- *property_modifier* shall not include `override`.
- An explicit interface member implementation shall not contain an *accessor_modifier* ([§15.7.3](classes.md#1573-accessors)).
- A derived interface may explicitly implement an abstract interface property declared in a base interface.

  > *Note*: As an interface cannot contain instance fields, an interface property cannot be an instance auto-property, as that would require the declaration of implicit hidden instance fields. *end note*

- The type of an interface property shall be output-safe if there is a get accessor, and shall be input-safe if there is a set or init accessor.
- An interface instance property or interface property accessor declaration that has a block body or expression body is `virtual`; the `virtual` modifier is not required, but is allowed. For a static property or property accessor the `virtual` modifier is permitted.
- An instance *property_declaration* that has no implementation is `abstract`; the `abstract` modifier is not required, but is allowed. It is *never* considered to be an automatically implemented property ([§15.7.4](classes.md#1574-automatically-implemented-properties)). However, the `abstract` modifier shall be present if a static property is to be abstract.
- A *property_declaration* may contain the `sealed` modifier.

### 19.4.5 Interface events

This subclause augments the description of events in classes [§15.8](classes.md#158-events) for events declared in interfaces.

Interface events are declared using *event_declaration*s ([§15.8.1](classes.md#1581-general)), with the following additional rules:

- *event_modifier* shall not include `override`.
- A derived interface may implement an abstract interface event declared in a base interface ([§15.8.5](classes.md#1585-virtual-sealed-override-and-abstract-accessors)).
- It is a compile-time error for *variable_declarators* in an instance *event_declaration* to contain any *variable_initializer*s.
- An instance event with the `virtual` or `sealed` modifiers shall declare accessors. It is *never* considered to be an automatically implemented field-like event ([§15.8.2](classes.md#1582-field-like-events)).
- An instance event with the `abstract` modifier shall not declare accessors.
- A static event may have `abstract`, `virtual`, and `sealed` modifiers.
- The type of an interface event shall be input-safe.

### 19.4.6 Interface indexers

This subclause augments the description of indexers in classes [§15.9](classes.md#159-indexers) for indexers declared in interfaces.

Interface indexers are declared using *indexer_declaration*s ([§15.9](classes.md#159-indexers)), with the following additional rules:

- *indexer_modifier* shall not include `override`.
- An *indexer_declaration* that has an *expression body* or contains an accessor with a block body or expression body is `virtual`; the `virtual` modifier is not required, but is allowed.
- An *indexer_declaration* whose accessor bodies are semi-colons (`;`) is `abstract`; the `abstract` modifier is not required, but is allowed.
- All the parameter types of an interface indexer shall be input-safe ([§19.2.3.2](interfaces.md#19232-variance-safety)).
- Any output or reference parameter types shall also be output-safe.

  > *Note*: Output parameters are required to be input-safe due to common implementation restrictions. *end note*

- The type of an interface indexer shall be output-safe if there is a get accessor, and shall be input-safe if there is a set or init accessor.

### 19.4.7 Interface operators

This subclause augments the description of *operator_declaration* members in classes [§15.10](classes.md#1510-operators) for operators declared in interfaces.

For an *operator_declaration* in an interface the *operator_body* shall only be a block body ([§15.6.1](classes.md#1561-general)) or an expression body ([§15.6.1](classes.md#1561-general)).

A static *operator_declaration* may have `abstract`, `virtual`, and `sealed` modifiers.

In the context of a class or struct, at least one of the *fixed_parameter*s in a *unary_operator_declarator* and *binary_operator_declarator* is required to have type `T` or `T?`, where `T` is the instance type of the enclosing type. This requirement is relaxed in the context of an interface in that a restricted operand is allowed to be of a type parameter that counts as “the instance type of the enclosing type.” In order for a type parameter `T` to count as that it shall meet the following requirements:

- `T` is a direct type parameter on the interface in which the operator declaration occurs, and
- `T` is directly constrained by the instance type; i.e., the surrounding interface with its own type parameters used as type arguments.

### 19.4.8 Interface static constructors

This subclause augments the description of static constructors in classes [§15.12](classes.md#1512-static-constructors) for static constructors declared in interfaces.

The static constructor for a closed ([§8.4.3](types.md#843-open-and-closed-types)) interface executes at most once in a given application domain. The execution of a static constructor is triggered by the first of the following actions to occur within an application domain:

- Any of the static members of the interface are referenced.
- Before the `Main` method is called for an interface containing the `Main` method ([§7.1](basic-concepts.md#71-application-startup)) in which execution begins.
- That interface provides an implementation for a member, and that implementation is accessed as the most specific implementation ([§19.4.10](interfaces.md#19410-most-specific-implementation)) for that member.

> *Note*: In the case where none of the preceding actions take place, the static constructor for an interface may not execute for a program where instances of types that implement the interface are created and used. *end note*

To initialize a new closed interface type, first a new set of static fields for that particular closed type is created. Each of the static fields is initialized to its default value. Next, the static field initializers are executed for those static fields. Finally, the static constructor is executed.

> *Note*: See [§19.4.2](interfaces.md#1942-interface-fields) for an example of using various kinds of static members (including a Main method) declared within an interface. *end note*

### 19.4.9 Interface nested types

This subclause augments the description of nested types in classes [§15.3.9](classes.md#1539-nested-types) for nested types declared in interfaces.

It is an error to declare a class type, struct type, or enum type within the scope of a type parameter that was declared with a *variance_annotation* ([§19.2.3.1](interfaces.md#19231-general)).

> *Example*: The declaration of `C` below is an error.
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceNestedTypes", expectedErrors:["CS8427"]} -->
> ```csharp
> interface IOuter<out T>
> {
>     class C { } // error: class declaration within scope of variant type parameter 'T'
> }
> ```
>
> *end example*

### 19.4.10 Most specific implementation

Every class and struct shall have a most specific implementation for every virtual member declared in all interfaces implemented by that type among the implementations appearing in the type or its direct and indirect interfaces. The ***most specific implementation*** is a unique implementation that is more specific than every other implementation.

> *Note*: The most specific implementation rule ensures that an ambiguity arising from diamond interface inheritance is resolved explicitly by the programmer at the point where the conflict occurs. *end note*

For a type `T` that is a struct or a class that implements interfaces `I2` and `I3`, where `I2` and `I3` both derive directly or indirectly from interface `I` that declares a member `M`, the most specific implementation of `M` is:

- If `T` declares an implementation of `I.M`, that implementation is the most specific implementation.
- Otherwise, if `T` is a class and a direct or indirect base class declares an implementation of `I.M`, the most derived base class of `T` is the most specific implementation.
- Otherwise, if `I2` and `I3` are interfaces implemented by `T` and `I3` derives from `I2` either directly or indirectly, `I3.M` is a more specific implementation than `I2.M`.
- Otherwise, neither `I2.M` nor `I3.M` are more specific and an error occurs.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceMethods4", expectedErrors:["CS8705"]} -->
> ```csharp
> interface IA
> {
>     void M() { Console.WriteLine("IA.M"); }
> }
> 
> interface IB : IA
> {
>     void IA.M() { Console.WriteLine("IB.M"); }
> }
> 
> interface IC: IA
> {
>     void IA.M() { Console.WriteLine("IC.M"); }
> }
> 
> abstract class C: IB, IC { } // error: no most specific implementation for 'IA.M'
> 
> abstract class D: IA, IB, IC // OK
> {
>     public abstract void M();
> }
> ```
>
> The most specific implementation rule ensures that a conflict (i.e., an ambiguity arising from diamond inheritance) is resolved explicitly by the programmer at the point where the conflict arises. *end example*

### 19.4.11 Interface member access

Interface members are accessed through member access ([§12.8.7](expressions.md#1287-member-access)) and indexer access ([§12.8.12.4](expressions.md#128124-indexer-access)) expressions of the form `I.M` and `I[A]`, where `I` is an interface type, `M` is a constant, field, method, property, or event of that interface type, and `A` is an indexer argument list.

In a class `D`, with direct or indirect base class `B`, where `B` directly or indirectly implements interface `I` and `I` defines a method `M()`, the expression `base.M()` is valid only if `base.M()` staticly ([§12.3](expressions.md#123-static-and-dynamic-binding)) binds to an implementation of `M()` in a class type.

For interfaces that are strictly single-inheritance (each interface in the inheritance chain has exactly zero or one direct base interface), the effects of the member lookup ([§12.5](expressions.md#125-member-lookup)), method invocation ([§12.8.10.2](expressions.md#128102-method-invocations)), and indexer access ([§12.8.12.4](expressions.md#128124-indexer-access)) rules are exactly the same as for classes and structs: More derived members hide less derived members with the same name or signature. However, for multiple-inheritance interfaces, ambiguities can occur when two or more unrelated base interfaces declare members with the same name or signature. This subclause shows several examples, some of which lead to ambiguities and others which do not. In all cases, explicit casts can be used to resolve the ambiguities.

> *Example*: In the following code
>
> <!-- NeedsReview$Example: {template:"standalone-lib-without-using", name:"InterfaceMemberAccess1", expectedErrors:["x","x"], expectedWarnings:["x","x"]} -->
> ```csharp
> interface IList
> {
>     int Count { get; set; }
> }
> 
> interface ICounter
> {
>     int Count { get; set; }
> }
>
> interface IListCounter : IList, ICounter {}
>
> class C
> {
>     void Test(IListCounter x)
>     {
>         x.Count = 1;             // Error
>         ((IList)x).Count = 1;    // Ok, invokes IList.Count.set
>         ((ICounter)x).Count = 1; // Ok, invokes ICounter.Count
>     }
> }
> ```
>
> the first statement causes a compile-time error because the member lookup ([§12.5](expressions.md#125-member-lookup)) of `Count` in `IListCounter` is ambiguous. As illustrated by the example, the ambiguity is resolved by casting `x` to the appropriate base interface type. Such casts have no run-time costs—they merely consist of viewing the instance as a less derived type at compile-time.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceMemberAccess2"} -->
> ```csharp
> interface IInteger
> {
>     void Add(int i);
> }
>
> interface IDouble
> {
>     void Add(double d);
> }
> 
> interface INumber : IInteger, IDouble {}
> 
> class C
> {
>     void Test(INumber n)
>     {
>         n.Add(1);             // Invokes IInteger.Add
>         n.Add(1.0);           // Only IDouble.Add is applicable
>         ((IInteger)n).Add(1); // Only IInteger.Add is a candidate
>         ((IDouble)n).Add(1);  // Only IDouble.Add is a candidate
>     }
> }
> ```
>
> the invocation `n.Add(1)` selects `IInteger.Add` by applying overload resolution rules of [§12.6.4](expressions.md#1264-overload-resolution). Similarly, the invocation `n.Add(1.0)` selects `IDouble.Add`. When explicit casts are inserted, there is only one candidate method, and thus no ambiguity.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceMemberAccess3"} -->
> ```csharp
> interface IBase
> {
>     void F(int i);
> }
>
> interface ILeft : IBase
> {
>     new void F(int i);
> }
>
> interface IRight : IBase
> {
>     void G();
> }
>
> interface IDerived : ILeft, IRight {}
>
> class A
> {
>     void Test(IDerived d)
>     {
>         d.F(1);           // Invokes ILeft.F
>         ((IBase)d).F(1);  // Invokes IBase.F
>         ((ILeft)d).F(1);  // Invokes ILeft.F
>         ((IRight)d).F(1); // Invokes IBase.F
>     }
> }
> ```
>
> the `IBase.F` member is hidden by the `ILeft.F` member. The invocation `d.F(1)` thus selects `ILeft.F`, even though `IBase.F` appears to not be hidden in the access path that leads through `IRight`.
>
> The intuitive rule for hiding in multiple-inheritance interfaces is simply this: If a member is hidden in any access path, it is hidden in all access paths. Because the access path from `IDerived` to `ILeft` to `IBase` hides `IBase.F`, the member is also hidden in the access path from `IDerived` to `IRight` to `IBase`.
>
> *end example*

## 19.5 Qualified interface member names

An interface member is sometimes referred to by its ***qualified interface member name***. The qualified name of an interface member consists of the name of the interface in which the member is declared, followed by a dot, followed by the name of the member. The qualified name of a member references the interface in which the member is declared.

> *Example*: Given the declarations
>
> <!-- Example: {template:"standalone-lib-without-using", name:"QualifiedInterfaceMemberNames1"} -->
> ```csharp
> interface IControl
> {
>     void Paint();
> }
> 
> interface ITextBox : IControl
> {
>     void SetText(string text);
> }
> ```
>
> the qualified name of `Paint` is `IControl.Paint` and the qualified name of SetText is `ITextBox.SetText`. In the example above, it is not possible to refer to `Paint` as `ITextBox.Paint`.
>
> *end example*

When an interface is part of a namespace, a qualified interface member name can include the namespace name.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"QualifiedInterfaceMemberNames2"} -->
> ```csharp
> namespace GraphicsLib
> {
>     interface IPolygon
>     {
>         void CalculateArea();
>     }
> }
> ```
>
> Within the `GraphicsLib` namespace, both `IPolygon.CalculateArea` and `GraphicsLib.IPolygon.CalculateArea` are qualified interface member names for the `CalculateArea` method.
>
> *end example*

## 19.6 Interface implementations

### 19.6.1 General

Interfaces may be implemented by classes and structs. To indicate that a class or struct directly implements an interface, the interface is included in the base class list of the class or struct.

A class or struct `C` that implements an interface `I` must provide or inherit an implementation for every member declared in `I` that `C` can access. Public members of `I` may be defined in public members of `C`. Non-public members declared in `I` that are accessible in `C` may be defined in `C` using explicit interface implementation ([§19.6.2](interfaces.md#1962-explicit-interface-member-implementations)).

 A member in a derived type that satisfies interface mapping ([§19.6.5](interfaces.md#1965-interface-mapping)) but does not implement the matching base interface member introduces a new member. This occurs when explicit interface implementation is required to define the interface member.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceImplementations1", replaceEllipsis:true, customEllipsisReplacements:["return default;","return default;"]} -->
> ```csharp
> interface ICloneable
> {
>     object Clone();
> }
>
> interface IComparable
> {
>     int CompareTo(object other);
> }
>
> class ListEntry : ICloneable, IComparable
> {
>     public object Clone() {...}    
>     public int CompareTo(object other) {...}
> }
> ```
>
> *end example*

A class or struct that directly implements an interface also implicitly implements all of the interface’s base interfaces. This is true even if the class or struct does not explicitly list all base interfaces in the base class list.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceImplementations2", replaceEllipsis:true} -->
> ```csharp
> interface IControl
> {
>     void Paint();
> }
>
> interface ITextBox : IControl
> {
>     void SetText(string text);
> }
>
> class TextBox : ITextBox
> {
>     public void Paint() {...}
>     public void SetText(string text) {...}
> }
> ```
>
> Here, class `TextBox` implements both `IControl` and `ITextBox`.
>
> *end example*

When a class `C` directly implements an interface, all classes derived from `C` also implement the interface implicitly.

The base interfaces specified in a class declaration can be constructed interface types ([§8.4](types.md#84-constructed-types), [§19.2](interfaces.md#192-interface-declarations)).

> *Example*: The following code illustrates how a class can implement constructed interface types:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceImplementations3"} -->
> ```csharp
> class C<U, V> {}
> interface I1<V> {}
> class D : C<string, int>, I1<string> {}
> class E<T> : C<int, T>, I1<T> {}
> ```
>
> *end example*

The base interfaces of a generic class declaration shall satisfy the uniqueness rule described in [§19.6.3](interfaces.md#1963-uniqueness-of-implemented-interfaces).

### 19.6.2 Explicit interface member implementations

<!-- The statement on class or structs implementing a non-public member requiring explicit interface member implementation is removed in C# 10. -->
For purposes of implementing interfaces, a class, struct, or interface may declare ***explicit interface member implementation***s. An explicit interface member implementation is a method, property, event, indexer, or operator declaration that references a qualified interface member name. A class or struct that implements a non-public member in a base interface must declare an explicit interface member implementation. An interface that implements a member in a base interface must declare an explicit interface member implementation.

A derived interface member that satisfies interface mapping ([§19.6.5](interfaces.md#1965-interface-mapping)) hides the base interface member ([§7.7.2](basic-concepts.md#772-name-hiding)). The compiler shall issue a warning unless the `new` modifier is present.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ExplicitInterfaceMemberImplementations1", replaceEllipsis:true, customEllipsisReplacements:["return default;","get { return default; }","return;"]} -->
> ```csharp
> interface IList<T>
> {
>     T[] GetElements();
> }
>
> interface IDictionary<K, V>
> {
>     V this[K key] { get; }
>     void Add(K key, V value);
> }
>
> class List<T> : IList<T>, IDictionary<int, T>
> {
>     public T[] GetElements() {...}
>     T IDictionary<int, T>.this[int index] {...}
>     void IDictionary<int, T>.Add(int index, T value) {...}
> }
> ```
>
> Here `IDictionary<int,T>.this` and `IDictionary<int,T>.Add` are explicit interface member implementations.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: In some cases, the name of an interface member might not be appropriate for the implementing class, in which case, the interface member may be implemented using explicit interface member implementation. A class implementing a file abstraction, for example, would likely implement a `Close` member function that has the effect of releasing the file resource, and implement the `Dispose` method of the `IDisposable` interface using explicit interface member implementation:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ExplicitInterfaceMemberImplementations2"} -->
> ```csharp
> interface IDisposable
> {
>     void Dispose();
> }
> 
> class MyFile : IDisposable
> {
>     void IDisposable.Dispose() => Close();
>
>     public void Close()
>     {
>         // Do what is necessary to close the file
>         System.GC.SuppressFinalize(this);
>     }
> }
> ```
>
> *end example*

It is not possible to access an explicit interface member implementation through its qualified interface member name in a method invocation, property access, event access, or indexer access. An explicit interface instance member implementation can only be accessed through an interface instance, and is in that case referenced simply by its member name. An explicit interface static member implementation can only be accessed through the interface name.

It is a compile-time error for an explicit interface member implementation to include any modifiers ([§15.6](classes.md#156-methods)) other than `extern`, `async`, or `static`. An explicit interface member implementation that implements a static member shall include the `static` modifier.

An explicit interface method implementation inherits any type parameter constraints from the interface.

A *type_parameter_constraints_clause* on an explicit interface method implementation may only consist of the `class` or `struct` *primary_constraint*s applied to *type_parameter*s which are known according to the inherited constraints to be either reference or value types respectively. Any type of the form `T?` in the signature of the explicit interface method implementation, where `T` is a type parameter, is interpreted as follows:

- If a `class` constraint is added for type parameter `T` then `T?` is a nullable reference type; otherwise
- If either there is no added constraint, or a `struct` constraint is added, for the type parameter `T` then `T?` is a nullable value type.

> *Example*: The following demonstrates how the rules work when type parameters are involved:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ExplicitInterfaceMemberImplementations6"} -->
> ```csharp
> #nullable enable
> interface I
> {
>     void Foo<T>(T? value) where T : class;
>     void Foo<T>(T? value) where T : struct;
> }
>
> class C : I
> {
>     void I.Foo<T>(T? value) where T : class { }
>     void I.Foo<T>(T? value) where T : struct { }
> }
> ```
>
> Without the type parameter constraint `where T : class`, the base method with the reference-typed type parameter cannot be overridden. *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: Explicit interface member implementations have different accessibility characteristics than other members. Because explicit interface member implementations are never accessible through a qualified interface member name in a method invocation or a property access, they are in a sense private. However, since they can be accessed through the interface, they are in a sense also as public as the interface in which they are declared.
> Explicit interface member implementations serve two primary purposes:
>
> - Because explicit interface member implementations are not accessible through class or struct instances, they allow interface implementations to be excluded from the public interface of a class or struct. This is particularly useful when a class or struct implements an internal interface that is of no interest to a consumer of that class or struct.
> - Explicit interface member implementations allow disambiguation of interface members with the same signature. Without explicit interface member implementations it would be impossible for a class, struct, or interface to have different implementations of interface members with the same signature and return type; and it would be impossible for a class, struct, or interface to have any implementation at all of interface members with the same signature but with different return types.
>
> *end note*

For an explicit interface member implementation to be valid, the class, struct, or interface shall name an interface in its base class or base interface list that contains a member whose qualified interface member name, type, number of type parameters, and parameter types exactly match those of the explicit interface member implementation. If an interface function member has a parameter array, the corresponding parameter of an associated explicit interface member implementation is allowed, but not required, to have the `params` modifier. If the interface function member does not have a parameter array then an associated explicit interface member implementation shall not have a parameter array.

For an explicit interface member implementation of a method, property, or indexer that has a return type, there shall be an identity conversion or (if the member has a value return) an implicit reference conversion from the return type of the explicit interface member implementation to the return type of every override of the interface member that is declared in a (direct or indirect) base interface.

> *Example*: Thus, in the following class
>
> <!-- Example: {template:"standalone-lib", name:"ExplicitInterfaceMemberImplementations3", replaceEllipsis:true, customEllipsisReplacements:["return default;","return default;"], expectedErrors:["CS0540"]} -->
> ```csharp
> class Shape : ICloneable
> {
>     object ICloneable.Clone() {...}
>     int IComparable.CompareTo(object other) {...} // invalid
> }
> ```
>
> the declaration of `IComparable.CompareTo` results in a compile-time error because `IComparable` is not listed in the base class list of `Shape` and is not a base interface of `ICloneable`. Likewise, in the declarations
>
> <!-- Example: {template:"standalone-lib", name:"ExplicitInterfaceMemberImplementations4", replaceEllipsis:true, customEllipsisReplacements:["return default;","return default;"], expectedErrors:["CS0540"]} -->
> ```csharp
> class Shape : ICloneable
> {
>     object ICloneable.Clone() {...}
> }
> 
> class Ellipse : Shape
> {
>     object ICloneable.Clone() {...} // invalid
> }
> ```
>
> the declaration of `ICloneable.Clone` in `Ellipse` results in a compile-time error because `ICloneable` is not explicitly listed in the base class list of `Ellipse`.
>
> *end example*

The qualified interface member name of an explicit interface member implementation shall reference the interface in which the member was declared.

> *Example*: Thus, in the declarations
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ExplicitInterfaceMemberImplementations5", replaceEllipsis:true} -->
> ```csharp
> interface IControl
> {
>     void Paint();
> }
>
> interface ITextBox : IControl
> {
>     void SetText(string text);
> }
> 
> class TextBox : ITextBox
> {
>     void IControl.Paint() {...}
>     void ITextBox.SetText(string text) {...}
> }
> ```
>
> the explicit interface member implementation of Paint must be written as `IControl.Paint`, not `ITextBox.Paint`.
>
> *end example*

An explicit interface member implementation that implements a static member shall itself be static.

### 19.6.3 Uniqueness of implemented interfaces

The interfaces implemented by a generic type declaration shall remain unique for all possible constructed types. Without this rule, it would be impossible to determine the correct method to call for certain constructed types.

> *Example*: Suppose a generic class declaration were permitted to be written as follows:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"UniquenessOfImplementedInterfaces1", replaceEllipsis:true, expectedErrors:["CS0695"]} -->
> ```csharp
> interface I<T>
> {
>     void F();
> }
>
> class X<U ,V> : I<U>, I<V> // Error: I<U> and I<V> conflict
> {
>     void I<U>.F() {...}
>     void I<V>.F() {...}
> }
> ```
>
> Were this permitted, it would be impossible to determine which code to execute in the following case:
>
> ```csharp
> I<int> x = new X<int, int>();
> x.F();
> ```
>
> *end example*

To determine if the interface list of a generic type declaration is valid, the following steps are performed:

- Let `L` be the list of interfaces directly specified in a generic class, struct, or interface declaration `C`.
- Add to `L` any base interfaces of the interfaces already in `L`.
- Remove any duplicates from `L`.
- If any possible constructed type created from `C` would, after type arguments are substituted into `L`, cause two interfaces in `L` to be identical, then the declaration of `C` is invalid. Constraint declarations are not considered when determining all possible constructed types.

> *Note*: In the class declaration `X` above, the interface list `L` consists of `l<U>` and `I<V>`. The declaration is invalid because any constructed type with `U` and `V` being the same type would cause these two interfaces to be identical types. *end note*

It is possible for interfaces specified at different inheritance levels to unify:

<!-- Example: {template:"standalone-lib-without-using", name:"UniquenessOfImplementedInterfaces2", replaceEllipsis:true} -->
```csharp
interface I<T>
{
    void F();
}

class Base<U> : I<U>
{
    void I<U>.F() {...}
}

class Derived<U, V> : Base<U>, I<V> // Ok
{
    void I<V>.F() {...}
}
```

This code is valid even though `Derived<U,V>` implements both `I<U>` and `I<V>`. The code

```csharp
I<int> x = new Derived<int, int>();
x.F();
```

invokes the method in `Derived`, since `Derived<int,int>'` effectively re-implements `I<int>` ([§19.6.7](interfaces.md#1967-interface-re-implementation)).

### 19.6.4 Implementation of generic methods

When a generic method implicitly implements an interface method, the constraints given for each method type parameter shall be equivalent in both declarations (after any interface type parameters are replaced with the appropriate type arguments), where method type parameters are identified by ordinal positions, left to right.

> *Example*: In the following code:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ImplementationOfGenericMethods1", replaceEllipsis:true, expectedErrors:["CS0425","CS0701"]} -->
> <!-- Maintenance Note: A version of this type exists in additional-files as "ITTT.cs". As such, certain changes to this type definition might need to be reflected in that file, in which case, *all* examples using that file should be tested. -->
> ```csharp
> interface I<X, Y, Z>
> {
>     void F<T>(T t) where T : X;
>     void G<T>(T t) where T : Y;
>     void H<T>(T t) where T : Z;
> }
>
> class C : I<object, C, string>
> {
>     public void F<T>(T t) {...}                  // Ok
>     public void G<T>(T t) where T : C {...}      // Ok
>     public void H<T>(T t) where T : string {...} // Error
> }
> ```
>
> the method `C.F<T>` implicitly implements `I<object,C,string>.F<T>`. In this case, `C.F<T>` is not required (nor permitted) to specify the constraint `T: object` since `object` is an implicit constraint on all type parameters. The method `C.G<T>` implicitly implements `I<object,C,string>.G<T>` because the constraints match those in the interface, after the interface type parameters are replaced with the corresponding type arguments. The constraint for method `C.H<T>` is an error because sealed types (`string` in this case) cannot be used as constraints. Omitting the constraint would also be an error since constraints of implicit interface method implementations are required to match. Thus, it is impossible to implicitly implement `I<object,C,string>.H<T>`. This interface method can only be implemented using an explicit interface member implementation:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ImplementationOfGenericMethods2", replaceEllipsis:true, additionalFiles:["ITTT.cs"]} -->
> ```csharp
> class C : I<object, C, string>
> {
>     ...
>     public void H<U>(U u) where U : class {...}
>
>     void I<object, C, string>.H<T>(T t)
>     {
>         string s = t; // Ok
>         H<T>(t);
>     }
> }
> ```
>
> In this case, the explicit interface member implementation invokes a public method having strictly weaker constraints. The assignment from t to s is valid since `T` inherits a constraint of `T: string`, even though this constraint is not expressible in source code.
>*end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: When a generic method explicitly implements an interface method no constraints are allowed on the implementing method ([§15.7.1](classes.md#1571-general), [§19.6.2](interfaces.md#1962-explicit-interface-member-implementations)). *end note*

### 19.6.5 Interface mapping

A class or struct shall provide implementations for all abstract members of the interfaces that are listed in the base class list of the class or struct which do not have a reachable implementation; where an implementation can become unreachable due to reabstraction [§19.4.3](interfaces.md#1943-interface-methods). The process of locating implementations of interface members in an implementing class or struct is known as ***interface mapping***.

Interface mapping for a class or struct `C` locates an implementation for each member of each interface specified in the base class list of `C`. The implementation of a particular interface member `I.M`, where `I` is the interface in which the member `M` is declared, is determined by examining each class, interface, or struct `S`, starting with `C` and repeating for each successive base class and implemented interface of `C`, until a match is located:

- If `S` contains a declaration of an explicit interface member implementation that matches `I` and `M`, then this member is the implementation of `I.M`.
- Otherwise, if `S` contains a declaration of a non-static public member that matches `M`, then this member is the implementation of `I.M`. If more than one member matches, it is unspecified which member is the implementation of `I.M`. This situation can only occur if `S` is a constructed type where the two members as declared in the generic type have different signatures, but the type arguments make their signatures identical.

A compile-time error occurs if implementations cannot be located for all members of all interfaces specified in the base class list of `C`. The members of an interface include those members that are inherited from base interfaces.

Members of a constructed interface type are considered to have any type parameters replaced with the corresponding type arguments as specified in [§15.3.3](classes.md#1533-members-of-constructed-types).

> *Example*: For example, given the generic interface declaration:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceMapping1"} -->
> ```csharp
> interface I<T>
> {
>     T F(int x, T[,] y);
>     T this[int y] { get; }
> }
> ```
>
> the constructed interface `I<string[]>` has the members:
>
> ```csharp
> string[] F(int x, string[,][] y);
> string[] this[int y] { get; }
> ```
>
> *end example*

For purposes of interface mapping, a class, interface, or struct member `A` matches an interface member `B` when:

- `A` and `B` are methods, and the name, type, and parameter lists of `A` and `B` are identical.
- `A` and `B` are properties, the name and type of `A` and `B` are identical, and `A` has the same accessors as `B` (`A` is permitted to have additional accessors if it is not an explicit interface member implementation).
- `A` and `B` are events, and the name and type of `A` and `B` are identical.
- `A` and `B` are indexers, the type and parameter lists of `A` and `B` are identical, and `A` has the same accessors as `B` (`A` is permitted to have additional accessors if it is not an explicit interface member implementation).

Notable implications of the interface-mapping algorithm are:

- Explicit interface member implementations take precedence over other members in the same class or struct when determining the class or struct member that implements an interface member.
- Neither non-public nor static members participate in interface mapping.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceMapping3", replaceEllipsis:true, customEllipsisReplacements:["return default;","return default;"]} -->
> ```csharp
> interface ICloneable
> {
>     object Clone();
> }
>
> class C : ICloneable
> {
>     object ICloneable.Clone() {...}
>     public object Clone() {...}
> }
> ```
>
> the `ICloneable.Clone` member of `C` becomes the implementation of `Clone` in `ICloneable` because explicit interface member implementations take precedence over other members.
>
> *end example*

If a class or struct implements two or more interfaces containing a member with the same name, type, and parameter types, it is possible to map each of those interface members onto a single class or struct member.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceMapping4", replaceEllipsis:true} -->
> ```csharp
> interface IControl
> {
>     void Paint();
> }
>
> interface IForm
> {
>     void Paint();
> }
>
> class Page : IControl, IForm
> {
>     public void Paint() {...}
> }
> ```
>
> Here, the `Paint` methods of both `IControl` and `IForm` are mapped onto the `Paint` method in `Page`. It is of course also possible to have separate explicit interface member implementations for the two methods.
>
> *end example*

If a class or struct implements an interface that contains hidden members, then some members may need to be implemented through explicit interface member implementations.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceMapping5"} -->
> ```csharp
> interface IBase
> {
>     int P { get; }
> }
>
> interface IDerived : IBase
> {
>     new int P();
> }
> ```
>
> An implementation of this interface would require at least one explicit interface member implementation, and would take one of the following forms
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceMapping6", replaceEllipsis:true, customEllipsisReplacements:["return default;","return default;","return default;"], additionalFiles:["IBase.cs","IDerived.cs"]} -->
> <!-- Maintenance Note: A version of this type exists in additional-files as "IBase.cs" and "IDerived.cs". As such, certain changes to this type definition might need to be reflected in that file, in which case, *all* examples using that file should be tested. -->
> ```csharp
> class C1 : IDerived
> {
>     int IBase.P { get; }
>     int IDerived.P() {...}
> }
> class C2 : IDerived
> {
>     public int P { get; }
>     int IDerived.P() {...}
> }
> class C3 : IDerived
> {
>     int IBase.P { get; }
>     public int P() {...}
> }
> ```
>
> *end example*

When a class implements multiple interfaces that have the same base interface, there can be only one implementation of the base interface.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceMapping7", replaceEllipsis:true} -->
> ```csharp
> interface IControl
> {
>     void Paint();
> }
>
> interface ITextBox : IControl
> {
>     void SetText(string text);
> }
>
> interface IListBox : IControl
> {
>     void SetItems(string[] items);
> }
>
> class ComboBox : IControl, ITextBox, IListBox
> {
>     void IControl.Paint() {...}
>     void ITextBox.SetText(string text) {...}
>     void IListBox.SetItems(string[] items) {...}
> }
> ```
>
> it is not possible to have separate implementations for the `IControl` named in the base class list, the `IControl` inherited by `ITextBox`, and the `IControl` inherited by `IListBox`. Indeed, there is no notion of a separate identity for these interfaces. Rather, the implementations of `ITextBox`and `IListBox` share the same implementation of `IControl`, and `ComboBox` is simply considered to implement three interfaces, `IControl`, `ITextBox`, and `IListBox`.
>
> *end example*

The members of a base class participate in interface mapping.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceMapping8"} -->
> ```csharp
> interface Interface1
> {
>     void F();
> }
>
> class Class1
> {
>     public void F() {}
>     public void G() {}
> }
>
> class Class2 : Class1, Interface1
> {
>     public new void G() {}
> }
> ```
>
> the method `F` in `Class1` is used in `Class2's` implementation of `Interface1`.
>
> *end example*

### 19.6.6 Interface implementation inheritance

A class inherits all interface implementations provided by its base classes.

Without explicitly re-implementing an interface, a derived class cannot in any way alter the interface mappings it inherits from its base classes.

> *Example*: In the declarations
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceImplementationInheritance1", replaceEllipsis:true} -->
> <!-- Maintenance Note: A version of these types exists in additional-files as "IControlControlTextBox1.cs". As such, certain changes to these type definitions might need to be reflected in that file, in which case, *all* examples using that file should be tested. -->
> ```csharp
> interface IControl
> {
>     void Paint();
> }
>
> class Control : IControl
> {
>     public void Paint() {...}
> }
>
> class TextBox : Control
> {
>     public new void Paint() {...}
> }
> ```
>
> the `Paint` method in `TextBox` hides the `Paint` method in `Control`, but it does not alter the mapping of `Control.Paint` onto `IControl.Paint`, and calls to `Paint` through class instances and interface instances will have the following effects
>
> <!-- Example: {template:"standalone-console-without-using", name:"InterfaceImplementationInheritance2", additionalFiles:["IControlControlTextBox1.cs"]} -->
> ```csharp
> Control c = new Control();
> TextBox t = new TextBox();
> IControl ic = c;
> IControl it = t;
> c.Paint();  // invokes Control.Paint();
> t.Paint();  // invokes TextBox.Paint();
> ic.Paint(); // invokes Control.Paint();
> it.Paint(); // invokes Control.Paint();
> ```
>
> *end example*

However, when an interface method is mapped onto a virtual method in a class, it is possible for derived classes to override the virtual method and alter the implementation of the interface.

> *Example*: Rewriting the declarations above to
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceImplementationInheritance3", replaceEllipsis:true} -->
> <!-- Maintenance Note: A version of these types exists in additional-files as "IControlControlTextBox2.cs". As such, certain changes to these type definitions might need to be reflected in that file, in which case, *all* examples using that file should be tested. -->
> ```csharp
> interface IControl
> {
>     void Paint();
> }
>
> class Control : IControl
> {
>     public virtual void Paint() {...}
> }
>
> class TextBox : Control
> {
>     public override void Paint() {...}
> }
> ```
>
> the following effects will now be observed
>
> <!-- Example: {template:"standalone-console-without-using", name:"InterfaceImplementationInheritance4", additionalFiles:["IControlControlTextBox2.cs"]} -->
> ```csharp
> Control c = new Control();
> TextBox t = new TextBox();
> IControl ic = c;
> IControl it = t;
> c.Paint();  // invokes Control.Paint();
> t.Paint();  // invokes TextBox.Paint();
> ic.Paint(); // invokes Control.Paint();
> it.Paint(); // invokes TextBox.Paint();
> ```
>
> *end example*

Since explicit interface member implementations cannot be declared virtual, it is not possible to override an explicit interface member implementation. However, it is perfectly valid for an explicit interface member implementation to call another method, and that other method can be declared virtual to allow derived classes to override it.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceImplementationInheritance5", replaceEllipsis:true} -->
> ```csharp
> interface IControl
> {
>     void Paint();
> }
>
> class Control : IControl
> {
>     void IControl.Paint() { PaintControl(); }
>     protected virtual void PaintControl() {...}
> }
> 
> class TextBox : Control
> {
>     protected override void PaintControl() {...}
> }
> ```
>
> Here, classes derived from `Control` can specialize the implementation of `IControl.Paint` by overriding the `PaintControl` method.
>
> *end example*

### 19.6.7 Interface re-implementation

A class that inherits an interface implementation is permitted to ***re-implement*** the interface by including it in the base class list.

A re-implementation of an interface follows exactly the same interface mapping rules as an initial implementation of an interface. Thus, the inherited interface mapping has no effect whatsoever on the interface mapping established for the re-implementation of the interface.

> *Example*: In the declarations
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceRe-implementation1", replaceEllipsis:true} -->
> ```csharp
> interface IControl
> {
>     void Paint();
> }
>
> class Control : IControl
> {
>     void IControl.Paint() {...}
> }
>
> class MyControl : Control, IControl
> {
>     public void Paint() {}
> }
> ```
>
> the fact that `Control` maps `IControl.Paint` onto `Control.IControl.Paint` does not affect the re-implementation in `MyControl`, which maps `IControl.Paint` onto `MyControl.Paint`.
>
> *end example*

Inherited public member declarations and inherited explicit interface member declarations participate in the interface mapping process for re-implemented interfaces.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceRe-implementation2"} -->
> ```csharp
> interface IMethods
> {
>     void F();
>     void G();
>     void H();
>     void I();
> }
>
> class Base : IMethods
> {
>     void IMethods.F() {}
>     void IMethods.G() {}
>     public void H() {}
>     public void I() {}
> }
>
> class Derived : Base, IMethods
> {
>     public void F() {}
>     void IMethods.H() {}
> }
> ```
>
> Here, the implementation of `IMethods` in `Derived` maps the interface methods onto `Derived.F`, `Base.IMethods.G`, `Derived.IMethods.H`, and `Base.I`.
>
> *end example*

When a class implements an interface, it implicitly also implements all that interface’s base interfaces. Likewise, a re-implementation of an interface is also implicitly a re-implementation of all of the interface’s base interfaces.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InterfaceRe-implementation3", replaceEllipsis:true} -->
> ```csharp
> interface IBase
> {
>     void F();
> }
>
> interface IDerived : IBase
> {
>     void G();
> }
>
> class C : IDerived
> {
>     void IBase.F() {...}
>     void IDerived.G() {...}
> }
>
> class D : C, IDerived
> {
>     public void F() {...}
>     public void G() {...}
> }
> ```
>
> Here, the re-implementation of `IDerived` also re-implements `IBase`, mapping `IBase.F` onto `D.F`.
>
> *end example*

### 19.6.8 Abstract classes and interfaces

Like a non-abstract class, an abstract class shall provide implementations for all abstract members of the interfaces that are listed in the base class list of the class which do not have a reachable implementation; where an implementation can become unreachable due to reabstraction [§19.4.3](interfaces.md#1943-interface-methods). However, an abstract class is permitted to map interface methods onto abstract methods.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AbstractClassesAndInterfaces1"} -->
> ```csharp
> interface IMethods
> {
>     void F();
>     void G();
> }
>
> abstract class C : IMethods
> {
>     public abstract void F();
>     public abstract void G();
> }
> ```
>
> Here, the implementation of `IMethods` maps `F` and `G` onto abstract methods, which shall be overridden in non-abstract classes that derive from `C`.
>
> *end example*

Explicit interface member implementations cannot be abstract, but explicit interface member implementations are of course permitted to call abstract methods.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AbstractClassesAndInterfaces2"} -->
> ```csharp
> interface IMethods
> {
>     void F();
>     void G();
> }
>
> abstract class C: IMethods
> {
>     void IMethods.F() { FF(); }
>     void IMethods.G() { GG(); }
>     protected abstract void FF();
>     protected abstract void GG();
> }
> ```
>
> Here, non-abstract classes that derive from `C` would be required to override `FF` and `GG`, thus providing the actual implementation of `IMethods`.
>
> *end example*
