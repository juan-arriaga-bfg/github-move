using System.Linq;
using UnityEngine;

public static class ECSExtentions
{
    public static void UnregisterRecursive(this IECSComponent itemToCleanup, ECSEntity parent = null)
    {
        return;
        
        // Debug.Log("======== UnregisterRecursive: " + itemToCleanup.GetType());
        switch (itemToCleanup)
        {
            case ECSEntity entity:
                var ecsComponents = entity.ComponentsCache.Values.ToList();
                for (var i = ecsComponents.Count - 1; i >= 0; i--)
                {
                    var item = ecsComponents[i];
                    item.UnregisterRecursive(entity);
                }

                break;
            
            case ECSComponentCollection collection:
                foreach (var item in collection.Components)
                {
                    item.UnregisterRecursive();
                }
                break;
            
            case IECSSystem system:
                ECSService.Current.SystemProcessor.UnRegisterSystem(system);
                break;
        }
        
        // Debug.Log($"== UnregisterRecursive: {itemToCleanup.GetType()} with context: {context?.GetType()}");
        parent?.UnRegisterComponent(itemToCleanup);
    }
}