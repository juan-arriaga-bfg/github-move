#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dws;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Debug = IW.Logger;

public enum LoadState
{
    Unknown,
    LastVersion,
    NeedUpdate,
    Validate,
    Load,
    Error
}

public class ConfigElementInfo
{
    public string Name;
    public LoadState State;
}

public class ConfigsGoogleLoader
{
    public static List<ConfigElementInfo> ConfigsStatus = new List<ConfigElementInfo>();
    
    [MenuItem("Tools/Configs/GenerateLinkSettings")]
    public static void GenerateLinkSettings()
    {
        if (GoogleLoaderSettings.Instance == null)
        {
            GoogleLoaderSettings.GenerateSettings<GoogleLoaderSettings>();

            Debug.Log("[GoogleLoaderSettings] -> settings generated");
        }
    }

    [MenuItem("Tools/Configs/Update configs with Google (force)", false, 50)]
    public static void ForceUpdateWithGoogleClick()
    {
        ConfigManager.Create();
        UpdateWithGoogle(true);
    }

    [MenuItem("Tools/Configs/Update configs with Google", false, 50)]
    public static void UpdateWithGoogleClick()
    {
        ConfigManager.Create();
        UpdateWithGoogle(false);
    }

    public static void UpdateTarget(List<string> configNames, bool forceUpdate)
    {
        var update = new List<KeyValuePair<string, GoogleLink>>();

        foreach (var path in NSConfigsSettings.Instance.ConfigNames)
        {
            var key = path.Substring(path.LastIndexOf("/") + 1);
            key = key.Substring(0, key.IndexOf("."));

            var gLink = GoogleLoaderSettings.Instance.ConfigLinks.Find(link => link.Key == key);

            if (gLink == null || configNames.Contains(gLink.Key) == false) continue;

            update.Add(new KeyValuePair<string, GoogleLink>(path, gLink));
        }

        CheckNeedToUpdate(forceUpdate, update);
    }
    
    public static void UpdateWithGoogle(bool forceUpdate)
    {
        var update = new List<KeyValuePair<string, GoogleLink>>();

        foreach (var path in NSConfigsSettings.Instance.ConfigNames)
        {
            var key = path.Substring(path.LastIndexOf("/") + 1);
            key = key.Substring(0, key.IndexOf("."));
            
            var gLink = GoogleLoaderSettings.Instance.ConfigLinks.Find(link => link.Key == key);

            if (gLink == null) continue;
            
            update.Add(new KeyValuePair<string, GoogleLink>(path, gLink));
        }
        
        CheckNeedToUpdate(forceUpdate, update);
    }

    private static ConfigElementInfo GetConfigInfo(string configName)
    {
        var gLink = GoogleLoaderSettings.Instance.ConfigLinks.Find(link => link.Key == configName);
        
        if (gLink == null)
        {
            return null;
        }
        
        if (ConfigsStatus.Any(elem => elem.Name == gLink.Key))
        {
            var configStatus = ConfigsStatus.First(elem => elem.Name == gLink.Key);
            return configStatus;
        }
        
        var newConfigStatus = new ConfigElementInfo {
            Name = configName,
            State = LoadState.Unknown
        };
        ConfigsStatus.Add(newConfigStatus);
        return newConfigStatus;
    }

