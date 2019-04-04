using DG.Tweening;
using UnityEngine;

public class MinePieceView : PieceBoardElementView
{   
    [SerializeField] private Transform rootObject; 

    public void PlayRestoreAnimation()
    {
        
    }

    public void CompleteRestoreAnimation()
    {
        
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

    public override void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context, piece);
    }

    
}