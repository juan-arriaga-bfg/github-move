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

		var size = GameDataService.Current.FogsManager
			.GetDef(new BoardPosition(piece.CachedPosition.X, piece.CachedPosition.Y)).Size.Y;
		
		foreach (var position in observer.Mask)
		{
			var fog = Instantiate(fogItem, fogItem.transform.parent);
			var touch = Instantiate(touchItem, touchItem.transform.parent);
			var pos = observer.GetPointInMask(piece.CachedPosition, position);
			var sprite = fog.GetComponent<SpriteRenderer>();

			sprite.sortingOrder = pos.X * Context.Context.BoardDef.Width - pos.Y + pos.Z * 100 + (pos.X + size) - pos.Y;
			
			fog.transform.localPosition = touch.transform.localPosition = piece.Context.BoardDef.GetSectorCenterWorldPosition(position.X, position.Y, 0);
			
			touchRegion.AddTouchRegion(touch.GetComponent<RectTransform>());
			
			views.Add(fog);
			views.Add(touch);
		}
		
		fogItem.SetActive(false);
		touchItem.SetActive(false);
		observer.UpdateResource(0);
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