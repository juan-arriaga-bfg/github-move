using Quests;

public class CheckQuestTutorialCondition : BaseTutorialCondition
{
	public string Target;
	public TaskState TargetState = TaskState.Pending; 
	
	public override bool Check()
	{
		var finishedQuest = GameDataService.Current?.QuestsManager?.FinishedQuests?.Find(id => id == Target);
		var activeQuest = GameDataService.Current?.QuestsManager?.ActiveQuests?.Find(entity => entity.Id == Target);
		
		return finishedQuest != null || activeQuest != null && activeQuest.State >= TargetState;
	}
}