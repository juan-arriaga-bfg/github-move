public enum MarketRandomType
{
    Base,
    BasePiecesEasy,
    BasePiecesNormal,
    BasePiecesHard,
    Ingredients,
    BaseChestsFirst,
    BaseChestsSecond,
    BaseChestsLast,
    NPCChests,
    Worker
}

public enum MarketItemBundle
{
    Base,
    Ingredients,
    Chests,
    Pieces
}

public class MarketDef
{
    public int Uid;
    public int UnlockLevel;
    public int Amount;
    public MarketRandomType RandomType;
    public MarketItemBundle Bundle;
    public CurrencyPair Price;
    public ItemWeight Weight;
    public bool IsPermanent;
    public string Name;
    public string Icon;
}