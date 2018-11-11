using Quests;

public class CheckQuestTutorialCondition : BaseTutorialCondition
{
	public string Target;
	public TaskState TargetState = TaskState.Pending; 
	
	public override bool Check()
	{
		var quest = GameDataService.Current?.QuestsManager?.ActiveQuests?.Find(entity => entity.Id == Target);
		
		return quest != null && quest.State == TargetState;
	}
}