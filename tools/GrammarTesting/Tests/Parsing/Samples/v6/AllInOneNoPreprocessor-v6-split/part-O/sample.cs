namespace Comments.XmlComments.UndocumentedKeywords
{
    class yield
    {
        void Params(ref dynamic a, out dynamic b, params dynamic[] c) {}
        void Params(out dynamic a = 2, ref dynamic c = default(dynamic), params dynamic[][] c) {}

        public override string ToString() { return base.ToString(); } 

        public partial void OnError();

        public partial void method()
        {
            int?[] a = new int?[5];/*[] bug*/ // YES []
            int[] var = { 1, 2, 3, 4, 5 };/*,;*/
            int i = a[i];/*[]*/
            Foo<T> f = new Foo<int>();/*<> ()*/
            f.method();/*().*/
            i = i + i - i * i / i % i & i | i ^ i;/*+ - * / % & | ^*/
            bool b = true & false | true ^ false;/*& | ^*/
        }
    }
}