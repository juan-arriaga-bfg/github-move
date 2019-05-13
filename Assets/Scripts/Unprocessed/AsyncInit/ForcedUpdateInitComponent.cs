public class ForcedUpdateInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        ForcedUpdateManager manager = new ForcedUpdateManager();
        ForcedUpdateService.Instance.SetManager(manager);
        manager.Init();
        
        isCompleted = true;
        OnComplete(this);
    }
}