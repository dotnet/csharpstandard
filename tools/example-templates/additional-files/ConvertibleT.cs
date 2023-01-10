struct Convertible<T>
{
    public static implicit operator Convertible<T>(T value) { return default; }
    public static explicit operator T(Convertible<T> value) { return default; }
}
