using System.Collections.Generic;

public class QuestDef
{
    public int Uid { get; set; }
    public string Message { get; set; }
    public List<CurrencyPair> Rewards { get; set; }
    public CurrencyPair Price { get; set; }
    public List<IQuestCondition> Conditions { get; set; }
}