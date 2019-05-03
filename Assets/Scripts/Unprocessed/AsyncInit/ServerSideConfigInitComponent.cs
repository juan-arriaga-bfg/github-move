public class ServerSideConfigInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        ServerSideConfigsManager manager = new ServerSideConfigsManager();
        ServerSideConfigService.Instance.SetManager(manager);
        
        manager
           .RegisterComponent(new GameEventServerSideConfigLoader()
               .SetUrl("game-event/get"));
        
        manager.UpdateAll();
        
        isCompleted = true;
        OnComplete(this);
    }
}