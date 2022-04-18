# Annex C Standard library

## C.1 General

A conforming C# implementation shall provide a minimum set of types having specific semantics. These types and their members are listed here, in alphabetical order by namespace and type. For a formal definition of these types and their members, refer to ISO/IEC 23271:2012 *Common Language Infrastructure (CLI), Partition IV; Base Class Library (BCL), Extended Numerics Library, and Extended Array Library*, which are included by reference in this specification.

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

```csharp
namespace System
{
    public delegate void Action();

    public class ArgumentException : SystemException
    {
        public ArgumentException();
        public ArgumentException(string message);
        public ArgumentException(string message, Exception innerException);
    }

    public class ArithmeticException : Exception
    {
        public ArithmeticException();
        public ArithmeticException(string message);
        public ArithmeticException(string message, Exception innerException);
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
        public ArrayTypeMismatchException(string message);
        public ArrayTypeMismatchException(string message,
            Exception innerException);
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

    public struct Boolean { }
    public struct Byte { }
    public struct Char { }
    public struct Decimal { }
    public abstract class Delegate { }

    public class DivideByZeroException : ArithmeticException
    {
        public DivideByZeroException();
        public DivideByZeroException(string message);
        public DivideByZeroException(string message, Exception innerException);
    }

    public struct Double { }

    public abstract class Enum : ValueType
    {
        protected Enum();
    }

    public class Exception
    {
        public Exception();
        public Exception(string message);
        public Exception(string message, Exception innerException);
        public sealed Exception InnerException { get; }
        public virtual string Message { get; }
    }

    public class GC { }

    public interface IDisposable
    {
        void Dispose();
    }

    public interface IFormattable { }

    public sealed class IndexOutOfRangeException : Exception
    {
        public IndexOutOfRangeException();
        public IndexOutOfRangeException(string message);
        public IndexOutOfRangeException(string message,
            Exception innerException);
    }

    public struct Int16 { }
    public struct Int32 { }
    public struct Int64 { }
    public struct IntPtr { }

    public class InvalidCastException : Exception
    {
        public InvalidCastException();
        public InvalidCastException(string message);
        public InvalidCastException(string message, Exception innerException);
    }

    public class InvalidOperationException : Exception
    {
        public InvalidOperationException();
        public InvalidOperationException(string message);
        public InvalidOperationException(string message,
            Exception innerException);
    }

    public class NotSupportedException : Exception
    {
        public NotSupportedException();
        public NotSupportedException(string message);
        public NotSupportedException(string message, Exception innerException);
    }

    public struct Nullable<T>
    {
        public bool HasValue { get; }
        public T Value { get; }
    }

    public class NullReferenceException : Exception
    {
        public NullReferenceException();
        public NullReferenceException(string message);
        public NullReferenceException(string message, Exception innerException);
    }

    public class Object
    {
        public Object();
        ~Object();
        public virtual bool Equals(object obj);
        public virtual int GetHashCode();
        public Type GetType();
        public virtual string ToString();
    }

    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Struct |
        AttributeTargets.Enum | AttributeTargets.Interface |
        AttributeTargets.Constructor | AttributeTargets.Method |
        AttributeTargets.Property | AttributeTargets.Field |
        AttributeTargets.Event | AttributeTargets.Delegate, Inherited = false)]
    public sealed class ObsoleteAttribute : Attribute
    {
        public ObsoleteAttribute();
        public ObsoleteAttribute(string message);
        public ObsoleteAttribute(string message, bool error);
        public bool IsError { get; }
        public string Message { get; }
    }

    public class OutOfMemoryException : Exception
    {
        public OutOfMemoryException();
        public OutOfMemoryException(string message);
        public OutOfMemoryException(string message, Exception innerException);
    }

    public class OverflowException : ArithmeticException
    {
        public OverflowException();
        public OverflowException(string message);
        public OverflowException(string message, Exception innerException);
    }

    public struct SByte { }
    public struct Single { }

    public sealed class StackOverflowException : Exception
    {
        public StackOverflowException();
        public StackOverflowException(string message);
        public StackOverflowException(string message, Exception innerException);
    }

    public sealed class String : IEnumerable<Char>, IEnumerable
    {
        public int Length { get; }
        public char this [int index] { get; }
        public static string Format(string format, params object[] args);
    }

    public abstract class Type : MemberInfo { }

    public sealed class TypeInitializationException : Exception
    {
        public TypeInitializationException(string fullTypeName,
            Exception innerException);
    }

    public struct UInt16 { }
    public struct UInt32 { }
    public struct UInt64 { }
    public struct UIntPtr { }

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
        object this [int index] { get; set; }
        int Add(object value);
        void Clear();
        bool Contains(object value);
        int IndexOf(object value);
        void Insert(int index, object value);
        void Remove(object value);
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

    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        int Count { get; }
    }

    public interface IReadOnlyList<out T> : IReadOnlyCollection<T>
    {
        T this [int index] { get; }
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
        public IndexerNameAttribute(String indexerName);
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

The following types, including the members listed, must be defined in a conforming standard library. (These types might be defined in a future edition of ISO/IEC 23271.) It is expected that many of these types will have more members available than are listed.

A conforming implementation may provide `Task.GetAwaiter()` and `Task<T>.GetAwaiter()` as extension methods.

```csharp
namespace System
{
    public class FormattableString : IFormattable { }
}

