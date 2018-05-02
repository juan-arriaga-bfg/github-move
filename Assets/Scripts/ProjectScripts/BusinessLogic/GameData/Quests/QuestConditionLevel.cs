public class QuestConditionLevel : IQuestCondition
{
    public int UserLevel { get; set; }
    
    public bool Check()
    {
        return UserLevel <= ProfileService.Current.GetStorageItem(Currency.Level.Name).Amount;
    }
}