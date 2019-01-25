using System;
using System.Collections.Generic;
using UnityEngine;

public class KillThemAll : MonoBehaviour
{
    private void Start()
    {
        //if (KillAllGos()) return;

        Resources.UnloadUnusedAssets();
        GC.Collect();
        
        PrintStats();
    }

    private bool KillAllGos()
    {
        List<GameObject> toSaveGos = new List<GameObject>();
        gameObject.scene.GetRootGameObjects(toSaveGos);

        HashSet<Transform> toSaveTransforms = new HashSet<Transform>();
        foreach (var go in toSaveGos)
        {
            toSaveTransforms.Add(go.transform);
        }

        var allTransforms = FindObjectsOfType<Transform>();

        foreach (var t in allTransforms)
        {
            if (toSaveTransforms.Contains(t))
            {
                continue;
            }

            if (t.parent != null)
            {
                return true;
            }

            Destroy(t.gameObject);
        }

        return false;
    }

    private void PrintStats()
    {

    }
}
