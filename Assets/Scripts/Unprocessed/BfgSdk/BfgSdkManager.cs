using UnityEngine;

public class BfgSdkManager : ECSEntity, IBfgSdkManager
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private ECSComponentCollection cachedThirdPartyInitComponentCollection;
    
    public bool IsAllComponentsInited()
    {
        if (cachedThirdPartyInitComponentCollection == null)
        {
            cachedThirdPartyInitComponentCollection = GetComponent<ECSComponentCollection>(ThirdPartyInitComponent.ComponentGuid);
        }

        if (cachedThirdPartyInitComponentCollection == null)
        {
            Debug.LogError($"[BfgSdkManager] => IsAllComponentsInited: No ThirdPartyInitComponent added. Do not call check until all components are registered");
            return false;
        }

        for (var i = 0; i < cachedThirdPartyInitComponentCollection.Components.Count; i++)
        {
            IECSComponent cmp = cachedThirdPartyInitComponentCollection.Components[i];
            ThirdPartyInitComponent component = (ThirdPartyInitComponent) cmp;
            if (!component.IsCompleted)
            {
                Debug.LogError($"[BfgSdkManager] => IsAllComponentsInited: Waiting for '{component.GetType()}'");
                return false;
            }
        }

        return true;
    }
}