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
        index = NSConfigsSettings.Instance.ConfigNames.Length;

        Load();
    }

    private static void Load()
    {
        index--;

        if (index < 0)
        {
            Debug.LogWarning("Configs load data complete!");
            NSConfigEncription.EncryptConfigs();
            return;
        }
        
        Debug.LogWarningFormat("Configs progress {0}/{1}!", NSConfigsSettings.Instance.ConfigNames.Length - index, NSConfigsSettings.Instance.ConfigNames.Length);

        var relativePath = NSConfigsSettings.Instance.ConfigNames[index];
        var key = relativePath.Substring(relativePath.LastIndexOf("/") + 1);
        key = key.Substring(0, key.IndexOf("."));

        var gLink = GoogleLoaderSettings.Instance.ConfigLinks.Find(link => link.Key == key);

        if (gLink == null)
        {
            Load();
            return;
        }

        var linkTest = GetUrl(gLink.Link, gLink.Pattern);
            
        var req = new WebRequestData(linkTest);

        WebHelper.MakeRequest(req, (response) =>
        {
            if (response.IsOk == false || string.IsNullOrEmpty(response.Error) == false )
            {
                Debug.LogErrorFormat("Can't load data {0}. response.IsOk = {1}. Error: {2}", gLink.Key, response.IsOk, response.Error);
                Load();
                return;
            }
                
            var root = JObject.Parse(response.Result);
            var result = root["result"];
                
            File.WriteAllText(Application.dataPath + relativePath, JsonConvert.SerializeObject(result, Formatting.Indented), Encoding.UTF8);
            
            Load();
        });
    }
    
    
    
    private static string GetUrl(string id, string json)
    {
        return string.Format("https://script.google.com/macros/s/AKfycbz82MTaf-dECcAPhCIveDy9R0OPApWfWUx6aLScGaWQKsIK6D4/exec?route={0}&spreadsheetId={1}&pattern={2}", "parse", id, json);
    }
}