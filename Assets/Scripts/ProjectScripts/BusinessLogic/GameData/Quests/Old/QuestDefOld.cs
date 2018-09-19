using System.Collections.Generic;

public class QuestDefOld
{
    public int Uid { get; set; }
    public string Message { get; set; }
    public List<CurrencyPair> Rewards { get; set; }
    public CurrencyPair Price { get; set; }
    public List<IQuestConditionOld> Conditions { get; set; }
}