using System.Collections;
using System.Collections.Generic;
using IW.Content.ContentModule;
using TMPro;
using UnityEngine;

public class FontStylesTester : MonoBehaviour
{
    [SerializeField] private Transform host;
    [SerializeField] private NSText prefab;

    private void Start()
    {
        IWAssetBundleManager assetBundleManager = new IWAssetBundleManager();
        IDataMapper<List<IWAssetBundleData>> assetBundleManagerDataMapper = new ResourceConfigDataMapper<List<IWAssetBundleData>>("iw/assetbundles.data", false);
        assetBundleManager.LoadData(assetBundleManagerDataMapper);
        IWAssetBundleService.Instance.SetManager(assetBundleManager);
        
        ContentManager contentManager = new DefaultContentManager();
        IDataMapper<List<ContentData>> contentManagerDataMapper = new ResourceConfigDataMapper<List<ContentData>>("iw/content.data", false);
        contentManager.LoadData(contentManagerDataMapper);
        ContentService.Instance.SetManager(contentManager);
        
        Refresh();
    }
    
    [ContextMenu("Refresh")]
    public void Refresh()
    {
        var childsCount = host.childCount;
        for (int i = childsCount - 1; i >= 0 ; i--)
        {
            var obj = host.GetChild(i);
            if (obj.transform == prefab.transform)
            {
                continue;
            }
            
            Destroy(obj.gameObject);
        }
        
        prefab.gameObject.SetActive(true);


        foreach (var style in FontsSettings.Instance.Styles)
        {
            NSText text = Instantiate(prefab.gameObject, prefab.transform.parent).GetComponent<NSText>();
            text.StyleId = style.Id;
            text.ApplyStyle();

            text.gameObject.name = $"[style '{style.NameId}' id {style.Id}]";
            text.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text.gameObject.name;
        }
        
        prefab.gameObject.SetActive(false);
    }
}
