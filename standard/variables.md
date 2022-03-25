# 9 Variables

## 9.1 General

Variables represent storage locations. Every variable has a type that determines what values can be stored in the variable. C# is a type-safe language, and the C# compiler guarantees that values stored in variables are always of the appropriate type. The value of a variable can be changed through assignment or through use of the `++` and `--` operators.

A variable shall be *definitely assigned* ([§9.4](variables.md#94-definite-assignment)) before its value can be obtained.

As described in the following subclauses, variables are either ***initially assigned*** or ***initially unassigned***. An initially assigned variable has a well-defined initial value and is always considered definitely assigned. An initially unassigned variable has no initial value. For an initially unassigned variable to be considered definitely assigned at a certain location, an assignment to the variable shall occur in every possible execution path leading to that location.

## 9.2 Variable categories

### 9.2.1 General

C# defines seven categories of variables: static variables, instance variables, array elements, value parameters, reference parameters, output parameters, and local variables. The subclauses that follow describe each of these categories.

> *Example*: In the following code
>
> ```csharp
> class A
> {
>     public static int x;
>     int y;
> 
>     void F(int[] v, int a, ref int b, out int c)
>     {
>         int i = 1;
>         c = a + b++;
>     }
> }
> ```
>
> `x` is a static variable, `y` is an instance variable, `v[0]` is an array element, `a` is a value parameter, `b` is a reference parameter, `c` is an output parameter, and `i` is a local variable. *end example*

### 9.2.2 Static variables

A field declared with the `static` modifier is a static variable. A static variable comes into existence before execution of the `static` constructor ([§14.12](classes.md#1412-static-constructors)) for its containing type, and ceases to exist when the associated application domain ceases to exist.

The initial value of a static variable is the default value ([§9.3](variables.md#93-default-values)) of the variable’s type.

For the purposes of definite assignment checking, a static variable is considered initially assigned.

### 9.2.3 Instance variables

#### 9.2.3.1 General

A field declared without the `static` modifier is an instance variable.

#### 9.2.3.2 Instance variables in classes

An instance variable of a class comes into existence when a new instance of that class is created, and ceases to exist when there are no references to that instance and the instance’s finalizer (if any) has executed.

The initial value of an instance variable of a class is the default value ([§9.3](variables.md#93-default-values)) of the variable’s type.

For the purpose of definite assignment checking, an instance variable of a class is considered initially assigned.

#### 9.2.3.3 Instance variables in structs

An instance variable of a struct has exactly the same lifetime as the struct variable to which it belongs. In other words, when a variable of a struct type comes into existence or ceases to exist, so too do the instance variables of the struct.

The initial assignment state of an instance variable of a struct is the same as that of the containing `struct` variable. In other words, when a struct variable is considered initially assigned, so too are its instance variables, and when a struct variable is considered initially unassigned, its instance variables are likewise unassigned.

### 9.2.4 Array elements

The elements of an array come into existence when an array instance is created, and cease to exist when there are no references to that array instance.

The initial value of each of the elements of an array is the default value ([§9.3](variables.md#93-default-values)) of the type of the array elements.

For the purpose of definite assignment checking, an array element is considered initially assigned.

### 9.2.5 Value parameters

A parameter declared without a `ref` or `out` modifier is a ***value parameter***.

A value parameter comes into existence upon invocation of the function member (method, instance constructor, accessor, or operator) or anonymous function to which the parameter belongs, and is initialized with the value of the argument given in the invocation. A value parameter normally ceases to exist when execution of the function body completes. However, if the value parameter is captured by an anonymous function ([§11.16.6.2](expressions.md#111662-captured-outer-variables)), its lifetime extends at least until the delegate or expression tree created from that anonymous function is eligible for garbage collection.

For the purpose of definite assignment checking, a value parameter is considered initially assigned.

### 9.2.6 Reference parameters

A parameter declared with a `ref` modifier is a ***reference parameter***.

A reference parameter does not create a new storage location. Instead, a reference parameter represents the same storage location as the variable given as the argument in the function member or anonymous function invocation. Thus, the value of a reference parameter is always the same as the underlying variable.

The following definite assignment rules apply to reference parameters.

> *Note*: The rules for output parameters are different, and are described in ([§9.2.7](variables.md#927-output-parameters)). *end note*

- A variable shall be definitely assigned ([§9.4](variables.md#94-definite-assignment)) before it can be passed as a reference parameter in a function member or delegate invocation.
- Within a function member or anonymous function, a reference parameter is considered initially assigned.

For a `struct` type, within an instance method or instance accessor ([§11.2.1](expressions.md#1121-general)) or instance constructor with a constructor initializer, the `this` keyword behaves exactly as a reference parameter of the struct type ([§11.7.12](expressions.md#11712-this-access)).

### 9.2.7 Output parameters

A parameter declared with an `out` modifier is an ***output parameter***.

An output parameter does not create a new storage location. Instead, an output parameter represents the same storage location as the variable given as the argument in the function member or delegate invocation. Thus, the value of an output parameter is always the same as the underlying variable.

The following definite assignment rules apply to output parameters.

> *Note*: The rules for reference parameters are different, and are described in ([§9.2.6](variables.md#926-reference-parameters)). *end note*

- A variable need not be definitely assigned before it can be passed as an output parameter in a function member or delegate invocation.
- Following the normal completion of a function member or delegate invocation, each variable that was passed as an output parameter is considered assigned in that execution path.
- Within a function member or anonymous function, an output parameter is considered initially unassigned.
- Every output parameter of a function member or anonymous function shall be definitely assigned ([§9.4](variables.md#94-definite-assignment)) before the function member or anonymous function returns normally.

Within an instance constructor of a struct type, the `this` keyword behaves exactly as an output or reference parameter of the struct type, depending on whether the constructor declaration includes a constructor initializer ([§11.7.12](expressions.md#11712-this-access)).

### 9.2.8 Local variables

A ***local variable*** is declared by a *local_variable_declaration*, *foreach_statement*, or *specific_catch_clause* of a *try_statement*. For a *foreach_statement*, the local variable is an iteration variable ([§12.9.5](statements.md#1295-the-foreach-statement)). For a *specific_catch_clause*, the local variable is an exception variable ([§12.11](statements.md#1211-the-try-statement)). A local variable declared by a *foreach_statement* or *specific_catch_clause* is considered initially assigned.

A *local_variable_declaration* can occur in a *block*, a *for_statement*, a *switch_block*, or a *using_statement*.

The lifetime of a local variable is the portion of program execution during which storage is guaranteed to be reserved for it. This lifetime extends from entry into the scope with which it is associated, at least until execution of that scope ends in some way. (Entering an enclosed *block*, calling a method, or yielding a value from an iterator block suspends, but does not end, execution of the current scope.) If the local variable is captured by an anonymous function ([§11.16.6.2](expressions.md#111662-captured-outer-variables)), its lifetime extends at least until the delegate or expression tree created from the anonymous function, along with any other objects that come to reference the captured variable, are eligible for garbage collection. If the parent scope is entered recursively or iteratively, a new instance of the local variable is created each time, and its *local_variable_initializer*, if any, is evaluated each time.

> *Note*: A local variable is instantiated each time its scope is entered. This behavior is visible to user code containing anonymous methods. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The lifetime of an *iteration variable* ([§12.9.5](statements.md#1295-the-foreach-statement)) declared by a *foreach_statement* is a single iteration of that statement. Each iteration creates a new variable. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The actual lifetime of a local variable is implementation-dependent. For example, a compiler might statically determine that a local variable in a block is only used for a small portion of that block. Using this analysis, the compiler could generate code that results in the variable’s storage having a shorter lifetime than its containing block.
>
> The storage referred to by a local reference variable is reclaimed independently of the lifetime of that local reference variable ([§7.9](basic-concepts.md#79-automatic-memory-management)). *end note*

A local variable introduced by a *local_variable_declaration* is not automatically initialized and thus has no default value. Such a local variable is considered initially unassigned.

> *Note*: A *local_variable_declaration* that includes a *local_variable_initializer* is still initially unassigned. Execution of the declaration behaves exactly like an assignment to the variable ([§9.4.4.5](variables.md#9445-declaration-statements)). It is possible to use a variable without executing its *local_variable_initializer*; e.g., within the initializer expression itself or by using a *goto_statement* to bypass the initialization:
>
> ```csharp
> goto L;
> 
> int x = 1; // never executed
> 
> L: x += 1; // error: x not definitely assigned
> ```
>
> Within the scope of a local variable, it is a compile-time error to refer to that local variable in a textual position that precedes its *local_variable_declarator*. *end note*

## 9.3 Default values

The following categories of variables are automatically initialized to their default values:

- Static variables.
- Instance variables of class instances.
- Array elements.

The default value of a variable depends on the type of the variable and is determined as follows:

- For a variable of a *value_type*, the default value is the same as the value computed by the *value_type*’s default constructor ([§8.3.3](types.md#833-default-constructors)).
- For a variable of a *reference_type*, the default value is `null`.

> *Note*: Initialization to default values is typically done by having the memory manager or garbage collector initialize memory to all-bits-zero before it is allocated for use. For this reason, it is convenient to use all-bits-zero to represent the null reference. *end note*

## 9.4 Definite assignment

### 9.4.1 General

At a given location in the executable code of a function member or an anonymous function, a variable is said to be ***definitely assigned*** if the compiler can prove, by a particular static flow analysis ([§9.4.4](variables.md#944-precise-rules-for-determining-definite-assignment)), that the variable has been automatically initialized or has been the target of at least one assignment.

> *Note*: Informally stated, the rules of definite assignment are:
>
> - An initially assigned variable ([§9.4.2](variables.md#942-initially-assigned-variables)) is always considered definitely assigned.
> - An initially unassigned variable ([§9.4.3](variables.md#943-initially-unassigned-variables)) is considered definitely assigned at a given location if all possible execution paths leading to that location contain at least one of the following:
>   - A simple assignment ([§11.18.2](expressions.md#11182-simple-assignment)) in which the variable is the left operand.
>   - An invocation expression ([§11.7.8](expressions.md#1178-invocation-expressions)) or object creation expression ([§11.7.15.2](expressions.md#117152-object-creation-expressions) that passes the variable as an output parameter.
>   - For a local variable, a local variable declaration for the variable ([§12.6.2](statements.md#1262-local-variable-declarations)) that includes a variable initializer.
>
> The formal specification underlying the above informal rules is described in [§9.4.2](variables.md#942-initially-assigned-variables), [§9.4.3](variables.md#943-initially-unassigned-variables), and [§9.4.4](variables.md#944-precise-rules-for-determining-definite-assignment). *end note*

The definite assignment states of instance variables of a *struct_type* variable are tracked individually as well as collectively. In additional to the rules above, the following rules apply to *struct_type* variables and their instance variables:

- An instance variable is considered definitely assigned if its containing *struct_type* variable is considered definitely assigned.
- A *struct_type* variable is considered definitely assigned if each of its instance variables is considered definitely assigned.

Definite assignment is a requirement in the following contexts:

- A variable shall be definitely assigned at each location where its value is obtained.
  > *Note*: This ensures that undefined values never occur. *end note*  
  The occurrence of a variable in an expression is considered to obtain the value of the variable, except when
  - the variable is the left operand of a simple assignment,
  - the variable is passed as an output parameter, or
  - the variable is a *struct_type* variable and occurs as the left operand of a member access.
- A variable shall be definitely assigned at each location where it is passed as a reference parameter.
  > *Note*: This ensures that the function member being invoked can consider the reference parameter initially assigned. *end note*
- All output parameters of a function member shall be definitely assigned at each location where the function member returns (through a return statement or through execution reaching the end of the function member body).
  > *Note*: This ensures that function members do not return undefined values in output parameters, thus enabling the compiler to consider a function member invocation that takes a variable as an output parameter equivalent to an assignment to the variable. *end note*
- The `this` variable of a *struct_type* instance constructor shall be definitely assigned at each location where that instance constructor returns.

### 9.4.2 Initially assigned variables

The following categories of variables are classified as initially assigned:

- Static variables.
- Instance variables of class instances.
- Instance variables of initially assigned struct variables.
- Array elements.
- Value parameters.
- Reference parameters.
- Variables declared in a `catch` clause or a `foreach` statement.

### 9.4.3 Initially unassigned variables

The following categories of variables are classified as initially unassigned:

- Instance variables of initially unassigned struct variables.
- Output parameters, including the `this` variable of struct instance constructors without a constructor initializer.
- Local variables, except those declared in a `catch` clause or a `foreach` statement.

### 9.4.4 Precise rules for determining definite assignment

#### 9.4.4.1 General

In order to determine that each used variable is definitely assigned, the compiler shall use a process that is equivalent to the one described in this subclause.

The compiler processes the body of each function member that has one or more initially unassigned variables. For each initially unassigned variable *v*, the compiler determines a ***definite assignment state*** for *v* at each of the following points in the function member:

- At the beginning of each statement
- At the end point ([§12.2](statements.md#122-end-points-and-reachability)) of each statement
- On each arc which transfers control to another statement or to the end point of a statement
- At the beginning of each expression
- At the end of each expression

The definite assignment state of *v* can be either:

- Definitely assigned. This indicates that on all possible control flows to this point, *v* has been assigned a value.
- Not definitely assigned. For the state of a variable at the end of an expression of type `bool`, the state of a variable that isn’t definitely assigned might (but doesn’t necessarily) fall into one of the following sub-states:
  - Definitely assigned after true expression. This state indicates that *v* is definitely assigned if the Boolean expression evaluated as true, but is not necessarily assigned if the Boolean expression evaluated as false.
  - Definitely assigned after false expression. This state indicates that *v* is definitely assigned if the Boolean expression evaluated as false, but is not necessarily assigned if the Boolean expression evaluated as true.

The following rules govern how the state of a variable *v* is determined at each location.

#### 9.4.4.2 General rules for statements

- *v* is not definitely assigned at the beginning of a function member body.
- The definite assignment state of *v* at the beginning of any other statement is determined by checking the definite assignment state of *v* on all control flow transfers that target the beginning of that statement. If (and only if) *v* is definitely assigned on all such control flow transfers, then *v* is definitely assigned at the beginning of the statement. The set of possible control flow transfers is determined in the same way as for checking statement reachability ([§12.2](statements.md#122-end-points-and-reachability)).
- The definite assignment state of *v* at the end point of a `block`, `checked`, `unchecked`, `if`, `while`, `do`, `for`, `foreach`, `lock`, `using`, or `switch` statement is determined by checking the definite assignment state of *v* on all control flow transfers that target the end point of that statement. If *v* is definitely assigned on all such control flow transfers, then *v* is definitely assigned at the end point of the statement. Otherwise, *v* is not definitely assigned at the end point of the statement. The set of possible control flow transfers is determined in the same way as for checking statement reachability ([§12.2](statements.md#122-end-points-and-reachability)).

> *Note*: Because there are no control paths to an unreachable statement, *v* is definitely assigned at the beginning of any unreachable statement. *end note*

#### 9.4.4.3 Block statements, checked, and unchecked statements

The definite assignment state of *v* on the control transfer to the first statement of the statement list in the block (or to the end point of the block, if the statement list is empty) is the same as the definite assignment statement of *v* before the block, `checked`, or `unchecked` statement.

#### 9.4.4.4 Expression statements

For an expression statement *stmt* that consists of the expression *expr*:

- *v* has the same definite assignment state at the beginning of *expr* as at the beginning of *stmt*.
- If *v* if definitely assigned at the end of *expr*, it is definitely assigned at the end point of *stmt*; otherwise, it is not definitely assigned at the end point of *stmt*.

#### 9.4.4.5 Declaration statements

- If *stmt* is a declaration statement without initializers, then *v* has the same definite assignment state at the end point of *stmt* as at the beginning of *stmt*.
- If *stmt* is a declaration statement with initializers, then the definite assignment state for *v* is determined as if *stmt* were a statement list, with one assignment statement for each declaration with an initializer (in the order of declaration).

#### 9.4.4.6 If statements

For an `if` statement *stmt* of the form:

`if (` *expr* `)` *then_stmt* `else` *else_stmt*

- *v* has the same definite assignment state at the beginning of *expr* as at the beginning of *stmt*.
- If *v* is definitely assigned at the end of *expr*, then it is definitely assigned on the control flow transfer to *then_stmt* and to either *else_stmt* or to the end-point of *stmt* if there is no else clause.
- If *v* has the state “definitely assigned after true expression” at the end of *expr*, then it is definitely assigned on the control flow transfer to *then_stmt*, and not definitely assigned on the control flow transfer to either *else_stmt* or to the end-point of *stmt* if there is no else clause.
- If *v* has the state “definitely assigned after false expression” at the end of *expr*, then it is definitely assigned on the control flow transfer to *else_stmt*, and not definitely assigned on the control flow transfer to *then_stmt*. It is definitely assigned at the end-point of *stmt* if and only if it is definitely assigned at the end-point of *then_stmt*.
- Otherwise, *v* is considered not definitely assigned on the control flow transfer to either the *then_stmt* or *else_stmt*, or to the end-point of *stmt* if there is no else clause.

#### 9.4.4.7 Switch statements

In a `switch` statement *stmt* with a controlling expression *expr*:

- The definite assignment state of *v* at the beginning of *expr* is the same as the state of *v* at the beginning of *stmt*.
- The definite assignment state of *v* on the control flow transfer to a reachable switch block statement list is the same as the definite assignment state of *v* at the end of *expr*.

#### 9.4.4.8 While statements

For a `while` statement *stmt* of the form:

`while (` *expr* `)` *while_body*

- *v* has the same definite assignment state at the beginning of *expr* as at the beginning of *stmt*.
- If *v* is definitely assigned at the end of *expr*, then it is definitely assigned on the control flow transfer to *while_body* and to the end point of *stmt*.
- If *v* has the state “definitely assigned after true expression” at the end of *expr*, then it is definitely assigned on the control flow transfer to *while_body*, but not definitely assigned at the end-point of *stmt*.
- If *v* has the state “definitely assigned after false expression” at the end of *expr*, then it is definitely assigned on the control flow transfer to the end point of *stmt*, but not definitely assigned on the control flow transfer to *while_body*.

#### 9.4.4.9 Do statements

For a `do` statement *stmt* of the form:

`do` *do_body* `while (` *expr* `) ;`

- *v* has the same definite assignment state on the control flow transfer from the beginning of *stmt* to *do_body* as at the beginning of *stmt*.
- *v* has the same definite assignment state at the beginning of *expr* as at the end point of *do_body*.
- If *v* is definitely assigned at the end of *expr*, then it is definitely assigned on the control flow transfer to the end point of *stmt*.
- If *v* has the state “definitely assigned after false expression” at the end of *expr*, then it is definitely assigned on the control flow transfer to the end point of *stmt*, but not definitely assigned on the control flow transfer to *do_body*.

#### 9.4.4.10 For statements

Definite assignment checking for a `for` statement of the form:

`for (` *for_initializer* `;` *for_condition* `;` *for_iterator* `)` *embedded_statement*

is done as if the statement were written:

```csharp
{
    «for_initializer» ;
    while ( «for_condition» )
    {
        «embedded_statement» ;
        LLoop: «for_iterator» ;
    }
}
```

with `continue` statements that target the `for` statement being translated to `goto` statements targeting the label `LLoop`. If the *for_condition* is omitted from the `for` statement, then evaluation of definite assignment proceeds as if *for_condition* were replaced with true in the above expansion.

#### 9.4.4.11 Break, continue, and goto statements

The definite assignment state of *v* on the control flow transfer caused by a `break`, `continue`, or `goto` statement is the same as the definite assignment state of *v* at the beginning of the statement.

#### 9.4.4.12 Throw statements

For a statement *stmt* of the form

`throw` *expr* `;`

the definite assignment state of *v* at the beginning of *expr* is the same as the definite assignment state of *v* at the beginning of *stmt*.

#### 9.4.4.13 Return statements

For a statement *stmt* of the form

`return` *expr* `;`

- The definite assignment state of *v* at the beginning of *expr* is the same as the definite assignment state of *v* at the beginning of *stmt*.
- If *v* is an output parameter, then it shall be definitely assigned either:
  - after *expr*
  - or at the end of the `finally` block of a `try`-`finally` or `try`-`catch`-`finally` that encloses the `return` statement.

For a statement *stmt* of the form:

`return ;`

- If *v* is an output parameter, then it shall be definitely assigned either:
  - before *stmt*
  - or at the end of the `finally` block of a `try`-`finally` or `try`-`catch`-`finally` that encloses the `return` statement.

#### 9.4.4.14 Try-catch statements

For a statement *stmt* of the form:

```csharp
try «try_block»
catch ( ... ) «catch_block_1»
...
catch ( ... ) «catch_block_n»
```

- The definite assignment state of *v* at the beginning of *try_block* is the same as the definite assignment state of *v* at the beginning of *stmt*.
- The definite assignment state of *v* at the beginning of *catch_block_i* (for any *i*) is the same as the definite assignment state of *v* at the beginning of *stmt*.
- The definite assignment state of *v* at the end-point of *stmt* is definitely assigned if (and only if) *v* is definitely assigned at the end-point of *try_block* and every *catch_block_i* (for every *i* from 1 to *n*).

#### 9.4.4.15 Try-finally statements

For a `try` statement *stmt* of the form:

`try` *try_block* `finally` *finally_block*

- The definite assignment state of *v* at the beginning of *try_block* is the same as the definite assignment state of *v* at the beginning of *stmt*.
- The definite assignment state of *v* at the beginning of *finally_block* is the same as the definite assignment state of *v* at the beginning of *stmt*.
- The definite assignment state of *v* at the end-point of *stmt* is definitely assigned if (and only if) at least one of the following is true:
  - *v* is definitely assigned at the end-point of *try_block*
  - *v* is definitely assigned at the end-point of *finally_block*

If a control flow transfer (such as a `goto` statement) is made that begins within *try_block*, and ends outside of *try_block*, then *v* is also considered definitely assigned on that control flow transfer if *v* is definitely assigned at the end-point of *finally_block*. (This is not an only if—if *v* is definitely assigned for another reason on this control flow transfer, then it is still considered definitely assigned.)

#### 9.4.4.16 Try-catch-finally statements

Definite assignment analysis for a `try`-`catch`-`finally` statement of the form:

```csharp
try «try_block»
catch ( ... ) «catch_block_1»
...
catch ( ... ) «catch_block_n»
finally «finally_block»
```

is done as if the statement were a `try`-`finally` statement enclosing a `try`-`catch` statement:

```csharp
try
{
    try «try_block»
    catch ( ... ) «catch_block_1»
    ...
    catch ( ... ) «catch_block_n»
}
finally «finally_block»
```

> *Example*: The following example demonstrates how the different blocks of a `try` statement ([§12.11](statements.md#1211-the-try-statement)) affect definite assignment.
>
> ```csharp
> class A
> {
>     static void F()
>     {
>         int i, j;
>         try
>         {
>             goto LABEL;
>             // neither i nor j definitely assigned
>             i = 1;
>             // i definitely assigned
>         }
>         catch
>         {
>             // neither i nor j definitely assigned
>             i = 3;
>             // i definitely assigned
>         }
>         finally
>         {
>             // neither i nor j definitely assigned
>             j = 5;
>             // j definitely assigned
>         }
>         // i and j definitely assigned
>         LABEL:
>         // j definitely assigned
>     }
> }
> ```
>
> *end example*

#### 9.4.4.17 Foreach statements

For a `foreach` statement *stmt* of the form:

`foreach (` *type* *identifier* `in` *expr* `)` *embedded_statement*

- The definite assignment state of *v* at the beginning of *expr* is the same as the state of *v* at the beginning of *stmt*.
- The definite assignment state of *v* on the control flow transfer to *embedded_statement* or to the end point of *stmt* is the same as the state of *v* at the end of *expr*.

#### 9.4.4.18 Using statements

For a `using` statement *stmt* of the form:

`using (` *resource_acquisition* `)` *embedded_statement*

- The definite assignment state of *v* at the beginning of *resource_acquisition* is the same as the state of *v* at the beginning of *stmt*.
- The definite assignment state of *v* on the control flow transfer to *embedded_statement* is the same as the state of *v* at the end of *resource_acquisition*.

#### 9.4.4.19 Lock statements

For a `lock` statement *stmt* of the form:

`lock (` *expr* `)` *embedded_statement*

- The definite assignment state of *v* at the beginning of *expr* is the same as the state of *v* at the beginning of *stmt*.
- The definite assignment state of *v* on the control flow transfer to *embedded_statement* is the same as the state of *v* at the end of *expr*.

#### 9.4.4.20 Yield statements

For a `yield return` statement *stmt* of the form:

`yield return` *expr* `;`

- The definite assignment state of *v* at the beginning of *expr* is the same as the state of *v* at the beginning of *stmt*.
- The definite assignment state of *v* at the end of *stmt* is the same as the state of *v* at the end of *expr*.

A `yield break` statement has no effect on the definite assignment state.

#### 9.4.4.21 General rules for constant expressions

The following applies to any constant expression, and takes priority over any rules from the following sections that might apply:

For a `constant` expression with value true:

- If *v* is definitely assigned before the expression, then *v* is definitely assigned after the expression.
- Otherwise *v* is “definitely assigned after false expression” after the expression.

> *Example*:
>
> ```csharp
> int x;
> if (true) {}
> else
> {
>     Console.WriteLine(x);
> }
> ```
>
> *end example*

For a constant expression with value `false`:

- If *v* is definitely assigned before the expression, then *v* is definitely assigned after the expression.
- Otherwise *v* is “definitely assigned after true expression” after the expression.

> *Example*:
>
> ```csharp
> int x;
> if (false)
> {
>     Console.WriteLine(x);
> }
> ```
>
> *end example*

For all other constant expressions, the definite assignment state of *v* after the expression is the same as the definite assignment state of *v* before the expression.

#### 9.4.4.22 General rules for simple expressions

The following rule applies to these kinds of expressions: literals ([§11.7.2](expressions.md#1172-literals)), simple names ([§11.7.4](expressions.md#1174-simple-names)), member access expressions ([§11.7.6](expressions.md#1176-member-access)), non-indexed base access expressions ([§11.7.13](expressions.md#11713-base-access)), `typeof` expressions ([§11.7.16](expressions.md#11716-the-typeof-operator)),  default value expressions ([§11.7.19](expressions.md#11719-default-value-expressions)), and `nameof` expressions ([§11.7.20](expressions.md#11720-nameof-expressions)).

- The definite assignment state of *v* at the end of such an expression is the same as the definite assignment state of *v* at the beginning of the expression.

#### 9.4.4.23 General rules for expressions with embedded expressions

The following rules apply to these kinds of expressions: parenthesized expressions ([§11.7.5](expressions.md#1175-parenthesized-expressions)), element access expressions ([§11.7.10](expressions.md#11710-element-access)), base access expressions with indexing ([§11.7.13](expressions.md#11713-base-access)), increment and decrement expressions ([§11.7.14](expressions.md#11714-postfix-increment-and-decrement-operators), [§11.8.6](expressions.md#1186-prefix-increment-and-decrement-operators)), cast expressions ([§11.8.7](expressions.md#1187-cast-expressions)), unary `+`, `-`, `~`, `*` expressions, binary `+`, `-`, `*`, `/`, `%`, `<<`, `>>`, `<`, `<=`, `>`, `>=`, `==`, `!=`, `is`, `as`, `&`, `|`, `^` expressions ([§11.9](expressions.md#119-arithmetic-operators), [§11.10](expressions.md#1110-shift-operators), [§11.11](expressions.md#1111-relational-and-type-testing-operators), [§11.12](expressions.md#1112-logical-operators)), compound assignment expressions ([§11.18.3](expressions.md#11183-compound-assignment)), `checked` and `unchecked` expressions ([§11.7.18](expressions.md#11718-the-checked-and-unchecked-operators)), array and delegate creation expressions ([§11.7.15](expressions.md#11715-the-new-operator)) , and `await` expressions ([§11.8.8](expressions.md#1188-await-expressions)).

Each of these expressions has one or more subexpressions that are unconditionally evaluated in a fixed order.

> *Example*: The binary `%` operator evaluates the left hand side of the operator, then the right hand side. An indexing operation evaluates the indexed expression, and then evaluates each of the index expressions, in order from left to right. *end example*

For an expression *expr*, which has subexpressions *expr₁*, *expr₂*, …, *exprₓ*, evaluated in that order:

- The definite assignment state of *v* at the beginning of *expr₁* is the same as the definite assignment state at the beginning of *expr*.
- The definite assignment state of *v* at the beginning of *exprᵢ* (*i* greater than one) is the same as the definite assignment state at the end of *exprᵢ₋₁*.
- The definite assignment state of *v* at the end of *expr* is the same as the definite assignment state at the end of *exprₓ*.

#### 9.4.4.24 Invocation expressions and object creation expressions

If the method to be invoked is a partial method that has no implementing partial method declaration, or is a conditional method for which the call is omitted ([§21.5.3.2](attributes.md#21532-conditional-methods)), then the definite assignment state of *v* after the invocation is the same as the definite assignment state of *v* before the invocation. Otherwise the following rules apply:

For an invocation expression *expr* of the form:

*primary_expression* `(` *arg₁*`,` *arg₂*`,` … `,` *argₓ* `)`

or an object creation expression *expr* of the form:

`new` *type* `(` *arg₁*`,` *arg₂*`,` … `,` *argₓ* `)`

- For an invocation expression, the definite assignment state of *v* before *primary_expression* is the same as the state of *v* before *expr*.
- For an invocation expression, the definite assignment state of *v* before *arg₁* is the same as the state of *v* after *primary_expression*.
- For an object creation expression, the definite assignment state of *v* before *arg₁* is the same as the state of *v* before *expr*.
- For each argument *argᵢ*, the definite assignment state of *v* after *argᵢ* is determined by the normal expression rules, ignoring any `ref` or `out` modifiers.
- For each argument *argᵢ* for any *i* greater than one, the definite assignment state of *v* before *argᵢ* is the same as the state of *v* after *argᵢ₋₁*.
- If the variable *v* is passed as an `out` argument (i.e., an argument of the form “out *v*”) in any of the arguments, then the state of *v* after *expr* is definitely assigned. Otherwise, the state of *v* after *expr* is the same as the state of *v* after *argₓ*.
- For array initializers ([§11.7.15.5](expressions.md#117155-array-creation-expressions)), object initializers ([§11.7.15.3](expressions.md#117153-object-initializers)), collection initializers ([§11.7.15.4](expressions.md#117154-collection-initializers)) and anonymous object initializers ([§11.7.15.7](expressions.md#117157-anonymous-object-creation-expressions)), the definite assignment state is determined by the expansion that these constructs are defined in terms of.

#### 9.4.4.25 Simple assignment expressions

For an expression *expr* of the form:

*w* `=` *expr_rhs*

- The definite assignment state of *v* before *w* is the same as the definite assignment state of *v* before *expr*.
- The definite assignment state of *v* before *expr_rhs* is the same as the definite assignment state of *v* after *w*.
- If *w* is the same variable as *v*, then the definite assignment state of *v* after *expr* is definitely assigned. Otherwise, if the assignment occurs within the instance constructor of a struct type, and *w* is a property access designating an automatically implemented property *P* on the instance being constructed and *v* is the hidden backing field of *P*, then the definite assignment state of *v* after *expr* is definitely assigned. Otherwise, the definite assignment state of *v* after *expr* is the same as the definite assignment state of *v* after *expr_rhs*.

> *Example*: In the following code
>
> ```csharp
> class A
> {
>     static void F(int[] arr)
>     {
>         int x;
>         arr[x = 1] = x; // ok
>     }
> }
> ```
>
> the variable `x` is considered definitely assigned after `arr[x = 1]` is evaluated as the left hand side of the second simple assignment. *end example*

#### 9.4.4.26 && expressions

For an expression *expr* of the form:

*expr_first* `&&` *expr_second*

- The definite assignment state of *v* before *expr_first* is the same as the definite assignment state of *v* before *expr*.
- The definite assignment state of *v* before *expr_second* is definitely assigned if and only if the state of *v* after *expr_first* is either definitely assigned or “definitely assigned after true expression”. Otherwise, it is not definitely assigned.
- The definite assignment state of *v* after *expr* is determined by:
  - If the state of *v* after *expr_first* is definitely assigned, then the state of *v* after *expr* is definitely assigned.
  - Otherwise, if the state of *v* after *expr_second* is definitely assigned, and the state of *v* after *expr_first* is “definitely assigned after false expression”, then the state of *v* after *expr* is definitely assigned.
  - Otherwise, if the state of *v* after *expr_second* is definitely assigned or “definitely assigned after true expression”, then the state of *v* after *expr* is “definitely assigned after true expression”.
  - Otherwise, if the state of *v* after *expr_first* is “definitely assigned after false expression”, and the state of *v* after *expr_second* is “definitely assigned after false expression”, then the state of *v* after *expr* is “definitely assigned after false expression”.
  - Otherwise, the state of *v* after *expr* is not definitely assigned.

> *Example*: In the following code
>
> ```csharp
> class A
> {
>     static void F(int x, int y)
>     {
>         int i;
>         if (x >= 0 && (i = y) >= 0)
>         {
>             // i definitely assigned
>         }
>         else
>         {
>             // i not definitely assigned
>         }
>         // i not definitely assigned
>     }
> }
> ```
>
> the variable `i` is considered definitely assigned in one of the embedded statements of an `if` statement but not in the other. In the `if` statement in method `F`, the variable `i` is definitely assigned in the first embedded statement because execution of the expression `(i = y)` always precedes execution of this embedded statement. In contrast, the variable `i` is not definitely assigned in the second embedded statement, since `x >= 0` might have tested false, resulting in the variable `i`’s being unassigned. *end example*

#### 9.4.4.27 || expressions

For an expression *expr* of the form:

*expr_first* `||` *expr_second*

- The definite assignment state of *v* before *expr_first* is the same as the definite assignment state of *v* before *expr*.
- The definite assignment state of *v* before *expr_second* is definitely assigned if and only if the state of *v* after *expr_first* is either definitely assigned or “definitely assigned after true expression”. Otherwise, it is not definitely assigned.
- The definite assignment statement of *v* after *expr* is determined by:
  - If the state of *v* after *expr_first* is definitely assigned, then the state of *v* after *expr* is definitely assigned.
  - Otherwise, if the state of *v* after *expr_second* is definitely assigned, and the state of *v* after *expr_first* is “definitely assigned after true expression”, then the state of *v* after *expr* is definitely assigned.
  - Otherwise, if the state of *v* after *expr_second* is definitely assigned or “definitely assigned after false expression”, then the state of *v* after *expr* is “definitely assigned after false expression”.
  - Otherwise, if the state of *v* after *expr_first* is “definitely assigned after true expression”, and the state of *v* after *expr_ second* is “definitely assigned after true expression”, then the state of *v* after *expr* is “definitely assigned after true expression”.
  - Otherwise, the state of *v* after *expr* is not definitely assigned.

> *Example*: In the following code
>
> ```csharp
> class A
> {
>     static void G(int x, int y)
>     {
>         int i;
>         if (x >= 0 || (i = y) >= 0)
>         {
>             // i not definitely assigned
>         }
>         else
>         {
>             // i definitely assigned
>         }
>         // i not definitely assigned
>     }
> }
> ```
>
> the variable `i` is considered definitely assigned in one of the embedded statements of an `if` statement but not in the other. In the `if` statement in method `G`, the variable `i` is definitely assigned in the second embedded statement because execution of the expression `(i = y)` always precedes execution of this embedded statement. In contrast, the variable `i` is not definitely assigned in the first embedded statement, since `x >= 0` might have tested true, resulting in the variable `i`’s being unassigned. *end example*

#### 9.4.4.28 ! expressions

For an expression *expr* of the form:

`!` *expr_operand*

- The definite assignment state of *v* before *expr_operand* is the same as the definite assignment state of *v* before *expr*.
- The definite assignment state of *v* after *expr* is determined by:
  - If the state of `v` after *expr_operand* is definitely assigned, then the state of `v` after *expr* is definitely assigned.
  - Otherwise, if the state of `v` after *expr_operand* is “definitely assigned after false expression”, then the state of `v` after *expr* is “definitely assigned after true expression”.
  - Otherwise, if the state of `v` after *expr_operand* is “definitely assigned after true expression”, then the state of v after *expr* is “definitely assigned after false expression”.
  - Otherwise, the state of `v` after *expr* is not definitely assigned.

#### 9.4.4.29 ?? expressions

For an expression *expr* of the form:

*expr_first* `??` *expr_second*

- The definite assignment state of *v* before *expr_first* is the same as the definite assignment state of *v* before *expr*.
- The definite assignment state of *v* before *expr_second* is the same as the definite assignment state of *v* after *expr_first*.
- The definite assignment statement of *v* after *expr* is determined by:
  - If *expr_first* is a constant expression ([§11.20](expressions.md#1120-constant-expressions)) with value `null`, then the state of *v* after *expr* is the same as the state of *v* after *expr_second*.
  - Otherwise, the state of *v* after *expr* is the same as the definite assignment state of *v* after *expr_first*.

#### 9.4.4.30 ?: expressions

For an expression *expr* of the form:

*expr_cond* `?` *expr_true* `:` *expr_false*

- The definite assignment state of *v* before *expr_cond* is the same as the state of *v* before *expr*.
- The definite assignment state of *v* before *expr_true* is definitely assigned if the state of *v* after *expr_cond* is definitely assigned or “definitely assigned after true expression”.
- The definite assignment state of *v* before *expr_false* is definitely assigned if the state of *v* after *expr_cond* is definitely assigned or “definitely assigned after false expression”.
- The definite assignment state of *v* after *expr* is determined by:
  - If *expr_cond* is a constant expression ([§11.20](expressions.md#1120-constant-expressions)) with value `true` then the state of *v* after *expr* is the same as the state of *v* after *expr_true*.
  - Otherwise, if *expr_cond* is a constant expression ([§11.20](expressions.md#1120-constant-expressions)) with value `false` then the state of *v* after *expr* is the same as the state of *v* after *expr_false*.
  - Otherwise, if the state of *v* after *expr_true* is definitely assigned and the state of *v* after *expr_false* is definitely assigned, then the state of *v* after *expr* is definitely assigned.
  - Otherwise, the state of *v* after *expr* is not definitely assigned.

#### 9.4.4.31 Anonymous functions

For a *lambda_expression* or *anonymous_method_expression* *expr* with a body (either *block* or *expression*) *body*:

- The definite assignment state of a parameter is the same as for a parameter of a named method ([§9.2.6](variables.md#926-reference-parameters), [§9.2.7](variables.md#927-output-parameters)).
- The definite assignment state of an outer variable *v* before *body* is the same as the state of *v* before *expr*. That is, definite assignment state of outer variables is inherited from the context of the anonymous function.
- The definite assignment state of an outer variable *v* after *expr* is the same as the state of *v* before *expr*.

> *Example*: The example
>
> ```csharp
> delegate bool Filter(int i);
> void F()
> {
>     int max;
>     // Error, max is not definitely assigned
>     Filter f = (int n) => n < max;
>     max = 5;
>     DoWork(f);
> }
> ```
>
> generates a compile-time error since max is not definitely assigned where the anonymous function is declared. *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The example
>
> ```csharp
> delegate void D();
> void F()
> {
>     int n;
>     D d = () => { n = 1; };
>     d();
>     // Error, n is not definitely assigned
>     Console.WriteLine(n);
> }
> ```
>
> also generates a compile-time error since the assignment to `n` in the anonymous function has no affect on the definite assignment state of `n` outside the anonymous function. *end example*

## 9.5 Variable references

A *variable_reference* is an *expression* that is classified as a variable. A *variable_reference* denotes a storage location that can be accessed both to fetch the current value and to store a new value.

```ANTLR
variable_reference
    : expression
    ;
```

> *Note*: In C and C++, a *variable_reference* is known as an *lvalue*. *end note*

## 9.6 Atomicity of variable references

Reads and writes of the following data types shall be atomic: `bool`, `char`, `byte`, `sbyte`, `short`, `ushort`, `uint`, `int`, `float`, and reference types. In addition, reads and writes of enum types with an underlying type in the previous list shall also be atomic. Reads and writes of other types, including `long`, `ulong`, `double`, and `decimal`, as well as user-defined types, need not be atomic. Aside from the library functions designed for that purpose, there is no guarantee of atomic read-modify-write, such as in the case of increment or decrement.
