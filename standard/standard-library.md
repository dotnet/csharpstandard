# Annex C Standard library

## C.1 General

A conforming C# implementation shall provide a minimum set of types having specific semantics. These types and their members are listed here, in alphabetical order by namespace and type. For a formal definition of the types and their members identified in ([§C.2](standard-library.md#c2-standard-library-types-defined-in-isoiec-23271)), refer to ISO/IEC 23271:2012 *Common Language Infrastructure (CLI), Partition IV; Base Class Library (BCL), Extended Numerics Library, and Extended Array Library*, which are included by reference in this specification. For a list of types and their members required beyond those identified in [§C.2](standard-library.md#c2-standard-library-types-defined-in-isoiec-23271), see [§C.3](standard-library.md#c3-standard-library-types-not-defined-in-isoiec-23271).

> *Note*: The adoption of a subset of the CLI’s library API does not create a dependency on the CLI itself; a conforming implementation need not be built upon, or target, the CLI.

**This text is informative.**

The standard library is intended to be the minimum set of types and members required by a conforming C# implementation. As such, it contains only those members that are explicitly required by the C# language specification.

It is expected that a conforming C# implementation will supply a significantly more extensive library that enables useful programs to be written. For example, a conforming implementation might extend this library by

- Adding namespaces.
- Adding types.
- Adding members to non-interface types.
- Adding intervening base classes or interfaces.
- Having struct and class types implement additional interfaces.
- Adding attributes (other than the `ConditionalAttribute`) to existing types and members.

**End of informative text.**

## C.2 Standard Library Types defined in ISO/IEC 23271

> *Note:* Some `struct` types below have the `readonly` modifier. This modifier was not available
> when ISO/IEC 23271 was released, but is required for conforming implementations of this specification. *end note*

```csharp
namespace System
{
    public delegate void Action();

    public class ArgumentException : SystemException
    {
        public ArgumentException();
        public ArgumentException(string? message);
        public ArgumentException(string? message, Exception? innerException);
    }

    public class ArgumentOutOfRangeException : ArgumentException
    {
        public ArgumentOutOfRangeException(string? paramName);
        public ArgumentOutOfRangeException(string? paramName, string? message);
    }

    public class ArithmeticException : Exception
    {
        public ArithmeticException();
        public ArithmeticException(string? message);
        public ArithmeticException(string? message, Exception? innerException);
    }

    public abstract class Array : IList, ICollection, IEnumerable
    {
        public int Length { get; }
        public int Rank { get; }
        public int GetLength(int dimension);
    }

    public class ArrayTypeMismatchException : Exception
    {
        public ArrayTypeMismatchException();
        public ArrayTypeMismatchException(string? message);
        public ArrayTypeMismatchException(string? message,
            Exception? innerException);
    }

    [AttributeUsageAttribute(AttributeTargets.All, Inherited = true,
        AllowMultiple = false)]
    public abstract class Attribute
    {
        protected Attribute();
    }

    public enum AttributeTargets
    {
        Assembly = 0x1,
        Module = 0x2,
        Class = 0x4,
        Struct = 0x8,
        Enum = 0x10,
        Constructor = 0x20,
        Method = 0x40,
        Property = 0x80,
        Field = 0x100,
        Event = 0x200,
        Interface = 0x400,
        Parameter = 0x800,
        Delegate = 0x1000,
        ReturnValue = 0x2000,
        GenericParameter = 0x4000,
        All = 0x7FFF
    }

    [AttributeUsageAttribute(AttributeTargets.Class, Inherited = true)]
    public sealed class AttributeUsageAttribute : Attribute
    {
        public AttributeUsageAttribute(AttributeTargets validOn);
        public bool AllowMultiple { get; set; }
        public bool Inherited { get; set; }
        public AttributeTargets ValidOn { get; }
    }

    public readonly struct Boolean { }
    public readonly struct Byte { }
    public readonly struct Char { }
    public readonly struct Decimal { }
    public abstract class Delegate { }

    public class DivideByZeroException : ArithmeticException
    {
        public DivideByZeroException();
        public DivideByZeroException(string? message);
        public DivideByZeroException(string? message, Exception? innerException);
    }

    public readonly struct Double { }

    public abstract class Enum : ValueType
    {
        protected Enum();
    }

    public class Exception
    {
        public Exception();
        public Exception(string? message);
        public Exception(string? message, Exception? innerException);
        public Exception? InnerException { get; }
        public virtual string Message { get; }
    }

    public static class GC { }

    public interface IDisposable
    {
        void Dispose();
    }

    public interface IEquatable<T>
    {
        bool Equals(T? other);
    }

    public interface IFormattable { }

    public sealed class IndexOutOfRangeException : Exception
    {
        public IndexOutOfRangeException();
        public IndexOutOfRangeException(string? message);
        public IndexOutOfRangeException(string? message,
            Exception? innerException);
    }

    public readonly struct Int16 { }
    public readonly struct Int32 { }
    public readonly struct Int64 { }
    public readonly struct IntPtr { }

    public class InvalidCastException : Exception
    {
        public InvalidCastException();
        public InvalidCastException(string? message);
        public InvalidCastException(string? message, Exception? innerException);
    }

    public class InvalidOperationException : Exception
    {
        public InvalidOperationException();
        public InvalidOperationException(string? message);
        public InvalidOperationException(string? message,
            Exception? innerException);
    }

    public class NotSupportedException : Exception
    {
        public NotSupportedException();
        public NotSupportedException(string? message);
        public NotSupportedException(string? message, 
            Exception? innerException);    
    }

    public struct Nullable<T> where T : struct
    {
        public bool HasValue { get; }
        public T Value { get; }
    }

    public class NullReferenceException : Exception
    {
        public NullReferenceException();
        public NullReferenceException(string? message);
        public NullReferenceException(string? message, Exception? innerException);
    }

    public class Object
    {
        public Object();
        ~Object();
        public virtual bool Equals(object? obj);
        public virtual int GetHashCode();
        public Type GetType();
        public virtual string? ToString();
    }

    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Struct |
        AttributeTargets.Enum | AttributeTargets.Interface |
        AttributeTargets.Constructor | AttributeTargets.Method |
        AttributeTargets.Property | AttributeTargets.Field |
        AttributeTargets.Event | AttributeTargets.Delegate, Inherited = false)]
    public sealed class ObsoleteAttribute : Attribute
    {
        public ObsoleteAttribute();
        public ObsoleteAttribute(string? message);
        public ObsoleteAttribute(string? message, bool error);
        public bool IsError { get; }
        public string? Message { get; }
    }

    public class OutOfMemoryException : Exception
    {
        public OutOfMemoryException();
        public OutOfMemoryException(string? message);
        public OutOfMemoryException(string? message, Exception? innerException);
    }

    public class OverflowException : ArithmeticException
    {
        public OverflowException();
        public OverflowException(string? message);
        public OverflowException(string? message, Exception? innerException);
    }

    public readonly struct SByte { }
    public readonly struct Single { }

    public sealed class StackOverflowException : Exception
    {
        public StackOverflowException();
        public StackOverflowException(string? message);
        public StackOverflowException(string? message, Exception? innerException);
    }

    public sealed class String : IEnumerable<char>, IEnumerable
    {
        public int Length { get; }
        public char this [int index] { get; }
        public static string Format(string format, params object?[] args);
        System.Collections.IEnumerator IEnumerable.GetEnumerator();
        System.Collections.Generic.IEnumerator<char> IEnumerable<char>.GetEnumerator();
    }

    public class SystemException : Exception
    {
        public SystemException();
        public SystemException(string? message);
        public SystemException(string? message, Exception? innerException);
    }

    public abstract class Type : MemberInfo { }

    public sealed class TypeInitializationException : Exception
    {
        public TypeInitializationException(string fullTypeName,
            Exception? innerException);
    }

    public readonly struct UInt16 { }
    public readonly struct UInt32 { }
    public readonly struct UInt64 { }
    public readonly struct UIntPtr { }

    public abstract class ValueType
    {
        protected ValueType();
    }
}

namespace System.Collections
{
    public interface ICollection : IEnumerable
    {
        int Count { get; }
        bool IsSynchronized { get; }
        object SyncRoot { get; }
        void CopyTo(Array array, int index);
    }

    public interface IEnumerable
    {
        IEnumerator GetEnumerator();
    }

    public interface IEnumerator
    {
        object Current { get; }
        bool MoveNext();
        void Reset();
    }

    public interface IList : ICollection, IEnumerable
    {
        bool IsFixedSize { get; }
        bool IsReadOnly { get; }
        object? this [int index] { get; set; }
        int Add(object? value);
        void Clear();
        bool Contains(object? value);
        int IndexOf(object? value);
        void Insert(int index, object? value);
        void Remove(object? value);
        void RemoveAt(int index);
    }
}

namespace System.Collections.Generic
{
    public interface ICollection<T> : IEnumerable<T>
    {
        int Count { get; }
        bool IsReadOnly { get; }
        void Add(T item);
        void Clear();
        bool Contains(T item);
        void CopyTo(T[] array, int arrayIndex);
        bool Remove(T item);
    }

    public interface IEnumerable<T> : IEnumerable
    {
        IEnumerator<T> GetEnumerator();
    }

    public interface IEnumerator<T> : IDisposable, IEnumerator
    {
        T Current { get; }
    }

    public interface IList<T> : ICollection<T>
    {
        T this [int index] { get; set; }
        int IndexOf(T item);
        void Insert(int index, T item);
        void RemoveAt(int index);
    }
}

namespace System.Diagnostics
{
    [AttributeUsageAttribute(AttributeTargets.Method | AttributeTargets.Class,
                             AllowMultiple = true)]
    public sealed class ConditionalAttribute : Attribute
    {
        public ConditionalAttribute(string conditionString);
        public string ConditionString { get; }
    }
}

namespace System.Reflection
{
    public abstract class MemberInfo
    {
        protected MemberInfo();
    }
}

namespace System.Runtime.CompilerServices
{
    public sealed class IndexerNameAttribute : Attribute
    {
        public IndexerNameAttribute(string indexerName);
    }

    public static class Unsafe
    {
        public static ref T NullRef<T>();
        public static bool IsNullRef<T>(ref T source);
    }
}

namespace System.Threading
{
    public static class Monitor
    {
        public static void Enter(object obj);
        public static void Exit(object obj);
    }
}
```

