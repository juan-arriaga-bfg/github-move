using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IW.Content.ContentModule;
using TMPro;
using UnityEngine;

public class FontTester : MonoBehaviour
{
    [SerializeField] private Transform host;
    [SerializeField] private NSText prefab;

    public static FontTester Instance;
    
    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        IWAssetBundleManager assetBundleManager = new IWAssetBundleManager();
        IDataMapper<List<IWAssetBundleData>> assetBundleManagerDataMapper = new ResourceConfigDataMapper<List<IWAssetBundleData>>("iw/assetbundles.data", false);
        assetBundleManager.LoadData(assetBundleManagerDataMapper);
        IWAssetBundleService.Instance.SetManager(assetBundleManager);
        
        ContentManager contentManager = new DefaultContentManager();
        IDataMapper<List<ContentData>> contentManagerDataMapper = new ResourceConfigDataMapper<List<ContentData>>("iw/content.data", false);
        contentManager.LoadData(contentManagerDataMapper);
        ContentService.Instance.SetManager(contentManager);
        
        RefreshToStyleTest();
    }

    [ContextMenu("Refresh")]
    public void RefreshToStyleTest()
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
    
    public void RefreshToIconTest()
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

        var iconManager = IconService.Instance.Manager as IconResourceManager;

        var usedIcons = new HashSet<string>();
        
        GenearateIconText(usedIcons, "pointLightProgressLine");
        GenearateIconText(usedIcons, "icon_Complete");
        
        var currencyType = typeof(Currency);
        var fields = currencyType.GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (var fieldInfo in fields)
        {
            var def = fieldInfo.GetValue(null) as CurrencyDef;
            if (def != null)
            {
                if (iconManager.CachedIconsData.ContainsKey(def.Icon))
                {
                    GenearateIconText(usedIcons, def.Icon);
                }
            }
        }
        
        foreach (var pieceId in PieceType.GetAllIds())
        {
            var pieceType = PieceType.GetDefById(pieceId);
            var pieceName = pieceType.Abbreviations[0];
            if (iconManager.CachedIconsData.ContainsKey(pieceName))
            {
                GenearateIconText(usedIcons, pieceName);    
            }
        }
        
        prefab.gameObject.SetActive(false);
    }

    private void GenearateIconText(HashSet<string> usedIcons, string iconName)
    {
        if (usedIcons.Contains(iconName))
        {
            return;
        }
        
        usedIcons.Add(iconName);
        
        NSText text = Instantiate(prefab.gameObject, prefab.transform.parent).GetComponent<NSText>();
            
        text.gameObject.name = $"[icon '{iconName}']";
        var textMeshId = text.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMeshId.text = text.gameObject.name;
            
        text.Text = $"{iconName}: text for <sprite name={iconName}> example \n";
        text.Text += $"<sprite name={iconName}> and icon on begining string and end string <sprite name={iconName}> \n";
        text.Text += $"icon without space before<sprite name={iconName}>and after\n";
        text.Text += $"numbers before 145<sprite name={iconName}>345 and after\n";
    }

    public void ToggleTest(bool value)
    {
        if (value)
        {
            RefreshToIconTest();
        }
        else
        {
            RefreshToStyleTest();
        }
    }
}
