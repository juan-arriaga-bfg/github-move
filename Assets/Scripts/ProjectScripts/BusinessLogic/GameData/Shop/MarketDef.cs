public enum MarketRandomType
{
    Base,
    BasePiecesEasy,
    BasePiecesHard,
    Ingredients,
    BaseСhests,
}

public class MarketDef
{
    public string Uid;
    public int UnlockLevel;
    public int Amount;
    public MarketRandomType RandomType;
    public CurrencyPair Price;
    public ItemWeight Weight;
}