## C.3 Standard Library Types not defined in ISO/IEC 23271

The following types, including the members listed, shall be defined in a conforming standard library. (These types might be defined in a future edition of ISO/IEC 23271.) It is expected that many of these types will have more members available than are listed.

A conforming implementation may provide `Task.GetAwaiter()` and `Task<TResult>.GetAwaiter()` as extension methods.

```csharp
namespace System
{
    public interface IAsyncDisposable
    {
        System.Threading.Tasks.ValueTask DisposeAsync();
    }

    public abstract class FormattableString : IFormattable { }

    public static class MemoryExtensions
    {
        public static ReadOnlySpan<char> AsSpan (this string? text);
        public static bool SequenceEqual<T> (this Span<T> span, ReadOnlySpan<T> other)
          where T : IEquatable<T>;
        public static bool SequenceEqual<T> (this ReadOnlySpan<T> span,
          ReadOnlySpan<T> other) where T : IEquatable<T>;
    }

    public class OperationCanceledException : Exception
    {
        public OperationCanceledException();
        public OperationCanceledException(string? message);
        public OperationCanceledException(string? message, Exception? innerException);
    }

    /// <summary>
    ///    A read-only value type which represents an abstract
    ///    index to be used with collections.
    ///    - The Index can be relative to the start or end of a
    ///      collection.
    ///    - An Index can be converted to a zero-based concrete
    ///      from-start index to be used with a collection
    ///      of some specified length.
    ///    - Equality between Index values is provided, however
    ///      unlike concrete indices they are not ordered.
    ///    - Array and String element access support indexing
    ///      with Index values.
    /// </summary>
    public readonly struct Index : IEquatable<Index>
    {
        /// <summary>
        ///    Construct an Index from an integer value and a
        ///    boolean indicating whether the value is relative
        ///    to the end (true) or start (false).
        /// </summary>
        /// <param name="value">
        ///    The value, must be ≥ 0.
        /// </param>
        /// <param name="fromEnd">
        ///    Optional boolean indicating whether the Index is
        ///    relative to the end (true) or start (false).
        ///    The default value is false.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///    Thrown if value < 0.
        /// </exception>
        /// <remarks>
        ///    If the Index is relative to the start then:
        ///       - the value 0 refers to the first element.
        ///    If the Index is relative to the end then:
        ///       - the value 1 refers to the last element; and
        ///       - the value 0 refers to beyond last element.
        /// </remarks>
        public Index(int value, bool fromEnd = false);

        /// <summary>
        ///    Implicit conversion from integer to a
        ///    from-start Index.
        /// </summary>
        /// <remarks>
        ///    The predefined operator:
        ///       <c>Index operator ^(int value);</c>
        ///    is provided to convert from integer to a
        ///    from-end Index.
        /// </remarks>
        public static implicit operator Index(int value);

        /// <summary>
        ///    Return the value.
        /// </summary>
        public int Value { get; }

        /// <summary>
        ///    Return whether the Index is relative to
        ///    the end (true) or start (false).
        /// </summary>
        public bool IsFromEnd { get; }

        /// <summary>
        ///    Return a concrete from-start index for a
        ///    given collection length.
        /// </summary>
        /// <param name="length">
        ///    The length of the collection that the index
        ///    will be used with.
        /// </param>
        /// <remarks>
        ///    This method performs no sanity checking and
        ///    will never throw an IndexOutOfRangeException.
        ///    It is expected that the returned index will be
        ///    used with a collection which will do validation.
        /// </remarks>
        public int GetOffset(int length);

        /// <summary>
        ///    Indicates whether the current Index value is
        ///    equal to another Index value.
        /// </summary>
        /// <param name="other">
        ///    The value to compare with this Index.
        /// </param>
        public bool Equals(Index other);
    }

    /// <summary>
    ///    A read-only value type which represents a range of
    ///    abstract indices to be used with collections.
    ///    - The Range has two Index properties, Start and End.
    ///    - A Range can be converted to a concrete index from
    ///      the start and a length value to be used with a
    ///      collection of some specified length.
    ///    - Equality between Range values is provided,
    ///      however they are not ordered.
    ///    - Array and String element access supports indexing
    ///      with Range values, returning a sub-array/substring
    ///      of the indexed value respectively.
    /// </summary>
    public readonly struct Range : IEquatable<Range>
    {
        /// <summary>
        ///    Construct a Range from two Index values.
        /// </summary>
        /// <param name="start">
        ///    The inclusive Index value for the start
        ///    of the range.
        /// </param>
        /// <param name="end">
        ///    The exclusive Index value for the end
        ///    of the range.</param>
        /// <remarks>
        ///    As Index values represent unordered abstract
        ///    indices no sanity checking can be performed
        ///    on the resultant Range value,
        ///    <see cref="GetOffsetAndLength">".
        ///
        ///    The predefined operator:
        ///       <c>Range operator ..(Index start, Index end);</c>
        ///    also exists to create a Range value.
        /// </remarks>
        public Range(Index start, Index end);

        /// <summary>Return the starting Index.</summary>
        public Index Start { get; }

        /// <summary>Return the ending Index.</summary>
        public Index End { get; }

        /// <summary>
        ///    Return a concrete from-start index and the
        ///    range length for a given collection length.
        /// </summary>
        /// <param name="length">
        ///    The length of the collection that the result
        ///    will be used with.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///    Thrown if the range is not valid wrt length.
        /// </exception>
        /// <returns>
        ///    A tuple consisting of an index value and range length
        /// </returns>
        public (int Offset, int Length) GetOffsetAndLength(int length);

        /// <summary>
        ///    Indicates whether the current Range value is equal
        ///    to another Range value.
        /// </summary>
        /// <param name="other">
        ///    The value to compare with this Range.
        /// </param>
        public bool Equals(Range other);
    }

    public readonly ref struct ReadOnlySpan<T>
    {
        public int Length { get; }
        public ref readonly T this[int index] { get; }
    }

    public readonly ref struct Span<T>
    {
        public int Length { get; }
        public ref T this[int index] { get; }
        public static implicit operator ReadOnlySpan<T>(Span<T> span);
    }

    public struct ValueTuple<T1>
    {
        public T1 Item1;
        public ValueTuple(T1 item1);
    }

    public struct ValueTuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
        public ValueTuple(T1 item1, T2 item2);
    }

    public struct ValueTuple<T1, T2, T3>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public ValueTuple(T1 item1, T2 item2, T3 item3);
    }

    public struct ValueTuple<T1, T2, T3, T4>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4);
    }

    public struct ValueTuple<T1, T2, T3, T4, T5>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5);
    }

    public struct ValueTuple<T1, T2, T3, T4, T5, T6>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5,
            T6 item6);
    }

    public struct ValueTuple<T1, T2, T3, T4, T5, T6, T7>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5,
            T6 item6, T7 item7);
    }

    public struct ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public TRest Rest;
        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5,
            T6 item6, T7 item7, TRest rest);
    }
}

namespace System.Collections.Generic
{
    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        int Count { get; }
    }

    public interface IReadOnlyList<out T> : IReadOnlyCollection<T>
    {
        T this [int index] { get; }
    }

    public interface IAsyncEnumerable<out T>
    {
        IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token = default);
    }

    public interface IAsyncEnumerator<out T> : IAsyncDisposable
    {
        System.Threading.Tasks.ValueTask<bool> MoveNextAsync();
        T Current { get; }
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    [System.AttributeUsage(System.AttributeTargets.Field |
      System.AttributeTargets.Parameter | System.AttributeTargets.Property,
      Inherited=false)]
    public sealed class AllowNullAttribute : Attribute
    {
        public AllowNullAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Field |
      System.AttributeTargets.Parameter | System.AttributeTargets.Property,
      Inherited=false)]
    public sealed class DisallowNullAttribute : Attribute
    {
        public DisallowNullAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Method, Inherited=false)]
    public sealed class DoesNotReturnAttribute : Attribute
    {
        public DoesNotReturnAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter, Inherited=false)]
    public sealed class DoesNotReturnIfAttribute : Attribute
    {
        public DoesNotReturnIfAttribute(bool parameterValue);
    }

    [System.AttributeUsage(System.AttributeTargets.Field | 
      System.AttributeTargets.Parameter | System.AttributeTargets.Property | 
      System.AttributeTargets.ReturnValue, Inherited=false)]
    public sealed class MaybeNullAttribute : Attribute
    {
        public MaybeNullAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter, Inherited=false)]
    public sealed class MaybeNullWhenAttribute : Attribute
    {
        public MaybeNullWhenAttribute(bool returnValue);
    }

    [System.AttributeUsage(System.AttributeTargets.Method | 
    System.AttributeTargets.Property, AllowMultiple=true, Inherited=false)]
    public sealed class MemberNotNullAttribute : Attribute
    {
        public MemberNotNullAttribute(string member) {}
        public MemberNotNullAttribute(params string[] members) {}
    }

    [System.AttributeUsage(System.AttributeTargets.Method | 
      System.AttributeTargets.Property, AllowMultiple=true, Inherited=false)]
    public sealed class MemberNotNullWhenAttribute : Attribute
    {
        public MemberNotNullWhenAttribute(bool returnValue, string member) {}
        public MemberNotNullWhenAttribute(bool returnValue, params string[] members) {}
    }

    [System.AttributeUsage(System.AttributeTargets.Field |
      System.AttributeTargets.Parameter | System.AttributeTargets.Property | 
      System.AttributeTargets.ReturnValue, Inherited=false)]
    public sealed class NotNullAttribute : Attribute
    {
        public NotNullAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter | 
      System.AttributeTargets.Property | System.AttributeTargets.ReturnValue, 
      AllowMultiple=true, Inherited=false)]
    public sealed class NotNullIfNotNullAttribute : Attribute
    {
        public NotNullIfNotNullAttribute(string parameterName);
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter, Inherited=false)]
    public sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue);
    }

    [System.AttributeUsage(System.AttributeTargets.Method
      | System.AttributeTargets.Parameter
      | System.AttributeTargets.Property,
      AllowMultiple=false, Inherited=false)]
    public sealed class UnscopedRefAttribute : Attribute
    {
        public UnscopedRefAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Constructor,
      AllowMultiple=false, Inherited=false)]
    public sealed class SetsRequiredMembersAttribute : Attribute
    {
        public SetsRequiredMembersAttribute() {}
    }
}

namespace System.Linq.Expressions
{
    public sealed class Expression<TDelegate>
    {
        public TDelegate Compile();
        public static UnaryExpression NegateChecked(Expression expression,
            MethodInfo? method);
        public static BinaryExpression AddChecked(Expression left,
            Expression right, MethodInfo? method);
        public static BinaryExpression SubtractChecked(Expression left,
            Expression right, MethodInfo? method);
        public static BinaryExpression MultiplyChecked(Expression left,
            Expression right, MethodInfo? method);
        public static UnaryExpression ConvertChecked(Expression expression,
            Type type, MethodInfo? method);
    }
}

namespace System.Runtime.CompilerServices
{
    [System.AttributeUsage(System.AttributeTargets.Class
      | System.AttributeTargets.Interface
      | System.AttributeTargets.Struct, Inherited=false)]
    public sealed class CollectionBuilderAttribute : System.Attribute
    {
        public CollectionBuilderAttribute(Type builderType, string methodName);
        public Type BuilderType { get; }
        public string MethodName { get; }
    }

    public ref struct DefaultInterpolatedStringHandler
    {
        public DefaultInterpolatedStringHandler(int literalLength,
            int formattedCount);
        public DefaultInterpolatedStringHandler(int literalLength,
            int formattedCount, IFormatProvider? provider);
        public DefaultInterpolatedStringHandler(int literalLength,
            int formattedCount, IFormatProvider? provider, Span<char> initialBuffer);
        public void AppendFormatted(scoped ReadOnlySpan<char> value);
        public void AppendFormatted(string? value);
        public void AppendFormatted(object? value, int alignment = 0,
            string? format = default);
        public void AppendFormatted(scoped ReadOnlySpan<char> value,
            int alignment = 0, string? format = default);
        public void AppendFormatted(string? value, int alignment = 0,
            string? format = default);
        public void AppendFormatted<T>(T value);
        public void AppendFormatted<T>(T value, int alignment);
        public void AppendFormatted<T>(T value, string? format);
        public void AppendFormatted<T>(T value, int alignment, string? format);
        public void AppendLiteral(string value);
        public override string ToString();
        public string ToStringAndClear();
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | 
        AttributeTargets.Interface | AttributeTargets.Method, 
        Inherited = false, AllowMultiple = false)]
    public sealed class AsyncMethodBuilderAttribute : Attribute
    {
        public AsyncMethodBuilderAttribute(Type builderType);
 
        public Type BuilderType { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false,
        Inherited = false)]
    public sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName);
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerFilePathAttribute : Attribute
    {
        public CallerFilePathAttribute();
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerLineNumberAttribute : Attribute
    {
        public CallerLineNumberAttribute();
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerMemberNameAttribute : Attribute
    {
        public CallerMemberNameAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter, Inherited=false)]
    public sealed class EnumeratorCancellationAttribute : Attribute
    {
        public EnumeratorCancellationAttribute();
    }
    
    public static class FormattableStringFactory
    {
        public static FormattableString Create(string format,
            params object?[] arguments);
    }

    public interface ICriticalNotifyCompletion : INotifyCompletion
    {
        void UnsafeOnCompleted(Action continuation);
    }

    [AttributeUsage(AttributeTargets.Struct, AllowMultiple=false)]
    public sealed class InlineArrayAttribute : Attribute
    {
        public InlineArrayAttribute(int length);

        public int Length { get; }
    }

    public interface INotifyCompletion
    {
        void OnCompleted(Action continuation);
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter,
        AllowMultiple=false, Inherited=false)]
    public sealed class InterpolatedStringHandlerArgumentAttribute : Attribute
    {
        public InterpolatedStringHandlerArgumentAttribute(string argument);
        public InterpolatedStringHandlerArgumentAttribute(params string[] arguments);
    }

    [System.AttributeUsage(System.AttributeTargets.Class |
        System.AttributeTargets.Struct, AllowMultiple=false, Inherited=false)]
    public sealed class InterpolatedStringHandlerAttribute : Attribute
    {
        public InterpolatedStringHandlerAttribute (); 
    }

    /// <summary>
    ///    Provides indexed access to the elements of a
    ///    tuple-like value at runtime. This interface is
    ///    used by the language to implement positional
    ///    pattern matching (§11.2.5) when no static
    ///    tuple type or <c>Deconstruct</c> method is
    ///    available.
    /// </summary>
    public interface ITuple
    {
        /// <summary>
        ///    The number of elements in the tuple.
        /// </summary>
        /// <remarks>
        ///    The value returned shall be non-negative,
        ///    and shall not change for the lifetime of
        ///    the instance.
        /// </remarks>
        int Length { get; }

        /// <summary>
        ///    Returns the element at the specified
        ///    zero-based position.
        /// </summary>
        /// <param name="index">
        ///    The zero-based position of the element
        ///    to return; shall be ≥ 0 and &lt; <see cref="Length"/>.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        ///    Thrown if <paramref name="index"/> is
        ///    outside the range
        ///    <c>[0, Length)</c>.
        /// </exception>
        object? this[int index] { get; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ModuleInitializerAttribute : Attribute
    {
        public ModuleInitializerAttribute() { }
    }

    [System.AttributeUsage(System.AttributeTargets.Class |
      System.AttributeTargets.Field | System.AttributeTargets.Property |
      System.AttributeTargets.Struct, AllowMultiple=false, Inherited=false)]
    public sealed class RequiredMemberAttribute : Attribute
    {
        public RequiredMemberAttribute() {}
    }

    public readonly struct TaskAwaiter : ICriticalNotifyCompletion,
        INotifyCompletion
    {
        public bool IsCompleted { get; }
        public void GetResult();
    }

    public readonly struct TaskAwaiter<TResult> : ICriticalNotifyCompletion,
        INotifyCompletion
    {
        public bool IsCompleted { get; }
        public TResult GetResult();
    }

    [System.AttributeUsage(System.AttributeTargets.Method, Inherited=false)]
    public sealed class UnmanagedCallersOnlyAttribute : Attribute
    {
        public UnmanagedCallersOnlyAttribute ();
        public Type[]? CallConvs;
        public string? EntryPoint;
    }

    public readonly struct ValueTaskAwaiter : ICriticalNotifyCompletion,
        INotifyCompletion
    {
        public bool IsCompleted { get; }
        public void GetResult();
    }

    public readonly struct ValueTaskAwaiter<TResult>
        : ICriticalNotifyCompletion, INotifyCompletion
    {
        public bool IsCompleted { get; }
        public TResult GetResult();
    }
}

namespace System.Threading
{
    public class CancellationTokenSource : IDisposable
    {
        public CancellationTokenSource();
        public System.Threading.CancellationToken Token { get; }
        public void Cancel();
        public static CancellationTokenSource CreateLinkedTokenSource
                                             (CancellationToken token1,
                                              CancellationToken token2);
    }

    public readonly struct CancellationToken : IEquatable<CancellationToken>
    {
        public bool IsCancellationRequested { get; }
    }
}

namespace System.Threading.Tasks
{
    public class Task
    {
        public System.Runtime.CompilerServices.TaskAwaiter GetAwaiter();
    }

    public class Task<TResult> : Task
    {
        public new System.Runtime.CompilerServices.TaskAwaiter<TResult> GetAwaiter();
    }

    public readonly struct ValueTask : System.IEquatable<ValueTask>
    {
        public System.Runtime.CompilerServices.ValueTaskAwaiter GetAwaiter();
    }

    public readonly struct ValueTask<TResult>
        : System.IEquatable<ValueTask<TResult>>
    {
        public new System.Runtime.CompilerServices.ValueTaskAwaiter<TResult>
            GetAwaiter();
    }
}
```

