public class QuestConditionOtherQuest : IQuestCondition
{
	public int OtherUid { get; set; }
	
	public bool Check()
	{
		return GameDataService.Current.QuestsManager.IsCompleted(OtherUid);
	}
}