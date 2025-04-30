namespace My
{
    public struct S : I
    {
        public S()
        {
        }
        private int f1;
        [Obsolete("Use Script instead", error: false)]
        private volatile int f2;
        public abstract int m<T>(T t)
          where T : struct
        {
            return 1;
        }
        public string P
        {
            get
            {
                int value = 0;
                return "A";
            }
            set;
        }
        public abstract string P
        {
            get;
        }
        public abstract int this[int index]
        {
            get;
            internal protected set;
        }
        public event Event E;
        public static A operator +(A first, A second)
        {
            return first.Add(second);
        }
        fixed int field[10];
        class C
        {
        }
    }
    public interface I
    {
        void A(int value);
        string Value
        {
            get;
            set;
        }
    }
}