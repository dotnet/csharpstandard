namespace ConsoleApplication1
{
    class Test
    {
        public int foo = 5;
        void Bar2()
        {
            foo = 6;
            this.Foo = 5.GetType(); Test t = "sss";
        }

        public event EventHandler MyEvent = delegate { };

        void Blah()
        {
            int i = 5;
            int? j = 6;

            Expression<Func<int>> e = () => i;
            Expression<Func<bool, Action>> e2 = b => () => { return; };
            Func<bool, bool> f = async delegate (bool a)
            {
                return await !a;
            };
            Func<int, int, int> f2 = (a, b) => 0;
            f2 = (int a, int b) => 1;
            Action a = Blah;
            f2 = () => {};
            f2 = () => {;};
        }

        delegate Recursive Recursive(Recursive r);
        delegate Recursive Recursive<A,R>(Recursive<A,R> r);
    }
}