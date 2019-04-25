using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;

public class LoadSilenceComponent: IECSComponent
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
    }

    public virtual void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnLoadComplete()
    {
        IW.Logger.LogError("Complete");
        ProfileService.Current.Settings.SetVolume("Sound", cachedSound);
        context.UnRegisterComponent(this);
    }
}