# 8 Basic concepts

## 8.1 Application startup

A program may be compiled either as a ***class library*** to be used as part of other applications, or as an ***application*** that may be started directly. The mechanism for determining this mode of compilation is implementation-specific and external to this specification.

A program compiled as an application shall contain at least one method qualifying as an entry point by satisfying the following requirements:

-   It shall have the name `Main`.
-   It shall be `static`.
-   It shall not be generic.
-   It shall be declared in a non-generic type. If the type declaring the method is a nested type, none of its enclosing types may be generic.
-   It shall not have the `async` modifier.
-   The return type shall be `void` or `int`.
-   It shall not be a partial method ([§15.6.9](classes.md#1569-partial-methods)) without an implementation.
-   The formal parameter list shall either be empty, or have a single value parameter of type `string[]`.

If more than one method qualifying as an entry point is declared within a program, an external mechanism may be used to specify which method is deemed to be the actual entry point for the application. It is a compile-time error for a program to be compiled as an application without exactly one entry point. A program compiled as a class library may contain methods that would qualify as application entry points, but the resulting library has no entry point.

Ordinarily, the declared accessibility ([§8.5.2](basic-concepts.md#852-declared-accessibility)) of a method is determined by the access modifiers ([§15.3.6](classes.md#1536-access-modifiers)) specified in its declaration, and similarly the declared accessibility of a type is determined by the access modifiers specified in its declaration. In order for a given method of a given type to be callable, both the type and the member shall be accessible. However, the application entry point is a special case. Specifically, the execution environment can access the application's entry point regardless of its declared accessibility and regardless of the declared accessibility of its enclosing type declarations.

When an application is run, a new ***application domain*** is created. Several different instantiations of an application may exist on the same machine at the same time, and each has its own application domain.
An application domain enables application isolation by acting as a container for application state. An application domain acts as a container and boundary for the types defined in the application and the class libraries it uses. Types loaded into one application domain are distinct from the same types loaded into another application domain, and instances of objects are not directly shared between application domains. For instance, each application domain has its own copy of static variables for these types, and a static constructor for a type is run at most once per application domain. Implementations are free to provide implementation-specific policy or mechanisms for the creation and destruction of application domains.

Application startup occurs when the execution environment calls the application's entry point. If the entry point declares a parameter, then during application startup, the implementation shall ensure that the initial value of parameter is a non-null reference to a string array. This array shall consist of non-null references to strings, called application parameters, which are given implementation-defined values by the host environment prior to application startup. The intent is to supply to the application information determined prior to application startup from elsewhere in the hosted environment.

> *Note*: On systems supporting a command line, application parameters correspond to what are generally known as command-line arguments. *end note*

If the entry point's return type is `int` rather than `void`, the return value from the method invocation by the execution environment is used in application termination ([§8.2](basic-concepts.md#82-application-termination)).

Other than the situations listed above, entry point methods behave like those that are not entry points in every respect. In particular, if the entry point is invoked at any other point during the application's lifetime, such as by regular method invocation, there is no special handling of the method: if there is a parameter, it may have an initial value of `null`, or a non-`null` value referring to an array that contains null references. Likewise, the return value of the entry point has no special significance other than in the invocation from the execution environment.

## 8.2 Application termination

***Application termination*** returns control to the execution environment.

If the return type of the application's entry point method is `int`, the value returned serves as the application's ***termination status code***. The purpose of this code is to allow communication of success or failure to the execution environment.

If the return type of the entry point method is `void`, reaching the right brace (`}`) that terminates that method, or executing a `return` statement that has no expression, results in a termination status code of `0`. If the entry point method terminates due to an exception ([§21.4](exceptions.md#214-how-exceptions-are-handled)), the exit code is implementation-specific. Additionally, the implementation may provide alternative APIs for specifying the exit code.

Prior to an application's termination, an implementation should make every reasonable effort to call finalizers ([§15.13](classes.md#1513-finalizers)) for all of its objects that have not yet been garbage collected, unless such cleanup has been suppressed (by a call to the library method `GC.SuppressFinalize`, for example). The implementation should document any conditions under which this behavior cannot be guaranteed.

## 8.3 Declarations

Declarations in a C# program define the constituent elements of the program. C# programs are organized using namespaces. These are introduced using namespace declarations ([§14](namespaces.md#14-namespaces)), which can contain type declarations and nested namespace declarations. Type declarations ([§14.7](namespaces.md#147-type-declarations)) are used to define classes ([§15](classes.md#15-classes)), structs ([§16](structs.md#16-structs)), interfaces ([§18](interfaces.md#18-interfaces)), enums ([§19](enums.md#19-enums)), and delegates ([§20](delegates.md#20-delegates)). The kinds of members permitted in a type declaration depend on the form of the type declaration. For instance, class declarations can contain declarations for constants ([§15.4](classes.md#154-constants)), fields ([§15.5](classes.md#155-fields)), methods ([§15.6](classes.md#156-methods)), properties ([§15.7](classes.md#157-properties)), events ([§15.8](classes.md#158-events)), indexers ([§15.9](classes.md#159-indexers)), operators ([§15.10](classes.md#1510-operators)), instance constructors ([§15.11](classes.md#1511-instance-constructors)), static constructors ([§15.12](classes.md#1512-static-constructors)), finalizers ([§15.13](classes.md#1513-finalizers)), and nested types ([§15.3.9](classes.md#1539-nested-types)).

A declaration defines a name in the ***declaration space*** to which the declaration belongs. It is a compile-time error to have two or more declarations that introduce members with the same name in a declaration space, except in the following cases:

-   Two or more namespace declarations with the same name are allowed in the same declaration space. Such namespace declarations are aggregated to form a single logical namespace and share a single declaration space.
-   Declarations in separate programs but in the same namespace declaration space are allowed to share the same name.  
    > *Note*: However, these declarations could introduce ambiguities if included in the same application. *end note*
-   Two or more methods with the same name but distinct signatures are allowed in the same declaration space ([§8.6](basic-concepts.md#86-signatures-and-overloading)).
-   Two or more type declarations with the same name but distinct numbers of type parameters are allowed in the same declaration space ([§8.8.2](basic-concepts.md#882-unqualified-names)).
-   Two or more type declarations with the partial modifier in the same declaration space may share the same name, same number of type parameters and same classification (class, struct or interface). In this case, the type declarations contribute to a single type and are themselves aggregated to form a single declaration space ([§15.2.7](classes.md#1527-partial-declarations)).
-   A namespace declaration and a type declaration in the same declaration space can share the same name as long as the type declaration has at least one type parameter ([§8.8.2](basic-concepts.md#882-unqualified-names)).

There are several different types of declaration spaces, as described in the following.

-   Within all compilation units of a program, *namespace_member_declaration*s with no enclosing *namespace_declaration* are members of a single combined declaration space called the ***global declaration space***.
-   Within all compilation units of a program, *namespace_member_declaration*s within *namespace_declaration*s that have the same fully qualified namespace name are members of a single combined declaration space.
-   Each *compilation_unit* and *namespace_body* has an ***alias declaration space***. Each *extern_alias_directive* and *using_alias_directive* of the *compilation_unit* or *namespace_body* contributes a member to the alias declaration space ([§14.5.2](namespaces.md#1452-using-alias-directives)).
-   Each non-partial class, struct, or interface declaration creates a new declaration space. Each partial class, struct, or interface declaration contributes to a declaration space shared by all matching parts in the same program ([§16.2.3](structs.md#1623-partial-modifier)).Names are introduced into this declaration space through *class_member_declaration*s, *struct_member_declaration*s, *interface_member_declaration*s, or *type_parameter*s. Except for overloaded instance constructor declarations and static constructor declarations, a class or struct cannot contain a member declaration with the same name as the class or struct. A class, struct, or interface permits the declaration of overloaded methods and indexers. Furthermore, a class or struct permits the declaration of overloaded instance constructors and operators. For example, a class, struct, or interface may contain multiple method declarations with the same name, provided these method declarations differ in their signature ([§8.6](basic-concepts.md#86-signatures-and-overloading)). Note that base classes do not contribute to the declaration space of a class, and base interfaces do not contribute to the declaration space of an interface. Thus, a derived class or interface is allowed to declare a member with the same name as an inherited member. Such a member is said to ***hide*** the inherited member.
-   Each delegate declaration creates a new declaration space. Names are introduced into this declaration space through formal parameters (*fixed_parameter*s and *parameter_array*s) and *type_parameter*s.
-   Each enumeration declaration creates a new declaration space. Names are introduced into this declaration space through *enum_member_declarations*.
-   Each method declaration, property declaration, property accessor declaration, indexer declaration, indexer accessor declaration, operator declaration, instance constructor declaration and anonymous function creates a new declaration space called a ***local variable declaration space***. Names are introduced into this declaration space through formal parameters (*fixed_parameter*s and *parameter_array*s) and *type_parameter*s. The set accessor for a property or an indexer introduces the valuename as a formal parameter. The body of the function member or anonymous function, if any, is considered to be nested within the local variable declaration space. It is an error for a local variable declaration space and a nested local variable declaration space to contain elements with the same name. Thus, within a nested declaration space it is not possible to declare a local variable or constant with the same name as a local variable or constant in an enclosing declaration space. It is possible for two declaration spaces to contain elements with the same name as long as neither declaration space contains the other.
-   Each *block* or *switch_block*, as well as a `for`, `foreach`, and `using` statement, creates a local variable declaration space for local variables and local constants. Names are introduced into this declaration space through *local_variable_declaration*s and *local_constant_declaration*s. Note that blocks that occur as or within the body of a function member or anonymous function are nested within the local variable declaration space declared by those functions for their parameters. Thus, it is an error to have, for example, a method with a local variable and a parameter of the same name.
-   Each *block* or *switch_block* creates a separate declaration space for labels. Names are introduced into this declaration space through *labeled_statement*s, and the names are referenced through *goto_statement*s. The ***label declaration space*** of a block includes any nested blocks. Thus, within a nested block it is not possible to declare a label with the same name as a label in an enclosing block.

The textual order in which names are declared is generally of no significance. In particular, textual order is not significant for the declaration and use of namespaces, constants, methods, properties, events, indexers, operators, instance constructors, finalizers, static constructors, and types. Declaration order is significant in the following ways:

-   Declaration order for field declarations determines the order in which their initializers (if any) are executed ([§15.5.6.2](classes.md#15562-static-field-initialization), [§15.5.6.3](classes.md#15563-instance-field-initialization)).
-   Local variables shall be defined before they are used ([§8.7](basic-concepts.md#87-scopes)).
-   Declaration order for enum member declarations ([§19.4](enums.md#194-enum-members)) is significant when *constant_expression* values are omitted.

> *Example*: The declaration space of a namespace is "open ended", and two namespace declarations with the same fully qualified name contribute to the same declaration space. For example
> ```csharp
> namespace Megacorp.Data
> {
>     class Customer
>     {
>         ...
>     }
> }
> namespace Megacorp.Data
> {
>     class Order
>     {
>         ...
>     }
> }
> ```
> The two namespace declarations above contribute to the same declaration space, in this case declaring two classes with the fully qualified names `Megacorp.Data.Customer` and `Megacorp.Data.Order`. Because the two declarations contribute to the same declaration space, it would have caused a compile-time error if each contained a declaration of a class with the same name. *end example*

> *Note*: As specified above, the declaration space of a block includes any nested blocks. Thus, in the following example, the `F` and `G` methods result in a compile-time error because the name `i` is declared in the outer block and cannot be redeclared in the inner block. However, the `H` and `I` methods are valid since the two `i`'s are declared in separate non-nested blocks.
> ```csharp
> class A
> {
>     void F() {
>         int i = 0;
>         if (true) {
>             int i = 1;
>     }
> }
>     void G() {
>         if (true) {
>             int i = 0;
>         }
>         int i = 1;
>     }
>     void H() {
>         if (true) {
>             int i = 0;
>         }
>         if (true) {
>             int i = 1;
>         }
>     }
>     void I() {
>     for (int i = 0; i < 10; i++)
>         H();
>     for (int i = 0; i < 10; i++)
>         H();
>     }
> }
> ```
> *end note*

## 8.4 Members

### 8.4.1 General

Namespaces and types have ***members***.

> *Note*: The members of an entity are generally available through the use of a qualified name that starts with a reference to the entity, followed by a "`.`" token, followed by the name of the member. *end note*

Members of a type are either declared in the type declaration or ***inherited*** from the base class of the type. When a type inherits from a base class, all members of the base class, except instance constructors, finalizers, and static constructors become members of the derived type. The declared accessibility of a base class member does not control whether the member is inherited—inheritance extends to any member that isn't an instance constructor, static constructor, or finalizer.

> *Note*: However, an inherited member might not be accessible in a derived type, for example because of its declared accessibility ([§8.5.2](basic-concepts.md#852-declared-accessibility)). *end note*

### 8.4.2 Namespace members

Namespaces and types that have no enclosing namespace are members of the ***global namespace***. This corresponds directly to the names declared in the global declaration space.

Namespaces and types declared within a namespace are members of that namespace. This corresponds directly to the names declared in the declaration space of the namespace.

Namespaces have no access restrictions. It is not possible to declare private, protected, or internal namespaces, and namespace names are always publicly accessible.

### 8.4.3 Struct members

The members of a struct are the members declared in the struct and the members inherited from the struct's direct base class `System.ValueType` and the indirect base class `object`.

The members of a simple type correspond directly to the members of the struct type aliased by the simple type ([§9.3.5](types.md#935-simple-types)).

### 8.4.4 Enumeration members

The members of an enumeration are the constants declared in the enumeration and the members inherited from the enumeration's direct base class `System.Enum` and the indirect base classes `System.ValueType` and `object`.

### 8.4.5 Class members

The members of a class are the members declared in the class and the members inherited from the base class (except for class `object` which has no base class). The members inherited from the base class include the constants, fields, methods, properties, events, indexers, operators, and types of the base class, but not the instance constructors, finalizers, and static constructors of the base class. Base class members are inherited without regard to their accessibility.

A class declaration may contain declarations of constants, fields, methods, properties, events, indexers, operators, instance constructors, finalizers, static constructors, and types.

The members of `object` ([§9.2.3](types.md#923-the-object-type)) and `string` ([§9.2.5](types.md#925-the-string-type)) correspond directly to the members of the class types they alias.

### 8.4.6 Interface members

The members of an interface are the members declared in the interface and in all base interfaces of the interface.

> *Note*: The members in class `object` are not, strictly speaking, members of any interface ([§18.4](interfaces.md#184-interface-members)). However, the members in class `object` are available via member lookup in any interface type ([§12.5](expressions.md#125-member-lookup)). *end note*

### 8.4.7 Array members

The members of an array are the members inherited from class `System.Array`.

### 8.4.8 Delegate members

A delegate inherits members from class `System.Delegate`. Additionally, it contains a method named `Invoke` with the same return type and formal parameter list specified in its declaration ([§20.2](delegates.md#202-delegate-declarations)). An invocation of this method shall behave identically to a delegate invocation ([§20.6](delegates.md#206-delegate-invocation)) on the same delegate instance.

An implementation may provide additional members, either through inheritance or directly in the delegate itself.

## 8.5 Member access

### 8.5.1 General

Declarations of members allow control over member access. The accessibility of a member is established by the declared accessibility ([§8.5.2](basic-concepts.md#852-declared-accessibility)) of the member combined with the accessibility of the immediately containing type, if any.

When access to a particular member is allowed, the member is said to be ***accessible.*** Conversely, when access to a particular member is disallowed, the member is said to be ***inaccessible***. Access to a member is permitted when the textual location in which the access takes place is included in the accessibility domain ([§8.5.3](basic-concepts.md#853-accessibility-domains)) of the member.

### 8.5.2 Declared accessibility

The ***declared accessibility*** of a member can be one of the following:

-   Public, which is selected by including a `public` modifier in the member declaration. The intuitive meaning of `public` is "access not limited".
-   Protected, which is selected by including a `protected` modifier in the member declaration. The intuitive meaning of `protected` is "access limited to the containing class or types derived from the containing class".
-   Internal, which is selected by including an `internal` modifier in the member declaration. The intuitive meaning of `internal` is "access limited to this assembly".
-   Protected internal, which is selected by including both a `protected` and an `internal` modifier in the member declaration. The intuitive meaning of `protected internal` is "accessible within this assembly as well as types derived from the containing class".
-   Private, which is selected by including a `private` modifier in the member declaration. The intuitive meaning of `private` is "access limited to the containing type".

Depending on the context in which a member declaration takes place, only certain types of declared accessibility are permitted. Furthermore, when a member declaration does not include any access modifiers, the context in which the declaration takes place determines the default declared accessibility.

-   Namespaces implicitly have `public` declared accessibility. No access modifiers are allowed on namespace declarations.
-   Types declared directly in compilation units or namespaces (as opposed to within other types) can have `public` or `internal` declared accessibility and default to `internal` declared accessibility.
-   Class members can have any of the five kinds of declared accessibility and default to `private` declared accessibility.  
    > *Note*: A type declared as a member of a class can have any of the five kinds of declared accessibility, whereas a type declared as a member of a namespace can have only `public` or `internal` declared accessibility. *end note*
-   Struct members can have `public`, `internal`, or `private` declared accessibility and default to `private` declared accessibility because structs are implicitly sealed. Struct members introduced in a `struct` (that is, not inherited by that struct) cannot have `protected` or `protected internal` declared accessibility.  
    > *Note*: A type declared as a member of a struct can have `public`, `internal`, or `private` declared accessibility, whereas a type declared as a member of a namespace can have only `public` or `internal` declared accessibility. *end note*
-   Interface members implicitly have `public` declared accessibility. No access modifiers are allowed on interface member declarations.
-   Enumeration members implicitly have `public` declared accessibility. No access modifiers are allowed on enumeration member declarations.

### 8.5.3 Accessibility domains

The ***accessibility domain*** of a member consists of the (possibly disjoint) sections of program text in which access to the member is permitted. For purposes of defining the accessibility domain of a member, a member is said to be ***top-level*** if it is not declared within a type, and a member is said to be ***nested*** if it is declared within another type. Furthermore, the ***program text*** of a program is defined as all text contained in all compilation units of the program, and the program text of a type is defined as all text contained in the *type_declaration*s of that type (including, possibly, types that are nested within the type).

The accessibility domain of a predefined type (such as `object`, `int`, or `double`) is unlimited.

The accessibility domain of a top-level unbound type `T` ([§9.4.4](types.md#944-bound-and-unbound-types)) that is declared in a program `P` is defined as follows:

-   If the declared accessibility of `T` is public, the accessibility domain of `T` is the program text of `P` and any program that references `P`.
-   If the declared accessibility of `T` is internal, the accessibility domain of `T` is the program text of `P`.

> *Note*: From these definitions, it follows that the accessibility domain of a top-level unbound type is always at least the program text of the program in which that type is declared. *end note*

The accessibility domain for a constructed type `T<A₁, ...,Aₑ>` is the intersection of the accessibility domain of the unbound generic type `T` and the accessibility domains of the type arguments `A₁, ...,Aₑ`.

The accessibility domain of a nested member `M` declared in a type `T` within a program `P`, is defined as follows (noting that `M` itself might possibly be a type):

-   If the declared accessibility of `M` is `public`, the accessibility domain of `M` is the accessibility domain of `T`.
-   If the declared accessibility of `M` is `protected internal`, let `D` be the union of the program text of `P` and the program text of any type derived from `T`, which is declared outside `P`. The accessibility domain of `M` is the intersection of the accessibility domain of `T` with `D`.
-   If the declared accessibility of `M` is `protected`, let `D` be the union of the program text of `T`and the program text of any type derived from `T`. The accessibility domain of `M` is the intersection of the accessibility domain of `T` with `D`.
-   If the declared accessibility of `M` is `internal`, the accessibility domain of `M` is the intersection of the accessibility domain of `T` with the program text of `P`.
-   If the declared accessibility of `M` is `private`, the accessibility domain of `M` is the program text of `T`.

> *Note*: From these definitions it follows that the accessibility domain of a nested member is always at least the program text of the type in which the member is declared. Furthermore, it follows that the accessibility domain of a member is never more inclusive than the accessibility domain of the type in which the member is declared. *end note*

> *Note*: In intuitive terms, when a type or member `M` is accessed, the following steps are evaluated to ensure that the access is permitted:

-   First, if `M` is declared within a type (as opposed to a compilation unit or a namespace), a compile-time error occurs if that type is not accessible.
-   Then, if `M` is `public`, the access is permitted.
-   Otherwise, if `M` is `protected internal`, the access is permitted if it occurs within the program in which `M` is declared, or if it occurs within a class derived from the class in which `M` is declared and takes place through the derived class type ([§8.5.4](basic-concepts.md#854-protected-access)).
-   Otherwise, if `M` is `protected`, the access is permitted if it occurs within the class in which `M` is declared, or if it occurs within a class derived from the class in which `M` is declared and takes place through the derived class type ([§8.5.4](basic-concepts.md#854-protected-access)).
-   Otherwise, if `M` is `internal`, the access is permitted if it occurs within the program in which `M` is declared.
-   Otherwise, if `M` is `private`, the access is permitted if it occurs within the type in which `M` is declared.
-   Otherwise, the type or member is inaccessible, and a compile-time error occurs.
*end note*
 
> *Example*: In the following code
> ```csharp
> public class A
> {
>     public static int X;
>     internal static int Y;
>     private static int Z;
> }
> internal class B
> {
>     public static int X;
>     internal static int Y;
>     private static int Z;
>     public class C
>     {
>         public static int X;
>         internal static int Y;
>         private static int Z;
>     }
>     private class D
>     {
>         public static int X;
>         internal static int Y;
>         private static int Z;
>     }
> }
> ```
> the classes and members have the following accessibility domains:
> 
> -   The accessibility domain of `A` and `A.X` is unlimited.
> -   The accessibility domain of `A.Y`, `B`, `B.X`, `B.Y`, `B.C`, `B.C.X`, and `B.C.Y` is the program text of the containing program.
> -   The accessibility domain of `A.Z` is the program text of `A`.
> -   The accessibility domain of `B.Z` and `B.D` is the program text of `B`, including the program text of `B.C` and `B.D`.
> -   The accessibility domain of `B.C.Z` is the program text of `B.C`.
> -   The accessibility domain of `B.D.X` and `B.D.Y` is the program text of `B`, including the program text of `B.C` and `B.D`.
> -   The accessibility domain of `B.D.Z` is the program text of `B.D`.
> As the example illustrates, the accessibility domain of a member is never larger than that of a containing type. For example, even though all `X` members have public declared accessibility, all but `A.X` have accessibility domains that are constrained by a containing type. *end example*

As described in [§8.4](basic-concepts.md#84-members), all members of a base class, except for instance constructors, finalizers, and static constructors, are inherited by derived types. This includes even private members of a base class. However, the accessibility domain of a private member includes only the program text of the type in which the member is declared.

> *Example*: In the following code
> ```csharp
> class 
> {
>     int x;
>     static void F(B b) {
>         b.x = 1;         // Ok
>     }
> }
> class B: A
> {
>     static void F(B b) {
>         b.x = 1;         // Error, x not accessible
>     }
> }
> ```
> the `B` class inherits the private member `x` from the `A` class. Because the member is private, it is only accessible within the *class-body* of `A`. Thus, the access to `b.x` succeeds in the `A.F` method, but fails in the `B.F` method. *end example*

### 8.5.4 Protected access

When a `protected` instance member is accessed outside the program text of the class in which it is declared, and when a `protected internal` instance member is accessed outside the program text of the program in which it is declared, the access shall take place within a class declaration that derives from the class in which it is declared. Furthermore, the access is required to take place *through* an instance of that derived class type or a class type constructed from it. This restriction prevents one derived class from accessing protected members of other derived classes, even when the members are inherited from the same base class.

Let `B` be a base class that declares a protected instance member `M`, and let `D` be a class that derives from `B`. Within the *class_body* of `D`, access to `M` can take one of the following forms:

-   An unqualified *type_name* or *primary_expression* of the form `M`.
-   A *primary_expression* of the form `E.M`, provided the type of `E` is `T` or a class derived from `T`, where `T` is the class `D`, or a class type constructed from `D`.
-   A *primary_expression* of the form `base.M`.
-   A *primary_expression* of the form `base[`*argument_list*`]`.

In addition to these forms of access, a derived class can access a protected instance constructor of a base class in a *constructor_initializer* ([§15.11.2](classes.md#15112-constructor-initializers)).

> *Example*: In the following code
> ```csharp
> public class A
> {
>     protected int x;
>     static void F(A a, B b) {
>         a.x = 1; // Ok
>         b.x = 1; // Ok
>     }
> }
> public class B: A
> {
>     static void F(A a, B b) {
>         a.x = 1; // Error, must access through instance of B
>         b.x = 1; // Ok
>     }
> }
> ```
> within `A`, it is possible to access `x` through instances of both `A` and `B`, since in either case the access takes place *through* an instance of `A` or a class derived from `A`. However, within `B`, it is not possible to access `x` through an instance of `A`, since `A` does not derive from `B`. *end example*

> *Example*:
> ```csharp
> class C<T>
> {
>     protected T x;
> }
> class D<T>: C<T>
> {
>     static void F() {
>         D<T> dt = new D<T>();
>         D<int> di = new D<int>();
>         D<string> ds = new D<string>();
>         dt.x = default(T);
>         di.x = 123;
>         ds.x = "test";
>     }
> }
> ```
> Here, the three assignments to `x` are permitted because they all take place through instances of class types constructed from the generic type. *end example*

> *Note:* The accessibility domain ([§8.5.3](basic-concepts.md#853-accessibility-domains)) of a protected member declared in a generic class includes the program text of all class declarations derived from any type constructed from that generic class. In the example:
> ```csharp
> class C<T>
> {
>     protected static T x;
> }
> class D: C<string>
> {
>     static void Main() {
>         C<int>.x = 5;
>     }
> }
> ```
> the reference to `protected` member `C<int>.x` in `D` is valid even though the class `D` derives from `C<string>`. *end note*

### 8.5.5 Accessibility constraints

Several constructs in the C# language require a type to be ***at least as accessible as*** a member or another type. A type `T` is said to be at least as accessible as a member or type `M` if the accessibility domain of `T` is a superset of the accessibility domain of `M`. In other words, `T` is at least as accessible as `M` if `T` is accessible in all contexts in which `M` is accessible.

The following accessibility constraints exist:

-   The direct base class of a class type shall be at least as accessible as the class type itself.
-   The explicit base interfaces of an interface type shall be at least as accessible as the interface type itself.
-   The return type and parameter types of a delegate type shall be at least as accessible as the delegate type itself.
-   The type of a constant shall be at least as accessible as the constant itself.
-   The type of a field shall be at least as accessible as the field itself.
-   The return type and parameter types of a method shall be at least as accessible as the method itself.
-   The type of a property shall be at least as accessible as the property itself.
-   The type of an event shall be at least as accessible as the event itself.
-   The type and parameter types of an indexer shall be at least as accessible as the indexer itself.
-   The return type and parameter types of an operator shall be at least as accessible as the operator itself.
-   The parameter types of an instance constructor shall be at least as accessible as the instance constructor itself.

> *Example*: In the following code
> ```csharp
> class A {...}
> public class B: A {...}
> ```
> the `B` class results in a compile-time error because `A` is not at least as accessible as `B`. *end example*

> *Example*: Likewise, in the following code
> ```csharp
> class A {...}
> public class B
> {
>     A F() {...}
>     internal A G() {...}
>     public A H() {...}
> }
> ```
> the `H` method in `B` results in a compile-time error because the return type `A` is not at least as accessible as the method. *end example*

## 8.6 Signatures and overloading

Methods, instance constructors, indexers, and operators are characterized by their ***signatures***:

-   The signature of a method consists of the name of the method, the number of type parameters, and the type and parameter-passing mode (value, reference, or output) of each of its formal parameters, considered in the order left to right. For these purposes, any type parameter of the method that occurs in the type of a formal parameter is identified not by its name, but by its ordinal position in the type parameter list of the method. The signature of a method specifically does not include the return type, parameter names, type parameter names, type parameter constraints, the `params` or `this` parameter modifiers, nor whether parameters are required or optional.

-   The signature of an instance constructor consists of the type and parameter-passing mode (value, reference, or output) of each of its formal parameters, considered in the order left to right. The signature of an instance constructor specifically does not include the `params` modifier that may be specified for the right-most parameter.
-   The signature of an indexer consists of the type of each of its formal parameters, considered in the order left to right. The signature of an indexer specifically does not include the element type, nor does it include the `params` modifier that may be specified for the right-most parameter.
-   The signature of an operator consists of the name of the operator and the type of each of its formal parameters, considered in the order left to right. The signature of an operator specifically does not include the result type.
-   The signature of a conversion operator consists of the source type and the target type. The implicit or explicit classification of a conversion operator is not part of the signature.
-   Two signatures of the same member kind (method, instance constructor, indexer or operator) are considered to be the *same signatures* if they have the same name, number of type parameters, number of parameters, and parameter-passing modes, and an identity conversion exists between the types of their corresponding parameters ([§11.2.2](conversions.md#1122-identity-conversion)).

Signatures are the enabling mechanism for ***overloading*** of members in classes, structs, and interfaces:

-   Overloading of methods permits a class, struct, or interface to declare multiple methods with the same name, provided their signatures are unique within that class, struct, or interface.
-   Overloading of instance constructors permits a class or struct to declare multiple instance constructors, provided their signatures are unique within that class or struct.
-   Overloading of indexers permits a class, struct, or interface to declare multiple indexers, provided their signatures are unique within that class, struct, or interface.
-   Overloading of operators permits a class or struct to declare multiple operators with the same name, provided their signatures are unique within that class or struct.

Although `out` and `ref` parameter modifiers are considered part of a signature, members declared in a single type cannot differ in signature solely by `ref` and `out`. A compile-time error occurs if two members are declared in the same type with signatures that would be the same if all parameters in both methods with `out` modifiers were changed to `ref` modifiers. For other purposes of signature matching (e.g., hiding or overriding), `ref` and `out` are considered part of the signature and do not match each other.

> *Note*: This restriction is to allow C# programs to be easily translated to run on the Common Language Infrastructure (CLI), which does not provide a way to define methods that differ solely in `ref` and `out`. *end note*

The types `object` and `dynamic` are not distinguished when comparing signatures. Therefore members declared in a single type whose signatures differ only by replacing `object` with `dynamic` are not allowed.

> *Example*: The following example shows a set of overloaded method declarations along with their signatures.
> ```csharp
> interface ITest
> {
>     void F();                   // F()
>     void F(int x);              // F(int)
>     void F(ref int x);          // F(ref int)
>     void F(out int x);          // F(out int) error
>     void F(object o);           // F(object)
>     void F(dynamic d);          // error.
>     void F(int x, int y);       // F(int, int)
>     int F(string s);            // F(string)
>     int F(int x);               // F(int) error
>     void F(string[] a);         // F(string[])
>     void F(params string[] a);  // F(string[]) error
>     void F<S>(S s);             // F<0>(0)
>     void F<T>(T t);             // F<0>(0) error
>     void F<S,T>(S s);           // F<0,1>(0)
>     void F<T,S>(S s);           // F<0,1>(1) ok
> }
> ```
> Note that any `ref` and `out` parameter modifiers ([§15.6.2](classes.md#1562-method-parameters)) are part of a signature. Thus, `F(int)`, `F(ref int)`, and `F(out int)` are all unique signatures. However, `F(ref int)` and `F(out int)` cannot be declared within the same interface because their signatures differ solely by `ref` and `out`. Also, note that the return type and the `params` modifier are not part of a signature, so it is not possible to overload solely based on return type or on the inclusion or exclusion of the `params` modifier. As such, the declarations of the methods `F(int)` and `F(params string[])` identified above, result in a compile-time error. *end example*

## 8.7 Scopes

### 8.7.1 General

The ***scope*** of a name is the region of program text within which it is possible to refer to the entity declared by the name without qualification of the name. Scopes can be *nested*, and an inner scope may redeclare the meaning of a name from an outer scope. (This does not, however, remove the restriction imposed by [§8.3](basic-concepts.md#83-declarations) that within a nested block it is not possible to declare a local variable or local constant with the same name as a local variable or local constant in an enclosing block.) The name from the outer scope is then said to be ***hidden*** in the region of program text covered by the inner scope, and access to the outer name is only possible by qualifying the name.

-   The scope of a namespace member declared by a *namespace_member_declaration* ([§14.6](namespaces.md#146-namespace-member-declarations)) with no enclosing *namespace_declaration* is the entire program text.
-   The scope of a namespace member declared by a *namespace_member_declaration* within a *namespace_declaration* whose fully qualified name is `N`, is the *namespace_body* of every *namespace_declaration* whose fully qualified name is `N` or starts with `N`, followed by a period.
-   The scope of a name defined by an *extern_alias_directive* ([§14.4](namespaces.md#144-extern-alias-directives)) extends over the *using_directives*, *global_attributes* and *namespace_member_declarations* of its immediately containing *compilation_unit* or *namespace_body*. An *extern_alias_directive* does not contribute any new members to the underlying declaration space. In other words, an *extern_alias_directive* is not transitive, but, rather, affects only the *compilation_unit* or *namespace_body* in which it occurs.
-   The scope of a name defined or imported by a *using_directive* ([§14.5](namespaces.md#145-using-directives)) extends over the *global_attributes* and *namespace_member_declarations* of the *compilation_unit* or *namespace_body* in which the *using_directive* occurs. A *using_directive* may make zero or more namespace or type names available within a particular *compilation_unit* or *namespace_body*, but does not contribute any new members to the underlying declaration space. In other words, a *using_directive* is not transitive but rather affects only the *compilation_unit* or *namespace_body* in which it occurs.
-   The scope of a type parameter declared by a *type_parameter_list* on a *class_declaration* ([§15.2](classes.md#152-class-declarations)) is the *class-base*, *type-parameter-constraints-clauses*, and *class-body* of that *class-declaration*.  
    > *Note*: Unlike members of a class, this scope does not extend to derived classes. *end note*
-   The scope of a type parameter declared by a *type_parameter_list* on a *struct_declaration* ([§16.2](structs.md#162-struct-declarations)) is the *struct_interfaces*, *type_parameter_constraints_clauses*, and *struct_body* of that *struct_declaration*.
-   The scope of a type parameter declared by a *type_parameter_list* on an *interface_declaration* ([§18.2](interfaces.md#182-interface-declarations)) is the *interface_base*, *type_parameter-constraints-clauses*, and *interface_body* of that *interface_declaration*.
-   The scope of a type parameter declared by a *type-parameter-list* on a *delegate-declaration* ([§20.2](delegates.md#202-delegate-declarations)) is the *return-type*, *formal-parameter-list*, and *type-parameter-constraints-clauses* of that *delegate-declaration*.
-   The scope of a type parameter declared by a *type_parameter_list* on a *method_declaration* ([§15.6.1](classes.md#1561-general)) is the *method_declaration*.
-   The scope of a member declared by a *class_member_declaration* ([§15.3.1](classes.md#1531-general)) is the *class_body* in which the declaration occurs. In addition, the scope of a class member extends to the *class_body* of those derived classes that are included in the accessibility domain ([§8.5.3](basic-concepts.md#853-accessibility-domains)) of the member.
-   The scope of a member declared by a *struct_member_declaration* ([§16.3](structs.md#163-struct-members)) is the *struct_body* in which the declaration occurs.

-   The scope of a member declared by an *enum_member_declaration* ([§19.4](enums.md#194-enum-members)) is the *enum_body* in which the declaration occurs.
-   The scope of a parameter declared in a *method_declaration* ([§15.6](classes.md#156-methods)) is the *method_body* of that *method_declaration*.
-   The scope of a parameter declared in an *indexer_declaration* ([§15.9](classes.md#159-indexers)) is the *accessor_declarations* of that *indexer_declaration*.
-   The scope of a parameter declared in an *operator_declaration* ([§15.10](classes.md#1510-operators)) is the *block* of that *operator_declaration*.
-   The scope of a parameter declared in a *constructor_declaration* ([§15.11](classes.md#1511-instance-constructors)) is the *constructor_initializer* and *block* of that *constructor_declaration*.
-   The scope of a parameter declared in a *lambda_expression* ([§12.16](expressions.md#1216-anonymous-function-expressions)) is the *lambda_expression_body* of that *lambda_expression*.
-   The scope of a parameter declared in an *anonymous_method_expression* ([§12.16](expressions.md#1216-anonymous-function-expressions)) is the *block* of that *anonymous_method_expression*.
-   The scope of a label declared in a *labeled_statement* ([§13.5](statements.md#135-labeled-statements)) is the *block* in which the declaration occurs.
-   The scope of a local variable declared in a *local_variable_declaration* ([§13.6.2](statements.md#1362-local-variable-declarations)) is the *block* in which the declaration occurs.
-   The scope of a local variable declared in a *switch_block* of a `switch` statement ([§13.8.3](statements.md#1383-the-switch-statement)) is the *switch_block*.
-   The scope of a local variable declared in a *for_initializer* of a `for` statement ([§13.9.4](statements.md#1394-the-for-statement)) is the *for_initializer*, the *for_condition*, the *for_iterator*, and the contained *statement* of the `for` statement.
-   The scope of a local constant declared in a *local_constant_declaration* ([§13.6.3](statements.md#1363-local-constant-declarations)) is the *block* in which the declaration occurs. It is a compile-time error to refer to a local constant in a textual position that precedes its *constant_declarator*.
-   The scope of a variable declared as part of a *foreach_statement*, *using_statement*, *lock_statement* or *query_expression* is determined by the expansion of the given construct.

Within the scope of a namespace, class, struct, or enumeration member it is possible to refer to the member in a textual position that precedes the declaration of the member.

> *Example*:
> ```csharp
> class A
>     {
>     void F() {
>         i = 1;
>     }
>     int i = 0;
> }
> ```
> Here, it is valid for `F` to refer to `i` before it is declared. *end example*

Within the scope of a local variable, it is a compile-time error to refer to the local variable in a textual position that precedes the *local_variable_declarator* of the local variable.

> *Example*:
> ```csharp
> class A
> {    int i = 0;
>     void F() {
>         i = 1;                // Error, use precedes declaration
>         int i;
>         i = 2;
>     }
>     void G() {
>         int j = (j = 1);     // Valid
>     }
>     void H() {
>         int a = 1, b = ++a; // Valid
>     }
> }
> ```
> In the `F` method above, the first assignment to `i` specifically does not refer to the field declared in the outer scope. Rather, it refers to the local variable and it results in a compile-time error because it textually precedes the declaration of the variable. In the `G` method, the use of `j` in the initializer for the declaration of `j` is valid because the use does not precede the *local_variable_declarator*. In the `H` method, a subsequent *local_variable_declarator* correctly refers to a local variable declared in an earlier *local_variable_declarator* within the same *local_variable_declaration*. *end example*

> *Note*: The scoping rules for local variables and local constants are designed to guarantee that the meaning of a name used in an expression context is always the same within a block. If the scope of a local variable were to extend only from its declaration to the end of the block, then in the example above, the first assignment would assign to the instance variable and the second assignment would assign to the local variable, possibly leading to compile-time errors if the statements of the block were later to be rearranged.)
> 
> The meaning of a name within a block may differ based on the context in which the name is used. In the example
> ```csharp
> using System;
> class A {}
> class Test
> {
>     static void Main() {
>         string A = "hello, world";
>         string s = A;                      // expression context
>         Type t = typeof(A);                // type context
>         Console.WriteLine(s);              // writes "hello, world"
>         Console.WriteLine(t);              // writes "A"
>     }
> }
> ```
> the name `A` is used in an expression context to refer to the local variable `A` and in a type context to refer to the class `A`. *end note*

### 8.7.2 Name hiding

#### 8.7.2.1 General

The scope of an entity typically encompasses more program text than the declaration space of the entity. In particular, the scope of an entity may include declarations that introduce new declaration spaces containing entities of the same name. Such declarations cause the original entity to become *hidden*. Conversely, an entity is said to be ***visible*** when it is not hidden.

Name hiding occurs when scopes overlap through nesting and when scopes overlap through inheritance. The characteristics of the two types of hiding are described in the following subclauses.

#### 8.7.2.2 Hiding through nesting

Name hiding through nesting can occur as a result of nesting namespaces or types within namespaces, as a result of nesting types within classes or structs, and as a result of parameter, local variable, and local constant declarations.

> *Example*: In the following code
> ```csharp
> class A
> {
>     int i = 0;
>     void F() {
>         int i = 1;
>     }
>     void G() {
>         i = 1;
>     }
> }
> ```
> within the `F` method, the instance variable `i` is hidden by the local variable `i`, but within the `G` method, `i` still refers to the instance variable. *end example*

When a name in an inner scope hides a name in an outer scope, it hides all overloaded occurrences of that name.

> *Example*: In the following code
> ```csharp
> class Outer
> {
>     static void F(int i) {}
>     static void F(string s) {}
>     class Inner
>     {
>         static void F(long l) {}
>         void G() {
>             F(1); // Invokes Outer.Inner.F
>             F("Hello"); // Error
>         }
>     }
> }
> ```
> the call `F(1)` invokes the `F` declared in `Inner` because all outer occurrences of `F` are hidden by the inner declaration. For the same reason, the call `F("Hello")` results in a compile-time error. *end example*

#### 8.7.2.3 Hiding through inheritance

Name hiding through inheritance occurs when classes or structs redeclare names that were inherited from base classes. This type of name hiding takes one of the following forms:

-   A constant, field, property, event, or type introduced in a class or struct hides all base class members with the same name.
-   A method introduced in a class or struct hides all non-method base class members with the same name, and all base class methods with the same signature ([§8.6](basic-concepts.md#86-signatures-and-overloading)).
-   An indexer introduced in a class or struct hides all base class indexers with the same signature ([§8.6](basic-concepts.md#86-signatures-and-overloading)) .

The rules governing operator declarations ([§15.10](classes.md#1510-operators)) make it impossible for a derived class to declare an operator with the same signature as an operator in a base class. Thus, operators never hide one another.

Contrary to hiding a name from an outer scope, hiding a visible name from an inherited scope causes a warning to be reported.

> *Example*: In the following code
> ```csharp
> class Base
> {
>     public void F() {}
> }
> class Derived: Base
> {
>     public void F() {} // Warning, hiding an inherited name
> }
> ```
> the declaration of `F` in `Derived` causes a warning to be reported. Hiding an inherited name is specifically not an error, since that would preclude separate evolution of base classes. For example, the above situation might have come about because a later version of `Base` introduced an `F` method that wasn’t present in an earlier version of the class. *end example*

The warning caused by hiding an inherited name can be eliminated through use of the `new` modifier:

> *Example*:
> ```csharp
> class Base
> {
>     public void F() {}
> }
> class Derived: Base
> {
>     new public void F() {}
> }
> ```
> The `new` modifier indicates that the `F` in `Derived` is “new”, and that it is indeed intended to hide the inherited member. *end example*

A declaration of a new member hides an inherited member only within the scope of the new member.

> *Example*:
> ```csharp
> class Base
> {
>     public static void F() {}
> }
> class Derived: Base
> {
>     new private static void F() {} // Hides Base.F in Derived only
> }
> class MoreDerived: Derived
> {
>     static void G() { F(); } // Invokes Base.F
> }
> ```
> In the example above, the declaration of `F` in `Derived` hides the `F` that was inherited from `Base`, but since the new `F` in `Derived` has private access, its scope does not extend to `MoreDerived`. Thus, the call `F()` in `MoreDerived.G` is valid and will invoke `Base.F`. *end example*

## 8.8 Namespace and type names

### 8.8.1 General

Several contexts in a C# program require a *namespace-name* or a *type-name* to be specified.
```ANTLR
namespace_name
    : namespace_or_type-name
    ;

type_name
    : namespace_or_type_name
    ;
    
namespace_or_type_name
    : identifier type_argument_list?
    | namespace_or_type_name '.' identifier type_argument_list?
    | qualified_alias_member
    ;
```
A *namespace_name* is a *namespace_or_type_name* that refers to a namespace.

Following resolution as described below, the *namespace_or_type_name* of a *namespace_name* shall refer to a namespace, or otherwise a compile-time error occurs. No type arguments ([§9.4.2](types.md#942-type-arguments)) can be present in a *namespace_name* (only types can have type arguments).

A *type_name* is a *namespace_or_type-name* that refers to a type. Following resolution as described below, the *namespace_or_type_name* of a *type_name* shall refer to a type, or otherwise a compile-time error occurs.

If the *namespace_or_type_name* is a *qualified_alias_member* its meaning is as described in [§14.8.1](namespaces.md#1481-general). Otherwise, a *namespace_or_type_name* has one of four forms:

-   `I`
-   `I<A₁, ..., Aₓ>`
-   `N.I`
-   `N.I<A₁, ..., Aₓ>`

where `I` is a single identifier, `N` is a *namespace_or_type_name* and `<A₁, ..., Aₓ>` is an optional *type_argument_list*. When no *type_argument_list* is specified, consider `x` to be zero.

The meaning of a *namespace_or_type_name* is determined as follows:

- If the *namespace_or_type_name* is a *qualified_alias_member*, the meaning is as specified in [§14.8.1](namespaces.md#1481-general).
- Otherwise, if the *namespace_or_type-name* is of the form `I` or of the form `I<A₁, ..., Aₓ>`:
  - If `x` is zero and the *namespace-or-type-name* appears within a generic method declaration ([§15.6](classes.md#156-methods)) but outside the *attributes* of its *method-header,* and if that declaration includes a type parameter ([§15.2.3](classes.md#1523-type-parameters)) with name `I`, then the *namespace-or-type-name* refers to that type parameter.
  - Otherwise, if the *namespace-or-type-name* appears within a type declaration, then for each instance type `T` ([§15.3.2](classes.md#1532-the-instance-type)), starting with the instance type of that type declaration and continuing with the instance type of each enclosing class or struct declaration (if any):
    - If `x` is zero and the declaration of `T` includes a type parameter with name `I`, then the *namespace-or-type-name* refers to that type parameter.
    - Otherwise, if the *namespace-or-type-name* appears within the body of the type declaration, and `T` or any of its base types contain a nested accessible type having name `I` and `x` type parameters, then the *namespace-or-type-name* refers to that type constructed with the given type arguments. If there is more than one such type, the type declared within the more derived type is selected.  
    > *Note*: Non-type members (constants, fields, methods, properties, indexers, operators, instance constructors, finalizers, and static constructors) and type members with a different number of type parameters are ignored when determining the meaning of the *namespace-or-type-name*. *end note*
  - Otherwise, for each namespace `N`, starting with the namespace in which the *namespace_or_type_name* occurs, continuing with each enclosing namespace (if any), and ending with the global namespace, the following steps are evaluated until an entity is located:
    - If `x` is zero and `I` is the name of a namespace in `N`, then:
      - If the location where the *namespace_or_type_name* occurs is enclosed by a namespace declaration for `N` and the namespace declaration contains an *extern_alias_directive* or *using_alias_directive* that associates the name `I` with a namespace or type, then the *namespace_or_type_name* is ambiguous and a compile-time error occurs.
      - Otherwise, the *namespace_or_type_name* refers to the namespace named `I` in `N`.
    - Otherwise, if `N` contains an accessible type having name `I` and `x` type parameters, then:
      - If `x` is zero and the location where the *namespace_or_type_name* occurs is enclosed by a namespace declaration for `N` and the namespace declaration contains an *extern_alias_directive* or *using_alias_directive* that associates the name `I` with a namespace or type, then the *namespace_or_type_name* is ambiguous and a compile-time error occurs.
      - Otherwise, the *namespace_or_type_name* refers to the type constructed with the given type arguments.
    - Otherwise, if the location where the *namespace_or_type_name* occurs is enclosed by a namespace declaration for `N`:
      - If `x` is zero and the namespace declaration contains an *extern_alias_directive* or *using_alias_directive* that associates the name `I` with an imported namespace or type, then the *namespace_or_type_name* refers to that namespace or type.
      - Otherwise, if the namespaces imported by the *using_namespace_directive*s of the namespace declaration contain exactly one type having name `I` and `x` type parameters, then the *namespace_or_type_name* refers to that type constructed with the given type arguments.
      - Otherwise, if the namespaces imported by the *using_namespace_directive*s of the namespace declaration contain more than one type having name `I` and `x` type parameters, then the *namespace_or_type_name* is ambiguous and an error occurs.
  - Otherwise, the *namespace_or_type_name* is undefined and a compile-time error occurs.
- Otherwise, the *namespace_or_type_name* is of the form `N.I` or of the form `N.I<A₁, ..., Aₓ>`. `N` is first resolved as a *namespace_or_type_name*. If the resolution of `N` is not successful, a compile-time error occurs. Otherwise, `N.I` or `N.I<A₁, ..., Aₓ>` is resolved as follows:
  - If `x` is zero and `N` refers to a namespace and `N` contains a nested namespace with name `I`, then the *namespace_or_type_name* refers to that nested namespace.
  - Otherwise, if `N` refers to a namespace and `N` contains an accessible type having name `I` and `x` type parameters, then the *namespace_or_type_name* refers to that type constructed with the given type arguments.
  - Otherwise, if `N` refers to a (possibly constructed) class or struct type and `N` or any of its base classes contain a nested accessible type having name `I` and `x` type parameters, then the *namespace_or_type_name* refers to that type constructed with the given type arguments. If there is more than one such type, the type declared within the more derived type is selected. *Note*: If the meaning of `N.I` is being determined as part of resolving the base class specification of `N` then the direct base class of `N` is considered to be object ([§15.2.4.2](classes.md#15242-base-classes)). *end note*
  - Otherwise, `N.I` is an invalid *namespace_or_type_name*, and a compile-time error occurs.

A *namespace_or_type_name* is permitted to reference a static class ([§15.2.2.4](classes.md#15224-static-classes)) only if

-   The *namespace_or_type_name* is the `T` in a *namespace_or_type_name* of the form `T.I`, or
-   The *namespace_or_type_name* is the `T` in a *typeof_expression* ([§12.7.12](expressions.md#12712-the-typeof-operator)) of the form `typeof(T)`

### 8.8.2 Unqualified names

Every namespace declaration and type declaration has an ***unqualified name*** determined as follows:

-   For a namespace declaration, the unqualified name is the *qualified_identifier* specified in the declaration.
-   For a type declaration with no *type_parameter_list*, the unqualified name is the *identifier* specified in the declaration.
-   For a type declaration with K type parameters, the unqualified name is the *identifier* specified in the declaration, followed by the *generic_dimension_specifier* ([§12.7.12](expressions.md#12712-the-typeof-operator)) for K type parameters.

### 8.8.3 Fully qualified names

Every namespace and type declaration has a ***fully qualified name,*** which uniquely identifies the namespace or type declaration amongst all others within the program. The fully qualified name of a namespace or type declaration with unqualified name `N` is determined as follows:

-   If `N` is a member of the global namespace, its fully qualified name is `N`.
-   Otherwise, its fully qualified name is `S.N`, where `S` is the fully qualified name of the namespace or type declaration in which `N` is declared.

In other words, the fully qualified name of `N` is the complete hierarchical path of identifiers and *generic_dimension_specifier*s that lead to `N`, starting from the global namespace. Because every member of a namespace or type shall have a unique name, it follows that the fully qualified name of a namespace or type declaration is always unique. It is a compile-time error for the same fully qualified name to refer to two distinct entities. In particular:

-   It is an error for both a namespace declaration and a type declaration to have the same fully qualified name.
-   It is an error for two different kinds of type declarations to have the same fully qualified name (for example, if both a struct and class declaration have the same fully qualified name).
-   It is an error for a type declaration without the partial modifier to have the same fully qualified name as another type declaration ([§15.2.7](classes.md#1527-partial-declarations)).

> *Example*: The example below shows several namespace and type declarations along with their associated fully qualified names.
> ```csharp
> class A {}                 // A
> namespace X                // X
> {
>     class B                // X.B
>     {
>         class C {}         // X.B.C
>     }
>     namespace Y            // X.Y
>     {
>         class D {}         // X.Y.D
>     }
> }
> namespace X.Y              // X.Y
> {
>     class E {}             // X.Y.E
>     class G<T> {           // X.Y.G<>
>         class H {}         // X.Y.G<>.H
>     }
>     class G<S,T> {         // X.Y.G<,>
>         class H<U> {}      // X.Y.G<,>.H<>
>     }
> }
> ```
> *end example*

## 8.9 Automatic memory management

C# employs automatic memory management, which frees developers from manually allocating and freeing the memory occupied by objects. Automatic memory management policies are implemented by a garbage collector. The memory management life cycle of an object is as follows:

1.  When the object is created, memory is allocated for it, the constructor is run, and the object is considered ***live***.
1.  If neither the object nor any of its instance fields can be accessed by any possible continuation of execution, other than the running of finalizers, the object is considered ***no longer in use*** and it becomes eligible for finalization.  
    > *Note*: The C# compiler and the garbage collector might choose to analyze code to determine which references to an object might be used in the future. For instance, if a local variable that is in scope is the only existing reference to an object, but that local variable is never referred to in any possible continuation of execution from the current execution point in the procedure, the garbage collector might (but is not required to) treat the object as no longer in use. *end note*
1.  Once the object is eligible for finalization, at some unspecified later time the finalizer ([§15.13](classes.md#1513-finalizers)) (if any) for the object is run. Under normal circumstances the finalizer for the object is run once only, though implementation-specific APIs may allow this behavior to be overridden.
1.  Once the finalizer for an object is run, if neither the object nor any of its instance fields can be accessed by any possible continuation of execution, including the running of finalizers, the object is considered inaccessible and the object becomes eligible for collection.  
    > *Note*: An object which could previously not be accessed may become accessible again due to its finalizer. An example of this is provided below. *end note* 
1.  Finally, at some time after the object becomes eligible for collection, the garbage collector frees the memory associated with that object.

The garbage collector maintains information about object usage, and uses this information to make memory management decisions, such as where in memory to locate a newly created object, when to relocate an object, and when an object is no longer in use or inaccessible.

Like other languages that assume the existence of a garbage collector, C# is designed so that the garbage collector might implement a wide range of memory management policies. C# requires that finalizers be run at some time between the time an object is eligible and the time that the application exits, but specifies neither a time constraint within that span, nor an order in which finalizers are run.

The behavior of the garbage collector can be controlled, to some degree, via static methods on the class `System.GC`. This class can be used to request a collection to occur, finalizers to be run (or not run), and so forth.

> *Example*: Since the garbage collector is allowed wide latitude in deciding when to collect objects and run finalizers, a conforming implementation might produce output that differs from that shown by the following code. The program
> ```csharp
> using System;
> class A
> {
>     ~A() {
>         Console.WriteLine("Finalize instance of A");
>     }
> }
> class B
> {
>     object Ref;
>     public B(object o) {
>         Ref = o;
>     }
>     ~B() {
>         Console.WriteLine("Finalize instance of B");
>     }
> }
> class Test
> {
>     static void Main() {
>         B b = new B(new A());
>         b = null;
>         GC.Collect();
>         GC.WaitForPendingFinalizers();
>     }
> }
> ```
> creates an instance of class `A` and an instance of class `B`. These objects become eligible for garbage collection when the variable `b` is assigned the value `null`, since after this time it is impossible for any user-written code to access them. The output could be either
> ```console
> Finalize instance of `A`
> Finalize instance of `B`
> ```
> or
> ```console
> Finalize instance of `B`
> Finalize instance of `A`
> ```
> because the language imposes no constraints on the order in which objects are garbage collected.
>
> In subtle cases, the distinction between "eligible for finalization" and "eligible for collection" can be important. For example,
> ```csharp
> using System;
> class A
> {
>     ~A() {
>         Console.WriteLine("Finalize instance of A");
>     }
>     public void F() {
>         Console.WriteLine("A.F");
>         Test.RefA = this;
>     }
> }
> class B
> {
>     public A Ref;
>     ~B() {
>         Console.WriteLine("Finalize instance of B");
>         Ref.F();
>     }
> }
> class Test
> {
>     public static A RefA;
>     public static B RefB;
>     static void Main() {
>         RefB = new B();
>         RefA = new A();
>         RefB.Ref = RefA;
>         RefB = null;
>         RefA = null;
>         // A and B now eligible for finalization
>         GC.Collect();
>         GC.WaitForPendingFinalizers();
>         // B now eligible for collection, but A is not
>         if (RefA != null)
>         Console.WriteLine("RefA is not null");
>     }
> }
> ```
> In the above program, if the garbage collector chooses to run the finalizer of `A` before the finalizer of `B`, then the output of this program might be:
> ```console
> Finalize instance of A
> Finalize instance of B
> A.F
> RefA is not null
> ```
> Note that although the instance of `A` was not in use and `A`'s finalizer was run, it is still possible for methods of `A` (in this case, `F`) to be called from another finalizer. Also, note that running of a finalizer might cause an object to become usable from the mainline program again. In this case, the running of `B`'s finalizer caused an instance of `A` that was previously not in use, to become accessible from the live reference `Test.RefA`. After the call to `WaitForPendingFinalizers`, the instance of `B` is eligible for collection, but the instance of `A` is not, because of the reference `Test.RefA`. *end example*

## 8.10 Execution order

Execution of a C# program proceeds such that the side effects of each executing thread are preserved at critical execution points. A ***side effect*** is defined as a read or write of a volatile field, a write to a non-volatile variable, a write to an external resource, and the throwing of an exception. The critical execution points at which the order of these side effects shall be preserved are references to volatile fields ([§15.5.4](classes.md#1554-volatile-fields)), `lock` statements ([§13.13](statements.md#1313-the-lock-statement)), and thread creation and termination. The execution environment is free to change the order of execution of a C# program, subject to the following constraints:

-   Data dependence is preserved within a thread of execution. That is, the value of each variable is computed as if all statements in the thread were executed in original program order.
-   Initialization ordering rules are preserved ([§15.5.5](classes.md#1555-field-initialization), [§15.5.6](classes.md#1556-variable-initializers)).
-   The ordering of side effects is preserved with respect to volatile reads and writes ([§15.5.4](classes.md#1554-volatile-fields)). Additionally, the execution environment need not evaluate part of an expression if it can deduce that that expression’s value is not used and that no needed side effects are produced (including any caused by calling a method or accessing a volatile field). When program execution is interrupted by an asynchronous event (such as an exception thrown by another thread), it is not guaranteed that the observable side effects are visible in the original program order.
