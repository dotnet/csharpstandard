using System;

class Program
{
    static void Main()
    {
        F1();

        void F1([CallerMemberName] string? name = null)
        {
            Console.WriteLine($"F1 MemberName: |{name}|");
            F2();
        }

        static void F2([CallerMemberName] string? name = null)
        {
            Console.WriteLine($"F2 MemberName: |{name}|");
        }
    }
}