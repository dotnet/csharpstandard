# 9 Variables

## 9.1 General

Variables represent storage locations. Every variable has a type that determines what values can be stored in the variable. C# is a type-safe language, and the C# compiler guarantees that values stored in variables are always of the appropriate type. The value of a variable can be changed through assignment or through use of the `++` and `--` operators.

A variable shall be *definitely assigned* ([§9.4](variables.md#94-definite-assignment)) before its value can be obtained.

As described in the following subclauses, variables are either ***initially assigned*** or ***initially unassigned***. An initially assigned variable has a well-defined initial value and is always considered definitely assigned. An initially unassigned variable has no initial value. For an initially unassigned variable to be considered definitely assigned at a certain location, an assignment to the variable shall occur in every possible execution path leading to that location.

## 9.2 Variable categories

### 9.2.1 General

C# defines eight categories of variables: static variables, instance variables, array elements, value parameters, input parameters, reference parameters, output parameters, and local variables. The subclauses that follow describe each of these categories.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"VariableCategories", ignoredWarnings:["CS0169","CS0219","CS0649"]} -->
> ```csharp
> class A
> {
>     public static int x;
>     int y;
> 
>     void F(int[] v, int a, ref int b, out int c, in int d)
>     {
>         int i = 1;
>         c = a + b++ + d;
>     }
> }
> ```
>
> `x` is a static variable, `y` is an instance variable, `v[0]` is an array element, `a` is a value parameter, `b` is a reference parameter, `c` is an output parameter, `d` is an input parameter, and `i` is a local variable. *end example*

### 9.2.2 Static variables

A field declared with the `static` modifier is a static variable. A static variable comes into existence before execution of the `static` constructor ([§15.12](classes.md#1512-static-constructors)) for its containing type, and ceases to exist when the associated application domain ceases to exist.

The initial value of a static variable is the default value ([§9.3](variables.md#93-default-values)) of the variable’s type.

For the purposes of definite-assignment checking, a static variable is considered initially assigned.

### 9.2.3 Instance variables

#### 9.2.3.1 General

A field declared without the `static` modifier is an instance variable.

#### 9.2.3.2 Instance variables in classes

An instance variable of a class comes into existence when a new instance of that class is created, and ceases to exist when there are no references to that instance and the instance’s finalizer (if any) has executed.

The initial value of an instance variable of a class is the default value ([§9.3](variables.md#93-default-values)) of the variable’s type.

For the purpose of definite-assignment checking, an instance variable of a class is considered initially assigned.

#### 9.2.3.3 Instance variables in structs

An instance variable of a struct has exactly the same lifetime as the struct variable to which it belongs. In other words, when a variable of a struct type comes into existence or ceases to exist, so too do the instance variables of the struct.

The initial assignment state of an instance variable of a struct is the same as that of the containing `struct` variable. In other words, when a struct variable is considered initially assigned, so too are its instance variables, and when a struct variable is considered initially unassigned, its instance variables are likewise unassigned.

### 9.2.4 Array elements

The elements of an array come into existence when an array instance is created, and cease to exist when there are no references to that array instance.

The initial value of each of the elements of an array is the default value ([§9.3](variables.md#93-default-values)) of the type of the array elements.

For the purpose of definite-assignment checking, an array element is considered initially assigned.

### 9.2.5 Value parameters

A value parameter comes into existence upon invocation of the function member (method, instance constructor, accessor, or operator) or anonymous function to which the parameter belongs, and is initialized with the value of the argument given in the invocation. A value parameter normally ceases to exist when execution of the function body completes. However, if the value parameter is captured by a non-`static` anonymous function ([§12.22.6.2](expressions.md#122262-captured-outer-variables)), its lifetime extends at least until the delegate or expression tree created from that anonymous function is eligible for garbage collection.

For the purpose of definite-assignment checking, a value parameter is considered initially assigned.

Value parameters are discussed further in [§15.6.2.2](classes.md#15622-value-parameters).

### 9.2.6 Reference parameters

A reference parameter is a reference variable ([§9.7](variables.md#97-reference-variables-and-returns)) which comes into existence upon invocation of the function member, delegate, anonymous function, or local function and its referent is initialized to the variable given as the argument in that invocation. A reference parameter ceases to exist when execution of the function body completes. Unlike value parameters a reference parameter shall not be captured ([§9.7.2.9](variables.md#9729-limitations-on-reference-variables)).

The following definite-assignment rules apply to reference parameters.

> *Note*: The rules for output parameters are different, and are described in ([§9.2.7](variables.md#927-output-parameters)). *end note*

- A variable shall be definitely assigned ([§9.4](variables.md#94-definite-assignment)) before it can be passed as a reference parameter in a function member or delegate invocation.
- Within a function member or anonymous function, a reference parameter is considered initially assigned.

Reference parameters are discussed further in [§15.6.2.3.3](classes.md#156233-reference-parameters).

### 9.2.7 Output parameters

An output parameter is a reference variable ([§9.7](variables.md#97-reference-variables-and-returns)) which comes into existence upon invocation of the function member, delegate, anonymous function, or local function and its referent is initialized to the variable given as the argument in that invocation. An output parameter ceases to exist when execution of the function body completes. Unlike value parameters an output parameter shall not be captured ([§9.7.2.9](variables.md#9729-limitations-on-reference-variables)).

The following definite-assignment rules apply to output parameters.

> *Note*: The rules for reference parameters are different, and are described in ([§9.2.6](variables.md#926-reference-parameters)). *end note*

- A variable need not be definitely assigned before it can be passed as an output parameter in a function member or delegate invocation.
- Following the normal completion of a function member or delegate invocation, each variable that was passed as an output parameter is considered assigned in that execution path.
- Within a function member or anonymous function, an output parameter is considered initially unassigned.
- Every output parameter of a function member, anonymous function, or local function shall be definitely assigned ([§9.4](variables.md#94-definite-assignment)) before the function member, anonymous function, or local function returns normally.

Output parameters are discussed further in [§15.6.2.3.4](classes.md#156234-output-parameters).

### 9.2.8 Input parameters

An input parameter is a reference variable ([§9.7](variables.md#97-reference-variables-and-returns)) which comes into existence upon invocation of the function member, delegate, anonymous function, or local function and its referent is initialized to the *variable_reference* given as the argument in that invocation. An input parameter ceases to exist when execution of the function body completes. Unlike value parameters an input parameter shall not be captured ([§9.7.2.9](variables.md#9729-limitations-on-reference-variables)).

The following definite assignment rules apply to input parameters.

- A variable shall be definitely assigned ([§9.4](variables.md#94-definite-assignment)) before it can be passed as an input parameter in a function member or delegate invocation.
- Within a function member, anonymous function, or local function an input parameter is considered initially assigned.

Input parameters are discussed further in [§15.6.2.3.2](classes.md#156232-input-parameters).

### 9.2.9 Local variables

#### 9.2.9.1 General

A ***local variable*** is declared by a *local_variable_declaration*, *declaration_expression*, *foreach_statement*, or *specific_catch_clause* of a *try_statement*. A local variable can also be declared by certain kinds of *pattern*s ([§11](patterns.md#11-patterns-and-pattern-matching)). For a *foreach_statement*, the local variable is an iteration variable ([§13.9.5](statements.md#1395-the-foreach-statement)). For a *specific_catch_clause*, the local variable is an exception variable ([§13.11](statements.md#1311-the-try-statement)). A local variable declared by a *foreach_statement* or *specific_catch_clause* is considered initially assigned.

A *local_variable_declaration* can occur in a *block*, a *for_statement*, a *switch_block*, or a *using_statement*. A *declaration_expression* can occur as an `out` *argument_value*, and as a *tuple_element* that is the target of a deconstructing assignment ([§12.24.2](expressions.md#12242-simple-assignment)).

The lifetime of a local variable is the portion of program execution during which storage is guaranteed to be reserved for it. This lifetime extends from entry into the scope with which it is associated, at least until execution of that scope ends in some way. (Entering an enclosed *block*, calling a method, or yielding a value from an iterator block suspends, but does not end, execution of the current scope.) If the local variable is captured by a non-`static` anonymous function ([§12.22.6.2](expressions.md#122262-captured-outer-variables)), its lifetime extends at least until the delegate or expression tree created from the anonymous function, along with any other objects that come to reference the captured variable, are eligible for garbage collection. If the parent scope is entered recursively or iteratively, a new instance of the local variable is created each time, and its initializer, if any, is evaluated each time.

> *Note*: A local variable is instantiated each time its scope is entered. This behavior is visible to user code containing anonymous methods. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The lifetime of an *iteration variable* ([§13.9.5](statements.md#1395-the-foreach-statement)) declared by a *foreach_statement* is a single iteration of that statement. Each iteration creates a new variable. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The actual lifetime of a local variable is implementation-dependent. For example, a compiler might statically determine that a local variable in a block is only used for a small portion of that block. Using this analysis, a compiler could generate code that results in the variable’s storage having a shorter lifetime than its containing block.
>
> The storage referred to by a local reference variable is reclaimed independently of the lifetime of that local reference variable ([§7.9](basic-concepts.md#79-automatic-memory-management)).
>
> *end note*

A local variable introduced by a *local_variable_declaration* or *declaration_expression* is not automatically initialized and thus has no default value. Such a local variable is considered initially unassigned.

> *Note*: A *local_variable_declaration* that includes an initializer is still initially unassigned. Execution of the declaration behaves exactly like an assignment to the variable ([§9.4.4.5](variables.md#9445-declaration-statements)). Using a variable before its initializer has been executed; e.g., within the initializer expression itself or by using a *goto_statement* which bypasses the initializer; is a compile-time error:
>
> <!-- Example: {template:"code-in-main-without-using", name:"LocalVariables", expectedErrors:["CS0165"], expectedWarnings:["CS0162"]} -->
> ```csharp
> goto L;
> 
> int x = 1; // never executed
> 
> L: x += 1; // error: x not definitely assigned
> ```
>
> Within the scope of a local variable, it is a compile-time error to refer to that local variable in a textual position that precedes its declarator.
>
> *end note*

#### 9.2.9.2 Discards

A ***discard*** is a local variable that has no name. A discard is introduced by a declaration expression ([§12.20](expressions.md#1220-declaration-expressions)) with the identifier `_`; and is either implicitly typed (`_` or `var _`) or explicitly typed (`T _`). A discard can also be introduced as a parameter of an anonymous function ([§12.22.2](expressions.md#12222-anonymous-function-signatures)).

> *Note*: `_` is a valid identifier in many forms of declarations. *end note*
  
Because a discard has no name, the only reference to the variable it represents is the expression that introduces it.

> *Note*: A discard can however be passed as an output argument, allowing the corresponding output parameter to denote its associated storage location. *end note*

A discard is not initially assigned, so it is always an error to access its value.

> *Example*:
>
> <!-- Example: {template:"standalone-console", name:"Discards1", replaceEllipsis:true, customEllipsisReplacements: ["i1 = 0; i2 = 0; i3 = 0; return (0,0,0);"]} -->
> ```csharp
> _ = "Hello".Length;
> (int, int, int) M(out int i1, out int i2, out int i3) { ... }
> (int _, var _, _) = M(out int _, out var _, out _);
> ```
>
> The example assumes that there is no declaration of the name `_` in scope.
>
> The assignment to `_` shows a simple pattern for ignoring the result of an expression.
> The call of `M` shows the different forms of discards available in tuples and as output parameters.
>
> *end example*

## 9.3 Default values

The following categories of variables are automatically initialized to their default values:

- Static variables.
- Instance variables of class and struct instances.
- Array elements.

The default value of a variable depends on the type of the variable and is determined as follows:

- For a variable of a *value_type*, the default value is the same as the value computed by the *value_type*’s default constructor ([§8.3.3](types.md#833-default-constructors)).
- For a variable of a *reference_type* or a reference variable, the default value is `null`.

> *Note*: Initialization to default values is typically done by having the memory manager or garbage collector initialize memory to all-bits-zero before it is allocated for use. For this reason, it is convenient to use all-bits-zero to represent the null reference. *end note*

> *Note*: To test if a ref variable has been assigned a referent, call `System.Runtime.CompilerServices.Unsafe.IsNullRef(ref fieldName)`. One cannot test a ref variable to see if it has been assigned a referent by using `fieldName == null`, as that tests the value of the (potentially non-existent) referent, not the reference itself. *end note*

## 9.4 Definite assignment

### 9.4.1 General

At a given location in the executable code of a function member or an anonymous function, a variable is said to be ***definitely assigned*** if a compiler can prove, by a particular static flow analysis ([§9.4.4](variables.md#944-precise-rules-for-determining-definite-assignment)), that the variable has been automatically initialized or has been the target of at least one assignment.

> *Note*: Informally stated, the rules of definite assignment are:
>
> - An initially assigned variable ([§9.4.2](variables.md#942-initially-assigned-variables)) is always considered definitely assigned.
> - An initially unassigned variable ([§9.4.3](variables.md#943-initially-unassigned-variables)) is considered definitely assigned at a given location if all possible execution paths leading to that location contain at least one of the following:
>   - A simple assignment ([§12.24.2](expressions.md#12242-simple-assignment)) in which the variable is the left operand.
>   - A deconstructing assignment ([§12.23.3](expressions.md#12233-query-expression-translation)) in which the variable occurs as a *deconstructor_element* in the *deconstructor*, including in any nested *deconstructor*s.
>   - An invocation expression ([§12.8.10](expressions.md#12810-invocation-expressions)) or object creation expression ([§12.8.17.2](expressions.md#128172-object-creation-expressions)) that passes the variable as an output parameter.
>   - For a local variable:
>     - a local variable declaration for the variable ([§13.6.2](statements.md#1362-local-variable-declarations)) that includes a variable initializer; or
>     - a deconstructing assignment ([§12.23.3](expressions.md#12233-query-expression-translation)) which declares the variable in its *destructor*.
>
> The formal specification underlying the above informal rules is described in [§9.4.2](variables.md#942-initially-assigned-variables), [§9.4.3](variables.md#943-initially-unassigned-variables), and [§9.4.4](variables.md#944-precise-rules-for-determining-definite-assignment).
>
> *end note*

The definite-assignment states of instance variables of a *struct_type* variable are tracked individually as well as collectively. In additional to the rules described in [§9.4.2](variables.md#942-initially-assigned-variables), [§9.4.3](variables.md#943-initially-unassigned-variables), and [§9.4.4](variables.md#944-precise-rules-for-determining-definite-assignment), the following rules apply to *struct_type* variables and their instance variables:

- An instance variable is considered definitely assigned if its containing *struct_type* variable is considered definitely assigned.
- A *struct_type* variable is considered definitely assigned if each of its instance variables is considered definitely assigned.

Definite assignment is a requirement in the following contexts:

- A variable shall be definitely assigned at each location where its value is obtained.
  > *Note*: This ensures that undefined values never occur. *end note*

  The occurrence of a variable in an expression is considered to obtain the value of the variable, except when:
  - the variable is the left operand of a simple assignment,
  - the variable is part of the left operand of a deconstructing assignment,
  - the variable is passed as an output parameter, or
  - the variable is a *struct_type* variable and occurs as the left operand of a member access.
- A variable shall be definitely assigned at each location where it is passed as a reference parameter.
  > *Note*: This ensures that the function member being invoked can consider the reference parameter initially assigned. *end note*
- A variable shall be definitely assigned at each location where it is passed as an input parameter.
  > *Note*: This ensures that the function member being invoked can consider the input parameter initially assigned. *end note*
- All output parameters of a function member shall be definitely assigned at each location where the function member returns (through a return statement or through execution reaching the end of the function member body).
  > *Note*: This ensures that function members do not return undefined values in output parameters, thus enabling a compiler to consider a function member invocation that takes a variable as an output parameter equivalent to an assignment to the variable. *end note*
- The `this` variable of a *struct_type* instance constructor shall be definitely assigned at each location where that instance constructor returns.

### 9.4.2 Initially assigned variables

The following categories of variables are classified as initially assigned:

- Static variables.
- Instance variables of class instances.
- Instance variables of initially assigned struct variables.
- Array elements.
- Value parameters.
- Reference parameters.
- Input parameters.
- Variables declared in a `catch` clause or a `foreach` statement.

### 9.4.3 Initially unassigned variables

The following categories of variables are classified as initially unassigned:

- Instance variables of initially unassigned struct variables.
- Output parameters, including the `this` variable of struct instance constructors without a constructor initializer.
- Local variables, except those declared in a `catch` clause or a `foreach` statement.

### 9.4.4 Precise rules for determining definite assignment

#### 9.4.4.1 General

In order to determine that each used variable is definitely assigned, a compiler shall use a process that is equivalent to the one described in this subclause.

The body of a function member may declare one or more initially unassigned variables. For each initially unassigned variable *v*, a compiler shall determine a ***definite-assignment state*** for *v* at each of the following points in the function member:

- At the beginning of each statement
- At the end point ([§13.2](statements.md#132-end-points-and-reachability)) of each statement
- On each arc which transfers control to another statement or to the end point of a statement
- At the beginning of each expression
- At the end of each expression

The definite-assignment state of *v* can be either:

- Definitely assigned. This indicates that on all possible control flows to this point, *v* has been assigned a value.
- Not definitely assigned. For the state of a variable at the end of an expression of type `bool`, the state of a variable that is not definitely assigned might (but does not necessarily) fall into one of the following sub-states:
  - Definitely assigned after true expression. This state indicates that *v* is definitely assigned if the Boolean expression evaluated as true, but is not necessarily assigned if the Boolean expression evaluated as false.
  - Definitely assigned after false expression. This state indicates that *v* is definitely assigned if the Boolean expression evaluated as false, but is not necessarily assigned if the Boolean expression evaluated as true.

The following rules govern how the state of a variable *v* is determined at each location.

#### 9.4.4.2 General rules for statements

- *v* is not definitely assigned at the beginning of a function member body.
- The definite-assignment state of *v* at the beginning of any other statement is determined by checking the definite-assignment state of *v* on all control flow transfers that target the beginning of that statement. If (and only if) *v* is definitely assigned on all such control flow transfers, then *v* is definitely assigned at the beginning of the statement. The set of possible control flow transfers is determined in the same way as for checking statement reachability ([§13.2](statements.md#132-end-points-and-reachability)).
- The definite-assignment state of *v* at the end point of a `block`, `checked`, `unchecked`, `if`, `while`, `do`, `for`, `foreach`, `lock`, `using`, or `switch` statement is determined by checking the definite-assignment state of *v* on all control flow transfers that target the end point of that statement. If *v* is definitely assigned on all such control flow transfers, then *v* is definitely assigned at the end point of the statement. Otherwise, *v* is not definitely assigned at the end point of the statement. The set of possible control flow transfers is determined in the same way as for checking statement reachability ([§13.2](statements.md#132-end-points-and-reachability)).

> *Note*: Because there are no control paths to an unreachable statement, *v* is definitely assigned at the beginning of any unreachable statement. *end note*

#### 9.4.4.3 Block statements, checked, and unchecked statements

The definite-assignment state of *v* on the control transfer to the first statement of the statement list in the block (or to the end point of the block, if the statement list is empty) is the same as the definite-assignment state of *v* before the block, `checked`, or `unchecked` statement.

#### 9.4.4.4 Expression statements

For an expression statement *stmt* that consists of the expression *expr*:

- *v* has the same definite-assignment state at the beginning of *expr* as at the beginning of *stmt*.
- If *v* is definitely assigned at the end of *expr*, it is definitely assigned at the end point of *stmt*; otherwise, it is not definitely assigned at the end point of *stmt*.

#### 9.4.4.5 Declaration statements

- If *stmt* is a declaration statement without initializers, then *v* has the same definite-assignment state at the end point of *stmt* as at the beginning of *stmt*.
- If *stmt* is a declaration statement with initializers, then the definite-assignment state for *v* is determined as if *stmt* were a statement list, with one assignment statement for each declaration with an initializer (in the order of declaration).

#### 9.4.4.6 If statements

For a statement *stmt* of the form:

```csharp
if ( «expr» ) «then_stmt» else «else_stmt»
```

- *v* has the same definite-assignment state at the beginning of *expr* as at the beginning of *stmt*.
- If *v* is definitely assigned at the end of *expr*, then it is definitely assigned on the control flow transfer to *then_stmt* and to either *else_stmt* or to the end-point of *stmt* if there is no else clause.
- If *v* has the state “definitely assigned after true expression” at the end of *expr*, then it is definitely assigned on the control flow transfer to *then_stmt*, and not definitely assigned on the control flow transfer to either *else_stmt* or to the end-point of *stmt* if there is no else clause.
- If *v* has the state “definitely assigned after false expression” at the end of *expr*, then it is definitely assigned on the control flow transfer to *else_stmt*, and not definitely assigned on the control flow transfer to *then_stmt*. It is definitely assigned at the end-point of *stmt* if and only if it is definitely assigned at the end-point of *then_stmt*.
- Otherwise, *v* is considered not definitely assigned on the control flow transfer to either the *then_stmt* or *else_stmt*, or to the end-point of *stmt* if there is no else clause.

#### 9.4.4.7 Switch statements

For a `switch` statement *stmt* with a controlling expression *expr*:

The definite-assignment state of *v* at the beginning of *expr* is the same as the state of *v* at the beginning of *stmt*.

The definite-assignment state of *v* at the beginning of a case’s guard clause is

- If *v* is a pattern variable declared in the *switch_label*: “definitely assigned”.
- If the switch label containing that guard clause ([§13.8.3](statements.md#1383-the-switch-statement)) is not reachable: “definitely assigned”.
- Otherwise, the state of *v* is the same as the state of *v* after *expr*.

> *Example*: The second rule eliminates the need for a compiler to issue an error if an unassigned variable is accessed in unreachable code. The state of *b* is “definitely assigned” in the unreachable switch label `case 2 when b`.
>
> <!-- Example: {template:"standalone-console-without-using", name:"DefAssignSwitch", expectedWarnings:["CS0162"]} -->
> ```csharp
> bool b;
> switch (1) 
> {
>     case 2 when b: // b is definitely assigned here.
>     break;
> }
> ```
>
> *end example*

The definite-assignment state of *v* on the control flow transfer to a reachable switch block statement list is

- If the control transfer was due to a ‘goto case’ or ‘goto default’ statement, then the state of *v* is the same as the state at the beginning of that ‘goto’ statement.
- If the control transfer was due to the `default` label of the switch, then the state of *v* is the same as the state of *v* after *expr*.
- If the control transfer was due to an unreachable switch label, then the state of *v* is “definitely assigned”.
- If the control transfer was due to a reachable switch label with a guard clause, then the state of *v* is the same as the state of *v* after the guard clause.
- If the control transfer was due to a reachable switch label without a guard clause, then the state of *v* is
  - If *v* is a pattern variable declared in the *switch_label*: “definitely assigned”.
  - Otherwise, the state of *v* is the same as the state of *v* after *expr*.

A consequence of these rules is that a pattern variable declared in a *switch_label* will be “not definitely assigned” in the statements of its switch section if it is not the only reachable switch label in its section.

> *Example*:
>
> ```csharp
> public static double ComputeArea(object shape)
> {
>     switch (shape)
>     {
>         case Square s when s.Side == 0:
>         case Circle c when c.Radius == 0:
>         case Triangle t when t.Base == 0 || t.Height == 0:
>         case Rectangle r when r.Length == 0 || r.Height == 0:
>             // none of s, c, t, or r is definitely assigned
>             return 0;
>         case Square s:
>             // s is definitely assigned
>             return s.Side * s.Side;
>         case Circle c:
>             // c is definitely assigned
>             return c.Radius * c.Radius * Math.PI;
>            …
>     }
> }
> ```
>
> *end example*

#### 9.4.4.8 While statements

For a statement *stmt* of the form:

```csharp
while ( «expr» ) «while_body»
```

- *v* has the same definite-assignment state at the beginning of *expr* as at the beginning of *stmt*.
- If *v* is definitely assigned at the end of *expr*, then it is definitely assigned on the control flow transfer to *while_body* and to the end point of *stmt*.
- If *v* has the state “definitely assigned after true expression” at the end of *expr*, then it is definitely assigned on the control flow transfer to *while_body*, but not definitely assigned at the end-point of *stmt*.
- If *v* has the state “definitely assigned after false expression” at the end of *expr*, then it is definitely assigned on the control flow transfer to the end point of *stmt*, but not definitely assigned on the control flow transfer to *while_body*.

#### 9.4.4.9 Do statements

For a statement *stmt* of the form:

```csharp
do «do_body» while ( «expr» ) ;
```

- *v* has the same definite-assignment state on the control flow transfer from the beginning of *stmt* to *do_body* as at the beginning of *stmt*.
- *v* has the same definite-assignment state at the beginning of *expr* as at the end point of *do_body*.
- If *v* is definitely assigned at the end of *expr*, then it is definitely assigned on the control flow transfer to the end point of *stmt*.
- If *v* has the state “definitely assigned after false expression” at the end of *expr*, then it is definitely assigned on the control flow transfer to the end point of *stmt*, but not definitely assigned on the control flow transfer to *do_body*.

#### 9.4.4.10 For statements

For a statement of the form:

```csharp
for ( «for_initializer» ; «for_condition» ; «for_iterator» )
    «embedded_statement»
```

definite-assignment checking is done as if the statement were written:

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

with `continue` statements that target the `for` statement being translated to `goto` statements targeting the label `LLoop`. If the *for_condition* is omitted from the `for` statement, then evaluation of definite-assignment proceeds as if *for_condition* were replaced with true in the above expansion.

#### 9.4.4.11 Break, continue, and goto statements

The definite-assignment state of *v* on the control flow transfer caused by a `break`, `continue`, or `goto` statement is the same as the definite-assignment state of *v* at the beginning of the statement.

#### 9.4.4.12 Throw statements

For a statement *stmt* of the form:

```csharp
throw «expr» ;
```

the definite-assignment state of *v* at the beginning of *expr* is the same as the definite-assignment state of *v* at the beginning of *stmt*.

#### 9.4.4.13 Return statements

For a statement *stmt* of the form:

```csharp
return «expr» ;
```

- The definite-assignment state of *v* at the beginning of *expr* is the same as the definite-assignment state of *v* at the beginning of *stmt*.
- If *v* is an output parameter, then it shall be definitely assigned either:
  - after *expr*
  - or at the end of the `finally` block of a `try`-`finally` or `try`-`catch`-`finally` that encloses the `return` statement.

For a statement *stmt* of the form:

```csharp
return ;
```

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

- The definite-assignment state of *v* at the beginning of *try_block* is the same as the definite-assignment state of *v* at the beginning of *stmt*.
- The definite-assignment state of *v* at the beginning of *catch_block_i* (for any *i*) is the same as the definite-assignment state of *v* at the beginning of *stmt*.
- The definite-assignment state of *v* at the end-point of *stmt* is definitely assigned if (and only if) *v* is definitely assigned at the end-point of *try_block* and every *catch_block_i* (for every *i* from 1 to *n*).

#### 9.4.4.15 Try-finally statements

For a statement *stmt* of the form:

```csharp
try «try_block» finally «finally_block»
```

- The definite-assignment state of *v* at the beginning of *try_block* is the same as the definite-assignment state of *v* at the beginning of *stmt*.
- The definite-assignment state of *v* at the beginning of *finally_block* is the same as the definite-assignment state of *v* at the beginning of *stmt*.
- The definite-assignment state of *v* at the end-point of *stmt* is definitely assigned if (and only if) at least one of the following is true:
  - *v* is definitely assigned at the end-point of *try_block*
  - *v* is definitely assigned at the end-point of *finally_block*

If a control flow transfer (such as a `goto` statement) is made that begins within *try_block*, and ends outside of *try_block*, then *v* is also considered definitely assigned on that control flow transfer if *v* is definitely assigned at the end-point of *finally_block*. (This is not an only if—if *v* is definitely assigned for another reason on this control flow transfer, then it is still considered definitely assigned.)

#### 9.4.4.16 Try-catch-finally statements

For a statement of the form:

```csharp
try «try_block»
catch ( ... ) «catch_block_1»
...
catch ( ... ) «catch_block_n»
finally «finally_block»
```

definite-assignment analysis is done as if the statement were a `try`-`finally` statement enclosing a `try`-`catch` statement:

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

> *Example*: The following example demonstrates how the different blocks of a `try` statement ([§13.11](statements.md#1311-the-try-statement)) affect definite assignment.
>
> <!-- Example: {template:"standalone-lib-without-using", name:"TryCatchFinally", expectedWarnings:["CS0162"], ignoredWarnings:["CS0219"]} -->
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
>         LABEL: ;
>         // j definitely assigned
>     }
> }
> ```
>
> *end example*

#### 9.4.4.17 Foreach statements

For a statement *stmt* of the form:

```csharp
foreach ( «type» «identifier» in «expr» ) «embedded_statement»
```

- The definite-assignment state of *v* at the beginning of *expr* is the same as the state of *v* at the beginning of *stmt*.
- The definite-assignment state of *v* on the control flow transfer to *embedded_statement* or to the end point of *stmt* is the same as the state of *v* at the end of *expr*.

#### 9.4.4.18 Using statements

For a statement *stmt* of the form:

```csharp
using ( «resource_acquisition» ) «embedded_statement»
```

- The definite-assignment state of *v* at the beginning of *resource_acquisition* is the same as the state of *v* at the beginning of *stmt*.
- The definite-assignment state of *v* on the control flow transfer to *embedded_statement* is the same as the state of *v* at the end of *resource_acquisition*.

#### 9.4.4.19 Lock statements

For a statement *stmt* of the form:

```csharp
lock ( «expr» ) «embedded_statement»
```

- The definite-assignment state of *v* at the beginning of *expr* is the same as the state of *v* at the beginning of *stmt*.
- The definite-assignment state of *v* on the control flow transfer to *embedded_statement* is the same as the state of *v* at the end of *expr*.

#### 9.4.4.20 Yield statements

For a statement *stmt* of the form:

```csharp
yield return «expr» ;
```

- The definite-assignment state of *v* at the beginning of *expr* is the same as the state of *v* at the beginning of *stmt*.
- The definite-assignment state of *v* at the end of *stmt* is the same as the state of *v* at the end of *expr*.

A `yield break` statement has no effect on the definite-assignment state.

#### 9.4.4.21 General rules for constant expressions

The following applies to any constant expression, and takes priority over any rules from the following subclauses that might apply:

For a constant expression with value `true`:

- If *v* is definitely assigned before the expression, then *v* is definitely assigned after the expression.
- Otherwise *v* is “definitely assigned after false expression” after the expression.

> *Example*:
>
> <!-- Example: {template:"standalone-console", name:"ConstantExpressions1", expectedWarnings:["CS0162"]} -->
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
> <!-- Example: {template:"standalone-console", name:"ConstantExpressions2", expectedWarnings:["CS0162"]} -->
> ```csharp
> int x;
> if (false)
> {
>     Console.WriteLine(x);
> }
> ```
>
> *end example*

For all other constant expressions, the definite-assignment state of *v* after the expression is the same as the definite-assignment state of *v* before the expression.

#### 9.4.4.22 General rules for simple expressions

The following rule applies to these kinds of expressions: literals ([§12.8.2](expressions.md#1282-literals)), simple names ([§12.8.4](expressions.md#1284-simple-names)), member access expressions ([§12.8.7](expressions.md#1287-member-access)), non-indexed base access expressions ([§12.8.15](expressions.md#12815-base-access)), `typeof` expressions ([§12.8.18](expressions.md#12818-the-typeof-operator)),  default value expressions ([§12.8.21](expressions.md#12821-default-value-expressions)), `nameof` expressions ([§12.8.23](expressions.md#12823-the-nameof-operator)), and declaration expressions ([§12.20](expressions.md#1220-declaration-expressions)).

- The definite-assignment state of *v* at the end of such an expression is the same as the definite-assignment state of *v* at the beginning of the expression.

#### 9.4.4.23 General rules for expressions with embedded expressions

The following rules apply to these kinds of expressions: parenthesized expressions ([§12.8.5](expressions.md#1285-parenthesized-expressions)), tuple expressions ([§12.8.6](expressions.md#1286-tuple-literals)), element access expressions ([§12.8.12](expressions.md#12812-element-access)), base access expressions with indexing ([§12.8.15](expressions.md#12815-base-access)), increment and decrement expressions ([§12.8.16](expressions.md#12816-postfix-increment-and-decrement-operators), [§12.9.7](expressions.md#1297-prefix-increment-and-decrement-operators)), cast expressions ([§12.9.8](expressions.md#1298-cast-expressions)), unary `+`, `-`, `~`, `*` expressions, binary `+`, `-`, `*`, `/`, `%`, `<<`, `>>`, `>>>`, `<`, `<=`, `>`, `>=`, `==`, `!=`, `is`, `as`, `&`, `|`, `^` expressions ([§12.13](expressions.md#1213-arithmetic-operators), [§12.14](expressions.md#1214-shift-operators), [§12.15](expressions.md#1215-relational-and-type-testing-operators), [§12.16](expressions.md#1216-logical-operators)), compound assignment expressions ([§12.24.5](expressions.md#12245-compound-assignment)), `checked` and `unchecked` expressions ([§12.8.20](expressions.md#12820-the-checked-and-unchecked-operators)), array and delegate creation expressions ([§12.8.17](expressions.md#12817-the-new-operator)) , and `await` expressions ([§12.9.9](expressions.md#1299-await-expressions)).

Each of these expressions has one or more subexpressions that are unconditionally evaluated in a fixed order.

> *Example*: The binary `%` operator evaluates the left hand side of the operator, then the right hand side. An indexing operation evaluates the indexed expression, and then evaluates each of the index expressions, in order from left to right. *end example*

For an expression *expr*, which has subexpressions *expr₁*, *expr₂*, …, *exprₓ*, evaluated in that order:

- The definite-assignment state of *v* at the beginning of *expr₁* is the same as the definite-assignment state at the beginning of *expr*.
- The definite-assignment state of *v* at the beginning of *exprᵢ* (*i* greater than one) is the same as the definite-assignment state at the end of *exprᵢ₋₁*.
- The definite-assignment state of *v* at the end of *expr* is the same as the definite-assignment state at the end of *exprₓ*.

#### 9.4.4.24 Invocation expressions and object creation expressions

If the method to be invoked is a partial method that has no implementing partial method declaration, or is a conditional method or conditional local function for which the call is omitted ([§23.5.3.2](attributes.md#23532-conditional-methods), [§23.5.3.3](attributes.md#23533-conditional-local-functions)), then the definite-assignment state of *v* after the invocation is the same as the definite-assignment state of *v* before the invocation. Otherwise the following rules apply:

For an invocation expression *expr* of the form:

```csharp
«primary_expression» ( «arg₁», «arg₂», … , «argₓ» )
```

or an object-creation expression *expr* of the form:

```csharp
new «type» ( «arg₁», «arg₂», … , «argₓ» )
```

- For an invocation expression, the definite assignment state of *v* before *primary_expression* is the same as the state of *v* before *expr*.
- For an invocation expression, the definite assignment state of *v* before *arg₁* is the same as the state of *v* after *primary_expression*.
- For an object creation expression, the definite assignment state of *v* before *arg₁* is the same as the state of *v* before *expr*.
- For each argument *argᵢ*, the definite assignment state of *v* after *argᵢ* is determined by the normal expression rules, ignoring any *parameter_mode_modifier*s.
- For each argument *argᵢ* for any *i* greater than one, the definite assignment state of *v* before *argᵢ* is the same as the state of *v* after *argᵢ₋₁*.
- If the variable *v* is passed as an `out` argument (i.e., an argument of the form “out *v*”) in any of the arguments, then the state of *v* after *expr* is definitely assigned. Otherwise, the state of *v* after *expr* is the same as the state of *v* after *argₓ*.
- For array initializers ([§12.8.17.5](expressions.md#128175-array-creation-expressions)), object initializers ([§12.8.17.3](expressions.md#128173-object-initializers)), collection initializers ([§12.8.17.3.1](expressions.md#1281731-collection-initializers)) and anonymous object initializers ([§12.8.17.4](expressions.md#128174-anonymous-object-creation-expressions)), the definite-assignment state is determined by the expansion that these constructs are defined in terms of.

#### 9.4.4.25 Simple and deconstructing assignment expressions

Let the set of *assignment targets* in an expression *e* be defined as follows:

- If *e* is a *deconstructor*, then the assignment targets in *e* are the union of the assignment targets of the elements of *e*.
- Otherwise, the assignment targets in *e* are *e*.

For an expression *expr* of the form:

```csharp
«expr_lhs» = «expr_rhs»
```

- The definite-assignment state of *v* before *expr_lhs* is the same as the definite-assignment state of *v* before *expr*.
- The definite-assignment state of *v* before *expr_rhs* is the same as the definite-assignment state of *v* after *expr_lhs*.
- If *v* is an assignment target of *expr_lhs*, then the definite-assignment state of *v* after *expr* is definitely assigned. Otherwise, if the assignment occurs within the instance constructor of a struct type, and *v* is the hidden backing field of an automatically implemented property *P* on the instance being constructed, and a property access designating *P* is an assignment target of *expr_lhs*, then the definite-assignment state of *v* after *expr* is definitely assigned. Otherwise, the definite-assignment state of *v* after *expr* is the same as the definite-assignment state of *v* after *expr_rhs*.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"SimpleAssignment"} -->
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
> the variable `x` is considered definitely assigned after `arr[x = 1]` is evaluated as the left hand side of the second simple assignment.
>
> *end example*

#### 9.4.4.26 && expressions

For an expression *expr* of the form:

```csharp
«expr_first» && «expr_second»
```

- The definite-assignment state of *v* before *expr_first* is the same as the definite-assignment state of *v* before *expr*.
- The definite-assignment state of *v* before *expr_second* is definitely assigned if and only if the state of *v* after *expr_first* is either definitely assigned or “definitely assigned after true expression”. Otherwise, it is not definitely assigned.
- The definite-assignment state of *v* after *expr* is determined by:
  - If the state of *v* after *expr_first* is definitely assigned, then the state of *v* after *expr* is definitely assigned.
  - Otherwise, if the state of *v* after *expr_second* is definitely assigned, and the state of *v* after *expr_first* is “definitely assigned after false expression”, then the state of *v* after *expr* is definitely assigned.
  - Otherwise, if the state of *v* after *expr_second* is definitely assigned or “definitely assigned after true expression”, then the state of *v* after *expr* is “definitely assigned after true expression”.
  - Otherwise, if the state of *v* after *expr_first* is “definitely assigned after false expression”, and the state of *v* after *expr_second* is “definitely assigned after false expression”, then the state of *v* after *expr* is “definitely assigned after false expression”.
  - Otherwise, the state of *v* after *expr* is not definitely assigned.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AndAnd"} -->
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
> the variable `i` is considered definitely assigned in one of the embedded statements of an `if` statement but not in the other. In the `if` statement in method `F`, the variable `i` is definitely assigned in the first embedded statement because execution of the expression `(i = y)` always precedes execution of this embedded statement. In contrast, the variable `i` is not definitely assigned in the second embedded statement, since `x >= 0` might have tested false, resulting in the variable `i`’s being unassigned.
>
> *end example*

#### 9.4.4.27 || expressions

For an expression *expr* of the form:

```csharp
«expr_first» || «expr_second»
```

- The definite-assignment state of *v* before *expr_first* is the same as the definite-assignment state of *v* before *expr*.
- The definite-assignment state of *v* before *expr_second* is definitely assigned if and only if the state of *v* after *expr_first* is either definitely assigned or “definitely assigned after false expression”. Otherwise, it is not definitely assigned.
- The definite-assignment state of *v* after *expr* is determined by:
  - If the state of *v* after *expr_first* is definitely assigned, then the state of *v* after *expr* is definitely assigned.
  - Otherwise, if the state of *v* after *expr_second* is definitely assigned, and the state of *v* after *expr_first* is “definitely assigned after true expression”, then the state of *v* after *expr* is definitely assigned.
  - Otherwise, if the state of *v* after *expr_second* is definitely assigned or “definitely assigned after false expression”, then the state of *v* after *expr* is “definitely assigned after false expression”.
  - Otherwise, if the state of *v* after *expr_first* is “definitely assigned after true expression”, and the state of *v* after *expr_second* is “definitely assigned after true expression”, then the state of *v* after *expr* is “definitely assigned after true expression”.
  - Otherwise, the state of *v* after *expr* is not definitely assigned.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-lib-without-using", name:"OrOr"} -->
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
> the variable `i` is considered definitely assigned in one of the embedded statements of an `if` statement but not in the other. In the `if` statement in method `G`, the variable `i` is definitely assigned in the second embedded statement because execution of the expression `(i = y)` always precedes execution of this embedded statement. In contrast, the variable `i` is not definitely assigned in the first embedded statement, since `x >= 0` might have tested true, resulting in the variable `i`’s being unassigned.
>
> *end example*

#### 9.4.4.28 ! expressions

For an expression *expr* of the form:

```csharp
! «expr_operand»
```

- The definite-assignment state of *v* before *expr_operand* is the same as the definite-assignment state of *v* before *expr*.
- The definite-assignment state of *v* after *expr* is determined by:
  - If the state of `v` after *expr_operand* is definitely assigned, then the state of `v` after *expr* is definitely assigned.
  - Otherwise, if the state of `v` after *expr_operand* is “definitely assigned after false expression”, then the state of `v` after *expr* is “definitely assigned after true expression”.
  - Otherwise, if the state of `v` after *expr_operand* is “definitely assigned after true expression”, then the state of v after *expr* is “definitely assigned after false expression”.
  - Otherwise, the state of `v` after *expr* is not definitely assigned.

#### 9.4.4.29 ?? expressions

For an expression *expr* of the form:

```csharp
«expr_first» ?? «expr_second»
```

- The definite-assignment state of *v* before *expr_first* is the same as the definite-assignment state of *v* before *expr*.
- The definite-assignment state of *v* before *expr_second* is the same as the definite-assignment state of *v* after *expr_first*.
- The definite-assignment statement of *v* after *expr* is determined by:
  - If *expr_first* is a constant expression ([§12.26](expressions.md#1226-constant-expressions)) with value `null`, then the state of *v* after *expr* is the same as the state of *v* after *expr_second*.
  - If *expr_first* directly contains ([§12.1](expressions.md#121-general)) a null-conditional expression *E*, and *v* is definitely assigned after the non-conditional counterpart *E₀* ([§9.4.4.35](variables.md#94435--expressions)), then the definite-assignment state of *v* after *expr* is the same as the definite-assignment state of *v* after *expr_second*.
  - Otherwise, the state of *v* after *expr* is the same as the definite-assignment state of *v* after *expr_first*.

> *Note*: The rule above formalizes that for an expression like `a?.M(out x) ?? (x = false)`, either the `a?.M(out x)` was fully evaluated and produced a non-null value, in which case `x` was assigned, or the `x = false` was evaluated, in which case `x` was also assigned. Therefore `x` is always assigned after this expression.
>
> This also handles the `dict?.TryGetValue(key, out var value) ?? false` scenario, by observing that *v* is definitely assigned after `dict.TryGetValue(key, out var value)`, and *v* is “definitely assigned when true” after `false`, and concluding that *v* must be “definitely assigned when true.”
>
> The more general formulation also allows the handling of some more unusual scenarios, such as:
>
> - `if (x?.M(out y) ?? (b && z.M(out y))) y.ToString();`
> - `if (x?.M(out y) ?? z?.M(out y) ?? false) y.ToString();`
>
> *end note*

#### 9.4.4.30 ?: expressions

For an expression *expr* of the form:

```csharp
«expr_cond» ? «expr_true» : «expr_false»
```

- The definite-assignment state of *v* before *expr_cond* is the same as the state of *v* before *expr*.
- The definite-assignment state of *v* before *expr_true* is definitely assigned if the state of *v* after *expr_cond* is definitely assigned or “definitely assigned after true expression”.
- The definite-assignment state of *v* before *expr_false* is definitely assigned if the state of *v* after *expr_cond* is definitely assigned or “definitely assigned after false expression”.
- The definite-assignment state of *v* after *expr* is determined by:
  - If *expr_cond* is a constant expression ([§12.26](expressions.md#1226-constant-expressions)) with value `true` then the state of *v* after *expr* is the same as the state of *v* after *expr_true*.
  - Otherwise, if *expr_cond* is a constant expression ([§12.26](expressions.md#1226-constant-expressions)) with value `false` then the state of *v* after *expr* is the same as the state of *v* after *expr_false*.
  - Otherwise, if the state of *v* after *expr_true* is definitely assigned and the state of *v* after *expr_false* is definitely assigned, then the state of *v* after *expr* is definitely assigned.
  - Otherwise, the state of *v* after *expr* is not definitely assigned.
  - If the state of *v* after *expr_true* is “definitely assigned when true,” and the state of *v* after *expr_false* is “definitely assigned when true,” then the state of *v* after *expr* is “definitely assigned when true.”
  - If the state of *v* after *expr_true* is “definitely assigned when false,” and the state of *v* after *expr_false* is “definitely assigned when false,” then the state of *v* after *expr* is “definitely assigned when false.”

> *Note*: When both arms of a conditional expression result in a conditional state, the corresponding conditional states are joined and propagated out instead of unsplitting the state and allowing the final state to be non-conditional. This enables scenarios like the following:
>
> ```csharp
> bool b = true;
> object x = null;
> int y;
> if (b ? x != null && Set(out y) : x != null && Set(out y))
> {
>     y.ToString();
> }
>
> bool Set(out int x) { x = 0; return true; }
> ```
>
> *end note*

#### 9.4.4.31 Anonymous functions

For a *lambda_expression* or *anonymous_method_expression* *expr* with a body (either *block* or *expression*) *body*:

- The definite assignment state of a parameter is the same as for a parameter of a named method ([§9.2.6](variables.md#926-reference-parameters), [§9.2.7](variables.md#927-output-parameters), [§9.2.8](variables.md#928-input-parameters)).
- The definite assignment state of an outer variable *v* before *body* is the same as the state of *v* before *expr*. That is, definite assignment state of outer variables is inherited from the context of the anonymous function.
- The definite assignment state of an outer variable *v* after *expr* is the same as the state of *v* before *expr*.

> *Example*: The example
>
> <!-- Example: {template:"standalone-lib-without-using", name:"AnonymousFunctions1", replaceEllipsis:true, expectedErrors:["CS0165"]} -->
> ```csharp
> class A
> {
>     delegate bool Filter(int i);
>     void F()
>     {
>         int max;
>         // Error, max is not definitely assigned
>         Filter f = (int n) => n < max;
>         max = 5;
>         DoWork(f);
>     }
>     void DoWork(Filter f) { ... }
> }
> ```
>
> generates a compile-time error since max is not definitely assigned where the anonymous function is declared.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The example
>
> <!-- Example: {template:"standalone-lib", name:"AnonymousFunctions2", expectedErrors:["CS0165"]} -->
> ```csharp
> class A
> {
>     delegate void D();
>     void F()
>     {
>         int n;
>         D d = () => { n = 1; };
>         d();
>         // Error, n is not definitely assigned
>         Console.WriteLine(n);
>     }
> }
> ```
>
> also generates a compile-time error since the assignment to `n` in the anonymous function has no affect on the definite-assignment state of `n` outside the anonymous function.
>
> *end example*

#### 9.4.4.32 Throw expressions

For an expression *expr* of the form:

`throw` *thrown_expr*

- The definite assignment state of *v* before *thrown_expr* is the same as the state of *v* before *expr*.
- The definite assignment state of *v* after *expr* is “definitely assigned”.

#### 9.4.4.33 Rules for variables in local functions

Local functions are analyzed in the context of their parent method. There are two control flow paths that matter for local functions: function calls and delegate conversions.

Definite assignment for the body of each local function is defined separately for each call site. At each invocation, variables captured by the local function are considered definitely assigned if they were definitely assigned at the point of call. A control flow path also exists to the local function body at this point and is considered reachable. After a call to the local function, captured variables that were definitely assigned at every control point leaving the function (`return` statements, `yield` statements, `await` expressions) are considered definitely assigned after the call location.

Delegate conversions have a control flow path to the local function body. Captured variables are definitely assigned for the body if they are definitely assigned before the conversion. Variables assigned by the local function are not considered assigned after the conversion.

> *Note*: the above implies that bodies are re-analyzed for definite assignment at every local function invocation or delegate conversion. Compilers are not required to re-analyze the body of a local function at each invocation or delegate conversion. The implementation must produce results equivalent to that description. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The following example demonstrates definite assignment for captured variables in local functions. If a local function reads a captured variable before writing it, the captured variable must be definitely assigned before calling the local function. The local function `F1` reads `s` without assigning it. It is an error if `F1` is called before `s` is definitely assigned. `F2` assigns `i` before reading it. It may be called before `i` is definitely assigned. Furthermore, `F3` may be called after `F2` because `s2` is definitely assigned in `F2`.
>
> <!-- Example: {template:"code-in-class-lib", name:"RulesForVarsInLocalFunctions", expectedErrors:["CS0165"]} -->
> ```csharp
> void M()
> {
>     string s;
>     int i;
>     string s2;
>    
>     // Error: Use of unassigned local variable s:
>     F1();
>     // OK, F2 assigns i before reading it.
>     F2();
>     
>     // OK, i is definitely assigned in the body of F2:
>     s = i.ToString();
>     
>     // OK. s is now definitely assigned.
>     F1();
>
>     // OK, F3 reads s2, which is definitely assigned in F2.
>     F3();
>
>     void F1()
>     {
>         Console.WriteLine(s);
>     }
>     
>     void F2()
>     {
>         i = 5;
>         // OK. i is definitely assigned.
>         Console.WriteLine(i);
>         s2 = i.ToString();
>     }
>
>     void F3()
>     {
>         Console.WriteLine(s2);
>     }
> }
> ```
>
> *end example*

#### 9.4.4.34 is-pattern expressions

For an expression *expr* of the form:

*expr_operand* is *pattern*

- The definite-assignment state of *v* before *expr_operand* is the same as the definite-assignment state of *v* before *expr*.
- If the variable ‘v’ is declared in *pattern*, then the definite-assignment state of ‘v’ after *expr* is “definitely assigned when true”.
- Otherwise the definite assignment state of ‘v’ after *expr* is the same as the definite assignment state of ‘v’ after *expr_operand*.

#### 9.4.4.35 ?. expressions

For an expression *E* of the form:

```csharp
«primary_expression» ?. «null_conditional_operation»
```

let *E₀* be the expression obtained by textually removing the leading `?` from each of the *null_conditional_operation*s of *E* that have one. (*E₀* is referred to as the ***non-conditional counterpart*** to the null-conditional expression.)

- The definite-assignment state of *v* at any point within *E* is the same as the definite-assignment state at the corresponding point within *E₀*.
- The definite-assignment state of *v* after *E* is the same as the definite-assignment state of *v* after *primary_expression*.

> *Note*: *null_conditional_operation* is not actually a grammar rule; rather, it represents any form permitted by the grammar at that location. It is used here for convenience. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The concept of “directly contains” allows skipping over relatively simple “wrapper” expressions when analyzing conditional accesses that are compared to other values. For example, in general, `((a?.b(out x))!) == true` is expected to result in the same flow state as `a?.b == true`.
>
> The intent is to allow analysis to function in the presence of a number of possible conversions on a conditional access. Propagating out “state when not null” is not possible when the conversion is user-defined, though, since one can’t count on user-defined conversions to honor the constraint that the output is non-null only if the input is non-null. The only exception to this is when the user-defined conversion’s input is a non-nullable value type. For example:
>
> ```csharp
> public struct S1 { }
> public struct S2 { public static implicit operator S2?(S1 s1) => null; }
> ```
>
> This also includes lifted conversions like the following:
>
> ```csharp
> string x;
>
> S1? s1 = null;
> _ = s1?.M1(x = "a") ?? s1.Value.M2(x = "a");
>
> x.ToString(); // ok
>
> public struct S1
> {
>     public S1 M1(object obj) => this;
>     public S2 M2(object obj) => new S2();
> }
> public struct S2
> {
>     public static implicit operator S2(S1 s1) => default;
> }
> ```
>
> When it is considered whether a variable is assigned at a given point within a null-conditional expression, it can simply be assumed that any preceding null-conditional operations within the same null-conditional expression succeeded.
>
> For example, given a conditional expression `a?.b(out x)?.c(x)`, the non-conditional counterpart is `a.b(out x).c(x)`. If the definite-assignment state of `x` before `?.c(x)` is to be determined, for example, a “hypothetical” analysis of `a.b(out x)` can be performed and the resulting state can be used as an input to `?.c(x)`. *end note*

#### 9.4.4.36 Boolean constant expressions

For an expression *expr*, where *expr* is a constant expression with a `bool` value, the definite-assignment state of *v* after *expr* is determined, as follows:

- If *expr* is a constant expression with value *true*, and the state of *v* before *expr* is “not definitely assigned,” then the state of *v* after *expr* is “definitely assigned when false.”
- If *expr* is a constant expression with value *false*, and the state of *v* before *expr* is “not definitely assigned,” then the state of *v* after *expr* is “definitely assigned when true.”

> *Note*: It is assumed that if an expression has a constant value bool `false`, that it’s impossible to reach any branch that requires the expression to return `true`. Therefore, variables are assumed to be definitely assigned in such branches.
>
> Being in a conditional state *before* visiting a constant expression, is never expected, so there is no need to account for scenarios such as “*expr* is a constant expression with value *true* and the state of *v* before *expr* is definitely assigned when true.” *end note*

#### 9.4.4.37 ==/!= expressions

For an expression *expr* of the form:

```csharp
«expr_first» == «expr_second»
```

where `==` is a predefined comparison operator ([§12.15](expressions.md#1215-relational-and-type-testing-operators)) or a lifted operator ([§12.4.8](expressions.md#1248-lifted-operators)), the definite-assignment state of *v* after *expr* is determined by:

- If *expr_first* directly contains ([§12.1](expressions.md#121-general)) a null-conditional expression *E* and *expr_second* is a constant expression with value `null`, and the state of *v* after the non-conditional counterpart *E₀* is “definitely assigned,” then the state of *v* after *expr* is “definitely assigned when false.”
- If *expr_first* directly contains a null-conditional expression *E* and *expr_second* is an expression of a non-nullable value type, or a constant expression with a non-null value, and the state of *v* after the non-conditional counterpart *E₀* is “definitely assigned,” then the state of *v* after *expr* is “definitely assigned when true.”
- If *expr_first* is of type `bool`, and *expr_second* is a constant expression with value `true`, then the definite-assignment state after *expr* is the same as the definite-assignment state after *expr_first*.
- If *expr_first* is of type `bool`, and *expr_second* is a constant expression with value `false`, then the definite-assignment state after *expr* is the same as the definite-assignment state of *v* after the logical negation expression `!`*expr_first*.

For an expression *expr* of the form:

```csharp
«expr_first» != «expr_second»
```

where `!=` is a predefined comparison operator ([§12.15](expressions.md#1215-relational-and-type-testing-operators)) or a lifted operator ([§12.4.8](expressions.md#1248-lifted-operators)), the definite-assignment state of *v* after *expr* is determined by:

- If *expr_first* directly contains a null-conditional expression *E* and *expr_second* is a constant expression with value `null`, and the state of *v* after the non-conditional counterpart *E₀* is “definitely assigned,” then the state of *v* after *expr* is “definitely assigned when true.”
- If *expr_first* directly contains a null-conditional expression *E* and *expr_second* is an expression of a non-nullable value type, or a constant expression with a non-null value, and the state of *v* after the non-conditional counterpart *E₀* is “definitely assigned,” then the state of *v* after *expr* is “definitely assigned when false.”
- If *expr_first* is of type `bool`, and *expr_second* is a constant expression with value `true`, then the definite-assignment state after *expr* is the same as the definite-assignment state of *v* after the logical negation expression `!`*expr_first*.
- If *expr_first* is of type `bool`, and *expr_second* is a constant expression with value `false`, then the definite-assignment state after *expr* is the same as the definite-assignment state after *expr_first*.

All of the above rules are commutative.

> *Note*: The general idea expressed by these rules is:
>
> - if a conditional access is compared to `null`, then the operations definitely occurred if the result of the comparison is `false`.
> - if a conditional access is compared to a non-nullable value type or a non-null constant, then the operations definitely occurred if the result of the comparison is `true`.
> - since user-defined operators can’t be trusted to provide reliable answers where initialization safety is concerned, the new rules only apply when a predefined `==`/`!=` operator is in use.
>
> Some consequences of these rules are:
>
> - `if (a?.b(out var x) == true) x() else x();` will error in the ‘else’ branch
> - `if (a?.b(out var x) == 42) x() else x();` will error in the ‘else’ branch
> - `if (a?.b(out var x) == false) x() else x();` will error in the ‘else’ branch
> - `if (a?.b(out var x) == null) x() else x();` will error in the ‘then’ branch
> - `if (a?.b(out var x) != true) x() else x();` will error in the ‘then’ branch
> - `if (a?.b(out var x) != 42) x() else x();` will error in the ‘then’ branch
> - `if (a?.b(out var x) != false) x() else x();` will error in the ‘then’ branch
> - `if (a?.b(out var x) != null) x() else x();` will error in the ‘else’ branch
>
> *end note*

#### 9.4.4.38 is operator and is pattern expressions

For an expression *expr* of the form:

```csharp
«E» is «T»
```

where *T* is any type or pattern:

- The definite-assignment state of *v* before *E* is the same as the definite-assignment state of *v* before *expr*.
- The definite-assignment state of *v* after *expr* is determined by:

  - If *E* directly contains ([§12.1](expressions.md#121-general)) a null-conditional expression, and the state of *v* after the non-conditional counterpart *E₀* is “definitely assigned,” and `T` is any type or a pattern that does not match a `null` input, then the state of *v* after *expr* is “definitely assigned when true.”
  - If *E* directly contains a null-conditional expression, and the state of *v* after the non-conditional counterpart *E₀* is “definitely assigned,” and `T` is a pattern that matches a `null` input, then the state of *v* after *expr* is “definitely assigned when false.”
  - If *E* is of type `bool` and `T` is a pattern that only matches a `true` input, then the definite-assignment state of *v* after *expr* is the same as the definite-assignment state of *v* after *E*.
  - If *E* is of type `bool` and `T` is a pattern that only matches a `false` input, then the definite-assignment state of *v* after *expr* is the same as the definite-assignment state of *v* after the logical negation expression `!`*expr*.
  - Otherwise, if the definite-assignment state of *v* after *E* is “definitely assigned,” then the definite-assignment state of *v* after *expr* is “definitely assigned.”

> *Note*: This subclause addresses similar scenarios as [§9.4.4.37](variables.md#94437--expressions). It does not, however, address recursive patterns; e.g., `(a?.b(out x), c?.d(out y)) is (object, object)`. *end note*

## 9.5 Variable references

A *variable_reference* is an *expression* that is classified as a variable. A *variable_reference* denotes a storage location that can be accessed both to fetch the current value and to store a new value.

```ANTLR
variable_reference
    : expression
    ;
```

> *Note*: In C and C++, a *variable_reference* is known as an *lvalue*. *end note*

## 9.6 Atomicity of variable references

Reads and writes of the following data types shall be atomic: `bool`, `char`, `byte`, `sbyte`, `short`, `ushort`, `uint`, `int`, `nint`, `nuint`, `float`, and reference types. In addition, reads and writes of enum types with an underlying type in the previous list shall also be atomic. Reads and writes of other types, including `long`, `ulong`, `double`, and `decimal`, as well as user-defined types, need not be atomic. Aside from the library functions designed for that purpose, there is no guarantee of atomic read-modify-write, such as in the case of increment or decrement.

## 9.7 Reference variables and returns

### 9.7.1 General

A ***reference variable*** is a variable that refers to another variable, called the referent ([§9.2.6](variables.md#926-reference-parameters)). A reference variable is a local variable or ref struct field declared with the `ref` modifier.

A reference variable stores a *variable_reference* ([§9.5](variables.md#95-variable-references)) to its referent and not the value of its referent. When a reference variable is used where a value is required its referent’s value is returned; similarly when a reference variable is the target of an assignment it is the referent which is assigned to. The variable to which a reference variable refers, i.e. the stored *variable_reference* for its referent, can be changed using a ref assignment (`= ref`).

> *Example:* The following example demonstrates a local reference variable whose referent is an element of an array:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"RefVarsAndReturns1"} -->
> ```csharp
> public class C
> {
>     public void M()
>     {
>         int[] arr = new int[10];
>         // element is a reference variable that refers to arr[5]
>         ref int element = ref arr[5];
>         element += 5; // arr[5] has been incremented by 5
>     }     
> }
> ```
>
> *end example*

A ***reference return*** is the *variable_reference* returned from a returns-by-ref method ([§15.6.1](classes.md#1561-general)). This *variable_reference* is the referent of the reference return.

> *Example:* The following example demonstrates a reference return whose referent is an element of an array field:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"RefVarsAndReturns2"} -->
> ```csharp
> public class C
> {
>     private int[] arr = new int[10];
>
>     public ref readonly int M()
>     {
>         // element is a reference variable that refers to arr[5]
>         ref int element = ref arr[5];
>         return ref element; // return reference to arr[5];
>     }     
> }
> ```
>
> *end example*

### 9.7.2 Ref safe contexts

#### 9.7.2.1 General

All reference variables obey safety rules that ensure the ref-safe-context of the reference variable is not greater than the ref-safe-context of its referent.

> *Note*: The related notion of a *safe-context* is defined in ([§16.8.15](structs.md#16815-safe-context-constraint)), along with associated constraints. *end note*

For any variable, the ***ref-safe-context*** of that variable is the context where a *variable_reference* ([§9.5](variables.md#95-variable-references)) to that variable is valid. The referent of a reference variable shall have a ref-safe-context that is at least as wide as the ref-safe-context of the reference variable itself.

> *Note*: A compiler determines the ref-safe-context through a static analysis of the program text. The ref-safe-context reflects the lifetime of a variable at runtime. *end note*

There are four ref-safe-contexts:

- ***declaration-block***: The ref-safe-context of a *variable_reference* to a local variable ([§9.2.9.1](variables.md#9291-general)) is that local variable’s scope ([§13.6.2](statements.md#1362-local-variable-declarations)), including any nested *embedded-statement*s in that scope.

  A *variable_reference* to a local variable is a valid referent for a reference variable only if the reference variable is declared within the ref-safe-context of that variable.

- ***function-member***: Within a function a *variable_reference* to any of the following has a ref-safe-context of function-member:

  - Value parameters ([§15.6.2.2](classes.md#15622-value-parameters)) on a function member declaration, including the implicit `this` of class member functions;
  - Output parameters ([§15.6.2.3.4](classes.md#156234-output-parameters)), which are implicitly `scoped ref`; and
  - The implicit reference (`ref`) parameter ([§15.6.2.3.3](classes.md#156233-reference-parameters)) `this` of a struct member function, which is implicitly `scoped ref`, along with its fields.

  A *variable_reference* with ref-safe-context of function-member is a valid referent only if the reference variable is declared in the same function member.

- ***return-only***: Within a function a *variable_reference* to any of the following has a ref-safe-context of return-only:

  - Reference parameters ([§9.2.6](variables.md#926-reference-parameters)) other than the implicit `this` of a struct member function and other than output parameters; and
  - Input parameters ([§15.6.2.3.2](classes.md#156232-input-parameters)).

  A *variable_reference* with ref-safe-context of return-only can be the referent of a reference return.

- ***caller-context***: Within a function a *variable_reference* to any of the following has a ref-safe-context of caller-context:
  - Member fields and elements of reference or input parameters;
  - Member fields of parameters of class type; and
  - Elements of parameters of array type.
  
A *variable_reference* with ref-safe-context of caller-context can be the referent of a reference return.

These values form a nesting relationship from narrowest (declaration-block) to widest (caller-context). Each nested block represents a different context.

> *Example*: The following code shows examples of the different ref-safe-contexts. The declarations show the ref-safe-context for a referent to be the initializing expression for a `ref` variable. The examples show the ref-safe-context for a reference return:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"RefSafeContexts1", expectedErrors:["CS8166"]} -->
> ```csharp
> public class C
> {
>     // ref safe context of arr is "caller-context". 
>     // ref safe context of arr[i] is "caller-context".
>     private int[] arr = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }; 
> 
>     // ref safe context is "return-only"
>     public ref int M1(ref int r1)
>     {
>         return ref r1; // r1 is safe to ref return
>     }
>
>     // ref safe context is "function-member"
>     public ref int M2(int v1)
>     {
>         return ref v1; // error: v1 is not safe to ref return
>     }
>
>     public ref int M3()
>     {
>         int v2 = 5;
> 
>         return ref arr[v2]; // arr[v2] is safe to ref return
>     }
> 
>     public void M4(int p) 
>     {
>         int v3 = 6;
>
>         // context of r2 is declaration-block,
>         // ref safe context of p is function-member
>         ref int r2 = ref p;
>
>         // context of r3 is declaration-block,
>         // ref safe context of v3 is declaration-block
>         ref int r3 = ref v3;
>
>         // context of r4 is declaration-block,
>         // ref safe context of arr[v3] is caller-context
>         ref int r4 = ref arr[v3]; 
>     }
> }
> ```
>
> *end example.*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: For `struct` types, the implicit `this` parameter is passed as a reference parameter. The ref-safe-context of the fields of a `struct` type as function-member prevents returning those fields by reference return. This rule prevents the following code:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"RefSafeContexts2", expectedErrors:["CS8170"]} -->
> ```csharp
> public struct S
> {
>      private int n;
>
>      // Disallowed: returning ref of a field.
>      public ref int GetN() => ref n;
> }
>
> class Test
> {
>     public ref int M()
>     {
>         S s = new S();
>         ref int numRef = ref s.GetN();
>         return ref numRef; // reference to local variable 'numRef' returned
>     }
> }
> ```
>
> *end example.*

A reference variable that is a local variable or parameter can be scoped explicitly; see [§9.7.3](variables.md#973-the-scoped-modifier).

#### 9.7.2.2 Local variable ref safe context

For a local variable `v`:

- If `v` is a reference variable, its ref-safe-context is the same as the ref-safe-context of its initializing expression.
- Otherwise its ref-safe-context is declaration-block.

#### 9.7.2.3 Parameter ref safe context

For a parameter `p`:

- If `p` is a reference or input parameter, its ref-safe-context is return-only. If `p` is an input parameter, it cannot be returned as a writable `ref` but can be returned as `ref readonly`.
- If `p` is an output parameter, its ref-safe-context is function-member. An output parameter is implicitly `scoped ref`.
- Otherwise, if `p` is the `this` parameter of a struct type, its ref-safe-context is function-member. The `this` parameter of a struct instance method is implicitly `scoped ref`.
- Otherwise, the parameter is a value parameter, and its ref-safe-context is the function-member.

When a parameter is annotated with `[UnscopedRef]` ([§23.5.8](attributes.md#2358-the-unscopedref-attribute)), its ref-safe-context is widened by one level from its default: function-member becomes return-only, and return-only becomes caller-context.

> *Example*: The following illustrates how the implicit `this` parameter of a struct instance method is `scoped ref` (ref-safe-context of *function-member*), and how `[UnscopedRef]` widens it to *return-only*, enabling ref returns of fields:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ParameterRefSafeContext", expectedErrors:["CS8170"]} -->
> ```csharp
> using System.Diagnostics.CodeAnalysis;
>
> struct S
> {
>     private int _field;
>
>     // Error: ref-safe-context of `this` is function-member,
>     // so ref-safe-context of `_field` is also function-member,
>     // which does not satisfy the return-only requirement.
>     public ref int Bad() => ref _field;
>
>     // OK: [UnscopedRef] widens `this` from function-member to
>     // return-only, so `_field` also has ref-safe-context of
>     // return-only, satisfying the ref return requirement.
>     [UnscopedRef]
>     public ref int Good() => ref _field;
> }
> ```
>
> *end example*

#### 9.7.2.4 Field ref safe context

For a variable designating a reference to a field, `e.F`:

- If `F` is a reference variable, its ref-safe-context is the safe-context of `e`.
- Else if `e` is of a reference type, its ref-safe-context is the caller-context.
- Otherwise, if `e` is of a value type, its ref-safe-context is the same as the ref-safe-context of `e`.

As a result, a field that is a reference variable may be returned as a reference variable from a `ref struct` or `readonly ref struct`, but a non-reference variable field may not.

> *Example*:
> <!-- Example: {template:"standalone-lib-without-using", name:"FieldsSafeContext", expectedErrors:["CS8170"]} -->
>
> ```csharp
> ref struct RS
> {
>     ref int _refField;
>     int _field;
>     public ref int Prop1 => ref _refField;  // OK
>     public ref int Prop2 => ref _field;     // Error
> }
> ```
>
> *end example*

#### 9.7.2.5 Operators

The conditional operator ([§12.21](expressions.md#1221-conditional-operator)), `c ? ref e1 : ref e2`, and reference assignment operator, `= ref e` ([§12.24.1](expressions.md#12241-general)) have reference variables as operands and yield a reference variable. For those operators, the ref-safe-context of the result is the narrowest context among the ref-safe-contexts of all `ref` operands.

#### 9.7.2.6 Function invocation

For a variable `c` resulting from a ref-returning function invocation, `ref e1.M(e2, ...)`, where `M()` does not return ref-to-ref-struct, its ref-safe-context is the narrowest of the following contexts:

- The caller-context.
- The safe-context ([§16.8.15](structs.md#16815-safe-context-constraint)) contributed by all argument expressions (including the receiver), excluding arguments corresponding to `scoped` parameters and excluding `out` arguments.
- The ref-safe-context contributed by all `ref` and `ref readonly` arguments, excluding those corresponding to `scoped ref` parameters and excluding `out` arguments.

If `M()` does return ref-to-ref-struct, the ref-safe-context is the narrowest ref-safe-context contributed by all arguments which are ref-to-ref-struct.

For the purpose of these rules, a given argument `expr` passed to parameter `p`:

1. If `p` is `scoped ref`, then `expr` does not contribute ref-safe-context.
2. If `p` is `scoped`, then `expr` does not contribute safe-context.
3. If `p` is `out`, then `expr` does not contribute ref-safe-context or safe-context.

> *Example*: the last bullet is necessary to handle code such as
>
> <!-- Example: {template:"standalone-console-without-using", name:"FunctionInvocation", expectedErrors:["CS8168","CS8347"], ignoredWarnings:["CS8321"]} -->
> ```csharp
> ref int M2()
> {
>     int v = 5;
>     // Not valid.
>     // ref safe context of "v" is block.
>     // Therefore, ref safe context of the return value of M() is block.
>     return ref M(ref v);
> }
> 
> ref int M(ref int p)
> {
>     return ref p;
> }
> ```
>
> *end example*

A property invocation and an indexer invocation (either `get` or `set`) is treated as a function invocation of the underlying accessor by the above rules. A local function invocation is a function invocation.

#### 9.7.2.7 Values

A value’s ref-safe-context is the nearest enclosing context.

> *Note*: This occurs in an invocation such as `M(ref d.Length)` where `d` is of type `dynamic`. It is also consistent with arguments corresponding to input parameters. *end note*

#### 9.7.2.8 Constructor invocations

A `new` expression that invokes a constructor obeys the same rules as a method invocation ([§9.7.2.6](variables.md#9726-function-invocation)) that is considered to return the type being constructed.

#### 9.7.2.9 Limitations on reference variables

- Neither a reference parameter, nor an output parameter, nor an input parameter, nor a `ref` local, nor a parameter or local of a `ref struct` type shall be captured by lambda expression or local function.
- Neither a reference parameter, nor an output parameter, nor an input parameter, nor a parameter of a `ref struct` type shall be an argument for an iterator method or an `async` method.
- Neither a `ref` local, nor a local of a `ref struct` type shall be in context at the point of a `yield return` statement or an `await` expression.
- For a ref reassignment `e1 = ref e2`, the ref-safe-context of `e2` shall be at least as wide a context as the *ref-safe-context* of `e1`.
- For a ref return statement `return ref e1`, the ref-safe-context of `e1` shall be at least return-only.

### 9.7.3 The scoped modifier

The contextual keyword `scoped` is used as a modifier to restrict the ref-safe-context ([§9.7.2](variables.md#972-ref-safe-contexts)) or safe-context ([§16.8.15](structs.md#16815-safe-context-constraint)) of a variable. The presence of this modifier requires that related code doesn’t extend the lifetime of the variable.

`scoped` shall only be applied to reference variables (which includes non-value parameters) and to variables of a ref struct type. `scoped` shall not be applied to fields, array elements, or return types.

Consider the following declarations and their safe contexts:

| Local Variable              | ref-safe-context | safe-context |
|---|---|---|
| `Span<int> s`               | *function-member*  | *caller-context* |
| `scoped Span<int> s`        | *function-member*  | *function-member* |
| `ref Span<int> s`           | *caller-context*   | *caller-context* |
| `scoped ref Span<int> s`    | *function-member*  | *caller-context* |

In this relationship the *ref-safe-context* of a value can never be wider than the *safe-context*.

### 9.7.4 Parameter scope variance

The `scoped` modifier ([§9.7.3](variables.md#973-the-scoped-modifier)) and `[UnscopedRef]` attribute ([§23.5.8](attributes.md#2358-the-unscopedref-attribute)) on parameters affect overriding, interface implementation, and `delegate` conversion. The signature for an override, interface implementation, or `delegate` conversion may:

- Add `scoped` to a `ref` or `in` parameter.
- Add `scoped` to a parameter of a `ref struct` type.
- Remove `[UnscopedRef]` from an `out` parameter.
- Remove `[UnscopedRef]` from a `ref` parameter of a `ref struct` type.

Any other difference with respect to `scoped` or `[UnscopedRef]` between the base and the overriding, implementing, or converting signature is a mismatch.

The `scoped` modifier and `[UnscopedRef]` attribute do not affect hiding.

Overloads shall not differ only on `scoped` or `[UnscopedRef]`.

> *Example*: The following illustrates valid and invalid scope variance in overrides:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"ParameterScopeVariance", expectedErrors:["CS0111"]} -->
> ```csharp
> using System;
>
> class Base
> {
>     public virtual void M(ref Span<int> x) { }
> }
>
> class Derived : Base
> {
>     // OK: adds scoped to a ref parameter
>     public override void M(scoped ref Span<int> x) { }
> }
>
> class C
> {
>     void N(Span<int> x) { }
>     void N(scoped Span<int> x) { }  // Error: overloads differ only on scoped
> }
> ```
>
> *end example*
