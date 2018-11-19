using System.Linq;
using UnityEngine;

public class BuildingPieceView : PieceBoardElementView
{
	[SerializeField] protected Material lockedMaterial;
    
	protected Material unlockedMaterial;
    
	private PieceStateComponent state;
	
	private GameObject warning;
    
	public override void Init(BoardRenderer context, Piece piece)
	{
		base.Init(context, piece);

		state = Piece.GetComponent<PieceStateComponent>(PieceStateComponent.ComponentGuid);

		if (bodySprites != null && bodySprites.Count > 0)
		{
			unlockedMaterial = bodySprites.First().material;
			bodySprites.ForEach(sprite => sprite.material = state == null ? unlockedMaterial : lockedMaterial);
		}
		
		if(state == null) return;
        
		state.OnChangeState += UpdateSate;
		
		if (warning == null) warning = CreateUi(ViewType.Warning);
		
		SyncRendererLayers(piece.CachedPosition);
		
		UpdateSate();
	}

	public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();
		
		bodySprites.ForEach(sprite => sprite.material = unlockedMaterial);
		
		if(state == null) return;
        
		state.OnChangeState -= UpdateSate;
	}
    
	private void UpdateSate()
	{
		if(state == null) return;
		
		warning.SetActive(state.State == BuildingState.Warning);
	}

	private GameObject CreateUi(ViewType view)
	{
		var go = Context.CreateElement((int) view);
		
		go.SetParent(bodySprites.First().transform, false);
		AddToLayerCache(go.gameObject);
		
		return go.gameObject;
	}
}