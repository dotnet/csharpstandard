namespace Comments.XmlComments.UndocumentedKeywords
{
    class yield
    {
        public partial void method()
        {
            b = !b;/*!*/
            i = ~i;/*~i*/
            b = i < i && i > i;/*< && >*/
            int? ii = 5;/*? bug*/ // NO ?
            int f = true ? 1 : 0;/*? :*/   // YES :
            i++;/*++*/
            i--;/*--*/
            b = true && false || true;/*&& ||*/
            b = i == i && i != i && i <= i && i >= i;/*= == && != <= >=*/
            i += 5.0;/*+=*/
            i -= i;/*-=*/
            i *= i;/**=*/
            i /= i;/*/=*/
            i %= i;/*%=*/
            i &= i;/*&=*/
            i |= i;/*|=*/
            i ^= i;/*^=*/
            i <<= i;/*<<=*/
            i >>= i;/*>>=*/
            object s = x => x + 1;/*=>*/
            double d = .3;
            Point point;
            unsafe
            {
                Point* p = &point;/** &*/
                p->x = 10;/*->*/
            }
            IO::BinaryReader br = null;
            x[i: 1] = 3;
            x[i: 1, j: 5] = "str";
        }

        struct Point { public int X; public int Y; public void ThisAccess() { this = this; } }
    }
}