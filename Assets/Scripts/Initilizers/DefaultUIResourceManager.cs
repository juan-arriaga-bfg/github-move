using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUIResourceManager : IIWResourceManager 
{
    #region IIWResourceManager implementation

    public T LoadResource<T>(string resourcePath) where T : Object
    {
        return ContentService.Instance.Manager.GetObjectByName(resourcePath) as T;
    }

    #endregion


}
