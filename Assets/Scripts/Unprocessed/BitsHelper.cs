public static class BitsHelper
{
    public static bool IsBitSet(int b, int pos)
    {
        return (b & (1 << pos)) != 0;
    }

    public static int CountOfSetBits(int i)
    {
        i = i - ((i >> 1) & 0x55555555);
        i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
        return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
    }

    public static int SetBit(int value, int pos)
    {
        return value | 1 << pos;
    }
    
    public static int ResetBit(int value, int pos)
    {
        return value & ~(1 << pos);
    }

    public static int ToggleBit(int value, int pos, bool enabled)
    {
        if (enabled)
        {
            return SetBit(value, pos);
        }

        return ResetBit(value, pos);
    }
}