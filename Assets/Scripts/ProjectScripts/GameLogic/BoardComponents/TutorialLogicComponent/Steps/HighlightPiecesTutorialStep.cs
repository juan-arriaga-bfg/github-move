using System.Collections.Generic;
using DG.Tweening;

public class HighlightPiecesTutorialStep : DelayTutorialStep
{
    public List<int> Targets;
    
    private List<PieceBoardElementView> selectPieces;
    
    public override void PauseOn()
    {
        base.PauseOn();

        SelectionOff();
    }
    
    protected override void Complete()
    {
        base.Complete();
        SelectionOff();
    }
    
    public override void Execute()
    {
        base.Execute();
        
        if(selectPieces != null) return;
        
        selectPieces = new List<PieceBoardElementView>();

        List<BoardPosition> positions = null;
        var amountMatch = 0;
        
        foreach (var target in Targets)
        {
            var cache = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(target);
            var amount = Context.Context.BoardLogic.MatchDefinition.GetPieceCountForMatch(target);
            
            if(cache.Count < amount) continue;

            positions = cache;
            amountMatch = amount;
            break;
        }
        
        if(positions == null) return;
        
        var options = new List<List<BoardPosition>>();

        foreach (var position in positions)
        {
            int amount;
            var field = new List<BoardPosition>();
            
            if(Context.Context.BoardLogic.FieldFinder.Find(position, field, out amount) == false) continue;
            
            options.Add(field);
        }
        
        options.Sort((a, b) => -a.Count.CompareTo(b.Count));

        var best = options[0];

        if (best.Count < amountMatch)
        {
            best.AddRange(best[0].GetImmediate(positions, best, amountMatch - best.Count));
        }
        
        foreach (var position in best)
        {
            var view = Context.Context.RendererContext.GetElementAt(position) as PieceBoardElementView;
            
            if(view == null) continue;
            
            view.ToggleSelection(true);
            selectPieces.Add(view);
        }
        
        var center = BoardPosition.GetCenter(best);
        var centerPos = Context.Context.BoardDef.GetPiecePosition(center.X, center.Y);

        if (Context.Context.Manipulator.CameraManipulator.CameraMove.IsLocked == false) Context.Context.Manipulator.CameraManipulator.MoveTo(centerPos);
    }
    
    public override bool IsExecuteable()
    {
        return selectPieces == null && base.IsExecuteable();
    }

    private void SelectionOff()
    {
        DOTween.Kill(this);
        
        if(selectPieces == null) return;
        
        foreach (var view in selectPieces)
        {
            view.ToggleSelection(false);
        }
        
        selectPieces = null;
    }
}