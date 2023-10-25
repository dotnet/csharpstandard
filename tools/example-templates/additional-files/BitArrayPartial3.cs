class Test
{
	static void Main()
	{
        BitArray ba = new BitArray(5);
        ba[0] = true;
        ba[3] = true;
        ba[^1] = true;
        Console.WriteLine("ba = >{0}<", ba);

        Range[] testRange = { 
// all elements
            /*0..5,*/ 0.., ..5, //.., 0..^0, ..^0, ^5..5, ^5.., ^5..^0,

// trailing part
//            1..5, 1.., 1..^0, ^4..5, ^4.., ^4..^0, 

// leading part
//            0..4, ..4, ^5..4, ^5..^1,

// middle part:
//            1..4, ^4..4, ^4..^1, 1..^1,
//            2..4, ^3..4, ^3..^1, 2..^1,
            3..4, ^2..4, //^2..^1, 3..^1,

// empty range
            0..0//, 1..1, 2..2, 3..3, 4..4, 5..5
        };

        foreach (Range r in testRange)
        {
            Console.WriteLine($"BitArray is >{ba[r]}<");
        }
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

    public override string ToString()
    {
        string retstr = "";
        int bitsWord;
        int upBound = bits.GetUpperBound(0);
        int bitCounter = Length;

        if (Length == 0)
        {
        	return retstr;
        }

        for (int i = 0; i <= upBound; ++i)
        {
        	bitsWord = bits[i];
        	for (int j = 0; j < 32; ++j)
        	{
        		if (bitCounter-- == 0)
        		{
        			break;
        		}
                retstr += ((bitsWord & 1) == 1) ? "1" : "0";
                bitsWord >>= 1;
        	}
        }

        return retstr;
    }

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