namespace System.Linq.Expressions
{
    public sealed class Expression<TDelegate>
    {
        public TDelegate Compile();
    }
}
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerFilePathAttribute : Attribute
    {
        public CallerFilePathAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerLineNumberAttribute : Attribute
    {
        public CallerLineNumberAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerMemberNameAttribute : Attribute
    {
        public CallerMemberNameAttribute() { }
    }

    public static class FormattableStringFactory
    {
        public static FormattableString Create(string format,
            params object[] arguments);
    }

    public interface ICriticalNotifyCompletion : INotifyCompletion
    {
        void UnsafeOnCompleted(Action continuation);
    }

    public interface INotifyCompletion
    {
        void OnCompleted(Action continuation);
    }

    public struct TaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
    {
        public bool IsCompleted { get; }
        public void GetResult();
    }

    public struct TaskAwaiter<T> : ICriticalNotifyCompletion, INotifyCompletion
    {
        public bool IsCompleted { get; }
        public T GetResult();
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
        public new System.Runtime.CompilerServices.TaskAwaiter<T> GetAwaiter();
    }
}
```

## C.4 Format Specifications

The meaning of the formats, as used in interpolated string expressions ([§11.7.3](expressions.md#1173-interpolated-string-expressions)), are defined in ISO/IEC 23271:2012. For convenience the following text is copied from the description of `System.IFormatable`.

**This text is informative.**

A *format* is a string that describes the appearance of an object when
it is converted to a string. Either standard or custom formats can be used. A
standard format takes the form *Axx*, where *A* is a single
alphabetic character called the *format specifier*, and *xx* is an integer between zero and 99 inclusive, called the *precision specifier*. The format specifier controls the type
of formatting applied to the value being represented as a string. The
*precision specifier* controls the number of significant digits or decimal places in the string, if applicable.

> *Note:* For the list of standard format specifiers, see the table below. Note that a given data type, such as `System.Int32`, might not support one or more of the standard format specifiers. *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note:* When a format includes symbols that vary by culture, such as the currencysymbol included by the ‘C’ and ‘c’ formats, a formatting object supplies the actual characters used in the string representation. A method might include a parameter to pass a `System.IFormatProvider` object that supplies a formatting object, or the method might use the default formatting object, which contains the symbol definitions for the current culture. The current culture typically uses the same set of symbols used system-wide by default. In the Base Class Library, the formatting object for system-supplied numeric types is a `System.Globalization.NumberFormatInfo` instance. For `System.DateTime` instances, a `System.Globalization.DateTimeFormatInfo` is used. *end note*

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
when used as a literal. Otherwise, the compiler interprets the character as
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
- `global::System.ArithmeticException`
- `global::System.Array`
- `global::System.ArrayTypeMisMatchException`
- `global::System.Attribute`
- `global::System.AttributeTargets`
- `global::System.AttributeUsageAttribute`
- `global::System.Boolean`
- `global::System.Byte`
- `global::System.Char`
- `global::System.Collections.Generic.ICollection<T>`
- `global::System.Collections.Generic.IEnumerable<T>`
- `global::System.Collections.Generic.IEnumerator<T>`
- `global::System.Collections.Generic.IList<T>`
- `global::System.Collections.Generic.IReadonlyCollection<out T>`
- `global::System.Collections.Generic.IReadOnlyList<out T>`
- `global::System.Collections.ICollection`
- `global::System.Collections.IEnumerable`
- `global::System.Collections.IList`
- `global::System.Collections.IEnumerator`
- `global::System.Decimal`
- `global::System.Delegate`
- `global::System.Diagnostics.ConditionalAttribute`
- `global::System.DivideByZeroException`
- `global::System.Double`
- `global::System.Enum`
- `global::System.Exception`
- `global::System.GC`
- `global::System.ICollection`
- `global::System.IDisposable`
- `global::System.IEnumerable`
- `global::System.IEnumerable<out T>`
- `global::System.IList`
- `global::System.IndexOutOfRangeException`
- `global::System.Int16`
- `global::System.Int32`
- `global::System.Int64`
- `global::System.IntPtr`
- `global::System.InvalidCastException`
- `global::System.InvalidOperationException`
- `global::System.Linq.Expressions.Expression<TDelegate>`
- `global::System.MemberInfo`
- `global::System.NotSupportedException`
- `global::System.Nullable<T>`
- `global::System.NullReferenceException`
- `global::System.Object`
- `global::System.ObsoleteAttribute`
- `global::System.OutOfMemoryException`
- `global::System.OverflowException`
- `global::System.Runtime.CompilerServices.CallerFileAttribute`
- `global::System.Runtime.CompilerServices.CallerLineNumberAttribute`
- `global::System.Runtime.CompilerServices.CallerMemberNameAttribute`
- `global::System.Runtime.CompilerServices.ICriticalNotifyCompletion`
- `global::System.Runtime.CompilerServices.IndexerNameAttribute`
- `global::System.Runtime.CompilerServices.INotifyCompletion`
- `global::System.Runtime.CompilerServices.TaskAwaiter`
- `global::System.Runtime.CompilerServices.TaskAwaiter<T>`
- `global::System.SByte`
- `global::System.Single`
- `global::System.StackOverflowException`
- `global::System.String`
- `global::System.SystemException`
- `global::System.Threading.Monitor`
- `global::System.Threading.Tasks.Task`
- `global::System.Threading.Tasks.Task<TResult>`
- `global::System.Type`
- `global::System.TypeInializationException`
- `global::System.UInt16`
- `global::System.UInt32`
- `global::System.UInt64`
- `global::System.UIntPtr`
- `global::System.ValueType`

**End of informative text.**
