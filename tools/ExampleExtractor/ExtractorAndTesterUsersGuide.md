# Guide to Annotating C# Examples for Extraction and Testing

## Table of Contents
- [Introduction](#introduction)
- [Annotation Format](#annotation-format)
  - [General](#general)
  - [Templates](#templates)
  - [Example Names](#example-names)
  - [Ellipsis Processing](#ellipsis-processing)
  - [Expected Compiler Errors](#expected-compiler-errors)
  - [Expected Compiler Warnings](#expected-compiler-warnings)
  - [Ignoring Compiler Warnings](#ignoring-compiler-warnings)
  - [Expected Runtime Output](#expected-runtime-output)
  - [Expected Exception](#expected-exception)
  - [Including Support Files](#including-support-files)
- [Other Information](#other-information)
  - [Unsafe Code](#unsafe-code)
  - [Examples Containing Pseudo-Code](#examples-containing-pseudo-code)
  - [Using Chevron-Quotes for Emphasis](#using-chevron-quotes-for-emphasis)
  - [Tips for Creating New Testable Examples](#tips-for-creating-new-testable-examples)

## Introduction

The C# specification contains many source-code examples, some complete and others partial. When changing existing examples or adding new ones, it is useful to have some automatic way to extract, compile, and possibly execute them, to ensure their correctness, or to re-confirm expected warnings, errors, and/or exceptions. This is done by annotating examples, extracting them using the ExampleExtractor tool, and testing them using the ExampleTester tool.

An example is annotated directly, prior to its occurrence in a spec md file, as described below in [Annotation Format](#annotation-format). The extraction and testing stages are part of PR processing within GitHub.

## Annotation Format

### General

An *example_annotation* is an HTML comment having a particular structure, as shown by the ANTLR grammar below. It uses [JSON Data Interchange Syntax](https://www.ecma-international.org/publications-and-standards/standards/ecma-404/) to control the extraction and testing processes via a series of *annotation_directive*s containing JSON name/value pairs.

```ANTLR
example_annotation
    : '<!-- Example:' '{' annotation_directive (',' annotation_directive)* '}' '-->'
    ;
annotation_directive
    : template
    | name
    | replace_ellipsis
    | custom_ellipsis_replacements
    | expected_errors
    | expected_warnings
    | ignored_warnings
    | expected_output
    | infer_output
    | ignore_output
    | expected_exception
    | additional_files
    ;
```

While an *example_annotation* must precede its corresponding example, it need not be immediately prior. The two can be separated by text and/or HTML comments. However, if multiple *example_annotation*s precede the same code block, the second and subsequent *example_annotation*s are ignored.

Arbitrary horizontal whitespace is permitted between any two adjacent tokens.

The ordering of annotation directives is not significant.

The C# spec contains two kinds of source-code examples: those within an \*Example\* and those not, as follows:

````
> *Example*:
>
> <!-- Example: { … } -->
> ```csharp
> // source code goes here
> ```
>
> *end example*
````

In this case, the *example_annotation* is preceded by `> ` to match the example’s formatting. However, in the following case, it is not, as no \*Example\* is present:

````
<!-- Example: { … } -->
```csharp
// source code goes here
```
````

### Templates

An annotation’s *template* is used to select certain compiler options, and to provide supporting machinery, as needed. This *annotation_directive* is required.

```ANTLR
template
    : 'template' ':' template_name
    ;
template_name
    : '"code-in-class-lib"'                 // actually, a JSON_string_value with this content
    | '"code-in-class-lib-without-using"'   // actually, a JSON_string_value with this content
    | '"code-in-main"'                      // actually, a JSON_string_value with this content
    | '"code-in-main-without-using"'        // actually, a JSON_string_value with this content
    | '"code-in-partial-class"'             // actually, a JSON_string_value with this content
    | '"standalone-console"'                // actually, a JSON_string_value with this content
    | '"standalone-console-without-using"'  // actually, a JSON_string_value with this content
    | '"standalone-lib"'                    // actually, a JSON_string_value with this content
    | '"standalone-lib-without-using"'      // actually, a JSON_string_value with this content
    ;
```

The unsuffixed and suffixed versions are identical, *except* that the unsuffixed ones have using directioves for all namespaces used by examples, while the suffixed ones do not. The unsuffixed versions are used by those few examples that begin with `#undef` or `#define`, which *must* precede using directives, and which might then have explicit using directives.

The template `standalone-console` indicates that the example is an application. For example:

````
> <!-- Example: {template:"standalone-console", …} -->
> ```csharp
> class Hello
> {
>     static void Main()
>     {
>         …
>     }
> }
> ```
````

The template `standalone-lib` indicates that the example is a class library. For example:

````
> <!-- Example: {template:"standalone-lib", … } -->
> ```csharp
> class Class1
> {
>     static void Test(bool status)
>     {
>         …
>     }
> }
> ```
````

The template `code-in-main` indicates that the example needs to be wrapped inside an entry-point method inside a class. For example, the example:

````
> <!-- Example: {template:"code-in-main", … } -->
> ```csharp
> int[][] pascals = 
> {
>     new int[] {1},
>     new int[] {1, 1},
>     new int[] {1, 2, 1},
>     new int[] {1, 3, 3, 1}
> };
> ```
````

gets transformed into the following application:

````
class Program
{
    static void Main()
    {
        int[][] pascals = 
{
    new int[] {1},
    new int[] {1, 1},
    new int[] {1, 2, 1},
    new int[] {1, 3, 3, 1}
};
    }
}
````

The template `code-in-class-lib` indicates that the example needs to be wrapped inside an entry-point method inside a class. For example, the example:

````
> <!-- Example: {template:"code-in-class-lib", ..."} -->
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
````

gets transformed into the following library:

````
class Class1
{
    public void Log(
    [CallerLineNumber] int line = -1,
    [CallerFilePath] string path = null,
    [CallerMemberName] string name = null
)
{
    Console.WriteLine((line < 0) ? "No line" : "Line "+ line);
    Console.WriteLine((path == null) ? "No file path" : path);
    Console.WriteLine((name == null) ? "No member name" : name);
}
}
````

The template `code-in-partial-class` indicates that the example is part of a multifile application. For example:

````
> <!-- Example: {template:"code-in-partial-class", name:"...", additionalFiles:["Caller.cs"], ...} -->
> ```csharp
> static D[] F()
> {
>     ...
> }
> ```
````

which gets transformed into the following:

````
partial class Class1
{
    static D[] F()
{
    ...
}
}

````

The additional file `Caller.cs` contains the following:

````
delegate void D();

partial class Class1
{
   static void Main()
   {
       foreach (D d in F())
       {
           d();
       }
   }
}
```

We use this to supplement the example.

### Example Names

An annotation’s *name* is the name of the resulting test file directory, **which must be unique across the whole C# spec**. This *annotation_directive* is required. *name* should be a valid C# idenifier.

```ANTLR
name
    : 'name' ':' test_filename
    ;
test_filename
    : JSON_string_value
    ;
```

The ExampleExtractor tool processes all md files in a given input folder, and writes out all corresponding test files to a given output folder.

In the following example, the class-library source-code is written to directory “NestedClassDependency”:

````
> <!-- Example: {template:"standalone-lib", name:"NestedClassDependency"} -->
> ```csharp
> class A
> {
>     class B : A {}
> }
> ```
````

### Ellipsis Processing

The treatment of ellipses is controlled by the *replace_ellipsis* annotation directive, which is optional.

```ANTLR
replace_ellipsis
    : 'replaceEllipsis' ':' ('true' | 'false')
    ;
```
The C# spec contains many examples having one or more occurrences of an ellipsis (`…`) as a placeholder for content not relevant to the discussion. For example:

````
> <!-- Example: {template:"standalone-console", name:"ReplaceEllipsisTrue", replaceEllipsis:true}    -->
> ```csharp
> class B<U,V> {...}
> class G<T> : B<string,T[]> {...}
> ```
````

When `true` is specified, each ellipsis—regardless of context (including inside string literals)—is replaced with `/* ... */`, as follows:

````
class B<U,V> {/* ... */}
class G<T> : B<string,T[]> {/* ... */}
````

When `false` is specified, or the annotation directive is omitted, ellipses are preserved, as written.

Specifying this *annotation_directive* when the example contains no ellipses, has no ill-effect.

Note, however, that such ellipsis replacement does *not* always result in syntactically correct code. When an ellipsis is used as the (unstated) body of a non-void method or a get accessor, for example, this results in compilation error CS0161, “not all code paths return a value.” For example:

````
> <!-- Example: {template:"standalone-lib", name:"MembersOfConstructedTypes", replaceEllipsis:true, expectedWarnings:["CS0649"]} -->
> ```csharp
> class Gen<T,U>
> {
>     public T[,] a;
>     public void G(int i, T t, Gen<U,T> gt) {...}
>     public U Prop { get {...} set {...} }
>     public int H(double d) {...}
> }
> ```
````

is transformed into the following, where the comments indicate how that line is treated by the compiler

````
class Gen<T,U>
{
    public T[,] a;
    public void G(int i, T t, Gen<U,T> gt) {/* ... */}  // OK, void method
    public U Prop { get {/* ... */} set {/* ... */} }   // CS0161, getter; setter OK
    public int H(double d) {/* ... */}                  // CS0161, non-void method
}
````

To rectify this, use the optional *customEllipsisReplacements* annotation directive as well. For example:

````
> <!-- Example: {template:"standalone-lib", name:"MembersOfConstructedTypes", replaceEllipsis:true, customEllipsisReplacements: ["null", "return default;", null, "return 0;"], expectedWarnings:["CS0649"]} -->
> ```csharp
> class Gen<T,U>
> {
>     public T[,] a;
>     public void G(int i, T t, Gen<U,T> gt) {...}
>     public U Prop { get {...} set {...} }
>     public int H(double d) {...}
> }
> ```
````

which is transformed into

````
class Gen<T,U>
{
    public T[,] a;
    public void G(int i, T t, Gen<U,T> gt) {/* ... */}
    public U Prop { get {return default;} set {/* ... */} }
    public int H(double d) {return 0;}
}
````

```ANTLR
custom_ellipsis_replacements
    : 'customEllipsisReplacements' ':' '[' replacement (',' replacement)* ']'
    ;

replacement
    : 'null'
    | output_string
    ;
```

If *replacement* is `null`, the ellipsis is replaced with its commented-out form, `/* ... */`. Otherwise, the text in *output_string* (which might be a declaration, an expression, one or more statements; whatever the compiler will accept) replaces the ellipsis.

If there are more ellipses than there are corresponding *replacement*s, the trailing ellipses are treated as though `null` was specified for each of them.

### Expected Compiler Errors

Some tests are expected to fail with one or more compilation errors, in which case, the list of expected error numbers is provided by the *expected_errors* annotation directive, so the numbers in that list can be verified. This *annotation_directive* is optional.

```ANTLR
expected_errors
    : 'expectedErrors' ':' '[' cs_number (',' cs_number)* ']'
    ;
cs_number
    : JSON_string_value      // of the form "CSnnnn"
    ;
```

The `nnnn` in `"CSnnnn"` is a 4-digit number (with leading zeros, if necessary) [as used by Microsoft’s compiler](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/) for the error message expected.

Error *cs_number*s shall appear in the order in which they are output to the console during compilation. When the compiler produces both error and warning messages, they may be interspersed; however, that has no impact on the checking order of this annotation directive’s list. 

Consider the following example, which is intended to result in five compilation errors: one occurrence of CS0663 (“Cannot define overloaded methods that differ only on ref and out”), followed by four occurrences of CS0111 (“Type 'class' already defines a member called 'member' with the same parameter types”):

````
> <!-- Example: {template:"standalone-lib", name:"SignatureOverloading",expectedErrors:["CS0663","CS0111","CS0111","CS0111","CS0111"]} -->
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
````

### Expected Compiler Warnings

Some tests are expected to result in one or more compilation warnings, in which case, the list of expected warning numbers is provided by the *expected_warnings* annotation directive, so the numbers in that list can be verified. This *annotation_directive* is optional.

```ANTLR
expected_warnings
    : 'expectedWarnings' ':' '[' cs_number (',' cs_number)* ']'
    ;
```

Warning *cs_number*s shall appear in the order in which they are output to the console during compilation. When the compiler produces both error and warning messages, they may be interspersed; however, that has no impact on the checking order of this annotation directive’s list. 
Consider the following example, which is intended to result in one warning, CS0108 (“'member1' hides inherited member 'member2'. Use the new keyword if hiding was intended.”):

````
> <!-- Example: {template:"standalone-lib", name:"HidingInherit1", expectedWarnings:["CS0108"]} -->
> ```csharp
> class Base
> {
>     public void F() {}
> }
>
> class Derived : Base
> {
>     public void F() {} // Warning, hiding an inherited name
> }
> ```
````

### Ignoring Compiler Warnings

Some tests may result in one or more compilation warnings, which, for the purpose of testing, may be safely ignored. Examples include CS0168 (“The variable 'var' is declared but never used”) and CS0169 (“The private field 'class member' is never used”. Such warnings can be explicitly ignored using this *annotation_directive*, which is optional.

```ANTLR
ignored_warnings
    : 'ignoredWarnings' ':' '[' cs_number (',' cs_number)* ']'
    ;
```

*cs_number*s may appear in any order, and there is no need to specify the same *cs_number* more than once.

Consider the following *example_annotation*, which expects three occurrences of the same warning:

````
> <!-- Example: {template:"standalone-lib", name:"UsingAliasDirectives11", expectedWarnings:["CS0169", "CS0169", "CS0169"]} -->
````

As that warning can safely be ignored, this intent can be expressed as the following instead:

````
> <!-- Example: {template:"standalone-lib", name:"UsingAliasDirectives11", ignoredWarnings:["CS0169"]} -->
````

Here’s an *example_annotation* showing both expected and ignored warnings:

````
> <!-- Example: {template:"standalone-lib", name:"TryCatchFinally", ignoredWarnings:["CS0219"], expectedWarnings:["CS0162"]} -->
````

### Expected Runtime Output

During execution, some test applications to write one or more lines of output to the console.

> **The tool trims all whitespace from the end of each output string *before* comparing it against any expected or console-block string. As such, make sure an example does *not* generate output lines containing trailing whitespace.**

**Scenario #1:**

In those cases in which an example is followed by a console block containing the expected output, the lines in that block can be checked against the runtime output, using the *annotation_directive* `inferOutput`, as follows:

```ANTLR
infer_output
    : 'inferOutput' ':' ('true' | 'false')
    ;
```

When `true` is specified, the console block is used. When `false` is specified, or the annotation directive is omitted, any console block is ignored.

Consider the following example:

````
> <!-- Example: {template:"standalone-console", name:"FieldInitialization", inferOutput:true, ignoredWarnings:["CS0649"]} -->
> ```csharp
> using System;
>
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
````

If no console block begins within the 8 lines following the end of such an example, the following error message results:

````
Example xx has InferOutput set but no ```console block shortly after it.
````

**Scenario #2:**

In those cases in which an example is *not* followed by a console block containing the expected output, the list of expected output-line strings is provided by the *expected_output* annotation directive, so the two sets of text can be compared, verbatim, for equality. This *annotation_directive* is optional. (This directive makes no provision for output written to a file.)

```ANTLR
expected_output
    : 'expectedOutput' ':' '[' output_string (',' output_string)* ']'
    ;
output_string
    : JSON_string_value
    ;
```

Consider the following example, which writes two lines to the console:

````
> <!-- Example: {template:"standalone-console", name:"ScopeGeneral3", expectedOutput:["hello, world", "A"]} -->
> ```csharp
> using System;
>
> class A {}
>
> class Test
> {
>     static void Main()
>     {
>         string A = "hello, world";
>         string s = A;                      // expression context
>         Type t = typeof(A);                // type context
>         Console.WriteLine(s);              // writes "hello, world"
>         Console.WriteLine(t);              // writes "A"
>     }
> }
> ```
````

**Scenario #3:**

In those cases in which the output is nondeterministic, we can ignore that output by using the following *annotation_directive*, which is optional.

```ANTLR
ignore_output
    : 'ignoreOutput' ':' ('true' | 'false')
```

### Expected Exception

An expected exception can be identified and checked for. This *annotation_directive* is optional.

```ANTLR
expected_exception
    : 'expectedException' ':' exception_name
    ;
exception_name
    : JSON_string_value    // an unqualified typename
    ;
```

Consider the following example:

````
> <!-- Example: {template:"standalone-console", name:"CovarianceException", expectedException:"ArrayTypeMismatchException"} -->
> ```csharp
> class Test
> {
>     static void Fill(object[] array, int index, int count, object value) 
>     {
>         for (int i = index; i < index + count; i++)
>         {
>             array[i] = value;
>         }
>     }
>
>     static void Main() 
>     {
>         string[] strings = new string[100];
>         Fill(strings, 0, 100, "Undefined");
>         Fill(strings, 0, 10, null);
>         Fill(strings, 90, 10, 0);  // throws System.ArrayTypeMismatchException
>     }
> }
> ```
````

### Including Support Files

When an example relies on external support information (such as a type declaration or document-comment include file), one or more files containing that information can be copied to the output directory from the `additional-files` directory within the `template` directory. The *annotation_directive* `additionalFiles` achieves this, and is optional.

```ANTLR
additional_files
    : 'additionalFiles' ':' '[' filename (',' filename)* ']'
    ;

filename
    : JSON_string_value
    ;
```

Consider the following example:

````
<!-- Example: {template:"standalone-lib", name:"IDStringsConstructors", replaceEllipsis:true, additionalFiles:["Acme.cs"]} -->
```csharp
namespace Acme
{
    class Widget : IProcess
    {
        static Widget() { ... }
        public Widget() { ... }
        public Widget(string s) { ... }
    }
}
```
````

where file Acme.cs contains the following:

````
namespace Acme
{
    enum Color { Red, Blue, Green }
    public interface IProcess {}
    public delegate void Del();
}
````

## Other Information

### Unsafe Code

All templates support compilation of examples containing unsafe code.

### Examples Containing Pseudo-Code

Some examples contain C# grammar-rule names, which show a general format rather than source code that will compile. Such names are fenced using the following notation: `«rule_name»`. These examples do not have *example_annotation*s.

Here’s an example:

> When a *resource_acquisition* takes the form of a *local_variable_declaration*, it is possible to acquire multiple resources of a given type. A `using` statement of the form
>
> ```csharp
> using (ResourceType r1 = e1, r2 = e2, ..., rN = eN) «statement»
> ```
>
> is precisely equivalent to a sequence of nested `using` statements:
>
> ```csharp
> using (ResourceType r1 = e1)
> using (ResourceType r2 = e2)
> ...
> using (ResourceType rN = eN)
> «statement»
> ```

### Using Chevron-Quotes for Emphasis

In the Expressions chapter, we use «…» notation for emphasis, as follows:

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

Clearly, this code won’t compile, as is. As such, the ExampleExtractor tool removes the delimiters, leaving identifiers, and the code compiles.

### Tips for Creating New Testable Examples

Here are some things to consider:

Always add an *example_annotation* to a new example that is intended to be tested. If the annotations details are *not* completely known, use something like the following:

````
<!-- Untested$Example: {template:"x", name:"x", replaceEllipsis:true, expectedOutput:["x", "x"], expectedErrors:["x","x"], expectedWarnings:["x","x"], ignoredWarnings:["x","x"], expectedException:"x"} -->
````

That way, a person or tool can easily find untested examples, resolve them, and fill-in the missing information, using something like

````
grep -E '<!-- [A-Za-z]+$Example' standard/*.md
````

If the runtime behavior is implementation-defined, and the annotation is incomplete, use 

````
<!-- ImplementationDefined$Example: { … } -->
````

If the runtime behavior is undefined, and the annotation is incomplete, use 

````
<!-- Undefined$Example: { … } -->
````

If the example involves multiple source files, use
````
<!-- RequiresSeparateFiles$Example: { … } -->
````
