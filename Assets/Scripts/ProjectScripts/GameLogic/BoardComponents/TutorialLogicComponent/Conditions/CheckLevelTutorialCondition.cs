public class CheckLevelTutorialCondition : BaseTutorialCondition
{
	public int Target;
	
	public override bool Check()
	{
		var level = GameDataService.Current?.LevelsManager?.Level;
		
		return level != null && level >= Target;
	}
}