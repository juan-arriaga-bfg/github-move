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
        
        target.SyncRendererLayers(context.Context.BoardDef.MaxPoit);
        target.CachedTransform
            .DOJump(new Vector3(to.x, to.y, target.CachedTransform.position.z), 1, 1, 0.4f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => CompleteAnimation(context));
    }
}