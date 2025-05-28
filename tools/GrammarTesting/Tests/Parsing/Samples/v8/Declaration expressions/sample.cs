class DeclarationExpressions
{
   void DeclarationExpressions_1()
   {
      (int i1, int _, (var i2, var _), _) = (1, 2, (3, 4), 5); // nest in tuples

      _ = P(x,(y,z));                              // simple discard or simple assignment

      (_, w) = (42, 'x');                          // not a declaration expression, in real code `w` would need to exist

      var s3 = Q(out int _, "Three", out var _);   // declaration expression in method call
   }

   void DeclarationExpressions_2()
   {
      R(A < B, C > D, E) = x;                      // method call, 3 arguments
      var y = (A < B, C > D, E);                   // 3-tuple – no decl expr allowed on RHS
      (A < B, C > D, E) = y;                       // similar tuple is 2-tuple on LHS – declaration expression & E (C#10)

      (A < B, C > D, E, F < G, H > I) = y;         // 3-tuple – decl expr, E, decl expr (C#10)
   }
}