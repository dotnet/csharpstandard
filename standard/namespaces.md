# 13 Namespaces

## 13.1 General

C# programs are organized using namespaces. Namespaces are used both as an “internal” organization system for a program, and as an “external” organization system—a way of presenting program elements that are exposed to other programs.

Using directives ([§13.5](namespaces.md#135-using-directives)) are provided to facilitate the use of namespaces.

## 13.2 Compilation units

A *compilation_unit* consists of zero or more *extern_alias_directive*s followed by zero or more *using_directive*s followed by zero or one *global_attributes* followed by zero or more *namespace_member_declaration*s. The *compilation_unit* defines the overall structure of the input.

```ANTLR
compilation_unit
    : extern_alias_directive* using_directive* global_attributes?
      namespace_member_declaration*
    ;
```

A C# program consists of one or more compilation units. When a C# program is compiled, all of the compilation units are processed together. Thus, compilation units can depend on each other, possibly in a circular fashion.

The *extern_alias_directive*s of a compilation unit affect the *using_directive*s, *global_attributes* and *namespace_member_declaration*s of that compilation unit, but have no effect on other compilation units.

The *using_directive*s of a compilation unit affect the *global_attributes* and *namespace_member_declaration*s of that compilation unit, but have no effect on other compilation units.

The *global_attributes* ([§21.3](attributes.md#213-attribute-specification)) of a compilation unit permit the specification of attributes for the target assembly and module. Assemblies and modules act as physical containers for types. An assembly may consist of several physically separate modules.

The *namespace_member_declaration*s of each compilation unit of a program contribute members to a single declaration space called the global namespace.

> *Example*:
>
> ```csharp
> File A.cs:
>     class A {}
> File B.cs:
>     class B {}
> ```
>
> The two compilation units contribute to the single global namespace, in this case declaring two classes with the fully qualified names `A` and `B`. Because the two compilation units contribute to the same declaration space, it would have been an error if each contained a declaration of a member with the same name.
>
> *end example*

## 13.3 Namespace declarations

A *namespace_declaration* consists of the keyword namespace, followed by a namespace name and body, optionally followed by a semicolon.

```ANTLR
namespace_declaration
    : 'namespace' qualified_identifier namespace_body ';'?
    ;

qualified_identifier
    : identifier ('.' identifier)*
    ;

namespace_body
    : '{' extern_alias_directive* using_directive*
      namespace_member_declaration* '}'
    ;
```

A *namespace_declaration* may occur as a top-level declaration in a *compilation_unit* or as a member declaration within another *namespace_declaration*. When a *namespace_declaration* occurs as a top-level declaration in a *compilation_unit*, the namespace becomes a member of the global namespace. When a *namespace_declaration* occurs within another *namespace_declaration*, the inner namespace becomes a member of the outer namespace. In either case, the name of a namespace shall be unique within the containing namespace.

Namespaces are implicitly `public` and the declaration of a namespace cannot include any access modifiers.

Within a *namespace_body*, the optional *using_directive*s import the names of other namespaces, types and members, allowing them to be referenced directly instead of through qualified names. The optional *namespace_member_declaration*s contribute members to the declaration space of the namespace. Note that all *using_directive*s must appear before any member declarations.

The *qualified_identifier* of a *namespace_declaration* may be a single identifier or a sequence of identifiers separated by “`.`” tokens. The latter form permits a program to define a nested namespace without lexically nesting several namespace declarations.

> *Example*:
>
> ```csharp
> namespace N1.N2
> {
>     class A {}
>     class B {}
> }
> ```
>
> is semantically equivalent to
>
> ```csharp
> namespace N1
> {
>     namespace N2
>     {
>         class A {}
>         class B {}
>     }
> }
> ```
>
> *end example*

Namespaces are open-ended, and two namespace declarations with the same fully qualified name ([§7.8.2](basic-concepts.md#782-unqualified-names)) contribute to the same declaration space ([§7.3](basic-concepts.md#73-declarations)).

> *Example*: In the following code
>
> ```csharp
> namespace N1.N2
> {
>     class A {}
> }
>
> namespace N1.N2
> {
>     class B {}
> }
> ```
>
> the two namespace declarations above contribute to the same declaration space, in this case declaring two classes with the fully qualified names `N1.N2.A` and `N1.N2.B`. Because the two declarations contribute to the same declaration space, it would have been an error if each contained a declaration of a member with the same name.
>
> *end example*

## 13.4 Extern alias directives

An *extern_alias_directive* introduces an identifier that serves as an alias for a namespace. The specification of the aliased namespace is external to the source code of the program and applies also to nested namespaces of the aliased namespace.

```ANTLR
extern_alias_directive
    : 'extern' 'alias' identifier ';'
    ;
```

The scope of an *extern_alias_directive* extends over the *using_directive*s, *global_attributes* and *namespace_member_declaration*s of its immediately containing *compilation_unit* or *namespace_body*.

Within a compilation unit or namespace body that contains an *extern_alias_directive*, the identifier introduced by the *extern_alias_directive* can be used to reference the aliased namespace. It is a compile-time error for the *identifier* to be the word `global`.

The alias introduced by an *extern_alias_directive* is very similar to the alias introduced by a *using_alias_directive*. See [§13.5.2](namespaces.md#1352-using-alias-directives) for more detailed discussion of *extern_alias_directive*s and *using_alias_directive*s.

`alias` is a contextual keyword ([§6.4.4](lexical-structure.md#644-keywords)) and only has special meaning when it immediately follows the `extern` keyword in an *extern_alias_directive*.

An error occurs if a program declares an extern alias for which no external definition is provided.

> *Example*: The following program declares and uses two extern aliases, `X` and `Y`, each of which represent the root of a distinct namespace hierarchy:
>
> ```csharp
> extern alias X;
> extern alias Y;
> 
> class Test
> {
>     X::N.A a;
>     X::N.B b1;
>     Y::N.B b2;
>     Y::N.C c;
> }
> ```
>
> The program declares the existence of the extern aliases `X` and `Y`, but the actual definitions of the aliases are external to the program. The identically named `N.B` classes can now be referenced as `X.N.B` and `Y.N.B`, or, using the namespace alias qualifier, `X::N.B` and `Y::N.B`.
> *end example*

## 13.5 Using directives

### 13.5.1 General

***Using directives*** facilitate the use of namespaces and types defined in other namespaces. Using directives impact the name resolution process of *namespace_or_type_name*s ([§7.8](basic-concepts.md#78-namespace-and-type-names)) and *simple_name*s ([§11.7.4](expressions.md#1174-simple-names)), but unlike declarations, *using_directive*s do not contribute new members to the underlying declaration spaces of the compilation units or namespaces within which they are used.

```ANTLR
using_directive
    : using_alias_directive
    | using_namespace_directive
    | using_static_directive    
    ;
```

A *using_alias_directive* ([§13.5.2](namespaces.md#1352-using-alias-directives)) introduces an alias for a namespace or type.

A *using_namespace_directive* ([§13.5.3](namespaces.md#1353-using-namespace-directives)) imports the type members of a namespace.

A *using_static_directive* ([§13.5.4](namespaces.md#1354-using-static-directives)) imports the nested types and static members of a type.

The scope of a *using_directive* extends over the *namespace_member_declarations* of its immediately containing compilation unit or namespace body. The scope of a *using_directive* specifically does not include its peer *using_directive*s. Thus, peer *using_directive*s do not affect each other, and the order in which they are written is insignificant. In contrast, the scope of an *extern_alias_directive* includes the *using_directive*s defined in the same compilation unit or namespace body.

### 13.5.2 Using alias directives

A *using_alias_directive* introduces an identifier that serves as an alias for a namespace or type within the immediately enclosing compilation unit or namespace body.

```ANTLR
using_alias_directive
    : 'using' identifier '=' namespace_or_type_name ';'
    ;
```

Within global attributes and member declarations in a compilation unit or namespace body that contains a *using_alias_directive*, the identifier introduced by the *using_alias_directive* can be used to reference the given namespace or type.

> *Example*:
>
> ```csharp
> namespace N1.N2
> {
>     class A {}
> }
> namespace N3
> {
>     using A = N1.N2.A;
>
>     class B: A {}
> }
> ```
>
> Above, within member declarations in the `N3` namespace, `A` is an alias for `N1.N2.A`, and thus class `N3.B` derives from class `N1.N2.A`. The same effect can be obtained by creating an alias `R` for `N1.N2` and then referencing `R.A`:
>
> ```csharp
> namespace N3
> {
>     using R = N1.N2;
>
>     class B : R.A {}
> }
> ```
>
> *end example*

Within using directives, global attributes and member declarations in a compilation unit or namespace body that contains an *extern_alias_directive*, the identifier introduced by the *extern_alias_directive* can be used to reference the associated namespace.

> *Example*: For example:
>
> ```csharp
> namespace N1
> {
>     extern alias N2;
>
>     class B : N2::A {}
> }
> ```
>
> Above, within member declarations in the `N1` namespace, `N2` is an alias for some namespace whose definition is external to the source code of the program. Class `N1.B` derives from class `N2.A`. The same effect can be obtained by creating an alias `A` for `N2.A` and then referencing `A`:
>
> ```csharp
> namespace N1
> {
>     extern alias N2;
>
>     using A = N2::A;
>
>     class B : A {}
> }
> ```
>
>*end example*

An *extern_alias_directive* or *using_alias_directive* makes an alias available within a particular compilation unit or namespace body, but it does not contribute any new members to the underlying declaration space. In other words, an alias directive is not transitive, but, rather, affects only the compilation unit or namespace body in which it occurs.

> *Example*: In the following code
>
> ```csharp
> namespace N3
> {
>     extern alias R1;
>
>     using R2 = N1.N2;
> }
>
> namespace N3
> {
>     class B : R1::A, R2.I {} // Error, R1 and R2 unknown
> }
> ```
>
> the scopes of the alias directives that introduce `R1` and `R2` only extend to member declarations in the namespace body in which they are contained, so `R1` and `R2` are unknown in the second namespace declaration. However, placing the alias directives in the containing compilation unit causes the alias to become available within both namespace declarations:
>
> ```csharp
> extern alias R1;
>
> using R2 = N1.N2;
>
> namespace N3
> {
>     class B : R1::A, R2.I {}
> }
>
> namespace N3
> {
>     class C : R1::A, R2.I {}
> }
> ```
>
> *end example*

Each *extern_alias_directive* or *using_alias_directive* in a *compilation_unit* or *namespace_body* contributes a name to the alias declaration space ([§7.3](basic-concepts.md#73-declarations)) of the immediately enclosing *compilation_unit* or *namespace_body*. The *identifier* of the alias directive shall be unique within the corresponding alias declaration space. The alias identifier need not be unique within the global declaration space or the declaration space of the corresponding namespace.

> *Example*:
>
> ```csharp
> extern alias A;
> extern alias B;
>
> using A = N1.N2; // Error: alias A already exists
>
> class B {} // Ok
> ```
>
> The using alias named `A` causes an error since there is already an alias named `A` in the same compilation unit. The class named `B` does not conflict with the extern alias named `B` since these names are added to distinct declaration spaces. The former is added to the global declaration space and the latter is added to the alias declaration space for this compilation unit.
>
> When an alias name matches the name of a member of a namespace, usage of either must be appropriately qualified:
>
> ```csharp
> namespace N1.N2
> {
>     class B {}
> }
>
> namespace N3
> {
>     class A {}
>     class B : A {}
> }
>
> namespace N3
> {
>     using A = N1.N2;
>     using B = N1.N2.B;
>
>     class W : B {} // Error: B is ambiguous
>     class X : A.B {} // Error: A is ambiguous
>     class Y : A::B {} // Ok: uses N1.N2.B
>     class Z : N3.B {} // Ok: uses N3.B
> }
> ```
>
> In the second namespace body for `N3`, unqualified use of `B` results in an error, since `N3` contains a member named `B` and the namespace body that also declares an alias with name `B`; likewise for `A`. The class `N3.B` can be referenced as `N3.B` or `global::N3.B`. The alias `A` can be used in a *qualified-alias-member* ([§13.8](namespaces.md#138-qualified-alias-member)), such as `A::B`. The alias `B` is essentially useless. It cannot be used in a *qualified_alias_member* since only namespace aliases can be used in a *qualified_alias_member* and `B` aliases a type.
>
> *end example*

Just like regular members, names introduced by *alias_directives* are hidden by similarly named members in nested scopes.

> *Example*: In the following code
>
> ```csharp
> using R = N1.N2;
>
> namespace N3
> {
>     class R {}
>     class B: R.A {} // Error, R has no member A
> }
> ```
>
> the reference to `R.A` in the declaration of `B` causes a compile-time error because `R` refers to `N3.R`, not `N1.N2`.
>
> *end example*

The order in which *extern_alias_directive*s are written has no significance. Likewise, the order in which *using_alias_directive*s are written has no significance, but all *using_alias_directives* must come after all *extern_alias_directive*s in the same compilation unit or namespace body. Resolution of the *namespace_or_type_name* referenced by a *using_alias_directive* is not affected by the *using_alias_directive* itself or by other *using_directive*s in the immediately containing compilation unit or namespace body, but may be affected by *extern_alias_directive*s in the immediately containing compilation unit or namespace body. In other words, the *namespace_or_type_name* of a *using_alias_directive* is resolved as if the immediately containing compilation unit or namespace body had no *using_directive*s but has the correct set of *extern_alias_directive*s.

> *Example*: In the following code
>
> ```csharp
> namespace N1.N2 {}
>
> namespace N3
> {
>     extern alias E;
>
>     using R1 = E::N; // OK
>     using R2 = N1; // OK
>     using R3 = N1.N2; // OK
>     using R4 = R2.N2; // Error, R2 unknown
> }
> ```
>
> the last *using_alias_directive* results in a compile-time error because it is not affected by the previous *using_alias_directive*. The first *using_alias_directive* does not result in an error since the scope of the extern alias E includes the *using_alias_directive*.
>
> *end example*

A *using_alias_directive* can create an alias for any namespace or type, including the namespace within which it appears and any namespace or type nested within that namespace.

Accessing a namespace or type through an alias yields exactly the same result as accessing that namespace or type through its declared name.

> *Example*: Given
>
> ```csharp
> namespace N1.N2
> {
>     class A {}
> }
>
> namespace N3
> {
>     using R1 = N1;
>     using R2 = N1.N2;
>
>     class B
>     {
>         N1.N2.A a; // refers to N1.N2.A
>         R1.N2.A b; // refers to N1.N2.A
>         R2.A c; // refers to N1.N2.A
>     }
> }
> ```
>
> the names `N1.N2.A`, `R1.N2.A`, and `R2.A` are equivalent and all refer to the class declaration whose fully qualified name is `N1.N2.A`.
>
> *end example*

Although each part of a partial type ([§14.2.7](classes.md#1427-partial-declarations)) is declared within the same namespace, the parts are typically written within different namespace declarations. Thus, different *extern_alias_directive*s and *using_directive*s can be present for each part. When interpreting simple names ([§11.7.4](expressions.md#1174-simple-names)) within one part, only the *extern_alias_directive*s and *using_directive*s of the namespace bodies and compilation unit enclosing that part are considered. This may result in the same identifier having different meanings in different parts.

> *Example*:
>
> ```csharp
> namespace N
> {
>     using List = System.Collections.ArrayList;
>
>     partial class A
>     {
>         List x; // x has type System.Collections.ArrayList
>     }
> }
>
> namespace N
> {
>     using List = Widgets.LinkedList;
>
>     partial class A
>     {
>         List y; // y has type Widgets.LinkedList
>     }
> }
> ```
>
> *end example*

Using aliases can name a closed constructed type, but cannot name an unbound generic type declaration without supplying type arguments.

> *Example*:
>
> ```csharp
> namespace N1
> {
>     class A<T>
>     {
>         class B {}
>     }
> }
>
> namespace N2
> {
>     using W = N1.A;       // Error, cannot name unbound generic type
>     using X = N1.A.B;     // Error, cannot name unbound generic type
>     using Y = N1.A<int>;  // Ok, can name closed constructed type
>     using Z<T> = N1.A<T>; // Error, using alias cannot have type parameters
> }
> ```
>
> *end example*

### 13.5.3 Using namespace directives

A *using_namespace_directive* imports the types contained in a namespace into the immediately enclosing compilation unit or namespace body, enabling the identifier of each type to be used without qualification.

```ANTLR
using_namespace_directive
    : 'using' namespace_name ';'
    ;
```

Within member declarations in a compilation unit or namespace body that contains a *using_namespace_directive*, the types contained in the given namespace can be referenced directly.

> *Example*:
>
> ```csharp
> namespace N1.N2
> {
>     class A {}
> }
>
> namespace N3
> {
>     using N1.N2;
>
>     class B : A {}
> }
> ```
>
> Above, within member declarations in the `N3` namespace, the type members of `N1.N2` are directly available, and thus class `N3.B` derives from class `N1.N2.A`.
>
> *end example*

A *using_namespace_directive* imports the types contained in the given namespace, but specifically does not import nested namespaces.

> *Example*: In the following code
>
> ```csharp
> namespace N1.N2
> {
>     class A {}
> }
>
> namespace N3
> {
>     using N1;
>     class B : N2.A {} // Error, N2 unknown
> }
> ```
>
> the *using_namespace_directive* imports the types contained in `N1`, but not the namespaces nested in `N1`. Thus, the reference to `N2.A` in the declaration of `B` results in a compile-time error because no members named `N2` are in scope.
>
> *end example*

Unlike a *using_alias_directive*, a *using_namespace_directive* may import types whose identifiers are already defined within the enclosing compilation unit or namespace body. In effect, names imported by a *using_namespace_directive* are hidden by similarly named members in the enclosing compilation unit or namespace body.

> *Example*:
>
> ```csharp
> namespace N1.N2
> {
>     class A {}
>     class B {}
> }
>
> namespace N3
> {
>     using N1.N2;
>     class A {}
> }
> ```
>
> Here, within member declarations in the `N3` namespace, `A` refers to `N3.A` rather than `N1.N2.A`.
>
> *end example*

Because names may be ambiguous when more than one imported namespace introduces the same type name, a *using_alias_directive* is useful to disambiguate the reference.

> *Example*: In the following code
>
> ```csharp
> namespace N1
> {
>     class A {}
> }
>
> namespace N2
> {
>     class A {}
> }
>
> namespace N3
> {
>     using N1;
>     using N2;
>
>     class B : A {} // Error, A is ambiguous
> }
> ```
>
> both `N1` and `N2` contain a member `A`, and because `N3` imports both, referencing `A` in `N3` is a compile-time error. In this situation, the conflict can be resolved either through qualification of references to `A`, or by introducing a *using_alias_directive* that picks a particular `A`. For example:
>
> ```csharp
> namespace N3
> {
>     using N1;
>     using N2;
>     using A = N1.A;
>
>     class B : A {} // A means N1.A
> }
> ```
>
> *end example*

Furthermore, when more than one namespace or type imported by *using_namespace_directive*s or *using_static_directive*s in the same compilation unit or namespace body contain types or members by the same name, references to that name as a *simple_name* are considered ambiguous.

> *Example*:
>
> ```csharp
> namespace N1
> {
>     class A {}
> }
>
> class C
> {
>     public static int A;
> }
>
> namespace N2
> {
>     using N1;
>     using static C;
>
>     class B
>     {
>         void M()
>         {
>             A a = new A();   // Ok, A is unambiguous as a type-name
>             A.Equals(2);     // Error, A is ambiguous as a simple-name
>         }
>     }
> }
> ```
>
> `N1` contains a type member `A`, and `C` contains a static field `A`, and because `N2` imports both, referencing `A` as a *simple_name* is ambiguous and a compile-time error.
>
> *end example*

Like a *using_alias_directive*, a *using_namespace_directive* does not contribute any new members to the underlying declaration space of the compilation unit or namespace, but, rather, affects only the compilation unit or namespace body in which it appears.

The *namespace_name* referenced by a *using_namespace_directive* is resolved in the same way as the *namespace_or_type_name* referenced by a *using_alias_directive*. Thus, *using_namespace_directive*s in the same compilation unit or namespace body do not affect each other and can be written in any order.

### 13.5.4 Using static directives

A *using_static_directive* imports the nested types and static members contained directly in a type declaration into the immediately enclosing compilation unit or namespace body, enabling the identifier of each member and type to be used without qualification.

```ANTLR
using_static_directive
    : 'using' 'static' type_name ';'
    ;
```

Within member declarations in a compilation unit or namespace body that contains a *using_static_directive*, the accessible nested types and static members (except extension methods) contained directly in the declaration of the given type can be referenced directly.

> *Example*:
>
> ```csharp
> namespace N1
> {
>    class A 
>    {
>         public class B {}
>         public static B M() => new B();
>    }
> }
>
> namespace N2
> {
>     using static N1.A;
>
>     class C
>     {
>         void N()
>         {
>             B b = M();
>         }
>     }
> }
> ```
>
> In the preceding code, within member declarations in the `N2` namespace, the static members and nested types of `N1.A` are directly available, and thus the method `N` is able to reference both the `B` and `M` members of `N1.A`.
>
> *end example*

A *using_static_directive* specifically does not import extension methods directly as static methods, but makes them available for extension method invocation ([§11.7.8.3](expressions.md#11783-extension-method-invocations)).

> *Example*:
>
> ```csharp
> namespace N1 
> {
>     static class A 
>     {
>         public static void M(this string s){}
>     }
> }
>
> namespace N2
> {
>     using static N1.A;
>
>     class B
>     {
>         void N()
>         {
>             M("A");      // Error, M unknown
>             "B".M();     // Ok, M known as extension method
>             N1.A.M("C"); // Ok, fully qualified
>         }
>     }
> }
> ```
>
> the *using_static_directive* imports the extension method `M` contained in `N1.A`, but only as an extension method. Thus, the first reference to `M` in the body of `B.N` results in a compile-time error because no members named `M` are in scope.
>
> *end example*

A *using_static_directive* only imports members and types declared directly in the given type, not members and types declared in base classes.

> *Example*:
>
> ```csharp
> namespace N1 
> {
>     static class A 
>     {
>         public static void M(string s){}
>     }
>
>     static class B : A
>     {
>         public static void M2(string s){}
>     }
> }
>
> namespace N2
> {
>     using static N1.B;
>
>     class C
>     {
>         void N()
>         {
>             M2("B");      // OK, calls B.M2
>             M("C");       // Error. M unknown 
>         }
>     }
> }
> ```
>
> the *using_static_directive* imports the method `M2` contained in `N1.B`, but does not import the method `M` contained in `N1.A`. Thus, the reference to `M` in the body of `C.N` results in a compile-time error because no members named `M` are in scope. Developers must add a second `using static` directive to specify that the methods in `N1.A` should also be imported.
>
> *end example*

Ambiguities between multiple *using_namespace_directives* and *using_static_directives* are discussed in [§13.5.3](namespaces.md#1353-using-namespace-directives).

## 13.6 Namespace member declarations

A *namespace_member_declaration* is either a *namespace_declaration* ([§13.3](namespaces.md#133-namespace-declarations)) or a *type_declaration* ([§13.7](namespaces.md#137-type-declarations)).

```ANTLR
namespace_member_declaration
    : namespace_declaration
    | type_declaration
    ;
```

A compilation unit or a namespace body can contain *namespace_member_declaration*s, and such declarations contribute new members to the underlying declaration space of the containing compilation unit or namespace body.

## 13.7 Type declarations

A *type_declaration* is a *class_declaration* ([§14.2](classes.md#142-class-declarations)), a *struct_declaration* ([§15.2](structs.md#152-struct-declarations)), an *interface_declaration* ([§17.2](interfaces.md#172-interface-declarations)), an *enum_declaration* ([§18.2](enums.md#182-enum-declarations)), or a *delegate_declaration* ([§19.2](delegates.md#192-delegate-declarations)).

```ANTLR
type_declaration
    : class_declaration
    | struct_declaration
    | interface_declaration
    | enum_declaration
    | delegate_declaration
    ;
```

A *type_declaration* can occur as a top-level declaration in a compilation unit or as a member declaration within a namespace, class, or struct.

When a type declaration for a type `T` occurs as a top-level declaration in a compilation unit, the fully qualified name ([§7.8.2](basic-concepts.md#782-unqualified-names)) of the type declaration is the same as the unqualified name of the declaration ([§7.8.2](basic-concepts.md#782-unqualified-names)). When a type declaration for a type `T` occurs within a namespace, class, or struct declaration, the fully qualified name ([§7.8.3](basic-concepts.md#783-fully-qualified-names)) of the type declarationis `S.N`, where `S` is the fully qualified name of the containing namespace, class, or struct declaration, and `N` is the unqualified name of the declaration.

A type declared within a class or struct is called a nested type ([§14.3.9](classes.md#1439-nested-types)).

The permitted access modifiers and the default access for a type declaration depend on the context in which the declaration takes place ([§7.5.2](basic-concepts.md#752-declared-accessibility)):

- Types declared in compilation units or namespaces can have `public` or `internal` access. The default is `internal` access.
- Types declared in classes can have `public`, `protected internal`, `protected`, `internal`, or `private` access. The default is `private` access.
- Types declared in structs can have `public`, `internal`, or `private` access. The default is `private` access.

## 13.8 Qualified alias member

### 13.8.1 General

The ***namespace alias qualifier*** `::` makes it possible to guarantee that type name lookups are unaffected by the introduction of new types and members. The namespace alias qualifier always appears between two identifiers referred to as the left-hand and right-hand identifiers. Unlike the regular `.` qualifier, the left-hand identifier of the `::` qualifier is looked up only as an extern or using alias.

A *qualified_alias_member* provides explicit access to the global namespace and to extern or using aliases that are potentially hidden by other entities.

```ANTLR
qualified_alias_member
    : identifier '::' identifier type_argument_list?
    ;
```

A *qualified_alias_member* can be used as a *namespace_or_type_name* ([§7.8](basic-concepts.md#78-namespace-and-type-names)) or as the left operand in a *member_access* ([§11.7.6](expressions.md#1176-member-access)).

A *qualified_alias_member* consists of two identifiers, referred to as the left-hand and right-hand identifiers, seperated by the `::` token and optionally followed by a *type_argument_list*. When the left-hand identifier is global then the global namespace is searched for the right-hand identifier. For any other left-hand identifier, that identifier is looked up as an extern or using alias ([§13.4](namespaces.md#134-extern-alias-directives) and [§13.5.2](namespaces.md#1352-using-alias-directives)). A compile-time error occurs if there is no such alias or the alias references a type. If the alias references a namespace then that namespace is searched for the right-hand identifier.

A *qualified_alias_member* has one of two forms:

- `N::I<A₁, ..., Aₑ>`, where `N` and `I` represent identifiers, and `<A₁, ..., Aₑ>` is a type argument list. (`e` is always at least one.)
- `N::I`, where `N` and `I` represent identifiers. (In this case, `e` is considered to be zero.)

Using this notation, the meaning of a *qualified_alias_member* is determined as follows:

- If `N` is the identifier `global`, then the global namespace is searched for `I`:
  - If the global namespace contains a namespace named `I` and `e` is zero, then the *qualified_alias_member* refers to that namespace.
  - Otherwise, if the global namespace contains a non-generic type named `I` and `e` is zero, then the *qualified_alias_member* refers to that type.
  - Otherwise, if the global namespace contains a type named `I` that has `e` type parameters, then the *qualified_alias_member* refers to that type constructed with the given type arguments.
  - Otherwise, the *qualified_alias_member* is undefined and a compile-time error occurs.
- Otherwise, starting with the namespace declaration ([§13.3](namespaces.md#133-namespace-declarations)) immediately containing the *qualified_alias_member* (if any), continuing with each enclosing namespace declaration (if any), and ending with the compilation unit containing the *qualified_alias_member*, the following steps are evaluated until an entity is located:
  - If the namespace declaration or compilation unit contains a *using_alias_directive* that associates N with a type, then the *qualified_alias_member* is undefined and a compile-time error occurs.
  - Otherwise, if the namespace declaration or compilation unit contains an *extern_alias_directive* or *using_alias_directive* that associates `N` with a namespace, then:
    - If the namespace associated with `N` contains a namespace named `I` and `e` is zero, then the *qualified_alias_member* refers to that namespace.
    - Otherwise, if the namespace associated with `N` contains a non-generic type named `I` and `e` is zero, then the *qualified_alias_member* refers to that type.
    - Otherwise, if the namespace associated with `N` contains a type named `I` that has `e` type parameters, then the *qualified_alias_member* refers to that type constructed with the given type arguments.
    - Otherwise, the *qualified_alias_member* is undefined and a compile-time error occurs.
- Otherwise, the *qualified_alias_member* is undefined and a compile-time error occurs.

> *Example*: In the code:
>
> ```csharp
> using S = System.Net.Sockets;
>
> class A
> {
>     public static int x;
> }
>
> class C
> {
>     public void F(int A, object S)
>     {
>         // Use global::A.x instead of A.x
>         global::A.x += A;
>         // Use S::Socket instead of S.Socket
>         S::Socket s = S as S::Socket;
>     }
> }
> ```
>
> the class `A` is referenced with `global::A` and the type `System.Net.Sockets.Socket` is referenced with `S::Socket`. Using `A.x` and `S.Socket` instead would have caused compile-time errors because `A` and `S` would have resolved to the parameters.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The identifier `global` has special meaning only when used as the left-hand identifier of a *qualified_alias_name*. It is not a keyword and it is not itself an alias; it is a contextual keyword ([§6.4.4](lexical-structure.md#644-keywords)). In the code:
>
> ```csharp
> class A { }
>
> class C
> {
>     global.A x; // Error: global is not defined
>     global::A y; // Valid: References A in the global namespace
> }
> ```
>
> using `global.A` causes a compile-time error since there is no entity named `global` in scope. If some entity named global were in scope, then `global` in `global.A` would have resolved to that entity.
>
> Using `global` as the left-hand identifier of a *qualified_alias_member* always causes a lookup in the `global` namespace, even if there is a using alias named `global`. In the code:
>
> ```csharp
> using global = MyGlobalTypes;
>
> class A { }
>
> class C 
> {
>     global.A x; // Valid: References MyGlobalTypes.A
>     global::A y; // Valid: References A in the global namespace
> }
> ```
>
> `global.A` resolves to `MyGlobalTypes.A` and `global::A` resolves to class `A` in the global namespace.
>
> *end note*

### 13.8.2 Uniqueness of aliases

Each compilation unit and namespace body has a separate declaration space for extern aliases and using aliases. Thus, while the name of an extern alias or using alias shall be unique within the set of extern aliases and using aliases declared in the immediately containing compilation unit or namespace body, an alias is permitted to have the same name as a type or namespace as long as it is used only with the `::` qualifier.

> *Example*: In the following:
>
> ```csharp
> namespace N
> {
>     public class A {}
>     public class B {}
> }
>
> namespace N
> {
>     using A = System.IO;
>
>     class X
>     {
>         A.Stream s1; // Error, A is ambiguous
>         A::Stream s2; // Ok
>     }
> }
> ```
>
> the name `A` has two possible meanings in the second namespace body because both the class `A` and the using alias `A` are in scope. For this reason, use of `A` in the qualified name `A.Stream` is ambiguous and causes a compile-time error to occur. However, use of `A` with the `::` qualifier is not an error because `A` is looked up only as a namespace alias.
>
> *end example*
