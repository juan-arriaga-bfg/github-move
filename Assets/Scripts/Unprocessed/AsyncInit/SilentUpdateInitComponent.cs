public class SilentUpdateInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        SilentUpdateManager manager = new SilentUpdateManager();
        SilentUpdateService.Instance.SetManager(manager);
        manager.Init();
        
        isCompleted = true;
        OnComplete(this);
    }
}