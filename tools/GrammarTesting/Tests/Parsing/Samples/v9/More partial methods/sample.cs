partial class C
{
    private int value;

    public partial ref int M1();
    public partial ref readonly int M2();
    private static partial void M3();
}

partial class C
{
    public partial ref int M1() => ref value;

    public partial ref readonly int M2()
    {
        return ref value;
    }

    [System.Runtime.InteropServices.DllImport("NativeLibrary")]
    private static extern partial void M3();
}
