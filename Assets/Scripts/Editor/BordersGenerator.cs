using System.Collections.Generic;
using System.IO;
using BestHTTP.Extensions;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class BordersGenerator : MonoBehaviour
{
      
}

#if UNITY_EDITOR

[CustomEditor(typeof(BordersGenerator))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        BordersGenerator script = (BordersGenerator)target;
        if(GUILayout.Button("Generate!"))
        {
            Generate();
        }
    }

    private void Generate()
    {
        const string TARGET = "BordersGenerator";

        string generatorPath = null;
        
        var guids = AssetDatabase.FindAssets("t:prefab");
        for (int i = 0; i < guids.Length; i++)
        {
            var guid = guids[i];
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains(TARGET))
            {
                generatorPath = path.Replace("\\", "/");
                break;
            }
        }
        
        HashSet<string> immortalFiles = new HashSet<string>
        {
            TARGET
        };
        
        string dir = Path.GetDirectoryName(generatorPath);
        string[] files = Directory.GetFiles(dir, "*.prefab", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            foreach (var immortal in immortalFiles)
            {
                if (file.Contains(immortal))
                {
                    goto NEXT;
                }
            }
            
            File.Delete(file);
            
            NEXT: ;
        }
        
        AssetDatabase.Refresh();
        
       
        HashSet<string> doNotClone = new HashSet<string>
        {
            "Helpers"
        };
        
        List<string> itemsToCreate = new List<string>();
        
        GameObject go = PrefabUtility.LoadPrefabContents(generatorPath);
        
        // Find childs
        int count = go.transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            Transform item = go.transform.GetChild(i);
            if (!doNotClone.Contains(item.name))
            {
                itemsToCreate.Add(item.name);
            }
        }
        
        // Clone items
        foreach (string itemName in itemsToCreate)
        {
            var src = generatorPath;
            var dst = $"{dir}/{itemName}.prefab";
            File.Copy(src, dst);
            
            AssetDatabase.ImportAsset(src);
            
            // Fix item
            GameObject newGo = PrefabUtility.LoadPrefabContents(dst);
            count = newGo.transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                Transform t = newGo.transform.GetChild(i);
                if (t.name != itemName)
                {
                    DestroyImmediate(t.gameObject);
                }
                else
                {
                    t.gameObject.SetActive(true);
                }
            }

            PrefabUtility.SaveAsPrefabAsset(newGo, dst);
        }
        
        AssetDatabase.Refresh();
        
        Debug.LogWarning("Done!");
    }
}

#endif
