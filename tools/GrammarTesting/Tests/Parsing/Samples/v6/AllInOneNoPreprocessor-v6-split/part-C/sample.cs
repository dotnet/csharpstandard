namespace My
{
    public unsafe partial class A : C, I
    {
        public A([param: Obsolete] int foo) :
            base(1)
        {
            if (i > 0)
            {
                return;
            }
            else if (i == 0)
            {
                throw new Exception();
            }
            var o1 = new MyObject();
            var o2 = new MyObject(var);
            var o3 = new MyObject { A = i };
            var o4 = new MyObject(@dynamic)
            {
                A = 0,
                B = 0,
                C = 0
            };
            var o5 = new { A = 0 };
            var dictionaryInitializer = new Dictionary<int, string> 
            { 
                {1, ""}, 
                {2, "a"} 
            };
            float[] a = new float[] 
            {
                0f,
                1.1f
            };
            int[, ,] cube = { { { 111, 112, }, { 121, 122 } }, { { 211, 212 }, { 221, 222 } } };
            int[][] jagged = { { 111 }, { 121, 122 } };
            int[][,] arr = new int[5][,]; // as opposed to new int[][5,5]
            arr[0] = new int[5,5];  // as opposed to arr[0,0] = new int[5];
            arr[0][0,0] = 47;
            int[] arrayTypeInference = new[] { 0, 1, };
        }
    }
}