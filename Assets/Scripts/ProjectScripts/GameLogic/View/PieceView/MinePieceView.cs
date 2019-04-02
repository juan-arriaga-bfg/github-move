using DG.Tweening;
using UnityEngine;

public class MinePieceView : CooldownPieceView
{
    [SerializeField] private Transform rootObject;
    protected override string coolDownParticle => R.MineProcessParticle;
    protected override string coolDownLeaveParticle => R.MineEndParticle;
    protected override string coolDownEnterParticle => R.MineEndParticle;

    protected override void OnComplete()
    {
        ParticleView.Show(coolDownLeaveParticle, Piece.CachedPosition.SetZ(Piece.CachedPosition.Z + 1));
        ToggleEffectsByState(false);

        var sequence = DOTween.Sequence();
        sequence.InsertCallback(0.2f, UpdateSate);
        
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