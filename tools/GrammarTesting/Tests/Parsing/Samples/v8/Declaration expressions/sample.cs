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
      (A < B, C > D, E, F < G, H > I) = y;         // syntax valid, semantically invalid:
                                                   //    cannot have both declarations and variable references
                                                   //    in the same deconstructor (will change in C#10)

      t = (1, 2, (3, 4));
      (p, _, (_, q)) = t;                          // deconstructing assignment
      (var p, _, (_, var q)) = t;                  // ditto
      (var p, _, _) = (_, _, (_, q)) = t;          // 2 deconstructing assignments

      (A < B, C > p, _, (_, A < B, C > q)) = t;                // deconstructing declaration w/TAL
      (A < B, C > p, _, _) = (_, _, (_, q)) = t;               // valid

      (A < B, C > p, _, _) = (_, _, (_, A < B, C > q)) = t;    // syntax valid, semantically invalid:
                                                               //    either the declaration A<B,C> q is not allowed here
                                                               //    or A < B and C > q are not a variable references
      (A < B, C > p, _, _) = (_, _, (_, (A) < B, C > q)) = t;  // syntax valid, semantically invalid:
                                                               //    (A) < B and C > q are not a variable references

      (var p, _, _) = (_, A < B, C > q) = x;                   // syntax valid, semantically invalid:
                                                               // A < B & C > q are not variable references

      (var p, _, _) = (_, a < b ? ref q : ref r, _) = x;       // valid, `a < b ? ref q : ref r` is ref-valued

      // abridged_deconstructor cases

      var (p, q, r) = w;
      var (p, (q, r)) = x;
      (p, q) = var (r, s) = y;   // syntax valid, semantically invalid: declaration var (r, s) not allowed here
      (p, var (q, r)) = z;       // syntax valid, semantically invalid:
                                 //    cannot have both declarations and variable references
                                 //    in the same deconstructor (will change in C#10)

       for ((_, var (i, j)) = t;
            (i - j) < 5;
            (_, int k) = (0, i - j), i--, j++)
       {
       }
   }
}
