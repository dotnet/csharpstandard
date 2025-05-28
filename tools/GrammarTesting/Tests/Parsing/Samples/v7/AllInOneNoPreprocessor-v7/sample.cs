class CSharp70
{
    void PatternMatching(string arg, int b)
    {
        switch (arg)
        {
            case "A" when b > 50:
            case "B" when b < 50:
            default:
                break;
        }

        (A<B,C> D, E<F,G> H) = e;

        if (x?.y?.z is Type value2)
        {
            // code using value2
        }

        if (expr is Type v) { Hello(); }
    }

	public static async Task LocalFunctions(string[] args)
	{
		string Hello2(int i)
        {
            return args[i];
        }

		async Task<string> Hello<T>(T i) => await Task.FromResult(args[i]);
		await Hello(1);
	}

	public static void OutVar(string[] args)
	{
		int.TryParse(Hello(1), out var item);
		int.TryParse(Hello(1), out int item);
	}

    public void ThrowExpression()
    {
        var result = nullableResult ?? throw new NullReferenceException();
    }

    public void BinaryLiterals()
    {
        int nineteen = 0b10011;
    }

    public void DigitSeparators()
    {
        int bin = 0b1001_1010_0001_0100;
        int hex = 0x1b_a0_44_fe;
        int dec = 33_554_432;
        int weird = 1_2__3___4____5_____6______7_______8________9;
        double real = 1_000.111_1e-1_000;
    }
}

class CSharp71
{
    void DefaultWithoutTypeName(string content = default)
    {
        DefaultWithoutTypeName(default);
    }

    void TupleRecognize(int a, (int, int) b, (int, int, int)? c)
    {
        var result = list.Select(c => (c.f1, f3: c.f2)).Where(t => t.f2 == 1);
    }
}

class CSharp72
{
    readonly struct ReadonlyRef1
    {    
        Func<int, int> s = (in int x) => x;
        ref TValue this[in TKey index] => ref null;
        public static Vector3 operator+(in Vector3 x, in Vector3 y) => null;

        static ref readonly Vector3 M1_Trace()
        {
            // OK
            ref readonly var r1 = ref M1();

            // Not valid. Need an LValue
            ref readonly Vector3 r2 = ref default(Vector3);

            // Not valid. r1 is readonly.
            Mutate(ref r1);

            // OK.
            Print(in r1);

            // OK.
            return ref r1;
        }
    }

    ref struct ReadonlyRef2
    {
        ref readonly Guid Test(in Vector3 v1, in Vector3 v2)
        {
            // not OK!!
            v1 = default(Vector3);

            // not OK!!
            v1.X = 0;

            // not OK!!
            foo(ref v1.X);

            return ref (arr != null ? ref arr[0]: ref otherArr[0]);

            Span<int> span = stackalloc int[1];

            // OK
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        ref T Choice(bool condition, ref T consequence, ref T alternative)
        {
            if (condition)
            {
                 return ref consequence;
            }
            else
            {
                 return ref alternative;
            }
        }
    }

    public void DoSomething(bool isEmployed, string personName, int personAge) { }

    public void NonTrailingNamedArguments()
    {
        DoSomething(isEmployed:true, name, age); // currently CS1738, but would become legal
        DoSomething(true, personName:name, age); // currently CS1738, but would become legal
        DoSomething(name, isEmployed:true, age); // remains illegal
        DoSomething(name, age, isEmployed:true); // remains illegal
        DoSomething(true, personAge:age, personName:name); // already legal
    }

    public void ConditionalRef()
    {
        ref var r = ref (arr != null ? ref arr[0]: ref otherArr[0]);
    }

    public void LeadingSeparator()
    {
        var res = 0
        + 123      // permitted in C# 1.0 and later
        + 1_2_3    // permitted in C# 7.0 and later
        + 0x1_2_3  // permitted in C# 7.0 and later
        + 0b101    // binary literals added in C# 7.0
        + 0b1_0_1  // permitted in C# 7.0 and later

        // in C# 7.2, _ is permitted after the `0x` or `0b`
        + 0x_1_2   // permitted in C# 7.2 and later
        + 0b_1_0_1 // permitted in C# 7.2 and later
        ;
    }
}

class CSharp73
{
    void Blittable<T>(T value) where T : unmanaged
    {
        var unmanaged = 666;
    }

    unsafe struct IndexingMovableFixed
    {
        public fixed int myFixedField[10];
    }

    static IndexingMovableFixed s;

    public unsafe void IndexingMovableFixedFields()
    {
        int* ptr = s.myFixedField;
        int t = s.myFixedField[5];
    }

    public void PatternBasedFixed()
    {
        fixed(byte* ptr = byteArray)
        {
           // ptr is a native pointer to the first element of the array
           // byteArray is protected from being moved/collected by the GC for the duration of this block 
        }
    }

    public void StackallocArrayInitializer()
    {
        Span<int> a = stackalloc int[3];               // currently allowed
        Span<int> a = stackalloc int[3] { 1, 2, 3 };
        Span<int> a = stackalloc int[] { 1, 2, 3 };
        Span<int> a = stackalloc[] { 1, 2, 3 };
    }

    public void TupleEquality()
    {
        (int, (int, int)) t1, t2;
        var res = t1 == (1, (2, 3));
    }
}
