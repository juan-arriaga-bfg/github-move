public class GameDataInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        // gamedata configs
        GameDataManager dataManager = new GameDataManager();
        GameDataService.Instance.SetManager(dataManager);
        
        dataManager.SetupComponents();

        isCompleted = true;
        OnComplete(this);
    }
}