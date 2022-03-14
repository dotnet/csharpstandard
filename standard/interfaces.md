# 17 Interfaces

## 17.1 General

An interface defines a contract. A class or struct that implements an interface shall adhere to its contract. An interface may inherit from multiple base interfaces, and a class or struct may implement multiple interfaces.

Interfaces can contain methods, properties, events, and indexers. The interface itself does not provide implementations for the members that it declares. The interface merely specifies the members that shall be supplied by classes or structs that implement the interface.

## 17.2 Interface declarations

### 17.2.1 General

An *interface_declaration* is a *type_declaration* ([§13.7](namespaces.md#137-type-declarations)) that declares a new interface type.

```ANTLR 
interface_declaration
    : attributes? interface_modifier* 'partial'? 'interface' identifier variant_type_parameter_list?
      interface_base? type_parameter_constraints_clause* interface_body ';'?
    ;
```

An *interface_declaration* consists of an optional set of *attributes* ([§21](attributes.md#21-attributes)), followed by an optional set of *interface_modifier*s ([§17.2.2](interfaces.md#1722-interface-modifiers)), followed by an optional partial modifier ([§14.2.7](classes.md#1427-partial-declarations)), followed by the keyword interface and an *identifier* that names the interface, followed by an optional *variant_type_parameter_list* specification ([§17.2.3](interfaces.md#1723-variant-type-parameter-lists)), followed by an optional *interface_base* specification ([§17.2.4](interfaces.md#1724-base-interfaces)), followed by an optional *type_parameter_constraints_clause*s specification ([§14.2.5](classes.md#1425-type-parameter-constraints)), followed by an *interface_body* ([§17.3](interfaces.md#173-interface-body)), optionally followed by a semicolon.

An interface declaration shall not supply a *type_parameter_constraints_clause*s unless it also supplies a *type_parameter_list*.

An interface declaration that supplies a *type_parameter_list* is a generic interface declaration. Additionally, any interface nested inside a generic class declaration or a generic struct declaration is itself a generic interface declaration, since type arguments for the containing type shall be supplied to create a constructed type.

### 17.2.2 Interface modifiers

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

*unsafe_modifier* ([§22.2](unsafe-code.md#222-unsafe-contexts)) is only available in unsafe code ([§22](unsafe-code.md#22-unsafe-code)).

It is a compile-time error for the same modifier to appear multiple times in an interface declaration.

The `new` modifier is only permitted on interfaces defined within a class. It specifies that the interface hides an inherited member by the same name, as described in [§14.3.5](classes.md#1435-the-new-modifier).

The `public`, `protected`, `internal`, and `private` modifiers control the accessibility of the interface. Depending on the context in which the interface declaration occurs, only some of these modifiers might be permitted ([§7.5.2](basic-concepts.md#752-declared-accessibility)). When a partial type declaration ([§14.2.7](classes.md#1427-partial-declarations)) includes an accessibility specification (via the `public`, `protected`, `internal`, and `private` modifiers), the rules in [§14.2.2](classes.md#1422-class-modifiers) apply.

### 17.2.3 Variant type parameter lists

#### 17.2.3.1 General

Variant type parameter lists can only occur on interface and delegate types. The difference from ordinary *type_parameter_list*s is the optional *variance_annotation* on each type parameter.

```ANTLR
variant_type_parameter_list
    : '<' variant_type_parameters '>'
    ;
```

```ANTLR
variant_type_parameters
    : attributes? variance_annotation? type_parameter
    | variant_type_parameters ',' attributes? variance_annotation? type_parameter
    ;
```

```ANTLR
variance_annotation
    : 'in'
    | 'out'
    ;
```

If the variance annotation is `out`, the type parameter is said to be ***covariant***. If the variance annotation is `in`, the type parameter is said to be ***contravariant***. If there is no variance annotation, the type parameter is said to be ***invariant***.

> *Example*: In the following:
> ```csharp
> interface C<out X, in Y, Z>
> {
>     X M(Y y);
>     Z P { get; set; }
> }
> ```
> `X` is covariant, `Y` is contravariant and `Z` is invariant. *end example*

If a generic interface is declared in multiple parts ([§14.2.3](classes.md#1423-type-parameters)), each partial declaration shall specify the same variance for each type parameter.

#### 17.2.3.2 Variance safety

The occurrence of variance annotations in the type parameter list of a type restricts the places where types can occur within the type declaration.

A type T is ***output-unsafe*** if one of the following holds:
- `T` is a contravariant type parameter
- `T` is an array type with an output-unsafe element type
- `T` is an interface or delegate type `Sᵢ,... Aₑ` constructed from a generic type `S<Xᵢ, ... Xₑ>` where for at least one `Aᵢ` one of the following holds:
    - `Xᵢ` is covariant or invariant and `Aᵢ` is output-unsafe.
    - `Xᵢ` is contravariant or invariant and `Aᵢ` is input-unsafe.

A type T is ***input-unsafe*** if one of the following holds:
- `T` is a covariant type parameter
- `T` is an array type with an input-unsafe element type
- `T` is an interface or delegate type  `S<Aᵢ,... Aₑ>` constructed from a generic type `S<Xᵢ, ... Xₑ>` where for at least one `Aᵢ` one of the following holds:
    - `Xᵢ` is covariant or invariant and `Aᵢ` is input-unsafe.
    - `Xᵢ` is contravariant or invariant and `Aᵢ` is output-unsafe.

Intuitively, an output-unsafe type is prohibited in an output position, and an input-unsafe type is prohibited in an input position.

A type is ***output-safe*** if it is not output-unsafe, and ***input-safe*** if it is not input-unsafe.

#### 17.2.3.3 Variance conversion

The purpose of variance annotations is to provide for more lenient (but still type safe) conversions to interface and delegate types. To this end the definitions of implicit ([§10.2](conversions.md#102-implicit-conversions)) and explicit conversions ([§10.3](conversions.md#103-explicit-conversions)) make use of the notion of variance-convertibility, which is defined as follows:

A type `T<Aᵢ, ..., Aᵥ>` is variance-convertible to a type `T<Bᵢ, ..., Aᵥ>` if `T` is either an interface or a delegate type declared with the variant type parameters `T<Xᵢ, ..., Xᵥ>`, and for each variant type parameter `Xᵢ` one of the following holds:

- `Xᵢ` is covariant and an implicit reference or identity conversion exists from `Aᵢ` to `Aᵢ`
- `Xᵢ` is contravariant and an implicit reference or identity conversion exists from `Bᵢ` to `Aᵢ`
- `Xᵢ` is invariant and an identity conversion exists from `Aᵢ` to `Bᵢ`

### 17.2.4 Base interfaces

An interface can inherit from zero or more interface types, which are called the ***explicit base interfaces*** of the interface. When an interface has one or more explicit base interfaces, then in the declaration of that interface, the interface identifier is followed by a colon and a comma-separated list of base interface types.

```ANTLR
interface_base
    : ':' interface_type_list
    ;
```

The explicit base interfaces can be constructed interface types ([§8.4](types.md#84-constructed-types), [§17.2](interfaces.md#172-interface-declarations)). A base interface cannot be a type parameter on its own, though it can involve the type parameters that are in scope.

For a constructed interface type, the explicit base interfaces are formed by taking the explicit base interface declarations on the generic type declaration, and substituting, for each *type_parameter* in the base interface declaration, the corresponding *type_argument* of the constructed type.

The explicit base interfaces of an interface shall be at least as accessible as the interface itself ([§7.5.5](basic-concepts.md#755-accessibility-constraints)). 

> *Note*: For example, it is a compile-time error to specify a `private` or `internal` interface in the *interface_base* of a `public` interface. *end note*

It is a compile-time error for an interface to directly or indirectly inherit from itself.

The ***base interfaces*** of an interface are the explicit base interfaces and their base interfaces. In other words, the set of base interfaces is the complete transitive closure of the explicit base interfaces, their explicit base interfaces, and so on. An interface inherits all members of its base interfaces.

> *Example*: In the following code
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
> the base interfaces of `IComboBox` are `IControl`, `ITextBox`, and `IListBox`. In other words, the `IComboBox` interface above inherits members `SetText` and `SetItems` as well as `Paint`. *end example*

Members inherited from a constructed generic type are inherited after type substitution. That is, any constituent types in the member have the base class declaration’s type parameters replaced with the corresponding type arguments used in the *class_base* specification.

> *Example*: In the following code
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
> the interface IDerived inherits the Combine method after the type parameter `T` is replaced with `string[,]`. *end example*

A class or struct that implements an interface also implicitly implements all of the interface's base interfaces.

The handling of interfaces on multiple parts of a partial interface declaration ([§14.2.7](classes.md#1427-partial-declarations)) are discussed further in [§14.2.4.3](classes.md#14243-interface-implementations).

Every base interface of an interface shall be output-safe ([§17.2.3.2](interfaces.md#17232-variance-safety)).

## 17.3 Interface body

The *interface_body* of an interface defines the members of the interface.

```ANTLR
interface_body
    : '{' interface_member_declaration* '}'
    ;
```

## 17.4 Interface members

### 17.4.1 General

The members of an interface are the members inherited from the base interfaces and the members declared by the interface itself.

```ANTLR
interface_member_declaration
    : interface_method_declaration
    | interface_property_declaration
    | interface_event_declaration
    | interface_indexer_declaration
    ;
```

An interface declaration declares zero or more members. The members of an interface shall be methods, properties, events, or indexers. An interface cannot contain constants, fields, operators, instance constructors, finalizers, or types, nor can an interface contain static members of any kind.

All interface members implicitly have public access. It is a compile-time error for interface member declarations to include any modifiers.

An *interface_declaration* creates a new declaration space ([§7.3](basic-concepts.md#73-declarations)), and the type parameters and *interface_member_declaration*s immediately contained by the *interface_declaration* introduce new members into this declaration space. The following rules apply to *interface_member_declaration*s:

- The name of a type parameter in the *type_parameter_list* of an interface declaration shall differ from the names of all other type parameters in the same *type_parameter_list* and shall differ from the names of all members of the interface.
- The name of a method shall differ from the names of all properties and events declared in the same interface. In addition, the signature ([§7.6](basic-concepts.md#76-signatures-and-overloading)) of a method shall differ from the signatures of all other methods declared in the same interface, and two methods declared in the same interface may not have signatures that differ solely by `ref` and `out`.
- The name of a property or event shall differ from the names of all other members declared in the same interface.
- The signature of an indexer shall differ from the signatures of all other indexers declared in the same interface.

The inherited members of an interface are specifically not part of the declaration space of the interface. Thus, an interface is allowed to declare a member with the same name or signature as an inherited member. When this occurs, the derived interface member is said to *hide* the base interface member. Hiding an inherited member is not considered an error, but it does cause the compiler to issue a warning. To suppress the warning, the declaration of the derived interface member shall include a `new` modifier to indicate that the derived member is intended to hide the base member. This topic is discussed further in [§7.7.2.3](basic-concepts.md#7723-hiding-through-inheritance).

If a `new` modifier is included in a declaration that doesn’t hide an inherited member, a warning is issued to that effect. This warning is suppressed by removing the `new` modifier.

> *Note*: The members in class `object` are not, strictly speaking, members of any interface ([§17.4](interfaces.md#174-interface-members)). However, the members in class object are available via member lookup in any interface type ([§11.5](expressions.md#115-member-lookup)). *end note*

The set of members of an interface declared in multiple parts ([§14.2.7](classes.md#1427-partial-declarations)) is the union of the members declared in each part. The bodies of all parts of the interface declaration share the same declaration space ([§7.3](basic-concepts.md#73-declarations)), and the scope of each member ([§7.7](basic-concepts.md#77-scopes)) extends to the bodies of all the parts.

### 17.4.2 Interface methods

Interface methods are declared using *interface_method_declaration*s:

```ANTLR
interface_method_declaration
    : attributes? 'new'? return_type identifier type_parameter_list? '(' formal_parameter_list? ')' type_parameter_constraints_clause* ';'
    ;
```

The *attributes*, *return_type*, *identifier*, and *formal_parameter_list* of an interface method declaration have the same meaning as those of a method declaration in a class ([§14.6](classes.md#146-methods)). An interface method declaration is not permitted to specify a method body, and the declaration therefore always ends with a semicolon. An *interface_method_declaration* shall not have *type_parameter_constraints_clause*s unless it also has a *type_parameter_list*.

All formal parameter types of an interface method shall be input-safe ([§17.2.3.2](interfaces.md#17232-variance-safety)), and the return type shall be either `void` or output-safe. In addition, any output or reference formal parameter types shall also be output-safe.

> *Note*: Output  parameters are required to be input-safe due to common implementation restrictions. *end note*

Furthermore, each class type constraint, interface type constraint and type parameter constraint on any type parameters of the method shall be input-safe. 

Furthermore, each class type constraint, interface type constraint and type parameter constraint on any type parameter of the method shall be input-safe.

These rules ensure that any covariant or contravariant usage of the interface remains typesafe.

> *Example*:
> ```csharp
> interface I<out T>
> {
>     void M<U>() where U : T; 
> }
> ```
> is ill-formed because the usage of `T` as a type parameter constraint on `U` is not input-safe.
> 
> Were this restriction not in place it would be possible to violate type safety in the following manner:
> ```csharp
> class B {}
> class D : B {}
> class E : B {}
> class C : I<D>
> {
>     public void M<U>() {...} 
> }
>
> ...
>
> I<B> b = new C();
> b.M<E>();
> ```
> This is actually a call to `C.M<E>`. But that call requires that `E` derive from `D`, so type safety would be violated here. *end example*

### 17.4.3 Interface properties

Interface properties are declared using *interface_property_declaration*s:

```ANTLR
interface_property_declaration
    : attributes? 'new'? type identifier '{' interface_accessors '}'
    ;
```

```ANTLR
interface_accessors
    : attributes? 'get' ';'
    | attributes? 'set' ';'
    | attributes? 'get' ';' attributes? 'set' ';'
    | attributes? 'set' ';' attributes? 'get' ';'
    ;
```

The *attributes*, *type*, and *identifier* of an interface property declaration have the same meaning as those of a property declaration in a class ([§14.7](classes.md#147-properties)).

The accessors of an interface property declaration correspond to the accessors of a class property declaration ([§14.7.3](classes.md#1473-accessors)), except that the accessor body shall always be a semicolon. Thus, the accessors simply indicate whether the property is read-write, read-only, or write-only.

The type of an interface property shall be output-safe if there is a get accessor, and shall be input-safe if there is a set accessor.

### 17.4.4 Interface events

Interface events are declared using *interface_event_declaration*s:

```ANTLR
interface_event_declaration
    : attributes? 'new'? 'event' type identifier ';'
    ;
```

The *attributes*, *type*, and *identifier* of an interface event declaration have the same meaning as those of an event declaration in a class ([§14.8](classes.md#148-events)).

The type of an interface event shall be input-safe.

### 17.4.5 Interface indexers

Interface indexers are declared using *interface_indexer_declaration*s:

```ANTLR
interface_indexer_declaration:
    attributes? 'new'? type 'this' '[' formal_parameter_list ']' '{' interface_accessors '}'
    ;
```

The *attributes*, *type*, and *formal_parameter_list* of an interface indexer declaration have the same meaning as those of an indexer declaration in a class ([§14.9](classes.md#149-indexers)).

The accessors of an interface indexer declaration correspond to the accessors of a class indexer declaration ([§14.9](classes.md#149-indexers)), except that the accessor body shall always be a semicolon. Thus, the accessors simply indicate whether the indexer is read-write, read-only, or write-only.

All the formal parameter types of an interface indexer shall be input-safe ([§17.2.3.2](interfaces.md#17232-variance-safety)). In addition, any output or reference formal parameter types shall also be output-safe.

> *Note*: Output parameters are required to be input-safe due to common implementation restrictions. *end note*

The type of an interface indexer shall be output-safe if there is a get accessor, and shall be input-safe if there is a set accessor.

### 17.4.6 Interface member access

Interface members are accessed through member access ([§11.7.6](expressions.md#1176-member-access)) and indexer access ([§11.7.10.3](expressions.md#117103-indexer-access)) expressions of the form `I.M` and `I[A]`, where `I` is an interface type, `M` is a method, property, or event of that interface type, and `A` is an indexer argument list.

For interfaces that are strictly single-inheritance (each interface in the inheritance chain has exactly zero or one direct base interface), the effects of the member lookup ([§11.5](expressions.md#115-member-lookup)), method invocation ([§11.7.8.2](expressions.md#11782-method-invocations)), and indexer access ([§11.7.10.3](expressions.md#117103-indexer-access)) rules are exactly the same as for classes and structs: More derived members hide less derived members with the same name or signature. However, for multiple-inheritance interfaces, ambiguities can occur when two or more unrelated base interfaces declare members with the same name or signature. This subclause shows several examples, some of which lead to ambiguities and others which don't. In all cases, explicit casts can be used to resolve the ambiguities.

> *Example*: In the following code
> 
> ```csharp
> interface IList
> {
>     int Count { get; set; }
> }
> 
> interface ICounter
> {
>     void Count(int i);
> }
>
> interface IListCounter : IList, ICounter {}
>
> class C
> {
>     void Test(IListCounter x)
>     {
>         x.Count(1);             // Error
>         x.Count = 1;            // Error
>         ((IList)x).Count = 1;   // Ok, invokes IList.Count.set
>         ((ICounter)x).Count(1); // Ok, invokes ICounter.Count
>     }
> }
> ```
> the first two statements cause compile-time errors because the member lookup ([§11.5](expressions.md#115-member-lookup)) of `Count` in `IListCounter` is ambiguous. As illustrated by the example, the ambiguity is resolved by casting `x` to the appropriate base interface type. Such casts have no run-time costs—they merely consist of viewing the instance as a less derived type at compile-time. *end example*

> *Example*: In the following code
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
> the invocation `n.Add(1)` selects `IInteger.Add` by applying overload resolution rules of [§11.6.4](expressions.md#1164-overload-resolution). Similarly, the invocation `n.Add(1.0)` selects `IDouble.Add`. When explicit casts are inserted, there is only one candidate method, and thus no ambiguity. *end example*

> *Example*: In the following code
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
> the `IBase.F` member is hidden by the `ILeft.F` member. The invocation `d.F(1)` thus selects `ILeft.F`, even though `IBase.F` appears to not be hidden in the access path that leads through `IRight`.
> 
> The intuitive rule for hiding in multiple-inheritance interfaces is simply this: If a member is hidden in any access path, it is hidden in all access paths. Because the access path from `IDerived` to `ILeft` to `IBase` hides `IBase.F`, the member is also hidden in the access path from `IDerived` to `IRight` to `IBase`. *end example*

## 17.5 Qualified interface member names

An interface member is sometimes referred to by its ***qualified interface member name***. The qualified name of an interface member consists of the name of the interface in which the member is declared, followed by a dot, followed by the name of the member. The qualified name of a member references the interface in which the member is declared.

> *Example*: Given the declarations
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
> the qualified name of `Paint` is `IControl.Paint` and the qualified name of SetText is `ITextBox.SetText`. In the example above, it is not possible to refer to `Paint` as `ITextBox.Paint`. *end example*

When an interface is part of a namespace, a qualified interface member name can include the namespace name.

> *Example*:
> ```csharp
> namespace System
> {
>     public interface ICloneable
>     {
>         object Clone();
>     }
> }
> ```
> Within the `System` namespace, both `ICloneable.Clone` and `System.ICloneable.Clone` are qualified interface member names for the `Clone` method. *end example*

## 17.6 Interface implementations

### 17.6.1 General

Interfaces may be implemented by classes and structs. To indicate that a class or struct directly implements an interface, the interface is included in the base class list of the class or struct.

> *Example*:
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
> *end example*

A class or struct that directly implements an interface also implicitly implements all of the interface's base interfaces. This is true even if the class or struct doesn't explicitly list all base interfaces in the base class list.

> *Example*:
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
> Here, class `TextBox` implements both `IControl` and `ITextBox`. *end example*

When a class `C` directly implements an interface, all classes derived from `C` also implement the interface implicitly.

The base interfaces specified in a class declaration can be constructed interface types ([§8.4](types.md#84-constructed-types), [§17.2](interfaces.md#172-interface-declarations)).

> *Example*: The following code illustrates how a class can implement constructed interface types:
> ```csharp
> class C<U, V> {}
> interface I1<V> {}
> class D : C<string, int>, I1<string> {}
> class E<T> : C<int, T>, I1<T> {}
> ```
> *end example*

The base interfaces of a generic class declaration shall satisfy the uniqueness rule described in [§17.6.3](interfaces.md#1763-uniqueness-of-implemented-interfaces).

### 17.6.2 Explicit interface member implementations

For purposes of implementing interfaces, a class or struct may declare ***explicit interface member implementations***. An explicit interface member implementation is a method, property, event, or indexer declaration that references a qualified interface member name.

> *Example*:
> ```csharp
> interface IList<T>
> {
>     T[] GetElements();
> }
>
> interface IDictionary<K, V>
> {
>     V this[K key];
>     void Add(K key, V value);
> }
>
> class List<T> : IList<T>, IDictionary<int, T>
> {
>     T[] T[] IList<T>. GetElements() {...}
>     T IDictionary<int, T>.this[int index] {...}
>     void IDictionary<int, T>.Add(int index, T value) {...}
> }
> ```
> Here `IDictionary<int,T>.this` and `IDictionary<int,T>.Add` are explicit interface member implementations. *end example*

> *Example*: In some cases, the name of an interface member might not be appropriate for the implementing class, in which case, the interface member may be implemented using explicit interface member implementation. A class implementing a file abstraction, for example, would likely implement a `Close` member function that has the effect of releasing the file resource, and implement the `Dispose` method of the `IDisposable` interface using explicit interface member implementation:
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
>         // Do what's necessary to close the file
>         System.GC.SuppressFinalize(this);
>     }
> }
> ```
> *end example*

It is not possible to access an explicit interface member implementation through its qualified interface member name in a method invocation, property access, event access, or indexer access. An explicit interface member implementation can only be accessed through an interface instance, and is in that case referenced simply by its member name.

It is a compile-time error for an explicit interface member implementation to include any modifiers ([§14.6](classes.md#146-methods)) other than `extern` or `async`.

It is a compile-time error for an explicit interface method implementation to include *type_parameter_constraints_clause*s. The constraints for a generic explicit interface method implementation are inherited from the interface method.

> *Note*: Explicit interface member implementations have different accessibility characteristics than other members. Because explicit interface member implementations are never accessible through a qualified interface member name in a method invocation or a property access, they are in a sense private. However, since they can be accessed through the interface, they are in a sense also as public as the interface in which they are declared.
> Explicit interface member implementations serve two primary purposes:
> - Because explicit interface member implementations are not accessible through class or struct instances, they allow interface implementations to be excluded from the public interface of a class or struct. This is particularly useful when a class or struct implements an internal interface that is of no interest to a consumer of that class or struct.
> - Explicit interface member implementations allow disambiguation of interface members with the same signature. Without explicit interface member implementations it would be impossible for a class or struct to have different implementations of interface members with the same signature and return type, as would it be impossible for a class or struct to have any implementation at all of interface members with the same signature but with different return types.  
*end note*

For an explicit interface member implementation to be valid, the class or struct shall name an interface in its base class list that contains a member whose qualified interface member name, type, number of type parameters, and parameter types exactly match those of the explicit interface member implementation. If an interface function member has a parameter array, the corresponding parameter of an associated explicit interface member implementation is allowed, but not required, to have the `params` modifier. If the interface function member does not have a parameter array then an associated explicit interface member implementation shall not have a parameter array.

> *Example*: Thus, in the following class
> ```csharp
> class Shape : ICloneable
> {
>     object ICloneable.Clone() {...}
>     int IComparable.CompareTo(object other) {...} // invalid
> }
> ```
> the declaration of `IComparable.CompareTo` results in a compile-time error because `IComparable` is not listed in the base class list of `Shape` and is not a base interface of `ICloneable`. Likewise, in the declarations
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
> the declaration of `ICloneable.Clone` in `Ellipse` results in a compile-time error because `ICloneable` is not explicitly listed in the base class list of `Ellipse`. *end example*

The qualified interface member name of an explicit interface member implementation shall reference the interface in which the member was declared.

> *Example*: Thus, in the declarations
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
> the explicit interface member implementation of Paint must be written as `IControl.Paint`, not `ITextBox.Paint`. *end example*

### 17.6.3 Uniqueness of implemented interfaces

The interfaces implemented by a generic type declaration shall remain unique for all possible constructed types. Without this rule, it would be impossible to determine the correct method to call for certain constructed types.

> *Example*: Suppose a generic class declaration were permitted to be written as follows:
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
> Were this permitted, it would be impossible to determine which code to execute in the following case:
> ```csharp
> I<int> x = new X<int, int>();
> x.F();
> ```
> *end example*

To determine if the interface list of a generic type declaration is valid, the following steps are performed:

- Let `L` be the list of interfaces directly specified in a generic class, struct, or interface declaration `C`.
- Add to `L` any base interfaces of the interfaces already in `L`.
- Remove any duplicates from `L`.
- If any possible constructed type created from `C` would, after type arguments are substituted into `L`, cause two interfaces in `L` to be identical, then the declaration of `C` is invalid. Constraint declarations are not considered when determining all possible constructed types.

> *Note*: In the class declaration `X` above, the interface list `L` consists of `l<U>` and `I<V>`. The declaration is invalid because any constructed type with `U` and `V` being the same type would cause these two interfaces to be identical types. *end note*

It is possible for interfaces specified at different inheritance levels to unify:

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

invokes the method in `Derived`, since `Derived<int,int>'` effectively re-implements `I<int>` ([§17.6.7](interfaces.md#1767-interface-re-implementation)).

### 17.6.4 Implementation of generic methods

When a generic method implicitly implements an interface method, the constraints given for each method type parameter shall be equivalent in both declarations (after any interface type parameters are replaced with the appropriate type arguments), where method type parameters are identified by ordinal positions, left to right.

> *Example*: In the following code:
> ```csharp
> interface I<X, Y, Z>
> {
>     void F<T>(T t) where T : X;
>     void G<T>(T t) where T : Y;
>     void H<T>(T t) where T : Z
> }
>
> class C : I<object, C, string>
> {
>     public void F<T>(T t) {...}                  // Ok
>     public void G<T>(T t) where T : C {...}      // Ok
>     public void H<T>(T t) where T : string {...} // Error
> }
> ```
> the method `C.F<T>` implicitly implements `I<object,C,string>.F<T>`. In this case, `C.F<T>` is not required (nor permitted) to specify the constraint `T: object` since `object` is an implicit constraint on all type parameters. The method `C.G<T>` implicitly implements `I<object,C,string>.G<T>` because the constraints match those in the interface, after the interface type parameters are replaced with the corresponding type arguments. The constraint for method `C.H<T>` is an error because sealed types (`string` in this case) cannot be used as constraints. Omitting the constraint would also be an error since constraints of implicit interface method implementations are required to match. Thus, it is impossible to implicitly implement `I<object,C,string>.H<T>`. This interface method can only be implemented using an explicit interface member implementation:
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
> In this case, the explicit interface member implementation invokes a public method having strictly weaker constraints. The assignment from t to s is valid since `T` inherits a constraint of `T: string`, even though this constraint is not expressible in source code.
>*end example*

> *Note*: When a generic method explicitly implements an interface method no constraints are allowed on the implementing method ([§14.7.1](classes.md#1471-general), [§17.6.2](interfaces.md#1762-explicit-interface-member-implementations)). *end note*

### 17.6.5 Interface mapping

A class or struct shall provide implementations of all members of the interfaces that are listed in the base class list of the class or struct. The process of locating implementations of interface members in an implementing class or struct is known as ***interface mapping***.

Interface mapping for a class or struct `C` locates an implementation for each member of each interface specified in the base class list of `C`. The implementation of a particular interface member `I.M`, where `I` is the interface in which the member `M` is declared, is determined by examining each class or struct `S`, starting with `C` and repeating for each successive base class of `C`, until a match is located:

- If `S` contains a declaration of an explicit interface member implementation that matches `I` and `M`, then this member is the implementation of `I.M`.
- Otherwise, if `S` contains a declaration of a non-static public member that matches `M`, then this member is the implementation of `I.M`. If more than one member matches, it is unspecified which member is the implementation of `I.M`. This situation can only occur if `S` is a constructed type where the two members as declared in the generic type have different signatures, but the type arguments make their signatures identical.

A compile-time error occurs if implementations cannot be located for all members of all interfaces specified in the base class list of `C`. The members of an interface include those members that are inherited from base interfaces.

Members of a constructed interface type are considered to have any type parameters replaced with the corresponding type arguments as specified in [§14.3.3](classes.md#1433-members-of-constructed-types).

> *Example*: For example, given the generic interface declaration:
> ```csharp
> interface I<T>
> {
>     T F(int x, T[,] y);
>     T this[int y] { get; }
> }
> ```
> the constructed interface `I<string[]>` has the members:
> ```csharp
> string[] F(int x, string[,][] y);
> string[] this[int y] { get; }
> ```
> *end example*

For purposes of interface mapping, a class or struct member `A` matches an interface member `B` when:

- `A` and `B` are methods, and the name, type, and formal parameter lists of `A` and `B` are identical.
- `A` and `B` are properties, the name and type of `A` and `B` are identical, and `A` has the same accessors as `B` (`A` is permitted to have additional accessors if it is not an explicit interface member implementation).
- `A` and `B` are events, and the name and type of `A` and `B` are identical.
- `A` and `B` are indexers, the type and formal parameter lists of `A` and `B` are identical, and `A` has the same accessors as `B` (`A` is permitted to have additional accessors if it is not an explicit interface member implementation).

Notable implications of the interface-mapping algorithm are:

- Explicit interface member implementations take precedence over other members in the same class or struct when determining the class or struct member that implements an interface member.
- Neither non-public nor static members participate in interface mapping.

> *Example*: In the following code
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
> the `ICloneable.Clone` member of `C` becomes the implementation of `Clone` in 'ICloneable' because explicit interface member implementations take precedence over other members. *end example*

If a class or struct implements two or more interfaces containing a member with the same name, type, and parameter types, it is possible to map each of those interface members onto a single class or struct member.

> *Example*:
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
> Here, the `Paint` methods of both `IControl` and `IForm` are mapped onto the `Paint` method in `Page`. It is of course also possible to have separate explicit interface member implementations for the two methods. *end example*

If a class or struct implements an interface that contains hidden members, then some members may need to be implemented through explicit interface member implementations.

> *Example*:
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
> An implementation of this interface would require at least one explicit interface member implementation, and would take one of the following forms
> ```csharp
> class C : IDerived
> {
>     int IBase.P { get; }
>     int IDerived.P() {...}
> }
> class C : IDerived
> {
>     public int P { get; }
>     int IDerived.P() {...}
> }
> class C : IDerived
> {
>     int IBase.P { get; }
>     public int P() {...}
> }
> ```
> *end example*

When a class implements multiple interfaces that have the same base interface, there can be only one implementation of the base interface.

> *Example*: In the following code
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
> it is not possible to have separate implementations for the `IControl` named in the base class list, the `IControl` inherited by `ITextBox`, and the `IControl` inherited by `IListBox`. Indeed, there is no notion of a separate identity for these interfaces. Rather, the implementations of `ITextBox`and `IListBox` share the same implementation of `IControl`, and `ComboBox` is simply considered to implement three interfaces, `IControl`, `ITextBox`, and `IListBox`. *end example*

The members of a base class participate in interface mapping.

> *Example*: In the following code
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
> the method `F` in `Class1` is used in `Class2's` implementation of `Interface1`. *end example*

### 17.6.6 Interface implementation inheritance

A class inherits all interface implementations provided by its base classes.

Without explicitly re-implementing an interface, a derived class cannot in any way alter the interface mappings it inherits from its base classes.

> *Example*: In the declarations
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
> the `Paint` method in `TextBox` hides the `Paint` method in `Control`, but it does not alter the mapping of `Control.Paint` onto `IControl.Paint`, and calls to `Paint` through class instances and interface instances will have the following effects
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
> *end example*

However, when an interface method is mapped onto a virtual method in a class, it is possible for derived classes to override the virtual method and alter the implementation of the interface.

> *Example*: Rewriting the declarations above to
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
> the following effects will now be observed
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
> *end example*

Since explicit interface member implementations cannot be declared virtual, it is not possible to override an explicit interface member implementation. However, it is perfectly valid for an explicit interface member implementation to call another method, and that other method can be declared virtual to allow derived classes to override it.

> *Example*:
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

Here, classes derived from `Control` can specialize the implementation of `IControl.Paint` by overriding the `PaintControl` method. *end example*

### 17.6.7 Interface re-implementation

A class that inherits an interface implementation is permitted to ***re-implement*** the interface by including it in the base class list.

A re-implementation of an interface follows exactly the same interface mapping rules as an initial implementation of an interface. Thus, the inherited interface mapping has no effect whatsoever on the interface mapping established for the re-implementation of the interface.

> *Example*: In the declarations
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
> the fact that `Control` maps `IControl.Paint` onto `Control.IControl.Paint` doesn't affect the re-implementation in `MyControl`, which maps `IControl.Paint` onto `MyControl.Paint`. *end example*

Inherited public member declarations and inherited explicit interface member declarations participate in the interface mapping process for re-implemented interfaces.

> *Example*:
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
> Here, the implementation of `IMethods` in `Derived` maps the interface methods onto `Derived.F`, `Base.IMethods.G`, `Derived.IMethods.H`, and `Base.I`. *end example*

When a class implements an interface, it implicitly also implements all that interface's base interfaces. Likewise, a re-implementation of an interface is also implicitly a re-implementation of all of the interface's base interfaces.

> *Example*:
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
> Here, the re-implementation of `IDerived` also re-implements `IBase`, mapping `IBase.F` onto `D.F`. *end example*

### 17.6.8 Abstract classes and interfaces

Like a non-abstract class, an abstract class shall provide implementations of all members of the interfaces that are listed in the base class list of the class. However, an abstract class is permitted to map interface methods onto abstract methods.

> *Example*:
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
>     }
> ```
> Here, the implementation of `IMethods` maps `F` and `G` onto abstract methods, which shall be overridden in non-abstract classes that derive from `C`. *end example*

Explicit interface member implementations cannot be abstract, but explicit interface member implementations are of course permitted to call abstract methods.

> *Example*:
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
> Here, non-abstract classes that derive from `C` would be required to override `FF` and `GG`, thus providing the actual implementation of `IMethods`. *end example*
