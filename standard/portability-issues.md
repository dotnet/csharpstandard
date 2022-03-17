# Annex B Portability issues

**This clause is informative.**

## B.1 General

This annex collects some information about portability that appears in this specification.

## B.2 Undefined behavior

The behavior is undefined in the following circumstances:

1.  The behavior of the enclosing async function when an awaiter’s implementation of the interface methods `INotifyCompletion.OnCompleted` and `ICriticalNotifyCompletion.UnsafeOnCompleted` does not cause the resumption delegate to be invoked at most once ([§11.8.8.4](expressions.md#11884-run-time-evaluation-of-await-expressions)).
1.  Passing pointers as `ref` or `out` parameters ([§22.3](unsafe-code.md#223-pointer-types)).
1.  When dereferencing the result of converting one pointer type to another and the resulting pointer is not correctly aligned for the pointed-to type. ([§22.5.1](unsafe-code.md#2251-general)).
1.  When the unary `*` operator is applied to a pointer containing an invalid value ([§22.6.2](unsafe-code.md#2262-pointer-indirection)).
1.  When a pointer is subscripted to access an out-of-bounds element ([§22.6.4](unsafe-code.md#2264-pointer-element-access)).
1.  Modifying objects of managed type through fixed pointers ([§22.7](unsafe-code.md#227-the-fixed-statement)).
1.  The content of memory newly allocated by `stackalloc` ([§22.9](unsafe-code.md#229-stack-allocation)).
1.  Attempting to allocate a negative number of items using `stackalloc`([§22.9](unsafe-code.md#229-stack-allocation)).

## B.3 Implementation-defined behavior

A conforming implementation is required to document its choice of behavior in each of the areas listed in this subclause. The following are implementation-defined:

1.  The behavior when an identifier not in Normalization Form C is encountered ([§6.4.3](lexical-structure.md#643-identifiers)).
1.  The interpretation of the *input_characters* in the *pp_pragma-text* of a #pragma directive ([§6.5.9](lexical-structure.md#659-pragma-directives)).
1.  The values of any application parameters passed to `Main` by the host environment prior to application startup ([§7.1](basic-concepts.md#71-application-startup)).
1.  The precise structure of the expression tree, as well as the exact process for creating it, when an anonymous function is converted to an expression-tree ([§10.7.3](conversions.md#1073-evaluation-of-lambda-expression-conversions-to-expression-tree-types)).
1.  Whether a `System.ArithmeticException` (or a subclass thereof) is thrown or the overflow goes unreported with the resulting value being that of the left operand, when in an `unchecked` context and the left operand of an integer division is the maximum negative `int` or `long` value and the right operand is `–1` ([§11.9.3](expressions.md#1193-division-operator)).
1.  When a `System.ArithmeticException` (or a subclass thereof) is thrown when performing a decimal remainder operation ([§11.9.4](expressions.md#1194-remainder-operator)).
1.  The impact of thread termination when a thread has no handler for an exception, and the thread is itself terminated ([§12.10.6](statements.md#12106-the-throw-statement)).
1.  The impact of thread termination when no matching `catch` clause is found for an exception and the code that initially started that thread is reached. ([§20.4](exceptions.md#204-how-exceptions-are-handled)).
1.  The mappings between pointers and integers ([§22.5.1](unsafe-code.md#2251-general)).
1.  The effect of applying the unary `*` operator to a `null` pointer ([§22.6.2](unsafe-code.md#2262-pointer-indirection)).
1.  The behavior when pointer arithmetic overflows the domain of the pointer type ([§22.6.6](unsafe-code.md#2266-pointer-increment-and-decrement), [§22.6.7](unsafe-code.md#2267-pointer-arithmetic)).
1.  The result of the `sizeof` operator for non-pre-defined value types ([§22.6.9](unsafe-code.md#2269-the-sizeof-operator)).
1.  The behavior of the `fixed` statement if the array expression is `null` or if the array has zero elements ([§22.7](unsafe-code.md#227-the-fixed-statement)).
1.  The behavior of the `fixed` statement if the string expression is `null` ([§22.7](unsafe-code.md#227-the-fixed-statement)).
1.  The value returned when a stack allocation of size zero is made ([§22.9](unsafe-code.md#229-stack-allocation)).

## B.4 Unspecified behavior

1.  The time at which the finalizer (if any) for an object is run, once that object has become eligible for finalization ([§7.9](basic-concepts.md#79-automatic-memory-management)).
1.  The value of the result when converting out-of-range values from `float` or `double` values to an integral type in an `unchecked` context ([§10.3.2](conversions.md#1032-explicit-numeric-conversions)).
1.  The exact target object and target method of the delegate produced from an *anonymous_method_expression* contains ([§10.7.2](conversions.md#1072-evaluation-of-anonymous-function-conversions-to-delegate-types)).
1.  The layout of arrays, except in an unsafe context ([§11.7.15.5](expressions.md#117155-array-creation-expressions)).
1.  Whether there is any way to execute the *block* of an anonymous function other than through evaluation and invocation of the *lambda_expression* or *anonymous_method-expression* ([§11.16.3](expressions.md#11163-anonymous-function-bodies)).
1.  The exact timing of static field initialization ([§14.5.6.2](classes.md#14562-static-field-initialization)).
1.  The result of invoking `MoveNext` when an enumerator object is running ([§14.14.5.2](classes.md#141452-the-movenext-method)).
1.  The result of accessing `Current` when an enumerator object is in the before, running, or after states ([§14.14.5.3](classes.md#141453-the-current-property)).
1.  The result of invoking `Dispose` when an enumerator object is in the running state ([§14.14.5.4](classes.md#141454-the-dispose-method)).
1.  The attributes of a type declared in multiple parts are determined by combining, in an unspecified order, the attributes of each of its parts ([§21.3](attributes.md#213-attribute-specification)).
1.  The order in which members are packed into a struct ([§22.6.9](unsafe-code.md#2269-the-sizeof-operator)).
1.  An exception occurs during finalizer execution, and that execution is not caught ([§20.4](exceptions.md#204-how-exceptions-are-handled)).
1.  If more than one member matches, which member is the implementation of I.M. ([§17.6.5](interfaces.md#1765-interface-mapping))

## B.5 Other Issues

1.  The exact results of floating-point expression evaluation can vary from one implementation to another, because an implementation is permitted to evaluate such expressions using a greater range and/or precision than is required. ([§8.3.7](types.md#837-floating-point-types))
1.  The CLI reserves certain signatures for compatibility with other programming languages. ([§14.3.9.7](classes.md#14397-nested-types-in-generic-classes))

**End of informative text.**
