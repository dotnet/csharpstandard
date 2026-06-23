class Sample
{
    public async Task Test()
    {
        (int, int)[] pairs = { (1, 2), (2, 3), (3, 4) };
        Func<int, int>[] funs = new Func<int, int>[pairs.Length];

        foreach ((int a, int b) in pairs) ;

        foreach ((int _, int b) in pairs);

        foreach ((_, _) in pairs);

        foreach (var _ in pairs);

        foreach ((int a, var _) in pairs);

        await foreach ((int a, int b) in GetNumbersAsync());

    }
    async IAsyncEnumerable<(int, int)> GetNumbersAsync()
    {
        for (int i = 0; i < 5; i++)
        {
            await Task.Delay(100); // Simulate asynchronous work
            yield return (i, i * i);
        }
    }
}