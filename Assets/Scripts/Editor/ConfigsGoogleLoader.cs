using System.IO;
using System.Text;
using Dws;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public class ConfigsGoogleLoader
{
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
        foreach (string relativePath in NSConfigsSettings.Instance.ConfigNames)
        {
            var key = relativePath.Substring(relativePath.LastIndexOf("/") + 1);
            key = key.Substring(0, key.IndexOf("."));

            var gLink = GoogleLoaderSettings.Instance.ConfigLinks.Find(link => link.Key == key);

            if (gLink == null || gLink.Key != "pieces") continue;

            var linkTest = GetUrl(gLink.Link, gLink.Pattern);
            
            var req = new WebRequestData(linkTest);

            WebHelper.MakeRequest(req, (response) =>
            {
                if (response.IsOk == false)
                {
                    Debug.LogErrorFormat("Can't load data {0}. response.IsOk = {1}", gLink.Key, response.IsOk);
                    return;
                }

                if (string.IsNullOrEmpty(response.Error) == false)
                {
                    Debug.LogErrorFormat("Can't load data {0}. Request Error: {1}", gLink.Key, response.Error);
                    return;
                }
                
                var root = JObject.Parse(response.Result);
                var result = root["result"];
                
                File.WriteAllText(Application.dataPath + relativePath, JsonConvert.SerializeObject(result, Formatting.Indented), Encoding.UTF8);
            });
        }
        
        //
        
        //{"Uid":"","Delay":0,"Price":{"Currency":"","Amount":0},"FastPrice":{"Currency":"","Amount":0},"PieceAmount":0,"PieceWeights":[{"Uid":"","Weight":0}],"ChestWeights":[{"Uid":"","Weight":0,"":"","Override":true}]}
//        NSConfigEncription.EncryptConfigs();
    }
    
    private static string GetUrl(string id, string json)
    {
        return string.Format("https://script.google.com/macros/s/AKfycbz82MTaf-dECcAPhCIveDy9R0OPApWfWUx6aLScGaWQKsIK6D4/exec?route={0}&spreadsheetId={1}&pattern={2}", "parse", id, json);
    }
}