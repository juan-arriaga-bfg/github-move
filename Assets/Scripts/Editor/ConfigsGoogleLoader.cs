using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BestHTTP;
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

            if (gLink == null) continue;
            
            GoogleDriveDownloader.Get(GetUrl(gLink.Link), (state, data) =>
            {
                if (state == HTTPRequestStates.Finished)
                {
                    string text = data;
                    Parse(key, text);
                    /*Debug.LogWarningFormat("TSV table downloaded: {0} bytes", text.Length);
                    File.WriteAllText(Application.dataPath + relativePath, text, Encoding.UTF8);*/
                }
                else
                {
                    Debug.LogErrorFormat("Can't load data: {0}", data ?? "Unknown error");
                }
            });
        }
    }
    
    private static string GetUrl(string id)
    {
        return string.Format("https://docs.google.com/spreadsheets/d/{0}/export?format=tsv", id);
    }

    private static void Parse(string key, string text)
    {
        var name = string.Format("{0}{1}ConfigParser", key.Substring(0, 1).ToUpper(), key.Substring(1));
        
        var parserType = Type.GetType(name, false, true);
 
        //если класс не найден
        if (parserType != null)
        {
            //получаем конструктор
            var constructor = parserType.GetConstructor(new Type[] { });
 
            //вызываем конструтор
            var parser = constructor.Invoke(new object[] { }) as IConfigParser;

            parser.Parse(text);
        }
        else
        {
            Debug.LogError("Класс не найден");
        }
    }
}