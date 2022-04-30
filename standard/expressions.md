# 11 Expressions

## 11.1 General

An expression is a sequence of operators and operands. This clause defines the syntax, order of evaluation of operands and operators, and meaning of expressions.

## 11.2 Expression classifications

### 11.2.1 General

The result of an expression is classified as one of the following:

- A value. Every value has an associated type.
- A variable. Every variable has an associated type, namely the declared type of the variable.
- A null literal. An expression with this classification can be implicitly converted to a reference type or nullable value type.
- An anonymous function. An expression with this classification can be implicitly converted to a compatible delegate type or expression tree type.
- A property access. Every property access has an associated type, namely the type of the property. Furthermore, a property access may have an associated instance expression. When an accessor of an instance property access is invoked, the result of evaluating the instance expression becomes the instance represented by `this` ([§11.7.12](expressions.md#11712-this-access)).
- An indexer access. Every indexer access has an associated type, namely the element type of the indexer. Furthermore, an indexer access has an associated instance expression and an associated argument list. When an accessor of an indexer access is invoked, the result of evaluating the instance expression becomes the instance represented by `this` ([§11.7.12](expressions.md#11712-this-access)), and the result of evaluating the argument list becomes the parameter list of the invocation.
- Nothing. This occurs when the expression is an invocation of a method with a return type of `void`. An expression classified as nothing is only valid in the context of a *statement_expression* ([§12.7](statements.md#127-expression-statements)) or as the body of a *lambda_expression* ([§11.16](expressions.md#1116-anonymous-function-expressions)).

For expressions which occur as subexpressions of larger expressions, with the noted restrictions, the result can also be classified as one of the following:

- A namespace. An expression with this classification can only appear as the left-hand side of a *member_access* ([§11.7.6](expressions.md#1176-member-access)). In any other context, an expression classified as a namespace causes a compile-time error.
- A type. An expression with this classification can only appear as the left-hand side of a *member_access* ([§11.7.6](expressions.md#1176-member-access)). In any other context, an expression classified as a type causes a compile-time error.
- A method group, which is a set of overloaded methods resulting from a member lookup ([§11.5](expressions.md#115-member-lookup)). A method group may have an associated instance expression and an associated type argument list. When an instance method is invoked, the result of evaluating the instance expression becomes the instance represented by `this` ([§11.7.12](expressions.md#11712-this-access)). A method group is permitted in an *invocation_expression* ([§11.7.8](expressions.md#1178-invocation-expressions)) or a *delegate_creation_expression* ([§11.7.15.6](expressions.md#117156-delegate-creation-expressions)), and can be implicitly converted to a compatible delegate type ([§10.8](conversions.md#108-method-group-conversions)). In any other context, an expression classified as a method group causes a compile-time error.
- An event access. Every event access has an associated type, namely the type of the event. Furthermore, an event access may have an associated instance expression. An event access may appear as the left-hand operand of the `+=` and `-=` operators ([§11.18.4](expressions.md#11184-event-assignment)). In any other context, an expression classified as an event access causes a compile-time error. When an accessor of an instance event access is invoked, the result of evaluating the instance expression becomes the instance represented by `this` ([§11.7.12](expressions.md#11712-this-access)).

A property access or indexer access is always reclassified as a value by performing an invocation of the *get_accessor* or the *set_accessor*. The particular accessor is determined by the context of the property or indexer access: If the access is the target of an assignment, the *set_accessor* is invoked to assign a new value ([§11.18.2](expressions.md#11182-simple-assignment)). Otherwise, the *get_accessor* is invoked to obtain the current value ([§11.2.2](expressions.md#1122-values-of-expressions)).

An ***instance accessor*** is a property access on an instance, an event access on an instance, or an indexer access.

### 11.2.2 Values of expressions

Most of the constructs that involve an expression ultimately require the expression to denote a ***value***. In such cases, if the actual expression denotes a namespace, a type, a method group, or nothing, a compile-time error occurs. However, if the expression denotes a property access, an indexer access, or a variable, the value of the property, indexer, or variable is implicitly substituted:

- The value of a variable is simply the value currently stored in the storage location identified by the variable. A variable shall be considered definitely assigned ([§9.4](variables.md#94-definite-assignment)) before its value can be obtained, or otherwise a compile-time error occurs.
- The value of a property access expression is obtained by invoking the *get_accessor* of the property. If the property has no *get_accessor*, a compile-time error occurs. Otherwise, a function member invocation ([§11.6.6](expressions.md#1166-function-member-invocation)) is performed, and the result of the invocation becomes the value of the property access expression.
- The value of an indexer access expression is obtained by invoking the *get_accessor* of the indexer. If the indexer has no *get_accessor*, a compile-time error occurs. Otherwise, a function member invocation ([§11.6.6](expressions.md#1166-function-member-invocation)) is performed with the argument list associated with the indexer access expression, and the result of the invocation becomes the value of the indexer access expression.

## 11.3 Static and Dynamic Binding

### 11.3.1 General

***Binding*** is the process of determining what an operation refers to, based on the type or value of expressions (arguments, operands, receivers). For instance, the binding of a method call is determined based on the type of the receiver and arguments. The binding of an operator is determined based on the type of its operands.

In C# the binding of an operation is usually determined at compile-time, based on the compile-time type of its subexpressions. Likewise, if an expression contains an error, the error is detected and reported by the compiler. This approach is known as ***static binding***.

However, if an expression is a *dynamic expression* (i.e., has the type `dynamic`) this indicates that any binding that it participates in should be based on its run-time type rather than the type it has at compile-time. The binding of such an operation is therefore deferred until the time where the operation is to be executed during the running of the program. This is referred to as ***dynamic binding***.

When an operation is dynamically bound, little or no checking is performed by the compiler. Instead if the run-time binding fails, errors are reported as exceptions at run-time.

The following operations in C# are subject to binding:

- Member access: `e.M`
- Method invocation: `e.M(e₁,...,eᵥ)`
- Delegate invocation: `e(e₁,...,eᵥ)`
- Element access: `e[e₁,...,eᵥ]`
- Object creation: new `C(e₁,...,eᵥ)`
- Overloaded unary operators: `+`, `-`, `!`, `~`, `++`, `--`, `true`, `false`
- Overloaded binary operators: `+`, `-`, `*`, `/`, `%`, `&`, `&&`, `|`, `||`, `??`, `^`, `<<`, `>>`, `==`, `!=`, `>`, `<`, `>=`, `<=`
- Assignment operators: `=`, `+=`, `-=`, `*=`, `/=`, `%=`, `&=`, `|=`, `^=`, `<<=`, `>>=`
- Implicit and explicit conversions

When no dynamic expressions are involved, C# defaults to static binding, which means that the compile-time types of subexpressions are used in the selection process. However, when one of the subexpressions in the operations listed above is a dynamic expression, the operation is instead dynamically bound.

### 11.3.2 Binding-time

Static binding takes place at compile-time, whereas dynamic binding takes place at run-time. In the following subclauses, the term ***binding-time*** refers to either compile-time or run-time, depending on when the binding takes place.

> *Example*: The following illustrates the notions of static and dynamic binding and of binding-time:
>
> ```csharp
> object o = 5;
> dynamic d = 5;
> Console.WriteLine(5); // static binding to Console.WriteLine(int)
> Console.WriteLine(o); // static binding to Console.WriteLine(object)
> Console.WriteLine(d); // dynamic binding to Console.WriteLine(int)
> ```
>
> The first two calls are statically bound: the overload of `Console.WriteLine` is picked based on the compile-time type of their argument. Thus, the binding-time is *compile-time*.
>
> The third call is dynamically bound: the overload of `Console.WriteLine` is picked based on the run-time type of its argument. This happens because the argument is a dynamic expression – its compile-time type is dynamic. Thus, the binding-time for the third call is *run-time*.
>
> *end example*

### 11.3.3 Dynamic binding

**This subclause is informative.**

Dynamic binding allows C# programs to interact with dynamic objects, i.e., objects that do not follow the normal rules of the C# type system. Dynamic objects may be objects from other programming languages with different types systems, or they may be objects that are programmatically setup to implement their own binding semantics for different operations.

The mechanism by which a dynamic object implements its own semantics is implementation-defined. A given interface – again implementation-defined – is implemented by dynamic objects to signal to the C# run-time that they have special semantics. Thus, whenever operations on a dynamic object are dynamically bound, their own binding semantics, rather than those of C# as specified in this specification, take over.

While the purpose of dynamic binding is to allow interoperation with dynamic objects, C# allows dynamic binding on all objects, whether they are dynamic or not. This allows for a smoother integration of dynamic objects, as the results of operations on them may not themselves be dynamic objects, but are still of a type unknown to the programmer at compile-time. Also, dynamic binding can help eliminate error-prone reflection-based code even when no objects involved are dynamic objects.

### 11.3.4 Types of subexpressions

When an operation is statically bound, the type of a subexpression (e.g., a receiver, and argument, an index or an operand) is always considered to be the compile-time type of that expression.

When an operation is dynamically bound, the type of a subexpression is determined in different ways depending on the compile-time type of the subexpression:

- A subexpression of compile-time type dynamic is considered to have the type of the actual value that the expression evaluates to at run-time
- A subexpression whose compile-time type is a type parameter is considered to have the type which the type parameter is bound to at run-time
- Otherwise, the subexpression is considered to have its compile-time type.

## 11.4 Operators

### 11.4.1 General

Expressions are constructed from operands and operators. The operators of an expression indicate which operations to apply to the operands.

> *Example*: Examples of operators include `+`, `-`, `*`, `/`, and `new`. Examples of operands include literals, fields, local variables, and expressions. *end example*

There are three kinds of operators:

- Unary operators. The unary operators take one operand and use either prefix notation (such as `–x`) or postfix notation (such as `x++`).
- Binary operators. The binary operators take two operands and all use infix notation (such as `x + y`).
- Ternary operator. Only one ternary operator, `?:`, exists; it takes three operands and uses infix notation (`c ? x : y`).

The order of evaluation of operators in an expression is determined by the *precedence* and *associativity* of the operators ([§11.4.2](expressions.md#1142-operator-precedence-and-associativity)).

Operands in an expression are evaluated from left to right.

> *Example*: In `F(i) + G(i++) * H(i)`, method `F` is called using the old value of `i`, then method `G` is called with the old value of `i`, and, finally, method `H` is called with the new value of i. This is separate from and unrelated to operator precedence. *end example*

Certain operators can be ***overloaded***. Operator overloading ([§11.4.3](expressions.md#1143-operator-overloading)) permits user-defined operator implementations to be specified for operations where one or both of the operands are of a user-defined class or struct type.

### 11.4.2 Operator precedence and associativity

When an expression contains multiple operators, the ***precedence*** of the operators controls the order in which the individual operators are evaluated.

> *Note*: For example, the expression `x + y * z` is evaluated as `x + (y * z)` because the `*` operator has higher precedence than the binary `+` operator. *end note*

The precedence of an operator is established by the definition of its associated grammar production.

> *Note*: For example, an *additive_expression* consists of a sequence of *multiplicative_expression*s separated by `+` or `-` operators, thus giving the `+` and `-` operators lower precedence than the `*`, `/`, and `%` operators. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The following table summarizes all operators in order of precedence from highest to lowest:
>
> |  **Subclause**      | **Category**                     | **Operators**
> |  -----------------  | -------------------------------  | -------------------------------------------------------
> |  [§11.7](expressions.md#117-primary-expressions)              | Primary                          | `x.y` `x?.y` `f(x)` `a[x]` `a?[x]` `x++` `x--` `new` `typeof` `default` `checked` `unchecked` `delegate`
> |  [§11.8](expressions.md#118-unary-operators)              | Unary                            | `+` `-` `!` `~` `++x` `--x` `(T)x` `await x`
> |  [§11.9](expressions.md#119-arithmetic-operators)              | Multiplicative                   | `*` `/` `%`
> |  [§11.9](expressions.md#119-arithmetic-operators)              | Additive                         | `+` `-`
> |  [§11.10](expressions.md#1110-shift-operators)             | Shift                            | `<<` `>>`
> |  [§11.11](expressions.md#1111-relational-and-type-testing-operators)             | Relational and type-testing      | `<` `>` `<=` `>=` `is` `as`
> |  [§11.11](expressions.md#1111-relational-and-type-testing-operators)             | Equality                         | `==` `!=`
> |  [§11.12](expressions.md#1112-logical-operators)             | Logical AND                      | `&`
> |  [§11.12](expressions.md#1112-logical-operators)             | Logical XOR                      | `^`
> |  [§11.12](expressions.md#1112-logical-operators)             | Logical OR                       | `|`
> |  [§11.13](expressions.md#1113-conditional-logical-operators)             | Conditional AND                  | `&&`
> |  [§11.13](expressions.md#1113-conditional-logical-operators)             | Conditional OR                   | `||`
> |  [§11.14](expressions.md#1114-the-null-coalescing-operator)             | Null coalescing                  | `??`
> |  [§11.15](expressions.md#1115-conditional-operator)             | Conditional                      | `?:`
> |  [§11.18](expressions.md#1118-assignment-operators) and [§11.16](expressions.md#1116-anonymous-function-expressions)  | Assignment and lambda expression | `=` `*=` `/=` `%=` `+=` `-=` `<<=` `>>=` `&=` `^=` `\|=` `=>`
>
> *end note*

When an operand occurs between two operators with the same precedence, the ***associativity*** of the operators controls the order in which the operations are performed:

- Except for the assignment operators and the null coalescing operator, all binary operators are ***left-associative***, meaning that operations are performed from left to right.
    > *Example*: `x + y + z` is evaluated as `(x + y) + z`. *end example*
- The assignment operators, the null coalescing operator and the conditional operator (`?:`) are ***right-associative***, meaning that operations are performed from right to left.
    > *Example*: `x = y = z` is evaluated as `x = (y = z)`. *end example*

Precedence and associativity can be controlled using parentheses.

> *Example*: `x + y * z` first multiplies `y` by `z` and then adds the result to `x`, but `(x + y) * z` first adds `x` and `y` and then multiplies the result by `z`. *end example*

### 11.4.3 Operator overloading

All unary and binary operators have predefined implementations. In addition, user-defined implementations can be introduced by including operator declarations ([§14.10](classes.md#1410-operators)) in classes and structs. User-defined operator implementations always take precedence over predefined operator implementations: Only when no applicable user-defined operator implementations exist will the predefined operator implementations be considered, as described in [§11.4.4](expressions.md#1144-unary-operator-overload-resolution) and [§11.4.5](expressions.md#1145-binary-operator-overload-resolution).

The ***overloadable unary operators*** are:

```csharp
+  -  !  ~  ++  --  true  false
```

> *Note*: Although `true` and `false` are not used explicitly in expressions (and therefore are not included in the precedence table in [§11.4.2](expressions.md#1142-operator-precedence-and-associativity)), they are considered operators because they are invoked in several expression contexts: Boolean expressions ([§11.21](expressions.md#1121-boolean-expressions)) and expressions involving the conditional ([§11.15](expressions.md#1115-conditional-operator)) and conditional logical operators ([§11.13](expressions.md#1113-conditional-logical-operators)). *end note*

The ***overloadable binary operators*** are:

```csharp
+  -  *  /  %  &  |  ^  <<  >>  ==  !=  >  <  <=  >=
```

Only the operators listed above can be overloaded. In particular, it is not possible to overload member access, method invocation, or the `=`, `&&`, `||`, `??`, `?:`, `=>`, `checked`, `unchecked`, `new`, `typeof`, `default`, `as`, and `is` operators.

When a binary operator is overloaded, the corresponding compound assignment operator, if any, is also implicitly overloaded.

> *Example*: An overload of operator `*` is also an overload of operator `*=`. This is described further in [§11.18](expressions.md#1118-assignment-operators). *end example*

The assignment operator itself `(=)` cannot be overloaded. An assignment always performs a simple store of a value into a variable ([§11.18.2](expressions.md#11182-simple-assignment)).

Cast operations, such as `(T)x`, are overloaded by providing user-defined conversions ([§10.5](conversions.md#105-user-defined-conversions)).

> *Note*: User-defined conversions do not affect the behavior of the `is` or `as` operators. *end note*

Element access, such as `a[x]`, is not considered an overloadable operator. Instead, user-defined indexing is supported through indexers ([§14.9](classes.md#149-indexers)).

In expressions, operators are referenced using operator notation, and in declarations, operators are referenced using functional notation. The following table shows the relationship between operator and functional notations for unary and binary operators. In the first entry, «op» denotes any overloadable unary prefix operator. In the second entry, «op» denotes the unary postfix `++` and `--` operators. In the third entry, «op» denotes any overloadable binary operator.

> *Note*: For an example of overloading the `++` and `--` operators see [§14.10.2](classes.md#14102-unary-operators). *end note*

  **Operator notation**  | **Functional notation**
  ---------------------- | -------------------------
  `«op» x`               | `operator «op»(x)`
  `x «op»`               | `operator «op»(x)`
  `x «op» y`             | `operator «op»(x, y)`

User-defined operator declarations always require at least one of the parameters to be of the class or struct type that contains the operator declaration.

> *Note*: Thus, it is not possible for a user-defined operator to have the same signature as a predefined operator. *end note*

User-defined operator declarations cannot modify the syntax, precedence, or associativity of an operator.

> *Example*: The `/` operator is always a binary operator, always has the precedence level specified in [§11.4.2](expressions.md#1142-operator-precedence-and-associativity), and is always left-associative. *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: While it is possible for a user-defined operator to perform any computation it pleases, implementations that produce results other than those that are intuitively expected are strongly discouraged. For example, an implementation of operator `==` should compare the two operands for equality and return an appropriate `bool` result. *end note*

The descriptions of individual operators in [§11.8](expressions.md#118-unary-operators) through [§11.18](expressions.md#1118-assignment-operators) specify the predefined implementations of the operators and any additional rules that apply to each operator. The descriptions make use of the terms ***unary operator overload resolution***, ***binary operator overload resolution***, ***numeric promotion***, and lifted operator definitions of which are found in the following subclauses.

### 11.4.4 Unary operator overload resolution

An operation of the form `«op» x` or `x «op»`, where «op» is an overloadable unary operator, and `x` is an expression of type `X`, is processed as follows:

- The set of candidate user-defined operators provided by `X` for the operation `operator «op»(x)` is determined using the rules of [§11.4.6](expressions.md#1146-candidate-user-defined-operators).
- If the set of candidate user-defined operators is not empty, then this becomes the set of candidate operators for the operation. Otherwise, the predefined binary `operator «op»` implementations, including their lifted forms, become the set of candidate operators for the operation. The predefined implementations of a given operator are specified in the description of the operator. The predefined operators provided by an enum or delegate type are only included in this set when the binding-time type—or the underlying type if it is a nullable type—of either operand is the enum or delegate type.
- The overload resolution rules of [§11.6.4](expressions.md#1164-overload-resolution) are applied to the set of candidate operators to select the best operator with respect to the argument list `(x)`, and this operator becomes the result of the overload resolution process. If overload resolution fails to select a single best operator, a binding-time error occurs.

### 11.4.5 Binary operator overload resolution

An operation of the form `x «op» y`, where «op» is an overloadable binary operator, `x` is an expression of type `X`, and `y` is an expression of type `Y`, is processed as follows:

- The set of candidate user-defined operators provided by `X` and `Y` for the operation `operator «op»(x, y)` is determined. The set consists of the union of the candidate operators provided by `X` and the candidate operators provided by `Y`, each determined using the rules of [§11.4.6](expressions.md#1146-candidate-user-defined-operators). For the combined set, candidates are merged as follows:
  - If `X` and `Y` are the same type, or if `X` and `Y` are derived from a common base type, then shared candidate operators only occur in the combined set once.
  - If there is an identity conversion between `X` and `Y`, an operator `«op»Y` provided by `Y` has the same return type as an `«op»X` provided by `X` and the operand types of `«op»Y` have an identity conversion to the corresponding operand types of `«op»X` then only `«op»X` occurs in the set.
- If the set of candidate user-defined operators is not empty, then this becomes the set of candidate operators for the operation. Otherwise, the predefined binary `operator «op»` implementations, including their lifted forms, become the set of candidate operators for the operation. The predefined implementations of a given operator are specified in the description of the operator. For predefined enum and delegate operators, the only operators considered are those provided by an enum or delegate type that is the binding-time type of one of the operands.
- The overload resolution rules of [§11.6.4](expressions.md#1164-overload-resolution) are applied to the set of candidate operators to select the best operator with respect to the argument list `(x, y)`, and this operator becomes the result of the overload resolution process. If overload resolution fails to select a single best operator, a binding-time error occurs.

### 11.4.6 Candidate user-defined operators

Given a type `T` and an operation `operator «op»(A)`, where «op» is an overloadable operator and `A` is an argument list, the set of candidate user-defined operators provided by `T` for operator `«op»(A)` is determined as follows:

- Determine the type `T₀`. If `T` is a nullable value type, `T₀` is its underlying type; otherwise, `T₀` is equal to `T`.
- For all `operator «op»` declarations in `T₀` and all lifted forms of such operators, if at least one operator is applicable ([§11.6.4.2](expressions.md#11642-applicable-function-member)) with respect to the argument list `A`, then the set of candidate operators consists of all such applicable operators in `T₀`.
- Otherwise, if `T₀` is `object`, the set of candidate operators is empty.
- Otherwise, the set of candidate operators provided by `T₀` is the set of candidate operators provided by the direct base class of `T₀`, or the effective base class of `T₀` if `T₀` is a type parameter.

### 11.4.7 Numeric promotions

#### 11.4.7.1 General

**This subclause is informative.**

[§11.4.7](expressions.md#1147-numeric-promotions) and its subclauses are a summary of the combined effect of:

- the rules for implicit numeric conversions ([§10.2.3](conversions.md#1023-implicit-numeric-conversions));
- the rules for better conversion ([§11.6.4.6](expressions.md#11646-better-conversion-target)); and
- the available arithmetic ([§11.9](expressions.md#119-arithmetic-operators)), relational ([§11.11](expressions.md#1111-relational-and-type-testing-operators)), and integral logical ([§11.12.2](expressions.md#11122-integer-logical-operators)) operators.

Numeric promotion consists of automatically performing certain implicit conversions of the operands of the predefined unary and binary numeric operators. Numeric promotion is not a distinct mechanism, but rather an effect of applying overload resolution to the predefined operators. Numeric promotion specifically does not affect evaluation of user-defined operators, although user-defined operators can be implemented to exhibit similar effects.

As an example of numeric promotion, consider the predefined implementations of the binary `*` operator:

```csharp
int operator *(int x, int y);
uint operator *(uint x, uint y);
long operator *(long x, long y);
ulong operator *(ulong x, ulong y);
float operator *(float x, float y);
double operator *(double x, double y);
decimal operator *(decimal x, decimal y);
```

When overload resolution rules ([§11.6.4](expressions.md#1164-overload-resolution)) are applied to this set of operators, the effect is to select the first of the operators for which implicit conversions exist from the operand types.

> *Example*: For the operation `b * s`, where `b` is a `byte` and `s` is a `short`, overload resolution selects `operator *(int, int)` as the best operator. Thus, the effect is that `b` and `s` are converted to `int`, and the type of the result is `int`. Likewise, for the operation `i * d`, where `i` is an `int` and `d` is a `double`, `overload` resolution selects `operator *(double, double)` as the best operator. *end example*

**End of informative text.**

#### 11.4.7.2 Unary numeric promotions

**This subclause is informative.**

Unary numeric promotion occurs for the operands of the predefined `+`, `–`, and `~` unary operators. Unary numeric promotion simply consists of converting operands of type `sbyte`, `byte`, `short`, `ushort`, or `char` to type `int`. Additionally, for the unary – operator, unary numeric promotion converts operands of type `uint` to type `long`.

**End of informative text.**

#### 11.4.7.3 Binary numeric promotions

**This subclause is informative.**

Binary numeric promotion occurs for the operands of the predefined `+`, `–`, `*`, `/`, `%`, `&`, `|`, `^`, `==`, `!=`, `>`, `<`, `>=`, and `<=` binary operators. Binary numeric promotion implicitly converts both operands to a common type which, in case of the non-relational operators, also becomes the result type of the operation. Binary numeric promotion consists of applying the following rules, in the order they appear here:

- If either operand is of type `decimal`, the other operand is converted to type `decimal`, or a binding-time error occurs if the other operand is of type `float` or `double`.
- Otherwise, if either operand is of type `double`, the other operand is converted to type `double`.
- Otherwise, if either operand is of type `float`, the other operand is converted to type `float`.
- Otherwise, if either operand is of type `ulong`, the other operand is converted to type `ulong`, or a binding-time error occurs if the other operand is of `type sbyte`, `short`, `int`, or `long`.
- Otherwise, if either operand is of type `long`, the other operand is converted to type `long`.
- Otherwise, if either operand is of type `uint` and the other operand is of type `sbyte`, `short`, or `int`, both operands are converted to type `long`.
- Otherwise, if either operand is of type `uint`, the other operand is converted to type `uint`.
- Otherwise, both operands are converted to type `int`.

> *Note*: The first rule disallows any operations that mix the `decimal` type with the `double` and `float` types. The rule follows from the fact that there are no implicit conversions between the `decimal` type and the `double` and `float` types. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: Also note that it is not possible for an operand to be of type `ulong` when the other operand is of a signed integral type. The reason is that no integral type exists that can represent the full range of `ulong` as well as the signed integral types. *end note*

In both of the above cases, a cast expression can be used to explicitly convert one operand to a type that is compatible with the other operand.

> *Example*: In the following code
>
> ```csharp
> decimal AddPercent(decimal x, double percent) =>
>     x * (1.0 + percent / 100.0);
> ```
>
> a binding-time error occurs because a `decimal` cannot be multiplied by a `double`. The error is resolved by explicitly converting the second operand to `decimal`, as follows:
>
> ```csharp
> decimal AddPercent(decimal x, double percent) =>
>     x * (decimal)(1.0 + percent / 100.0);
> ```
>
> *end example*

**End of informative text.**

### 11.4.8 Lifted operators

***Lifted operators*** permit predefined and user-defined operators that operate on non-nullable value types to also be used with nullable forms of those types. Lifted operators are constructed from predefined and user-defined operators that meet certain requirements, as described in the following:

- For the unary operators `+`, `++`, `-`, `--`, `!`, and `~`, a lifted form of an operator exists if the operand and result types are both non-nullable value types. The lifted form is constructed by adding a single `?` modifier to the operand and result types. The lifted operator produces a `null` value if the operand is `null`. Otherwise, the lifted operator unwraps the operand, applies the underlying operator, and wraps the result.
- For the binary operators `+`, `-`, `*`, `/`, `%`, `&`, `|`, `^`, `<<`, and `>>`, a lifted form of an operator exists if the operand and result types are all non-nullable value types. The lifted form is constructed by adding a single `?` modifier to each operand and result type. The lifted operator produces a `null` value if one or both operands are `null` (an exception being the `&` and `|` operators of the `bool?` type, as described in [§11.12.5](expressions.md#11125-nullable-boolean--and--operators)). Otherwise, the lifted operator unwraps the operands, applies the underlying operator, and wraps the result.
- For the equality operators `==` and `!=`, a lifted form of an operator exists if the operand types are both non-nullable value types and if the result type is `bool`. The lifted form is constructed by adding a single `?` modifier to each operand type. The lifted operator considers two `null` values equal, and a `null` value unequal to any non-`null` value. If both operands are non-`null`, the lifted operator unwraps the operands and applies the underlying operator to produce the `bool` result.
- For the relational operators `<`, `>`, `<=`, and `>=`, a lifted form of an operator exists if the operand types are both non-nullable value types and if the result type is `bool`. The lifted form is constructed by adding a single `?` modifier to each operand type. The lifted operator produces the value `false` if one or both operands are `null`. Otherwise, the lifted operator unwraps the operands and applies the underlying operator to produce the `bool` result.

## 11.5 Member lookup

### 11.5.1 General

A member lookup is the process whereby the meaning of a name in the context of a type is determined. A member lookup can occur as part of evaluating a *simple_name* ([§11.7.4](expressions.md#1174-simple-names)) or a *member_access* ([§11.7.6](expressions.md#1176-member-access)) in an expression. If the *simple_name* or *member_access* occurs as the *primary_expression* of an *invocation_expression* ([§11.7.8.2](expressions.md#11782-method-invocations)), the member is said to be *invoked*.

If a member is a method or event, or if it is a constant, field or property of either a delegate type ([§19](delegates.md#19-delegates)) or the type `dynamic` ([§8.2.4](types.md#824-the-dynamic-type)), then the member is said to be *invocable.*

Member lookup considers not only the name of a member but also the number of type parameters the member has and whether the member is accessible. For the purposes of member lookup, generic methods and nested generic types have the number of type parameters indicated in their respective declarations and all other members have zero type parameters.

A member lookup of a name `N` with `K` type arguments in a type `T` is processed as follows:

- First, a set of accessible members named `N` is determined:
  - If `T` is a type parameter, then the set is the union of the sets of accessible members named `N` in each of the types specified as a primary constraint or secondary constraint ([§14.2.5](classes.md#1425-type-parameter-constraints)) for `T`, along with the set of accessible members named `N` in `object`.
  - Otherwise, the set consists of all accessible ([§7.5](basic-concepts.md#75-member-access)) members named `N` in `T`, including inherited members and the accessible members named `N` in `object`. If `T` is a constructed type, the set of members is obtained by substituting type arguments as described in [§14.3.3](classes.md#1433-members-of-constructed-types). Members that include an `override` modifier are excluded from the set.
- Next, if `K` is zero, all nested types whose declarations include type parameters are removed. If `K` is not zero, all members with a different number of type parameters are removed. When `K` is zero, methods having type parameters are not removed, since the type inference process ([§11.6.3](expressions.md#1163-type-inference)) might be able to infer the type arguments.
- Next, if the member is invoked, all non-invocable members are removed from the set.
- Next, members that are hidden by other members are removed from the set. For every member `S.M` in the set, where `S` is the type in which the member `M` is declared, the following rules are applied:
  - If `M` is a constant, field, property, event, or enumeration member, then all members declared in a base type of `S` are removed from the set.
  - If `M` is a type declaration, then all non-types declared in a base type of `S` are removed from the set, and all type declarations with the same number of type parameters as `M` declared in a base type of `S` are removed from the set.
  - If `M` is a method, then all non-method members declared in a base type of `S` are removed from the set.
- Next, interface members that are hidden by class members are removed from the set. This step only has an effect if `T` is a type parameter and `T` has both an effective base class other than `object` and a non-empty effective interface set ([§14.2.5](classes.md#1425-type-parameter-constraints)). For every member `S.M` in the set, where `S` is the type in which the member `M` is declared, the following rules are applied if `S` is a class declaration other than `object`:
  - If `M` is a constant, field, property, event, enumeration member, or type declaration, then all members declared in an interface declaration are removed from the set.
  - If `M` is a method, then all non-method members declared in an interface declaration are removed from the set, and all methods with the same signature as `M` declared in an interface declaration are removed from the set.
- Finally, having removed hidden members, the result of the lookup is determined:
  - If the set consists of a single member that is not a method, then this member is the result of the lookup.
  - Otherwise, if the set contains only methods, then this group of methods is the result of the lookup.
  - Otherwise, the lookup is ambiguous, and a binding-time error occurs.

For member lookups in types other than type parameters and interfaces, and member lookups in interfaces that are strictly single-inheritance (each interface in the inheritance chain has exactly zero or one direct base interface), the effect of the lookup rules is simply that derived members hide base members with the same name or signature. Such single-inheritance lookups are never ambiguous. The ambiguities that can possibly arise from member lookups in multiple-inheritance interfaces are described in [§17.4.6](interfaces.md#1746-interface-member-access).

> *Note*: This phase only accounts for one kind of ambiguity. If the member lookup results in a method group, further uses of method group may fail due to ambiguity, for example as described in [§11.6.4.1](expressions.md#11641-general) and [§11.6.6.2](expressions.md#11662-invocations-on-boxed-instances). *end note*

### 11.5.2 Base types

For purposes of member lookup, a type `T` is considered to have the following base types:

- If `T` is `object` or `dynamic`, then `T` has no base type.
- If `T` is an *enum_type*, the base types of `T` are the class types `System.Enum`, `System.ValueType`, and `object`.
- If `T` is a *struct_type*, the base types of `T` are the class types `System.ValueType` and `object`.
  > *Note*: A *nullable_value_type* is a *struct_type* ([§8.3.1](types.md#831-general)). *end note*
- If `T` is a *class_type*, the base types of `T` are the base classes of `T`, including the class type `object`.
- If `T` is an *interface_type*, the base types of `T` are the base interfaces of `T` and the class type `object`.
- If `T` is an *array_type*, the base types of `T` are the class types `System.Array` and `object`.
- If `T` is a *delegate_type*, the base types of `T` are the class types `System.Delegate` and `object`.

## 11.6 Function members

### 11.6.1 General

Function members are members that contain executable statements. Function members are always members of types and cannot be members of namespaces. C# defines the following categories of function members:

- Methods
- Properties
- Events
- Indexers
- User-defined operators
- Instance constructors
- Static constructors
- Finalizers

Except for finalizers and static constructors (which cannot be invoked explicitly), the statements contained in function members are executed through function member invocations. The actual syntax for writing a function member invocation depends on the particular function member category.

The argument list ([§11.6.2](expressions.md#1162-argument-lists)) of a function member invocation provides actual values or variable references for the parameters of the function member.

Invocations of generic methods may employ type inference to determine the set of type arguments to pass to the method. This process is described in [§11.6.3](expressions.md#1163-type-inference).

Invocations of methods, indexers, operators, and instance constructors employ overload resolution to determine which of a candidate set of function members to invoke. This process is described in [§11.6.4](expressions.md#1164-overload-resolution).

Once a particular function member has been identified at binding-time, possibly through overload resolution, the actual run-time process of invoking the function member is described in [§11.6.6](expressions.md#1166-function-member-invocation).

> *Note*: The following table summarizes the processing that takes place in constructs involving the six categories of function members that can be explicitly invoked. In the table, `e`, `x`, `y`, and `value` indicate expressions classified as variables or values, `T` indicates an expression classified as a type, `F` is the simple name of a method, and `P` is the simple name of a property.

<!-- Custom Word conversion: function_members -->
<table>
  <tr>
    <th>Construct</th>
    <th>Example</th>
    <th>Description</th>
  </tr>
  <tr>
    <td rowspan="3">Method invocation</td>
    <td><code>F(x, y)</code></td>
    <td>Overload resolution is applied to select the best method <code>F</code> in the containing class or struct. The method is invoked with the argument list <code>(x, y)</code>. If the method is not <code>static</code>, the instance expression is <code>this</code>.</td>
  </tr>
  <tr>
    <td><code>T.F(x, y)</code></td>
    <td>Overload resolution is applied to select the best method <code>F</code> in the class or struct <code>T</code>. A binding-time error occurs if the method is not <code>static</code>. The method is invoked with the argument list <code>(x, y)</code>.</td>
  </tr>
  <tr>
    <td><code>e.F(x, y)</code></td>
    <td>Overload resolution is applied to select the best method <code>F</code> in the class, struct, or interface given by the type of <code>e</code>. A binding-time error occurs if the method is <code>static</code>. The method is invoked with the instance expression <code>e</code> and the argument list <code>(x, y)</code>.</td>
  </tr>
  <tr>
    <td rowspan="6">Property access</td>
    <td><code>P</code></td>
    <td>The get accessor of the property <code>P</code> in the containing class or struct is invoked. A compile-time error occurs if <code>P</code> is write-only. If <code>P</code> is not <code>static</code>, the instance expression is <code>this</code>.</td>
  </tr>
  <tr>
    <td><code>P = value</code></td>
    <td>The set accessor of the property <code>P</code> in the containing class or struct is invoked with the argument list <code>(value)</code>. A compile-time error occurs if <code>P</code> is read-only. If <code>P</code> is not <code>static</code>, the instance expression is <code>this</code>.</td>
  </tr>  
  <tr>
    <td><code>T.P</code></td>
    <td>The get accessor of the property <code>P</code> in the class or struct <code>T</code> is invoked. A compile-time error occurs if <code>P</code> is not <code>static</code> or if <code>P</code> is write-only.</td>
  </tr>  
  <tr>
    <td><code>T.P = value</code></td>
    <td>The set accessor of the property <code>P</code> in the class or struct <code>T</code> is invoked with the argument list <code>(value)</code>. A compile-time error occurs if <code>P</code> is not <code>static</code> or if <code>P</code> is read-only.</td>
  </tr>
  <tr>
    <td><code>e.P</code></td>
    <td>The get accessor of the property <code>P</code> in the class, struct, or interface given by the type of <code>E</code> is invoked with the instance expression <code>e</code>. A binding-time error occurs if <code>P</code> is <code>static</code> or if <code>P</code> is write-only.</td>
  </tr>
  <tr>
    <td><code>e.P = value</code></td>
    <td>The set accessor of the property <code>P</code> in the class, struct, or interface given by the type of <code>E</code> is invoked with the instance expression <code>e</code> and the argument list <code>(value)</code>. A binding-time error occurs if <code>P</code> is <code>static</code> or if <code>P</code> is read-only.</td>
  </tr>  
  <tr>
    <td rowspan="6">Event access</td>
    <td><code>E += value</code></td>
    <td>The add accessor of the event <code>E</code> in the containing class or struct is invoked. If <code>E</code> is not <code>static</code>, the instance expression is <code>this</code>.</td>
  </tr>
  <tr>
    <td><code>E -= value</code></td>
    <td>The remove accessor of the event <code>E</code> in the containing class or struct is invoked. If <code>E</code> is not <code>static</code>, the instance expression is <code>this</code>.</td>
  </tr>
  <tr>
    <td><code>T.E += value</code></td>
    <td>The add accessor of the event <code>E</code> in the class or struct <code>T</code> is invoked. A binding-time error occurs if <code>E</code> is not <code>static</code>.</td>
  </tr>
  <tr>
    <td><code>T.E -= value</code></td>
    <td>The remove accessor of the event <code>E</code> in the class or struct <code>T</code> is invoked. A binding-time error occurs if <code>E</code> is not <code>static</code>.</td>
  </tr>
  <tr>
    <td><code>e.E += value</code></td>
    <td>The add accessor of the event <code>E</code> in the class, struct, or interface given by the type of <code>E</code> is invoked with the instance expression <code>e</code>. A binding-time error occurs if <code>E</code> is <code>static</code>.</td>
  </tr>
  <tr>
    <td><code>e.E -= value</code></td>
    <td>The remove accessor of the event <code>E</code> in the class, struct, or interface given by the type of <code>E</code> is invoked with the instance expression <code>e</code>. A binding-time error occurs if <code>E</code> is <code>static</code>.</td>
  </tr>
  <tr>
    <td rowspan="2">Indexer access</td>
    <td><code>e[x, y]</code></td>
    <td>Overload resolution is applied to select the best indexer in the class, struct, or interface given by the type of <code>e</code>. The get accessor of the indexer is invoked with the instance expression <code>e</code> and the argument list <code>(x, y)</code>. A binding-time error occurs if the indexer is write-only.</td>
  </tr>
  <tr>
    <td><code>e[x, y] = value</code></td>
    <td>Overload resolution is applied to select the best indexer in the class, struct, or interface given by the type of <code>e</code>. The set accessor of the indexer is invoked with the instance expression <code>e</code> and the argument list <code>(x, y, value)</code>. A binding-time error occurs if the indexer is read-only.</td>
  </tr>
  <tr>
    <td rowspan="2">Operator invocation</td>
    <td><code>-x</code></td>
    <td>Overload resolution is applied to select the best unary operator in the class or struct given by the type of <code>x</code>. The selected operator is invoked with the argument list <code>(x)</code>.</td>
  </tr>
  <tr>
    <td><code>x + y</code></td>
    <td>Overload resolution is applied to select the best binary operator in the classes or structs given by the types of <code>x</code> and <code>y</code>. The selected operator is invoked with the argument list <code>(x, y)</code>.</td>
  </tr>
  <tr>
    <td>Instance constructor invocation</td>
    <td><code>new T(x, y)</code></td>
    <td>Overload resolution is applied to select the best instance constructor in the class or struct <code>T</code>. The instance constructor is invoked with the argument list <code>(x, y)</code>.</td>
  </tr>
</table>

### 11.6.2 Argument lists

#### 11.6.2.1 General

Every function member and delegate invocation includes an argument list, which provides actual values or variable references for the parameters of the function member. The syntax for specifying the argument list of a function member invocation depends on the function member category:

- For instance constructors, methods, indexers and delegates, the arguments are specified as an *argument_list*, as described below. For indexers, when invoking the set accessor, the argument list additionally includes the expression specified as the right operand of the assignment operator.  
   > *Note*: This additional argument is not used for overload resolution, just during invocation of the set accessor. *end note*
- For properties, the argument list is empty when invoking the get accessor, and consists of the expression specified as the right operand of the assignment operator when invoking the set accessor.
- For events, the argument list consists of the expression specified as the right operand of the `+=` or `-=` operator.
- For user-defined operators, the argument list consists of the single operand of the unary operator or the two operands of the binary operator.

The arguments of properties ([§14.7](classes.md#147-properties)), events ([§14.8](classes.md#148-events)), and user-defined operators ([§14.10](classes.md#1410-operators)) are always passed as value parameters ([§14.6.2.2](classes.md#14622-value-parameters)). The arguments of indexers ([§14.9](classes.md#149-indexers)) are always passed as value parameters ([§14.6.2.2](classes.md#14622-value-parameters)) or parameter arrays ([§14.6.2.5](classes.md#14625-parameter-arrays)). Reference and output parameters are not supported for these categories of function members.

The arguments of an instance constructor, method, indexer, or delegate invocation are specified as an *argument_list*:

```ANTLR
argument_list
    : argument (',' argument)*
    ;

argument
    : argument_name? argument_value
    ;

argument_name
    : identifier ':'
    ;

argument_value
    : expression
    | 'ref' variable_reference
    | 'out' variable_reference
    ;
```

An *argument_list* consists of one or more *argument*s, separated by commas. Each argument consists of an optional *argument_name* followed by an *argument_value*. An *argument* with an *argument_name* is referred to as a ***named argument***, whereas an *argument* without an *argument_name* is a ***positional argument***. It is an error for a positional argument to appear after a named argument in an *argument_list*.

The *argument_value* can take one of the following forms:

- An *expression*, indicating that the argument is passed as a value parameter ([§14.6.2.2](classes.md#14622-value-parameters)).
- The keyword `ref` followed by a *variable_reference* ([§9.5](variables.md#95-variable-references)), indicating that the argument is passed as a reference parameter ([§14.6.2.3](classes.md#14623-reference-parameters)). A variable shall be definitely assigned ([§9.4](variables.md#94-definite-assignment)) before it can be passed as a reference parameter.
- The keyword `out` followed by a *variable_reference* ([§9.5](variables.md#95-variable-references)), indicating that the argument is passed as an output parameter ([§14.6.2.4](classes.md#14624-output-parameters)). A variable is considered definitely assigned ([§9.4](variables.md#94-definite-assignment)) following a function member invocation in which the variable is passed as an output parameter.

The form determines the ***parameter-passing mode*** of the argument: *value*, *reference*, or *output*, respectively.

Passing a volatile field ([§14.5.4](classes.md#1454-volatile-fields)) as a reference parameter or output parameter causes a warning, since the field may not be treated as volatile by the invoked method.

#### 11.6.2.2 Corresponding parameters

For each argument in an argument list there has to be a corresponding parameter in the function member or delegate being invoked.

The parameter list used in the following is determined as follows:

- For virtual methods and indexers defined in classes, the parameter list is picked from the first declaration or override of the function member found when starting with the static type of the receiver, and searching through its base classes.
- For partial methods, the parameter list of the defining partial method declaration is used.
- For all other function members and delegates there is only a single parameter list, which is the one used.

The position of an argument or parameter is defined as the number of arguments or parameters preceding it in the argument list or parameter list.

The corresponding parameters for function member arguments are established as follows:

- Arguments in the *argument_list* of instance constructors, methods, indexers and delegates:
  - A positional argument where a parameter occurs at the same position in the parameter list corresponds to that parameter, unless the parameter is a parameter array and the function member is invoked in its expanded form.
  - A positional argument of a function member with a parameter array invoked in its expanded form, which occurs at or after the position of the parameter array in the parameter list, corresponds to an element in the parameter array.
  - A named argument corresponds to the parameter of the same name in the parameter list.
  - For indexers, when invoking the set accessor, the expression specified as the right operand of the assignment operator corresponds to the implicit `value` parameter of the set accessor declaration.
- For properties, when invoking the get accessor there are no arguments. When invoking the set accessor, the expression specified as the right operand of the assignment operator corresponds to the implicit value parameter of the set accessor declaration.
- For user-defined unary operators (including conversions), the single operand corresponds to the single parameter of the operator declaration.
- For user-defined binary operators, the left operand corresponds to the first parameter, and the right operand corresponds to the second parameter of the operator declaration.

#### 11.6.2.3 Run-time evaluation of argument lists

During the run-time processing of a function member invocation ([§11.6.6](expressions.md#1166-function-member-invocation)), the expressions or variable references of an argument list are evaluated in order, from left to right, as follows:

- For a value parameter, the argument expression is evaluated and an implicit conversion ([§10.2](conversions.md#102-implicit-conversions)) to the corresponding parameter type is performed. The resulting value becomes the initial value of the value parameter in the function member invocation.
- For a reference or output parameter, the variable reference is evaluated and the resulting storage location becomes the storage location represented by the parameter in the function member invocation. If the variable reference given as a reference or output parameter is an array element of a *reference_type*, a run-time check is performed to ensure that the element type of the array is identical to the type of the parameter. If this check fails, a `System.ArrayTypeMismatchException` is thrown.

Methods, indexers, and instance constructors may declare their right-most parameter to be a parameter array ([§14.6.2.5](classes.md#14625-parameter-arrays)). Such function members are invoked either in their normal form or in their expanded form depending on which is applicable ([§11.6.4.2](expressions.md#11642-applicable-function-member)):

- When a function member with a parameter array is invoked in its normal form, the argument given for the parameter array shall be a single expression that is implicitly convertible ([§10.2](conversions.md#102-implicit-conversions)) to the parameter array type. In this case, the parameter array acts precisely like a value parameter.
- When a function member with a parameter array is invoked in its expanded form, the invocation shall specify zero or more positional arguments for the parameter array, where each argument is an expression that is implicitly convertible ([§10.2](conversions.md#102-implicit-conversions)) to the element type of the parameter array. In this case, the invocation creates an instance of the parameter array type with a length corresponding to the number of arguments, initializes the elements of the array instance with the given argument values, and uses the newly created array instance as the actual argument.

The expressions of an argument list are always evaluated in textual order.

> *Example*: Thus, the example
>
> ```csharp
> class Test
> {
>     static void F(int x, int y = -1, int z = -2) =>
>         System.Console.WriteLine($"x = {x}, y = {y}, z = {z}");
>
>     static void Main()
>     {
>         int i = 0;
>         F(i++, i++, i++);
>         F(z: i++, x: i++);
>     }
> }
> ```
>
> produces the output
>
> ```console
> x = 0, y = 1, z = 2
> x = 4, y = -1, z = 3
> ```
>
> *end example*

The array co-variance rules ([§16.6](arrays.md#166-array-covariance)) permit a value of an array type `A[]` to be a reference to an instance of an array type `B[]`, provided an implicit reference conversion exists from `B` to `A`. Because of these rules, when an array element of a *reference_type* is passed as a reference or output parameter, a run-time check is required to ensure that the actual element type of the array is *identical* to that of the parameter.

> *Example*: In the following code
>
> ```csharp
> class Test
> {
>     static void F(ref object x) {...}
>
>     static void Main()
>     {
>         object[] a = new object[10];
>         object[] b = new string[10];
>         F(ref a[0]); // Ok
>         F(ref b[1]); // ArrayTypeMismatchException
>     }
> }
> ```
>
> the second invocation of `F` causes a `System.ArrayTypeMismatchException` to be thrown because the actual element type of `b` is `string` and not `object`.
>
> *end example*

When a function member with a parameter array is invoked in its expanded form with at least one expanded argument, the invocation is processed as if an array creation expression with an array initializer ([§11.7.15.5](expressions.md#117155-array-creation-expressions)) was inserted around the expanded arguments. An empty array is passed when there are no arguments for the parameter array; it is unspecified whether the reference passed is to a newly allocated or existing empty array.

> *Example*: Given the declaration
>
> ```csharp
> void F(int x, int y, params object[] args);
> ```
>
> the following invocations of the expanded form of the method
>
> ```csharp
> F(10, 20, 30, 40);
> F(10, 20, 1, "hello", 3.0);
> ```
>
> correspond exactly to
>
> ```csharp
> F(10, 20, new object[] { 30, 40 });
> F(10, 20, new object[] { 1, "hello", 3.0 });
> ```
>
> *end example*

When arguments are omitted from a function member with corresponding optional parameters, the default arguments of the function member declaration are implicitly passed.

> *Note*: Because these are always constant, their evaluation will not impact the evaluation of the remaining arguments. *end note*

### 11.6.3 Type inference

#### 11.6.3.1 General

When a generic method is called without specifying type arguments, a ***type inference*** process attempts to infer type arguments for the call. The presence of type inference allows a more convenient syntax to be used for calling a generic method, and allows the programmer to avoid specifying redundant type information.

> *Example*: Given the method declaration:
>
> ```csharp
> class Chooser
> {
>     static Random rand = new Random();
>
>     public static T Choose<T>(T first, T second) =>
>         rand.Next(2) == 0 ? first : second;
> }
> ```
>
> it is possible to invoke the `Choose` method without explicitly specifying a type argument:
>
> ```csharp
> int i = Chooser.Choose(5, 213); // Calls Choose<int>
> string s = Chooser.Choose("apple", "banana"); // Calls Choose<string>
> ```
>
> Through type inference, the type arguments `int` and `string` are determined from the arguments to the method.
>
> *end example*

Type inference occurs as part of the binding-time processing of a method invocation ([§11.7.8.2](expressions.md#11782-method-invocations)) and takes place before the overload resolution step of the invocation. When a particular method group is specified in a method invocation, and no type arguments are specified as part of the method invocation, type inference is applied to each generic method in the method group. If type inference succeeds, then the inferred type arguments are used to determine the types of arguments for subsequent overload resolution. If overload resolution chooses a generic method as the one to invoke, then the inferred type arguments are used as the type arguments for the invocation. If type inference for a particular method fails, that method does not participate in overload resolution. The failure of type inference, in and of itself, does not cause a binding-time error. However, it often leads to a binding-time error when overload resolution then fails to find any applicable methods.

If each supplied argument does not correspond to exactly one parameter in the method ([§11.6.2.2](expressions.md#11622-corresponding-parameters)), or there is a non-optional parameter with no corresponding argument, then inference immediately fails. Otherwise, assume that the generic method has the following signature:

`Tₑ M<X₁...Xᵥ>(T₁ p₁ ... Tₓ pₓ)`

With a method call of the form `M(E₁ ...Eₓ)` the task of type inference is to find unique type arguments `S₁...Sᵥ` for each of the type parameters `X₁...Xᵥ` so that the call `M<S₁...Sᵥ>(E₁...Eₓ)` becomes valid.

The process of type inference is described below as an algorithm. A conformant compiler may be implemented using an alternative approach, provided it reaches the same result in all cases.

During the process of inference each type parameter `Xᵢ` is either *fixed* to a particular type `Sᵢ` or *unfixed* with an associated set of *bounds.* Each of the bounds is some type `T`. Initially each type variable `Xᵢ` is unfixed with an empty set of bounds.

Type inference takes place in phases. Each phase will try to infer type arguments for more type variables based on the findings of the previous phase. The first phase makes some initial inferences of bounds, whereas the second phase fixes type variables to specific types and infers further bounds. The second phase may have to be repeated a number of times.

> *Note*: Type inference is also used in other contexts including for conversion of method groups ([§11.6.3.14](expressions.md#116314-type-inference-for-conversion-of-method-groups)) and finding the best common type of a set of expressions ([§11.6.3.15](expressions.md#116315-finding-the-best-common-type-of-a-set-of-expressions)). *end note*

#### 11.6.3.2 The first phase

For each of the method arguments `Eᵢ`:

- If `Eᵢ` is an anonymous function, an *explicit parameter type inference* ([§11.6.3.8](expressions.md#11638-explicit-parameter-type-inferences)) is made *from* `Eᵢ` *to* `Tᵢ`
- Otherwise, if `Eᵢ` has a type `U` and `xᵢ` is a value parameter ([§14.6.2.2](classes.md#14622-value-parameters)) then a *lower-bound inference* ([§11.6.3.10](expressions.md#116310-lower-bound-inferences)) is made *from* `U` *to* `Tᵢ`.
- Otherwise, if `Eᵢ` has a type `U` and `xᵢ` is a reference ([§14.6.2.3](classes.md#14623-reference-parameters)) or output ([§14.6.2.4](classes.md#14624-output-parameters)) parameter then an *exact inference* ([§11.6.3.9](expressions.md#11639-exact-inferences)) is made *from* `U` *to* `Tᵢ`.
- Otherwise, no inference is made for this argument.

#### 11.6.3.3 The second phase

The second phase proceeds as follows:

- All *unfixed* type variables `Xᵢ` which do not *depend on* ([§11.6.3.6](expressions.md#11636-dependence)) any `Xₑ` are fixed ([§11.6.3.12](expressions.md#116312-fixing)).
- If no such type variables exist, all *unfixed* type variables `Xᵢ` are *fixed* for which all of the following hold:
  - There is at least one type variable `Xₑ` that *depends on* `Xᵢ`
  - `Xᵢ` has a non-empty set of bounds
- If no such type variables exist and there are still *unfixed* type variables, type inference fails.
- Otherwise, if no further *unfixed* type variables exist, type inference succeeds.
- Otherwise, for all arguments `Eᵢ` with corresponding parameter type `Tᵢ` where the *output types* ([§11.6.3.5](expressions.md#11635-output-types)) contain *unfixed* type variables `Xₑ` but the *input types* ([§11.6.3.4](expressions.md#11634-input-types)) do not, an *output type inference* ([§11.6.3.7](expressions.md#11637-output-type-inferences)) is made *from* `Eᵢ` *to* `Tᵢ`. Then the second phase is repeated.

#### 11.6.3.4 Input types

If `E` is a method group or implicitly typed anonymous function and `T` is a delegate type or expression tree type then all the parameter types of `T` are *input types of* `E` *with type* `T`.

#### 11.6.3.5 Output types

If `E` is a method group or an anonymous function and `T` is a delegate type or expression tree type then the return type of `T` is an *output type of* `E` *with type* `T`.

#### 11.6.3.6 Dependence

An *unfixed* type variable `Xᵢ` *depends directly on* an *unfixed* type variable `Xₑ` if for some argument `Eᵥ` with type `Tᵥ` `Xₑ` occurs in an *input type* of `Eᵥ` with type `Tᵥ` and `Xᵢ` occurs in an *output type* of `Eᵥ` with type `Tᵥ`.

`Xₑ` *depends on* `Xᵢ` if `Xₑ` *depends directly on* `Xᵢ` or if `Xᵢ` *depends directly on* `Xᵥ` and `Xᵥ` *depends on* `Xₑ`. Thus “*depends on*” is the transitive but not reflexive closure of “*depends directly on*”.

#### 11.6.3.7 Output type inferences

An *output type inference* is made *from* an expression `E` *to* a type T in the following way:

- If `E` is an anonymous function with inferred return type `U` ([§11.6.3.13](expressions.md#116313-inferred-return-type)) and `T` is a delegate type or expression tree type with return type `Tₓ`, then a *lower-bound inference* ([§11.6.3.10](expressions.md#116310-lower-bound-inferences)) is made *from* `U` *to* `Tₓ`.
- Otherwise, if `E` is a method group and `T` is a delegate type or expression tree type with parameter types `T₁...Tᵥ` and return type `Tₓ`, and overload resolution of `E` with the types `T₁...Tᵥ` yields a single method with return type `U`, then a *lower-bound inference* is made *from* `U` *to* `Tₓ`.
- Otherwise, if `E` is an expression with type `U`, then a *lower-bound inference* is made *from* `U` *to* `T`.
- Otherwise, no inferences are made.

#### 11.6.3.8 Explicit parameter type inferences

An *explicit parameter type inference* is made *from* an expression `E` *to* a type `T` in the following way:

- If `E` is an explicitly typed anonymous function with parameter types `U₁...Uᵥ` and `T` is a delegate type or expression tree type with parameter types `V₁...Vᵥ` then for each `Uᵢ` an *exact inference* ([§11.6.3.9](expressions.md#11639-exact-inferences)) is made *from* `Uᵢ` *to* the corresponding `Vᵢ`.

#### 11.6.3.9 Exact inferences

An *exact inference* *from* a type `U` *to* a type `V` is made as follows:

- If `V` is one of the *unfixed* `Xᵢ` then `U` is added to the set of exact bounds for `Xᵢ`.
- Otherwise, sets `V₁...Vₑ` and `U₁...Uₑ` are determined by checking if any of the following cases apply:
  - `V` is an array type `V₁[...]` and `U` is an array type `U₁[...]` of the same rank
  - `V` is the type `V₁?` and `U` is the type `U₁`
  - `V` is a constructed type `C<V₁...Vₑ>` and `U` is a constructed type `C<U₁...Uₑ>`  
  If any of these cases apply then an *exact inference* is made from each `Uᵢ` to the corresponding `Vᵢ`.
- Otherwise, no inferences are made.

#### 11.6.3.10 Lower-bound inferences

A *lower-bound inference from* a type `U` *to* a type `V` is made as follows:

- If `V` is one of the *unfixed* `Xᵢ` then `U` is added to the set of lower bounds for `Xᵢ`.
- Otherwise, if `V` is the type `V₁?` and `U` is the type `U₁?` then a lower bound inference is made from `U₁` to `V₁`.
- Otherwise, sets `U₁...Uₑ` and `V₁...Vₑ` are determined by checking if any of the following cases apply:
  - `V` is an array type `V₁[...]`and `U` is an array type `U₁[...]`of the same rank
  - `V` is one of `IEnumerable<V₁>`, `ICollection<V₁>`, `IReadOnlyList<V₁>>`, `IReadOnlyCollection<V₁>` or `IList<V₁>` and `U` is a single-dimensional array type `U₁[]`
  - `V` is a constructed `class`, `struct`, `interface` or `delegate` type `C<V₁...Vₑ>` and there is a unique type `C<U₁...Uₑ>` such that `U` (or, if `U` is a type `parameter`, its effective base class or any member of its effective interface set) is identical to, `inherits` from (directly or indirectly), or implements (directly or indirectly) `C<U₁...Uₑ>`.
  - (The “uniqueness” restriction means that in the case interface `C<T>{} class U: C<X>, C<Y>{}`, then no inference is made when inferring from `U` to `C<T>` because `U₁` could be `X` or `Y`.)  
  If any of these cases apply then an inference is made from each `Uᵢ` to the corresponding `Vᵢ` as follows:
  - If `Uᵢ` is not known to be a reference type then an *exact inference* is made
  - Otherwise, if `U` is an array type then a *lower-bound inference* is made
  - Otherwise, if `V` is `C<V₁...Vₑ>` then inference depends on the `i-th` type parameter of `C`:
    - If it is covariant then a *lower-bound inference* is made.
    - If it is contravariant then an *upper-bound inference* is made.
    - If it is invariant then an *exact inference* is made.
- Otherwise, no inferences are made.

#### 11.6.3.11 Upper-bound inferences

An *upper-bound inference from* a type `U` *to* a type `V` is made as follows:

- If `V` is one of the *unfixed* `Xᵢ` then `U` is added to the set of upper bounds for `Xᵢ`.
- Otherwise, sets `V₁...Vₑ` and `U₁...Uₑ` are determined by checking if any of the following cases apply:
  - `U` is an array type `U₁[...]`and `V` is an array type `V₁[...]`of the same rank
  - `U` is one of `IEnumerable<Uₑ>`, `ICollection<Uₑ>`, `IReadOnlyList<Uₑ>`, `IReadOnlyCollection<Uₑ>` or `IList<Uₑ>` and `V` is a single-dimensional array type `Vₑ[]`
  - `U` is the type `U1?` and `V` is the type `V1?`
  - `U` is constructed class, struct, interface or delegate type `C<U₁...Uₑ>` and `V` is a `class, struct, interface` or `delegate` type which is `identical` to, `inherits` from (directly or indirectly), or implements (directly or indirectly) a unique type `C<V₁...Vₑ>`
  - (The “uniqueness” restriction means that if we have interface `C<T>{} class V<Z>: C<X<Z>>, C<Y<Z>>{}`, then no inference is made when inferring from `C<U₁>` to `V<Q>`. Inferences are not made from `U₁` to either `X<Q>` or `Y<Q>`.)  
  If any of these cases apply then an inference is made from each `Uᵢ` to the corresponding `Vᵢ` as follows:
  - If `Uᵢ` is not known to be a reference type then an *exact inference* is made
  - Otherwise, if `V` is an array type then an *upper-bound inference* is made
  - Otherwise, if `U` is `C<U₁...Uₑ>` then inference depends on the `i-th` type parameter of `C`:
    - If it is covariant then an *upper-bound inference* is made.
    - If it is contravariant then a *lower-bound inference* is made.
    - If it is invariant then an *exact inference* is made.
- Otherwise, no inferences are made.

#### 11.6.3.12 Fixing

An *unfixed* type variable `Xᵢ` with a set of bounds is *fixed* as follows:

- The set of *candidate types* `Uₑ` starts out as the set of all types in the set of bounds for `Xᵢ`.
- We then examine each bound for `Xᵢ` in turn: For each exact bound U of `Xᵢ` all types `Uₑ` that are not identical to `U` are removed from the candidate set. For each lower bound `U` of `Xᵢ` all types `Uₑ` to which there is *not* an implicit conversion from `U` are removed from the candidate set. For each upper-bound U of `Xᵢ` all types `Uₑ` from which there is *not* an implicit conversion to `U` are removed from the candidate set.
- If among the remaining candidate types `Uₑ` there is a unique type `V` to which there is an implicit conversion from all the other candidate types, then `Xᵢ` is fixed to `V`.
- Otherwise, type inference fails.

#### 11.6.3.13 Inferred return type

The inferred return type of an anonymous function `F` is used during type inference and overload resolution. The inferred return type can only be determined for an anonymous function where all parameter types are known, either because they are explicitly given, provided through an anonymous function conversion or inferred during type inference on an enclosing generic method invocation.

The ***inferred effective return type*** is determined as follows:

- If the body of `F` is an *expression* that has a type, then the inferred effective return type of `F` is the type of that expression.
- If the body of `F` is a *block* and the set of expressions in the block’s `return` statements has a best common type `T` ([§11.6.3.15](expressions.md#116315-finding-the-best-common-type-of-a-set-of-expressions)), then the inferred effective return type of `F` is `T`.
- Otherwise, an effective return type cannot be inferred for `F`.

The ***inferred return type*** is determined as follows:

- If `F` is async and the body of `F` is either an expression classified as nothing ([§11.2](expressions.md#112-expression-classifications)), or a block where no `return` statements have expressions, the inferred return type is `System.Threading.Tasks.Task`.
- If `F` is async and has an inferred effective return type `T`, the inferred return type is `System.Threading.Tasks.Task<T>`.
- If `F` is non-async and has an inferred effective return type `T`, the inferred return type is `T`.
- Otherwise, a return type cannot be inferred for `F`.

> *Example*: As an example of type inference involving anonymous functions, consider the `Select` extension method declared in the `System.Linq.Enumerable` class:
>
> ```csharp
> namespace System.Linq
> {
>     public static class Enumerable
>     {
>         public static IEnumerable<TResult> Select<TSource,TResult>(
>             this IEnumerable<TSource> source,
>             Func<TSource,TResult> selector)
>         {
>             foreach (TSource element in source)
>             {
>                 yield return selector(element);
>             }
>         }
>    }
> }
> ```
>
> Assuming the `System.Linq` namespace was imported with a `using namespace` directive, and given a class `Customer` with a `Name` property of type `string`, the `Select` method can be used to select the names of a list of customers:
>
> ```csharp
> List<Customer> customers = GetCustomerList();
> IEnumerable<string> names = customers.Select(c => c.Name);
> ```
>
> The extension method invocation ([§11.7.8.3](expressions.md#11783-extension-method-invocations)) of `Select` is processed by rewriting the invocation to a static method invocation:
>
> ```csharp
> IEnumerable<string> names = Enumerable.Select(customers, c => c.Name);
> ```
>
> Since type arguments were not explicitly specified, type inference is used to infer the type arguments. First, the customers argument is related to the source parameter, inferring `TSource` to be `Customer`. Then, using the anonymous function type inference process described above, `c` is given type `Customer`, and the expression `c.Name` is related to the return type of the selector parameter, inferring `TResult` to be `string`. Thus, the invocation is equivalent to
>
> ```csharp
> Sequence.Select<Customer,string>(customers, (Customer c) => c.Name)
> ```
>
> and the result is of type `IEnumerable<string>`.
>
> The following example demonstrates how anonymous function type inference allows type information to “flow” between arguments in a generic method invocation. Given the method:
>
> ```csharp
> static Z F<X,Y,Z>(X value, Func<X,Y> f1, Func<Y,Z> f2)
> {
>    return f2(f1(value));
> }
> ```
>
> Type inference for the invocation:
>
> ```csharp
> double seconds = F("1:15:30", s => TimeSpan.Parse(s), t => t.TotalSeconds);
> ```
>
> proceeds as follows: First, the argument “1:15:30” is related to the value parameter, inferring `X` to be string. Then, the parameter of the first anonymous function, `s`, is given the inferred type `string`, and the expression `TimeSpan.Parse(s)` is related to the return type of `f1`, inferring `Y` to be `System.TimeSpan`. Finally, the parameter of the second anonymous function, `t`, is given the inferred type `System.TimeSpan`, and the expression `t.TotalSeconds` is related to the return type of `f2`, inferring `Z` to be `double`. Thus, the result of the invocation is of type `double`.
>
> *end example*

#### 11.6.3.14 Type inference for conversion of method groups

Similar to calls of generic methods, type inference shall also be applied when a method group `M` containing a generic method is converted to a given delegate type `D` ([§10.8](conversions.md#108-method-group-conversions)). Given a method

`Tₑ M<X₁...Xᵥ>(T₁ x₁ ... Tₑ xₑ)`

and the method group `M` being assigned to the delegate type `D` the task of type inference is to find type arguments `S₁...Sᵥ` so that the expression:

`M<S₁...Sᵥ>`

becomes compatible ([§19.2](delegates.md#192-delegate-declarations)) with `D`.

Unlike the type inference algorithm for generic method calls, in this case, there are only argument *types*, no argument *expressions*. In particular, there are no anonymous functions and hence no need for multiple phases of inference.

Instead, all `Xᵢ` are considered *unfixed*, and a *lower-bound inference* is made *from* each argument type `Uₑ` of `D` *to* the corresponding parameter type `Tₑ` of `M`. If for any of the `Xᵢ` no bounds were found, type inference fails. Otherwise, all `Xᵢ` are *fixed* to corresponding `Sᵢ`, which are the result of type inference.

#### 11.6.3.15 Finding the best common type of a set of expressions

In some cases, a common type needs to be inferred for a set of expressions. In particular, the element types of implicitly typed arrays and the return types of anonymous functions with *block* bodies are found in this way.

The best common type for a set of expressions `E₁...Eᵥ` is determined as follows:

- A new *unfixed* type variable `X` is introduced.
- For each expression `Ei` an *output type inference* ([§11.6.3.7](expressions.md#11637-output-type-inferences)) is performed from it to `X`.
- `X` is *fixed* ([§11.6.3.12](expressions.md#116312-fixing)), if possible, and the resulting type is the best common type.
- Otherwise inference fails.

> *Note*: Intuitively this inference is equivalent to calling a method `void M<X>(X x₁ ... X xᵥ)` with the `Eᵢ` as arguments and inferring `X`. *end note*

### 11.6.4 Overload resolution

#### 11.6.4.1 General

Overload resolution is a binding-time mechanism for selecting the best function member to invoke given an argument list and a set of candidate function members. Overload resolution selects the function member to invoke in the following distinct contexts within C#:

- Invocation of a method named in an *invocation_expression* ([§11.7.8](expressions.md#1178-invocation-expressions)).
- Invocation of an instance constructor named in an *object_creation_expression* ([§11.7.15.2](expressions.md#117152-object-creation-expressions)).
- Invocation of an indexer accessor through an *element_access* ([§11.7.10](expressions.md#11710-element-access)).
- Invocation of a predefined or user-defined operator referenced in an expression ([§11.4.4](expressions.md#1144-unary-operator-overload-resolution) and [§11.4.5](expressions.md#1145-binary-operator-overload-resolution)).

Each of these contexts defines the set of candidate function members and the list of arguments in its own unique way. For instance, the set of candidates for a method invocation does not include methods marked override ([§11.5](expressions.md#115-member-lookup)), and methods in a base class are not candidates if any method in a derived class is applicable ([§11.7.8.2](expressions.md#11782-method-invocations)).

Once the candidate function members and the argument list have been identified, the selection of the best function member is the same in all cases:

- First, the set of candidate function members is reduced to those function members that are applicable with respect to the given argument list ([§11.6.4.2](expressions.md#11642-applicable-function-member)). If this reduced set is empty, a compile-time error occurs.
- Then, the best function member from the set of applicable candidate function members is located. If the set contains only one function member, then that function member is the best function member. Otherwise, the best function member is the one function member that is better than all other function members with respect to the given argument list, provided that each function member is compared to all other function members using the rules in [§11.6.4.3](expressions.md#11643-better-function-member). If there is not exactly one function member that is better than all other function members, then the function member invocation is ambiguous and a binding-time error occurs.

The following subclauses define the exact meanings of the terms *applicable function member* and *better function member*.

#### 11.6.4.2 Applicable function member

A function member is said to be an ***applicable function member*** with respect to an argument list `A` when all of the following are true:

- Each argument in `A` corresponds to a parameter in the function member declaration as described in [§11.6.2.2](expressions.md#11622-corresponding-parameters), at most one argument corresponds to each parameter, and any parameter to which no argument corresponds is an optional parameter.
- For each argument in `A`, the parameter-passing mode of the argument is identical to the parameter-passing mode of the corresponding parameter, and
  - for a value parameter or a parameter array, an implicit conversion ([§10.2](conversions.md#102-implicit-conversions)) exists from the argument expression to the type of the corresponding parameter, or
  - for a `ref` or `out` parameter, there is an identity conversion between the type of the argument expression and the type of the corresponding parameter

For a function member that includes a parameter array, if the function member is applicable by the above rules, it is said to be applicable in its ***normal form***. If a function member that includes a parameter array is not applicable in its normal form, the function member might instead be applicable in its ***expanded form***:

- The expanded form is constructed by replacing the parameter array in the function member declaration with zero or more value parameters of the element type of the parameter array such that the number of arguments in the argument list `A` matches the total number of parameters. If `A` has fewer arguments than the number of fixed parameters in the function member declaration, the expanded form of the function member cannot be constructed and is thus not applicable.
- Otherwise, the expanded form is applicable if for each argument in `A` the parameter-passing mode of the argument is identical to the parameter-passing mode of the corresponding parameter, and
  - for a fixed value parameter or a value parameter created by the expansion, an implicit conversion ([§10.2](conversions.md#102-implicit-conversions)) exists from the argument expression to the type of the corresponding parameter, or
  - for a `ref` or `out` parameter, the type of the argument expression is identical to the type of the corresponding parameter.

#### 11.6.4.3 Better function member

For the purposes of determining the better function member, a stripped-down argument list `A` is constructed containing just the argument expressions themselves in the order they appear in the original argument list.

Parameter lists for each of the candidate function members are constructed in the following way:

- The expanded form is used if the function member was applicable only in the expanded form.
- Optional parameters with no corresponding arguments are removed from the parameter list
- The parameters are reordered so that they occur at the same position as the corresponding argument in the argument list.

Given an argument list `A` with a set of argument expressions `{E₁, E₂, ..., Eᵥ}` and two applicable function members `Mᵥ` and `Mₓ` with parameter types `{P₁, P₂, ..., Pᵥ}` and `{Q₁, Q₂, ..., Qᵥ}`, `Mᵥ` is defined to be a ***better function member*** than `Mₓ` if

- for each argument, the implicit conversion from `Eᵥ` to `Qᵥ` is not better than the implicit conversion from `Eᵥ` to `Pᵥ`, and
- for at least one argument, the conversion from `Eᵥ` to `Pᵥ` is better than the conversion from `Eᵥ` to `Qᵥ`.

In case the parameter type sequences `{P₁, P₂, ..., Pᵥ}` and `{Q₁, Q₂, ..., Qᵥ}` are equivalent (i.e., each `Pᵢ` has an identity conversion to the corresponding `Qᵢ`), the following tie-breaking rules are applied, in order, to determine the better function member.

- If `Mᵢ` is a non-generic method and `Mₑ` is a generic method, then `Mᵢ` is better than `Mₑ`.
- Otherwise, if `Mᵢ` is applicable in its normal form and `Mₑ` has a params array and is applicable only in its expanded form, then `Mᵢ` is better than `Mₑ`.
- Otherwise, if both methods have params arrays and are applicable only in their expanded forms, and if the params array of `Mᵢ` has fewer elements than the params array of `Mₑ`, then `Mᵢ` is better than `Mₑ`.
- Otherwise, if `Mᵥ` has more specific parameter types than `Mₓ`, then `Mᵥ` is better than `Mₓ`. Let `{R1, R2, ..., Rn}` and `{S1, S2, ..., Sn}` represent the uninstantiated and unexpanded parameter types of `Mᵥ` and `Mₓ`. `Mᵥ`’s parameter types are more specific than `Mₓ`s if, for each parameter, `Rx` is not less specific than `Sx`, and, for at least one parameter, `Rx` is more specific than `Sx`:
  - A type parameter is less specific than a non-type parameter.
  - Recursively, a constructed type is more specific than another constructed type (with the same number of type arguments) if at least one type argument is more specific and no type argument is less specific than the corresponding type argument in the other.
  - An array type is more specific than another array type (with the same number of dimensions) if the element type of the first is more specific than the element type of the second.
- Otherwise if one member is a non-lifted operator and the other is a lifted operator, the non-lifted one is better.
- If neither function member was found to be better, and all parameters of `Mᵥ` have a corresponding argument whereas default arguments need to be substituted for at least one optional parameter in `Mₓ`, then `Mᵥ` is better than `Mₓ`. Otherwise, no function member is better.

#### 11.6.4.4 Better conversion from expression

Given an implicit conversion `C₁` that converts from an expression `E` to a type `T₁`, and an implicit conversion `C₂` that converts from an expression `E` to a type `T₂`, `C₁` is a ***better conversion*** than `C₂` if one of the following holds:

- `E` exactly matches `T₁` and `E` does not exactly match `T₂` ([§11.6.4.5](expressions.md#11645-exactly-matching-expression))
- `E` exactly matches both or neither of `T₁` and `T₂`, and `T₁` is a better conversion target than `T₂` ([§11.6.4.6](expressions.md#11646-better-conversion-target))
- `E` is a method group ([§11.2](expressions.md#112-expression-classifications)), `T₁` is compatible ([§19.4](delegates.md#194-delegate-compatibility)) with the single best method from the method group for conversion `C₁`, and `T₂` is not compatible with the single best method from the method group for conversion `C₂`

#### 11.6.4.5 Exactly matching expression

Given an expression `E` and a type `T`, `E` ***exactly matches*** `T` if one of the following holds:

- `E` has a type `S`, and an identity conversion exists from `S` to `T`
- `E` is an anonymous function, `T` is either a delegate type `D` or an expression tree type `Expression<D>` and one of the following holds:
  - An inferred return type `X` exists for `E` in the context of the parameter list of `D` ([§11.6.3.12](expressions.md#116312-fixing)), and an identity conversion exists from `X` to the return type of `D`
  - Either `E` is non-async and `D` has a return type `Y` or `E` is async and  `D` has a return type `Task<Y>`, and one of the following holds:
    - The body of `E` is an expression that exactly matches `Y`
    - The body of `E` is a block where every return statement returns an expression that exactly matches `Y`

#### 11.6.4.6 Better conversion target

Given two types `T₁` and `T₂`, `T₁` is a ***better conversion target*** than `T₂` if one of the following holds:

- An implicit conversion from `T₁` to `T₂` exists and no implicit conversion from `T₂` to `T₁` exists
- `T₁` is `Task<S₁>`, `T₂` is `Task<S₂>`, and `S₁` is a better conversion target than `S₂`
- `T₁` is `S₁` or `S₁?` where `S₁` is a signed integral type, and `T₂` is `S₂` or `S₂?` where `S₂` is an unsigned integral type. Specifically:
  - `S₁` is `sbyte` and `S₂` is `byte`, `ushort`, `uint`, or `ulong`
  - `S₁` is `short` and `S₂` is `ushort`, `uint`, or `ulong`
  - `S₁` is `int` and `S₂` is `uint`, or `ulong`
  - `S₁` is `long` and `S₂` is `ulong`

#### 11.6.4.7 Overloading in generic classes

> *Note*: While signatures as declared shall be unique ([§8.6](types.md#86-expression-tree-types)), it is possible that substitution of type arguments results in identical signatures. In such a situation, overload resolution will pick the most specific ([§11.6.4.3](expressions.md#11643-better-function-member)) of the original signatures (before substitution of type arguments), if it exists, and otherwise report an error. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The following examples show overloads that are valid and invalid according to this rule:
>
> ```csharp
> interface I1<T> {...}
> interface I2<T> {...}
>
> class G1<U>
> {
>     int F1(U u);               // Overload resulotion for G<int>.F1
>     int F1(int i);             // will pick non-generic
>     void F2(I1<U> a);          // Valid overload
>     void F2(I2<U> a);
> }
>
> class G2<U,V>
> {
>     void F3(U u, V v);         // Valid, but overload resolution for
>     void F3(V v, U u);         // G2<int,int>.F3 will fail
>     void F4(U u, I1<V> v);     // Valid, but overload resolution for
>     void F4(I1<V> v, U u);     // G2<I1<int>,int>.F4 will fail
>     void F5(U u1, I1<V> v2);   // Valid overload
>     void F5(V v1, U u2);
>     void F6(ref U u);          // valid overload
>     void F6(out V v);
> }
> ```
>
> *end example*

### 11.6.5 Compile-time checking of dynamic member invocation

Even though overload resolution of a dynamically bound operation takes place at run-time, it is sometimes possible at compile-time to know the list of function members from which an overload will be chosen:

- For a delegate invocation ([§11.7.8.4](expressions.md#11784-delegate-invocations)), the list is a single function member with the same parameter list as the *delegate_type* of the invocation
- For a method invocation ([§11.7.8.2](expressions.md#11782-method-invocations)) on a type, or on a value whose static type is not dynamic, the set of accessible methods in the method group is known at compile-time.
- For an object creation expression ([§11.7.15.2](expressions.md#117152-object-creation-expressions)) the set of accessible constructors in the type is known at compile-time.
- For an indexer access ([§11.7.10.3](expressions.md#117103-indexer-access)) the set of accessible indexers in the receiver is known at compile-time.

In these cases a limited compile-time check is performed on each member in the known set of function members, to see if it can be known for certain never to be invoked at run-time. For each function member `F` a modified parameter and argument list are constructed:

- First, if `F` is a generic method and type arguments were provided, then those are substituted for the type parameters in the parameter list. However, if type arguments were not provided, no such substitution happens.
- Then, any parameter whose type is open (i.e., contains a type parameter; see [§8.4.3](types.md#843-open-and-closed-types)) is elided, along with its corresponding parameter(s).

For `F` to pass the check, all of the following shall hold:

- The modified parameter list for `F` is applicable to the modified argument list in terms of [§11.6.4.2](expressions.md#11642-applicable-function-member).
- All constructed types in the modified parameter list satisfy their constraints ([§8.4.5](types.md#845-satisfying-constraints)).
- If the type parameters of `F` were substituted in the step above, their constraints are satisfied.
- If `F` is a static method, the method group shall not have resulted from a *member_access* whose receiver is known at compile-time to be a variable or value.
- If `F` is an instance method, the method group shall not have resulted from a *member_access* whose receiver is known at compile-time to be a type.

If no candidate passes this test, a compile-time error occurs.

### 11.6.6 Function member invocation

#### 11.6.6.1 General

This subclause describes the process that takes place at run-time to invoke a particular function member. It is assumed that a binding-time process has already determined the particular member to invoke, possibly by applying overload resolution to a set of candidate function members.

For purposes of describing the invocation process, function members are divided into two categories:

- Static function members. These are static methods, static property accessors, and user-defined operators. Static function members are always non-virtual.
- Instance function members. These are instance methods, instance constructors, instance property accessors, and indexer accessors. Instance function members are either non-virtual or virtual, and are always invoked on a particular instance. The instance is computed by an instance expression, and it becomes accessible within the function member as `this` ([§11.7.12](expressions.md#11712-this-access)). For an instance constructor, the instance expression is taken to be the newly allocated object.

The run-time processing of a function member invocation consists of the following steps, where `M` is the function member and, if `M` is an instance member, `E` is the instance expression:

- If `M` is a static function member:
  - The argument list is evaluated as described in [§11.6.2](expressions.md#1162-argument-lists).
  - `M` is invoked.
- Otherwise, if the type of `E` is a value-type `V`, and `M` is declared or overridden in `V`:
  - `E` is evaluated. If this evaluation causes an exception, then no further steps are executed. For an instance constructor, this evaluation consists of allocating storage (typically from an execution stack) for the new object. In this case `E` is classified as a variable.
  - If `E` is not classified as a variable, then a temporary local variable of `E`’s type is created and the value of `E` is assigned to that variable. `E` is then reclassified as a reference to that temporary local variable. The temporary variable is accessible as `this` within `M`, but not in any other way. Thus, only when `E` is a true variable is it possible for the caller to observe the changes that `M` makes to `this`.
  - The argument list is evaluated as described in [§11.6.2](expressions.md#1162-argument-lists).
  - `M` is invoked. The variable referenced by `E` becomes the variable referenced by `this`.
- Otherwise:
  - `E` is evaluated. If this evaluation causes an exception, then no further steps are executed.
  - The argument list is evaluated as described in [§11.6.2](expressions.md#1162-argument-lists).
  - If the type of `E` is a *value_type*, a boxing conversion ([§10.2.9](conversions.md#1029-boxing-conversions)) is performed to convert `E` to a *class_type*, and `E` is considered to be of that *class_type* in the following steps. If the *value_type* is an *enum_type*, the *class_type* is `System.Enum;` otherwise, it is `System.ValueType`.
  - The value of `E` is checked to be valid. If the value of `E` is null, a `System.NullReferenceException` is thrown and no further steps are executed.
  - The function member implementation to invoke is determined:
    - If the binding-time type of `E` is an interface, the function member to invoke is the implementation of `M` provided by the run-time type of the instance referenced by `E`. This function member is determined by applying the interface mapping rules ([§17.6.5](interfaces.md#1765-interface-mapping)) to determine the implementation of `M` provided by the run-time type of the instance referenced by `E`.
    - Otherwise, if `M` is a virtual function member, the function member to invoke is the implementation of `M` provided by the run-time type of the instance referenced by `E`. This function member is determined by applying the rules for determining the most derived implementation ([§14.6.4](classes.md#1464-virtual-methods)) of `M` with respect to the run-time type of the instance referenced by `E`.
    - Otherwise, `M` is a non-virtual function member, and the function member to invoke is `M` itself.
  - The function member implementation determined in the step above is invoked. The object referenced by `E` becomes the object referenced by this.

The result of the invocation of an instance constructor ([§11.7.15.2](expressions.md#117152-object-creation-expressions)) is the value created. The result of the invocation of any other function member is the value, if any, returned ([§12.10.5](statements.md#12105-the-return-statement)) from its body.

#### 11.6.6.2 Invocations on boxed instances

A function member implemented in a *value_type* can be invoked through a boxed instance of that *value_type* in the following situations:

- When the function member is an override of a method inherited from type *class_type* and is invoked through an instance expression of that *class_type*.
  > *Note*: The *class_type* will always be one of `System.Object`, `System.ValueType` or `System.Enum`. *end note*
- When the function member is an implementation of an interface function member and is invoked through an instance expression of an *interface_type*.
- When the function member is invoked through a delegate.

In these situations, the boxed instance is considered to contain a variable of the *value_type*, and this variable becomes the variable referenced by this within the function member invocation.

> *Note*: In particular, this means that when a function member is invoked on a boxed instance, it is possible for the function member to modify the value contained in the boxed instance. *end note*

## 11.7 Primary expressions

### 11.7.1 General

Primary expressions include the simplest forms of expressions.

```ANTLR
primary_expression
    : primary_no_array_creation_expression
    | array_creation_expression
    ;

primary_no_array_creation_expression
    : literal
    | interpolated_string_expression
    | simple_name
    | parenthesized_expression
    | member_access
    | null_conditional_member_access
    | invocation_expression
    | element_access
    | null_conditional_element_access
    | this_access
    | base_access
    | post_increment_expression
    | post_decrement_expression
    | object_creation_expression
    | delegate_creation_expression
    | anonymous_object_creation_expression
    | typeof_expression
    | sizeof_expression
    | checked_expression
    | unchecked_expression
    | default_value_expression
    | nameof_expression    
    | anonymous_method_expression
    | pointer_member_access     // unsafe code support
    | pointer_element_access    // unsafe code support
    ;
```

> *Note*: These grammar rules are not ANTLR-ready as they are part of a set of mutually left-recursive rules (`primary_expression`, `primary_no_array_creation_expression`, `member_access`, `invocation_expression`, `element_access`, `post_increment_expression`, `post_decrement_expression`, `pointer_member_access` and `pointer_element_access`) which ANTLR does not handle. Standard techniques can be used to transform the grammar to remove the mutual left-recursion. This has not been done as not all parsing strategies require it (e.g. an LALR parser would not) and doing so would obfuscate the structure and description.

*pointer_member_access* ([§22.6.3](unsafe-code.md#2263-pointer-member-access)) and *pointer_element_access* ([§22.6.4](unsafe-code.md#2264-pointer-element-access)) are only available in unsafe code ([§22](unsafe-code.md#22-unsafe-code)).

Primary expressions are divided between *array_creation_expression*s and *primary_no_array_creation_expression*s. Treating *array_creation_expression* in this way, rather than listing it along with the other simple expression forms, enables the grammar to disallow potentially confusing code such as

```csharp
object o = new int[3][1];
```

which would otherwise be interpreted as

```csharp
object o = (new int[3])[1];
```

### 11.7.2 Literals

A *primary_expression* that consists of a *literal* ([§6.4.5](lexical-structure.md#645-literals)) is classified as a value.

### 11.7.3 Interpolated string expressions

An *interpolated_string_expression* consists of a `$` character immediately followed by text within `"` characters. Within the quoted text there are zero or more ***interpolations*** delimited by `{` and `}` characters, each of which encloses an *expression* and optional formatting specifications.

Interpolated string expressions have two forms; regular (*interpolated_regular_string_expression*)
and verbatim (*interpolated_verbatim_string_expression*); which are lexically similar to, but differ semantically from, the two forms of string
literals ([§6.4.5.6](lexical-structure.md#6456-string-literals)).

```ANTLR
interpolated_string_expression
    : interpolated_regular_string_expression
    | interpolated_verbatim_string_expression
    ;

// interpolated regular string expressions

interpolated_regular_string_expression
    : Interpolated_Regular_String_Start Interpolated_Regular_String_Mid?
      ('{' regular_interpolation '}' Interpolated_Regular_String_Mid?)*
      Interpolated_Regular_String_End
    ;

regular_interpolation
    : expression (',' interpolation_minimum_width)?
      Regular_Interpolation_Format?
    ;

interpolation_minimum_width
    : constant_expression
    ;

Interpolated_Regular_String_Start
    : '$"'
    ;

// the following three lexical rules are context sensitive, see details below

Interpolated_Regular_String_Mid
    : Interpolated_Regular_String_Element+
    ;

Regular_Interpolation_Format
    : ':' Interpolated_Regular_String_Element+
    ;

Interpolated_Regular_String_End
    : '"'
    ;

fragment Interpolated_Regular_String_Element
    : Interpolated_Regular_String_Character
    | Simple_Escape_Sequence
    | Hexadecimal_Escape_Sequence
    | Unicode_Escape_Sequence
    | Open_Brace_Escape_Sequence
    | Close_Brace_Escape_Sequence
    ;

fragment Interpolated_Regular_String_Character
    // Any character except " (U+0022), \\ (U+005C),
    // { (U+007B), } (U+007D), and New_Line_Character.
    : ~["\\{}\u000D\u000A\u0085\u2028\u2029]
    ;

// interpolated verbatim string expressions

interpolated_verbatim_string_expression
    : Interpolated_Verbatim_String_Start Interpolated_Verbatim_String_Mid?
      ('{' verbatim_interpolation '}' Interpolated_Verbatim_String_Mid?)*
      Interpolated_Verbatim_String_End
    ;

verbatim_interpolation
    : expression (',' interpolation_minimum_width)?
      Verbatim_Interpolation_Format?
    ;

Interpolated_Verbatim_String_Start
    : '$@"'
    ;

// the following three lexical rules are context sensitive, see details below

Interpolated_Verbatim_String_Mid
    : Interpolated_Verbatim_String_Element+
    ;

Verbatim_Interpolation_Format
    : ':' Interpolated_Verbatim_String_Element+
    ;

Interpolated_Verbatim_String_End
    : '"'
    ;

fragment Interpolated_Verbatim_String_Element
    : Interpolated_Verbatim_String_Character
    | Quote_Escape_Sequence
    | Open_Brace_Escape_Sequence
    | Close_Brace_Escape_Sequence
    ;

fragment Interpolated_Verbatim_String_Character
    : ~["{}]    // Any character except " (U+0022), { (U+007B) and } (U+007D)
    ;

// lexical fragments used by both regular and verbatim interpolated strings

fragment Open_Brace_Escape_Sequence
    : '{{'
    ;

fragment Close_Brace_Escape_Sequence
    : '}}'
    ;
```

Six of the lexical rules defined above are *context sensitive* as follows:

| **Rule** | **Contextual Requirements** |
| :------- | :-------------------------- |
| *Interpolated_Regular_String_Mid* | Only recognised after an *Interpolated_Regular_String_Start*, between any following interpolations, and before the corresponding *Interpolated_Regular_String_End*. |
| *Regular_Interpolation_Format* | Only recognised within a *regular_interpolation* and when the starting colon (:) is not nested within any kind of bracket (parentheses/braces/square). |
| *Interpolated_Regular_String_End* | Only recognised after an *Interpolated_Regular_String_Start* and only if any intervening tokens are either *Interpolated_Regular_String_Mid*s or tokens that can be part of *regular_interpolation*s, including tokens for any *interpolated_regular_string_expression*s contained within such interpolations. |
| *Interpolated_Verbatim_String_Mid* *Verbatim_Interpolation_Format* *Interpolated_Verbatim_String_End* | Recognition of these three rules follows that of the corresponding rules above with each mentioned *regular* grammar rule replaced by the corresponding *verbatim* one. |

> *Note:* The above rules are context sensitive as their definitions overlap with those of
other tokens in the language. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note:* The above grammar is not ANTLR-ready due to the context sensitive lexical rules. As with
other lexer generators ANTLR supports context sensitive lexical rules, for example using its *lexical modes*,
but this is an implementation detail and therefore not part of this Standard. *end note*

An *interpolated_string_expression* is classified as a value. If it is immediately converted to `System.IFormattable` or `System.FormattableString` with an implicit interpolated string conversion ([§10.2.5](conversions.md#1025-implicit-interpolated-string-conversions)), the interpolated string expression has that type. Otherwise, it has the type `string`.

> *Note:* The differences between the possible types an *interpolated_string_expression* may be determined from the documentation for `System.String` ([§C.2](standard-library.md#c2-standard-library-types-defined-in-isoiec-23271)) and `System.FormattableString` ([§C.3](standard-library.md#c3-standard-library-types-not-defined-in-isoiec-23271)). *end note*

The meaning of an interpolation, both *regular_interpolation* and *verbatim_interpolation*, is to format the value of the *expression* as a `string` either according to the format specified by the *Regular_Interpolation_Format* or *Verbatim_Interpolation_Format*, or according to a default format for the type of *expression*. The formatted string is then modified by the *interpolation_minimum_width*, if any, to produce the final `string` to be interpolated into the *interpolated_string_expression*.

> *Note:* How the default format for a type is determined is detailed in the documentation for `System.String` ([§C.2](standard-library.md#c2-standard-library-types-defined-in-isoiec-23271)) and `System.FormattableString` ([§C.3](standard-library.md#c3-standard-library-types-not-defined-in-isoiec-23271)). Descriptions of standard formats, which are identical for *Regular_Interpolation_Format* and *Verbatim_Interpolation_Format*, may be found in the documentation for `System.IFormattable` ([§C.4](standard-library.md#c4-format-specifications)) and in other types in the standard library ([§C](standard-library.md#annex-c-standard-library)). *end note*

In an *interpolation_minimum_width* the *constant_expression* shall have an implicit conversion to `int`. Let the *field width* be the absolute value of this *constant_expression* and the *alignment* be the sign (positive or negative) of the value of this *constant_expression*:

- If the value of field width is less than or equal to the length of the formatted string the formatted string is not modified.
- Otherwise the formatted string is padded with white space characters so that its length is equal to field width:
  - If the alignment is positive the formatted string is right-aligned by prepending the padding,
  - Otherwise it is left-aligned by appending the padding.

The overall meaning of an *interpolated_string_expression*, including the above formatting and padding of interpolations, is defined by a conversion of the expression to a method invocation: if the type of the expression is `System.IFormattable` or `System.FormattableString` that method is `System.Runtime.CompilerServices.FormattableStringFactory.Create` ([§C.3](standard-library.md#c3-standard-library-types-not-defined-in-isoiec-23271)) which returns a value of type `System.FormattableString`; otherwise the type must be `string` and the method is `string.Format` ([§C.2](standard-library.md#c2-standard-library-types-defined-in-isoiec-23271)) which returns a value of type `string`.

In both cases, the argument list of the call consists of a *format string literal* with *format specifications* for each interpolation, and an argument for each expression corresponding to the format specifications.

The format string literal is constructed as follows, where `N` is the number of interpolations in the *interpolated_string_expression*. The format string literal consists of, in order:

- The characters of the *Interpolated_Regular_String_Start* or *Interpolated_Verbatim_String_Start*
- The characters of the *Interpolated_Regular_String_Mid* or *Interpolated_Verbatim_String_Mid*, if any
- Then if `N ≥ 1` for each number `I` from `0` to `N-1`:
  - A placeholder specification:
    - A left brace (`{`) character
    - The decimal representation of `I`
    - Then, if the corresponding *regular_interpolation* or *verbatim_interpolation* has a *interpolation_minimum_width*, a comma (`,`) followed by the decimal representation of the value of the *constant_expression*
    - The characters of the *Regular_Interpolation_Format* or *Verbatim_Interpolation_Format*, if any, of the corresponding *regular_interpolation* or *verbatim_interpolation*
    - A right brace (`}`) character
  - The characters of the *Interpolated_Regular_String_Mid* or *Interpolated_Verbatim_String_Mid* immediately following the corresponding interpolation, if any
- Finally the characters of the *Interpolated_Regular_String_End* or *Interpolated_Verbatim_String_End*.

The subsequent arguments are the *expression*s from the interpolations, if any, in order.

When an *interpolated_string_expression* contains multiple interpolations, the expressions in those interpolations are evaluated in textual order from the left to right.

*Example*:

This example uses the following format specification features:

- the `X` format specification which formats integers as uppercase hexadecimal,
- the default format for a `string` value is the value itself,
- positive alignment values that right-justify within the specified minimum field width,
- negative alignment values that left-justify within the specified minimum field width,
- defined constants for the *interpolation_minimum_width*, and
- that `{{` and `}}` are formatted as `{` and `}` respectively.

Given:

```csharp
string text = "red";
int number = 14;
const int width = -4;
```

Then:

| **Interpolated String Expression**   | **Equivalent Meaning As `string`**                            | **Value**    |
| :----------------------------------- | :------------------------------------------------------------ | :----------- |
| `$"{text}"`                          | `string.Format("{0}", text)`                                  | `"red"`      |
| `$"{{text}}"`                        | `string.Format("{{text}})`                                    | `"{text}"`   |
| `$"{ text , 4 }"`                    | `string.Format("{0,4}", text)`                                | `" red"`     |
| `$"{ text , width }"`                | `string.Format("{0,-4}", text)`                               | `"red "`     |
| `$"{number:X}"`                      | `string.Format("{0:X}", number)`                              | `"E"`        |
| `$"{text + '?'} {number % 3}"`       | `string.Format("{0} {1}", text + '?', number % 3)`            | `"red? 2"`   |
| `$"{text + $"[{number}]"}"`          | `string.Format("{0}", text + string.Format("[{0}]", number))` | `"red[14]"`  |
| `$"{(number==0?"Zero":"Non-zero")}"` | `string.Format("{0}", (number==0?"Zero":"Non-zero"))`         | `"Non-zero"` |

*end example*

### 11.7.4 Simple names

A *simple_name* consists of an identifier, optionally followed by a type argument list:

```ANTLR
simple_name
    : identifier type_argument_list?
    ;
```

A *simple_name* is either of the form `I` or of the form `I<A₁, ..., Aₑ>`, where `I` is a single identifier and `I<A₁, ..., Aₑ>` is an optional *type_argument_list*. When no *type_argument_list* is specified, consider `e` to be zero. The *simple_name* is evaluated and classified as follows:

- If `e` is zero and the *simple_name* appears within a *block* and if the *block*’s (or an enclosing *block*’s) local variable declaration space ([§7.3](basic-concepts.md#73-declarations)) contains a local variable, parameter or constant with name `I`, then the *simple_name* refers to that local variable, parameter or constant and is classified as a variable or value.
- If `e` is zero and the *simple_name* appears within a generic method declaration but outside the *attributes* of its *method_header,* and if that declaration includes a type parameter with name `I`, then the *simple_name* refers to that type parameter.
- Otherwise, for each instance type `T` ([§14.3.2](classes.md#1432-the-instance-type)), starting with the instance type of the immediately enclosing type declaration and continuing with the instance type of each enclosing class or struct declaration (if any):
  - If `e` is zero and the declaration of `T` includes a type parameter with name `I`, then the *simple_name* refers to that type parameter.
  - Otherwise, if a member lookup ([§11.5](expressions.md#115-member-lookup)) of `I` in `T` with `e` type arguments produces a match:
    - If `T` is the instance type of the immediately enclosing class or struct type and the lookup identifies one or more methods, the result is a method group with an associated instance expression of `this`. If a type argument list was specified, it is used in calling a generic method ([§11.7.8.2](expressions.md#11782-method-invocations)).
    - Otherwise, if `T` is the instance type of the immediately enclosing class or struct type, if the lookup identifies an instance member, and if the reference occurs within the *block* of an instance constructor, an instance method, or an instance accessor ([§11.2.1](expressions.md#1121-general)), the result is the same as a member access ([§11.7.6](expressions.md#1176-member-access)) of the form `this.I`. This can only happen when `e` is zero.
    - Otherwise, the result is the same as a member access ([§11.7.6](expressions.md#1176-member-access)) of the form `T.I` or `T.I<A₁, ..., Aₑ>`.
- Otherwise, for each namespace `N`, starting with the namespace in which the *simple_name* occurs, continuing with each enclosing namespace (if any), and ending with the global namespace, the following steps are evaluated until an entity is located:
  - If `e` is zero and `I` is the name of a namespace in `N`, then:
    - If the location where the *simple_name* occurs is enclosed by a namespace declaration for `N` and the namespace declaration contains an *extern_alias_directive* or *using_alias_directive* that associates the name `I` with a namespace or type, then the *simple_name* is ambiguous and a compile-time error occurs.
    - Otherwise, the *simple_name* refers to the namespace named `I` in `N`.
  - Otherwise, if `N` contains an accessible type having name `I` and `e` type parameters, then:
    - If `e` is zero and the location where the *simple_name* occurs is enclosed by a namespace declaration for `N` and the namespace declaration contains an *extern_alias_directive* or *using_alias_directive* that associates the name `I` with a namespace or type, then the *simple_name* is ambiguous and a compile-time error occurs.
    - Otherwise, the *namespace_or_type_name* refers to the type constructed with the given type arguments.
  - Otherwise, if the location where the *simple_name* occurs is enclosed by a namespace declaration for `N`:
    - If `e` is zero and the namespace declaration contains an *extern_alias_directive* or *using_alias_directive* that associates the name `I` with an imported namespace or type, then the *simple_name* refers to that namespace or type.
    - Otherwise, if the namespaces imported by the *using_namespace_directive*s of the namespace declaration contain exactly one type having name `I` and `e` type parameters, then the *simple_name* refers to that type constructed with the given type arguments.
    - Otherwise, if the namespaces imported by the *using_namespace_directive*s of the namespace declaration contain more than one type having name `I` and `e` type parameters, then the *simple_name* is ambiguous and a compile-time error occurs.  
  > *Note*: This entire step is exactly parallel to the corresponding step in the processing of a *namespace_or_type_name* ([§7.8](basic-concepts.md#78-namespace-and-type-names)). *end note*
- Otherwise, the *simple_name* is undefined and a compile-time error occurs.

### 11.7.5 Parenthesized expressions

A *parenthesized_expression* consists of an *expression* enclosed in parentheses.

```ANTLR
parenthesized_expression
    : '(' expression ')'
    ;
```

A *parenthesized_expression* is evaluated by evaluating the *expression* within the parentheses. If the *expression* within the parentheses denotes a namespace or type, a compile-time error occurs. Otherwise, the result of the *parenthesized_expression* is the result of the evaluation of the contained *expression*.

### 11.7.6 Member access

#### 11.7.6.1 General

A *member_access* consists of a *primary_expression*, a *predefined_type*, or a *qualified_alias_member*, followed by a “`.`” token, followed by an *identifier*, optionally followed by a *type_argument_list*.

```ANTLR
member_access
    : primary_expression '.' identifier type_argument_list?
    | predefined_type '.' identifier type_argument_list?
    | qualified_alias_member '.' identifier type_argument_list?
    ;

predefined_type
    : 'bool' | 'byte' | 'char' | 'decimal' | 'double' | 'float' | 'int'
    | 'long' | 'object' | 'sbyte' | 'short' | 'string' | 'uint' | 'ulong'
    | 'ushort'
    ;
```

The *qualified_alias_member* production is defined in [§13.8](namespaces.md#138-qualified-alias-member).

A *member_access* is either of the form `E.I` or of the form `E.I<A₁, ..., Aₑ>`, where `E` is a *primary_expression*, *predefined_type* or *qualified_alias_member,* `I` is a single identifier, and `<A₁, ..., Aₑ>` is an optional *type_argument_list*. When no *type_argument_list* is specified, consider `e` to be zero.

A *member_access* with a *primary_expression* of type `dynamic` is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)). In this case, the compiler classifies the member access as a property access of type `dynamic`. The rules below to determine the meaning of the *member_access* are then applied at run-time, using the run-time type instead of the compile-time type of the *primary_expression*. If this run-time classification leads to a method group, then the member access shall be the *primary_expression* of an *invocation_expression*.

The *member_access* is evaluated and classified as follows:

- If `e` is zero and `E` is a namespace and `E` contains a nested namespace with name `I`, then the result is that namespace.
- Otherwise, if `E` is a namespace and `E` contains an accessible type having name `I` and `K` type parameters, then the result is that type constructed with the given type arguments.
- If `E` is classified as a type, if `E` is not a type parameter, and if a member lookup ([§11.5](expressions.md#115-member-lookup)) of `I` in `E` with `K` type parameters produces a match, then `E.I` is evaluated and classified as follows:  
  > *Note*: When the result of such a member lookup is a method group and `K` is zero, the method group can contain methods having type parameters. This allows such methods to be considered for type argument inferencing. *end note*
  - If `I` identifies a type, then the result is that type constructed with any given type arguments.
  - If `I` identifies one or more methods, then the result is a method group with no associated instance expression.
  - If `I` identifies a static property, then the result is a property access with no associated instance expression.
  - If `I` identifies a static field:
    - If the field is readonly and the reference occurs outside the static constructor of the class or struct in which the field is declared, then the result is a value, namely the value of the static field `I` in `E`.
    - Otherwise, the result is a variable, namely the static field `I` in `E`.
  - If `I` identifies a static event:
    - If the reference occurs within the class or struct in which the event is declared, and the event was declared without *event_accessor_declarations* ([§14.8.1](classes.md#1481-general)), then `E.I` is processed exactly as if `I` were a static field.
    - Otherwise, the result is an event access with no associated instance expression.
  - If `I` identifies a constant, then the result is a value, namely the value of that constant.
  - If `I` identifies an enumeration member, then the result is a value, namely the value of that enumeration member.
  - Otherwise, `E.I` is an invalid member reference, and a compile-time error occurs.
- If `E` is a property access, indexer access, variable, or value, the type of which is `T`, and a member lookup ([§11.5](expressions.md#115-member-lookup)) of `I` in `T` with `K` type arguments produces a match, then `E.I` is evaluated and classified as follows:
  - First, if `E` is a property or indexer access, then the value of the property or indexer access is obtained ([§11.2.2](expressions.md#1122-values-of-expressions)) and E is reclassified as a value.
  - If `I` identifies one or more methods, then the result is a method group with an associated instance expression of `E`.
  - If `I` identifies an instance property, then the result is a property access with an associated instance expression of `E` and an associated type that is the type of the property. If `T` is a class type, the associated type is picked from the first declaration or override of the property found when starting with `T`, and searching through its base classes.
  - If `T` is a *class_type* and `I` identifies an instance field of that *class_type*:
    - If the value of `E` is `null`, then a `System.NullReferenceException` is thrown.
    - Otherwise, if the field is readonly and the reference occurs outside an instance constructor of the class in which the field is declared, then the result is a value, namely the value of the field `I` in the object referenced by `E`.
    - Otherwise, the result is a variable, namely the field `I` in the object referenced by `E`.
  - If `T` is a *struct_type* and `I` identifies an instance field of that *struct_type*:
    - If `E` is a value, or if the field is readonly and the reference occurs outside an instance constructor of the struct in which the field is declared, then the result is a value, namely the value of the field `I` in the struct instance given by `E`.
    - Otherwise, the result is a variable, namely the field `I` in the struct instance given by `E`.
  - If `I` identifies an instance event:
    - If the reference occurs within the class or struct in which the event is declared, and the event was declared without *event_accessor_declarations* ([§14.8.1](classes.md#1481-general)), and the reference does not occur as the left-hand side of `a +=` or `-=` operator, then `E.I` is processed exactly as if `I` was an instance field.
    - Otherwise, the result is an event access with an associated instance expression of `E`.
- Otherwise, an attempt is made to process `E.I` as an extension method invocation ([§11.7.8.3](expressions.md#11783-extension-method-invocations)). If this fails, `E.I` is an invalid member reference, and a binding-time error occurs.

#### 11.7.6.2 Identical simple names and type names

In a member access of the form `E.I`, if `E` is a single identifier, and if the meaning of `E` as a *simple_name* ([§11.7.4](expressions.md#1174-simple-names)) is a constant, field, property, local variable, or parameter with the same type as the meaning of `E` as a *type_name* ([§7.8.1](basic-concepts.md#781-general)), then both possible meanings of `E` are permitted. The member lookup of `E.I` is never ambiguous, since `I` shall necessarily be a member of the type `E` in both cases. In other words, the rule simply permits access to the static members and nested types of `E` where a compile-time error would otherwise have occurred.

> *Example*:
>
> ```csharp
> struct Color
> {
>     public static readonly Color White = new Color(...);
>     public static readonly Color Black = new Color(...);
>     public Color Complement() {...}
> }
>
> class A
> {
>     public «Color» Color;              // Field Color of type Color
>
>     void F()
>     {
>         Color = «Color».Black;         // Refers to Color.Black static member
>         Color = Color.Complement();  // Invokes Complement() on Color field
>     }
>
>     static void G()
>     {
>         «Color» c = «Color».White;       // Refers to Color.White static member
>     }
> }
> ```
>
> For expository purposes only, within the `A` class, those occurrences of the `Color` identifier that reference the `Color` type are delimited by `«...»`, and those that reference the `Color` field are not.
>
> *end example*

### 11.7.7 Null Conditional Member Access

A *null_conditional_member_access* is a conditional version of *member_access* ([§11.7.6](expressions.md#1176-member-access)) and it is a binding time error if the result type is `void`. For a null conditional expression where the result type may be `void` see ([§11.7.9](expressions.md#1179-null-conditional-invocation-expression)).

A *null_conditional_member_access* consists of a *primary_expression* followed by the two tokens “`?`” and “`.`”, followed by an *identifier* with an optional *type_argument_list*, followed by zero or more *dependent_access*es.

```ANTLR
null_conditional_member_access
    : primary_expression '?' '.' identifier type_argument_list?
      dependent_access*
    ;
    
dependent_access
    : '.' identifier type_argument_list?    // member access
    | '[' argument_list ']'                 // element access
    | '(' argument_list? ')'                // invocation
    ;

null_conditional_projection_initializer
    : primary_expression '?' '.' identifier type_argument_list?
    ;
```

A  *null_conditional_member_access* expression `E` is of the form `P?.A`. Let `T` be the type of the expression `P.A`. The meaning of `E` is determined as follows:

- If `T` is a type parameter that is not known to be a reference type or a non-nullable value type, a compile-time error occurs.
- If `T` is a non-nullable value type, then the type of `E` is `T?`, and the meaning of `E` is the same as the meaning of:

  ```csharp
  ((object)P == null) ? (T?)null : P.A
  ```

  Except that `P` is evaluated only once.
- Otherwise the type of `E` is `T`, and the meaning of `E` is the same as the meaning of:

  ```csharp
  ((object)P == null) ? null : P.A
  ```

  Except that `P` is evaluated only once.

> *Note*: In an expression of the form:
>
> ```csharp
> P?.A₀?.A₁
> ```
>
> then if `P` evaluates to `null` neither `A₀` or `A₁` are evaluated. The same is true if an expression is a sequence of *null_conditional_member_access* or *null_conditional_element_access* [§11.7.11](expressions.md#11711-null-conditional-element-access) operations.
>
> *end note*

A *null_conditional_projection_initializer* is a restriction of *null_conditional_member_access* and has the same semantics. It only occurs as a projection initializer in an anonymous object creation expression ([§11.7.15.7](expressions.md#117157-anonymous-object-creation-expressions)).

### 11.7.8 Invocation expressions

#### 11.7.8.1 General

An *invocation_expression* is used to invoke a method.

```ANTLR
invocation_expression
    : primary_expression '(' argument_list? ')'
    ;
```

An *invocation_expression* is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)) if at least one of the following holds:

- The *primary_expression* has compile-time type `dynamic`.
- At least one argument of the optional *argument_list* has compile-time type `dynamic`.

In this case, the compiler classifies the *invocation_expression* as a value of type `dynamic`. The rules below to determine the meaning of the *invocation_expression* are then applied at run-time, using the run-time type instead of the compile-time type of those of the *primary_expression* and arguments that have the compile-time type `dynamic`. If the *primary_expression* does not have compile-time type `dynamic`, then the method invocation undergoes a limited compile-time check as described in [§11.6.5](expressions.md#1165-compile-time-checking-of-dynamic-member-invocation).

The *primary_expression* of an *invocation_expression* shall be a method group or a value of a *delegate_type*. If the *primary_expression* is a method group, the *invocation_expression* is a method invocation ([§11.7.8.2](expressions.md#11782-method-invocations)). If the *primary_expression* is a value of a *delegate_type*, the *invocation_expression* is a delegate invocation ([§11.7.8.4](expressions.md#11784-delegate-invocations)). If the *primary_expression* is neither a method group nor a value of a *delegate_type*, a binding-time error occurs.

The optional *argument_list* ([§11.6.2](expressions.md#1162-argument-lists)) provides values or variable references for the parameters of the method.

The result of evaluating an *invocation_expression* is classified as follows:

- If the *invocation_expression* invokes a method or delegate that returns void, the result is nothing. An expression that is classified as nothing is permitted only in the context of a *statement_expression* ([§12.7](statements.md#127-expression-statements)) or as the body of a *lambda_expression* ([§11.16](expressions.md#1116-anonymous-function-expressions)). Otherwise a binding-time error occurs.
- Otherwise, the result is a value, with an associated type of the return type of the method or delegate after any type argument substitutions ([§11.7.8.2](expressions.md#11782-method-invocations)) have been performed. If the invocation is of an instance method, and the receiver is of a class type `T`, the associated type is picked from the first declaration or override of the method found when starting with `T` and searching through its base classes.

#### 11.7.8.2 Method invocations

For a method invocation, the *primary_expression* of the *invocation_expression* shall be a method group. The method group identifies the one method to invoke or the set of overloaded methods from which to choose a specific method to invoke. In the latter case, determination of the specific method to invoke is based on the context provided by the types of the arguments in the *argument_list*.

The binding-time processing of a method invocation of the form `M(A)`, where `M` is a method group (possibly including a *type_argument_list*), and `A` is an optional *argument_list*, consists of the following steps:

- The set of candidate methods for the method invocation is constructed. For each method `F` associated with the method group `M`:
  - If `F` is non-generic, `F` is a candidate when:
    - `M` has no type argument list, and
    - `F` is applicable with respect to `A` ([§11.6.4.2](expressions.md#11642-applicable-function-member)).
  - If `F` is generic and `M` has no type argument list, `F` is a candidate when:
    - Type inference ([§11.6.3](expressions.md#1163-type-inference)) succeeds, inferring a list of type arguments for the call, and
    - Once the inferred type arguments are substituted for the corresponding method type parameters, all constructed types in the parameter list of `F` satisfy their constraints ([§8.4.5](types.md#845-satisfying-constraints)), and the parameter list of `F` is applicable with respect to `A` ([§11.6.4.2](expressions.md#11642-applicable-function-member))
  - If `F` is generic and `M` includes a type argument list, `F` is a candidate when:
    - `F` has the same number of method type parameters as were supplied in the type argument list, and
    - Once the type arguments are substituted for the corresponding method type parameters, all constructed types in the parameter list of `F` satisfy their constraints ([§8.4.5](types.md#845-satisfying-constraints)), and the parameter list of `F` is applicable with respect to `A` ([§11.6.4.2](expressions.md#11642-applicable-function-member)).
- The set of candidate methods is reduced to contain only methods from the most derived types: For each method `C.F` in the set, where `C` is the type in which the method `F` is declared, all methods declared in a base type of `C` are removed from the set. Furthermore, if `C` is a class type other than `object`, all methods declared in an interface type are removed from the set.  
  > *Note*: This latter rule only has an effect when the method group was the result of a member lookup on a type parameter having an effective base class other than `object` and a non-empty effective interface set. *end note*
- If the resulting set of candidate methods is empty, then further processing along the following steps are abandoned, and instead an attempt is made to process the invocation as an extension method invocation ([§11.7.8.3](expressions.md#11783-extension-method-invocations)). If this fails, then no applicable methods exist, and a binding-time error occurs.
- The best method of the set of candidate methods is identified using the overload resolution rules of [§11.6.4](expressions.md#1164-overload-resolution). If a single best method cannot be identified, the method invocation is ambiguous, and a binding-time error occurs. When performing overload resolution, the parameters of a generic method are considered after substituting the type arguments (supplied or inferred) for the corresponding method type parameters.
- ***Final validation*** of the chosen best method is performed:
  - The method is validated in the context of the method group: If the best method is a static method, the method group shall have resulted from a *simple_name* or a *member_access* through a type. If the best method is an instance method, the method group shall have resulted from a *simple_name*, a *member_access* through a variable or value, or a *base_access*. If neither of these requirements is true, a binding-time error occurs.
  - If the best method is a generic method, the type arguments (supplied or inferred) are checked against the constraints ([§8.4.5](types.md#845-satisfying-constraints)) declared on the generic method. If any type argument does not satisfy the corresponding constraint(s) on the type parameter, a binding-time error occurs.

Once a method has been selected and validated at binding-time by the above steps, the actual run-time invocation is processed according to the rules of function member invocation described in [§11.6.6](expressions.md#1166-function-member-invocation).

> *Note*: The intuitive effect of the resolution rules described above is as follows: To locate the particular method invoked by a method invocation, start with the type indicated by the method invocation and proceed up the inheritance chain until at least one applicable, accessible, non-override method declaration is found. Then perform type inference and overload resolution on the set of applicable, accessible, non-override methods declared in that type and invoke the method thus selected. If no method was found, try instead to process the invocation as an extension-method invocation. *end note*

#### 11.7.8.3 Extension method invocations

In a method invocation ([§11.6.6.2](expressions.md#11662-invocations-on-boxed-instances)) of one of the forms

```csharp
«expr» . «identifier» ( )  
«expr» . «identifier» ( «args» )  
«expr» . «identifier» < «typeargs» > ( )  
«expr» . «identifier» < «typeargs» > ( «args» )
```

if the normal processing of the invocation finds no applicable methods, an attempt is made to process the construct as an extension method invocation. If «expr» or any of the «args» has compile-time type `dynamic`, extension methods will not apply.

The objective is to find the best *type_name* `C`, so that the corresponding static method invocation can take place:

```csharp
C . «identifier» ( «expr» )  
C . «identifier» ( «expr» , «args» )  
C . «identifier» < «typeargs» > ( «expr» )  
C . «identifier» < «typeargs» > ( «expr» , «args» )
```

An extension method `Cᵢ.Mₑ` is ***eligible*** if:

- `Cᵢ` is a non-generic, non-nested class
- The name of `Mₑ` is *identifier*
- `Mₑ` is accessible and applicable when applied to the arguments as a static method as shown above
- An implicit identity, reference or boxing conversion exists from *expr* to the type of the first parameter of `Mₑ`.

The search for `C` proceeds as follows:

- Starting with the closest enclosing namespace declaration, continuing with each enclosing namespace declaration, and ending with the containing compilation unit, successive attempts are made to find a candidate set of extension methods:
  - If the given namespace or compilation unit directly contains non-generic type declarations `Cᵢ` with eligible extension methods `Mₑ`, then the set of those extension methods is the candidate set.
  - If namespaces imported by using namespace directives in the given namespace or compilation unit directly contain non-generic type declarations `Cᵢ` with eligible extension methods `Mₑ`, then the set of those extension methods is the candidate set.
- If no candidate set is found in any enclosing namespace declaration or compilation unit, a compile-time error occurs.
- Otherwise, overload resolution is applied to the candidate set as described in [§11.6.4](expressions.md#1164-overload-resolution). If no single best method is found, a compile-time error occurs.
- `C` is the type within which the best method is declared as an extension method.

Using `C` as a target, the method call is then processed as a static method invocation ([§11.6.6](expressions.md#1166-function-member-invocation)).

> *Note*: Unlike an instance method invocation, no exception is thrown when *expr* evaluates to a null reference. Instead, this `null` value is passed to the extension method as it would be via a regular static method invocation. It is up to the extension method implementation to decide how to respond to such a call. *end note*

The preceding rules mean that instance methods take precedence over extension methods, that extension methods available in inner namespace declarations take precedence over extension methods available in outer namespace declarations, and that extension methods declared directly in a namespace take precedence over extension methods imported into that same namespace with a using namespace directive.

> *Example*:
>
> ```csharp
> public static class E
> {
>     public static void F(this object obj, int i) { }
>     public static void F(this object obj, string s) { }
> }
>
> class A { }
>
> class B
> {
>     public void F(int i) { }
> }
>
> class C
> {
>     public void F(object obj) { }
> }
>
> class X
> {
>     static void Test(A a, B b, C c)
>     {
>         a.F(1);            // E.F(object, int)
>         a.F("hello");      // E.F(object, string)
>         b.F(1);            // B.F(int)
>         b.F("hello");      // E.F(object, string)
>         c.F(1);            // C.F(object)
>         c.F("hello");      // C.F(object)
>     }
> }
> ```
>
> In the example, `B`’s method takes precedence over the first extension method, and `C`’s method takes precedence over both extension methods.
>
> ```csharp
> public static class C
> {
>     public static void F(this int i) => Console.WriteLine($"C.F({i})");
>     public static void G(this int i) => Console.WriteLine($"C.G({i})");
>     public static void H(this int i) => Console.WriteLine($"C.H({i})");
> }
>
> namespace N1
> {
>     public static class D
>     {
>         public static void F(this int i) => Console.WriteLine($"D.F({i})");
>         public static void G(this int i) => Console.WriteLine($"D.G({i})");
>     }
> }
>
> namespace N2
> {
>     using N1;
>
>     public static class E
>     {
>         public static void F(this int i) => Console.WriteLine($"E.F({i})");
>     }
>
>     class Test
>     {
>         static void Main(string[] args)
>         {
>             1.F();
>             2.G();
>             3.H();
>         }
>     }
> }
> ```
>
> The output of this example is:
>
> ```console
> E.F(1)
> D.G(2)
> C.H(3)
> ```
>
> `D.G` takes precendece over `C.G`, and `E.F` takes precedence over both `D.F` and `C.F`.
>
> *end example*

#### 11.7.8.4 Delegate invocations

For a delegate invocation, the *primary_expression* of the *invocation_expression* shall be a value of a *delegate_type*. Furthermore, considering the *delegate_type* to be a function member with the same parameter list as the *delegate_type*, the *delegate_type* shall be applicable ([§11.6.4.2](expressions.md#11642-applicable-function-member)) with respect to the *argument_list* of the *invocation_expression*.

The run-time processing of a delegate invocation of the form `D(A)`, where `D` is a *primary_expression* of a *delegate_type* and `A` is an optional *argument_list*, consists of the following steps:

- `D` is evaluated. If this evaluation causes an exception, no further steps are executed.
- The argument list `A` is evaluated. If this evaluation causes an exception, no further steps are executed.
- The value of `D` is checked to be valid. If the value of `D` is `null`, a `System.NullReferenceException` is thrown and no further steps are executed.
- Otherwise, `D` is a reference to a delegate instance. Function member invocations ([§11.6.6](expressions.md#1166-function-member-invocation)) are performed on each of the callable entities in the invocation list of the delegate. For callable entities consisting of an instance and instance method, the instance for the invocation is the instance contained in the callable entity.

See [§19.6](delegates.md#196-delegate-invocation) for details of multiple invocation lists without parameters.

### 11.7.9 Null Conditional Invocation Expression

A *null_conditional_invocation_expression* is syntactically either a *null_conditional_member_access* ([§11.7.7](expressions.md#1177-null-conditional-member-access)) or *null_conditional_element_access* ([§11.7.11](expressions.md#11711-null-conditional-element-access)) where the final *dependent_access* is an invocation expression ([§11.7.8](expressions.md#1178-invocation-expressions)).

A *null_conditional_invocation_expression* occurs within the context of a *statement_expression* ([§12.7](statements.md#127-expression-statements)), *anonymous_function_body* ([§11.16.1](expressions.md#11161-general)), or *method_body* ([§14.6.1](classes.md#1461-general)).

Unlike the syntactically equivalent *null_conditional_member_access* or *null_conditional_element_access*, a *null_conditional_invocation_expression* may be classified as nothing.

```ANTLR
null_conditional_invocation_expression
    : null_conditional_member_access '(' argument_list? ')'
    | null_conditional_element_access '(' argument_list? ')'
    ;
```

A  *null_conditional_invocation_expression* expression `E` is of the form `P?A`; where `A` is the remainder of the syntactically equivalent *null_conditional_member_access* or *null_conditional_element_access*, `A` will therefore start with `.` or `[`. Let `PA` signify the concatention of `P` and `A`.

When `E` occurs as a *statement_expression* the meaning of `E` is the same as the meaning of the *statement*:

```csharp
if ((object)P != null) PA
```

except that `P` is evaluated only once.

When `E` occurs as a *anonymous_function_body* or *method_body* the meaning of `E` depends on its classification:

- If `E` is classified as nothing then its meaning is the same as the meaning of the *block*:

  ```csharp
  { if ((object)P != null) PA; }
  ```

  except that `P` is evaluated only once.
- Otherwise the meaning of `E` is the same as the meaning of the *block*:

  ```csharp
  { return E; }
  ```

  and in turn the meaning of this *block* depends on whether `E` is syntactically equivalent to a *null_conditional_member_access* ([§11.7.7](expressions.md#1177-null-conditional-member-access)) or *null_conditional_element_access* ([§11.7.11](expressions.md#11711-null-conditional-element-access)).

### 11.7.10 Element access

#### 11.7.10.1 General

An *element_access* consists of a *primary_no_array_creation_expression*, followed by a “`[`” token, followed by an *argument_list*, followed by a “`]`” token. The *argument_list* consists of one or more *argument*s, separated by commas.

```ANTLR
element_access
    : primary_no_array_creation_expression '[' argument_list ']'
    ;
```

The *argument_list* of an *element_access* is not allowed to contain `ref` or `out` arguments.

An *element_access* is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)) if at least one of the following holds:

- The *primary_no_array_creation_expression* has compile-time type `dynamic`.
- At least one expression of the *argument_list* has compile-time type `dynamic` and the *primary_no_array_creation_expression* does not have an array type.

In this case, the compiler classifies the *element_access* as a value of type `dynamic`. The rules below to determine the meaning of the *element_access* are then applied at run-time, using the run-time type instead of the compile-time type of those of the *primary_no_array_creation_expression* and *argument_list* expressions which have the compile-time type `dynamic`. If the *primary_no_array_creation_expression* does not have compile-time type `dynamic`, then the element access undergoes a limited compile-time check as described in [§11.6.5](expressions.md#1165-compile-time-checking-of-dynamic-member-invocation).

If the *primary_no_array_creation_expression* of an *element_access* is a value of an *array_type*, the *element_access* is an array access ([§11.7.10.2](expressions.md#117102-array-access)). Otherwise, the *primary_no_array_creation_expression* shall be a variable or value of a class, struct, or interface type that has one or more indexer members, in which case the *element_access* is an indexer access ([§11.7.10.3](expressions.md#117103-indexer-access)).

#### 11.7.10.2 Array access

For an array access, the *primary_no_array_creation_expression* of the *element_access* shall be a value of an *array_type*. Furthermore, the *argument_list* of an array access is not allowed to contain named arguments. The number of expressions in the *argument_list* shall be the same as the rank of the *array_type*, and each expression shall be of type `int`, `uint`, `long`, or `ulong,` or shall be implicitly convertible to one or more of these types.

The result of evaluating an array access is a variable of the element type of the array, namely the array element selected by the value(s) of the expression(s) in the *argument_list*.

The run-time processing of an array access of the form `P[A]`, where `P` is a *primary_no_array_creation_expression* of an *array_type* and `A` is an *argument_list*, consists of the following steps:

- `P` is evaluated. If this evaluation causes an exception, no further steps are executed.
- The index expressions of the *argument_list* are evaluated in order, from left to right. Following evaluation of each index expression, an implicit conversion ([§10.2](conversions.md#102-implicit-conversions)) to one of the following types is performed: `int`, `uint`, `long`, `ulong`. The first type in this list for which an implicit conversion exists is chosen. For instance, if the index expression is of type `short` then an implicit conversion to `int` is performed, since implicit conversions from `short` to `int` and from `short` to `long` are possible. If evaluation of an index expression or the subsequent implicit conversion causes an exception, then no further index expressions are evaluated and no further steps are executed.
- The value of `P` is checked to be valid. If the value of `P` is `null`, a `System.NullReferenceException` is thrown and no further steps are executed.
- The value of each expression in the *argument_list* is checked against the actual bounds of each dimension of the array instance referenced by `P`. If one or more values are out of range, a `System.IndexOutOfRangeException` is thrown and no further steps are executed.
- The location of the array element given by the index expression(s) is computed, and this location becomes the result of the array access.

#### 11.7.10.3 Indexer access

For an indexer access, the *primary_no_array_creation_expression* of the *element_access* shall be a variable or value of a class, struct, or interface type, and this type shall implement one or more indexers that are applicable with respect to the *argument_list* of the *element_access*.

The binding-time processing of an indexer access of the form `P[A]`, where `P` is a *primary_no_array_creation_expression* of a class, struct, or interface type `T`, and `A` is an *argument_list*, consists of the following steps:

- The set of indexers provided by `T` is constructed. The set consists of all indexers declared in `T` or a base type of `T` that are not override declarations and are accessible in the current context ([§7.5](basic-concepts.md#75-member-access)).
- The set is reduced to those indexers that are applicable and not hidden by other indexers. The following rules are applied to each indexer `S.I` in the set, where `S` is the type in which the indexer `I` is declared:
  - If `I` is not applicable with respect to `A` ([§11.6.4.2](expressions.md#11642-applicable-function-member)), then `I` is removed from the set.
  - If `I` is applicable with respect to `A` ([§11.6.4.2](expressions.md#11642-applicable-function-member)), then all indexers declared in a base type of `S` are removed from the set.
  - If `I` is applicable with respect to `A` ([§11.6.4.2](expressions.md#11642-applicable-function-member)) and `S` is a class type other than `object`, all indexers declared in an interface are removed from the set.
- If the resulting set of candidate indexers is empty, then no applicable indexers exist, and a binding-time error occurs.
- The best indexer of the set of candidate indexers is identified using the overload resolution rules of [§11.6.4](expressions.md#1164-overload-resolution). If a single best indexer cannot be identified, the indexer access is ambiguous, and a binding-time error occurs.
- The index expressions of the *argument_list* are evaluated in order, from left to right. The result of processing the indexer access is an expression classified as an indexer access. The indexer access expression references the indexer determined in the step above, and has an associated instance expression of `P` and an associated argument list of `A`, and an associated type that is the type of the indexer. If `T` is a class type, the associated type is picked from the first declaration or override of the indexer found when starting with `T` and searching through its base classes.

Depending on the context in which it is used, an indexer access causes invocation of either the *get_accessor* or the *set_accessor* of the indexer. If the indexer access is the target of an assignment, the *set_accessor* is invoked to assign a new value ([§11.18.2](expressions.md#11182-simple-assignment)). In all other cases, the *get_accessor* is invoked to obtain the current value ([§11.2.2](expressions.md#1122-values-of-expressions)).

### 11.7.11 Null Conditional Element Access

A *null_conditional_element_access* consists of a *primary_no_array_creation_expression* followed by the two tokens “`?`” and “`[`”, followed by an *argument_list*, followed by a “`]`” token, followed by zero or more *dependent_access*es.

```ANTLR
null_conditional_element_access
    : primary_no_array_creation_expression '?' '[' argument_list ']'
      dependent_access*
    ;
```

A *null_conditional_element_access* is a conditional version of *element_access* ([§11.7.10](expressions.md#11710-element-access)) and it is a binding time error if the result type is `void`. For a null conditional expression where the result type may be `void` see ([§11.7.9](expressions.md#1179-null-conditional-invocation-expression)).

A *null_conditional_element_access* expression `E` is of the form `P?[A]B`; where `B` are the *dependent_access*es, if any. Let `T` be the type of the expression `P[A]B`.  The meaning of `E` is determined as follows:

- If `T` is a type parameter that is not known to be a reference type or a non-nullable value type, a compile-time error occurs.
- If `T` is a non-nullable value type, then the type of `E` is `T?`, and the meaning of `E` is the same as the meaning of:

  ```csharp
  ((object)P == null) ? (T?)null : P[A]B
  ```

  Except that `P` is evaluated only once.
- Otherwise the type of `E` is `T`, and the meaning of `E` is the same as the meaning of:

  ```csharp
  ((object)P == null) ? null : P[A]B
  ```

  Except that `P` is evaluated only once.

> *Note*: In an expression of the form:
>
> ```csharp
> P?[A₀]?[A₁]
> ```
>
> if `P` evaluates to `null` neither `A₀` or `A₁` are evaluated. The same is true if an expression is a sequence of *null_conditional_element_access* or *null_conditional_member_access* [§11.7.7](expressions.md#1177-null-conditional-member-access) operations.
>
> *end note*

### 11.7.12 This access

A *this_access* consists of the keyword `this`.

```ANTLR
this_access
    : 'this'
    ;
```

A *this_access* is permitted only in the *block* of an instance constructor, an instance method, an instance accessor ([§11.2.1](expressions.md#1121-general)), or a finalizer. It has one of the following meanings:

- When `this` is used in a *primary_expression* within an instance constructor of a class, it is classified as a value. The type of the value is the instance type ([§14.3.2](classes.md#1432-the-instance-type)) of the class within which the usage occurs, and the value is a reference to the object being constructed.
- When `this` is used in a *primary_expression* within an instance method or instance accessor of a class, it is classified as a value. The type of the value is the instance type ([§14.3.2](classes.md#1432-the-instance-type)) of the class within which the usage occurs, and the value is a reference to the object for which the method or accessor was invoked.
- When `this` is used in a *primary_expression* within an instance constructor of a struct, it is classified as a variable. The type of the variable is the instance type ([§14.3.2](classes.md#1432-the-instance-type)) of the struct within which the usage occurs, and the variable represents the struct being constructed.
  - If the constructor declaration has no constructor initializer, the `this` variable behaves exactly the same as an `out` parameter of the struct type. In particular, this means that the variable shall be definitely assigned in every execution path of the instance constructor.
  - Otherwise, the `this` variable behaves exactly the same as a `ref` parameter of the struct type. In particular, this means that the variable is considered initially assigned.
- When `this` is used in a *primary_expression* within an instance method or instance accessor of a struct, it is classified as a variable. The type of the variable is the instance type ([§14.3.2](classes.md#1432-the-instance-type)) of the struct within which the usage occurs.
  - If the method or accessor is not an iterator ([§14.14](classes.md#1414-iterators)) or async function ([§14.15](classes.md#1415-async-functions)), the `this` variable represents the struct for which the method or accessor was invoked, and behaves exactly the same as a `ref` parameter of the struct type.
  - If the method or accessor is an iterator or async function, the `this` variable represents a *copy* of the struct for which the method or accessor was invoked, and behaves exactly the same as a *value* parameter of the struct type.

Use of `this` in a *primary_expression* in a context other than the ones listed above is a compile-time error. In particular, it is not possible to refer to `this` in a static method, a static property accessor, or in a *variable_initializer* of a field declaration.

### 11.7.13 Base access

A *base_access* consists of the keyword base followed by either a “`.`” token and an identifier and optional *type_argument_list* or an *argument_list* enclosed in square brackets:

```ANTLR
base_access
    : 'base' '.' identifier type_argument_list?
    | 'base' '[' argument_list ']'
    ;
```

A *base_access* is used to access base class members that are hidden by similarly named members in the current class or struct. A *base_access* is permitted only in the *block* of an instance constructor, an instance method, an instance accessor ([§11.2.1](expressions.md#1121-general)), or a finalizer. When `base.I` occurs in a class or struct, I shall denote a member of the base class of that class or struct. Likewise, when `base[E]` occurs in a class, an applicable indexer shall exist in the base class.

At binding-time, *base_access* expressions of the form `base.I` and `base[E]` are evaluated exactly as if they were written `((B)this).I` and `((B)this)[E]`, where `B` is the base class of the class or struct in which the construct occurs. Thus, `base.I` and `base[E]` correspond to `this.I` and `this[E]`, except `this` is viewed as an instance of the base class.

When a *base_access* references a virtual function member (a method, property, or indexer), the determination of which function member to invoke at run-time ([§11.6.6](expressions.md#1166-function-member-invocation)) is changed. The function member that is invoked is determined by finding the most derived implementation ([§14.6.4](classes.md#1464-virtual-methods)) of the function member with respect to `B` (instead of with respect to the run-time type of `this`, as would be usual in a non-base access). Thus, within an override of a virtual function member, a *base_access* can be used to invoke the inherited implementation of the function member. If the function member referenced by a *base_access* is abstract, a binding-time error occurs.

> *Note*: Unlike `this`, `base` is not an expression in itself. It is a keyword only used in the context of a *base_access* or a *constructor_initializer* ([§14.11.2](classes.md#14112-constructor-initializers)). *end note*

### 11.7.14 Postfix increment and decrement operators

```ANTLR
post_increment_expression
    : primary_expression '++'
    ;

post_decrement_expression
    : primary_expression '--'
    ;
```

The operand of a postfix increment or decrement operation shall be an expression classified as a variable, a property access, or an indexer access. The result of the operation is a value of the same type as the operand.

If the *primary_expression* has the compile-time type `dynamic` then the operator is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)), the *post_increment_expression* or *post_decrement_expression* has the compile-time type `dynamic` and the following rules are applied at run-time using the run-time type of the *primary_expression*.

If the operand of a postfix increment or decrement operation is a property or indexer access, the property or indexer shall have both a get and a set accessor. If this is not the case, a binding-time error occurs.

Unary operator overload resolution ([§11.4.4](expressions.md#1144-unary-operator-overload-resolution)) is applied to select a specific operator implementation. Predefined `++` and `--` operators exist for the following types: `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `float`, `double`, `decimal`, and any enum type. The predefined `++` operators return the value produced by adding `1` to the operand, and the predefined `--` operators return the value produced by subtracting `1` from the operand. In a checked context, if the result of this addition or subtraction is outside the range of the result type and the result type is an integral type or enum type, a `System.OverflowException` is thrown.

There shall be an implicit conversion from the return type of the selected unary operator to the type of the *primary_expression*, otherwise a compile-time error occurs.

The run-time processing of a postfix increment or decrement operation of the form `x++` or `x--` consists of the following steps:

- If `x` is classified as a variable:
  - `x` is evaluated to produce the variable.
  - The value of `x` is saved.
  - The saved value of `x` is converted to the operand type of the selected operator and the operator is invoked with this value as its argument.
  - The value returned by the operator is converted to the type of `X` and stored in the location given by the earlier evaluation of `x`.
  - The saved value of `x` becomes the result of the operation.
- If `x` is classified as a property or indexer access:
  - The instance expression (if `x` is not `static`) and the argument list (if `x` is an indexer access) associated with `x` are evaluated, and the results are used in the subsequent get and set accessor invocations.
  - The get accessor of `x` is invoked and the returned value is saved.
  - The saved value of `x` is converted to the operand type of the selected operator and the operator is invoked with this value as its argument.
  - The value returned by the operator is converted to the type of `x` and the set accessor of `x` is invoked with this value as its value argument.
  - The saved value of `x` becomes the result of the operation.

The `++` and `--` operators also support prefix notation ([§11.8.6](expressions.md#1186-prefix-increment-and-decrement-operators)). Typically, the result of `x++` or `x--` is the value of `X` *before* the operation, whereas the result of `++x` or `--x` is the value of `X` *after* the operation. In either case, `x` itself has the same value after the operation.

An operator `++` or operator `--` implementation can be invoked using either postfix or prefix notation. It is not possible to have separate operator implementations for the two notations.

### 11.7.15 The new operator

#### 11.7.15.1 General

The `new` operator is used to create new instances of types.

There are three forms of new expressions:

- Object creation expressions and anonymous object creation expressions are used to create new instances of class types and value types.
- Array creation expressions are used to create new instances of array types.
- Delegate creation expressions are used to obtain instances of delegate types.

The `new` operator implies creation of an instance of a type, but does not necessarily imply allocation of memory. In particular, instances of value types require no additional memory beyond the variables in which they reside, and no allocations occur when `new` is used to create instances of value types.

> *Note*: Delegate creation expressions do not always create new instances. When the expression is processed in the same way as a method group conversion ([§10.8](conversions.md#108-method-group-conversions)) or an anonymous function conversion ([§10.7](conversions.md#107-anonymous-function-conversions)) this may result in an existing delegate instance being reused. *end note*

#### 11.7.15.2 Object creation expressions

An *object_creation_expression* is used to create a new instance of a *class_type* or a *value_type*.

```ANTLR
object_creation_expression
    : 'new' type '(' argument_list? ')' object_or_collection_initializer?
    | 'new' type object_or_collection_initializer
    ;

object_or_collection_initializer
    : object_initializer
    | collection_initializer
    ;
```

The *type* of an *object_creation_expression* shall be a *class_type*, a *value_type*, or a *type_parameter*. The *type* cannot be an abstract or static *class_type*.

The optional *argument_list* ([§11.6.2](expressions.md#1162-argument-lists)) is permitted only if the *type* is a *class_type* or a *struct_type*.

An object creation expression can omit the constructor argument list and enclosing parentheses provided it includes an object initializer or collection initializer. Omitting the constructor argument list and enclosing parentheses is equivalent to specifying an empty argument list.

Processing of an object creation expression that includes an object initializer or collection initializer consists of first processing the instance constructor and then processing the member or element initializations specified by the object initializer ([§11.7.15.3](expressions.md#117153-object-initializers)) or collection initializer ([§11.7.15.4](expressions.md#117154-collection-initializers)).

If any of the arguments in the optional *argument_list* has the compile-time type `dynamic` then the *object_creation_expression* is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)) and the following rules are applied at run-time using the run-time type of those arguments of the *argument_list* that have the compile-time type `dynamic`. However, the object creation undergoes a limited compile-time check as described in [§11.6.5](expressions.md#1165-compile-time-checking-of-dynamic-member-invocation).

The binding-time processing of an *object_creation_expression* of the form new `T(A)`, where `T` is a *class_type*, or a *value_type*, and `A` is an optional *argument_list*, consists of the following steps:

- If `T` is a *value_type* and `A` is not present:
  - The *object_creation_expression* is a default constructor invocation. The result of the *object_creation_expression* is a value of type `T`, namely the default value for `T` as defined in [§8.3.3](types.md#833-default-constructors).
- Otherwise, if `T` is a *type_parameter* and `A` is not present:
  - If no value type constraint or constructor constraint ([§14.2.5](classes.md#1425-type-parameter-constraints)) has been specified for `T`, a binding-time error occurs.
  - The result of the *object_creation_expression* is a value of the run-time type that the type parameter has been bound to, namely the result of invoking the default constructor of that type. The run-time type may be a reference type or a value type.
- Otherwise, if `T` is a *class_type* or a *struct_type*:
  - If `T` is an abstract or static *class_type*, a compile-time error occurs.
  - The instance constructor to invoke is determined using the overload resolution rules of [§11.6.4](expressions.md#1164-overload-resolution). The set of candidate instance constructors consists of all accessible instance constructors declared in `T`, which are applicable with respect to A ([§11.6.4.2](expressions.md#11642-applicable-function-member)). If the set of candidate instance constructors is empty, or if a single best instance constructor cannot be identified, a binding-time error occurs.
  - The result of the *object_creation_expression* is a value of type `T`, namely the value produced by invoking the instance constructor determined in the step above.
  - Otherwise, the *object_creation_expression* is invalid, and a binding-time error occurs.

Even if the *object_creation_expression* is dynamically bound, the compile-time type is still `T`.

The run-time processing of an *object_creation_expression* of the form new `T(A)`, where `T` is *class_type* or a *struct_type* and `A` is an optional *argument_list*, consists of the following steps:

- If `T` is a *class_type*:
  - A new instance of class `T` is allocated. If there is not enough memory available to allocate the new instance, a `System.OutOfMemoryException` is thrown and no further steps are executed.
  - All fields of the new instance are initialized to their default values ([§9.3](variables.md#93-default-values)).
  - The instance constructor is invoked according to the rules of function member invocation ([§11.6.6](expressions.md#1166-function-member-invocation)). A reference to the newly allocated instance is automatically passed to the instance constructor and the instance can be accessed from within that constructor as this.
- If `T` is a *struct_type*:
  - An instance of type `T` is created by allocating a temporary local variable. Since an instance constructor of a *struct_type* is required to definitely assign a value to each field of the instance being created, no initialization of the temporary variable is necessary.
  - The instance constructor is invoked according to the rules of function member invocation ([§11.6.6](expressions.md#1166-function-member-invocation)). A reference to the newly allocated instance is automatically passed to the instance constructor and the instance can be accessed from within that constructor as this.

#### 11.7.15.3 Object initializers

An ***object initializer*** specifies values for zero or more fields, properties, or indexed elements of an object.

```ANTLR
object_initializer
    : '{' member_initializer_list? '}'
    | '{' member_initializer_list ',' '}'
    ;

member_initializer_list
    : member_initializer (',' member_initializer)*
    ;

member_initializer
    : initializer_target '=' initializer_value
    ;

initializer_target
    : identifier
    | '[' argument_list ']'
    ;

initializer_value
    : expression
    | object_or_collection_initializer
    ;
```

An object initializer consists of a sequence of member initializers, enclosed by `{` and `}` tokens and separated by commas. Each *member_initializer* shall designate a target for the initialization. An *identifier* shall name an accessible field or property of the object being initialized, whereas an *argument_list* enclosed in square brackets shall specify arguments for an accessible indexer on the object being initialized. It is an error for an object initializer to include more than one member initializer for the same field or property.

> *Note*: While an object initializer is not permitted to set the same field or property more than once, there are no such restrictions for indexers. An object initializer may contain multiple initializer targets referring to indexers, and may even use the same indexer arguments multiple times. *end note*

Each *initializer_target* is followed by an equals sign and either an expression, an object initializer or a collection initializer. It is not possible for expressions within the object initializer to refer to the newly created object it is initializing.

A member initializer that specifies an expression after the equals sign is processed in the same way as an assignment ([§11.18.2](expressions.md#11182-simple-assignment)) to the target.

A member initializer that specifies an object initializer after the equals sign is a ***nested object initializer***, i.e., an initialization of an embedded object. Instead of assigning a new value to the field or property, the assignments in the nested object initializer are treated as assignments to members of the field or property. Nested object initializers cannot be applied to properties with a value type, or to read-only fields with a value type.

A member initializer that specifies a collection initializer after the equals sign is an initialization of an embedded collection. Instead of assigning a new collection to the target field, property, or indexer, the elements given in the initializer are added to the collection referenced by the target. The target shall be of a collection type that satisfies the requirements specified in [§11.7.15.4](expressions.md#117154-collection-initializers).

When an initializer target refers to an indexer, the arguments to the indexer shall always be evaluated exactly once. Thus, even if the arguments end up never getting used (e.g., because of an empty nested initializer), they are evaluated for their side effects.

> *Example*: The following class represents a point with two coordinates:
>
> ```csharp
> public class Point
> {
>     public int X { get; set; }
>     public int Y { get; set; }
> }
> ```
>
> An instance of `Point` can be created and initialized as follows:
>
> ```csharp
> Point a = new Point { X = 0, Y = 1 };
> ```
>
> which has the same effect as
>
> ```csharp
> Point __a = new Point();
> __a.X = 0;
> __a.Y = 1;
> Point a = __a;
> ```
>
> where `__a` is an otherwise invisible and inaccessible temporary variable. The following class represents a rectangle created from two points:
>
> ```csharp
> public class Rectangle
> {
>     public Point P1 { get; set; }
>     public Point P2 { get; set; }
> }
> ```
>
> An instance of `Rectangle` can be created and initialized as follows:
>
> ```csharp
> Rectangle r = new Rectangle
> {
>     P1 = new Point { X = 0, Y = 1 },
>     P2 = new Point { X = 2, Y = 3 }
> };
> ```
>
> which has the same effect as
>
> ```csharp
> Rectangle __r = new Rectangle();
> Point __p1 = new Point();
> __p1.X = 0;
> __p1.Y = 1;
> __r.P1 = __p1;
> Point __p2 = new Point();
> __p2.X = 2;
> __p2.Y = 3;
> __r.P2 = __p2;
> Rectangle r = __r;
> ```
>
> where `__r`, `__p1` and `__p2` are temporary variables that are otherwise invisible and inaccessible.
>
> If `Rectangle`’s constructor allocates the two embedded `Point` instances
>
> ```csharp
> public class Rectangle
> {
>     public Point P1 { get; } = new Point();
>     public Point P2 { get; } = new Point();
> }
> ```
>
> the following construct can be used to initialize the embedded `Point` instances instead of assigning new instances:
>
> ```csharp
> Rectangle r = new Rectangle
> {
>     P1 = { X = 0, Y = 1 },
>     P2 = { X = 2, Y = 3 }
> };
> ```
>
> which has the same effect as
>
> ```csharp
> Rectangle __r = new Rectangle();
> __r.P1.X = 0;
> __r.P1.Y = 1;
> __r.P2.X = 2;
> __r.P2.Y = 3;
> Rectangle r = __r;
> ```
>
> *end example*

#### 11.7.15.4 Collection initializers

A collection initializer specifies the elements of a collection.

```ANTLR
collection_initializer
    : '{' element_initializer_list '}'
    | '{' element_initializer_list ',' '}'
    ;

element_initializer_list
    : element_initializer (',' element_initializer)*
    ;

element_initializer
    : non_assignment_expression
    | '{' expression_list '}'
    ;

expression_list
    : expression
    | expression_list ',' expression
    ;
```

A collection initializer consists of a sequence of element initializers, enclosed by `{` and `}` tokens and separated by commas. Each element initializer specifies an element to be added to the collection object being initialized, and consists of a list of expressions enclosed by `{` and `}` tokens and separated by commas. A single-expression element initializer can be written without braces, but cannot then be an assignment expression, to avoid ambiguity with member initializers. The *non_assignment_expression* production is defined in [§11.19](expressions.md#1119-expression).

> *Example*:
> The following is an example of an object creation expression that includes a collection initializer:
>
> ```csharp
> List<int> digits = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
> ```
>
> *end example*

The collection object to which a collection initializer is applied shall be of a type that implements `System.Collections.IEnumerable` or a compile-time error occurs. For each specified element in order, normal member lookup is applied to find a member named `Add`. If the result of the member lookup is not a method group, a compile-time error occurs. Otherwise, overload resolution is applied with the expression list of the element initializer as the argument list, and the collection initializer invokes the resulting method. Thus, the collection object shall contain an applicable instance or extension method with the name `Add` for each element initializer.

> *Example*:The following class represents a contact with a name and a list of phone numbers:
>
> ```csharp
> public class Contact
> {
>     public string Name { get; set; }
>     public List<string> PhoneNumbers { get; } = new List<string>();
> }
> ```
>
> A `List<Contact>` can be created and initialized as follows:
>
> ```csharp
> var contacts = new List<Contact>
> {
>     new Contact
>     {
>         Name = "Chris Smith",
>         PhoneNumbers = { "206-555-0101", "425-882-8080" }
>     },
>     new Contact
>     {
>         Name = "Bob Harris",
>         PhoneNumbers = { "650-555-0199" }
>     }
> };
> ```
>
> which has the same effect as
>
> ```csharp
> var __clist = new List<Contact>();
> Contact __c1 = new Contact();
> __c1.Name = "Chris Smith";
> __c1.PhoneNumbers.Add("206-555-0101");
> __c1.PhoneNumbers.Add("425-882-8080");
> __clist.Add(__c1);
> Contact __c2 = new Contact();
> __c2.Name = "Bob Harris";
> __c2.PhoneNumbers.Add("650-555-0199");
> __clist.Add(__c2);
> var contacts = __clist;
> ```
>
> where `__clist`, `__c1` and `__c2` are temporary variables that are otherwise invisible and inaccessible.
>
> *end example*

#### 11.7.15.5 Array creation expressions

An *array_creation_expression* is used to create a new instance of an *array_type*.

```ANTLR
array_creation_expression
    : 'new' non_array_type '[' expression_list ']' rank_specifier*
      array_initializer?
    | 'new' array_type array_initializer
    | 'new' rank_specifier array_initializer
    ;
```

An array creation expression of the first form allocates an array instance of the type that results from deleting each of the individual expressions from the expression list.

> *Example*: The array creation expression `new int[10,20]` produces an array instance of type `int[,]`, and the array creation expression new `int[10][,]` produces an array instance of type `int[][,]`. *end example*

Each expression in the expression list shall be of type `int`, `uint`, `long`, or `ulong`, or implicitly convertible to one or more of these types. The value of each expression determines the length of the corresponding dimension in the newly allocated array instance. Since the length of an array dimension shall be nonnegative, it is a compile-time error to have a constant expression with a negative value, in the expression list.

Except in an unsafe context ([§22.2](unsafe-code.md#222-unsafe-contexts)), the layout of arrays is unspecified.

If an array creation expression of the first form includes an array initializer, each expression in the expression list shall be a constant and the rank and dimension lengths specified by the expression list shall match those of the array initializer.

In an array creation expression of the second or third form, the rank of the specified array type or rank specifier shall match that of the array initializer. The individual dimension lengths are inferred from the number of elements in each of the corresponding nesting levels of the array initializer. Thus, the expression

```csharp
new int[,] {{0, 1}, {2, 3}, {4, 5}}
```

exactly corresponds to

```csharp
new int[3, 2] {{0, 1}, {2, 3}, {4, 5}}
```

An array creation expression of the third form is referred to as an ***implicitly typed array-creation expression***. It is similar to the second form, except that the element type of the array is not explicitly given, but determined as the best common type ([§11.6.3.15](expressions.md#116315-finding-the-best-common-type-of-a-set-of-expressions)) of the set of expressions in the array initializer. For a multidimensional array, i.e., one where the *rank_specifier* contains at least one comma, this set comprises all *expression*s found in nested *array_initializer*s.

Array initializers are described further in [§16.7](arrays.md#167-array-initializers).

The result of evaluating an array creation expression is classified as a value, namely a reference to the newly allocated array instance. The run-time processing of an array creation expression consists of the following steps:

- The dimension length expressions of the *expression_list* are evaluated in order, from left to right. Following evaluation of each expression, an implicit conversion ([§10.2](conversions.md#102-implicit-conversions)) to one of the following types is performed: `int`, `uint`, `long`, `ulong`. The first type in this list for which an implicit conversion exists is chosen. If evaluation of an expression or the subsequent implicit conversion causes an exception, then no further expressions are evaluated and no further steps are executed.
- The computed values for the dimension lengths are validated, as follows: If one or more of the values are less than zero, a `System.OverflowException` is thrown and no further steps are executed.
- An array instance with the given dimension lengths is allocated. If there is not enough memory available to allocate the new instance, a `System.OutOfMemoryException` is thrown and no further steps are executed.
- All elements of the new array instance are initialized to their default values ([§9.3](variables.md#93-default-values)).
- If the array creation expression contains an array initializer, then each expression in the array initializer is evaluated and assigned to its corresponding array element. The evaluations and assignments are performed in the order the expressions are written in the array initializer—in other words, elements are initialized in increasing index order, with the rightmost dimension increasing first. If evaluation of a given expression or the subsequent assignment to the corresponding array element causes an exception, then no further elements are initialized (and the remaining elements will thus have their default values).

An array creation expression permits instantiation of an array with elements of an array type, but the elements of such an array shall be manually initialized.

> *Example*: The statement
>
> ```csharp
> int[][] a = new int[100][];
> ```
>
> creates a single-dimensional array with 100 elements of type `int[]`. The initial value of each element is `null`. It is not possible for the same array creation expression to also instantiate the sub-arrays, and the statement
>
> ```csharp
> int[][] a = new int[100][5]; // Error
> ```
>
> results in a compile-time error. Instantiation of the sub-arrays can instead be performed manually, as in
>
> ```csharp
> int[][] a = new int[100][];
> for (int i = 0; i < 100; i++)
> {
>     a[i] = new int[5];
> }
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*:  When an array of arrays has a “rectangular” shape, that is when the sub-arrays are all of the same length, it is more efficient to use a multi-dimensional array. In the example above, instantiation of the array of arrays creates 101 objects—one outer array and 100 sub-arrays. In contrast,
>
> ```csharp
> int[,] = new int[100, 5];
> ```
>
> creates only a single object, a two-dimensional array, and accomplishes the allocation in a single statement.
>
> *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The following are examples of implicitly typed array creation expressions:
>
> ```csharp
> var a = new[] { 1, 10, 100, 1000 };                     // int[]
> var b = new[] { 1, 1.5, 2, 2.5 };                       // double[]
> var c = new[,] { { "hello", null }, { "world", "!" } }; // string[,]
> var d = new[] { 1, "one", 2, "two" };                   // Error
> ```
>
> The last expression causes a compile-time error because neither `int` nor `string` is implicitly convertible to the other, and so there is no best common type. An explicitly typed array creation expression must be used in this case, for example specifying the type to be `object[]`. Alternatively, one of the elements can be cast to a common base type, which would then become the inferred element type.
>
> *end example*

Implicitly typed array creation expressions can be combined with anonymous object initializers ([§11.7.15.7](expressions.md#117157-anonymous-object-creation-expressions)) to create anonymously typed data structures.

> *Example*:
>
> ```csharp
> var contacts = new[]
> {
>     new
>     {
>         Name = "Chris Smith",
>         PhoneNumbers = new[] { "206-555-0101", "425-882-8080" }
>     },
>     new 
>     {
>         Name = "Bob Harris",
>        PhoneNumbers = new[] { "650-555-0199" }
>     }
> };
> ```
>
> *end example*

#### 11.7.15.6 Delegate creation expressions

A *delegate_creation_expression* is used to obtain an instance of a *delegate_type*.

```ANTLR
delegate_creation_expression
    : 'new' delegate_type '(' expression ')'
    ;
```

The argument of a delegate creation expression shall be a method group, an anonymous function, or a value of either the compile-time type `dynamic` or a *delegate_type*. If the argument is a method group, it identifies the method and, for an instance method, the object for which to create a delegate. If the argument is an anonymous function it directly defines the parameters and method body of the delegate target. If the argument is a value it identifies a delegate instance of which to create a copy.

If the *expression* has the compile-time type `dynamic`, the *delegate_creation_expression* is dynamically bound ([§11.7.15.6](expressions.md#117156-delegate-creation-expressions)), and the rules below are applied at run-time using the run-time type of the *expression*. Otherwise, the rules are applied at compile-time.

The binding-time processing of a *delegate_creation_expression* of the form new `D(E)`, where `D` is a *delegate_type* and `E` is an *expression*, consists of the following steps:

- If `E` is a method group, the delegate creation expression is processed in the same way as a method group conversion ([§10.8](conversions.md#108-method-group-conversions)) from `E` to `D`.

- If `E` is an anonymous function, the delegate creation expression is processed in the same way as an anonymous function conversion ([§10.7](conversions.md#107-anonymous-function-conversions)) from `E` to `D`.
- If `E` is a value, `E` shall be compatible ([§19.2](delegates.md#192-delegate-declarations)) with `D`, and the result is a reference to a newly created delegate with a single-entry invocation list that invokes  `E`.

The run-time processing of a *delegate_creation_expression* of the form new `D(E)`, where `D` is a *delegate_type* and `E` is an *expression*, consists of the following steps:

- If `E` is a method group, the delegate creation expression is evaluated as a method group conversion ([§10.8](conversions.md#108-method-group-conversions)) from `E` to `D`.
- If `E` is an anonymous function, the delegate creation is evaluated as an anonymous function conversion from `E` to `D` ([§10.7](conversions.md#107-anonymous-function-conversions)).
- If `E` is a value of a *delegate_type*:
  - `E` is evaluated. If this evaluation causes an exception, no further steps are executed.
  - If the value of `E` is `null`, a `System.NullReferenceException` is thrown and no further steps are executed.
  - A new instance of the delegate type `D` is allocated. If there is not enough memory available to allocate the new instance, a `System.OutOfMemoryException` is thrown and no further steps are executed.
  - The new delegate instance is initialized with a single-entry invocation list that invokes `E`.

The invocation list of a delegate is determined when the delegate is instantiated and then remains constant for the entire lifetime of the delegate. In other words, it is not possible to change the target callable entities of a delegate once it has been created.

> *Note*: Remember, when two delegates are combined or one is removed from another, a new delegate results; no existing delegate has its content changed. *end note*

It is not possible to create a delegate that refers to a property, indexer, user-defined operator, instance constructor, finalizer, or static constructor.

> *Example*: As described above, when a delegate is created from a method group, the formal parameter list and return type of the delegate determine which of the overloaded methods to select. In the example
>
> ```csharp
> delegate double DoubleFunc(double x);
>
> class A
> {
>     DoubleFunc f = new DoubleFunc(Square);
>
>     static float Square(float x) => x * x;
>     static double Square(double x) => x * x;
> }
> ```
>
> the `A.f` field is initialized with a delegate that refers to the second `Square` method because that method exactly matches the formal parameter list and return type of `DoubleFunc`. Had the second `Square` method not been present, a compile-time error would have occurred.
>
> *end example*

#### 11.7.15.7 Anonymous object creation expressions

An *anonymous_object_creation_expression* is used to create an object of an anonymous type.

```ANTLR
anonymous_object_creation_expression
    : 'new' anonymous_object_initializer
    ;

anonymous_object_initializer
    : '{' member_declarator_list? '}'
    | '{' member_declarator_list ',' '}'
    ;

member_declarator_list
    : member_declarator (',' member_declarator)*
    ;

member_declarator
    : simple_name
    | member_access
    | null_conditional_projection_initializer
    | base_access
    | identifier '=' expression
    ;
```

An anonymous object initializer declares an anonymous type and returns an instance of that type. An anonymous type is a nameless class type that inherits directly from `object`. The members of an anonymous type are a sequence of read-only properties inferred from the anonymous object initializer used to create an instance of the type. Specifically, an anonymous object initializer of the form

`new {` *p₁* `=` *e₁* `,` *p₂* `=` *e₂* `,` … *pᵥ* `=` *eᵥ* `}`

declares an anonymous type of the form

```csharp
class __Anonymous1
{
    private readonly «T1» «f1»;
    private readonly «T2» «f2»;
    ...
    private readonly «Tn» «fn»;

    public __Anonymous1(«T1» «a1», «T2» «a2»,..., «Tn» «an»)
    {
        «f1» = «a1»;
        «f2» = «a2»;
        ...
        «fn» = «an»;
    }

    public «T1» «p1» { get { return «f1»; } }
    public «T2» «p2» { get { return «f2»; } }
    ...
    public «Tn» «pn» { get { return «fn»; } }
    public override bool Equals(object __o) { ... }
    public override int GetHashCode() { ... }
}
```

where each «Tx» is the type of the corresponding expression «ex». The expression used in a *member_declarator* shall have a type. Thus, it is a compile-time error for an expression in a *member_declarator* to be `null` or an anonymous function. It is also a compile-time error for the expression to have a pointer type ([§22.3](unsafe-code.md#223-pointer-types)).

The names of an anonymous type and of the parameter to its `Equals` method are automatically generated by the compiler and cannot be referenced in program text.

Within the same program, two anonymous object initializers that specify a sequence of properties of the same names and compile-time types in the same order will produce instances of the same anonymous type.

> *Example*: In the example
>
> ```csharp
> var p1 = new { Name = "Lawnmower", Price = 495.00 };
> var p2 = new { Name = "Shovel", Price = 26.95 };
> p1 = p2;
> ```
>
> the assignment on the last line is permitted because `p1` and `p2` are of the same anonymous type.
>
> *end example*

The `Equals` and `GetHashcode` methods on anonymous types override the methods inherited from `object`, and are defined in terms of the `Equals` and `GetHashcode` of the properties, so that two instances of the same anonymous type are equal if and only if all their properties are equal.

A member declarator can be abbreviated to a simple name ([§11.7.4](expressions.md#1174-simple-names)), a member access ([§11.7.6](expressions.md#1176-member-access)), a null conditional projection initializer [§11.7.7](expressions.md#1177-null-conditional-member-access) or a base access ([§11.7.13](expressions.md#11713-base-access)). This is called a ***projection initializer*** and is shorthand for a declaration of and assignment to a property with the same name. Specifically, member declarators of the forms

`«identifier»`, `«expr» . «identifier»` and `«expr» ? . «identifier»`

are precisely equivalent to the following, respectively:

`«identifer» = «identifier»`, `«identifier» = «expr» . «identifier»` and `«identifier» = «expr» ? . «identifier»`

Thus, in a projection initializer the identifier selects both the value and the field or property to which the value is assigned. Intuitively, a projection initializer projects not just a value, but also the name of the value.

### 11.7.16 The typeof operator

The `typeof` operator is used to obtain the `System.Type` object for a type.

```ANTLR
typeof_expression
    : 'typeof' '(' type ')'
    | 'typeof' '(' unbound_type_name ')'
    | 'typeof' '(' 'void' ')'
    ;

unbound_type_name
    : identifier generic_dimension_specifier?
    | identifier '::' identifier generic_dimension_specifier?
    | unbound_type_name '.' identifier generic_dimension_specifier?
    ;

generic_dimension_specifier
    : '<' comma* '>'
    ;

comma
    : ','
    ;

```

The first form of *typeof_expression* consists of a `typeof` keyword followed by a parenthesized type. The result of an expression of this form is the `System.Type` object for the indicated type. There is only one `System.Type` object for any given type. This means that for a type `T`, `typeof(T) == typeof(T)` is always true. The type cannot be `dynamic`.

The second form of *typeof_expression* consists of a `typeof` keyword followed by a parenthesized *unbound_type_name*.

> *Note*: An *unbound_type_name* is very similar to a *type_name* ([§7.8](basic-concepts.md#78-namespace-and-type-names)) except that an *unbound_type_name* contains *generic_dimension_specifier*s where a *type_name* contains *type_argument_list*s. *end note*

When the operand of a *typeof_expression* is a sequence of tokens that satisfies the grammars of both *unbound_type_name* and *type_name*, namely when it contains neither a *generic_dimension_specifier* nor a *type_argument_list*, the sequence of tokens is considered to be a *type_name*. The meaning of an *unbound_type_name* is determined as follows:

- Convert the sequence of tokens to a *type_name* by replacing each *generic_dimension_specifier* with a *type_argument_list* having the same number of commas and the keyword `object` as each *type_argument*.
- Evaluate the resulting *type_name*, while ignoring all type parameter constraints.
- The *unbound_type_name* resolves to the unbound generic type associated with the resulting constructed type ([§8.4](types.md#84-constructed-types)).

The result of the *typeof_expression* is the `System.Type` object for the resulting unbound generic type.

The third form of *typeof_expression* consists of a `typeof` keyword followed by a parenthesized `void` keyword. The result of an expression of this form is the `System.Type` object that represents the absence of a type. The type object returned by `typeof(void)` is distinct from the type object returned for any type.

> *Note*: This special type `object` is useful in class libraries that allow reflection onto methods in the language, where those methods wish to have a way to represent the return type of any method, including `void` methods, with an instance of `System.Type`. *end note*

The `typeof` operator can be used on a type parameter. The result is the `System.Type` object for the run-time type that was bound to the type parameter. The `typeof` operator can also be used on a constructed type or an unbound generic type ([§8.4.4](types.md#844-bound-and-unbound-types)). The `System.Type` object for an unbound generic type is not the same as the `System.Type` object of the instance type ([§14.3.2](classes.md#1432-the-instance-type)). The instance type is always a closed constructed type at run-time so its `System.Type` object depends on the run-time type arguments in use. The unbound generic type, on the other hand, has no type arguments, and yields the same `System.Type` object regardless of runtime type arguments.

> *Example*: The example
>
> ```csharp
> using System;
> class X<T>
> {
>     public static void PrintTypes()
>     {
>         Type[] t =
>         {
>             typeof(int),
>             typeof(System.Int32),
>             typeof(string),
>             typeof(double[]),
>             typeof(void),
>             typeof(T),
>             typeof(X<T>),
>             typeof(X<X<T>>),
>             typeof(X<>)
>         };
>         for (int i = 0; i < t.Length; i++)
>         {
>             Console.WriteLine(t[i]);
>         }
>     }
> }
>
> class Test
> {
>     static void Main()
>     {
>         X<int>.PrintTypes();
>     }
> }
> ```
>
> produces the following output:
>
> ```console
> System.Int32
> System.Int32
> System.String
> System.Double[]
> System.Void
> System.Int32
> X`1[System.Int32]
> X`1[X`1[System.Int32]]
> X`1[T]
> ```
>
> Note that `int` and `System.Int32` are the same type.
> The result of `typeof(X<>)` does not depend on the type argument but the result of `typeof(X<T>)` does.
>
> *end example*

### 11.7.17 The sizeof operator

The `sizeof` operator returns the number of 8-bit bytes occupied by a variable of a given type. The type specified as an operand to sizeof shall be an *unmanaged_type* ([§8.8](types.md#88-unmanaged-types)).

```ANTLR
sizeof_expression
   : 'sizeof' '(' unmanaged_type ')'
   ;
```

For certain predefined types the `sizeof` operator yields a constant `int` value as shown in the table below:

**Expression**     | **Result**
-----------------  | --------
`sizeof(sbyte)`    | 1
`sizeof(byte)`     | 1
`sizeof(short)`    | 2
`sizeof(ushort)`   | 2
`sizeof(int)`      | 4
`sizeof(uint)`     | 4
`sizeof(long)`     | 8
`sizeof(ulong)`    | 8
`sizeof(char)`     | 2
`sizeof(float)`    | 4
`sizeof(double)`   | 8
`sizeof(bool)`     | 1
`sizeof(decimal)`  | 16

For an enum type `T`, the result of the expression `sizeof(T)` is a constant value equal to the size of its underlying type, as given above. For all other operand types, the `sizeof` operator is specified in [§22.6.9](unsafe-code.md#2269-the-sizeof-operator).

### 11.7.18 The checked and unchecked operators

The `checked` and `unchecked` operators are used to control the overflow-checking context for integral-type arithmetic operations and conversions.

```ANTLR
checked_expression
    : 'checked' '(' expression ')'
    ;

unchecked_expression
    : 'unchecked' '(' expression ')'
    ;
```

The `checked` operator evaluates the contained expression in a checked context, and the `unchecked` operator evaluates the contained expression in an unchecked context. A *checked_expression* or *unchecked_expression* corresponds exactly to a *parenthesized_expression* ([§11.7.5](expressions.md#1175-parenthesized-expressions)), except that the contained expression is evaluated in the given overflow checking context.

The overflow checking context can also be controlled through the `checked` and `unchecked` statements ([§12.12](statements.md#1212-the-checked-and-unchecked-statements)).

The following operations are affected by the overflow checking context established by the checked and unchecked operators and statements:

- The predefined `++` and `--` operators ([§11.7.14](expressions.md#11714-postfix-increment-and-decrement-operators) and [§11.8.6](expressions.md#1186-prefix-increment-and-decrement-operators)), when the operand is of an integral or enum type.
- The predefined `-` unary operator ([§11.8.3](expressions.md#1183-unary-minus-operator)), when the operand is of an integral type.
- The predefined `+`, `-`, `\`, and `/` binary operators ([§11.9](expressions.md#119-arithmetic-operators)), when both operands are of integral or enum types.
- Explicit numeric conversions ([§10.3.2](conversions.md#1032-explicit-numeric-conversions)) from one integral or enumtype to another integral or enum type, or from `float` or `double` to an integral or enum type.

When one of the above operations produces a result that is too large to represent in the destination type, the context in which the operation is performed controls the resulting behavior:

- In a `checked` context, if the operation is a constant expression ([§11.20](expressions.md#1120-constant-expressions)), a compile-time error occurs. Otherwise, when the operation is performed at run-time, a `System.OverflowException` is thrown.
- In an `unchecked` context, the result is truncated by discarding any high-order bits that do not fit in the destination type.

For non-constant expressions ([§11.20](expressions.md#1120-constant-expressions)) (expressions that are evaluated at run-time) that are not enclosed by any `checked` or `unchecked` operators or statements, the default overflow checking context is unchecked, unless external factors (such as compiler switches and execution environment configuration) call for checked evaluation.

For constant expressions ([§11.20](expressions.md#1120-constant-expressions)) (expressions that can be fully evaluated at compile-time), the default overflow checking context is always checked. Unless a constant expression is explicitly placed in an `unchecked` context, overflows that occur during the compile-time evaluation of the expression always cause compile-time errors.

The body of an anonymous function is not affected by `checked` or `unchecked` contexts in which the anonymous function occurs.

> *Example*: In the following code
>
> ```csharp
> class Test
> {
>     static readonly int x = 1000000;
>     static readonly int y = 1000000;
>
>     static int F() => checked(x * y);    // Throws OverflowException
>     static int G() => unchecked(x * y);  // Returns -727379968
>     static int H() => x * y;             // Depends on default
> }
> ```
>
> no compile-time errors are reported since neither of the expressions can be evaluated at compile-time. At run-time, the `F` method throws a `System.OverflowException`, and the `G` method returns –727379968 (the lower 32 bits of the out-of-range result). The behavior of the `H` method depends on the default overflow-checking context for the compilation, but it is either the same as `F` or the same as `G`.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: In the following code
>
> ```csharp
> class Test
> {
>     const int x = 1000000;
>     const int y = 1000000;
>
>     static int F() => checked(x * y);    // Compile-time error, overflow
>     static int G() => unchecked(x * y);  // Returns -727379968
>     static int H() => x * y;             // Compile-time error, overflow
> }
> ```
>
> the overflows that occur when evaluating the constant expressions in `F` and `H` cause compile-time errors to be reported because the expressions are evaluated in a `checked` context. An overflow also occurs when evaluating the constant expression in `G`, but since the evaluation takes place in an `unchecked` context, the overflow is not reported.
>
> *end example*

The `checked` and `unchecked` operators only affect the overflow checking context for those operations that are textually contained within the “`(`” and “`)`” tokens. The operators have no effect on function members that are invoked as a result of evaluating the contained expression.

> *Example*: In the following code
>
> ```csharp
> class Test
> {
>     static int Multiply(int x, int y) => x * y;
>
>     static int F() => checked(Multiply(1000000, 1000000));
> }
> ```
>
> the use of `checked` in F does not affect the evaluation of `x * y` in `Multiply`, so `x * y` is evaluated in the default overflow checking context.
>
> *end example*

The `unchecked` operator is convenient when writing constants of the signed integral types in hexadecimal notation.

> *Example*:
>
> ```csharp
> class Test
> {
>     public const int AllBits = unchecked((int)0xFFFFFFFF);
>     public const int HighBit = unchecked((int)0x80000000);
> }
> ```
>
> Both of the hexadecimal constants above are of type `uint`. Because the constants are outside the `int` range, without the `unchecked` operator, the casts to `int` would produce compile-time errors.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The `checked` and `unchecked` operators and statements allow programmers to control certain aspects of some numeric calculations. However, the behavior of some numeric operators depends on their operands’ data types. For example, multiplying two decimals always results in an exception on overflow even within an explicitly unchecked construct. Similarly, multiplying two floats never results in an exception on overflow even within an explicitly checked construct. In addition, other operators are never affected by the mode of checking, whether default or explicit. *end note*

### 11.7.19 Default value expressions

A default value expression is used to obtain the default value ([§9.3](variables.md#93-default-values)) of a type. Typically a default value expression is used for type parameters, since it might not be known if the type parameter is a value type or a reference type. (No conversion exists from the `null` literal ([§6.4.5.7](lexical-structure.md#6457-the-null-literal)) to a type parameter unless the type parameter is known to be a reference type ([§8.2](types.md#82-reference-types)).)

```ANTLR
default_value_expression
    : 'default' '(' type ')'
    ;
```

If the *type* in a *default_value_expression* evaluates at run-time to a reference type, the result is `null` converted to that type. If the *type* in a *default_value_expression* evaluates at run-time to a value type, the result is the value type’s default value ([§8.3.3](types.md#833-default-constructors)).

A *default_value_expression* is a constant expression ([§11.20](expressions.md#1120-constant-expressions)) if *type* is a reference type or a type parameter that is known to be a reference type ([§8.2](types.md#82-reference-types)). In addition, a *default_value_expression* is a constant expression if the type is one of the following value types: `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `float`, `double`, `decimal`, `bool,` or any enumeration type.

### 11.7.20 Nameof expressions

A *nameof_expression* is used to obtain the name of a program entity as a constant string.

```ANTLR
nameof_expression
    : 'nameof' '(' named_entity ')'
    ;
    
named_entity
    : named_entity_target ('.' identifier type_argument_list?)*
    ;
    
named_entity_target
    : simple_name
    | 'this'
    | 'base'
    | predefined_type 
    | qualified_alias_member
    ;
```

Because `nameof` is not a keyword, a *nameof_expression* is always syntactically ambiguous with an invocation of the simple name `nameof`. For compatibility reasons, if a name lookup ([§11.7.4](expressions.md#1174-simple-names)) of the name `nameof` succeeds, the expression is treated as an *invocation_expression* — regardless of whether the invocation is valid. Otherwise it is a *nameof_expression*.

Simple name and member access lookups are performed on the *named_entity* at compile time, following the rules described in [§11.7.4](expressions.md#1174-simple-names) and [§11.7.6](expressions.md#1176-member-access). However, where the lookup described in [§11.7.4](expressions.md#1174-simple-names) and [§11.7.6](expressions.md#1176-member-access) results in an error because an instance member was found in a static context, a *nameof_expression* produces no such error.

It is a compile-time error for a *named_entity* designating a method group to have a *type_argument_list*. It is a compile time error for a *named_entity_target* to have the type `dynamic`.

A *nameof_expression* is a constant expression of type `string`, and has no effect at runtime. Specifically, its *named_entity* is not evaluated, and is ignored for the purposes of definite assignment analysis ([§9.4.4.22](variables.md#94422-general-rules-for-simple-expressions)). Its value is the last identifier of the *named_entity* before the optional final *type_argument_list*, transformed in the following way:

- The prefix “`@`”, if used, is removed.
- Each *unicode_escape_sequence* is transformed into its corresponding Unicode character.
- Any *formatting_characters* are removed.

These are the same transformations applied in [§6.4.3](lexical-structure.md#643-identifiers) when testing equality between identifiers.

> *Example*: The following illustrates the results of various `nameof` expressions, assuming a generic type `List<T>` declared within the `System.Collections.Generic` namespace:
>
> ```csharp
> using System.Collections.Generic;
> 
> using TestAlias = System.String;
> 
> class Program
> {
>     static void Main()
>     {
>         var point = (x: 3, y: 4);
> 
>         string n1 = nameof(System);                      // "System"
>         string n2 = nameof(System.Collections.Generic);  // "Generic"
>         string n3 = nameof(point);                       // "point"
>         string n4 = nameof(point.x);                     // "x"
>         string n5 = nameof(Program);                     // "Program"
>         string n6 = nameof(System.Int32);                // "Int32"
>         string n7 = nameof(TestAlias);                   // "TestAlias"
>         string n8 = nameof(List<int>);                   // "List"
>         string n9 = nameof(Program.InstanceMethod);      // "InstanceMethod"
>         string n10 = nameof(Program.GenericMethod);      // "GenericMethod"
>         string n11 = nameof(Program.NestedClass);        // "NestedClass"
> 
>         // Invalid
>         // string x1 = nameof(List<>);            // Empty type argument list
>         // string x2 = nameof(List<T>);           // T is not in scope
>         // string x3 = nameof(GenericMethod<>);   // Empty type argument list
>         // string x4 = nameof(GenericMethod<T>);  // T is not in scope
>         // string x5 = nameof(int);               // Keywords not permitted
>         // Type arguments not permitted for method group
>         // string x6 = nameof(GenericMethod<Program>);
>     }
> 
>     void InstanceMethod() { }
> 
>     void GenericMethod<T>()
>     {
>         string n1 = nameof(List<T>); // "List"
>         string n2 = nameof(T);       // "T"
>     }
> 
>     class NestedClass { }
> }
> ```
>
> Potentially surprising parts of this example are the resolution of `nameof(System.Collections.Generic)` to just “Generic” instead of the full namespace, and of `nameof(TestAlias)` to “TestAlias” rather than “String”.
> *end example*

### 11.7.21 Anonymous method expressions

An *anonymous_method_expression* is one of two ways of defining an anonymous function. These are further described in [§11.16](expressions.md#1116-anonymous-function-expressions).

## 11.8 Unary operators

### 11.8.1 General

The `+`, `-`, `!`, `~`, `++`, `--`, cast, and `await` operators are called the unary operators.

```ANTLR
unary_expression
    : primary_expression
    | '+' unary_expression
    | '-' unary_expression
    | '!' unary_expression
    | '~' unary_expression
    | pre_increment_expression
    | pre_decrement_expression
    | cast_expression
    | await_expression
    | pointer_indirection_expression    // unsafe code support
    | addressof_expression              // unsafe code support
    ;
```

*pointer_indirection_expression* ([§22.6.2](unsafe-code.md#2262-pointer-indirection)) and *addressof_expression* ([§22.6.5](unsafe-code.md#2265-the-address-of-operator)) are available only in unsafe code ([§22](unsafe-code.md#22-unsafe-code)).

If the operand of a *unary_expression* has the compile-time type `dynamic`, it is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)). In this case, the compile-time type of the *unary_expression* is `dynamic`, and the resolution described below will take place at run-time using the run-time type of the operand.

### 11.8.2 Unary plus operator

For an operation of the form `+x`, unary operator overload resolution ([§11.4.4](expressions.md#1144-unary-operator-overload-resolution)) is applied to select a specific operator implementation. The operand is converted to the parameter type of the selected operator, and the type of the result is the return type of the operator. The predefined unary plus operators are:

```csharp
int operator +(int x);
uint operator +(uint x);
long operator +(long x);
ulong operator +(ulong x);
float operator +(float x);
double operator +(double x);
decimal operator +(decimal x);
```

For each of these operators, the result is simply the value of the operand.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined unary plus operators defined above are also predefined.

### 11.8.3 Unary minus operator

For an operation of the form `–x`, unary operator overload resolution ([§11.4.4](expressions.md#1144-unary-operator-overload-resolution)) is applied to select a specific operator implementation. The operand is converted to the parameter type of the selected operator, and the type of the result is the return type of the operator. The predefined unary minus operators are:

- Integer negation:

  ```csharp
  int operator –(int x);
  long operator –(long x);
  ```  

  The result is computed by subtracting `X` from zero. If the value of `X` is the smallest representable value of the operand type (−2³¹ for `int` or −2⁶³ for `long`), then the mathematical negation of `X` is not representable within the operand type. If this occurs within a `checked` context, a `System.OverflowException` is thrown; if it occurs within an `unchecked` context, the result is the value of the operand and the overflow is not reported.
  
  If the operand of the negation operator is of type `uint`, it is converted to type `long`, and the type of the result is `long`. An exception is the rule that permits the `int` value `−2147483648` (−2³¹) to be written as a decimal integer literal ([§6.4.5.3](lexical-structure.md#6453-integer-literals)).
  
  If the operand of the negation operator is of type `ulong`, a compile-time error occurs. An exception is the rule that permits the `long` value `−9223372036854775808` (−2⁶³) to be written as a decimal integer literal ([§6.4.5.3](lexical-structure.md#6453-integer-literals))
- Floating-point negation:

  ```csharp
  float operator –(float x);
  double operator –(double x);
  ```
  
  The result is the value of `X` with its sign inverted. If `x` is `NaN`, the result is also `NaN`.
- Decimal negation:

  ```csharp
  decimal operator –(decimal x);
  ```
  
  The result is computed by subtracting `X` from zero. Decimal negation is equivalent to using the unary minus operator of type `System.Decimal`.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined unary minus operators defined above are also predefined.

### 11.8.4 Logical negation operator

For an operation of the form `!x`, unary operator overload resolution ([§11.4.4](expressions.md#1144-unary-operator-overload-resolution)) is applied to select a specific operator implementation. The operand is converted to the parameter type of the selected operator, and the type of the result is the return type of the operator. Only one predefined logical negation operator exists:

```csharp
bool operator !(bool x);
```

This operator computes the logical negation of the operand: If the operand is `true`, the result is `false`. If the operand is `false`, the result is `true`.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined logical negation operator defined above are also predefined.

### 11.8.5 Bitwise complement operator

For an operation of the form `~x`, unary operator overload resolution ([§11.4.4](expressions.md#1144-unary-operator-overload-resolution)) is applied to select a specific operator implementation. The operand is converted to the parameter type of the selected operator, and the type of the result is the return type of the operator. The predefined bitwise complement operators are:

```csharp
int operator ~(int x);
uint operator ~(uint x);
long operator ~(long x);
ulong operator ~(ulong x);
```

For each of these operators, the result of the operation is the bitwise complement of `x`.

Every enumeration type `E` implicitly provides the following bitwise complement operator:

```csharp
E operator ~(E x);
```

The result of evaluating `~x`, where `X` is an expression of an enumeration type `E` with an underlying type `U`, is exactly the same as evaluating `(E)(~(U)x)`, except that the conversion to `E` is always performed as if in an `unchecked` context ([§11.7.18](expressions.md#11718-the-checked-and-unchecked-operators)).

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined bitwise complement operators defined above are also predefined.

### 11.8.6 Prefix increment and decrement operators

```ANTLR
pre_increment_expression
    : '++' unary_expression
    ;

pre_decrement_expression
    : '--' unary_expression
    ;
```

The operand of a prefix increment or decrement operation shall be an expression classified as a variable, a property access, or an indexer access. The result of the operation is a value of the same type as the operand.

If the operand of a prefix increment or decrement operation is a property or indexer access, the property or indexer shall have both a get and a set accessor. If this is not the case, a binding-time error occurs.

Unary operator overload resolution ([§11.4.4](expressions.md#1144-unary-operator-overload-resolution)) is applied to select a specific operator implementation. Predefined `++` and `--` operators exist for the following types: `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `float`, `double`, `decimal`, and any enum type. The predefined `++` operators return the value produced by adding `1` to the operand, and the predefined `--` operators return the value produced by subtracting `1` from the operand. In a `checked` context, if the result of this addition or subtraction is outside the range of the result type and the result type is an integral type or enum type, a `System.OverflowException` is thrown.

There shall be an implicit conversion from the return type of the selected unary operator to the type of the *unary_expression*, otherwise a compile-time error occurs.

The run-time processing of a prefix increment or decrement operation of the form `++x` or `--x` consists of the following steps:

- If `x` is classified as a variable:
  - `x` is evaluated to produce the variable.
  - The value of `x` is converted to the operand type of the selected operator and the operator is invoked with this value as its argument.
  - The value returned by the operator is converted to the type of `x`. The resulting value is stored in the location given by the evaluation of `x`.
  - and becomes the result of the operation.
- If `x` is classified as a property or indexer access:
  - The instance expression (if `x` is not `static`) and the argument list (if `x` is an indexer access) associated with `x` are evaluated, and the results are used in the subsequent get and set accessor invocations.
  - The get accessor of `X` is invoked.
  - The value returned by the get accessor is converted to the operand type of the selected operator and operator is invoked with this value as its argument.
  - The value returned by the operator is converted to the type of `x`. The set accessor of `X` is invoked with this value as its value argument.
  - This value also becomes the result of the operation.

The `++` and `--` operators also support postfix notation ([§11.7.14](expressions.md#11714-postfix-increment-and-decrement-operators)). Typically, the result of `x++` or `x--` is the value of `X` before the operation, whereas the result of `++x` or `--x` is the value of `X` after the operation. In either case, `x` itself has the same value after the operation.

An operator `++` or operator `--` implementation can be invoked using either postfix or prefix notation. It is not possible to have separate operator implementations for the two notations.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined prefix increment and decrement operators defined above are also predefined.

### 11.8.7 Cast expressions

A *cast_expression* is used to convert explicitly an expression to a given type.

```ANTLR
cast_expression
    : '(' type ')' unary_expression
    ;
```

A *cast_expression* of the form `(T)E`, where `T` is a type and `E` is a *unary_expression*, performs an explicit conversion ([§10.3](conversions.md#103-explicit-conversions)) of the value of `E` to type `T`. If no explicit conversion exists from `E` to `T`, a binding-time error occurs. Otherwise, the result is the value produced by the explicit conversion. The result is always classified as a value, even if `E` denotes a variable.

The grammar for a *cast_expression* leads to certain syntactic ambiguities.

> *Example*: The expression `(x)–y` could either be interpreted as a *cast_expression* (a cast of `–y` to type `x`) or as an *additive_expression* combined with a *parenthesized_expression* (which computes the value `x – y`). *end example*

To resolve *cast_expression* ambiguities, the following rule exists: A sequence of one or more tokens ([§6.4](lexical-structure.md#64-tokens)) enclosed in parentheses is considered the start of a *cast_expression* only if at least one of the following are true:

- The sequence of tokens is correct grammar for a type, but not for an expression.
- The sequence of tokens is correct grammar for a type, and the token immediately following the closing parentheses is the token “`~`”, the token “`!`”, the token “`(`”, an identifier ([§6.4.3](lexical-structure.md#643-identifiers)), a literal ([§6.4.5](lexical-structure.md#645-literals)), or any keyword ([§6.4.4](lexical-structure.md#644-keywords)) except `as` and `is`.

The term “correct grammar” above means only that the sequence of tokens shall conform to the particular grammatical production. It specifically does not consider the actual meaning of any constituent identifiers.

> *Example*: If `x` and `y` are identifiers, then `x.y` is correct grammar for a type, even if `x.y` doesn’t actually denote a type. *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: From the disambiguation rule, it follows that, if `x` and `y` are identifiers, `(x)y`, `(x)(y)`, and `(x)(-y)` are *cast_expression*s, but `(x)-y` is not, even if `x` identifies a type. However, if `x` is a keyword that identifies a predefined type (such as `int`), then all four forms are *cast_expression*s (because such a keyword could not possibly be an expression by itself). *end note*

### 11.8.8 Await expressions

#### 11.8.8.1 General

The `await` operator is used to suspend evaluation of the enclosing async function until the asynchronous operation represented by the operand has completed.

```ANTLR
await_expression
    : 'await' unary_expression
    ;
```

An *await_expression* is only allowed in the body of an async function ([§14.15](classes.md#1415-async-functions)). Within the nearest enclosing async function, an *await_expression* shall not occur in these places:

- Inside a nested (non-async) anonymous function
- Inside the block of a *lock_statement*
- In an anonymous function conversion to an expression tree type ([§10.7.3](conversions.md#1073-evaluation-of-lambda-expression-conversions-to-expression-tree-types))
- In an unsafe context

> *Note*: An *await_expression* cannot occur in most places within a *query_expression*, because those are syntactically transformed to use non-async lambda expressions. *end note*

Inside an async function, `await` shall not be used as an *available_identifier* although the verbatim identifier `@await` may be used. There is therefore no syntactic ambiguity between *await_expression*s and various expressions involving identifiers. Outside of async functions, `await` acts as a normal identifier.

The operand of an *await_expression* is called the ***task***. It represents an asynchronous operation that may or may not be complete at the time the *await_expression* is evaluated. The purpose of the `await` operator is to suspend execution of the enclosing async function until the awaited task is complete, and then obtain its outcome.

#### 11.8.8.2 Awaitable expressions

The task of an *await_expression* is required to be ***awaitable***. An expression `t` is awaitable if one of the following holds:

- `t` is of compile-time type `dynamic`
- `t` has an accessible instance or extension method called `GetAwaiter` with no parameters and no type parameters, and a return type `A` for which all of the following hold:
  - `A` implements the interface `System.Runtime.CompilerServices.INotifyCompletion` (hereafter known as `INotifyCompletion` for brevity)
  - `A` has an accessible, readable instance property `IsCompleted` of type `bool`
  - `A` has an accessible instance method `GetResult` with no parameters and no type parameters

The purpose of the `GetAwaiter` method is to obtain an ***awaiter*** for the task. The type `A` is called the ***awaiter type*** for the await expression.

The purpose of the `IsCompleted` property is to determine if the task is already complete. If so, there is no need to suspend evaluation.

The purpose of the `INotifyCompletion.OnCompleted` method is to sign up a “continuation” to the task; i.e., a delegate (of type `System.Action`) that will be invoked once the task is complete.

The purpose of the `GetResult` method is to obtain the outcome of the task once it is complete. This outcome may be successful completion, possibly with a result value, or it may be an exception which is thrown by the `GetResult` method.

#### 11.8.8.3 Classification of await expressions

The expression `await t` is classified the same way as the expression `(t).GetAwaiter().GetResult()`. Thus, if the return type of `GetResult` is `void`, the *await_expression* is classified as nothing. If it has a non-`void` return type `T`, the *await_expression* is classified as a value of type `T`.

#### 11.8.8.4 Run-time evaluation of await expressions

At run-time, the expression `await t` is evaluated as follows:

- An awaiter `a` is obtained by evaluating the expression `(t).GetAwaiter()`.
- A `bool` `b` is obtained by evaluating the expression `(a).IsCompleted`.
- If `b` is `false` then evaluation depends on whether a implements the interface `System.Runtime.CompilerServices.ICriticalNotifyCompletion` (hereafter known as `ICriticalNotifyCompletion` for brevity). This check is done at binding time; i.e., at run-time if `a` has the compile-time type `dynamic`, and at compile-time otherwise. Let `r` denote the resumption delegate ([§14.15](classes.md#1415-async-functions)):
  - If `a` does not implement `ICriticalNotifyCompletion`, then the expression
    `((a) as INotifyCompletion).OnCompleted(r)` is evaluated.
  - If `a` does implement `ICriticalNotifyCompletion`, then the expression
    `((a) as ICriticalNotifyCompletion).UnsafeOnCompleted(r)` is evaluated.
  - Evaluation is then suspended, and control is returned to the current caller of the async function.
- Either immediately after (if `b` was `true`), or upon later invocation of the resumption delegate (if `b` was `false`), the expression `(a).GetResult()` is evaluated. If it returns a value, that value is the result of the *await_expression*. Otherwise, the result is nothing.

An awaiter’s implementation of the interface methods `INotifyCompletion.OnCompleted` and `ICriticalNotifyCompletion.UnsafeOnCompleted` should cause the delegate `r` to be invoked at most once. Otherwise, the behavior of the enclosing async function is undefined.

## 11.9 Arithmetic operators

### 11.9.1 General

The `*`, `/`, `%`, `+`, and `–` operators are called the arithmetic operators.

```ANTLR
multiplicative_expression
    : unary_expression
    | multiplicative_expression '*' unary_expression
    | multiplicative_expression '/' unary_expression
    | multiplicative_expression '%' unary_expression
    ;

additive_expression
    : multiplicative_expression
    | additive_expression '+' multiplicative_expression
    | additive_expression '-' multiplicative_expression
    ;
```

If an operand of an arithmetic operator has the compile-time type `dynamic`, then the expression is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)). In this case, the compile-time type of the expression is `dynamic`, and the resolution described below will take place at run-time using the run-time type of those operands that have the compile-time type `dynamic`.

### 11.9.2 Multiplication operator

For an operation of the form `x * y`, binary operator overload resolution ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) is applied to select a specific operator implementation. The operands are converted to the parameter types of the selected operator, and the type of the result is the return type of the operator.

The predefined multiplication operators are listed below. The operators all compute the product of `x` and `y`.

- Integer multiplication:

  ```csharp
  int operator *(int x, int y);
  uint operator *(uint x, uint y);
  long operator *(long x, long y);
  ulong operator *(ulong x, ulong y);
  ```

  In a `checked` context, if the product is outside the range of the result type, a `System.OverflowException` is thrown. In an `unchecked` context, overflows are not reported and any significant high-order bits outside the range of the result type are discarded.
- Floating-point multiplication:

  ```csharp
  float operator *(float x, float y);
  double operator *(double x, double y);
  ```

  The product is computed according to the rules of IEC 60559 arithmetic. The following table lists the results of all possible combinations of nonzero finite values, zeros, infinities, and NaNs. In the table, `x` and `y` are positive finite values. `z` is the result of `x * y`, rounded to the nearest representable value. If the magnitude of the result is too large for the destination type, `z` is infinity. Because of rounding, `z` may be zero even though neither `x` nor `y` is zero.
  
  <!-- Custom Word conversion: multiplication -->
  <table>
  <!-- md equivalent:   ` `   | **`+y`**  | **`-y`**  | **`+0`**  | **`-0`**  | **`+∞`**  | **`-∞`**  | **`NaN`** -->
    <tr>
      <td></td>
     <td><b><code>+y</code></b></td>
      <td><b><code>-y</code></b></td>
      <td><b><code>+0</code></b></td>
      <td><b><code>-0</code></b></td>
      <td><b><code>+∞</code></b></td>
      <td><b><code>-∞</code></b></td>
      <td><b><code>NaN</code></b></td>
    </tr>
  <!-- md equivalent: `+x`  | `+z`  | `-z`  | `+0`  | `-0`  | `+∞`  | `-∞`  | `NaN` -->
    <tr>
      <td><code>+x</code></td>
      <td><code>+z</code></td>
      <td><code>-z</code></td>
      <td><code>+0</code></td>
      <td><code>-0</code></td>
      <td><code>+∞</code></td>
      <td><code>-∞</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `-x`  | `-z`  | `+z`  | `-0`  | `+0`  | `-∞`  | `+∞`  | `NaN` -->
    <tr>
      <td><code>-x</code></td>
      <td><code>-z</code></td>
      <td><code>+z</code></td>
      <td><code>-0</code></td>
      <td><code>+0</code></td>
      <td><code>-∞</code></td>
      <td><code>+∞</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent:   `+0`  | `+0`  | `-0`  | `+0`  | `-0`  | `NaN` | `NaN` | `NaN` -->
    <tr>
      <td><code>+0</code></td>
      <td><code>+0</code></td>
      <td><code>-0</code></td>
      <td><code>+0</code></td>
      <td><code>-0</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `-0`  | `-0`  | `+0`  | `-0`  | `+0`  | `NaN` | `NaN` | `NaN` -->
    <tr>
      <td><code>-0</code></td>
      <td><code>-0</code></td>
      <td><code>+0</code></td>
      <td><code>-0</code></td>
      <td><code>+0</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `+∞`  | `+∞`  | `-∞`  | `NaN` | `NaN` | `+∞`  | `-∞`  | `NaN` -->
    <tr>
      <td><code>+∞</code></td>
      <td><code>+∞</code></td>
      <td><code>-∞</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>+∞</code></td>
      <td><code>-∞</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `-∞`  | `-∞`  | `+∞`  | `NaN` | `NaN` | `-∞`  | `+∞`  | `NaN` -->
    <tr>
      <td><code>-∞</code></td>
      <td><code>-∞</code></td>
      <td><code>+∞</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>-∞</code></td>
      <td><code>+∞</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` -->
    <tr>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  </table>

  (Except were otherwise noted, in the floating-point tables in [§11.9.2](expressions.md#1192-multiplication-operator)–[§11.9.6](expressions.md#1196-subtraction-operator) the use of “`+`” means the value is positive; the use of “`-`” means the value is negative; and the lack of a sign means the value may be positive or negative or has no sign (NaN).)
- Decimal multiplication:

  ```csharp
  decimal operator *(decimal x, decimal y);
  ```

  If the magnitude of the resulting value is too large to represent in the decimal format, a `System.OverflowException` is thrown. Because of rounding, the result may be zero even though neither operand is zero. The scale of the result, before any rounding, is the sum of the scales of the two operands.
  Decimal multiplication is equivalent to using the multiplication operator of type `System.Decimal`.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined multiplication operators defined above are also predefined.

### 11.9.3 Division operator

For an operation of the form `x / y`, binary operator overload resolution ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) is applied to select a specific operator implementation. The operands are converted to the parameter types of the selected operator, and the type of the result is the return type of the operator.

The predefined division operators are listed below. The operators all compute the quotient of `x` and `y`.

- Integer division:

  ```csharp
  int operator /(int x, int y);
  uint operator /(uint x, uint y);
  long operator /(long x, long y);
  ulong operator /(ulong x, ulong y);
  ```

  If the value of the right operand is zero, a `System.DivideByZeroException` is thrown.

  The division rounds the result towards zero. Thus the absolute value of the result is the largest possible integer that is less than or equal to the absolute value of the quotient of the two operands. The result is zero or positive when the two operands have the same sign and zero or negative when the two operands have opposite signs.

  If the left operand is the smallest representable `int` or `long` value and the right operand is `–1`, an overflow occurs. In a `checked` context, this causes a `System.ArithmeticException` (or a subclass thereof) to be thrown. In an `unchecked` context, it is implementation-defined as to whether a `System.ArithmeticException` (or a subclass thereof) is thrown or the overflow goes unreported with the resulting value being that of the left operand.
- Floating-point division:

  ```csharp
  float operator /(float x, float y);
  double operator /(double x, double y);
  ```

  The quotient is computed according to the rules of IEC 60559 arithmetic. The following table lists the results of all possible combinations of nonzero finite values, zeros, infinities, and NaNs. In the table, `x` and `y` are positive finite values. `z` is the result of `x / y`, rounded to the nearest representable value.

  <!-- Custom Word conversion: division -->
  <table>
  <!-- md equivalent:   ` `   | **`+y`**  | **`-y`**  | **`+0`**  | **`-0`**  | **`+∞`**  | **`-∞`**  | **`NaN`** -->
    <tr>
      <td></td>
      <td><b><code>+y</code></b></td>
      <td><b><code>-y</code></b></td>
      <td><b><code>+0</code></b></td>
      <td><b><code>-0</code></b></td>
      <td><b><code>+∞</code></b></td>
      <td><b><code>-∞</code></b></td>
      <td><b><code>NaN</code></b></td>
    </tr>
  <!-- md equivalent:   `+x`  | `+z`  | `-z`  | `+∞`  | `-∞`  | `+0`  | `-0`  | `NaN` -->
    <tr>
      <td><code>+x</code></td>
      <td><code>+z</code></td>
      <td><code>-z</code></td>
      <td><code>+∞</code></td>
      <td><code>–∞</code></td>
      <td><code>+0</code></td>
      <td><code>-0</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `-x`  | `-z`  | `+z`  | `-∞`  | `+∞`  | `-0`  | `+0`  | `NaN` -->
    <tr>
      <td><code>-x</code></td>
      <td><code>-z</code></td>
      <td><code>+z</code></td>
      <td><code>-∞</code></td>
      <td><code>+∞</code></td>
      <td><code>-0</code></td>
      <td><code>+0</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent:  `+0`  | `+0`  | `-0`  | `NaN` | `NaN` | `+0`  | `-0`  | `NaN` -->
    <tr>
      <td><code>+0</code></td>
      <td><code>+0</code></td>
      <td><code>-0</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>+0</code></td>
      <td><code>-0</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `-0`  | `-0`  | `+0`  | `NaN` | `NaN` | `-0`  | `+0`  | `NaN` -->
    <tr>
      <td><code>-0</code></td>
      <td><code>-0</code></td>
      <td><code>+0</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>-0</code></td>
      <td><code>+0</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `+∞`  | `+∞`  | `-∞`  | `+∞`  | `-∞`  | `NaN` | `NaN` | `NaN` -->
    <tr>
      <td><code>+∞</code></td>
      <td><code>+∞</code></td>
      <td><code>-∞</code></td>
      <td><code>+∞</code></td>
      <td><code>-∞</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent:  `-∞`  | `-∞`  | `+∞`  | `-∞`  | `+∞`  | `NaN` | `NaN` | `NaN` -->
    <tr>
      <td><code>-∞</code></td>
      <td><code>-∞</code></td>
      <td><code>+∞</code></td>
      <td><code>-∞</code></td>
      <td><code>+∞</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent:   `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` -->
    <tr>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  </table>

- Decimal division:

  ```csharp
  decimal operator /(decimal x, decimal y);
  ```

  If the value of the right operand is zero, a `System.DivideByZeroException` is thrown. If the magnitude of the resulting value is too large to represent in the decimal format, a `System.OverflowException` is thrown. Because of rounding, the result may be zero even though the first operand is not zero. The scale of the result, before any rounding, is the closest scale to the preferred scale that will preserve a result equal to the exact result. The preferred scale is the scale of `x` less the scale of `y`.

  Decimal division is equivalent to using the division operator of type `System.Decimal`.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined division operators defined above are also predefined.

### 11.9.4 Remainder operator

For an operation of the form `x % y`, binary operator overload resolution ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) is applied to select a specific operator implementation. The operands are converted to the parameter types of the selected operator, and the type of the result is the return type of the operator.

The predefined remainder operators are listed below. The operators all compute the remainder of the division between `x` and `y`.

- Integer remainder:

  ```csharp
  int operator %(int x, int y);
  uint operator %(uint x, uint y);
  long operator %(long x, long y);
  ulong operator %(ulong x, ulong y);
  ```
  
  The result of `x % y` is the value produced by `x – (x / y) * y`. If `y` is zero, a `System.DivideByZeroException` is thrown.

  If the left operand is the smallest `int` or `long` value and the right operand is `–1`, a `System.OverflowException` is thrown if and only if `x / y` would throw an exception.
- Floating-point remainder:

  ```csharp
  float operator %(float x, float y);
  double operator %(double x, double y);
  ```
  
  The following table lists the results of all possible combinations of nonzero finite values, zeros, infinities, and NaNs. In the table, `x` and `y` are positive finite values. `z` is the result of `x % y` and is computed as `x – n * y`, where n is the largest possible integer that is less than or equal to `x / y`. This method of computing the remainder is analogous to that used for integer operands, but differs from the IEC 60559 definition (in which `n` is the integer closest to `x / y`).

  <!-- Custom Word conversion: remainder -->
  <table>
  <!-- md equivalent: ` `   | **`+y`**  | **`-y`**  | **`+0`**  | **`-0`**  | **`+∞`**  | **`-∞`**  | **`NaN`** -->
    <tr>
      <td></td>
      <td><b><code>+y</code></b></td>
      <td><b><code>-y</code></b></td>
      <td><b><code>+0</code></b></td>
      <td><b><code>-0</code></b></td>
      <td><b><code>+∞</code></b></td>
      <td><b><code>–∞</code></b></td>
      <td><b><code>NaN</code></b></td>
    </tr>
  <!-- md equivalent: `+x`  | `+z`  | `+z`  | `NaN` | `NaN` | `+x`  | `+x`  | `NaN` -->
    <tr>
      <td><code>+x</code></td>
      <td><code>+z</code></td>
      <td><code>+z</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>+x</code></td>
      <td><code>+x</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `-x`  | `-z`  | `-z`  | `NaN` | `NaN` | `-x`  | `-x`  | `NaN` -->
    <tr>
      <td><code>-x</code></td>
      <td><code>-z</code></td>
      <td><code>-z</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>-x</code></td>
      <td><code>-x</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `+0`  | `+0`  | `+0`  | `NaN` | `NaN` | `+0`  | `+0`  | `NaN` -->
    <tr>
      <td><code>+0</code></td>
      <td><code>+0</code></td>
      <td><code>+0</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>+0</code></td>
      <td><code>+0</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `-0`  | `-0`  | `-0`  | `NaN` | `NaN` | `-0`  | `-0`  | `NaN` -->
    <tr>
      <td><code>-0</code></td>
      <td><code>-0</code></td>
      <td><code>-0</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>-0</code></td>
      <td><code>-0</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `+∞`  | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` -->
    <tr>
      <td><code>+∞</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `-∞`  | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` -->
    <tr>
      <td><code>-∞</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` -->
    <tr>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  </table>
- Decimal remainder:

  ```csharp
  decimal operator %(decimal x, decimal y);
  ```

  If the value of the right operand is zero, a `System.DivideByZeroException` is thrown. It is implementation-defined when a `System.ArithmeticException` (or a subclass thereof) is thrown. A conforming implementation shall not throw an exception for `x % y` in any case where `x / y` does not throw an exception. The scale of the result, before any rounding, is the larger of the scales of the two operands, and the sign of the result, if non-zero, is the same as that of `x`.
  
  Decimal remainder is equivalent to using the remainder operator of type `System.Decimal`.
  > *Note*: These rules ensure that for all types, the result never has the opposite sign of the left operand. *end note*

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined remainder operators defined above are also predefined.

### 11.9.5 Addition operator

For an operation of the form `x + y`, binary operator overload resolution ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) is applied to select a specific operator implementation. The operands are converted to the parameter types of the selected operator, and the type of the result is the return type of the operator.

The predefined addition operators are listed below. For numeric and enumeration types, the predefined addition operators compute the sum of the two operands. When one or both operands are of type `string`, the predefined addition operators concatenate the string representation of the operands.

- Integer addition:

  ```csharp
  int operator +(int x, int y);
  uint operator +(uint x, uint y);
  long operator +(long x, long y);
  ulong operator +(ulong x, ulong y
  ```

  In a `checked` context, if the sum is outside the range of the result type, a `System.OverflowException` is thrown. In an `unchecked` context, overflows are not reported and any significant high-order bits outside the range of the result type are discarded.

- Floating-point addition:

  ```csharp
  float operator +(float x, float y);
  double operator +(double x, double y);
  ```

  The sum is computed according to the rules of IEC 60559 arithmetic. The following table lists the results of all possible combinations of nonzero finite values, zeros, infinities, and NaNs. In the table, `x` and `y` are nonzero finite values, and `z` is the result of `x + y`,. If `x` and `y` have the same magnitude but opposite signs, `z` is positive zero. If `x + y` is too large to represent in the destination type, `z` is an infinity with the same sign as `x + y`.

  <!-- Custom Word conversion: addition -->
  <table>
  <!-- md equivalent: ` `   | **`y`**   | **`+0`**  | **`-0`**  | **`+∞`**  | **`-∞`**  | **`NaN`* -->
    <tr>
      <td></td>
      <td><b><code>y</code></b></td>
      <td><b><code>+0</code></b></td>
      <td><b><code>-0</code></b></td>
      <td><b><code>+∞</code></b></td>
      <td><b><code>–∞</code></b></td>
      <td><b><code>NaN</code></b></td>
    </tr>
  <!-- md equivalent: `x`   | `z`   | `x`   | `x`   | `+∞`  | `-∞`  | `NaN` -->
    <tr>
      <td><code>x</code></td>
      <td><code>z</code></td>
      <td><code>x</code></td>
      <td><code>x</code></td>
      <td><code>+∞</code></td>
      <td><code>-∞</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `+0`  | `y`   | `+0`  | `+0`  | `+∞`  | `–∞`  | `NaN` -->
    <tr>
      <td><code>+0</code></td>
      <td><code>y</code></td>
      <td><code>+0</code></td>
      <td><code>+0</code></td>
      <td><code>+∞</code></td>
      <td><code>–∞</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `-0`  | `y`   | `+0`  | `-0`  | `+∞`  | `-∞`  | `NaN` -->
    <tr>
      <td><code>-0</code></td>
      <td><code>y</code></td>
      <td><code>+0</code></td>
      <td><code>-0</code></td>
      <td><code>+∞</code></td>
      <td><code>-∞</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `+∞`  | `+∞`  | `+∞`  | `+∞`  | `+∞`  | `NaN` | `NaN` -->
    <tr>
      <td><code>+∞</code></td>
      <td><code>+∞</code></td>
      <td><code>+∞</code></td>
      <td><code>+∞</code></td>
      <td><code>+∞</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent:   `-∞`  | `-∞`  | `-∞`  | `-∞`  | `NaN` | `-∞`  | `NaN` -->
    <tr>
      <td><code>-∞</code></td>
      <td><code>-∞</code></td>
      <td><code>-∞</code></td>
      <td><code>-∞</code></td>
      <td><code>NaN</code></td>
      <td><code>-∞</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent:  `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` -->
    <tr>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  </table>
  
- Decimal addition:

  ```csharp
  decimal operator +(decimal x, decimal y);
  ```

  If the magnitude of the resulting value is too large to represent in the decimal format, a `System.OverflowException` is thrown. The scale of the result, before any rounding, is the larger of the scales of the two operands.

  Decimal addition is equivalent to using the addition operator of type `System.Decimal`.
- Enumeration addition. Every enumeration type implicitly provides the following predefined operators, where `E` is the enum type, and `U` is the underlying type of `E`:

  ```csharp
  E operator +(E x, U y);
  E operator +(U x, E y);
  ```

  At run-time these operators are evaluated exactly as `(E)((U)x + (U)y`).
- String concatenation:

  ```csharp
  string operator +(string x, string y);
  string operator +(string x, object y);
  string operator +(object x, string y);
  ```

  These overloads of the binary `+` operator perform string concatenation. If an operand of string concatenation is `null`, an empty string is substituted. Otherwise, any non-`string` operand is converted to its string representation by invoking the virtual `ToString` method inherited from type `object`. If `ToString` returns `null`, an empty string is substituted.
  
  > *Example*:
  >
  > ```csharp
  > using System;
  > class Test
  > {
  >     static void Main()
  >     {
  >         string s = null;
  >         Console.WriteLine("s = >" + s + "<");  // Displays s = ><
  >
  >         int i = 1;
  >         Console.WriteLine("i = " + i);         // Displays i = 1
  >
  >         float f = 1.2300E+15F;
  >         Console.WriteLine("f = " + f);         // Displays f = 1.23E+15
  >
  >         decimal d = 2.900m;
  >         Console.WriteLine("d = " + d);         // Displays d = 2.900
  >    }
  > }
  > ```
  >
  > The output shown in the comments is the typical result on a US-English system. The precise output might depend on the regional settings of the execution environment. The string-concatenation operator itself behaves the same way in each case, but the `ToString` methods implicitly called during execution might be affected by regional settings.
  >
  > *end example*

  The result of the string concatenation operator is a `string` that consists of the characters of the left operand followed by the characters of the right operand. The string concatenation operator never returns a `null` value. A `System.OutOfMemoryException` may be thrown if there is not enough memory available to allocate the resulting string.
- Delegate combination. Every delegate type implicitly provides the following predefined operator, where `D` is the delegate type:

  ```csharp
  D operator +(D x, D y);
  ```

  If the first operand is `null`, the result of the operation is the value of the second operand (even if that is also `null`). Otherwise, if the second operand is `null`, then the result of the operation is the value of the first operand. Otherwise, the result of the operation is a new delegate instance whose invocation list consists of the elements in the invocation list of the first operand, followed by the elements in the invocation list of the second operand. That is, the invocation list of the resulting delegate is the concatenation of the invocation lists of the two operands.

  > *Note*: For examples of delegate combination, see [§11.9.6](expressions.md#1196-subtraction-operator) and [§19.6](delegates.md#196-delegate-invocation). Since `System.Delegate` is not a delegate type, operator + is not defined for it. *end note*

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined addition operators defined above are also predefined.

### 11.9.6 Subtraction operator

For an operation of the form `x – y`, binary operator overload resolution ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) is applied to select a specific operator implementation. The operands are converted to the parameter types of the selected operator, and the type of the result is the return type of the operator.

The predefined subtraction operators are listed below. The operators all subtract `y` from `x`.

- Integer subtraction:

  ```csharp
  int operator –(int x, int y);
  uint operator –(uint x, uint y);
  long operator –(long x, long y);
  ulong operator –(ulong x, ulong y
  ```

  In a `checked` context, if the difference is outside the range of the result type, a `System.OverflowException` is thrown. In an `unchecked` context, overflows are not reported and any significant high-order bits outside the range of the result type are discarded.
- Floating-point subtraction:

  ```csharp
  float operator –(float x, float y);
  double operator –(double x, double y);
  ```

  The difference is computed according to the rules of IEC 60559 arithmetic. The following table lists the results of all possible combinations of nonzero finite values, zeros, infinities, and NaNs. In the table, `x` and `y` are nonzero finite values, and `z` is the result of `x – y`. If `x` and `y` are equal, `z` is positive zero. If `x – y` is too large to represent in the destination type, `z` is an infinity with the same sign as `x – y`.

  <!-- Custom Word conversion: subtraction -->
  <table>
  <!-- md equivalent: ` `   | **`y`**   | **`+0`**  | **`-0`**  | **`+∞`**  | **`-∞`**  | **`NaN`** -->
    <tr>
      <td></td>
      <td><b><code>y</code></b></td>
      <td><b><code>+0</code></b></td>
      <td><b><code>-0</code></b></td>
      <td><b><code>+∞</code></b></td>
      <td><b><code>–∞</code></b></td>
      <td><b><code>NaN</code></b></td>
    </tr>
  <!-- md equivalent: `x`   | `z`   | `x`   | `x`   | `-∞`  | `+∞`  | `NaN` -->
    <tr>
      <td><code>x</code></td>
      <td><code>z</code></td>
      <td><code>x</code></td>
      <td><code>x</code></td>
      <td><code>–∞</code></td>
      <td><code>+∞</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `+0`  | `-y`  | `+0`  | `+0`  | `-∞`  | `+∞`  | `NaN` -->
    <tr>
      <td><code>+0</code></td>
      <td><code>-y</code></td>
      <td><code>+0</code></td>
      <td><code>+0</code></td>
      <td><code>–∞</code></td>
      <td><code>+∞</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `-0`  | `-y`  | `-0`  | `+0`  | `-∞`  | `+∞`  | `NaN` -->
    <tr>
      <td><code>-0</code></td>
      <td><code>-y</code></td>
      <td><code>-0</code></td>
      <td><code>+0</code></td>
      <td><code>–∞</code></td>
      <td><code>+∞</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `+∞`  | `+∞`  | `+∞`  | `+∞`  | `NaN` | `+∞`  | `NaN` -->
    <tr>
      <td><code>+∞</code></td>
      <td><code>+∞</code></td>
      <td><code>+∞</code></td>
      <td><code>+∞</code></td>
      <td><code>NaN</code></td>
      <td><code>+∞</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent:  `-∞`  | `-∞`  | `-∞`  | `-∞`  | `-∞`  | `NaN` | `NaN` -->
    <tr>
      <td><code>-∞</code></td>
      <td><code>-∞</code></td>
      <td><code>-∞</code></td>
      <td><code>-∞</code></td>
      <td><code>–∞</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  <!-- md equivalent: `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` -->
    <tr>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
      <td><code>NaN</code></td>
    </tr>
  </table>
  
  (In the above table the `-y` entries denote the *negation* of `y`, not that the value is negative.)
- Decimal subtraction:

  ```csharp
  decimal operator –(decimal x, decimal y);
  ```

  If the magnitude of the resulting value is too large to represent in the decimal format, a `System.OverflowException` is thrown. The scale of the result, before any rounding, is the larger of the scales of the two operands.

  Decimal subtraction is equivalent to using the subtraction operator of type `System.Decimal`.

- Enumeration subtraction. Every enumeration type implicitly provides the following predefined operator, where `E` is the enum type, and `U` is the underlying type of `E`:

  ```csharp
  U operator –(E x, E y);
  ```

  This operator is evaluated exactly as `(U)((U)x – (U)y)`. In other words, the operator computes the difference between the ordinal values of `x` and `y`, and the type of the result is the underlying type of the enumeration.

  ```csharp
  E operator –(E x, U y);
  ```

  This operator is evaluated exactly as `(E)((U)x – y)`. In other words, the operator subtracts a value from the underlying type of the enumeration, yielding a value of the enumeration.

- Delegate removal. Every delegate type implicitly provides the following predefined operator, where `D` is the delegate type:

  ```csharp
  D operator –(D x, D y);
  ```

  The semantics are as follows:
  - If the first operand is `null`, the result of the operation is `null`.
  - Otherwise, if the second operand is `null`, then the result of the operation is the value of the first operand.
  - Otherwise, both operands represent non-empty invocation lists ([§19.2](delegates.md#192-delegate-declarations)).
    - If the lists compare equal, as determined by the delegate equality operator ([§11.11.9](expressions.md#11119-delegate-equality-operators)), the result of the operation is `null`.
    - Otherwise, the result of the operation is a new invocation list consisting of the first operand’s list with the second operand’s entries removed from it, provided the second operand’s list is a sublist of the first’s. (To determine sublist equality, corresponding entries are compared as for the delegate equality operator.) If the second operand’s list matches multiple sublists of contiguous entries in the first operand’s list, the last matching sublist of contiguous entries is removed.
    - Otherwise, the result of the operation is the value of the left operand.

  Neither of the operands’ lists (if any) is changed in the process.

  > *Example*:
  >
  > ```csharp
  >
  > delegate void D(int x);
  >
  > class C
  > {
  >     public static void M1(int i) { /* ... */ }
  >     public static void M2(int i) { /* ... */ }
  > }
  >
  > class Test
  > {
  >     static void Main()
  >     {
  >         D cd1 = new D(C.M1);
  >         D cd2 = new D(C.M2);
  >         D list = null;
  > 
  >         list = null - cd1;                             // null
  >         list = (cd1 + cd2 + cd2 + cd1) - null;         // M1 + M2 + M2 + M1
  >         list = (cd1 + cd2 + cd2 + cd1) - cd1;          // M1 + M2 + M2
  >         list = (cd1 + cd2 + cd2 + cd1) - (cd1 + cd2);  // M2 + M1
  >         list = (cd1 + cd2 + cd2 + cd1) - (cd2 + cd2);  // M1 + M1
  >         list = (cd1 + cd2 + cd2 + cd1) - (cd2 + cd1);  // M1 + M2
  >         list = (cd1 + cd2 + cd2 + cd1) - (cd1 + cd1);  // M1 + M2 + M2 + M1
  >         list = (cd1 + cd2 + cd2 + cd1) - (cd1 + cd2 + cd2 + cd1);  // null
  >     }
  > }
  > ```
  >
  > *end example*

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined subtraction operators defined above are also predefined.

## 11.10 Shift operators

The `<<` and `>>` operators are used to perform bit-shifting operations.

```ANTLR
shift_expression
    : additive_expression
    | shift_expression '<<' additive_expression
    | shift_expression right_shift additive_expression
    ;
```

If an operand of a *shift_expression* has the compile-time type `dynamic`, then the expression is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)). In this case, the compile-time type of the expression is `dynamic`, and the resolution described below will take place at run-time using the run-time type of those operands that have the compile-time type `dynamic`.

For an operation of the form `x << count` or `x >> count`, binary operator overload resolution ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) is applied to select a specific operator implementation. The operands are converted to the parameter types of the selected operator, and the type of the result is the return type of the operator.

When declaring an overloaded shift operator, the type of the first operand shall always be the class or struct containing the operator declaration, and the type of the second operand shall always be `int`.

The predefined shift operators are listed below.

- Shift left:

  ```csharp
  int operator <<(int x, int count);
  uint operator <<(uint x, int count);
  long operator <<(long x, int count);
  ulong operator <<(ulong x, int count);
  ```

  The `<<` operator shifts `x` left by a number of bits computed as described below.

  The high-order bits outside the range of the result type of `x` are discarded, the remaining bits are shifted left, and the low-order empty bit positions are set to zero.
- Shift right:

  ```csharp
  int operator >>(int x, int count);
  uint operator >>(uint x, int count);
  long operator >>(long x, int count);
  ulong operator >>(ulong x, int count);
  ```

  The `>>` operator shifts `x` right by a number of bits computed as described below.

  When `x` is of type `int` or `long`, the low-order bits of `x` are discarded, the remaining bits are shifted right, and the high-order empty bit positions are set to zero if `x` is non-negative and set to one if `x` is negative.

  When `x` is of type `uint` or `ulong`, the low-order bits of `x` are discarded, the remaining bits are shifted right, and the high-order empty bit positions are set to zero.

For the predefined operators, the number of bits to shift is computed as follows:

- When the type of `x` is `int` or `uint`, the shift count is given by the low-order five bits of `count`. In other words, the shift count is computed from `count & 0x1F`.
- When the type of `x` is `long` or `ulong`, the shift count is given by the low-order six bits of `count`. In other words, the shift count is computed from `count & 0x3F`.

If the resulting shift count is zero, the shift operators simply return the value of `x`.

Shift operations never cause overflows and produce the same results in checked and unchecked contexts.

When the left operand of the `>>` operator is of a signed integral type, the operator performs an *arithmetic* shift right wherein the value of the most significant bit (the sign bit) of the operand is propagated to the high-order empty bit positions. When the left operand of the `>>` operator is of an unsigned integral type, the operator performs a *logical* shift right wherein high-order empty bit positions are always set to zero. To perform the opposite operation of that inferred from the operand type, explicit casts can be used.

> *Example*: If `x` is a variable of type `int`, the operation `unchecked ((int)((uint)x >> y))` performs a logical shift right of `x`. *end example*

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined shift operators defined above are also predefined.

## 11.11 Relational and type-testing operators

### 11.11.1 General

The `==`, `!=`, `<`, `>`, `<=`, `>=`, `is`, and `as` operators are called the relational and type-testing operators.

```ANTLR
relational_expression
    : shift_expression
    | relational_expression '<' shift_expression
    | relational_expression '>' shift_expression
    | relational_expression '<=' shift_expression
    | relational_expression '>=' shift_expression
    | relational_expression 'is' type
    | relational_expression 'as' type
    ;

equality_expression
    : relational_expression
    | equality_expression '==' relational_expression
    | equality_expression '!=' relational_expression
    ;
```

The `is` operator is described in [§11.11.11](expressions.md#111111-the-is-operator) and the `as` operator is described in [§11.11.12](expressions.md#111112-the-as-operator).

The `==`, `!=`, `<`, `>`, `<=` and `>=` operators are ***comparison operators***.

If an operand of a comparison operator has the compile-time type `dynamic`, then the expression is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)). In this case the compile-time type of the expression is `dynamic`, and the resolution described below will take place at run-time using the run-time type of those operands that have the compile-time type `dynamic`.

For an operation of the form `x «op» y`, where «op» is a comparison operator, overload resolution ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) is applied to select a specific operator implementation. The operands are converted to the parameter types of the selected operator, and the type of the result is the return type of the operator. If both operands of an *equality_expression* are the `null` literal, then overload resolution is not performed and the expression evaluates to a constant value of `true` or `false` according to whether the operator is `==` or `!=`.

The predefined comparison operators are described in the following subclauses. All predefined comparison operators return a result of type bool, as described in the following table.

**Operation** | **Result**
------------- | ----------------------------------------------------------
`x == y`      | `true` if `x` is equal to `y`, `false` otherwise
`x != y`      | `true` if `x` is not equal to `y`, `false` otherwise
`x < y`       | `true` if `x` is less than `y`, `false` otherwise
`x > y`       | `true` if `x` is greater than `y`, `false` otherwise
`x <= y`      | `true` if `x` is less than or equal to `y`, `false` otherwise
`x >= y`      | `true` if `x` is greater than or equal to `y`, `false` otherwise

### 11.11.2 Integer comparison operators

The predefined integer comparison operators are:

```csharp
bool operator ==(int x, int y);
bool operator ==(uint x, uint y);
bool operator ==(long x, long y);
bool operator ==(ulong x, ulong y);

bool operator !=(int x, int y);
bool operator !=(uint x, uint y);
bool operator !=(long x, long y);
bool operator !=(ulong x, ulong y);

bool operator <(int x, int y);
bool operator <(uint x, uint y);
bool operator <(long x, long y);
bool operator <(ulong x, ulong y);

bool operator >(int x, int y);
bool operator >(uint x, uint y);
bool operator >(long x, long y);
bool operator >(ulong x, ulong y);

bool operator <=(int x, int y);
bool operator <=(uint x, uint y);
bool operator <=(long x, long y);
bool operator <=(ulong x, ulong y);

bool operator >=(int x, int y);
bool operator >=(uint x, uint y);
bool operator >=(long x, long y);
bool operator >=(ulong x, ulong y);
```

Each of these operators compares the numeric values of the two integer operands and returns a `bool` value that indicates whether the particular relation is `true` or `false`.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined integer comparison operators defined above are also predefined.

### 11.11.3 Floating-point comparison operators

The predefined floating-point comparison operators are:

```csharp
bool operator ==(float x, float y);
bool operator ==(double x, double y);

bool operator !=(float x, float y);
bool operator !=(double x, double y);

bool operator <(float x, float y);
bool operator <(double x, double y);

bool operator >(float x, float y);
bool operator >(double x, double y);

bool operator <=(float x, float y);
bool operator <=(double x, double y);

bool operator >=(float x, float y);
bool operator >=(double x, double y);
```

The operators compare the operands according to the rules of the IEC 60559 standard:

If either operand is NaN, the result is `false` for all operators except `!=`, for which the result is `true`. For any two operands, `x != y` always produces the same result as `!(x == y)`. However, when one or both operands are NaN, the `<`, `>`, `<=`, and `>=` operators do *not* produce the same results as the logical negation of the opposite operator.

> *Example*: If either of `x` and `y` is NaN, then `x < y` is `false`, but `!(x >= y)` is `true`. *end example*

When neither operand is NaN, the operators compare the values of the two floating-point operands with respect to the ordering

```csharp
–∞ < –max < ... < –min < –0.0 == +0.0 < +min < ... < +max < +∞
```

where `min` and `max` are the smallest and largest positive finite values that can be represented in the given floating-point format. Notable effects of this ordering are:

- Negative and positive zeros are considered equal.
- A negative infinity is considered less than all other values, but equal to another negative infinity.
- A positive infinity is considered greater than all other values, but equal to another positive infinity.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined floating-point comparison operators defined above are also predefined.

### 11.11.4 Decimal comparison operators

The predefined decimal comparison operators are:

```csharp
bool operator ==(decimal x, decimal y);
bool operator !=(decimal x, decimal y);
bool operator <(decimal x, decimal y);
bool operator >(decimal x, decimal y);
bool operator <=(decimal x, decimal y);
bool operator >=(decimal x, decimal y);
```

Each of these operators compares the numeric values of the two decimal operands and returns a `bool` value that indicates whether the particular relation is `true` or `false`. Each decimal comparison is equivalent to using the corresponding relational or equality operator of type `System.Decimal`.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined decimal comparison operators defined above are also predefined.

### 11.11.5 Boolean equality operators

The predefined Boolean equality operators are:

```csharp
bool operator ==(bool x, bool y);
bool operator !=(bool x, bool y);
```

The result of `==` is `true` if both `x` and `y` are `true` or if both `x` and `y` are `false`. Otherwise, the result is `false`.

The result of `!=` is `false` if both `x` and `y` are `true` or if both `x` and `y` are `false`. Otherwise, the result is `true`. When the operands are of type `bool`, the `!=` operator produces the same result as the `^` operator.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined Boolean equality operators defined above are also predefined.

### 11.11.6 Enumeration comparison operators

Every enumeration type implicitly provides the following predefined comparison operators

```csharp
bool operator ==(E x, E y);
bool operator !=(E x, E y);

bool operator <(E x, E y);
bool operator >(E x, E y);
bool operator <=(E x, E y);
bool operator >=(E x, E y);
```

The result of evaluating `x «op» y`, where x and y are expressions of an enumeration type `E` with an underlying type `U`, and «op» is one of the comparison operators, is exactly the same as evaluating `((U)x) «op» ((U)y)`. In other words, the enumeration type comparison operators simply compare the underlying integral values of the two operands.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined enumeration comparison operators defined above are also predefined.

### 11.11.7 Reference type equality operators

Every class type `C` implicitly provides the following predefined reference type equality operators:

``` csharp
bool operator ==(C x, C y);
bool operator !=(C x, C y);
```

unless predefined equality operators otherwise exist for `C` (for example, when `C` is `string` or `System.Delegate`).

The operators return the result of comparing the two references for equality or non-equality. `operator ==` returns `true` if and only if `x` and `y` refer to the same instance or are both `null`, while `operator !=` returns `true` if and only if `operator ==` with the same operands would return `false`.

In addition to normal applicability rules ([§11.6.4.2](expressions.md#11642-applicable-function-member)), the predefined reference type equality operators require one of the following in order to be applicable:

- Both operands are a value of a type known to be a *reference_type* or the literal `null`. Furthermore, an identity or explicit reference conversion ([§10.3.5](conversions.md#1035-explicit-reference-conversions)) exists from either operand to the type of the other operand.
- One operand is the literal `null`, and the other operand is a value of type `T` where `T` is a *type_parameter* that is not known to be a value type, and does not have the value type constraint.
  - If at runtime `T` is a non-nullable value type, the result of `==` is `false` and the result of `!=` is `true`.
  - If at runtime `T` is a nullable value type, the result is computed from the `HasValue` property of the operand, as described in ([§11.11.10](expressions.md#111110-equality-operators-between-nullable-value-types-and-the-null-literal)).
  - If at runtime `T` is a reference type, the result is `true` if the operand is `null`, and `false` otherwise.

Unless one of these conditions is true, a binding-time error occurs.

> *Note*: Notable implications of these rules are:
>
> - It is a binding-time error to use the predefined reference type equality operators to compare two references that are known to be different at binding-time. For example, if the binding-time types of the operands are two class types, and if neither derives from the other, then it would be impossible for the two operands to reference the same object. Thus, the operation is considered a binding-time error.
> - The predefined reference type equality operators do not permit value type operands to be compared (except when type parameters are compared to `null`, which is handled specially).
> - Operands of predefined reference type equality operators are never boxed. It would be meaningless to perform such boxing operations, since references to the newly allocated boxed instances would necessarily differ from all other references.
>
> For an operation of the form `x == y` or `x != y`, if any applicable user-defined `operator ==` or `operator !=` exists, the operator overload resolution rules ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) will select that operator instead of the predefined reference type equality operator. It is always possible to select the predefined reference type equality operator by explicitly casting one or both of the operands to type `object`.
>
> *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The following example checks whether an argument of an unconstrained type parameter type is `null`.
>
> ```csharp
> class C<T>
> {
>    void F(T x)
>    {
>       if (x == null)
>       {
>           throw new ArgumentNullException();
>       }
>       ...
>    }
> }
> ```
>
> The `x == null` construct is permitted even though `T` could represent a non-nullable value type, and the result is simply defined to be `false` when `T` is a non-nullable value type.
>
> *end example*

For an operation of the form `x == y` or `x != y`, if any applicable `operator ==` or `operator !=` exists, the operator overload resolution ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) rules will select that operator instead of the predefined reference type equality operator.

> *Note*: It is always possible to select the predefined reference type equality operator by explicitly casting both of the operands to type `object`. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The example
>
> ```csharp
> using System;
> class Test
> {
>     static void Main()
>     {
>         string s = "Test";
>         string t = string.Copy(s);
>         Console.WriteLine(s == t);
>         Console.WriteLine((object)s == t);
>         Console.WriteLine(s == (object)t);
>         Console.WriteLine((object)s == (object)t);
>     }
> }
> ```
>
> produces the output
>
> ```console
> True
> False
> False
> False
> ```
>
> The `s` and `t` variables refer to two distinct string instances containing the same characters. The first comparison outputs `True` because the predefined string equality operator ([§11.11.8](expressions.md#11118-string-equality-operators)) is selected when both operands are of type `string`. The remaining comparisons all output `False` because the overload of `operator ==` in the `string` type is not applicable when either operand has a binding-time type of `object`.
>
> Note that the above technique is not meaningful for value types. The example
>
> ```csharp
> class Test
> {
>     static void Main()
>     {
>         int i = 123;
>         int j = 123;
>         System.Console.WriteLine((object)i == (object)j);
>     }
> }
> ```
>
> outputs `False` because the casts create references to two separate instances of boxed `int` values.
>
> *end example*

### 11.11.8 String equality operators

The predefined string equality operators are:

```csharp
bool operator ==(string x, string y);
bool operator !=(string x, string y);
```

Two `string` values are considered equal when one of the following is true:

- Both values are `null`.
- Both values are non-`null` references to string instances that have identical lengths and identical characters in each character position.

The string equality operators compare string values rather than string references. When two separate string instances contain the exact same sequence of characters, the values of the strings are equal, but the references are different.

> *Note*: As described in [§11.11.7](expressions.md#11117-reference-type-equality-operators), the reference type equality operators can be used to compare string references instead of string values. *end note*

### 11.11.9 Delegate equality operators

The predefined delegate equality operators are:

```csharp
bool operator ==(System.Delegate x, System.Delegate y);
bool operator !=(System.Delegate x, System.Delegate y);
```

Two delegate instances are considered equal as follows:

- If either of the delegate instances is `null`, they are equal if and only if both are `null`.
- If the delegates have different run-time type, they are never equal.
- If both of the delegate instances have an invocation list ([§19.2](delegates.md#192-delegate-declarations)), those instances are equal if and only if their invocation lists are the same length, and each entry in one’s invocation list is equal (as defined below) to the corresponding entry, in order, in the other’s invocation list.

The following rules govern the equality of invocation list entries:

- If two invocation list entries both refer to the same static method then the entries are equal.
- If two invocation list entries both refer to the same non-static method on the same target object (as defined by the reference equality operators) then the entries are equal.
- Invocation list entries produced from evaluation of semantically identical anonymous functions ([§11.16](expressions.md#1116-anonymous-function-expressions)) with the same (possibly empty) set of captured outer variable instances are permitted (but not required) to be equal.

If operator overload resolution resolves to either delegate equality operator, and the binding-time types of both operands are delegate types as described in [§19](delegates.md#19-delegates) rather than `System.Delegate`, and there is no identity conversion between the binding-type operand types, a binding-time error occurs.

> *Note*: This rule prevents comparisons which can never consider non-`null` values as equal due to being references to instances of different types of delegates. *end note*

### 11.11.10 Equality operators between nullable value types and the null literal

The `==` and `!=` operators permit one operand to be a value of a nullable value type and the other to be the `null` literal, even if no predefined or user-defined operator (in unlifted or lifted form) exists for the operation.

For an operation of one of the forms

```csharp
x == null    null == x    x != null    null != x
```

where `x` is an expression of a nullable value type, if operator overload resolution ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) fails to find an applicable operator, the result is instead computed from the `HasValue` property of `x`. Specifically, the first two forms are translated into `!x.HasValue`, and the last two forms are translated into `x.HasValue`.

### 11.11.11 The is operator

The `is` operator is used to check if the run-time type of an object is compatible with a given type. The check is performed at runtime. The result of the operation `E is T`, where `E` is an expression and `T` is a type other than `dynamic`, is a Boolean value indicating whether `E` is non-null and can successfully be converted to type `T` by a reference conversion, a boxing conversion, an unboxing conversion, a wrapping conversion, or an unwrapping conversion.

The operation is evaluated as follows:

1. If `E` is an anonymous function, a compile-time error occurs
1. If `E` is a method group or the `null` literal, of if the value of `E` is `null`, the result is `false`.
1. Otherwise:
1. Let `R` be the runtime type of `E`.
1. Let `D` be derived from `R` as follows:
1. If `R` is a nullable value type, `D` is the underlying type of `R`.
1. Otherwise, `D` is `R`.
1. The result depends on `D` and `T` as follows:
1. If `T` is a reference type, the result is `true` if:
    - `D` and `T` are the same type,
    - `D` is a reference type and an implicit reference conversion from `D` to `T` exists, or
    - Either: `D` is a value type and a boxing conversion from `D` to `T` exists.  
      Or: `D` is a value type and `T` is an interface type implemented by `D`.
1. If `T` is a nullable value type, the result is `true` if `D` is the underlying type of `T`.
1. If `T` is a non-nullable value type, the result is `true` if `D` and `T` are the same type.
1. Otherwise, the result is `false`.

User defined conversions are not considered by the `is` operator.

> *Note*: As the `is` operator is evaluated at runtime, all type arguments have been substituted and there are no open types ([§8.4.3](types.md#843-open-and-closed-types)) to consider. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The `is` operator can be understood in terms of compile-time types and conversions as follows, where `C` is the compile-time type of `E`:
>
> - If the compile-time type of `e` is the same as `T`, or if an implicit reference conversion ([§10.2.8](conversions.md#1028-implicit-reference-conversions)), boxing conversion ([§10.2.9](conversions.md#1029-boxing-conversions)), wrapping conversion ([§10.6](conversions.md#106-conversions-involving-nullable-types)), or an explicit unwrapping conversion ([§10.6](conversions.md#106-conversions-involving-nullable-types)) exists from the compile-time type of `E` to `T`:
>   - If `C` is of a non-nullable value type, the result of the operation is `true`.
>   - Otherwise, the result of the operation is equivalent to evaluating `E != null`.
> - Otherwise, if an explicit reference conversion ([§10.3.5](conversions.md#1035-explicit-reference-conversions)) or unboxing conversion ([§10.3.6](conversions.md#1036-unboxing-conversions)) exists from `C` to `T`, or if `C` or `T` is an open type ([§8.4.3](types.md#843-open-and-closed-types)), then runtime checks as above must be peformed.
> - Otherwise, no reference, boxing, wrapping, or unwrapping conversion of `E` to type `T` is possible, and the result of the operation is `false`.
> A compiler may implement optimisations based on the compile-time type.
>
> *end note*

### 11.11.12 The as operator

The `as` operator is used to explicitly convert a value to a given reference type or nullable value type. Unlike a cast expression ([§11.8.7](expressions.md#1187-cast-expressions)), the `as` operator never throws an exception. Instead, if the indicated conversion is not possible, the resulting value is `null`.

In an operation of the form `E as T`, `E` shall be an expression and `T` shall be a reference type, a type parameter known to be a reference type, or a nullable value type. Furthermore, at least one of the following shall be true, or otherwise a compile-time error occurs:

- An identity ([§10.2.2](conversions.md#1022-identity-conversion)), implicit nullable ([§10.2.6](conversions.md#1026-implicit-nullable-conversions)), implicit reference ([§10.2.8](conversions.md#1028-implicit-reference-conversions)), boxing ([§10.2.9](conversions.md#1029-boxing-conversions)), explicit nullable ([§10.3.4](conversions.md#1034-explicit-nullable-conversions)), explicit reference ([§10.3.5](conversions.md#1035-explicit-reference-conversions)), or wrapping ([§8.3.11](types.md#8311-nullable-value-types)) conversion exists from `E` to `T`.
- The type of `E` or `T` is an open type.
- `E` is the `null` literal.

If the compile-time type of `E` is not `dynamic`, the operation `E` as `T` produces the same result as

```csharp
E is T ? (T)(E) : (T)null
```

except that `E` is only evaluated once. The compiler can be expected to optimize `E` as `T` to perform at most one runtime type check as opposed to the two runtime type checks implied by the expansion above.

If the compile-time type of `E` is `dynamic`, unlike the cast operator the `a` operator is not dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)). Therefore the expansion in this case is:

```csharp
E is T ? (T)(object)(E) : (T)null
```

Note that some conversions, such as user defined conversions, are not possible with the `as` operator and should instead be performed using cast expressions.

> *Example*: In the example
>
> ```csharp
> class X
> {
>     public string F(object o)
>     {
>         return o as string;  // OK, string is a reference type
>     }
>
>     public T G<T>(object o)
>         where T : Attribute
>     {
>         return o as T;       // Ok, T has a class constraint
>     }
>
>     public U H<U>(object o)
>     {
>         return o as U;       // Error, U is unconstrained
>     }
> }
> ```
>
> the type parameter `T` of `G` is known to be a reference type, because it has the class constraint. The type parameter `U` of `H` is not however; hence the use of the `as` operator in `H` is disallowed.
>
> *end example*

## 11.12 Logical operators

### 11.12.1 General

The `&,` `^`, and `|` operators are called the logical operators.

```ANTLR
and_expression
    : equality_expression
    | and_expression '&' equality_expression
    ;

exclusive_or_expression
    : and_expression
    | exclusive_or_expression '^' and_expression
    ;

inclusive_or_expression
    : exclusive_or_expression
    | inclusive_or_expression '|' exclusive_or_expression
    ;
```

If an operand of a logical operator has the compile-time type `dynamic`, then the expression is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)). In this case the compile-time type of the expression is `dynamic`, and the resolution described below will take place at run-time using the run-time type of those operands that have the compile-time type `dynamic`.

For an operation of the form `x «op» y`, where «op» is one of the logical operators, overload resolution ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) is applied to select a specific operator implementation. The operands are converted to the parameter types of the selected operator, and the type of the result is the return type of the operator.

The predefined logical operators are described in the following subclauses.

### 11.12.2 Integer logical operators

The predefined integer logical operators are:

```csharp
int operator &(int x, int y);
uint operator &(uint x, uint y);
long operator &(long x, long y);
ulong operator &(ulong x, ulong y);

int operator |(int x, int y);
uint operator |(uint x, uint y);
long operator |(long x, long y);
ulong operator |(ulong x, ulong y);

int operator ^(int x, int y);
uint operator ^(uint x, uint y);
long operator ^(long x, long y);
ulong operator ^(ulong x, ulong y);
```

The `&` operator computes the bitwise logical AND of the two operands, the `|` operator computes the bitwise logical OR of the two operands, and the `^` operator computes the bitwise logical exclusive OR of the two operands. No overflows are possible from these operations.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined integer logical operators defined above are also predefined.

### 11.12.3 Enumeration logical operators

Every enumeration type `E` implicitly provides the following predefined logical operators:

```csharp
E operator &(E x, E y);
E operator |(E x, E y);
E operator ^(E x, E y);
```

The result of evaluating `x «op» y`, where `x` and `y` are expressions of an enumeration type `E` with an underlying type `U`, and «op» is one of the logical operators, is exactly the same as evaluating `(E)((U)x «op» (U)y)`. In other words, the enumeration type logical operators simply perform the logical operation on the underlying type of the two operands.

Lifted ([§11.4.8](expressions.md#1148-lifted-operators)) forms of the unlifted predefined enumeration logical operators defined above are also predefined.

### 11.12.4 Boolean logical operators

The predefined Boolean logical operators are:

```csharp
bool operator &(bool x, bool y);
bool operator |(bool x, bool y);
bool operator ^(bool x, bool y);
```

The result of `x & y` is `true` if both `x` and `y` are `true`. Otherwise, the result is `false`.

The result of `x | y` is `true` if either `x` or `y` is `true`. Otherwise, the result is `false`.

The result of `x ^ y` is `true` if `x` is `true` and `y` is `false`, or `x` is `false` and `y` is `true`. Otherwise, the result is `false`. When the operands are of type `bool`, the `^` operator computes the same result as the `!=` operator.

### 11.12.5 Nullable Boolean & and | operators

The nullable Boolean type `bool?` can represent three values, `true`, `false`, and `null`.

As with the other binary operators, lifted forms of the logical operators `&` and `|` ([§11.12.4](expressions.md#11124-boolean-logical-operators)) are also pre-defined:

```csharp
bool? operator &(bool? x, bool? y);
bool? operator |(bool? x, bool? y);
```

The semantics of the lifted `&` and `|` operators are defined by the following table:

**`x`** | **`y`** | **`x & y`** | **`x \| y`**
------- | ------- | ------- | -------
`true`  | `true`  | `true`  | `true`
`true`  | `false` | `false` | `true`
`true`  | `null`  | `null`  | `true`
`false` | `true`  | `false` | `true`
`false` | `false` | `false` | `false`
`false` | `null`  | `false` | `null`
`null`  | `true`  | `null`  | `true`
`null`  | `false` | `false` | `null`
`null`  | `null`  | `null`  | `null`

> *Note*: The `bool?` type is conceptually similar to the three-valued type used for Boolean expressions in SQL. The table above follows the same semantics as SQL, whereas applying the rules of [§11.4.8](expressions.md#1148-lifted-operators) to the `&` and `|` operators would not. The rules of [§11.4.8](expressions.md#1148-lifted-operators) already provide SQL-like semantics for the lifted `^` operator. *end note*

## 11.13 Conditional logical operators

### 11.13.1 General

The `&&` and `||` operators are called the conditional logical operators. They are also called the “short-circuiting” logical operators.

```ANTLR
conditional_and_expression
    : inclusive_or_expression
    | conditional_and_expression '&&' inclusive_or_expression
    ;

conditional_or_expression
    : conditional_and_expression
    | conditional_or_expression '||' conditional_and_expression
    ;
```

The `&&` and `||` operators are conditional versions of the `&` and `|` operators:

- The operation `x && y` corresponds to the operation `x & y`, except that `y` is evaluated only if `x` is not `false`.
- The operation `x || y` corresponds to the operation `x | y`, except that `y` is evaluated only if `x` is not `true`.

> *Note*: The reason that short circuiting uses the ‘not true’ and ‘not false’ conditions is to enable user-defined conditional operators to define when short circuiting applies. User-defined types could be in a state where `operator true` returns `false` and `operator false` returns `false`. In those cases, neither `&&` nor `||` would short circuit. *end note*

If an operand of a conditional logical operator has the compile-time type `dynamic`, then the expression is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)). In this case the compile-time type of the expression is `dynamic`, and the resolution described below will take place at run-time using the run-time type of those operands that have the compile-time type `dynamic`.

An operation of the form `x && y` or `x || y` is processed by applying overload resolution ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) as if the operation was written `x & y` or `x | y`. Then,

- If overload resolution fails to find a single best operator, or if overload resolution selects one of the predefined integer logical operators or nullable Boolean logical operators ([§11.12.5](expressions.md#11125-nullable-boolean--and--operators)), a binding-time error occurs.
- Otherwise, if the selected operator is one of the predefined Boolean logical operators ([§11.12.4](expressions.md#11124-boolean-logical-operators)), the operation is processed as described in [§11.13.2](expressions.md#11132-boolean-conditional-logical-operators).
- Otherwise, the selected operator is a user-defined operator, and the operation is processed as described in [§11.13.3](expressions.md#11133-user-defined-conditional-logical-operators).

It is not possible to directly overload the conditional logical operators. However, because the conditional logical operators are evaluated in terms of the regular logical operators, overloads of the regular logical operators are, with certain restrictions, also considered overloads of the conditional logical operators. This is described further in [§11.13.3](expressions.md#11133-user-defined-conditional-logical-operators).

### 11.13.2 Boolean conditional logical operators

When the operands of `&&` or `||` are of type `bool`, or when the operands are of types that do not define an applicable `operator &` or `operator |`, but do define implicit conversions to `bool`, the operation is processed as follows:

- The operation `x && y` is evaluated as `x ? y : false`. In other words, `x` is first evaluated and converted to type `bool`. Then, if `x` is `true`, `y` is evaluated and converted to type `bool`, and this becomes the result of the operation. Otherwise, the result of the operation is `false`.
- The operation `x || y` is evaluated as `x ? true : y`. In other words, `x` is first evaluated and converted to type `bool`. Then, if `x` is `true`, the result of the operation is `true`. Otherwise, `y` is evaluated and converted to type `bool`, and this becomes the result of the operation.

### 11.13.3 User-defined conditional logical operators

When the operands of `&&` or `||` are of types that declare an applicable user-defined `operator &` or `operator |`, both of the following shall be true, where `T` is the type in which the selected operator is declared:

- The return type and the type of each parameter of the selected operator shall be `T`. In other words, the operator shall compute the logical AND or the logical OR of two operands of type `T`, and shall return a result of type `T`.
- `T` shall contain declarations of `operator true` and `operator false`.

A binding-time error occurs if either of these requirements is not satisfied. Otherwise, the `&&` or `||` operation is evaluated by combining the user-defined `operator true` or `operator false` with the selected user-defined operator:

- The operation `x && y` is evaluated as `T.false(x) ? x : T.&(x, y)`, where `T.false(x)` is an invocation of the `operator false` declared in `T`, and `T.&(x, y)` is an invocation of the selected `operator &`. In other words, `x` is first evaluated and `operator false` is invoked on the result to determine if `x` is definitely false. Then, if `x` is definitely false, the result of the operation is the value previously computed for `x`. Otherwise, `y` is evaluated, and the selected `operator &` is invoked on the value previously computed for `x` and the value computed for `y` to produce the result of the operation.
- The operation `x || y` is evaluated as `T.true(x) ? x : T.|(x, y)`, where `T.true(x)` is an invocation of the `operator true` declared in `T`, and `T.|(x, y)` is an invocation of the selected `operator |`. In other words, `x` is first evaluated and `operator true` is invoked on the result to determine if `x` is definitely true. Then, if `x` is definitely true, the result of the operation is the value previously computed for `x`. Otherwise, `y` is evaluated, and the selected `operator |` is invoked on the value previously computed for `x` and the value computed for `y` to produce the result of the operation.

In either of these operations, the expression given by `x` is only evaluated once, and the expression given by `y` is either not evaluated or evaluated exactly once.

## 11.14 The null coalescing operator

The `??` operator is called the null coalescing operator.

```ANTLR
null_coalescing_expression
    : conditional_or_expression
    | conditional_or_expression '??' null_coalescing_expression
    ;
```

In a null coalescing expression of the form `a ?? b`, if `a` is non-`null`, the result is `a`; otherwise, the result is `b`. The operation evaluates `b` only if `a` is `null`.

The null coalescing operator is right-associative, meaning that operations are grouped from right to left.

> *Example*: An expression of the form `a ?? b ?? c` is evaluated as a `?? (b ?? c)`. In general terms, an expression of the form `E1 ?? E2 ?? ... ?? EN` returns the first of the operands that is non-`null`, or `null` if all operands are `null`. *end example*

The type of the expression `a ?? b` depends on which implicit conversions are available on the operands. In order of preference, the type of `a ?? b` is `A₀`, `A`, or `B`, where `A` is the type of `a` (provided that `a` has a type), `B` is the type of `b`(provided that `b` has a type), and `A₀` is the underlying type of `A` if `A` is a nullable value type, or `A` otherwise. Specifically, `a ?? b` is processed as follows:

- If `A` exists and is not a nullable value type or a reference type, a compile-time error occurs.
- Otherwise, if `A` exists and `b` is a dynamic expression, the result type is `dynamic`. At run-time, `a` is first evaluated. If `a` is not `null`, `a` is converted to `dynamic`, and this becomes the result. Otherwise, `b` is evaluated, and this becomes the result.
- Otherwise, if `A` exists and is a nullable value type and an implicit conversion exists from `b` to `A₀`, the result type is `A₀`. At run-time, `a` is first evaluated. If `a` is not `null`, `a` is unwrapped to type `A₀`, and this becomes the result. Otherwise, `b` is evaluated and converted to type `A₀`, and this becomes the result.
- Otherwise, if `A` exists and an implicit conversion exists from `b` to `A`, the result type is `A`. At run-time, a is first evaluated. If a is not null, a becomes the result. Otherwise, `b` is evaluated and converted to type `A`, and this becomes the result.
- Otherwise, if `A` exists and is a nullable value type, `b` has a type `B` and an implicit conversion exists from `A₀` to `B`, the result type is `B`. At run-time, `a` is first evaluated. If `a` is not `null`, `a` is unwrapped to type `A₀` and converted to type `B`, and this becomes the result. Otherwise, `b` is evaluated and becomes the result.
- Otherwise, if `b` has a type `B` and an implicit conversion exists from `a` to `B`, the result type is `B`. At run-time, `a` is first evaluated. If `a` is not `null`, `a` is converted to type `B`, and this becomes the result. Otherwise, `b` is evaluated and becomes the result.

Otherwise, `a` and `b` are incompatible, and `a` compile-time error occurs.

## 11.15 Conditional operator

The `?:` operator is called the conditional operator. It is at times also called the ternary operator.

```ANTLR
conditional_expression
    : null_coalescing_expression
    | null_coalescing_expression '?' expression ':' expression
    ;
```

A conditional expression of the form `b ? x : y` first evaluates the condition `b`. Then, if `b` is `true`, `x` is evaluated and becomes the result of the operation. Otherwise, `y` is evaluated and becomes the result of the operation. A conditional expression never evaluates both `x` and `y`.

The conditional operator is right-associative, meaning that operations are grouped from right to left.

> *Example*: An expression of the form `a ? b : c ? d : e` is evaluated as `a ? b : (c ? d : e)`. *end example*

The first operand of the `?:` operator shall be an expression that can be implicitly converted to `bool`, or an expression of a type that implements `operator true`. If neither of these requirements is satisfied, a compile-time error occurs.

The second and third operands, `x` and `y`, of the `?:` operator control the type of the conditional expression.

- If `x` has type `X` and `y` has type `Y` then,
  - If `X` and `Y` are the same type, then this is the type of the conditional expression.
  - Otherwise, if an implicit conversion ([§10.2](conversions.md#102-implicit-conversions)) exists from `X` to `Y`, but not from `Y` to `X`, then `Y` is the type of the conditional expression.
  - Otherwise, if an implicit enumeration conversion ([§10.2.4](conversions.md#1024-implicit-enumeration-conversions)) exists from `X` to `Y`, then `Y` is the type of the conditional expression.
  - Otherwise, if an implicit enumeration conversion ([§10.2.4](conversions.md#1024-implicit-enumeration-conversions)) exists from `Y` to `X`, then `X` is the type of the conditional expression.
  - Otherwise, if an implicit conversion ([§10.2](conversions.md#102-implicit-conversions)) exists from `Y` to `X`, but not from `X` to `Y`, then `X` is the type of the conditional expression.
  - Otherwise, no expression type can be determined, and a compile-time error occurs.
- If only one of `x` and `y` has a type, and both `x` and `y` are implicitly convertible to that type, then that is the type of the conditional expression.
- Otherwise, no expression type can be determined, and a compile-time error occurs.

The run-time processing of a conditional expression of the form `b ? x : y` consists of the following steps:

- First, `b` is evaluated, and the `bool` value of `b` is determined:
  - If an implicit conversion from the type of `b` to `bool` exists, then this implicit conversion is performed to produce a `bool` value.
  - Otherwise, the `operator true` defined by the type of `b` is invoked to produce a `bool` value.
- If the `bool` value produced by the step above is `true`, then `x` is evaluated and converted to the type of the conditional expression, and this becomes the result of the conditional expression.
- Otherwise, `y` is evaluated and converted to the type of the conditional expression, and this becomes the result of the conditional expression.

## 11.16 Anonymous function expressions

### 11.16.1 General

An ***anonymous function*** is an expression that represents an “in-line” method definition. An anonymous function does not have a value or type in and of itself, but is convertible to a compatible delegate or expression-tree type. The evaluation of an anonymous-function conversion depends on the target type of the conversion: If it is a delegate type, the conversion evaluates to a delegate value referencing the method that the anonymous function defines. If it is an expression-tree type, the conversion evaluates to an expression tree that represents the structure of the method as an object structure.

> *Note*: For historical reasons, there are two syntactic flavors of anonymous functions, namely *lambda_expression*s and *anonymous_method_expression*s. For almost all purposes, *lambda_expression*s are more concise and expressive than *anonymous_method_expression*s, which remain in the language for backwards compatibility.

```ANTLR
lambda_expression
    : 'async'? anonymous_function_signature '=>' anonymous_function_body
    ;

anonymous_method_expression
    : 'async'? 'delegate' explicit_anonymous_function_signature? block
    ;

anonymous_function_signature
    : explicit_anonymous_function_signature
    | implicit_anonymous_function_signature
    ;

explicit_anonymous_function_signature
    : '(' explicit_anonymous_function_parameter_list? ')'
    ;

explicit_anonymous_function_parameter_list
    : explicit_anonymous_function_parameter
      (',' explicit_anonymous_function_parameter)*
    ;

explicit_anonymous_function_parameter
    : anonymous_function_parameter_modifier? type identifier
    ;

anonymous_function_parameter_modifier
    : 'ref'
    | 'out'
    ;

implicit_anonymous_function_signature
    : '(' implicit_anonymous_function_parameter_list? ')'
    | implicit_anonymous_function_parameter
    ;

implicit_anonymous_function_parameter_list
    : implicit_anonymous_function_parameter
      (',' implicit_anonymous_function_parameter)*
    ;

implicit_anonymous_function_parameter
    : identifier
    ;

anonymous_function_body
    : null_conditional_invocation_expression
    | expression
    | block
    ;
```

When recognising an *anonymous_function_body* if both the *null_conditional_invocation_expression* and *expression* alternatives are applicable then the former shall be chosen.

> *Note*: The overlapping of, and priority between, alternatives here is solely for descriptive convenience; the grammar rules could be elaborated to remove the overlap. ANTLR, and other grammar systems, adopt the same convenience and so *anonymous_function_body* has the specified semantics automatically.

The `=>` operator has the same precedence as assignment (`=`) and is right-associative.

An anonymous function with the `async` modifier is an async function and follows the rules described in [§14.15](classes.md#1415-async-functions).

The parameters of an anonymous function in the form of a *lambda_expression* can be explicitly or implicitly typed. In an explicitly typed parameter list, the type of each parameter is explicitly stated. In an implicitly typed parameter list, the types of the parameters are inferred from the context in which the anonymous function occurs—specifically, when the anonymous function is converted to a compatible delegate type or expression tree type, that type provides the parameter types ([§10.7](conversions.md#107-anonymous-function-conversions)).

In a *lambda_expression* with a single, implicitly typed parameter, the parentheses may be omitted from the parameter list. In other words, an anonymous function of the form

```csharp
( «param» ) => «expr»
```

can be abbreviated to

```csharp
«param» => «expr»
```

The parameter list of an anonymous function in the form of an *anonymous_method_expression* is optional. If given, the parameters shall be explicitly typed. If not, the anonymous function is convertible to a delegate with any parameter list not containing out parameters.

A *block* body of an anonymous function is always reachable ([§12.2](statements.md#122-end-points-and-reachability)).

> *Example*: Some examples of anonymous functions follow below:
>
> ```csharp
> x => x + 1                             // Implicitly typed, expression body
> x => { return x + 1; }                 // Implicitly typed, block body
> (int x) => x + 1                       // Explicitly typed, expression body
> (int x) => { return x + 1; }           // Explicitly typed, block body
> (x, y) => x * y                        // Multiple parameters
> () => Console.WriteLine()              // No parameters
> async (t1,t2) => await t1 + await t2   // Async
> delegate (int x) { return x + 1; }     // Anonymous method expression
> delegate { return 1 + 1; }             // Parameter list omitted
> ```
>
> *end example*

The behavior of *lambda_expression*s and *anonymous_method_expression*s is the same except for the following points:

- *anonymous_method_expression*s permit the parameter list to be omitted entirely, yielding convertibility to delegate types of any list of value parameters.
- *lambda_expression*s permit parameter types to be omitted and inferred whereas *anonymous_method_expression*s require parameter types to be explicitly stated.
- The body of a *lambda_expression* can be an expression or a block whereas the body of an *anonymous_method_expression* shall be a block.
- Only *lambda_expression*s have conversions to compatible expression tree types ([§8.6](types.md#86-expression-tree-types)).

### 11.16.2 Anonymous function signatures

The *anonymous_function_signature* of an anonymous function defines the names and optionally the types of the formal parameters for the anonymous function. The scope of the parameters of the anonymous function is the *anonymous_function_body* ([§7.7](basic-concepts.md#77-scopes)). Together with the parameter list (if given) the anonymous-method-body constitutes a declaration space ([§7.3](basic-concepts.md#73-declarations)). It is thus a compile-time error for the name of a parameter of the anonymous function to match the name of a local variable, local constant or parameter whose scope includes the *anonymous_method_expression* or *lambda_expression*.

If an anonymous function has an *explicit_anonymous_function_signature*, then the set of compatible delegate types and expression tree types is restricted to those that have the same parameter types and modifiers in the same order ([§10.7](conversions.md#107-anonymous-function-conversions)). In contrast to method group conversions ([§10.8](conversions.md#108-method-group-conversions)), contra-variance of anonymous function parameter types is not supported. If an anonymous function does not have an *anonymous_function_signature*, then the set of compatible delegate types and expression tree types is restricted to those that have no out parameters.

Note that an *anonymous_function_signature* cannot include attributes or a parameter array. Nevertheless, an *anonymous_function_signature* may be compatible with a delegate type whose parameter list contains a parameter array.

Note also that conversion to an expression tree type, even if compatible, may still fail at compile-time ([§8.6](types.md#86-expression-tree-types)).

### 11.16.3 Anonymous function bodies

The body (*expression* or *block*) of an anonymous function is subject to the following rules:

- If the anonymous function includes a signature, the parameters specified in the signature are available in the body. If the anonymous function has no signature it can be converted to a delegate type or expression type having parameters ([§10.7](conversions.md#107-anonymous-function-conversions)), but the parameters cannot be accessed in the body.
- Except for `ref` or `out` parameters specified in the signature (if any) of the nearest enclosing anonymous function, it is a compile-time error for the body to access a `ref` or `out` parameter.
- When the type of `this` is a struct type, it is a compile-time error for the body to access `this`. This is true whether the access is explicit (as in `this.x`) or implicit (as in `x` where `x` is an instance member of the struct). This rule simply prohibits such access and does not affect whether member lookup results in a member of the struct.
- The body has access to the outer variables ([§11.16.6](expressions.md#11166-outer-variables)) of the anonymous function. Access of an outer variable will reference the instance of the variable that is active at the time the *lambda_expression* or *anonymous_method_expression* is evaluated ([§11.16.7](expressions.md#11167-evaluation-of-anonymous-function-expressions)).
- It is a compile-time error for the body to contain a `goto` statement, a `break` statement, or a `continue` statement whose target is outside the body or within the body of a contained anonymous function.
- A `return` statement in the body returns control from an invocation of the nearest enclosing anonymous function, not from the enclosing function member.

It is explicitly unspecified whether there is any way to execute the block of an anonymous function other than through evaluation and invocation of the *lambda_expression* or *anonymous_method_expression*. In particular, the compiler may choose to implement an anonymous function by synthesizing one or more named methods or types. The names of any such synthesized elements shall be of a form reserved for compiler use ([§6.4.3](lexical-structure.md#643-identifiers)).

### 11.16.4 Overload resolution

Anonymous functions in an argument list participate in type inference and overload resolution. Refer to [§11.6.3](expressions.md#1163-type-inference) and [§11.6.4](expressions.md#1164-overload-resolution) for the exact rules.

> *Example*: The following example illustrates the effect of anonymous functions on overload resolution.
>
> ```csharp
> class ItemList<T> : List<T>
> {
>     public int Sum(Func<T, int> selector)
>     {
>         int sum = 0;
>         foreach (T item in this)
>         {
>             sum += selector(item);
>         }
>         return sum;
>     }
>
>     public double Sum(Func<T, double> selector)
>     {
>         double sum = 0;
>         foreach (T item in this)
>         {
>             sum += selector(item);
>         }
>         return sum;
>     }
> }
> ```
>
> The `ItemList<T>` class has two `Sum` methods. Each takes a `selector` argument, which extracts the value to sum over from a list item. The extracted value can be either an `int` or a `double` and the resulting sum is likewise either an `int` or a `double`.
>
> The `Sum` methods could for example be used to compute sums from a list of detail lines in an order.
>
> ```csharp
> class Detail
> {
>     public int UnitCount;
>     public double UnitPrice;
>     ...
> }
>
> void ComputeSums()
> {
>     ItemList<Detail> orderDetails = GetOrderDetails(...);
>     int totalUnits = orderDetails.Sum(d => d.UnitCount);
>     double orderTotal = orderDetails.Sum(d => d.UnitPrice * d.UnitCount);
>     ...
> }
> ```
>
> In the first invocation of `orderDetails.Sum`, both `Sum` methods are applicable because the anonymous function `d => d.UnitCount` is compatible with both `Func<Detail,int>` and `Func<Detail,double>`. However, overload resolution picks the first `Sum` method because the conversion to `Func<Detail,int>` is better than the conversion to `Func<Detail,double>`.
>
> In the second invocation of `orderDetails.Sum`, only the second `Sum` method is applicable because the anonymous function `d => d.UnitPrice * d.UnitCount` produces a value of type `double`. Thus, overload resolution picks the second `Sum` method for that invocation.
>
> *end example*

### 11.16.5 Anonymous functions and dynamic binding

An anonymous function cannot be a receiver, argument, or operand of a dynamically bound operation.

### 11.16.6 Outer variables

#### 11.16.6.1 General

Any local variable, value parameter, or parameter array whose scope includes the *lambda_expression* or *anonymous_method_expression* is called an ***outer variable*** of the anonymous function. In an instance function member of a class, the this value is considered a value parameter and is an outer variable of any anonymous function contained within the function member.

#### 11.16.6.2 Captured outer variables

When an outer variable is referenced by an anonymous function, the outer variable is said to have been ***captured*** by the anonymous function. Ordinarily, the lifetime of a local variable is limited to execution of the block or statement with which it is associated ([§9.2.8](variables.md#928-local-variables)). However, the lifetime of a captured outer variable is extended at least until the delegate or expression tree created from the anonymous function becomes eligible for garbage collection.

> *Example*: In the example
>
> ```csharp
> using System;
>
> delegate int D();
>
> class Test
> {
>     static D F()
>     {
>         int x = 0;
>         D result = () => ++x;
>         return result;
>     }
>
>     static void Main()
>     {
>         D d = F();
>         Console.WriteLine(d());
>         Console.WriteLine(d());
>         Console.WriteLine(d());
>     }
> }
> ```
>
> the local variable `x` is captured by the anonymous function, and the lifetime of `x` is extended at least until the delegate returned from `F` becomes eligible for garbage collection. Since each invocation of the anonymous function operates on the same instance of `x`, the output of the example is:
>
> ```csharp
> 1
> 2
> 3
> ```
>
> *end example*

When a local variable or a value parameter is captured by an anonymous function, the local variable or parameter is no longer considered to be a fixed variable ([§22.4](unsafe-code.md#224-fixed-and-moveable-variables)), but is instead considered to be a moveable variable. However, captured outer variables cannot be used in a `fixed` statement ([§22.7](unsafe-code.md#227-the-fixed-statement)), so the address of a captured outer variable cannot be taken.

> *Note*: Unlike an uncaptured variable, a captured local variable can be simultaneously exposed to multiple threads of execution. *end note*

#### 11.16.6.3 Instantiation of local variables

A local variable is considered to be ***instantiated*** when execution enters the scope of the variable.

> *Example*: For example, when the following method is invoked, the local variable `x` is instantiated and initialized three times—once for each iteration of the loop.
>
> ```csharp
> static void F()
> {
>     for (int i = 0; i < 3; i++)
>     {
>         int x = i * 2 + 1;
>         ...
>     }
> }
> ```
>
> However, moving the declaration of `x` outside the loop results in a single instantiation of `x`:
>
> ```csharp
> static void F()
> {
>     int x;
>     for (int i = 0; i < 3; i++)
>     {
>         x = i * 2 + 1;
>         ...
>     }
> }
> ```
>
> *end example*

When not captured, there is no way to observe exactly how often a local variable is instantiated—because the lifetimes of the instantiations are disjoint, it is possible for each instantation to simply use the same storage location. However, when an anonymous function captures a local variable, the effects of instantiation become apparent.

> *Example*: The example
>
> ```csharp
> using System;
> delegate void D();
> class Test
> {
>     static D[] F()
>     {
>         D[] result = new D[3];
>         for (int i = 0; i < 3; i++)
>         {
>             int x = i * 2 + 1;
>             result[i] = () => Console.WriteLine(x);
>         }
>         return result;
>     }
>
>     static void Main()
>     {
>         foreach (D d in F())
>         {
>             d();
>         }
>     }
> }
> ```
>
> produces the output:
>
> ```console
> 1
> 3
> 5
> ```
>
> However, when the declaration of `x` is moved outside the loop:
>
> ```csharp
> static D[] F()
> {
>     D[] result = new D[3];
>     int x;
>     for (int i = 0; i < 3; i++)
>     {
>         x = i * 2 + 1;
>         result[i] = () => Console.WriteLine(x);
>     }
>     return result;
> }
> ```
>
> the output is:
>
> ```console
> 5
> 5
> 5
> ```
>
> Note that the compiler is permitted (but not required) to optimize the three instantiations into a single delegate instance ([§10.7.2](conversions.md#1072-evaluation-of-anonymous-function-conversions-to-delegate-types)).
>
> *end example*

If a for-loop declares an iteration variable, that variable itself is considered to be declared outside of the loop.

> *Example*: Thus, if the example is changed to capture the iteration variable itself:
>
> ```csharp
> static D[] F()
> {
>     D[] result = new D[3];
>     for (int i = 0; i < 3; i++)
>     {
>         result[i] = () => Console.WriteLine(i);
>     }
>     return result;
> }
> ```
>
> only one instance of the iteration variable is captured, which produces the output:
>
> ```console
> 3
> 3
> 3
> ```
>
> *end example*

It is possible for anonymous function delegates to share some captured variables yet have separate instances of others.

> *Example*: For example, if `F` is changed to
>
> ```csharp
> static D[] F()
> {
>     D[] result = new D[3];
>     int x = 0;
>     for (int i = 0; i < 3; i++)
>     {
>         int y = 0;
>         result[i] = () => Console.WriteLine($"{++x} {++y}");
>     }
>     return result;
> }
> ```
>
> the three delegates capture the same instance of `x` but separate instances of `y`, and the output is:
>
> ```console
> 1 1
> 2 1
> 3 1
> ```
>
> *end example*

Separate anonymous functions can capture the same instance of an outer variable.

> *Example*: In the example:
>
> ```csharp
> using System;
>
> delegate void Setter(int value);
> delegate int Getter();
>
> class Test
> {
>     static void Main()
>     {
>         int x = 0;
>         Setter s = (int value) => x = value;
>         Getter g = () => x;
>         s(5);
>         Console.WriteLine(g());
>         s(10);
>         Console.WriteLine(g());
>     }
> }
> ```
>
> the two anonymous functions capture the same instance of the local variable `x`, and they can thus “communicate” through that variable. The output of the example is:
>
> ```console
> 5
> 10
> ```
>
> *end example*

### 11.16.7 Evaluation of anonymous function expressions

An anonymous function `F` shall always be converted to a delegate type `D` or an expression-tree type `E`, either directly or through the execution of a delegate creation expression `new D(F)`. This conversion determines the result of the anonymous function, as described in [§10.7](conversions.md#107-anonymous-function-conversions).

### 11.16.8 Implementation Example

**This subclause is informative.**

This subclause describes a possible implementation of anonymous function conversions in terms of other C# constructs. The implementation described here is based on the same principles used by a commercial C# compiler, but it is by no means a mandated implementation, nor is it the only one possible. It only briefly mentions conversions to expression trees, as their exact semantics are outside the scope of this specification.

The remainder of this subclause gives several examples of code that contains anonymous functions with different characteristics. For each example, a corresponding translation to code that uses only other C# constructs is provided. In the examples, the identifier `D` is assumed by represent the following delegate type:

```csharp
public delegate void D();
```

The simplest form of an anonymous function is one that captures no outer variables:

```csharp
class Test
{
    static void F()
    {
        D d = () => Console.WriteLine("test");
    }
}
```

This can be translated to a delegate instantiation that references a compiler generated static method in which the code of the anonymous function is placed:

```csharp
class Test
{
    static void F()
    {
        D d = new D(__Method1);
    }

    static void __Method1()
    {
        Console.WriteLine("test");
    }
}
```

In the following example, the anonymous function references instance members of `this`:

```csharp
class Test
{
    int x;

    void F()
    {
        D d = () => Console.WriteLine(x);
    }
}
```

This can be translated to a compiler generated instance method containing the code of the anonymous function:

```csharp
class Test
{
   int x;

   void F()
   {
       D d = new D(__Method1);
   }

   void __Method1()
   {
       Console.WriteLine(x);
   }
}
```

In this example, the anonymous function captures a local variable:

```csharp
class Test
{
    void F()
    {
        int y = 123;
        D d = () => Console.WriteLine(y);
    }
}
```

The lifetime of the local variable must now be extended to at least the lifetime of the anonymous function delegate. This can be achieved by “hoisting” the local variable into a field of a compiler-generated class. Instantiation of the local variable ([§11.16.6.3](expressions.md#111663-instantiation-of-local-variables)) then corresponds to creating an instance of the compiler generated class, and accessing the local variable corresponds to accessing a field in the instance of the compiler generated class. Furthermore, the anonymous function becomes an instance method of the compiler-generated class:

```csharp
class Test
{
    void F()
    {
        __Locals1 __locals1 = new __Locals1();
        __locals1.y = 123;
        D d = new D(__locals1.__Method1);
    }

    class __Locals1
    {
        public int y;

        public void __Method1()
        {
            Console.WriteLine(y);
        }
    }
}
```

Finally, the following anonymous function captures `this` as well as two local variables with different lifetimes:

```csharp
class Test
{
   int x;

   void F()
   {
       int y = 123;
       for (int i = 0; i < 10; i++)
       {
           int z = i * 2;
           D d = () => Console.WriteLine(x + y + z);
       }
   }
}
```

Here, a compiler-generated class is created for each block in which locals are captured such that the locals in the different blocks can have independent lifetimes. An instance of `__Locals2`, the compiler generated class for the inner block, contains the local variable `z` and a field that references an instance of `__Locals1`. An instance of `__Locals1`, the compiler generated class for the outer block, contains the local variable `y` and a field that references `this` of the enclosing function member. With these data structures, it is possible to reach all captured outer variables through an instance of `__Local2`, and the code of the anonymous function can thus be implemented as an instance method of that class.

```csharp
class Test
{
    void F()
    {
        __Locals1 __locals1 = new __Locals1();
        __locals1.__this = this;
        __locals1.y = 123;
        for (int i = 0; i < 10; i++)
        {
            __Locals2 __locals2 = new __Locals2();
            __locals2.__locals1 = __locals1;
            __locals2.z = i * 2;
            D d = new D(__locals2.__Method1);
        }
    }

    class __Locals1
    {
        public Test __this;
        public int y;
    }

    class __Locals2
    {
        public __Locals1 __locals1;
        public int z;

        public void __Method1()
        {
            Console.WriteLine(__locals1.__this.x + __locals1.y + z);
        }
    }
}
```

The same technique applied here to capture local variables can also be used when converting anonymous functions to expression trees: references to the compiler-generated objects can be stored in the expression tree, and access to the local variables can be represented as field accesses on these objects. The advantage of this approach is that it allows the “lifted” local variables to be shared between delegates and expression trees.

**End of informative text.**

## 11.17 Query expressions

### 11.17.1 General

***Query expressions*** provide a language-integrated syntax for queries that is similar to relational and hierarchical query languages such as SQL and XQuery.

```ANTLR
query_expression
    : from_clause query_body
    ;

from_clause
    : 'from' type? identifier 'in' expression
    ;

query_body
    : query_body_clauses? select_or_group_clause query_continuation?
    ;

query_body_clauses
    : query_body_clause
    | query_body_clauses query_body_clause
    ;

query_body_clause
    : from_clause
    | let_clause
    | where_clause
    | join_clause
    | join_into_clause
    | orderby_clause
    ;

let_clause
    : 'let' identifier '=' expression
    ;

where_clause
    : 'where' boolean_expression
    ;

join_clause
    : 'join' type? identifier 'in' expression 'on' expression
      'equals' expression
    ;

join_into_clause
    : 'join' type? identifier 'in' expression 'on' expression
      'equals' expression 'into' identifier
    ;

orderby_clause
    : 'orderby' orderings
    ;

orderings
    : ordering (',' ordering)*
    ;

ordering
    : expression ordering_direction?
    ;

ordering_direction
    : 'ascending'
    | 'descending'
    ;

select_or_group_clause
    : select_clause
    | group_clause
    ;

select_clause
    : 'select' expression
    ;

group_clause
    : 'group' expression 'by' expression
    ;

query_continuation
    : 'into' identifier query_body
    ;
```

A query expression begins with a `from` clause and ends with either a `select` or `group` clause. The initial `from` clause may be followed by zero or more `from`, `let`, `where`, `join` or `orderby` clauses. Each `from` clause is a generator introducing a ***range variable*** that ranges over the elements of a ***sequence***. Each `let` clause introduces a range variable representing a value computed by means of previous range variables. Each `where` clause is a filter that excludes items from the result. Each `join` clause compares specified keys of the source sequence with keys of another sequence, yielding matching pairs. Each `orderby` clause reorders items according to specified criteria.The final `select` or `group` clause specifies the shape of the result in terms of the range variables. Finally, an `into` clause can be used to “splice” queries by treating the results of one query as a generator in a subsequent query.

### 11.17.2 Ambiguities in query expressions

Query expressions use a number of contextual keywords ([§6.4.4](lexical-structure.md#644-keywords)): `ascending`, `by`, `descending`, `equals`, `from`, `group`, `into`, `join`, `let`, `on`, `orderby`, `select` and `where`.

To avoid ambiguities that could arise from the use of these identifiers both as keywords and simple names these identifiers are considered keywords anywhere within a query expression, unless they are prefixed with “`@`” ([§6.4.4](lexical-structure.md#644-keywords)) in which case they are considered identifiers. For this purpose, a query expression is any expression that starts with “`from` *identifier*” followed by any token except “`;`”, “`=`” or “`,`”.

### 11.17.3 Query expression translation

#### 11.17.3.1 General

The C# language does not specify the execution semantics of query expressions. Rather, query expressions are translated into invocations of methods that adhere to the query-expression pattern ([§11.17.4](expressions.md#11174-the-query-expression-pattern)). Specifically, query expressions are translated into invocations of methods named `Where`, `Select`, `SelectMany`, `Join`, `GroupJoin`, `OrderBy`, `OrderByDescending`, `ThenBy`, `ThenByDescending`, `GroupBy`, and `Cast`. These methods are expected to have particular signatures and return types, as described in [§11.17.4](expressions.md#11174-the-query-expression-pattern). These methods may be instance methods of the object being queried or extension methods that are external to the object. These methods implement the actual execution of the query.

The translation from query expressions to method invocations is a syntactic mapping that occurs before any type binding or overload resolution has been performed. Following translation of query expressions, the resulting method invocations are processed as regular method invocations, and this may in turn uncover compile time errors. These error conditions include, but are not limited to, methods that do not exist, arguments of the wrong types, and generic methods where type inference fails.

A query expression is processed by repeatedly applying the following translations until no further reductions are possible. The translations are listed in order of application: each section assumes that the translations in the preceding sections have been performed exhaustively, and once exhausted, a section will not later be revisited in the processing of the same query expression.

It is a compile time error for a query expression to include an assignment to a range variable, or the use of a range variable as an argument for a `ref` or `out` parameter.

Certain translations inject range variables with *transparent identifiers* denoted by \*. These are described further in [§11.17.3.8](expressions.md#111738-transparent-identifiers).

#### 11.17.3.2 select and group … by clauses with continuations

A query expression with a group clause using a property `Prop` of `y` and a query body `Q` containing a continuation in the form:

```csharp
from «y» in S group «y» by «y».Prop into «x» Q
```

is translated into:

```csharp
from «x» in ( from «y» in S group «y» by «y».Prop ) Q
```

The translations in the following sections assume that queries have no into continuations.

> *Example*: The example:
>
> ```csharp
> from c in customers
> group c by c.Country into g
> select new { Country = g.Key, CustCount = g.Count() }
> ```
>
> is translated into:
>
> ```csharp
> from g in
>    (from c in customers
>    group c by c.Country)
> select new { Country = g.Key, CustCount = g.Count() }
> ```
>
> the final translation of which is:
>
> ```csharp
> customers.
> GroupBy(c => c.Country).
> Select(g => new { Country = g.Key, CustCount = g.Count() })
> ```
>
> *end example*

#### 11.17.3.3 Explicit range variable types

A `from` clause that explicitly specifies a range variable type

```csharp
from «T» «x» in «e»
```

is translated into

```csharp
from «x» in ( «e» ) . Cast < «T» > ( )
```

A `join` clause that explicitly specifies a range variable type

```csharp
join «T» «x» in «e» on «k1» equals «k2»
```

is translated into

```csharp
join «x» in ( «e» ) . Cast < «T» > ( ) on «k1» equals «k2»
```

The translations in the following sections assume that queries have no explicit range variable types.

> *Example*: The example
>
> ```csharp
> from Customer c in customers
> where c.City == "London"
> select c
> ```
>
> is translated into
>
> ```csharp
> from c in (customers).Cast<Customer>()
> where c.City == "London"
> select c
> ```
>
> the final translation of which is
>
> ```csharp
> customers.
> Cast<Customer>().
> Where(c => c.City == "London")
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: Explicit range variable types are useful for querying collections that implement the non-generic `IEnumerable` interface, but not the generic `IEnumerable<T>` interface. In the example above, this would be the case if customers were of type `ArrayList`. *end note*

#### 11.17.3.4 Degenerate query expressions

A query expression of the form

```csharp
from «x» in «e» select «x»
```

is translated into

```csharp
( «e» ) . Select ( «x» => «x» )
```

> *Example*: The example
>
> ```csharp
> from c in customers
> select c
> ```
>
> is translated into
>
> ```csharp
> (customers).Select(c => c)
> ```
>
> *end example*

A degenerate query expression is one that trivially selects the elements of the source.

> *Note*: Later phases of the translation ([§11.17.3.6](expressions.md#111736-select-clauses) and [§11.17.3.7](expressions.md#111737-group-clauses)) remove degenerate queries introduced by other translation steps by replacing them with their source. It is important, however, to ensure that the result of a query expression is never the source object itself. Otherwise, returning the result of such a query might inadvertently expose private data (e.g., an element array) to a caller. Therefore this step protects degenerate queries written directly in source code by explicitly calling `Select` on the source. It is then up to the implementers of `Select` and other query operators to ensure that these methods never return the source object itself. *end note*

#### 11.17.3.5 From, let, where, join and orderby clauses

A query expression with a second `from` clause followed by a `select` clause

```csharp
from «x1» in «e1»  
from «x2» in «e2»  
select «v»
```

is translated into

```csharp
( «e1» ) . SelectMany( «x1» => «e2» , ( «x1» , «x2» ) => «v» )
```

> *Example*: The example
>
> ```csharp
> from c in customers
> from o in c.Orders
> select new { c.Name, o.OrderID, o.Total }
> ```
>
> is translated into
>
> ```csharp
> (customers).
> SelectMany(c => c.Orders,
> (c,o) => new { c.Name, o.OrderID, o.Total }
> )
> ```
>
> *end example*

A query expression with a second `from` clause followed by a query body `Q` containing a non-empty set of query body clauses:

```csharp
from «x1» in «e1»
from «x2» in «e2»
Q
```

is translated into

```csharp
from * in («e1») . SelectMany( «x1» => «e2» ,
                              ( «x1» , «x2» ) => new { «x1» , «x2» } )
Q
```

> *Example*: The example
>
> ```csharp
> from c in customers
> from o in c.Orders
> orderby o.Total descending
> select new { c.Name, o.OrderID, o.Total }
> ```
>
> is translated into
>
> ```csharp
> from * in (customers).
>    SelectMany(c => c.Orders, (c,o) => new { c, o })
> orderby o.Total descending
> select new { c.Name, o.OrderID, o.Total }
> ```
>
> the final translation of which is
>
> ```csharp
> customers.
> SelectMany(c => c.Orders, (c,o) => new { c, o }).
> OrderByDescending(x => x.o.Total).
> Select(x => new { x.c.Name, x.o.OrderID, x.o.Total })
> ```
>
> where `x` is a compiler generated identifier that is otherwise invisible and inaccessible.
>
> *end example*

A `let` expression along with its preceding `from` clause:

```csharp
from «x» in «e»  
let «y» = «f»  
...
```

is translated into

```csharp
from * in ( «e» ) . Select ( «x» => new { «x» , «y» = «f» } )  
...
```

> *Example*: The example
>
> ```csharp
> from o in orders
> let t = o.Details.Sum(d => d.UnitPrice * d.Quantity)
> where t >= 1000
> select new { o.OrderID, Total = t }
> ```
>
> is translated into
>
> ```csharp
> from * in (orders).Select(
>     o => new { o, t = o.Details.Sum(d => d.UnitPrice * d.Quantity) })
> where t >= 1000
> select new { o.OrderID, Total = t }
> ```
>
> the final translation of which is
>
> ```csharp
> orders
>     .Select(o => new { o, t = o.Details.Sum(d => d.UnitPrice * d.Quantity) })
>     .Where(x => x.t >= 1000)
>     .Select(x => new { x.o.OrderID, Total = x.t })
> ```
>
> where `x` is a compiler generated identifier that is otherwise invisible and inaccessible.
>
> *end example*

A `where` expression along with its preceding `from` clause:

```csharp
from «x» in «e»  
where «f»  
...
```

is translated into

```csharp
from «x» in ( «e» ) . Where ( «x» => «f» )  
...
```

A `join` clause immediately followed by a `select` clause

```csharp
from «x1» in «e1»  
join «x2» in «e2» on «k1» equals «k2»  
select «v»
```

is translated into

```csharp
( «e1» ) . Join( «e2» , «x1» => «k1» , «x2» => «k2» , ( «x1» , «x2» ) => «v» )
```

> *Example*: The example
>
> ```csharp
> from c in customersh
> join o in orders on c.CustomerID equals o.CustomerID
> select new { c.Name, o.OrderDate, o.Total }
> ```
>
> is translated into
>
> ```csharp
> (customers).Join(
>    orders,
>    c => c.CustomerID, o => o.CustomerID,
>    (c, o) => new { c.Name, o.OrderDate, o.Total })
> ```
>
> *end example*

A `join` clause followed by a query body clause:

```csharp
from «x1» in «e1»  
join «x2» in «e2» on «k1» equals «k2»  
...
```

is translated into

```csharp
from * in ( «e1» ) . Join(  
«e2» , «x1» => «k1» , «x2» => «k2» ,
( «x1» , «x2» ) => new { «x1» , «x2» })  
...
```

A `join`-`into` clause immediately followed by a `select` clause

```csharp
from «x1» in «e1»  
join «x2» in «e2» on «k1» equals «k2» into «g»  
select «v»
```

is translated into

```csharp
( «e1» ) . GroupJoin( «e2» , «x1» => «k1» , «x2» => «k2» ,
                     ( «x1» , «g» ) => «v» )
```

A `join into` clause followed by a query body clause

```csharp
from «x1» in «e1»  
join «x2» in «e2» on «k1» equals «k2» into *g»  
...
```

is translated into

```csharp
from * in ( «e1» ) . GroupJoin(  
   «e2» , «x1» => «k1» , «x2» => «k2» , ( «x1» , «g» ) => new { «x1» , «g» })
...
```

> *Example*: The example
>
> ```csharp
> from c in customers
> join o in orders on c.CustomerID equals o.CustomerID into co
> let n = co.Count()
> where n >= 10
> select new { c.Name, OrderCount = n }
> ```
>
> is translated into
>
> ```csharp
> from * in (customers).GroupJoin(
>     orders,
>     c => c.CustomerID,
>     o => o.CustomerID,
>     (c, co) => new { c, co })
> let n = co.Count()
> where n >= 10
> select new { c.Name, OrderCount = n }
> ```
>
> the final translation of which is
>
> ```csharp
> customers
>     .GroupJoin(
>         orders,
>         c => c.CustomerID,
>         o => o.CustomerID,
>         (c, co) => new { c, co })
>     .Select(x => new { x, n = x.co.Count() })
>     .Where(y => y.n >= 10)
>     .Select(y => new { y.x.c.Name, OrderCount = y.n })
> ```
>
> where `x` and `y` are compiler generated identifiers that are otherwise invisible and inaccessible.
>
> *end example*

An `orderby` clause and its preceding `from` clause:

```csharp
from «x» in «e»  
orderby «k1» , «k2» , ... , «kn»  
...
```

is translated into

```csharp
from «x» in ( «e» ) .  
OrderBy ( «x» => «k1» ) .  
ThenBy ( «x» => «k2» ) .  
... .  
ThenBy ( «x» => «kn» )  
...
```

If an `ordering` clause specifies a descending direction indicator, an invocation of `OrderByDescending` or `ThenByDescending` is produced instead.

> *Example*: The example
>
> ```csharp
> from o in orders
> orderby o.Customer.Name, o.Total descending
> select o
> ```
>
> has the final translation
>
> ```csharp
> (orders)
>     .OrderBy(o => o.Customer.Name)
>     .ThenByDescending(o => o.Total)
> ```
>
> *end example*

The following translations assume that there are no `let`, `where`, `join` or `orderby` clauses, and no more than the one initial `from` clause in each query expression.

#### 11.17.3.6 Select clauses

A query expression of the form

```csharp
from «x» in «e» select «v»
```

is translated into

```csharp
( «e» ) . Select ( «x» => «v» )
```

except when `«v»` is the identifier `«x»`, the translation is simply

```csharp
( «e» )
```

> *Example*: The example
>
> ```csharp
> from c in customers.Where(c => c.City == "London")
> select c
> ```
>
> is simply translated into
>
> ```csharp
> (customers).Where(c => c.City == "London")
> ```
>
> *end example*

#### 11.17.3.7 Group clauses

A `group` clause

```csharp
from «x» in «e» group «v» by «k»
```

is translated into

```csharp
( «e» ) . GroupBy ( «x» => «k» , «x» => «v» )
```

except when `«v»` is the identifier `«x»`, the translation is

```csharp
( «e» ) . GroupBy ( «x» => «k» )
```

> *Example*: The example
>
> ```csharp
> from c in customers
> group c.Name by c.Country
> ```
>
> is translated into
>
> ```csharp
> (customers).GroupBy(c => c.Country, c => c.Name)
> ```
>
> *end example*

#### 11.17.3.8 Transparent identifiers

Certain translations inject range variables with ***transparent identifiers*** denoted by `*`. Transparent identifiers exist only as an intermediate step in the query-expression translation process.

When a query translation injects a transparent identifier, further translation steps propagate the transparent identifier into anonymous functions and anonymous object initializers. In those contexts, transparent identifiers have the following behavior:

- When a transparent identifier occurs as a parameter in an anonymous function, the members of the associated anonymous type are automatically in scope in the body of the anonymous function.
- When a member with a transparent identifier is in scope, the members of that member are in scope as well.
- When a transparent identifier occurs as a member declarator in an anonymous object initializer, it introduces a member with a transparent identifier.

In the translation steps described above, transparent identifiers are always introduced together with anonymous types, with the intent of capturing multiple range variables as members of a single object. An implementation of C# is permitted to use a different mechanism than anonymous types to group together multiple range variables. The following translation examples assume that anonymous types are used, and shows one possible translation of transparent identifiers.

> *Example*: The example
>
> ```csharp
> from c in customers
> from o in c.Orders
> orderby o.Total descending
> select new { c.Name, o.Total }
> ```
>
> is translated into
>
> ```csharp
> from * in (customers).SelectMany(c => c.Orders, (c,o) => new { c, o })
> orderby o.Total descending
> select new { c.Name, o.Total }
> ```
>
> which is further translated into
>
> ```csharp
> customers
>     .SelectMany(c => c.Orders, (c,o) => new { c, o })
>     .OrderByDescending(* => o.Total)
>     .Select(\* => new { c.Name, o.Total })
> ```
>
> which, when transparent identifiers are erased, is equivalent to
>
> ```csharp
> customers
>     .SelectMany(c => c.Orders, (c,o) => new { c, o })
>     .OrderByDescending(x => x.o.Total)
>     .Select(x => new { x.c.Name, x.o.Total })
> ```
>
> where `x` is a compiler generated identifier that is otherwise invisible and inaccessible.
>
> The example
>
> ```csharp
> from c in customers
> join o in orders on c.CustomerID equals o.CustomerID
> join d in details on o.OrderID equals d.OrderID
> join p in products on d.ProductID equals p.ProductID
> select new { c.Name, o.OrderDate, p.ProductName }
> ```
>
> is translated into
>
> ```csharp
> from * in (customers).Join(
>     orders,
>     c => c.CustomerID,
>     o => o.CustomerID,
>     (c, o) => new { c, o })
> join d in details on o.OrderID equals d.OrderID
> join p in products on d.ProductID equals p.ProductID
> select new { c.Name, o.OrderDate, p.ProductName }
> ```
>
> which is further reduced to
>
> ```csharp
> customers
>     .Join(orders, c => c.CustomerID,
>         o => o.CustomerID, (c, o) => new { c, o })
>     .Join(details, * => o.OrderID, d => d.OrderID, (*, d) => new { *, d })
>     .Join(products, * => d.ProductID, p => p.ProductID,
>         (*, p) => new { c.Name, o.OrderDate, p.ProductName })
> ```
>
> the final translation of which is
>
> ```csharp
> customers
>     .Join(orders, c => c.CustomerID,
>         o => o.CustomerID, (c, o) => new { c, o })
>     .Join(details, x => x.o.OrderID, d => d.OrderID, (x, d) => new { x, d })
>     .Join(products, y => y.d.ProductID, p => p.ProductID,
>         (y, p) => new { y.x.c.Name, y.x.o.OrderDate, p.ProductName })
> ```
>
> where `x` and `y` are compiler-generated identifiers that are otherwise invisible and inaccessible.
> *end example*

### 11.17.4 The query-expression pattern

The ***Query-expression pattern*** establishes a pattern of methods that types can implement to support query expressions.

A generic type `C<T>` supports the query-expression-pattern if its public member methods and the publicly accessible extension methods could be replaced by the following class definition. The members and accessible extenson methods is referred to as the “shape” of a generic type `C<T>`. A generic type is used in order to illustrate the proper relationships between parameter and return types, but it is possible to implement the pattern for non-generic types as well.

```csharp
delegate R Func<T1,T2,R>(T1 arg1, T2 arg2);

class C
{
    public C<T> Cast<T>();
}

class C<T> : C
{
    public C<T> Where(Func<T,bool> predicate);
    public C<U> Select<U>(Func<T,U> selector);
    public C<V> SelectMany<U,V>(Func<T,C<U>> selector,
        Func<T,U,V> resultSelector);
    public C<V> Join<U,K,V>(C<U> inner, Func<T,K> outerKeySelector,
        Func<U,K> innerKeySelector, Func<T,U,V> resultSelector);
    public C<V> GroupJoin<U,K,V>(C<U> inner, Func<T,K> outerKeySelector,
        Func<U,K> innerKeySelector, Func<T,C<U>,V> resultSelector);
    public O<T> OrderBy<K>(Func<T,K> keySelector);
    public O<T> OrderByDescending<K>(Func<T,K> keySelector);
    public C<G<K,T>> GroupBy<K>(Func<T,K> keySelector);
    public C<G<K,E>> GroupBy<K,E>(Func<T,K> keySelector,
        Func<T,E> elementSelector);
}

class O<T> : C<T>
{
    public O<T> ThenBy<K>(Func<T,K> keySelector);
    public O<T> ThenByDescending<K>(Func<T,K> keySelector);
}

class G<K,T> : C<T>
{
    public K Key { get; }
}
```

The methods above use the generic delegate types `Func<T1, R>` and `Func<T1, T2, R>`, but they could equally well have used other delegate or expression-tree types with the same relationships in parameter and return types.

> *Note*: The recommended relationship between `C<T>` and `O<T>` that ensures that the `ThenBy` and `ThenByDescending` methods are available only on the result of an `OrderBy` or `OrderByDescending`. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The recommended shape of the result of `GroupBy`—a sequence of sequences, where each inner sequence has an additional `Key` property. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: Because query expressions are translated to method invocations by means of a syntactic mapping, types have considerable flexibility in how they implement any or all of the query-expression pattern. For example, the methods of the pattern can be implemented as instance methods or as extension methods because the two have the same invocation syntax, and the methods can request delegates or expression trees because anonymous functions are convertible to both. Types implementing only some of the query expression pattern support only query expression translations that map to the methods that type supports. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The `System.Linq` namespace provides an implementation of the query-expression pattern for any type that implements the `System.Collections.Generic.IEnumerable<T>` interface. *end note*

## 11.18 Assignment operators

### 11.18.1 General

The assignment operators assign a new value to a variable, a property, an event, or an indexer element.

```ANTLR
assignment
    : unary_expression assignment_operator expression
    ;

assignment_operator
    : '=' | '+=' | '-=' | '*=' | '/=' | '%=' | '&=' | '|=' | '^=' | '<<='
    | right_shift_assignment
    ;
```

The left operand of an assignment shall be an expression classified as a variable, a property access, an indexer access, or an event access.

The `=` operator is called the ***simple assignment operator***. It assigns the value of the right operand to the variable, property, or indexer element given by the left operand. The left operand of the simple assignment operator shall not be an event access (except as described in [§14.8.2](classes.md#1482-field-like-events)). The simple assignment operator is described in [§11.18.2](expressions.md#11182-simple-assignment).

The assignment operators other than the `=` operator are called the ***compound assignment operators***. These operators perform the indicated operation on the two operands, and then assign the resulting value to the variable, property, or indexer element given by the left operand. The compound assignment operators are described in [§11.18.3](expressions.md#11183-compound-assignment).

The `+=` and `-=` operators with an event access expression as the left operand are called the ***event assignment operators***. No other assignment operator is valid with an event access as the left operand. The event assignment operators are described in [§11.18.4](expressions.md#11184-event-assignment).

The assignment operators are right-associative, meaning that operations are grouped from right to left.

> *Example*: An expression of the form `a = b = c` is evaluated as `a = (b = c)`. *end example*

### 11.18.2 Simple assignment

The `=` operator is called the simple assignment operator.

If the left operand of a simple assignment is of the form `E.P` or `E[Ei]` where `E` has the compile-time type `dynamic`, then the assignment is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)). In this case, the compile-time type of the assignment expression is `dynamic`, and the resolution described below will take place at run-time based on the run-time type of `E`. If the left operand is of the form `E[Ei]` where at least one element of `Ei` has the compile-time type `dynamic`, and the compile-time type of `E` is not an array, the resulting indexer access is dynamically bound, but with limited compile-time checking ([§11.6.5](expressions.md#1165-compile-time-checking-of-dynamic-member-invocation)).

In a simple assignment, the right operand shall be an expression that is implicitly convertible to the type of the left operand. The operation assigns the value of the right operand to the variable, property, or indexer element given by the left operand.

The result of a simple assignment expression is the value assigned to the left operand. The result has the same type as the left operand, and is always classified as a value.

If the left operand is a property or indexer access, the property or indexer shall have an accessible set accessor. If this is not the case, a binding-time error occurs.

The run-time processing of a simple assignment of the form `x` = `y` consists of the following steps:

- If `x` is classified as a variable:
  - `x` is evaluated to produce the variable.
  - `y` is evaluated and, if required, converted to the type of `x` through an implicit conversion ([§10.2](conversions.md#102-implicit-conversions)).
  - If the variable given by `x` is an array element of a *reference_type*, a run-time check is performed to ensure that the value computed for `y` is compatible with the array instance of which `x` is an element. The check succeeds if `y` is `null`, or if an implicit reference conversion ([§10.2.8](conversions.md#1028-implicit-reference-conversions)) exists from the type of the instance referenced by `y` to the actual element type of the array instance containing `x`. Otherwise, a `System.ArrayTypeMismatchException` is thrown.
  - The value resulting from the evaluation and conversion of `y` is stored into the location given by the evaluation of `x`.
- If `x` is classified as a property or indexer access:
  - The instance expression (if `x` is not `static`) and the argument list (if `x` is an indexer access) associated with `x` are evaluated, and the results are used in the subsequent set accessor invocation.
  - `y` is evaluated and, if required, converted to the type of `x` through an implicit conversion ([§10.2](conversions.md#102-implicit-conversions)).
  - The set accessor of `x` is invoked with the value computed for `y` as its value argument.

> *Note*: if the compile time type of `x` is `dynamic` and there is an implicit conversion from the compile time type of `y` to `dynamic`, no runtime resolution is required. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The array co-variance rules ([§16.6](arrays.md#166-array-covariance)) permit a value of an array type `A[]` to be a reference to an instance of an array type `B[]`, provided an implicit reference conversion exists from `B` to `A`. Because of these rules, assignment to an array element of a *reference_type* requires a run-time check to ensure that the value being assigned is compatible with the array instance. In the example
>
> ```csharp
> string[] sa = new string[10];
> object[] oa = sa;
> oa[0] = null;              // OK
> oa[1] = "Hello";           // OK
> oa[2] = new ArrayList();   // ArrayTypeMismatchException
> ```
>
> the last assignment causes a `System.ArrayTypeMismatchException` to be thrown because a reference to an `ArrayList` cannot be stored in an element of a `string[]`.
>
> *end note*

When a property or indexer declared in a *struct_type* is the target of an assignment, the instance expression associated with the property or indexer access shall be classified as a variable. If the instance expression is classified as a value, a binding-time error occurs.

> *Note*: Because of [§11.7.6](expressions.md#1176-member-access), the same rule also applies to fields. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: Given the declarations:
>
> ```csharp
> struct Point
> {
>    int x, y;
>
>    public Point(int x, int y)
>    {
>       this.x = x;
>       this.y = y;
>    }
>
>    public int X
>    {
>       get { return x; }
>       set { x = value; }
>    }
>
>    public int Y {
>       get { return y; }
>       set { y = value; }
>    }
> }
>
> struct Rectangle
> {
>     Point a, b;
>
>     public Rectangle(Point a, Point b)
>     {
>         this.a = a;
>         this.b = b;
>     }
>
>     public Point A
>     {
>         get { return a; }
>         set { a = value; }
>     }
>
>     public Point B
>     {
>         get { return b; }
>         set { b = value; }
>     }
> }
> ```
>
> in the example
>
> ```csharp
> Point p = new Point();
> p.X = 100;
> p.Y = 100;
> Rectangle r = new Rectangle();
> r.A = new Point(10, 10);
> r.B = p;
> ```
>
> the assignments to `p.X`, `p.Y`, `r.A`, and `r.B` are permitted because `p` and `r` are variables. However, in the example
>
> ```csharp
> Rectangle r = new Rectangle();
> r.A.X = 10;
> r.A.Y = 10;
> r.B.X = 100;
> r.B.Y = 100;
> ```
>
> the assignments are all invalid, since `r.A` and `r.B` are not variables.
>
> *end example*

### 11.18.3 Compound assignment

If the left operand of a compound assignment is of the form `E.P` or `E[Ei]` where `E` has the compile-time type `dynamic`, then the assignment is dynamically bound ([§11.3.3](expressions.md#1133-dynamic-binding)). In this case, the compile-time type of the assignment expression is `dynamic`, and the resolution described below will take place at run-time based on the run-time type of `E`. If the left operand is of the form `E[Ei]` where at least one element of `Ei` has the compile-time type `dynamic`, and the compile-time type of `E` is not an array, the resulting indexer access is dynamically bound, but with limited compile-time checking ([§11.6.5](expressions.md#1165-compile-time-checking-of-dynamic-member-invocation)).

An operation of the form `x «op»= y` is processed by applying binary operator overload resolution ([§11.4.5](expressions.md#1145-binary-operator-overload-resolution)) as if the operation was written `x «op» y`. Then,

- If the return type of the selected operator is implicitly convertible to the type of `x`, the operation is evaluated as `x = x «op» y`, except that `x` is evaluated only once.
- Otherwise, if the selected operator is a predefined operator, if the return type of the selected operator is explicitly convertible to the type of `x` , and if `y` is implicitly convertible to the type of `x`  or the operator is a shift operator, then the operation is evaluated as `x = (T)(x «op» y)`, where `T` is the type of `x`, except that `x` is evaluated only once.
- Otherwise, the compound assignment is invalid, and a binding-time error occurs.

The term “evaluated only once” means that in the evaluation of `x «op» y`, the results of any constituent expressions of `x` are temporarily saved and then reused when performing the assignment to `x`.

> *Example*: In the assignment `A()[B()] += C()`, where `A` is a method returning `int[]`, and `B` and `C` are methods returning `int`, the methods are invoked only once, in the order `A`, `B`, `C`. *end example*

When the left operand of a compound assignment is a property access or indexer access, the property or indexer shall have both a get accessor and a set accessor. If this is not the case, a binding-time error occurs.

The second rule above permits `x «op»= y` to be evaluated as `x = (T)(x «op» y)` in certain contexts. The rule exists such that the predefined operators can be used as compound operators when the left operand is of type `sbyte`, `byte`, `short`, `ushort`, or `char`. Even when both arguments are of one of those types, the predefined operators produce a result of type `int`, as described in [§11.4.7.3](expressions.md#11473-binary-numeric-promotions). Thus, without a cast it would not be possible to assign the result to the left operand.

The intuitive effect of the rule for predefined operators is simply that `x «op»= y` is permitted if both of `x «op» y` and `x = y` are permitted.

> *Example*: In the following code
>
> ```csharp
> byte b = 0;
> char ch = '\0';
> int i = 0;
> b += 1;           // OK
> b += 1000;        // Error, b = 1000 not permitted
> b += i;           // Error, b = i not permitted
> b += (byte)i;     // OK
> ch += 1;          // Error, ch = 1 not permitted
> ch += (char)1;    // OK
> ```
>
> the intuitive reason for each error is that a corresponding simple assignment would also have been an error.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: This also means that compound assignment operations support lifted operators. Since a compound assignment `x «op»= y` is evaluated as either `x = x «op» y` or `x = (T)(x «op» y)`, the rules of evaluation implicitly cover lifted operators. *end note*

### 11.18.4 Event assignment

If the left operand of `a += or -=` operator is classified as an event access, then the expression is evaluated as follows:

- The instance expression, if any, of the event access is evaluated.
- The right operand of the `+=` or `-=` operator is evaluated, and, if required, converted to the type of the left operand through an implicit conversion ([§10.2](conversions.md#102-implicit-conversions)).
- An event accessor of the event is invoked, with an argument list consisting of the value computed in the previous step. If the operator was `+=`, the add accessor is invoked; if the operator was `-=`, the remove accessor is invoked.

An event assignment expression does not yield a value. Thus, an event assignment expression is valid only in the context of a *statement_expression* ([§12.7](statements.md#127-expression-statements)).

## 11.19 Expression

An *expression* is either a *non_assignment_expression* or an *assignment*.

```ANTLR
expression
    : non_assignment_expression
    | assignment
    ;

non_assignment_expression
    : conditional_expression
    | lambda_expression
    | query_expression
    ;
```

## 11.20 Constant expressions

A constant expression is an expression that shall be fully evaluated at compile-time.

```ANTLR
constant_expression
    : expression
    ;
```

A constant expression may be either a value type or a reference type. If a constant expression is a value type, it must be one of the following types: `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `float`, `double`, `decimal`, `bool,` or any enumeration type. If a constant expression is a reference type, it must be the `string` type, a default value expression ([§11.7.19](expressions.md#11719-default-value-expressions)) for some reference type, or the value of the expression must be `null`.

Only the following constructs are permitted in constant expressions:

- Literals (including the `null` literal).
- References to `const` members of class and struct types.
- References to members of enumeration types.
- References to `const` parameters or local variables
- Parenthesized subexpressions, which are themselves constant expressions.
- Cast expressions.
- `checked` and `unchecked` expressions.
- `nameof` expressions
- The predefined `+`, `–`, `!`, and `~` unary operators.
- The predefined `+`, `–`, `*`, `/`, `%`, `<<`, `>>`, `&`, `|`, `^`, `&&`, `||`, `==`, `!=`, `<`, `>`, `<=`, and `>=` binary operators.
- The `?:` conditional operator.
- `sizeof` expressions, provided the unmanaged-type is one of the types specified in [§22.6.9](unsafe-code.md#2269-the-sizeof-operator) for which `sizeof` returns a constant value.
- Default value expressions, provided the type is one of the types listed above.

The following conversions are permitted in constant expressions:

- Identity conversions
- Numeric conversions
- Enumeration conversions
- Constant expression conversions
- Implicit and explicit reference conversions, provided the source of the conversions is a constant expression that evaluates to the `null` value.

> *Note*: Other conversions including boxing, unboxing, and implicit reference conversions of non-`null` values are not permitted in constant expressions. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: In the following code
>
> ```csharp
> class C
> {
>     const object i = 5;         // error: boxing conversion not permitted
>     const object str = "hello"; // error: implicit reference conversion
> }
> ```
>
> the initialization of `i` is an error because a boxing conversion is required. The initialization of `str` is an error because an implicit reference conversion from a non-`null` value is required.
>
> *end example*

Whenever an expression fulfills the requirements listed above, the expression is evaluated at compile-time. This is true even if the expression is a subexpression of a larger expression that contains non-constant constructs.

The compile-time evaluation of constant expressions uses the same rules as run-time evaluation of non-constant expressions, except that where run-time evaluation would have thrown an exception, compile-time evaluation causes a compile-time error to occur.

Unless a constant expression is explicitly placed in an `unchecked` context, overflows that occur in integral-type arithmetic operations and conversions during the compile-time evaluation of the expression always cause compile-time errors ([§11.7.18](expressions.md#11718-the-checked-and-unchecked-operators)).

Constant expressions are required in the contexts listed below and this is indicated in the grammar by using *constant_expression*. In these contexts, a compile-time error occurs if an expression cannot be fully evaluated at compile-time.

- Constant declarations ([§14.4](classes.md#144-constants))
- Enumeration member declarations ([§18.4](enums.md#184-enum-members))
- Default arguments of formal parameter lists ([§14.6.2](classes.md#1462-method-parameters))
- `case` labels of a `switch` statement ([§12.8.3](statements.md#1283-the-switch-statement)).
- `goto case` statements ([§12.10.4](statements.md#12104-the-goto-statement))
- Dimension lengths in an array creation expression ([§11.7.15.5](expressions.md#117155-array-creation-expressions)) that includes an initializer.
- Attributes ([§21](attributes.md#21-attributes))

An implicit constant expression conversion ([§10.2.11](conversions.md#10211-implicit-constant-expression-conversions)) permits a constant expression of type `int` to be converted to `sbyte`, `byte`, `short`, `ushort`, `uint`, or `ulong`, provided the value of the constant expression is within the range of the destination type.

## 11.21 Boolean expressions

A *boolean_expression* is an expression that yields a result of type `bool`; either directly or through application of `operator true` in certain contexts as specified in the following:

```ANTLR
boolean_expression
    : expression
    ;
```

The controlling conditional expression of an *if_statement* ([§12.8.2](statements.md#1282-the-if-statement)), *while_statement* ([§12.9.2](statements.md#1292-the-while-statement)), *do_statement* ([§12.9.3](statements.md#1293-the-do-statement)), or *for_statement* ([§12.9.4](statements.md#1294-the-for-statement)) is a *boolean_expression*. The controlling conditional expression of the `?:` operator ([§11.15](expressions.md#1115-conditional-operator)) follows the same rules as a *boolean_expression*, but for reasons of operator precedence is classified as a *null_coalescing_expression*.

A *boolean_expression* `E` is required to be able to produce a value of type `bool`, as follows:

- If E is implicitly convertible to `bool` then at run-time that implicit conversion is applied.
- Otherwise, unary operator overload resolution ([§11.4.4](expressions.md#1144-unary-operator-overload-resolution)) is used to find a unique best implementation of `operator true` on `E`, and that implementation is applied at run-time.
- If no such operator is found, a binding-time error occurs.
