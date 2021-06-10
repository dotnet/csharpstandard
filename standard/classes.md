# 15 Classes

## 15.1 General

A class is a data structure that may contain data members (constants and fields), function members (methods, properties, events, indexers, operators, instance constructors, finalizers, and static constructors), and nested types. Class types support inheritance, a mechanism whereby a ***derived class*** can extend and specialize a ***base class***.

## 15.2 Class declarations

### 15.2.1 General

A *class_declaration* is a *type_declaration* ([§14.7](namespaces.md#147-type-declarations)) that declares a new class.

```ANTLR
class_declaration
  : attributes? class_modifier* 'partial'? 'class' Identifier type_parameter_list?
  class_base? type_parameter_constraints_clause* class_body ';'?
  ;
```

A *class_declaration* consists of an optional set of *attributes* ([§22](attributes.md#22-attributes)), followed by an optional set of *class_modifier*s ([§15.2.2](classes.md#1522-class-modifiers)), followed by an optional `partial` modifier ([§15.2.7](classes.md#1527-partial-declarations)), followed by the keyword `class` and an *Identifier* that names the class, followed by an optional *type_parameter_list* ([§15.2.3](classes.md#1523-type-parameters)), followed by an optional *class_base* specification ([§15.2.4](classes.md#1524-class-base-specification)), followed by an optional set of *type_parameter_constraints_clause*s ([§15.2.5](classes.md#1525-type-parameter-constraints)), followed by a *class_body* ([§15.2.6](classes.md#1526-class-body)), optionally followed by a semicolon.

A class declaration shall not supply a *type_parameter_constraints_clause*s unless it also supplies a *type_parameter_list*.

A class declaration that supplies a *type_parameter_list* is a generic class declaration. Additionally, any class nested inside a generic class declaration or a generic struct declaration is itself a generic class declaration, since type arguments for the containing type shall be supplied to create a constructed type.

### 15.2.2 Class modifiers

#### 15.2.2.1 General

A *class_declaration* may optionally include a sequence of class modifiers:

```ANTLR
class_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | 'abstract'
    | 'sealed'
    | 'static'
    | unsafe_modifier   // unsafe code support
    ;
```

*unsafe_modifier* ([§23.2](unsafe-code.md#232-unsafe-contexts)) is only available in unsafe code ([§23](unsafe-code.md#23-unsafe-code)).

It is a compile-time error for the same modifier to appear multiple times in a class declaration.

The `new` modifier is permitted on nested classes. It specifies that the class hides an inherited member by the same name, as described in [§15.3.5](classes.md#1535-the-new-modifier). It is a compile-time error for the `new` modifier to appear on a class declaration that is not a nested class declaration.

The `public`, `protected`, `internal`, and `private` modifiers control the accessibility of the class. Depending on the context in which the class declaration occurs, some of these modifiers might not be permitted ([§8.5.2](basic-concepts.md#852-declared-accessibility)).

When a partial type declaration ([§15.2.7](classes.md#1527-partial-declarations)) includes an accessibility specification (via the `public`, `protected`, `internal`, and `private` modifiers), that specification shall agree with all other parts that include an accessibility specification. If no part of a partial type includes an accessibility specification, the type is given the appropriate default accessibility ([§8.5.2](basic-concepts.md#852-declared-accessibility)).

The `abstract`, `sealed`, and `static` modifiers are discussed in the following subclauses.

#### 15.2.2.2 Abstract classes

The `abstract` modifier is used to indicate that a class is incomplete and that it is intended to be used only as a base class. An ***abstract class*** differs from a ***non-abstract class*** in the following ways:

-  An abstract class cannot be instantiated directly, and it is a compile-time error to use the `new` operator on an abstract class. While it is possible to have variables and values whose compile-time types are abstract, such variables and values will necessarily either be `null` or contain references to instances of non-abstract classes derived from the abstract types.
-  An abstract class is permitted (but not required) to contain abstract members.
-  An abstract class cannot be sealed.

When a non-abstract class is derived from an abstract class, the non-abstract class shall include actual implementations of all inherited abstract members, thereby overriding those abstract members.

> *Example*: In the following code
> ```csharp
> abstract class A
> {
>     public abstract void F();
> }
> abstract class B: A
> {
>     public void G() {}
> }
> class C: B
> {
>     public override void F() {
>         // actual implementation of F
>     }
> }
> ```
> the abstract class `A` introduces an abstract method `F`. Class `B` introduces an additional method `G`, but since it doesn't provide an implementation of `F`, `B` shall also be declared abstract. Class `C` overrides `F` and provides an actual implementation. Since there are no abstract members in `C`, `C` is permitted (but not required) to be non-abstract. *end example*

If one or more parts of a partial type declaration ([§15.2.7](classes.md#1527-partial-declarations)) of a class include the `abstract` modifier, the class is abstract. Otherwise, the class is non-abstract.

#### 15.2.2.3 Sealed classes

The `sealed` modifier is used to prevent derivation from a class. A compile-time error occurs if a sealed class is specified as the base class of another class.

A sealed class cannot also be an abstract class.

> *Note*: The `sealed` modifier is primarily used to prevent unintended derivation, but it also enables certain run-time optimizations. In particular, because a sealed class is known to never have any derived classes, it is possible to transform virtual function member invocations on sealed class instances into non-virtual invocations. *end note*

If one or more parts of a partial type declaration ([§15.2.7](classes.md#1527-partial-declarations)) of a class include the `sealed` modifier, the class is sealed. Otherwise, the class is unsealed.

#### 15.2.2.4 Static classes

##### 15.2.2.4.1 General

The `static` modifier is used to mark the class being declared as a ***static class***. A static class shall not be instantiated, shall not be used as a type and shall contain only static members. Only a static class can contain declarations of extension methods ([§15.6.10](classes.md#15610-extension-methods)).

A static class declaration is subject to the following restrictions:

-  A static class shall not include a `sealed` or `abstract` modifier. (However, since a static class cannot be instantiated or derived from, it behaves as if it was both sealed and abstract.)
-  A static class shall not include a *class-base* specification ([§15.2.4](classes.md#1524-class-base-specification)) and cannot explicitly specify a base class or a list of implemented interfaces. A static class implicitly inherits from type `object`.
-  A static class shall only contain static members ([§15.3.8](classes.md#1538-static-and-instance-members)).  
   > *Note*: All constants and nested types are classified as static members. *end note*
-  A static class shall not have members with `protected` or `protected internal` declared accessibility.

It is a compile-time error to violate any of these restrictions.

A static class has no instance constructors. It is not possible to declare an instance constructor in a static class, and no default instance constructor ([§15.11.5](classes.md#15115-default-constructors)) is provided for a static class.

The members of a static class are not automatically static, and the member declarations shall explicitly include a `static` modifier (except for constants and nested types). When a class is nested within a static outer class, the nested class is not a static class unless it explicitly includes a `static` modifier.

If one or more parts of a partial type declaration ([§15.2.7](classes.md#1527-partial-declarations)) of a class include the `static` modifier, the class is static. Otherwise, the class is not static.

##### 15.2.2.4.2 Referencing static class types

A *namespace_or_type_name* ([§8.8](basic-concepts.md#88-namespace-and-type-names)) is permitted to reference a static class if

-  The *namespace_or_type_name* is the `T` in a *namespace_or_type_name* of the form `T.I`, or
-  The *namespace_or_type-name* is the `T` in a *typeof_expression* ([§12.7.12](expressions.md#12712-the-typeof-operator)) of the form `typeof(T)`.
A *primary_expression* ([§12.7](expressions.md#127-primary-expressions)) is permitted to reference a static class if
-  The *primary_expression* is the `E` in a *member_access* ([§12.7.5](expressions.md#1275-member-access)) of the form `E.I`.

In any other context, it is a compile-time error to reference a static class.

> *Note*: For example, it is an error for a static class to be used as a base class, a constituent type ([§15.3.7](classes.md#1537-constituent-types)) of a member, a generic type argument, or a type parameter constraint. Likewise, a static class cannot be used in an array type, a pointer type, a new expression, a cast expression, an is expression, an as expression, a `sizeof` expression, or a default value expression. *end note*

### 15.2.3 Type parameters

A type parameter is a simple identifier that denotes a placeholder for a type argument supplied to create a constructed type. By constrast, a type argument ([§9.4.2](types.md#942-type-arguments)) is the type that is substituted for the type parameter when a constructed type is created.

```ANTLR
type_parameter_list
  : '<' type_parameters '>'
  ;

type_parameters
  : attributes? type_parameter
  | type_parameters ',' attributes? type_parameter
  ;
```

*type_parameter* is defined in [§9.5](types.md#95-type-parameters).

Each type parameter in a class declaration defines a name in the declaration space ([§8.3](basic-concepts.md#83-declarations)) of that class. Thus, it cannot have the same name as another type parameter of that class or a member declared in that class. A type parameter cannot have the same name as the type itself.

Two partial generic type declarations (in the same program) contribute to the same unbound generic type if they have the same fully qualified name (which includes a *generic_dimension_specifier* ([§12.7.12](expressions.md#12712-the-typeof-operator)) for the number of type parameters) ([§8.8.3](basic-concepts.md#883-fully-qualified-names)). Two such partial type declarations shall specify the same name for each type parameter, in order.

### 15.2.4 Class base specification

#### 15.2.4.1 General

A class declaration may include a *class_base* specification, which defines the direct base class of the class and the interfaces ([§18](interfaces.md#18-interfaces)) directly implemented by the class.

```ANTLR
class_base
  : ':' class_type
  | ':' interface_type_list
  | ':' class_type ',' interface_type_list
  ;

interface_type_list
  : interface_type (',' interface_type)*
  ;
```

#### 15.2.4.2 Base classes

When a *class_type* is included in the *class_base*, it specifies the direct base class of the class being declared. If a non-partial class declaration has no *class_base*, or if the *class_base* lists only interface types, the direct base class is assumed to be `object`. When a partial class declaration includes a base class specification, that base class specification shall reference the same type as all other parts of that partial type that include a base class specification. If no part of a partial class includes a base class specification, the base class is object. A class inherits members from its direct base class, as described in [§15.3.4](classes.md#1534-inheritance).

> *Example*: In the following code
> ```csharp
> class A {}
> class B: A {}
> ```
> Class `A` is said to be the direct base class of `B`, and `B` is said to be derived from `A`. Since `A` does not explicitly specify a direct base class, its direct base class is implicitly `object`. *end example*

For a constructed class type, including a nested type declared within a generic type declaration ([§15.3.9.7](classes.md#15397-nested-types-in-generic-classes)), if a base class is specified in the generic class declaration, the base class of the constructed type is obtained by substituting, for each *type_parameter* in the base class declaration, the corresponding *type_argument* of the constructed type. 

> *Example*: Given the generic class declarations
> ```csharp
> class B<U,V> {...}
> class G<T>: B<string,T[]> {...}
> ```
> the base class of the constructed type `G<int>` would be `B<string,int[]>`. *end example*

The base class specified in a class declaration can be a constructed class type ([§9.4](types.md#94-constructed-types)). A base class cannot be a type parameter on its own ([§9.5](types.md#95-type-parameters)), though it can involve the type parameters that are in scope.

> *Example*:
> ```csharp
> class Base<T> {}
> class Extend : Base<int> // Valid, non-constructed class with
> // constructed base class
> class Extend<V>: V {} // Error, type parameter used as base class
> class Extend<V> : Base<V> {} // Valid, type parameter used as type
> // argument for base class
> ```
> *end example*

The direct base class of a class type shall be at least as accessible as the class type itself ([§8.5.5](basic-concepts.md#855-accessibility-constraints)). For example, it is a compile-time error for a public class to derive from a private or internal class.

The direct base class of a class type shall not be any of the following types: `System.Array`, `System.Delegate`, `System.Enum`, or `System.ValueType`. Furthermore, a generic class declaration shall not use `System.Attribute` as a direct or indirect base class ([§22.2.1](attributes.md#2221-general)).

In determining the meaning of the direct base class specification `A` of a class `B`, the direct base class of `B` is temporarily assumed to be `object`, which ensures that the meaning of a base class specification cannot recursively depend on itself.

> *Example*: The following
> ```csharp
> class X<T> {
>     public class Y{}
> }
> class Z : X<Z.Y> {}
> ```
> is in error since in the base class specification `X<Z.Y>` the direct base class of `Z` is considered to be object, and hence (by the rules of [§8.8](basic-concepts.md#88-namespace-and-type-names)) `Z` is not considered to have a member `Y`. *end example*

The base classes of a class are the direct base class and its base classes. In other words, the set of base classes is the transitive closure of the direct base class relationship.

> *Example*: In the following:
> ```csharp
> class A {...}
> class B<T>: A {...}
> class C<T>: B<IComparable<T>> {...}
> class D<T>: C<T[]> {...}
> ```
> the base classes of `D<int>` are `C<int[]>`, `B<IComparable<int[]>>`, `A`, and `object`. *end example*

Except for class `object`, every class has exactly one direct base class. The `object` class has no direct base class and is the ultimate base class of all other classes.

It is a compile-time error for a class to depend on itself. For the purpose of this rule, a class ***directly depends on*** its direct base class (if any) and *directly depends on* the nearest enclosing class within which it is nested (if any). Given this definition, the complete set of classes upon which a class depends is the transitive closure of the *directly depends on* relationship.

> *Example*: The example
> ```csharp
> class A: A {}
> ```
> is erroneous because the class depends on itself. Likewise, the example
> ```csharp
> class A: B {}
> class B: C {}
> class C: A {}
> ```
> is in error because the classes circularly depend on themselves. Finally, the example
> ```csharp
> class A: B.C {}
> class B: A
> {
>     public class C {}
> }
> ```
> results in a compile-time error because A depends on `B.C` (its direct base class), which depends on `B` (its immediately enclosing class), which circularly depends on `A`. *end example*

A class does not depend on the classes that are nested within it.

> *Example*: In the following code
> ```csharp
> class A
> {
> class B: A {}
> }
> ```
> `B` depends on `A` (because `A` is both its direct base class and its immediately enclosing class), but `A` does not depend on `B` (since `B` is neither a base class nor an enclosing class of `A`). Thus, the example is valid. *end example*

It is not possible to derive from a sealed class.
> *Example*: In the following code
> ```csharp
> sealed class A {}
> class B: A {} // Error, cannot derive from a sealed class
> ```
> Class `B` is in error because it attempts to derive from the sealed class `A`. *end example*

#### 15.2.4.3 Interface implementations

A *class_base* specification may include a list of interface types, in which case the class is said to implement the given interface types. For a constructed class type, including a nested type declared within a generic type declaration ([§15.3.9.7](classes.md#15397-nested-types-in-generic-classes)), each implemented interface type is obtained by substituting, for each *type_parameter* in the given interface, the corresponding *type_argument* of the constructed type.

The set of interfaces for a type declared in multiple parts ([§15.2.7](classes.md#1527-partial-declarations)) is the union of the interfaces specified on each part. A particular interface can only be named once on each part, but multiple parts can name the same base interface(s). There shall only be one implementation of each member of any given interface.

> *Example*: In the following:
> ```csharp
> partial class C: IA, IB {...}
> partial class C: IC {...}
> partial class C: IA, IB {...}
> ```
> the set of base interfaces for class `C` is `IA`, `IB`, and `IC`. *end example*

Typically, each part provides an implementation of the interface(s) declared on that part; however, this is not a requirement. A part can provide the implementation for an interface declared on a different part.

> *Example*:
> ```csharp
> partial class X
> {
>     int IComparable.CompareTo(object o) {...}
> }
> partial class X: IComparable
> {
>     ...
> }
> ```
> *end example*

The base interfaces specified in a class declaration can be constructed interface types ([§9.4](types.md#94-constructed-types), [§18.2](interfaces.md#182-interface-declarations)). A base interface cannot be a type parameter on its own, though it can involve the type parameters that are in scope.

> *Example*: The following code illustrates how a class can implement and extend constructed types:
> ```csharp
> class C<U, V> {}
> interface I1<V> {}
> class D: C<string, int>, I1<string> {}
> class E<T>: C<int, T>, I1<T> {}
> ```
> *end example*

Interface implementations are discussed further in [§18.6](interfaces.md#186-interface-implementations).

### 15.2.5 Type parameter constraints

Generic type and method declarations can optionally specify type parameter constraints by including *type_parameter_constraints_clause*s.

```ANTLR
type_parameter_constraints_clauses
    : type_parameter_constraints_clause
    | type_parameter_constraints_clauses type_parameter_constraints_clause
    ;
    
type_parameter_constraints_clause
    : 'where' type_parameter ':' type_parameter_constraints
    ;

type_parameter_constraints
    : primary_constraint
    | secondary_constraints
    | constructor_constraint
    | primary_constraint ',' secondary_constraints
    | primary_constraint ',' constructor_constraint
    | secondary_constraints ',' constructor_constraint
    | primary_constraint ',' secondary_constraints ',' constructor_constraint
    ;

primary_constraint
    : class_type
    | 'class'
    | 'struct'
    ;

secondary_constraints
    : interface_type
    | type_parameter
    | secondary_constraints ',' interface_type
    | secondary_constraints ',' type_parameter
    ;

constructor_constraint
    : 'new' '(' ')'
    ;
```

Each *type_parameter_constraints_clause* consists of the token `where`, followed by the name of a type parameter, followed by a colon and the list of constraints for that type parameter. There can be at most one `where` clause for each type parameter, and the `where` clauses can be listed in any order. Like the `get` and `set` tokens in a property accessor, the `where` token is not a keyword.

The list of constraints given in a `where` clause can include any of the following components, in this order: a single primary constraint, one or more secondary constraints, and the constructor constraint, `new()`.

A primary constraint can be a class type or the ***reference type constraint*** `class` or the ***value type constraint*** `struct`. A secondary constraint can be a *type_parameter* or *interface_type*.

The reference type constraint specifies that a type argument used for the type parameter shall be a reference type. All class types, interface types, delegate types, array types, and type parameters known to be a reference type (as defined below) satisfy this constraint.

The value type constraint specifies that a type argument used for the type parameter shall be a non-nullable value type. All non-nullable struct types, enum types, and type parameters having the value type constraint satisfy this constraint. Note that although classified as a value type, a nullable value type ([§9.3.11](types.md#9311-nullable-value-types)) does not satisfy the value type constraint. A type parameter having the value type constraint shall not also have the *constructor_constraint*, although it may be used as a type argument for another type parameter with a *constructor_constraint*. 

> *Note*: The `System.Nullable<T>` type specifies the non-nullable value type constraint for `T`. Thus, recursively constructed types of the forms `T??` and `Nullable<Nullable<T>>` are prohibited. *end note*

Pointer types are never allowed to be type arguments and are not considered to satisfy either the reference type or value type constraints.

If a constraint is a class type, an interface type, or a type parameter, that type specifies a minimal "base type" that every type argument used for that type parameter shall support. Whenever a constructed type or generic method is used, the type argument is checked against the constraints on the type parameter at compile-time. The type argument supplied shall satisfy the conditions described in [§9.4.5](types.md#945-satisfying-constraints).

A *class_type* constraint shall satisfy the following rules:

-  The type shall be a class type.
-  The type shall not be `sealed`.
-  The type shall not be one of the following types: `System.Array`, `System.Delegate`, `System.Enum`, or `System.ValueType`.
-  The type shall not be `object`.
-  At most one constraint for a given type parameter may be a class type.

A type specified as an *interface_type* constraint shall satisfy the following rules:

-  The type shall be an interface type.
-  A type shall not be specified more than once in a given `where` clause.

In either case, the constraint may involve any of the type parameters of the associated type or method declaration as part of a constructed type, and may involve the type being declared.

Any class or interface type specified as a type parameter constraint shall be at least as accessible ([§8.5.5](basic-concepts.md#855-accessibility-constraints)) as the generic type or method being declared.

A type specified as a *type_parameter* constraint shall satisfy the following rules:

-  The type shall be a type parameter.
-  A type shall not be specified more than once in a given `where` clause.

In addition there shall be no cycles in the dependency graph of type parameters, where dependency is a transitive relation defined by:

-  If a type parameter `T` is used as a constraint for type parameter `S` then `S` ***depends on*** `T`.
-  If a type parameter `S` depends on a type parameter `T` and `T` depends on a type parameter `U` then `S` *depends on* `U`.

Given this relation, it is a compile-time error for a type parameter to depend on itself (directly or indirectly).

Any constraints shall be consistent among dependent type parameters. If type parameter `S` depends on type parameter `T` then:

-  `T` shall not have the value type constraint. Otherwise, `T` is effectively sealed so `S` would be forced to be the same type as `T`, eliminating the need for two type parameters.
-  If `S` has the value type constraint then `T` shall not have a *class_type* constraint.
-  If `S` has a *class_type* constraint `A` and `T` has a *class_type* constraint `B` then there shall be an identity conversion or implicit reference conversion from `A` to `B` or an implicit reference conversion from `B` to `A`.
-  If `S` also depends on type parameter `U` and `U` has a *class_type* constraint `A` and `T` has a *class_type* constraint `B` then there shall be an identity conversion or implicit reference conversion from `A` to `B` or an implicit reference conversion from `B` to `A`.

It is valid for `S` to have the value type constraint and `T` to have the reference type constraint. Effectively this limits `T` to the types `System.Object`, `System.ValueType`, `System.Enum`, and any interface type.

If the `where` clause for a type parameter includes a constructor constraint (which has the form `new()`), it is possible to use the `new` operator to create instances of the type ([§12.7.11.2](expressions.md#127112-object-creation-expressions)). Any type argument used for a type parameter with a constructor constraint shall be a value type, a non-abstract class having a public parameterless constructor, or a type parameter having the value type constraint or constructor constraint.

It is a compile-time error for *type_parameter_constraints* having a *primary_constraint* of `struct` to also have a *constructor_constraint*.

> *Example*: The following are examples of constraints:
> ```csharp
> interface IPrintable
> {
>     void Print();
> }
> interface IComparable<T>
> {
>     int CompareTo(T value);
> }
> interface IKeyProvider<T>
> {
>     T GetKey();
> }
> class Printer<T> where T: IPrintable {...}
> class SortedList<T> where T: IComparable<T> {...}
> class Dictionary<K,V>
>     where K: IComparable<K>
>     where V: IPrintable, IKeyProvider<K>, new()
> {
> ...
> }
> ```
> The following example is in error because it causes a circularity in the dependency graph of the type parameters:
> ```csharp
> class Circular<S,T>
>     where S: T
>     where T: S // Error, circularity in dependency graph
> {
> ...
> }
> ```
> The following examples illustrate additional invalid situations:
> ```csharp
> class Sealed<S,T>
>     where S: T
>     where T: struct // Error, `T` is sealed
> {
> ...
> }
> class A {...}
> class B {...}
> class Incompat<S,T>
>     where S: A, T
>     where T: B // Error, incompatible class-type constraints
> {
> ...
> }
> class StructWithClass<S,T,U>
>     where S: struct, T
>     where T: U
>     where U: A // Error, A incompatible with struct
> {
> ...
> }
> ```
> *end example*

The ***dynamic erasure*** of a type `C` is type `Cₓ` constructed as follows: 

-  If `C` is a nested type `Outer.Inner` then `Cₓ` is a nested type `Outerₓ.Innerₓ`.
-  If `C` `Cₓ`is a constructed type `G<A¹, ..., Aⁿ>` with type arguments `A¹, ..., Aⁿ` then `Cₓ` is the constructed type `G<A¹ₓ, ..., Aⁿₓ>`.
-  If `C` is an array type `E[]` then `Cₓ` is the array type `Eₓ[]`.
-  If `C` is a pointer type `E*` then `Cₓ` is the pointer type `Eₓ*`.
-  If `C` is dynamic then `Cₓ` is object.
-  Otherwise, `Cₓ` is `C`.

The ***effective base class*** of a type parameter `T` is defined as follows:

Let `R` be a set of types such that:

-  For each constraint of `T` that is a type parameter, `R` contains its effective base class.
-  For each constraint of `T` that is a struct type, `R` contains `System.ValueType`.
-  For each constraint of `T` that is an enumeration type, `R` contains `System.Enum`.
-  For each constraint of `T` that is a delegate type, `R` contains its dynamic erasure.
-  For each constraint of `T` that is an array type, `R` contains `System.Array`.
-  For each constraint of `T` that is a class type, `R` contains its dynamic erasure.

Then

-  If `T` has the value type constraint, its effective base class is `System.ValueType`.
-  Otherwise, if `R` is empty then the effective base class is `object`.
-  Otherwise, the effective base class of `T` is the most-encompassed type ([§11.5.3](conversions.md#1153-evaluation-of-user-defined-conversions)) of set `R`. If the set has no encompassed type, the effective base class of `T` is object. The consistency rules ensure that the most-encompassed type exists.

If the type parameter is a method type parameter whose constraints are inherited from the base method the effective base class is calculated after type substitution.

These rules ensure that the effective base class is always a *class-type*.

The ***effective interface set*** of a type parameter `T` is defined as follows:

-  If `T` has no *secondary_constraints*, its effective interface set is empty.
-  If `T` has *interface_type* constraints but no *type_parameter* constraints, its effective interface set is the set of dynamic erasures of its *interface_type* constraints.
-  If `T` has no *interface_type* constraints but has *type_parameter* constraints, its effective interface set is the union of the effective interface sets of its *type_parameter* constraints.
-  If `T` has both *interface_type* constraints and *type_parameter* constraints, its effective interface set is the union of the set of dynamic erasures of its *interface_type* constraints and the effective interface sets of its *type_parameter* constraints.

A type parameter is ***known to be a reference type*** if it has the reference type constraint or its effective base class is not `object` or `System.ValueType`.

Values of a constrained type parameter type can be used to access the instance members implied by the constraints.

> *Example*: In the following:
> ```csharp
> interface IPrintable
> {
>     void Print();
> }
> class Printer<T> where T: IPrintable
> {
> void PrintOne(T x) {
>     x.Print();
>     }
> }
> ```
> the methods of `IPrintable` can be invoked directly on `x` because `T` is constrained to always implement `IPrintable`. *end example*

When a partial generic type declaration includes constraints, the constraints shall agree with all other parts that include constraints. Specifically, each part that includes constraints shall have constraints for the same set of type parameters, and for each type parameter, the sets of primary, secondary, and constructor constraints shall be equivalent. Two sets of constraints are equivalent if they contain the same members. If no part of a partial generic type specifies type parameter constraints, the type parameters are considered unconstrained.

> *Example*:
> ```csharp
> partial class Map<K,V>
>     where K: IComparable<K>
>     where V: IKeyProvider<K>, new()
> {
> ...
> }
> partial class Map<K,V>
>     where V: IKeyProvider<K>, new()
>     where K: IComparable<K>
> {
> ...
> }
> partial class Map<K,V>
> {
> ...
> }
> ```
> is correct because those parts that include constraints (the first two) effectively specify the same set of primary, secondary, and constructor constraints for the same set of type parameters, respectively. *end example*

### 15.2.6 Class body

The *class_body* of a class defines the members of that class.

```ANTLR
class_body
  : '{' class_member_declaration* '}'
  ;
```

### 15.2.7 Partial declarations

The modifier `partial` is used when defining a class, struct, or interface type in multiple parts. The `partial` modifier is a contextual keyword ([§7.4.4](lexical-structure.md#744-keywords)) and only has special meaning immediately before one of the keywords `class`, `struct`, or `interface`.

Each part of a ***partial type*** declaration shall include a `partial` modifier and shall be declared in the same namespace or containing type as the other parts. The `partial` modifier indicates that additional parts of the type declaration might exist elsewhere, but the existence of such additional parts is not a requirement; it is valid for the only declaration of a type to include the `partial` modifier.

All parts of a partial type shall be compiled together such that the parts can be merged at compile-time. Partial types specifically do not allow already compiled types to be extended.

Nested types can be declared in multiple parts by using the `partial` modifier. Typically, the containing type is declared using `partial` as well, and each part of the nested type is declared in a different part of the containing type.

> [*Example*: The following partial class is implemented in two parts, which reside in different compilation units. The first part is machine generated by a database-mapping tool while the second part is manually authored:
> ```csharp
> public partial class Customer
> {
>     private int id;
>     private string name;
>     private string address;
>     private List<Order> orders;
>     public Customer() {
>     ...
>     }
> }
> public partial class Customer
> {
>     public void SubmitOrder(Order orderSubmitted) {
>        orders.Add(orderSubmitted);
>     }
>     public bool HasOutstandingOrders() {
>        return orders.Count > 0;
>     }
> }
> ```
> When the two parts above are compiled together, the resulting code behaves as if the class had been written as a single unit, as follows:
> ```csharp
> public class Customer
> {
>     private int id;
>     private string name;
>     private string address;
>     private List<Order> orders;
>     public Customer() {
>     ...
>     }
>     public void SubmitOrder(Order orderSubmitted) {
>        orders.Add(orderSubmitted);
>     }
>     public bool HasOutstandingOrders() {
>        return orders.Count > 0;
>     }
> }
> ```
> *end example*

The handling of attributes specified on the type or type parameters of different parts of a partial declaration is discussed in [§22.3](attributes.md#223-attribute-specification).

## 15.3 Class members

### 15.3.1 General

The members of a class consist of the members introduced by its *class_member_declaration*s and the members inherited from the direct base class.

```ANTLR
class_member_declaration
    : constant_declaration
    | field_declaration
    | method_declaration
    | property_declaration
    | event_declaration
    | indexer_declaration
    | operator_declaration
    | constructor_declaration
    | finalizer_declaration
    | static_constructor_declaration
    | type_declaration
    ;
```

The members of a class are divided into the following categories:

-  Constants, which represent constant values associated with the class ([§15.4](classes.md#154-constants)).
-  Fields, which are the variables of the class ([§15.5](classes.md#155-fields)).
-  Methods, which implement the computations and actions that can be performed by the class ([§15.6](classes.md#156-methods)).
-  Properties, which define named characteristics and the actions associated with reading and writing those characteristics ([§15.7](classes.md#157-properties)).
-  Events, which define notifications that can be generated by the class ([§15.8](classes.md#158-events)).
-  Indexers, which permit instances of the class to be indexed in the same way (syntactically) as arrays ([§15.9](classes.md#159-indexers)).
-  Operators, which define the expression operators that can be applied to instances of the class ([§15.10](classes.md#1510-operators)).
-  Instance constructors, which implement the actions required to initialize instances of the class ([§15.11](classes.md#1511-instance-constructors))
-  Finalizers, which implement the actions to be performed before instances of the class are permanently discarded ([§15.13](classes.md#1513-finalizers)).
-  Static constructors, which implement the actions required to initialize the class itself ([§15.12](classes.md#1512-static-constructors)).
-  Types, which represent the types that are local to the class ([§14.7](namespaces.md#147-type-declarations)).

A *class_declaration* creates a new declaration space ([§8.3](basic-concepts.md#83-declarations)), and the *type_parameter*s and the *class_member_declaration*s immediately contained by the *class_declaration* introduce new members into this declaration space. The following rules apply to *class_member_declaration*s:

-  Instance constructors, finalizers, and static constructors shall have the same name as the immediately enclosing class. All other members shall have names that differ from the name of the immediately enclosing class.

-  The name of a type parameter in the *type_parameter_list* of a class declaration shall differ from the names of all other type parameters in the same *type_parameter_list* and shall differ from the name of the class and the names of all members of the class.

-  The name of a type shall differ from the names of all non-type members declared in the same class. If two or more type declarations share the same fully qualified name, the declarations shall have the `partial` modifier ([§15.2.7](classes.md#1527-partial-declarations)) and these declarations combine to define a single type.   
>  *Note*: Since the fully qualified name of a type declaration encodes the number of type parameters, two distinct types may share the  same name as long as they have different number of type parameters. *end note*

-  The name of a constant, field, property, or event shall differ from the names of all other members declared in the same class.

-  The name of a method shall differ from the names of all other non-methods declared in the same class. In addition, the signature ([§8.6](basic-concepts.md#86-signatures-and-overloading)) of a method shall differ from the signatures of all other methods declared in the same class, and two methods declared in the same class shall not have signatures that differ solely by `ref` and `out`.

-  The signature of an instance constructor shall differ from the signatures of all other instance constructors declared in the same class, and two constructors declared in the same class shall not have signatures that differ solely by `ref` and `out`.

-  The signature of an indexer shall differ from the signatures of all other indexers declared in the same class.

-  The signature of an operator shall differ from the signatures of all other operators declared in the same class.

The inherited members of a class ([§15.3.4](classes.md#1534-inheritance)) are not part of the declaration space of a class. 

> *Note*: Thus, a derived class is allowed to declare a member with the same name or signature as an inherited member (which in effect hides the inherited member). *end note*

The set of members of a type declared in multiple parts ([§15.2.7](classes.md#1527-partial-declarations)) is the union of the members declared in each part. The bodies of all parts of the type declaration share the same declaration space ([§8.3](basic-concepts.md#83-declarations)), and the scope of each member ([§8.7](basic-concepts.md#87-scopes)) extends to the bodies of all the parts. The accessibility domain of any member always includes all the parts of the enclosing type; a private member declared in one part is freely accessible from another part. It is a compile-time error to declare the same member in more than one part of the type, unless that member is a type having the `partial` modifier.

> *Example*:
> ```csharp
> partial class A
> {
>     int x;                     // Error, cannot declare x more than once
>     partial class Inner       // Ok, Inner is a partial type
>     {
>         int y;
>     }
> }
> partial class A
> {
>     int x;                   // Error, cannot declare x more than once
>     partial class Inner     // Ok, Inner is a partial type
>     {
>         int z;
>     }
> }
> ```
> *end example*

Field initialization order can be significant within C# code, and some guarantees are provided, as defined in [§15.5.6.1](classes.md#15561-general). Otherwise, the ordering of members within a type is rarely significant, but may be significant when interfacing with other languages and environments. In these cases, the ordering of members within a type declared in multiple parts is undefined.

### 15.3.2 The instance type

Each class declaration has an associated ***instance type***. For a generic class declaration, the instance type is formed by creating a constructed type ([§9.4](types.md#94-constructed-types)) from the type declaration, with each of the supplied type arguments being the corresponding type parameter. Since the instance type uses the type parameters, it can only be used where the type parameters are in scope; that is, inside the class declaration. The instance type is the type of `this` for code written inside the class declaration. For non-generic classes, the instance type is simply the declared class.

> *Example*: The following shows several class declarations along with their instance types:
> ```csharp
> class A<T>             // instance type: A<T>
> {
>     class B {}         // instance type: A<T>.B
>     class C<U> {}      // instance type: A<T>.C<U>
> }
> class D {}             // instance type: D
> ```
> *end example*

### 15.3.3 Members of constructed types

The non-inherited members of a constructed type are obtained by substituting, for each *type_parameter* in the member declaration, the corresponding *type_argument* of the constructed type. The substitution process is based on the semantic meaning of type declarations, and is not simply textual substitution.

> *Example*: Given the generic class declaration
> ```csharp
> class Gen<T,U>
> {
>     public T[,] a;
>     public void G(int i, T t, Gen<U,T> gt) {...}
>     public U Prop { get {...} set {...} }
>     public int H(double d) {...}
> }
> ```
> the constructed type `Gen<int[],IComparable<string>>` has the following members:
> ```csharp
> public int[,][] a;
> public void G(int i, int[] t, Gen<IComparable<string>,int[]> gt) {...}
> public IComparable<string> Prop { get {...} set {...} }
> public int H(double d) {...}
> ```
> The type of the member `a` in the generic class declaration `Gen` is "two-dimensional array of `T`", so the type of the member `a` in the constructed type above is "two-dimensional array of single-dimensional array of `int`", or `int[,][]`. *end example*

Within instance function members, the type of `this` is the instance type ([§15.3.2](classes.md#1532-the-instance-type)) of the containing declaration.

All members of a generic class can use type parameters from any enclosing class, either directly or as part of a constructed type. When a particular closed constructed type ([§9.4.3](types.md#943-open-and-closed-types)) is used at run-time, each use of a type parameter is replaced with the type argument supplied to the constructed type.

> *Example*:
> ```csharp
> class C<V>
> {
>     public V f1;
>     public C<V> f2 = null;
>     public C(V x) {
>         this.f1 = x;
>         this.f2 = this;
>     }
> }
> class Application
> {
>     static void Main() {
>         C<int> x1 = new C<int>(1);
>         Console.WriteLine(x1.f1);         // Prints 1
>         C<double> x2 = new C<double>(3.1415);
>         Console.WriteLine(x2.f1);         // Prints 3.1415
>     }
> }
> ```
> *end example*

### 15.3.4 Inheritance

A class ***inherits*** the members of its direct base class. Inheritance means that a class implicitly contains all members of its direct base class, except for the instance constructors, finalizers, and static constructors of the base class. Some important aspects of inheritance are:

-  Inheritance is transitive. If `C` is derived from `B`, and `B` is derived from `A`, then `C` inherits the members declared in `B` as well as the members declared in `A`.

-  A derived class *extends* its direct base class. A derived class can add new members to those it inherits, but it cannot remove the definition of an inherited member.

-  Instance constructors, finalizers, and static constructors are not inherited, but all other members are, regardless of their declared accessibility ([§8.5](basic-concepts.md#85-member-access)). However, depending on their declared accessibility, inherited members might not be accessible in a derived class.

-  A derived class can *hide* ([§8.7.2.3](basic-concepts.md#8723-hiding-through-inheritance)) inherited members by declaring new members with the same name or signature. However, hiding an inherited member does not remove that member—it merely makes that member inaccessible directly through the derived class.

-  An instance of a class contains a set of all instance fields declared in the class and its base classes, and an implicit conversion ([§11.2.7](conversions.md#1127-implicit-reference-conversions)) exists from a derived class type to any of its base class types. Thus, a reference to an instance of some derived class can be treated as a reference to an instance of any of its base classes.

-  A class can declare virtual methods, properties, indexers, and events, and derived classes can override the implementation of these function members. This enables classes to exhibit polymorphic behavior wherein the actions performed by a function member invocation vary depending on the run-time type of the instance through which that function member is invoked.

The inherited members of a constructed class type are the members of the immediate base class type ([§15.2.4.2](classes.md#15242-base-classes)), which is found by substituting the type arguments of the constructed type for each occurrence of the corresponding type parameters in the *base_class_specification*. These members, in turn, are transformed by substituting, for each *type_parameter* in the member declaration, the corresponding *type_argument* of the *base_class_specification*.

> *Example*:
> ```csharp
> class B<U>
> {
>     public U F(long index) {...}
> }
> class D<T>: B<T[]>
> {
>     public T` G(string s) {...}
> }
> ```
> In the code above, the constructed type `D<int>` has a non-inherited member public `int` `G(string s)` obtained by substituting the type argument `int` for the type parameter `T`. `D<int>` also has an inherited member from the class declaration `B`. This inherited member is determined by first determining the base class type `B<int[]>` of `D<int>` by substituting `int` for `T` in the base class specification `B<T[]>`. Then, as a type argument to `B`, `int[]` is substituted for `U` in `public U F(long index)`, yielding the inherited member `public int[] F(long index)`. *end example*

### 15.3.5 The new modifier

A *class_member_declaration* is permitted to declare a member with the same name or signature as an inherited member. When this occurs, the derived class member is said to *hide* the base class member. See [§8.7.2.3](basic-concepts.md#8723-hiding-through-inheritance) for a precise specification of when a member hides an inherited member.

An inherited member `M` is considered to be ***available*** if `M` is accessible and there is no other inherited accessible member N that already hides `M`. Implicitly hiding an inherited member is not considered an error, but it does cause the compiler to issue a warning unless the declaration of the derived class member includes a `new` modifier to explicitly indicate that the derived member is intended to hide the base member. If one or more parts of a partial declaration ([§15.2.7](classes.md#1527-partial-declarations)) of a nested type include the `new` modifier, no warning is issued if the nested type hides an available inherited member.

If a `new` modifier is included in a declaration that doesn't hide an available inherited member, a warning to that effect is issued.

### 15.3.6 Access modifiers

A *class_member_declaration* can have any one of the five possible kinds of declared accessibility ([§8.5.2](basic-concepts.md#852-declared-accessibility)): `public`, `protected internal`, `protected`, `internal`, or `private`. Except for the `protected internal` combination, it is a compile-time error to specify more than one access modifier. When a *class_member_declaration* does not include any access modifiers, `private` is assumed.

### 15.3.7 Constituent types

Types that are used in the declaration of a member are called the ***constituent types*** of that member. Possible constituent types are the type of a constant, field, property, event, or indexer, the return type of a method or operator, and the parameter types of a method, indexer, operator, or instance constructor. The constituent types of a member shall be at least as accessible as that member itself ([§8.5.5](basic-concepts.md#855-accessibility-constraints)).

### 15.3.8 Static and instance members

Members of a class are either ***static members*** or ***instance members***. 

> *Note*: Generally speaking, it is useful to think of static members as belonging to classes and instance members as belonging to objects (instances of classes). *end note*

When a field, method, property, event, operator, or constructor declaration includes a `static` modifier, it declares a static member. In addition, a constant or type declaration implicitly declares a static member. Static members have the following characteristics:

-  When a static member `M` is referenced in a *member-access* ([§12.7.5](expressions.md#1275-member-access)) of the form `E.M`, `E` shall denote a type that has a member `M`. It is a compile-time error for `E` to denote an instance.
-  A static field in a non-generic class identifies exactly one storage location. No matter how many instances of a non-generic class are created, there is only ever one copy of a static field. Each distinct closed constructed type ([§9.4.3](types.md#943-open-and-closed-types)) has its own set of static fields, regardless of the number of instances of the closed constructed type.
-  A static function member (method, property, event, operator, or constructor) does not operate on a specific instance, and it is a compile-time error to refer to this in such a function member.

When a field, method, property, event, indexer, constructor, or finalizer declaration does not include a static modifier, it declares an instance member. (An instance member is sometimes called a non-static member.) Instance members have the following characteristics:

-  When an instance member `M` is referenced in a *member_access* ([§12.7.5](expressions.md#1275-member-access)) of the form `E.M`, `E` shall denote an instance of a type that has a member `M`. It is a binding-time error for E to denote a type.
-  Every instance of a class contains a separate set of all instance fields of the class.
-  An instance function member (method, property, indexer, instance constructor, or finalizer) operates on a given instance of the class, and this instance can be accessed as `this` ([§12.7.8](expressions.md#1278-this-access)).

> *Example*: The following example illustrates the rules for accessing static and instance members:
> ```csharp
> class Test
> {
>     int x;
>     static int y;
>     void F() {
>         x = 1;               // Ok, same as this.x = 1
>         y = 1;               // Ok, same as Test.y = 1
>     }
>     static void G() {
>         x = 1;               // Error, cannot access this.x
>         y = 1;               // Ok, same as Test.y = 1
>     }
>     static void Main() {
>         Test t = new Test();
>         t.x = 1;             // Ok
>         t.y = 1;             // Error, cannot access static member through instance
>         Test.x = 1;          // Error, cannot access instance member through type
>         Test.y = 1;          // Ok
>     }
> }
> ```
> The `F` method shows that in an instance function member, a *simple_name* ([§12.7.3](expressions.md#1273-simple-names)) can be used to access both instance members and static members. The `G` method shows that in a static function member, it is a compile-time error to access an instance member through a *simple_name*. The `Main` method shows that in a *member_access* ([§12.7.5](expressions.md#1275-member-access)), instance members shall be accessed through instances, and static members shall be accessed through types. *end example*

### 15.3.9 Nested types

#### 15.3.9.1 General

A type declared within a class or struct is called a ***nested type***. A type that is declared within a compilation unit or namespace is called a ***non-nested type***.

> *Example*: In the following example:
> ```csharp
> using System;
> class A
> {
>     class B
>     {
>         static void F() {
>             Console.WriteLine("A.B.F");
>         }
>     }
> }
> ```
> class `B` is a nested type because it is declared within class `A`, and class `A` is a non-nested type because it is declared within a compilation unit. *end example*

#### 15.3.9.2 Fully qualified name

The fully qualified name ([§8.8.3](basic-concepts.md#883-fully-qualified-names)) for a nested type declarationis `S.N` where `S` is the fully qualified name of the type declarationin which type `N` is declared and `N` is the unqualified name ([§8.8.2](basic-concepts.md#882-unqualified-names)) of the nested type declaration (including any *generic_dimension_specifier* ([§12.7.12](expressions.md#12712-the-typeof-operator))).

#### 15.3.9.3 Declared accessibility

Non-nested types can have `public` or `internal` declared accessibility and have `internal` declared accessibility by default. Nested types can have these forms of declared accessibility too, plus one or more additional forms of declared accessibility, depending on whether the containing type is a class or struct:

-  A nested type that is declared in a class can have any of five forms of declared accessibility (`public`, `protected` `internal`, `protected`, `internal`, or `private`) and, like other class members, defaults to `private` declared accessibility.

-  A nested type that is declared in a struct can have any of three forms of declared accessibility (`public`, `internal`, or `private`) and, like other struct members, defaults to `private` declared accessibility.

> *Example*: The example
> ```csharp
> public class List
> {
>     // Private data structure
>     private class Node
>     {
>         public object Data;
>         public Node Next;
>         public Node(object data, Node next) {
>         this.Data = data;
>         this.Next = next;
>         }
>     }
>     private Node first = null;
>     private Node last = null;
>     // Public interface
>     public void AddToFront(object o) {...}
>     public void AddToBack(object o) {...}
>     public object RemoveFromFront() {...}
>     public object RemoveFromBack() {...}
>     public int Count { get {...} }
> }
> ```
> declares a private nested class `Node`. *end example*

#### 15.3.9.4 Hiding

A nested type may hide ([§8.7.2.2](basic-concepts.md#8722-hiding-through-nesting)) a base member. The `new` modifier ([§15.3.5](classes.md#1535-the-new-modifier)) is permitted on nested type declarations so that hiding can be expressed explicitly.

> *Example*: The example
> ```csharp
> using System;
> class Base
> {
>     public static void M() {
>         Console.WriteLine("Base.M");
>     }
> }
> class Derived: Base
> {
>     new public class M
>     {
>         public static void F() {
>             Console.WriteLine("Derived.M.F");
>         }
>     }
> }
> class Test
> {
>     static void Main() {
>         Derived.M.F();
>     }
> }
> ```
> shows a nested class `M` that hides the method `M` defined in `Base`. *end example*

#### 15.3.9.5 this access

A nested type and its containing type do not have a special relationship with regard to *this_access* ([§12.7.8](expressions.md#1278-this-access)). Specifically, `this` within a nested type cannot be used to refer to instance members of the containing type. In cases where a nested type needs access to the instance members of its containing type, access can be provided by providing the `this` for the instance of the containing type as a constructor argument for the nested type.

> *Example*: The following example
> ```csharp
> using System;
> class C
> {
>     int i = 123;
>     public void F() {
>         Nested n = new Nested(this);
>         n.G();
>     }
>     public class Nested
>     {
>         C this_c;
>         public Nested(C c) {
>         this_c = c;
>         }
>         public void G() {
>         Console.WriteLine(this_c.i);
>         }
>     }
> }
> class Test
> {
>     static void Main() {
>         C c = new C();
>         c.F();
>     }
> }
> ```
> shows this technique. An instance of `C` creates an instance of `Nested`, and passes its own this to `Nested`'s constructor in order to provide subsequent access to `C`'s instance members. *end example*

#### 15.3.9.6 Access to private and protected members of the containing type

A nested type has access to all of the members that are accessible to its containing type, including members of the containing type that have `private` and `protected` declared accessibility.

> *Example*: The example
> ```csharp
> using System;
> class C
> {
>     private static void F() {
>         Console.WriteLine("C.F");
>     }
>     public class Nested
>     {
>         public static void G() {
>             F();
>         }
>     }
> }
> class Test
> {
>     static void Main() {
>         C.Nested.G();
>     }
> }
> ```
> shows a class `C` that contains a nested class `Nested`. Within `Nested`, the method `G` calls the static method `F` defined in `C`, and `F` has private declared accessibility. *end example*

A nested type also may access protected members defined in a base type of its containing type.

> *Example*: In the following code
> ```csharp
> using System;
> class Base
> {
>     protected void F() {
>         Console.WriteLine("Base.F");
>     }
> }
> class Derived: Base
> {
>     public class Nested
>     {
>         public void G() {
>             Derived d = new Derived();
>             d.F(); // ok
>         }
>     }
> }
> class Test
> {
>     static void Main() {
>         Derived.Nested n = new Derived.Nested();
>         n.G();
>     }
> }
> ```
> the nested class `Derived.Nested` accesses the protected method `F` defined in `Derived`'s base class, `Base`, by calling through an instance of `Derived`. *end example*

#### 15.3.9.7 Nested types in generic classes

A generic class declaration may contain nested type declarations. The type parameters of the enclosing class may be used within the nested types. A nested type declaration may contain additional type parameters that apply only to the nested type.

Every type declaration contained within a generic class declaration is implicitly a generic type declaration. When writing a reference to a type nested within a generic type, the containing constructed type, including its type arguments, shall be named. However, from within the outer class, the nested type may be used without qualification; the instance type of the outer class may be implicitly used when constructing the nested type.

> *Example*: The following shows three different correct ways to refer to a constructed type created from `Inner`; the first two are equivalent:
> ```csharp
> class Outer<T>
> {
>     class Inner<U>
>     {
>         public static void F(T t, U u) {...}
>     }
>     static void F(T t) {
>         Outer<T>.Inner<string>.F(t, "abc");         // These two statements have
>         Inner<string>.F(t, "abc");                  // the same effect
>         Outer<int>.Inner<string>.F(3, "abc");       // This type is different
>         Outer.Inner<string>.F(t, "abc");            // Error, Outer needs type arg
>     }
> }
> ```
> *end example*

Although it is bad programming style, a type parameter in a nested type can hide a member or type parameter declared in the outer type.

> *Example*:
> ```csharp
> class Outer<T>
> {
>     class Inner<T>                                 // Valid, hides Outer's T
>     {
>         public T t;                                 // Refers to Inner's T
>     }
> }
> ```
> *end example*

### 15.3.10 Reserved member names

#### 15.3.10.1 General

To facilitate the underlying C# run-time implementation, for each source member declaration that is a property, event, or indexer, the implementation shall reserve two method signatures based on the kind of the member declaration, its name, and its type ([§15.3.10.2](classes.md#153102-member-names-reserved-for-properties), [§15.3.10.3](classes.md#153103-member-names-reserved-for-events), [§15.3.10.4](classes.md#153104-member-names-reserved-for-indexers)). It is a compile-time error for a program to declare a member whose signature matches a signature reserved by a member declared in the same scope, even if the underlying run-time implementation does not make use of these reservations.

The reserved names do not introduce declarations, thus they do not participate in member lookup. However, a declaration's associated reserved method signatures do participate in inheritance ([§15.3.4](classes.md#1534-inheritance)), and can be hidden with the `new` modifier ([§15.3.5](classes.md#1535-the-new-modifier)).

> *Note*: The reservation of these names serves three purposes:
> 1.  To allow the underlying implementation to use an ordinary identifier as a method name for get or set access to the C# language feature.
> 2.  To allow other languages to interoperate using an ordinary identifier as a method name for get or set access to the C# language feature.
> 3.  To help ensure that the source accepted by one conforming compiler is accepted by another, by making the specifics of reserved member names consistent across all C# implementations.
> *end note*

The declaration of a finalizer ([§15.13](classes.md#1513-finalizers)) also causes a signature to be reserved ([§15.3.10.5](classes.md#153105-member-names-reserved-for-finalizers)).

#### 15.3.10.2 Member names reserved for properties

For a property `P` ([§15.7](classes.md#157-properties)) of type `T`, the following signatures are reserved:

```csharp
T get_P();
void set_P(T value);
```
Both signatures are reserved, even if the property is read-only or write-only.

> *Example*: In the following code
> ```csharp
> using System;
> class A
> {
>     public int P {
>         get { return 123; }
>     }
> }
> class B: A
> {
>     new public int get_P() {
>         return 456;
>     }
>     new public void set_P(int value) {
>     }
> }
> class Test
> {
>     static void Main() {
>     B b = new B();
>     A a = b;
>     Console.WriteLine(a.P);
>     Console.WriteLine(b.P);
>     Console.WriteLine(b.get_P());
>     }
> }
> ```
> A class `A` defines a read-only property `P`, thus reserving signatures for `get_P` and `set_P` methods. `A` class `B` derives from `A` and hides both of these reserved signatures. The example produces the output:
> ```console
> 123
> 123
> 456
> ```
> *end example*

#### 15.3.10.3 Member names reserved for events

For an event `E` ([§15.8](classes.md#158-events)) of delegate type `T`, the following signatures are reserved:

```csharp
void add_E(T handler);
void remove_E(T handler);
```

#### 15.3.10.4 Member names reserved for indexers

For an indexer ([§15.9](classes.md#159-indexers)) of type `T` with parameter-list `L`, the following signatures are reserved:

```csharp
T get_Item(L);
void set_Item(L, T value);
```

Both signatures are reserved, even if the indexer is read-only or write-only.

Furthermore the member name `Item` is reserved.

#### 15.3.10.5 Member names reserved for finalizers

For a class containing a finalizer ([§15.13](classes.md#1513-finalizers)), the following signature is reserved:

```csharp
void Finalize();
```

## 15.4 Constants

A ***constant*** is a class member that represents a constant value: a value that can be computed at compile-time. A *constant_declaration* introduces one or more constants of a given type.

```ANTLR
constant_declaration
    : attributes? constant_modifier* 'const' type constant_declarators ';'
    ;

constant_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    ;
```

A *constant_declaration* may include a set of *attributes* ([§22](attributes.md#22-attributes)), a `new` modifier ([§15.3.5](classes.md#1535-the-new-modifier)), and a valid combination of the four access modifiers ([§15.3.6](classes.md#1536-access-modifiers)). The attributes and modifiers apply to all of the members declared by the *constant_declaration*. Even though constants are considered static members, a *constant_declaration* neither requires nor allows a `static` modifier. It is an error for the same modifier to appear multiple times in a constant declaration.

The *type* of a *constant_declaration* specifies the type of the members introduced by the declaration. The type is followed by a list of *constant_declarator*s ([§13.6.3](statements.md#1363-local-constant-declarations)), each of which introduces a new member. A *constant_declarator* consists of an *Identifier* that names the member, followed by an "`=`" token, followed by a *constant_expression* ([§12.20](expressions.md#1220-constant-expressions)) that gives the value of the member.

The *type* specified in a constant declaration shall be `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `float`, `double`, `decimal`, `bool`, `string`, an *enum_type*, or a *reference_type*. Each *constant_expression* shall yield a value of the target type or of a type that can be converted to the target type by an implicit conversion ([§11.2](conversions.md#112-implicit-conversions)).

The *type* of a constant shall be at least as accessible as the constant itself ([§8.5.5](basic-concepts.md#855-accessibility-constraints)).

The value of a constant is obtained in an expression using a *simple_name* ([§12.7.3](expressions.md#1273-simple-names)) or a *member_access* ([§12.7.5](expressions.md#1275-member-access)).

A constant can itself participate in a *constant_expression*. Thus, a constant may be used in any construct that requires a *constant_expression*. 

> *Note*: Examples of such constructs include `case` labels, `goto case` statements, `enum` member declarations, attributes, and other constant declarations. *end note*

> *Note*: As described in [§12.20](expressions.md#1220-constant-expressions), a *constant_expression* is an expression that can be fully evaluated at compile-time. Since the only way to create a non-null value of a *reference_type* other than `string` is to apply the `new` operator, and since the `new` operator is not permitted in a *constant_expression*, the only possible value for constants of *reference_type*s other than `string` is `null`. *end note*

When a symbolic name for a constant value is desired, but when the type of that value is not permitted in a constant declaration, or when the value cannot be computed at compile-time by a *constant_expression*, a readonly field ([§15.5.3](classes.md#1553-readonly-fields)) may be used instead. 

> *Note*: The versioning semantics of `const` and `readonly` differ ([§15.5.3.3](classes.md#15533-versioning-of-constants-and-static-readonly-fields)). *end note*

A constant declaration that declares multiple constants is equivalent to multiple declarations of single constants with the same attributes, modifiers, and type.

> *Example*:
> ```csharp
> class A
> {
>     public const double X = 1.0, Y = 2.0, Z = 3.0;
> }
> ```
> is equivalent to
> ```csharp
> class A
> {
>     public const double X = 1.0;
>     public const double Y = 2.0;
>     public const double Z = 3.0;
> }
> ```
> *end example*

Constants are permitted to depend on other constants within the same program as long as the dependencies are not of a circular nature. The compiler automatically arranges to evaluate the constant declarations in the appropriate order.

> *Example*: In the following code
> ```csharp
> class A
> {
>     public const int X = B.Z + 1;
>     public const int Y = 10;
> }
> class B
> {
>     public const int Z = A.Y + 1;
> }
> ```
> the compiler first evaluates `A.Y`, then evaluates `B.Z`, and finally evaluates `A.X`, producing the values `10`, `11`, and `12`. *end example* 

Constant declarations may depend on constants from other programs, but such dependencies are only possible in one direction.

> *Example*: Referring to the example above, if `A` and `B` were declared in separate programs, it would be possible for `A.X` to depend on `B.Z`, but `B.Z` could then not simultaneously depend on `A.Y`. *end example*

## 15.5 Fields

### 15.5.1 General

A ***field*** is a member that represents a variable associated with an object or class. A *field_declaration* introduces one or more fields of a given type.

```ANTLR
field_declaration
    : attributes? field_modifier* type variable_declarators ';'
    ;

field_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | 'static'
    | 'readonly'
    | 'volatile'
    | unsafe_modifier   // unsafe code support
    ;

variable_declarators
    : variable_declarator (',' variable_declarator)*
    ;

variable_declarator
    : Identifier ('=' variable_initializer)?
    ;
```

*unsafe_modifier* ([§23.2](unsafe-code.md#232-unsafe-contexts)) is only available in unsafe code ([§23](unsafe-code.md#23-unsafe-code)).

A *field_declaration* may include a set of *attributes* ([§22](attributes.md#22-attributes)), a `new` modifier ([§15.3.5](classes.md#1535-the-new-modifier)), a valid combination of the four access modifiers ([§15.3.6](classes.md#1536-access-modifiers)), and a `static` modifier ([§15.5.2](classes.md#1552-static-and-instance-fields)). In addition, a *field_declaration* may include a `readonly` modifier ([§15.5.3](classes.md#1553-readonly-fields)) or a `volatile` modifier ([§15.5.4](classes.md#1554-volatile-fields)), but not both. The attributes and modifiers apply to all of the members declared by the *field_declaration*. It is an error for the same modifier to appear multiple times in a *field_declaration*.

The *type* of a *field_declaration* specifies the type of the members introduced by the declaration. The type is followed by a list of *variable_declarator*s, each of which introduces a new member. A *variable_declarator* consists of an *Identifier* that names that member, optionally followed by an "`=`" token and a *variable_initializer* ([§15.5.6](classes.md#1556-variable-initializers)) that gives the initial value of that member.

The *type* of a field shall be at least as accessible as the field itself ([§8.5.5](basic-concepts.md#855-accessibility-constraints)).

The value of a field is obtained in an expression using a *simple_name* ([§12.7.3](expressions.md#1273-simple-names)), a *member_access* ([§12.7.5](expressions.md#1275-member-access)) or a base_access ([§12.7.9](expressions.md#1279-base-access)). The value of a non-readonly field is modified using an *assignment* ([§12.18](expressions.md#1218-assignment-operators)). The value of a non-readonly field can be both obtained and modified using postfix increment and decrement operators ([§12.7.10](expressions.md#12710-postfix-increment-and-decrement-operators)) and prefix increment and decrement operators ([§12.8.6](expressions.md#1286-prefix-increment-and-decrement-operators)).

A field declaration that declares multiple fields is equivalent to multiple declarations of single fields with the same attributes, modifiers, and type.

> *Example*:
> ```csharp
> class A
> {
>     public static int X = 1, Y, Z = 100;
> }
> ```
> is equivalent to
> ```csharp
> class A
> {
>     public static int X = 1;
>     public static int Y;
>     public static int Z = 100;
> }
> ```
> *end example*

### 15.5.2 Static and instance fields

When a field declaration includes a `static` modifier, the fields introduced by the declaration are ***static fields***. When no `static` modifier is present, the fields introduced by the declaration are ***instance fields***. Static fields and instance fields are two of the several kinds of variables ([§10](variables.md#10-variables)) supported by C#, and at times they are referred to as ***static variables*** and ***instance variables***, respectively.

As explained in [§15.3.8](classes.md#1538-static-and-instance-members), each instance of a class contains a complete set of the instance fields of the class, while there is only one set of static fields for each non-generic class or closed constructed type, regardless of the number of instances of the class or closed constructed type.

### 15.5.3 Readonly fields

#### 15.5.3.1 General

When a *field_declaration* includes a `readonly` modifier, the fields introduced by the declaration are ***readonly fields***. Direct assignments to readonly fields can only occur as part of that declaration or in an instance constructor or static constructor in the same class. (A readonly field can be assigned to multiple times in these contexts.) Specifically, direct assignments to a readonly field are permitted only in the following contexts:

-  In the *variable_declarator* that introduces the field (by including a *variable_initializer* in the declaration).
-  For an instance field, in the instance constructors of the class that contains the field declaration; for a static field, in the static constructor of the class that contains the field declaration. These are also the only contexts in which it is valid to pass a readonly field as an `out` or `ref` parameter.

Attempting to assign to a readonly field or pass it as an `out` or `ref` parameter in any other context is a compile-time error.

#### 15.5.3.2 Using static readonly fields for constants

A static readonly field is useful when a symbolic name for a constant value is desired, but when the type of the value is not permitted in a const declaration, or when the value cannot be computed at compile-time.

> *Example*: In the following code
> ```csharp
> public class Color
> {
>     public static readonly Color Black = new Color(0, 0, 0);
>     public static readonly Color White = new Color(255, 255, 255);
>     public static readonly Color Red = new Color(255, 0, 0);
>     public static readonly Color Green = new Color(0, 255, 0);
>     public static readonly Color Blue = new Color(0, 0, 255);
>     private byte red, green, blue;
>     public Color(byte r, byte g, byte b) {
>         red = r;
>         green = g;
>         blue = b;
>     }
> }
> ```
> the `Black`, `White`, `Red`, `Green`, and `Blue` members cannot be declared as const members because their values cannot be computed at compile-time. However, declaring them `static readonly` instead has much the same effect. *end example*

#### 15.5.3.3 Versioning of constants and static readonly fields

Constants and readonly fields have different binary versioning semantics. When an expression references a constant, the value of the constant is obtained at compile-time, but when an expression references a readonly field, the value of the field is not obtained until run-time.

> *Example*: Consider an application that consists of two separate programs:
> ```csharp
> namespace Program1
>     {
>     public class Utils
>     {
>         public static readonly int x = 1;
>     }
> }
> ```
> and
> ```csharp
> using System;
> namespace Program2
> {
>     class Test
>     {
>         static void Main() {
>             Console.WriteLine(Program1.Utils.X);
>         }
>     }
> }
> ```
> The `Program1` and `Program2` namespaces denote two programs that are compiled separately. Because `Program1.Utils.X` is declared as a `static readonly` field, the value output by the `Console.WriteLine` statement is not known at compile-time, but rather is obtained at run-time. Thus, if the value of `X` is changed and `Program1` is recompiled, the `Console.WriteLine` statement will output the new value even if `Program2` isn't recompiled. However, had `X` been a constant, the value of `X` would have been obtained at the time `Program2` was compiled, and would remain unaffected by changes in `Program1` until `Program2` is recompiled. *end example*

### 15.5.4 Volatile fields

When a *field_declaration* includes a `volatile` modifier, the fields introduced by that declaration are ***volatile fields***. For non-volatile fields, optimization techniques that reorder instructions can lead to unexpected and unpredictable results in multi-threaded programs that access fields without synchronization such as that provided by the *lock_statement* ([§13.13](statements.md#1313-the-lock-statement)). These optimizations can be performed by the compiler, by the run-time system, or by hardware. For volatile fields, such reordering optimizations are restricted:

-  A read of a volatile field is called a ***volatile read***. A volatile read has "acquire semantics"; that is, it is guaranteed to occur prior to any references to memory that occur after it in the instruction sequence.
-  A write of a volatile field is called a ***volatile write***. A volatile write has "release semantics"; that is, it is guaranteed to happen after any memory references prior to the write instruction in the instruction sequence.

These restrictions ensure that all threads will observe volatile writes performed by any other thread in the order in which they were performed. A conforming implementation is not required to provide a single total ordering of volatile writes as seen from all threads of execution. The type of a volatile field shall be one of the following:

-  A *reference_type*.
-  A *type_parameter* that is known to be a reference type ([§15.2.5](classes.md#1525-type-parameter-constraints)).
-  The type `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `char`, `float`, `bool`, `System.IntPtr`, or `System.UIntPtr`.
-  An *enum_type* having an *enum_base* type of `byte`, `sbyte`, `short`, `ushort`, `int`, or `uint`.

> *Example*: The example
> ```csharp
> using System;
> using System.Threading;
> class Test
> {
>     public static int result;
>     public static volatile bool finished;
>     static void Thread2() {
>         result = 143;
>         finished = true;
>     }
>     static void Main() {
>         finished = false;                                // Run Thread2() in a new thread
>         new Thread(new ThreadStart(Thread2)).Start();    // Wait for Thread2 to signal that it has a result by setting
>                                                          // finished to true.
>         for (;;) {
>             if (finished) {
>                 Console.WriteLine("result = {0}", result);
>                 return;
>             }
>         }
>     }
> }
> ```
> produces the output:
> ```console
> result = 143
> ```
> In this example, the method `Main` starts a new thread that runs the method `Thread2`. This method stores a value into a non-volatile field called `result`, then stores `true` in the volatile field `finished`. The main thread waits for the field `finished` to be set to `true`, then reads the field `result`. Since `finished` has been declared `volatile`, the main thread shall read the value `143` from the field `result`. If the field `finished` had not been declared `volatile`, then it would be permissible for the store to `result` to be visible to the main thread *after* the store to `finished`, and hence for the main thread to read the value 0 from the field `result`. Declaring `finished` as a `volatile` field prevents any such inconsistency. *end example*

### 15.5.5 Field initialization

The initial value of a field, whether it be a static field or an instance field, is the default value ([§10.3](variables.md#103-default-values)) of the field's type. It is not possible to observe the value of a field before this default initialization has occurred, and a field is thus never "uninitialized".

> *Example*: The example
> ```csharp
> using System;
> class Test
> {
>     static bool b;
>     int i;
>     static void Main() {
>         Test t = new Test();
>         Console.WriteLine("b = {0}, i = {1}", b, t.i);
>     }
> }
> ```
> produces the output
> ```console
> b = False, i = 0
> ```
> because `b` and `i` are both automatically initialized to default values. *end example*

### 15.5.6 Variable initializers

#### 15.5.6.1 General

Field declarations may include *variable_initializer*s. For static fields, variable initializers correspond to assignment statements that are executed during class initialization. For instance fields, variable initializers correspond to assignment statements that are executed when an instance of the class is created.

> *Example*: The example
> ```csharp
> using System;
> class Test
> {
>     static double x = Math.Sqrt(2.0);
>     int i = 100;
>     string s = "Hello";
>     static void Main() {
>         Test a = new Test();
>         Console.WriteLine("x = {0}, i = {1}, s = {2}", x, a.i, a.s);
>     }
> }
> ```
> produces the output
> ```console
> x = 1.4142135623731, i = 100, s = Hello
> ```
> because an assignment to `x` occurs when static field initializers execute and assignments to `i` and `s` occur when the instance field initializers execute. *end example*

The default value initialization described in [§15.5.5](classes.md#1555-field-initialization) occurs for all fields, including fields that have variable initializers. Thus, when a class is initialized, all static fields in that class are first initialized to their default values, and then the static field initializers are executed in textual order. Likewise, when an instance of a class is created, all instance fields in that instance are first initialized to their default values, and then the instance field initializers are executed in textual order. When there are field declarations in multiple partial type declarations for the same type, the order of the parts is unspecified. However, within each part the field initializers are executed in order.

It is possible for static fields with variable initializers to be observed in their default value state.

> *Example*: However, this is strongly discouraged as a matter of style. The example
> ```csharp
> using System;
> class Test
> {
>     static int a = b + 1;
>     static int b = a + 1;
>     static void Main() {
>         Console.WriteLine("a = {0}, b = {1}", a, b);
>     }
> }
> ```
> exhibits this behavior. Despite the circular definitions of `a` and `b`, the program is valid. It results in the output
> ```console
> a = 1, b = 2
> ```
> because the static fields `a` and `b` are initialized to `0` (the default value for `int`) before their initializers are executed. When the initializer for `a` runs, the value of `b` is zero, and so `a` is initialized to `1`. When the initializer for `b` runs, the value of a is already `1`, and so `b` is initialized to `2`. *end example*

#### 15.5.6.2 Static field initialization

The static field variable initializers of a class correspond to a sequence of assignments that are executed in the textual order in which they appear in the class declaration ([§15.5.6.1](classes.md#15561-general)). Within a partial class, the meaning of "textual order" is specified by [§15.5.6.1](classes.md#15561-general). If a static constructor ([§15.12](classes.md#1512-static-constructors)) exists in the class, execution of the static field initializers occurs immediately prior to executing that static constructor. Otherwise, the static field initializers are executed at an implementation-dependent time prior to the first use of a static field of that class.

> *Example*: The example
> ```csharp
> using System;
> class Test
> {
>     static void Main() {
>     Console.WriteLine("{0} {1}", B.Y, A.X);
> }
> public static int F(string s) {
>     Console.WriteLine(s);
>         return 1;
>     }
> }
> class A
> {
>     public static int X = Test.F("Init A");
> }
> class B
> {
>     public static int Y = Test.F("Init B");
> }
> ```
> might produce either the output:
> ```console
> Init A
> Init B
> 1 1
> ```
> or the output:
> ```console
> Init B
> Init A
> 1 1
> ```
> because the execution of `X`'s initializer and `Y`'s initializer could occur in either order; they are only constrained to occur before the references to those fields. However, in the example:
> ```csharp
> using System;
> class Test
> {
>     static void Main() {
>         Console.WriteLine("{0} {1}", B.Y, A.X);
>     }
>     public static int F(string s) {
>         Console.WriteLine(s);
>         return 1;
>     }
> }
> class A
> {
>     static A() {}
>     public static int X = Test.F("Init A");
> }
> class B
> {
>     static B() {}
>     public static int Y = Test.F("Init B");
> }
> ```
> the output shall be:
> ```csharp
> Init B
> Init A
> 1 1
> ```
> because the rules for when static constructors execute (as defined in [§15.12](classes.md#1512-static-constructors)) provide that `B`'s static constructor (and hence `B`'s static field initializers) shall run before `A`'s static constructor and field initializers. *end example*

#### 15.5.6.3 Instance field initialization

The instance field variable initializers of a class correspond to a sequence of assignments that are executed immediately upon entry to any one of the instance constructors ([§15.11.3](classes.md#15113-instance-variable-initializers)) of that class. Within a partial class, the meaning of "textual order" is specified by [§15.5.6.1](classes.md#15561-general). The variable initializers are executed in the textual order in which they appear in the class declaration ([§15.5.6.1](classes.md#15561-general)). The class instance creation and initialization process is described further in [§15.11](classes.md#1511-instance-constructors).

A variable initializer for an instance field cannot reference the instance being created. Thus, it is a compile-time error to reference `this` in a variable initializer, as it is a compile-time error for a variable initializer to reference any instance member through a *simple_name*.

> *Example*: In the following code
> ```csharp
> class A
> {
>     int x = 1;
>     int y = x + 1;     // Error, reference to instance member of this
> }
> ```
> the variable initializer for `y` results in a compile-time error because it references a member of the instance being created. *end example*

## 15.6 Methods

### 15.6.1 General

A ***method*** is a member that implements a computation or action that can be performed by an `object` or class. Methods are declared using *method_declaration*s:
```ANTLR
method_declaration
    : method_header method_body
    ;

method_header
    : attributes? method_modifier* 'partial'? return_type member_name type_parameter_list? '(' formal_parameter_list? ')' type_parameter_constraints_clause*
    ;

method_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | 'static'
    | 'virtual'
    | 'sealed'
    | 'override'
    | 'abstract'
    | 'extern'
    | 'async'
    | unsafe_modifier   // unsafe code support
    ;

return_type
    : type
    | 'void'
    ;

member_name
    : Identifier
    | interface_type '.' Identifier
    ;

method_body
    : block
    | '=>' expression ';'
    | ';'
    ;
```

*unsafe_modifier* ([§23.2](unsafe-code.md#232-unsafe-contexts)) is only available in unsafe code ([§23](unsafe-code.md#23-unsafe-code)).

A *method_declaration* may include a set of *attributes* ([§22](attributes.md#22-attributes)) and a valid combination of the four access modifiers ([§15.3.6](classes.md#1536-access-modifiers)), the `new` ([§15.3.5](classes.md#1535-the-new-modifier)), `static` ([§15.6.3](classes.md#1563-static-and-instance-methods)), `virtual` ([§15.6.4](classes.md#1564-virtual-methods)), `override` ([§15.6.5](classes.md#1565-override-methods)), `sealed` ([§15.6.6](classes.md#1566-sealed-methods)), `abstract` ([§15.6.7](classes.md#1567-abstract-methods)), `extern` ([§15.6.8](classes.md#1568-external-methods)) and `async` ([§15.15](classes.md#1515-async-functions)) modifiers.

A declaration has a valid combination of modifiers if all of the following are true:

-  The declaration includes a valid combination of access modifiers ([§15.3.6](classes.md#1536-access-modifiers)).
-  The declaration does not include the same modifier multiple times.
-  The declaration includes at most one of the following modifiers: `static`, `virtual`, and `override`.
-  The declaration includes at most one of the following modifiers: `new` and `override`.
-  If the declaration includes the `abstract` modifier, then the declaration does not include any of the following modifiers: `static`, `virtual`, `sealed`, or `extern`.
-  If the declaration includes the `private` modifier, then the declaration does not include any of the following modifiers: `virtual`, `override`, or `abstract`.
-  If the declaration includes the `sealed` modifier, then the declaration also includes the `override` modifier.
-  If the declaration includes the `partial` modifier, then it does not include any of the following modifiers: new, `public`, `protected`, `internal`, `private`, `virtual`, `sealed`, `override`, `abstract`, or `extern`.

The *return_type* of a method declaration specifies the type of the value computed and returned by the method. The *return_type* is `void` if the method does not return a value. If the declaration includes the `partial` modifier, then the return type shall be `void` ([§15.6.9](classes.md#1569-partial-methods)). If the declaration includes the `async` modifier then the return type shall be `void` or a *task type* ([§15.15.1](classes.md#15151-general)).

A generic method is a method whose declaration includes a *type_parameter_list*. This specifies the type parameters for the method. The optional *type_parameter_constraints_clause*s specify the constraints for the type parameters. A *method_declaration* shall not have *type_parameter_constraints_clauses* unless it also has a *type_parameter_list*. A *method_declaration* for an explicit interface member implementation shall not have any *type_parameter_constraints_clause*s. A generic *method_declaration* for an explicit interface member implementation inherits any constraints from the constraints on the interface method. Similarly, a method declaration with the `override` modifier shall not have any *type_parameter_constraints_clause*s and the constraints of the method's type parameters are inherited from the virtual method being overridden.The *member_name* specifies the name of the method. Unless the method is an explicit interface member implementation ([§18.6.2](interfaces.md#1862-explicit-interface-member-implementations)), the *member_name* is simply an *Identifier*. For an explicit interface member implementation, the *member_name* consists of an *interface_type* followed by a "`.`" and an *Identifier*. In this case, the declaration shall include no modifiers other than (possibly) `extern` or `async`.

The optional *formal_parameter_list* specifies the parameters of the method ([§15.6.2](classes.md#1562-method-parameters)).

The *return_type* and each of the types referenced in the *formal_parameter_list* of a method shall be at least as accessible as the method itself ([§8.5.5](basic-concepts.md#855-accessibility-constraints)).

The *method_body* is either a semicolon, a ***block body*** or an ***expression body***. A block body consists of a *block*, which specifies the statements to execute when the method is invoked. An expression body consists of `=>` followed by an *expression* and a semicolon, and denotes a single expression to perform when the method is invoked.

For abstract and extern methods, the *method_body* consists simply of a semicolon. For partial methods the *method_body* may consist of either a semicolon, a block body or an expression body. For all other methods, the *method_body* is either a block body or an expression body.

If the *method_body* consists of a semicolon, the declaration shall not include the `async` modifier.

The name, the number of type parameters, and the formal parameter list of a method define the signature ([§8.6](basic-concepts.md#86-signatures-and-overloading)) of the method. Specifically, the signature of a method consists of its name, the number of its type parameters, and the number, *parameter_mode_modifier*s ([§15.6.2.1](classes.md#15621-general)), and types of its formal parameters. The return type is not part of a method's signature, nor are the names of the formal parameters, the names of the type parameters, or the constraints. When a formal parameter type references a type parameter of the method, the ordinal position of the type parameter (not the name of the type parameter) is used for type equivalence.

The name of a method shall differ from the names of all other non-methods declared in the same class. In addition, the signature of a method shall differ from the signatures of all other methods declared in the same class, and two methods declared in the same class may not have signatures that differ solely by `ref` and `out`.

The method's *type_parameter*s are in scope throughout the *method_declaration*, and can be used to form types throughout that scope in *return_type*, *method_body*, and *type_parameter_constraints_clause*s but not in *attributes*.

All formal parameters and type parameters shall have different names.

### 15.6.2 Method parameters

#### 15.6.2.1 General

The parameters of a method, if any, are declared by the method's *formal_parameter_list*.

```ANTLR
formal_parameter_list
    : fixed_parameters
    | fixed_parameters ',' parameter_array
    | parameter_array
    ;

fixed_parameters
    : fixed_parameter (',' fixed_parameter)*
    ;

fixed_parameter
    : attributes? parameter_modifier? type Identifier default_argument?
    ;

default_argument
    : '=' expression
    ;

parameter_modifier
    : parameter_mode_modifier
    | 'this'
    ;

parameter_mode_modifier
    : 'ref'
    | 'out'
    ;

parameter_array
    : attributes? 'params' array_type Identifier
    ;
```

The formal parameter list consists of one or more comma-separated parameters of which only the last may be a *parameter_array*.

A *fixed_parameter* consists of an optional set of *attributes* ([§22](attributes.md#22-attributes)); an optional `ref`, `out`, or `this` modifier; a *type*; an *Identifier*; and an optional *default-argument*. Each *fixed_parameter* declares a parameter of the given type with the given name. The `this` modifier designates the method as an extension method and is only allowed on the first parameter of a static method in a non-generic, non-nested static class. Extension methods are further described in [§15.6.10](classes.md#15610-extension-methods). A *fixed_parameter* with a *default_argument* is known as an ***optional parameter***, whereas a *fixed_parameter* without a *default_argument* is a ***required parameter***. A required parameter may not appear after an optional parameter in a *formal_parameter_list*.

A parameter with a `ref`, `out` or `this` modifier cannot have a *default_argument*. The *expression* in a *default_argument* shall be one of the following:

-  a *constant_expression*
-  an expression of the form `new S()` where `S` is a value type
-  an expression of the form `default(S)` where `S` is a value type

The *expression* shall be implicitly convertible by an identity or nullable conversion to the type of the parameter.

If optional parameters occur in an implementing partial method declaration ([§15.6.9](classes.md#1569-partial-methods)), an explicit interface member implementation ([§18.6.2](interfaces.md#1862-explicit-interface-member-implementations)), a single-parameter indexer declaration ([§15.9](classes.md#159-indexers)), or in an operator declaration ([§15.10.1](classes.md#15101-general)) the compiler should give a warning, since these members can never be invoked in a way that permits arguments to be omitted.

A *parameter_array* consists of an optional set of *attributes* ([§22](attributes.md#22-attributes)), a `params` modifier, an *array_type*, and an *Identifier*. A parameter array declares a single parameter of the given array type with the given name. The *array_type* of a parameter array shall be a single-dimensional array type ([§17.2](arrays.md#172-array-types)). In a method invocation, a parameter array permits either a single argument of the given array type to be specified, or it permits zero or more arguments of the array element type to be specified. Parameter arrays are described further in [§15.6.2.5](classes.md#15625-parameter-arrays).

A *parameter_array* may occur after an optional parameter, but cannot have a default value – the omission of arguments for a *parameter_array* would instead result in the creation of an empty array.

> *Example*: The following illustrates different kinds of parameters:
> ```csharp
> public void M(
>     ref int i,
>     decimal d,
>     bool b = false,
>     bool? n = false,
>     string s = "Hello",
>     object o = null,
>     T t = default(T),
>     params int[] a
> ) { }
> ```
> In the *formal_parameter_list* for `M`, `i` is a required `ref` parameter, `d` is a required value parameter, `b`, `s`, `o` and `t` are optional value parameters and `a` is a parameter array. *end example*

A method declaration creates a separate declaration space ([§8.3](basic-concepts.md#83-declarations)) for parameters and type parameters. Names are introduced into this declaration space by the type parameter list and the formal parameter list of the method. The body of the method, if any, is considered to be nested within this declaration space. It is an error for two members of a method declaration space to have the same name. It is an error for the method declaration space and the local variable declaration space of a nested declaration space to contain elements with the same name.

A method invocation ([§12.7.6.2](expressions.md#12762-method-invocations)) creates a copy, specific to that invocation, of the formal parameters and local variables of the method, and the argument list of the invocation assigns values or variable references to the newly created formal parameters. Within the *block* of a method, formal parameters can be referenced by their identifiers in *simple_name* expressions ([§12.7.3](expressions.md#1273-simple-names)).

There are four kinds of formal parameters:

-  Value parameters, which are declared without any modifiers.
-  Reference parameters, which are declared with the `ref` modifier.
-  Output parameters, which are declared with the `out` modifier.
-  Parameter arrays, which are declared with the `params` modifier.

> *Note*: As described in [§8.6](basic-concepts.md#86-signatures-and-overloading), the `ref` and `out` modifiers are part of a method's signature, but the `params` modifier is not. 

#### 15.6.2.2 Value parameters

A parameter declared with no modifiers is a value parameter. A value parameter corresponds to a local variable that gets its initial value from the corresponding argument supplied in the method invocation.

When a formal parameter is a value parameter, the corresponding argument in a method invocation shall be an expression that is implicitly convertible ([§11.2](conversions.md#112-implicit-conversions)) to the formal parameter type.

A method is permitted to assign new values to a value parameter. Such assignments only affect the local storage location represented by the value parameter—they have no effect on the actual argument given in the method invocation.

#### 15.6.2.3 Reference parameters

A parameter declared with a `ref` modifier is a reference parameter. Unlike a value parameter, a reference parameter does not create a new storage location. Instead, a reference parameter represents the same storage location as the variable given as the argument in the method invocation.

When a formal parameter is a reference parameter, the corresponding argument in a method invocation shall consist of the keyword `ref` followed by a *variable_reference* ([§10.5](variables.md#105-variable-references)) of the same type as the formal parameter. A variable shall be definitely assigned before it can be passed as a reference parameter.

Within a method, a reference parameter is always considered definitely assigned.

A method declared as an iterator ([§15.14](classes.md#1514-iterators)) may not have reference parameters.

> *Example*: The example
> ```csharp
> using System;
> class Test
> {
>     static void Swap(ref int x, ref int y) {
>         int temp = x;
>         x = y;
>         y = temp;
>     }
> static void Main() {
>        int i = 1, j = 2;
>        Swap(ref i, ref j);
>         Console.WriteLine("i = {0}, j = {1}", i, j);
>     }
> }
> ```
> produces the output
> ```console
> i = 2, j = 1
> ```
> For the invocation of `Swap` in `Main`, `x` represents `i` and `y` represents `j`. Thus, the invocation has the effect of swapping the values of `i` and `j`. *end example*

In a method that takes reference parameters, it is possible for multiple names to represent the same storage location.

> *Example*: In the following code
> ```csharp
> class A
> {
>     string s;
>     void F(ref string a, ref string b) {
>         s = "One";
>         a = "Two";
>         b = "Three";
>     }
>     void G() {
>        F(ref s, ref s);
>     }
> }
> ```
> the invocation of `F` in `G` passes a reference to `s` for both `a` and `b`. Thus, for that invocation, the names `s`, `a`, and `b` all refer to the same storage location, and the three assignments all modify the instance field `s`. *end example*

#### 15.6.2.4 Output parameters

A parameter declared with an `out` modifier is an output parameter. Similar to a reference parameter, an output parameter does not create a new storage location. Instead, an output parameter represents the same storage location as the variable given as the argument in the method invocation.

When a formal parameter is an output parameter, the corresponding argument in a method invocation shall consist of the keyword `out` followed by a *variable_reference* ([§10.5](variables.md#105-variable-references)) of the same type as the formal parameter. A variable need not be definitely assigned before it can be passed as an output parameter, but following an invocation where a variable was passed as an output parameter, the variable is considered definitely assigned.

Within a method, just like a local variable, an output parameter is initially considered unassigned and shall be definitely assigned before its value is used.

Every output parameter of a method shall be definitely assigned before the method returns.

A method declared as a partial method ([§15.6.9](classes.md#1569-partial-methods)) or an iterator ([§15.14](classes.md#1514-iterators)) may not have output parameters.

Output parameters are typically used in methods that produce multiple return values.

> *Example*:
> ```csharp
> using System;
> class Test
> {
>     static void SplitPath(string path, out string dir, out string name) {
>         int i = path.Length;
>         while (i > 0) {
>             char ch = path[i – 1];
>             if (ch == '\\' || ch == '/' || ch == ':') break;
>             i--;
>         }
>         dir = path.Substring(0, i);
>         name = path.Substring(i);
>     }
>     static void Main() {
>         string dir, name;
>         SplitPath("c:\\\Windows\\\\System\\\\hello.txt", out dir, out name);
>         Console.WriteLine(dir);
>         Console.WriteLine(name);
>     }
> }
> ```
> The example produces the output:
> ```console
> c:\Windows\System\
> hello.txt
> ```
> Note that the `dir` and `name` variables can be unassigned before they are passed to `SplitPath`, and that they are considered definitely assigned following the call. *end example*

#### 15.6.2.5 Parameter arrays

A parameter declared with a `params` modifier is a parameter array. If a formal parameter list includes a parameter array, it shall be the last parameter in the list and it shall be of a single-dimensional array type.

> *Example*: The types `string[]` and `string[][]` can be used as the type of a parameter array, but the type `string[,]` can not. *end example*

It is not possible to combine the `params` modifier with the modifiers `ref` and `out`.

A parameter array permits arguments to be specified in one of two ways in a method invocation:

-  The argument given for a parameter array can be a single expression that is implicitly convertible ([§11.2](conversions.md#112-implicit-conversions)) to the parameter array type. In this case, the parameter array acts precisely like a value parameter.
-  Alternatively, the invocation can specify zero or more arguments for the parameter array, where each argument is an expression that is implicitly convertible ([§11.2](conversions.md#112-implicit-conversions)) to the element type of the parameter array. In this case, the invocation creates an instance of the parameter array type with a length corresponding to the number of arguments, initializes the elements of the array instance with the given argument values, and uses the newly created array instance as the actual argument.

Except for allowing a variable number of arguments in an invocation, a parameter array is precisely equivalent to a value parameter ([§15.6.2.2](classes.md#15622-value-parameters)) of the same type.

> *Example*: The example
> ```csharp
> using System;
> class Test
> {
>     static void F(params int[] args) {
>         Console.Write("Array contains {0} elements:", args.Length);
>         foreach (int i in args)
>         Console.Write(" {0}", i);
>         Console.WriteLine();
>     }
>     static void Main() {
>         int[] arr = {1, 2, 3};
>         F(arr);
>         F(10, 20, 30, 40);
>         F();
>     }
> }
> ```
> produces the output
> ```console
> Array contains 3 elements: 1 2 3
> Array contains 4 elements: 10 20 30 40
> Array contains 0 elements:
> ```
> The first invocation of `F` simply passes the array `arr` as a value parameter. The second invocation of F automatically creates a four-element `int[]` with the given element values and passes that array instance as a value parameter. Likewise, the third invocation of `F` creates a zero-element `int[]` and passes that instance as a value parameter. The second and third invocations are precisely equivalent to writing:
> ```csharp
> F(new int[] {10, 20, 30, 40});
> F(new int[] {});
> ```
> *end example*

When performing overload resolution, a method with a parameter array might be applicable, either in its normal form or in its expanded form ([§12.6.4.2](expressions.md#12642-applicable-function-member)). The expanded form of a method is available only if the normal form of the method is not applicable and only if an applicable method with the same signature as the expanded form is not already declared in the same type.

> *Example*: The example
> ```csharp
> using System;
> class Test
> {
>     static void F(params object[] a) {
>         Console.WriteLine("F(object[])");
>     }
>     static void F() {
>         Console.WriteLine("F()");
>     }
>     static void F(object a0, object a1) {
>        Console.WriteLine("F(object,object)");
>     }
>     static void Main() {
>         F();
>         F(1);
>         F(1, 2);
>         F(1, 2, 3);
>         F(1, 2, 3, 4);
>     }
> }
> ```
> produces the output
> ```console
> F();
> F(object[]);
> F(object,object);
> F(object[]);
> F(object[]);
> ```
> In the example, two of the possible expanded forms of the method with a parameter array are already included in the class as regular methods. These expanded forms are therefore not considered when performing overload resolution, and the first and third method invocations thus select the regular methods. When a class declares a method with a parameter array, it is not uncommon to also include some of the expanded forms as regular methods. By doing so, it is possible to avoid the allocation of an array instance that occurs when an expanded form of a method with a parameter array is invoked. *end example*

> An array is a reference type, so the value passed for a parameter array can be `null`.
> 
> *Example*: The example:
> ```csharp
> using System;
> class Test {
>     void F(params string[] array)
>     {
>          Console.WriteLine(array == null);
>     }
> 
>     static void Main()
>     {
>         F(null);
>         F((string) null);
>     }
> ```
> produces the output:
> ```csharp
> True
> False
> ```
> The second invocation produces `False` as it is equivalent to `F(new string[] { null })` and passes an array containing a single null reference. *end example*

When the type of a parameter array is `object[]`, a potential ambiguity arises between the normal form of the method and the expanded form for a single `object` parameter. The reason for the ambiguity is that an `object[]` is itself implicitly convertible to type `object`. The ambiguity presents no problem, however, since it can be resolved by inserting a cast if needed.

> *Example*: The example
> ```csharp
> using System;
> class Test
> {
>     static void F(params object[] args) {
>         foreach (object o in args) {
>         Console.Write(o.GetType().FullName);
>         Console.Write(" ");
>     }
> Console.WriteLine();
> }
> static void Main() {
>     object[] a = {1, "Hello", 123.456};
>     object o = a;
>     F(a);
>     F((object)a);
>     F(o);
>     F((object[])o);
>     }
> }
> ```
> produces the output
> ```console
> System.Int32 System.String System.Double
> System.Object[]
> System.Object[]
> System.Int32 System.String System.Double
> ```
> In the first and last invocations of `F`, the normal form of `F` is applicable because an implicit conversion exists from the argument type to the parameter type (both are of type `object[]`). Thus, overload resolution selects the normal form of `F`, and the argument is passed as a regular value parameter. In the second and third invocations, the normal form of `F` is not applicable because no implicit conversion exists from the argument type to the parameter type (type `object` cannot be implicitly converted to type `object[]`). However, the expanded form of `F` is applicable, so it is selected by overload resolution. As a result, a one-element `object[]` is created by the invocation, and the single element of the array is initialized with the given argument value (which itself is a reference to an `object[]`). *end example*

### 15.6.3 Static and instance methods

When a method declaration includes a `static` modifier, that method is said to be a static method. When no `static` modifier is present, the method is said to be an instance method.

A static method does not operate on a specific instance, and it is a compile-time error to refer to `this` in a static method.

An instance method operates on a given instance of a class, and that instance can be accessed as `this` ([§12.7.8](expressions.md#1278-this-access)).

The differences between static and instance members are discussed further in [§15.3.8](classes.md#1538-static-and-instance-members).

### 15.6.4 Virtual methods

When an instance method declaration includes a virtual modifier, that method is said to be a ***virtual method***. When no virtual modifier is present, the method is said to be a ***non-virtual method***.

The implementation of a non-virtual method is invariant: The implementation is the same whether the method is invoked on an instance of the class in which it is declared or an instance of a derived class. In contrast, the implementation of a virtual method can be superseded by derived classes. The process of superseding the implementation of an inherited virtual method is known as ***overriding*** that method ([§15.6.5](classes.md#1565-override-methods)).

In a virtual method invocation, the ***run-time type*** of the instance for which that invocation takes place determines the actual method implementation to invoke. In a non-virtual method invocation, the ***compile-time type*** of the instance is the determining factor. In precise terms, when a method named `N` is invoked with an argument list `A` on an instance with a compile-time type `C` and a run-time type `R` (where `R` is either `C` or a class derived from `C`), the invocation is processed as follows:

-  At binding-time, overload resolution is applied to `C`, `N`, and `A`, to select a specific method `M` from the set of methods declared in and inherited by `C`. This is described in [§12.7.6.2](expressions.md#12762-method-invocations).
- Then at run-time:
  - If `M` is a non-virtual method, `M` is invoked.
  - Otherwise, `M` is a virtual method, and the most derived implementation of `M` with respect to `R` is invoked.

For every virtual method declared in or inherited by a class, there exists a ***most derived implementation*** of the method with respect to that class. The most derived implementation of a virtual method `M` with respect to a class `R` is determined as follows:

-  If `R` contains the introducing virtual declaration of `M`, then this is the most derived implementation of `M` with respect to `R`.
-  Otherwise, if `R` contains an override of `M`, then this is the most derived implementation of `M` with respect to `R`.
-  Otherwise, the most derived implementation of `M` with respect to `R` is the same as the most derived implementation of `M` with respect to the direct base class of `R`.

> *Example*: The following example illustrates the differences between virtual and non-virtual methods:
> ```csharp
> using System;
> class A
> {
>     public void F() { Console.WriteLine("A.F"); }
>     public virtual void G() { Console.WriteLine("A.G"); }
> }
> class B: A
> {
>     new public void F() { Console.WriteLine("B.F"); }
>     public override void G() { Console.WriteLine("B.G"); }
> }
> class Test
> {
>     static void Main() {
>         B b = new B();
>         A a = b;
>         a.F();
>         b.F();
>         a.G();
>         b.G();
>     }
> }
> ```
> In the example, `A` introduces a non-virtual method `F` and a virtual method `G`. The class `B` introduces a *new* non-virtual method `F`, thus *hiding* the inherited `F`, and also *overrides* the inherited method `G`. The example produces the output:
> ```console
> A.F
> B.F
> B.G
> B.G
> ```
> Notice that the statement `a.G()` invokes `B.G`, not `A.G`. This is because the run-time type of the instance (which is `B`), not the compile-time type of the instance (which is `A`), determines the actual method implementation to invoke. *end example*

Because methods are allowed to hide inherited methods, it is possible for a class to contain several virtual methods with the same signature. This does not present an ambiguity problem, since all but the most derived method are hidden.

> *Example*: In the following code
> ```csharp
> using System;
> class A
> {
>     public virtual void F() { Console.WriteLine("A.F"); }
> }
> class B: A
> {
>     public override void F() { Console.WriteLine("B.F"); }
> }
> class C: B
> {
>     new public virtual void F() { Console.WriteLine("C.F"); }
> }
> class D: C
> {
>     public override void F() { Console.WriteLine("D.F"); }
> }
> class Test
> {
>     static void Main() {
>         D d = new D();
>         A a = d;
>         B b = d;
>         C c = d;
>         a.F();
>         b.F();
>         c.F();
>         d.F();
>    }
> }
> ```
> the `C` and `D` classes contain two virtual methods with the same signature: The one introduced by `A` and the one introduced by `C`. The method introduced by `C` hides the method inherited from `A`. Thus, the override declaration in `D` overrides the method introduced by `C`, and it is not possible for `D` to override the method introduced by `A`. The example produces the output:
> ```console
> B.F
> B.F
> D.F
> D.F
> ```
> Note that it is possible to invoke the hidden virtual method by accessing an instance of `D` through a less derived type in which the method is not hidden. *end example*

### 15.6.5 Override methods

When an instance method declaration includes an `override` modifier, the method is said to be an ***override method***. An override method overrides an inherited virtual method with the same signature. Whereas a virtual method declaration *introduces* a new method, an override method declaration *specializes* an existing inherited virtual method by providing a new implementation of that method.

The method overridden by an override declaration is known as the ***overridden base method*** For an override method `M` declared in a class `C`, the overridden base method is determined by examining each base class of `C`, starting with the direct base class of `C` and continuing with each successive direct base class, until in a given base class type at least one accessible method is located which has the same signature as `M` after substitution of type arguments. For the purposes of locating the overridden base method, a method is considered accessible if it is `public`, if it is `protected`, if it is `protected internal`, or if it is `internal` and declared in the same program as `C`.

A compile-time error occurs unless all of the following are true for an override declaration:

-  An overridden base method can be located as described above.
-  There is exactly one such overridden base method. This restriction has effect only if the base class type is a constructed type where the substitution of type arguments makes the signature of two methods the same.
-  The overridden base method is a virtual, abstract, or override method. In other words, the overridden base method cannot be static or non-virtual.
-  The overridden base method is not a sealed method.
-  There is an identity conversion between the return type of the overridden base method and the override method.
-  The override declaration and the overridden base method have the same declared accessibility. In other words, an override declaration cannot change the accessibility of the virtual method. However, if the overridden base method is protected internal and it is declared in a different assembly than the assembly containing the override declaration then the override declaration's declared accessibility shall be protected.
-  The override declaration does not specify any *type_parameter_constraints_clause*s. Instead, the constraints are inherited from the overridden base method. Constraints that are type parameters in the overridden method may be replaced by type arguments in the inherited constraint. This can lead to constraints that are not valid when explicitly specified, such as value types or sealed types.

> *Example*: The following demonstrates how the overriding rules work for generic classes:
> ```csharp
> abstract class C<T>
> {
>     public virtual T F() {...}
>     public virtual C<T> G() {...}
>     public virtual void H(C<T> x) {...}
> }
> class D: C<string>
> {
>     public override string F() {...}                // Ok
>     public override C<string> G() {...}             // Ok
>     public override void H(C<T> x) {...}            // Error, should be C<string>
> }
> class E<T,U>: C<U>
> {
>     public override U F() {...}                     // Ok
>     public override C<U> G() {...}                  // Ok
>     public override void H(C<T> x) {...}            // Error, should be C<U>
> }
> ```
> *end example*

An override declaration can access the overridden base method using a *base_access* ([§12.7.9](expressions.md#1279-base-access)).

> *Example*: In the following code
> ```csharp
> class A
> {
>     int x;
>     public virtual void PrintFields() {
>         Console.WriteLine("x = {0}", x);
>     }
> }
> class B: A
> {
>     int y;
>     public override void PrintFields() {
>         base.PrintFields();
>         Console.WriteLine("y = {0}", y);
>     }
> }
> ```
> the `base.PrintFields()` invocation in `B` invokes the PrintFields method declared in `A`. A *base_access* disables the virtual invocation mechanism and simply treats the base method as a non-`virtual` method. Had the invocation in `B` been written `((A)this).PrintFields()`, it would recursively invoke the `PrintFields` method declared in `B`, not the one declared in `A`, since `PrintFields` is virtual and the run-time type of `((A)this)` is `B`. *end example*

Only by including an `override` modifier can a method override another method. In all other cases, a method with the same signature as an inherited method simply hides the inherited method.

> *Example*: In the following code
> ```csharp
> class A
> {
>     public virtual void F() {}
> }
> class B: A
> {
>     public virtual void F() {} // Warning, hiding inherited F()
> }
> ```
> the `F` method in `B` does not include an `override` modifier and therefore does not override the `F` method in `A`. Rather, the `F` method in `B` hides the method in `A`, and a warning is reported because the declaration does not include a new modifier. *end example*

> *Example*: In the following code
> ```csharp
> class A
> {
>     public virtual void F() {}
> }
> class B: A
> {
>     new private void F() {} // Hides A.F within body of B
> }
> 
> class C: B
> {
>     public override void F() {} // Ok, overrides A.F
> }
> ```
> the `F` method in `B` hides the virtual `F` method inherited from `A`. Since the new `F` in `B` has private access, its scope only includes the class body of `B` and does not extend to `C`. Therefore, the declaration of `F` in `C` is permitted to override the `F` inherited from `A`. *end example*

### 15.6.6 Sealed methods

When an instance method declaration includes a `sealed` modifier, that method is said to be a ***sealed method***. A sealed method overrides an inherited virtual method with the same signature. A sealed method shall also be marked with the `override` modifier. Use of the `sealed` modifier prevents a derived class from further overriding the method.

> *Example*: The example
> ```csharp
> using System;
> class A
> {
>     public virtual void F() {
>         Console.WriteLine("A.F");
>     }
>     public virtual void G() {
>         Console.WriteLine("A.G");
>     }
> }
> class B: A
>     {
>     public sealed override void F() {
>         Console.WriteLine("B.F");
>     }
>     public override void G() {
>         Console.WriteLine("B.G");
>     }
> }
> class C: B
>     {
>     public override void G() {
>         Console.WriteLine("C.G");
>     }
> }
> ```
> the class `B` provides two override methods: an `F` method that has the `sealed` modifier and a `G` method that does not. `B`'s use of the `sealed` modifier prevents `C` from further overriding `F`. *end example*

### 15.6.7 Abstract methods

When an instance method declaration includes an `abstract` modifier, that method is said to be an ***abstract method***. Although an abstract method is implicitly also a virtual method, it cannot have the modifier `virtual`.

An abstract method declaration introduces a new virtual method but does not provide an implementation of that method. Instead, non-abstract derived classes are required to provide their own implementation by overriding that method. Because an abstract method provides no actual implementation, the *method-body* of an abstract method simply consists of a semicolon.

Abstract method declarations are only permitted in abstract classes ([§15.2.2.2](classes.md#15222-abstract-classes)).

> *Example*: In the following code
> ```csharp
> public abstract class Shape
> {
>     public abstract void Paint(Graphics g, Rectangle r);
> }
> public class Ellipse: Shape
> {
>     public override void Paint(Graphics g, Rectangle r) {
>         g.DrawEllipse(r);
>     }
> }
> public class Box: Shape
> {
>     public override void Paint(Graphics g, Rectangle r) {
>         g.DrawRect(r);
>     }
> }
> ```
> the `Shape` class defines the abstract notion of a geometrical shape object that can paint itself. The `Paint` method is abstract because there is no meaningful default implementation. The `Ellipse` and `Box` classes are concrete `Shape` implementations. Because these classes are non-abstract, they are required to override the `Paint` method and provide an actual implementation. *end example*

It is a compile-time error for a *base_access* ([§12.7.9](expressions.md#1279-base-access)) to reference an abstract method.

> *Example*: In the following code
> ```csharp
> abstract class A
> {
>     public abstract void F();
> }
> class B: A
> {
>     public override void F() {
>         base.F(); // Error, base.F is abstract\
>     }
> }
> ```
> a compile-time error is reported for the `base.F()` invocation because it references an abstract method. *end example*

An abstract method declaration is permitted to override a virtual method. This allows an abstract class to force re-implementation of the method in derived classes, and makes the original implementation of the method unavailable.

> *Example*: In the following code
> ```csharp
> using System;
> class A
> {
>     public virtual void F() {
>         Console.WriteLine("A.F");
>     }
> }
> abstract class B: A
> {
>     public abstract override void F();
> }
> class C: B
> {
>     public override void F() {
>         Console.WriteLine("C.F");
>     }
> }
> ```
> class `A` declares a virtual method, class `B` overrides this method with an abstract method, and class `C` overrides the abstract method to provide its own implementation. *end example*

### 15.6.8 External methods

When a method declaration includes an `extern` modifier, the method is said to be an ***external method***. External methods are implemented externally, typically using a language other than C#. Because an external method declaration provides no actual implementation, the *method_body* of an external method simply consists of a semicolon. An external method shall not be generic.

The mechanism by which linkage to an external method is achieved, is implementation-defined.

> *Example*: The following example demonstrates the use of the `extern` modifier and the `DllImport` attribute:
> ```csharp
> using System.Text;
> using System.Security.Permissions;
> using System.Runtime.InteropServices;
> class Path
> {
>     [DllImport("kernel32", SetLastError=true)]
>     static extern bool CreateDirectory(string name, SecurityAttribute sa);
>     [DllImport("kernel32", SetLastError=true)]
>     static extern bool RemoveDirectory(string name);
>     [DllImport("kernel32", SetLastError=true)]
>     static extern `int` GetCurrentDirectory(int bufSize, StringBuilder buf);
>     [DllImport("kernel32", SetLastError=true)]
>     static extern bool SetCurrentDirectory(string name);
> }
> ```
> *end example*

### 15.6.9 Partial methods

When a method declaration includes a `partial` modifier, that method is said to be a ***partial method***. Partial methods may only be declared as members of partial types ([§15.2.7](classes.md#1527-partial-declarations)), and are subject to a number of restrictions.

Partial methods may be defined in one part of a type declaration and implemented in another. The implementation is optional; if no part implements the partial method, the partial method declaration and all calls to it are removed from the type declaration resulting from the combination of the parts.

Partial methods shall not define access modifiers; they are implicitly private. Their return type shall be `void`, and their parameters shall not have the `out` modifier. The identifier partial is recognized as a contextual keyword ([§7.4.4](lexical-structure.md#744-keywords)) in a method declaration only if it appears immediately before the `void` keyword. A partial method cannot explicitly implement interface methods.

There are two kinds of partial method declarations: If the body of the method declaration is a semicolon, the declaration is said to be a ***defining partial method declaration***. If the body is given as a *block*, the declaration is said to be an ***implementing partial method declaration***. Across the parts of a type declaration, there may be only one defining partial method declaration with a given signature, and there may be only one implementing partial method declaration with a given signature. If an implementing partial method declaration is given, a corresponding defining partial method declaration shall exist, and the declarations shall match as specified in the following:

-  The declarations shall have the same modifiers (although not necessarily in the same order), method name, number of type parameters and number of parameters.
-  Corresponding parameters in the declarations shall have the same modifiers (although not necessarily in the same order) and the same types (modulo differences in type parameter names).
-  Corresponding type parameters in the declarations shall have the same constraints (modulo differences in type parameter names).

An implementing partial method declaration can appear in the same part as the corresponding defining partial method declaration.

Only a defining partial method participates in overload resolution. Thus, whether or not an implementing declaration is given, invocation expressions may resolve to invocations of the partial method. Because a partial method always returns `void`, such invocation expressions will always be expression statements. Furthermore, because a partial method is implicitly `private`, such statements will always occur within one of the parts of the type declaration within which the partial method is declared.

> *Note*: The definition of matching defining and implementing partial method declarations does not require parameter names to match. This can produce *surprising*, albeit *well defined*, behaviour when named arguments ([§12.6.2.1](expressions.md#12621-general)) are used. For example, given the defining partial method declaration for `M`:
> ```csharp
> partial class P
> {
>     static partial void M(int x);
> }
> ```
> Then the implementing partial method declaration and invocation in other file:
> ```csharp
> partial class P
> {
>     static void Caller()
>     {
>         M(y: 0);
>     }
> 
>     static partial void M(int y) {}
> }
> ```
> is **invalid** as the invocation uses the argument name from the implementing and not the defining partial method declaration. *end note*

If no part of a partial type declaration contains an implementing declaration for a given partial method, any expression statement invoking it is simply removed from the combined type declaration. Thus the invocation expression, including any subexpressions, has no effect at run-time. The partial method itself is also removed and will not be a member of the combined type declaration.

If an implementing declaration exists for a given partial method, the invocations of the partial methods are retained. The partial method gives rise to a method declaration similar to the implementing partial method declaration except for the following:

-  The `partial` modifier is not included.

-  The attributes in the resulting method declaration are the combined attributes of the defining and the implementing partial method declaration in unspecified order. Duplicates are not removed.

-  The attributes on the parameters of the resulting method declaration are the combined attributes of the corresponding parameters of the defining and the implementing partial method declaration in unspecified order. Duplicates are not removed.

If a defining declaration but not an implementing declaration is given for a partial method `M`, the following restrictions apply:

-  It is a compile-time error to create a delegate from `M` ([§12.7.11.6](expressions.md#127116-delegate-creation-expressions)).

-  It is a compile-time error to refer to `M` inside an anonymous function that is converted to an expression tree type ([§9.6](types.md#96-expression-tree-types)).

-  Expressions occurring as part of an invocation of `M` do not affect the definite assignment state ([§10.4](variables.md#104-definite-assignment)), which can potentially lead to compile-time errors.

-  `M` cannot be the entry point for an application ([§8.1](basic-concepts.md#81-application-startup)).

Partial methods are useful for allowing one part of a type declaration to customize the behavior of another part, e.g., one that is generated by a tool. Consider the following partial class declaration:

```csharp
partial class Customer
{
    string name;
    public string name {
        get { return name; }
        set {
            OnNameChanging(value);
            name = value;
            OnNameChanged();
        }
    }
    partial void OnNameChanging(string newName);
    partial void OnNameChanged();
}
```

If this class is compiled without any other parts, the defining partial method declarations and their invocations will be removed, and the resulting combined class declaration will be equivalent to the following:

```csharp
class Customer
{
    string name;
    public string name {
        get { return name; }
        set { name = value; }
    }
}
```

Assume that another part is given, however, which provides implementing declarations of the partial methods:

```csharp
partial class Customer
{
    partial void OnNameChanging(string newName)
    {
        Console.WriteLine("Changing " + name + " to " + newName);
    }
    partial void OnNameChanged()
    {
        Console.WriteLine("Changed to " + name);
    }
}
```

Then the resulting combined class declaration will be equivalent to the following:

```csharp
class Customer
{
    string name;
    public string name {
        get { return name; }
        set {
            OnNameChanging(value);
            name = value;
            OnNameChanged();
        }
    }
    void OnNameChanging(string newName)
    {
        Console.WriteLine("Changing " + name + " to " + newName);
    }
    void OnNameChanged()
    {
        Console.WriteLine("Changed to " + name);
    }
}
```

### 15.6.10 Extension methods

When the first parameter of a method includes the `this` modifier, that method is said to be an ***extension method***. Extension methods shall only be declared in non-generic, non-nested static classes. The first parameter of an extension method may have no modifiers other than `this`, and the parameter type may not be a pointer type.

> *Example*: The following is an example of a static class that declares two extension methods:
> ```csharp
> public static class Extensions
> {
>     public static int ToInt32(this string s) {
>         return Int32.Parse(s);
>     }
>     public static T[] Slice<T>(this T[] source, int index, int count) {
>         if (index < 0 || count < 0 || source.Length – index < count)
>         throw new ArgumentException();
>         T[] result = new T[count];
>         Array.Copy(source, index, result, 0, count);
>         return result;
>     }
> }
> ```
> *end example*

An extension method is a regular static method. In addition, where its enclosing static class is in scope, an extension method may be invoked using instance method invocation syntax ([§12.7.6.3](expressions.md#12763-extension-method-invocations)), using the receiver expression as the first argument.

> *Example*: The following program uses the extension methods declared above:
> ```csharp
> static class Program
> {
>     static void Main() {
>         string[] strings = { "1", "22", "333", "4444" };
>         foreach (string s in strings.Slice(1, 2)) {
>             Console.WriteLine(s.ToInt32());
>         }
>     }
> }
> ```
> The `Slice` method is available on the `string[]`, and the `ToInt32` method is available on `string`, because they have been declared as extension methods. The meaning of the program is the same as the following, using ordinary static method calls:
> ```csharp
> static class Program
> {
>     static void Main() {
>     string[] strings = { "1", "22", "333", "4444" };
>     foreach (string s in Extensions.Slice(strings, 1, 2)) {
>             Console.WriteLine(Extensions.ToInt32(s));
>         }
>     }
> }
> ```
> *end example*

### 15.6.11 Method body

The *method_body* of a method declaration consists of either a block body, an expression body or a semicolon.

Abstract and external method declarations do not provide a method implementation, so their method bodies simply consist of a semicolon. For any other method, the method body is a block ([§13.3](statements.md#133-blocks)) that contains the statements to execute when that method is invoked.

The ***effective return type*** of a method is `void` if the return type is `void`, or if the method is async and the return type is `System.Threading.Tasks.Task`. Otherwise, the effective return type of a non-async method is its return type, and the effective return type of an async method with return type `System.Threading.Tasks.Task<T>` is `T`.

When the effective return type of a method is `void` and the method has a block body, `return` statements ([§13.10.5](statements.md#13105-the-return-statement)) in the block shall not specify an expression. If execution of the block of a void method completes normally (that is, control flows off the end of the method body), that method simply returns to its caller.

When a method has a `void` result and an expression body, the expression `E` shall be a *statement_expression*, and the body is exactly equivalent to a statment body of the form `{ E; }`.

When the effective return type of a method is not `void` and the method has a block body, each return statement in that method's body shall specify an expression that is implicitly convertible to the effective return type. The endpoint of the method body of a value-returning method shall not be reachable. In other words, in a value-returning method with a block body, control is not permitted to flow off the end of the method body.

When the effective return type of a method is not `void` and the method has an expression body, `E`, the expression shall be implicitly convertible to the effective return type, and the body is exactly equivalent to a block body of the form `{ return E; }`.

> *Example*: In the following code
> ```csharp
> class A
> {
>     public int F() {} // Error, return value required
>     public int G() {
>         return 1;
>     }
>     public int H(bool b) {
>         if (b) {
>             return 1;
>         }
>         else {
>             return 0;
>         }
>     }
>     public int I(bool b) => b ? 1 : 0;
> }
> ```
> the value-returning `F` method results in a compile-time error because control can flow off the end of the method body. The `G` and `H` methods are correct because all possible execution paths end in a return statement that specifies a return value. The `I` method is correct, because its body is equivalent to a statement block with just a single return statement in it. *end example*]

## 15.7 Properties

### 15.7.1 General

A ***property*** is a member that provides access to a characteristic of an object or a class. Examples of properties include the length of a string, the size of a font, the caption of a window, the name of a customer, and so on. Properties are a natural extension of fields—both are named members with associated types, and the syntax for accessing fields and properties is the same. However, unlike fields, properties do not denote storage locations. Instead, properties have ***accessors*** that specify the statements to be executed when their values are read or written. Properties thus provide a mechanism for associating actions with the reading and writing of an object's characteristics; furthermore, they permit such characteristics to be computed.

Properties are declared using *property_declaration*s:

```ANTLR
property_declaration
    : attributes? property_modifier* type member_name property_body
    ;    

property_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | 'static'
    | 'virtual'
    | 'sealed'
    | 'override'
    | 'abstract'
    | 'extern'
    | unsafe_modifier   // unsafe code support
    ;
    
property_body
    : '{' accessor_declarations '}' property_initializer?
    | '=>' expression ';'
    ;

property_initializer
    : '=' variable_initializer ';'
    ;
```

*unsafe_modifier* ([§23.2](unsafe-code.md#232-unsafe-contexts)) is only available in unsafe code ([§23](unsafe-code.md#23-unsafe-code)).

A *property_declaration* may include a set of *attributes* ([§22](attributes.md#22-attributes)) and a valid combination of the four access modifiers ([§15.3.6](classes.md#1536-access-modifiers)), the `new` ([§15.3.5](classes.md#1535-the-new-modifier)), `static` ([§15.7.2](classes.md#1572-static-and-instance-properties)), `virtual` ([§15.6.4](classes.md#1564-virtual-methods), [§15.7.6](classes.md#1576-virtual-sealed-override-and-abstract-accessors)), `override` ([§15.6.5](classes.md#1565-override-methods), [§15.7.6](classes.md#1576-virtual-sealed-override-and-abstract-accessors)), `sealed` ([§15.6.6](classes.md#1566-sealed-methods)), `abstract` ([§15.6.7](classes.md#1567-abstract-methods), [§15.7.6](classes.md#1576-virtual-sealed-override-and-abstract-accessors)), and `extern` ([§15.6.8](classes.md#1568-external-methods)) modifiers.

Property declarations are subject to the same rules as method declarations ([§15.6](classes.md#156-methods)) with regard to valid combinations of modifiers.

The *type* of a property declaration specifies the type of the property introduced by the declaration, and the *member_name* ([§15.6.1](classes.md#1561-general)) specifies the name of the property. Unless the property is an explicit interface member implementation, the *member_name* is simply an *Identifier*. For an explicit interface member implementation ([§18.6.2](interfaces.md#1862-explicit-interface-member-implementations)), the *member_name* consists of an *interface_type* followed by a "`.`" and an *Identifier*.

The *type* of a property shall be at least as accessible as the property itself ([§8.5.5](basic-concepts.md#855-accessibility-constraints)).

A *property_body* may either consist of an ***accessor body*** or an expression body ([§15.6.1](classes.md#1561-general)). In an accessor body,  *accessor_declarations*, which shall be enclosed in "`{`" and "`}`" tokens, declare the accessors ([§15.7.3](classes.md#1573-accessors)) of the property. The accessors specify the executable statements associated with reading and writing the property.

An expression body consisting of `=>` followed by an *expression* `E` and a semicolon is exactly equivalent to the block body `{ get { return E; } }`, and can therefore only be used to specify getter-only properties where the result of the getter is given by a single expression.

A *property_initializer* may only be given for an automatically implemented property ([§15.7.4](classes.md#1574-automatically-implemented-properties)), and causes the initialization of the underlying field of such properties with the value given by the *expression*.

Even though the syntax for accessing a property is the same as that for a field, a property is not classified as a variable. Thus, it is not possible to pass a property as a `ref` or `out` argument.

When a property declaration includes an `extern` modifier, the property is said to be an ***external property***. Because an external property declaration provides no actual implementation, each of its *accessor_declarations* consists of a semicolon.

### 15.7.2 Static and instance properties

When a property declaration includes a `static` modifier, the property is said to be a ***static property***. When no `static` modifier is present, the property is said to be an ***instance property***.

A static property is not associated with a specific instance, and it is a compile-time error to refer to `this` in the accessors of a static property.

An instance property is associated with a given instance of a class, and that instance can be accessed as `this` ([§12.7.8](expressions.md#1278-this-access)) in the accessors of that property.

The differences between static and instance members are discussed further in [§15.3.8](classes.md#1538-static-and-instance-members).

### 15.7.3 Accessors

The *accessor_declarations* of a property specify the executable statements associated with reading and writing that property.

```ANTLR
accessor_declarations
   : get_accessor_declaration set_accessor_declaration?
   | set_accessor_declaration get_accessor_declaration?
   ;

get_accessor_declaration
    : attributes? accessor_modifier? 'get' accessor_body
    ;

set_accessor_declaration
    : attributes? accessor_modifier? 'set' accessor_body
    ;

accessor_modifier
    : 'protected'
    | 'internal'
    | 'private'
    | 'protected' 'internal'
    | 'internal' 'protected'
    ;

accessor_body
    : block
    | ';' 
    ;
```

The accessor declarations consist of a *get_accessor_declaration*, a *set_accessor_declaration*, or both. Each accessor declaration consists of optional attributes, an optional *accessor_modifier*, the token `get` or `set`, followed by an *accessor_body*.

The use of *accessor_modifier*s is governed by the following restrictions:

- An *accessor_modifier* shall not be used in an interface or in an explicit interface member implementation.
- For a property or indexer that has no `override` modifier, an *accessor_modifier* is permitted only if the property or indexer has both a `get` and `set` accessor, and then is permitted only on one of those accessors.
- For a property or indexer that includes an `override` modifier, an accessor shall match the *accessor_modifier*, if any, of the accessor being overridden.
- The *accessor_modifier* shall declare an accessibility that is strictly more restrictive than the declared accessibility of the property or indexer itself. To be precise:
  - If the property or indexer has a declared accessibility of `public`, the *accessor_modifier* may be either `protected internal`, `internal`, `protected`, or `private`.
  - If the property or indexer has a declared accessibility of `protected internal`, the *accessor_modifier* may be either `internal`, `protected`, or `private`.
  - If the property or indexer has a declared accessibility of `internal` or `protected`, the *accessor_modifier* shall be `private`.
  - If the property or indexer has a declared accessibility of `private`, no *accessor_modifier* may be used.

For `abstract` and `extern` properties, the *accessor_body* for each accessor specified is simply a semicolon. A non-abstract, non-extern property may be an ***automatically implemented property***, in which case both `get` and `set` accessors shall be given, both with a semicolon body ([§15.7.4](classes.md#1574-automatically-implemented-properties)). For the accessors of any other non-abstract, non-extern property, the *accessor_body* is a *block* that specifies the statements to be executed when the corresponding accessor is invoked.

A `get` accessor corresponds to a parameterless method with a return value of the property type. Except as the target of an assignment, when a property is referenced in an expression, the `get` accessor of the property is invoked to compute the value of the property ([§12.2.2](expressions.md#1222-values-of-expressions)). The body of a `get` accessor shall conform to the rules for value-returning methods described in [§15.6.11](classes.md#15611-method-body). In particular, all `return` statements in the body of a `get` accessor shall specify an expression that is implicitly convertible to the property type. Furthermore, the endpoint of a `get` accessor shall not be reachable.

A `set` accessor corresponds to a method with a single value parameter of the property type and a `void` return type. The implicit parameter of a `set` accessor is always named value. When a property is referenced as the target of an assignment ([§12.18](expressions.md#1218-assignment-operators)), or as the operand of `++` or `–-` ([§12.7.10](expressions.md#12710-postfix-increment-and-decrement-operators), [§12.8.6](expressions.md#1286-prefix-increment-and-decrement-operators)), the `set` accessor is invoked with an argument that provides the new value ([§12.18.2](expressions.md#12182-simple-assignment)). The body of a `set` accessor shall conform to the rules for `void` methods described in [§15.6.11](classes.md#15611-method-body). In particular, return statements in the `set` accessor body are not permitted to specify an expression. Since a `set` accessor implicitly has a parameter named `value`, it is a compile-time error for a local variable or constant declaration in a `set` accessor to have that name.

Based on the presence or absence of the `get` and `set` accessors, a property is classified as follows:

-  A property that includes both a `get` accessor and a `set` accessor is said to be a ***read-write*** property.
-  A property that has only a `get` accessor is said to be a ***read-only*** property. It is a compile-time error for a read-only property to be the target of an assignment.
-  A property that has only a `set` accessor is said to be a ***write-only*** property. Except as the target of an assignment, it is a compile-time error to reference a write-only property in an expression. 

> *Note*: The pre- and postfix `++` and `--` operators and compound assignment operators cannot be applied to write-only properties, since these operators read the old value of their operand before they write the new one. *end note*

> *Example*: In the following code
> ```csharp
> public class Button: Control
> {
>     private string caption;
>     public string Caption {
>         get {
>             return caption;
>         }
>         set {
>             if (caption != value) {
>                 caption = value;
>                 Repaint();
>             }
>         }
>     }
>     public override void Paint(Graphics g, Rectangle r) {
>         // Painting code goes here
>     }
> }
> ```
> the `Button` control declares a public `Caption` property. The `get` accessor of the Caption property returns the `string` stored in the private `caption` field. The `set` accessor checks if the new value is different from the current value, and if so, it stores the new value and repaints the control. Properties often follow the pattern shown above: The `get` accessor simply returns a value stored in a `private` field, and the `set` accessor modifies that `private` field and then performs any additional actions required to update fully the state of the object.
> Given the `Button` class above, the following is an example of use of the `Caption` property:
> ```csharp
> Button okButton = new Button();
> okButton.Caption = "OK"; // Invokes set accessor
> string s = okButton.Caption; // Invokes get accessor
> ```
> Here, the `set` accessor is invoked by assigning a value to the property, and the `get` accessor is invoked by referencing the property in an expression. *end example*

The `get` and `set` accessors of a property are not distinct members, and it is not possible to declare the accessors of a property separately.

> *Example*: The example
> ```csharp
> class A
> {
>     private string name;
>     public string Name { // Error, duplicate member name
>         get { return name; }
>     }
>     public string Name { // Error, duplicate member name
>         set { name = value; }
>     }
> }
> ```
> does not declare a single read-write property. Rather, it declares two properties with the same name, one read-only and one write-only. Since two members declared in the same class cannot have the same name, the example causes a compile-time error to occur. *end example*

When a derived class declares a property by the same name as an inherited property, the derived property hides the inherited property with respect to both reading and writing.

> *Example*: In the following code
> ```csharp
> class A
> {
>     public int P {
>         set {...}
>     }
> }
> class B: A
> {
>     new public int P {
>         get {...}
>     }
> }
> ```
> the `P` property in `B` hides the `P` property in `A` with respect to both reading and writing. Thus, in the statements
> ```csharp
> B b = new B();
> b.P = 1; // Error, B.P is read-only
> ((A)b).P = 1; // Ok, reference to A.P
> ```
> the assignment to `b.P` causes a compile-time error to be reported, since the read-only `P` property in `B` hides the write-only `P` property in `A`. Note, however, that a cast can be used to access the hidden `P` property. *end example*

Unlike public fields, properties provide a separation between an object's internal state and its public interface.

> *Example*: Consider the following code, which uses a `Point` struct to represent a location:
> ```csharp
> class Label
> {
>     private int x, y;
>     private string caption;
>     public Label(int x, int y, string caption) {
>         this.x = x;
>         this.y = y;
>         this.caption = caption;
>     }
>     public int X {
>         get { return x; }
>     }
>     public int Y {
>         get { return y; }
>     }
>     public Point Location {
>         get { return new Point(x, y); }
>     }
>     public string Caption {
>        get { return caption; }
>     }
> }
> ```
> Here, the `Label` class uses two `int` fields, `x` and `y`, to store its location. The location is publicly exposed both as an `X` and a `Y` property and as a `Location` property of type `Point`. If, in a future version of `Label`, it becomes more convenient to store the location as a `Point` internally, the change can be made without affecting the public interface of the class:
> ```csharp
> class Label
> {
>     private Point location;
>     private string caption;
>     public Label(int x, int y, string caption) {
>         this.location = new Point(x, y);
>         this.caption = caption;
>     }
>     public int X {
>         get { return location.x; }
>     }
>     public int Y {
>         get { return location.y; }
>     }
>     public Point Location {
>        get { return location; }
>     }
>     public string Caption {
>         get { return caption; }
>     }
> }
> ```
> Had `x` and `y` instead been `public readonly` fields, it would have been impossible to make such a change to the `Label` class. *end example*

> *Note*: Exposing state through properties is not necessarily any less efficient than exposing fields directly. In particular, when a property is non-virtual and contains only a small amount of code, the execution environment might replace calls to accessors with the actual code of the accessors. This process is known as ***inlining***, and it makes property access as efficient as field access, yet preserves the increased flexibility of properties. *end note*

> *Example*: Since invoking a `get` accessor is conceptually equivalent to reading the value of a field, it is considered bad programming style for `get` accessors to have observable side-effects. In the example
> ```csharp
> class Counter
> {
>     private int next;
>     public int Next {
>         get { return next++; }
>     }
> }
> ```
> the value of the `Next` property depends on the number of times the property has previously been accessed. Thus, accessing the property produces an observable side effect, and the property should be implemented as a method instead.
> 
> The "no side-effects" convention for `get` accessors doesn't mean that `get` accessors should always be written simply to return values stored in fields. Indeed, `get` accessors often compute the value of a property by accessing multiple fields or invoking methods. However, a properly designed `get` accessor performs no actions that cause observable changes in the state of the object. *end example*

Properties can be used to delay initialization of a resource until the moment it is first referenced.

> *Example*:
> ```csharp
> using System.IO;
> public class Console
> {
>     private static TextReader reader;
>     private static TextWriter writer;
>     private static TextWriter error;
>     public static TextReader In {
>         get {
>             if (reader == null) {
>                 reader = new StreamReader(Console.OpenStandardInput());
>             }
>             return reader;
>         }
>     }
>     public static TextWriter Out {
>         get {
>             if (writer == null) {
>                 writer = new StreamWriter(Console.OpenStandardOutput());
>             }
>             return writer;
>         }
>     }
>     public static TextWriter Error {
>         get {
>             if (error == null) {
>                 error = new StreamWriter(Console.OpenStandardError());
>             }
>             return error;
>         }
>     }
> ...
> }
> ```
> The `Console` class contains three properties, `In`, `Out`, and `Error`, that represent the standard input, output, and error devices, respectively. By exposing these members as properties, the `Console` class can delay their initialization until they are actually used. For example, upon first referencing the `Out` property, as in
> ```csharp
> Console.Out.WriteLine("hello, world");
> ```
> the underlying `TextWriter` for the output device is created. However, if the application makes no reference to the `In` and `Error` properties, then no objects are created for those devices. *end example*

### 15.7.4 Automatically implemented properties

When a property is specified as an automatically implemented property, a hidden backing field is automatically available for the property, and the accessors are implemented to read from and write to that backing field. The hidden backing field is inaccessible, it can be read and written only through the automatically implemented property accessors, even within the containing type.

> *Example*:
> ```csharp
> public class Point {
>     public int X { get; set; } // automatically implemented
>     public int Y { get; set; } // automatically implemented
> }
> ```
> is equivalent to the following declaration:
> ```csharp
> public class Point {
>     private int x;
>     private int y;
>     public int X { get { return x; } set { x = value; } }
>     public int Y { get { return y; } set { y = value; } }
> }
> ```
> *end example*

Because the backing field is inaccessible, automatically implemented read-only or write-only properties do not make sense, and are disallowed. It is however possible to set the access level of each accessor differently. Thus, the effect of a read-only property with a private backing field can be mimicked like this:

```csharp
public class ReadOnlyPoint {
    public int X { get; private set; }
    public int Y { get; private set; }
    public ReadOnlyPoint(int x, int y) { X = x; Y = y; }
}
```

### 15.7.5 Accessibility

If an accessor has an *accessor_modifier*, the accessibility domain ([§8.5.3](basic-concepts.md#853-accessibility-domains)) of the accessor is determined using the declared accessibility of the *accessor_modifier*. If an accessor does not have an *accessor_modifier*, the accessibility domain of the accessor is determined from the declared accessibility of the property or indexer.

The presence of an *accessor_modifier* never affects member lookup ([§12.5](expressions.md#125-member-lookup)) or overload resolution ([§12.6.4](expressions.md#1264-overload-resolution)). The modifiers on the property or indexer always determine which property or indexer is bound to, regardless of the context of the access.

Once a particular property or indexer has been selected, the accessibility domains of the specific accessors involved are used to determine if that usage is valid:

-  If the usage is as a value ([§12.2.2](expressions.md#1222-values-of-expressions)), the `get` accessor shall exist and be accessible.
-  If the usage is as the target of a simple assignment ([§12.18.2](expressions.md#12182-simple-assignment)), the `set` accessor shall exist and be accessible.
-  If the usage is as the target of compound assignment ([§12.18.3](expressions.md#12183-compound-assignment)), or as the target of the `++` or `--` operators ([§12.7.10](expressions.md#12710-postfix-increment-and-decrement-operators), [§12.8.6](expressions.md#1286-prefix-increment-and-decrement-operators)), both the `get` accessors and the `set` accessor shall exist and be accessible.

> *Example*: In the following example, the property `A.Text` is hidden by the property` B.Text`, even in contexts where only the `set` accessor is called. In contrast, the property `B.Count` is not accessible to class `M`, so the accessible property `A.Count` is used instead.
> ```csharp
> class A
> {
>     public string Text {
>         get { return "hello"; }
>         set { }
>     }
>     public int Count {
>         get { return 5; }
>         set { }
>     }
> }
> 
> class B: A
> {
>     private string text = "goodbye";
>     private int count = 0;
> 
>     new public string Text {
>         get { return text; }
>         protected set { text = value; }
>     }
>     new protected int Count {
>         get { return count; }
>         set { count = value; }
>     }
> }
> class M
> {
>     static void Main() {
>         B b = new B();
>         b.Count = 12; // Calls A.Count set accessor
>         int i = b.Count; // Calls A.Count get accessor
>         b.Text = "howdy"; // Error, B.Text set accessor not accessible
>         string s = b.Text; // Calls B.Text get accessor
>     }
> }
> ```
> *end example*

An accessor that is used to implement an interface shall not have an *accessor_modifier*. If only one accessor is used to implement an interface, the other accessor may be declared with an *accessor_modifier*:

> *Example*:
> ```csharp
> public interface I
> {
>     string Prop { get; }
> }
> public class C: I
> {
>     public Prop {
>         get { return "April"; } // Must not have a modifier here
>         internal set {...} // Ok, because I.Prop has no set accessor
>     }
> }
> ```
> *end example*

### 15.7.6 Virtual, sealed, override, and abstract accessors

A virtual property declaration specifies that the accessors of the property are virtual. The `virtual` modifier applies to all non-private accessors of a property. When an accessor of a virtual property has the private *accessor_modifier*, the `private` accessor is implicitly not virtual.

An abstract property declaration specifies that the accessors of the property are virtual, but does not provide an actual implementation of the accessors. Instead, non-abstract derived classes are required to provide their own implementation for the accessors by overriding the property. Because an accessor for an abstract property declaration provides no actual implementation, its *accessor-body* simply consists of a semicolon. An abstract property shall not have a `private` accessor.

A property declaration that includes both the `abstract` and `override` modifiers specifies that the property is abstract and overrides a base property. The accessors of such a property are also abstract.

Abstract property declarations are only permitted in abstract classes ([§15.2.2.2](classes.md#15222-abstract-classes)). The accessors of an inherited virtual property can be overridden in a derived class by including a property declaration that specifies an `override` directive. This is known as an ***overriding property declaration***. An overriding property declaration does not declare a new property. Instead, it simply specializes the implementations of the accessors of an existing virtual property.

An overriding property declaration shall specify the exact same accessibility modifiers and name as the inherited property, and there shall be an identity conversion between the type of the overriding and the inherited property. If the inherited property has only a single accessor (i.e., if the inherited property is read-only or write-only), the overriding property shall include only that accessor. If the inherited property includes both accessors (i.e., if the inherited property is read-write), the overriding property can include either a single accessor or both accessors.

An overriding property declaration may include the `sealed` modifier. Use of this modifier prevents a derived class from further overriding the property. The accessors of a sealed property are also sealed.

Except for differences in declaration and invocation syntax, virtual, sealed, override, and abstract accessors behave exactly like virtual, sealed, override and abstract methods. Specifically, the rules described in [§15.6.4](classes.md#1564-virtual-methods), [§15.6.5](classes.md#1565-override-methods), [§15.6.6](classes.md#1566-sealed-methods), and [§15.6.7](classes.md#1567-abstract-methods) apply as if accessors were methods of a corresponding form:

-  A `get` accessor corresponds to a parameterless method with a return value of the property type and the same modifiers as the containing property.
-  A `set` accessor corresponds to a method with a single value parameter of the property type, a void return type, and the same modifiers as the containing property.

> *Example*: In the following code
> ```csharp
> abstract class A
> {
>     int y;
>     public virtual int X {
>         get { return 0; }
>     }
>     public virtual int Y {
>         get { return y; }
>         set { y = value; }
>     }
>     public abstract int Z { get; set; }
> }
> ```
> `X` is a virtual read-only property, `Y` is a virtual read-write property, and `Z` is an abstract read-write property. Because `Z` is abstract, the containing class A shall also be declared abstract.
>
> A class that derives from `A` is shown below:
> ```csharp
> class B: A
> {
>     int z;
>     public override int X {
>         get { return base.X + 1; }
>     }
>     public override int Y {
>         set { base.Y = value < 0? 0: value; }
>     }
>     public override int Z {
>         get { return z; }
>         set { z = value; }
>     }
> }
> ```
> Here, the declarations of `X`, `Y`, and `Z` are overriding property declarations. Each property declaration exactly matches the accessibility modifiers, type, and name of the corresponding inherited property. The `get` accessor of `X` and the `set` accessor of `Y` use the base keyword to access the inherited accessors. The declaration of `Z` overrides both abstract accessors—thus, there are no outstanding `abstract` function members in `B`, and `B` is permitted to be a non-abstract class. *end example*

When a property is declared as an override, any overridden accessors shall be accessible to the overriding code. In addition, the declared accessibility of both the property or indexer itself, and of the accessors, shall match that of the overridden member and accessors.

> *Example*:
> ```csharp
> public class B
> {
>     public virtual int P {
>         protected set {...}
>         get {...}
>     }
> }
> public class D: B
> {
>     public override int P {
>         protected set {...} // Must specify protected here
>         get {...} // Must not have a modifier here
>     }
> }
> ```
> *end example*

## 15.8 Events

### 15.8.1 General

An ***event*** is a member that enables an object or class to provide notifications. Clients can attach executable code for events by supplying ***event handlers***.

Events are declared using *event_declaration*s:

```ANTLR
event_declaration
  : attributes? event_modifier* 'event' type variable_declarators ';'
  | attributes? event_modifier* 'event' type member_name '{' event_accessor_declarations '}'
  ;

event_modifier
  : 'new'
  | 'public'
  | 'protected'
  | 'internal'
  | 'private'
  | 'static'
  | 'virtual'
  | 'sealed'
  | 'override'
  | 'abstract'
  | 'extern'
  | unsafe_modifier   // unsafe code support
  ;

event_accessor_declarations
  : add_accessor_declaration remove_accessor_declaration
  | remove_accessor_declaration add_accessor_declaration
  ;

add_accessor_declaration
  : attributes? 'add' block
  ;

remove_accessor_declaration
  : attributes? 'remove' block
  ;
```

*unsafe_modifier* ([§23.2](unsafe-code.md#232-unsafe-contexts)) is only available in unsafe code ([§23](unsafe-code.md#23-unsafe-code)).

An *event_declaration* may include a set of *attributes* ([§22](attributes.md#22-attributes)) and a valid combination of the four access modifiers ([§15.3.6](classes.md#1536-access-modifiers)), the `new` ([§15.3.5](classes.md#1535-the-new-modifier)), `static` ([§15.6.3](classes.md#1563-static-and-instance-methods), [§15.8.4](classes.md#1584-static-and-instance-events)), `virtual` ([§15.6.4](classes.md#1564-virtual-methods), [§15.8.5](classes.md#1585-virtual-sealed-override-and-abstract-accessors)), `override` ([§15.6.5](classes.md#1565-override-methods), [§15.8.5](classes.md#1585-virtual-sealed-override-and-abstract-accessors)), `sealed` ([§15.6.6](classes.md#1566-sealed-methods)), `abstract` ([§15.6.7](classes.md#1567-abstract-methods), [§15.8.5](classes.md#1585-virtual-sealed-override-and-abstract-accessors)), and `extern` ([§15.6.8](classes.md#1568-external-methods)) modifiers.

Event declarations are subject to the same rules as method declarations ([§15.6](classes.md#156-methods)) with regard to valid combinations of modifiers.

The *type* of an event declaration shall be a *delegate_type* ([§9.2.8](types.md#928-delegate-types)), and that *delegate_type* shall be at least as accessible as the event itself ([§8.5.5](basic-concepts.md#855-accessibility-constraints)).

An event declaration can include *event_accessor_declaration*s. However, if it does not, for non-extern, non-abstract events, the compiler shall supply them automatically ([§15.8.2](classes.md#1582-field-like-events)); for `extern` events, the accessors are provided externally.

An event declaration that omits *event_accessor_declaration*s defines one or more events—one for each of the *variable_declarator*s. The attributes and modifiers apply to all of the members declared by such an *event_declaration*.

It is a compile-time error for an *event_declaration* to include both the `abstract` modifier and *event_accessor_declaration*s.

When an event declaration includes an `extern` modifier, the event is said to be an ***external event***. Because an external event declaration provides no actual implementation, it is an error for it to include both the `extern` modifier and *event_accessor_declaration*s.

It is a compile-time error for a *variable_declarator* of an event declaration with an `abstract` or `external` modifier to include a *variable_initializer*.

An event can be used as the left-hand operand of the `+=` and `-=` operators. These operators are used, respectively, to attach event handlers to, or to remove event handlers from an event, and the access modifiers of the event control the contexts in which such operations are permitted.

The only operations that are permitted on an event by code that is outside the type in which that event is declared, are `+=` and `-=`. Therefore, while such code can add and remove handlers for an event, it cannot directly obtain or modify the underlying list of event handlers.

In an operation of the form `x += y` or `x –= y`, when `x` is an event the result of the operation has type `void` ([§12.18.4](expressions.md#12184-event-assignment)) (as opposed to having the type of `x`, with the value of `x` after the assignment, as for other the `+=` and `-=` operators defined on non-event types). This prevents external code from indirectly examining the underlying delegate of an event.

> *Example*: The following example shows how event handlers are attached to instances of the `Button` class:
> ```csharp
> public delegate void EventHandler(object sender, EventArgs e);
> public class Button: Control
> {
>     public event EventHandler Click;
> }
> public class LoginDialog: Form
> {
>     Button okButton;
>     Button cancelButton;
>     public LoginDialog() {
>         okButton = new Button(...);
>         okButton.Click += new EventHandler(OkButtonClick);
>         cancelButton = new Button(...);
>         cancelButton.Click += new EventHandler(CancelButtonClick);
>     }
>     void OkButtonClick(object sender, EventArgs e) {
>         // Handle okButton.Click event
>     }
>     void CancelButtonClick(object sender, EventArgs e) {
>         // Handle cancelButton.Click event
>     }
> }
> ```
> Here, the `LoginDialog` instance constructor creates two `Button` instances and attaches event handlers to the `Click` events. *end example*

### 15.8.2 Field-like events

Within the program text of the class or struct that contains the declaration of an event, certain events can be used like fields. To be used in this way, an event shall not be abstract or extern, and shall not explicitly include *event_accessor_declaration*s. Such an event can be used in any context that permits a field. The field contains a delegate ([§20](delegates.md#20-delegates)), which refers to the list of event handlers that have been added to the event. If no event handlers have been added, the field contains `null`.

> *Example*: In the following code
> ```csharp
> public delegate void EventHandler(object sender, EventArgs e);
> public class Button: Control
> {
>     public event EventHandler Click;
>     protected void OnClick(EventArgs e) {
>         EventHandler handler = Click;
>         if (handler != null)
>             handler(this, e);
>     }
>     public void Reset() {
>         Click = null;
>     }
> }
> ```
> `Click` is used as a field within the `Button` class. As the example demonstrates, the field can be examined, modified, and used in delegate invocation expressions. The `OnClick` method in the `Button` class "raises" the `Click` event. The notion of raising an event is precisely equivalent to invoking the delegate represented by the event—thus, there are no special language constructs for raising events. Note that the delegate invocation is preceded by a check that ensures the delegate is non-null and that the check is made on a local copy to ensure thread safety.
>
> Outside the declaration of the `Button` class, the `Click` member can only be used on the left-hand side of the `+=` and `–=` operators, as in
> ```csharp
> b.Click += new EventHandler(...);
> ```
> which appends a delegate to the invocation list of the `Click` event, and
> ```csharp
> Click –= new EventHandler(...);
> ```
> which removes a delegate from the invocation list of the `Click` event. *end example*

When compiling a field-like event, the compiler automatically creates storage to hold the delegate, and creates accessors for the event that add or remove event handlers to the delegate field. The addition and removal operations are thread safe, and may (but are not required to) be done while holding the lock ([§10.4.4.19](variables.md#104419-lock-statements)) in the containing object for an instance event, or the type object ([§12.7.11.7](expressions.md#127117-anonymous-object-creation-expressions)) for a static event.

> *Note*: Thus, an instance event declaration of the form:
> ```csharp
> class X
> {
>     public event D Ev;
> }
> ```
> shall be compiled to something equivalent to:
> ```csharp
> class X
> {
>     private D __Ev; // field to hold the delegate
>     public event D Ev {
>         add {
>             /* add the delegate in a thread safe way */
>         }
>         remove {
>             /* remove the delegate in a thread safe way */
>         }
>     }
> }
> ```

Within the class `X`, references to `Ev` on the left-hand side of the `+=` and `–=` operators cause the add and remove accessors to be invoked. All other references to `Ev` are compiled to reference the hidden field `__Ev` instead ([§12.7.5](expressions.md#1275-member-access)). The name "`__Ev`" is arbitrary; the hidden field could have any name or no name at all. *end note*

### 15.8.3 Event accessors

> *Note*: Event declarations typically omit *event_accessor_declaration*s, as in the `Button` example above. For example, they might be included if the storage cost of one field per event is not acceptable. In such cases, a class can include *event_accessor_declaration*s and use a private mechanism for storing the list of event handlers. *end note*

The *event_accessor_declarations* of an event specify the executable statements associated with adding and removing event handlers.

The accessor declarations consist of an *add_accessor_declaration* and a *remove_accessor_declaration*. Each accessor declaration consists of the token add or remove followed by a *block*. The *block* associated with an *add_accessor_declaration* specifies the statements to execute when an event handler is added, and the *block* associated with a *remove_accessor_declaration* specifies the statements to execute when an event handler is removed.

Each *add_accessor_declaration* and *remove_accessor_declaration* corresponds to a method with a single value parameter of the event type, and a `void` return type. The implicit parameter of an `event` accessor is named `value`. When an event is used in an event assignment, the appropriate `event` accessor is used. Specifically, if the assignment operator is `+=` then the `add` accessor is used, and if the assignment operator is `–=` then the `remove` accessor is used. In either case, the right-hand operand of the assignment operator is used as the argument to the `event` accessor. The block of an *add_accessor_declaration* or a *remove_accessor_declaration* shall conform to the rules for `void` methods described in [§15.6.9](classes.md#1569-partial-methods). In particular, `return` statements in such a block are not permitted to specify an expression.

Since an `event` accessor implicitly has a parameter named `value`, it is a compile-time error for a local variable or constant declared in an `event` accessor to have that name.

> *Example*: In the following code
> ```csharp
> class Control: Component
> {
>     // Unique keys for events
>     static readonly object mouseDownEventKey = new object();
>     static readonly object mouseUpEventKey = new object();
>     // Return event handler associated with key
>     protected Delegate GetEventHandler(object key) {...}
>     // Add event handler associated with key
>     protected void AddEventHandler(object key, Delegate handler) {...}
>     // Remove event handler associated with key
>     protected void RemoveEventHandler(object key, Delegate handler) {...}
>     // MouseDown event
>     public event MouseEventHandler MouseDown {
>         add { AddEventHandler(mouseDownEventKey, value); }
>         remove { RemoveEventHandler(mouseDownEventKey, value); }
>     }
>     // MouseUp event
>     public event MouseEventHandler MouseUp {
>         add { AddEventHandler(mouseUpEventKey, value); }
>         remove { RemoveEventHandler(mouseUpEventKey, value); }
>     }
>     // Invoke the MouseUp event
>     protected void OnMouseUp(MouseEventArgs args) {
>         MouseEventHandler handler;
>         handler = (MouseEventHandler)GetEventHandler(mouseUpEventKey);
>         if (handler != null)
>             handler(this, args);
>     }
> }
> ```
> the `Control` class implements an internal storage mechanism for events. The `AddEventHandler` method associates a delegate value with a key, the `GetEventHandler` method returns the delegate currently associated with a key, and the `RemoveEventHandler` method removes a delegate as an event handler for the specified event. Presumably, the underlying storage mechanism is designed such that there is no cost for associating a null delegate value with a key, and thus unhandled events consume no storage. *end example*

### 15.8.4 Static and instance events

When an event declaration includes a `static` modifier, the event is said to be a ***static event***. When no `static` modifier is present, the event is said to be an ***instance event***.

A static event is not associated with a specific instance, and it is a compile-time error to refer to `this` in the accessors of a static event.

An instance event is associated with a given instance of a class, and this instance can be accessed as `this` ([§12.7.8](expressions.md#1278-this-access)) in the accessors of that event.

The differences between static and instance members are discussed further in [§15.3.8](classes.md#1538-static-and-instance-members).

### 15.8.5 Virtual, sealed, override, and abstract accessors

A virtual event declaration specifies that the accessors of that event are virtual. The `virtual` modifier applies to both accessors of an event.

An abstract event declaration specifies that the accessors of the event are virtual, but does not provide an actual implementation of the accessors. Instead, non-abstract derived classes are required to provide their own implementation for the accessors by overriding the event. Because an accessor for an abstract event declaration provides no actual implementation, it shall not provide *event_accessor_declaration*s.

An event declaration that includes both the `abstract` and `override` modifiers specifies that the event is abstract and overrides a base event. The accessors of such an event are also abstract.

Abstract event declarations are only permitted in abstract classes ([§15.2.2.2](classes.md#15222-abstract-classes)).

The accessors of an inherited virtual event can be overridden in a derived class by including an event declaration that specifies an `override` modifier. This is known as an ***overriding event declaration***. An overriding event declaration does not declare a new event. Instead, it simply specializes the implementations of the accessors of an existing virtual event.

An overriding event declaration shall specify the exact same accessibility modifiers and name as the overridden event, there shall be an identity conversion between the type of the overriding and the overridden event, and both the `add` and `remove` accessors shall be specified within the declaration.

An overriding event declaration can include the `sealed` modifier. Use of `this` modifier prevents a derived class from further overriding the event. The accessors of a sealed event are also sealed.

It is a compile-time error for an overriding event declaration to include a `new` modifier.

Except for differences in declaration and invocation syntax, virtual, sealed, override, and abstract accessors behave exactly like virtual, sealed, override and abstract methods. Specifically, the rules described in [§15.6.4](classes.md#1564-virtual-methods), [§15.6.5](classes.md#1565-override-methods), [§15.6.6](classes.md#1566-sealed-methods), and [§15.6.7](classes.md#1567-abstract-methods) apply as if accessors were methods of a corresponding form. Each accessor corresponds to a method with a single value parameter of the event type, a `void` return type, and the same modifiers as the containing event.

## 15.9 Indexers

An ***indexer*** is a member that enables an object to be indexed in the same way as an array. Indexers are declared using *indexer_declaration*s:

```ANTLR
indexer_declaration
    : attributes? indexer_modifier* indexer_declarator indexer_body
    ;

indexer_modifier
  : 'new'
  | 'public'
  | 'protected'
  | 'internal'
  | 'private'
  | 'virtual'
  | 'sealed'
  | 'override'
  | 'abstract'
  | 'extern'
  | unsafe_modifier   // unsafe code support
  ;

indexer_declarator
  : type 'this' '[' formal_parameter_list ']'
  | type interface_type '.' 'this' '[' formal_parameter_list ']'
  ;

indexer_body
    : '{' accessor_declarations '}' 
    | '=>' expression ';'
    ;  
```

*unsafe_modifier* ([§23.2](unsafe-code.md#232-unsafe-contexts)) is only available in unsafe code ([§23](unsafe-code.md#23-unsafe-code)).

An *indexer_declaration* may include a set of *attributes* ([§22](attributes.md#22-attributes)) and a valid combination of the four access modifiers ([§15.3.6](classes.md#1536-access-modifiers)), the `new` ([§15.3.5](classes.md#1535-the-new-modifier)), `virtual` ([§15.6.4](classes.md#1564-virtual-methods)), `override` ([§15.6.5](classes.md#1565-override-methods)), `sealed` ([§15.6.6](classes.md#1566-sealed-methods)), `abstract` ([§15.6.7](classes.md#1567-abstract-methods)), and `extern` ([§15.6.8](classes.md#1568-external-methods)) modifiers.

Indexer declarations are subject to the same rules as method declarations ([§15.6](classes.md#156-methods)) with regard to valid combinations of modifiers, with the one exception being that the `static` modifier is not permitted on an indexer declaration.

The modifiers `virtual`, `override`, and `abstract` are mutually exclusive except in one case. The `abstract` and `override` modifiers may be used together so that an abstract indexer can override a virtual one.

The *type* of an indexer declaration specifies the element type of the indexer introduced by the declaration. 

> *Note:* As indexers are designed to be used in array element-like contexts, the term *element type* as defined for an array is also used with an indexer. *end note*

Unless the indexer is an explicit interface member implementation, the *type* is followed by the keyword `this`. For an explicit interface member implementation, the *type* is followed by an *interface_type*, a "`.`", and the keyword `this`. Unlike other members, indexers do not have user-defined names.

The *formal_parameter_list* specifies the parameters of the indexer. The formal parameter list of an indexer corresponds to that of a method ([§15.6.2](classes.md#1562-method-parameters)), except that at least one parameter shall be specified, and that the `this`, `ref`, and `out` parameter modifiers are not permitted.

The *type* of an indexer and each of the types referenced in the *formal_parameter_list* shall be at least as accessible as the indexer itself ([§8.5.5](basic-concepts.md#855-accessibility-constraints)).

An *indexer_body* may either consist of an accessor body ([§15.7.1](classes.md#1571-general)) or an expression body ([§15.6.1](classes.md#1561-general)). In an accessor body, *accessor_declarations*, which shall be enclosed in "`{`" and "`}`" tokens, declare the accessors ([§15.7.3](classes.md#1573-accessors)) of the indexer. The accessors specify the executable statements associated with reading and writing indexer elements.

An expression body consisting of "`=>`" followed by an expression `E` and a semicolon is exactly equivalent to the block body `{ get { return E; } }`, and can therefore only be used to specify getter-only indexers where the result of the getter is given by a single expression.

Even though the syntax for accessing an indexer element is the same as that for an array element, an indexer element is not classified as a variable. Thus, it is not possible to pass an indexer element as a `ref` or `out` argument.

The *formal_parameter_list* of an indexer defines the signature ([§8.6](basic-concepts.md#86-signatures-and-overloading)) of the indexer. Specifically, the signature of an indexer consists of the number and types of its formal parameters. The element type and names of the formal parameters are not part of an indexer's signature.

The signature of an indexer shall differ from the signatures of all other indexers declared in the same class.

Indexers and properties are very similar in concept, but differ in the following ways:

-  A property is identified by its name, whereas an indexer is identified by its signature.
-  A property is accessed through a *simple_name* ([§12.7.3](expressions.md#1273-simple-names)) or a *member_access* ([§12.7.5](expressions.md#1275-member-access)), whereas an indexer element is accessed through an *element_access* ([§12.7.7.3](expressions.md#12773-indexer-access)).
-  A property can be a static member, whereas an indexer is always an instance member.
-  A `get` accessor of a property corresponds to a method with no parameters, whereas a `get` accessor of an indexer corresponds to a method with the same formal parameter list as the indexer.
-  A `set` accessor of a property corresponds to a method with a single parameter named value, whereas a `set` accessor of an indexer corresponds to a method with the same formal parameter list as the indexer, plus an additional parameter named value.
-  It is a compile-time error for an indexer accessor to declare a local variable or local constant with the same name as an indexer parameter.
-  In an overriding property declaration, the inherited property is accessed using the syntax `base.P`, where `P` is the property name. In an overriding indexer declaration, the inherited indexer is accessed using the syntax `base[E]`, where `E` is a comma-separated list of expressions.
-  There is no concept of an "automatically implemented indexer". It is an error to have a non-abstract, non-external indexer with semicolon accessors.

Aside from these differences, all rules defined in [§15.7.3](classes.md#1573-accessors) and [§15.7.4](classes.md#1574-automatically-implemented-properties) apply to indexer accessors as well as to property accessors.

When an indexer declaration includes an `extern` modifier, the indexer is said to be an ***external indexer***. Because an external indexer declaration provides no actual implementation, each of its *accessor_declarations* consists of a semicolon.

> *Example*: The example below declares a `BitArray` class that implements an indexer for accessing the individual bits in the bit array.
> ```csharp
> using System;
> class BitArray
> {
>     int[] bits;
>     int length;
>     public BitArray(int length) {
>         if (length < 0) throw new ArgumentException();
>         bits = new int[((length - 1) >> 5) + 1];
>         this.length = length;
>     }
>     public int Length {
>         get { return length; }
>     }
>     public bool this[int index] {
>         get {
>             if (index < 0 || index >= length) {
>                 throw new IndexOutOfRangeException();
>             }
>             return (bits[index >> 5] & 1 << index) != 0;
>         }
>         set {
>             if (index < 0 || index >= length) {
>                 throw new IndexOutOfRangeException();
>             }
>             if (value) {
>                 bits[index >> 5] |= 1 << index;
>             }
>             else {
>                 bits[index >> 5] &= ~(1 << index);
>             }
>         }
>     }
> }
> ```
> An instance of the `BitArray` class consumes substantially less memory than a corresponding `bool[]` (since each value of the former occupies only one bit instead of the latter's one `byte`), but it permits the same operations as a `bool[]`.
> 
> The following `CountPrimes` class uses a `BitArray` and the classical "sieve" algorithm to compute the number of primes between 2 and a given maximum:
> ```csharp
> class CountPrimes
> {
>     static int Count(int max) {
>         BitArray flags = new BitArray(max + 1);
>         int count = 0;
>         for (int i = 2; i <= max; i++) {
>             if (!flags[i]) {
>                 for (int j = i * 2; j <= max; j += i) flags[j] = true;
>                 count++;
>             }
>         }
>         return count;
>     }
>     static void Main(string[] args) {
>         int max = int.Parse(args[0]);
>         int count = Count(max);
>         Console.WriteLine(
>         "Found {0} primes between 2 and {1}", count, max);
>     }
> }
> ```
> Note that the syntax for accessing elements of the `BitArray` is precisely the same as for a `bool[]`. *end example*

> *Example*: The following example shows a 26×10 grid class that has an indexer with two parameters. The first parameter is required to be an upper- or lowercase letter in the range A–Z, and the second is required to be an integer in the range 0–9.
> ```csharp
> using System;
> class Grid
> {
>     const int NumRows = 26;
>     const int NumCols = 10;
>     int[,] cells = new int[NumRows, NumCols];
>     public int this[char row, int col]
>     {
>         get {
>             row = Char.ToUpper(row);
>             if (row < 'A' || row > 'Z') {
>                 throw new ArgumentOutOfRangeException("row");
>             }
>             if (col < 0 || col >= NumCols) {
>                 throw new ArgumentOutOfRangeException ("col");
>             }
>             return cells[row - 'A', col];
>         }
>         set {
>             row = Char.ToUpper(row);
>             if (row < 'A' || row > 'Z') {
>                 throw new ArgumentOutOfRangeException ("row");
>             }
>             if (col < 0 || col >= NumCols) {
>                 throw new ArgumentOutOfRangeException ("col");
>             }
>             cells[row - 'A', col] = value;
>         }
>     }
> }
> ```
> *end example*

## 15.10 Operators

### 15.10.1 General

An ***operator*** is a member that defines the meaning of an expression operator that can be applied to instances of the class. Operators are declared using *operator_declaration*s:

```ANTLR
operator_declaration
  : attributes? operator_modifier+ operator_declarator operator_body
  ;

operator_modifier
  : 'public'
  | 'static'
  | 'extern'
  | unsafe_modifier   // unsafe code support
  ;

operator_declarator
  : unary_operator_declarator
  | binary_operator_declarator
  | conversion_operator_declarator
  ;

unary_operator_declarator
  : type 'operator' overloadable_unary_operator '(' fixed_parameter ')'
  ;

overloadable_unary_operator
  : '+' | '-' | '!' | '~' | '++' | '--' | 'true' | 'false'
  ;

binary_operator_declarator
  : type 'operator' overloadable_binary_operator '(' fixed_parameter ',' fixed_parameter ')'
  ;

overloadable_binary_operator
  : '+'  | '-'  | '*'  | '/'  | '%'  | '&' | '|' | '^'  | '<<' 
  | Right_Shift | '==' | '!=' | '>' | '<' | '>=' | '<='
  ;

conversion_operator_declarator
  : 'implicit' 'operator' type '(' fixed_parameter ')'
  | 'explicit' 'operator' type '(' fixed_parameter ')'
  ;

operator_body
  : block
  | '=>' expression ';'
  | ';'
  ;

```

*unsafe_modifier* ([§23.2](unsafe-code.md#232-unsafe-contexts)) is only available in unsafe code ([§23](unsafe-code.md#23-unsafe-code)).

There are three categories of overloadable operators: Unary operators ([§15.10.2](classes.md#15102-unary-operators)), binary operators ([§15.10.3](classes.md#15103-binary-operators)), and conversion operators ([§15.10.4](classes.md#15104-conversion-operators)).

The *operator_body* is either a semicolon, a block body ([§15.6.1](classes.md#1561-general)) or an expression body ([§15.6.1](classes.md#1561-general)). A block body consists of a *block*, which specifies the statements to execute when the operator is invoked. The *block* shall conform to the rules for value-returning methods described in [§15.6.11](classes.md#15611-method-body). An expression body consists of `=>` followed by an expression and a semicolon, and denotes a single expression to perform when the operator is invoked.

For `extern` operators, the *operator_body* consists simply of a semicolon. For all other operators, the *operator_body* is either a block body or an expression body.

The following rules apply to all operator declarations:

-  An operator declaration shall include both a `public` and a `static` modifier.
-  The parameter(s) of an operator shall have no modifiers.
-  The signature of an operator ([§15.10.2](classes.md#15102-unary-operators), [§15.10.3](classes.md#15103-binary-operators), [§15.10.4](classes.md#15104-conversion-operators)) shall differ from the signatures of all other operators declared in the same class.
-  All types referenced in an operator declaration shall be at least as accessible as the operator itself ([§8.5.5](basic-concepts.md#855-accessibility-constraints)).
-  It is an error for the same modifier to appear multiple times in an operator declaration.

Each operator category imposes additional restrictions, as described in the following subclauses.

Like other members, operators declared in a base class are inherited by derived classes. Because operator declarations always require the class or struct in which the operator is declared to participate in the signature of the operator, it is not possible for an operator declared in a derived class to hide an operator declared in a base class. Thus, the `new` modifier is never required, and therefore never permitted, in an operator declaration.

Additional information on unary and binary operators can be found in [§12.4](expressions.md#124-operators).

Additional information on conversion operators can be found in [§11.5](conversions.md#115-user-defined-conversions).

### 15.10.2 Unary operators

The following rules apply to unary operator declarations, where `T` denotes the instance type of the class or struct that contains the operator declaration:

-  A unary `+`, `-`, `!`, or `~` operator shall take a single parameter of type `T` or `T?` and can return any type.
-  A unary `++` or `--` operator shall take a single parameter of type `T` or `T?` and shall return that same type or a type derived from it.
-  A unary `true` or `false` operator shall take a single parameter of type `T` or `T?` and shall return type `bool`.

The signature of a unary operator consists of the operator token (`+`, `-`, `!`, `~`, `++`, `--`, `true`, or `false`) and the type of the single formal parameter. The return type is not part of a unary operator's signature, nor is the name of the formal parameter.

The `true` and `false` unary operators require pair-wise declaration. A compile-time error occurs if a class declares one of these operators without also declaring the other. The `true` and `false` operators are described further in [§12.21](expressions.md#1221-boolean-expressions).

> *Example*: The following example shows an implementation and subsequent usage of operator++ for an integer vector class:
> ```csharp
> public class IntVector
> {
>     public IntVector(int length) {...}
>     public int Length { ... } // read-only property
>     public int this[int index] { ... } // read-write indexer
>     public static IntVector operator++(IntVector iv) {
>         IntVector temp = new IntVector(iv.Length);
>         for (int i = 0; i < iv.Length; i++)
>         temp[i] = iv[i] + 1;
>         return temp;
>     }
> }
> 
> class Test
> {
>     static void Main() {
>         IntVector iv1 = new IntVector(4); // vector of 4 x 0
>         IntVector iv2;
>         iv2 = iv1++; // iv2 contains 4 x 0, iv1 contains 4 x 1
>         iv2 = ++iv1; // iv2 contains 4 x 2, iv1 contains 4 x 2
>     }
> }
> ```
> Note how the operator method returns the value produced by adding 1 to the operand, just like the postfix increment and decrement operators ([§12.7.10](expressions.md#12710-postfix-increment-and-decrement-operators)), and the prefix increment and decrement operators ([§12.8.6](expressions.md#1286-prefix-increment-and-decrement-operators)). Unlike in C++, this method should not modify the value of its operand directly as this would violate the standard semantics of the postfix increment operator ([§12.8.6](expressions.md#1286-prefix-increment-and-decrement-operators)). *end example*

### 15.10.3 Binary operators

The following rules apply to binary operator declarations, where `T` denotes the instance type of the class or struct that contains the operator declaration:

-  A binary non-shift operator shall take two parameters, at least one of which shall have type `T` or `T?`, and can return any type.
-  A binary `<<` or `>>` operator ([§12.10](expressions.md#1210-shift-operators)) shall take two parameters, the first of which shall have type `T` or T? and the second of which shall have type `int` or `int?`, and can return any type.
The signature of a binary operator consists of the operator token (`+`, `-`, `*`, `/`, `%`, `&`, `|`, `^`, `<<`, `>>`, `==`, `!=`, `>`, `<`, `>=`, or `<=`) and the types of the two formal parameters. The return type and the names of the formal parameters are not part of a binary operator's signature.

Certain binary operators require pair-wise declaration. For every declaration of either operator of a pair, there shall be a matching declaration of the other operator of the pair. Two operator declarations match if identity conversions exist between their return types and their corresponding parameter types. The following operators require pair-wise declaration:

-  operator `==` and operator `!=`
-  operator `>` and operator `<`
-  operator `>=` and operator `<=`

### 15.10.4 Conversion operators

A conversion operator declaration introduces a ***user-defined conversion*** ([§11.5](conversions.md#115-user-defined-conversions)), which augments the pre-defined implicit and explicit conversions.

A conversion operator declaration that includes the `implicit` keyword introduces a user-defined implicit conversion. Implicit conversions can occur in a variety of situations, including function member invocations, cast expressions, and assignments. This is described further in [§11.2](conversions.md#112-implicit-conversions).

A conversion operator declaration that includes the `explicit` keyword introduces a user-defined explicit conversion. Explicit conversions can occur in cast expressions, and are described further in [§11.3](conversions.md#113-explicit-conversions).

A conversion operator converts from a source type, indicated by the parameter type of the conversion operator, to a target type, indicated by the return type of the conversion operator.

For a given source type `S` and target type `T`, if `S` or `T` are nullable value types, let `S₀` and `T₀` refer to their underlying types; otherwise, `S₀` and `T₀` are equal to `S` and `T` respectively. A class or struct is permitted to declare a conversion from a source type `S` to a target type `T` only if all of the following are true:

-  `S₀` and `T₀` are different types.

-  Either `S₀` or `T₀` is the instance type of the class or struct that contains the operator declaration.

-  Neither `S₀` nor `T₀` is an *interface_type*.

-  Excluding user-defined conversions, a conversion does not exist from `S` to `T` or from `T` to `S`.

For the purposes of these rules, any type parameters associated with `S` or `T` are considered to be unique types that have no inheritance relationship with other types, and any constraints on those type parameters are ignored.

> *Example*: In the following:
> ```csharp
> class C<T> {...}
> class D<T>: C<T>
> {
>     public static implicit operator C<int>(D<T> value) {...} // Ok
>     public static implicit operator C<string>(D<T> value) {...} // Ok
>     public static implicit operator C<T>(D<T> value) {...} // Error
> }
> ```
> the first two operator declarations are permitted because `T` and `int` and `string`, respectively are considered unique types with no relationship. However, the third operator is an error because `C<T>` is the base class of `D<T>`. *end example*

From the second rule, it follows that a conversion operator shall convert either to or from the class or struct type in which the operator is declared.

> *Example*: It is possible for a class or struct type `C` to define a conversion from `C` to `int` and from `int` to `C`, but not from `int` to `bool`. *end example*

It is not possible to directly redefine a pre-defined conversion. Thus, conversion operators are not allowed to convert from or to `object` because implicit and explicit conversions already exist between `object` and all other types. Likewise, neither the source nor the target types of a conversion can be a base type of the other, since a conversion would then already exist. However, it *is* possible to declare operators on generic types that, for particular type arguments, specify conversions that already exist as pre-defined conversions.

> *Example*:
> ```csharp
> struct Convertible<T>
> {
>     public static implicit operator Convertible<T>(T value) {...}
>     public static explicit operator T(Convertible<T> value) {...}
> }
> ```
> when type `object` is specified as a type argument for `T`, the second operator declares a conversion that already exists (an implicit, and therefore also an explicit, conversion exists from any type to type object). *end example*

In cases where a pre-defined conversion exists between two types, any user-defined conversions between those types are ignored. Specifically:

-  If a pre-defined implicit conversion ([§11.2](conversions.md#112-implicit-conversions)) exists from type `S` to type `T`, all user-defined conversions (implicit or explicit) from `S` to `T` are ignored.
-  If a pre-defined explicit conversion ([§11.3](conversions.md#113-explicit-conversions)) exists from type `S` to type `T`, any user-defined explicit conversions from `S` to `T` are ignored. Furthermore:
    -  If either `S` or `T` is an interface type, user-defined implicit conversions from `S` to `T` are ignored.
    -  Otherwise, user-defined implicit conversions from `S` to `T` are still considered.

For all types but object, the operators declared by the `Convertible<T>` type above do not conflict with pre-defined conversions.

> *Example*:
> ```csharp
> void F(int i, Convertible<int> n) {
>     i = n; // Error
>     i = (int)n; // User-defined explicit conversion
>     n = i; // User-defined implicit conversion
>     n = (Convertible<int>)i; // User-defined implicit conversion
> }
> ```
> However, for type object, pre-defined conversions hide the user-defined conversions in all cases but one:
> ```csharp
> void F(object o, Convertible<object> n) {
>     o = n; // Pre-defined boxing conversion
>     o = (object)n; // Pre-defined boxing conversion
>     n = o; // User-defined implicit conversion
>     n = (Convertible<object>)o; // Pre-defined unboxing conversion
> }
> ```
> *end example*

User-defined conversions are not allowed to convert from or to *interface_type*s. In particular, this restriction ensures that no user-defined transformations occur when converting to an *interface_type*, and that a conversion to an *interface_type* succeeds only if the `object` being converted actually implements the specified *interface_type*.

The signature of a conversion operator consists of the source type and the target type. (This is the only form of member for which the return type participates in the signature.) The implicit or explicit classification of a conversion operator is not part of the operator's signature. Thus, a class or struct cannot declare both an implicit and an explicit conversion operator with the same source and target types.

> *Note*: In general, user-defined implicit conversions should be designed to never throw exceptions and never lose information. If a user-defined conversion can give rise to exceptions (for example, because the source argument is out of range) or loss of information (such as discarding high-order bits), then that conversion should be defined as an explicit conversion. *end note*

> *Example*: In the following code
> ```csharp
> using System;
> public struct Digit
> {
>     byte value;
>     public Digit(byte value) {
>         if (value < 0 || value > 9) throw new ArgumentException();
>         this.value = value;
>     }
>     public static implicit operator byte(Digit d) {
>         return d.value;
>     }
>     public static explicit operator Digit(byte b) {
>         return new Digit(b);
>     }
> }
> ```
> the conversion from `Digit` to `byte` is implicit because it never throws exceptions or loses information, but the conversion from `byte` to `Digit` is explicit since `Digit` can only represent a subset of the possible values of a `byte`. *end example*

## 15.11 Instance constructors

### 15.11.1 General

An ***instance constructor*** is a member that implements the actions required to initialize an instance of a class. Instance constructors are declared using *constructor_declaration*s:

```ANTLR
constructor_declaration
  : attributes? constructor_modifier* constructor_declarator constructor_body
  ;

constructor_modifier
  : 'public'
  | 'protected'
  | 'internal'
  | 'private'
  | 'extern'
  | unsafe_modifier   // unsafe code support
  ;

constructor_declarator
  : Identifier '(' formal_parameter_list? ')' constructor_initializer?
  ;

constructor_initializer
  : ':' 'base' '(' argument_list? ')'
  | ':' 'this' '(' argument_list? ')'
  ;

constructor_body
  : block
  | ';'
  ;
```

*unsafe_modifier* ([§23.2](unsafe-code.md#232-unsafe-contexts)) is only available in unsafe code ([§23](unsafe-code.md#23-unsafe-code)).

A *constructor_declaration* may include a set of *attributes* ([§22](attributes.md#22-attributes)), a valid combination of the four access modifiers ([§15.3.6](classes.md#1536-access-modifiers)), and an `extern` ([§15.6.8](classes.md#1568-external-methods)) modifier. A constructor declaration is not permitted to include the same modifier multiple times.

The *Identifier* of a *constructor_declarator* shall name the class in which the instance constructor is declared. If any other name is specified, a compile-time error occurs.

The optional *formal_parameter_list* of an instance constructor is subject to the same rules as the *formal_parameter_list* of a method ([§15.6](classes.md#156-methods)). As the `this` modifier for parameters only applies to extension methods ([§15.6.10](classes.md#15610-extension-methods)), no parameter in a constructor's *formal_parameter_list* shall contain the `this` modifier. The formal parameter list defines the signature ([§8.6](basic-concepts.md#86-signatures-and-overloading)) of an instance constructor and governs the process whereby overload resolution ([§12.6.4](expressions.md#1264-overload-resolution)) selects a particular instance constructor in an invocation.

Each of the types referenced in the *formal_parameter_list* of an instance constructor shall be at least as accessible as the constructor itself ([§8.5.5](basic-concepts.md#855-accessibility-constraints)).

The optional *constructor_initializer* specifies another instance constructor to invoke before executing the statements given in the *constructor_body* of this instance constructor. This is described further in [§15.11.2](classes.md#15112-constructor-initializers).

When a constructor declaration includes an `extern` modifier, the constructor is said to be an ***external constructor***. Because an external constructor declaration provides no actual implementation, its *constructor-body* consists of a semicolon. For all other constructors, the *constructor_body* consists of a *block*, which specifies the statements to initialize a new instance of the class. This corresponds exactly to the *block* of an instance method with a `void` return type ([§15.6.11](classes.md#15611-method-body)).

Instance constructors are not inherited. Thus, a class has no instance constructors other than those actually declared in the class, with the exception that if a class contains no instance constructor declarations, a default instance constructor is automatically provided ([§15.11.5](classes.md#15115-default-constructors)).

Instance constructors are invoked by *object_creation_expression*s ([§12.7.11.2](expressions.md#127112-object-creation-expressions)) and through *constructor_initializer*s.

### 15.11.2 Constructor initializers

All instance constructors (except those for class` object`) implicitly include an invocation of another instance constructor immediately before the *constructor_body*. The constructor to implicitly invoke is determined by the *constructor_initializer*:

-  An instance constructor initializer of the form `base(`*argument_list*`)` (where *argument_list* is optional) causes an instance constructor from the direct base class to be invoked. That constructor is selected using *argument_list* and the overload resolution rules of [§12.6.4](expressions.md#1264-overload-resolution). The set of candidate instance constructors consists of all the accessible instance constructors of the direct base class. If this set is empty, or if a single best instance constructor cannot be identified, a compile-time error occurs.
-  An instance constructor initializer of the form `this(`*argument_list*`)` (where *argument_list* is optional) invokes another instance constructor from the same class. The constructor is selected using *argument_list* and the overload resolution rules of [§12.6.4](expressions.md#1264-overload-resolution). The set of candidate instance constructors consists of all instance constructors declared in the class itself. If the resulting set of applicable instance constructors is empty, or if a single best instance constructor cannot be identified, a compile-time error occurs. If an instance constructor declaration invokes itself through a chain of one or more constructor initializers, a compile-time error occurs.

If an instance constructor has no constructor initializer, a constructor initializer of the form `base()` is implicitly provided. 

> *Note*: Thus, an instance constructor declaration of the form
> ```csharp
> C(...) {...}
> ```
> is exactly equivalent to
> ```csharp
> C(...): base() {...}
> ```
> *end note*

The scope of the parameters given by the *formal_parameter_list* of an instance constructor declaration includes the constructor initializer of that declaration. Thus, a constructor initializer is permitted to access the parameters of the constructor.

> *Example*:
> ```csharp
> class A
> {
>     public A(int x, int y) {}
> }
> 
> class B: A
> {
>     public B(int x, int y): base(x + y, x - y) {}
> }
> ```
> *end example*

An instance constructor initializer cannot access the instance being created. Therefore it is a compile-time error to reference this in an argument expression of the constructor initializer, as it is a compile-time error for an argument expression to reference any instance member through a *simple_name*.

### 15.11.3 Instance variable initializers

When an instance constructor has no constructor initializer, or it has a constructor initializer of the form `base(...)`, that constructor implicitly performs the initializations specified by the *variable_initializer*s of the instance fields declared in its class. This corresponds to a sequence of assignments that are executed immediately upon entry to the constructor and before the implicit invocation of the direct base class constructor. The variable initializers are executed in the textual order in which they appear in the class declaration ([§15.5.6](classes.md#1556-variable-initializers)).

### 15.11.4 Constructor execution

Variable initializers are transformed into assignment statements, and these assignment statements are executed *before* the invocation of the base class instance constructor. This ordering ensures that all instance fields are initialized by their variable initializers before *any* statements that have access to that instance are executed.

> *Example*: Given the following:
> ```csharp
> using System;
> class A
> {
>     public A() {
>         PrintFields();
>     }
>     public virtual void PrintFields() {}
> }
> class B: A
> {
>     int x = 1;
>     int y;
>     public B() {
>         y = -1;
>     }
>     public override void PrintFields() {
>         Console.WriteLine("x = {0}, y = {1}", x, y);
>     }
> }
> ```
> when new `B()` is used to create an instance of `B`, the following output is produced:
> ```console
> x = 1, y = 0
> ```
> The value of `x` is 1 because the variable initializer is executed before the base class instance constructor is invoked. However, the value of `y` is 0 (the default value of an `int`) because the assignment to `y` is not executed until after the base class constructor returns.
> It is useful to think of instance variable initializers and constructor initializers as statements that are automatically inserted before the *constructor_body*. The example
> ```csharp
> using System;
> using System.Collections;
> class A
> {
>     int x = 1, y = -1, count;
>     public A() {
>         count = 0;
>     }
>     public A(int n) {
>         count = n;
>     }
> }
> class B: A
> {
>     double sqrt2 = Math.Sqrt(2.0);
>     ArrayList items = new ArrayList(100);
>     int max;
>     public B(): this(100) {
>         items.Add("default");
>     }
>     public B(int n): base(n – 1) {
>         max = n;
>     }
> }
> ```
> contains several variable initializers; it also contains constructor initializers of both forms (`base` and `this`). The example corresponds to the code shown below, where each comment indicates an automatically inserted statement (the syntax used for the automatically inserted constructor invocations isn't valid, but merely serves to illustrate the mechanism).
> ```csharp
> using System.Collections;
> class A
> {
>     int x, y, count;
>     public A() {
>         x = 1; // Variable initializer
>         y = -1; // Variable initializer
>         object(); // Invoke object() constructor
>         count = 0;
>     }
>     public A(int n) {
>         x = 1; // Variable initializer
>         y = -1; // Variable initializer
>         object(); // Invoke object() constructor
>         count = n;
>     }
> }
> class B: A
>     {
>     double sqrt2;
>     ArrayList items;
>     int max;
>     public B(): this(100) {
>         B(100); // Invoke B(int) constructor
>         items.Add("default");
>     }
>     public B(int n): base(n – 1) {
>         sqrt2 = Math.Sqrt(2.0); // Variable initializer
>         items = new ArrayList(100); // Variable initializer
>         A(n – 1); // Invoke A(int) constructor
>         max = n;
>     }
> }
> ```
> *end example*

### 15.11.5 Default constructors

If a class contains no instance constructor declarations, a default instance constructor is automatically provided. That default constructor simply invokes a constructor of the direct base class, as if it had a constructor initializer of the form `base()`. If the class is abstract then the declared accessibility for the default constructor is protected. Otherwise, the declared accessibility for the default constructor is public. 

> *Note*: Thus, the default constructor is always of the form
> ```csharp
> protected C(): base() {}
> ```
> or
> ```csharp
> public C(): base() {}
> ```
> where `C` is the name of the class. *end note* 

If overload resolution is unable to determine a unique best candidate for the base-class constructor initializer then a compile-time error occurs.

> *Example*: In the following code
> ```csharp
> class Message
> {
>     object sender;
>     string text;
> }
> ```
> a default constructor is provided because the class contains no instance constructor declarations. Thus, the example is precisely equivalent to
> ```csharp
> class Message
> {
>     object sender;
>     string text;
>     public Message(): base() {}
> }
> ```
> *end example*

## 15.12 Static constructors

A ***static constructor*** is a member that implements the actions required to initialize a closed class. Static constructors are declared using *static_constructor_declaration*s:

```ANTLR
static_constructor_declaration
  : attributes? static_constructor_modifiers Identifier '(' ')' static_constructor_body
  ;

static_constructor_modifiers
  : 'static'
  | 'static' 'extern' unsafe_modifier?
  | 'static' unsafe_modifier 'extern'?
  | 'extern' 'static' unsafe_modifier?
  | 'extern' unsafe_modifier 'static'
  | unsafe_modifier 'static' 'extern'?
  | unsafe_modifier 'extern' 'static'
  ;

static_constructor_body
  : block
  | ';'
  ;
```

*unsafe_modifier* ([§23.2](unsafe-code.md#232-unsafe-contexts)) is only available in unsafe code ([§23](unsafe-code.md#23-unsafe-code)).

A *static_constructor_declaration* may include a set of *attributes* ([§22](attributes.md#22-attributes)) and an `extern` modifier ([§15.6.8](classes.md#1568-external-methods)).

The *Identifier* of a *static_constructor_declaration* shall name the class in which the static constructor is declared. If any other name is specified, a compile-time error occurs.

When a static constructor declaration includes an `extern` modifier, the static constructor is said to be an ***external static constructor***. Because an external static constructor declaration provides no actual implementation, its *static_constructor_body* consists of a semicolon. For all other static constructor declarations, the *static_constructor_body* consists of a *block*, which specifies the statements to execute in order to initialize the class. This corresponds exactly to the *method_body* of a static method with a `void` return type ([§15.6.11](classes.md#15611-method-body)).

Static constructors are not inherited, and cannot be called directly.

The static constructor for a closed class executes at most once in a given application domain. The execution of a static constructor is triggered by the first of the following events to occur within an application domain:

-  An instance of the class is created.
-  Any of the static members of the class are referenced.

If a class contains the `Main` method ([§8.1](basic-concepts.md#81-application-startup)) in which execution begins, the static constructor for that class executes before the `Main` method is called.

To initialize a new closed class type, first a new set of static fields ([§15.5.2](classes.md#1552-static-and-instance-fields)) for that particular closed type is created. Each of the static fields is initialized to its default value ([§15.5.5](classes.md#1555-field-initialization)). Next, the static field initializers ([§15.5.6.2](classes.md#15562-static-field-initialization)) are executed for those static fields. Finally, the static constructor is executed.

> *Example*: The example
> ```csharp
> using System;
> class Test
> {
>     static void Main() {
>         A.F();
>         B.F();
>     }
> }
> class A
> {
>     static A() {
>         Console.WriteLine("Init A");
>     }
>     public static void F() {
>         Console.WriteLine("A.F");
>     }
> }
> class B
> {
>     static B() {
>         Console.WriteLine("Init B");
>     }
>     public static void F() {
>         Console.WriteLine("B.F");
>     }
> }
> ```
> must produce the output:
> ```console
> Init A
> A.F
> Init B
> B.F
> ```
> because the execution of `A`'s static constructor is triggered by the call to `A.F`, and the execution of `B`'s static constructor is triggered by the call to `B.F`. *end example*

It is possible to construct circular dependencies that allow static fields with variable initializers to be observed in their default value state.

> *Example*: The example
> ```csharp
> using System;
> class A
> {
>     public static int X;
>     static A() {
>         X = B.Y + 1;
>     }
> }
> class B
> {
>     public static int Y = A.X + 1;
>     static B() {}
>         static void Main() {
>         Console.WriteLine("X = {0}, Y = {1}", A.X, B.Y);
>     }
> }
> ```
> produces the output
> 
> ```console
> X = 1, Y = 2
> ```
> To execute the `Main` method, the system first runs the initializer for `B.Y`, prior to class `B`'s static constructor. `Y`'s initializer causes `A`'s `static` constructor to be run because the value of `A.X` is referenced. The static constructor of `A` in turn proceeds to compute the value of `X`, and in doing so fetches the default value of `Y`, which is zero. `A.X` is thus initialized to 1. The process of running `A`'s static field initializers and static constructor then completes, returning to the calculation of the initial value of `Y`, the result of which becomes 2. *end example*

Because the static constructor is executed exactly once for each closed constructed class type, it is a convenient place to enforce run-time checks on the type parameter that cannot be checked at compile-time via constraints ([§15.2.5](classes.md#1525-type-parameter-constraints)).

> *Example*: The following type uses a static constructor to enforce that the type argument is an enum:
> ```csharp
> class Gen<T> where T: struct
> {
>     static Gen() {
>         if (!typeof(T).IsEnum) {
>             throw new ArgumentException("T must be an enum");
>         }
>     }
> }
> ```
> *end example*

## 15.13 Finalizers

> *Note*: In an earlier version of this standard, what is now referred to as a "finalizer" was called a "destructor". Experience has shown that the term "destructor" caused confusion and often resulted to incorrect expectations, especially to programmers knowing C++. In C++, a destructor is called in a determinate manner, whereas, in C#, a finalizer is not. To get determinate behavior from C#, one should use `Dispose`. *end note*

A ***finalizer*** is a member that implements the actions required to finalize an instance of a class. A finalizer is declared using a *finalizer_declaration*:

```ANTLR
finalizer_declaration
    : attributes? '~' Identifier '(' ')' finalizer_body
    | attributes? 'extern' unsafe_modifier? '~' Identifier '(' ')' finalizer_body
    | attributes? unsafe_modifier 'extern'? '~' Identifier '(' ')' finalizer_body
    ;

finalizer_body
    : block
    | ';'
    ;
```

*unsafe_modifier* ([§23.2](unsafe-code.md#232-unsafe-contexts)) is only available in unsafe code ([§23](unsafe-code.md#23-unsafe-code)).

A *finalizer_declaration* may include a set of *attributes* ([§22](attributes.md#22-attributes)).

The *Identifier* of a *finalizer_declarator* shall name the class in which the finalizer is declared. If any other name is specified, a compile-time error occurs.

When a finalizer declaration includes an `extern` modifier, the finalizer is said to be an ***external finalizer***. Because an external finalizer declaration provides no actual implementation, its *finalizer_body* consists of a semicolon. For all other finalizers, the *finalizer_body* consists of a *block*, which specifies the statements to execute in order to finalize an instance of the class. A *finalizer_body* corresponds exactly to the *method_body* of an instance method with a `void` return type ([§15.6.11](classes.md#15611-method-body)).

Finalizers are not inherited. Thus, a class has no finalizers other than the one that may be declared in that class.

> *Note*: Since a finalizer is required to have no parameters, it cannot be overloaded, so a class can have, at most, one finalizer. *end note*

Finalizers are invoked automatically, and cannot be invoked explicitly. An instance becomes eligible for finalization when it is no longer possible for any code to use that instance. Execution of the finalizer for the instance may occur at any time after the instance becomes eligible for finalization ([§8.9](basic-concepts.md#89-automatic-memory-management)). When an instance is finalized, the finalizers in that instance's inheritance chain are called, in order, from most derived to least derived. A finalizer may be executed on any thread. For further discussion of the rules that govern when and how a finalizer is executed, see [§8.9](basic-concepts.md#89-automatic-memory-management).

> *Example*: The output of the example
> ```csharp
> using System;
> class A
> {
>     ~A() {
>         Console.WriteLine("A's finalizer");
>     }
> }
> class B: A
> {
>     ~B() {
>         Console.WriteLine("B's finalizer");
>     }
> }
> class Test
> {
>     static void Main() {
>         B b = new B();
>         b = null;
>         GC.Collect();
>         GC.WaitForPendingFinalizers();
>     }
> }
> ```
> is
> ```console
> B's finalizer
> A's finalizer
> ```
> since finalizers in an inheritance chain are called in order, from most derived to least derived. *end example*

Finalizers are implemented by overriding the virtual method `Finalize` on `System.Object`. C# programs are not permitted to override this method or call it (or overrides of it) directly.

> *Example*: For instance, the program
> ```csharp
> class A
> {
>     override protected void Finalize() {} // error
>     public void F() {
>         this.Finalize(); // error
>     }
> }
> ```
> contains two errors. *end example*

The compiler behaves as if this method, and overrides of it, do not exist at all.

> *Example*: Thus, this program:
> ```csharp
> class A
> {
>     void Finalize() {} // permitted
> }
> ```
> is valid and the method shown hides `System.Object`'s `Finalize` method. *end example*

For a discussion of the behavior when an exception is thrown from a finalizer, see [§21.4](exceptions.md#214-how-exceptions-are-handled).

## 15.14 Iterators

### 15.14.1 General

A function member ([§12.6](expressions.md#126-function-members)) implemented using an iterator block ([§13.3](statements.md#133-blocks)) is called an ***iterator***.

An iterator block may be used as the body of a function member as long as the return type of the corresponding function member is one of the enumerator interfaces ([§15.14.2](classes.md#15142-enumerator-interfaces)) or one of the enumerable interfaces ([§15.14.3](classes.md#15143-enumerable-interfaces)). It may occur as a *method_body*, *operator_body* or *accessor_body*, whereas events, instance constructors, static constructors and finalizers may not be implemented as iterators.

When a function member is implemented using an iterator block, it is a compile-time error for the formal parameter list of the function member to specify any `ref` or `out` parameters.

### 15.14.2 Enumerator interfaces

The ***enumerator interfaces*** are the non-generic interface `System.Collections.IEnumerator` and all instantiations of the generic interface `System.Collections.Generic.IEnumerator<T>`. For the sake of brevity, in this subclause and its siblings these interfaces are referenced as `IEnumerator` and `IEnumerator<T>`, respectively.

### 15.14.3 Enumerable interfaces

The ***enumerable interfaces*** are the non-generic interface `System.Collections.IEnumerable` and all instantiations of the generic interface `System.Collections.Generic.IEnumerable<T>`. For the sake of brevity, in this subclause and its siblings these interfaces are referenced as `IEnumerable` and `IEnumerable<T>`, respectively.

### 15.14.4 Yield type

An iterator produces a sequence of values, all of the same type. This type is called the ***yield type*** of the iterator.

-  The yield type of an iterator that returns `IEnumerator` or `IEnumerable` is object.
-  The yield type of an iterator that returns `IEnumerator<T>` or `IEnumerable<T>` is `T`.

### 15.14.5 Enumerator objects

#### 15.14.5.1 General

When a function member returning an enumerator interface type is implemented using an iterator block, invoking the function member does not immediately execute the code in the iterator block. Instead, an ***enumerator object*** is created and returned. This object encapsulates the code specified in the iterator block, and execution of the code in the iterator block occurs when the enumerator object's `MoveNext` method is invoked. An enumerator object has the following characteristics:

-  It implements `IEnumerator` and `IEnumerator<T>`, where `T` is the yield type of the iterator.
-  It implements `System.IDisposable`.
-  It is initialized with a copy of the argument values (if any) and instance value passed to the function member.
-  It has four potential states, **before**, **running**, **suspended**, and **after**, and is initially in the **before** state.

An enumerator object is typically an instance of a compiler-generated enumerator class that encapsulates the code in the iterator block and implements the enumerator interfaces, but other methods of implementation are possible. If an enumerator class is generated by the compiler, that class will be nested, directly or indirectly, in the class containing the function member, it will have private accessibility, and it will have a name reserved for compiler use ([§7.4.3](lexical-structure.md#743-identifiers)).

An enumerator object may implement more interfaces than those specified above.

The following subclauses describe the required behavior of the `MoveNext`, `Current`, and `Dispose` members of the `IEnumerator` and `IEnumerator<T>` interface implementations provided by an enumerator object.

Enumerator objects do not support the `IEnumerator.Reset` method. Invoking this method causes a `System.NotSupportedException` to be thrown.

#### 15.14.5.2 The MoveNext method

The `MoveNext` method of an enumerator object encapsulates the code of an iterator block. Invoking the `MoveNext` method executes code in the iterator block and sets the `Current` property of the enumerator object as appropriate. The precise action performed by `MoveNext` depends on the state of the enumerator object when `MoveNext` is invoked:

- If the state of the enumerator object is **before**, invoking `MoveNext`:
  - Changes the state to **running**.
  - Initializes the parameters (including `this`) of the iterator block to the argument values and instance value saved when the enumerator object was initialized.
  - Executes the iterator block from the beginning until execution is interrupted (as described below).
- If the state of the enumerator object is **running**, the result of invoking `MoveNext` is unspecified.
- If the state of the enumerator object is **suspended**, invoking MoveNext:
  - Changes the state to **running**.
  - Restores the values of all local variables and parameters (including `this`) to the values saved when execution of the iterator block was last suspended.  
    > *Note*: The contents of any objects referenced by these variables may have changed since the previous call to `MoveNext`. *end note*
  - Resumes execution of the iterator block immediately following the yield return statement that caused the suspension of execution and continues until execution is interrupted (as described below).
- If the state of the enumerator object is **after**, invoking `MoveNext` returns false.

When `MoveNext` executes the iterator block, execution can be interrupted in four ways: By a `yield return` statement, by a `yield break` statement, by encountering the end of the iterator block, and by an exception being thrown and propagated out of the iterator block.

- When a `yield return` statement is encountered ([§10.4.4.20](variables.md#104420-yield-statements)):
  - The expression given in the statement is evaluated, implicitly converted to the yield type, and assigned to the `Current` property of the enumerator object.
  - Execution of the iterator body is suspended. The values of all local variables and parameters (including `this`) are saved, as is the location of this `yield return` statement. If the `yield return` statement is within one or more `try` blocks, the associated finally blocks are *not* executed at this time.
  - The state of the enumerator object is changed to **suspended**.
  - The `MoveNext` method returns `true` to its caller, indicating that the iteration successfully advanced to the next value.
- When a `yield break` statement is encountered ([§10.4.4.20](variables.md#104420-yield-statements)):
  - If the `yield break` statement is within one or more `try` blocks, the associated `finally` blocks are executed.
  - The state of the enumerator object is changed to **after**.
  - The `MoveNext` method returns `false` to its caller, indicating that the iteration is complete.
- When the end of the iterator body is encountered:
  - The state of the enumerator object is changed to **after**.
  - The `MoveNext` method returns `false` to its caller, indicating that the iteration is complete.
- When an exception is thrown and propagated out of the iterator block:
  - Appropriate `finally` blocks in the iterator body will have been executed by the exception propagation.
  - The state of the enumerator object is changed to **after**.
  - The exception propagation continues to the caller of the `MoveNext` method.

#### 15.14.5.3 The Current property

An enumerator object's `Current` property is affected by `yield return` statements in the iterator block.

When an enumerator object is in the **suspended** state, the value of `Current` is the value set by the previous call to `MoveNext`. When an enumerator object is in the **before**, **running**, or **after** states, the result of accessing `Current` is unspecified.

For an iterator with a yield type other than `object`, the result of accessing `Current` through the enumerator object's `IEnumerable` implementation corresponds to accessing `Current` through the enumerator object's `IEnumerator<T>` implementation and casting the result to `object`.

#### 15.14.5.4 The Dispose method

The `Dispose` method is used to clean up the iteration by bringing the enumerator object to the **after** state.

- If the state of the enumerator object is **before**, invoking `Dispose` changes the state to **after**.
- If the state of the enumerator object is **running**, the result of invoking `Dispose` is unspecified.
- If the state of the enumerator object is **suspended**, invoking `Dispose`:
  - Changes the state to **running**.
  - Executes any finally blocks as if the last executed `yield return` statement were a `yield break` statement. If this causes an exception to be thrown and propagated out of the iterator body, the state of the enumerator object is set to **after** and the exception is propagated to the caller of the `Dispose` method.
  - Changes the state to **after**.
- If the state of the enumerator object is **after**, invoking `Dispose` has no affect.

### 15.14.6 Enumerable objects

#### 15.14.6.1 General

When a function member returning an enumerable interface type is implemented using an iterator block, invoking the function member does not immediately execute the code in the iterator block. Instead, an ***enumerable object*** is created and returned. The enumerable object's `GetEnumerator` method returns an enumerator object that encapsulates the code specified in the iterator block, and execution of the code in the iterator block occurs when the enumerator object's `MoveNext` method is invoked. An enumerable object has the following characteristics:

-  It implements `IEnumerable` and `IEnumerable<T>`, where `T` is the yield type of the iterator.
-  It is initialized with a copy of the argument values (if any) and instance value passed to the function member.

An enumerable object is typically an instance of a compiler-generated enumerable class that encapsulates the code in the iterator block and implements the enumerable interfaces, but other methods of implementation are possible. If an enumerable class is generated by the compiler, that class will be nested, directly or indirectly, in the class containing the function member, it will have private accessibility, and it will have a name reserved for compiler use ([§7.4.3](lexical-structure.md#743-identifiers)).

An enumerable object may implement more interfaces than those specified above. 

> *Note*: For example, an enumerable object may also implement `IEnumerator` and `IEnumerator<T>`, enabling it to serve as both an enumerable and an enumerator. Typically, such an implementation would return its own instance (to save allocations) from the first call to `GetEnumerator`. Subsequent invocations of `GetEnumerator`, if any, would return a new class instance, typically of the same class, so that calls to different enumerator instances will not affect each other. It cannot return the same instance even if the previous enumerator has already enumerated past the end of the sequence, since all future calls to an exhausted enumerator must throw exceptions. *end note*

#### 15.14.6.2 The GetEnumerator method

An enumerable object provides an implementation of the `GetEnumerator` methods of the `IEnumerable` and `IEnumerable<T>` interfaces. The two `GetEnumerator` methods share a common implementation that acquires and returns an available enumerator object. The enumerator object is initialized with the argument values and instance value saved when the enumerable object was initialized, but otherwise the enumerator object functions as described in [§15.14.5](classes.md#15145-enumerator-objects).

## 15.15 Async Functions

### 15.15.1 General

A method ([§15.6](classes.md#156-methods)) or anonymous function ([§12.16](expressions.md#1216-anonymous-function-expressions)) with the `async` modifier is called an ***async function***. In general, the term ***async*** is used to describe any kind of function that has the `async` modifier.

It is a compile-time error for the formal parameter list of an async function to specify any `ref` or `out` parameters.

The *return_type* of an async method shall be either `void` or a ***task type***. The task types are `System.Threading.Tasks.Task` and types constructed from `System.Threading.Tasks.Task<T>`. For the sake of brevity, in this clause these types are referenced as `Task` and `Task<T>`, respectively. An async method returning a task type is said to be ***task-returning***.

The exact definition of the task types is implementation-defined, but from the language's point of view, a task type is in one of the states *incomplete*, *succeeded* or *faulted*. A *faulted* task records a pertinent exception. A *succeeded* `Task<*T*>` records a result of type `*T*`. Task types are awaitable, and tasks can therefore be the operands of await expressions ([§12.8.8](expressions.md#1288-await-expressions)).

An async function has the ability to suspend evaluation by means of await expressions ([§12.8.8](expressions.md#1288-await-expressions)) in its body. Evaluation may later be resumed at the point of the suspending await expression by means of a ***resumption delegate***. The resumption delegate is of type `System.Action`, and when it is invoked, evaluation of the async function invocation will resume from the await expression where it left off. The ***current caller*** of an async function invocation is the original caller if the function invocation has never been suspended or the most recent caller of the resumption delegate otherwise.

### 15.15.2 Evaluation of a task-returning async function

Invocation of a task-returning async function causes an instance of the returned task type to be generated. This is called the ***return task*** of the async function. The task is initially in an *incomplete* state.

The async function body is then evaluated until it is either suspended (by reaching an await expression) or terminates, at which point control is returned to the caller, along with the return task.

When the body of the async function terminates, the return task is moved out of the incomplete state:

-  If the function body terminates as the result of reaching a return statement or the end of the body, any result value is recorded in the return task, which is put into a *succeeded* state.
-  If the function body terminates as the result of an uncaught exception ([§13.10.6](statements.md#13106-the-throw-statement)) the exception is recorded in the return task which is put into a *faulted* state.

### 15.15.3 Evaluation of a void-returning async function

If the return type of the async function is `void`, evaluation differs from the above in the following way: Because no task is returned, the function instead communicates completion and exceptions to the current thread's ***synchronization context***. The exact definition of synchronization context is implementation-dependent, but is a representation of "where" the current thread is running. The synchronization context is notified when evaluation of a `void`-returning async function commences, completes successfully, or causes an uncaught exception to be thrown.

This allows the context to keep track of how many `void`-returning async functions are running under it, and to decide how to propagate exceptions coming out of them.
