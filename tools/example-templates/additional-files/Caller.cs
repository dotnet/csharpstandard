delegate void D();

partial class Class1
{
   static void Main()
   {
       foreach (D d in F())
       {
           d();
       }
   }
}
