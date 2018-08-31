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
        
		if(state == null) return;
        
		state.OnChangeState += UpdateSate;

		if (sprite != null) unlockedMaterial = sprite.material;

		if (warning == null) warning = CreateUi(ViewType.Warning);
		if (hourglass == null) hourglass = CreateUi(ViewType.Hourglass);
		
		SyncRendererLayers(piece.CachedPosition);
		
		UpdateSate();
	}

	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();
		
		if(state == null) return;
        
		state.OnChangeState -= UpdateSate;
	}
    
	private void UpdateSate()
	{
		if(state == null || sprite == null) return;

		sprite.material = state.State == BuildingState.Complete ? unlockedMaterial : lockedMaterial;
		warning.SetActive(state.State == BuildingState.Warning);
		hourglass.SetActive(state.State == BuildingState.InProgress);
	}

	private GameObject CreateUi(ViewType view)
	{
		var go = Context.CreateElement((int) view);
		var renderers = go.GetComponentsInChildren<Renderer>();
		
		go.SetParent(sprite.transform.parent, false);

		foreach (var items in renderers)
		{
			AddLayerToCache(items);
		}
		
		return go.gameObject;
	}
}