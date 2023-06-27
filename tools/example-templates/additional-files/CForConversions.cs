class C
{
    int i;

    public C(int i)
    {
        this.i = i;
    }

    public static explicit operator C(string s)
    {
        return new C(int.Parse(s));
    }
}