## C.4 Format Specifications

The meaning of the formats, as used in interpolated string expressions ([§12.8.3](expressions.md#1283-interpolated-string-expressions)), are defined in ISO/IEC 23271:2012. For convenience the following text is copied from the description of `System.IFormattable`.

**This text is informative.**

A *format* is a string that describes the appearance of an object when
it is converted to a string. Either standard or custom formats can be used. A
standard format takes the form *Axx*, where *A* is a single
alphabetic character called the *format specifier*, and *xx* is an integer between zero and 99 inclusive, called the *precision specifier*. The format specifier controls the type
of formatting applied to the value being represented as a string. The
*precision specifier* controls the number of significant digits or decimal places in the string, if applicable.

> *Note*: For the list of standard format specifiers, see the table below. Note that a given data type, such as `System.Int32`, might not support one or more of the standard format specifiers. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: When a format includes symbols that vary by culture, such as the currencysymbol included by the ‘C’ and ‘c’ formats, a formatting object supplies the actual characters used in the string representation. A method might include a parameter to pass a `System.IFormatProvider` object that supplies a formatting object, or the method might use the default formatting object, which contains the symbol definitions for the current culture. The current culture typically uses the same set of symbols used system-wide by default. In the Base Class Library, the formatting object for system-supplied numeric types is a `System.Globalization.NumberFormatInfo` instance. For `System.DateTime` instances, a `System.Globalization.DateTimeFormatInfo` is used. *end note*

The following table describes the standard format specifiers and associated formatting
object members that are used with numeric data types in the Base Class
Library.

<!-- Custom Word conversion: format_strings_1 -->
<table>
<tr>
<th>Format Specifier</th>
<th>Description</th>
</tr>
<tr>
<td><p><code>C</code></p>
<p><code>c</code></p></td>
<td><p><strong>Currency Format:</strong> Used for strings containing a monetary value. The <code>System.Globalization.NumberFormatInfo.CurrencySymbol</code>, <code>System.Globalization.NumberFormatInfo.CurrencyGroupSizes</code>, <code>System.Globalization.NumberFormatInfo.CurrencyGroupSeparator</code>, and <code>System.Globalization.NumberFormatInfo.CurrencyDecimalSeparator</code> members of a <code>System.Globalization.NumberFormatInfo</code>
supply the currency symbol, size and separator for digit groupings, and
decimal separator, respectively.</p>
<p><code>System.Globalization.NumberFormatInfo.CurrencyNegativePattern</code> and <code>System.Globalization.NumberFormatInfo.CurrencyPositivePattern</code> determine the symbols used to represent negative
and positive values. For example, a negative value can be prefixed with a
minus sign, or enclosed in parentheses.</p>
<p>If the precision specifier is omitted, <code>System.Globalization.NumberFormatInfo.CurrencyDecimalDigits</code> determines the number of decimal places in the
string. Results are rounded to the nearest representable value when
necessary.</p></td>
</tr>
<tr>
<td><p><code>D</code></p>
<p><code>d</code></p></td>
<td><p><strong>Decimal Format:</strong> (This format is valid only
when specified with integral data types.) Used for strings containing
integer values. Negative numbers are prefixed with the negative number
symbol specified by the <code>System.Globalization.NumberFormatInfo.NegativeSign</code>
property.</p>
<p>The precision specifier determines the
minimum number of digits that appear in the string. If the specified
precision requires more digits than the value contains, the string is
left-padded with zeros. If the precision specifier specifies fewer digits
than are in the value, the precision specifier is
ignored.</p></td>
</tr>
<tr>
<td><p><code>E</code></p>
<p><code>e</code></p></td>
<td><p><strong>Scientific (Engineering) Format:</strong> Used for strings in
one of the following forms:</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-]<em>m.dddddd</em>E<em>+xxx</em></p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-]<em>m.dddddd</em>E<em>-xxx</em></p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-]<em>m.dddddd</em>e<em>+xxx</em></p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-]<em>m.dddddd</em>e<em>-xxx</em></p>
<p>The negative number symbol (‘-’) appears only if
the value is negative, and is supplied by the <code>System.Globalization.NumberFormatInfo.NegativeSign</code> property.</p>
<p>Exactly one non-zero decimal digit (<em>m</em>) precedes the decimal separator (‘.’), which
is supplied by the <code>System.Globalization.NumberFormatInfo.NumberDecimalSeparator</code>
property.</p>
<p>The precision specifier determines the number of decimal places
(<em>dddddd</em>) in the string. If the precision specifier
is omitted, six decimal places are included in the
string.</p>
<p>The exponent
(<em>+/-xxx</em>)
consists of either a positive or negative number symbol followed by a
minimum of three digits (<em>xxx</em>). The exponent is
left-padded with zeros, if necessary. The case of the format specifier
(‘E’ or ‘e’) determines the case used for the exponent prefix (E or e) in
the string. Results are rounded to the nearest representable value when
necessary. The positive number symbol is supplied by the <code>System.Globalization.NumberFormatInfo.PositiveSign</code>
property.</p></td>
</tr>
<tr>
<td><p><code>F</code></p>
<p><code>f</code></p></td>
<td><p><strong>Fixed-Point Format:</strong> Used for strings in the following
form:</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-]<em>m.dd...d</em></p>
<p>At least one non-zero decimal digit (<em>m</em>) precedes the decimal separator (‘.’), which is
supplied by the <code>System.Globalization.NumberFormatInfo.NumberDecimalSeparator</code>
property.</p>
<p>A
negative number symbol sign (‘-’) precedes <em>m</em> only if the value is negative. This symbol is
supplied by the <code>System.Globalization.NumberFormatInfo.NegativeSign</code>
property.</p>
<p>The precision specifier determines the number of decimal places
(<em>dd...d</em>) in the string. If the precision specifier is omitted,
<code>System.Globalization.NumberFormatInfo.NumberDecimalDigits</code> determines the number of decimal places in the string. Results are rounded to the nearest representable
value when necessary.</p></td>
</tr>
<tr>
<td><p><code>G</code></p>
<p><code>g</code></p></td>
<td><p><strong>General Format:</strong> The string is formatted in either fixed-point format (‘F’ or ‘f’) or scientific format (‘E’ or ‘e’).</p>
<p>For integral types:</p>
<p>Values are formatted using fixed-point format if
<em>exponent</em> &lt; precision specifier, where <em>exponent </em> is the exponent of the value in scientific format. For all other values, scientific format is used.</p>
<p>If the precision specifier is omitted, a default
precision equal to the field width required
to display the
maximum value for the data
type is used, which results in the value being formatted in
fixed-point format. The default precisions for integral types are as
follows:</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<code>System.Int16</code>, <code>System.UInt16</code> : 5</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<code>System.Int32</code>, <code>System.UInt32</code> : 10</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<code>System.Int64</code>, <code>System.UInt64</code> : 19</p>
<p>For Single, Decimal and Double
types:</p>
<p>Values are formatted using fixed-point format
if <em>exponent</em> ≥ -4 and <em>exponent</em> &lt; precision specifier, where <em>exponent</em> is
the exponent of the value in scientific format. For all other values,
scientific format is used. Results
are rounded to the nearest representable value when necessary.</p>
<p>If the precision specifier is omitted, the following default precisions are used:</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<code>System.Single</code> : 7</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<code>System.Double</code> : 15</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<code>System.Decimal</code> : 29</p>
<p>For all types:</p>
<ul><li>The
number of digits that appear in the result (not including the exponent)
will not exceed the value of the precision specifier; values are rounded
as necessary.</li>
<li>The
decimal point and any trailing zeros after the decimal point are removed
whenever possible.</li>
<li>The
case of the format specifier (‘G’ or ‘g’) determines whether ‘E’ or ‘e’
prefixes the scientific format exponent.</li></ul></p></td>
</tr>
<tr>
<td><p><code>N</code></p>
<p><code>n</code></p></td>
<td><p><strong>Number Format:</strong> Used for strings in the following form:</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[-]<em>d,ddd,ddd.dd...d</em></p>
<p>The representation of negative values is
determined by the <code>System.Globalization.NumberFormatInfo.NumberNegativePattern</code> property. If the pattern includes a negative number
symbol (‘-’), this symbol is supplied by the <code>System.Globalization.NumberFormatInfo.NegativeSign</code> property.</p>
<p>At least one non-zero decimal digit (<em>d</em>) precedes
the decimal separator (‘.’), which is supplied by the <code>System.Globalization.NumberFormatInfo.NumberDecimalSeparator</code> property. Digits between the decimal
point and the most significant digit in the value are grouped using the
group size specified by the <code>System.Globalization.NumberFormatInfo.NumberGroupSizes</code> property. The group separator (‘,’)
is inserted between each digit group, and is supplied by the <code>System.Globalization.NumberFormatInfo.NumberGroupSeparator</code>
property.</p>
<p>The precision specifier determines the number of
decimal places (<em>dd...d</em>). If the precision specifier is omitted,
<code>System.Globalization.NumberFormatInfo.NumberDecimalDigits</code> determines the number of decimal places in the
string. Results are rounded to the nearest representable value when
necessary.</p></td>
</tr>
<tr>
<td><p><code>P</code></p>
<p><code>p</code></p></td>
<td><p><strong>Percent Format:</strong> Used for strings containing a
percentage. The <code>System.Globalization.NumberFormatInfo.PercentSymbol</code>, <code>System.Globalization.NumberFormatInfo.PercentGroupSizes</code>, <code>System.Globalization.NumberFormatInfo.PercentGroupSeparator</code>, and <code>System.Globalization.NumberFormatInfo.PercentDecimalSeparator</code> members of a <code>System.Globalization.NumberFormatInfo</code>
supply the percent symbol, size and separator for digit groupings, and
decimal separator, respectively.</p>
<p><code>System.Globalization.NumberFormatInfo.PercentNegativePattern</code> and <code>System.Globalization.NumberFormatInfo.PercentPositivePattern</code> determine the symbols used to represent negative
and positive values. For example, a negative value can be prefixed with a
minus sign, or enclosed in parentheses.</p>
<p>If no precision is specified, the number of decimal places in the
result is determined by <code>System.Globalization.NumberFormatInfo.PercentDecimalDigits</code>. Results are rounded to the nearest representable
value when necessary.</p>
<p>The result is scaled by 100 (.99 becomes 99%).</p></td>
</tr>
<tr>
<td><p><code>R</code></p>
<p><code>r</code></p></td>
<td><strong>Round trip Format:</strong> (This format is valid only when
specified with <code>System.Double</code> or <code>System.Single</code>.) Used to ensure that the precision of the string
representation of a floating-point value is such that parsing the string
does not result in a loss of precision when compared to the original
value. If the maximum precision of the data type (7 for <code>System.Single</code>, and 15 for
<code>System.Double</code>) would result in a loss of precision, the precision is increased by
two decimal places. If a precision specifier is supplied with this format specifier,
it is ignored. This format is otherwise identical to the fixed-point
format.</td>
</tr>
<tr>
<td><p><code>X</code></p>
<p><code>x</code></p></td>
<td><strong>Hexadecimal Format:</strong> (This format is valid only when
specified with integral data types.) Used for string representations of numbers in Base
16. The precision determines the minimum number of digits in
the string. If the precision specifies more digits than the number contains,
the number is left-padded with zeros. The case of the format specifier
(‘X’ or ‘x’) determines whether upper case or lower case
letters are used in the hexadecimal representation.</td>
</tr>
</table>