    public static void UpdateStatus(List<string> configNames)
    {
        var update = new List<GoogleLink>();

        foreach (var currentConfigName in configNames)
        {
            var gLink = GoogleLoaderSettings.Instance.ConfigLinks.Find(link => link.Key == currentConfigName);
            if (gLink == null) continue;
            
            update.Add(gLink);
        }
        
        var idsStr = string.Join(",", update.Select(e => e.Link).Distinct());
        
        foreach (var configName in update.Select(elem => elem.Key))
        {
            var configStatus = GetConfigInfo(configName);
            if (configStatus.State == LoadState.Load)
            {
                continue;
            }

            configStatus.State = LoadState.Validate;
        }
        
        WebHelper.MakeRequest(GetUrl("getLastUpdated", "ids=" + idsStr), (error, response) =>
        {
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogErrorFormat("Can't check last updated.");
                return;
            }
                
            var root = JObject.Parse(response);
            var result = root["result"];

            Dictionary<string, long> timestamps = new Dictionary<string, long>();

            var timeStamp = result.First;
            while (timeStamp != null)
            {
                timestamps.Add(timeStamp["id"].ToString(), timeStamp["date"].Value<long>());
                timeStamp = timeStamp.Next;
            }
            
            for (int i = update.Count - 1; i >= 0; i--)
            {
                var gLink = update[i];

                var configInfo = GetConfigInfo(gLink.Key);
                
                if (configInfo.State == LoadState.Load)
                {
                    continue;
                }
                
                configInfo.State = LoadState.NeedUpdate;
                
                if (!timestamps.ContainsKey(gLink.Link))
                {
                    Debug.LogWarningFormat("Config {0} need to update", gLink.Key);
                    continue;
                }
                
                if (EditorPrefs.HasKey(gLink.Key) == false)
                {
                    Debug.LogWarningFormat("Config {0} need to update", gLink.Key);
                    continue;
                }

                var then = long.Parse(EditorPrefs.GetString(gLink.Key));
                var now = timestamps[gLink.Link];
                
                if (then < now)
                {
                    Debug.LogWarningFormat("Config {0} need to update. then {1} < now {2}", gLink.Key, then, now);
                }
                else
                {
                    Debug.LogWarningFormat("Config {0} is up to date", gLink.Key);
                    configInfo.State = LoadState.LastVersion;
                }
            }
        });
    }
    
    private static void CheckNeedToUpdate(bool forceUpdate, List<KeyValuePair<string, GoogleLink>> update)
    {
//        HashSet<string> alwaysUpdate = new HashSet<string>
//        {
//            "layout"
//        };
        
        var idsArray = update.Select(e => e.Value.Link).ToArray();
        HashSet<string> uniqIds = new HashSet<string>(idsArray);
        var idsStr = string.Join(",", uniqIds);

        foreach (var configName in update.Select(elem => elem.Value.Key))
        {
            var configStatus = GetConfigInfo(configName);
            if (configStatus.State == LoadState.Load || configStatus.State == LoadState.Validate)
            {
                update.RemoveAll(elem => elem.Key == configName);
                continue;
            }

            configStatus.State = LoadState.Validate;
        }
        
        ConfigManager.AsyncProgressStart();
        
        WebHelper.MakeRequest(GetUrl("getLastUpdated", "ids=" + idsStr), (error, response) =>
        {
            foreach (var configName in update.Select(elem => elem.Value.Key))
            {
                var configStatus = GetConfigInfo(configName);
                configStatus.State = LoadState.LastVersion;
            }
            
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogErrorFormat("Can't check last updated");
                return;
            }
                
            var root = JObject.Parse(response);
            var result = root["result"];

            Dictionary<string, long> timestamps = new Dictionary<string, long>();

            var timeStamp = result.First;
            while (timeStamp != null)
            {
                timestamps.Add(timeStamp["id"].ToString(), timeStamp["date"].Value<long>());
                timeStamp = timeStamp.Next;
            }
            
            for (int i = update.Count - 1; i >= 0; i--)
            {
                var item = update[i];
                var gLink = item.Value;

                if (!timestamps.ContainsKey(gLink.Link))
                {
                    Debug.LogWarningFormat("Config {0} need to update", gLink.Key);
                    continue;
                }
                
                if (EditorPrefs.HasKey(gLink.Key) == false)
                {
                    Debug.LogWarningFormat("Config {0} need to update", gLink.Key);
                    continue;
                }

                var then = long.Parse(EditorPrefs.GetString(gLink.Key));
                var now = timestamps[gLink.Link];
                
                if (then < now)
                {
                    Debug.LogWarningFormat("Config {0} need to update. then {1} < now {2}", gLink.Key, then, now);
                }
                else
                {
                    Debug.LogWarningFormat("Config {0} is up to date", gLink.Key);
                    if (!forceUpdate)
                    {
                        update.Remove(update[i]);
                    }
                }
            }

            Debug.LogWarning("Check for the need to update the configs completed!");
            
            ConfigManager.AsyncProgressValidateStepComplete(update.Count);
            if (update.Count == 0)
            {
                ConfigManager.AsyncProgressEnd();
                return;
            }
            
            Load(update);
        });
    }

    private static void OnLoadComplete()
    {
        Debug.LogWarning("Configs load data complete!");
        ConfigManager.AsyncProgressEnd();
        EditorMainThreadSync.Execute(NSConfigEncription.EncryptConfigs);
    }
    
    private static void Load(List<KeyValuePair<string, GoogleLink>> update)
    {
        DevTools.IsSequenceReset = true;

        if (update.Count == 0)
        {
            return;
        }

        var path = update[0].Key;
        var gLink = update[0].Value;
        update.RemoveAt(0);
        
        Load(update);
        
        var configStatus = GetConfigInfo(gLink.Key);
        if (configStatus.State == LoadState.Load)
        {
            return;
        }

        Task.Run(() =>
        {
            string text = string.Format("Downloading: '{0}'", gLink.Key);
            Debug.LogWarningFormat(text);

            Stopwatch sw = new Stopwatch();
            sw.Start();
        
        
            configStatus.State = LoadState.Load;
        
            WebHelper.MakeRequest(GetUrl(gLink.Link, gLink.Route, gLink.Pattern), (error, response) =>
            {
                sw.Stop();
            
                Debug.LogFormat($"Request '{gLink.Key}' completed in {sw.Elapsed.TotalSeconds:F}s");
                
                ConfigManager.AsyncProgressLoadStepComplete();
                
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogErrorFormat("Can't load data {0}", gLink.Key);
                    configStatus.State = LoadState.Error;
                    if (ConfigsStatus.All(elem => elem.State != LoadState.Load))
                    {
                        OnLoadComplete();
                    }   
                    return;
                }

                try
                {
                    var root   = JObject.Parse(response);
                    var result = root["result"];

                    File.WriteAllText(Application.dataPath + path, JsonConvert.SerializeObject(result, Formatting.Indented), Encoding.UTF8);

                    EditorPrefs.SetString(gLink.Key, DateTime.UtcNow.ConvertToUnixTimeMilliseconds().ToString());
                
                    configStatus.State = LoadState.LastVersion;
                }
                catch (Exception e)
                {
                    configStatus.State = LoadState.Error;
                    throw;
                }
            
            
                if (ConfigsStatus.All(elem => elem.State != LoadState.Load))
                {
                    OnLoadComplete();
                }
            });
        });
    }
    
    private static string GetUrl(string id, string route, string json)
    {
        return $"https://script.google.com/macros/s/AKfycbz82MTaf-dECcAPhCIveDy9R0OPApWfWUx6aLScGaWQKsIK6D4/exec?route={route}&spreadsheetId={id}&pattern={json}";
    }
    
    private static string GetUrl(string route, string prms)
    {
        return $"https://script.google.com/macros/s/AKfycbz82MTaf-dECcAPhCIveDy9R0OPApWfWUx6aLScGaWQKsIK6D4/exec?route={route}&{prms}";
    }
}
#endif
