using UnityEngine;

public class TouchReactonConditionDelayView : MonoBehaviour
{
	[SerializeField] private Transform anchorView;
	
	[SerializeField] private Vector3 arrowOffset;
	
	private PieceBoardElementView context;

	private ResourceGenerationTimerView resourceGenerationTimerView;

	private bool isAwakedCalled = false;

	private void Awake()
	{
		context = GetComponent<PieceBoardElementView>();

		foreach (Transform view in anchorView)
		{
			DestroyImmediate(view.gameObject);
		}

		var go = (GameObject)Instantiate(ContentService.Instance.Manager.GetObjectByName(R.ResourceGenerationTimerView));
		resourceGenerationTimerView = go.GetComponent<ResourceGenerationTimerView>();

		resourceGenerationTimerView.CachedTransform.SetParent(anchorView);
		resourceGenerationTimerView.CachedTransform.localPosition = Vector3.zero;
		resourceGenerationTimerView.CachedTransform.localRotation = Quaternion.identity;
		resourceGenerationTimerView.CachedTransform.localScale = Vector3.one;
		
		resourceGenerationTimerView.Init(context, arrowOffset);
		
		context.ClearCacheLayers();

		
	}


}
