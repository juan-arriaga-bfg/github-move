using System.Collections.Generic;

public class HighlightPiecesTutorialStep : BaseTutorialStep, IBoardEventListener
{
    public List<int> Targets;
    
    private List<PieceBoardElementView> selectPieces;
    
    protected override void OnFirstStart()
    {
        //nothing to do
    }
    
    public override void PauseOn()
    {
        base.PauseOn();

        SelectionOff();
    }

    public override void PauseOff()
    {
        base.PauseOff();
        
        SelectionOff();
        Execute();
    }

    public override void Perform()
    {
        if(IsPerform) return;
        
        base.Perform();
        
        Context.Context.BoardEvents.AddListener(this, GameEventsCodes.ChangePiecePosition);
        Execute();
    }

    protected override void Complete()
    {
        Context.Context.BoardEvents.RemoveListener(this, GameEventsCodes.ChangePiecePosition);
        SelectionOff();
        
        base.Complete();
    }
    
    public void OnBoardEvent(int code, object context)
    {
        if (isPauseOn || Targets.Contains((int) context) == false) return;
        
        SelectionOff();
        Execute();
    }
    
    private void Execute()
    {
        if (selectPieces != null) return;
        
        selectPieces = new List<PieceBoardElementView>();

        List<BoardPosition> positions = null;
        var amountMatch = 0;
        
        foreach (var target in Targets)
        {
            var cache = Context.Context.BoardLogic.PositionsCache.GetUnlockedPiecePositionsByType(target);
            var amount = Context.Context.BoardLogic.MatchDefinition.GetPieceCountForMatch(target);
            
            if(cache.Count < amount) continue;

            positions = cache;
            amountMatch = amount;
            break;
        }
        
        if (positions == null) return;
        
        var options = new List<List<BoardPosition>>();

        foreach (var position in positions)
        {
            int amount;
            var field = new List<BoardPosition>();
            
            if(Context.Context.BoardLogic.FieldFinder.Find(position, field, out amount, true) == false) continue;
            
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

            if (view == null) continue;
            
            view.ToggleSelection(true);
            selectPieces.Add(view);
        }
        
        if (best.Count >= amountMatch && IsFirstStartEvent())
        {
            tutorialDataManager.SetStarted(Id);
            OnFirstStartCallback?.Invoke(this);
        }
    }

    private void SelectionOff()
    {
        if (selectPieces == null) return;
        
        foreach (var view in selectPieces)
        {
            view.ToggleSelection(false);
        }
        
        selectPieces = null;
    }
}