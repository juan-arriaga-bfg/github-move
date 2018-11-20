#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Transform))]
class TransformResetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var transform = target as Transform;
        
        var position = Controll("Position", transform, transform.localPosition, Vector3.zero);
        var eulerAngles = Controll("Rotation", transform, transform.localEulerAngles, Vector3.zero);
        var scale = Controll("Scale", transform, transform.localScale, Vector3.one);

        if (!GUI.changed) return;
        
        Undo.RegisterCompleteObjectUndo(transform, "Transform Change");
        
        transform.localPosition = FixIfNaN(position);
        transform.localEulerAngles = FixIfNaN(eulerAngles);
        transform.localScale = FixIfNaN(scale);
    }

    private Vector3 Controll(string label, Transform transform, Vector3 value, Vector3 def)
    {
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Reset", GUILayout.Width(50)))
        {
            Undo.RegisterCompleteObjectUndo(transform, "Reset" + transform.name);
            value = def;
        }

        EditorGUILayout.LabelField(label, GUILayout.Width(50));
        
        value = EditorGUILayout.Vector3Field("", value);
        
        EditorGUILayout.EndHorizontal();
        
        return value;
    }
    
    private Vector3 FixIfNaN(Vector3 v)
    {
        if(float.IsNaN(v.x)) v.x = 0;

        if (float.IsNaN(v.y)) v.y = 0;

        if (float.IsNaN(v.z)) v.z = 0;
        
        return v;
    }
}
#endif
