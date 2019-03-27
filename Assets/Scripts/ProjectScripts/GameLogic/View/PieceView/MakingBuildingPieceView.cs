public class MakingBuildingPieceView : BuildingPieceView
{
    private TimerComponent timer;
    private ParticleView cooldownParticles;
    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);

        var life = Piece.GetComponent<MakingLifeComponent>(MakingLifeComponent.ComponentGuid);

        timer = life?.TimerMain;
        
        if(timer == null) return;
        
        timer.OnStart += UpdateLockSate;
        timer.OnComplete += UpdateLockSate;

        UpdateLockSate();
    }
    
    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        
        if(timer == null) return;
        
        timer.OnStart -= UpdateLockSate;
        timer.OnComplete -= UpdateLockSate;
    }
    
    private void UpdateLockSate()
    {
        if(timer == null || bodySprites == null) return;

        ToggleCooldownState(timer.IsStarted);
    }

    private void ToggleCooldownState(bool isCooldown)
    {
        if (isLockVisual) return;
        
        if (isCooldown)
        {
            SetCustomMaterial(BoardElementMaterialType.PiecesSepiaDefaultMaterial, true);
            SaveCurrentMaterialAsDefault();
            if (cooldownParticles != null) cooldownParticles.DestroyOnBoard();
            cooldownParticles = ParticleView.Show(R.MonumentCooldownParticle, Piece.CachedPosition);
            cooldownParticles.transform.SetParentAndReset(transform);
            cooldownParticles.SyncRendererLayers(Piece.CachedPosition.SetZ(BoardLayer.FX.Layer));
        }
        else
        {
            ClearCurrentMaterialAsDefault();
            ResetDefaultMaterial();
            if (cooldownParticles != null) cooldownParticles.DestroyOnBoard();
        }
    }
}