public class GameDataInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        // gamedata configs
        
        var profile = ProfileService.Current;
        
        ProfileService.AllowAccess = false;
        
        GameDataManager dataManager = new GameDataManager();
        GameDataService.Instance.SetManager(dataManager);
        
        dataManager.SetupComponents(profile);

        ProfileService.AllowAccess = true;
        
        isCompleted = true;
        OnComplete(this);
    }
}