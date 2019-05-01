 using System;
 using System.Collections.Generic;

 public class ServerSideConfigsManager : ECSEntity
{    
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private readonly Dictionary<int, object> data = new Dictionary<int, object>();

    public Action<int, object> OnDataReceived;

    public readonly List<ServerSideConfigLoaderBase> loaders = new List<ServerSideConfigLoaderBase>();
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        foreach (var loader in loaders)
        {
            UnRegisterComponent(loader);
        }
        
        InternetMonitorService.Current.OnStateChange += OnInternetStateChanged;
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        Cleanup();
    }

    public void Cleanup()
    {
        InternetMonitorService.Current.OnStateChange -= OnInternetStateChanged;
        OnDataReceived = null;
    }

    private void OnInternetStateChanged(InternetConnectionState state)
    {
        if (state == InternetConnectionState.Available)
        {
            UpdateAll();
        }
    }

    public override ECSEntity RegisterComponent(IECSComponent component, bool isCollection = false)
    {
        if (component is ServerSideConfigLoaderBase loader)
        {
           loaders.Add(loader); 
        }
        
        return base.RegisterComponent(component, isCollection);
    }

    public T GetData<T>()
    {
        foreach (var item in data.Values)
        {
            if (item is T ret)
            {
                return ret;
            }
        }

        return default;
    }
    
    public object GetData(int guid)
    {
        return data.TryGetValue(guid, out object ret) ? ret : null;
    }

    public void UpdateAll()
    {
        IW.Logger.Log($"[ServerSideConfigsManager] => UpdateAll");

        foreach (var loader in loaders)
        {
            loader.Update();
        }
    }
    
    public void DataReceived(IECSComponent sender, object receivedData)
    {
        data.Remove(sender.Guid);
        data.Add(sender.Guid, receivedData);

        OnDataReceived?.Invoke(sender.Guid, receivedData);
    }
}