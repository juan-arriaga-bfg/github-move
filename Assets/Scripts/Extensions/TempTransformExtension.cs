using UnityEngine;

public static class TempTransformExtension
{
    public static void SetLayerRecursive(this Transform view, int targetLayer)
    {
        Transform[] includedObjects = view.GetComponentsInChildren<Transform>(true);
        foreach (Transform includedObject in includedObjects)
        {
            includedObject.gameObject.layer = targetLayer;
        }
        view.gameObject.layer = targetLayer;
    }
}