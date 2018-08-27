using UnityEngine;

public class BuildingPieceView : PieceBoardElementView
{
	[SerializeField] private Material lockedMaterial;
    
	private Material unlockedMaterial;
    
	private PieceStateComponent state;
	private GameObject warning;
    
	public override void Init(BoardRenderer context, Piece piece)
	{
		base.Init(context, piece);

		state = Piece.GetComponent<PieceStateComponent>(PieceStateComponent.ComponentGuid);
        
		if(state == null) return;
        
		state.OnChangeState += UpdateSate;

		if (sprite != null) unlockedMaterial = sprite.material;

		if (warning == null)
		{
			var icon = Context.CreateElement((int) ViewType.Warning);
			
			icon.SetParent(sprite.transform.parent, false);
			warning = icon.gameObject;
			
			AddLayerToCache(icon.GetComponentInChildren<Renderer>());
			SyncRendererLayers(piece.CachedPosition);
		}
		
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
	}
}