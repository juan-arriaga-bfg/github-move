using System.Runtime.Remoting.Contexts;

public class LoadSilenceComponent: IECSComponent, IECSSystem
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private ECSEntity context;
    private float cachedSound;
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        context = entity;

        cachedSound = ProfileService.Current.Settings.GetVolume("Sound");
        ProfileService.Current.Settings.SetVolume("Sound", 0f);
        IW.Logger.LogError("LogLogLog");
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
//        OnLoadComplete();
    }

    public bool IsExecuteable()
    {
        return true;
    }

    public void Execute()
    {
        bool isGameLoaded = AsyncInitService.Current?.IsAllComponentsInited() ?? false;
        if (isGameLoaded)
        {
            context.UnRegisterComponent(this);
        }
    }

    public object GetDependency()
    {
        return null;
    }

    public void OnLoadComplete()
    {
        ProfileService.Current.Settings.SetVolume("Sound", cachedSound);
    }
}