If the numerical value is a `System.Single` or `System.Double` with a value of `NaN`,
`PositiveInfinity`, or `NegativeInfinity`, the format specifier is ignored, and one of the following is returned: `System.Globalization.NumberFormatInfo.NaNSymbol`, `System.Globalization.NumberFormatInfo.PositiveInfinitySymbol`, or `System.Globalization.NumberFormatInfo.NegativeInfinitySymbol`.

A custom format is any string specified as a format that
is not in the form of a standard format string (Axx) described above. The
following table describes the characters that are used in constructing custom
formats.

<!-- Custom Word conversion: format_strings_2 -->
<table>
<tr>
<th>Format Specifier</th>
<th>Description</th>
</tr>
<tr>
<td><code>0</code> (zero)</td>
<td><p><strong>Zero placeholder:</strong>
If
the value being formatted has a digit in the position where a ‘0’ appears in the custom format, then that digit is copied to the output string;
otherwise a zero is stored in that position in the output string. The
position of the leftmost ‘0’ before the decimal separator and the
rightmost ‘0’ after the decimal separator determine the range of digits
that are always present in the output string.</p>
<p>The number of Zero and/or Digit placeholders after
the decimal separator determines the number of digits that appear after
the decimal separator. Values are rounded as necessary.</p></td>
</tr>
<tr>
<td><code>#</code></td>
<td><p><strong>Digit placeholder:</strong>
If the value being formatted has a digit in
the position where a ‘#’ appears in the custom format, then that digit
is copied to the output string; otherwise, nothing is stored in that
position in the output string. Note that this specifier never stores the
‘0’ character if it is not a significant digit, even if ‘0’ is the only
digit in the string. (It does display the ‘0’ character in the output string
if it is a significant digit.)</p>
<p>The number of Zero and/or Digit
placeholders after the decimal separator determines the number of digits that appear after the decimal
separator. Values are rounded as necessary.</p></td>
</tr>
<tr>
<td><code>.</code> (period)</td>
<td><strong>Decimal separator:</strong>
The left most ‘.’
character in the format string determines the location of the
decimal separator in the formatted value; any additional ‘.’ characters are
ignored. The <code>System.Globalization.NumberFormatInfo.NumberDecimalSeparator</code> property determines
the symbol used as the decimal
separator.</td>
</tr>
<tr>
<td><code>,</code> (comma)</td>
<td><p><strong>Group separator and number scaling:</strong>
The ‘,’ character serves two purposes. First,
if the custom format contains this character between two Zero or Digit placeholders (0 or #)
and to the left of the decimal separator if one is present,
then the output will have group separators inserted between each group of digits
to the left of the decimal separator. The <code>System.Globalization.NumberFormatInfo.NumberGroupSeparator</code>
and <code>System.Globalization.NumberFormatInfo.NumberGroupSizes</code>
properties determine the symbol used as the group separator and
the number of digits in each group, respectively.</p>
<p>If
the format
string contains one or more ‘,’ characters immediately to the left of
the decimal separator, then the number will be scaled. The scale factor is
determined by the number of group separator characters immediately to the
left of the decimal separator. If there are x characters, then the value is
divided by 1000<sup>X</sup> before it is formatted. For example, the format string ‘0,,’
will divide a value by one million. Note that the presence of the ‘,’
character to indicate scaling does not insert group separators in the
output string. Thus, to scale a number by 1 million and insert group
separators, use a custom format similar to ‘#,##0,,’.</p></td>
</tr>
<tr>
<td><code>%</code> (percent)</td>
<td><strong>Percentage placeholder:</strong>
The presence of a ‘%’ character
in a custom format causes a number to be multiplied by 100
before it is formatted. The percent symbol is inserted in the output string
at the location where the ‘%’ appears in the format string. The <code>System.Globalization.NumberFormatInfo.PercentSymbol</code> property determines
the percent
symbol.</td>
</tr>
<tr>
<td><p><code>E0</code></p>
<p><code>E+0</code></p>
<p><code>E-0</code></p>
<p><code>e0</code></p>
<p><code>e+0</code></p>
<p><code>e-0</code></p></td>
<td><strong>Engineering format:</strong> If any of the strings ‘E’, ‘E+’, ‘E-’, ‘e’, ‘e+’, or ‘e-’ are present
in a custom format and is followed immediately by at least one ‘0’
character, then the value is formatted using scientific notation. The number
of ‘0’ characters following the exponent prefix (E or e) determines the
minimum number of digits in the exponent. The ‘E+’ and ‘e+’ formats indicate
that a positive or negative number symbol always precedes the
exponent. The ‘E’, ‘E-’, ‘e’, or ‘e-’ formats indicate that a negative number symbol
precedes negative exponents; no symbol is precedes positive exponents. The
positive number symbol is supplied by the <code>System.Globalization.NumberFormatInfo.PositiveSign</code> property. The negative number symbol
is supplied by the <code>System.Globalization.NumberFormatInfo.NegativeSign</code>
property.</td>
</tr>
<tr>
<td><code>\</code> (backslash)</td>
<td><strong>Escape character:</strong> In some languages, such as C#, the
backslash character causes the next character in the custom format to be interpreted
as an escape sequence. It is used with C language
formatting sequences, such as ‘\n’ (newline). In some languages, the escape character
itself is required to be preceded by an escape character
when used as a literal. Otherwise, a compiler interprets the character as
an escape sequence. This escape character is not required to be
supported in all programming languages.</td>
</tr>
<tr>
<td><p><code>'ABC'</code></p>
<p><code>"ABC"</code></p></td>
<td><strong>Literal string:</strong> Characters enclosed in single or double quotes are
copied to the output string literally, and do not affect formatting.</td>
</tr>
<tr>
<td><code>;</code> (semicolon)</td>
<td><strong>Section separator:</strong> The ‘;’ character is used to separate sections for
positive, negative, and zero numbers in the format string. (This feature
is described in detail below.)</td>
</tr>
<tr>
<td>Other</td>
<td><strong>All other characters:</strong> All other characters are stored in the output
string as literals in the position in which they
appear.</td>
</tr>
</table>

Note that for fixed-point format strings (strings not containing an ‘E0’,
‘E+0’, ‘E-0’, ‘e0’, ‘e+0’, or ‘e-0’), numbers are rounded to as many decimal
places as there are Zero or Digit placeholders to the right of the decimal
separator. If the custom format does not contain a decimal separator, the number is
rounded to the nearest integer. If the number has more digits than there are
Zero or Digit placeholders to the left of the decimal separator, the extra
digits are copied to the output string immediately before the first Zero or
Digit placeholder.

A custom format can contain
up to three sections separated by section separator characters, to specify different formatting for
positive, negative, and zero values. The sections are interpreted as follows:

- **One section**: The
custom format applies to all values (positive, negative and zero). Negative
values include a negative sign.

- **Two sections**: The
first section applies to positive values and zeros, and the second section
applies to negative values. If the value to be formatted is negative, but
becomes zero after rounding according to the format in the second section,
then the resulting zero is formatted according to the first section. Negative
values do not include a negative sign to allow full control over
representations of negative values. For example, a negative can be represented
in parenthesis using a custom format similar to ‘####.####;(####.####)’.

- **Three sections**:
The first section applies to positive values, the second section
applies to negative values, and the third section applies to zeros. The
second section can be empty (nothing appears between the semicolons), in which case the
first section applies to all nonzero values, and negative values include a
negative sign. If the number to be formatted is nonzero, but becomes zero
after rounding according to the format in the first or second section, then
the resulting zero is formatted according to the third section.

The `System.Enum` and `System.DateTime` types also support using format specifiers to
format string representations of values. The meaning of a specific format specifier varies
according to the kind of data (numeric, date/time, enumeration) being formatted. See
`System.Enum` and `System.Globalization.DateTimeFormatInfo` for a comprehensive list of
the format specifiers supported by each type.

## C.5 Library Type Abbreviations

The following library types are referenced in this specification. The full names of those types, including the global namespace qualifier are listed below. Throughout this specification, these types appear as either the fully qualified name; with the global namespace qualifier omitted; or as a simple unqualified type name, with the namespace omitted as well. For example, the type `ICollection<T>`, when used in this specification, always means the type `global::System.Collections.Generic.ICollection<T>`.

- `global::System.Action`
- `global::System.ArgumentException`
- `global::System.ArgumentOutOfRangeException`
- `global::System.ArithmeticException`
- `global::System.Array`
- `global::System.ArrayTypeMismatchException`
- `global::System.Attribute`
- `global::System.AttributeTargets`
- `global::System.AttributeUsageAttribute`
- `global::System.Boolean`
- `global::System.Byte`
- `global::System.Char`
- `global::System.Decimal`
- `global::System.Delegate`
- `global::System.DivideByZeroException`
- `global::System.Double`
- `global::System.Enum`
- `global::System.Exception`
- `global::System.FormattableString`
- `global::System.GC`
- `global::System.IAsyncDisposable`
- `global::System.IDisposable`
- `global::System.IEquatable<T>`
- `global::System.IFormattable`
- `global::System.Index`
- `global::System.IndexOutOfRangeException`
- `global::System.Int16`
- `global::System.Int32`
- `global::System.Int64`
- `global::System.IntPtr`
- `global::System.InvalidCastException`
- `global::System.InvalidOperationException`
- `global::System.MemoryExtensions`
- `global::System.NotSupportedException`
- `global::System.Nullable<T>`
- `global::System.NullReferenceException`
- `global::System.Object`
- `global::System.ObsoleteAttribute`
- `global::System.OperationCanceledException`
- `global::System.OutOfMemoryException`
- `global::System.OverflowException`
- `global::System.Range`
- `global::System.ReadOnlySpan`
- `global::System.SByte`
- `global::System.Single`
- `global::System.Span`
- `global::System.StackOverflowException`
- `global::System.String`
- `global::System.SystemException`
- `global::System.Type`
- `global::System.TypeInitializationException`
- `global::System.UInt16`
- `global::System.UInt32`
- `global::System.UInt64`
- `global::System.UIntPtr`
- `global::System.ValueTuple<T1>`
- `global::System.ValueTuple<T1, T2>`
- `global::System.ValueTuple<T1, T2, T3>`
- `global::System.ValueTuple<T1, T2, T3, T4>`
- `global::System.ValueTuple<T1, T2, T3, T4, T5>`
- `global::System.ValueTuple<T1, T2, T3, T4, T5, T6>`
- `global::System.ValueTuple<T1, T2, T3, T4, T5, T6, T7>`
- `global::System.ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>`
- `global::System.ValueType`
- `global::System.Collections.ICollection`
- `global::System.Collections.IEnumerable`
- `global::System.Collections.IEnumerator`
- `global::System.Collections.IList`
- `global::System.Collections.Generic.IAsyncEnumerable<out T>`
- `global::System.Collections.Generic.IAsyncEnumerator<out T>`
- `global::System.Collections.Generic.ICollection<T>`
- `global::System.Collections.Generic.IEnumerable<T>`
- `global::System.Collections.Generic.IEnumerator<T>`
- `global::System.Collections.Generic.IList<T>`
- `global::System.Collections.Generic.IReadOnlyCollection<out T>`
- `global::System.Collections.Generic.IReadOnlyList<out T>`
- `global::System.Diagnostics.ConditionalAttribute`
- `global::System.Diagnostics.CodeAnalysis.AllowNullAttribute`
- `global::System.Diagnostics.CodeAnalysis.DisallowNullAttribute`
- `global::System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute`
- `global::System.Diagnostics.CodeAnalysis.DoesNotReturnIfAttribute`
- `global::System.Diagnostics.CodeAnalysis.MaybeNullAttribute`
- `global::System.Diagnostics.CodeAnalysis.MaybeNullWhenAttribute`
- `global::System.Diagnostics.CodeAnalysis.MemberNotNullAttribute`
- `global::System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute`
- `global::System.Diagnostics.CodeAnalysis.NotNullAttribute`
- `global::System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute`
- `global::System.Diagnostics.CodeAnalysis.NotNullWhenAttribute`
- `global::System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute`
- `global::System.Diagnostics.CodeAnalysis.UnscopedRefAttribute`
- `global::System.Linq.Expressions.Expression<TDelegate>`
- `global::System.Reflection.MemberInfo`
- `global::System.Runtime.CompilerServices.AsyncMethodBuilderAttribute`
- `global::System.Runtime.CompilerServices.CallerArgumentExpressionAttribute`
- `global::System.Runtime.CompilerServices.CallerFileAttribute`
- `global::System.Runtime.CompilerServices.CallerFilePathAttribute`
- `global::System.Runtime.CompilerServices.CallerLineNumberAttribute`
- `global::System.Runtime.CompilerServices.CallerMemberNameAttribute`
- `global::System.Runtime.CompilerServices.CollectionBuilderAttribute`
- `global::System.Runtime.CompilerServices.DefaultInterpolatedStringHandler`
- `global::System.Runtime.CompilerServices.FormattableStringFactory`
- `global::System.Runtime.CompilerServices.ICriticalNotifyCompletion`
- `global::System.Runtime.CompilerServices.IndexerNameAttribute`
- `global::System.Runtime.CompilerServices.InlineArrayAttribute`
- `global::System.Runtime.CompilerServices.INotifyCompletion`
- `global::System.Runtime.CompilerServices.InterpolatedStringHandlerArgumentAttribute`
- `global::System.Runtime.CompilerServices.InterpolatedStringHandlerAttribute`
- `global::System.Runtime.CompilerServices.ITuple`
- `global::System.Runtime.CompilerServices.ModuleInitializerAttribute`
- `global::System.Runtime.CompilerServices.RequiredMemberAttribute`
- `global::System.Runtime.CompilerServices.TaskAwaiter`
- `global::System.Runtime.CompilerServices.TaskAwaiter<TResult>`
- `global::System.Runtime.CompilerServices.ValueTaskAwaiter`
- `global::System.Runtime.CompilerServices.ValueTaskAwaiter<TResult>`
- `global::System.Runtime.CompilerServices.Unsafe`
- `global::System.Runtime.InteropServices.UnmanagedCallersOnlyAttribute`
- `global::System.Threading.Monitor`
- `global::System.Threading.Tasks.Task`
- `global::System.Threading.Tasks.Task<TResult>`

**End of informative text.**
