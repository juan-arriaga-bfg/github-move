public class ServerSideConfigInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        ServerSideConfigsManager manager = new ServerSideConfigsManager();
        ServerSideConfigService.Instance.SetManager(manager);

        manager.RegisterComponent(new GameEventServerSideConfigLoader()
            .SetUrl("game-event/get"));
            
        manager.RegisterComponent(new ForcedUpdateServerSideConfigLoader()
            .SetUrl("forced-update/get")
            .SetCacheMode(ServerSideConfigLoaderCacheMode.Fallback));
            
        manager.RegisterComponent(new SilentUpdateServerSideConfigLoader()
            .SetUrl("client-update/get")
            .SetCacheMode(ServerSideConfigLoaderCacheMode.Cache));

        manager.UpdateAll();
        
        isCompleted = true;
        OnComplete(this);
    }
}