public static class Extensions
{
    public static int ToInt32(this string s) => Int32.Parse(s);

    public static T[] Slice<T>(this T[] source, int index, int count)
    {
        if (index < 0 || count < 0 || source.Length - index < count)
        {
            throw new ArgumentException();
        }
        T[] result = new T[count];
        Array.Copy(source, index, result, 0, count);
        return result;
    }
}