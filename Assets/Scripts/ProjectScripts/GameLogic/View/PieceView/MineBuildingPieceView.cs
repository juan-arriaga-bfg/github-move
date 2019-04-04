using System.Linq;
using UnityEngine;

public class MineBuildingPieceView : BuildingPieceView
{
    private ParticleView workingParticle;
    
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        if (state != null)
        {
            state.OnChangeState += BuildAnimation;
        }
        
        BuildAnimation();
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        if (state != null)
        {
            state.OnChangeState -= BuildAnimation;
        }
    }
    
    private void BuildAnimation()
    {
        if (state.State == BuildingState.InProgress && workingParticle == null)
        {
            StartAnimation();
        }
        else
        {
            CompleteAnimation();
        }
    }
    
    

    private void StartAnimation()
    {
        workingParticle?.DestroyOnBoard();
        
        workingParticle = ParticleView.Show(R.MineProcessParticle, Piece.CachedPosition);
        workingParticle.transform.SetParentAndReset(transform);
        workingParticle.SyncRendererLayers(Piece.CachedPosition.SetZ(BoardLayer.FX.Layer));
    }

    private void CompleteAnimation()
    {
        workingParticle?.DestroyOnBoard();
        workingParticle = null;
    }

    public override void ToggleLockedState(bool isLocked)
    {
	    // Do nothing
    }
}