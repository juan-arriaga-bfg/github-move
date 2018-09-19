public class QuestConditionOtherQuest : IQuestConditionOld
{
	public int OtherUid { get; set; }
	
	public bool Check()
	{
		return GameDataService.Current.QuestsManagerOld.IsCompleted(OtherUid);
	}
}