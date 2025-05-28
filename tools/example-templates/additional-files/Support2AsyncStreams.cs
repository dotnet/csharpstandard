using System.Collections.Generic;
using System.Threading.Tasks;

partial class Class1
{
    static async Task Main()
    {
        await M();
        await Task.Delay(2000);
    }

    static async IAsyncEnumerable<int> GenerateSequence()
    {
        for (int i = 0; i <= 5; i++)
        {
            await Task.Delay(100);
            yield return i;
        }
    }
}