namespace My
{
    public unsafe partial class A : C, I
    {
        ~A()
        {
        }
        private readonly int f1;
        [Obsolete]
        [NonExisting]
        [Foo::NonExisting(var, 5)]
        [CLSCompliant(false)]
        [Obsolete, System.NonSerialized, NonSerialized, CLSCompliant(true || false & true)]
        private volatile int f2;
        [return: Obsolete]
        [method: Obsolete]
        public void Handler(object value)
        {
        }
        public int m<T>(T t)
          where T : class, new()
        {
            base.m(t);
            return 1;
        }
        public string P
        {
            get
            {
                return "A";
            }
            set;
        }
        public abstract string P
        {
            get;
        }
    }
}