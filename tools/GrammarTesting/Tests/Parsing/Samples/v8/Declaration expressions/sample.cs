class DeclarationExpressions
{
   void DeclarationExpressions()
   {     
      (int i1, int _, (var i2, var _), _) = (1, 2, (3, 4), 5); // nest in tuples

      (_, w) = (42, 'x');                          // not a declaration expression, in real code `w` would need to exist

      var s3 = M(out int _, "Three", out var _);   // declaration expression in method call
      
      var X = M(A < B, C > D, E);                  // method call, 3 arguments
      var Y = (A < B, C > D, E);                   // similar tuple is 2-tuple – declaration expression & E
      var Y = (A < B, C > D, E, F < G, H > I);     // 3-tuple – decl expr, E, decl expr
   }
}