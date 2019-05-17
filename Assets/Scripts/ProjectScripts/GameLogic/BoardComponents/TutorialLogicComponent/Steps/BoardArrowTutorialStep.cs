using System.Collections.Generic;

public class BoardArrowTutorialStep : BaseTutorialStep, IBoardEventListener
{
    public List<int> Targets;

    public override void PauseOff()
    {
        base.PauseOff();
        
        UpdateArrow(Targets);
    }
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();
        
        AddArrow();
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ChangePiecePosition);
    }
    
    protected override void Complete()
    {
        Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ChangePiecePosition);
        RemoveArrow();
        
        base.Complete();
    }
    
    public void OnBoardEvent(int code, object context)
    {
        var id = (int) context;
        if (Targets.Contains(id) == false) return;
        
        UpdateArrow(new List<int>{id});
    }
    
    private void AddArrow()
    {
        var positions = GetPositions(Targets);
        
        foreach (var position in positions)
        {
            var element = Context.Context.BoardLogic.GetPieceAt(position).ActorView;
            
            element.AddArrow();
        }
    }

    private void UpdateArrow(List<int> targets)
    {
        var positions = GetPositions(targets);

        foreach (var position in positions)
        {
            if (Context.Context.BoardLogic.IsLockedCell(position)) continue;
            
            var element = Context.Context.BoardLogic.GetPieceAt(position).ActorView;

            if (element == null) continue;
            
            element.UpdateArrow();
            element.AddArrow();
        }
    }
    
    private void RemoveArrow()
    {
        var positions = GetPositions(Targets);

        foreach (var position in positions)
        {
            var element = Context.Context.BoardLogic.GetPieceAt(position).ActorView;
                
            element.RemoveArrow();
        }
    }

    private List<BoardPosition> GetPositions(List<int> targets)
    {
        var list = new List<BoardPosition>();

        foreach (var id in targets)
        {
            list.AddRange(Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(id));
        }

        return list;
    }
}