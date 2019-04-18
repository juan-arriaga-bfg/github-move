#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class ConfigManager: CustomEditorBase
{
    private Dictionary<string, GoogleLink> cachedConfigLinks = new Dictionary<string, GoogleLink>();
    private HashSet<string> selectedConfigs = new HashSet<string>();
    private Dictionary<string, bool> configsStatus = new Dictionary<string, bool>();

    private bool isInitComplete = false;
    
    private GUIStyle waitUpdateTextStyle;
    private GUIStyle lastVersionTextStyle;
    
    [MenuItem("Tools/Configs/Manager", false, 50)]
    public static void Create()
    {
        var window = GetWindow(typeof(ConfigManager)) as ConfigManager;
        window.Show();
        
        window.RefreshElements();
    }
    
    protected virtual void OnGUI()
    {
        if (isInitComplete == false)
        {
            InitUI();
            isInitComplete = true;
        }
        
        HorizontalArea(() =>
        {
            Button("Refresh list", RefreshElements);
            Button("Update all (force)", UpdateConfigsForce);
            Button("Update all", UpdateConfigsDefault);
            Button("Update selected", UpdateSelected);    
        });
        ScrollArea(this, () =>
        {
           ShowConfigList(); 
        });
    }

#region WindowUI

    private void InitUI()
    {
        waitUpdateTextStyle = new GUIStyle();
        waitUpdateTextStyle.normal.textColor = Color.red;
        
        lastVersionTextStyle = new GUIStyle();
        lastVersionTextStyle.normal.textColor = Color.grey;
    }
    
    private void ConfigElement(string configName)
    {
        HorizontalArea(() =>
        {
            ConfigElementToggle(configName);
            VerticalArea(() =>
            {
                EditorGUILayout.TextField(configName);
                ConfigStatusLabel(configName);
            });
            
            Button("Update", () => UpdateTarget(configName));
//            Button("More", null);
        });
        Separator();
    }

    private void ConfigStatusLabel(string configName)
    {
        var isWaitUpdate = configsStatus.ContainsKey(configName) ? (bool?)configsStatus[configName] : null;
        if (isWaitUpdate == null)
        {
            EditorGUILayout.LabelField("Status: unknown", waitUpdateTextStyle);
        }
        else if (isWaitUpdate == true)
        {
            EditorGUILayout.LabelField("Status: wait update", waitUpdateTextStyle);   
        }
        else
        {
            EditorGUILayout.LabelField("Status: last version", lastVersionTextStyle);
        }
    }

    private void ConfigElementToggle(string configName)
    {
        var isSelected = selectedConfigs.Contains(configName);

        if (EditorGUILayout.Toggle(isSelected, GUILayout.MaxWidth(EditorGUIUtility.fieldWidth * 0.5f)))
        {
            selectedConfigs.Add(configName);
        }
        else
        {
            selectedConfigs.Remove(configName);
        }
    }

    private void ShowConfigList()
    {
        foreach (var configLink in cachedConfigLinks.Values)
        {
            ConfigElement(configLink.Key);
        }
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
                
            cachedConfigLinks.Add(path, gLink);
        }

        configsStatus = ConfigsGoogleLoader.GetConfigsStatus(cachedConfigLinks.Select(elem => elem.Value.Key).ToList());
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