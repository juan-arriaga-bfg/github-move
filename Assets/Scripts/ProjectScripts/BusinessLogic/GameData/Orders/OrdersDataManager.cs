using System.Collections.Generic;
using UnityEngine;

public class OrdersDataManager : IECSComponent, IDataManager, IDataLoader<List<OrderDef>>
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    public void OnRegisterEntity(ECSEntity entity)
    {
        Reload();
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }

    public List<OrderDef> Orders;

    public void Reload()
    {
        Orders = null;
        LoadData(new ResourceConfigDataMapper<List<OrderDef>>("configs/orders.data",
            NSConfigsSettings.Instance.IsUseEncryption));
    }

    public void LoadData(IDataMapper<List<OrderDef>> dataMapper)
    {
        dataMapper.LoadData((data, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                Orders = data;
            }
            else
            {
                Debug.LogWarningFormat("[{0}]: config not loaded", GetType());
            }
        });
    }
}