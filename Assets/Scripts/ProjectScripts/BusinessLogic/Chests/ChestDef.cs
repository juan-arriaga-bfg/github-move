public class ChestDef 
{
    public string Uid { get; set; }
    public int Time { get; set; }
    
    public CurrencyPair Price { get; set; }
    public CurrencyPair Reward { get; set; }

    public override string ToString()
    {
        return string.Format("Uid {0}, Time {1}, Price: {2} - {3}, Reward: {4} - {5}", Uid, Time, Price.Currency, Price.Amount,
            Reward.Currency, Reward.Amount);
    }
}