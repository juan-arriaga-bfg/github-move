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
     
     [Serializable]
     private class TMP_SpriteBackup: TMP_TextElement
     {
          public string name; 
     }

     private static TMP_SpriteAsset LoadSettings()
     {
          if (TMP_Settings.defaultSpriteAsset == null)
          {
               TMP_Settings.LoadDefaultSettings();
          }
          
          var spriteAsset = TMP_Settings.defaultSpriteAsset;

          if (spriteAsset == null)
          {
               Debug.LogError("Can't open TMP_Settings. Please Start and Stop the game in the Editor to reload."); 
          }
          
          return spriteAsset;
     }

     [MenuItem("Tools/Content/TMP Sprite Font/Backup items")]
     public static void Backup()
     {
          // Load Default SpriteAsset
          var spriteAsset = LoadSettings();
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
          EditorPrefs.SetString(PREFS_KEY, serialized);
          
          Debug.LogWarning("Backed up!");
     }
     
     [MenuItem("Tools/Content/TMP Sprite Font/Restore items")]
     public static void Restore()
     {
          string serializedBackup = EditorPrefs.GetString(PREFS_KEY);
          if (string.IsNullOrEmpty(serializedBackup))
          {
               Debug.LogError("No backup found!");
               return;
          }

          List<TMP_SpriteBackup> backup = JsonConvert.DeserializeObject<List<TMP_SpriteBackup>>(serializedBackup);

          // Load Default SpriteAsset
          var spriteAsset = LoadSettings();

          foreach (var item in spriteAsset.spriteInfoList)
          {
               var backupItem = backup.FirstOrDefault(e => e.name == item.name);
               if (backupItem == null)
               {
                    continue;
               }

               if ((int) item.width != (int) backupItem.width || (int) item.height != (int) backupItem.height)
               {
                    Debug.LogWarning($"Restore: size of '{item.name}' changed. Please review all the params!");
               }
               
               item.xOffset = backupItem.xOffset;
               item.yOffset = backupItem.yOffset;
               item.xAdvance = backupItem.xAdvance;
               item.scale = backupItem.scale;
          }
          
          Debug.LogWarning("Restored!");
     }
}

#endif
