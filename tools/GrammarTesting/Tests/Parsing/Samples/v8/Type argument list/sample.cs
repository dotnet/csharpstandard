class TypeArgumentListChecks
{
   void TypeArgumentListChecks()
   {
      F(G<A, B>(7));       // a call to F with one argument, which is a call to a generic
                           // method G with two type arguments and one regular argument
      F(base.G<A, B>(7));  // ditto

      F(G<A, B>7);         // a call to F with two arguments
      F(base.G<A, B>7);    // ditto
      F(G<A, B>>7);        // ditto

      x = F<A> + y;        // a less-than operator, greater-than operator and unary-plus
                           // operator, as if the statement had been written:
                           //    x = (F < A) > (+y)

      x = y is C<T> && z;           // a namespace_or_type_name with a type_argument_list

      var v = (A < B, C);

      var v = (A < B, C > D);       // a tuple with two elements, each a comparison.

      var v = (A<B,C> D, E);        // tuple with two elements, the first of which is
                                    // a declaration expression.

      var v = M(A < B, C > D, E);   // call with three arguments.

      var v = M(out A<B,C> D, E);   // call with two arguments, the first of which is
                                    // an out declaration.

      if (e is A<B> C)              // a declaration pattern.
         W(C);

      if (e is A<B>)                // a type
         W(C);

      switch (x)
      {
         case A<B> C:               // a declaration pattern.
            var z = C;
            break;
      }

      var p = x is A<B>>C;          // (x is A<B>) > C
      var q = A<B>>C;               // A < (B >> C)
      var r = (x is A<B>)>C;        // (x is A<B>) > C
      var s = x is A<B> orderby;    // declaration pattern defining `orderby`
   }
}