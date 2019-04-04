using UnityEngine;

public class AmountRange
{
    public int Min;
    public int Max;

    private int value = -1;

    public int Value
    {
        get
        {
            if (value == -1) Range();
            
            return value;
        }
    }

    public int Range()
    {
        value = Random.Range(Min, Max + 1);
        return value;
    }

    public bool IsActive => Min >= 0 && Max > 0;
}