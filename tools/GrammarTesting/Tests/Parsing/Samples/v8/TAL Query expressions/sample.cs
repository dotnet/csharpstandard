class TypeArgumentListChecks
{
   void TypeArgumentListChecks()
   {
      /* While a *type_argument_list* occurring within a *query_expression* before a query
         contextual keyword is syntactically supported we do not have a sample which uses
         it and makes semantic sense (the above samples all at least make vague sense),
         so instead we take the semantically valid:

            var query = from c in customers
                        let d = c
                        where d != null
                        join c1 in customers on c1.GetHashCode() equals c.GetHashCode()
                        join c1 in customers on c1.GetHashCode() equals c.GetHashCode() into e
                        group c by c.Country
                            into g
                            orderby g.Count() ascending
                            orderby g.Key descending
                            select new { Country = g.Key, CustCount = g.Count() };

          and liberally sprinkle *type_argument_list*s where syntactically allowed and
          use that as a test…
       */

      var query = from c in customers<A>
                  let d = c<B>
                  where d != null && d<C,D>
                  join c1 in customers<E> on c1.GetHashCode() && c1<G> equals c.GetHashCode() && c<H>
                  join c1 in customers on c1.GetHashCode() equals c.GetHashCode() into e
                  group c<K> by c.Country<M>
                      into g
                      orderby g.Count().O<P> ascending
                      orderby g.Key descending
                      select new { Country = g.Key, CustCount = g.Count() };

   }
}
