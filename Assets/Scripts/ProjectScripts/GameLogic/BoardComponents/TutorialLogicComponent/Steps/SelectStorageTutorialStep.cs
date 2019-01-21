using System.Collections.Generic;
using DG.Tweening;

public class SelectStorageTutorialStep : DelayTutorialStep
{
    public List<int> Targets;
    
    public bool IsFastStart;
    
    private HintArrowView arrow;
    private ObstacleBubbleView bubble;
    
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
        if(IsFastStart) startTime = startTime.AddSeconds(-(Delay-0.5f));
    }
    
    public override void Execute()
    {
        base.Execute();
        
        KillBubble();
        Find();
            
        if (bubble != null)
        {
            CheckBubble();
            return;
        }
        
        foreach (var target in Targets)
        {
            var positions = Context.Context.BoardLogic.PositionsCache.GetRandomPositions(target, 1);
            
            if(positions.Count == 0) continue;
            
            arrow = HintArrowView.Show(positions[0], 0, 0, !Context.Context.Manipulator.CameraManipulator.CameraMove.IsLocked, true);
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

        KillBubble();
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
        KillBubble();
        
        var sequence = DOTween.Sequence().SetId(this).SetLoops(int.MaxValue);

        sequence.AppendCallback(Bounce);
        sequence.AppendInterval(1f);
        sequence.AppendCallback(Bounce);
        sequence.AppendInterval(2f);
    }

    private void Bounce()
    {
        if (bubble != null)
        {
            bubble.Attention();
            return;
        }

        Find();
            
        if (bubble != null)
        {
            bubble.Attention();
            return;
        }

        if (IsExecuteable()) DOTween.Kill(this);
    }
    
    private void Find()
    {
        foreach (var target in Targets)
        {
            var positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(target);
            
            if(positions.Count == 0) continue;

            foreach (var position in positions)
            {
                var piece = Context.Context.BoardLogic.GetPieceAt(position);

                bubble = piece?.ViewDefinition?.GetViews().Find(view => view is ObstacleBubbleView) as ObstacleBubbleView;
        
                if(bubble != null) return;
            }
        }
    }

    private void KillBubble()
    {
        DOTween.Kill(this);
        bubble = null;
    }
}