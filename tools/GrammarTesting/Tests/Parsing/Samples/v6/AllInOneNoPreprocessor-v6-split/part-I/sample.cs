namespace My
{
    [type: Flags]
    public enum E
    {
        A,
        B = A,
        C = 2 + A,
        D,
    }
    
    public delegate void Delegate(object P);
    namespace Test
    {
        using System;
        using System.Collections;
        public class Список
        {
            public static IEnumerable Power(int number, int exponent)
            {
                Список Список = new Список();
                Список.Main();
                int counter = (0 + 0);
                int אתר = 0;
                while (++counter++ < --exponent--)
                {
                    result = result * number + +number+++++number;
                    yield return result;
                }
            }
            static void Main()
            {
                foreach (int i in Power(2, 8))
                {
                    Console.Write("{0} ", i);
                }
            }
            async void Wait()
            {
                await System.Threading.Tasks.Task.Delay(0);
            }
            void AsyncAnonymous() // C # 5 feature
            {
                var task = Task.Factory.StartNew(async () =>
                {
                    return await new WebClient().DownloadStringTaskAsync("http://example.com");
                });
            }
        }
    }
}