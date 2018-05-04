public class CurrencyPair
{
    public string Currency;
    public int Amount;

    public override string ToString()
    {
        return string.Format("{0}: {1}", Currency, Amount);
    }

    public string ToStringIcon()
    {
        return string.Format("<sprite name={0}> {1}", Currency, Amount);
    }
}