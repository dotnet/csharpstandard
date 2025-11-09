namespace My
{
    public unsafe partial class A : C, I
    {
        public A([param: Obsolete] int foo) :
            base(1)
        {
            lock (sync)
                process();
            using (var v = BeginScope())
            using (A a = new A())
            using (A a = new A(), b = new A())
            using (BeginScope())
                return;
            yield return this.items[3];
            yield break;
            fixed (int* p = stackalloc int[100], q = &y)
            {
                *intref = 1;
            }
            fixed (int* p = stackalloc int[100])
            {
                *intref = 1;
            }
            unsafe
            {
                int* p = null;
            }
            try
            {
                throw null;
            }
            catch (System.AccessViolationException av)
            {
                throw av;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try { } catch { }
            }
            var anonymous = 
            {
                A = 1,
                B = 2,
                C = 3,
            };
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
            query = from c in customers
                    select c into d
                    select d;
        }
    }
}