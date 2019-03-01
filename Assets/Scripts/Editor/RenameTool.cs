#if UNITY_EDITOR

using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RenameTool: EditorWindow
{
    private string sourceFormat = "";
    private string destFormat = "";
    private string targetObjectName = "";
    private string spriteFolderPath = "";
    private string fileNameSelector = "";
    private ComponentType componentType = ComponentType.SpriteRenderer;
    private bool recursionSearch = false;
    private bool useFileName = false;

    private enum ComponentType
    {
        Image,
        SpriteRenderer
    }

    [MenuItem("Window/Utils/RenameTool")]
    public static void Create()
    {
        RenameTool renameTool = GetWindow(typeof(RenameTool)) as RenameTool;
        renameTool.Show();
    }

    protected virtual void OnGUI()
    {
        GUILayout.Label("Base Replace", EditorStyles.boldLabel);
        DrawSourceReplaceField();
        DrawDestReplaceField();
        recursionSearch = EditorGUILayout.Toggle("Recursion", recursionSearch);
        DrawReplaceButton();
        
        GUILayout.Label("Prefab Replace", EditorStyles.boldLabel);
        DrawTargetObjectField();
        DrawComponentCombobox();
        
        useFileName = EditorGUILayout.BeginToggleGroup("Use file name", useFileName);
        DrawFileNameSelectorField();
        EditorGUILayout.EndToggleGroup();
        
        SelectFolderButton();
        DrawPrefabReplaceButton();
    }

    private void DrawSourceReplaceField()
    {
        sourceFormat = EditorGUILayout.TextField("From (regex)", sourceFormat);
    }
    
    private void DrawDestReplaceField()
    {
        destFormat = EditorGUILayout.TextField("To (regex)", destFormat);
    }
    
    private void DrawFileNameSelectorField()
    {
        fileNameSelector = EditorGUILayout.TextField("File name selector (regex)", fileNameSelector);
    }

    private void DrawReplaceButton()
    {
        if (GUILayout.Button("Replace"))
        {
            var path = "";
            var obj = Selection.activeObject;
            
            if (obj == null || string.IsNullOrEmpty(sourceFormat) || string.IsNullOrEmpty(destFormat)) return;
            
            path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if (Directory.Exists(path))
            {
                //for folder
                ReplaceInFolder(path);
            }
            else if(File.Exists(path))
            {
                //for file
                ReplaceForFile(path);
            }
            
            AssetDatabase.Refresh();
        }
    }
    
    private void DrawTargetObjectField()
    {
        targetObjectName = EditorGUILayout.TextField("Target name:", targetObjectName);
    }

    private void DrawComponentCombobox()
    {
        string[] options = new string[]
        {
            ComponentType.Image.ToString(), ComponentType.SpriteRenderer.ToString() 
        };
        componentType = (ComponentType)EditorGUILayout.Popup("ComponentType", (int)componentType, options);
    }

    private void DrawPrefabReplaceButton()
    {
        if (GUILayout.Button("Replace"))
        {
            var path = "";
            var obj = Selection.activeObject;
            
            if (obj == null || string.IsNullOrEmpty(sourceFormat) || string.IsNullOrEmpty(destFormat) || string.IsNullOrEmpty(targetObjectName)) return;
            
            path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if (Directory.Exists(path))
            {
                //for folder
                ReplacePrefabsInFolder(path);
            }
            else if(File.Exists(path))
            {
                //for file
                ReplacePrefabForFile(path);
            }

        }
    }

    private void ReplaceInFolder(string folderPath)
    {
        var files = Directory.GetFiles(folderPath);
        foreach (var file in files)
        {
            ReplaceForFile(file);
        }

        if (recursionSearch)
        {
            var directories = Directory.GetDirectories(folderPath);
            foreach (var directory in directories)
            {
                ReplaceInFolder(directory);
            }
        }
    }

    private void ReplaceForFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        if (Regex.IsMatch(fileName, sourceFormat))
        {
            var directory = Directory.GetParent(filePath).FullName;
            var newName = Regex.Replace(fileName, sourceFormat, destFormat);
            var resultPath = Path.Combine(directory, newName);
            File.Move(filePath, resultPath);
        }
    }
    
    private void ReplacePrefabsInFolder(string folderPath)
    {
        var files = Directory.GetFiles(folderPath);
        foreach (var file in files)
        {
            if (ReplacePrefabForFile(file) == false) return;
        }

        if (recursionSearch)
        {
            var directories = Directory.GetDirectories(folderPath);
            foreach (var directory in directories)
            {
                ReplacePrefabsInFolder(directory);
            }
        }
    }

    private bool ReplacePrefabForFile(string filePath)
    {
        var ext = Path.GetExtension(filePath);
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        if (ext != ".prefab") return true;
        
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
        var target = FindDeepChild(asset.transform, targetObjectName);
        
        if (target == null || target.gameObject == null) return true;
        
        var targetObj = target.gameObject;
        
        if (componentType == ComponentType.Image)
        {
            var component = targetObj.GetComponent<Image>();
            var spriteName = useFileName ? fileName : component.sprite.name;
            
            if (useFileName && string.IsNullOrEmpty(fileNameSelector) == false)
            {
                spriteName = Regex.Match(spriteName ?? "", fileNameSelector).Value;
            }

            if (spriteName == null) return true;
            if (Regex.IsMatch(spriteName, sourceFormat) == false) return true;
            
            string resultPath;
            var interrupt = GetNewSpritePath(spriteName, out resultPath);
            
            if (interrupt == false) return false;
            if (interrupt == null) return true;
            
            resultPath = resultPath.Replace("\\", "/");
            resultPath = Regex.Match(resultPath, @"Assets/.*").Value;
            
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(resultPath);
            component.sprite = sprite;
        }
        else if (componentType == ComponentType.SpriteRenderer)
        {
            var component = targetObj.GetComponent<SpriteRenderer>();
            var spriteName = useFileName ? fileName : component.sprite.name;
            if (useFileName && string.IsNullOrEmpty(fileNameSelector) == false)
            {
                spriteName = Regex.Match(spriteName, fileNameSelector).Value;
            }
            
            if (Regex.IsMatch(spriteName, sourceFormat) == false) return true;
            
            string resultPath;
            var interrupt = GetNewSpritePath(spriteName, out resultPath);
            
            if (interrupt == false) return false;
            if (interrupt == null) return true;
            
            resultPath = resultPath.Replace("\\", "/");
            resultPath = Regex.Match(resultPath, @"Assets/.*").Value;
            
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(resultPath);
            component.sprite = sprite;
        }
        PrefabUtility.SavePrefabAsset(asset);
        AssetDatabase.SaveAssets();
        return true;
    }

    private void SelectFolderButton()
    {
        if (GUILayout.Button("Select sprite folder") == false) return;
        spriteFolderPath = EditorUtility.OpenFolderPanel("Select folder with sprites", "", "");
    }
    
    private static Transform FindDeepChild(Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach(Transform child in aParent)
        {
            result = FindDeepChild(child, aName);
            if (result != null)
                return result;
        }
        return null;
    }

    private bool? GetNewSpritePath(string spriteName, out string path)
    {
        path = null;
        int fileCount = 0;
        while (fileCount != 1)
        {
            var targetName = Regex.Replace(spriteName, sourceFormat, destFormat);
            var files = Directory.GetFiles(spriteFolderPath, $"{targetName}.png", SearchOption.AllDirectories);
            fileCount = files.Length;
            if (fileCount > 1)
            {
                var userSelect = EditorUtility.DisplayDialog("Select specific folder",
                    "Found many sprites by template. Select more specific folder", "select", "interrupt");
            
                if (userSelect == false) return false;
                spriteFolderPath = EditorUtility.OpenFolderPanel("Select folder with sprites", "", "");
            }
            else if(fileCount == 1)
            {
                var filePath = files[0];
                path = filePath;
            }
            else
            {
                Debug.LogError($"Sprite with name {targetName} not found");
                return null;
            }
        }

        return true;
    }
}

#endif