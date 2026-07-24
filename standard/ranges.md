# 18 Extended indexing and slicing

## 18.1 General

This clause introduces a model for *extended indexable* and *sliceable* *collection* types built on:

- The types introduced in this clause, `System.Index` ([§18.2](ranges.md#182-the-index-type)) and `System.Range` ([§18.3](ranges.md#183-the-range-type));
- The pre-defined unary `^` ([§12.9.6](expressions.md#1296-index-from-end-operator)) and binary `..` ([§12.11](expressions.md#1211-range-operator)) operators; and
- The *element_access* expression.

Under the model a type is classified as:

- a *collection* if it represents a group of *element*s
- an *extended indexable* collection if it supports an *element_access* expression which has a single argument expression of type `Index` which returns and/or sets a single element of the type, either by value or by reference; and
- an *extended sliceable* collection if it supports an *element_access* expression which has a single argument expression of type `Range` which returns a *slice* of the elements of the type by value.

> *Note*: The model does not require that a slice of the type can be set, but a type may support it as an extension of the model. *end note*

The model is supported for single-dimensional arrays ([§12.8.12.2](expressions.md#128122-array-access)) and strings ([§12.8.12.3](expressions.md#128123-string-access)).

The model can be supported by any class, struct or interface type which provides appropriate indexers ([§15.9](classes.md#159-indexers)) which implement the model semantics.

Implicit support for the model is provided for types which do not directly support it but which provide a certain *pattern* of members ([§18.4](ranges.md#184-pattern-based-implicit-support-for-index-and-range)). This support is pattern-based, rather than semantic-based, as the semantics of the type members upon which it is based are *assumed* – the language does not enforce, or check, the semantics of these type members.

For the purposes of this clause the following terms are defined:

- A ***collection*** is a type which represents a group of ***element***s.
- A ***countable collection*** is one which provides a ***countable property*** an `int`-valued instance property whose value is the number of elements currently in the group. This property shall be named either `Length` or `Count`. The former is chosen if both exist.
- A ***sequence*** or ***indexable type*** is a collection:
  - which is countable;
  - where every element can be accessed using an *element_access* expression with a single required `int` argument, the ***from-start index***, additional optional arguments are allowed;
  - a sequence is ***modifiable*** if every element can also be set using an *element_access* expression;
  - an element’s from-start index is the number of elements before it in the sequence, for a sequence containing *N* elements:
    - the first and last elements have indices of 0 and *N*-1 respectively, and
    - the ***past-end index***, an index which represents a hypothetical element after the last one, has the value *N*.
- A ***from-end index*** represents an element’s position within a sequence relative to the past-end index. For a sequence containing *N* elements the first, last and past-end indices are *N*, 1 and 0 respectively.
- A ***range*** is a contiguous run of zero or more indices starting at any index within a sequence.
- A ***slice*** is the collection of elements within a range.
- A ***sliceable collection*** is one which:
  - is countable;
  - provides a method `Slice` which takes two `int` parameters specifying a range, being a starting index and a count of elements respectively, and returns a new slice constructed from the elements in the range.

The above definitions are extended for uses of `Index` and `Range` as follows:

- A type is also a *sequence* if an *element_access* expression taking a single required `Index` argument, rather than an `int` argument, is supported. Where a distinction is required the type is termed ***extended indexable***.
- A type is also *sliceable* if an *element_access* expression taking a single required `Range` argument, rather than a `Slice` method, is supported. Where a distinction is required the type is termed ***extended sliceable***.

Whether a type is classified as countable, indexable, or sliceable is subject to the constraints of member accessibility ([§7.5](basic-concepts.md#75-member-access)) and therefore dependent on where the type is being used.

> *Example*: A type where the countable property and/or the indexer are `protected` is only a sequence to members of itself and any derived types. *end example*

The required members for a type to qualify as a sequence or sliceable may be inherited.

> *Example*: In the following code
>
> ```csharp
> public class A
> {
>     public int Length { get { … } }
> }
>
> public class B : A
> {
>     public int this[int index] { … }
> }
>
> public class C : B
> {
>     public int[] Slice(int index, int count) { … }
> }
> ```
>
> The type `A` is countable, `B` is a sequence, and `C` is sliceable and a sequence.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*:
>
> - A type can be sliceable without being indexable due to the lack of an (accessible) indexer.
> - For a type to be sliceable and/or indexable requires the type to be countable.
> - While the elements of a sequence are ordered by *position* within the sequence, the elements themselves need not be ordered by their value, or even orderable.
>
> *end note*

## 18.2 The Index type

The `System.Index` type represents an *abstract* index which represents either a from-start index or a from-end index.

```csharp
    public readonly struct Index : IEquatable<Index>
    {
        public int Value { get; }
        public bool IsFromEnd { get; }

        public Index(int value, bool fromEnd = false);

        public static implicit operator Index(int value);
        public int GetOffset(int length);
        public bool Equals(Index other);
    }
```

`Index` values are constructed from an `int`, specifying the non-negative offset, and a `bool`, indicating whether the offset is from the end (`true`) or start (`false`). If the specified offset is negative an `ArgumentOutOfRangeException` is thrown.

> *Example*
>
> ```csharp
> Index first = new Index(0, false); // first element index
> var last = new Index(1, true);     // last element index
> var past = new Index(0, true);     // past-end index
>
> Index invalid = new Index(-1);     // throws ArgumentOutOfRangeException
> ```
>
> *end example*

There is an implicit conversion from `int` to `Index` which produces from-start indices, and a language-defined unary operator `^` ([§12.9.6](expressions.md#1296-index-from-end-operator)) from `int` to `Index` which produces from-end indices.

> *Example*
>
> Using implicit conversions and the unary `^` operator the above examples may be written:
>
> ```csharp
> Index first = 0; // first element index
> var last = ^1;   // last element index
> var past = ^0;   // past-end index
> ```
>
> *end example*

The method `GetOffset` converts from an abstract `Index` value to a concrete `int` index value for a sequence of the specified `length`. If the `Index` value, `I`, is from-end this method returns the same value as `length - I.Value`, otherwise it returns the same value as `I.Value`.

This method does **not** check that the return value is in the valid range of `0` through `length-1` inclusive.

> *Note:* No checking is specified as the expected use of the result is to index into a sequence with `length` elements, and that indexing operation is expected to perform the appropriate checks. *end note*

`Index` implements `IEquatable<Index>` and values may be compared for equality based on the abstract value; two `Index` values are equal if and only if the respective `Value` and `IsFromEnd` properties are equal. However `Index` values are not ordered and no other comparison operations are provided.

> *Note:* `Index` values are unordered as they are abstract indices, it is in general impossible to determine whether a from-end index comes before or after a from start index without reference to a sequence length. Once converted to concrete indices, e.g. by `GetOffset`, those concrete indices are comparable. *end note*

`Index` values may be directly used in the *argument_list* of an *element_access* expression ([§12.8.12](expressions.md#12812-element-access)) which is:

- an array access and the target is a single-dimensional array ([§12.8.12.2](expressions.md#128122-array-access));
- a string access ([§12.8.12.3](expressions.md#128123-string-access))
- an indexer access and the target type has an indexer with corresponding parameters of either `Index` type ([§12.8.12.4](expressions.md#128124-indexer-access)) or of a type to which `Index` values are implicitly convertible; or
- an indexer access and the target type conforms to a sequence pattern for which implicit `Index` support is specified ([§18.4.2](ranges.md#1842-implicit-index-support)).

## 18.3 The Range type

The `System.Range` type represents the abstract range of `Index`es from a `Start` index up to, but not including, an `End` index.

```csharp
    public readonly struct Range : IEquatable<Range>
    {
        public Index Start { get; }
        public Index End { get; }

        public Range(Index start, Index end);

        public (int Offset, int Length) GetOffsetAndLength(int length);
        public bool Equals(Range other);
    }
```

`Range` values are constructed from two `Index` values.

> *Example*
>
> The following examples use the implicit conversion from `int` to `Index` ([§18.2](ranges.md#182-the-index-type)) and the `^` ([§12.9.6](expressions.md#1296-index-from-end-operator)) operator to create the `Index` values for each `Range`:
>
> ```csharp
> var firstQuad = new Range(0, 4);  // the indices from `0` to `3`
>                                   // int values impicitly convert to `Index`
> var nextQuad = new Range(4, 8);   // the indices from `4` to `7`
> var wholeSeq = new Range(0, ^0);  // the indices from `0` to `N-1` where `N` is the
>                                   // length of the sequence wholeSeq is used with
> var dropFirst = new Range(1, ^0); // the indices from `1` to `N-1`
> var dropLast = new Range(0, ^1);  // the indices from `0` to `N-2`
> var maybeLast = new Range(^1, 6); // the indices from `N-1` to 5
> var lastTwo = new Range(^2, ^0);  // the indices from `N-2` to `N-1`
> ```
>
> *end example*

The language-defined operator `..` ([§12.11](expressions.md#1211-range-operator)) creates a `Range` value from `Index` values.

> *Example*
>
> Using the `..` operator the above examples may be written:
>
> ```csharp
> var firstQuad = 0..4;  // the indices from `0` to `3`
> var nextQuad = 4..8;   // the indices from `4` to `7`
> var wholeSeq = 0..^0;  // the indices from `0` to `N-1`
> var dropFirst = 1..^0; // the indices from `1` to `N-1`
> var dropLast = 0..^1;  // the indices from `0` to `N-2`
> var maybeLast = ^1..6; // the indices from `N-1` to 5
> var lastTwo = ^2..^0;  // the indices from `N-2` to `N-1`
> ```
>
> *end example*

The operands of `..` are optional, the first defaults to `0`, the second defaults to `^0`.

> *Example*
>
> Five of the above examples can be shortened by relying on default values for operands:
>
> ```csharp
> var firstQuad = ..4; // the indices from `0` to `3`
> var wholeSeq = ..;   // the indices from `0` to `N-1`
> var dropFirst = 1..; // the indices from `1` to `N-1`
> var dropLast = ..^1; // the indices from `0` to `N-2`
> var lastTwo = ^2..;  // the indices from `N-2` to `N-1`
> ```
>
> *end example*

A `Range` value is *valid* with respect to a length *L* if:

- the concrete indices with respect to *L* of the `Range` properties `Start` and `End` are in the range 0 to *L*; and
- the concrete index for `Start` is not greater than the concrete index for `End`

The method `GetOffsetAndLength` with an argument `length` converts an abstract `Range` value to a concrete `Range` value represented by tuple. If the `Range` is not valid with respect to `length` the method throws `ArgumentOutOfRangeException`.

The returned concrete `Range` tuple is a pair of the form `(S, N)` where:

- `S` is the starting offset of the range, being the concrete index for the `Start` property of the `Range`; and
- `N` is the number of items in the range, being the difference between the concrete indices for the `End` and `Start` properties;
- both values being calculated with respect to `length`.

A concrete range value is *empty* if `N` is zero. An empty concrete range may have an `S` value equal to concrete past-end index ([§18.1](ranges.md#181-general)), a non-empty range may not. When a `Range` which is used to slice ([§18.1](ranges.md#181-general)) a collection is valid and empty with respect to that collection then the resulting slice is an empty collection.

> *Note:* A consequence of the above is that a `Range` value which is valid and empty with respect to a `length` of zero may be used to slice an empty collection and results in an empty slice. This differs from indexing which throws an exception if the collection is empty. *end note**
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*
>
> Using the variables defined above with `GetOffsetAndLength(6)`:
>
> ```csharp
> var (ix0, len0) = firstQuad.GetOffsetAndLength(6); // ix0 = 0, len0 = 4
> var (ix1, len1) = nextQuad.GetOffsetAndLength(6);  // throws
>    // ArgumentOutOfRangeException as range crosses sequence end
> var (ix2, len2) = wholeSeq.GetOffsetAndLength(6);  // ix2 = 0, len2 = 6
> var (ix3, len3) = dropFirst.GetOffsetAndLength(6); // ix3 = 1, len3 = 5
> var (ix4, len4) = dropLast.GetOffsetAndLength(6);  // ix4 = 0, len4 = 5
> var (ix5, len5) = maybeLast.GetOffsetAndLength(6); // ix5 = 5, len5 = 1
> var (ix6, len6) = lastTwo.GetOffsetAndLength(6);   // ix6 = 4, len6 = 2
> ```

`Range` implements `IEquatable<Range>` and values may be compared for equality based on the abstract value; two `Range` values are equal if and only if the abstract values of the respective `Start` and `End` properties are equal ([§18.2](ranges.md#182-the-index-type)). However `Range` values are not ordered and no other comparison operations are provided.

> *Note:* `Range` values are unordered both as they are abstract and there is no unique ordering relation. Once converted to a concrete start and length, e.g. by `GetOffsetAndLength`, an ordering relation could be defined. *end note*

`Range` values can be directly used in the *argument_list* of an *element_access* expression ([§12.8.12](expressions.md#12812-element-access)) which is:

- an array access and the target is a single-dimensional array ([§12.8.12.2](expressions.md#128122-array-access));
- a string access ([§12.8.12.3](expressions.md#128123-string-access));
- an indexer access and the target type has an indexer with corresponding parameters of either `Range` type ([§12.8.12.4](expressions.md#128124-indexer-access)) or of a type to which `Range` values are implicitly convertible; or
- an indexer access ([§12.8.12.4](expressions.md#128124-indexer-access)) and the target type conforms to a sequence pattern for which implicit `Range` support is specified ([§18.4.3](ranges.md#1843-implicit-range-support)).

## 18.4 Pattern-based implicit support for Index and Range

### 18.4.1 General

If an *element_access* expression ([§12.8.12](expressions.md#12812-element-access)) of the form `E[A]`; where `E` has type `T` and `A` is a single expression implicitly convertible to `Index` or `Range`; fails to be identified as:

- an array access ([§12.8.12.2](expressions.md#128122-array-access)),
- a string access ([§12.8.12.3](expressions.md#128123-string-access)), or
- an indexer access ([§12.8.12.4](expressions.md#128124-indexer-access)) as `T` provides no suitable accessible indexer

then implicit support for the expression is provided if `T` conforms to a particular pattern. If `T` does not conform to this pattern then a compile-time error occurs.

### 18.4.2 Implicit Index support

If in any context an *element_access* expression ([§12.8.12](expressions.md#12812-element-access)) of the form `E[A]`; where `E` has type `T` and `A` is a single expression implicitly convertible to `Index`; is not valid ([§18.4.1](ranges.md#1841-general)) then if in the same context:

- `T` provides accessible members qualifying it as a *sequence* ([§18.1](ranges.md#181-general)); and
- the expression `E[0]` is valid and uses the same indexer that qualifies `T` as a sequence

then the expression `E[A]` shall be implicitly supported.

Without otherwise constraining implementations of this Standard the order of evaluation of the expression shall be equivalent to:

1. `E` is evaluated;
2. `A` is evaluated;
3. the countable property of `T` is evaluated, if required by the implementation;
4. the get or set accessor of the `int` based indexer of `T` that would be used by `E[0]` in the same context is invoked.

### 18.4.3 Implicit Range support

If in any context an *element_access* expression ([§12.8.12](expressions.md#12812-element-access)) of the form `E[A]`; where `E` has type `T` and `A` is a single expression implicitly convertible to `Range`; is not valid ([§18.4.1](ranges.md#1841-general)) then if in the same context:

- `T` provides accessible members qualifying it as both *countable* and *sliceable* ([§18.1](ranges.md#181-general))

then the expression `E[A]` shall be implicitly supported.

Without otherwise constraining implementations of this Standard the order of evaluation of the expression shall be equivalent to:

1. `E` is evaluated;
2. `A` is evaluated;
3. the countable property of `T` is evaluated, if required by the implementation;
4. the `Slice` method of `T` is invoked.
