partial class Class1
{
    static async Task Main()
    {
        await M();
        await Task.Delay(2000);
    }

    static async Task M()
    {
        await foreach (var number in GenerateSequence())
        {
            Console.WriteLine(number);
        }
    }
}