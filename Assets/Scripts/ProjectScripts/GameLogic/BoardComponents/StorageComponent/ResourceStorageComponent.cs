﻿public class ResourceStorageComponent : IECSComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public CurrencyPair Resources { get; set; }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
}