class IsPattern
{
   void IsPattern()
   {
      // Some of these may have semantic errors, the samples are to
      // show how the syntax should be parsed ignoring semantics
      // (for those with semantic errors a compiler may produce the
      // same parse and then issue semantic errors)
      // Try the samples in your favourite compiler…

      bool A = false;
      const bool B = true;
      const bool C = true;
      int X = 0;
      const int Y = 42;
      const int Z = 24;

      const bool D = Y is int;

      bool N = A is B is C;            // (A is B) is C
      bool O = A is (Y is int);

      bool P = X is Y >> 4;            // X is (Y >> 4)
      bool Q = (X is Y) >> 4;

      bool R = A is B <= C;            // (A is B) <= C
      bool S = A is (Y <= 4);

      bool T = A is B == C;            // (A is B) == C
      bool U = A is (Y == Z);

      bool V = A is B || C;            // (A is B) || C;
      bool W = A is (B || C);

      // T => is type, P => is pattern
      int? Yn = Y;
      int? Zn = null;
      bool J = Yn is int;             // T: true
      bool K = Zn is int;             // T: false
      bool Jn = Yn is int?;           // T: true
      bool Kn = Zn is int?;           // T: false
      bool Ly = Yn is 7 * 6;          // P: true
      bool Lz = Zn is 7 * 6;          // P: false
      bool M = X is int?;             // T: true
      bool Mn = Zn is null;           // P: true

      _ = A switch
      {
         (a => b) => c,                // (a => b) is invalid semantically but syntax
                                       // allows it as constant_expression includes
                                       // '(' expression ')'
         a => b => c,                  // a => (b => c) – returning lambda OK
         a => (b => c)                 // ditto
      };

      static void f(Func<int, int> g)
      {
          var I = g switch
          {
              null => 0,               // can switch on a function value
              var k => k(42),          // but only null and any patterns valid
              (w => x) => g(42)        // as above syntactically OK, semantically invalid
          };
      };
   }
}