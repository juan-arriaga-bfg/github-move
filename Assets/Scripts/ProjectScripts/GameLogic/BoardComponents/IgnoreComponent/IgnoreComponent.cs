using System.Collections.Generic;
using NUnit.Framework;

public abstract class IgnoreComponent<TIgnore>:IECSComponent
{
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid
    {
        get { return ComponentGuid; }
    }

    public abstract bool CanIgnore(TIgnore item);
}