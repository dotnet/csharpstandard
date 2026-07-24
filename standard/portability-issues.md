# Annex B Portability issues

**This clause is informative.**

## B.1 General

This annex collects some information about portability that appears in this specification.

## B.2 Undefined behavior

The behavior is undefined in the following circumstances:

1. The behavior of the enclosing async function when an awaiter’s implementation of the interface methods `INotifyCompletion.OnCompleted` and `ICriticalNotifyCompletion.UnsafeOnCompleted` does not cause the resumption delegate to be invoked at most once ([§12.9.9.4](expressions.md#12994-run-time-evaluation-of-await-expressions)).
1. Passing pointers as `ref` or `out` parameters ([§24.3.2](unsafe-code.md#2432-data-pointers)).
1. When dereferencing the result of converting one pointer type to another and the resulting pointer is not correctly aligned for the pointed-to type. ([§24.5.1](unsafe-code.md#2451-general)).
1. When the unary `*` operator is applied to a pointer containing an invalid value ([§24.6.2](unsafe-code.md#2462-pointer-indirection)).
1. When a pointer is subscripted to access an out-of-bounds element ([§24.6.4](unsafe-code.md#2464-pointer-element-access)).
1. When comparing values of *funcptr_type*s, or `void*` copies thereof ([§24.6.8](unsafe-code.md#2468-pointer-comparison).
1. Modifying objects of managed type through fixed pointers ([§24.7](unsafe-code.md#247-the-fixed-statement)).
1. The content of memory newly allocated by `stackalloc` ([§12.8.22](expressions.md#12822-stack-allocation)).
1. Attempting to allocate a negative number of items using `stackalloc`([§12.8.22](expressions.md#12822-stack-allocation)).
1. Implicit dynamic conversions ([§10.2.10](conversions.md#10210-implicit-dynamic-conversions)) of input parameters with value arguments ([§12.6.4.2](expressions.md#12642-applicable-function-member)).

## B.3 Implementation-defined behavior

A conforming implementation is required to document its choice of behavior in each of the areas listed in this subclause. The following are implementation-defined:

1. The handling of the Unicode NULL character (U+0000) in a compilation unit. ([§6.1](lexical-structure.md#61-programs))
1. The behavior when an identifier not in Normalization Form C is encountered. ([§6.4.3](lexical-structure.md#643-identifiers))
1. The maximum value allowed for `Decimal_Digit+` in `PP_Line_Indicator`. ([§6.5.8](lexical-structure.md#658-line-directives))
1. The interpretation of the *input_characters* in the *pp_pragma-text* of a #pragma directive. ([§6.5.10](lexical-structure.md#6510-pragma-directives))
1. The values of any application parameters passed to `Main` by the host environment prior to application startup. ([§7.1](basic-concepts.md#71-application-startup))
1. The mechanism for determining whether a program is compiled as a class library or as an application. ([§7.1](basic-concepts.md#71-application-startup))
1. The policy or mechanisms used by an implementation for the creation and destruction of application domains. ([§7.1](basic-concepts.md#71-application-startup))
1. The exit code if the effective entry point method terminates due to an exception. ([§7.2](basic-concepts.md#72-application-termination))
1. Whether or not finalizers are run as part of application termination. ([§7.2](basic-concepts.md#72-application-termination), [§7.9](basic-concepts.md#79-automatic-memory-management))
1. Whether APIs allow a finalizer to be run more than once. ([§7.9](basic-concepts.md#79-automatic-memory-management))
1. The size and value range of the types `nint` and `nuint`. ([§8.3.6](types.md#836-integral-types))
1. The API surface provided by `Expression<TDelegate>` beyond the requirement for a `Compile` method. ([§8.6](types.md#86-expression-tree-types))
1. The precise structure of the expression tree, as well as the exact process for creating it, when an anonymous function is converted to an expression-tree. ([§10.7.3](conversions.md#1073-evaluation-of-lambda-expression-conversions-to-expression-tree-types))
1. The reason a conversion to a compatible delegate type may fail at compile-time. ([§10.7.3](conversions.md#1073-evaluation-of-lambda-expression-conversions-to-expression-tree-types))
1. The value returned when a stack allocation of size zero is made. ([§12.8.22](expressions.md#12822-stack-allocation))
1. Whether a `System.ArithmeticException` (or a subclass thereof) is thrown or the overflow goes unreported with the resulting value being that of the left operand, when in an `unchecked` context and the left operand of an integer division is the maximum negative `int` or `long` value and the right operand is `–1`. ([§12.13.3](expressions.md#12133-division-operator))
1. When a `System.ArithmeticException` (or a subclass thereof) is thrown when performing a decimal remainder operation. ([§12.13.4](expressions.md#12134-remainder-operator))
1. The mechanism for distinguishing a property’s set accessor signature from that of an init accessor ([§15.3.10.2](classes.md#153102-member-names-reserved-for-properties)).
1. The mechanism for distinguishing an indexer’s set accessor signature from that of an init accessor ([§15.3.10.4](classes.md#153104-member-names-reserved-for-indexers)).
1. The impact of thread termination when a thread has no handler for an exception, and the thread is itself terminated. ([§13.10.6](statements.md#13106-the-throw-statement))
1. The mechanism by which linkage to an external method is achieved. ([§15.6.8](classes.md#1568-external-methods))
1. The impact of thread termination when no matching `catch` clause is found for an exception and the code that initially started that thread is reached. ([§22.4](exceptions.md#224-how-exceptions-are-handled)).
1. The token name mapping and semantics of unmanaged calling conventions beyond those required by this specification, and the set of valid combinations of those tokens ([§24.3.3](unsafe-code.md#2433-function-pointers)).
1. The order of execution of module initializers in a module ([§23.5.9](attributes.md#2359-the-moduleinitializer-attribute)).
1. An execution environment may provide additional attributes that affect the execution of a C# program. ([§23.5.1](attributes.md#2351-general))
1. The mappings between pointers and integers. ([§24.5.1](unsafe-code.md#2451-general))
1. The effect of applying the unary `*` operator to a `null` pointer. ([§24.6.2](unsafe-code.md#2462-pointer-indirection))
1. The type of exception thrown when the *primary_expression* of an *invocation_expression* is a function pointer with value `null`, and an attempt is made to invoke the (non-existent) pointed-to method ([§12.8.10](expressions.md#12810-invocation-expressions)).
1. The behavior when pointer arithmetic overflows the domain of the pointer type. ([§24.6.6](unsafe-code.md#2466-pointer-increment-and-decrement), [§24.6.7](unsafe-code.md#2467-pointer-arithmetic))
1. The result of the `sizeof` operator for non-pre-defined value types. ([§24.6.9](unsafe-code.md#2469-the-sizeof-operator))
1. The behavior of the `fixed` statement if the array expression is `null` or if the array has zero elements. ([§24.7](unsafe-code.md#247-the-fixed-statement))
1. The behavior of the `fixed` statement if the string expression is `null`. ([§24.7](unsafe-code.md#247-the-fixed-statement))
1. The value returned when a stack allocation of size zero is made ([§12.8.22](expressions.md#12822-stack-allocation)).

## B.4 Unspecified behavior

1. The name of the entry-point method generated to contain top-level statements ([§7.1.3](basic-concepts.md#713-using-top-level-statements)).
1. The time at which the finalizer (if any) for an object is run, once that object has become eligible for finalization ([§7.9](basic-concepts.md#79-automatic-memory-management)).
1. The representation of `true` ([§8.3.9](types.md#839-the-bool-type)).
1. The value of the result when converting out-of-range values from `float` or `double` values to an integral type in an `unchecked` context ([§10.3.2](conversions.md#1032-explicit-numeric-conversions)).
1. The exact target object and target method of the delegate produced from an *anonymous_method_expression* contains ([§10.7.2](conversions.md#1072-evaluation-of-anonymous-function-conversions-to-delegate-types)).
1. The layout of arrays, except in an unsafe context ([§12.8.17.4](expressions.md#128174-array-creation-expressions)).
1. Whether there is any way to execute the *block* of an anonymous function other than through evaluation and invocation of the *lambda_expression* or *anonymous_method-expression* ([§12.22.3](expressions.md#12223-anonymous-function-bodies)).
1. The exact timing of static field initialization ([§15.5.6.2](classes.md#15562-static-field-initialization)).
1. The result of invoking `MoveNext` when an enumerator object is running ([§15.15.5.2](classes.md#151552-advance-the-enumerator)).
1. The result of accessing `Current` when an enumerator object is in the before, running, or after states ([§15.15.5.3](classes.md#151553-retrieve-the-current-value)).
1. The result of invoking `Dispose` when an enumerator object is in the running state ([§15.15.5.4](classes.md#151554-dispose-of-resources)).
1. The attributes of a type declared in multiple parts are determined by combining, in an unspecified order, the attributes of each of its parts ([§23.3](attributes.md#233-attribute-specification)).
1. The order in which members are packed into a struct ([§24.6.9](unsafe-code.md#2469-the-sizeof-operator)).
1. An exception occurs during finalizer execution, and that exception is not caught ([§22.4](exceptions.md#224-how-exceptions-are-handled)).
1. If more than one member matches, which member is the implementation of `I.M` ([§19.6.5](interfaces.md#1965-interface-mapping)).

## B.5 Other issues

1. The exact results of floating-point expression evaluation can vary from one implementation to another, because an implementation is permitted to evaluate such expressions using a greater range and/or precision than is required ([§8.3.7](types.md#837-floating-point-types)).
1. Certain signatures are reserved for compatibility with other programming languages ([§15.3.10](classes.md#15310-reserved-member-names)).

**End of informative text.**
