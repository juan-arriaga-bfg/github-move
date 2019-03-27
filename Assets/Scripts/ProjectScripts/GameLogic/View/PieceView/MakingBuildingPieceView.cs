using DG.Tweening;
using UnityEngine;

public class MakingBuildingPieceView : BuildingPieceView
{
    [SerializeField] private Transform rootObject;
    
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
    
    public void PlayWorkAnimation()
    {
        var particle = ParticleView.Show(R.MiningParticles, Piece.CachedPosition);
        particle.transform.SetParent(transform);
        particle.SyncRendererLayers(Piece.CachedPosition);

        SetCustomMaterial(BoardElementMaterialType.PiecesHighlightMaterial, true);
        if (rootObject == null) return;
        
        var sequence = DOTween.Sequence().SetId(animationUid);
        sequence.OnKill(() => rootObject.localScale = Vector3.one);
        
        sequence.SetLoops(3);

        var ease = Ease.Linear;
        sequence.Append(rootObject.DOScale(new Vector3(1.13f, 0.98f, 1f), 0.18f).SetEase(ease));
        sequence.Append(rootObject.DOScale(new Vector3(1.03f, 1.13f, 1f), 0.18f).SetEase(ease));
        sequence.Append(rootObject.DOScale(new Vector3(1.13f, 0.98f, 1f), 0.18f).SetEase(ease));
        sequence.Append(rootObject.DOScale(new Vector3(1f, 1f, 1f), 0.18f).SetEase(ease));
    }
    
    public void CompleteWorkAnimation()
    {
        SetCustomMaterial(BoardElementMaterialType.PiecesHighlightMaterial, false);
    }
}