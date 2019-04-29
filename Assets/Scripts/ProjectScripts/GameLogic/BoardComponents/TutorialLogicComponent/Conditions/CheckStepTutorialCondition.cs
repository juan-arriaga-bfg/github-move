public class CheckStepTutorialCondition : BaseTutorialCondition
{
    public int Target;
    
    public override bool Check()
    {
        var tutorialDataManager = GameDataService.Current.TutorialDataManager;
        return tutorialDataManager.IsCompeted(Target);
    }
}