class MyBitArray
{
    int[] bits;
    int length;

    public MyBitArray(int length)
    {
        if (length < 0)
        {
            throw new ArgumentException();
        }
        bits = new int[((length - 1) >> 5) + 1];
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
