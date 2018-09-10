using UnityEngine;

public class BuildingPieceView : PieceBoardElementView
{
	[SerializeField] private Material lockedMaterial;
    
	private Material unlockedMaterial;
    
	private PieceStateComponent state;
	
	private GameObject warning;
	private GameObject hourglass;
    
	public override void Init(BoardRenderer context, Piece piece)
	{
		base.Init(context, piece);

		state = Piece.GetComponent<PieceStateComponent>(PieceStateComponent.ComponentGuid);

		if (sprite != null)
		{
			unlockedMaterial = sprite.material;
			sprite.material = state == null ? unlockedMaterial : lockedMaterial;
		}
		
		if(state == null) return;
        
		state.OnChangeState += UpdateSate;
		
		if (warning == null) warning = CreateUi(ViewType.Warning);
		if (hourglass == null) hourglass = CreateUi(ViewType.Hourglass);
		
		SyncRendererLayers(piece.CachedPosition);
		
		UpdateSate();
	}

	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();
		
		sprite.material = unlockedMaterial;
		
		if(state == null) return;
        
		state.OnChangeState -= UpdateSate;
	}
    
	private void UpdateSate()
	{
		if(state == null) return;
		
		warning.SetActive(state.State == BuildingState.Warning);
		hourglass.SetActive(state.State == BuildingState.InProgress);
	}

	private GameObject CreateUi(ViewType view)
	{
		var go = Context.CreateElement((int) view);
		var renderers = go.GetComponentsInChildren<Renderer>();
		
		go.SetParent(sprite.transform, false);

		foreach (var items in renderers)
		{
			AddLayerToCache(items);
		}
		
		return go.gameObject;
	}
}