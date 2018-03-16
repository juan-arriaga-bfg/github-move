using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivePieceComponentView : MonoBehaviour {

	[SerializeField] private Transform anchorView;
	
	private PieceBoardElementView context;

	private HitboxView hitboxView;

	private void Awake()
	{
		context = GetComponent<PieceBoardElementView>();

		foreach (Transform view in anchorView)
		{
			DestroyImmediate(view.gameObject);
		}

		var go = (GameObject)Instantiate(ContentService.Instance.Manager.GetObjectByName(R.HitboxView));
		hitboxView = go.GetComponent<HitboxView>();

		hitboxView.CachedTransform.SetParent(anchorView);
		hitboxView.CachedTransform.localPosition = Vector3.zero;
		hitboxView.CachedTransform.localRotation = Quaternion.identity;
		hitboxView.CachedTransform.localScale = Vector3.one;
		
		hitboxView.Init(context);
		
		context.ClearCacheLayers();

		
	}
}
