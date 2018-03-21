using UnityEngine;

public class PieceStorageView : MonoBehaviour
{
    [SerializeField] private Transform anchorView;
    [SerializeField] private bool isShowTimer;
    [SerializeField] private Vector2 timerViewOfset;
    
    private void Awake()
    {
        var context = GetComponent<PieceBoardElementView>();

        foreach (Transform view in anchorView)
        {
            DestroyImmediate(view.gameObject);
        }
        
        var stateView = CreateObject<ChangeStorageStateView>(R.ChangeStorageStateView, Vector3.zero);
        stateView.Init(context);

        if (isShowTimer)
        {
            var timer = CreateObject<BoardTimerView>(R.BoardTimerView, timerViewOfset);
            timer.Init(context);
        }
		
        context.ClearCacheLayers();
    }

    private T CreateObject<T>(string prefab, Vector2 ofset) where T : IWBaseMonoBehaviour
    {
        var go = (GameObject)Instantiate(ContentService.Instance.Manager.GetObjectByName(prefab));
        var view = go.GetComponent<T>();

        view.CachedTransform.SetParent(anchorView);
        view.CachedTransform.localPosition = ofset;
        view.CachedTransform.localRotation = Quaternion.identity;
        view.CachedTransform.localScale = Vector3.one;
        
        return view;
    }
}