using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceObstacleStateView : MonoBehaviour
{
    [SerializeField] private Transform anchorView;
    
    private void Awake()
    {
        var context = GetComponent<PieceBoardElementView>();

        foreach (Transform view in anchorView)
        {
            DestroyImmediate(view.gameObject);
        }

        var go = (GameObject)Instantiate(ContentService.Instance.Manager.GetObjectByName(R.ChangeObstacleStateView));
        var stateview = go.GetComponent<ChangeObstacleStateView>();

        stateview.CachedTransform.SetParent(anchorView);
        stateview.CachedTransform.localPosition = Vector3.zero;
        stateview.CachedTransform.localRotation = Quaternion.identity;
        stateview.CachedTransform.localScale = Vector3.one;
        stateview.Init(context);
		
        context.ClearCacheLayers();
    }
}