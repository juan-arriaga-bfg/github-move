using System.Collections.Generic;
using DG.Tweening;

public class SelectStorageTutorialStep : DelayTutorialStep
{
    public List<int> Targets;

    private BoardPosition? position;
    private HintArrowView arrow;
    private ChangeObstacleStateView bubble;
    
    public override void PauseOn()
    {
        base.PauseOn();
        RemoveArrow();
        CheckBubble();
    }

    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();

        Context.Context.HintCooldown.Pause(this);
    }
    
    public override void Execute()
    {
        base.Execute();
        
        foreach (var target in Targets)
        {
            var positions = Context.Context.BoardLogic.PositionsCache.GetRandomPositions(target, 1);
            
            if(positions.Count == 0) continue;

            position = positions[0];
            
            arrow = HintArrowView.Show(position.Value, 0, 0, true, true);
            break;
        }
    }

    public override bool IsExecuteable()
    {
        return arrow == null && bubble == null && base.IsExecuteable();
    }

    protected override void Complete()
    {
        base.Complete();

        DOTween.Kill(this);
        bubble = null;
        
        RemoveArrow();
        Context.Context.HintCooldown.Resume(this);
    }

    private void RemoveArrow()
    {
        if (arrow == null) return;
        
        arrow.Remove(0);
        arrow = null;
    }

    private void CheckBubble()
    {
        DOTween.Kill(this);
        bubble = null;
        
        if(position == null) return;
        
        var sequence = DOTween.Sequence().SetId(this).SetLoops(int.MaxValue);
        
        sequence.InsertCallback(3f, () =>
        {
            if (bubble != null)
            {
                bubble.Attention();
                return;
            }
            
            var piece = Context.Context.BoardLogic.GetPieceAt(position.Value);

            bubble = piece?.ViewDefinition?.GetViews().Find(view => view is ChangeObstacleStateView) as ChangeObstacleStateView;
        
            if(bubble == null) return;
            
            bubble.Attention();
            position = null;
        });
    }
}