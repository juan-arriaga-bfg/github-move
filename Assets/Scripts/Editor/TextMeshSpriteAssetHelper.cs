#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEditor;
using UnityEngine;

public class TextMeshSpriteAssetHelper : MonoBehaviour
{
    private const string PREFS_KEY = "TextMeshSpriteAssetHelper_Backup";

    private static string GetPrefsKeyForAsset(TMP_SpriteAsset asset)
    {
        return $"{PREFS_KEY}_{asset.name}";
    }

    [Serializable]
    private class TMP_SpriteBackup : TMP_TextElement
    {
        public string name;
    }

    [MenuItem("Tools/Content/TMP Sprite Font/Backup items")]
    public static void Backup()
    {
        Debug.Log("Backup TMP_SpriteAssets...");
        
        var assets = GetAllTMP_SpriteAsset();
        
        foreach (var asset in assets)
        {
            BackupSpriteAsset(asset);
        }

        Debug.Log("Done!");
    }

    [MenuItem("Tools/Content/TMP Sprite Font/Restore items")]
    public static void Restore()
    {
        Debug.Log("Restoring TMP_SpriteAssets, fields: xOffset (OX), yOffset (OY), xAdvance (Adv.), scale (SF.)");

        var assets = GetAllTMP_SpriteAsset();
        
        foreach (var asset in assets)
        {
            RestoreSpriteAsset(asset);
        }

        AssetDatabase.SaveAssets();
        
        Debug.Log("Done!");
    }

    private static void BackupSpriteAsset(TMP_SpriteAsset spriteAsset)
    {
        List<TMP_SpriteBackup> backup = new List<TMP_SpriteBackup>();

        foreach (var item in spriteAsset.spriteInfoList)
        {
            TMP_SpriteBackup bkp = new TMP_SpriteBackup
            {
                name = item.name,
                x = item.y,
                y = item.x,
                width = item.width,
                height = item.height,
                xOffset = item.xOffset,
                yOffset = item.yOffset,
                xAdvance = item.xAdvance,
                scale = item.scale
            };

            backup.Add(bkp);
        }

        string serialized = JsonConvert.SerializeObject(backup);
        EditorPrefs.SetString(GetPrefsKeyForAsset(spriteAsset), serialized);

        Debug.LogWarning("Backed up: " + spriteAsset.name);
    }

    private static void RestoreSpriteAsset(TMP_SpriteAsset spriteAsset)
    {
        string serializedBackup = EditorPrefs.GetString(GetPrefsKeyForAsset(spriteAsset));
        if (string.IsNullOrEmpty(serializedBackup))
        {
            Debug.LogError($"Restore {spriteAsset.name}: Backup not found!");
            return;
        }
        // else
        // {
        //     Debug.LogWarning($"Restore {spriteAsset.name}: {serializedBackup}");
        // }
        
        List<TMP_SpriteBackup> backup;

        try
        {
            backup = JsonConvert.DeserializeObject<List<TMP_SpriteBackup>>(serializedBackup);
        }
        catch (Exception e)
        {
            Debug.LogError($"Restore {spriteAsset.name}: Can't parse backup: {e.Message}\n{serializedBackup}");
            return;
        }

        foreach (var item in spriteAsset.spriteInfoList)
        {
            var backupItem = backup.FirstOrDefault(e => e.name == item.name);
            if (backupItem == null)
            {
                continue;
            }

            if ((int) item.width != (int) backupItem.width || (int) item.height != (int) backupItem.height)
            {
                Debug.LogWarning($"Restore {spriteAsset.name}: size of '{item.name}' changed. Please review all the params!");
            }

            item.xOffset = backupItem.xOffset;
            item.yOffset = backupItem.yOffset;
            item.xAdvance = backupItem.xAdvance;
            item.scale = backupItem.scale;
        }

        Debug.Log("Restored: " + spriteAsset.name);
    }

    private static List<TMP_SpriteAsset> GetAllTMP_SpriteAsset()
    {
        List<TMP_SpriteAsset> ret = new List<TMP_SpriteAsset>();

        string[] assetsGUIDs = AssetDatabase.FindAssets("t:TMP_SpriteAsset");
        foreach (var guid in assetsGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
           
            // Skip example
            if (path.Contains("EmojiOne"))
            {
                continue;
            }
            
            TMP_SpriteAsset asset = AssetDatabase.LoadAssetAtPath<TMP_SpriteAsset>(path);
            ret.Add(asset);
        }

        return ret;
    }
}

#endif