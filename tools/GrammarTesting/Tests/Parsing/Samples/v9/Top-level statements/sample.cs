using System;
using System.Runtime.CompilerServices;

Console.WriteLine($"{GetName()}: args[0] = {args[0]}");
Program.Main(args);

string GetName([CallerMemberName] string memberName = "") { return memberName; }

partial class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine($"Main: args[0] = {args[0]}");
    }
}