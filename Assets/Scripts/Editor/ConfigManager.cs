#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public enum ProgressState
{
    None,
    Start,
    Progress,
}

public class ConfigManager: CustomEditorBase
{
    private List<GoogleLink> cachedConfigLinks = new List<GoogleLink>();
    private List<string> selectedConfigs = new List<string>();

    private bool isInitComplete = false;
    private bool IsSelectAll => cachedConfigLinks.Count > 0 && cachedConfigLinks.Any(link => selectedConfigs.Contains(link.Key) && IsVisible(link.Key));

    private string filter;
    
    private GUIStyle waitUpdateTextStyle;
    private GUIStyle lastVersionTextStyle;
    private GUIStyle loadTextStyle;

    private int asyncCountAll;
    private int asyncCountNow;
    private ProgressState progressState;
    private bool isAsyncStateChanged;
    
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

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayStateChanged;
    }

    private void OnPlayStateChanged(PlayModeStateChange state)
    {
        asyncCountAll = 0;
        asyncCountNow = 0;
        progressState = ProgressState.None;
        isAsyncStateChanged = false;
        EditorUtility.ClearProgressBar();
    }

    protected virtual void OnGUI()
    {
        if (isInitComplete == false)
        {
            InitStyles();
            isInitComplete = true;
        }

        if (isAsyncStateChanged)
        {
            OnAsyncStateChanged();
            isAsyncStateChanged = false;
        }

        BeginDisableArea(progressState != ProgressState.None, () =>
        {
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
        });
    }
    
    void  OnInspectorUpdate()
    {
        Repaint();
    }

#region WindowUI

    private void OnAsyncStateChanged()
    {
        var title = "update configs...";
        
        if (progressState == ProgressState.None)
        {
            EditorUtility.ClearProgressBar();
        }
        else if (progressState == ProgressState.Start)
        {
            EditorUtility.DisplayProgressBar(title, "Validate", 0);
        }
        else
        {
            EditorUtility.DisplayProgressBar(title, $"Download: {asyncCountNow}/{asyncCountAll}", asyncCountNow/(float)asyncCountAll);
        }
    }

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

    public static void AsyncProgressStart()
    {
        var window = GetWindow(typeof(ConfigManager)) as ConfigManager;
        if (window == null)
        {
            return;
        }
        window.asyncCountAll = 1;
        window.asyncCountNow = 0;
        window.progressState = ProgressState.Start;
        window.isAsyncStateChanged = true;
    }

    public static void AsyncProgressValidateStepComplete(int count)
    {
        var window = GetWindow(typeof(ConfigManager)) as ConfigManager;
        if (window == null)
        {
            return;
        }
        window.asyncCountAll = count;
        window.progressState = ProgressState.Progress;
        window.isAsyncStateChanged = true;
    }
    
    public static void AsyncProgressLoadStepComplete()
    {
        var window = GetWindow(typeof(ConfigManager)) as ConfigManager;
        if (window == null)
        {
            return;
        }

        window.asyncCountNow++;
        window.isAsyncStateChanged = true;
    }
    
    public static void AsyncProgressEnd()
    {
        var window = GetWindow(typeof(ConfigManager)) as ConfigManager;
        
        window.progressState = ProgressState.None;
        window.isAsyncStateChanged = true;
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
        ConfigsGoogleLoader.UpdateWithGoogle(true);
    }

    private void UpdateConfigsDefault()
    {
        ConfigsGoogleLoader.UpdateWithGoogle(false);
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