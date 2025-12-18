interface KAI
{
   // constant
   public const int Constant = 100;

   // field
   private static int field;
   
   // methods
   int M1();
   protected new int M2() { return 42; }
   protected new int M3() => 24;

   // properties
   int P1 { get; }
   public new int P2 { get { return 10; } }
   
   // event
   event EventHandler E1;
   sealed event EventHandler E2 { add { } remove { } }
   
   // indexer
   bool this[int ix1] { set; }
   internal bool this[int ix2] { get { return true; } }
   internal bool this[int ix3] => false;
   
   // static constructor
   static KAI() { field = 50; }
   
   // operator
   static KAI operator++(KAI arg1);
   public static KAI operator--(KAI arg1) { return arg1; }
   public static KAI operator--(KAI arg1) => arg1;
   
   // type
   interface Nested
   {
      // constant
      public const int FortyTwo = 42;
   }
}