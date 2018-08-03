using System.Collections.Generic;
using System.IO;
using System.Text;
using Dws;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

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
    
    [MenuItem("Tools/Configs/Update with Google")]
    public static void UpdateWithGoogle()
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
        
        CheckNeedToUpdate();
    }
    
    private static void CheckNeedToUpdate()
    {
        index--;

        if (index < 0)
        {
            Debug.LogWarning("check for the need to update the configs completed!");
            index = update.Count;
            
            EditorUtility.ClearProgressBar();
            
            Load();

            return;
        }

        var gLink = update[index].Value;
        var req = new WebRequestData(GetUrl(gLink.Link, "getLastUpdated", ""));

        float progress = 1 - (index / (float)filesToCheckCount);
        EditorUtility.DisplayProgressBar("Configs update...", string.Format("[{1}/{2}] Validating '{0}'", update[index].Key, filesToCheckCount - index, filesToCheckCount), progress);
        
        WebHelper.MakeRequest(req, (response) =>
        {
            if (response.IsOk == false || string.IsNullOrEmpty(response.Error) == false )
            {
                Debug.LogErrorFormat("Can't check last updated {0}. response.IsOk = {1}. Error: {2}", gLink.Key, response.IsOk, response.Error);
                CheckNeedToUpdate();
                
                EditorUtility.ClearProgressBar();
                
                return;
            }
                
            var root = JObject.Parse(response.Result);
            var result = root["result"];
            
            if (EditorPrefs.HasKey(gLink.Key) == false)
            {
                EditorPrefs.SetString(gLink.Key, result.ToString());
                Debug.LogWarningFormat("Config {0} need to update", gLink.Key);
            }
            else
            {
                var then = long.Parse(EditorPrefs.GetString(gLink.Key));
                var now = long.Parse(result.ToString());
                
                if (then < now)
                {
                    EditorPrefs.SetString(gLink.Key, result.ToString());
                    Debug.LogWarningFormat("Config {0} need to update", gLink.Key);
                }
                else
                {
                    update.Remove(update[index]);
                }
            }

            CheckNeedToUpdate();
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

        WebHelper.MakeRequest(req, (response) =>
        {
            if (response.IsOk == false || string.IsNullOrEmpty(response.Error) == false )
            {
                Debug.LogErrorFormat("Can't load data {0}. response.IsOk = {1}. Error: {2}", gLink.Key, response.IsOk, response.Error);
                Load();
                
                EditorUtility.ClearProgressBar();
                
                return;
            }
                
            var root = JObject.Parse(response.Result);
            var result = root["result"];
                
            File.WriteAllText(Application.dataPath + update[index].Key, JsonConvert.SerializeObject(result, Formatting.Indented), Encoding.UTF8);
            
            Load();
        });
    }
    
    private static string GetUrl(string id, string route, string json)
    {
        return string.Format("https://script.google.com/macros/s/AKfycbz82MTaf-dECcAPhCIveDy9R0OPApWfWUx6aLScGaWQKsIK6D4/exec?route={0}&spreadsheetId={1}&pattern={2}", route, id, json);
    }
}