using System.Collections.Generic;

public class HighlightPiecesTutorialStep : DelayTutorialStep
{
    public List<int> Targets;
    
    private List<PartCellView> selectCells;
    
    public override void PauseOn()
    {
        if(selectCells == null) return;
        
        base.PauseOn();
        
        foreach (var cell in selectCells)
        {
            cell.ToggleSelection(false);
            Context.Context.RendererContext.DestroyElement(cell);
        }
        
        selectCells = null;
    }
    
    public override void PauseOff()
    {
        if(selectCells != null) return;
        
        base.PauseOff();
    }
    
    protected override void Complete()
    {
        PauseOn();
    }
    
    public override void Execute()
    {
        base.Execute();
        
        if(selectCells != null) return;
        
        selectCells = new List<PartCellView>();

        List<BoardPosition> positions = null;
        
        foreach (var target in Targets)
        {
            var cache = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(target);
            var amount = Context.Context.BoardLogic.MatchDefinition.GetPieceCountForMatch(target);
            
            if(cache.Count < amount) continue;

            positions = cache;
            break;
        }
        
        if(positions == null) return;
        
        foreach (var position in positions)
        {
            var cell = Context.Context.RendererContext.CreateBoardElementAt<PartCellView>(R.Cell, new BoardPosition(position.X, position.Y, 21));

            cell.ToggleSelection(true);
            selectCells.Add(cell);
        }
    }
    
    public override bool IsExecuteable()
    {
        return selectCells == null && base.IsExecuteable();
    }
}