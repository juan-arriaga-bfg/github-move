using UnityEngine;

public class PieceStorageView : MonoBehaviour
{
    [SerializeField] private Transform anchorView;
    
    private void Awake()
    {
        var context = GetComponent<PieceBoardElementView>();

        foreach (Transform view in anchorView)
        {
            DestroyImmediate(view.gameObject);
        }

        var go = (GameObject)Instantiate(ContentService.Instance.Manager.GetObjectByName(R.ChangeStorageStateView));
        var stateView = go.GetComponent<ChangeStorageStateView>();

        stateView.CachedTransform.SetParent(anchorView);
        stateView.CachedTransform.localPosition = Vector3.zero;
        stateView.CachedTransform.localRotation = Quaternion.identity;
        stateView.CachedTransform.localScale = Vector3.one;
        stateView.Init(context);
		
        context.ClearCacheLayers();
    }
}