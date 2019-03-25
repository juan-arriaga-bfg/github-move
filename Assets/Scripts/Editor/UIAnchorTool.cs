#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class UIAnchorTool: CustomEditorBase
{
    private ViewType targetView = ViewType.None;
    
    private float offsetY = 0f;
    private float ratioCoefficient = 0.8f;
    
    private bool isRecursion;
    private bool useSpriteHeight;
    private bool useProportionalOffset;
    
    private string targetSpriteObject = "sprite";
    
    [MenuItem("Window/Utils/UIAnchorTool")]
    public static void Create()
    {
        var anchorTool = GetWindow(typeof(UIAnchorTool)) as UIAnchorTool;
        anchorTool.Show();
    }
    
    protected virtual void OnGUI()
    {
        ScrollArea(this, () =>
        {
            Header("Base");
            ComboboxTargetViewType();
            FieldOffsetY();
        
            Header("Options");
            ToggleRecursion();
            ToggleGroup(this, out useSpriteHeight, "Use sprite height", () =>
            {
                ToggleProportionalOffset();
                FieldTargetSpriteObject();
                FieldProportionalRatio();
            });

            Separator();
            Button("Execute", OnExecuteClick);
        });
    }

#region WindowUI
    private void FieldTargetSpriteObject()
    {
        targetSpriteObject = EditorGUILayout.TextField("Target sprite object", targetSpriteObject);
    }

    private void ComboboxTargetViewType()
    {
        targetView = (ViewType)EditorGUILayout.EnumPopup("ViewType", targetView);
    }

    private void ToggleRecursion()
    {
        isRecursion = EditorGUILayout.ToggleLeft("Recursion", isRecursion);
    }

    private void ToggleProportionalOffset()
    {
        useProportionalOffset = EditorGUILayout.ToggleLeft("Use proportional offset", useProportionalOffset);
    }
    
    private void FieldOffsetY()
    {
        offsetY = EditorGUILayout.FloatField("Offset Y", offsetY);
    }
    
    private void FieldProportionalRatio()
    {
        ratioCoefficient = EditorGUILayout.FloatField("Ratio coefficient", ratioCoefficient);
    }
#endregion

#region WindowLogic
    private void OnExecuteClick()
    {
        if (targetView == ViewType.None)
        {
            IW.Logger.LogError($"[UIAnchorTool] => Target view not selected");
            return;
        }
        var obj = Selection.activeObject;
        var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
    
        if (Directory.Exists(path))
        {
            RecursionAddAnchors(path);    
        }
        else if(File.Exists(path))
        {
            ChangeAnchorForFile(path);
        }
    }
    
    private void RecursionAddAnchors(string folderPath)
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
                RecursionAddAnchors(directory);
            }
        }
    }

    private void ChangeAnchorForFile(string filePath)
    {
        if (Path.GetExtension(filePath) == ".prefab")
        {
            var asset = PrefabUtility.LoadPrefabContents(filePath);
            ChangeAnchorForPrefab(asset, filePath);
            PrefabUtility.UnloadPrefabContents(asset);
        }
    }
    
    private void ChangeAnchorForPrefab(GameObject prefab, string path)
    {
        string rootName = "Anchors";
        var anchorRoot = FindOrCreateAnchorRoot(prefab, rootName);
        
        var anchorName = $"{Enum.GetName(typeof(ViewType), targetView)}Anchor";
        var anchor = FindOrCreateAnchor(anchorRoot, anchorName);

        var spriteOffset = GetSpriteOffset(prefab);
        var offsetBubble = offsetY;
        
        var newPosition = anchor.transform.localPosition;
        newPosition = new Vector3(newPosition.x, spriteOffset, newPosition.z);
        newPosition.y += offsetBubble;
        anchor.transform.localPosition = newPosition;

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

    private GameObject FindOrCreateAnchorRoot(GameObject prefab, string anchorRootName)
    {
        var anchorRoot = prefab.transform.Find(anchorRootName)?.gameObject;
        if (anchorRoot == null)
        {
            anchorRoot = new GameObject(anchorRootName);
            anchorRoot.transform.SetParent(prefab.transform);
            anchorRoot.transform.localPosition = Vector3.zero;
        }

        return anchorRoot;
    }

    private GameObject FindOrCreateAnchor(GameObject anchorRoot, string anchorName)
    {
        GameObject anchor = anchorRoot.transform.Find(anchorName)?.gameObject;
        if (anchor == null)
        {
            anchor = new GameObject(anchorName);
            anchor.transform.parent = anchorRoot.transform;
            anchor.transform.localPosition = Vector3.zero;
        }

        return anchor;
    }

    private float GetSpriteOffset(GameObject prefab)
    {
        if (useSpriteHeight == false)
        {
            return 0f;
        }
        
        var sprite = FindDeepChild(prefab.transform, string.IsNullOrEmpty(targetSpriteObject) ? "sprite" : targetSpriteObject);
        if (sprite == null)
        {
            return 0f;
        }
        
        var renderer = sprite.GetComponent<Renderer>();
        var size = renderer.bounds.size;

        var spriteOffset = size.y / 2;
        if (useProportionalOffset && size.y > size.x)
        {
            spriteOffset *= size.x / size.y * ratioCoefficient;
        }

        spriteOffset += sprite.localPosition.y;

        return spriteOffset;
    }
#endregion
    
}
#endif