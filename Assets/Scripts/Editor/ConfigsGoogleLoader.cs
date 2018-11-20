#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Dws;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ConfigsGoogleLoader
{
    private static int index;
    private static List<KeyValuePair<string, GoogleLink>> update;
    private static int filesToCheckCount;
    
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
    
    private static void CheckNeedToUpdate(bool forceUpdate)
    {
        HashSet<string> alwaysUpdate = new HashSet<string>
        {
            "questStartConversations"
        };
        
        var idsArray = update.Select(e => e.Value.Link).ToArray();
        HashSet<string> uniqIds = new HashSet<string>(idsArray);
        var idsStr = string.Join(",", uniqIds);

        //var gLink = update[index].Value;
        var req = new WebRequestData(GetUrl("getLastUpdated", "ids=" + idsStr));
        EditorUtility.DisplayProgressBar("Configs update...", "Validating...", 0);
               
        WebHelper.MakeRequest(req, (response) =>
        {
            EditorUtility.ClearProgressBar();
            
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

    private static void Load()
    {
        index--;

        if (index < 0)
        {
            Debug.LogWarning("Configs load data complete!");
            if(update.Count != 0) NSConfigEncription.EncryptConfigs();
            
            EditorUtility.ClearProgressBar();
            
            return;
        }
        
        var gLink = update[index].Value;

        string text = string.Format("[{1}/{2}] Downloading: '{0}'", gLink.Key, update.Count - index, update.Count);
        EditorUtility.DisplayProgressBar("Configs update...", text, 1 - (index / (float)update.Count));

        Debug.LogWarningFormat(text);
        
        var req = new WebRequestData(GetUrl(gLink.Link, gLink.Route, gLink.Pattern));

        Stopwatch sw = new Stopwatch();
        sw.Start();
        
        WebHelper.MakeRequest(req, (response) =>
        {
            sw.Stop();
            
            Debug.LogFormat($"Request '{gLink.Key}' completed in {sw.Elapsed.TotalSeconds:F}s");
            
            if (response.IsOk == false || string.IsNullOrEmpty(response.Error) == false )
            {
                Debug.LogErrorFormat("Can't load data {0}. response.IsOk = {1}. Error: {2}", gLink.Key, response.IsOk, response.Error);
                Load();
                
                EditorUtility.ClearProgressBar();
                
                return;
            }

            try
            {
                var root   = JObject.Parse(response.Result);
                var result = root["result"];

                File.WriteAllText(Application.dataPath + update[index].Key, JsonConvert.SerializeObject(result, Formatting.Indented), Encoding.UTF8);

                EditorPrefs.SetString(gLink.Key, DateTime.UtcNow.ConvertToUnixTimeMilliseconds().ToString());
                
                Load();
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw;
            }
        });
    }
    
    private static string GetUrl(string id, string route, string json)
    {
        return string.Format("https://script.google.com/macros/s/AKfycbz82MTaf-dECcAPhCIveDy9R0OPApWfWUx6aLScGaWQKsIK6D4/exec?route={0}&spreadsheetId={1}&pattern={2}", route, id, json);
    }
    
    private static string GetUrl(string route, string prms)
    {
        return string.Format("https://script.google.com/macros/s/AKfycbz82MTaf-dECcAPhCIveDy9R0OPApWfWUx6aLScGaWQKsIK6D4/exec?route={0}&{1}", route, prms);
    }
}
#endif
