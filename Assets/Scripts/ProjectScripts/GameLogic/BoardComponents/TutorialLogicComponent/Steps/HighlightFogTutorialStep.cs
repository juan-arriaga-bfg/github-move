using System.Collections.Generic;
using DG.Tweening;

public class HighlightFogTutorialStep : LoopFingerTutorialStep
{
    private readonly int mana = PieceType.Mana1.Id;
    private bool isSpawn;
    
    private BubbleView bubble;
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();
        Context.Context.HintCooldown.Pause(this);
        
        var positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(mana);
        
        if (positions.Count != 0)
        {
            isSpawn = true;
            return;
        }

        var positionsHero = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Character);
        
        if (positionsHero.Count == 0) return;
        
        Context.Context.ActionExecutor.AddAction(new EjectionPieceAction
        {
            From = positionsHero[0],
            Pieces = new Dictionary<int, int> {{mana, 1}},
            OnComplete = () => { isSpawn = true; }
        });
    }
    
    public override void Execute()
    {
        var positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(mana);

        if (positions.Count == 0)
        {
            StepBubble();
            return;
        }
        
        from = positions[0];
        to = GameDataService.Current.FogsManager.GetNextRandomFog().GetCenter();
        
        base.Execute();
    }
    
    private void StepBubble()
    {
        var position = to;
        position.Z = BoardLayer.Piece.Layer;
        
        var piece = Context.Context.BoardLogic.GetPieceAt(position);
        
        bubble = piece?.ViewDefinition?.GetViews().Find(view => view is BubbleView) as BubbleView;

        if (bubble == null) return;
        
        var sequence = DOTween.Sequence().SetId(this).SetLoops(int.MaxValue);

        sequence.AppendCallback(bubble.Attention);
        sequence.AppendInterval(1f);
        sequence.AppendCallback(bubble.Attention);
        sequence.AppendInterval(2f);
    }

    protected override void Complete()
    {
        DOTween.Kill(this);
        bubble = null;
        
        base.Complete();
        
        Context.Context.HintCooldown.Resume(this);
    }

    public override bool IsExecuteable()
    {
        return isSpawn && bubble == null && base.IsExecuteable();
    }
}