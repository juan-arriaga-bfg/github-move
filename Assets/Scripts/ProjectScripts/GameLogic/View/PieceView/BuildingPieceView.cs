using System.Linq;
using UnityEngine;

public class BuildingPieceView : PieceBoardElementView
{
	protected PieceStateComponent state;
	
	private GameObject warning;
    
	public override void Init(BoardRenderer context, Piece piece)
	{
		base.Init(context, piece);

		state = Piece.GetComponent<PieceStateComponent>(PieceStateComponent.ComponentGuid);

	    ToggleLockedState(state != null);
	    
	    SyncRendererLayers(piece.CachedPosition);
		
		if(state == null) return;
        
		state.OnChangeState += UpdateState;
		
		if (warning == null) warning = CreateUi(ViewType.Warning);
	    
	    SyncRendererLayers(piece.CachedPosition);
		
		UpdateState();
	}

    public virtual void ToggleLockedState(bool isLocked)
    {
        if (isLockVisual) return;
        
        if (isLocked)
        {
            SetCustomMaterial(BoardElementMaterialType.PiecesLockedMaterial, true);
            SaveCurrentMaterialAsDefault();
        }
        else
        {
            ClearCurrentMaterialAsDefault();
            ResetDefaultMaterial();
        }
    }

    public override void ToggleLockView(bool enabled)
    {
        base.ToggleLockView(enabled);

        if (enabled == false)
        {
            ToggleLockedState(state != null);
        }
    }

    public override void ResetViewOnDestroy()
	{
		base.ResetViewOnDestroy();
	    
	    DestroyUi(warning);
	    warning = null;
		
		if(state == null) return;
        
		state.OnChangeState -= UpdateState;
	    
	}
    
	private void UpdateState()
	{
		if(state == null) return;
			
		warning.SetActive(state.State == BuildingState.Warning);
	}

    private void DestroyUi(GameObject view)
    {
        if (view == null) return;
        
        Context.DestroyElement(view);
        
        RemoveFromLayerCache(view);
    }

	private GameObject CreateUi(ViewType view)
	{
		var go = Context.CreateElement((int) view);
		
		go.SetParent(bodySprites.First().transform, false);
		AddToLayerCache(go.gameObject);
		
		return go.gameObject;
	}
}