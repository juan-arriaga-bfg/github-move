using System;
using System.Collections.Generic;

public class IapCollection
{
    public readonly List<IapDefinition> Defs = new List<IapDefinition>();

    public readonly List<string> InapIds  = new List<string>();
    public readonly List<string> GoogleIds  = new List<string>();
    public readonly List<string> AmazonIds  = new List<string>();
    public readonly List<string> AppStoreIds  = new List<string>();
    
    public IapCollection Add(IapDefinition definition)
    {
        Defs.Add(definition);
        
        InapIds.Add(definition.Id);
        GoogleIds.Add(definition.GooglePlayId);
        AmazonIds.Add(definition.AmazonAppStoreId);
        AppStoreIds.Add(definition.AppleAppStoreId);
        
        return this;
    }

    private List<string> GetListOfIdsForCurrentStore()
    {
        List<string> storeIds;

#if UNITY_ANDROID
    #if AMAZON
        storeIds = AmazonIds;
    #else
        storeIds = GoogleIds;
    #endif    
#else
        storeIds = AppStoreIds;
#endif

        return storeIds;
    }
    
    public string GetIdByStoreId(string storeId)
    {
        var storeIds = GetListOfIdsForCurrentStore();
        
        int index = storeIds.IndexOf(storeId);
        if (index == -1)
        {
            throw new Exception($"[IapsCollection] => GetIdByStoreId({storeId}): Not found!");
        }

        return InapIds[index];
    }

    public string GetStoreId(string id)
    {
        var storeIds = GetListOfIdsForCurrentStore();
        
        int index = InapIds.IndexOf(id);
        if (index == -1)
        {
            throw new Exception($"[IapsCollection] => GetStoreId({id}): Not found!");
        }

        return storeIds[index];
    }
}