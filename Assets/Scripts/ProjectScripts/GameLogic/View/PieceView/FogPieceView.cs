using System.Collections.Generic;
using UnityEngine;

public class FogPieceView : PieceBoardElementView
{
	[SerializeField] private GameObject fogItem;
	[SerializeField] private GameObject touchItem;
	
	[SerializeField] private TouchRegion touchRegion;
	
	private List<GameObject> views = new List<GameObject>();
	private FogObserver observer;
	
	public override void Init(BoardRenderer context, Piece piece)
	{
		base.Init(context, piece);

		observer = piece.GetComponent<FogObserver>(FogObserver.ComponentGuid);
		
		if(observer == null) return;
		
		foreach (var position in observer.Mask)
		{
			var fog = Instantiate(fogItem, fogItem.transform.parent);
			var touch = Instantiate(touchItem, touchItem.transform.parent);
			var sprite = fog.GetComponent<SpriteRenderer>();
			
			sprite.sortingOrder = position.X * Context.Context.BoardDef.Width - position.Y + 101;
			
			fog.transform.position = touch.transform.position = piece.Context.BoardDef.GetSectorCenterWorldPosition(position.X, position.Y, 0);
			
			touchRegion.AddTouchRegion(touch.GetComponent<RectTransform>());
			
			views.Add(fog);
			views.Add(touch);
		}
		
		
		
		fogItem.SetActive(false);
		touchItem.SetActive(false);
	}

	public override void SyncRendererLayers(BoardPosition boardPosition)
	{
		CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, CachedTransform.localPosition.y, -boardPosition.Z * 0.1f);
	}

	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();

		foreach (var view in views)
		{
			Destroy(view);
		}
		
		views = new List<GameObject>();
		
		touchRegion.ClearTouchRegion();
		
		fogItem.SetActive(true);
		touchItem.SetActive(true);
		
		observer.Clear();
	}
}