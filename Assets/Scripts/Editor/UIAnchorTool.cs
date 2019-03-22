#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class UIAnchorTool: EditorWindow
{
    private ViewType targetView = ViewType.None;
    private bool isRecursion;
    private float offsetY = -1.8f;
    
    [MenuItem("Window/Utils/UIAnchorToolWindow")]
    public static void Create()
    {
        var anchorTool = GetWindow(typeof(UIAnchorTool)) as UIAnchorTool;
        anchorTool.Show();
    }
    
    protected virtual void OnGUI()
    {
        DrawComponentCombobox();
        DrawRecursionToggle();
        DrawOffsetField();
        DrawExecuteButton();
    }
    
    private void DrawComponentCombobox()
    {
        string[] options = Enum.GetNames(typeof(ViewType));
        
        ViewType.TryParse(options[EditorGUILayout.Popup("ViewType", Array.IndexOf(options, Enum.GetName(typeof(ViewType), targetView)), options)], out targetView);
    }

    private void DrawOffsetField()
    {
        offsetY = EditorGUILayout.FloatField("Offset Y", offsetY);
    }

    private void DrawRecursionToggle()
    {
        isRecursion = GUILayout.Toggle(isRecursion, "Recursion");
    }

    private void DrawExecuteButton()
    {
        if (GUILayout.Button("Execute"))
        {
            var obj = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if (Directory.Exists(path))
            {
                RecursionAddAnchor(path);    
            }
            else if(File.Exists(path))
            {
                ChangeAnchorForFile(path);
            }
        }
    }

    private void RecursionAddAnchor(string folderPath)
    {
        var files = Directory.GetFiles(folderPath);
        foreach (var file in files)
        {
            ChangeAnchorForFile(file);
        }

        if (isRecursion)
        {
            var directories = Directory.GetDirectories(folderPath);
            foreach (var directory in directories)
            {
                RecursionAddAnchor(directory);
            }
        }
    }

    private void ChangeAnchorForFile(string filePath)
    {
        
        if (Path.GetExtension(filePath) == ".prefab")
        {
            var asset = PrefabUtility.LoadPrefabContents(filePath);
            try
            {
                ChangeAnchorForPrefab(asset, filePath);
            }
            catch (Exception e)
            {
                IW.Logger.LogError(e);
            }
            
        }
    }

    private void ChangeAnchorForPrefab(GameObject prefab, string path)
    {
        string rootName = "Anchors";
        var anchorRoot = prefab.transform.Find(rootName)?.gameObject;
        if (anchorRoot == null)
        {
            anchorRoot = new GameObject();
            anchorRoot.transform.SetParent(prefab.transform);
            anchorRoot.transform.localPosition = Vector3.zero;
            anchorRoot.name = rootName;
        }

        var sprite = FindDeepChild(prefab.transform, "sprite");
        if (sprite == null)
        {
            return;
        }

        var anchorName = $"{Enum.GetName(typeof(ViewType), targetView)}Anchor";
        GameObject anchor = anchorRoot.transform.Find(anchorName)?.gameObject;
        if (anchor == null)
        {
            anchor = new GameObject();
            anchor.transform.parent = anchorRoot.transform;
            anchor.transform.localPosition = Vector3.zero;
            anchor.name = anchorName;
        }        
        
        var renderer = sprite.GetComponent<Renderer>();
        var size = renderer.bounds.size;

        var spriteOffset = size.y / 2;
        if (size.y > size.x)
        {
            spriteOffset *= size.x / size.y * 0.8f;
        }
        var offsetBubble = offsetY;
        
        var newPosition = anchor.transform.position;
        newPosition = new Vector3(newPosition.x, sprite.position.y, newPosition.z);
        newPosition.y += spriteOffset;
        newPosition.y += offsetBubble;
        anchor.transform.position = newPosition;

        var pieceView = prefab.GetComponent<PieceBoardElementView>();
        ViewAnchorLink viewAnchorLink = pieceView.Anchors.Find(elem => elem.Key == targetView);
        if (viewAnchorLink == null)
        {
            viewAnchorLink = new ViewAnchorLink();
            viewAnchorLink.Key = targetView;
            viewAnchorLink.Anchor = anchor.transform;
        }

        pieceView.Anchors.RemoveAll(elem => elem.Key == targetView);
        pieceView.Anchors.Add(viewAnchorLink);
        
        PrefabUtility.SaveAsPrefabAsset(prefab, path);
    }
    
    public static Transform FindDeepChild(Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach(Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }
}
#endif