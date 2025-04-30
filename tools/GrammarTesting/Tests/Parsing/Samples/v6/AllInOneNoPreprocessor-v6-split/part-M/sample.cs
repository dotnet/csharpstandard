namespace ConsoleApplication1
{
    class Test
    {
        public Type Foo
        {
            [Obsolete("Name", error = false)]
            get
            {
                var result = typeof(IEnumerable<int>);
                var t = typeof(int?) == typeof(Nullable<int>);
                t = typeof(IEnumerable<int?[][][]>);
                return typeof(IEnumerable<>);
            }
            set
            {
                var t = typeof(System.Int32);
                t.ToString();
                t = value;
            }
        }

        public void Constants()
        {
            int i = 1 + 2 + 3 + 5;
            global::System.String s = "a" + (System.String)"a" + "a" + "a" + "a" + "A";
        }

        public void ConstructedType()
        {
            List<int> i = null;
            int c = i.Count;
        }
    }
}