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
      (A < B, C > D, _) = y;                       // similar tuple is 2-tuple on LHS – declaration expression & _

      (A < B, C > D, _, F < G, H > I) = y;         // 3-tuple – decl expr, _, decl expr

      t = (1, 2, (3, 4));
      (p, _, (_, q)) = t;                          // deconstructing assignment
      (var p, _, (_, var q)) = t;                  // deconstructing declaration
      (var p, _, _) = (_, _, (_, q)) = t;          // both

      (A < B, C > p, _, (_, A < B, C > q)) = t;                // deconstructing declaration w/TAL
      (A < B, C > p, _, _) = (_, _, (_, q)) = t;               // valid

      (A < B, C > p, _, _) = (_, _, (_, A < B, C > q)) = t;    // syntax valid, semantically invalid:
                                                               // A < B & C > q are not a variable references

      (var p, _, _) = (_, A < B, C > q) = x;                   // syntax valid, semantically invalid:
                                                               // A < B & C > q are not variable references

      (var p, _, _) = (_, a < b ? ref q : ref r, _) = x;       // valid, `a < b ? ref q : ref r` is ref-valued

   }
}
