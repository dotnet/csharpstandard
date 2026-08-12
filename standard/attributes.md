# 23 Attributes

## 23.1 General

Much of the C# language enables the programmer to specify declarative information about the entities defined in the program. For example, the accessibility of a method in a class is specified by decorating it with the *method_modifier*s `public`, `protected`, `internal`, and `private`.

C# enables programmers to invent new kinds of declarative information, called ***attribute***s. Programmers can then attach attributes to various program entities, and retrieve attribute information in a run-time environment.

> *Note*: For instance, a framework might define a `HelpAttribute` attribute that can be placed on certain program elements (such as classes and methods) to provide a mapping from those program elements to their documentation. *end note*

Attributes are defined through the declaration of attribute classes ([§23.2](attributes.md#232-attribute-classes)), which can have positional and named parameters ([§23.2.3](attributes.md#2323-positional-and-named-parameters)). Attributes are attached to entities in a C# program using attribute specifications ([§23.3](attributes.md#233-attribute-specification)), and can be retrieved at run-time as attribute instances ([§23.4](attributes.md#234-attribute-instances)).

## 23.2 Attribute classes

### 23.2.1 General

A class that derives from the abstract class `System.Attribute`, whether directly or indirectly, is an ***attribute class***. The declaration of an attribute class defines a new kind of attribute that can be placed on program entities. By convention, attribute classes are named with a suffix of `Attribute`. Uses of an attribute may either include or omit this suffix.

A generic class declaration shall not use `System.Attribute` as a direct or indirect base class.

> *Example*:
>
> <!-- Example: {template:"standalone-lib", name:"AttributeCantBeGeneric", expectedErrors:["CS8936"], ignoredWarnings:["CS0169"]} -->
> ```csharp
> public class B : Attribute {}
> public class C<T> : B {} // Error – generic cannot be an attribute
> ```
>
> *end example*

### 23.2.2 Attribute usage

The attribute `AttributeUsage` ([§23.5.2](attributes.md#2352-the-attributeusage-attribute)) is used to describe how an attribute class can be used.

`AttributeUsage` has a positional parameter ([§23.2.3](attributes.md#2323-positional-and-named-parameters)) that enables an attribute class to specify the kinds of program entities on which it can be used.

> *Example*: The following example defines an attribute class named `SimpleAttribute` that can be placed on *class_declaration*s and *interface_declaration*s only, and shows several uses of the `Simple` attribute.
>
> <!-- Example: {template:"standalone-lib", name:"AttributeUsage1", replaceEllipsis:true} -->
> <!-- Maintenance Note: A version of this type exists in additional-files as "SimpleAttribute.cs". As such, certain changes to this type definition might need to be reflected in that file, in which case, *all* examples using that file should be tested. -->
> ```csharp
> [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
> public class SimpleAttribute : Attribute
> { 
>     ... 
> }
>
> [Simple] class Class1 {...}
> [Simple] interface Interface1 {...}
> ```
>
> Although this attribute is defined with the name `SimpleAttribute`, when this attribute is used, the `Attribute` suffix may be omitted, resulting in the short name `Simple`. Thus, the example above is semantically equivalent to the following
>
> <!-- Example: {template:"standalone-lib", name:"AttributeUsage2", replaceEllipsis:true, additionalFiles:["SimpleAttribute.cs"]} -->
> ```csharp
> [SimpleAttribute] class Class1 {...}
> [SimpleAttribute] interface Interface1 {...}
> ```
>
> *end example*

`AttributeUsage` has a named parameter ([§23.2.3](attributes.md#2323-positional-and-named-parameters)), called `AllowMultiple`, which indicates whether the attribute can be specified more than once for a given entity. If `AllowMultiple` for an attribute class is true, then that attribute class is a ***multi-use attribute class***, and can be specified more than once on an entity. If `AllowMultiple` for an attribute class is false or it is unspecified, then that attribute class is a ***single-use attribute class***, and can be specified at most once on an entity.

> *Example*: The following example defines a multi-use attribute class named `AuthorAttribute` and shows a class declaration with two uses of the `Author` attribute:
>
> <!-- Example: {template:"standalone-lib", name:"AttributeUsage4", replaceEllipsis:true} -->
> <!-- Maintenance Note: A version of this type exists in additional-files as "AuthorAttribute.cs". As such, certain changes to this type definition might need to be reflected in that file, in which case, *all* examples using that file should be tested. -->
> ```csharp
> [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
> public class AuthorAttribute : Attribute
> {
>     public string Name { get; }
>     public AuthorAttribute(string name) => Name = name;
> }
>
> [Author("Brian Kernighan"), Author("Dennis Ritchie")]
> class Class1 
> {
>     ...
> }
> ```
>
> *end example*

`AttributeUsage` has another named parameter ([§23.2.3](attributes.md#2323-positional-and-named-parameters)), called `Inherited`, which indicates whether the attribute, when specified on a base class, is also inherited by classes that derive from that base class. If `Inherited` for an attribute class is true, then that attribute is inherited. If `Inherited` for an attribute class is false then that attribute is not inherited. If it is unspecified, its default value is true.

An attribute class `X` not having an `AttributeUsage` attribute attached to it, as in

<!-- Example: {template:"standalone-lib", name:"AttributeUsage6", replaceEllipsis:true} -->
```csharp
class X : Attribute { ... }
```

is equivalent to the following:

<!-- Example: {template:"standalone-lib", name:"AttributeUsage7", replaceEllipsis:true} -->
```csharp
[AttributeUsage(
   AttributeTargets.All,
   AllowMultiple = false,
   Inherited = true)
]
class X : Attribute { ... }
```

### 23.2.3 Positional and named parameters

Attribute classes can have ***positional parameter***s and ***named parameter***s. Each public instance constructor for an attribute class defines a valid sequence of positional parameters for that attribute class. Each non-static public read-write field and non-static public read-write or read-init property for an attribute class defines a named parameter for the attribute class. For a property to define a named parameter, that property shall have both a public get accessor and a public set or init accessor.

> *Example*: The following example defines an attribute class named `HelpAttribute` that has one positional parameter, `url`, and one named parameter, `Topic`. Although it is non-static and public, the property `Url` does not define a named parameter, since it is not read-write or read-init. Two uses of this attribute are also shown:
>
> <!-- Example: {template:"standalone-lib", name:"PositionalAndNamedParameters1", replaceEllipsis:true} -->
> <!-- Maintenance Note: A version of this type exists in additional-files as "HelpAttribute.cs". As such, certain changes to this type definition might need to be reflected in that file, in which case, *all* examples using that file should be tested. -->
> ```csharp
> [AttributeUsage(AttributeTargets.Class)]
> public class HelpAttribute : Attribute
> {
>     public HelpAttribute(string url) // url is a positional parameter
>     { 
>         ...
>     }
>
>     // Topic is a named parameter
>     public string Topic
>     { 
>         get;
>         set;
>     }
>
>     public string Url { get; }
> }
>
> [Help("http://www.mycompany.com/xxx/Class1.htm")]
> class Class1
> {
> }
>
> [Help("http://www.mycompany.com/xxx/Misc.htm", Topic ="Class2")]
> class Class2
> {
> }
> ```
>
> *end example*

### 23.2.4 Attribute parameter types

The types of positional and named parameters for an attribute class are limited to the ***attribute parameter type***s, which are:

- One of the following types: `bool`, `byte`, `char`, `double`, `float`, `int`, `long`, `sbyte`, `short`, `string`, `uint`, `ulong`, `ushort`.
- The type `object`.
- The type `System.Type`.
- Enum types.
- Single-dimensional arrays of the above types.
- A constructor argument or public field that does not have one of these types, shall not be used as a positional or named parameter in an attribute specification.

## 23.3 Attribute specification

Application of a previously defined attribute to a program entity is called ***attribute specification***. An attribute is a piece of additional declarative information that is specified for a program entity. Attributes can be specified at global scope (to specify attributes on the containing assembly or module) and for *type_declaration*s ([§14.8](namespaces.md#148-type-declarations)), *class_member_declaration*s ([§15.3](classes.md#153-class-members)), *interface_member_declaration*s ([§19.4](interfaces.md#194-interface-members)), *struct_member_declaration*s ([§16.3](structs.md#163-struct-members)), *enum_member_declaration*s ([§20.2](enums.md#202-enum-declarations)), *accessor_declaration*s ([§15.7.3](classes.md#1573-accessors)), *event_accessor_declaration*s ([§15.8](classes.md#158-events)), *local_function_declaration*s ([§13.6.4](statements.md#1364-local-function-declarations)), elements of *parameter_list*s ([§15.6.2](classes.md#1562-method-parameters)), elements of *type_parameter_list*s ([§15.2.3](classes.md#1523-type-parameters)), *lambda_expression*s ([§12.22.1](expressions.md#12221-general)), and elements of *explicit_anonymous_function_parameter*s and *implicit_anonymous_function_parameter*s ([§12.22.1](expressions.md#12221-general)).

Attributes are specified in ***attribute section***s. An attribute section consists of a pair of square brackets, which surround a comma-separated list of one or more attributes. The order in which attributes are specified in such a list, and the order in which sections attached to the same program entity are arranged, is not significant. For instance, the attribute specifications `[A][B]`, `[B][A]`, `[A, B]`, and `[B, A]` are equivalent.

```ANTLR
global_attributes
    : global_attribute_section+
    ;

global_attribute_section
    : '[' global_attribute_target_specifier attribute_list ']'
    ;

global_attribute_target_specifier
    : global_attribute_target ':'
    ;

global_attribute_target
    : identifier
    ;

attributes
    : attribute_section+
    ;

attribute_section
    : '[' attribute_target_specifier? attribute_list ']'
    ;

attribute_target_specifier
    : attribute_target ':'
    ;

attribute_target
    : identifier
    | keyword
    ;

attribute_list
    : attribute (',' attribute)* ','?
    ;

attribute
    : attribute_name attribute_arguments?
    ;

attribute_name
    : type_name
    ;

attribute_arguments
    : '(' ')'
    | '(' positional_argument_list (',' named_argument_list)? ')'
    | '(' named_argument_list ')'
    ;

positional_argument_list
    : positional_argument (',' positional_argument)*
    ;

positional_argument
    : argument_name? attribute_argument_expression
    ;

named_argument_list
    : named_argument (','  named_argument)*
    ;

named_argument
    : identifier '=' attribute_argument_expression
    ;

attribute_argument_expression
    : non_assignment_expression
    ;
```

For the production *global_attribute_target*, and in the text below, *identifier* shall have a spelling equal to `assembly` or `module`, where equality is that defined in [§6.4.3](lexical-structure.md#643-identifiers). For the production *attribute_target*, and in the text below, *identifier* shall have a spelling that is not equal to `assembly` or `module`, using the same definition of equality as above.

An attribute consists of an *attribute_name* and an optional list of positional and named arguments. The positional arguments (if any) precede the named arguments. A positional argument consists of an *attribute_argument_expression*; a named argument consists of a name, followed by an equal sign, followed by an *attribute_argument_expression*, which, together, are constrained by the same rules as simple assignment. The order of named arguments is not significant.

> *Note*: For convenience, a trailing comma is allowed in a *global_attribute_section* and an *attribute_section*, just as one is allowed in an *array_initializer* ([§17.7](arrays.md#177-array-initializers)). *end note*

The *attribute_name* identifies an attribute class.

When an attribute is placed at the global level, a *global_attribute_target_specifier* is required. When the *global_attribute_target* is equal to:

- `assembly` — the target is the containing assembly
- `module` — the target is the containing module

No other values for *global_attribute_target* are allowed.

The standardized *attribute_target* names are `event`, `field`, `method`, `param`, `property`, `return`, `type`, and `typevar`. These target names shall only be used in the following contexts:

- `event` — an event.
- `field` — a field. A field-like event (i.e., one without accessors) ([§15.8.2](classes.md#1582-field-like-events)) and an automatically implemented property ([§15.7.4](classes.md#1574-automatically-implemented-properties)) can also have an attribute with this target.
- `method` — a constructor; finalizer; method; operator; local function, property get, set, and init accessors; indexer get, set, and init accessors; event add and remove accessors; and lambda expressions. A field-like event (i.e., one without accessors) can also have an attribute with this target.
- `param` — property set and init accessors, indexer set and init accessors, event add and remove accessors, and a parameter in a constructor, method, local fuction, and operator.
- `property` — a property and an indexer.
- `return` — a delegate, method, local function, operator, property get accessor, indexer get accessor, and lambda expression.
- `type` — a delegate, class, struct, enum, and interface.
- `typevar` — a type parameter.

Certain contexts permit the specification of an attribute on more than one target. A program can explicitly specify the target by including an *attribute_target_specifier*. Without an *attribute_target_specifier* a default is applied, but an *attribute_target_specifier* can be used to affirm or override the default. The contexts are resolved as follows:

- For an attribute on a delegate declaration the default target is the delegate. Otherwise when the *attribute_target* is equal to:
  - `type` — the target is the delegate
  - `return` — the target is the return value
- For an attribute on a method declaration the default target is the method. Otherwise when the *attribute_target* is equal to:
  - `method` — the target is the method
  - `return` — the target is the return value
- For an attribute on a local function declaration the default target is the local function. Otherwise when the *attribute_target* is equal to:
  - `method` — the target is the local function
  - `return` — the target is the return value
- For an attribute on an operator declaration the default target is the operator. Otherwise when the *attribute_target* is equal to:
  - `method` — the target is the operator
  - `return` — the target is the return value
- For an attribute on a get accessor declaration for a property or indexer declaration the default target is the associated method. Otherwise when the *attribute_target* is equal to:
  - `method` — the target is the associated method
  - `return` — the target is the return value
- For an attribute specified on a set or init accessor for a property or indexer declaration the default target is the associated method. Otherwise when the *attribute_target* is equal to:
  - `method` — the target is the associated method
  - `param` — the target is the lone implicit parameter
- For an attribute on an automatically implemented property declaration the default target is the property. Otherwise when the *attribute_target* is equal to:
  - `field` — the target is the compiler-generated backing field for the property
- For an attribute specified on an event declaration that omits *event_accessor_declarations* the default target is the event declaration. Otherwise when the *attribute_target* is equal to:
  - `event` — the target is the event declaration
  - `field` — the target is the field
  - `method` — the targets are the methods
- In the case of an event declaration that does not omit *event_accessor_declarations* the default target is the method.
  - `method` — the target is the associated method
  - `param` — the target is the lone parameter
- For an attribute on a *lambda_expression* the default target is the method. Otherwise when the *attribute_target* is equal to:
  - `method` — the target is the method
  - `return` — the target is the return value

In all other contexts, inclusion of an *attribute_target_specifier* is permitted but unnecessary.

> *Example*: a class declaration may either include or omit the specifier `type`:
>
> <!-- Example: {template:"standalone-lib", name:"AttributeSpecification1", additionalFiles:["AuthorAttribute.cs"]} -->
> ```csharp
> [type: Author("Brian Kernighan")]
> class Class1 {}
> 
> [Author("Dennis Ritchie")]
> class Class2 {}
> ```
>
> *end example*.

An implementation can accept other *attribute_target*s, the purposes of which are implementation defined. An implementation that does not recognize such an *attribute_target* shall issue a warning and ignore the containing *attribute_section*.

By convention, attribute classes are named with a suffix of `Attribute`. An *attribute_name* can either include or omit this suffix. Specifically, an *attribute_name* is resolved as follows:

- If the right-most identifier of the *attribute_name* is a verbatim identifier ([§6.4.3](lexical-structure.md#643-identifiers)), then the *attribute_name* is resolved as a *type_name* ([§7.8](basic-concepts.md#78-namespace-and-type-names)). If the result is not a type derived from `System.Attribute`, a compile-time error occurs.
- Otherwise,
  - The *attribute_name* is resolved as a *type_name* ([§7.8](basic-concepts.md#78-namespace-and-type-names)) except any errors are suppressed. If this resolution is successful and results in a type derived from `System.Attribute` then the type is the result of this step.
  - The characters `Attribute` are appended to the right-most identifier in the *attribute_name* and the resulting string of tokens is resolved as a *type_name* ([§7.8](basic-concepts.md#78-namespace-and-type-names)) except any errors are suppressed. If this resolution is successful and results in a type derived from `System.Attribute` then the type is the result of this step.

If exactly one of the two steps above results in a type derived from `System.Attribute`, then that type is the result of the *attribute_name*. Otherwise a compile-time error occurs.

> *Example*: If an attribute class is found both with and without this suffix, an ambiguity is present, and a compile-time error results. If the *attribute_name* is spelled such that its right-most *identifier* is a verbatim identifier ([§6.4.3](lexical-structure.md#643-identifiers)), then only an attribute without a suffix is matched, thus enabling such an ambiguity to be resolved. The example
>
> <!-- Example: {template:"standalone-lib", name:"AttributeSpecification2", expectedErrors:["CS1614"]} -->
> ```csharp
> [AttributeUsage(AttributeTargets.All)]
> public class Example : Attribute
> {}
> 
> [AttributeUsage(AttributeTargets.All)]
> public class ExampleAttribute : Attribute
> {}
>
> [Example]               // Error: ambiguity
> class Class1 {}
>
> [ExampleAttribute]      // Refers to ExampleAttribute
> class Class2 {}
>
> [@Example]              // Refers to Example
> class Class3 {}
>
> [@ExampleAttribute]     // Refers to ExampleAttribute
> class Class4 {}
> ```
>
> shows two attribute classes named `Example` and `ExampleAttribute`. The attribute `[Example]` is ambiguous, since it could refer to either `Example` or `ExampleAttribute`. Using a verbatim identifier allows the exact intent to be specified in such rare cases. The attribute `[ExampleAttribute]` is not ambiguous (although it would be if there were an attribute class named `ExampleAttributeAttribute`!). If the declaration for class `Example` is removed, then both attributes refer to the attribute class named `ExampleAttribute`, as follows:
>
> <!-- Example: {template:"standalone-lib", name:"AttributeSpecification3", expectedErrors:["CS0246"]} -->
> ```csharp
> [AttributeUsage(AttributeTargets.All)]
> public class ExampleAttribute : Attribute
> {}
>
> [Example]            // Refers to ExampleAttribute
> class Class1 {}
>
> [ExampleAttribute]   // Refers to ExampleAttribute
> class Class2 {}
>
> [@Example]           // Error: no attribute named “Example”
> class Class3 {}
> ```
>
> *end example*

It is a compile-time error to use a single-use attribute class more than once on the same entity.

> *Example*: The example
>
> <!-- Example: {template:"standalone-lib", name:"AttributeSpecification4", expectedErrors:["CS0579"]} -->
> ```csharp
> [AttributeUsage(AttributeTargets.Class)]
> public class HelpStringAttribute : Attribute
> {
>     public HelpStringAttribute(string value)
>     {
>         Value = value;
>     }
>
>     public string Value { get; }
> }
> [HelpString("Description of Class1")]
> [HelpString("Another description of Class1")]   // multiple uses not allowed
> public class Class1 {}
> ```
>
> results in a compile-time error because it attempts to use `HelpString`, which is a single-use attribute class, more than once on the declaration of `Class1`.
>
> *end example*

An expression `E` is an *attribute_argument_expression* if all of the following statements are true:

- The type of `E` is an attribute parameter type ([§23.2.4](attributes.md#2324-attribute-parameter-types)).
- At compile-time, the value of `E` can be resolved to one of the following:
  - A constant value.
  - A `System.Type` object obtained using a *typeof_expression* ([§12.8.18](expressions.md#12818-the-typeof-operator)) specifying a non-generic type, a closed constructed type ([§8.4.3](types.md#843-open-and-closed-types)), or an unbound generic type ([§8.4.4](types.md#844-bound-and-unbound-types)), but not an open type ([§8.4.3](types.md#843-open-and-closed-types)).
  - A single-dimensional array of *attribute_argument_expression*s.

> *Example*:
>
> <!-- Example: {template:"standalone-lib", name:"AttributeSpecification5", expectedErrors:["CS0416","CS0416"], ignoredWarnings:["CS0169"]} -->
> ```csharp
> [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
> public class TestAttribute : Attribute
> {
>     public int P1 { get; set; }
>
>     public Type P2 { get; set; }
>
>     public object P3 { get; set; }
> }
>
> [Test(P1 = 1234, P3 = new int[]{1, 3, 5}, P2 = typeof(float))]
> class MyClass {}
>
> class C<T> {
>     [Test(P2 = typeof(T))] // Error – T not a closed type.
>     int x1;
>
>     [Test(P2 = typeof(C<T>))] // Error – C<;T>; not a closed type.
>     int x2;
>
>     [Test(P2 = typeof(C<int>))] // Ok
>     int x3;
>
>     [Test(P2 = typeof(C<>))] // Ok
>     int x4;
> }
> ```
>
> *end example*

The attributes of a type declared in multiple parts are determined by combining, in an unspecified order, the attributes of each of its parts. If the same attribute is placed on multiple parts, it is equivalent to specifying that attribute multiple times on the type.

> *Example*: The two parts:
>
> <!-- Example: {template:"standalone-lib", name:"AttributeSpecification6", additionalFiles:["Attr1Attribute.cs","Attr2Attribute.cs","Attr3Attribute.cs"]} -->
> ```csharp
> [Attr1, Attr2("hello")]
> partial class A {}
>
> [Attr3, Attr2("goodbye")]
> partial class A {}
> ```
>
> are equivalent to the following single declaration:
>
> <!-- Example: {template:"standalone-lib", name:"AttributeSpecification7", additionalFiles:["Attr1Attribute.cs","Attr2Attribute.cs","Attr3Attribute.cs"]} -->
> ```csharp
> [Attr1, Attr2("hello"), Attr3, Attr2("goodbye")]
> class A {}
> ```
>
> *end example*

Attributes on type parameters combine in the same way.

## 23.4 Attribute instances

### 23.4.1 General

An ***attribute instance*** is an instance that represents an attribute at run-time. An attribute is defined with an attribute class, positional arguments, and named arguments. An attribute instance is an instance of the attribute class that is initialized with the positional and named arguments.

Retrieval of an attribute instance involves both compile-time and run-time processing, as described in the following subclauses.

### 23.4.2 Compilation of an attribute

The compilation of an *attribute* with attribute class `T`, *positional_argument_list* `P`, *named_argument_list* `N`, and specified on a program entity `E` is compiled into an assembly `A` via the following steps:

- Follow the compile-time processing steps for compiling an *object_creation_expression* of the form new `T(P)`. These steps either result in a compile-time error, or determine an instance constructor `C` on `T` that can be invoked at run-time.
- If `C` does not have public accessibility, then a compile-time error occurs.
- For each *named_argument* `Arg` in `N`:
  - Let `Name` be the *identifier* of the *named_argument* `Arg`.
  - `Name` shall identify a non-static read-write public field or a public non-static read-write or read-init property on `T`. If `T` has no such field or property, then a compile-time error occurs.
- If any of the values within *positional_argument_list* `P` or one of the values within *named_argument_list* `N` is of type `System.String` and the value is not well-formed as defined by the Unicode Standard, it is implementation-defined whether the value compiled is equal to the run-time value retrieved ([§23.4.3](attributes.md#2343-run-time-retrieval-of-an-attribute-instance)).
  > *Note*: As an example, a string which contains a high surrogate UTF-16 code unit which is not immediately followed by a low surrogate code unit is not well-formed. *end note*
- Store the following information (for run-time instantiation of the attribute) in the assembly output by the compiler as a result of compiling the program containing the attribute: the attribute class `T`, the instance constructor `C` on `T`, the *positional_argument_list* `P`, the *named_argument_list* `N`, and the associated program entity `E`, with the values resolved completely at compile-time.

### 23.4.3 Run-time retrieval of an attribute instance

Using the terms defined in [§23.4.2](attributes.md#2342-compilation-of-an-attribute), the attribute instance represented by `T`, `C`, `P`, and `N`, and associated with `E` can be retrieved at run-time from the assembly `A` using the following steps:

- Follow the run-time processing steps for executing an *object_creation_expression* of the form `new T(P)`, using the instance constructor `C` and values as determined at compile-time. These steps either result in an exception, or produce an instance `O` of `T`.
- For each *named_argument* `Arg` in `N`, in order:
  - Let `Name` be the *identifier* of the *named_argument* `Arg`. If `Name` does not identify a non-static public read-write field or a non-static public read-write or read-init property on `O`, then an exception is thrown.
  - Let `Value` be the result of evaluating the *attribute_argument_expression* of `Arg`.
  - If `Name` identifies a field on `O`, then set this field to `Value`.
  - Otherwise, Name identifies a property on `O`. Set this property to Value.
  - The result is `O`, an instance of the attribute class `T` that has been initialized with the *positional_argument_list* `P` and the *named_argument_list* `N`.

> *Note*: The format for storing `T`, `C`, `P`, `N` (and associating it with `E`) in `A` and the mechanism to specify `E` and retrieve `T`, `C`, `P`, `N` from `A` (and hence how an attribute instance is obtained at runtime) is beyond the scope of this specification. *end note*

## 23.5 Reserved attributes

### 23.5.1 General

A number of attributes affect the language in some way. These attributes include:

- `System.AttributeUsageAttribute` ([§23.5.2](attributes.md#2352-the-attributeusage-attribute)), which is used to describe the ways in which an attribute class can be used.
- `System.Diagnostics.ConditionalAttribute` ([§23.5.3](attributes.md#2353-the-conditional-attribute)), is a multi-use attribute class which is used to define conditional methods and conditional attribute classes. This attribute indicates a condition by testing a conditional compilation symbol.
- `System.ObsoleteAttribute` ([§23.5.4](attributes.md#2354-the-obsolete-attribute)), which is used to mark a member as obsolete.
- `System.Runtime.CompilerServices.AsyncMethodBuilderAttribute` ([§23.5.5](attributes.md#2355-the-asyncmethodbuilder-attribute)), which is used to establish a task builder for an async method.
- `System.Runtime.CompilerServices.CallerLineNumberAttribute` ([§23.5.6.2](attributes.md#23562-the-callerlinenumber-attribute)), `System.Runtime.CompilerServices.CallerFilePathAttribute` ([§23.5.6.3](attributes.md#23563-the-callerfilepath-attribute)), `System.Runtime.CompilerServices.CallerMemberNameAttribute` ([§23.5.6.4](attributes.md#23564-the-callermembername-attribute)), and `System.Runtime.CompilerServices.CallerArgumentExpressionAttribute` ([§23.5.6.5](attributes.md#23565-the-callerargumentexpression-attribute)), which are used to supply information about the calling context to optional parameters.
- `System.Runtime.CompilerServices.EnumeratorCancellationAttribute` ([§23.5.9](attributes.md#2359-the-enumeratorcancellation-attribute)), which is used to specify parameter for the cancellation token in an asynchronous iterator.
- `System.Runtime.CompilerServices.ModuleInitializer` ([§23.5.10](attributes.md#23510-the-moduleinitializer-attribute)), which is used to mark a method as a module initializer.
- `System.Runtime.CompilerServices.InterpolatedStringHandlerAttribute` and `System.Runtime.CompilerServices.InterpolatedStringHandlerArgumentAttribute`, which are used to declare a custom interpolated string expression handler ([§23.5.10.1](attributes.md#235101-custom-interpolated-string-expression-handlers)) and to call one of its constructors, respectively.
- System.Diagnostics.CodeAnalysis.UnscopedRefAttribute ([§23.5.8](attributes.md#2358-the-unscopedref-attribute)), which allows an otherwise implicitly scoped ref to be treated as not being scoped.
- `System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute` ([§23.5.11.1](attributes.md#235111-the-setsrequiredmembers-attribute)) and `System.Runtime.CompilerServices.RequiredMemberAttribute` ([§23.5.11.2](attributes.md#235112-the-requiredmember-attribute)), which are used in required-member contexts ([§15.7.1](classes.md#1571-general)).
- `System.Runtime.CompilerServices.CollectionBuilderAttribute` ([§23.5.12](attributes.md#23512-the-collectionbuilder-attribute)), which designates a collection type as having a collection-creation method.
- `System.Runtime.CompilerServices.InlineArrayAttribute` ([§23.5.13](attributes.md#23513-the-inlinearray-attribute)), which marks a struct type as an inline array type ([§16.6](structs.md#166-inline-arrays)).

The Nullable static analysis attributes ([§23.5.7](attributes.md#2357-code-analysis-attributes)) can improve the correctness of warnings generated for nullabilities and null states ([§8.9.5](types.md#895-nullabilities-and-null-states)).

An execution environment may provide additional implementation-defined attributes that affect the execution of a C# program.

### 23.5.2 The AttributeUsage attribute

The attribute `AttributeUsage` is used to describe the manner in which the attribute class can be used.

A class that is decorated with the `AttributeUsage` attribute shall derive from `System.Attribute`, either directly or indirectly. Otherwise, a compile-time error occurs.

> *Note*: For an example of using this attribute, see [§23.2.2](attributes.md#2322-attribute-usage). *end note*

### 23.5.3 The Conditional attribute

#### 23.5.3.1 General

The attribute `Conditional` enables the definition of ***conditional method***s, ***conditional local function***s, and ***conditional attribute class***es.

#### 23.5.3.2 Conditional methods

A method decorated with the `Conditional` attribute is a conditional method. Each conditional method is thus associated with the conditional compilation symbols declared in its `Conditional` attributes.

> *Example*:
>
> <!-- Example: {template:"standalone-lib", name:"ConditionalMethods1", replaceEllipsis:true} -->
> ```csharp
> class Eg
> {
>     [Conditional("ALPHA")]
>     [Conditional("BETA")]
>     public static void M()
>     {
>         // ...
>     }
> }
> ```
>
> declares `Eg.M` as a conditional method associated with the two conditional compilation symbols `ALPHA` and `BETA`.
>
> *end example*

A call to a conditional method is included if one or more of its associated conditional compilation symbols is defined at the point of call, otherwise the call is omitted.

A conditional method is subject to the following restrictions:

- The conditional method shall be a method in a *class_declaration* or *struct_declaration*. A compile-time error occurs if the `Conditional` attribute is specified on a method in an interface declaration.
- The conditional method shall not be an accessor of a property, indexer or event.
- The conditional method shall have a return type of `void`.
- The conditional method shall not be marked with the `override` modifier. A conditional method can be marked with the `virtual` modifier, however. Overrides of such a method are implicitly conditional, and shall not be explicitly marked with a `Conditional` attribute.
- The conditional method shall not be an implementation of an interface method. Otherwise, a compile-time error occurs.
- The parameters of the conditional method shall not be output parameters.

> *Note*: Attributes with an `AttributeUsage` ([§23.2.2](attributes.md#2322-attribute-usage))  including `AttributeTargets.Method` can normally be applied to accessors of properties, indexers and events. The restrictions above prohibit this usage of the `Conditional` attribute. *end note*

In addition, a compile-time error occurs if a delegate is created from a conditional method.

> *Example*: The example
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ConditionalMethods2"} -->
> ```csharp
> #define DEBUG
> using System;
> using System.Diagnostics;
>
> class Class1
> {
>     [Conditional("DEBUG")]
>     public static void M()
>     {
>         Console.WriteLine("Executed Class1.M");
>     }
> }
>
> class Class2
> {
>     public static void Test()
>     {
>         Class1.M();
>     }
> }
> ```
>
> declares `Class1.M` as a conditional method. `Class2`’s `Test` method calls this method. Since the conditional compilation symbol `DEBUG` is defined, if `Class2.Test` is called, it will call `M`. If the symbol `DEBUG` had not been defined, then `Class2.Test` would not call `Class1.M`.
>
> *end example*

It is important to understand that the inclusion or exclusion of a call to a conditional method is controlled by the conditional compilation symbols at the point of the call.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib", name:"ConditionalMethods3"} -->
> ```csharp
> // File Class1.cs:
> using System;
> using System.Diagnostics;
> class Class1
> {
>     [Conditional("DEBUG")]
>     public static void F()
>     {
>         Console.WriteLine("Executed Class1.F");
>     }
> }
> 
> // File Class2.cs:
> #define DEBUG
> class Class2
> {
>     public static void G()
>     {
>         Class1.F(); // F is called
>     }
> }
> 
> // File Class3.cs:
> #undef DEBUG
> class Class3
> {
>     public static void H()
>     {
>         Class1.F(); // F is not called
>     }
> }
> ```
>
> the classes `Class2` and `Class3` each contain calls to the conditional method `Class1.F`, which is conditional based on whether or not `DEBUG` is defined. Since this symbol is defined in the context of `Class2` but not `Class3`, the call to `F` in `Class2` is included, while the call to `F` in `Class3` is omitted.
>
> *end example*

The use of conditional methods in an inheritance chain can be confusing. Calls made to a conditional method through `base`, of the form `base.M`, are subject to the normal conditional method call rules.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-console", name:"ConditionalMethods4", expectedOutput:["Class2.M executed"]} -->
> ```csharp
> // File Class1.cs
> using System;
> using System.Diagnostics;
> class Class1
> {
>     [Conditional("DEBUG")]
>     public virtual void M() => Console.WriteLine("Class1.M executed");
> }
> 
> // File Class2.cs
> class Class2 : Class1
> {
>     public override void M()
>     {
>         Console.WriteLine("Class2.M executed");
>         base.M(); // base.M is not called!
>     }
> }
> 
> // File Class3.cs
> #define DEBUG
> class Class3
> {
>     public static void Main()
>     {
>         Class2 c = new Class2();
>         c.M(); // M is called
>     }
> }
> ```
>
> `Class2` includes a call to the `M` defined in its base class. This call is omitted because the base method is conditional based on the presence of the symbol `DEBUG`, which is undefined. Thus, the method writes to the console “`Class2.M executed`” only. Judicious use of *pp_declaration*s can eliminate such problems.
>
> *end example*

#### 23.5.3.3 Conditional local functions

A static local function may be made conditional in the same sense as a conditional method ([§23.5.3.2](attributes.md#23532-conditional-methods)).

A compile time error occurs if a non-static local function is made conditional.

#### 23.5.3.4 Conditional attribute classes

An attribute class ([§23.2](attributes.md#232-attribute-classes)) decorated with one or more `Conditional` attributes is a conditional attribute class. A conditional attribute class is thus associated with the conditional compilation symbols declared in its `Conditional` attributes.

> *Example*:
>
> <!-- Example: {template:"standalone-lib", name:"ConditionalAttributeClasses1"} -->
> ```csharp
> [Conditional("ALPHA")]
> [Conditional("BETA")]
> public class TestAttribute : Attribute {}
> ```
>
> declares `TestAttribute` as a conditional attribute class associated with the conditional compilations symbols `ALPHA` and `BETA`.
>
> *end example*

Attribute specifications ([§23.3](attributes.md#233-attribute-specification)) of a conditional attribute are included if one or more of its associated conditional compilation symbols is defined at the point of specification, otherwise the attribute specification is omitted.

It is important to note that the inclusion or exclusion of an attribute specification of a conditional attribute class is controlled by the conditional compilation symbols at the point of the specification.

> *Example*: In the example
>
> <!-- Example: {template:"standalone-lib", name:"ConditionalAttributeClasses2"} -->
> ```csharp
> // File Test.cs:
> using System;
> using System.Diagnostics;
> [Conditional("DEBUG")]
> public class TestAttribute : Attribute {}
> 
> // File Class1.cs:
> #define DEBUG
> [Test] // TestAttribute is specified
> class Class1 {}
> 
> // File Class2.cs:
> #undef DEBUG
> [Test] // TestAttribute is not specified
> class Class2 {}
> ```
>
> the classes `Class1` and `Class2` are each decorated with attribute `Test`, which is conditional based on whether or not `DEBUG` is defined. Since this symbol is defined in the context of `Class1` but not `Class2`, the specification of the Test attribute on `Class1` is included, while the specification of the `Test` attribute on `Class2` is omitted.
>
> *end example*

### 23.5.4 The Obsolete attribute

The attribute `Obsolete` is used to mark types and members of types that should no longer be used.

If a program uses a type or member that is decorated with the `Obsolete` attribute, a compiler shall issue a warning or an error. Specifically, a compiler shall issue a warning if no error parameter is provided, or if the error parameter is provided and has the value `false`. A compiler shall issue an error if the error parameter is specified and has the value `true`.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-console", name:"ObsoleteAttribute", expectedWarnings:["CS0618","CS0618"]} -->
> ```csharp
> [Obsolete("This class is obsolete; use class B instead")]
> class A
> {
>     public void F() {}
> }
>
> class B
> {
>     public void F() {}
> }
>
> class Test
> {
>     static void Main()
>     {
>         A a = new A(); // Warning
>         a.F();
>     }
> }
> ```
>
> the class `A` is decorated with the `Obsolete` attribute. Each use of `A` in `Main` results in a warning that includes the specified message, “This class is obsolete; use class `B` instead”.
>
> *end example*

### 23.5.5 The AsyncMethodBuilder attribute

This attribute is described in [§15.14.1](classes.md#15141-general).

### 23.5.6 Caller-info attributes

#### 23.5.6.1 General

For purposes such as logging and reporting, it is sometimes useful for a function member to obtain certain compile-time information about the calling code. The caller-info attributes provide a way to pass such information transparently.

When an optional parameter is annotated with one of the caller-info attributes, omitting the corresponding argument in a call does not necessarily cause the default parameter value to be substituted. Instead, if the specified information about the calling context is available, that information will be passed as the argument value.

> *Example*:
>
> <!-- Example: {template:"code-in-class-lib", name:"CallerInfoAttributes"} -->
> ```csharp
> public void Log(
>     [CallerLineNumber] int line = -1,
>     [CallerFilePath] string path = null,
>     [CallerMemberName] string name = null
> )
> {
>     Console.WriteLine((line < 0) ? "No line" : "Line "+ line);
>     Console.WriteLine((path == null) ? "No file path" : path);
>     Console.WriteLine((name == null) ? "No member name" : name);
> }
> ```
>
> A call to `Log()` with no arguments would print the line number and file path of the call, as well as the name of the member within which the call occurred.
>
> *end example*

Caller-info attributes can occur on optional parameters anywhere, including in delegate declarations. However, the specific caller-info attributes have restrictions on the types of the parameters they can attribute, so that there will always be an implicit conversion from a substituted value to the parameter type.

It is an error to have the same caller-info attribute on a parameter of both the defining and implementing part of a partial method or partial indexer declaration. Only caller-info attributes in the defining part are applied, whereas caller-info attributes occurring only in the implementing part are ignored.

Caller information does not affect overload resolution. As the attributed optional parameters are still omitted from the source code of the caller, overload resolution ignores those parameters in the same way it ignores other omitted optional parameters ([§12.6.4](expressions.md#1264-overload-resolution)).

Caller information is only substituted when a function is explicitly invoked in source code. Implicit invocations such as implicit parent constructor calls do not have a source location and will not substitute caller information. Also, calls that are dynamically bound will not substitute caller information. When a caller-info attributed parameter is omitted in such cases, the specified default value of the parameter is used instead.

One exception is query expressions. These are considered syntactic expansions, and if the calls they expand to omit optional parameters with caller-info attributes, caller information will be substituted. The location used is the location of the query clause which the call was generated from.

If more than one caller-info attribute is specified on a given parameter, they are recognized in the following order: `CallerLineNumber`, `CallerFilePath`, `CallerMemberName`, `CallerArgumentExpression`. Consider the following parameter declaration:

```csharp
[CallerMemberName, CallerFilePath, CallerLineNumber] object p = ...
```

`CallerLineNumber` takes precedence, and the other three attributes are ignored. If `CallerLineNumber` were omitted, `CallerFilePath` would take precedence, and `CallerMemberName` and `CallerArgumentExpression` would be ignored. The lexical ordering of these attributes is irrelevant.

#### 23.5.6.2 The CallerLineNumber attribute

The attribute `System.Runtime.CompilerServices.CallerLineNumberAttribute` is allowed on optional parameters when there is a standard implicit conversion ([§10.4.2](conversions.md#1042-standard-implicit-conversions)) from the constant value `int.MaxValue` to the parameter’s type. This ensures that any non-negative line number up to that value can be passed without error.

If a function invocation from a location in source code omits an optional parameter with the `CallerLineNumberAttribute`, then a numeric literal representing that location’s line number is used as an argument to the invocation instead of the default parameter value.

If the invocation spans multiple lines, the line chosen is implementation-dependent.

The line number may be affected by `#line` directives ([§6.5.8](lexical-structure.md#658-line-directives)).

#### 23.5.6.3 The CallerFilePath attribute

The attribute `System.Runtime.CompilerServices.CallerFilePathAttribute` is allowed on optional parameters when there is a standard implicit conversion ([§10.4.2](conversions.md#1042-standard-implicit-conversions)) from `string` to the parameter’s type.

If a function invocation from a location in source code omits an optional parameter with the `CallerFilePathAttribute`, then a UTF-16 string literal representing that location’s file path is used as an argument to the invocation instead of the default parameter value.

The format of the file path is implementation-dependent.

The file path may be affected by `#line` directives ([§6.5.8](lexical-structure.md#658-line-directives)).

#### 23.5.6.4 The CallerMemberName attribute

The attribute `System.Runtime.CompilerServices.CallerMemberNameAttribute` is allowed on optional parameters when there is a standard implicit conversion ([§10.4.2](conversions.md#1042-standard-implicit-conversions)) from `string` to the parameter’s type.

If a function invocation from a location within the body of a function member or within an attribute applied to the function member itself or its return type, parameters or type parameters in source code omits an optional parameter with the `CallerMemberNameAttribute`, then a UTF-16 string literal representing the name of that member is used as an argument to the invocation instead of the default parameter value. (In the case of a function invocation from a top-level statement ([§7.1.3](basic-concepts.md#713-using-top-level-statements)), the member name is that generated by the implementation.)

For invocations that occur within generic methods, only the method name itself is used, without the type parameter list.

For invocations that occur within explicit interface member implementations, only the method name itself is used, without the preceding interface qualification.

For invocations that occur within property or event accessors, the member name used is that of the property or event itself.

For invocations that occur within indexer accessors, the member name used is that supplied by an `IndexerNameAttribute` ([§23.6](attributes.md#236-attributes-for-interoperation)) on the indexer member, if present, or the default name `Item` otherwise.

For invocations that occur within field or event initializers, the member name used is the name of the field or event being initialized.

For invocations that occur within declarations of instance constructors, static constructors, finalizers and operators the member name used is implementation-dependent.

For an invocation that occurs within a local function or an anonymous function, the name of the member method that calls that function is used.

> *Example*: Consider the following:
>
> <!-- Example: {template:"standalone-console", name:"CallerMemberName1", inferOutput:true} -->
> ```csharp
> class Program
> {
>     static void Main()
>     {
>         F1();
>         Action anonymousFunction = () => F2();
>         anonymousFunction();
>
>         void F1([CallerMemberName] string? name = null)
>         {
>             Console.WriteLine($"F1 MemberName: |{name}|");
>             F2();
>         }
>
>         static void F2([CallerMemberName] string? name = null)
>         {
>             Console.WriteLine($"F2 MemberName: |{name}|");
>         }
>     }
> }
> ```
>
> which produces the output
>
> ```console
> F1 MemberName: |Main|
> F2 MemberName: |Main|
> F2 MemberName: |Main|
> ```
>
> This attribute supplies the name of the calling function member, which for local function `F1` is the method `Main`. And even though `F2` is called by `F1`, a local function is *not* a function member, so the reported caller of that invocation of `F2` is also `Main`. Similarly, when `F2` is called by the anonymous function assigned to `anonymousFunction`, the reported caller is the method `Main`, which calls that anonymous function. *end example*

#### 23.5.6.5 The CallerArgumentExpression attribute

The attribute `System.Runtime.CompilerServices.CallerArgumentExpressionAttribute` is applied to a *target parameter*, and can result in the capture of the source-code text of a sibling parameter’s argument as a string, referred to here as the *captured string*.

Except when it is the first parameter in an extension method, the target parameter shall have a *default_argument*.

Consider the following method declaration:

<!-- Example: {template:"standalone-lib-without-using", name:"CallerArgumentAttr1"} -->
```csharp
using System;
using System.Runtime.CompilerServices;
#nullable enable
class Test
{
    public static void M(int val = 0, [CallerArgumentExpression("val")] string? text = null)
    {
        Console.WriteLine($"val = {val}, text = <{text}>");
    }
}
```

in which the target parameter is `text` and the sibling parameter is `val`, whose corresponding argument’s source-code text can be captured in `text` when `M` is called.

The attribute constructor takes an argument of type `string`. That string

- Shall contain the name of a sibling parameter; otherwise, the attribute is ignored.
- Shall omit the leading `@` from a parameter name having that prefix.

A *parameter_list* may contain multiple target parameters.

The type of the target parameter shall have a standard conversion from `string`.

> *Note:* This means no user-defined conversions from `string` are allowed, and in practice means the type of such a parameter must be `string`, `object`, or an interface implemented by `string`. *end note*

If an explicit argument is passed for the target parameter, no string is captured, and that parameter takes on that argument’s value. Otherwise, the text for the argument corresponding to the sibling parameter is converted to a captured string, according to the following rules:

- Leading and trailing white space is removed both before and after any outermost grouping parentheses are removed.
- All outermost grouping parentheses are removed both before and after any leading and trailing white space is removed.
- All other *input_element*s are retained verbatim (including white space, comments, *Unicode_Escape_Sequence*s, and `@` prefixes on identifiers).

The captured string is then passed as the argument corresponding to the target parameter. However, if the argument for the sibling parameter is omitted, the target parameter takes on its *default_argument* value.

> *Example*: Given the declaration of `M` above, consider the following calls to `M`:
>
> <!-- Example: {template:"standalone-console", name:"CallerArgumentAttr2", inferOutput:true, additionalFiles:["CallerArgumentAttrM.cs"]} -->
> ```csharp
> Test.M();
> Test.M(123);
> Test.M(123, null);
> Test.M(123, "xyz");
> Test.M(  1  +      2 );
> Test.M(( ( (123) + 0) ) );
> int local = 10;
> Test.M(l\u006fcal /*...*/ + // xxx
>   5);
> ```
>
> the output produced is
>
> ```console
> val = 0, text = <>
> val = 123, text = <123>
> val = 123, text = <>
> val = 123, text = <xyz>
> val = 3, text = <1  +      2>
> val = 123, text = <(123) + 0>
> val = 15, text = <l\u006fcal /*...*/ + // xxx
>   5>
> ```
>
> *end example*

### 23.5.7 Code analysis attributes

#### 23.5.7.1 General

The attributes in this subclause are used to provide additional information to support a compiler that provides nullability and null-state diagnostics ([§8.9.5](types.md#895-nullabilities-and-null-states)). A compiler is not required to perform any null-state diagnostics. The presence or absence of these attributes do not affect the language nor the behavior of a program. A compiler that does not provide null-state diagnostics shall read and ignore the presence of these attributes. A compiler that provides null-state diagnostics shall use the meaning defined in this subclause for any of these attributes which it uses to inform its diagnostics.

The code-analysis attributes are declared in namespace `System.Diagnostics.CodeAnalysis`.

**Attribute**  | **Meaning**
------------------  | ------------------
`AllowNull` ([§23.5.7.2](attributes.md#23572-the-allownull-attribute))  | A non-nullable argument may be null.
`DisallowNull` ([§23.5.7.3](attributes.md#23573-the-disallownull-attribute))  | A nullable argument should never be null.
`MaybeNull` ([§23.5.7.6](attributes.md#23576-the-maybenull-attribute))  | A non-nullable return value may be null.
`NotNull` ([§23.5.7.10](attributes.md#235710-the-notnull-attribute))  | A nullable return value will never be null.
`MaybeNullWhen` ([§23.5.7.7](attributes.md#23577-the-maybenullwhen-attribute))  | A non-nullable argument may be null when the method returns the specified `bool` value.
`NotNullWhen` ([§23.5.7.12](attributes.md#235712-the-notnullwhen-attribute))  | A nullable argument won’t be null when the method returns the specified `bool` value.
`NotNullIfNotNull` ([§23.5.7.11](attributes.md#235711-the-notnullifnotnull-attribute))  | A return value isn’t null if the argument for the specified parameter isn’t null.
`MemberNotNull` ([§23.5.7.8](attributes.md#23578-the-membernotnull-attribute))  | The listed member won’t be null when the method returns.
`MemberNotNullWhen` ([§23.5.7.9](attributes.md#23579-the-membernotnullwhen-attribute))  | The listed member won’t be null when the method returns the specified `bool` value.
`DoesNotReturn` ([§23.5.7.4](attributes.md#23574-the-doesnotreturn-attribute))  | This method never returns.
`DoesNotReturnIf` ([§23.5.7.5](attributes.md#23575-the-doesnotreturnif-attribute))  | This method never returns if the associated `bool` parameter has the specified value.

The following subclauses in [§23.5.7](attributes.md#2357-code-analysis-attributes) are conditionally normative.

#### 23.5.7.2 The AllowNull attribute

Specifies that a null value is allowed as an input even if the corresponding type disallows it.

> *Example*: Consider the following read/write property that never returns `null` because it has a reasonable default value. However, a user can give null to the set accessor to set the property to that default value.
>
> <!-- Example: {template:"standalone-lib", name:"AllowNullAttribute", replaceEllipsis:true, customEllipsisReplacements:["\"XYZ\""]} -->
> ```csharp
> #nullable enable
> public class X
> {
>     [AllowNull]
>     public string ScreenName
>     {
>         get => _screenName;
>         set => _screenName = value ?? GenerateRandomScreenName();
>     }
>     private string _screenName = GenerateRandomScreenName();
>     private static string GenerateRandomScreenName() => ...;
> }
> ```
>
> Given the following use of that property’s set accessor
>
> ```csharp
> var v = new X();
> v.ScreenName = null;   // may warn without attribute AllowNull
> ```
>
> without the attribute, a compiler may generate a warning because the non-nullable-typed property appears to be set to a null value. The presence of the attribute suppresses that warning. *end example*

#### 23.5.7.3 The DisallowNull attribute

Specifies that a null value is disallowed as an input even if the corresponding type allows it.

> *Example*: Consider the following property in which null is the default value, but clients can only set it to a non-null value.
>
> <!-- Example: {template:"standalone-lib", name:"DisallowNullAttribute"} -->
> ```csharp
> #nullable enable
> public class X
> {
>     [DisallowNull]
>     public string? ReviewComment
>     {
>         get => _comment;
>         set => _comment = value ?? throw new ArgumentNullException(nameof(value),
>            "Cannot set to null");
>     }
>     private string? _comment = default;
> }
> ```
>
> The get accessor could return the default value of `null`, so a compiler may warn that it must be checked before access. Furthermore, it warns callers that, even though it could be null, callers should not explicitly set it to null. *end example*

#### 23.5.7.4 The DoesNotReturn attribute

Specifies that a given method never returns.

> *Example*: Consider the following:
>
> <!-- Example: {template:"standalone-lib", name:"DoesNotReturnAttribute"} -->
> ```csharp
> public class X
> {
>     [DoesNotReturn]
>     private void FailFast() =>
>         throw new InvalidOperationException();
>
>     public void SetState(object? containedField)
>     {
>         if ((!isInitialized) || (containedField == null))
>         {
>             FailFast();
>         }
>         // null check not needed.
>         _field = containedField;
>     }
> 
>     private bool isInitialized = false;
>     private object _field;
> }
> ```
>
> The presence of the attribute helps a compiler in a number of ways. First, a compiler can issue a warning if there is a path where the method can exit without throwing an exception. Second, a compiler can suppress nullable warnings in any code after a call to that method, until an appropriate catch clause is found. Third, the unreachable code will not affect any null states.
>
> The attribute does not change reachability ([§13.2](statements.md#132-end-points-and-reachability)) or definite assignment ([§9.4](variables.md#94-definite-assignment)) analysis based on the presence of this attribute. It is used only to impact nullability warnings. *end example*

#### 23.5.7.5 The DoesNotReturnIf attribute

Specifies that a given method never returns if the associated `bool` parameter has the specified value.

> *Example*: Consider the following:
>
> <!-- Example: {template:"standalone-lib", name:"DoesNotReturnIfAttribute", expectedWarnings:["CS0414"]}  -->
> ```csharp
> #nullable enable
> public class X
> {
>     private void ThrowIfNull([DoesNotReturnIf(true)] bool isNull, string argumentName)
>     {
>         if (isNull)
>         {
>             throw new ArgumentException(argumentName,
>               $"argument {argumentName} cannot be null");
>         }
>     }
>
>     public void SetFieldState(object containedField)
>     {
>         ThrowIfNull(containedField == null, nameof(containedField));
>         // unreachable code when "isInitialized" is false:
>         _field = containedField;
>     }
> 
>     private bool isInitialized = false;
>     private object _field = default!;
> }
> ```
>
> *end example*

#### 23.5.7.6 The MaybeNull attribute

Specifies that a non-nullable return value may be null.

> *Example*: Consider the following generic method:
>
> <!-- Example: {template:"code-in-class-lib", name:"MaybeNull2Attribute", replaceEllipsis:true, customEllipsisReplacements: ["return default;"]} -->
> ```csharp
> #nullable enable
> [return: MaybeNull]
> public T Find<T>(IEnumerable<T> sequence, Func<T, bool> predicate) { ... }
> ```
>
> Without the attribute the compiler might generate a warning if the method could return `null`. The presence of the attribute suppresses that warning. *end example*

#### 23.5.7.7 The MaybeNullWhen attribute

Specifies that a non-nullable argument may be `null` when the method returns the specified `bool` value. This is similar to the `MaybeNull` attribute ([§23.5.7.6](attributes.md#23576-the-maybenull-attribute)), but includes a parameter for the specified return value.

#### 23.5.7.8 The MemberNotNull attribute

Specifies that the given member won’t be ``null`` when the method returns.

> *Example*: A helper method may include the ``MemberNotNull`` attribute to list any fields that are assigned to a non-null value in that method. A compiler that analyzes constructors to determine whether all non-nullable reference fields have been initialized may then use this attribute to discover which fields have been set by those helper methods. Consider the following example:
>
> <!-- Example: {template:"standalone-lib", name:"MemberNotNullAttribute"} -->
> ``````csharp
> #nullable enable
> public class Container
> {
>     private string _uniqueIdentifier; // must be initialized.
>     private string? _optionalMessage;
>
>     public Container()
>     {
>         Helper();
>     }
>
>     public Container(string message)
>     {
>         Helper();
>         _optionalMessage = message;
>     }
>
>     [MemberNotNull(nameof(_uniqueIdentifier))]
>     private void Helper()
>     {
>         _uniqueIdentifier = DateTime.Now.Ticks.ToString();
>     }
> }
> ``````
>
> Multiple field names may be given as arguments to the attribute’s constructor. *end example*

#### 23.5.7.9 The MemberNotNullWhen attribute

Specifies that the listed member won’t be ``null`` when the method returns the specified ``bool`` value.

> *Example*: This attribute is like `MemberNotNull` ([§23.5.7.8](attributes.md#23578-the-membernotnull-attribute)) except that `MemberNotNullWhen` takes a `bool` argument. `MemberNotNullWhen` is intended for use in situations in which a helper method returns a `bool` indicating whether it initialized fields. *end example*

#### 23.5.7.10 The NotNull attribute

Specifies that a nullable value will never be `null` if the method returns (rather than throwing).

> *Example*: Consider the following:
>
> <!-- Example: {template:"code-in-class-lib", name:"NotNullAttribute"} -->
> ```csharp
> #nullable enable
> public static void ThrowWhenNull([NotNull] object? value,
>   string valueExpression = "") =>
>     _ = value ?? throw new ArgumentNullException(valueExpression);
>
> public static void LogMessage(string? message)
> {
>     ThrowWhenNull(message, nameof(message));
>     Console.WriteLine(message.Length);
> }
> ```
>
> When nullable reference types are enabled, method `ThrowWhenNull` compiles without warnings. When that method returns, the `value` argument is guaranteed to be not `null`. However, it is acceptable to call `ThrowWhenNull` with a null reference. *end example*

#### 23.5.7.11 The NotNullIfNotNull attribute

Specifies that a return value is not `null` if the argument for the specified parameter is not `null`.

> *Example*: The null state of a return value could depend on the null state of one or more arguments. To assist a compiler’s analysis when a method always returns a non-null value when certain arguments are not `null` the `NotNullIfNotNull` attribute may be used. Consider the following method:
>
> <!-- Example: {template:"code-in-class-lib", name:"NotNullIfNotNull1Attribute", replaceEllipsis:true, customEllipsisReplacements: ["return \"\";"]} -->
> ```csharp
> #nullable enable
> string GetTopLevelDomainFromFullUrl(string url) { ... }
> ```
>
> If the `url` argument is not `null`, `null` is not returned. When nullable references are enabled, that signature works correctly, provided the API never accepts a null argument. However, if the argument could be null, then the return value could also be null. To express that contract correctly, annotate this method as follows:
>
> <!-- Example: {template:"code-in-class-lib", name:"NotNullIfNotNull2Attribute", replaceEllipsis:true, customEllipsisReplacements: ["return \"\";"]} -->
> ```csharp
> #nullable enable
> [return: NotNullIfNotNull("url")]
> string? GetTopLevelDomainFromFullUrl(string? url) { ... }
> ```
>
> *end example*

#### 23.5.7.12 The NotNullWhen attribute

Specifies that a nullable argument will not be `null` when the method returns the specified `bool` value.

> *Example*: The library method `String.IsNullOrEmpty(String)` returns `true` when the argument is `null` or an empty string. It is a form of null-check: Callers do not need to null-check the argument if the method returns `false`. To make a method like this nullable aware, make the parameter type a nullable reference type, and add the NotNullWhen attribute:
>
> <!-- Example: {template:"code-in-class-lib", name:"NotNullWhenAttribute", replaceEllipsis:true, customEllipsisReplacements: ["return default;"]} -->
> ```csharp
> #nullable enable
> bool IsNullOrEmpty([NotNullWhen(false)] string? value) { ... }
> ```
>
> *end example*

### 23.5.8 The UnscopedRef attribute

There are several cases in which a ref is treated as being implicitly scoped ([§9.7.3](variables.md#973-the-scoped-modifier)); that is, the ref is not allowed to escape a method. For example:

- `this` for struct instance methods.
- ref parameters that refer to ref struct types.
- out parameters.

This attribute is used in those situations where the ref should be allowed to escape.

This attribute may can be applied to any `ref` and it changes the ref-safe-context to be one level wider than its default. For example:

| UnscopedRef applied to | Original ref-safe-context | New ref-safe-context |
| --- | --- | --- |
| instance member | function-member | return-only |
| `in` / `ref` parameter | return-only | caller-context |
| `out` parameter | function-member | return-only |

When applying this attribute to an instance method of a struct it modifies the implicit `this` parameter; that is, `this` acts as an unannotated `ref` of the same type.

An instance method or property annotated with `[UnscopedRef]` has the ref-safe-context of `this` set to the *caller-context*.

A member annotated with `[UnscopedRef]` may not implement an interface.

It is an error to use `[UnscopedRef]` on

- A member that is not declared on a `struct`.
- A `static` member, `init` member, or constructor on a `struct`.
- A parameter marked `scoped`.
- A parameter passed by value.
- A parameter passed by reference that is not implicitly scoped.

See [§9.7.3](variables.md#973-the-scoped-modifier) for more information.

### 23.5.9 The EnumeratorCancellation attribute

Specifies the parameter representing the `CancellationToken` for an asynchronous iterator ([§15.15](classes.md#1515-synchronous-and-asynchronous-iterators)). The argument for this parameter shall be combined with the argument passed to `IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken)`. This combined token shall be polled by `IAsyncEnumerator<T>.MoveNextAsync()` ([§15.15.5.2](classes.md#151552-advance-the-enumerator)). The tokens shall be combined into a single token as if by `CancellationToken.CreateLinkedTokenSource` and its `Token` property. The combined token will be canceled if either of the two source tokens are canceled. The combined token is seen as the argument to the asynchronous iterator method ([§15.15](classes.md#1515-synchronous-and-asynchronous-iterators)) in the body of that method.

It is an error if the `System.Runtime.CompilerServices.EnumeratorCancellation` attribute is applied to more than one parameter. The compiler may produce a warning if:

- The `EnumeratorCancellation` attribute is applied to a parameter of a type other than `CancellationToken`,
- or if the `EnumeratorCancellation` attribute is applied to a parameter on a method that is not an asynchronous iterator ([§15.15](classes.md#1515-synchronous-and-asynchronous-iterators)),
- or if the `EnumeratorCancellation` attribute is applied to a parameter on a method that returns an asynchronous enumerator interface ([§15.15.2](classes.md#15152-enumerator-interfaces)) rather than an asynchronous enumerable interface ([§15.15.3](classes.md#15153-enumerable-interfaces)).

The iterator will not have access to the `CancellationToken` argument for `GetAsyncEnumerator` when no attributes have this parameter.

> *Example*: The method `GetStringsAsync()` is an asynchronous iterator. Before doing any work to retrieve the next value, it checks the cancellation token to determine if the iteration should be canceled. If cancellation is requested, no further action is taken.
>
> <!-- Example: {template:"code-in-class-lib", name:"AsyncEnumeratorCancellation"} -->
> ```csharp
> public static async Task ExampleCombination()
> {
>     var sourceOne = new CancellationTokenSource();
>     var sourceTwo = new CancellationTokenSource();
>     await using (IAsyncEnumerator<string> enumerator =
>         GetStringsAsync(sourceOne.Token).GetAsyncEnumerator(sourceTwo.Token))
>     {
>         while (await enumerator.MoveNextAsync())
>         {
>             string number = enumerator.Current;
>             if (number == "8") sourceOne.Cancel();
>             if (number == "5") sourceTwo.Cancel();
>             Console.WriteLine(number);
>         }
>     }
> }
>
> static async IAsyncEnumerable<string> GetStringsAsync(
>   [EnumeratorCancellation] CancellationToken token)
> {
>     for (int i = 0; i < 10; i++)
>     {
>         if (token.IsCancellationRequested) yield break;
>         await Task.Delay(1000, token);
>         yield return i.ToString();
>     }
> }
> ```
>
> *end example*

### 23.5.10 The ModuleInitializer attribute

The attribute `ModuleInitializer` is used to mark a method as a ***module initializer***. Such a method is called during initialization of the containing module. A module may have multiple initializers, which are called in an implementation-defined order.

There are no limitations on what code is permitted in a module initializer.

A module initializer shall have the following characteristics:

- The *method_modifier* `static`.
- No *parameter_list*.
- A *return_type* of `void`.
- No *type_parameter_list*.
- Not be declared inside a *class_declaration* having a *type_parameter_list*.
- Be accessible from the containing module (that is, have an access modifier `internal` or `public`).
- Not be a local function.

#### 23.5.10.1 Custom interpolated string expression handlers

##### 23.5.10.1.1 Declaring a custom handler

Consider the following program, which implements a simple message logger:

<!-- Example: {template:"standalone-console", name:"DeclCustomHandler1", inferOutput:true} -->
```csharp
using System;
public class Logger
{
    public void LogMessage(string msg)
    {
        Console.WriteLine(msg);
    }
}
public class Program
{
    static void Main()
    {
        var logger = new Logger();
        int val = 255;
        logger.LogMessage($"val = {{{val,4:X}}}; 2 * val = {2 * val}.");
    }
}
```

The output produced is, as follows:

```console
val = {  FF}; 2 * val = 510.
```

In the call to `LogMessage`, the target of the interpolated string expression argument is parameter `msg`, which has type `string`. As such, according to [§12.8.3](expressions.md#1283-interpolated-string-expressions), the default interpolated string expression handler is invoked. The following subclause ([§23.5.10.1.1](attributes.md#2351011-declaring-a-custom-handler)) shows how to use a custom handler.

In order to provide custom processing to the program above, a *custom interpolated string expression handler* is needed. Here then is the message logger with a custom handler added (which although it does nothing more than behave like the default handler, it provides the hooks for customization):

<!-- Example: {template:"standalone-console-without-using", name:"DeclCustomHandler2", inferOutput:true} -->
```csharp
using System;
using System.Text;
using System.Runtime.CompilerServices;

[InterpolatedStringHandler]
public ref struct LogInterpolatedStringHandler
{
    StringBuilder builder; // Storage for the built-up string
    public LogInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        builder = new StringBuilder(literalLength);
    }
    public void AppendLiteral(string s)
    {
        builder.Append(s);
    }
    public void AppendFormatted<T>(T t)
    {
        builder.Append(t?.ToString());
    }
    public void AppendFormatted<T>(T t, string format) where T : IFormattable
    {
        builder.Append(t?.ToString(format, null));
    }
    public void AppendFormatted<T>(T t, int alignment, string format)
        where T : IFormattable
    {
        builder.Append(String.Format("{0" + "," + alignment + ":" + format + "}", t));
    }
    public override string ToString() => builder.ToString();
}

public class Logger
{
    public void LogMessage(string msg)
    {
        Console.WriteLine(msg);
    }
    public void LogMessage(LogInterpolatedStringHandler builder)
    {
        Console.WriteLine(builder.ToString());
    }
}

public class Program
{
    static void Main()
    {
        var logger = new Logger();
        int val = 255;
        logger.LogMessage($"val = {{{val,4:X}}}; 2 * val = {2 * val}.");
    }
}
```

The output produced is, as follows:

```console
val = {  FF}; 2 * val = 510.
```

A type having the attribute `System.Runtime.CompilerServices.InterpolatedStringHandlerAttribute` is said to be an *applicable interpolated string handler type*.

To qualify as a custom interpolated string expression handler, a class or struct type shall have the following characteristics:

- Be marked with the attribute `System.Runtime.CompilerServices.InterpolatedStringHandlerAttribute`.
- Have an accessible constructor whose first two parameters have type `int`. (Other parameters may follow, which are used to pass information to/from the handler. These are discussed in [§23.5.10.1.3](attributes.md#2351013-passing-information-tofrom-a-custom-handler). An optional final parameter may be declared to inhibit the handler from processing the interpolated string. This is discussed in [§23.5.10.1.2](attributes.md#2351012-inhibiting-a-custom-handler)).

When the compiler-generated code calls the constructor, the first parameter is set to the sum of the lengths of the interpolated string expression segments ([§12.8.3](expressions.md#1283-interpolated-string-expressions)) in the interpolated string expression, and the second parameter is set to the number of interpolations. (For `($"val = {{{val,4:X}}}; 2 * val = {2 * val}."`, these values are 21 and 2, respectively.)

- Have an accessible method with the signature `void AppendLiteral(string s)`, which is called to process a single interpolated string expression literal segment.
- Have a set of accessible overloaded methods called `AppendFormatted`, one of which is called to process a single interpolation, based on that interpolation’s content. Their signatures are, as follows:
  - `void AppendFormatted<T>(T t)`, which deals with interpolations having no explicit format or alignment, as in the case of `{2 * val}`.
  - `void AppendFormatted<T>(T t, string format) where T : System.IFormattable`, which deals with interpolations having an explicit format, but no alignment, as in the case of `{val:X4}`.
  - `void AppendFormatted<T>(T t, int alignment, string format) where T : System.IFormattable`, which deals with interpolations having an explicit format and alignment, as in the case of `{val,4:X}`.
- Have a public method with the signature `override string ToString()`, which returns the built string.

> *Note*: It is not a compile-time error to omit any of the `AppendFormatted` overloads, but if the handler is to be maximally robust, it should support all the formats recognized by the default handler. *end note*

The new overload of `LogMessage` takes a custom handler instead of `string`, and retrieves the string as formatted by that handler. When such overloads exist, if a corresponding handler exists and the interpolated string expression is not a constant ([§12.8.3](expressions.md#1283-interpolated-string-expressions)), the compiler generates code to call the one taking a handler. In such cases, the compiler generates code to

- call the handler constructor
- in lexical order within the interpolated string expression
  - pass each interpolated string expression segment to `AppendLiteral`
  - pass each interpolation to the appropriate `AppendFormatted` method.
- return the final string as the value of the interpolated string expression.
- execute the body of `LogMessage`.

##### 23.5.10.1.2 Inhibiting a custom handler

If a handler constructor has a final parameter of type `bool` that is an out parameter, when that constructor is called that parameter’s value is tested. If it is true, the behavior is as if that parameter were omitted. However, if it is false, the interpolated string expression is not processed further; that is, the handler is *inhibited*. Specifically, the interpolation expressions are not evaluated, and the methods `AppendLiteral` and `AppendFormatted` are not called.

``` csharp
public LogInterpolatedStringHandler(int literalLength, int formattedCount,
    out bool processString)
{
    if (some_condition)
    {
        processString = false;
        return;
    }
    else 
    {
        processString = true;
        // continue construction
    }
}
```

*Note*: The interpolations in an interpolated string expression may contain side effects (as result from `++`, `--`,  assignment, and some method calls). If a handler is inhibited, none of the side effects in the interpolated string expression are evaluated. If a handler is not inhibited, all of the side effects in the interpolated string expression are evaluated. *end note*

##### 23.5.10.1.3 Passing information to/from a custom handler

It can be useful to pass other information to, and receive information back from, the custom handler. This is done via the attribute `System.Runtime.CompilerServices.InterpolatedStringHandlerArgument`. Consider the following new overloads to the message logger program:

```csharp
public class Logger
{
    // …
    public void LogMessage(bool flag, int count,
        [InterpolatedStringHandlerArgument("count","flag","")] 
        LogInterpolatedStringHandler builder)
    {
        // …
    }
}

public ref struct LogInterpolatedStringHandler
{
    // …
    public LogInterpolatedStringHandler(int literalLength, int formattedCount,
        int count, bool flag, Logger logger)
    {
        // …
    }
}
```

Attribute `InterpolatedStringHandlerArgument` is applied to the handler parameter, which shall follow the declarations of the parameters that are to be passed to the handler. The attribute constructor argument shall be a comma-separated list of zero or more strings that name the parameters to be passed, along with their order. An empty string designates the instance from which the handler is being invoked. As such, the attribute constructor call above containing `"count","flag",""` requires a matching handler constructor. If the attribute constructor argument list is empty, the behavior is as if the attribute was omitted.

If an `out bool` parameter is also declared to allow the handler to be inhibited ([§23.5.10.1.2](attributes.md#2351012-inhibiting-a-custom-handler)) that parameter shall be the final one.

### 23.5.11 Required member attributes

#### 23.5.11.1 The SetsRequiredMembers attribute

This attribute indicates that the constructor it decorates sets all required members for the current type, so callers do not need to set any required members themselves. However, the compiler doesn’t verify that the constructor actually initializes all required members.

> *Example*:
>
> <!-- Example: {template:"standalone-lib", name:"SetsRequiredMembers", expectedErrors:["CS9035","CS9035"]} -->
> ```csharp
> public class Person
> {
>     public Person() { }
>
>     [SetsRequiredMembers]
>     public Person(string firstName, string lastName) =>
>         (FirstName, LastName) = (firstName, lastName);
>
>     public required string FirstName { get; init; }
>     public required string LastName { get; init; }
>
>     public int? Age { get; set; }
> }
>
> public class Student : Person
> {
>     public Student() : base()
>     {
>     }
>
>     [SetsRequiredMembers]
>     public Student(string firstName, string lastName) :
>         base(firstName, lastName)
>     {
>     }
>
>     public double GPA { get; set; }
> }
>
> public class Test
> {
>     public static void M()
>     {
>         var p1 = new Student(); // error: doesn't set required members
>         var p2 = new Student("Jane", "Williams"); // OK
>     }
> }
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: As the derived-type constructor `Student(string, string)` chains to the base-type constructor `Person(string, string)`, which has this attribute, the derived-type constructor must also have that attribute ([§15.11.1](classes.md#15111-general)). *end note*

#### 23.5.11.2 The RequiredMember attribute

This attribute indicates that the current type has one or more required members ([§15.7.1](classes.md#1571-general)), or that a specific member of that type is required. However, it is an error for this attribute to be used explicitly. Instead, the presence of the modifier `required` results in the type or member being treated as if it were decorated with this attribute.

### 23.5.12 The CollectionBuilder attribute

This attribute designates a collection type as having a collection-creation method ([§15.17.1](classes.md#15171-general)).

The constructor takes a builder type and the name of the method to be invoked to construct an instance of the collection type.

The attribute can be applied to a class, struct, ref struct, or interface. The attribute is not inherited although it can be applied to a base class or an abstract class.

The builder type shall be a non-generic class or struct.
### 23.5.13 The InlineArray attribute

The attribute `InlineArray` is used to identify a non-record struct as an inline array type. For further information and examples of its use, see [§16.6](structs.md#166-inline-arrays).

## 23.6 Attributes for interoperation

For interoperation with other languages, an indexer may be implemented using indexed properties. If no `IndexerName` attribute is present for an indexer, then the name `Item` is used by default. The `IndexerName` attribute enables a developer to override this default and specify a different name.

> *Example*: By default, an indexer’s name is `Item`. This can be overridden, as follows:
>
> <!-- Example: {template:"code-in-class-lib", name:"AttributesForInteroperation", replaceEllipsis:true, customEllipsisReplacements:["return 0;",""]} -->
> ```csharp
> [System.Runtime.CompilerServices.IndexerName("TheItem")]
> public int this[int index]
> {
>     get { ... }
>     set { ... }
> }
> ```
>
> Now, the indexer’s name is `TheItem`.
>
> *end example*
