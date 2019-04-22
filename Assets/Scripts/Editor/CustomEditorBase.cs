#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class CustomEditorBase:EditorWindow
{
    private Dictionary<object, Vector2> scrollValues;
    private Dictionary<object, bool> toggleGroupValues;
    
    protected void Header(string text)
    {
        GUILayout.Label(text, EditorStyles.boldLabel);
    }

    protected void ScrollArea([NotNull]object id, Action body)
    {
        if (scrollValues == null)
        {
            scrollValues = new Dictionary<object, Vector2>();
        }

        if (scrollValues.ContainsKey(id) == false)
        {
            scrollValues[id] = Vector2.zero;
        }
        
        scrollValues[id] = EditorGUILayout.BeginScrollView(scrollValues[id]);
        body?.Invoke();
        EditorGUILayout.EndScrollView();
    }

    protected void HorizontalArea(Action body)
    {   
        EditorGUILayout.BeginHorizontal();
        EditorGUI.indentLevel++;
        body?.Invoke();
        EditorGUI.indentLevel--;
        EditorGUILayout.EndHorizontal();
    }

    protected void VerticalArea(Action body)
    {
        EditorGUILayout.BeginVertical();
        EditorGUI.indentLevel++;
        body?.Invoke();
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    protected void BeginDisableArea(bool disable, Action body)
    {
        EditorGUI.BeginDisabledGroup(disable);
        body?.Invoke();
        EditorGUI.EndDisabledGroup();
    }

    protected void ToggleGroup([NotNull] object id, out bool currentStatus, string label, Action body)
    {
        if (toggleGroupValues == null)
        {
            toggleGroupValues = new Dictionary<object, bool>();
        }

        if (toggleGroupValues.ContainsKey(id) == false)
        {
            toggleGroupValues[id] = true;
        }

        toggleGroupValues[id] = EditorGUILayout.BeginToggleGroup(label, toggleGroupValues[id]);
        currentStatus = toggleGroupValues[id];
        EditorGUI.indentLevel++;
        body?.Invoke();
        EditorGUI.indentLevel--;
        EditorGUILayout.EndToggleGroup();
    }

    protected void ToggleGroup([NotNull] object id, string label, Action body)
    {
        ToggleGroup(id, out _, label, body);
    }

    protected void Button(string label, Action onclick)
    {
        if (GUILayout.Button(label))
        {
            onclick?.Invoke();
        }
    }

    protected void Separator()
    {
        var thickness = 2;
        var padding = 10;
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y+= padding / 2;
        r.x-=2;
        r.width +=6;
        EditorGUI.DrawRect(r, Color.gray);
    }
    
    protected static Transform FindDeepChild(Transform aParent, string aName)
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