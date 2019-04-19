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

public class ConfigsGoogleLoader
{
    private static int index;
    private static List<KeyValuePair<string, GoogleLink>> update;
    private static int filesToCheckCount;

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
        UpdateWithGoogle(true);
    }

    [MenuItem("Tools/Configs/Update configs with Google", false, 50)]
    public static void UpdateWithGoogleClick()
    {
        UpdateWithGoogle(false);
    }

    public static void UpdateTarget(List<string> configNames, bool forceUpdate)
    {
        update = new List<KeyValuePair<string, GoogleLink>>();

        foreach (var path in NSConfigsSettings.Instance.ConfigNames)
        {
            var key = path.Substring(path.LastIndexOf("/") + 1);
            key = key.Substring(0, key.IndexOf("."));
            
            var gLink = GoogleLoaderSettings.Instance.ConfigLinks.Find(link => link.Key == key);

            if (gLink == null || configNames.Contains(gLink.Key) == false) continue;
            
            update.Add(new KeyValuePair<string, GoogleLink>(path, gLink));
        }
        
        index = update.Count;
        filesToCheckCount = index;
        
        CheckNeedToUpdate(forceUpdate);
    }
    
    private static void UpdateWithGoogle(bool forceUpdate)
    {
        update = new List<KeyValuePair<string, GoogleLink>>();

        foreach (var path in NSConfigsSettings.Instance.ConfigNames)
        {
            var key = path.Substring(path.LastIndexOf("/") + 1);
            key = key.Substring(0, key.IndexOf("."));
            
            var gLink = GoogleLoaderSettings.Instance.ConfigLinks.Find(link => link.Key == key);

            if (gLink == null) continue;
            
            update.Add(new KeyValuePair<string, GoogleLink>(path, gLink));
        }
        
        index = update.Count;
        filesToCheckCount = index;
        
        CheckNeedToUpdate(forceUpdate);
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

        foreach (var configName in configNames)
        {
            var gLink = GoogleLoaderSettings.Instance.ConfigLinks.Find(link => link.Key == configName);

            if (gLink == null) continue;
            
            update.Add(gLink);
        }
        
        index = update.Count;
        filesToCheckCount = index;
        
        var idsArray = update.Select(e => e.Link).ToArray();
        HashSet<string> uniqIds = new HashSet<string>(idsArray);
        var idsStr = string.Join(",", uniqIds);

        //var gLink = update[index].Value;
        var req = new WebRequestData(GetUrl("getLastUpdated", "ids=" + idsStr));
        
        foreach (var configName in update.Select(elem => elem.Key))
        {
            var configStatus = GetConfigInfo(configName);
            if (configStatus.State == LoadState.Load)
            {
                continue;
            }

            configStatus.State = LoadState.Validate;
        }
        
        WebHelper.MakeRequest(req, (response) =>
        {
            if (response.IsOk == false || string.IsNullOrEmpty(response.Error) == false )
            {
                Debug.LogErrorFormat("Can't check last updated. Response.IsOk = {1}. Error: {2}", response.IsOk, response.Error);
                return;
            }
                
            var root = JObject.Parse(response.Result);
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
                
                if (configInfo.State == LoadState.Unknown)
                {
                    configInfo.State = LoadState.NeedUpdate;
                }
                
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
                    GetConfigInfo(gLink.Key).State = LoadState.LastVersion;
                }
            }
        });
    }
    
    private static void CheckNeedToUpdate(bool forceUpdate)
    {
        HashSet<string> alwaysUpdate = new HashSet<string>
        {
            //"conversations"
        };
        
        var idsArray = update.Select(e => e.Value.Link).ToArray();
        HashSet<string> uniqIds = new HashSet<string>(idsArray);
        var idsStr = string.Join(",", uniqIds);

        //var gLink = update[index].Value;
        var req = new WebRequestData(GetUrl("getLastUpdated", "ids=" + idsStr));
        
        foreach (var configName in update.Select(elem => elem.Value.Key))
        {
            var configStatus = GetConfigInfo(configName);
            if (configStatus.State == LoadState.Load)
            {
                continue;
            }

            configStatus.State = LoadState.Validate;
        }
        
        WebHelper.MakeRequest(req, (response) =>
        {
            foreach (var configName in update.Select(elem => elem.Value.Key))
            {
                var configStatus = GetConfigInfo(configName);
                if (configStatus.State == LoadState.Load)
                {
                    continue;
                }

                configStatus.State = LoadState.LastVersion;
            }
            
            if (response.IsOk == false || string.IsNullOrEmpty(response.Error) == false )
            {
                Debug.LogErrorFormat("Can't check last updated. Response.IsOk = {1}. Error: {2}", response.IsOk, response.Error);
                return;
            }
                
            var root = JObject.Parse(response.Result);
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
                
                if (then < now || alwaysUpdate.Contains(gLink.Key))
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
            index = update.Count;

            Load();
        });
    }

    private static void OnLoadComplete()
    {
        Debug.LogWarning("Configs load data complete!");
        if (update.Count != 0)
        {
            NSConfigEncription.EncryptConfigs();
        }
    }
    
    private static void Load()
    {
        DevTools.IsSequenceReset = true;

        if (update.Count == 0)
        {
            return;
        }

        var path = update[0].Key;
        var gLink = update[0].Value;
        update.RemoveAt(0);
        
        Load();
        
        var configStatus = GetConfigInfo(gLink.Key);
        if (configStatus.State == LoadState.Load)
        {
            return;
        }

        Task.Run(() =>
        {
            string text = string.Format("Downloading: '{0}'", gLink.Key);
            Debug.LogWarningFormat(text);
        
            var req = new WebRequestData(GetUrl(gLink.Link, gLink.Route, gLink.Pattern));

            Stopwatch sw = new Stopwatch();
            sw.Start();
        
        
            configStatus.State = LoadState.Load;
        
            WebHelper.MakeRequest(req, (response) =>
            {
                sw.Stop();
            
                Debug.LogFormat($"Request '{gLink.Key}' completed in {sw.Elapsed.TotalSeconds:F}s");

                if (response.IsOk == false || string.IsNullOrEmpty(response.Error) == false )
                {
                    Debug.LogErrorFormat("Can't load data {0}. response.IsOk = {1}. Error: {2}", gLink.Key, response.IsOk, response.Error);
                    configStatus.State = LoadState.Error;
                    if (ConfigsStatus.All(elem => elem.State != LoadState.Load))
                    {
                        OnLoadComplete();
                    }   
                    return;
                }

                try
                {
                    var root   = JObject.Parse(response.Result);
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
