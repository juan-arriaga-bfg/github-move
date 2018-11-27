﻿using System.Collections.Generic;
using DG.Tweening;

public class SelectStorageTutorialStep : DelayTutorialStep
{
    public List<int> Targets;
    
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
        
        KillBubble();
        Find();
            
        if (bubble != null)
        {
            bubble.Attention();
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
        
        sequence.InsertCallback(3f, () =>
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
        });
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

                bubble = piece?.ViewDefinition?.GetViews().Find(view => view is ChangeObstacleStateView) as ChangeObstacleStateView;
        
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