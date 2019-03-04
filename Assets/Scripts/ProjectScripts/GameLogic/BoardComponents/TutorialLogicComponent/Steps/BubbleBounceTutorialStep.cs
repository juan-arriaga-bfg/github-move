using System.Collections.Generic;
using DG.Tweening;

public class BubbleBounceTutorialStep<T> : DelayTutorialStep
    where T : UIBoardView
{
    public List<int> Targets;
    
    protected T bubble;
    
    public override void PauseOn()
    {
        base.PauseOn();
        
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

        if (bubble == null) return;
        
        CheckBubble();
    }
    
    public override bool IsExecuteable()
    {
        if (bubble != null && bubble.IsShow == false) bubble = null;
        
        return bubble == null && base.IsExecuteable();
    }
    
    protected override void Complete()
    {
        base.Complete();

        KillBubble();
        Context.Context.HintCooldown.Resume(this);
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

                bubble = piece?.ViewDefinition?.GetViews().Find(view => view is T) as T;
        
                if(bubble != null) return;
            }
        }
    }

    private void KillBubble()
    {
        DOTween.Kill(this);
        bubble = null;
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        base.OnUnRegisterEntity(entity);
        KillBubble();
    }
}
