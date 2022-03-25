# 19 Delegates

## 19.1 General

A delegate declaration defines a class that is derived from the class `System.Delegate`. A delegate instance encapsulates an ***invocation list***, which is a list of one or more methods, each of which is referred to as a ***callable entity***. For instance methods, a callable entity consists of an instance and a method on that instance. For static methods, a callable entity consists of just a method. Invoking a delegate instance with an appropriate set of arguments causes each of the delegate’s callable entities to be invoked with the given set of arguments.

> *Note*: An interesting and useful property of a delegate instance is that it does not know or care about the classes of the methods it encapsulates; all that matters is that those methods be compatible ([§19.4](delegates.md#194-delegate-compatibility)) with the delegate’s type. This makes delegates perfectly suited for “anonymous” invocation. *end note*

## 19.2 Delegate declarations

A *delegate_declaration* is a *type_declaration* ([§13.7](namespaces.md#137-type-declarations)) that declares a new delegate type.

```ANTLR
delegate_declaration
    : attributes? delegate_modifier* 'delegate' return_type identifier
      variant_type_parameter_list? '(' formal_parameter_list? ')'
      type_parameter_constraints_clause* ';'
    ;
    
delegate_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | unsafe_modifier   // unsafe code support
    ;
```

*unsafe_modifier* is defined in [§22.2](unsafe-code.md#222-unsafe-contexts).

It is a compile-time error for the same modifier to appear multiple times in a delegate declaration.

A delegate declaration shall not supply any *type_parameter_constraints_clause*s unless it also supplies a *variant_type_parameter_list*.

A delegate declaration that supplies a *variant_type_parameter_list* is a generic delegate declaration. Additionally, any delegate nested inside a generic class declaration or a generic struct declaration is itself a generic delegate declaration, since type arguments for the containing type shall be supplied to create a constructed type ([§8.4](types.md#84-constructed-types)).

The `new` modifier is only permitted on delegates declared within another type, in which case it specifies that such a delegate hides an inherited member by the same name, as described in [§14.3.5](classes.md#1435-the-new-modifier).

The `public`, `protected`, `internal`, and `private` modifiers control the accessibility of the delegate type. Depending on the context in which the delegate declaration occurs, some of these modifiers might not be permitted ([§7.5.2](basic-concepts.md#752-declared-accessibility)).

The delegate’s type name is *identifier*.

The optional *formal_parameter_list* specifies the parameters of the delegate, and *return_type* indicates the return type of the delegate.

The optional *variant_type_parameter_list* ([§17.2.3](interfaces.md#1723-variant-type-parameter-lists)) specifies the type parameters to the delegate itself.

The return type of a delegate type shall be either `void`, or output-safe ([§17.2.3.2](interfaces.md#17232-variance-safety)).

All the formal parameter types of a delegate type shall be input-safe ([§17.2.3.2](interfaces.md#17232-variance-safety)). In addition, any output or reference parameter types shall also be output-safe.

> *Note*: Output parameters are required to be input-safe due to common implementation restrictions. *end note*

Furthermore, each class type constraint, interface type constraint and type parameter constraint on any type parameters of the delegate shall be input-safe.

Delegate types in C# are name equivalent, not structurally equivalent.

> *Example*:
>
> ```csharp
> delegate int D1(int i, double d);
> delegate int D2(int c, double d);
> ```
>
> The delegate types `D1` and `D2` are two different types, so they are not interchangeable, despite their identical signatures. *end example*

Like other generic type declarations, type arguments shall be given to create a constructed delegate type. The parameter types and return type of a constructed delegate type are created by substituting, for each type parameter in the delegate declaration, the corresponding type argument of the constructed delegate type.

The only way to declare a delegate type is via a *delegate_declaration*. Every delegate type is a reference type that is derived from `System.Delegate`. The members required for every delegate type are detailed in [§19.3](delegates.md#193-delegate-members). Delegate types are implicitly `sealed`, so it is not permissible to derive any type from a delegate type. It is also not permissible to declare a non-delegate class type deriving from `System.Delegate`. `System.Delegate` is not itself a delegate type; it is a class type from which all delegate types are derived.

## 19.3 Delegate members

Every delegate type inherits members from the `Delegate` class as described in [§14.3.4](classes.md#1434-inheritance). In addition, every delegate type must provide a non-generic `Invoke` method whose parameter list matches the *formal_parameter_list* in the delegate declaration, and whose return type matches the *return_type* in the delegate declaration. The `Invoke` method shall be at least as accessible as the containing delegate type. Calling the `Invoke` method on a delegate type is semantically equivalent to using the delegate invocation syntax ([§19.6](delegates.md#196-delegate-invocation)) .

Implementations may define additional members in the delegate type.

Except for instantiation, any operation that can be applied to a class or class instance can also be applied to a delegate class or instance, respectively. In particular, it is possible to access members of the `System.Delegate` type via the usual member access syntax.

## 19.4 Delegate compatibility

A method or delegate type `M` is ***compatible*** with a delegate type `D` if all of the following are true:

- `D` and `M` have the same number of parameters, and each parameter in `D` has the same `ref` or `out` modifiers as the corresponding parameter in `M`.
- For each value parameter (a parameter with no `ref` or `out` modifier), an identity conversion ([§10.2.2](conversions.md#1022-identity-conversion)) or implicit reference conversion ([§10.2.8](conversions.md#1028-implicit-reference-conversions)) exists from the parameter type in `D` to the corresponding parameter type in `M`.
- For each `ref` or `out` parameter, the parameter type in `D` is the same as the parameter type in `M`.
- An identity or implicit reference conversion exists from the return type of `M` to the return type of `D`.

This definition of consistency allows covariance in return type and contravariance in parameter types.

> *Example*:
>
> ```csharp
> delegate int D1(int i, double d);
> delegate int D2(int c, double d);
> delegate object D3(string s);
> 
> class A
> {
>     public static int M1(int a, double b) {...}
> }
>
> class B
> {
>     public static int M1(int f, double g) {...}
>     public static void M2(int k, double l) {...}
>     public static int M3(int g) {...}
>     public static void M4(int g) {...}
>     public static object M5(string s) {...}
>     public static int[] M6(object o) {...}
> }
> ```
>
> The methods `A.M1` and `B.M1` are compatible with both the delegate types `D1` and `D2`, since they have the same return type and parameter list. The methods `B.M2`, `B.M3`, and `B.M4` are incompatible with the delegate types `D1` and `D2`, since they have different return types or parameter lists. The methods `B.M5` and `B.M6` are both compatible with delegate type `D3`. *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*:
>
> ```csharp
> delegate bool Predicate<T>(T value);
>
> class X
> {
>     static bool F(int i) {...}
>     static bool G(string s) {...}
> }
> ```
>
> The method `X.F` is compatible with the delegate type `Predicate<int>` and the method `X.G` is compatible with the delegate type `Predicate<string>`.  *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The intuitive meaning of delegate compatibility is that a method is compatible with a delegate type if every invocation of the delegate could be replaced with an invocation of the method without violating type safety, treating optional parameters and parameter arrays as explicit parameters. For example, in the following code:
>
> ```csharp
> delegate void Action<T>(T arg);
> 
> class Test
> {
>     static void Print(object value) => Console.WriteLine(value);
>
>     static void Main()
>     {
>         Action<string> log = Print;
>         log("text");
>     }
> }
> ```
>
> The `Print` method is compatible with the `Action<string>` delegate type because any invocation of an `Action<string>` delegate would also be a valid invocation of the `Print` method.
>
> If the signature of the `Print` method above were changed to `Print(object value, bool prependTimestamp = false)` for example, the `Print` method would no longer be compatible with `Action<string>` by the rules of this clause. *end note*

## 19.5 Delegate instantiation

An instance of a delegate is created by a *delegate_creation_expression* ([§11.7.15.6](expressions.md#117156-delegate-creation-expressions)), a conversion to a delegate type, delegate combination or delegate removal. The newly created delegate instance then refers to one or more of:

- The static method referenced in the *delegate_creation_expression*, or
- The target object (which cannot be `null`) and instance method referenced in the *delegate_creation_expression*, or
- Another delegate ([§11.7.15.6](expressions.md#117156-delegate-creation-expressions)).

> *Example*:
>
> ```csharp
> delegate void D(int x);
>
> class C
> {
>     public static void M1(int i) {...}
>     public void M2(int i) {...}
> }
>
> class Test
> {
>     static void Main()
>     {
>         D cd1 = new D(C.M1); // Static method
>         C t = new C();
>         D cd2 = new D(t.M2); // Instance method
>         D cd3 = new D(cd2);  // Another delegate
>     }
> }
> ```
>
> *end example*

The set of methods encapsulated by a delegate instance is called an *invocation list*. When a delegate instance is created from a single method, it encapsulates that method, and its invocation list contains only one entry. However, when two non-`null` delegate instances are combined, their invocation lists are concatenated—in the order left operand then right operand—to form a new invocation list, which contains two or more entries.

When a new delegate is created from a single delegate the resultant invocation list has just one entry, which is the source delegate ([§11.7.15.6](expressions.md#117156-delegate-creation-expressions)).

Delegates are combined using the binary `+` ([§11.9.5](expressions.md#1195-addition-operator)) and `+=` operators ([§11.18.3](expressions.md#11183-compound-assignment)). A delegate can be removed from a combination of delegates, using the binary `-` ([§11.9.6](expressions.md#1196-subtraction-operator)) and `-=` operators ([§11.18.3](expressions.md#11183-compound-assignment)). Delegates can be compared for equality ([§11.11.9](expressions.md#11119-delegate-equality-operators)).

> *Example*: The following example shows the instantiation of a number of delegates, and their corresponding invocation lists:
>
> ```csharp
> delegate void D(int x);
>
> class C
> {
>     public static void M1(int i) {...}
>     public static void M2(int i) {...}
> }
>
> class Test
> {
>     static void Main() 
>     {
>         D cd1 = new D(C.M1); // M1 - one entry in invocation list
>         D cd2 = new D(C.M2); // M2 - one entry
>         D cd3 = cd1 + cd2;   // M1 + M2 - two entries
>         D cd4 = cd3 + cd1;   // M1 + M2 + M1 - three entries
>         D cd5 = cd4 + cd3;   // M1 + M2 + M1 + M1 + M2 - five entries
>         D td3 = new D(cd3);  // [M1 + M2] - ONE entry in invocation
>                              // list, which is itself a list of two methods.
>         D td4 = td3 + cd1;   // [M1 + M2] + M1 - two entries
>         D cd6 = cd4 - cd2;   // M1 + M1 - two entries in invocation list
>         D td6 = td4 - cd2;   // [M1 + M2] + M1 - two entries in invocation list,
>                              // but still three methods called, M2 not removed.
>    }
> }
> ```
>
> When `cd1` and `cd2` are instantiated, they each encapsulate one method. When `cd3` is instantiated, it has an invocation list of two methods, `M1` and `M2`, in that order. `cd4`’s invocation list contains `M1`, `M2`, and `M1`, in that order. For `cd5`, the invocation list contains `M1`, `M2`, `M1`, `M1`, and `M2`, in that order.
>
> When `cd1` and `cd2` are instantiated, they each encapsulate one method. When `cd3` is instantiated, it has an invocation list of two methods, `M1` and `M2`, in that order. `cd4`s invocation list contains `M1`, `M2`, and `M1`, in that order. For `cd5` the invocation list contains `M1`, `M2`, `M1`, `M1`, and `M2`, in that order.
>
> When creating a delegate from another delegate with a *delegate_creation_expression* the result has an invocation list with a different structure from the original, but which results in the same methods being invoked in the same order. When `td3` is created from `cd3` its invocation list has just one member, but that member is a list of the methods `M1` and `M2` and those methods are invoked by `td3` in the same order as they are invoked by `cd3`. Similarly when `td4` is instantiated its invocation list has just two entries but it invokes the three methods `M1`, `M2`, and `M1`, in that order just as `cd4` does.
>
> The structure of the invocation list affects delegate subtraction. Delegate `cd6`, created by subtracting `cd2` (which invokes `M2`) from `cd4` (which invokes `M1`, `M2`, and `M1`) invokes `M1` and `M1`. However delegate `td6`, created by subtracting `cd2` (which invokes `M2`) from `td4` (which invokes `M1`, `M2`, and `M1`) still invokes `M1`, `M2` and `M1`, in that order, as `M2` is not a single entry in the list but a member of a nested list.
> For more examples of combining (as well as removing) delegates, see [§19.6](delegates.md#196-delegate-invocation). *end example*

Once instantiated, a delegate instance always refers to the same invocation list.

> *Note*: Remember, when two delegates are combined, or one is removed from another, a new delegate results with its own invocation list; the invocation lists of the delegates combined or removed remain unchanged. *end note*

## 19.6 Delegate invocation

C# provides special syntax for invoking a delegate. When a non-`null` delegate instance whose invocation list contains one entry, is invoked, it invokes the one method with the same arguments it was given, and returns the same value as the referred to method. (See [§11.7.8.4](expressions.md#11784-delegate-invocations) for detailed information on delegate invocation.) If an exception occurs during the invocation of such a delegate, and that exception is not caught within the method that was invoked, the search for an exception catch clause continues in the method that called the delegate, as if that method had directly called the method to which that delegate referred.

Invocation of a delegate instance whose invocation list contains multiple entries, proceeds by invoking each of the methods in the invocation list, synchronously, in order. Each method so called is passed the same set of arguments as was given to the delegate instance. If such a delegate invocation includes reference parameters ([§14.6.2.3](classes.md#14623-reference-parameters)), each method invocation will occur with a reference to the same variable; changes to that variable by one method in the invocation list will be visible to methods further down the invocation list. If the delegate invocation includes output parameters or a return value, their final value will come from the invocation of the last delegate in the list. If an exception occurs during processing of the invocation of such a delegate, and that exception is not caught within the method that was invoked, the search for an exception catch clause continues in the method that called the delegate, and any methods further down the invocation list are not invoked.

Attempting to invoke a delegate instance whose value is `null` results in an exception of type `System.NullReferenceException`.

> *Example*: The following example shows how to instantiate, combine, remove, and invoke delegates:
>
> ```csharp
> using System;
>
> delegate void D(int x);
>
> class C
> {
>     public static void M1(int i) => Console.WriteLine("C.M1: " + i);
>
>     public static void M2(int i) => Console.WriteLine("C.M2: " + i);
>
>     public void M3(int i) => Console.WriteLine("C.M3: " + i);
> }
>
> class Test
> {
>     static void Main()
>     {
>         D cd1 = new D(C.M1);
>         cd1(-1);             // call M1
>         D cd2 = new D(C.M2);
>         cd2(-2);             // call M2
>         D cd3 = cd1 + cd2;
>         cd3(10);             // call M1 then M2
>         cd3 += cd1;
>         cd3(20);             // call M1, M2, then M1
>         C c = new C();
>         D cd4 = new D(c.M3);
>         cd3 += cd4;
>         cd3(30);             // call M1, M2, M1, then M3
>         cd3 -= cd1;          // remove last M1
>         cd3(40);             // call M1, M2, then M3
>         cd3 -= cd4;
>         cd3(50);             // call M1 then M2
>         cd3 -= cd2;
>         cd3(60);             // call M1
>         cd3 -= cd2;          // impossible removal is benign
>         cd3(60);             // call M1
>         cd3 -= cd1;          // invocation list is empty so cd3 is null
>         // cd3(70);          // System.NullReferenceException thrown
>         cd3 -= cd1;          // impossible removal is benign
>     }
> }
> ```
>
> As shown in the statement `cd3 += cd1;`, a delegate can be present in an invocation list multiple times. In this case, it is simply invoked once per occurrence. In an invocation list such as this, when that delegate is removed, the last occurrence in the invocation list is the one actually removed.
>
> Immediately prior to the execution of the final statement, `cd3 -= cd1`;, the delegate `cd3` refers to an empty invocation list. Attempting to remove a delegate from an empty list (or to remove a non-existent delegate from a non-empty list) is not an error.
>
> The output produced is:
>
> ```console
> C.M1: -1
> C.M2: -2
> C.M1: 10
> C.M2: 10
> C.M1: 20
> C.M2: 20
> C.M1: 20
> C.M1: 30
> C.M2: 30
> C.M1: 30
> C.M3: 30
> C.M1: 40
> C.M2: 40
> C.M3: 40
> C.M1: 50
> C.M2: 50
> C.M1: 60
> C.M1: 60
> ```
>
> *end example*
