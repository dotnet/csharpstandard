using System;
using System.Runtime.CompilerServices;
#nullable enable
class Test
{
    public static void M(int val = 0, [CallerArgumentExpression("val")] string? text = null)
    {
        Console.WriteLine($"val = {val}, text = <{text}>");
    }
}
