using UnityEngine;

public class AsyncInitManager : ECSEntity, IAsyncInitManager
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public bool IsAllComponentsInited()
    {
        var collection = GetComponent<ECSComponentCollection>(AsyncInitComponent.ComponentGuid);

        if (collection == null)
        {
            Debug.LogError($"[AsyncInitManager] => IsAllComponentsInited: No AsyncInitComponent added. Do not call check until all components are registered");
            return false;
        }

        for (var i = 0; i < collection.Components.Count; i++)
        {
            IECSComponent cmp = collection.Components[i];
            AsyncInitComponent component = (AsyncInitComponent) cmp;
            if (!component.IsCompleted)
            {
                Debug.LogError($"[AsyncInitManager] => IsAllComponentsInited: Waiting for '{component.GetType()}'");
                return false;
            }
        }

        return true;
    }

    public T GetAsyncInitComponent<T>() where T:AsyncInitComponent
    {
        var collection = GetComponent<ECSComponentCollection>(AsyncInitComponent.ComponentGuid);
        
        if (collection == null)
        {
            return default;
        }
        
        for (var i = 0; i < collection.Components.Count; i++)
        {
            IECSComponent cmp = collection.Components[i];
            if (cmp is T initComponent)
            {
                return initComponent;
            }
        }
        
        return default;
    }
}