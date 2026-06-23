using System;

class Program
{
    const bool B = true;

    public static void Main(string[] args)
    {
        object a = null;
        B c = null;
        Console.WriteLine(a is B & c); // prints 5 (correct)
        Console.WriteLine(a is B > c); // prints 6 (correct)
        Console.WriteLine(a is B < c); // syntax error but should print 7
        Console.WriteLine(a is B + c); // should be syntax error but prints 8
    }
}

struct S<T> { }

class B
{
    public static int operator &(bool left, B right) => 5;
    public static int operator >(bool left, B right) => 6;
    public static int operator <(bool left, B right) => 7;
    public static int operator +(bool left, B right) => 8;
}