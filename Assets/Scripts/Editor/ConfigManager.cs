#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public enum LoadState
{
    Unknown,
    LastVersion,
    NeedUpdate,
    Validate,
    Load,
    Error
}

[Serializable]
public class ConfigElementInfo
{
    public string Name;
    public LoadState State;
}

public class ConfigManager: CustomEditorBase
{
    public  List<ConfigElementInfo> configsStatus = new List<ConfigElementInfo>();
    private List<GoogleLink> cachedConfigLinks = new List<GoogleLink>();
    private List<string> selectedConfigs = new List<string>();

    private bool isInitComplete = false;
    private bool IsSelectAll => cachedConfigLinks.Count > 0 && cachedConfigLinks.Any(link => selectedConfigs.Contains(link.Key) && IsVisible(link.Key));

    private string filter;
    
    private GUIStyle waitUpdateTextStyle;
    private GUIStyle lastVersionTextStyle;
    private GUIStyle loadTextStyle;
    
    [MenuItem("Tools/Configs/Manager", false, 50)]
    public static void Create()
    {
        var window = GetWindow(typeof(ConfigManager)) as ConfigManager;
        window.Show();

        if (window.cachedConfigLinks.Count == 0)
        {
            window.RefreshElements();    
        }
    }
    
    protected virtual void OnGUI()
    {
        if (isInitComplete == false)
        {
            InitStyles();
            isInitComplete = true;
        }
        
        HorizontalArea(() =>
        {
            Button("Refresh", RefreshElements);
            Button("Update all (force)", UpdateConfigsForce);
            Button("Update all", UpdateConfigsDefault);
            Button("Update selected", UpdateSelected);    
        });
        
        HorizontalArea(() =>
        {
            SelectAllToggle(); 
            filter = EditorGUILayout.TextField(filter);
        });
        Separator();

        ScrollArea(this, () =>
        {
           ShowConfigList(); 
        });       
    }
    
    void  OnInspectorUpdate()
    {
        Repaint();
    }

#region WindowUI

    private void InitStyles()
    {
        waitUpdateTextStyle = new GUIStyle();
        waitUpdateTextStyle.normal.textColor = Color.red;
        
        lastVersionTextStyle = new GUIStyle();
        lastVersionTextStyle.normal.textColor = Color.grey;
        
        loadTextStyle = new GUIStyle();
        loadTextStyle.normal.textColor = Color.blue;
    }
    
    private void ConfigElement(string configName)
    {
        HorizontalArea(() =>
        {
            ConfigElementToggle(configName);

            EditorGUILayout.TextField(configName);
            ConfigStatusLabel(configName);

            Button("Update", () => UpdateTarget(configName));
        });
    }

    private void ConfigStatusLabel(string configName)
    {
        var selected = ConfigsGoogleLoader.ConfigsStatus.Where(config => config.Name == configName).ToList();

        var widthStyle = GUILayout.MaxWidth(130f);
        var fontStyle = waitUpdateTextStyle;
        
        if (selected.Count == 0)
        {
            EditorGUILayout.LabelField($"status: {LoadState.Unknown}", fontStyle, widthStyle);
            return;
        }
        
        var targetConfig = selected[0];
        string message = Enum.GetName(typeof(LoadState), targetConfig.State);

        if (targetConfig.State == LoadState.Load || targetConfig.State == LoadState.Validate)
        {
            fontStyle = loadTextStyle;
        }
        else if (targetConfig.State == LoadState.LastVersion)
        {
            fontStyle = lastVersionTextStyle;
        }
        
        EditorGUILayout.LabelField($"status: {message}", fontStyle, widthStyle);
    }

    private void ConfigElementToggle(string configName)
    {
        var isSelected = selectedConfigs.Contains(configName);

        var toggleState = EditorGUILayout.Toggle(isSelected, GUILayout.MaxWidth(EditorGUIUtility.fieldWidth * 0.75f));
        if (toggleState != isSelected)
        {
            if (toggleState)
            {
                selectedConfigs.Add(configName);
            }
            else
            {
                selectedConfigs.Remove(configName);
            }
        }
    }

    private void ShowConfigList()
    {
        foreach (var configLink in cachedConfigLinks)
        {
            if (IsVisible(configLink.Key))
            {
                ConfigElement(configLink.Key);    
            }
        }
    }

    private void SelectAllToggle()
    {
        var isSelectAll = IsSelectAll;
        if (EditorGUILayout.Toggle(isSelectAll, GUILayout.MaxWidth(EditorGUIUtility.fieldWidth * 0.75f)) != isSelectAll)
        {
            selectedConfigs.RemoveAll(elem => IsVisible(elem));
            if (!isSelectAll)
            {
                foreach (var configLink in cachedConfigLinks.Where(link => IsVisible(link.Key)))
                {
                    selectedConfigs.Add(configLink.Key);    
                }
            }
        }
    }
    
    private bool IsVisible(string configName)
    {
        return cachedConfigLinks.Any(elem => elem.Key == configName) && string.IsNullOrWhiteSpace(filter) || Regex.IsMatch(configName, $"^{filter}");
    }
    
#endregion

#region WindowLogic

    private void RefreshElements()
    {
        cachedConfigLinks.Clear();
        foreach (var path in NSConfigsSettings.Instance.ConfigNames)
        {
            var key = path.Substring(path.LastIndexOf("/") + 1);
            key = key.Substring(0, key.IndexOf("."));
                
            var gLink = GoogleLoaderSettings.Instance.ConfigLinks.Find(link => link.Key == key);
    
            if (gLink == null) continue;
                
            cachedConfigLinks.Add(gLink);
        }

        cachedConfigLinks = cachedConfigLinks.OrderBy(elem => elem.Key).ToList();
        
        ConfigsGoogleLoader.UpdateStatus(cachedConfigLinks.Select(elem => elem.Key).ToList());
    }

    private void UpdateConfigsForce()
    {
        ConfigsGoogleLoader.ForceUpdateWithGoogleClick();
    }

    private void UpdateConfigsDefault()
    {
        ConfigsGoogleLoader.UpdateWithGoogleClick();
    }

    private void UpdateTarget(string configName)
    {
        ConfigsGoogleLoader.UpdateTarget(new List<string> {configName}, true);
    }

    private void UpdateSelected()
    {
        if (selectedConfigs.Count == 0)
        {
            return;
        }
        ConfigsGoogleLoader.UpdateTarget(selectedConfigs.ToList(), true);
    }

#endregion
    
}
#endif