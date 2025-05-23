namespace My
{
    public unsafe partial class A : C, I
    {
        public A([param: Obsolete] int foo) :
            base(1)
        {
            switch (3) { }
            switch (i)
            {
                case 0:
                case 1:
                    {
                        goto case 2;
                    }
                case 2 + 3:
                    {
                        goto default;
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
            while (i < 10)
            {
                ++i;
                if (true) continue;
                break;
            }
            do
            {
                ++i;
                if (true) continue;
                break;
            }
            while (i < 10);
            for (int j = 0; j < 100; ++j)
            {
                for(;;) 
                {
                    for (int i = 0, j = 0; i < length; i++, j++) { }
                    if (true) continue;
                    break;
                }
            }
            label:
            goto label;
            label2: ;
            foreach (var i in Items())
            {
                if (i == 7)
                    return;
                else
                    continue;
            }
        }
    }
}