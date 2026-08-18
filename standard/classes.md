# 15 Classes

## 15.1 General

A class is a data structure that may contain data members (constants and fields), function members (methods, properties, events, indexers, operators, instance constructors, finalizers, and static constructors), and nested types. Class types support inheritance, a mechanism whereby a ***derived class*** can extend and specialize a ***base class***.

Structs ([§16](structs.md#16-structs)) and interfaces ([§19](interfaces.md#19-interfaces)) have members similar to classes but with certain restrictions. This clause defines the declarations for classes and class members. The clauses for structs and interfaces define the restrictions for those types in terms of the corresponding declarations in class types.

## 15.2 Class declarations

### 15.2.1 General

A *class_declaration* is a *type_declaration* ([§14.8](namespaces.md#148-type-declarations)) that declares a new class.

```ANTLR
class_declaration
    : non_record_class_declaration
    | record_class_declaration
    ;

non_record_class_declaration
    : non_record_class_without_positional_members
    | non_record_class_with_positional_members
    ;

non_record_class_without_positional_members
    : attributes? class_modifier* 'partial'? 'class' identifier
        type_parameter_list? class_base?
        type_parameter_constraints_clause* class_body
    ;

non_record_class_with_positional_members
    : attributes? class_modifier* 'partial'? 'class' identifier
        type_parameter_list? delimited_parameter_list class_base?
        type_parameter_constraints_clause* class_body
    ;
```

There are two kinds of class: ***non-record class***, as declared by *non_record_class_declaration*, and ***record class***, as declared by  *record_class_declaration*. A non-record class is the kind of class that C# has supported since the language’s inception. Record classes were added much later and are discussed in [§15.16](classes.md#1516-record-classes). The differences between the two kinds are discussed in [§15.18](classes.md#1518-record-class-and-non-record-class-differences).

A *non_record_class_declaration* can have one of two almost identical forms: *non_record_class_without_positional_members* and *non_record_class_with_positional_members*.

A *non_record_class_without_positional_members* consists of an optional set of *attributes* ([§23](attributes.md#23-attributes)), followed by an optional set of *class_modifier*s ([§15.2.2](classes.md#1522-class-modifiers)), followed by an optional `partial` modifier ([§15.2.7](classes.md#1527-partial-type-declarations)), followed by the keyword `class` and an *identifier* that names the class, followed by an optional *type_parameter_list* ([§15.2.3](classes.md#1523-type-parameters)), followed by an optional *class_base* specification ([§15.2.4](classes.md#1524-class-base-specification)), followed by an optional set of *type_parameter_constraints_clause*s ([§15.2.5](classes.md#1525-type-parameter-constraints)), followed by a *class_body* ([§15.2.6](classes.md#1526-class-body)).

A *non_record_class_with_positional_members* has the same syntax but requires a *delimited_parameter_list*, as shown above in that grammar rule. For a discussion of *delimited_parameter_list*, see [§15.11.6](classes.md#15116-primary-constructors).

A class having a required member ([§15.7.1](classes.md#1571-general)) directly (that is, not through inheritance) shall be treated as if it were decorated with the attribute `System.Runtime.CompilerServices.RequiredMemberAttribute` ([§23.5.11.2](attributes.md#235112-the-requiredmember-attribute)).

A class declaration shall not supply *type_parameter_constraints_clause*s unless it also supplies a *type_parameter_list*.

A class declaration that supplies a *type_parameter_list* is a generic class declaration. Additionally, any class nested inside a generic class declaration or a generic struct declaration is itself a generic class declaration, since type arguments for the containing type shall be supplied to create a constructed type ([§8.4](types.md#84-constructed-types)).

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
    | 'file'
    | unsafe_modifier   // unsafe code support
    ;
```

*unsafe_modifier* ([§24.2](unsafe-code.md#242-unsafe-contexts)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).

It is a compile-time error for the same modifier to appear multiple times in a class declaration.

The `new` modifier is permitted on nested classes. It specifies that the class hides an inherited member by the same name, as described in [§15.3.5](classes.md#1535-the-new-modifier). It is a compile-time error for the `new` modifier to appear on a class declaration that is not a nested class declaration.

The `public`, `protected`, `internal`, and `private` modifiers control the accessibility of the class. Depending on the context in which the class declaration occurs, some of these modifiers might not be permitted ([§7.5.2](basic-concepts.md#752-declared-accessibility)).

It is a compile-time error for any of the `public`, `protected`, `internal`, and `private` modifiers to be combined with the `file` modifier.

When a partial type declaration ([§15.2.7](classes.md#1527-partial-type-declarations)) includes an accessibility specification (via the `public`, `protected`, `internal`, and `private` modifiers), that specification shall agree with all other parts that include an accessibility specification. If no part of a partial type includes an accessibility specification, the type is given the appropriate default accessibility ([§7.5.2](basic-concepts.md#752-declared-accessibility)).

The `abstract`, `sealed`, and `static` modifiers are discussed in the following subclauses.

The `file` modifier specifies that the type being declared is local to its parent compilation unit. A compilation unit containing a `file`-modified type shall not also contain a type declaration having the same name but without the `file` modifier.

The `file` modifier shall only appear in a type declaration for a top-level type.

When a type declaration contains the `file` modifier, that type is said to be a ***file-local type***.

The implementation shall guarantee that file-local types with the same name declared in different compilation units, are distinct at runtime. The implementation shall guarantee that a file-local type with the same name as a non-file-local type declared in different compilation units, are distinct at runtime.

A file-local class that is an attribute type may be used an as attribute within both file-local types and non-file-local types, just as if the attribute type were a non-file-local class.

#### 15.2.2.2 Abstract classes

The `abstract` modifier is used to indicate that a class is incomplete and that it is intended to be used only as a base class. An ***abstract class*** differs from a ***non-abstract class*** in the following ways:

- An abstract class cannot be instantiated directly, and it is a compile-time error to use the `new` operator on an abstract class. While it is possible to have variables and values whose compile-time types are abstract, such variables and values will necessarily either be `null` or contain references to instances of non-abstract classes derived from the abstract types.
- An abstract class is permitted (but not required) to contain abstract members.
- An abstract class cannot be sealed.

When a non-abstract class is derived from an abstract class, the non-abstract class shall include actual implementations of all inherited abstract members, thereby overriding those abstract members.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AbstractMethodImplementation"} -->
> ```csharp
> abstract class A
> {
>     public abstract void F();
> }
>
> abstract class B : A
> {
>     public void G() {}
> }
>
> class C : B
> {
>     public override void F()
>     {
>         // Actual implementation of F
>     }
> }
> ```
>
> the abstract class `A` introduces an abstract method `F`. Class `B` introduces an additional method `G`, but since it does not provide an implementation of `F`, `B` shall also be declared abstract. Class `C` overrides `F` and provides an actual implementation. Since there are no abstract members in `C`, `C` is permitted (but not required) to be non-abstract.
>
> *end example*

If one or more parts of a partial type declaration ([§15.2.7](classes.md#1527-partial-type-declarations)) of a class include the `abstract` modifier, the class is abstract. Otherwise, the class is non-abstract.

#### 15.2.2.3 Sealed classes

The `sealed` modifier is used to prevent derivation from a class. A compile-time error occurs if a sealed class is specified as the base class of another class.

A sealed class cannot also be an abstract class.

> *Note*: The `sealed` modifier is primarily used to prevent unintended derivation, but it also enables certain run-time optimizations. In particular, because a sealed class is known to never have any derived classes, it is possible to transform virtual function member invocations on sealed class instances into non-virtual invocations. *end note*

If one or more parts of a partial type declaration ([§15.2.7](classes.md#1527-partial-type-declarations)) of a class include the `sealed` modifier, the class is sealed. Otherwise, the class is unsealed.

#### 15.2.2.4 Static classes

##### 15.2.2.4.1 General

The `static` modifier is used to mark the class being declared as a ***static class***. A static class shall not be instantiated, shall not be used as a type and shall contain only static members. Only a static class can contain declarations of extension methods ([§15.6.10](classes.md#15610-extension-methods)).

A static class declaration is subject to the following restrictions:

- A static class shall not include a `sealed` or `abstract` modifier. (However, since a static class cannot be instantiated or derived from, it behaves as if it was both sealed and abstract.)
- A static class shall not include a *class_base* specification ([§15.2.4](classes.md#1524-class-base-specification)) and cannot explicitly specify a base class or a list of implemented interfaces. A static class implicitly inherits from type `object`.
- A static class shall only contain static members ([§15.3.8](classes.md#1538-static-and-instance-members)).  
   > *Note*: All constants and nested types are classified as static members. *end note*
- A static class shall not have members with `protected`, `private protected`, or `protected internal` declared accessibility.

It is a compile-time error to violate any of these restrictions.

A static class has no instance constructors. It is not possible to declare an instance constructor in a static class, and no default instance constructor ([§15.11.5](classes.md#15115-default-constructors)) is provided for a static class.

The members of a static class are not automatically static, and the member declarations shall explicitly include a `static` modifier (except for constants and nested types). When a class is nested within a static outer class, the nested class is not a static class unless it explicitly includes a `static` modifier.

If one or more parts of a partial type declaration ([§15.2.7](classes.md#1527-partial-type-declarations)) of a class include the `static` modifier, the class is static. Otherwise, the class is not static.

##### 15.2.2.4.2 Referencing static class types

A *namespace_or_type_name* ([§7.8](basic-concepts.md#78-namespace-and-type-names)) is permitted to reference a static class if

- The *namespace_or_type_name* is the `T` in a *namespace_or_type_name* of the form `T.I`, or
- The *namespace_or_type_name* is the `T` in a *typeof_expression* ([§12.8.18](expressions.md#12818-the-typeof-operator)) of the form `typeof(T)`.

A *primary_expression* ([§12.8](expressions.md#128-primary-expressions)) is permitted to reference a static class if

- The *primary_expression* is the `E` in a *member_access* ([§12.8.7](expressions.md#1287-member-access)) of the form `E.I`.

In any other context, it is a compile-time error to reference a static class.

> *Note*: For example, it is an error for a static class to be used as a base class, a constituent type ([§15.3.7](classes.md#1537-constituent-types)) of a member, a generic type argument, or a type parameter constraint. Likewise, a static class cannot be used in an array type, a new expression, a cast expression, an is expression, an as expression, a `sizeof` expression, or a default value expression. *end note*

### 15.2.3 Type parameters

A type parameter is a simple identifier that denotes a placeholder for a type argument supplied to create a constructed type. By contrast, a type argument ([§8.4.2](types.md#842-type-arguments)) is the type that is substituted for the type parameter when a constructed type is created.

```ANTLR
type_parameter_list
    : '<' decorated_type_parameter (',' decorated_type_parameter)* '>'
    ;

decorated_type_parameter
    : attributes? type_parameter
    ;
```

*type_parameter* is defined in [§8.5](types.md#85-type-parameters).

Each type parameter in a class declaration defines a name in the declaration space ([§7.3](basic-concepts.md#73-declarations)) of that class. Thus, it cannot have the same name as another type parameter of that class or a member declared in that class. A type parameter cannot have the same name as the type itself.

Two partial generic type declarations (in the same program) contribute to the same unbound generic type if they have the same fully qualified name (which includes a *generic_dimension_specifier* ([§12.8.18](expressions.md#12818-the-typeof-operator)) for the number of type parameters) ([§7.8.3](basic-concepts.md#783-fully-qualified-names)). Two such partial type declarations shall specify the same name for each type parameter, in order.

### 15.2.4 Class base specification

#### 15.2.4.1 General

A class declaration may include a *class_base* specification, which defines the direct base class of the class and the interfaces ([§19](interfaces.md#19-interfaces)) directly implemented by the class.

```ANTLR
class_base
    : ':' class_type base_argument_list?
    | ':' interface_type_list
    | ':' class_type base_argument_list? ',' interface_type_list
    ;

base_argument_list
    : '(' argument_list? ')'
    ;

interface_type_list
    : interface_type (',' interface_type)*
    ;
```

*argument_list* corresponds to the base class’s positional member list *delimited_parameter_list*.

A warning shall be produced for an in or by-value argument in a *base_argument_list* when all the following conditions are true:

- The argument represents an implicit or explicit identity conversion of a primary constructor parameter ([§15.11.6](classes.md#15116-primary-constructors));
- The argument is not part of an expanded params argument;
- The primary constructor parameter is captured into the state of the enclosing type.

A record class may not inherit from a non-record class other than `object`, and a non-record class may not inherit from a record class.

It is an error for *class_base* to have a *base_argument_list* if the corresponding *class_declaration* does not contain a *delimited_parameter_list*.

#### 15.2.4.2 Base classes

When a *class_type* is included in the *class_base*, it specifies the direct base class of the class being declared. If a non-partial class declaration has no *class_base*, or if the *class_base* lists only interface types, the direct base class is assumed to be `object`. When a partial class declaration includes a base class specification, that base class specification shall reference the same type as all other parts of that partial type that include a base class specification. If no part of a partial class includes a base class specification, the base class is `object`. A class inherits members from its direct base class, as described in [§15.3.4](classes.md#1534-inheritance).

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"DirectBaseClass"} -->
> ```csharp
> class A {}
> class B : A {}
> ```
>
> Class `A` is said to be the direct base class of `B`, and `B` is said to be derived from `A`. Since `A` does not explicitly specify a direct base class, its direct base class is implicitly `object`.
>
> *end example*

For a constructed class type, including a nested type declared within a generic type declaration ([§15.3.9.7](classes.md#15397-nested-types-in-generic-classes)), if a base class is specified in the generic class declaration, the base class of the constructed type is obtained by substituting, for each *type_parameter* in the base class declaration, the corresponding *type_argument* of the constructed type.

> *Example*: Given the generic class declarations
>
> <!-- Example: {template:"standalone-lib-without-using", name:"GenericBaseClass", replaceEllipsis:true} -->
> ```csharp
> class B<U,V> {...}
> class G<T> : B<string,T[]> {...}
> ```
>
> the base class of the constructed type `G<int>` would be `B<string,int[]>`.
>
> *end example*

The base class specified in a class declaration can be a constructed class type ([§8.4](types.md#84-constructed-types)). A base class cannot be a type parameter on its own ([§8.5](types.md#85-type-parameters)), though it can involve the type parameters that are in scope.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"TypeParameterUsedAsBaseClass", expectedErrors:["CS0689"]} -->
> ```csharp
> class Base<T> {}
>
> // Valid, non-constructed class with constructed base class
> class Extend1 : Base<int> {}
>
> // Error, type parameter used as base class
> class Extend2<V> : V {}
>
> // Valid, type parameter used as type argument for base class
> class Extend3<V> : Base<V> {}
> ```
>
> *end example*

The direct base class of a class type shall be at least as accessible as the class type itself ([§7.5.5](basic-concepts.md#755-accessibility-constraints)). For example, it is a compile-time error for a public class to derive from a private or internal class.

The direct base class of a class type shall not be any of the following types: `System.Array`, `System.Delegate`, `System.Enum`, `System.ValueType` or the `dynamic` type.

A file-local class shall not be used as a base type of a non-file-local class.

In determining the meaning of the direct base class specification `A` of a class `B`, the direct base class of `B` is temporarily assumed to be `object`, which ensures that the meaning of a base class specification cannot recursively depend on itself. Given this definition, the complete set of types upon which a class depends is the transitive closure of the *directly depends on* relationship.

> *Example*: The following
>
> <!-- Example: {template:"standalone-lib-without-using", name:"RecursiveBaseClassSpecification", expectedErrors:["CS0146"]} -->
> ```csharp
> class X<T>
> {
>     public class Y{}
> }
>
> class Z : X<Z.Y> {}
> ```
>
> is in error since in the base class specification `X<Z.Y>` the direct base class of `Z` is considered to be `object`, and hence (by the rules of [§7.8](basic-concepts.md#78-namespace-and-type-names)) `Z` is not considered to have a member `Y`.
>
> *end example*

The base classes of a class are the direct base class and its base classes. In other words, the set of base classes is the transitive closure of the direct base class relationship.

> *Example*: In the following:
>
> <!-- Example: {template:"standalone-lib", name:"DirectBaseClasses", replaceEllipsis:true} -->
> ```csharp
> class A {...}
> class B<T> : A {...}
> class C<T> : B<IComparable<T>> {...}
> class D<T> : C<T[]> {...}
> ```
>
> the base classes of `D<int>` are `C<int[]>`, `B<IComparable<int[]>>`, `A`, and `object`.
>
> *end example*

Except for class `object`, every class has exactly one direct base class. The `object` class has no direct base class and is the ultimate base class of all other classes.

It is a compile-time error for a class to depend on itself. For the purpose of this rule, a class ***directly depends on*** its direct base class (if any) and *directly depends on* the type within which it is immediately nested (if any).

> *Example*: The example
>
> <!-- Example: {template:"standalone-lib-without-using", name:"SelfBaseClass", expectedErrors:["CS0146"]} -->
> ```csharp
> class A : A {}
> ```
>
> is erroneous because the class depends on itself. Likewise, the example
>
> <!-- Example: {template:"standalone-lib-without-using", name:"CircularBaseClass1", expectedErrors:["CS0146","CS0146","CS0146"]} -->
> ```csharp
> class A : B {}
> class B : C {}
> class C : A {}
> ```
>
> is in error because the classes circularly depend on themselves. Finally, the example
>
> <!-- Example: {template:"standalone-lib-without-using", name:"CircularBaseClass2", expectedErrors:["CS0146","CS0146"]} -->
> ```csharp
> class A : B.C {}
> class B : A
> {
>     public class C {}
> }
> ```
>
> results in a compile-time error because A depends on `B.C` (its direct base class), which depends on `B` (its immediately enclosing class), which circularly depends on `A`.
>
> *end example*

A class does not depend on the classes that are nested within it.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"NestedClassDependency"} -->
> ```csharp
> class A
> {
>     class B : A {}
> }
> ```
>
> `B` depends on `A` (because `A` is both its direct base class and its immediately enclosing class), but `A` does not depend on `B` (since `B` is neither a base class nor an enclosing class of `A`). Thus, the example is valid.
>
> *end example*

It is not possible to derive from a sealed class.
> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"DeriveFromSealedClass", expectedErrors:["CS0509"]} -->
> ```csharp
> sealed class A {}
> class B : A {} // Error, cannot derive from a sealed class
> ```
>
> Class `B` is in error because it attempts to derive from the sealed class `A`.
>
> *end example*

#### 15.2.4.3 Interface implementations

A *class_base* specification may include a list of interface types, in which case the class is said to implement the given interface types. For a constructed class type, including a nested type declared within a generic type declaration ([§15.3.9.7](classes.md#15397-nested-types-in-generic-classes)), each implemented interface type is obtained by substituting, for each *type_parameter* in the given interface, the corresponding *type_argument* of the constructed type.

The set of interfaces for a type declared in multiple parts ([§15.2.7](classes.md#1527-partial-type-declarations)) is the union of the interfaces specified on each part. A particular interface can only be named once on each part, but multiple parts can name the same base interfaces. There shall only be one implementation of each member of any given interface.

> *Example*: In the following:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ClassesInterfaceImplementations1", replaceEllipsis:true, additionalFiles:["IA.cs","IB.cs","IC.cs"]} -->
> ```csharp
> partial class C : IA, IB {...}
> partial class C : IC {...}
> partial class C : IA, IB {...}
> ```
>
> the set of base interfaces for class `C` is `IA`, `IB`, and `IC`.
>
> *end example*

Typically, each part provides an implementation of the interfaces declared on that part; however, this is not a requirement. A part can provide the implementation for an interface declared on a different part.

> *Example*:
>
> <!-- Example: {template:"standalone-lib", name:"ClassesInterfaceImplementations2", replaceEllipsis:true, customEllipsisReplacements:["return 0;"]} -->
> ```csharp
> partial class X
> {
>     int IComparable.CompareTo(object o) {...}
> }
>
> partial class X : IComparable
> {
>     ...
> }
> ```
>
> *end example*

The base interfaces specified in a class declaration can be constructed interface types ([§8.4](types.md#84-constructed-types), [§19.2](interfaces.md#192-interface-declarations)). A base interface cannot be a type parameter on its own, though it can involve the type parameters that are in scope.

> *Example*: The following code illustrates how a class can implement and extend constructed types:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ClassesInterfaceImplementations3"} -->
> ```csharp
> class C<U, V> {}
> interface I1<V> {}
> class D : C<string, int>, I1<string> {}
> class E<T> : C<int, T>, I1<T> {}
> ```
>
> *end example*

Interface implementations are discussed further in [§19.6](interfaces.md#196-interface-implementations).

### 15.2.5 Type parameter constraints

Generic type and method declarations can optionally specify type parameter constraints by including *type_parameter_constraints_clause*s.

```ANTLR
type_parameter_constraints_clause
    : 'where' type_parameter ':' type_parameter_constraints
    ;

type_parameter_constraints
    : restrictive_type_parameter_constraints (',' anti_constraints_clause)?
    | anti_constraints_clause
    ;

restrictive_type_parameter_constraints
    : primary_constraint (',' secondary_constraints)? (',' constructor_constraint)?
    | secondary_constraints (',' constructor_constraint)?
    | constructor_constraint
    ;

primary_constraint
    : class_type nullable_type_annotation?
    | 'class' nullable_type_annotation?
    | 'struct'
    | 'notnull'
    | 'unmanaged'
    | 'default'
    ;

secondary_constraint
    : interface_type nullable_type_annotation?
    | type_parameter nullable_type_annotation?
    ;

secondary_constraints
    : secondary_constraint (',' secondary_constraint)*
    ;

constructor_constraint
    : 'new' '(' ')'
    ;

anti_constraints_clause
    : 'allows' anti_constraints

anti_constraints
    : anti_constraint (',' anti_constraint)*

anti_constraint
    : ref_struct_clause

ref_struct_clause
    : 'ref' 'struct'
```

Each *type_parameter_constraints_clause* consists of the token `where`, followed by the name of a type parameter, followed by a colon and the list of constraints and anti-constraints (see definition below) for that type parameter. There can be at most one `where` clause for each type parameter, and the `where` clauses can be listed in any order. Like the `get` and `set` tokens in a property accessor, the `where` token is not a keyword.

The list of constraints and anti-constraints given in a `where` clause can include any of the following components, in this order: a *primary_constraint*, one or more *secondary_constraint*s, a *constructor_constraint*, and an *anti_constraints_clause*.

> *Note*: Although the grammar permits *anti_constraints* to contain multiple *anti_constraint*s, this is for future expansion, and in this edition of this specification the only anti-constraint is `ref struct`. *end note*

A primary constraint can be a class type, the ***reference type constraint*** `class`, the ***value type constraint*** `struct`, the ***not null constraint*** `notnull`, the ***unmanaged type constraint*** `unmanaged`, or `default`. The class type and the reference type constraint can include the *nullable_type_annotation*.

It is a compile-time error to use a `default` constraint other than on a method override or explicit implementation. It is a compile-time error to use a `default` constraint when the corresponding type parameter in the overridden or interface method is constrained to a reference type or value type.

A secondary constraint can be an *interface_type* or *type_parameter*, optionally followed by a *nullable_type_annotation*. The presence of the *nullable_type_annotation* indicates that the type argument is allowed to be the nullable reference type that corresponds to a non-nullable reference type that satisfies the constraint.

The reference type constraint specifies that a type argument used for the type parameter shall be a reference type. All class types, interface types, delegate types, array types, and type parameters known to be a reference type (as defined below) satisfy this constraint.

The class type, reference type constraint, and secondary constraints can include the nullable type annotation. The presence or absence of this annotation on the type parameter indicates the nullability expectations for the type argument:

- If the constraint does not include the nullable type annotation, the type argument is expected to be a non-nullable reference type. A compiler may issue a warning if the type argument is a nullable reference type.
- If the constraint includes the nullable type annotation, the constraint is satisfied by both a non-nullable reference type and a nullable reference type.

The nullability of the type argument need not match the nullability of the type parameter. A compiler may issue a warning if the nullability of the type parameter does not match the nullability of the type argument.

> *Note*: To specify that a type argument is a nullable reference type, do not add the nullable type annotation as a constraint (use `T : class` or `T : BaseClass`), but use `T?` throughout the generic declaration to indicate the corresponding nullable reference type for the type argument. *end note*

Unless a type parameter is explicitly constrained to value types, the nullable type annotation `?` can only be applied to a type parameter within a `#nullable enable` context.

For a type parameter `T` when the type argument is a nullable reference type `C?`, instances of `T?` are interpreted as `C?`, not `C??`.

> *Example*: The following examples show how the nullability of a type argument impacts the nullability of a declaration of its type parameter:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"RepeatedNullable"} -->
> ```csharp
> public class C
> {
> }
> 
> public static class  Extensions
> {
>     public static void M<T>(this T? arg) where T : notnull
>     {
> 
>     }
> }
> 
> public class Test
> {
>     public void M()
>     {
>         C? mightBeNull = new C();
>         C notNull = new C();
> 
>         int number = 5;
>         int? missing = null;
> 
>         mightBeNull.M(); // arg is C?
>         notNull.M(); //  arg is C?
>         number.M(); // arg is int?
>         missing.M(); // arg is int?
>     }
> }
> ```
>
> When the type argument is a non-nullable type, the `?` type annotation indicates that the parameter is the corresponding nullable type. When the type argument is already a nullable reference type, the parameter is that same nullable type.
>
> *end example*

The not null constraint specifies that a type argument used for the type parameter should be a non-nullable value type or a non-nullable reference type. A type argument that is not a non-nullable value type or a non-nullable reference type is allowed, but a compiler may produce a diagnostic warning.

Because `notnull` is not a keyword, in *primary_constraint* the not null constraint is always syntactically ambiguous with *class_type*. For compatibility reasons, if a name lookup ([§12.8.4](expressions.md#1284-simple-names)) of the name `notnull` succeeds it shall be treated as a `class_type`. Otherwise it shall be treated as the not null constraint.

> *Example*: The following class demonstrates the use of various type arguments against different constraints, indicating warnings which may be issued by a compiler.
>
> <!-- Example: {template:"standalone-lib-without-using", name:"TypeParameterConstraints0", expectedWarnings:["CS8714","CS8714","CS8631"], ignoredWarnings:["CS0168"]} -->
> ```csharp
> #nullable enable
> public class C { }
> public class A<T> where T : notnull { }
> public class B1<T> where T : C { }
> public class B2<T> where T : C? { }
> class Test
> {
>     static void M()
>     {
>         // nonnull constraint allows nonnullable struct type argument
>         A<int> x1;
>         // possible warning: nonnull constraint prohibits nullable struct type argument
>         A<int?> x2;
>         // nonnull constraint allows nonnullable class type argument
>         A<C> x3;
>         // possible warning: nonnull constraint prohibits nullable class type argument
>         A<C?> x4;
>         // nonnullable base class requirement allows nonnullable class type argument
>         B1<C> x5;
>         // possible warning: nonnullable base class requirement prohibits nullable
>         // class type argument
>         B1<C?> x6;
>         // nullable base class requirement allows nonnullable class type argument
>         B2<C> x7;
>         // nullable base class requirement allows nullable class type argument
>         B2<C?> x8;
>     }
> }
> ```

The value type constraint specifies that a type argument used for the type parameter shall be a non-nullable value type. All non-nullable struct types, enum types, and type parameters having the value type constraint satisfy this constraint. Note that although classified as a value type, a nullable value type ([§8.3.12](types.md#8312-nullable-value-types)) does not satisfy the value type constraint. A type parameter having the value type constraint shall not also have the *constructor_constraint*, although it may be used as a type argument for another type parameter with a *constructor_constraint*.

> *Note*: The `System.Nullable<T>` type specifies the non-nullable value type constraint for `T`. Thus, recursively constructed types of the forms `T??` and `Nullable<Nullable<T>>` are prohibited. *end note*

The unmanaged type constraint specifies that a type argument used for the type parameter shall be a non-nullable unmanaged type ([§8.8](types.md#88-unmanaged-types)).

Because `unmanaged` is not a keyword, in *primary_constraint* the unmanaged constraint is always syntactically ambiguous with *class_type*. For compatibility reasons, if a name lookup ([§12.8.4](expressions.md#1284-simple-names)) of the name `unmanaged` succeeds it is treated as a `class_type`. Otherwise it is treated as the unmanaged constraint.

The `default` constraint allows a *nullable_type_annotation* on a type parameter that is not constrained to reference types or value types.

Pointer types are never allowed to be type arguments, and do not satisfy any type constraints, even unmanaged, despite being unmanaged types.

If a constraint is a class type, an interface type, or a type parameter, that type specifies a minimal “base type” that every type argument used for that type parameter shall support. Whenever a constructed type or generic method is used, the type argument is checked against the constraints on the type parameter at compile-time. The type argument supplied shall satisfy the conditions described in [§8.4.5](types.md#845-satisfying-constraints).

A *class_type* constraint shall satisfy the following rules:

- The type shall be a class type.
- The type shall not be `sealed`.
- The type shall not be one of the following types: `System.Array` or `System.ValueType`.
- The type shall not be `object`.
- At most one constraint for a given type parameter may be a class type.

A type specified as an *interface_type* constraint shall satisfy the following rules:

- The type shall be an interface type.
- A type shall not be specified more than once in a given `where` clause.

In either case, the constraint may involve any of the type parameters of the associated type or method declaration as part of a constructed type, and may involve the type being declared.

Any class or interface type specified as a type parameter constraint shall be at least as accessible ([§7.5.5](basic-concepts.md#755-accessibility-constraints)) as the generic type or method being declared.

A type specified as a *type_parameter* constraint shall satisfy the following rules:

- The type shall be a type parameter.
- A type shall not be specified more than once in a given `where` clause.

In addition there shall be no cycles in the dependency graph of type parameters, where dependency is a transitive relation defined by:

- If a type parameter `T` is used as a constraint for type parameter `S` then `S` ***depends on*** `T`.
- If a type parameter `S` depends on a type parameter `T` and `T` depends on a type parameter `U` then `S` *depends on* `U`.

Given this relation, it is a compile-time error for a type parameter to depend on itself (directly or indirectly).

Any constraints shall be consistent among dependent type parameters. If type parameter `S` depends on type parameter `T` then:

- `T` shall not have the value type constraint. Otherwise, `T` is effectively sealed so `S` would be forced to be the same type as `T`, eliminating the need for two type parameters.
- If `S` has the value type constraint then `T` shall not have a *class_type* constraint.
- If `S` has a *class_type* constraint `A` and `T` has a *class_type* constraint `B` then there shall be an identity conversion or implicit reference conversion from `A` to `B` or an implicit reference conversion from `B` to `A`.
- If `S` also depends on type parameter `U` and `U` has a *class_type* constraint `A` and `T` has a *class_type* constraint `B` then there shall be an identity conversion or implicit reference conversion from `A` to `B` or an implicit reference conversion from `B` to `A`.

It is valid for `S` to have the value type constraint and `T` to have the reference type constraint. Effectively this limits `T` to the types `System.Object`, `System.ValueType`, `System.Enum`, and any interface type.

If the `where` clause for a type parameter includes a constructor constraint (which has the form `new()`), it is possible to use the `new` operator to create instances of the type ([§12.8.17.2](expressions.md#128172-object-creation-expressions)). Any type argument used for a type parameter with a constructor constraint shall be a value type, a non-abstract class having a public parameterless constructor, or a type parameter having the value type constraint or constructor constraint.

It is a compile-time error for *type_parameter_constraints* having a *primary_constraint* of `struct` or `unmanaged` to also have a *constructor_constraint*.

Ordinarily, a ref struct type cannot be used as a type argument for a generic type or method. However, the presence of an *anti_constraints_clause* containing `allows ref struct` permits such a use. This clause is referred to as an ***anti-constraint***, as it expands the set of allowed type arguments rather than limits them like all other constraints.
When a type parameter has this anti-constraint, ref safety rules on all instances of that type parameter shall be enforced.

The anti-constraint is not inherited from a type parameter type constraint. In the code below, `S` cannot be substituted with a ref struct:

```csharp
class C<T, S>
    where T : allows ref struct
    where S : T
{}
```

Given `where T : allows ref struct`, `T` shall not

- Also be constrained to a known reference type
- Also be constrained with `class` or `class?`
- Be used as a generic argument unless the corresponding parameter also has the anti-constraint

It is a compile-time error to invoke a non-virtual instance method (or property) on a type parameter with `allows ref struct`.

> *Example*: The following are examples of constraints:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"TypeParameterConstraints1", replaceEllipsis:true} -->
> ```csharp
> interface IPrintable
> {
>     void Print();
> }
>
> interface IComparable<T>
> {
>     int CompareTo(T value);
> }
>
> interface IKeyProvider<T>
> {
>     T GetKey();
> }
>
> class Printer<T> where T : IPrintable {...}
> class SortedList<T> where T : IComparable<T> {...}
>
> class Dictionary<K,V>
>     where K : IComparable<K>
>     where V : IPrintable, IKeyProvider<K>, new()
> {
>     ...
> }
> ```
>
> The following example is in error because it causes a circularity in the dependency graph of the type parameters:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"TypeParameterConstraints2", replaceEllipsis:true, expectedErrors:["CS0454"]} -->
> ```csharp
> class Circular<S,T>
>     where S: T
>     where T: S // Error, circularity in dependency graph
> {
>     ...
> }
> ```
>
> The following examples illustrate additional invalid situations:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"TypeParameterConstraints3", replaceEllipsis:true, expectedErrors:["CS0456","CS0455","CS0455"]} -->
> ```csharp
> class Sealed<S,T>
>     where S : T
>     where T : struct // Error, `T` is sealed
> {
>     ...
> }
>
> class A {...}
> class B {...}
>
> class Incompat<S,T>
>     where S : A, T
>     where T : B // Error, incompatible class-type constraints
> {
>     ...
> }
>
> class StructWithClass<S,T,U>
>     where S : struct, T
>     where T : U
>     where U : A // Error, A incompatible with struct
> {
>     ...
> }
> ```
>
> *end example*

The ***dynamic erasure*** of a type `C` is type `Cₓ` constructed as follows:

- If `C` is a nested type `Outer.Inner` then `Cₓ` is a nested type `Outerₓ.Innerₓ`.
- If `C` is a constructed type `G<A¹, ..., Aⁿ>` with type arguments `A¹, ..., Aⁿ` then `Cₓ` is the constructed type `G<A¹ₓ, ..., Aⁿₓ>`.
- If `C` is an array type `E[]` then `Cₓ` is the array type `Eₓ[]`.
- If `C` is dynamic then `Cₓ` is `object`.
- Otherwise, `Cₓ` is `C`.

The ***effective base class*** of a type parameter `T` is defined as follows:

Let `R` be a set of types such that:

- For each constraint of `T` that is a type parameter, `R` contains its effective base class.
- For each constraint of `T` that is a struct type, `R` contains `System.ValueType`.
- For each constraint of `T` that is an enumeration type, `R` contains `System.Enum`.
- For each constraint of `T` that is a delegate type, `R` contains its dynamic erasure.
- For each constraint of `T` that is an array type, `R` contains `System.Array`.
- For each constraint of `T` that is a class type, `R` contains its dynamic erasure.

Then

- If `T` has the value type constraint, its effective base class is `System.ValueType`.
- Otherwise, if `R` is empty then the effective base class is `object`.
- Otherwise, the effective base class of `T` is the most-encompassed type ([§10.5.3](conversions.md#1053-evaluation-of-user-defined-conversions)) of set `R`. If the set has no encompassed type, the effective base class of `T` is `object`. The consistency rules ensure that the most-encompassed type exists.

If the type parameter is a method type parameter whose constraints are inherited from the base method the effective base class is calculated after type substitution.

These rules ensure that the effective base class is always a *class_type*.

The ***effective interface set*** of a type parameter `T` is defined as follows:

- If `T` has no *secondary_constraints*, its effective interface set is empty.
- If `T` has *interface_type* constraints but no *type_parameter* constraints, its effective interface set is the set of dynamic erasures of its *interface_type* constraints.
- If `T` has no *interface_type* constraints but has *type_parameter* constraints, its effective interface set is the union of the effective interface sets of its *type_parameter* constraints.
- If `T` has both *interface_type* constraints and *type_parameter* constraints, its effective interface set is the union of the set of dynamic erasures of its *interface_type* constraints and the effective interface sets of its *type_parameter* constraints.

A type parameter is *known to be a reference type* if it has the reference type constraint or its effective base class is not `object` or `System.ValueType`. A type parameter is *known to be a non-nullable reference type* if it is known to be a reference type and has the non-nullable reference type constraint.

Values of a constrained type parameter type can be used to access the instance members implied by the constraints.

> *Example*: In the following:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"TypeParameterConstraints4"} -->
> ```csharp
> interface IPrintable
> {
>     void Print();
> }
>
> class Printer<T> where T : IPrintable
> {
>     void PrintOne(T x) => x.Print();
> }
> ```
>
> the methods of `IPrintable` can be invoked directly on `x` because `T` is constrained to always implement `IPrintable`.
>
> *end example*

When a partial generic type declaration includes constraints, the constraints shall agree with all other parts that include constraints. Specifically, each part that includes constraints shall have constraints for the same set of type parameters, and for each type parameter, the sets of primary, secondary, and constructor constraints shall be equivalent. Two sets of constraints are equivalent if they contain the same members. If no part of a partial generic type specifies type parameter constraints, the type parameters are considered unconstrained.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"TypeParameterConstraints5", replaceEllipsis:true, additionalFiles:["IComparableT.cs","IKeyProviderT.cs"]} -->
> ```csharp
> partial class Map<K,V>
>     where K : IComparable<K>
>     where V : IKeyProvider<K>, new()
> {
>     ...
> }
>
> partial class Map<K,V>
>     where V : IKeyProvider<K>, new()
>     where K : IComparable<K>
> {
>     ...
> }
>
> partial class Map<K,V>
> {
>     ...
> }
> ```
>
> is correct because those parts that include constraints (the first two) effectively specify the same set of primary, secondary, and constructor constraints for the same set of type parameters, respectively.
>
> *end example*

### 15.2.6 Class body

The *class_body* of a class defines the members of that class.

```ANTLR
class_body
    : '{' class_member_declaration* '}' ';'?
    | ';'
    ;
```

The *class_body*s `{}`, `{};`, and `;` are equivalent, and the *class_body*s `{…}` and `{…};` are equivalent.

### 15.2.7 Partial type declarations

The modifier `partial` is used when defining a class, struct, or interface type in multiple parts. The `partial` modifier is a contextual keyword ([§6.4.4](lexical-structure.md#644-keywords)) and has special meaning in a *class_declaration*, a *struct_declaration*, or an *interface_declaration*. (A partial type may contain partial method declarations ([§15.6.9](classes.md#1569-partial-methods)).

Each part of a ***partial type*** declaration shall include a `partial` modifier and shall be declared in the same namespace or containing type as the other parts. The `partial` modifier indicates that additional parts of the type declaration might exist elsewhere, but the existence of such additional parts is not a requirement; it is valid for the only declaration of a type to include the `partial` modifier. It is valid for only one declaration of a partial type to include the base class or implemented interfaces. However, all declarations of a base class or implemented interfaces shall match, including the nullability of any specified type arguments.

All parts of a partial type shall be compiled together such that the parts can be merged at compile-time. Partial types specifically do not allow already compiled types to be extended.

Nested types may be declared in multiple parts by using the `partial` modifier. Typically, the containing type is declared using `partial` as well, and each part of the nested type is declared in a different part of the containing type.

> *Example*: The following partial class is implemented in two parts, which reside in different compilation units. The first part is machine generated by a database-mapping tool while the second part is manually authored:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"PartialDeclarations1", replaceEllipsis:true, expectedWarnings:["CS0169","CS0169","CS0169","CS0649"], additionalFiles:["Order.cs"]} -->
> ```csharp
> public partial class Customer
> {
>     private int id;
>     private string name;
>     private string address;
>     private List<Order> orders;
>
>     public Customer()
>     {
>         ...
>     }
> }
>
> // File: Customer2.cs
> public partial class Customer
> {
>     public void SubmitOrder(Order orderSubmitted) => orders.Add(orderSubmitted);
>
>     public bool HasOutstandingOrders() => orders.Count > 0;
> }
> ```
>
> When the two parts above are compiled together, the resulting code behaves as if the class had been written as a single unit, as follows:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"PartialDeclarations2", replaceEllipsis:true, expectedWarnings:["CS0169","CS0169","CS0169","CS0649"], additionalFiles:["Order.cs"]} -->
> ```csharp
> public class Customer
> {
>     private int id;
>     private string name;
>     private string address;
>     private List<Order> orders;
>
>     public Customer()
>     {
>         ...
>     }
>
>     public void SubmitOrder(Order orderSubmitted) => orders.Add(orderSubmitted);
>
>     public bool HasOutstandingOrders() => orders.Count > 0;
> }
> ```
>
> *end example*

The handling of attributes specified on the type or type parameters of different parts of a partial type declaration is discussed in [§23.3](attributes.md#233-attribute-specification).

## 15.3 Class members

### 15.3.1 General

The members of a class consist of the members introduced by its *class_member_declaration*s, the members inherited from the direct base class, and any members implicitly provided by the implementation ([§15.16.4](classes.md#15164-implicit-record-class-members)).

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

The signature of a member in a non-file-local type shall not contain a file-local type.

The members of a class are divided into the following categories:

- Constants, which represent constant values associated with the class ([§15.4](classes.md#154-constants)).
- Fields, which are the variables of the class ([§15.5](classes.md#155-fields)).
- Methods, which implement the computations and actions that can be performed by the class ([§15.6](classes.md#156-methods)).
- Properties, which define named characteristics and the actions associated with reading and writing those characteristics ([§15.7](classes.md#157-properties)).
- Events, which define notifications that can be generated by the class ([§15.8](classes.md#158-events)).
- Indexers, which permit instances of the class to be indexed in the same way (syntactically) as arrays ([§15.9](classes.md#159-indexers)).
- Operators, which define the expression operators that can be applied to instances of the class ([§15.10](classes.md#1510-operators)).
- Instance constructors, which implement the actions required to initialize instances of the class ([§15.11](classes.md#1511-instance-constructors))
- Finalizers, which implement the actions to be performed before instances of the class are permanently discarded ([§15.13](classes.md#1513-finalizers)).
- Static constructors, which implement the actions required to initialize the class itself ([§15.12](classes.md#1512-static-constructors)).
- Types, which represent the types that are local to the class ([§14.8](namespaces.md#148-type-declarations)).

A *class_declaration* creates a new declaration space ([§7.3](basic-concepts.md#73-declarations)), and the *type_parameter*s and the *class_member_declaration*s immediately contained by the *class_declaration* introduce new members into this declaration space. The following rules apply to *class_member_declaration*s:

- Instance constructors, finalizers, and static constructors shall have the same name as the immediately enclosing class. All other members shall have names that differ from the name of the immediately enclosing class.

- The name of a type parameter in the *type_parameter_list* of a class declaration shall differ from the names of all other type parameters in the same *type_parameter_list* and shall differ from the name of the class and the names of all members of the class.

- The name of a type shall differ from the names of all non-type members declared in the same class. If two or more type declarations share the same fully qualified name, the declarations shall have the `partial` modifier ([§15.2.7](classes.md#1527-partial-type-declarations)) and these declarations combine to define a single type.

> *Note*: Since the fully qualified name of a type declaration encodes the number of type parameters, two distinct types may share the same name as long as they have different number of type parameters. *end note*

- The name of a constant, field, property, or event shall differ from the names of all other members declared in the same class.
- The name of a method shall differ from the names of all other non-methods declared in the same class. In addition, the signature ([§7.6](basic-concepts.md#76-signatures-and-overloading)) of a method shall differ from the signatures of all other methods declared in the same class, and two methods declared in the same class shall not have signatures that differ solely by `in`, `out`, and `ref`.

- The signature of an instance constructor shall differ from the signatures of all other instance constructors declared in the same class, and two constructors declared in the same class shall not have signatures that differ solely by `ref` and `out`.

- The signature of an indexer shall differ from the signatures of all other indexers declared in the same class.

- The signature of an operator shall differ from the signatures of all other operators declared in the same class.

The inherited members of a class ([§15.3.4](classes.md#1534-inheritance)) are not part of the declaration space of a class.

> *Note*: Thus, a derived class is allowed to declare a member with the same name or signature as an inherited member (which in effect hides the inherited member). *end note*

The set of members of a type declared in multiple parts ([§15.2.7](classes.md#1527-partial-type-declarations)) is the union of the members declared in each part. The bodies of all parts of the type declaration share the same declaration space ([§7.3](basic-concepts.md#73-declarations)), and the scope of each member ([§7.7](basic-concepts.md#77-scopes)) extends to the bodies of all the parts. The accessibility domain of any member always includes all the parts of the enclosing type; a private member declared in one part is freely accessible from another part. It is a compile-time error to declare the same member in more than one part of the type, unless that member has the `partial` modifier.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ClassMembers", expectedErrors:["CS0102"], ignoredWarnings:["CS0169"]} -->
> ```csharp
> partial class A
> {
>     int x;                   // Error, cannot declare x more than once
>     partial void M();        // Ok, defining partial method declaration
>
>     partial class Inner      // Ok, Inner is a partial type
>     {
>         int y;
>     }
> }
>
> partial class A
> {
>     int x;                   // Error, cannot declare x more than once
>     partial void M() { }     // Ok, implementing partial method declaration
>
>     partial class Inner      // Ok, Inner is a partial type
>     {
>         int z;
>     }
> }
> ```
>
> *end example*

Field initialization order can be significant within C# code, and some guarantees are provided, as defined in [§15.5.6.1](classes.md#15561-general). Otherwise, the ordering of members within a type is rarely significant, but may be significant when interfacing with other languages and environments. In these cases, the ordering of members within a type declared in multiple parts is undefined.

It is an error for a member of a record class to be named `Clone`.

It is an error for an instance field of a record class to have an unsafe type.

### 15.3.2 The instance type

Each class declaration has an associated ***instance type***. For a generic class declaration, the instance type is formed by creating a constructed type ([§8.4](types.md#84-constructed-types)) from the type declaration, with each of the supplied type arguments being the corresponding type parameter. Since the instance type uses the type parameters, it can only be used where the type parameters are in scope; that is, inside the class declaration. The instance type is the type of `this` for code written inside the class declaration. For non-generic classes, the instance type is simply the declared class.

> *Example*: The following shows several class declarations along with their instance types:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InstanceType"} -->
> ```csharp
> class A<T>             // instance type: A<T>
> {
>     class B {}         // instance type: A<T>.B
>     class C<U> {}      // instance type: A<T>.C<U>
> }
> class D {}             // instance type: D
> ```
>
> *end example*

### 15.3.3 Members of constructed types

The non-inherited members of a constructed type are obtained by substituting, for each *type_parameter* in the member declaration, the corresponding *type_argument* of the constructed type. The substitution process is based on the semantic meaning of type declarations, and is not simply textual substitution.

> *Example*: Given the generic class declaration
>
> <!-- Example: {template:"standalone-lib-without-using", name:"MembersOfConstructedTypes", replaceEllipsis:true, customEllipsisReplacements:[null,"return default;",null,"return 0;"], expectedWarnings:["CS0649"]} -->
> ```csharp
> class Gen<T,U>
> {
>     public T[,] a;
>     public void G(int i, T t, Gen<U,T> gt) {...}
>     public U Prop { get {...} set {...} }
>     public int H(double d) {...}
> }
> ```
>
> the constructed type `Gen<int[],IComparable<string>>` has the following members:
>
> ```csharp
> public int[,][] a;
> public void G(int i, int[] t, Gen<IComparable<string>,int[]> gt) {...}
> public IComparable<string> Prop { get {...} set {...} }
> public int H(double d) {...}
> ```
>
> The type of the member `a` in the generic class declaration `Gen` is “two-dimensional array of `T`”, so the type of the member `a` in the constructed type above is “two-dimensional array of single-dimensional array of `int`”, or `int[,][]`.
>
> *end example*

Within instance function members, the type of `this` is the instance type ([§15.3.2](classes.md#1532-the-instance-type)) of the containing declaration.

All members of a generic class can use type parameters from any enclosing class, either directly or as part of a constructed type. When a particular closed constructed type ([§8.4.3](types.md#843-open-and-closed-types)) is used at run-time, each use of a type parameter is replaced with the type argument supplied to the constructed type.

> *Example*:
>
> <!-- Example: {template:"standalone-console", name:"TypeParameterSubstitution", expectedOutput:["1","3.1415"]} -->
> ```csharp
> class C<V>
> {
>     public V f1;
>     public C<V> f2;
>
>     public C(V x)
>     {
>         this.f1 = x;
>         this.f2 = this;
>     }
> }
>
> class Application
> {
>     static void Main()
>     {
>         C<int> x1 = new C<int>(1);
>         Console.WriteLine(x1.f1);              // Prints 1
>
>         C<double> x2 = new C<double>(3.1415);
>         Console.WriteLine(x2.f1);              // Prints 3.1415
>     }
> }
> ```
>
> *end example*

### 15.3.4 Inheritance

A class ***inherits*** the members of its direct base class. Inheritance means that a class implicitly contains all members of its direct base class, except for the instance constructors, finalizers, and static constructors of the base class. Some important aspects of inheritance are:

- Inheritance is transitive. If `C` is derived from `B`, and `B` is derived from `A`, then `C` inherits the members declared in `B` as well as the members declared in `A`.

- A derived class *extends* its direct base class. A derived class can add new members to those it inherits, but it cannot remove the definition of an inherited member.

- Instance constructors, finalizers, and static constructors are not inherited, but all other members are, regardless of their declared accessibility ([§7.5](basic-concepts.md#75-member-access)). However, depending on their declared accessibility, inherited members might not be accessible in a derived class.

- A derived class can *hide* ([§7.7.2.3](basic-concepts.md#7723-hiding-through-inheritance)) inherited members by declaring new members with the same name or signature. However, hiding an inherited member does not remove that member—it merely makes that member inaccessible directly through the derived class.

- An instance of a class contains a set of all instance fields declared in the class and its base classes, and an implicit conversion ([§10.2.8](conversions.md#1028-implicit-reference-conversions)) exists from a derived class type to any of its base class types. Thus, a reference to an instance of some derived class can be treated as a reference to an instance of any of its base classes.

- A class can declare virtual methods, properties, indexers, and events, and derived classes can override the implementation of these function members. This enables classes to exhibit polymorphic behavior wherein the actions performed by a function member invocation vary depending on the run-time type of the instance through which that function member is invoked.

The inherited members of a constructed class type are the members of the immediate base class type ([§15.2.4.2](classes.md#15242-base-classes)), which is found by substituting the type arguments of the constructed type for each occurrence of the corresponding type parameters in the *base_class_specification*. These members, in turn, are transformed by substituting, for each *type_parameter* in the member declaration, the corresponding *type_argument* of the *base_class_specification*.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Inheritance", replaceEllipsis:true, customEllipsisReplacements:["return default;","return default;"]} -->
> ```csharp
> class B<U>
> {
>     public U F(long index) {...}
> }
>
> class D<T> : B<T[]>
> {
>     public T G(string s) {...}
> }
> ```
>
> In the code above, the constructed type `D<int>` has a non-inherited member public `int` `G(string s)` obtained by substituting the type argument `int` for the type parameter `T`. `D<int>` also has an inherited member from the class declaration `B`. This inherited member is determined by first determining the base class type `B<int[]>` of `D<int>` by substituting `int` for `T` in the base class specification `B<T[]>`. Then, as a type argument to `B`, `int[]` is substituted for `U` in `public U F(long index)`, yielding the inherited member `public int[] F(long index)`.
>
> *end example*

### 15.3.5 The new modifier

A *class_member_declaration* is permitted to declare a member with the same name or signature as an inherited member. When this occurs, the derived class member is said to *hide* the base class member. However, it is an error to hide a required member ([§15.7.1](classes.md#1571-general)). See [§7.7.2.3](basic-concepts.md#7723-hiding-through-inheritance) for a precise specification of when a member hides an inherited member.

An inherited member `M` is considered to be ***available*** if `M` is accessible and there is no other inherited accessible member N that already hides `M`. Implicitly hiding an inherited member is not considered an error, but a compiler shall issue a warning unless the declaration of the derived class member includes a `new` modifier to explicitly indicate that the derived member is intended to hide the base member. If one or more parts of a partial declaration ([§15.2.7](classes.md#1527-partial-type-declarations)) of a nested type include the `new` modifier, no warning is issued if the nested type hides an available inherited member.

If a `new` modifier is included in a declaration that does not hide an available inherited member, a warning to that effect is issued.

### 15.3.6 Access modifiers

A *class_member_declaration* can have any one of the permitted kinds of declared accessibility ([§7.5.2](basic-concepts.md#752-declared-accessibility)): `public`, `protected internal`, `protected`, `private protected`, `internal`, or `private`. Except for the `protected internal` and `private protected` combinations, it is a compile-time error to specify more than one access modifier. When a *class_member_declaration* does not include any access modifiers, `private` is assumed.

### 15.3.7 Constituent types

Types that are used in the declaration of a member are called the ***constituent type***s of that member. Possible constituent types are the type of a constant, field, property, event, or indexer, the return type of a method or operator, and the parameter types of a method, indexer, operator, or instance constructor. The constituent types of a member shall be at least as accessible as that member itself ([§7.5.5](basic-concepts.md#755-accessibility-constraints)).

### 15.3.8 Static and instance members

Members of a class are either ***static member***s or ***instance member***s.

> *Note*: Generally speaking, it is useful to think of static members as belonging to classes and instance members as belonging to objects (instances of classes). *end note*

When a field, method, property, event, operator, or constructor declaration includes a `static` modifier, it declares a static member. In addition, a constant or type declaration implicitly declares a static member. Static members have the following characteristics:

- When a static member `M` is referenced in a *member_access* ([§12.8.7](expressions.md#1287-member-access)) of the form `E.M`, `E` shall denote a type that has a member `M`. It is a compile-time error for `E` to denote an instance.
- A static field in a non-generic class identifies exactly one storage location. No matter how many instances of a non-generic class are created, there is only ever one copy of a static field. Each distinct closed constructed type ([§8.4.3](types.md#843-open-and-closed-types)) has its own set of static fields, regardless of the number of instances of the closed constructed type.
- A static function member (method, property, event, operator, or constructor) does not operate on a specific instance, and it is a compile-time error to refer to this in such a function member.

When a field, method, property, event, indexer, constructor, or finalizer declaration does not include a static modifier, it declares an instance member. (An instance member is sometimes called a non-static member.) Instance members have the following characteristics:

- When an instance member `M` is referenced in a *member_access* ([§12.8.7](expressions.md#1287-member-access)) of the form `E.M`, `E` shall denote an instance of a type that has a member `M`. It is a binding-time error for E to denote a type.
- Every instance of a class contains a separate set of all instance fields of the class.
- An instance function member (method, property, indexer, instance constructor, or finalizer) operates on a given instance of the class, and this instance can be accessed as `this` ([§12.8.14](expressions.md#12814-this-access)).

> *Example*: The following example illustrates the rules for accessing static and instance members:
>
> <!-- Example: {template:"standalone-console-without-using", name:"StaticAndInstanceMembers", expectedErrors:["CS0120","CS0176","CS0120"], ignoredWarnings:["CS0414"]} -->
> ```csharp
> class Test
> {
>     int x;
>     static int y;
>     void F()
>     {
>         x = 1;               // Ok, same as this.x = 1
>         y = 1;               // Ok, same as Test.y = 1
>     }
>
>     static void G()
>     {
>         x = 1;               // Error, cannot access this.x
>         y = 1;               // Ok, same as Test.y = 1
>     }
>
>     static void Main()
>     {
>         Test t = new Test();
>         t.x = 1;       // Ok
>         t.y = 1;       // Error, cannot access static member through instance
>         Test.x = 1;    // Error, cannot access instance member through type
>         Test.y = 1;    // Ok
>     }
> }
> ```
>
> The `F` method shows that in an instance function member, a *simple_name* ([§12.8.4](expressions.md#1284-simple-names)) can be used to access both instance members and static members. The `G` method shows that in a static function member, it is a compile-time error to access an instance member through a *simple_name*. The `Main` method shows that in a *member_access* ([§12.8.7](expressions.md#1287-member-access)), instance members shall be accessed through instances, and static members shall be accessed through types.
>
> *end example*

### 15.3.9 Nested types

#### 15.3.9.1 General

A type declared within a class, struct, or interface is called a ***nested type***. A type that is declared within a compilation unit or namespace is called a ***non-nested type***.

> *Example*: In the following example:
>
> <!-- Example: {template:"standalone-lib", name:"NestedTypes"} -->
> ```csharp
> class A
> {
>     class B
>     {
>         static void F()
>         {
>             Console.WriteLine("A.B.F");
>         }
>     }
> }
> ```
>
> class `B` is a nested type because it is declared within class `A`, and class `A` is a non-nested type because it is declared within a compilation unit.
>
> *end example*

#### 15.3.9.2 Fully qualified name

The fully qualified name ([§7.8.3](basic-concepts.md#783-fully-qualified-names)) for a nested type declaration is `S.N` where `S` is the fully qualified name of the type declaration in which type `N` is declared and `N` is the unqualified name ([§7.8.2](basic-concepts.md#782-unqualified-names)) of the nested type declaration (including any *generic_dimension_specifier* ([§12.8.18](expressions.md#12818-the-typeof-operator))).

#### 15.3.9.3 Declared accessibility

Non-nested types can have `public` or `internal` declared accessibility and have `internal` declared accessibility by default. Nested types can have these forms of declared accessibility too, plus one or more additional forms of declared accessibility, depending on whether the containing type is a class, struct, or interface:

- A nested type that is declared in a class can have any of the permitted kinds of declared accessibility and, like other class members, defaults to `private` declared accessibility.
- A nested type that is declared in a struct can have any of three forms of declared accessibility (`public`, `internal`, or `private`) and, like other struct members, defaults to `private` declared accessibility.

> *Example*: The example
>
> <!-- Example: {template:"standalone-lib-without-using", name:"DeclaredAccessibility", replaceEllipsis:true, customEllipsisReplacements:[null,null,"return null;","return null;","return 0;"], ignoredWarnings:["CS0414"]} -->
> ```csharp
> public class List
> {
>     // Private data structure
>     private class Node
>     {
>         public object Data;
>         public Node? Next;
>
>         public Node(object data, Node? next)
>         {
>             this.Data = data;
>             this.Next = next;
>         }
>     }
>
>     private Node? first = null;
>     private Node? last = null;
>
>     // Public interface
>     public void AddToFront(object o) {...}
>     public void AddToBack(object o) {...}
>     public object RemoveFromFront() {...}
>     public object RemoveFromBack() {...}
>     public int Count { get {...} }
> }
> ```
>
> declares a private nested class `Node`.
>
> *end example*

#### 15.3.9.4 Hiding

A nested type may hide ([§7.7.2.2](basic-concepts.md#7722-hiding-through-nesting)) a base member. The `new` modifier ([§15.3.5](classes.md#1535-the-new-modifier)) is permitted on nested type declarations so that hiding can be expressed explicitly.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"Hiding", expectedOutput:["Derived.M.F"]} -->
> ```csharp
> class Base
> {
>     public static void M()
>     {
>         Console.WriteLine("Base.M");
>     }
> }
>
> class Derived: Base
> {
>     public new class M
>     {
>         public static void F()
>         {
>             Console.WriteLine("Derived.M.F");
>         }
>     }
> }
>
> class Test
> {
>     static void Main()
>     {
>         Derived.M.F();
>     }
> }
> ```
>
> shows a nested class `M` that hides the method `M` defined in `Base`.
>
> *end example*

#### 15.3.9.5 this access

A nested type and its containing type do not have a special relationship with regard to *this_access* ([§12.8.14](expressions.md#12814-this-access)). Specifically, `this` within a nested type cannot be used to refer to instance members of the containing type. In cases where a nested type needs access to the instance members of its containing type, access can be provided by providing the `this` for the instance of the containing type as a constructor argument for the nested type.

> *Example*: The following example
>
> <!-- Example: {template:"standalone-console", name:"ThisAccess", expectedOutput:["123"]} -->
> ```csharp
> class C
> {
>     int i = 123;
>     public void F()
>     {
>         Nested n = new Nested(this);
>         n.G();
>     }
>
>     public class Nested
>     {
>         C this_c;
>
>         public Nested(C c)
>         {
>             this_c = c;
>         }
>
>         public void G()
>         {
>             Console.WriteLine(this_c.i);
>         }
>     }
> }
>
> class Test
> {
>     static void Main()
>     {
>         C c = new C();
>         c.F();
>     }
> }
> ```
>
> shows this technique. An instance of `C` creates an instance of `Nested`, and passes its own this to `Nested`’s constructor in order to provide subsequent access to `C`’s instance members.
>
> *end example*

#### 15.3.9.6 Access to private and protected members of the containing type

A nested type has access to all of the members that are accessible to its containing type, including members of the containing type that have `private` and `protected` declared accessibility.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"AccessToPrivateAndProtectedMembers1", expectedOutput:["C.F"]} -->
> ```csharp
> class C
> {
>     private static void F() => Console.WriteLine("C.F");
>
>     public class Nested
>     {
>         public static void G() => F();
>     }
> }
>
> class Test
> {
>     static void Main() => C.Nested.G();
> }
> ```
>
> shows a class `C` that contains a nested class `Nested`. Within `Nested`, the method `G` calls the static method `F` defined in `C`, and `F` has private declared accessibility.
>
> *end example*

A nested type also may access protected members defined in a base type of its containing type.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-console", name:"AccessToPrivateAndProtectedMembers2", expectedOutput:["Base.F"]} -->
> ```csharp
> class Base
> {
>     protected void F() => Console.WriteLine("Base.F");
> }
>
> class Derived: Base
> {
>     public class Nested
>     {
>         public void G()
>         {
>             Derived d = new Derived();
>             d.F(); // ok
>         }
>     }
> }
>
> class Test
> {
>     static void Main()
>     {
>         Derived.Nested n = new Derived.Nested();
>         n.G();
>     }
> }
> ```
>
> the nested class `Derived.Nested` accesses the protected method `F` defined in `Derived`’s base class, `Base`, by calling through an instance of `Derived`.
>
> *end example*

#### 15.3.9.7 Nested types in generic classes

A generic class declaration may contain nested type declarations. The type parameters of the enclosing class may be used within the nested types. A nested type declaration may contain additional type parameters that apply only to the nested type.

Every type declaration contained within a generic class declaration is implicitly a generic type declaration. When writing a reference to a type nested within a generic type, the containing constructed type, including its type arguments, shall be named. However, from within the outer class, the nested type may be used without qualification; the instance type of the outer class may be implicitly used when constructing the nested type.

> *Example*: The following shows three different correct ways to refer to a constructed type created from `Inner`; the first two are equivalent:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"NestedTypesInGenericClasses1", replaceEllipsis:true, expectedErrors:["CS0305"]} -->
> ```csharp
> class Outer<T>
> {
>     class Inner<U>
>     {
>         public static void F(T t, U u) {...}
>     }
>
>     static void F(T t)
>     {
>         Outer<T>.Inner<string>.F(t, "abc");    // These two statements have
>         Inner<string>.F(t, "abc");             // the same effect
>         Outer<int>.Inner<string>.F(3, "abc");  // This type is different
>         Outer.Inner<string>.F(t, "abc");       // Error, Outer needs type arg
>     }
> }
> ```
>
> *end example*

Although it is bad programming style, a type parameter in a nested type can hide a member or type parameter declared in the outer type.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"NestedTypesInGenericClasses2", expectedWarnings:["CS0693"], ignoredWarnings:["CS0649"]} -->
> ```csharp
> class Outer<T>
> {
>     class Inner<T>                                  // Valid, hides Outer's T
>     {
>         public T t;                                 // Refers to Inner's T
>     }
> }
> ```
>
> *end example*

### 15.3.10 Reserved member names

#### 15.3.10.1 General

To facilitate the underlying C# run-time implementation, for each source member declaration that is a property, event, or indexer, the implementation shall reserve two method signatures based on the kind of the member declaration, its name, and its type ([§15.3.10.2](classes.md#153102-member-names-reserved-for-properties), [§15.3.10.3](classes.md#153103-member-names-reserved-for-events), [§15.3.10.4](classes.md#153104-member-names-reserved-for-indexers)). It is a compile-time error for a program to declare a member whose signature matches a signature reserved by a member declared in the same scope, even if the underlying run-time implementation does not make use of these reservations.

The reserved names do not introduce declarations, thus they do not participate in member lookup. However, a declaration’s associated reserved method signatures do participate in inheritance ([§15.3.4](classes.md#1534-inheritance)), and can be hidden with the `new` modifier ([§15.3.5](classes.md#1535-the-new-modifier)).

> *Note*: The reservation of these names serves three purposes:
>
> 1. To allow the underlying implementation to use an ordinary identifier as a method name for get or set access to the C# language feature.
> 2. To allow other languages to interoperate using an ordinary identifier as a method name for get or set access to the C# language feature.
> 3. To help ensure that the source accepted by one conforming compiler is accepted by another, by making the specifics of reserved member names consistent across all C# implementations.
>
> *end note*

The declaration of a finalizer ([§15.13](classes.md#1513-finalizers)) also causes a signature to be reserved ([§15.3.10.5](classes.md#153105-member-names-reserved-for-finalizers)).

Certain names are reserved for use as operator method names ([§15.3.10.6](classes.md#153106-method-names-reserved-for-operators)).

#### 15.3.10.2 Member names reserved for properties

For a property `P` ([§15.7](classes.md#157-properties)) of type `T`, the following signatures are reserved:

```csharp
T get_P();
void set_P(T value);
```

Both signatures are reserved, even if the property has only one accessor.

A set accessor and init accessor have the same signature; however, that for the init accessor also has an implementation-defined form of annotation to distinguish it from a set accessor.

> *Example*: In the following code
>
> <!-- TODO: Check why CS0109 (The member 'B.get_P()' does not hide an accessible member. The new keyword is not required.) is emitted. -->
> <!-- Example: {template:"standalone-console", name:"PropertyReservedSignatures", expectedWarnings:["CS0109","CS0109"], inferOutput:true} -->
> ```csharp
> class A
> {
>     public int P
>     {
>         get => 123;
>     }
> }
>
> class B : A
> {
>     public new int get_P() => 456;
>
>     public new void set_P(int value)
>     {
>     }
> }
>
> class Test
> {
>     static void Main()
>     {
>         B b = new B();
>         A a = b;
>         Console.WriteLine(a.P);
>         Console.WriteLine(b.P);
>         Console.WriteLine(b.get_P());
>     }
> }
> ```
>
> A class `A` defines a read-only property `P`, thus reserving signatures for `get_P` and `set_P` methods. `A` class `B` derives from `A` and hides both of these reserved signatures. The example produces the output:
>
> ```console
> 123
> 123
> 456
> ```
>
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

Both signatures are reserved, even if the indexer has only one accessor.

A set accessor and init accessor have the same signature; however, that for the init accessor also has an implementation-defined form of annotation to distinguish it from a set accessor.

Furthermore the member name `Item` is reserved.

#### 15.3.10.5 Member names reserved for finalizers

For a class containing a finalizer ([§15.13](classes.md#1513-finalizers)), the following signature is reserved:

```csharp
void Finalize();
```

#### 15.3.10.6 Method names reserved for operators

The following method names are reserved. While many have corresponding operators in this specification, some are reserved for use by future versions, while some are reserved for interop with other languages.

| **Method Name** | **C# Operator** |
|--------------|----------------------|
| `op_Addition` | `+` (binary) |
| `op_AdditionAssignment` | (reserved) |
| `op_AddressOf` | (reserved) |
| `op_Assign` | (reserved) |
| `op_BitwiseAnd` | `&` (binary) |
| `op_BitwiseAndAssignment` | (reserved) |
| `op_BitwiseOr` | `\|` |
| `op_BitwiseOrAssignment` | (reserved) |
| `op_CheckedAddition` | checked addition `+` |
| `op_CheckedDecrement` | checked decrement `--` |
| `op_CheckedDivision` | checked division `/` |
| `op_CheckedExplicit` | checked explicit (narrowing) coercion |
| `op_CheckedIncrement` | checked increment `++` |
| `op_CheckedMultiply` | checked multiply `*` |
| `op_CheckedSubtraction` | checked subtraction `-` |
| `op_CheckedUnaryNegation` | checked unary negation `-` |
| `op_Comma` | (reserved) |
| `op_Decrement` | `--` (prefix and postfix) |
| `op_Division` | `/` |
| `op_DivisionAssignment` | (reserved) |
| `op_Equality` | `==` |
| `op_ExclusiveOr` | `^` |
| `op_ExclusiveOrAssignment` | (reserved) |
| `op_Explicit` | explicit (narrowing) coercion |
| `op_False` | `false` |
| `op_GreaterThan` | `>` |
| `op_GreaterThanOrEqual` | `>=` |
| `op_Implicit` | implicit (widening) coercion |
| `op_Increment` | `++` (prefix and postfix) |
| `op_Inequality` | `!=` |
| `op_LeftShift` | `<<` |
| `op_LeftShiftAssignment` | (reserved) |
| `op_LessThan` | `<` |
| `op_LessThanOrEqual` | `<=` |
| `op_LogicalAnd` | (reserved) |
| `op_LogicalNot` | `!` |
| `op_LogicalOr` | (reserved) |
| `op_MemberSelection` | (reserved) |
| `op_Modulus` | `%` |
| `op_ModulusAssignment` | (reserved) |
| `op_MultiplicationAssignment` | (reserved) |
| `op_Multiply` | `*` (binary) |
| `op_OnesComplement` | `~` |
| `op_PointerDereference` | (reserved) |
| `op_PointerToMemberSelection` | (reserved) |
| `op_RightShift` | `>>` |
| `op_RightShiftAssignment` | (reserved) |
| `op_SignedRightShift` | (reserved) |
| `op_Subtraction` | `-` (binary) |
| `op_SubtractionAssignment` | (reserved) |
| `op_True` | `true` |
| `op_UnaryNegation` | `-` (unary) |
| `op_UnaryPlus` | `+` (unary) |
| `op_UnsignedRightShift` | (reserved for future use) |
| `op_UnsignedRightShiftAssignment` | (reserved) |

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

A *constant_declaration* may include a set of *attributes* ([§23](attributes.md#23-attributes)), a `new` modifier ([§15.3.5](classes.md#1535-the-new-modifier)), and any one of the permitted kinds of declared accessibility ([§15.3.6](classes.md#1536-access-modifiers)). The attributes and modifiers apply to all of the members declared by the *constant_declaration*. Even though constants are considered static members, a *constant_declaration* neither requires nor allows a `static` modifier. It is an error for the same modifier to appear multiple times in a constant declaration.

The *type* of a *constant_declaration* specifies the type of the members introduced by the declaration. The type is followed by a list of *constant_declarator*s ([§13.6.3](statements.md#1363-local-constant-declarations)), each of which introduces a new member. A *constant_declarator* consists of an *identifier* that names the member, followed by an “`=`” token, followed by a *constant_expression* ([§12.26](expressions.md#1226-constant-expressions)) that gives the value of the member.

The *type* specified in a constant declaration shall be `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `nint`, `nuint`, `long`, `ulong`, `char`, `float`, `double`, `decimal`, `bool`, `string`, an *enum_type*, or a *reference_type*. Each *constant_expression* shall yield a value of the target type or of a type that can be converted to the target type by an implicit conversion ([§10.2](conversions.md#102-implicit-conversions)).

The *type* of a constant shall be at least as accessible as the constant itself ([§7.5.5](basic-concepts.md#755-accessibility-constraints)).

The value of a constant is obtained in an expression using a *simple_name* ([§12.8.4](expressions.md#1284-simple-names)) or a *member_access* ([§12.8.7](expressions.md#1287-member-access)).

A constant can itself participate in a *constant_expression*. Thus, a constant may be used in any construct that requires a *constant_expression*.

> *Note*: Examples of such constructs include `case` labels, `goto case` statements, `enum` member declarations, attributes, and other constant declarations. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: As described in [§12.26](expressions.md#1226-constant-expressions), a *constant_expression* is an expression that can be fully evaluated at compile-time. Since the only way to create a non-null value of a *reference_type* other than `string` is to apply the `new` operator, and since the `new` operator is not permitted in a *constant_expression*, the only possible value for constants of *reference_type*s other than `string` is `null`. *end note*

When a symbolic name for a constant value is desired, but when the type of that value is not permitted in a constant declaration, or when the value cannot be computed at compile-time by a *constant_expression*, a readonly field ([§15.5.3](classes.md#1553-readonly-fields)) may be used instead.

> *Note*: The versioning semantics of `const` and `readonly` differ ([§15.5.3.3](classes.md#15533-versioning-of-constants-and-static-readonly-fields)). *end note*

A constant declaration that declares multiple constants is equivalent to multiple declarations of single constants with the same attributes, modifiers, and type.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Constants1"} -->
> ```csharp
> class A
> {
>     public const double X = 1.0, Y = 2.0, Z = 3.0;
> }
> ```
>
> is equivalent to
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Constants2"} -->
> ```csharp
> class A
> {
>     public const double X = 1.0;
>     public const double Y = 2.0;
>     public const double Z = 3.0;
> }
> ```
>
> *end example*

Constants are permitted to depend on other constants within the same program as long as the dependencies are not of a circular nature.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Constants3"} -->
> ```csharp
> class A
> {
>     public const int X = B.Z + 1;
>     public const int Y = 10;
> }
>
> class B
> {
>     public const int Z = A.Y + 1;
> }
> ```
>
> a compiler must first evaluate `A.Y`, then evaluate `B.Z`, and finally evaluate `A.X`, producing the values `10`, `11`, and `12`.
>
> *end example*

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
    | 'required'
    | unsafe_modifier   // unsafe code support
    ;

variable_declarators
    : variable_declarator (',' variable_declarator)*
    ;

variable_declarator
    : identifier ('=' variable_initializer)?
    ;
```

*unsafe_modifier* ([§24.2](unsafe-code.md#242-unsafe-contexts)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).

A *field_declaration* may include a set of *attributes* ([§23](attributes.md#23-attributes)), a `new` modifier ([§15.3.5](classes.md#1535-the-new-modifier)), a valid combination of the four access modifiers ([§15.3.6](classes.md#1536-access-modifiers)), and a `static` modifier ([§15.5.2](classes.md#1552-static-and-instance-fields)). In addition, a *field_declaration* may include a `readonly` modifier ([§15.5.3](classes.md#1553-readonly-fields)) or a `volatile` modifier ([§15.5.4](classes.md#1554-volatile-fields)), but not both. A *field_declaration* may also include a `required` modifier ([§15.7.1](classes.md#1571-general)), provided it does not have a `readonly` modifier. The attributes and modifiers apply to all of the members declared by the *field_declaration*. It is an error for the same modifier to appear multiple times in a *field_declaration*.

The *type* of a *field_declaration* specifies the type of the members introduced by the declaration. The type is followed by a list of *variable_declarator*s, each of which introduces a new member. A *variable_declarator* consists of an *identifier* that names that member, optionally followed by an “`=`” token and a *variable_initializer* ([§15.5.6](classes.md#1556-variable-initializers)) that gives the initial value of that member.

The *type* of a field shall be at least as accessible as the field itself ([§7.5.5](basic-concepts.md#755-accessibility-constraints)).

The value of a field is obtained in an expression using a *simple_name* ([§12.8.4](expressions.md#1284-simple-names)), a *member_access* ([§12.8.7](expressions.md#1287-member-access)) or a base_access ([§12.8.15](expressions.md#12815-base-access)). The value of a non-readonly field is modified using an *assignment* ([§12.24](expressions.md#1224-assignment-operators)). The value of a non-readonly field can be both obtained and modified using postfix increment and decrement operators ([§12.8.16](expressions.md#12816-postfix-increment-and-decrement-operators)) and prefix increment and decrement operators ([§12.9.7](expressions.md#1297-prefix-increment-and-decrement-operators)).

A field declaration that declares multiple fields is equivalent to multiple declarations of single fields with the same attributes, modifiers, and type.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Fields1", ignoredWarnings:["CS0649"]} -->
> ```csharp
> class A
> {
>     public static int X = 1, Y, Z = 100;
> }
> ```
>
> is equivalent to
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Fields2", ignoredWarnings:["CS0649"]} -->
> ```csharp
> class A
> {
>     public static int X = 1;
>     public static int Y;
>     public static int Z = 100;
> }
> ```
>
> *end example*

### 15.5.2 Static and instance fields

When a field declaration includes a `static` modifier, the fields introduced by the declaration are ***static field***s. When no `static` modifier is present, the fields introduced by the declaration are ***instance field***s. Static fields and instance fields are two of the several kinds of variables ([§9](variables.md#9-variables)) supported by C#, and at times they are referred to as ***static variable***s and ***instance variable***s, respectively.

As explained in [§15.3.8](classes.md#1538-static-and-instance-members), each instance of a class contains a complete set of the instance fields of the class, while there is only one set of static fields for each non-generic class or closed constructed type, regardless of the number of instances of the class or closed constructed type.

### 15.5.3 Readonly fields

#### 15.5.3.1 General

When a *field_declaration* includes a `readonly` modifier, the fields introduced by the declaration are ***readonly field***s. Direct assignments to readonly fields can only occur as part of that declaration or in an instance constructor or static constructor in the same class. (A readonly field can be assigned to multiple times in these contexts.) Specifically, direct assignments to a readonly field are permitted only in the following contexts:

- In the *variable_declarator* that introduces the field (by including a *variable_initializer* in the declaration).
- For an instance field, in the instance constructors of the class that contains the field declaration, excluding local functions and anonymous functions, and only on the instance being constructed. For a static field, in the static constructor or a static field or property initializer in the class that contains the field declaration, excluding local functions and anonymous functions. These are also the only contexts in which it is valid to pass a readonly field as an output or reference parameter.

Attempting to assign to a readonly field or pass it as an output or reference parameter in any other context is a compile-time error.

#### 15.5.3.2 Using static readonly fields for constants

A static readonly field is useful when a symbolic name for a constant value is desired, but when the type of the value is not permitted in a const declaration, or when the value cannot be computed at compile-time.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"StaticReadonlyFieldsAsConstants"} -->
> ```csharp
> public class Color
> {
>     public static readonly Color Black = new Color(0, 0, 0);
>     public static readonly Color White = new Color(255, 255, 255);
>     public static readonly Color Red = new Color(255, 0, 0);
>     public static readonly Color Green = new Color(0, 255, 0);
>     public static readonly Color Blue = new Color(0, 0, 255);
>
>     private byte red, green, blue;
>
>     public Color(byte r, byte g, byte b)
>     {
>         red = r;
>         green = g;
>         blue = b;
>     }
> }
> ```
>
> the `Black`, `White`, `Red`, `Green`, and `Blue` members cannot be declared as const members because their values cannot be computed at compile-time. However, declaring them `static readonly` instead has much the same effect.
>
> *end example*

#### 15.5.3.3 Versioning of constants and static readonly fields

Constants and readonly fields have different binary versioning semantics. When an expression references a constant, the value of the constant is obtained at compile-time, but when an expression references a readonly field, the value of the field is not obtained until run-time.

> *Example*: Consider an application that consists of two separate programs:
>
> <!-- Incomplete$Example: {template:"standalone-lib-without-using", name:"VersioningOfConstantsAndStaticReadonlyFields1"} -->
> ```csharp
> namespace Program1
> {
>     public class Utils
>     {
>         public static readonly int x = 1;
>     }
> }
> ```
>
> and
>
> <!-- Incomplete$Example: {template:"standalone-console", name:"VersioningOfConstantsAndStaticReadonlyFields2", expectedOutput:["x", "x", "x"], expectedErrors:["x","x"], expectedWarnings:["x","x"]} -->
> ```csharp
> namespace Program2
> {
>     class Test
>     {
>         static void Main()
>         {
>             Console.WriteLine(Program1.Utils.X);
>         }
>     }
> }
> ```
>
> The `Program1` and `Program2` namespaces denote two programs that are compiled separately. Because `Program1.Utils.X` is declared as a `static readonly` field, the value output by the `Console.WriteLine` statement is not known at compile-time, but rather is obtained at run-time. Thus, if the value of `X` is changed and `Program1` is recompiled, the `Console.WriteLine` statement will output the new value even if `Program2` is not recompiled. However, had `X` been a constant, the value of `X` would have been obtained at the time `Program2` was compiled, and would remain unaffected by changes in `Program1` until `Program2` is recompiled.
>
> *end example*

### 15.5.4 Volatile fields

When a *field_declaration* includes a `volatile` modifier, the fields introduced by that declaration are ***volatile field***s. For non-volatile fields, optimization techniques that reorder instructions can lead to unexpected and unpredictable results in multi-threaded programs that access fields without synchronization such as that provided by the *lock_statement* ([§13.13](statements.md#1313-the-lock-statement)). These optimizations can be performed by the compiler, by the run-time system, or by hardware. For volatile fields, such reordering optimizations are restricted:

- A read of a volatile field is called a ***volatile read***. A volatile read has “acquire semantics”; that is, it is guaranteed to occur prior to any references to memory that occur after it in the instruction sequence.
- A write of a volatile field is called a ***volatile write***. A volatile write has “release semantics”; that is, it is guaranteed to happen after any memory references prior to the write instruction in the instruction sequence.

These restrictions ensure that all threads will observe volatile writes performed by any other thread in the order in which they were performed. A conforming implementation is not required to provide a single total ordering of volatile writes as seen from all threads of execution. The type of a volatile field shall be one of the following:

- A *reference_type*.
- A *type_parameter* that is known to be a reference type ([§15.2.5](classes.md#1525-type-parameter-constraints)).
- The type `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `nint`, `nuint`, `char`, `float`, `bool`, `System.IntPtr`, or `System.UIntPtr`.
- An *enum_type* having an *enum_base* type of `byte`, `sbyte`, `short`, `ushort`, `int`, or `uint`.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"VolatileFields", inferOutput:true} -->
> ```csharp
> class Test
> {
>     public static int result;
>     public static volatile bool finished;
>
>     static void Thread2()
>     {
>         result = 143;
>         finished = true;
>     }
>
>     static void Main()
>     {
>         finished = false;
>
>         // Run Thread2() in a new thread
>         new Thread(new ThreadStart(Thread2)).Start();    
>
>         // Wait for Thread2() to signal that it has a result
>         // by setting finished to true.
>         for (;;)
>         {
>             if (finished)
>             {
>                 Console.WriteLine($"result = {result}");
>                 return;
>             }
>         }
>     }
> }
>
> ```
>
> produces the output:
>
> ```console
> result = 143
> ```
>
> In this example, the method `Main` starts a new thread that runs the method `Thread2`. This method stores a value into a non-volatile field called `result`, then stores `true` in the volatile field `finished`. The main thread waits for the field `finished` to be set to `true`, then reads the field `result`. Since `finished` has been declared `volatile`, the main thread shall read the value `143` from the field `result`. If the field `finished` had not been declared `volatile`, then it would be permissible for the store to `result` to be visible to the main thread *after* the store to `finished`, and hence for the main thread to read the value 0 from the field `result`. Declaring `finished` as a `volatile` field prevents any such inconsistency.
>
> *end example*

### 15.5.5 Field initialization

The initial value of a field, whether it be a static field or an instance field, is the default value ([§9.3](variables.md#93-default-values)) of the field’s type. It is not possible to observe the value of a field before this default initialization has occurred, and a field is thus never “uninitialized”.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"FieldInitialization", ignoredWarnings:["CS0649"], inferOutput:true} -->
> ```csharp
> class Test
> {
>     static bool b;
>     int i;
>
>     static void Main()
>     {
>         Test t = new Test();
>         Console.WriteLine($"b = {b}, i = {t.i}");
>     }
> }
> ```
>
> produces the output
>
> ```console
> b = False, i = 0
> ```
>
> because `b` and `i` are both automatically initialized to default values.
>
> *end example*

### 15.5.6 Variable initializers

#### 15.5.6.1 General

Field declarations may include *variable_initializer*s. For static fields, variable initializers correspond to assignment statements that are executed during class initialization. For instance fields, variable initializers correspond to assignment statements that are executed when an instance of the class is created.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"VariableInitializers1", inferOutput:true} -->
> ```csharp
> class Test
> {
>     static double x = Math.Sqrt(2.0);
>     int i = 100;
>     string s = "Hello";
>
>     static void Main()
>     {
>         Test a = new Test();
>         Console.WriteLine($"x = {x}, i = {a.i}, s = {a.s}");
>     }
> }
> ```
>
> produces the output
>
> ```console
> x = 1.4142135623730951, i = 100, s = Hello
> ```
>
> because an assignment to `x` occurs when static field initializers execute and assignments to `i` and `s` occur when the instance field initializers execute.
>
> *end example*

The default value initialization described in [§15.5.5](classes.md#1555-field-initialization) occurs for all fields, including fields that have variable initializers. Thus, when a class is initialized, all static fields in that class are first initialized to their default values, and then the static field initializers are executed in textual order. Likewise, when an instance of a class is created, all instance fields in that instance are first initialized to their default values, and then the instance field initializers are executed in textual order. When there are field declarations in multiple partial type declarations for the same type, the order of the parts is unspecified. However, within each part the field initializers are executed in order.

It is possible for static fields with variable initializers to be observed in their default value state.

> *Example*: However, this is strongly discouraged as a matter of style. The example
>
> <!-- Example: {template:"standalone-console", name:"VariableInitializers2", inferOutput:true} -->
> ```csharp
> class Test
> {
>     static int a = b + 1;
>     static int b = a + 1;
>
>     static void Main()
>     {
>         Console.WriteLine($"a = {a}, b = {b}");
>     }
> }
> ```
>
> exhibits this behavior. Despite the circular definitions of `a` and `b`, the program is valid. It results in the output
>
> ```console
> a = 1, b = 2
> ```
>
> because the static fields `a` and `b` are initialized to `0` (the default value for `int`) before their initializers are executed. When the initializer for `a` runs, the value of `b` is zero, and so `a` is initialized to `1`. When the initializer for `b` runs, the value of a is already `1`, and so `b` is initialized to `2`.
>
> *end example*

#### 15.5.6.2 Static field initialization

The static field variable initializers of a class correspond to a sequence of assignments that are executed in the textual order in which they appear in the class declaration ([§15.5.6.1](classes.md#15561-general)). Within a partial class, the meaning of “textual order” is specified by [§15.5.6.1](classes.md#15561-general). If a static constructor ([§15.12](classes.md#1512-static-constructors)) exists in the class, execution of the static field initializers occurs immediately prior to executing that static constructor. Otherwise, the static field initializers are executed at an implementation-dependent time prior to the first use of a static field of that class.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"StaticFieldInitialization1", ignoreOutput:true} -->
> <!-- Maintenance Note: The behavior of this example is implementation-defined. As such, the metadata does not compare test output with any ```console block. -->
> ```csharp
> class Test
> {
>     static void Main()
>     {
>         Console.WriteLine($"{B.Y} {A.X}");
>     }
>
>     public static int F(string s)
>     {
>         Console.WriteLine(s);
>         return 1;
>     }
> }
>
> class A
> {
>     public static int X = Test.F("Init A");
> }
>
> class B
> {
>     public static int Y = Test.F("Init B");
> }
> ```
>
> might produce either the output:
>
> ```console
> Init A
> Init B
> 1 1
> ```
>
> or the output:
>
> ```console
> Init B
> Init A
> 1 1
> ```
>
> because the execution of `X`’s initializer and `Y`’s initializer could occur in either order; they are only constrained to occur before the references to those fields. However, in the example:
>
> <!-- Example: {template:"standalone-console", name:"StaticFieldInitialization2", inferOutput:true} -->
> ```csharp
> class Test
> {
>     static void Main()
>     {
>         Console.WriteLine($"{B.Y} {A.X}");
>     }
>
>     public static int F(string s)
>     {
>         Console.WriteLine(s);
>         return 1;
>     }
> }
>
> class A
> {
>     static A() {}
>     public static int X = Test.F("Init A");
> }
>
> class B
> {
>     static B() {}
>     public static int Y = Test.F("Init B");
> }
> ```
>
> the output shall be:
>
> ```console
> Init B
> Init A
> 1 1
> ```
>
> because the rules for when static constructors execute (as defined in [§15.12](classes.md#1512-static-constructors)) provide that `B`’s static constructor (and hence `B`’s static field initializers) shall run before `A`’s static constructor and field initializers.
>
> *end example*

#### 15.5.6.3 Instance field initialization

The instance field variable initializers of a class correspond to a sequence of assignments that are executed immediately upon entry to any one of the instance constructors ([§15.11.3](classes.md#15113-instance-variable-initializers)) of that class. Within a partial class, the meaning of “textual order” is specified by [§15.5.6.1](classes.md#15561-general). The variable initializers are executed in the textual order in which they appear in the class declaration ([§15.5.6.1](classes.md#15561-general)). The class instance creation and initialization process is described further in [§15.11](classes.md#1511-instance-constructors).

A variable initializer for an instance field cannot reference the instance being created. Thus, it is a compile-time error to reference `this` in a variable initializer, as it is a compile-time error for a variable initializer to reference any instance member through a *simple_name*.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"InstanceFieldInitialization", expectedErrors:["CS0236"]} -->
> ```csharp
> class A
> {
>     int x = 1;
>     int y = x + 1;     // Error, reference to instance member of this
> }
> ```
>
> the variable initializer for `y` results in a compile-time error because it references a member of the instance being created.
>
> *end example*

## 15.6 Methods

### 15.6.1 General

[§15.6](classes.md#156-methods) and its subclauses cover method declarations in classes. That text is augmented by information about declaring methods in structs ([§16.6](structs.md#166-class-and-struct-differences)) and interfaces ([§19.4.3](interfaces.md#1943-interface-methods)).

A ***method*** is a member that implements a computation or action that can be performed by an object or class. Methods are declared using *method_declaration*s:

```ANTLR
method_declaration
    : attributes? method_modifiers return_type method_header method_body
    | attributes? ref_method_modifiers ref_kind ref_return_type method_header
      ref_method_body
    ;

method_modifiers
    : method_modifier* 'partial'?
    ;

ref_kind
    : 'ref'
    | 'ref' 'readonly'
    ;

ref_method_modifiers
    : ref_method_modifier* 'partial'?
    ;

method_header
    : member_name '(' parameter_list? ')'
    | member_name type_parameter_list '(' parameter_list? ')'
      type_parameter_constraints_clause*
    ;

method_modifier
    : ref_method_modifier
    | 'async'
    ;

ref_method_modifier
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
    | 'readonly'        // struct members only
    | unsafe_modifier   // unsafe code support
    ;

return_type
    : ref_return_type
    | 'void'
    ;

ref_return_type
    : type
    ;

member_name
    : identifier
    | interface_type '.' identifier
    ;

method_body
    : block
    | '=>' null_conditional_invocation_expression ';'
    | '=>' expression ';'
    | ';'
    ;

ref_method_body
    : block
    | '=>' 'ref' variable_reference ';'
    | ';'
    ;
```

Grammar notes:

- *unsafe_modifier* ([§24.2](unsafe-code.md#242-unsafe-contexts)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).
- when recognising a *method_body* if both the *null_conditional_invocation_expression* and *expression* alternatives are applicable then the former shall be chosen.

> *Note*: The overlapping of, and priority between, alternatives here is solely for descriptive convenience; the grammar rules could be elaborated to remove the overlap. ANTLR, and other grammar systems, adopt the same convenience and so *method_body* has the specified semantics automatically. *end note*

A *method_declaration* may include a set of *attributes* ([§23](attributes.md#23-attributes)) and one of the permitted kinds of declared accessibility ([§15.3.6](classes.md#1536-access-modifiers)), the `new` ([§15.3.5](classes.md#1535-the-new-modifier)), `static` ([§15.6.3](classes.md#1563-static-and-instance-methods)), `virtual` ([§15.6.4](classes.md#1564-virtual-methods)), `override` ([§15.6.5](classes.md#1565-override-methods)), `sealed` ([§15.6.6](classes.md#1566-sealed-methods)), `abstract` ([§15.6.7](classes.md#1567-abstract-methods)), `extern` ([§15.6.8](classes.md#1568-external-methods)) and `async` ([§15.14](classes.md#1514-async-functions)) modifiers. Additionally a  *method_declaration* that is contained directly by a *struct_declaration* may include the `readonly` modifier ([§16.6.12](structs.md#16612-methods)).

A *method_declaration* has a valid combination of modifiers if all of the following are true. (These rules are modified slightly in the context of an interface; see [§19.4.1](interfaces.md#1941-general).):

- The declaration includes a valid combination of access modifiers ([§15.3.6](classes.md#1536-access-modifiers)).
- The declaration does not include the same modifier multiple times.
- The declaration includes at most one of the following modifiers: `static`, `virtual`, and `override`.
- The declaration includes at most one of the following modifiers: `new` and `override`.
- If the declaration includes the `abstract` modifier, then the declaration does not include any of the following modifiers: `static`, `virtual`, `sealed`, or `extern`.
- The declaration may include the `abstract` and `override` modifiers so that an abstract member may override a virtual member.
- If the declaration includes the `private` modifier, then the declaration does not include any of the following modifiers: `virtual`, `override`, or `abstract`.
- If the declaration includes the `sealed` modifier, then the declaration also includes the `override` modifier.
- If the declaration includes the `partial` modifier, then it does not include the modifier `abstract`.
- If the declaration is for an optional partial method ([§15.6.9.2](classes.md#15692-optional-partial-methods)), then it does not include any of the following modifiers: `new`, `public`, `protected`, `internal`, `private`, `virtual`, `sealed`, `override`, or `extern`.

Methods are classified according to what, if anything, they return:

- If `ref` is present, the method is ***returns-by-ref*** and returns a *variable reference*, that is optionally read-only;
- Otherwise, if *return_type* is `void`, the method is ***returns-no-value*** and does not return a value;
- Otherwise, the method is ***returns-by-value*** and returns a value.

The *return_type* of a returns-by-value or returns-no-value method declaration specifies the type of the result, if any, returned by the method. If the declaration includes the `async` modifier then *return_type* shall be `void` or the method returns-by-value and the return type shall be a *task type* ([§15.14.1](classes.md#15141-general)).

The *ref_return_type* of a returns-by-ref method declaration specifies the type of the variable referenced by the *variable_reference* returned by the method.

A generic method is a method whose declaration includes a *type_parameter_list*. This specifies the type parameters for the method. The optional *type_parameter_constraints_clause*s specify the constraints for the type parameters.

A generic *method_declaration*, either with an `override` modifier, or for an explicit interface member implementation, inherits type parameter constraints from the overridden method or interface member respectively. Such declarations may only have *type_parameter_constraints_clause*s containing the *primary_constraint*s `class`, `struct`, and `default`, the meaning of which in this context is defined in [§15.6.5](classes.md#1565-override-methods) and [§19.6.2](interfaces.md#1962-explicit-interface-member-implementations) for overriding methods and explicit interface implementations respectively.

The *member_name* specifies the name of the method. Unless the method is an explicit interface member implementation ([§19.6.2](interfaces.md#1962-explicit-interface-member-implementations)), the *member_name* is simply an *identifier*.

For an explicit interface member implementation, the *member_name* consists of an *interface_type* followed by a “`.`” and an *identifier*. In this case, the declaration shall include no modifiers other than (possibly) `extern` or `async`.

The optional *parameter_list* specifies the parameters of the method ([§15.6.2](classes.md#1562-method-parameters)).

The *return_type* or *ref_return_type*, and each of the types referenced in the *parameter_list* of a method, shall be at least as accessible as the method itself ([§7.5.5](basic-concepts.md#755-accessibility-constraints)).

The *method_body* of a returns-by-value or returns-no-value method is either a semicolon, a block body or an expression body. A block body consists of a *block*, which specifies the statements to execute when the method is invoked. An expression body consists of `=>`, followed by a *null_conditional_invocation_expression* or *expression*, and a semicolon, and denotes a single expression to perform when the method is invoked.

For abstract and extern methods, the *method_body* consists simply of a semicolon. For partial methods the *method_body* may consist of either a semicolon, a block body or an expression body. For all other methods, the *method_body* is either a block body or an expression body.

If the *method_body* consists of a semicolon, the declaration shall not include the `async` modifier.

The *ref_method_body* of a returns-by-ref method is either a semicolon, a block body or an expression body. A block body consists of a *block*, which specifies the statements to execute when the method is invoked. An expression body consists of `=>`, followed by `ref`, a *variable_reference*, and a semicolon, and denotes a single *variable_reference* to evaluate when the method is invoked.

For abstract and extern methods, the *ref_method_body* consists simply of a semicolon. For partial methods the *ref_method_body* may consist of either a semicolon, a block body or an expression body. For all other methods, the *ref_method_body* is either a block body or an expression body.

The name, the number of type parameters, and the parameter list of a method define the signature ([§7.6](basic-concepts.md#76-signatures-and-overloading)) of the method. Specifically, the signature of a method consists of its name, the number of its type parameters, and the number, *parameter_mode_modifier*s ([§15.6.2.1](classes.md#15621-general)), and types of its parameters. The return type is not part of a method’s signature, nor are the names of the parameters, the names of the type parameters, or the constraints. When a parameter type references a type parameter of the method, the ordinal position of the type parameter (not the name of the type parameter) is used for type equivalence.

The name of a method shall differ from the names of all other non-methods declared in the same class. In addition, the signature of a method shall differ from the signatures of all other methods declared in the same class, and two methods declared in the same class shall not have signatures that differ solely by `in`, `out`, and `ref`.

The method’s *type_parameter*s are in scope throughout the *method_declaration*, and can be used to form types throughout that scope in *return_type* or *ref_return_type*, *method_body* or *ref_method_body*, and *type_parameter_constraints_clause*s but not in *attributes*, except within a `nameof` expression in *attributes*.

All parameters and type parameters shall have different names.

### 15.6.2 Method parameters

#### 15.6.2.1 General

The parameters of a method, if any, are declared by the method’s *parameter_list*.

```ANTLR
delimited_parameter_list
    : '(' parameter_list? ')'
    ;

parameter_list
    : fixed_parameters
    | fixed_parameters ',' parameter_collection
    | parameter_collection
    ;

fixed_parameters
    : fixed_parameter (',' fixed_parameter)*
    ;

fixed_parameter
    : attributes? parameter_modifier? type identifier default_argument?
    ;

default_argument
    : '=' expression
    ;

parameter_modifier
    : parameter_mode_modifier
    | 'this' 'scoped'? parameter_mode_modifier?
    | 'scoped'? parameter_mode_modifier? 'this'
    | 'scoped' parameter_mode_modifier?
    ;

parameter_mode_modifier
    : ref_kind
    | 'out'
    | 'in'
    ;

parameter_collection
    : attributes? 'params' type identifier
    ;
```

The parameter list consists of one or more comma-separated parameters of which only the last may be a *parameter_collection*.

A *fixed_parameter* consists of an optional set of *attributes* ([§23](attributes.md#23-attributes)); an optional `this` modifier; an optional `scoped` modifier; an optional `in`, `out`, `ref` modifier, or `ref readonly`; a *type*; an *identifier*; and an optional *default_argument*. Each *fixed_parameter* declares a parameter of the given type with the given name. The `this` modifier designates the method as an extension method and is only allowed on the first parameter of a static method in a non-generic, non-nested static class. If the parameter is a `struct` type or a type parameter constrained to a `struct`, the `this` modifier may be combined with the `ref`, `ref readonly`, or `in` modifier, but not the `out` modifier. Extension methods are further described in [§15.6.10](classes.md#15610-extension-methods). A *fixed_parameter* with a *default_argument* is known as an ***optional parameter***, whereas a *fixed_parameter* without a *default_argument* is a ***required parameter***. A required parameter shall not appear after an optional parameter in a *parameter_list*.

An output parameter implicitly has the `scoped` modifier.

For a discussion of `scoped`, see [§9.7.3](variables.md#973-the-scoped-modifier).

A parameter with a `ref`, `out` or `this` modifier cannot have a *default_argument*. A parameter with an `ref readonly` or `in` modifier may have a *default_argument*; however, in the `ref readonly` case, a warning shall be issued. The *expression* in a *default_argument* shall be one of the following:

- a *constant_expression*
- an expression of the form `new S()` where `S` is a value type
- an expression of the form `default(S)` where `S` is a value type

The *expression* shall be implicitly convertible by an identity or nullable conversion to the type of the parameter.

If optional parameters occur in an implementing partial method declaration ([§15.6.9](classes.md#1569-partial-methods)), an explicit interface member implementation ([§19.6.2](interfaces.md#1962-explicit-interface-member-implementations)), a single-parameter indexer declaration ([§15.9](classes.md#159-indexers)), or in an operator declaration ([§15.10.1](classes.md#15101-general)) a compiler should give a warning, since these members can never be invoked in a way that permits arguments to be omitted.

A *parameter_collection* consists of an optional set of *attributes* ([§23](attributes.md#23-attributes)), a `params` modifier, a *type*, and an *identifier*. A parameter collection declares a single parameter of the given array type with the given name. The *type* of a parameter collection shall be one of the following valid target types for a collection expression:

- A single dimensional array type `T[]`, in which case the element type is `T`
- A span type
  - `System.Span<T>`
  - `System.ReadOnlySpan<T>`
  in which cases the element type is `T`
- A type with an appropriate collection-creation method ((§15.17.1](classes.md#15171-general)) that can be invoked with no additional arguments, which is at least as accessible as the declaring member, and with a corresponding element type resulting from that determination
- A struct or class type that implements `System.Collections.IEnumerable` where:
  - The type has a constructor that can be invoked with no arguments, and the constructor is at least as accessible as the declaring member.
  - The type has an instance (not an extension) method `Add` where:
    - The method can be invoked with a single value argument.
    - If the method is generic, the type arguments can be inferred from the argument.
    - The method is at least as accessible as the declaring member.
    In which case the element type is the iteration type ([§13.9.5.1]( statements.md#13951-general)) of *type*.
- An interface type
  - `System.Collections.Generic.IEnumerable<T>`
  - `System.Collections.Generic.IReadOnlyCollection<T>`
  - `System.Collections.Generic.IReadOnlyList<T>`
  - `System.Collections.Generic.ICollection<T>`
  - `System.Collections.Generic.IList<T>`  
  in which case the element type is `T`.

In a method invocation, a parameter collection permits either a single argument of the given array type to be specified, or it permits zero or more arguments of the array element type to be specified. Parameter collections are described further in [§15.6.2.4](classes.md#15624-parameter-arrays).

A *parameter_collection* may occur after an optional parameter, but cannot have a default value – the omission of arguments for a *parameter_collection* would instead result in the creation of an empty collection.

> *Example*: The following illustrates different kinds of parameters:
>
> <!-- Example: {template:"standalone-console-without-using", name:"MethodParameters", ignoredWarnings:["CS8321"]} -->
> ```csharp
> void M<T>(
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
>
> In the *parameter_list* for `M`, `i` is a required `ref` parameter, `d` is a required value parameter, `b`, `s`, `o` and `t` are optional value parameters and `a` is a parameter collection.
>
> *end example*

A method declaration creates a separate declaration space ([§7.3](basic-concepts.md#73-declarations)) for parameters and type parameters. Names are introduced into this declaration space by the type parameter list and the parameter list of the method. Names are also introduced into this declaration space by the type parameter list and the formal parameter list of the method in `nameof` expressions in attributes placed on the method or its parameters. The body of the method, if any, is considered to be nested within this declaration space. It is an error for two members of a method declaration space to have the same name.

A method invocation ([§12.8.10.2](expressions.md#128102-method-invocations)) creates a copy, specific to that invocation, of the parameters and local variables of the method, and the argument list of the invocation assigns values or variable references to the newly created parameters. Within the *block* of a method, parameters can be referenced by their identifiers in *simple_name* expressions ([§12.8.4](expressions.md#1284-simple-names)). Within a `nameof` expression in attributes placed on the method or its parameters, formal parameters can be referenced by their identifiers in *simple_name* expressions.

The following kinds of parameters exist:

- Value parameters ([§15.6.2.2](classes.md#15622-value-parameters)).
- Input parameters ([§15.6.2.3.2](classes.md#156232-input-parameters)).
- Output parameters ([§15.6.2.3.4](classes.md#156234-output-parameters)).
- Reference parameters ([§15.6.2.3.3](classes.md#156233-reference-parameters)).
- Reference readonly parameters, which are reference parameters that also have the `readonly` modifier.
- Parameter collections ([§15.6.2.4](classes.md#15624-parameter-collections)).

> *Note*: As described in [§7.6](basic-concepts.md#76-signatures-and-overloading), the `in`, `out`, `ref`, and `ref readonly` modifiers are part of a method’s signature, but the `params` and `scoped` modifiers are not. *end note*

#### 15.6.2.2 Value parameters

A parameter declared with no modifiers is a ***value parameter***. A value parameter is a local variable that gets its initial value from the corresponding argument supplied in the method invocation.

For definite-assignment rules, see [§9.2.5](variables.md#925-value-parameters).

The corresponding argument in a method invocation shall be an expression that is implicitly convertible ([§10.2](conversions.md#102-implicit-conversions)) to the parameter type.

A method is permitted to assign new values to a value parameter. Such assignments only affect the local storage location represented by the value parameter—they have no effect on the actual argument given in the method invocation.

#### 15.6.2.3 By-reference parameters

##### 15.6.2.3.1 General

Input, output, and reference parameters are ***by-reference parameter***s. A by-reference parameter is a local reference variable ([§9.7](variables.md#97-reference-variables-and-returns)); the initial referent is obtained from the corresponding argument supplied in the method invocation.

> *Note*: The referent of a by-reference parameter can be changed using the ref assignment (`= ref`) operator.

When a parameter is a by-reference parameter, the corresponding argument in a method invocation shall consist of the corresponding keyword, `in`, `ref`, or `out`, followed by a *variable_reference* ([§9.5](variables.md#95-variable-references)) of the same type as the parameter. However, when the parameter is a `ref readonly` or `in` parameter, the argument may be an *expression* for which an implicit conversion ([§10.2](conversions.md#102-implicit-conversions)) exists from that argument expression to the type of the corresponding parameter.

By-reference parameters are not allowed on functions declared as an iterator ([§15.15](classes.md#1515-synchronous-and-asynchronous-iterators)) or async function ([§15.14](classes.md#1514-async-functions)).

In a method that has multiple by-reference parameters, it is possible for multiple parameter names to represent the same storage location.

##### 15.6.2.3.2 Input parameters

A parameter declared with an `in` modifier is an ***input parameter***. The argument corresponding to an input parameter is either a variable existing at the point of the method invocation, or one created by the implementation ([§12.6.2.3](expressions.md#12623-run-time-evaluation-of-argument-lists)) in the method invocation. For definite-assignment rules, see [§9.2.8](variables.md#928-input-parameters).

It is a compile-time error to modify the value of an input parameter.

> *Note*: The primary purpose of input parameters is for efficiency. When the type of a method parameter is a large struct (in terms of memory requirements), it is useful to be able to avoid copying the whole value of the argument when calling the method. Input parameters allow methods to refer to existing values in memory, while providing protection against unwanted changes to those values. *end note*

##### 15.6.2.3.3 Reference parameters

A parameter declared with a `ref` or `ref readonly` modifier is a ***reference parameter***. For definite-assignment rules, see [§9.2.6](variables.md#926-reference-parameters).

It is a compile-time error to modify the value of a `ref readonly` parameter.

The argument corresponding to a `ref readonly` parameter may be a value, in which case, a variable having that value is created by the implementation ([§12.6.2.3](expressions.md#12623-run-time-evaluation-of-argument-lists)) in the method invocation.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"ReferenceParameters1", inferOutput:true} -->
> ```csharp
> class Test
> {
>     static void Swap(ref int x, ref int y)
>     {
>         int temp = x;
>         x = y;
>         y = temp;
>     }
>
>     static void Main()
>     {
>         int i = 1, j = 2;
>         Swap(ref i, ref j);
>         Console.WriteLine($"i = {i}, j = {j}");
>     }
> }
> ```
>
> produces the output
>
> ```console
> i = 2, j = 1
> ```
>
> For the invocation of `Swap` in `Main`, `x` represents `i` and `y` represents `j`. Thus, the invocation has the effect of swapping the values of `i` and `j`.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ReferenceParameters2"} -->
> ```csharp
> class A
> {
>     string s;
>     void F(ref string a, ref string b)
>     {
>         s = "One";
>         a = "Two";
>         b = "Three";
>     }
>
>     void G()
>     {
>         F(ref s, ref s);
>     }
> }
> ```
>
> the invocation of `F` in `G` passes a reference to `s` for both `a` and `b`. Thus, for that invocation, the names `s`, `a`, and `b` all refer to the same storage location, and the three assignments all modify the instance field `s`.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The following example
>
> <!-- Example: {template:"standalone-console-without-using", name:"ReferenceParameters3"} -->
> ```csharp
> LargeStruct ls /* init somehow */;
> M(ref ls);
> static void M(ref readonly LargeStruct p) { /* ... */ }
> struct LargeStruct { /* ... */ }
> ```
>
> shows a large struct being passed by reference for efficiency, but without the called method having the ability to modify that that struct. *end example*

For a `struct` type, within an instance method, instance accessor ([§12.2.1](expressions.md#1221-general)), or instance constructor with a constructor initializer, the `this` keyword behaves exactly as a reference parameter of the struct type ([§12.8.14](expressions.md#12814-this-access)).

##### 15.6.2.3.4 Output parameters

A parameter declared with an `out` modifier is an ***output parameter***. For definite-assignment rules, see [§9.2.7](variables.md#927-output-parameters).

A method declared as an optional partial method ([§15.6.9.2](classes.md#15692-optional-partial-methods)) shall not have output parameters.

> *Note*: Output parameters are typically used in methods that produce multiple return values. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*:
>
> <!-- Example: {template:"standalone-console", name:"OutputParameters", inferOutput:true} -->
> ```csharp
> class Test
> {
>     static void SplitPath(string path, out string dir, out string name)
>     {
>         int i = path.Length;
>         while (i > 0)
>         {
>             char ch = path[i - 1];
>             if (ch == '\\' || ch == '/' || ch == ':')
>             {
>                 break;
>             }
>             i--;
>         }
>         dir = path.Substring(0, i);
>         name = path.Substring(i);
>     }
>
>     static void Main()
>     {
>         string dir, name;
>         SplitPath(@"c:\Windows\System\hello.txt", out dir, out name);
>         Console.WriteLine(dir);
>         Console.WriteLine(name);
>     }
> }
> ```
>
> The example produces the output:
>
> ```console
> c:\Windows\System\
> hello.txt
> ```
>
> Note that the `dir` and `name` variables can be unassigned before they are passed to `SplitPath`, and that they are considered definitely assigned following the call.
>
> *end example*

#### 15.6.2.4 Parameter collections

A parameter declared with a `params` modifier is a parameter collection.

> *Example*: The types `string[]` and `string[][]` can be used as the type of a parameter collection, but the type `string[,]` cannot. *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: It is not possible to combine the `params` modifier with the modifiers `in`, `out`, or `ref`. *end note*

A parameter collection permits arguments to be specified in one of two ways in a method invocation:

- The argument given for a parameter collection can be a single expression that is implicitly convertible ([§10.2](conversions.md#102-implicit-conversions)) to the parameter collection type. In this case, the parameter collection acts precisely like a value parameter.
- Alternatively, the invocation can specify zero or more arguments for the parameter collection, where each argument is an expression that is implicitly convertible ([§10.2](conversions.md#102-implicit-conversions)) to the element type of the parameter collection. In this case, the invocation creates an instance of the parameter collection type according to the rules specified in [§12.8.25](expressions.md#12825-collection-expressions) as though the arguments were used as expression elements in a collection expression in the same order, and uses the newly created collection instance as the actual argument. When constructing the collection instance, the original unconverted arguments are used.

Except for allowing a variable number of arguments in an invocation, a parameter collection is precisely equivalent to a value parameter ([§15.6.2.2](classes.md#15622-value-parameters)) of the same type.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"ParameterCollections1", inferOutput:true} -->
> ```csharp
> class Test
> {
>     static void F(params int[] args)
>     {
>         Console.Write($"Array contains {args.Length} elements:");
>         foreach (int i in args)
>         {
>             Console.Write($" {i}");
>         }
>         Console.WriteLine();
>     }
>
>     static void Main()
>     {
>         int[] arr = {1, 2, 3};
>         F(arr);
>         F(10, 20, 30, 40);
>         F();
>         F([-5, 3, 9]);
>         F([]);
>     }
> }
> ```
>
> produces the output
>
> ```console
> Array contains 3 elements: 1 2 3
> Array contains 4 elements: 10 20 30 40
> Array contains 0 elements:
> Array contains 3 elements: -5 3 9
> Array contains 0 elements:
> ```
>
> The first invocation of `F` simply passes the array `arr` as a value parameter. The second invocation of F automatically creates a four-element `int[]` with the given element values and passes that array instance as a value parameter. Likewise, the third invocation of `F` creates a zero-element `int[]` and passes that instance as a value parameter. The second and third invocations are precisely equivalent to writing:
>
> ```csharp
> F(new int[] {10, 20, 30, 40});
> F(new int[] {});
> ```
>
> The fourth and fifth invocations pass a three-element and an empty collection expression, respectively. *end example*

When performing overload resolution, a method with a parameter collection might be applicable, either in its normal form or in its expanded form ([§12.6.4.2](expressions.md#12642-applicable-function-member)). The expanded form of a method is available only if the normal form of the method is not applicable and only if an applicable method with the same signature as the expanded form is not already declared in the same type.

A potential ambiguity arises between the normal form and the expanded form of the method with a single parameter collection argument when it can be used as the parameter collection itself and as the element of the parameter collection at the same time. The ambiguity presents no problem, however, since it can be resolved by inserting a cast or using a collection expression, if needed.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"ParameterCollections3", inferOutput:true} -->
> ```csharp
> class Test
> {
>     static void F(params object[] a) =>
>         Console.WriteLine("F(object[])");
>
>     static void F() =>
>         Console.WriteLine("F()");
>
>     static void F(object a0, object a1) =>
>         Console.WriteLine("F(object,object)");
>
>     static void Main()
>     {
>         F();
>         F(1);
>         F(1, 2);
>         F(1, 2, 3);
>         F(1, 2, 3, 4);
>     }
> }
> ```
>
> produces the output
>
> ```console
> F()
> F(object[])
> F(object,object)
> F(object[])
> F(object[])
> ```
>
> In the example, two of the possible expanded forms of the method with a parameter collection are already included in the class as regular methods. These expanded forms are therefore not considered when performing overload resolution, and the first and third method invocations thus select the regular methods. When a class declares a method with a parameter collection, it is not uncommon to also include some of the expanded forms as regular methods. By doing so, it is possible to avoid the allocation of a collection instance that occurs when an expanded form of a method with a parameter collection is invoked.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> An array is a reference type, so the value passed for a parameter collection can be `null`.
>
> *Example*: The example:
>
> <!-- Example: {template:"standalone-console", name:"ParameterCollections4", inferOutput:true} -->
> ```csharp
> class Test
> {
>     static void F(params string[] array) =>
>         Console.WriteLine(array == null);
> 
>     static void Main()
>     {
>         F(null);
>         F((string) null);
>     }
> }
> ```
>
> produces the output:
>
> ```console
> True
> False
> ```
>
> The second invocation produces `False` as it is equivalent to `F(new string[] { null })` and passes an array containing a single null reference.
>
> *end example*

When the type of a parameter collection is `object[]`, a potential ambiguity arises between the normal form of the method and the expanded form for a single `object` parameter. The reason for the ambiguity is that an `object[]` is itself implicitly convertible to type `object`. The ambiguity presents no problem, however, since it can be resolved by inserting a cast if needed.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"ParameterCollections5", inferOutput:true} -->
> ```csharp
> class Test
> {
>     static void F(params object[] args)
>     {
>         foreach (object o in args)
>         {
>             Console.Write(o.GetType().FullName);
>             Console.Write(" ");
>         }
>         Console.WriteLine();
>     }
>
>     static void Main()
>     {
>         object[] a = {1, "Hello", 123.456};
>         object o = a;
>         F(a);
>         F((object)a);
>         F(o);
>         F((object[])o);
>     }
> }
> ```
>
> produces the output
>
> ```console
> System.Int32 System.String System.Double
> System.Object[]
> System.Object[]
> System.Int32 System.String System.Double
> ```
>
> In the first and last invocations of `F`, the normal form of `F` is applicable because an implicit conversion exists from the argument type to the parameter type (both are of type `object[]`). Thus, overload resolution selects the normal form of `F`, and the argument is passed as a regular value parameter. In the second and third invocations, the normal form of `F` is not applicable because no implicit conversion exists from the argument type to the parameter type (type `object` cannot be implicitly converted to type `object[]`). However, the expanded form of `F` is applicable, so it is selected by overload resolution. As a result, a one-element `object[]` is created by the invocation, and the single element of the array is initialized with the given argument value (which itself is a reference to an `object[]`).
>
> *end example*

### 15.6.3 Static and instance methods

When a method declaration includes a `static` modifier, that method is said to be a static method. When no `static` modifier is present, the method is said to be an instance method.

A static method does not operate on a specific instance, and it is a compile-time error to refer to `this` in a static method.

An instance method operates on a given instance of a class, and that instance can be accessed as `this` ([§12.8.14](expressions.md#12814-this-access)).

The differences between static and instance members are discussed further in [§15.3.8](classes.md#1538-static-and-instance-members).

### 15.6.4 Virtual methods

When an instance method declaration includes a virtual modifier, that method is said to be a ***virtual method***. When no virtual modifier is present, the method is said to be a ***non-virtual method***.

The implementation of a non-virtual method is invariant: The implementation is the same whether the method is invoked on an instance of the class in which it is declared or an instance of a derived class. In contrast, the implementation of a virtual method can be superseded by derived classes. The process of superseding the implementation of an inherited virtual method is known as ***overriding*** that method ([§15.6.5](classes.md#1565-override-methods)).

In a virtual method invocation, the ***run-time type*** of the instance for which that invocation takes place determines the actual method implementation to invoke. In a non-virtual method invocation, the ***compile-time type*** of the instance is the determining factor. In precise terms, when a method named `N` is invoked with an argument list `A` on an instance with a compile-time type `C` and a run-time type `R` (where `R` is either `C` or a class derived from `C`), the invocation is processed as follows:

- At binding-time, overload resolution is applied to `C`, `N`, and `A`, to select a specific method `M` from the set of methods declared in and inherited by `C`. This is described in [§12.8.10.2](expressions.md#128102-method-invocations).
- Then at run-time:
  - If `M` is a non-virtual method, `M` is invoked.
  - Otherwise, `M` is a virtual method, and the most derived implementation of `M` with respect to `R` is invoked.

For every virtual method declared in or inherited by a class, there exists a ***most derived implementation*** of the method with respect to that class. The most derived implementation of a virtual method `M` with respect to a class `R` is determined as follows:

- If `R` contains the introducing virtual declaration of `M`, then this is the most derived implementation of `M` with respect to `R`.
- Otherwise, if `R` contains an override of `M`, then this is the most derived implementation of `M` with respect to `R`.
- Otherwise, the most derived implementation of `M` with respect to `R` is the same as the most derived implementation of `M` with respect to the direct base class of `R`.

> *Example*: The following example illustrates the differences between virtual and non-virtual methods:
>
> <!-- Example: {template:"standalone-console", name:"VirtualMethods1", inferOutput:true} -->
> ```csharp
> class A
> {
>     public void F() => Console.WriteLine("A.F");
>     public virtual void G() => Console.WriteLine("A.G");
> }
>
> class B : A
> {
>     public new void F() => Console.WriteLine("B.F");
>     public override void G() => Console.WriteLine("B.G");
> }
>
> class Test
> {
>     static void Main()
>     {
>         B b = new B();
>         A a = b;
>         a.F();
>         b.F();
>         a.G();
>         b.G();
>     }
> }
> ```
>
> In the example, `A` introduces a non-virtual method `F` and a virtual method `G`. The class `B` introduces a *new* non-virtual method `F`, thus *hiding* the inherited `F`, and also *overrides* the inherited method `G`. The example produces the output:
>
> ```console
> A.F
> B.F
> B.G
> B.G
> ```
>
> Notice that the statement `a.G()` invokes `B.G`, not `A.G`. This is because the run-time type of the instance (which is `B`), not the compile-time type of the instance (which is `A`), determines the actual method implementation to invoke.
>
> *end example*

Because methods are allowed to hide inherited methods, it is possible for a class to contain several virtual methods with the same signature. This does not present an ambiguity problem, since all but the most derived method are hidden.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-console", name:"VirtualMethods2", inferOutput:true} -->
> ```csharp
> class A
> {
>     public virtual void F() => Console.WriteLine("A.F");
> }
>
> class B : A
> {
>     public override void F() => Console.WriteLine("B.F");
> }
>
> class C : B
> {
>     public new virtual void F() => Console.WriteLine("C.F");
> }
>
> class D : C
> {
>     public override void F() => Console.WriteLine("D.F");
> }
>
> class Test
> {
>     static void Main()
>     {
>         D d = new D();
>         A a = d;
>         B b = d;
>         C c = d;
>         a.F();
>         b.F();
>         c.F();
>         d.F();
>     }
> }
> ```
>
> the `C` and `D` classes contain two virtual methods with the same signature: The one introduced by `A` and the one introduced by `C`. The method introduced by `C` hides the method inherited from `A`. Thus, the override declaration in `D` overrides the method introduced by `C`, and it is not possible for `D` to override the method introduced by `A`. The example produces the output:
>
> ```console
> B.F
> B.F
> D.F
> D.F
> ```
>
> Note that it is possible to invoke the hidden virtual method by accessing an instance of `D` through a less derived type in which the method is not hidden.
>
> *end example*

### 15.6.5 Override methods

When an instance method declaration includes an `override` modifier, the method is said to be an ***override method***. An override method overrides an inherited virtual method with the same signature. Whereas a virtual method declaration *introduces* a new method, an override method declaration *specializes* an existing inherited virtual method by providing a new implementation of that method.

The method overridden by an override declaration is known as the ***overridden base method*** For an override method `M` declared in a class `C`, the overridden base method is determined by examining each base class of `C`, starting with the direct base class of `C` and continuing with each successive direct base class, until in a given base class type at least one accessible method is located which has the same signature as `M` after substitution of type arguments. For the purposes of locating the overridden base method, a method is considered accessible if it is `public`, if it is `protected`, if it is `protected internal`, or if it is either `internal` or `private protected`  and declared in the same program as `C`.

The overriding method inherits any *type_parameter_constraints_clause*s of the overridden base method.

A compile-time error occurs unless all of the following are true for an override declaration:

- An overridden base method can be located as described above.
- There is exactly one such overridden base method. This restriction has effect only if the base class type is a constructed type where the substitution of type arguments makes the signature of two methods the same.
- The overridden base method is a virtual, abstract, or override method. In other words, the overridden base method cannot be static or non-virtual.
- The overridden base method is not a sealed method.
- The override method has a return type that is convertible by an identity conversion or (if the method has a value return) an implicit reference conversion to the return type of the overridden base method.
- The override method has a return type that is convertible by an identity conversion or (if the method has a value return) an implicit reference conversion to the return type of every override of the overridden base method that is declared in a (direct or indirect) base type of the override method.
- The override declaration and the overridden base method have the same declared accessibility.
- The override method’s return type is at least as accessible as the override method.
  > *Note*: This constraint permits an override method in a `private` class to have a `private` return type.  However, it requires a `public` override method in a `public` type to have a `public` return type. *end note* In other words, an override declaration cannot change the accessibility of the virtual method. However, if the overridden base method is protected internal and it is declared in a different assembly than the assembly containing the override declaration then the override declaration’s declared accessibility shall be protected.
- A *type_parameter_constraints_clause* may only consist of the `class`, `struct`, or `default` *primary_constraint*s. The `class` and `struct` constraints are applied to *type_parameter*s which are known according to the inherited constraints to be either reference or value types respectively. The `default` constraint is applied to *type_parameter*s that are not constrained to either reference or value types. Any type of the form `T?` in the overriding method’s signature, where `T` is a type parameter, is interpreted as follows:
  - If a `class` constraint is added for type parameter `T` then `T?` is a nullable reference type.
  - If either a `struct` constraint is added, or no constraint is added and the inherited constraint is a value type constraint, for the type parameter `T` then `T?` is a nullable value type.
  - If a `default` constraint is added for type parameter `T` then `T?` represents a nullable instance of the corresponding reference type when `T` is a reference type, and an instance of `T` when `T` is a value type. If `T` is substituted with an annotated type `U?`, then `T?` represents `U?`, not `U??`.

> *Example*: The following demonstrates how the overriding rules work for generic classes:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"OverrideMethods1", replaceEllipsis:true, customEllipsisReplacements:["return default;","return default;",null,"return default;","return default;",null,"return default;","return default;",null], expectedErrors:["CS0246","CS0115"]} -->
> ```csharp
> abstract class C<T>
> {
>     public virtual T F() {...}
>     public virtual C<T> G() {...}
>     public virtual void H(C<T> x) {...}
> }
>
> class D : C<string>
> {
>     public override string F() {...}            // Ok
>     public override C<string> G() {...}         // Ok
>     public override void H(C<T> x) {...}        // Error, should be C<string>
> }
>
> class E<T,U> : C<U>
> {
>     public override U F() {...}                 // Ok
>     public override C<U> G() {...}              // Ok
>     public override void H(C<T> x) {...}        // Error, should be C<U>
> }
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The following demonstrates how the overriding rules work when type parameters are involved:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"OverrideMethods5"} -->
> ```csharp
> #nullable enable
> class A
> {
>     public virtual void Foo<T>(T? value) where T : class { }
>     public virtual void Foo<T>(T? value) where T : struct { }
> }
> class B: A
> {
>     public override void Foo<T>(T? value) where T : class { }
>     public override void Foo<T>(T? value) where T : struct { }
> }
> ```
>
> Without the type parameter constraint `where T : class`, the base method with the reference-typed type parameter cannot be overridden. *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The following demonstrates how the `default` constraint works when overriding a method with an unconstrained type parameter:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"OverrideMethods6"} -->
> ```csharp
> #nullable enable
> class A2
> {
>     public virtual void F2<T>(T? t) where T : struct { }
>     public virtual void F2<T>(T? t) { }
> }
>
> class B2 : A2
> {
>     public override void F2<T>(T? t) /*where T : struct*/ { }
>     public override void F2<T>(T? t) where T : default { }
> }
> ```
>
> The `default` constraint on `B2.F2` is required to override the unconstrained `A2.F2` with a `T?` parameter. Without it, `T?` would be interpreted as a nullable value type. *end example*

An override declaration can access the overridden base method using a *base_access* ([§12.8.15](expressions.md#12815-base-access)).

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib", name:"OverrideMethods2", ignoredWarnings:["CS0649"]} -->
> ```csharp
> class A
> {
>     int x;
>
>     public virtual void PrintFields() => Console.WriteLine($"x = {x}");
> }
>
> class B : A
> {
>     int y;
>
>     public override void PrintFields()
>     {
>         base.PrintFields();
>         Console.WriteLine($"y = {y}");
>     }
> }
> ```
>
> the `base.PrintFields()` invocation in `B` invokes the PrintFields method declared in `A`. A *base_access* disables the virtual invocation mechanism and simply treats the base method as a non-`virtual` method. Had the invocation in `B` been written `((A)this).PrintFields()`, it would recursively invoke the `PrintFields` method declared in `B`, not the one declared in `A`, since `PrintFields` is virtual and the run-time type of `((A)this)` is `B`.
>
> *end example*

Only by including an `override` modifier can a method override another method. In all other cases, a method with the same signature as an inherited method simply hides the inherited method.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"OverrideMethods3", expectedWarnings:["CS0114"]} -->
> ```csharp
> class A
> {
>     public virtual void F() {}
> }
>
> class B : A
> {
>     public virtual void F() {} // Warning, hiding inherited F()
> }
> ```
>
> the `F` method in `B` does not include an `override` modifier and therefore does not override the `F` method in `A`. Rather, the `F` method in `B` hides the method in `A`, and a warning is reported because the declaration does not include a new modifier.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"OverrideMethods4"} -->
> ```csharp
> class A
> {
>     public virtual void F() {}
> }
>
> class B : A
> {
>     private new void F() {} // Hides A.F within body of B
> }
> 
> class C : B
> {
>     public override void F() {} // Ok, overrides A.F
> }
> ```
>
> the `F` method in `B` hides the virtual `F` method inherited from `A`. Since the new `F` in `B` has private access, its scope only includes the class body of `B` and does not extend to `C`. Therefore, the declaration of `F` in `C` is permitted to override the `F` inherited from `A`.
>
> *end example*

### 15.6.6 Sealed methods

When an instance method declaration includes a `sealed` modifier, that method is said to be a ***sealed method***. A sealed method overrides an inherited virtual method with the same signature. A sealed method shall also be marked with the `override` modifier. Use of the `sealed` modifier prevents a derived class from further overriding the method.

> *Example*: The example
>
> <!-- Example: {template:"standalone-lib", name:"SealedMethods"} -->
> ```csharp
> class A
> {
>     public virtual void F() => Console.WriteLine("A.F");
>     public virtual void G() => Console.WriteLine("A.G");
> }
>
> class B : A
> {
>     public sealed override void F() => Console.WriteLine("B.F");
>     public override void G()        => Console.WriteLine("B.G");
> }
>
> class C : B
> {
>     public override void G() => Console.WriteLine("C.G");
> }
> ```
>
> the class `B` provides two override methods: an `F` method that has the `sealed` modifier and a `G` method that does not. `B`’s use of the `sealed` modifier prevents `C` from further overriding `F`.
>
> *end example*

### 15.6.7 Abstract methods

When an instance method declaration includes an `abstract` modifier, that method is said to be an ***abstract method***. Although an abstract method is implicitly also a virtual method, it cannot have the modifier `virtual`.

An abstract method declaration introduces a new virtual method but does not provide an implementation of that method. Instead, non-abstract derived classes are required to provide their own implementation by overriding that method. The *method_body* of an abstract method simply consists of a semicolon.

Abstract method declarations are only permitted in abstract classes ([§15.2.2.2](classes.md#15222-abstract-classes)) and interfaces ([§19.4.3](interfaces.md#1943-interface-methods)).

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AbstractMethods1", additionalFiles:["Graphics.cs","RectangleNoPoint.cs"]} -->
> ```csharp
> public abstract class Shape
> {
>     public abstract void Paint(Graphics g, Rectangle r);
> }
>
> public class Ellipse : Shape
> {
>     public override void Paint(Graphics g, Rectangle r) => g.DrawEllipse(r);
> }
>
> public class Box : Shape
> {
>     public override void Paint(Graphics g, Rectangle r) => g.DrawRect(r);
> }
> ```
>
> the `Shape` class defines the abstract notion of a geometrical shape object that can paint itself. The `Paint` method is abstract because there is no meaningful fallback implementation for the abstract concept of shape. The `Ellipse` and `Box` classes are concrete `Shape` implementations. Because these classes are non-abstract, they are required to override the `Paint` method and provide an actual implementation.
>
> *end example*

It is a compile-time error for a *base_access* ([§12.8.15](expressions.md#12815-base-access)) to reference an abstract method.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AbstractMethods2", expectedErrors:["CS0205"]} -->
> ```csharp
> abstract class A
> {
>     public abstract void F();
> }
>
> class B : A
> {
>     // Error, base.F is abstract
>     public override void F() => base.F();
> }
> ```
>
> a compile-time error is reported for the `base.F()` invocation because it references an abstract method.
>
> *end example*

An abstract method declaration is permitted to override a virtual method. This allows an abstract class to force re-implementation of the method in derived classes, and makes the original implementation of the method unavailable.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib", name:"AbstractMethods3"} -->
> ```csharp
> class A
> {
>     public virtual void F() => Console.WriteLine("A.F");
> }
>
> abstract class B: A
> {
>     public abstract override void F();
> }
>
> class C : B
> {
>     public override void F() => Console.WriteLine("C.F");
> }
> ```
>
> class `A` declares a virtual method, class `B` overrides this method with an abstract method, and class `C` overrides the abstract method to provide its own implementation.
>
> *end example*

### 15.6.8 External methods

When a method declaration includes an `extern` modifier, the method is said to be an ***external method***. External methods are implemented externally, typically using a language other than C#. Because an external method declaration provides no actual implementation, the method body of an external method simply consists of a semicolon. An external method shall not be generic.

The mechanism by which linkage to an external method is achieved is implementation-defined.

> *Example*: The following example demonstrates the use of the `extern` modifier and the `DllImport` attribute:
>
> <!-- Example: {template:"standalone-lib", name:"ExternalMethods", ignoredWarnings:["SYSLIB0003"]} -->
> ```csharp
> class Path
> {
>     [DllImport("kernel32", SetLastError=true)]
>     static extern bool CreateDirectory(string name, SecurityAttribute sa);
>
>     [DllImport("kernel32", SetLastError=true)]
>     static extern bool RemoveDirectory(string name);
>
>     [DllImport("kernel32", SetLastError=true)]
>     static extern int GetCurrentDirectory(int bufSize, StringBuilder buf);
>
>     [DllImport("kernel32", SetLastError=true)]
>     static extern bool SetCurrentDirectory(string name);
> }
> ```
>
> *end example*

### 15.6.9 Partial methods

#### 15.6.9.1 General

When a *method declaration* includes a `partial` modifier, that method is said to be a ***partial method***. Partial methods may only be declared as members of partial types ([§15.2.7](classes.md#1527-partial-type-declarations)). Partial methods may be defined in one part of a type declaration and implemented in another.

In *method_declaration*, the identifier `partial` is recognized as a contextual keyword ([§6.4.4](lexical-structure.md#644-keywords)) only if it immediately precedes the *return_type*. A partial method cannot explicitly implement interface methods.

Partial method declarations are classified as follows:

- A method with an *expression-body* or a *block-body* or is declared with the `extern` modifier is said to be an ***implementing partial method declaration***.
- Otherwise, a method declaration where the body of the method declaration is a semicolon is said to be a ***defining partial method declaration***.

Across the parts of a type declaration, there shall be exactly one defining partial method declaration with a given signature. If an implementing partial method declaration is given, a corresponding defining partial method declaration shall exist, and the declarations shall match as specified in the following:

- The declarations shall have the same method name, number of type parameters, and number of parameters.
- The declarations shall have the same modifiers except for the `async` and `extern` modifiers. The `async` and `extern` modifiers are allowed only on the implementing partial method declaration.
- Corresponding parameters in the declarations shall have the same modifiers (although not necessarily in the same order) and the same types (modulo differences in type parameter names). Tuple types ([§8.3.11](types.md#8311-tuple-types)) used as parameters or return types shall have the same item names in both the defining and implementing partial method declarations.
- Corresponding type parameters in the declarations shall have the same constraints. An implementation may choose to issue a warning if the type parameter names are different in the defining and implementing declarations.

There are two variations of partial methods: required and optional. A ***required partial method*** ([§15.6.9.3](classes.md#15693-required-partial-methods)) is a partial method that includes one or more explicit access modifiers. An ***optional partial method*** ([§15.6.9.2](classes.md#15692-optional-partial-methods)) has no explicit access modifiers, and is implicitly private.

For a required partial method both the definition and implementation shall exist.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"PartialMethods2", replaceEllipsis:true, customEllipsisReplacements: ["", "return true;", "i = 10;"]} -->
> ```csharp
> // part containing defining partial method declarations
> partial class C
> {
>     partial void M1();                  // implementation optional
>     private partial void M2();          // required, impl. required
>     protected partial bool M3();        // required, impl. required
>     public partial void M4(out int i);  // required, impl. required
> }
> 
> // part containing implementing partial method declarations
> partial class C
> {
>     private partial void M2() { ... }
>     protected partial bool M3() { ... }
>     public partial void M4(out int i) { ... }
> }
> ```
>
> *end example*

Only a defining partial method participates in overload resolution.

> *Note*: The definition of matching defining and implementing partial method declarations does not require parameter names to match. This can produce *surprising*, albeit *well defined*, behaviour when named arguments ([§12.6.2.1](expressions.md#12621-general)) are used. For example, given the defining partial method declaration for `M` in one file, and the implementing partial method declaration in another file:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"PartialMethods1", "expectedErrors":["CS1739"], "expectedWarnings":["CS8826"]} -->
> ```csharp
> // File P1.cs:
> partial class P
> {
>     static partial void M(int x);
> }
>
> // File P2.cs:
> partial class P
> {
>     static void Caller() => M(y: 0);
>     static partial void M(int y) {}
> }
> ```
>
> is **invalid** as the invocation uses the argument name from the implementing and not the defining partial method declaration.
>
> *end note*

If an implementing declaration exists for a given partial method, the invocations of the partial methods are retained. The partial method gives rise to a method declaration similar to the implementing partial method declaration except for the following:

- The `partial` modifier is not included in the combined method declaration.
- The attributes in the resulting method declaration are the combined attributes of the defining and the implementing partial method declaration in unspecified order. Duplicates are not removed.
- The attributes on the parameters of the resulting method declaration are the combined attributes of the corresponding parameters of the defining and the implementing partial method declaration in unspecified order. Duplicates are not removed.
- Any default arguments ([§15.6.2](classes.md#1562-method-parameters)) in the implementing declaration are removed.

Partial methods are useful for allowing one part of a type declaration to customize the behavior of another part, e.g., one that is generated by a tool. Consider the following partial class declaration:

<!-- Example: {template:"standalone-lib-without-using", name:"PartialMethods3"} -->
<!-- Maintenance Note: This code exists in additional-files as "Customer.cs" to be used in an example below. As such, certain changes to this example should be reflected in that file, in which case, *all* examples using that file should be tested. -->
```csharp
partial class Customer
{
    string name;

    public string Name
    {
        get => name;
        set
        {
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

<!-- Example: {template:"standalone-lib-without-using", name:"PartialMethods4"} -->
```csharp
class Customer
{
    string name;

    public string Name
    {
        get => name;
        set => name = value;
    }
}
```

Assume that another part is given, however, which provides implementing declarations of the partial methods:

<!-- Example: {template:"standalone-lib", name:"PartialMethods5", additionalFiles:["Customer.cs"]} -->
```csharp
partial class Customer
{
    partial void OnNameChanging(string newName) =>
        Console.WriteLine($"Changing {name} to {newName}");

    partial void OnNameChanged() =>
        Console.WriteLine($"Changed to {name}");
}
```

Then the resulting combined class declaration will be equivalent to the following:

<!-- Example: {template:"standalone-lib", name:"PartialMethods6"} -->
```csharp
class Customer
{
    string name;

    public string Name
    {
        get => name;
        set
        {
            OnNameChanging(value);
            name = value;
            OnNameChanged();
        }
    }

    void OnNameChanging(string newName) =>
        Console.WriteLine($"Changing {name} to {newName}");

    void OnNameChanged() =>
        Console.WriteLine($"Changed to {name}");
}
```

#### 15.6.9.2 Optional partial methods

An optional partial method shall have a `void` return type, and shall not declare out parameters. There shall be zero or one implementing declaration for each defining declaration. If no part implements the partial method, the partial method declaration and all calls to it are removed from the type declaration resulting from the combination of the parts. Whether or not an implementing declaration is given, invocation expressions may resolve to invocations of the partial method.

The implementing member for an optional partial method shall not be an external method ([§15.6.8](classes.md#1568-external-methods)).

> *Note*: Because an optional partial method always returns `void`, such invocation expressions will always be expression statements. Furthermore, because an optional partial method is implicitly `private`, such statements will always occur within one of the parts of the type declaration within which the partial method is declared. *end note*

If an optional partial method has no implementation, any expression statement invoking it is removed from the combined type declaration. Thus, the invocation expression, including any subexpressions, has no effect at run-time. The partial method itself is also removed and will not be a member of the combined type declaration.

If a defining declaration but not an implementing declaration is given for an optional partial method `M`, the following restrictions apply:

- It is a compile-time error to create a delegate from `M` ([§12.8.17.6](expressions.md#128176-delegate-creation-expressions)).
- It is a compile-time error to refer to `M` inside an anonymous function that is converted to an expression tree type ([§8.6](types.md#86-expression-tree-types)).
- Expressions occurring as part of an invocation of `M` do not affect the definite assignment state ([§9.4](variables.md#94-definite-assignment)), which can potentially lead to compile-time errors.
- `M` cannot be the entry point for an application ([§7.1](basic-concepts.md#71-application-startup)).

#### 15.6.9.3 Required partial methods

A required partial method declaration includes an explicit access modifier. There shall be exactly one implementing partial method declaration.

The implementing declaration for a required partial method may be an external method ([§15.6.8](classes.md#1568-external-methods)). The `extern` modifier is allowed on an implementing partial declaration. It shall not be present on a defining partial declaration.

> *Note:* The `private` access modifier is required on both the ***defining partial method declaration*** and the ***implementing partial method declaration*** of a private required partial method. *end note*

### 15.6.10 Extension methods

When the first parameter of a method includes the `this` modifier, that method is said to be an ***extension method***. Extension methods shall only be declared in non-generic, non-nested static classes. The first parameter of an extension method is restricted, as follows:

- It may only be an input parameter if it has a value type.
- It may only be a reference parameter if it has a value type or has a generic type constrained to struct.
- It shall not be an output parameter.
- It shall not be a pointer type.

> *Example*: The following is an example of a static class that declares two extension methods:
>
> <!-- Example: {template:"standalone-lib", name:"ExtensionMethods1"} -->
> <!-- Maintenance Note: This code exists in additional-files as "Extensions.cs" to be used in the examples below. As such, certain changes to this example should be reflected in that file, in which case, *all* examples using that file should be tested. -->
> ```csharp
> public static class Extensions
> {
>     public static int ToInt32(this string s) => Int32.Parse(s);
>
>     public static T[] Slice<T>(this T[] source, int index, int count)
>     {
>         if (index < 0 || count < 0 || source.Length - index < count)
>         {
>             throw new ArgumentException();
>         }
>         T[] result = new T[count];
>         Array.Copy(source, index, result, 0, count);
>         return result;
>     }
> }
> ```
>
> *end example*

An extension method is a regular static method. In addition, where its enclosing static class is in scope, an extension method may be invoked using instance method invocation syntax ([§12.8.10.3](expressions.md#128103-extension-method-invocations)), using the receiver expression as the first argument.

> *Example*: The following program uses the extension methods declared above:
>
> <!-- Example: {template:"standalone-console", name:"ExtensionMethods2", additionalFiles:["Extensions.cs"], expectedOutput:["22", "333"]} -->
> ```csharp
> static class Program
> {
>     static void Main()
>     {
>         string[] strings = { "1", "22", "333", "4444" };
>         foreach (string s in strings.Slice(1, 2))
>         {
>             Console.WriteLine(s.ToInt32());
>         }
>     }
> }
> ```
>
> The `Slice` method is available on the `string[]`, and the `ToInt32` method is available on `string`, because they have been declared as extension methods. The meaning of the program is the same as the following, using ordinary static method calls:
>
> <!-- Example: {template:"standalone-console", name:"ExtensionMethods3", additionalFiles:["Extensions.cs"], expectedOutput:["22", "333"]} -->
> ```csharp
> static class Program
> {
>     static void Main()
>     {
>         string[] strings = { "1", "22", "333", "4444" };
>         foreach (string s in Extensions.Slice(strings, 1, 2))
>         {
>             Console.WriteLine(Extensions.ToInt32(s));
>         }
>     }
> }
> ```
>
> *end example*

### 15.6.11 Method body

The method body of a method declaration consists of either a block body, an expression body or a semicolon.

Abstract and external method declarations do not provide a method implementation, so their method bodies simply consist of a semicolon. For any other method, the method body is a block ([§13.3](statements.md#133-blocks)) that contains the statements to execute when that method is invoked.

The ***effective return type*** of a method is `void` if the return type is `void`, or if the method is async and the return type is `«TaskType»` ([§15.14.1](classes.md#15141-general)). Otherwise, the effective return type of a non-async method is its return type, and the effective return type of an async method with return type `«TaskType»<T>`([§15.14.1](classes.md#15141-general)) is `T`.

When the effective return type of a method is `void` and the method has a block body, `return` statements ([§13.10.5](statements.md#13105-the-return-statement)) in the block shall not specify an expression. If execution of the block of a void method completes normally (that is, control flows off the end of the method body), that method simply returns to its caller.

When the effective return type of a method is `void` and the method has an expression body, the expression `E` shall be a *statement_expression*, and the body is exactly equivalent to a block body of the form `{ E; }`.

For a returns-by-value method ([§15.6.1](classes.md#1561-general)), each return statement in that method’s body shall specify an expression that is implicitly convertible to the effective return type.

For a returns-by-ref method ([§15.6.1](classes.md#1561-general)), each return statement in that method’s body shall specify an expression whose type is that of the effective return type, and has a *ref-safe-context* of *caller-context* ([§9.7.2](variables.md#972-ref-safe-contexts)).

For returns-by-value and returns-by-ref methods the endpoint of the method body shall not be reachable. In other words, control is not permitted to flow off the end of the method body.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"MethodBody", expectedErrors:["CS0161"]} -->
> ```csharp
> class A
> {
>     public int F() {} // Error, return value required
>
>     public int G()
>     {
>         return 1;
>     }
>
>     public int H(bool b)
>     {
>         if (b)
>         {
>             return 1;
>         }
>         else
>         {
>             return 0;
>         }
>     }
>
>     public int I(bool b) => b ? 1 : 0;
> }
> ```
>
> the value-returning `F` method results in a compile-time error because control can flow off the end of the method body. The `G` and `H` methods are correct because all possible execution paths end in a return statement that specifies a return value. The `I` method is correct, because its body is equivalent to a block with just a single return statement in it.
>
> *end example*

## 15.7 Properties

### 15.7.1 General

A ***property*** is a member that provides access to a characteristic of an object or a class. Examples of properties include the length of a string, the size of a font, the caption of a window, and the name of a customer. Properties are a natural extension of fields—both are named members with associated types, and the syntax for accessing fields and properties is the same. However, unlike fields, properties do not denote storage locations. Instead, properties have ***accessor***s that specify the statements to be executed when their values are read or written. Properties thus provide a mechanism for associating actions with the reading and writing of an object or class’s characteristics; furthermore, they permit such characteristics to be computed.

Properties are declared using *property_declaration*s:

```ANTLR
property_declaration
    : attributes? property_modifier* type member_name property_body
    | attributes? property_modifier* ref_kind type member_name ref_property_body
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
    | 'readonly'        // struct members only
    | 'required'
    | unsafe_modifier   // unsafe code support
    ;
    
property_body
    : '{' accessor_declarations '}' property_initializer?
    | '=>' expression ';'
    ;

property_initializer
    : '=' variable_initializer ';'
    ;

ref_property_body
    : '{' ref_get_accessor_declaration '}'
    | '=>' 'ref' variable_reference ';'
    ;
```

*unsafe_modifier* ([§24.2](unsafe-code.md#242-unsafe-contexts)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).

A *property_declaration* may include a set of *attributes* ([§23](attributes.md#23-attributes)) and any one of the permitted kinds of declared accessibility ([§15.3.6](classes.md#1536-access-modifiers)), the `new` ([§15.3.5](classes.md#1535-the-new-modifier)), `static` ([§15.7.2](classes.md#1572-static-and-instance-properties)), `virtual` ([§15.6.4](classes.md#1564-virtual-methods), [§15.7.6](classes.md#1576-virtual-sealed-override-and-abstract-accessors)), `override` ([§15.6.5](classes.md#1565-override-methods), [§15.7.6](classes.md#1576-virtual-sealed-override-and-abstract-accessors)), `sealed` ([§15.6.6](classes.md#1566-sealed-methods)), `abstract` ([§15.6.7](classes.md#1567-abstract-methods), [§15.7.6](classes.md#1576-virtual-sealed-override-and-abstract-accessors)) and `extern` ([§15.6.8](classes.md#1568-external-methods)). Additionally a *property_declaration* that is contained directly by a *struct_declaration* may include the `readonly` modifier ([§16.6.11](structs.md#16611-properties)).

- The first declares a non-ref-valued property. Its value has type *type*. This kind of property may be readable and/or writeable.
- The second declares a ref-valued property. Its value is a *variable_reference* ([§9.5](variables.md#95-variable-references)), that may be `readonly`, to a variable of type *type*. This kind of property is only readable.

A *property_declaration* may include a set of *attributes* ([§23](attributes.md#23-attributes)) and any one of the permitted kinds of declared accessibility ([§15.3.6](classes.md#1536-access-modifiers)), the `new` ([§15.3.5](classes.md#1535-the-new-modifier)), `static` ([§15.7.2](classes.md#1572-static-and-instance-properties)), `virtual` ([§15.6.4](classes.md#1564-virtual-methods), [§15.7.6](classes.md#1576-virtual-sealed-override-and-abstract-accessors)), `override` ([§15.6.5](classes.md#1565-override-methods), [§15.7.6](classes.md#1576-virtual-sealed-override-and-abstract-accessors)), `sealed` ([§15.6.6](classes.md#1566-sealed-methods)), `abstract` ([§15.6.7](classes.md#1567-abstract-methods), [§15.7.6](classes.md#1576-virtual-sealed-override-and-abstract-accessors)), and `extern` ([§15.6.8](classes.md#1568-external-methods)) modifiers.

Property declarations are subject to the same rules as method declarations ([§15.6](classes.md#156-methods)) with regard to valid combinations of modifiers.

The *member_name* ([§15.6.1](classes.md#1561-general)) specifies the name of the property. Unless the property is an explicit interface member implementation, the *member_name* is simply an *identifier*. For an explicit interface member implementation ([§19.6.2](interfaces.md#1962-explicit-interface-member-implementations)), the *member_name* consists of an *interface_type* followed by a “`.`” and an *identifier*.

The *type* of a property shall be at least as accessible as the property itself ([§7.5.5](basic-concepts.md#755-accessibility-constraints)).

A *property_body* may either consist of a statement body or an expression body. In a statement body,  *accessor_declarations*, which shall be enclosed in “`{`” and “`}`” tokens, declare the accessors ([§15.7.3](classes.md#1573-accessors)) of the property. The accessors specify the executable statements associated with reading and writing the property.

In a *property_body* an expression body consisting of `=>` followed by an *expression* `E` and a semicolon is exactly equivalent to the statement body `{ get { return E; } }`, and can therefore only be used to specify read-only properties where the result of the get accessor is given by a single expression.

A *property_initializer* may only be given for an automatically implemented property ([§15.7.4](classes.md#1574-automatically-implemented-properties)), and causes the initialization of the underlying field of such properties with the value given by the *expression*.

A *ref_property_body* may either consist of a statement body or an expression body. In a statement body a *get_accessor_declaration* declares the get accessor ([§15.7.3](classes.md#1573-accessors)) of the property. The accessor specifies the executable statements associated with reading the property.

In a *ref_property_body* an expression body consisting of `=>` followed by `ref`, a *variable_reference* `V` and a semicolon is exactly equivalent to the statement body `{ get { return ref V; } }`.

> *Note*: Even though the syntax for accessing a property is the same as that for a field, a property is not classified as a variable. Thus, it is not possible to pass a property as an `in`, `out`, or `ref` argument unless the property is ref-valued and therefore returns a variable reference ([§9.7](variables.md#97-reference-variables-and-returns)). *end note*

When a property declaration includes an `extern` modifier, the property is said to be an ***external property***. Because an external property declaration provides no actual implementation, each of the *accessor_body*s in its *accessor_declarations* shall be a semicolon.

The modifier `required` indicates the instance member being declared is required to be set during object initialization, which forces the instance creator to provide an initial value for the member in an object initializer at the creation site. (See  [§12.8.21](expressions.md#12821-default-value-expressions) and [§23.5.11.1](attributes.md#235111-the-setsrequiredmembers-attribute) for exemptions to this requirement.) A required member shall not be static. A required member shall be at least as accessible as its containing type.

> *Note*: Although a required member declaration may include an initializer (*property_initializer* for a property, *variable_initializer* for a field), ordinarily, that initializer serves no purpose, as the instance creator is required to provide an initial value for that member anyway. However, if a constructor is decorated with the `SetsRequiredMembers` attribute the compiler assumes that member has been initialized correctly, and will not require an explicit initializer by the instance creator, resulting in the member’s initial value being that of its initializer, if one is present. *end note*

A ***required member list*** is a list of all the members of a type that are required. A type automatically inherits this list from its base type.
To build the required member list `R` for a type `T`, the following steps are used:

1. For every type `Tb`, starting with `T` and working through the base type chain until `object` is reached.
1. If `Tb` is decorated with the `RequiredMember` attribute ([§23.5.11.2](attributes.md#235112-the-requiredmember-attribute)), then all members of `Tb` marked with that attribute are gathered into `Rb`

    1. For every `Ri` in `Rb`, if `Ri` is overridden by any member of `R`, it is skipped.
    1. Otherwise, if any `Ri` is hidden by a member of `R`, then the lookup of required members fails, and no further steps are taken. Calling any constructor of `T` not decorated with the `SetsRequiredMembers` is an error.
    1. Otherwise, `Ri` is added to `R`.

A required member shall be treated as if it were decorated with the attribute `System.Runtime.CompilerServices.RequiredMemberAttribute` ([§23.5.11.2](attributes.md#235112-the-requiredmember-attribute)).
With regard to nullable reference type analysis ([§8.9](types.md#89-reference-types-and-nullability)), a required member need not be initialized to a valid nullable state when any of its instance constructors returns. Any required member in a type and its base types is considered by nullable analysis to have its default value at the beginning of any instance constructor in that type, unless it chains to a `this` or `base` constructor that is decorated with the `SetsRequiredMembersAttribute` attribute.

Nullable analysis shall warn about all required members from the current and base types that do not have a valid nullable state at the end of a constructor decorated with the `SetsRequiredMembersAttribute` attribute.

### 15.7.2 Static and instance properties

When a property declaration includes a `static` modifier, the property is said to be a ***static property***. When no `static` modifier is present, the property is said to be an ***instance property***.

A static property is not associated with a specific instance, and it is a compile-time error to refer to `this` in the accessors of a static property.

An instance property is associated with a given instance of a class, and that instance can be accessed as `this` ([§12.8.14](expressions.md#12814-this-access)) in the accessors of that property.

The differences between static and instance members are discussed further in [§15.3.8](classes.md#1538-static-and-instance-members).

### 15.7.3 Accessors

#### 15.7.3.1 General

*Note*: This subclause and its sibling subclauses apply to both properties ([§15.7](classes.md#157-properties)) and indexers ([§15.9](classes.md#159-indexers)). The text is written in terms of properties; when reading for indexers substitute indexer/indexers for property/properties and consult the list of differences between properties and indexers given in [§15.9.2](classes.md#1592-indexer-and-property-differences). *end note*

The *accessor_declarations* of a property specify the executable statements associated with writing and/or reading that property.

```ANTLR
accessor_declarations
    : get_accessor_declaration
      (set_accessor_declaration | init_accessor_declaration)?
    | (set_accessor_declaration | init_accessor_declaration)
      get_accessor_declaration?
    ;

get_accessor_declaration
    : attributes? accessor_modifier? 'get' accessor_body
    ;

set_accessor_declaration
    : attributes? accessor_modifier? 'set' accessor_body
    ;

init_accessor_declaration
    : attributes? accessor_modifier? 'init' accessor_body
    ;

accessor_modifier
    : 'protected'
    | 'internal'
    | 'private'
    | 'protected' 'internal'
    | 'internal' 'protected'
    | 'protected' 'private'
    | 'private' 'protected'
    | 'readonly'        // struct members only
    ;

accessor_body
    : block
    | '=>' expression ';'
    | ';' 
    ;

ref_get_accessor_declaration
    : attributes? accessor_modifier? 'get' ref_accessor_body
    ;
    
ref_accessor_body
    : block
    | '=>' 'ref' variable_reference ';'
    | ';'
    ;
```

The *accessor_declarations* consist of either a *get_accessor_declaration*, optionally with a *set_accessor_declaration* or an *init_accessor_declaration*, or a *set_accessor_declaration* or *init_accessor_declaration* optionally with a *get_accessor_declaration*.  Each accessor declaration consists of optional *attributes*, an optional *accessor_modifier*, the token `get`, `init`, or `set`, followed by an *accessor_body*.

For a ref-valued property the *ref_get_accessor_declaration* consists optional attributes, an optional *accessor_modifier*, the token `get`, followed by an *ref_accessor_body*.

The use of *accessor_modifier*s is governed by the following restrictions:

- An *accessor_modifier* shall not be used in an explicit interface member implementation.
- The *accessor_modifier* `readonly` is permitted only in a *property_declaration* or *indexer_declaration* that is contained directly by a *struct_declaration* ([§16.6.11](structs.md#16611-properties), [§16.6.13](structs.md#16613-indexers)).
- For a property or indexer that has no `override` modifier, an *accessor_modifier* is permitted only if the property or indexer has both a get and set or init accessor, and then is permitted only on one of those accessors.
- For a property or indexer that includes an `override` modifier, an accessor shall match the *accessor_modifier*, if any, of the accessor being overridden.
- The *accessor_modifier* shall declare an accessibility that is strictly more restrictive than the declared accessibility of the property or indexer itself. To be precise:
  - If the property or indexer has a declared accessibility of `public`, the accessibility declared by *accessor_modifier* may be either `private protected`, `protected internal`, `internal`, `protected`, or `private`.
  - If the property or indexer has a declared accessibility of `protected internal`, the accessibility declared by *accessor_modifier* may be either `private protected`, `protected private`, `internal`, `protected`, or `private`.
  - If the property or indexer has a declared accessibility of `internal` or `protected`, the accessibility declared by *accessor_modifier* shall be either `private protected` or `private`.
  - If the property or indexer has a declared accessibility of `private protected`, the accessibility declared by *accessor_modifier* shall be `private`.
  - If the property or indexer has a declared accessibility of `private`, no *accessor_modifier* may be used.

For `abstract` and `extern` non-ref-valued properties, any *accessor_body* for each accessor specified is simply a semicolon. A non-abstract, non-extern property, but not an indexer, may also have the *accessor_body* for all accessors specified be a semicolon, in which case it is an ***automatically implemented property*** ([§15.7.4](classes.md#1574-automatically-implemented-properties)). An automatically implemented property shall have at least a get accessor. For the accessors of any other non-abstract, non-extern property, the *accessor_body* is either:

- a *block* that specifies the statements to be executed when the corresponding accessor is invoked; or
- an expression body, which consists of `=>` followed by an *expression* and a semicolon, and denotes a single expression to be executed when the corresponding accessor is invoked.

For `abstract` and `extern` ref-valued properties the *ref_accessor_body* is simply a semicolon. For the accessor of any other non-abstract, non-extern property, the *ref_accessor_body* is either:

- a *block* that specifies the statements to be executed when the get accessor is invoked; or
- an expression body, which consists of `=>` followed by `ref`, a *variable_reference* and a semicolon. The variable reference is evaluated when the get accessor is invoked.

A get accessor for a non-ref-valued property corresponds to a parameterless method with a return value of the property type. Except as the target of an assignment, when such a property is referenced in an expression its get accessor is invoked to compute the value of the property ([§12.2.2](expressions.md#1222-values-of-expressions)).

The body of a get accessor for a non-ref-valued property shall conform to the rules for value-returning methods described in [§15.6.11](classes.md#15611-method-body). In particular, all `return` statements in the body of a get accessor shall specify an expression that is implicitly convertible to the property type. Furthermore, the endpoint of a get accessor shall not be reachable.

A get accessor for a ref-valued property corresponds to a parameterless method with a return value of a *variable_reference* to a variable of the property type. When such a property is referenced in an expression its get accessor is invoked to compute the *variable_reference* value of the property. That *variable reference*, like any other, is then used to read or, for non-readonly *variable_reference*s, write the referenced variable as required by the context.

> *Example*: The following example illustrates a ref-valued property as the target of an assignment:
>
> ```csharp
> class Program
> {
>     static int field;
>     static ref int Property => ref field;
>
>     static void Main()
>     {
>         field = 10;
>         Console.WriteLine(Property); // Prints 10
>         Property = 20;               // This invokes the get accessor, then assigns
>                                      // via the resulting variable reference
>         Console.WriteLine(field);    // Prints 20
>     }
> }
> ```
>
> *end example*

The body of a get accessor for a ref-valued property shall conform to the rules for ref-valued methods described in [§15.6.11](classes.md#15611-method-body).

A set accessor corresponds to a method with a single value parameter of the property type and a `void` return type. The implicit parameter of a set accessor is always named `value`. When a property is referenced as the target of an assignment ([§12.24](expressions.md#1224-assignment-operators)), or as the operand of `++` or `–-` ([§12.8.16](expressions.md#12816-postfix-increment-and-decrement-operators), [§12.9.7](expressions.md#1297-prefix-increment-and-decrement-operators)), the set accessor is invoked with an argument that provides the new value ([§12.24.2](expressions.md#12242-simple-assignment)). The body of a set accessor shall conform to the rules for `void` methods described in [§15.6.11](classes.md#15611-method-body). In particular, return statements in the set accessor body are not permitted to specify an expression. Since a set accessor implicitly has a parameter named `value`, it is a compile-time error for a local variable or constant declaration in a set accessor to have that name.

An init accessor corresponds to a method with a single value parameter of the property type and a `void` return type. The implicit parameter of an init accessor is always named `value`. Only during the construction phase of an object ([§15.7.3.3](classes.md#15733-init-accessors)), and if an init accessor exists, when a property is referenced as the target of an assignment ([§12.24](expressions.md#1224-assignment-operators)), or as the operand of `++` or `–-` ([§12.8.16](expressions.md#12816-postfix-increment-and-decrement-operators), [§12.9.7](expressions.md#1297-prefix-increment-and-decrement-operators)), the init accessor is invoked with an argument that provides the new value ([§12.24.2](expressions.md#12242-simple-assignment)). The body of an init accessor shall conform to the rules for `void` methods described in [§15.6.11](classes.md#15611-method-body). In particular, return statements in the init accessor body are not permitted to specify an expression. Since an init accessor implicitly has a parameter named `value`, it is a compile-time error for a local variable or constant declaration in an init accessor to have that name.

It is a compile-time error for a *property_declaration* containing an *init_accessor_declaration* to also have the *property_modifier* `static`.

Based on the presence or absence of get, set, and init accessors, a property is classified as follows:

- A property that includes both a get accessor and a set accessor is said to be a ***read-write property***.
- A property that includes both a get accessor and an init accessor is said to be a ***read-init property***. It is a compile-time error for a read-init property to be the target of an assignment except during the construction phase of an object ([§15.7.3.3](classes.md#15733-init-accessors)).
- A property that has only a get accessor is said to be a ***read-only property***. It is a compile-time error for a read-only property to be the target of an assignment.
- A property that has only a set accessor is said to be a ***write-only property***. Except as the target of an assignment, it is a compile-time error to reference a write-only property in an expression.
- A property that has only an init accessor is said to be an ***init-only property***. Except as the target of an assignment during the construction phase of an object ([§15.7.3.3](classes.md#15733-init-accessors)), it is a compile-time error to reference an init-only property in an expression.

> *Note*: The pre- and postfix `++` and `--` operators and compound assignment operators cannot be applied to write-only properties, since these operators read the old value of their operand before they write the new one. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: In the following code
>
> <!-- Maintenance Note: A version of this type exists in additional-files as "Control.cs". As such, certain changes to this type definition might need to be reflected in that file, in which case, *all* examples using that file should be tested. -->
> <!-- Example: {template:"standalone-lib-without-using", name:"Accessors1", additionalFiles:["Graphics.cs","RectangleNoPoint.cs","Control.cs"]} -->
> ```csharp
> public class Button : Control
> {
>     private string caption;
>
>     public string Caption
>     {
>         get => caption;
>         set
>         {
>             if (caption != value)
>             {
>                 caption = value;
>                 Repaint();
>             }
>         }
>     }
>
>     public override void Paint(Graphics g, Rectangle r)
>     {
>         // Painting code goes here
>     }
> }
> ```
>
> the `Button` control declares a public `Caption` property. The get accessor of the Caption property returns the `string` stored in the private `caption` field. The set accessor checks if the new value is different from the current value, and if so, it stores the new value and repaints the control. Properties often follow the pattern shown above: The get accessor simply returns a value stored in a `private` field, and the set accessor modifies that `private` field and then performs any additional actions required to update fully the state of the object.
> Given the `Button` class above, the following is an example of use of the `Caption` property:
>
> ```csharp
> Button okButton = new Button();
> okButton.Caption = "OK"; // Invokes set accessor
> string s = okButton.Caption; // Invokes get accessor
> ```
>
> Here, the set accessor is invoked by assigning a value to the property, and the get accessor is invoked by referencing the property in an expression.
>
> *end example*

For more information about set and get accessors, see [§15.7.3.2](classes.md#15732-set-and-get-accessors).

> *Example*: Consider the following, immutable type, which has auto-implemented properties:
>
> <!-- Example: {template:"standalone-lib", name:"AccessorsInit1"} -->
> ```csharp
> struct Point
> {
>     public int X { get; init; }
>     public int Y { get; init; }
> }
> ```
>
> A consumer can then use object initializers to create the object, as follows:
>
> <!-- Example: {template:"code-in-main-without-using", name:"AccessorsInit2", additionalFiles:["PointStructWithInit.cs"]} -->
>
> ```csharp
> var p = new Point() { X = 42, Y = 13 };
> ```
>
> *end example*

For more information about init accessors, see [§15.7.3.3](classes.md#15733-init-accessors).

#### 15.7.3.2 Set and get accessors

The get and set accessors of a property are not distinct members, and it is not possible to declare the accessors of a property separately.

> *Example*: The example
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Accessors2", expectedErrors:["CS0102"]} -->
> ```csharp
> class A
> {
>     private string name;
>
>     // Error, duplicate member name
>     public string Name
>     { 
>         get => name;
>     }
>
>     // Error, duplicate member name
>     public string Name
>     { 
>         set => name = value;
>     }
> }
> ```
>
> does not declare a single read-write property. Rather, it declares two properties with the same name, one read-only and one write-only. Since two members declared in the same class cannot have the same name, the example causes a compile-time error to occur.
>
> *end example*

When a derived class declares a property by the same name as an inherited property, the derived property hides the inherited property with respect to both reading and writing.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Accessors3", replaceEllipsis:true, expectedErrors:["CS0161"]} -->
> ```csharp
> class A
> {
>     public int P
>     {
>         set {...}
>     }
> }
>
> class B : A
> {
>     public new int P
>     {
>         get {...}
>     }
> }
> ```
>
> the `P` property in `B` hides the `P` property in `A` with respect to both reading and writing. Thus, in the statements
>
> ```csharp
> B b = new B();
> b.P = 1;       // Error, B.P is read-only
> ((A)b).P = 1;  // Ok, reference to A.P
> ```
>
> the assignment to `b.P` causes a compile-time error to be reported, since the read-only `P` property in `B` hides the write-only `P` property in `A`. Note, however, that a cast can be used to access the hidden `P` property.
>
> *end example*

Unlike public fields, properties provide a separation between an object’s internal state and its public interface.

> *Example*: Consider the following code, which uses a `Point` struct to represent a location:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Accessors4", additionalFiles:["Point.cs"]} -->
> ```csharp
> class Label
> {
>     private int x, y;
>     private string caption;
>
>     public Label(int x, int y, string caption)
>     {
>         this.x = x;
>         this.y = y;
>         this.caption = caption;
>     }
>
>     public int X => x;
>     public int Y => y;
>     public Point Location => new Point(x, y);
>     public string Caption => caption;
> }
> ```
>
> Here, the `Label` class uses two `int` fields, `x` and `y`, to store its location. The location is publicly exposed both as an `X` and a `Y` property and as a `Location` property of type `Point`. If, in a future version of `Label`, it becomes more convenient to store the location as a `Point` internally, the change can be made without affecting the public interface of the class:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Accessors5", additionalFiles:["Point.cs"]} -->
> ```csharp
> class Label
> {
>     private Point location;
>     private string caption;
>
>     public Label(int x, int y, string caption)
>     {
>         this.location = new Point(x, y);
>         this.caption = caption;
>     }
>
>     public int X => location.X;
>     public int Y => location.Y;
>     public Point Location => location;
>     public string Caption => caption;
> }
> ```
>
> Had `x` and `y` instead been `public readonly` fields, it would have been impossible to make such a change to the `Label` class.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: Exposing state through properties is not necessarily any less efficient than exposing fields directly. In particular, when a property is non-virtual and contains only a small amount of code, the execution environment might replace calls to accessors with the actual code of the accessors. This process is known as ***inlining***, and it makes property access as efficient as field access, yet preserves the increased flexibility of properties. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: Since invoking a get accessor is conceptually equivalent to reading the value of a field, it is considered bad programming style for get accessors to have observable side-effects. In the example
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Accessors6"} -->
> ```csharp
> class Counter
> {
>     private int next;
>
>     public int Next => next++;
> }
> ```
>
> the value of the `Next` property depends on the number of times the property has previously been accessed. Thus, accessing the property produces an observable side effect, and the property should be implemented as a method instead.
>
> The “no side-effects” convention for get accessors does not mean that get accessors should always be written simply to return values stored in fields. Indeed, get accessors often compute the value of a property by accessing multiple fields or invoking methods. However, a properly designed get accessor performs no actions that cause observable changes in the state of the object.
>
> *end example*

Properties can be used to delay initialization of a resource until the moment it is first referenced.

> *Example*:
>
> <!-- Example: {template:"standalone-lib", name:"Accessors7", replaceEllipsis:true, customEllipsisReplacements:["static Stream OpenStandardInput() => null; static Stream OpenStandardOutput() => null; static Stream OpenStandardError() => null; "]} -->
> ```csharp
> public class Console
> {
>     private static TextReader reader;
>     private static TextWriter writer;
>     private static TextWriter error;
>
>     public static TextReader In
>     {
>         get
>         {
>             if (reader == null)
>             {
>                 reader = new StreamReader(Console.OpenStandardInput());
>             }
>             return reader;
>         }
>     }
>
>     public static TextWriter Out
>     {
>         get
>         {
>             if (writer == null)
>             {
>                 writer = new StreamWriter(Console.OpenStandardOutput());
>             }
>             return writer;
>         }
>     }
>
>     public static TextWriter Error
>     {
>         get
>         {
>             if (error == null)
>             {
>                 error = new StreamWriter(Console.OpenStandardError());
>             }
>             return error;
>         }
>     }
> ...
> }
> ```
>
> The `Console` class contains three properties, `In`, `Out`, and `Error`, that represent the standard input, output, and error devices, respectively. By exposing these members as properties, the `Console` class can delay their initialization until they are actually used. For example, upon first referencing the `Out` property, as in
>
> <!-- Example: {template:"code-in-main", name:"ConsoleOutWriteLine", expectedOutput:["hello, world"]} -->
> ```csharp
> Console.Out.WriteLine("hello, world");
> ```
>
> the underlying `TextWriter` for the output device is created. However, if the application makes no reference to the `In` and `Error` properties, then no objects are created for those devices.
>
> *end example*

#### 15.7.3.3 Init accessors

An instance property containing an *init_accessor_declaration* is considered settable during the construction phase of the object, except when in a local function or lambda. The ***construction phase of an object*** includes the following:

- During execution of an *object_initializer* ([§12.8.17.3](expressions.md#128173-object-initializers))
- During evaluation of a *with_expression*’s *member_initializer_list*  ([§12.10](expressions.md#1210-with-expressions))

- Inside an instance constructor of the containing or derived type, on `this` or `base`
- Inside the *init_accessor_declaration* of any property, on `this` or `base`
- Inside attribute usages with named parameters ([§23.2.3](attributes.md#2323-positional-and-named-parameters))

> *Example*: Consider the following:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AccessorsInit3", expectedErrors:["CS8852"]} -->
> ```csharp
> class Student
> {
>     public string? FirstName { get; init; }
>     public string? LastName  { get; init; }
> }
> 
> class Consumption
> {
>     static void Example()
>     {
>         var s = new Student()
>         {
>             FirstName = "Jared",
>             LastName = "Parosns",  // OK: LastName is settable
>         };
>         s.LastName = "Parsons";    // Error: LastName is not settable
>     }
> }
> ```
>
> *end example*

The rules around when init accessors are settable extend across type hierarchies. If the member is accessible and the object is known to be in the construction phase, then the member is settable.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AccessorsInit4"} -->
> ```csharp
> class Base
> {
>     public bool Value { get; init; }
> }
>
> class Derived : Base
> {
>     public Derived()
>     {
>         Value = true;
>     }
> }
> 
> class Consumption
> {
>     void Example()
>     {
>         var d = new Derived() { Value = true };
>     }
> }
> ```
>
> *end example*

At the point an init accessor is invoked, the instance is known to be in the construction phase. Hence an init accessor may take the following actions in addition to what a set accessor can do:

1. Call other init accessors available through `this` or `base`
1. Assign `readonly` fields declared on the same type through `this`

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AccessorsInit5", ignoredWarnings:["CS0414"]} -->
> ```csharp
> class Complex
> {
>     readonly int Field1;
>     int Field2;
>     int Prop1 { get; init ; }
>     int Prop2
>     {
>         get => 42;
>         init
>         {
>             Field1 = 13; // OK
>             Field2 = 13; // OK
>             Prop1 = 13;  // OK
>         }
>     }
> }
> ```
>
> *end example*

The ability to assign to `readonly` fields from an init accessor is limited to those fields declared on the same type as the accessor. It cannot be used to assign `readonly` fields in a base type. This rule ensures that type authors remain in control over the mutability behavior of their type. Developers who do not wish to utilize init accessors cannot be impacted from other types choosing to do so.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AccessorsInit6", expectedErrors:["CS0191","CS0191"]} -->
> ```csharp
> class Base
> {
>     internal readonly int Field;
>     internal int Property
>     {
>         get => Field;
>         init => Field = value; // OK
>     }
>     internal int OtherProperty { get; init; }
> }
> 
> class Derived : Base
> {
>     internal readonly int DerivedField;
> 
>     internal int DerivedProperty
>     {
>         get => DerivedField;
>         init
>         {
>             DerivedField = 42;  // OK
>             Property = 0;       // OK
>             Field = 13;         // Error: Field is readonly
>         }
>     }
> 
>     public Derived()
>     {
>         Property = 42;  // OK 
>         Field = 13;     // Error: Field is readonly
>     }
> }
> ```
>
> *end example*

An interface declaration can also participate in `init`-style initialization, via the following pattern:

<!-- Example: {template:"standalone-lib-without-using", name:"AccessorsInit7", expectedErrors:["CS8852"]} -->
```csharp

interface IPerson
{
    string Name { get; init; }
}

class Init
{
    void M<T>() where T : IPerson, new()
    {
        var local = new T()
        {
            Name = "Jared"
        };
        local.Name = "Jraed"; // Error
    }
}
```

However, there are restrictions:

- The init accessor can only be used on instance properties
- A property cannot contain both an init accessor and a set accessor
- All overrides of a property shall have `init` if the base has `init`. This rule also applies to interface implementation.

Init accessors (both auto- and manually-implemented) are permitted on properties of `readonly struct`s, as well as `readonly` properties. Init accessors are not permitted to be marked `readonly` themselves, in both `readonly` and non-`readonly struct` types.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AccessorsInit8", expectedErrors:["CS8903"]} -->
> ```csharp
> readonly struct ReadonlyStruct1
> {
>     public int Prop1 { get; init; } // OK
> }
>
> struct ReadonlyStruct2
> {
>     public readonly int Prop2 { get; init; } // OK
>     public int Prop3 { get; readonly init; } // Error
> }
> ```
>
> *end example*

### 15.7.4 Automatically implemented properties

An automatically implemented property (or auto-property for short), is a non-abstract, non-extern, non-ref-valued property with semicolon-only *accessor_body*s. An auto-property shall have a get accessor and may optionally have a set or init accessor.

When a property is specified as an automatically implemented property, a hidden backing field is automatically available for the property, and the accessors are implemented to read from and write to that backing field. The hidden backing field is inaccessible, it can be read and written only through the automatically implemented property accessors, even within the containing type. If the auto-property has no set or init accessor, the backing field is considered `readonly` ([§15.5.3](classes.md#1553-readonly-fields)). Just like a `readonly` field, a read-only auto-property may also be assigned to in the body of a constructor of the enclosing class. Such an assignment assigns directly to the read-only backing field of the property. If the auto-property has an init accessor, the backing field may be assigned to during the construction phase of an object ([§15.7.3.3](classes.md#15733-init-accessors)).

An auto-property may optionally have a *property_initializer*, which is applied directly to the backing field as a *variable_initializer* ([§17.7](arrays.md#177-array-initializers)).

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AutomaticProperties1"} -->
> ```csharp
> public class Point
> {
>     public int X { get; set; } // Automatically implemented
>     public int Y { get; set; } // Automatically implemented
> }
> ```
>
> is equivalent to the following declaration:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AutomaticProperties2"} -->
> ```csharp
> public class Point
> {
>     private int x;
>     private int y;
>
>     public int X { get { return x; } set { x = value; } }
>     public int Y { get { return y; } set { y = value; } }
> }
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: In the following
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AutomaticProperties3"} -->
> ```csharp
> public class ReadOnlyPoint
> {
>     public int X { get; }
>     public int Y { get; }
>
>     public ReadOnlyPoint(int x, int y)
>     {
>         X = x;
>         Y = y;
>     }
> }
> ```
>
> is equivalent to the following declaration:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AutomaticProperties4"} -->
> ```csharp
> public class ReadOnlyPoint
> {
>     private readonly int __x;
>     private readonly int __y;
>     public int X { get { return __x; } }
>     public int Y { get { return __y; } }
>
>     public ReadOnlyPoint(int x, int y)
>     {
>         __x = x;
>         __y = y;
>     }
> }
> ```
>
> The assignments to the read-only field are valid, because they occur within the constructor.
>
> *end example*

Although the backing field is hidden, that field may have field-targeted attributes applied directly to it via the automatically implemented property’s *property_declaration* ([§15.7.1](classes.md#1571-general)).

> *Example*: The following code
>
> <!-- Example: {template:"standalone-lib", name:"AutomaticProperties5"} -->
> ```csharp
> [Serializable]
> public class Foo
> {
>     [field: NonSerialized]
>     public string MySecret { get; set; }
> }
> ```
>
> results in the field-targeted attribute `NonSerialized` being applied to the compiler-generated backing field, as if the code had been written as follows:
>
> <!-- Example: {template:"standalone-lib", name:"AutomaticProperties6"} -->
> ```csharp
> [Serializable]
> public class Foo
> {
>     [NonSerialized]
>     private string _mySecretBackingField;
>     public string MySecret
>     {
>         get { return _mySecretBackingField; }
>         set { _mySecretBackingField = value; }
>     }
> }
> ```
>
> *end example*

### 15.7.5 Accessibility

If an accessor has an *accessor_modifier*, the accessibility domain ([§7.5.3](basic-concepts.md#753-accessibility-domains)) of the accessor is determined using the declared accessibility of the *accessor_modifier*. If an accessor does not have an *accessor_modifier*, the accessibility domain of the accessor is determined from the declared accessibility of the property or indexer.

The presence of an *accessor_modifier* never affects member lookup ([§12.5](expressions.md#125-member-lookup)) or overload resolution ([§12.6.4](expressions.md#1264-overload-resolution)). The modifiers on the property or indexer always determine which property or indexer is bound to, regardless of the context of the access.

Once a particular non-ref-valued property or non-ref-valued indexer has been selected, the accessibility domains of the specific accessors involved are used to determine if that usage is valid:

- If the usage is as a value ([§12.2.2](expressions.md#1222-values-of-expressions)), the get accessor shall exist and be accessible.
- If the usage is as the target of a simple assignment ([§12.24.2](expressions.md#12242-simple-assignment)), the set or init accessor shall exist and be accessible.
- If the usage is as the target of compound assignment ([§12.24.5](expressions.md#12245-compound-assignment)), or as the target of the `++` or `--` operators ([§12.8.16](expressions.md#12816-postfix-increment-and-decrement-operators), [§12.9.7](expressions.md#1297-prefix-increment-and-decrement-operators)), both the get accessors and the set or init accessor shall exist and be accessible.

> *Example*: In the following example, the property `A.Text` is hidden by the property `B.Text`, even in contexts where only the set accessor is called. In contrast, the property `B.Count` is not accessible to class `M`, so the accessible property `A.Count` is used instead.
>
> <!-- Example: {template:"standalone-console-without-using", name:"Accessibility1", expectedErrors:["CS0272"]} -->
> ```csharp
> class A
> {
>     public string Text
>     {
>         get => "hello";
>         set { }
>     }
>
>     public int Count
>     {
>         get => 5;
>         set { }
>     }
> }
> 
> class B : A
> {
>     private string text = "goodbye";
>     private int count = 0;
> 
>     public new string Text
>     {
>         get => text;
>         protected set => text = value;
>     }
>
>     protected new int Count
>     {
>         get => count;
>         set => count = value;
>     }
> }
>
> class M
> {
>     static void Main()
>     {
>         B b = new B();
>         b.Count = 12;       // Calls A.Count set accessor
>         int i = b.Count;    // Calls A.Count get accessor
>         b.Text = "howdy";   // Error, B.Text set accessor not accessible
>         string s = b.Text;  // Calls B.Text get accessor
>     }
> }
> ```
>
> *end example*

Once a particular ref-valued property or ref-valued indexer has been selected—whether the usage is as a value, the target of a simple assignment, or the target of a compound assignment—the accessibility domain of the get accessor involved is used to determine if that usage is valid.

An accessor that is used to implement an interface shall not have an *accessor_modifier*. If only one accessor is used to implement an interface, the other accessor may be declared with an *accessor_modifier*:

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Accessibility2", replaceEllipsis:true} -->
> ```csharp
> public interface I
> {
>     string Prop { get; }
> }
>
> public class C : I
> {
>     public string Prop
>     {
>         get => "April";     // Must not have a modifier here
>         internal set {...}  // Ok, because I.Prop has no set accessor
>     }
> }
> ```
>
> *end example*

The set or init accessor of a required property ([§15.7.1](classes.md#1571-general)) shall be at least as accessible as that property’s containing type.

### 15.7.6 Virtual, sealed, override, and abstract accessors

*Note*: This subclause applies to both properties ([§15.7](classes.md#157-properties)) and indexers ([§15.9](classes.md#159-indexers)). The subclause is written in terms of properties, when reading for indexers substitute indexer/indexers for property/properties and consult the list of differences between properties and indexers given in [§15.9.2](classes.md#1592-indexer-and-property-differences). *end note*

A virtual property declaration specifies that the accessors of the property are virtual. The `virtual` modifier applies to all non-private accessors of a property. When an accessor of a virtual property has the `private` *accessor_modifier*, the private accessor is implicitly not virtual.

An abstract property declaration specifies that the accessors of the property are virtual, but does not provide an actual implementation of the accessors. Instead, non-abstract derived classes are required to provide their own implementation for the accessors by overriding the property. Because an accessor for an abstract property declaration provides no actual implementation, its *accessor_body* simply consists of a semicolon. An abstract property shall not have a `private` accessor.

A property that overrides a base property that is required ([§15.7.1](classes.md#1571-general)) shall itself be required. A property that overrides a base property that is not required may itself be required, in which case, the derived type member is added to that type’s required member list.

> *Note*: A type is permitted to override a required virtual property. This means that if the base virtual property has storage, and the derived type tries to access the base implementation of that property, it could observe uninitialized storage. *end note*

A property declaration that includes both the `abstract` and `override` modifiers specifies that the property is abstract and overrides a base property. The accessors of such a property are also abstract.

Abstract property declarations are only permitted in abstract classes ([§15.2.2.2](classes.md#15222-abstract-classes)) and interfaces ([§19.4.4](interfaces.md#1944-interface-properties)). The accessors of an inherited virtual property can be overridden in a derived class or interface by including a property declaration that specifies an `override` directive. This is known as an ***overriding property declaration***. An overriding property declaration does not declare a new property. Instead, it simply specializes the implementations of the accessors of an existing virtual property.

For an override property `P` declared in an interface `I`, the overridden base property is determined by examining each direct or indirect base interface of `I`, collecting the set of interfaces declaring an accessible property which has the same name as `P`. If this set of interfaces has a *most derived type*, to which there is an identity or implicit reference conversion from every type in this set, and that type contains a unique such property declaration, then that is the overridden base property.

The override declaration and the overridden base property are required to have the same declared accessibility. In other words, an override declaration shall not change the accessibility of the base property. However, if the overridden base property is protected internal and it is declared in a different assembly than the assembly containing the override declaration then the override declaration’s declared accessibility shall be protected. The overriding property’s type shall be at least as accessible as the overriding property. If the inherited property has only a single accessor (i.e., if the inherited property is read-only or write-only), the overriding property shall include only that accessor. If the inherited property includes both accessors (i.e., if the inherited property is read-write), the overriding property can include either a single accessor or both accessors. There shall be an identity conversion or (if the inherited property is read-only and has a value return) an implicit reference conversion between the type of the overriding and the inherited property. For an override property declared in an interface, there shall also be an identity conversion or (if the inherited property is read-only and has a value return) an implicit reference conversion between the type of the overriding property and the type of every override of the overridden base property that is declared in a (direct or indirect) base interface of the overriding property.

> *Note*: The accessibility constraint on the overriding property’s type permits an override property in a `private` class to have a `private` property type. However, it requires a `public` override property in a `public` type to have a `public` property type. *end note*

An overriding property declaration may include the `sealed` modifier. Use of this modifier prevents a derived class or interface from further overriding the property. The accessors of a sealed property are also sealed.

Except for differences in declaration and invocation syntax, virtual, sealed, override, and abstract accessors behave exactly like virtual, sealed, override and abstract methods. Specifically, the rules described in [§15.6.4](classes.md#1564-virtual-methods), [§15.6.5](classes.md#1565-override-methods), [§15.6.6](classes.md#1566-sealed-methods), and [§15.6.7](classes.md#1567-abstract-methods) apply as if accessors were methods of a corresponding form:

- A get accessor corresponds to a parameterless method with a return value of the property type and the same modifiers as the containing property.
- A set accessor corresponds to a method with a single value parameter of the property type, a void return type, and the same modifiers as the containing property.
- An init accessor corresponds to a method with a single value parameter of the property type, a `void` return type, and the same modifiers as the containing property.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"VirtualAbstractAccessors"} -->
> <!-- Maintenance Note: A version of this type exists in additional-files as "A.cs". As such, certain changes to this type definition might need to be reflected in that file, in which case, *all* examples using that file should be tested. -->
> ```csharp
> abstract class A
> {
>     int y;
>
>     public virtual int X
>     {
>         get => 0;
>     }
>
>     public virtual int Y
>     {
>         get => y;
>         set => y = value;
>     }
>
>     public abstract int Z { get; set; }
> }
> ```
>
> `X` is a virtual read-only property, `Y` is a virtual read-write property, and `Z` is an abstract read-write property. Because `Z` is abstract, the containing class A shall also be declared abstract.
>
> A class that derives from `A` is shown below:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"OverrideAccessors", additionalFiles:["A.cs"]} -->
> ```csharp
> class B : A
> {
>     int z;
>
>     public override int X
>     {
>         get => base.X + 1;
>     }
>
>     public override int Y
>     {
>         set => base.Y = value < 0 ? 0: value;
>     }
>
>     public override int Z
>     {
>         get => z;
>         set => z = value;
>     }
> }
> ```
>
> Here, the declarations of `X`, `Y`, and `Z` are overriding property declarations. Each property declaration exactly matches the accessibility modifiers, type, and name of the corresponding inherited property. The get accessor of `X` and the set accessor of `Y` use the base keyword to access the inherited accessors. The declaration of `Z` overrides both abstract accessors—thus, there are no outstanding `abstract` function members in `B`, and `B` is permitted to be a non-abstract class.
>
> *end example*

When a property is declared as an override, any overridden accessors shall be accessible to the overriding code. In addition, the declared accessibility of both the property or indexer itself, and of the accessors, shall match that of the overridden member and accessors.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"VirtualOverrideAccessors", replaceEllipsis:true, customEllipsisReplacements:["return 0;",null,"return 0;"]} -->
> ```csharp
> public class B
> {
>     public virtual int P
>     {
>         get {...}
>         protected set {...}
>     }
> }
>
> public class D: B
> {
>     public override int P
>     {
>         get {...}            // Must not have a modifier here
>         protected set {...}  // Must specify protected here
>     }
> }
> ```
>
> *end example*

When an init accessor appears in a virtual property, all overrides for it shall also be marked `init`. Likewise, it is not possible to override a set accessor with an init accessor.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AccessorsInit9", expectedErrors:["CS8853"]} -->
> ```csharp
> class Base
> {
>     public virtual int Property { get; init; }
> }
> 
> class C1 : Base
> {
>     public override int Property { get; init; }
> }
> 
> class C2 : Base
> {
>     // Error: Property must have init to override Base.Property
>     public override int Property { get; set; }
> }
> ```
>
> *end example*

## 15.8 Events

### 15.8.1 General

An ***event*** is a member that enables an object or class to provide notifications. Clients can attach executable code for events by supplying ***event handler***s.

Events are declared using *event_declaration*s:

```ANTLR
event_declaration
    : attributes? event_modifier* 'event' type variable_declarators ';'
    | attributes? event_modifier* 'event' type member_name
        '{' event_accessor_declarations '}'
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
    | 'readonly'        // struct members only
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

*unsafe_modifier* ([§24.2](unsafe-code.md#242-unsafe-contexts)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).

An *event_declaration* may include a set of *attributes* ([§23](attributes.md#23-attributes)) and any one of the permitted kinds of declared accessibility ([§15.3.6](classes.md#1536-access-modifiers)), the `new` ([§15.3.5](classes.md#1535-the-new-modifier)), `static` ([§15.6.3](classes.md#1563-static-and-instance-methods), [§15.8.4](classes.md#1584-static-and-instance-events)), `virtual` ([§15.6.4](classes.md#1564-virtual-methods), [§15.8.5](classes.md#1585-virtual-sealed-override-and-abstract-accessors)), `override` ([§15.6.5](classes.md#1565-override-methods), [§15.8.5](classes.md#1585-virtual-sealed-override-and-abstract-accessors)), `sealed` ([§15.6.6](classes.md#1566-sealed-methods)), `abstract` ([§15.6.7](classes.md#1567-abstract-methods), [§15.8.5](classes.md#1585-virtual-sealed-override-and-abstract-accessors)) and `extern` ([§15.6.8](classes.md#1568-external-methods)) modifiers. Additionally an *event_declaration* that is contained directly by a *struct_declaration* may include the `readonly` modifier ([§16.6.12](structs.md#16612-methods)).

Event declarations are subject to the same rules as method declarations ([§15.6](classes.md#156-methods)) with regard to valid combinations of modifiers.

The *type* of an event declaration shall be a *delegate_type* ([§8.2.8](types.md#828-delegate-types)), and that *delegate_type* shall be at least as accessible as the event itself ([§7.5.5](basic-concepts.md#755-accessibility-constraints)).

An event declaration can include *event_accessor_declaration*s. However, if it does not, for non-extern, non-abstract events, a compiler shall supply them automatically ([§15.8.2](classes.md#1582-field-like-events)); for `extern` events, the accessors are provided externally.

An event declaration that omits *event_accessor_declaration*s defines one or more events—one for each of the *variable_declarator*s. The attributes and modifiers apply to all of the members declared by such an *event_declaration*.

It is a compile-time error for an *event_declaration* to include both the `abstract` modifier and *event_accessor_declaration*s.

When an event declaration includes an `extern` modifier, the event is said to be an ***external event***. Because an external event declaration provides no actual implementation, it is an error for it to include both the `extern` modifier and *event_accessor_declaration*s.

It is a compile-time error for a *variable_declarator* of an event declaration with an `abstract` or `extern` modifier to include a *variable_initializer*.

An event can be used as the left operand of the `+=` and `-=` operators. These operators are used, respectively, to attach event handlers to, or to remove event handlers from an event, and the access modifiers of the event control the contexts in which such operations are permitted.

The only operations that are permitted on an event by code that is outside the type in which that event is declared, are `+=` and `-=`. Therefore, while such code can add and remove handlers for an event, it cannot directly obtain or modify the underlying list of event handlers.

In an operation of the form `x += y` or `x –= y`, when `x` is an event the result of the operation has type `void` ([§12.24.6](expressions.md#12246-event-assignment)) (as opposed to having the type of `x`, with the value of `x` after the assignment, as for other the `+=` and `-=` operators defined on non-event types). This prevents external code from indirectly examining the underlying delegate of an event.

> *Example*: The following example shows how event handlers are attached to instances of the `Button` class:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Events", replaceEllipsis:true, expectedWarnings:["CS0067"], additionalFiles:["Control.cs","Graphics.cs","Rectangle.cs","Point.cs","Form.cs"]} -->
> ```csharp
> public delegate void EventHandler(object sender, EventArgs e);
>
> public class Button : Control
> {
>     public event EventHandler Click;
> }
>
> public class LoginDialog : Form
> {
>     Button okButton;
>     Button cancelButton;
>
>     public LoginDialog()
>     {
>         okButton = new Button(...);
>         okButton.Click += new EventHandler(OkButtonClick);
>         cancelButton = new Button(...);
>         cancelButton.Click += new EventHandler(CancelButtonClick);
>     }
>
>     void OkButtonClick(object sender, EventArgs e)
>     {
>         // Handle okButton.Click event
>     }
>
>     void CancelButtonClick(object sender, EventArgs e)
>     {
>         // Handle cancelButton.Click event
>     }
> }
> ```
>
> Here, the `LoginDialog` instance constructor creates two `Button` instances and attaches event handlers to the `Click` events.
>
> *end example*

### 15.8.2 Field-like events

Within the program text of the class or struct that contains the declaration of an event, certain events can be used like fields. To be used in this way, an event shall not be abstract or extern, and shall not explicitly include *event_accessor_declaration*s. Such an event can be used in any context that permits a field. The field contains a delegate ([§21](delegates.md#21-delegates)), which refers to the list of event handlers that have been added to the event. If no event handlers have been added, the field contains `null`.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"FieldlikeEvents1", additionalFiles:["Control.cs","Graphics.cs","Rectangle.cs","Point.cs"]} -->
> ```csharp
> public delegate void EventHandler(object sender, EventArgs e);
>
> public class Button : Control
> {
>     public event EventHandler Click;
>
>     protected void OnClick(EventArgs e)
>     {
>         EventHandler handler = Click;
>         if (handler != null)
>         {
>             handler(this, e);
>         }
>     }
>
>     public void Reset() => Click = null;
> }
> ```
>
> `Click` is used as a field within the `Button` class. As the example demonstrates, the field can be examined, modified, and used in delegate invocation expressions. The `OnClick` method in the `Button` class “raises” the `Click` event. The notion of raising an event is precisely equivalent to invoking the delegate represented by the event—thus, there are no special language constructs for raising events. Note that the delegate invocation is preceded by a check that ensures the delegate is non-null and that the check is made on a local copy to ensure thread safety.
>
> Outside the declaration of the `Button` class, the `Click` member can only be used on the left-hand side of the `+=` and `–=` operators, as in
>
> ```csharp
> b.Click += new EventHandler(...);
> ```
>
> which appends a delegate to the invocation list of the `Click` event, and
>
> ```csharp
> Click –= new EventHandler(...);
> ```
>
> which removes a delegate from the invocation list of the `Click` event.
>
> *end example*

When compiling a field-like event, a compiler shall automatically create storage to hold the delegate, and shall create accessors for the event that add or remove event handlers to the delegate field. The addition and removal operations are thread safe, and may (but are not required to) be done while holding the lock ([§13.13](statements.md#1313-the-lock-statement)) on the containing object for an instance event, or the `System.Type` object ([§12.8.18](expressions.md#12818-the-typeof-operator)) for a static event.

> *Note*: Thus, an instance event declaration of the form:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"FieldlikeEvents2", expectedWarnings:["CS0067"], additionalFiles:["D.cs"]} -->
> ```csharp
> class X
> {
>     public event D Ev;
> }
> ```
>
> shall be compiled to something equivalent to:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"FieldlikeEvents3", expectedWarnings:["CS0169"], additionalFiles:["D.cs"]} -->
> ```csharp
> class X
> {
>     private D __Ev; // field to hold the delegate
>
>     public event D Ev
>     {
>         add
>         {
>             /* Add the delegate in a thread safe way */
>         }
>         remove
>         {
>             /* Remove the delegate in a thread safe way */
>         }
>     }
> }
> ```
>
> Within the class `X`, references to `Ev` on the left-hand side of the `+=` and `–=` operators cause the add and remove accessors to be invoked. All other references to `Ev` are compiled to reference the hidden field `__Ev` instead ([§12.8.7](expressions.md#1287-member-access)). The name “`__Ev`” is arbitrary; the hidden field could have any name or no name at all.
>
> *end note*

### 15.8.3 Event accessors

> *Note*: Event declarations typically omit *event_accessor_declaration*s, as in the `Button` example above. For example, they might be included if the storage cost of one field per event is not acceptable. In such cases, a class can include *event_accessor_declaration*s and use a private mechanism for storing the list of event handlers. *end note*

The *event_accessor_declarations* of an event specify the executable statements associated with adding and removing event handlers.

The accessor declarations consist of an *add_accessor_declaration* and a *remove_accessor_declaration*. Each accessor declaration consists of the token add or remove followed by a *block*. The *block* associated with an *add_accessor_declaration* specifies the statements to execute when an event handler is added, and the *block* associated with a *remove_accessor_declaration* specifies the statements to execute when an event handler is removed.

Each *add_accessor_declaration* and *remove_accessor_declaration* corresponds to a method with a single value parameter of the event type, and a `void` return type. The implicit parameter of an event accessor is named `value`. When an event is used in an event assignment, the appropriate event accessor is used. Specifically, if the assignment operator is `+=` then the add accessor is used, and if the assignment operator is `–=` then the remove accessor is used. In either case, the right operand of the assignment operator is used as the argument to the event accessor. The block of an *add_accessor_declaration* or a *remove_accessor_declaration* shall conform to the rules for `void` methods described in [§15.6.11](classes.md#15611-method-body). In particular, `return` statements in such a block are not permitted to specify an expression.

Since an event accessor implicitly has a parameter named `value`, it is a compile-time error for a local variable or constant declared in an event accessor to have that name.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"EventAccessors", replaceEllipsis:true, customEllipsisReplacements:["return null;"], additionalFiles:["Component.cs","MouseEventHandler.cs","MouseEventArgs.cs"]} -->
> ```csharp
>
> class Control : Component
> {
>     // Unique keys for events
>     static readonly object mouseDownEventKey = new object();
>     static readonly object mouseUpEventKey = new object();
>
>     // Return event handler associated with key
>     protected Delegate GetEventHandler(object key) {...}
>
>     // Add event handler associated with key
>     protected void AddEventHandler(object key, Delegate handler) {...}
>
>     // Remove event handler associated with key
>     protected void RemoveEventHandler(object key, Delegate handler) {...}
>
>     // MouseDown event
>     public event MouseEventHandler MouseDown
>     {
>         add { AddEventHandler(mouseDownEventKey, value); }
>         remove { RemoveEventHandler(mouseDownEventKey, value); }
>     }
>
>     // MouseUp event
>     public event MouseEventHandler MouseUp
>     {
>         add { AddEventHandler(mouseUpEventKey, value); }
>         remove { RemoveEventHandler(mouseUpEventKey, value); }
>     }
>
>     // Invoke the MouseUp event
>     protected void OnMouseUp(MouseEventArgs args)
>     {
>         MouseEventHandler handler;
>         handler = (MouseEventHandler)GetEventHandler(mouseUpEventKey);
>         if (handler != null)
>         {
>             handler(this, args);
>         }
>     }
> }
> ```
>
> the `Control` class implements an internal storage mechanism for events. The `AddEventHandler` method associates a delegate value with a key, the `GetEventHandler` method returns the delegate currently associated with a key, and the `RemoveEventHandler` method removes a delegate as an event handler for the specified event. Presumably, the underlying storage mechanism is designed such that there is no cost for associating a null delegate value with a key, and thus unhandled events consume no storage.
>
> *end example*

### 15.8.4 Static and instance events

When an event declaration includes a `static` modifier, the event is said to be a ***static event***. When no `static` modifier is present, the event is said to be an ***instance event***.

A static event is not associated with a specific instance, and it is a compile-time error to refer to `this` in the accessors of a static event.

An instance event is associated with a given instance of a class, and this instance can be accessed as `this` ([§12.8.14](expressions.md#12814-this-access)) in the accessors of that event.

The differences between static and instance members are discussed further in [§15.3.8](classes.md#1538-static-and-instance-members).

### 15.8.5 Virtual, sealed, override, and abstract accessors

A virtual event declaration specifies that the accessors of that event are virtual. The `virtual` modifier applies to both accessors of an event.

An abstract event declaration specifies that the accessors of the event are virtual, but does not provide an actual implementation of the accessors. Instead, non-abstract derived classes are required to provide their own implementation for the accessors by overriding the event. Because an accessor for an abstract event declaration provides no actual implementation, it shall not provide *event_accessor_declaration*s.

An event declaration that includes both the `abstract` and `override` modifiers specifies that the event is abstract and overrides a base event. The accessors of such an event are also abstract.

Abstract event declarations are only permitted in abstract classes ([§15.2.2.2](classes.md#15222-abstract-classes)) and interfaces ([§19.4.5](interfaces.md#1945-interface-events)).

The accessors of an inherited virtual event can be overridden in a derived class by including an event declaration that specifies an `override` modifier. This is known as an ***overriding event declaration***. An overriding event declaration does not declare a new event. Instead, it simply specializes the implementations of the accessors of an existing virtual event.

An overriding event declaration shall specify the exact same accessibility modifiers and name as the overridden event, there shall be an identity conversion between the type of the overriding and the overridden event, and both the add and remove accessors shall be specified within the declaration.

An overriding event declaration can include the `sealed` modifier. Use of `this` modifier prevents a derived class from further overriding the event. The accessors of a sealed event are also sealed.

It is a compile-time error for an overriding event declaration to include a `new` modifier.

Except for differences in declaration and invocation syntax, virtual, sealed, override, and abstract accessors behave exactly like virtual, sealed, override and abstract methods. Specifically, the rules described in [§15.6.4](classes.md#1564-virtual-methods), [§15.6.5](classes.md#1565-override-methods), [§15.6.6](classes.md#1566-sealed-methods), and [§15.6.7](classes.md#1567-abstract-methods) apply as if accessors were methods of a corresponding form. Each accessor corresponds to a method with a single value parameter of the event type, a `void` return type, and the same modifiers as the containing event.

## 15.9 Indexers

### 15.9.1 General

An ***indexer*** is a member that enables an object to be indexed in the same way as an array. Indexers are declared using *indexer_declaration*s:

```ANTLR
indexer_declaration
    : attributes? indexer_modifier* indexer_declarator indexer_body
    | attributes? indexer_modifier* ref_kind indexer_declarator ref_indexer_body
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
    | 'readonly'        // struct members only
    | unsafe_modifier   // unsafe code support
    ;

indexer_declarator
    : type 'this' '[' parameter_list ']'
    | type interface_type '.' 'this' '[' parameter_list ']'
    ;

indexer_body
    : '{' accessor_declarations '}' 
    | '=>' expression ';'
    ;  

ref_indexer_body
    : '{' ref_get_accessor_declaration '}'
    | '=>' 'ref' variable_reference ';'
    ;
```

*unsafe_modifier* ([§24.2](unsafe-code.md#242-unsafe-contexts)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).

An *indexer_declaration* may include a set of *attributes* ([§23](attributes.md#23-attributes)) and any one of the permitted kinds of declared accessibility ([§15.3.6](classes.md#1536-access-modifiers)), the `new` ([§15.3.5](classes.md#1535-the-new-modifier)), `virtual` ([§15.6.4](classes.md#1564-virtual-methods)), `override` ([§15.6.5](classes.md#1565-override-methods)), `sealed` ([§15.6.6](classes.md#1566-sealed-methods)), `abstract` ([§15.6.7](classes.md#1567-abstract-methods)) and `extern` ([§15.6.8](classes.md#1568-external-methods)) modifiers. Additionally an *indexer_declaration* that is contained directly by a *struct_declaration* may include the `readonly` modifier ([§16.6.12](structs.md#16612-methods)).

- The first declares a non-ref-valued indexer. Its value has type *type*. This kind of indexer may be readable and/or writeable.
- The second declares a ref-valued indexer. Its value is a *variable_reference* ([§9.5](variables.md#95-variable-references)), that may be `readonly`, to a variable of type *type*. This kind of indexer is only readable.

An *indexer_declaration* may include a set of *attributes* ([§23](attributes.md#23-attributes)) and any one of the permitted kinds of declared accessibility ([§15.3.6](classes.md#1536-access-modifiers)), the `new` ([§15.3.5](classes.md#1535-the-new-modifier)), `virtual` ([§15.6.4](classes.md#1564-virtual-methods)), `override` ([§15.6.5](classes.md#1565-override-methods)), `sealed` ([§15.6.6](classes.md#1566-sealed-methods)), `abstract` ([§15.6.7](classes.md#1567-abstract-methods)), and `extern` ([§15.6.8](classes.md#1568-external-methods)) modifiers.

Indexer declarations are subject to the same rules as method declarations ([§15.6](classes.md#156-methods)) with regard to valid combinations of modifiers, with the one exception being that the `static` modifier is not permitted on an indexer declaration.

The *type* of an indexer declaration specifies the element type of the indexer introduced by the declaration.

> *Note*: As indexers are designed to be used in array element-like contexts, the term *element type* as defined for an array is also used with an indexer. *end note*

Unless the indexer is an explicit interface member implementation, the *type* is followed by the keyword `this`. For an explicit interface member implementation, the *type* is followed by an *interface_type*, a “`.`”, and the keyword `this`. Unlike other members, indexers do not have user-defined names.

The *parameter_list* specifies the parameters of the indexer. The parameter list of an indexer corresponds to that of a method ([§15.6.2](classes.md#1562-method-parameters)), except that at least one parameter shall be specified, and that the `this`, `ref`, and `out` parameter modifiers are not permitted.

The *type* of an indexer and each of the types referenced in the *parameter_list* shall be at least as accessible as the indexer itself ([§7.5.5](basic-concepts.md#755-accessibility-constraints)).

An *indexer_body* may either consist of a statement body ([§15.7.1](classes.md#1571-general)) or an expression body ([§15.6.1](classes.md#1561-general)). In a statement body, *accessor_declarations*, which shall be enclosed in “`{`” and “`}`” tokens, declare the accessors ([§15.7.3](classes.md#1573-accessors)) of the indexer. The accessors specify the executable statements associated with reading and writing indexer elements.
  
In an *indexer_body* an expression body consisting of “`=>`” followed by an expression `E` and a semicolon is exactly equivalent to the statement body `{ get { return E; } }`, and can therefore only be used to specify read-only indexers where the result of the get accessor is given by a single expression.

A *ref_indexer_body* may either consist of a statement body or an expression body. In a statement body a *get_accessor_declaration* declares the get accessor ([§15.7.3](classes.md#1573-accessors)) of the indexer. The accessor specifies the executable statements associated with reading the indexer.

In a *ref_indexer_body* an expression body consisting of `=>` followed by `ref`, a *variable_reference* `V` and a semicolon is exactly equivalent to the statement body `{ get { return ref V; } }`.

> *Note*: Even though the syntax for accessing an indexer element is the same as that for an array element, an indexer element is not classified as a variable. Thus, it is not possible to pass an indexer element as an `in`, `out`, or `ref` argument unless the indexer is ref-valued and therefore returns a reference ([§9.7](variables.md#97-reference-variables-and-returns)). *end note*

The *parameter_list* of an indexer defines the signature ([§7.6](basic-concepts.md#76-signatures-and-overloading)) of the indexer. Specifically, the signature of an indexer consists of the number and types of its parameters. The element type and names of the parameters are not part of an indexer’s signature.

The signature of an indexer shall differ from the signatures of all other indexers declared in the same class.

When an indexer declaration includes an `extern` modifier, the indexer is said to be an ***external indexer***. Because an external indexer declaration provides no actual implementation, each of the *accessor_body*s in its *accessor_declarations* shall be a semicolon.

> *Example*: The example below declares a `BitArray` class that implements an indexer for accessing the individual bits in the bit array.
>
> <!-- Example: {template:"standalone-lib", name:"Indexers1"} -->
> <!-- Maintenance Note: A version of this type exists in additional-files as "MyBitArray.cs". As such, certain changes to this type definition might need to be reflected in that file, in which case, *all* examples using that file should be tested. -->
> ```csharp
> class BitArray
> {
>     int[] bits;
>     int length;
>
>     public BitArray(int length)
>     {
>         if (length < 0)
>         {
>             throw new ArgumentException();
>         }
>         bits = new int[((length - 1) >> 5) + 1];
>         this.length = length;
>     }
>
>     public int Length => length;
>
>     public bool this[int index]
>     {
>         get
>         {
>             if (index < 0 || index >= length)
>             {
>                 throw new IndexOutOfRangeException();
>             }
>             return (bits[index >> 5] & 1 << index) != 0;
>         }
>         set
>         {
>             if (index < 0 || index >= length)
>             {
>                 throw new IndexOutOfRangeException();
>             }
>             if (value)
>             {
>                 bits[index >> 5] |= 1 << index;
>             }
>             else
>             {
>                 bits[index >> 5] &= ~(1 << index);
>             }
>         }
>     }
> }
> ```
>
> An instance of the `BitArray` class consumes substantially less memory than a corresponding `bool[]` (since each value of the former occupies only one bit instead of the latter’s one `byte`), but it permits the same operations as a `bool[]`.
>
> The following `CountPrimes` class uses a `BitArray` and the classical “sieve” algorithm to compute the number of primes between 2 and a given maximum:
>
> <!-- Example: {template:"standalone-console", name:"Indexers2", additionalFiles:["MyBitArray.cs"], expectedOutput:["Found 6 primes between 2 and 13"], executionArgs:["13"]} -->
> ```csharp
> class CountPrimes
> {
>     static int Count(int max)
>     {
>         BitArray flags = new BitArray(max + 1);
>         int count = 0;
>         for (int i = 2; i <= max; i++)
>         {
>             if (!flags[i])
>             {
>                 for (int j = i * 2; j <= max; j += i)
>                 {
>                     flags[j] = true;
>                 }
>                 count++;
>             }
>         }
>         return count;
>     }
> 
>     static void Main(string[] args)
>     {
>         int max = int.Parse(args[0]);
>         int count = Count(max);
>         Console.WriteLine($"Found {count} primes between 2 and {max}");
>     }
> }
> ```
>
> Note that the syntax for accessing elements of the `BitArray` is precisely the same as for a `bool[]`.
>
> The following example shows a 26×10 grid class that has an indexer with two parameters. The first parameter is required to be an upper- or lowercase letter in the range A–Z, and the second is required to be an integer in the range 0–9.
>
> <!-- Example: {template:"standalone-lib", name:"Indexers3"} -->
> ```csharp
> class Grid
> {
>     const int NumRows = 26;
>     const int NumCols = 10;
>     int[,] cells = new int[NumRows, NumCols];
>
>     public int this[char row, int col]
>     {
>         get
>         {
>             row = Char.ToUpper(row);
>             if (row < 'A' || row > 'Z')
>             {
>                 throw new ArgumentOutOfRangeException("row");
>             }
>             if (col < 0 || col >= NumCols)
>             {
>                 throw new ArgumentOutOfRangeException ("col");
>             }
>             return cells[row - 'A', col];
>         }
>         set
>         {
>             row = Char.ToUpper(row);
>             if (row < 'A' || row > 'Z')
>             {
>                 throw new ArgumentOutOfRangeException ("row");
>             }
>             if (col < 0 || col >= NumCols)
>             {
>                 throw new ArgumentOutOfRangeException ("col");
>             }
>             cells[row - 'A', col] = value;
>         }
>     }
> }
> ```
>
> *end example*

### 15.9.2 Indexer and Property Differences

Indexers and properties are very similar in concept, but differ in the following ways:

- A property is identified by its name, whereas an indexer is identified by its signature.
- A property is accessed through a *simple_name* ([§12.8.4](expressions.md#1284-simple-names)) or a *member_access* ([§12.8.7](expressions.md#1287-member-access)), whereas an indexer element is accessed through an *element_access* ([§12.8.12.4](expressions.md#128124-indexer-access)).
- A property can be a static member, whereas an indexer is always an instance member.
- A get accessor of a property corresponds to a method with no parameters, whereas a get accessor of an indexer corresponds to a method with the same parameter list as the indexer.
- A set accessor of a property corresponds to a method with a single parameter named `value`, whereas a set accessor of an indexer corresponds to a method with the same parameter list as the indexer, plus an additional parameter named `value`.
- An init accessor of a property corresponds to a method with a single parameter named `value`, whereas an init accessor of an indexer corresponds to a method with the same formal parameter list as the indexer, plus an additional parameter named `value`.
- It is a compile-time error for an indexer accessor to declare a local variable or local constant with the same name as an indexer parameter.
- In an overriding property declaration, the inherited property is accessed using the syntax `base.P`, where `P` is the property name. In an overriding indexer declaration, the inherited indexer is accessed using the syntax `base[E]`, where `E` is a comma-separated list of expressions.
- There is no concept of an “automatically implemented indexer.” It is an error to have a non-abstract, non-external indexer with semicolon *accessor_body*s.

Aside from these differences, all rules defined in [§15.7.3](classes.md#1573-accessors), [§15.7.5](classes.md#1575-accessibility) and [§15.7.6](classes.md#1576-virtual-sealed-override-and-abstract-accessors) apply to indexer accessors as well as to property accessors.

This replacing of property/properties with indexer/indexers when reading [§15.7.3](classes.md#1573-accessors), [§15.7.5](classes.md#1575-accessibility) and [§15.7.6](classes.md#1576-virtual-sealed-override-and-abstract-accessors) applies to defined terms as well. Specifically, *read-write property* becomes ***read-write indexer***, *read-init property* becomes ***read-init indexer***, *read-only property* becomes ***read-only indexer***, and *write-only property* becomes ***write-only indexer***, and *init-only property* becomes ***init-only indexer***.

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

logical_negation_operator
    : '!'
    ;

overloadable_unary_operator
    : '+' | 'checked'? '-' | '!' | '~' | 'checked'? '++' | 'checked'? '--' | 'true' | 'false'
    ;

binary_operator_declarator
    : type 'operator' overloadable_binary_operator
        '(' fixed_parameter ',' fixed_parameter ')'
    ;

overloadable_binary_operator
    : 'checked'? '+'  | 'checked'? '-'  | 'checked'? '*'  | 'checked'? '/'  | '%'  | '&' | '|' | '^'  | '<<' 
      | right_shift | unsigned_right_shift | '==' | '!=' | '>' | '<' | '>=' | '<='
    ;

conversion_operator_declarator
    : 'implicit' 'operator' type '(' fixed_parameter ')'
    | 'explicit' 'operator' 'checked'? type '(' fixed_parameter ')'
    ;

operator_body
    : block
    | '=>' expression ';'
    | ';'
    ;
```

*unsafe_modifier* ([§24.2](unsafe-code.md#242-unsafe-contexts)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).

*Note*: The prefix logical negation ([§12.9.4](expressions.md#1294-logical-negation-operator)) and postfix null-forgiving operators ([§12.8.9](expressions.md#1289-null-forgiving-expressions)), while represented by the same lexical token (`!`), are distinct. The latter is not an overloadable operator. *end note*

There are three categories of overloadable operators: Unary operators ([§15.10.2](classes.md#15102-unary-operators)), binary operators ([§15.10.3](classes.md#15103-binary-operators)), and conversion operators ([§15.10.4](classes.md#15104-conversion-operators)).

The *operator_body* is either a semicolon, a block body ([§15.6.1](classes.md#1561-general)) or an expression body ([§15.6.1](classes.md#1561-general)). A block body consists of a *block*, which specifies the statements to execute when the operator is invoked. The *block* shall conform to the rules for value-returning methods described in [§15.6.11](classes.md#15611-method-body). An expression body consists of `=>` followed by an expression and a semicolon, and denotes a single expression to perform when the operator is invoked.

For `extern` operators, the *operator_body* consists simply of a semicolon. For all other operators, the *operator_body* is either a block body or an expression body.

The following rules apply to all operator declarations:

- An operator declaration shall include both a `public` and a `static` modifier.
- The parameters of an operator shall have no modifiers other than `in`.
- The signature of an operator ([§15.10.2](classes.md#15102-unary-operators), [§15.10.3](classes.md#15103-binary-operators), [§15.10.4](classes.md#15104-conversion-operators)) shall differ from the signatures of all other operators declared in the same class.
- All types referenced in an operator declaration shall be at least as accessible as the operator itself ([§7.5.5](basic-concepts.md#755-accessibility-constraints)).
- It is an error for the same modifier to appear multiple times in an operator declaration.

Each operator category imposes additional restrictions, as described in the following subclauses.

Like other members, operators declared in a base class are inherited by derived classes. Because operator declarations always require the class or struct in which the operator is declared to participate in the signature of the operator, it is not possible for an operator declared in a derived class to hide an operator declared in a base class. Thus, the `new` modifier is never required, and therefore never permitted, in an operator declaration.

Additional information on unary and binary operators can be found in [§12.4](expressions.md#124-operators).

Additional information on conversion operators can be found in [§10.5](conversions.md#105-user-defined-conversions).

An *operator_declaration* containing `checked` declares a ***checked operator***. For each checked operator in that type there shall be a corresponding ***regular operator*** having the same signature, but without `checked`, and return type.

> *Note*: The opposite is not required. Specifically, an *operator_declaration* without `checked` need not have a matching checked operator. When this is the case, the operator is neither a checked operator nor a regular operator. *end note*

It is suggested that a user-defined checked operator throw an exception (such as `System.OverflowException`) when the result of an operation is too large to represent in the destination type, whatever that might mean for that type.

It is suggested that a user-defined regular operator not throw an exception when the result of an operation is too large to represent in the destination type, whatever that might mean for that type. Instead, it should return an instance representing a truncated result, whatever that might mean for that type.

A checked operator does not implement a regular operator, and vice versa.
The presence of `checked` in a checked operator declaration has no effect on the checked context of that operator’s *operator_body*. That is controlled by the `checked` and `unchecked` operators or statements present in that *operator_body*, or by the default checking context provided by the runtime environment.

### 15.10.2 Unary operators

The following rules apply to unary operator declarations, where `T` denotes the instance type of the class or struct that contains the operator declaration:

- A unary `+`, `-`, `!` (logical negation only), or `~` operator shall take a single parameter of type `T` or `T?` and can return any type.
- A unary `++` or `--` operator shall take a single parameter of type `T` or `T?` and shall return that same type or a type derived from it.
- A unary `true` or `false` operator shall take a single parameter of type `T` or `T?` and shall return type `bool`.

The signature of a unary operator consists of the operator token (`+`, `-`, `!`, `~`, `++`, `--`, `true`, or `false`) and the type of the single parameter. The return type is not part of a unary operator’s signature, nor is the name of the parameter.

The `true` and `false` unary operators require pair-wise declaration. A compile-time error occurs if a class declares one of these operators without also declaring the other. The `true` and `false` operators are described further in [§12.27](expressions.md#1227-boolean-expressions).

> *Example*: The following example shows an implementation and subsequent usage of operator++ for an integer vector class:
>
> <!-- Example: {template:"standalone-console-without-using", name:"UnaryOperators", replaceEllipsis:true, customEllipsisReplacements:[null,"return 0;","return 0;",null]} -->
> ```csharp
> public class IntVector
> {
>     public IntVector(int length) {...}
>     public int Length { get { ... } }                      // Read-only property
>     public int this[int index] { get { ... } set { ... } } // Read-write indexer
>
>     public static IntVector operator++(IntVector iv)
>     {
>         IntVector temp = new IntVector(iv.Length);
>         for (int i = 0; i < iv.Length; i++)
>         {
>             temp[i] = iv[i] + 1;
>         }
>         return temp;
>     }
> }
> 
> class Test
> {
>     static void Main()
>     {
>         IntVector iv1 = new IntVector(4); // Vector of 4 x 0
>         IntVector iv2;
>         iv2 = iv1++;              // iv2 contains 4 x 0, iv1 contains 4 x 1
>         iv2 = ++iv1;              // iv2 contains 4 x 2, iv1 contains 4 x 2
>     }
> }
> ```
>
> Note how the operator method returns the value produced by adding 1 to the operand, just like the postfix increment and decrement operators ([§12.8.16](expressions.md#12816-postfix-increment-and-decrement-operators)), and the prefix increment and decrement operators ([§12.9.7](expressions.md#1297-prefix-increment-and-decrement-operators)). Unlike in C++, this method should not modify the value of its operand directly as this would violate the standard semantics of the postfix increment operator ([§12.8.16](expressions.md#12816-postfix-increment-and-decrement-operators)).
>
> *end example*

### 15.10.3 Binary operators

The following rules apply to binary operator declarations, where `T` denotes the instance type of the class or struct that contains the operator declaration:

- A binary non-shift operator shall take two parameters, at least one of which shall have type `T` or `T?`, and can return any type.
- A binary `<<`, `>>`, or `>>>` operator ([§12.14](expressions.md#1214-shift-operators)) shall take two parameters, the first of which shall have type `T` or `T?`, and can return any type.

The signature of a binary operator consists of the operator token (`+`, `-`, `*`, `/`, `%`, `&`, `|`, `^`, `<<`, `>>`, `==`, `!=`, `>`, `<`, `>=`, or `<=`) and the types of the two parameters. The return type and the names of the parameters are not part of a binary operator’s signature.

Certain binary operators require pair-wise declaration. For every declaration of either operator of a pair, there shall be a matching declaration of the other operator of the pair. Two operator declarations match if identity conversions exist between their return types and their corresponding parameter types. The following operators require pair-wise declaration:

- operator `==` and operator `!=`
- operator `>` and operator `<`
- operator `>=` and operator `<=`

### 15.10.4 Conversion operators

A conversion operator declaration introduces a ***user-defined conversion*** ([§10.5](conversions.md#105-user-defined-conversions)), which augments the pre-defined implicit and explicit conversions.

A conversion operator declaration that includes the `implicit` keyword introduces a user-defined implicit conversion. Implicit conversions can occur in a variety of situations, including function member invocations, cast expressions, and assignments. This is described further in [§10.2](conversions.md#102-implicit-conversions).

A conversion operator declaration that includes the `explicit` keyword introduces a user-defined explicit conversion. Explicit conversions can occur in cast expressions, and are described further in [§10.3](conversions.md#103-explicit-conversions).

A conversion operator converts from a source type, indicated by the parameter type of the conversion operator, to a target type, indicated by the return type of the conversion operator.

For a given source type `S` and target type `T`, if `S` or `T` are nullable value types, let `S₀` and `T₀` refer to their underlying types; otherwise, `S₀` and `T₀` are equal to `S` and `T` respectively. A class or struct is permitted to declare a conversion from a source type `S` to a target type `T` only if all of the following are true:

- `S₀` and `T₀` are different types.

- Either `S₀` or `T₀` is the instance type of the class or struct that contains the operator declaration.

- Neither `S₀` nor `T₀` is an *interface_type*.

- Excluding user-defined conversions, a conversion does not exist from `S` to `T` or from `T` to `S`.

For the purposes of these rules, any type parameters associated with `S` or `T` are considered to be unique types that have no inheritance relationship with other types, and any constraints on those type parameters are ignored.

> *Example*: In the following:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ConversionOperators1", replaceEllipsis:true, customEllipsisReplacements:[null,"return default;","return default;","return default;"], expectedErrors:["CS0553"]} -->
> ```csharp
> class C<T> {...}
>
> class D<T> : C<T>
> {
>     public static implicit operator C<int>(D<T> value) {...}     // Ok
>     public static implicit operator C<string>(D<T> value) {...}  // Ok
>     public static implicit operator C<T>(D<T> value) {...}       // Error
> }
> ```
>
> the first two operator declarations are permitted because `T` and `int` and `string`, respectively are considered unique types with no relationship. However, the third operator is an error because `C<T>` is the base class of `D<T>`.
>
> *end example*

From the second rule, it follows that a conversion operator shall convert either to or from the class or struct type in which the operator is declared.

> *Example*: It is possible for a class or struct type `C` to define a conversion from `C` to `int` and from `int` to `C`, but not from `int` to `bool`. *end example*

It is not possible to directly redefine a pre-defined conversion. Thus, conversion operators are not allowed to convert from or to `object` because implicit and explicit conversions already exist between `object` and all other types. Likewise, neither the source nor the target types of a conversion can be a base type of the other, since a conversion would then already exist. However, it *is* possible to declare operators on generic types that, for particular type arguments, specify conversions that already exist as pre-defined conversions.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ConversionOperators2", replaceEllipsis:true, customEllipsisReplacements:["return default;","return default;"]} -->
> ```csharp
> struct Convertible<T>
> {
>     public static implicit operator Convertible<T>(T value) {...}
>     public static explicit operator T(Convertible<T> value) {...}
> }
> ```
>
> when type `object` is specified as a type argument for `T`, the second operator declares a conversion that already exists (an implicit, and therefore also an explicit, conversion exists from any type to type object).
>
> *end example*

In cases where a pre-defined conversion exists between two types, any user-defined conversions between those types are ignored. Specifically:

- If a pre-defined implicit conversion ([§10.2](conversions.md#102-implicit-conversions)) exists from type `S` to type `T`, all user-defined conversions (implicit or explicit) from `S` to `T` are ignored.
- If a pre-defined explicit conversion ([§10.3](conversions.md#103-explicit-conversions)) exists from type `S` to type `T`, any user-defined explicit conversions from `S` to `T` are ignored. Furthermore:
  - If either `S` or `T` is an interface type, user-defined implicit conversions from `S` to `T` are ignored.
  - Otherwise, user-defined implicit conversions from `S` to `T` are still considered.

For all types but `object`, the operators declared by the `Convertible<T>` type above do not conflict with pre-defined conversions.

> *Example*:
>
> <!-- Example: {template:"standalone-console-without-using", name:"ConversionOperators3", expectedErrors:["CS0266"], expectedWarnings:["CS8321"], additionalFiles:["ConvertibleT.cs"]} -->
> ```csharp
> void F(int i, Convertible<int> n)
> {
>     i = n;                    // Error
>     i = (int)n;               // User-defined explicit conversion
>     n = i;                    // User-defined implicit conversion
>     n = (Convertible<int>)i;  // User-defined implicit conversion
> }
> ```
>
> However, for type `object`, pre-defined conversions hide the user-defined conversions in all cases but one:
>
> <!-- Example: {template:"standalone-console-without-using", name:"ConversionOperators4", expectedWarnings:["CS8321"], additionalFiles:["ConvertibleT.cs"]} -->
> ```csharp
> void F(object o, Convertible<object> n)
> {
>     o = n;                       // Pre-defined boxing conversion
>     o = (object)n;               // Pre-defined boxing conversion
>     n = o;                       // User-defined implicit conversion
>     n = (Convertible<object>)o;  // Pre-defined unboxing conversion
> }
> ```
>
> *end example*

User-defined conversions are not allowed to convert from or to *interface_type*s. In particular, this restriction ensures that no user-defined transformations occur when converting to an *interface_type*, and that a conversion to an *interface_type* succeeds only if the `object` being converted actually implements the specified *interface_type*.

The signature of a conversion operator consists of the source type and the target type. (This is the only form of member for which the return type participates in the signature.) To allow a type to have both checked and regular operators with the same source- and target-type combination, the signature of a checked conversion operator also includes the fact that it is checked. The implicit or explicit classification of a conversion operator is not part of the operator’s signature. Thus, a class or struct cannot declare both an implicit and an explicit conversion operator with the same source and target types.

> *Note*: In general, user-defined implicit conversions should be designed to never throw exceptions and never lose information. If a user-defined conversion can give rise to exceptions (for example, because the source argument is out of range) or loss of information (such as discarding high-order bits), then that conversion should be defined as an explicit conversion. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib", name:"ConversionOperators5"} -->
> ```csharp
> public struct Digit
> {
>     byte value;
>
>     public Digit(byte value)
>     {
>         if (value < 0 || value > 9)
>         {
>             throw new ArgumentException();
>         }
>         this.value = value;
>     }
>
>     public static implicit operator byte(Digit d) => d.value;
>     public static explicit operator Digit(byte b) => new Digit(b);
> }
> ```
>
> the conversion from `Digit` to `byte` is implicit because it never throws exceptions or loses information, but the conversion from `byte` to `Digit` is explicit since `Digit` can only represent a subset of the possible values of a `byte`.
>
> *end example*

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
    : identifier '(' parameter_list? ')' constructor_initializer?
    ;

constructor_initializer
    : ':' 'base' '(' argument_list? ')'
    | ':' 'this' '(' argument_list? ')'
    ;

constructor_body
    : block
    | '=>' expression ';'
    | ';'
    ;
```

*unsafe_modifier* ([§24.2](unsafe-code.md#242-unsafe-contexts)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).

A *constructor_declaration* may include a set of *attributes* ([§23](attributes.md#23-attributes)),  any one of the permitted kinds of declared accessibility ([§15.3.6](classes.md#1536-access-modifiers)), and an `extern` ([§15.6.8](classes.md#1568-external-methods)) modifier. A constructor declaration is not permitted to include the same modifier multiple times.

The *identifier* of a *constructor_declarator* shall name the class in which the instance constructor is declared. If any other name is specified, a compile-time error occurs.

The optional *parameter_list* of an instance constructor is subject to the same rules as the *parameter_list* of a method ([§15.6](classes.md#156-methods)). As the `this` modifier for parameters only applies to extension methods ([§15.6.10](classes.md#15610-extension-methods)), no parameter in a constructor’s *parameter_list* shall contain the `this` modifier. The parameter list defines the signature ([§7.6](basic-concepts.md#76-signatures-and-overloading)) of an instance constructor and governs the process whereby overload resolution ([§12.6.4](expressions.md#1264-overload-resolution)) selects a particular instance constructor in an invocation.

Each of the types referenced in the *parameter_list* of an instance constructor shall be at least as accessible as the constructor itself ([§7.5.5](basic-concepts.md#755-accessibility-constraints)).

The optional *constructor_initializer* specifies another instance constructor to invoke before executing the statements given in the *constructor_body* of this instance constructor. This is described further in [§15.11.2](classes.md#15112-constructor-initializers).

All instance constructors on a type that has a required member list ([§15.7.1](classes.md#1571-general)) automatically advertise a contract that consumers of the type shall initialize all of the properties in the list. It is an error for an instance constructor to advertise a contract that requires a member that is not at least as accessible as the constructor itself.
An instance constructor whose *constructor_initializer* chains to another constructor decorated with the `SetsRequiredMembers` attribute ([§23.5.11.1](attributes.md#235111-the-setsrequiredmembers-attribute)), shall also be decorated with that attribute.
For every instance constructor `Ci` in type `T` with required members `R`, unless `Ci` is decorated with the attribute `SetsRequiredMembers`consumers, calling `Ci` shall involve one of the following:

- Set all members of `R` in an *object_initializer* on the *object_creation_expression*,
- Or set all members of `R` via the *named_argument_list* of an *attribute*.

If the current context does not permit an *object_initializer* or is not an *attribute* with *named_argument_list*, and `Ci` is not decorated with `SetsRequiredMembers`, then it is an error to call `Ci`.

When a constructor declaration includes an `extern` modifier, the constructor is said to be an ***external constructor***. Because an external constructor declaration provides no actual implementation, its *constructor_body* consists of a semicolon. For all other constructors, the *constructor_body* consists of either

- a *block*, which specifies the statements to initialize a new instance of the class; or
- an expression body, which consists of `=>` followed by an *expression* and a semicolon, and denotes a single expression to initialize a new instance of the class.

A *constructor_body* that is a *block* or expression body corresponds exactly to the *block* of an instance method with a `void` return type ([§15.6.11](classes.md#15611-method-body)).

Instance constructors are not inherited. Thus, a class has no instance constructors other than those actually declared in the class, with the exception that if a class contains no instance constructor declarations, a default instance constructor is automatically provided ([§15.11.5](classes.md#15115-default-constructors)).

Instance constructors are invoked by *object_creation_expression*s ([§12.8.17.2](expressions.md#128172-object-creation-expressions)) and through *constructor_initializer*s.

### 15.11.2 Constructor initializers

All instance constructors (except those for class `object`) implicitly include an invocation of another instance constructor immediately before the *constructor_body*. The constructor to implicitly invoke is determined by the *constructor_initializer*:

- An instance constructor initializer of the form `base(`*argument_list*`)` (where *argument_list* is optional) causes an instance constructor from the direct base class to be invoked. That constructor is selected using *argument_list* and the overload resolution rules of [§12.6.4](expressions.md#1264-overload-resolution). The set of candidate instance constructors consists of all the accessible instance constructors of the direct base class. If this set is empty, or if a single best instance constructor cannot be identified, a compile-time error occurs.
- An instance constructor initializer of the form `this(`*argument_list*`)` (where *argument_list* is optional) invokes another instance constructor from the same class. The constructor is selected using *argument_list* and the overload resolution rules of [§12.6.4](expressions.md#1264-overload-resolution). The set of candidate instance constructors consists of all instance constructors declared in the class itself. If the resulting set of applicable instance constructors is empty, or if a single best instance constructor cannot be identified, a compile-time error occurs. If an instance constructor declaration invokes itself through a chain of one or more constructor initializers, a compile-time error occurs.

If an instance constructor has no constructor initializer, a constructor initializer of the form `base()` is implicitly provided.

> *Note*: Thus, an instance constructor declaration of the form
>
> ```csharp
> C(...) {...}
> ```
>
> is exactly equivalent to
>
> ```csharp
> C(...) : base() {...}
> ```
>
> *end note*

The scope of the parameters given by the *parameter_list* of an instance constructor declaration includes the constructor initializer of that declaration. Thus, a constructor initializer is permitted to access the parameters of the constructor.

> *Example*:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ConstructorInitializers"} -->
> ```csharp
> class A
> {
>     public A(int x, int y) {}
> }
> 
> class B: A
> {
>     public B(int x, int y) : base(x + y, x - y) {}
> }
> ```
>
> *end example*

An instance constructor initializer cannot access the instance being created. Therefore it is a compile-time error to reference this in an argument expression of the constructor initializer, as it is a compile-time error for an argument expression to reference any instance member through a *simple_name*.

### 15.11.3 Instance variable initializers

When a non-extern class instance constructor has no constructor initializer, or it has a constructor initializer of the form `base(...)`, that constructor implicitly performs the initializations specified by the *variable_initializer*s of the instance fields declared in its class. This corresponds to a sequence of assignments that are executed immediately upon entry to the constructor and before the implicit invocation of the direct base class constructor. The variable initializers are executed in the textual order in which they appear in the class declaration ([§15.5.6](classes.md#1556-variable-initializers)).

When a struct instance constructor has no constructor initializer, that constructor implicitly performs the initializations specified by the *variable_initializer*s of the instance fields declared in its struct. This corresponds to a sequence of assignments that are executed immediately upon entry to the constructor.

When a struct instance constructor has a `this()` constructor initializer that represents the *default parameterless constructor*, the declared constructor implicitly clears all instance fields and performs the initializations specified by the *variable_initializer*s of the instance fields declared in its struct. Immediately upon entry to the constructor, all value type fields are set to their default value and all reference type fields are set to `null`. Immediately after that, a sequence of assignments corresponding to the *variable_initializer*s are executed.

Variable initializers are not required to be executed by extern instance constructors.

### 15.11.4 Constructor execution

Variable initializers are transformed into assignment statements, and these assignment statements are executed *before* the invocation of the base class instance constructor. This ordering ensures that all instance fields are initialized by their variable initializers before *any* statements that have access to that instance are executed.

> *Example*: Given the following:
>
> <!-- Example: {template:"standalone-lib", name:"ConstructorExecution1"} -->
> ```csharp
> class A
> {
>     public A()
>     {
>         PrintFields();
>     }
>
>     public virtual void PrintFields() {}
> }
> class B: A
> {
>     int x = 1;
>     int y;
>
>     public B()
>     {
>         y = -1;
>     }
>
>     public override void PrintFields() =>
>         Console.WriteLine($"x = {x}, y = {y}");
> }
> ```
>
> when new `B()` is used to create an instance of `B`, the following output is produced:
>
> ```console
> x = 1, y = 0
> ```
>
> The value of `x` is 1 because the variable initializer is executed before the base class instance constructor is invoked. However, the value of `y` is 0 (the default value of an `int`) because the assignment to `y` is not executed until after the base class constructor returns.
> It is useful to think of instance variable initializers and constructor initializers as statements that are automatically inserted before the *constructor_body*. The example
>
> <!-- Example: {template:"standalone-lib", name:"ConstructorExecution2", ignoredWarnings:["CS0414","CS0414"]} -->
> ```csharp
> class A
> {
>     int x = 1, y = -1, count;
>
>     public A()
>     {
>         count = 0;
>     }
>
>     public A(int n)
>     {
>         count = n;
>     }
> }
>
> class B : A
> {
>     double sqrt2 = Math.Sqrt(2.0);
>     ArrayList items = new ArrayList(100);
>     int max;
>
>     public B(): this(100)
>     {
>         items.Add("default");
>     }
>
>     public B(int n) : base(n - 1)
>     {
>         max = n;
>     }
> }
> ```
>
> contains several variable initializers; it also contains constructor initializers of both forms (`base` and `this`). The example corresponds to the code shown below, where each comment indicates an automatically inserted statement (the syntax used for the automatically inserted constructor invocations is not valid, but merely serves to illustrate the mechanism).
>
> ```csharp
> class A
> {
>     int x, y, count;
>     public A()
>     {
>         x = 1;      // Variable initializer
>         y = -1;     // Variable initializer
>         object();   // Invoke object() constructor
>         count = 0;
>     }
>
>     public A(int n)
>     {
>         x = 1;      // Variable initializer
>         y = -1;     // Variable initializer
>         object();   // Invoke object() constructor
>         count = n;
>     }
> }
>
> class B : A
> {
>     double sqrt2;
>     ArrayList items;
>     int max;
>     public B() : this(100)
>     {
>         B(100);                      // Invoke B(int) constructor
>         items.Add("default");
>     }
>
>     public B(int n) : base(n - 1)
>     {
>         sqrt2 = Math.Sqrt(2.0);      // Variable initializer
>         items = new ArrayList(100);  // Variable initializer
>         A(n - 1);                    // Invoke A(int) constructor
>         max = n;
>     }
> }
> ```
>
> *end example*

### 15.11.5 Default constructors

If a class contains no instance constructor declarations, a default instance constructor is automatically provided. That default constructor simply invokes a constructor of the direct base class, as if it had a constructor initializer of the form `base()`. If the class is abstract then the declared accessibility for the default constructor is protected. Otherwise, the declared accessibility for the default constructor is public.

> *Note*: Thus, the default constructor is always of the form
>
> ```csharp
> protected C(): base() {}
> ```
>
> or
>
> ```csharp
> public C(): base() {}
> ```
>
> where `C` is the name of the class.
>
> *end note*

If overload resolution is unable to determine a unique best candidate for the base-class constructor initializer then a compile-time error occurs.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"DefaultConstructors3", ignoredWarnings:["CS0169"]} -->
> ```csharp
> class Message
> {
>     object sender;
>     string text;
> }
> ```
>
> a default constructor is provided because the class contains no instance constructor declarations. Thus, the example is precisely equivalent to
>
> <!-- Example: {template:"standalone-lib-without-using", name:"DefaultConstructors4", ignoredWarnings:["CS0169"]} -->
> ```csharp
> class Message
> {
>     object sender;
>     string text;
>
>     public Message() : base() {}
> }
> ```
>
> *end example*

### 15.11.6 Primary constructors

For a class type with a *delimited_parameter_list* the implementation shall provide a public constructor whose signature corresponds to the value parameters, if any, of the type declaration. This constructor is called the ***primary constructor*** for that type, and causes the implicitly declared default constructor, to be suppressed. It is an error to have a primary constructor and an explicit constructor with the same signature in the type. If the type declaration does not include a *delimited_parameter_list*, no primary constructor is provided.

Consider the following:

<!-- Example: {template:"standalone-console", name:"NonRecordClassPrimaryConstructor", inferOutput:true} -->
```csharp
public class Person(string FirstName, string LastName)
{
    public string? Title { get; set; }
    public Person(string title, string fName, string lName) : this(fName, lName)
    {
        Title = title;
    }
    public override string ToString()
    {
        return (Title != null ? Title + " " : "") + FirstName + " " + LastName;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine(new Person("Jane", "Wilson");
        Console.WriteLine(new Person("Dr.", "Jane", "Wilson");
    }
}
```

The output produced is:

```console
Jane Wilson
Dr. Jane Wilson
```

Based on the class’s *delimited_parameter_list*, a primary constructor with the following signature is provided (the parameter names are for expository purposes only):

```csharp
public Person(string firstName, string lastName);
```

As shown, the *constructor_initializer* of the explicit constructor is a call to the primary constructor, as is required by all user-defined constructors.

At runtime the primary constructor

1. Stores the value of each parameter in some unspecified manner.
1. Executes the instance initializers appearing in *class_body*.
1. Invokes the base record class constructor with the arguments provided in the *record_base* clause, if present.

Each reference to a parameter in user code is replaced with a reference to the corresponding storage place.

It is an error to reference a primary constructor parameter if the reference does not occur within one of the following:

- a `nameof` argument.
- an initializer of an instance field, property or event of the declaring type.
- the `argument_list` of `class_base` of the declaring type.
- the body of an instance method of the declaring type.
- the body of an instance accessor of the declaring type.

In other words, primary constructor parameters are in scope throughout the declaring type body. They shadow members of the declaring type within an initializer of a field, property or event of the declaring type, or within the `argument_list` of `class_base` of the declaring type. They are shadowed by members of the declaring type everywhere else. Thus, in the following declaration:

```csharp
class C(int i)
{
    protected int i = i;
    public int I => i;
}
```

the initializer for the field `i` references the parameter `i`, whereas the body of the property `I` references the field `i`.

A warning shall be produced if a parameter of the primary constructor is not read.

Expression variables declared in *argument_list* are in scope within the *argument_list*. The same shadowing rules as within an argument list of a regular *constructor_initializer* apply.

All instance member initializers in *class_body* become assignments in the primary constructor.

A warning shall be issued on the usage of an identifier when a base member shadows a primary constructor parameter if that primary constructor parameter was not passed to the base type via its constructor.

A primary constructor parameter is considered to be passed to the base type via its constructor when all the following conditions are true for an argument in *class_base*:

- The argument represents an implicit or explicit identity conversion of a primary constructor parameter;
- The argument is not part of an expanded `params` argument;

If the class being declared has a *class_base* containing *base_argument_list*, the primary constructor shall have a *constructor_initializer* of the form `: base (` … `)` that corresponds to the *class_base*’s *delimited_parameter_list*, if any.

A parameter in a *delimited_parameter_list* can be declared `ref`, `in`, or `out`.

If a primary constructor parameter is referenced from within an instance member, and the reference is not a `nameof` argument, it shall be captured into the state of the enclosing type, so that it remains accessible after the termination of the constructor. Access to captured parameters within a readonly member have similar restrictions as access to instance fields in the same context.
Capturing is not permitted for a parameter that has ref-like type, and capturing is not permitted for ref, in, or out parameters.
A warning shall be issued for primary constructor parameters under the following circumstances:

- For a by-value parameter, if the parameter is not captured and is not read within any instance initializers or base initializer.
- For an in parameter, if the parameter is not read within any instance initializers or base initializer.
- For a ref parameter, if the parameter is not read or written to within any instance initializers or base initializer.
A *class_declaration* may have a `method:` attribute target, which shall apply to the corresponding primary constructor, if any. If the class has no *delimited_parameter_list*, this attribute shall be ignored and a warning produced.

## 15.12 Static constructors

A ***static constructor*** is a member that implements the actions required to initialize a closed class. Static constructors are declared using *static_constructor_declaration*s:

```ANTLR
static_constructor_declaration
    : attributes? static_constructor_modifiers identifier '(' ')'
        static_constructor_body
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
    | '=>' expression ';'
    | ';'
    ;
```

*unsafe_modifier* ([§24.2](unsafe-code.md#242-unsafe-contexts)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).

A *static_constructor_declaration* may include a set of *attributes* ([§23](attributes.md#23-attributes)) and an `extern` modifier ([§15.6.8](classes.md#1568-external-methods)).

The *identifier* of a *static_constructor_declaration* shall name the class in which the static constructor is declared. If any other name is specified, a compile-time error occurs.

When a static constructor declaration includes an `extern` modifier, the static constructor is said to be an ***external static constructor***. Because an external static constructor declaration provides no actual implementation, its *static_constructor_body* consists of a semicolon. For all other static constructor declarations, the *static_constructor_body* consists of either

- a *block*, which specifies the statements to execute in order to initialize the class; or
- an expression body, which consists of `=>` followed by an *expression* and a semicolon, and denotes a single expression to execute in order to initialize the class.

A *static_constructor_body* that is a *block* or expression body corresponds exactly to the *method_body* of a static method with a `void` return type ([§15.6.11](classes.md#15611-method-body)).

Static constructors are not inherited, and cannot be called directly.

The static constructor for a closed class executes at most once in a given application domain. The execution of a static constructor is triggered by the first of the following events to occur within an application domain:

- An instance of the class is created.
- Any of the static members of the class are referenced.

If a class contains the `Main` method ([§7.1](basic-concepts.md#71-application-startup)) in which execution begins, the static constructor for that class executes before the `Main` method is called.

To initialize a new closed class type, first a new set of static fields ([§15.5.2](classes.md#1552-static-and-instance-fields)) for that particular closed type shall be created. Each of the static fields shall be initialized to its default value ([§15.5.5](classes.md#1555-field-initialization)). Following this:

- If there is either no static constructor or a non-extern static constructor then:
  - the static field initializers ([§15.5.6.2](classes.md#15562-static-field-initialization)) shall be executed for those static fields;
  - then the non-extern static constructor, if any, shall be executed.
- Otherwise if there is an extern static constructor it shall be executed. Static variable initializers are not required to be executed by extern static constructors.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"StaticConstructors1", inferOutput:true} -->
> ```csharp
> class Test
> {
>     static void Main()
>     {
>         A.F();
>         B.F();
>     }
> }
>
> class A
> {
>     static A()
>     {
>         Console.WriteLine("Init A");
>     }
>
>     public static void F()
>     {
>         Console.WriteLine("A.F");
>     }
> }
>
> class B
> {
>     static B()
>     {
>         Console.WriteLine("Init B");
>     }
>
>     public static void F()
>     {
>         Console.WriteLine("B.F");
>     }
> }
> ```
>
> must produce the output:
>
> ```console
> Init A
> A.F
> Init B
> B.F
> ```
>
> because the execution of `A`’s static constructor is triggered by the call to `A.F`, and the execution of `B`’s static constructor is triggered by the call to `B.F`.
>
> *end example*

It is possible to construct circular dependencies that allow static fields with variable initializers to be observed in their default value state.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"StaticConstructors2", inferOutput:true} -->
> ```csharp
> class A
> {
>     public static int X;
>
>     static A()
>     {
>         X = B.Y + 1;
>     }
> }
>
> class B
> {
>     public static int Y = A.X + 1;
>
>     static B() {}
>
>     static void Main()
>     {
>         Console.WriteLine($"X = {A.X}, Y = {B.Y}");
>     }
> }
> ```
>
> produces the output
>
> ```console
> X = 1, Y = 2
> ```
>
> To execute the `Main` method, the system first runs the initializer for `B.Y`, prior to class `B`’s static constructor. `Y`’s initializer causes `A`’s `static` constructor to be run because the value of `A.X` is referenced. The static constructor of `A` in turn proceeds to compute the value of `X`, and in doing so fetches the default value of `Y`, which is zero. `A.X` is thus initialized to 1. The process of running `A`’s static field initializers and static constructor then completes, returning to the calculation of the initial value of `Y`, the result of which becomes 2.
>
> *end example*

Because the static constructor is executed exactly once for each closed constructed class type, it is a convenient place to enforce run-time checks on the type parameter that cannot be checked at compile-time via constraints ([§15.2.5](classes.md#1525-type-parameter-constraints)).

> *Example*: The following type uses a static constructor to enforce that the type argument is an enum:
>
> <!-- Example: {template:"standalone-lib", name:"StaticConstructors3"} -->
> ```csharp
> class Gen<T> where T : struct
> {
>     static Gen()
>     {
>         if (!typeof(T).IsEnum)
>         {
>             throw new ArgumentException("T must be an enum");
>         }
>     }
> }
> ```
>
> *end example*

## 15.13 Finalizers

> *Note*: In an earlier version of this specification, what is now referred to as a “finalizer” was called a “destructor”. Experience has shown that the term “destructor” caused confusion and often resulted to incorrect expectations, especially to programmers knowing C++. In C++, a destructor is called in a determinate manner, whereas, in C#, a finalizer is not. To get determinate behavior from C#, one should use `Dispose`. *end note*

A ***finalizer*** is a member that implements the actions required to finalize an instance of a class. A finalizer is declared using a *finalizer_declaration*:

```ANTLR
finalizer_declaration
    : attributes? '~' identifier '(' ')' finalizer_body
    | attributes? 'extern' unsafe_modifier? '~' identifier '(' ')'
      finalizer_body
    | attributes? unsafe_modifier 'extern'? '~' identifier '(' ')'
      finalizer_body
    ;

finalizer_body
    : block
    | '=>' expression ';'
    | ';'
    ;
```

*unsafe_modifier* ([§24.2](unsafe-code.md#242-unsafe-contexts)) is only available in unsafe code ([§24](unsafe-code.md#24-unsafe-code)).

A *finalizer_declaration* may include a set of *attributes* ([§23](attributes.md#23-attributes)).

The *identifier* of a *finalizer_declarator* shall name the class in which the finalizer is declared. If any other name is specified, a compile-time error occurs.

When a finalizer declaration includes an `extern` modifier, the finalizer is said to be an ***external finalizer***. Because an external finalizer declaration provides no actual implementation, its *finalizer_body* consists of a semicolon. For all other finalizers, the *finalizer_body* consists of either

- a *block*, which specifies the statements to execute in order to finalize an instance of the class.
- or an expression body, which consists of `=>` followed by an *expression* and a semicolon, and denotes a single expression to execute in order to finalize an instance of the class.

A *finalizer_body* that is a *block* or expression body corresponds exactly to the *method_body* of an instance method with a `void` return type ([§15.6.11](classes.md#15611-method-body)).

Finalizers are not inherited. Thus, a class has no finalizers other than the one that may be declared in that class.

> *Note*: Since a finalizer is required to have no parameters, it cannot be overloaded, so a class can have, at most, one finalizer. *end note*

Finalizers are invoked automatically, and cannot be invoked explicitly. An instance becomes eligible for finalization when it is no longer possible for any code to use that instance. Execution of the finalizer for the instance may occur at any time after the instance becomes eligible for finalization ([§7.9](basic-concepts.md#79-automatic-memory-management)). When an instance is finalized, the finalizers in that instance’s inheritance chain are called, in order, from most derived to least derived. A finalizer may be executed on any thread. For further discussion of the rules that govern when and how a finalizer is executed, see [§7.9](basic-concepts.md#79-automatic-memory-management).

> *Example*: The output of the example
>
> <!-- Example: {template:"standalone-console", name:"Finalizers1", inferOutput: true} -->
> ```csharp
> class A
> {
>     ~A()
>     {
>         Console.WriteLine("A's finalizer");
>     }
> }
>
> class B : A
> {
>     ~B()
>     {
>         Console.WriteLine("B's finalizer");
>     }
> }
>
> class Test
> {
>     static void Main()
>     {
>         B b = new B();
>         b = null;
>         GC.Collect();
>         GC.WaitForPendingFinalizers();
>     }
> }
> ```
>
> is
>
> ```console
> B's finalizer
> A's finalizer
> ```
>
> since finalizers in an inheritance chain are called in order, from most derived to least derived.
>
> *end example*

Finalizers are implemented by overriding the virtual method `Finalize` on `System.Object`. C# programs are not permitted to override this method or call it (or overrides of it) directly.

> *Example*: For instance, the program
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Finalizers2", expectedErrors:["CS0249","CS0245"], expectedWarnings:["CS0465"]} -->
> ```csharp
> class A
> {
>     override protected void Finalize() {}  // Error
>     public void F()
>     {
>         this.Finalize();                   // Error
>     }
> }
> ```
>
> contains two errors.
>
> *end example*

A compiler shall behave as if this method, and overrides of it, do not exist at all.

> *Example*: Thus, this program:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"Finalizers3", expectedWarnings:["CS0465"]} -->
> ```csharp
> class A
> {
>     void Finalize() {}  // Permitted
> }
> ```
>
> is valid and the method shown hides `System.Object`’s `Finalize` method.
>
> *end example*

For a discussion of the behavior when an exception is thrown from a finalizer, see [§22.4](exceptions.md#224-how-exceptions-are-handled).

## 15.14 Async Functions

### 15.14.1 General

A method ([§15.6](classes.md#156-methods)), anonymous function ([§12.22](expressions.md#1222-anonymous-function-expressions)), or local function ([§13.6.4](statements.md#1364-local-function-declarations)) with the `async` modifier is called an ***async function***. In general, the term ***async*** is used to describe any kind of function that has the `async` modifier.

It is a compile-time error for the parameter list of an async function to specify any `in`, `out`, or `ref` parameters, or any parameter of a `ref struct` type.

The *return_type* of an async method shall be either `void`, a ***task type***, or an ***asynchronous iterator type*** ([§15.15](classes.md#1515-synchronous-and-asynchronous-iterators)). For an async method that produces a result value, a task type or an asynchronous iterator type ([§15.15.3](classes.md#15153-enumerable-interfaces)) shall be generic. For an async method that does not produce a result value, a task type shall not be generic. Such types are referred to in this specification as `«TaskType»<T>` and `«TaskType»`, respectively. The Standard library type `System.Threading.Tasks.Task` and types constructed from `System.Threading.Tasks.Task<TResult>` and `System.Threading.Tasks.ValueTask<T>` are task types, as well as a class, struct or interface type that is associated with a ***task builder type*** via the attribute `System.Runtime.CompilerServices.AsyncMethodBuilderAttribute`. Such types are referred to in this specification as `«TaskBuilderType»<T>` and `«TaskBuilderType»`. A task type can have at most one type parameter and cannot be nested in a generic type.

An async method returning a task type is said to be ***task-returning***.

Task types can vary in their exact definition, but from the language’s point of view, a task type is in one of the states *incomplete*, *succeeded*, *canceled*, or *faulted*. A *faulted* task records a pertinent exception. A *canceled* task was stopped before it completed and records the `OperationCanceledException` as the pertinent exception. A *succeeded* `«TaskType»<T>` records a result of type `T`. Task types are awaitable, and tasks can therefore be the operands of await expressions ([§12.9.9](expressions.md#1299-await-expressions)).

> *Example*: The task type `MyTask<T>` is associated with the task builder type `MyTaskMethodBuilder<T>` and the awaiter type `Awaiter<T>`:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AsyncFunctions1", replaceEllipsis:true, customEllipsisReplacements: ["return new Awaiter<T>();", "", "return default(T);"], additionalFiles:["MyTaskMethodBuilderT.cs"]} -->
> ```csharp
> using System.Runtime.CompilerServices; 
> [AsyncMethodBuilder(typeof(MyTaskMethodBuilder<>))]
> class MyTask<T>
> {
>     public Awaiter<T> GetAwaiter() { ... }
> }
>
> class Awaiter<T> : INotifyCompletion
> {
>     public void OnCompleted(Action completion) { ... }
>     public bool IsCompleted { get; }
>     public T GetResult() { ... }
> }
> ```
>
> *end example*

A task builder type is a class or struct type that corresponds to a specific task type ([§15.14.2](classes.md#15142-task-type-builder-pattern)). The task builder type shall exactly match the declared accessibility of its corresponding task type.

> *Note:* If the task type is declared `internal`, the the corresponding builder type must also be declared `internal` and be defined in the same assembly. If the task type is nested inside another type, the task builder type must also be nested in that same type. *end note*

An async function has the ability to suspend evaluation by means of await expressions ([§12.9.9](expressions.md#1299-await-expressions)) in its body. Evaluation may later be resumed at the point of the suspending await expression by means of a ***resumption delegate***. The resumption delegate is of type `System.Action`, and when it is invoked, evaluation of the async function invocation will resume from the await expression where it left off. The ***current caller*** of an async function invocation is the original caller if the function invocation has never been suspended or the most recent caller of the resumption delegate otherwise.

### 15.14.2 Task-type builder pattern

A task builder type can have at most one type parameter and cannot be nested in a generic type. A task builder type shall have the following members (for non-generic task builder types, `SetResult` has no parameters) with declared `public` accessibility:

```csharp
class «TaskBuilderType»<T>
{
    public static «TaskBuilderType»<T> Create();
    public void Start<TStateMachine>(ref TStateMachine stateMachine)
                where TStateMachine : IAsyncStateMachine;
    public void SetStateMachine(IAsyncStateMachine stateMachine);
    public void SetException(Exception exception);
    public void SetResult(T result);
    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine;
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine;
    public «TaskType»<T> Task { get; }
}
```

A compiler shall generate code that uses the «TaskBuilderType» to implement the semantics of suspending and resuming the evaluation of the async function. A compiler shall use the «TaskBuilderType» as follows:

- `«TaskBuilderType».Create()` is invoked to create an instance of the «TaskBuilderType», named `builder` in this list.
- `builder.Start(ref stateMachine)` is invoked to associate the builder with a compiler-generated state machine instance, `stateMachine`.
  - The builder shall call `stateMachine.MoveNext()` either in `Start()` or after `Start()` has returned to advance the state machine.
- After `Start()` returns, the `async` method invokes `builder.Task` for the task to return from the async method.
- Each call to `stateMachine.MoveNext()` will advance the state machine.
- If the state machine completes successfully, `builder.SetResult()` is called, with the method return value, if any.
- Otherwise, if an exception, `e` is thrown in the state machine, `builder.SetException(e)` is called.
- If the state machine reaches an `await expr` expression, `expr.GetAwaiter()` is invoked.
- If the awaiter implements `ICriticalNotifyCompletion` and `IsCompleted` is false, the state machine invokes `builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine)`.
  - `AwaitUnsafeOnCompleted()` should call `awaiter.UnsafeOnCompleted(action)` with an `Action` that calls `stateMachine.MoveNext()` when the awaiter completes.
- Otherwise, the state machine invokes `builder.AwaitOnCompleted(ref awaiter, ref stateMachine)`.
  - `AwaitOnCompleted()` should call `awaiter.OnCompleted(action)` with an `Action` that calls `stateMachine.MoveNext()` when the awaiter completes.
- `SetStateMachine(IAsyncStateMachine)` may be called by the compiler-generated `IAsyncStateMachine` implementation to identify the instance of the builder associated with a state machine instance, particularly for cases where the state machine is implemented as a value type.
  - If the builder calls `stateMachine.SetStateMachine(stateMachine)`, the `stateMachine` will call `builder.SetStateMachine(stateMachine)` on the *builder instance associated with* `stateMachine`.

> *Note*: For both `SetResult(T result)` and `«TaskType»<T> Task { get; }`, the parameter and argument respectively must be identity convertible to `T`. This allows a task-type builder to support types such as tuples, where two types that are not the same are identity convertible. *end note*

### 15.14.3 Evaluation of a task-returning async function

Invocation of a task-returning async function causes an instance of the returned task type to be generated. This is called the ***return task*** of the async function. The task is initially in an *incomplete* state.

The async function body is then evaluated until it is either suspended (by reaching an await expression) or terminates, at which point control is returned to the caller, along with the return task.

When the body of the async function terminates, the return task is moved out of the incomplete state:

- If the function body terminates as the result of reaching a return statement or the end of the body, any result value is recorded in the return task, which is put into a *succeeded* state.
- If the function body terminates because of an uncaught `OperationCanceledException`, the exception is recorded in the return task which is put into the *canceled* state.
- If the function body terminates as the result of any other uncaught exception ([§13.10.6](statements.md#13106-the-throw-statement)) the exception is recorded in the return task which is put into a *faulted* state.

### 15.14.4 Evaluation of a void-returning async function

If the return type of the async function is `void`, evaluation differs from the above in the following way: Because no task is returned, the function instead communicates completion and exceptions to the current thread’s ***synchronization context***. The exact definition of synchronization context is implementation-dependent, but is a representation of “where” the current thread is running. The synchronization context is notified when evaluation of a `void`-returning async function commences, completes successfully, or causes an uncaught exception to be thrown.

This allows the context to keep track of how many `void`-returning async functions are running under it, and to decide how to propagate exceptions coming out of them.

Rather than using a task builder type based on the *return_type* of an async function, the attribute `AsyncMethodBuilder` can be applied to that async function to indicate a different task builder type.

It is an error to apply this attribute to a lambda with an implicit return type.

The ability to provide an alternate builder type shall not be used when the synthesized entry-point for top-level statements is async ([§7.1.3](basic-concepts.md#713-using-top-level-statements)). For that, an explicit entry-point is needed.

When an async function is compiled, the builder type is determined by:

1. Using the builder type from the `AsyncMethodBuilder` attribute, if one is present;
1. Otherwise, falling back to the builder type determined by the async function’s *return_type* ([§15.14.2](classes.md#15142-task-type-builder-pattern)).

If an `AsyncMethodBuilder` attribute is present, the builder type specified by that attribute is constructed, if necessary.

If the override type is an open generic type, take the single type argument of the async function’s return type and substitute it into the override type.

If the override type is a bound generic type, then an error results.

If the async function’s return type does not have a single type argument, an error results.

To verify that the builder type is compatible with *return_type* of the async function:

1. Look for the public `Create` method with no type parameters and no parameters on the constructed builder type. It is an error if the method is not found, or if the method returns a type other than the constructed builder type.
1. Look for the public `Task` property. It is an error if the property is not found.
1. Consider the type of that `Task` property (a task-like type). It is an error if the task-like type does not match the *return_type* of the async function. (It is not necessary for *return_type* to be a task-like type.)

> *Example*: Consider the following code fragment:
>
> ```csharp
> public async ValueTask<int> ExampleAsync() { … }
> ```
>
> This will be compiled to something like the following:
>
> ```csharp
> [AsyncStateMachine(typeof(<ExampleAsync>d__29))]
> [CompilerGenerated]
> static ValueTask<int> ExampleAsync()
> {
>     <ExampleAsync>d__29 stateMachine;
>     stateMachine.<>t__builder 
>         = AsyncValueTaskMethodBuilder<int>.Create();
>     stateMachine.<>1__state = -1;
>     stateMachine.<>t__builder.Start(ref stateMachine);
>     return stateMachine.<>t__builder.Task;
> }
> ```
>
> However, the following code fragment:
>
> ```csharp
> [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
> static async ValueTask<int> ExampleAsync() { … }
> ```
>
> in which the attribute `AsyncMethodBuilder` is applied to that async function, would instead be compiled to something like:
>
> ```csharp
> [AsyncStateMachine(typeof(<ExampleAsync>d__29))]
> [CompilerGenerated]
> [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
> static ValueTask<int> ExampleAsync()
> {
>     <ExampleAsync>d__29 stateMachine;
>     stateMachine.<>t__builder 
>         = PoolingAsyncValueTaskMethodBuilder<int>.Create();
>         // <>t__builder now a different type
>     stateMachine.<>1__state = -1;
>     stateMachine.<>t__builder.Start(ref stateMachine);
>     return stateMachine.<>t__builder.Task;
> }
> ```
>
> *end example*

## 15.15 Synchronous and asynchronous iterators

### 15.15.1 General

A function member ([§12.6](expressions.md#126-function-members)) or local function ([§13.6.4](statements.md#1364-local-function-declarations)) implemented using an iterator block ([§13.3](statements.md#133-blocks)) is called an ***iterator***. An iterator block may be used as the body of a function as long as the return type of the corresponding function is one of the enumerator interfaces ([§15.15.2](classes.md#15152-enumerator-interfaces)) or one of the enumerable interfaces ([§15.15.3](classes.md#15153-enumerable-interfaces)).

An async function ([§15.14](classes.md#1514-async-functions)) or local function ([§13.6.4](statements.md#1364-local-function-declarations)) implemented using an iterator block ([§13.3](statements.md#133-blocks)) is called an ***asynchronous iterator***. An asynchronous iterator block may be used as the body of a function as long as the return type of the corresponding function is the asynchronous enumerator interface ([§15.15.2](classes.md#15152-enumerator-interfaces)) or the asynchronous enumerable interface ([§15.15.3](classes.md#15153-enumerable-interfaces)).

An iterator block may occur as a *method_body*, *operator_body* or *accessor_body*, whereas events, instance constructors, static constructors and finalizer shall not be implemented as synchronous or asynchronous iterators.

When a function is implemented using an iterator block, it is a compile-time error for the parameter list of the function to specify any `in`, `out`, or `ref` parameters, or a parameter of a `ref struct` type.

An asynchronous iterator shall support cancellation of the asynchronous operation. This is described in [§23.5.9](attributes.md#2359-the-enumeratorcancellation-attribute).

Rather than using a task builder type based on the *return_type* of an async method, the attribute `AsyncMethodBuilder` can be applied to that method to indicate a different task builder type.

It is an error to apply this attribute to a lambda with an implicit return type.

The ability to provide an alternate builder type shall not be used when the synthesized entry-point for top-level statements is async ([§7.1.3](basic-concepts.md#713-using-top-level-statements)). For that, an explicit entry-point is needed.

When an async method is compiled, the builder type is determined by:

1. Using the builder type from the `AsyncMethodBuilder` attribute, if one is present;
1. Otherwise, falling back to the builder type determined by the method’s *return_type*.

If an `AsyncMethodBuilder` attribute is present, the builder type specified by that attribute is constructed, if necessary.

If the override type is an open generic type, take the single type argument of the async method’s return type and substitute it into the override type.

If the override type is a bound generic type, then an error results.

If the async method’s return type does not have a single type argument, an error results.

To verify that the builder type is compatible with *return_type* of the async method:

1. Look for the public `Create` method with no type parameters and no parameters on the constructed builder type. It is an error if the method is not found, or if the method returns a type other than the constructed builder type.
1. Look for the public `Task` property. It is an error if the property is not found.
1. Consider the type of that `Task` property (a task-like type). It is an error if the task-like type does not match the *return_type* of the async method. (It is not necessary for *return_type* to be a task-like type.)

Consider the following code fragment:

```csharp
public async ValueTask<T> ExampleAsync() { … }
```

This will be compiled to something like the following:

```csharp
[AsyncStateMachine(typeof(<ExampleAsync>d__29))]
[CompilerGenerated]
static ValueTask<int> ExampleAsync()
{
    <ExampleAsync>d__29 stateMachine;
    stateMachine.<>t__builder
        = AsyncValueTaskMethodBuilder<int>.Create();
    stateMachine.<>1__state = -1;
    stateMachine.<>t__builder.Start(ref stateMachine);
    return stateMachine.<>t__builder.Task;
}
```

However, the following code fragment:

```csharp
[AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
static async ValueTask<int> ExampleAsync() { … }
```

in which the attribute `AsyncMethodBuilder` is applied to that method, would instead be compiled to something like:

```csharp
[AsyncStateMachine(typeof(<ExampleAsync>d__29))]
[CompilerGenerated]
[AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
static ValueTask<int> ExampleAsync()
{
    <ExampleAsync>d__29 stateMachine;
    stateMachine.<>t__builder
        = PoolingAsyncValueTaskMethodBuilder<int>.Create();
        // <>t__builder now a different type
    stateMachine.<>1__state = -1;
    stateMachine.<>t__builder.Start(ref stateMachine);
    return stateMachine.<>t__builder.Task;
}
```

### 15.15.2 Enumerator interfaces

The ***enumerator interface***s are the non-generic interface `System.Collections.IEnumerator` and the generic interface `System.Collections.Generic.IEnumerator<T>`.

The ***asynchronous enumerator interface*** is the generic interface `System.Collections.Generic.IAsyncEnumerator<T>`.

For the sake of brevity, in this subclause and its siblings these interfaces are referenced as `IEnumerator`, `IEnumerator<T>`, and `IAsyncEnumerator<T>`, respectively.

### 15.15.3 Enumerable interfaces

The ***enumerable interface***s are the non-generic interface `System.Collections.IEnumerable` and the generic interfaces `System.Collections.Generic.IEnumerable<T>`.

The ***asynchronous enumerable interface*** is the generic interface `System.Collections.Generic.IAsyncEnumerable<T>`.

For the sake of brevity, in this subclause and its siblings these interfaces are referenced as `IEnumerable`, `IEnumerable<T>`, and `IAsyncEnumerable<T>`, respectively.

### 15.15.4 Yield type

An iterator produces a sequence of values, all of the same type. This type is called the ***yield type*** of the iterator.

- The yield type of an iterator that returns `IEnumerator` or `IEnumerable` is `object`.
- The yield type of an iterator that returns an `IEnumerator<T>`, `IAsyncEnumerator<T>`, `IEnumerable<T>`, or `IAsyncEnumerable<T>` is `T`.

### 15.15.5 Enumerator objects

#### 15.15.5.1 General

When a function member or local function returning an enumerator interface type or an asynchronous enumerator interface type is implemented using an iterator block, invoking the function does not immediately execute the code in the iterator block. Instead, an ***enumerator object*** is created and returned. This object encapsulates the code specified in the iterator block, and execution of the code in the iterator block occurs when the enumerator object’s `MoveNext` or `MoveNextAsync` method is invoked. An enumerator object has the following characteristics:

- It implements `System.IDisposable`, `IEnumerator` and `IEnumerator<T>`, or `System.IAsyncDisposable` and `IAsyncEnumerator<T>`, where `T` is the yield type of the iterator.
- It is initialized with a copy of the argument values (if any) and instance value passed to the function.
- It has four potential states, **before**, **running**, **suspended**, and **after**, and is initially in the **before** state.

An enumerator object is typically an instance of a compiler-generated enumerator class that encapsulates the code in the iterator block and implements the enumerator interfaces, but other methods of implementation are possible. If an enumerator class is generated by the compiler, that class will be nested, directly or indirectly, in the class containing the function, it will have private accessibility, and it will have a name reserved for compiler use ([§6.4.3](lexical-structure.md#643-identifiers)).

An enumerator object may implement more interfaces than those specified above.

The following subclauses describe the required behavior of the member to advance the enumerator, retrieve the current value from the enumerator, and dispose of resources used by the enumerator. These are defined in the following members for synchronous and asynchronous enumerators, respectively:

- To advance the enumerator: `MoveNext` and `MoveNextAsync`.
- To retrieve the current value: `Current`.
- To dispose of resources: `Dispose` and `DisposeAsync`.

Enumerator objects do not support the `IEnumerator.Reset` method. Invoking this method causes a `System.NotSupportedException` to be thrown.

Synchronous and asynchronous iterator blocks differ in that asynchronous iterator members return task types and may be awaited.

#### 15.15.5.2 Advance the enumerator

The `MoveNext` and `MoveNextAsync` methods of an enumerator object encapsulates the code of an iterator block. Invoking the `MoveNext` or `MoveNextAsync` method executes code in the iterator block and sets the `Current` property of the enumerator object as appropriate.

`MoveNext` returns a `bool` value whose meaning is described below. `MoveNextAsync` returns a `ValueTask<bool>` ([§15.14.3](classes.md#15143-evaluation-of-a-task-returning-async-function)). The result value of the task returned from `MoveNextAsync` has the same meaning as the result value from `MoveNext`. In the following description, the actions described for `MoveNext` apply to `MoveNextAsync` with the following difference: Where stated that `MoveNext` returns `true` or `false`, `MoveNextAsync` sets its task to the *completed* state, and sets the task’s result value to the corresponding `true` or `false` value.

The precise action performed by `MoveNext` or `MoveNextAsync` depends on the state of the enumerator object when invoked:

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

> *Note*: `MoveNextAsync` is suspended if it evaluates an `await` expression that awaits a task type that has not completed. *end note*

- When a `yield return` statement is encountered ([§9.4.4.20](variables.md#94420-yield-statements)):
  - The expression given in the statement is evaluated, implicitly converted to the yield type, and assigned to the `Current` property of the enumerator object.
  - Execution of the iterator body is suspended. The values of all local variables and parameters (including `this`) are saved, as is the location of this `yield return` statement. If the `yield return` statement is within one or more `try` blocks, the associated finally blocks are *not* executed at this time.
  - The state of the enumerator object is changed to **suspended**.
  - The `MoveNext` method returns `true` to its caller, indicating that the iteration successfully advanced to the next value.
- When a `yield break` statement is encountered ([§9.4.4.20](variables.md#94420-yield-statements)):
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

#### 15.15.5.3 Retrieve the current value

An enumerator object’s `Current` property is affected by `yield return` statements in the iterator block.

 > *Note*: The `Current` property is a synchronous property for both synchronous and asynchronous iterator objects. *end note*

When an enumerator object is in the **suspended** state, the value of `Current` is the value set by the previous call to `MoveNext`. When an enumerator object is in the **before**, **running**, or **after** states, the result of accessing `Current` is unspecified.

For an iterator with a yield type other than `object`, the result of accessing `Current` through the enumerator object’s `IEnumerable` implementation corresponds to accessing `Current` through the enumerator object’s `IEnumerator<T>` implementation and casting the result to `object`.

#### 15.15.5.4 Dispose of resources

The `Dispose` or `DisposeAsync` method is used to clean up the iteration by bringing the enumerator object to the **after** state.

- If the state of the enumerator object is **before**, invoking `Dispose` changes the state to **after**.
- If the state of the enumerator object is **running**, the result of invoking `Dispose` is unspecified.
- If the state of the enumerator object is **suspended**, invoking `Dispose`:
  - Changes the state to **running**.
  - Executes any finally blocks as if the last executed `yield return` statement were a `yield break` statement. If this causes an exception to be thrown and propagated out of the iterator body, the state of the enumerator object is set to **after** and the exception is propagated to the caller of the `Dispose` method.
  - Changes the state to **after**.
- If the state of the enumerator object is **after**, invoking `Dispose` has no affect.

### 15.15.6 Enumerable objects

#### 15.15.6.1 General

When a function member or local function returning an enumerable interface type or an async enumerable interface type is implemented using an iterator block, invoking the function does not immediately execute the code in the iterator block. Instead, an ***enumerable object*** is created and returned.

A synchronous enumerable object implements `IEnumerable` and `IEnumerable<T>`, where `T` is the yield type of the iterator. Its `GetEnumerator` method returns an enumerator object ([§15.15.5](classes.md#15155-enumerator-objects)). An async enumerable object implements `IAsyncEnumerable<T>` where `T` is the yield type of the iterator. Its `GetAsyncEnumerator` method returns an asynchronous enumerator object  ([§15.15.5](classes.md#15155-enumerator-objects)).

An enumerable object is initialized with a copy of the argument values (if any) and instance value passed to the function.

An enumerable object is typically an instance of a compiler-generated enumerable class that encapsulates the code in the iterator block and implements the enumerable interfaces, but other methods of implementation are possible. If an enumerable class is generated by the compiler, that class will be nested, directly or indirectly, in the class containing the function, it will have private accessibility, and it will have a name reserved for compiler use ([§6.4.3](lexical-structure.md#643-identifiers)).

An enumerable object may implement more interfaces than those specified above.

> *Note*: For example, an enumerable object may also implement `IEnumerator` and `IEnumerator<T>`, enabling it to serve as both an enumerable and an enumerator. Typically, such an implementation would return its own instance (to save allocations) from the first call to `GetEnumerator`. Subsequent invocations of `GetEnumerator`, if any, would return a new class instance, typically of the same class, so that calls to different enumerator instances will not affect each other. *end note*

#### 15.15.6.2 The GetEnumerator or GetAsyncEnumerator method

An enumerable object provides an implementation of the `GetEnumerator` methods of the `IEnumerable` and `IEnumerable<T>` interfaces. The two `GetEnumerator` methods share a common implementation that acquires and returns an available enumerator object. The enumerator object is initialized with the argument values and instance value saved when the enumerable object was initialized, but otherwise the enumerator object functions as described in [§15.15.5](classes.md#15155-enumerator-objects).

An asynchronous enumerable object provides an implementation of the `GetAsyncEnumerator` method of the `IAsyncEnumerable<T>` interface. This method returns an available asynchronous enumerator object. The enumerator object is initialized with the argument values and instance value saved when the enumerable object was initialized, including the optional cancellation token, but otherwise the enumerator object functions as described in [§15.15.5](classes.md#15155-enumerator-objects). An asynchronous iterator method can mark one parameter as the cancellation token using `System.Runtime.CompilerServices.EnumeratorCancellationAttribute` ([§23.5.9](attributes.md#2359-the-enumeratorcancellation-attribute)). An implementation shall provide a mechanism to combine cancellation tokens such that an asynchronous iterator is canceled when either cancellation token (the argument to `GetAsyncEnumerator` or the argument attributed with the attribute `System.Runtime.CompilerServices.EnumeratorCancellationAttribute`) requests cancellation.

## 15.16 Record classes

### 15.16.1 General

A record class is a specialized reference type that is optimized for storing data rather than behavior. It provides built-in functionality that would normally require significant “boilerplate” code in a non-record class, such as value-based equality and easy immutability.

```ANTLR
record_class_declaration
    : attributes? class_modifier* 'partial'? 'record' 'class'? identifier
      type_parameter_list? delimited_parameter_list? class_base?
      type_parameter_constraints_clause* class_body
    ;
```

A *record_class_declaration* consists of an optional set of *attributes* ([§23](attributes.md#23-attributes)), followed by an optional set of *class_modifier*s ([§15.2.2](classes.md#1522-class-modifiers)), followed by an optional `partial` modifier ([§15.2.7](classes.md#1527-partial-type-declarations)), followed by the keyword `record`, optionally followed by the keyword `class`, and an *identifier* that names the class, followed by an optional *type_parameter_list* ([§15.2.3](classes.md#1523-type-parameters)), followed by an optional *delimited_parameter_list* ([§15.6.2.1](classes.md#15621-general)), followed by an optional *class_base* specification ([§15.2.4](classes.md#1524-class-base-specification)), followed by an optional set of *type_parameter_constraints_clause*s ([§15.2.5](classes.md#1525-type-parameter-constraints)), followed by a *class_body* ([§15.2.6](classes.md#1526-class-body)).

`record` and `record class` are equivalent.

*class_modifier* shall not be `static`.

A *record_class_declaration* having a *delimited_parameter_list* declares a ***positional record class***.

At most only one partial type declaration of a partial record class may provide a *delimited_parameter_list*.

Parameters in *delimited_parameter_list* shall not have `ref`, `out` or `this` modifiers; however, `in` and `params` modifiers are permitted.

### 15.16.2 Class members

It is an error for a member of a record class to be named `Clone`.

It is an error for an instance field of a record class to have an unsafe type.

### 15.16.3 Instance constructors

A positional record class ([§15.16.1](classes.md#15161-general)) has a primary constructor; see [§15.16.6.6.2](classes.md#1516662-primary-constructor) for more information.

### 15.16.4 Implicit record class members

#### 15.16.4.1 General

Certain members are provided by the implementation unless a member with a matching signature is declared in the *class_body*, or an accessible concrete, non-virtual member with a matching signature is inherited. A matching member prevents the implementation from providing that member only, not any other provided members. Two members are considered matching if they have the same signature or would be considered hiding in an inheritance scenario.

The members provided by the implementation are described in the following subclauses.

#### 15.16.4.2 Copy constructors

A ***copy constructor*** for a type `T` is a constructor having a single parameter of type `T`. The purpose of a copy constructor is to copy the state from the parameter to the new instance being created.

> *Example*: Consider the following:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"CopyConstructors1"} -->
> ```csharp
> class Person
> {
>     public int Age { get; set; }
>     public string Name { get; set; }
>     public Person(Person aPerson)
>     {
>         Name = aPerson.Name;
>         Age = aPerson.Age;
>     }
> }
> ````
>
> This declares a mutable, non-record class with two read-write properties, and a user-written copy constructor.
>
> In the following case,
>
> <!-- Example: {template:"standalone-lib-without-using", name:"CopyConstructors2"} -->
> ```csharp
> record Person(int Age, string Name);
> ````
>
> the record class is immutable. The provided auto properties `Age` and `Name` are read-init. A copy constructor is provided, as is a primary constructor. *end example*

In certain circumstances ([§15.16.6.4](classes.md#151664-copy-and-clone-members)), a copy constructor may be provided by the compiler, and called by provided code.

A copy constructor on a type that has a required member list ([§15.7.1](classes.md#1571-general)) shall be decorated with the `SetsRequiredMembers` attribute ([§23.5.11.1](attributes.md#235111-the-setsrequiredmembers-attribute)).

#### 15.16.4.3 Equality members

If a record class is derived directly from `object`, the record class type has a provided property declared as follows:

```csharp
System.Type EqualityContract { get; };
```

The property is `private` if the record class type is `sealed`. Otherwise, the property is `virtual` and `protected`. The property may be declared explicitly. It is an error if the explicit declaration does not match the expected signature or accessibility, or if the explicit declaration doesn’t allow overriding in a derived type and the record class type is not `sealed`.

If the record class type is derived from some base record class type `Base`, the record class type includes a provided property declared as follows:

```csharp
protected override System.Type EqualityContract { get; };
```

The property may be declared explicitly. It is an error if the explicit declaration does not match the expected signature or accessibility, or if the explicit declaration doesn’t allow overriding in a derived type and the record class type is not `sealed`. It is an error if either the provided or the explicitly declared property doesn’t override a property with this signature in the record class type `Base` (for example, if the property is missing in the `Base`, or is sealed, or is not virtual). The provided property returns `typeof(R)` where `R` is the record class type.

The record class type implements `System.IEquatable<R>` and includes a provided, strongly-typed overload of `Equals(R? other)` where `R` is the record class type. The method is `public`, and the method is `virtual` unless the record class type is `sealed`. The method can be declared explicitly. It is an error if the explicit declaration does not match the expected signature or accessibility, or the explicit declaration doesn’t allow overriding in a derived type and the record class type is not `sealed`.

If `Equals(R? other)` is user-defined but `GetHashCode` is not, a warning shall be issued.

```csharp
public virtual bool Equals(R? other);
```

The provided `Equals(R?)` returns `true` if and only if each of the following are `true`:

- `other` is not `null`, and
- For each instance field `fieldN` in the record class type that is not inherited, the value of `System.Collections.Generic.EqualityComparer<TN>.Default.Equals(fieldN, other.fieldN)` where `TN` is the field type, and
- If there is a base record class type, the value of `base.Equals(other)` (a non-virtual call to `public virtual bool Equals(Base? other)`); otherwise the value of `EqualityContract == other.EqualityContract`.

The record class type includes provided `==` and `!=` operators declared as follows:

```csharp
public static bool operator==(R? left, R? right) =>
    (object)left == right || (left?.Equals(right) ?? false);
public static bool operator!=(R? left, R? right) => !(left == right);
```

The `Equals` method called by the `==` operator is the `Equals(R? other)` method specified above. The `!=` operator delegates to the `==` operator. It is an error if these operators are declared explicitly.

If the record class type is derived from some base record class type, `Base`, the record class type includes a provided override equivalent to a method declared as follows:

```csharp
public sealed override bool Equals(Base? other);
```

It is an error if the override is declared explicitly. It is an error if the method doesn’t override a method with the same signature in record class type `Base` (for example, if the method is missing in the `Base`, or is sealed, or is not virtual). The provided override returns `Equals((object?)other)`.

The record class type includes a provided override declared as follows:

```csharp
public override bool Equals(object? obj);
```

It is an error if the override is declared explicitly. It is an error if the method doesn’t override `object.Equals(object? obj)` (for example, due to shadowing in intermediate base types). The provided override returns `Equals(other as R)` where `R` is the record class type.

The record class type includes a provided override method declared as follows:

```csharp
public override int GetHashCode();
```

The method may be declared explicitly. It is an error if the explicit declaration doesn’t allow overriding it in a derived type and the record class type is not `sealed`. It is an error if either the provided, or the explicitly declared, method doesn’t override `object.GetHashCode()` (for example, due to shadowing in intermediate base types).

A warning shall be issued if one of `Equals(R?)` and `GetHashCode()` is explicitly declared, but the other is not.

The provided override of `GetHashCode()` returns an `int` result of combining the following values:

- For each instance field `fieldN` in the record class type that is not inherited, the value of `System.Collections.Generic.EqualityComparer<TN>.Default.GetHashCode(fieldN)` where `TN` is the field type, and
- If there is a base record class type, the value of `base.GetHashCode()`; otherwise the value of `System.Collections.Generic.EqualityComparer<System.Type>.Default.GetHashCode(EqualityContract)`.

> *Example*: Consider the following record class types:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"RecordEqualityMembers1", additionalFiles:["T1T2T3.cs"]} -->
> <!-- FIX: create and add file T1T2T3.cs. These are really placeholders for 3 arbitrary types. -->
> ```csharp
> record R1(T1 P1);
> record R2(T1 P1, T2 P2) : R1(P1);
> record R3(T1 P1, T2 P2, T3 P3) : R2(P1, P2);
> ```
>
> For those record class types, the provided equality members would be something like the following:
>
> <!-- Example: {template:"standalone-lib", name:"RecordEqualityMembers2", additionalFiles:["T1T2T3.cs"], ignoredWarnings:["CS8618", "CS8600"]} -->
> <!-- FIX: requires template to enable nullable references. -->
> <!-- NOTE: T1T2T3.cs declares T1, T2, and T3 as classes although they really represent 3 arbitrary types. As they are classes, and nullable checking is enabled, the compiler issues warnings CS8618 and CS8600, which for this purpose are superfluous, so they are ignored. -->
> ```csharp
> class R1 : IEquatable<R1>
> {
>     public T1 P1 { get; init; }
>     protected virtual Type EqualityContract => typeof(R1);
>     public override bool Equals(object? obj) => Equals(obj as R1);
> 
>     public virtual bool Equals(R1? other)
>     {
>         return !(other is null) &&
>             EqualityContract == other.EqualityContract &&
>             EqualityComparer<T1>.Default.Equals(P1, other.P1);
>     }
> 
>     public static bool operator==(R1? left, R1? right) =>
>         (object)left == right || (left?.Equals(right) ?? false);
> 
>     public static bool operator!=(R1? left, R1? right) => !(left == right);
> 
>     public override int GetHashCode()
>     {
>         return HashCode.Combine(EqualityComparer<Type>.Default.GetHashCode(EqualityContract),
>             EqualityComparer<T1>.Default.GetHashCode(P1));
>     }
> }
> 
> class R2 : R1, IEquatable<R2>
> {
>     public T2 P2 { get; init; }
>     protected override Type EqualityContract => typeof(R2);
>     public override bool Equals(object? obj) => Equals(obj as R2);
>     public sealed override bool Equals(R1? other) => Equals((object?)other);
> 
>     public virtual bool Equals(R2? other)
>     {
>         return base.Equals((R1?)other) &&
>             EqualityComparer<T2>.Default.Equals(P2, other.P2);
>     }
> 
>     public static bool operator==(R2? left, R2? right) =>
>         (object)left == right || (left?.Equals(right) ?? false);
> 
>     public static bool operator!=(R2? left, R2? right) => !(left == right);
> 
>     public override int GetHashCode()
>     {
>         return HashCode.Combine(base.GetHashCode(),
>             EqualityComparer<T2>.Default.GetHashCode(P2));
>     }
> }
> 
> class R3 : R2, IEquatable<R3>
> {
>     public T3 P3 { get; init; }
>     protected override Type EqualityContract => typeof(R3);
>     public override bool Equals(object? obj) => Equals(obj as R3);
>     public sealed override bool Equals(R2? other) => Equals((object?)other);
> 
>     public virtual bool Equals(R3? other)
>     {
>         return base.Equals((R2?)other) &&
>             EqualityComparer<T3>.Default.Equals(P3, other.P3);
>     }
> 
>     public static bool operator==(R3? left, R3? right) =>
>         (object)left == right || (left?.Equals(right) ?? false);
> 
>     public static bool operator!=(R3? left, R3? right) => !(left == right);
> 
>     public override int GetHashCode()
>     {
>         return HashCode.Combine(base.GetHashCode(),
>             EqualityComparer<T3>.Default.GetHashCode(P3));
>     }
> }
> ```
>
> *end example*

#### 15.16.4.4 Copy and clone members

A record class type contains two copying members:

- A copy constructor ([§15.16.6.2](classes.md#151662-copy-constructors))
- A provided public, parameter-less, instance clone method having an unspecified reserved name

The copy constructor shall not execute any instance field/property initializers present in the record class declaration. If the constructor is not explicitly declared, it shall be provided by the implementation. If the provided record class is sealed, the constructor shall be private; otherwise; it shall be protected. An explicitly declared copy constructor shall be either public or protected, unless the record class is sealed. The first thing the constructor shall do, is to call a copy constructor of the base class, or a parameter-less `object` constructor if the record inherits from `object`. It is an error for a user-defined copy constructor to use an implicit or explicit *constructor_initializer* that doesn’t fulfill this requirement. After a base copy constructor is invoked, a provided copy constructor shall copy values for all instance fields implicitly or explicitly declared within the record class type.  The sole presence of a copy constructor, whether explicit or implicit, shall not prevent an automatic addition of a default instance constructor.

If a virtual clone method is present in the base record class, the provided clone method shall override it, and the return type of the clone method shall be the current containing type if the covariant-returns feature is supported, and the override return type otherwise. It is an error if the base record class clone method is sealed. If a virtual clone method is not present in the base record class, the return type of the clone method shall be the containing type and the method shall be virtual, unless the record class is sealed or abstract. If the containing record class is abstract, the provided clone method shall also be abstract. If the clone method is not abstract, it shall return the result of a call to a copy constructor.

#### 15.16.4.5 Printing members

If a record class is derived directly from `object`, the class includes a provided method declared as follows:

```csharp
bool PrintMembers(System.Text.StringBuilder builder);
```

The method is `private` if the record class type is sealed. Otherwise, the method is virtual and protected.

The ***printable members of a class*** are the instance public field and readable property members.

The method performs the following tasks:

1. Calls the method `System.Runtime.CompilerServices.RuntimeHelpers.EnsureSufficientExecutionStack()` if that method is present and the record class has printable members.
2. For each of the record class’s printable members, appends that member’s name followed by space `=`space, followed by the member’s value separated with `,` and a space.
3. Returns `true` if the record class has printable members; otherwise, `false`.

For a member that has a value type, its value is converted to a string representation.

If the record class type is derived from some base record class, `Base`, the record class shall include a provided override method declared as follows:

```csharp
protected override bool PrintMembers(System.Text.StringBuilder builder);
```

If the record class has no printable members, the method shall call the base `PrintMembers` method with one argument (its `builder` parameter) and return the result. Otherwise, the method:

1. Calls the base `PrintMembers` method with one argument (its `builder` parameter),
2. If the `PrintMembers` method returned `true`, append `,` and a space to the builder,
3. For each of the record class’s printable members, appends that member’s name followed by space `=` space, followed by the member’s value: `this.member` (or `this.member.ToString()` for value types), separated with `,` and space,
4. Returns `true`.

The `PrintMembers` method may be declared explicitly. It is an error if the explicit declaration does not match the expected signature or accessibility, or if the explicit declaration doesn’t allow overriding it in a derived type and the record class type is not sealed.

The record class shall include a provided method method declared as follows:

```csharp
public override string ToString();
```

The method may be declared explicitly. It is an error if the explicit declaration does not match the expected signature or accessibility, or if the explicit declaration doesn’t allow overriding it in a derived type and the record class type is not sealed. It is an error if either provided, or explicitly declared, method doesn’t override `object.ToString()` (for example, due to shadowing in intermediate base types).

Sealing an explicitly declared `ToString` method prevents the compiler from synthesizing a `ToString` method for any derived record types. However, this does not prevent the compiler from synthesizing `PrintMembers`.

The provided method:

1. Creates a `StringBuilder` instance,
1. Appends the record class name to the builder, followed by ` { `,
1. Invokes the record class’s `PrintMembers` method giving it the builder, followed by a space if it returned `true`,
1. Appends `}`,
1. Returns the builder’s contents with `builder.ToString()`.

> *Example*: Given the following:
>
> <!-- Example: {template:"standalone-console", name:"PrintingMembers1", inferOutput:true} -->
> ```csharp
> public record Base
> {
>     public string FirstName { get; set; }
>     public string LastName { get; set; }
> 
>     public Base(string firstName, string lastName)
>     {
>         FirstName = firstName;
>         LastName = lastName;
>     }
> }
> 
> public record R2 : Base
> {
>     public int Age { get; set; }
>     public R2(string firstName, string lastName, int age) :
>         base(firstName, lastName) { Age = age; }
> }
> 
> class Program
> {
>     static void Main()
>     {
>         Console.WriteLine(new Base("Martin", "Jane"));
>         Console.WriteLine(new R2("Wilson", "Peter", 34));
>     }
> }
> ```
>
> the output produced is
>
> ```console
> Base { FirstName = Martin, LastName = Jane }
> R2 { FirstName = Wilson, LastName = Peter, Age = 34 }
> ```
>
> Consider the following record class types:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"PrintingMembers2", additionalFiles:["T1T2T3.cs"]} -->
> ```csharp
> record R1(T1 P1);
> record R2(T1 P1, T2 P2, T3 P3) : R1(P1);
> ```
>
> For these record class types, the provided printing members would be something like the following:
>
> <!-- Example: {template:"standalone-lib", name:"PrintingMembers3", additionalFiles:["T1T2T3.cs"], ignoredWarnings:["CS8618", "CS8600"], expectedErrors:["CS0535","CS0535"]} -->
> <!-- NOTE: In reality, classes R1 and R2 will also have provided implementations of interface member 'IEquatable<R1>.Equals(R1?)', but as those are not relevant to this printing-member example, error CS0535 re this omission has been ignored. -->
> ```csharp
> class R1 : IEquatable<R1>
> {
>     public T1 P1 { get; init; }
> 
>     protected virtual bool PrintMembers(StringBuilder builder)
>     {
>         builder.Append(nameof(P1));
>         builder.Append(" = ");
>         builder.Append(this.P1); // or builder.Append(this.P1.ToString()); if P1 has a value type
>         return true;
>     }
> 
>     public override string ToString()
>     {
>         var builder = new StringBuilder();
>         builder.Append(nameof(R1));
>         builder.Append(" { ");
>         if (PrintMembers(builder))
>         {
>            builder.Append(" ");
>         }
>         builder.Append("}");
>         return builder.ToString();
>     }
> }
> 
> class R2 : R1, IEquatable<R2>
> {
>     public T2 P2 { get; init; }
>     public T3 P3 { get; init; }
>     
>     protected override bool PrintMembers(StringBuilder builder)
>     {
>         if (base.PrintMembers(builder))
>         {
>             builder.Append(", ");
>         }
>         builder.Append(nameof(P2));
>         builder.Append(" = ");
>         builder.Append(this.P2); // or builder.Append(this.P2); if P2 has a value type
>         builder.Append(", ");
>         builder.Append(nameof(P3));
>         builder.Append(" = ");
>         builder.Append(this.P3); // or builder.Append(this.P3); if P3 has a value type
>         return true;
>     }
>     
>     public override string ToString()
>     {
>         var builder = new StringBuilder();
>         builder.Append(nameof(R2));
>         builder.Append(" { ");
>         if (PrintMembers(builder))
>         {
>             builder.Append(" ");
>         }
>         builder.Append("}");
>         return builder.ToString();
>     }
> }
> ```
>
> *end example*

#### 15.16.4.6 Positional record class members

##### 15.16.4.6.1 General

As well as providing the members described in the preceding subclauses, positional record classes ([§15.2.1](classes.md#1521-general)) result in the implementation  providing additional members with the same conditions as the other provided members, as described in the following subclauses.

##### 15.16.4.6.2 Primary constructor

The primary constructor of a record class is like that of a non-record class ([§15.11.6](classes.md#15116-primary-constructors)), with the following difference: Each parameter value is stored in a corresponding private instance field having a corresponding property with set and get accessors.

##### 15.16.4.6.3 Properties

For each parameter of a *delimited_parameter_list* that has the same name and type as an explicitly declared instance field, the remainder of this subclause does not apply.

For each parameter of a positional *record_declaration* ([§15.2.1](classes.md#1521-general)) that has the same name and type as an inherited or explicitly declared instance field, the remainder of this subclause does not apply.

For each parameter of a *delimited_parameter_list* there is provided a corresponding public property member whose name and type are taken from the value parameter declaration.

For a record class:

- A public auto-property is created with get and init accessors.
- An inherited abstract property with matching type is overridden. It is an error if the inherited property does not have public overridable get and init accessors. It is an error if the inherited property is hidden.  
- The auto-property is initialized to the value of the corresponding primary constructor parameter.
- Attributes may be applied to the provided auto-property and its backing field by using `property:` or `field:` targets, respectively, for attributes syntactically applied to the corresponding record class parameter.

> *Example*: Given the following record class declaration:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"RecordProperties1"} -->
> ```csharp
> public record R(string FirstName, string LastName);
> ```
>
> based on the *parameter_list*, the following properties are provided:
>
> <!-- Example: {template:"code-in-class-lib-without-using", name:"RecordProperties2"} -->
> ```csharp
> public string FirstName { get; init; }
> public string LastName  { get; init; }
> ```
>
> *end example*

##### 15.16.4.6.4 Deconstruct

A positional record class ([§15.2.1](classes.md#1521-general)) with at least one parameter causes to be provided a public `void`-returning instance method called `Deconstruct` with an out parameter declaration for each parameter of the primary constructor declaration. Each parameter of `Deconstruct` has the same type as the corresponding parameter of the primary constructor declaration. The body of the method assigns to each parameter of `Deconstruct` the value from an instance member access to a member of the same name. The method may be declared explicitly. It is an error if the explicit declaration does not match the expected signature or accessibility, or is static.

> *Example*: Consider the following record class having a user-defined `Deconstruct`:
>
> <!-- Example: {template:"standalone-console", name:"RecordDeconstruct", expectedOutput:["p1: 12, p2: xyz"]} -->
> ```csharp
> public record R(int P1, string P2 = "xyz")
> {
>     public void Deconstruct(out int P1, out string P2)
>     {
>         P1 = this.P1;
>         P2 = this.P2;
>     }
> }
>
> class Program
> {
>     static void Main()
>     {
>         R r = new R(12);
>         (int p1, string p2) = r;
>         Console.WriteLine($"p1: {p1}, p2: {p2}");
>     }
> }
> ```
>
> *end example*

## 15.17 Declaring a collection type

### 15.17.1 General

There are a number of contexts in which a collection expression ([§12.8.25](expressions.md#12825-collection-expressions)) may be converted to a collection type ([§10.2.22](conversions.md#10222-implicit-collection-expression-conversions)). One of them is for a target class, struct, or interface type to be made a collection type by annotating it with an attribute, as shown below.

Here is a simple user-defined collection type and its associated builder type:

<!-- Example: {template:"standalone-lib", name:"CollectionType1"} -->
```csharp
[CollectionBuilder(typeof(MyCollectionBuilder), "Create")]
public class MyCollection<T> : IEnumerable<T>
{
    private readonly T[] _storage;
    public int Count { get; }
    public T this[int index]
    {
        get
        {
            return _storage[index];
        }

        set
        {
            _storage[index] = value;
        }
    }
    public MyCollection(ReadOnlySpan<T> elements)
    {
        Count = elements.Length;
        _storage = new T[Count];
        for (int i = 0; i < Count; i++)
        {
            _storage[i] = elements[i];
        }
    }
    public IEnumerator<T> GetEnumerator() =>
      _storage.AsEnumerable<T>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() =>
      _storage.GetEnumerator();
}
internal static class MyCollectionBuilder
{
    internal static MyCollection<T> 
      Create<T>(ReadOnlySpan<T> values) =>
        new MyCollection<T>(values);
}
```

The collection type shall be annotated with a  `CollectionBuilder` attribute ([§23.5.12](attributes.md#23512-the-collectionbuilder-attribute)) that designates an associated, non-generic builder class or struct type having a collection-creation method (whose name is user-defined; in this case, it is `Create`).

The job of a ***collection-creation method*** is to create and initialize an instance of its associated collection type.

First, the set of applicable collection-creation methods `CM` is determined.  It consists of methods that meet the following requirements:

- Be directly declared in the builder type
- Be static
- Be accessible at its point of use
- Its arity shall match that of the collection type
- It shall have a single parameter of type `System.ReadOnlySpan<E>`, passed by value.
- There shall be an implicit identity conversion ([§10.2.2](conversions.md#1022-identity-conversion)), an implicit reference conversion ([§10.2.8](conversions.md#1028-implicit-reference-conversions)), or a boxing conversion ([§10.2.9](conversions.md#1029-boxing-conversions)) from the method return type to the collection type.
- It shall return an instance of the collection being built, which contains a copy of the data from the span parameter.

Methods declared on base types or interfaces are ignored and not part of the `CM` set.

If the `CM` set is empty, then the collection type doesn't have an element type, doesn't have a collection-creation method, and none of the following steps apply.

If only one method among those in the `CM` set has an identity conversion from `E` to the element type of the collection type, that is the collection-creation method for the collection type. Otherwise, the collection type doesn't have a collection-creation method.

It is an error if the `CollectionBuilder` attribute does not refer to an invokable method with the expected signature.

For a *collection_expression* with a target type `C<S₀, S₁, …>` where the type declaration `C<T₀, T₁, …>` has an associated collection-creation method `B.M<U₀, U₁, …>()`, the generic type arguments from the target type are applied in order (from outermost containing type to innermost) to the collection-creation method.

The span parameter for the collection-creation method may be explicitly marked `scoped` or `[UnscopedRef] ([§9.7.3](variables.md#973-the-scoped-modifier))`. If the parameter is implicitly or explicitly `scoped`, the compiler may allocate the storage for the span on the stack rather than the heap.

The construction of an instance of a collection type is described in [§15.17.2](classes.md#15172-collection-construction).

### 15.17.2 Collection construction

The *collection_element*s of a *collection_expression* are evaluated in order, left to right. Each *collection_element* is evaluated exactly once, and any further references to the any elements refer to the results of this initial evaluation.

A *spread_element* may be iterated before or after the subsequent elements in the *collection_expression* are evaluated.

An unhandled exception thrown from any of the methods used during construction shall go uncaught and shall prevent further steps in the construction.

`Length`, `Count`, and `GetEnumerator` are assumed to have no side effects.

If the target type is a struct or class type that implements `System.Collections.IEnumerable`, and the target type does not have a collection-creation method ([§15.17.1](classes.md#15171-general)), the construction of the collection instance steps are, as follows:

- The elements are evaluated in order. Some or all elements may be evaluated during the steps below rather than before.
- The compiler may determine the known length of the collection expression by invoking countable properties ([§18.1](ranges.md#181-general)) or equivalent properties from well-known interfaces or types, on each *spread_element*’s *expression*.
- The constructor that is applicable with no arguments is invoked.
- For each *collection_element*, in order:

  - If the *collection_element* is an *expression_element*, the applicable `Add` instance or extension method is invoked with the *element_expression* as the argument. (Unlike classic collection initializer behavior ([§12.8.17.3.1](expressions.md#1281731-collection-initializers)), element evaluation and `Add` calls are not necessarily interleaved.)
  - If the *collection_element* is a *spread_element* then one of the following steps is used:

    - An applicable `GetEnumerator` instance or extension method is invoked on the *spread_element*’s *expression*, and for each item from the enumerator the applicable `Add` instance or extension method is invoked on the collection instance with the item as the argument. If the enumerator implements `IDisposable`, then `Dispose` shall be called after enumeration, regardless of any exceptions.
    - An applicable `AddRange` instance or extension method is invoked on the collection instance with the *spread_element*’s *expression* as the argument.
    - An applicable `CopyTo` instance or extension method is invoked on the *spread_element*’s *expression* with the collection instance and `int` index as arguments.

- During the construction steps above, an applicable `EnsureCapacity` instance or extension method may be invoked one or more times on the collection instance with an `int` capacity argument.

If the target type is an array, a `Span` or `ReadOnlySpan`, a type with a collection-creation method, or an interface, the construction steps of the collection instance are, as follows:

- The elements are evaluated in order. Some or all elements may be evaluated during the steps below rather than before.
- The compiler may determine the known length of the collection expression by invoking countable properties (or equivalent properties from well-known interfaces or types) on each *spread_element*’s *expression*.

- An *initialization instance* is created as follows:

  - If the target type is an array and the collection expression has a known length, an array is allocated with the expected length.
  - If the target type is a `Span` or `ReadOnlySpan`, or a type with a collection-creation method, and the collection has a known length, a `Span` or `ReadOnlySpan` with the expected length is created referring to contiguous storage.
  - Otherwise, intermediate storage is allocated.

- For each *collection_element*, in order:

  - If the *collection_element*is an *expression_element*, the initialization instance indexer is invoked to add the evaluated expression at the current index.
  - If the element is a *spread_element* then one of the following is used:

    - A member of a well-known interface or type is invoked to copy items from the spread element expression to the initialization instance.
    - An applicable `GetEnumerator` instance or extension method is invoked on the *spread_element*’s *expression*, and for each item from the enumerator, the initialization instance indexer is invoked to add the item at the current index. If the enumerator implements `IDisposable`, then `Dispose` shall be called after enumeration, regardless of any exceptions.
    - An applicable `CopyTo` instance or extension method is invoked on the *spread_element*’s *expression* with the initialization instance and `int` index as arguments.

- If intermediate storage was allocated for the collection, a collection instance is allocated with the actual collection length and the values from the initialization instance are copied to the collection instance, or if a span is required the compiler may use a span of the actual collection length from the intermediate storage. Otherwise, the initialization instance is the collection instance.
- If the target type has a collection-creation method, that method is invoked with the span instance.

> *Note:* The compiler might delay adding elements to the collection (or delay iterating through *spread_element*s) until after evaluating subsequent elements. (When subsequent *spread_element*s have countable properties that would allow calculating the expected length of the collection before allocating the collection.) Conversely, the compiler might eagerly add elements to the collection (and eagerly iterate through *spread_element*s) when there is no advantage to delaying.
>
> Consider the following collection expression:
>
> ```csharp
> int[] x = [a, ..b, ..c, d];
> ```
>
> If *spread_element*s `b` and `c` are countable, the compiler could delay adding items from `a` and `b` until after `c` is evaluated, to allow allocating the resulting array at the expected length. After that, the compiler could eagerly add items from `c`, before evaluating `d`, as shown below.
>
> ```csharp
> var __tmp1 = a;
> var __tmp2 = b;
> var __tmp3 = c;
> var __result = new int[2 + __tmp2.Length + __tmp3.Length];
> int __index = 0;
> __result[__index++] = __tmp1;
> foreach (var __i in __tmp2) __result[__index++] = __i;
> foreach (var __i in __tmp3) __result[__index++] = __i;
> __result[__index++] = d;
> x = __result;
> ```
>
> *end note*

## 15.18 Record class and non-record class differences

A record class differs from a non-record class in several important ways:

- It is declared using the keyword `record` instead of `class`.
- It has a number of members provided for it by the implementation, including a copy constructor.
- Its declaration may contain a *delimited_parameter_list* having zero or more parameters, which results in the provision of a primary constructor having those parameters. For each parameter, the implementation shall provide field-like storage and a property with get and init accessor. A `Deconstruct` method is also provided.
- If it is derived from other than `object`, it may pass arguments to its base type.
- Its class body may be omitted.
- It shall not have a member called `Clone`.
- It shall not have an instance field with an unsafe type.
