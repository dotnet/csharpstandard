class Test
{
	static void Main()
	{
        BitArray ba1 = new BitArray(100);
        ba1[0] = true;
        ba1[^98] = true;
        ba1[99] = true;
        Console.WriteLine("ba1[0] = {0}", ba1[0]);                    // True
        Console.WriteLine("ba1[98] = {0}", ba1[98]);                  // False
        Console.WriteLine("ba1[Index 0] = {0}", ba1[new Index(0)]);   // True
        Console.WriteLine("ba1[^1] = {0}", ba1[^1]);                  // True
	}
}

partial class BitArray
{
    int[] bits;
    int length;

    public BitArray(int length)
    {
        if (length < 0)
        {
            throw new ArgumentException();
        }
        else if (length == 0)
        {
            bits = new int[1];
        }
        else
        {
            bits = new int[((length - 1) >> 5) + 1];
        }
        this.length = length;
    }

    public int Length => length;

    public bool this[int index]
    {
        get
        {
            if (index < 0 || index >= length)
            {
                throw new IndexOutOfRangeException();
            }
            return (bits[index >> 5] & 1 << index) != 0;
        }
        set
        {
            if (index < 0 || index >= length)
            {
                throw new IndexOutOfRangeException();
            }
            if (value)
            {
                bits[index >> 5] |= 1 << index;
            }
            else
            {
                bits[index >> 5] &= ~(1 << index);
            }
        }
    }
}
