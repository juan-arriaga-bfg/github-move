﻿public enum MarketRandomType
{
    Base,
    BasePiecesEasy,
    BasePiecesHard,
    Ingredients,
    BaseChestsFirst,
    BaseChestsSecond,
    BaseChestsLast,
    NPCChests,
}

public class MarketDef
{
    public int Uid;
    public int UnlockLevel;
    public int Amount;
    public MarketRandomType RandomType;
    public CurrencyPair Price;
    public ItemWeight Weight;
    public bool IsPermanent;
    public string Name;
    public string Icon;
}