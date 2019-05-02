using DG.Tweening;
using UnityEngine;

public class CollapseManaAnimation : BoardAnimation
{
    public BoardPosition From;
    public BoardPosition To;
    
    public override void Animate(BoardRenderer context)
    {
        var target = context.GetElementAt(From);
        var to = context.Context.BoardDef.GetPiecePosition(To.X, To.Y);
        var targetTransform = target.CachedTransform;
        
        target.SyncRendererLayers(context.Context.BoardDef.MaxPoit);

        var sequence = DOTween.Sequence();
        
        sequence
            .Insert(0.0f, targetTransform.DOJump(new Vector3(to.x, to.y, targetTransform.position.z), 1, 1, 0.4f).SetEase(Ease.InOutSine))
            .InsertCallback(0.0f, () => ParticleView.Show(R.MagicWandFlyParticle, From.SetZ(BoardLayer.FX.Layer)))
            .OnComplete(() => CompleteAnimation(context));
    }
}