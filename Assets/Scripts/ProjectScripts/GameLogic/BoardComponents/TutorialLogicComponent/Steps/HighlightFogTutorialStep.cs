using System.Collections.Generic;

public class HighlightFogTutorialStep : LoopFingerTutorialStep
{
    private readonly int mana = PieceType.Mana1.Id;
    private bool isSpawn;
    
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

        if (positions.Count == 0) return;
        
        from = positions[0];
        to = GameDataService.Current.FogsManager.GetNextRandomFog().GetCenter();
        
        base.Execute();
    }

    protected override void Complete()
    {
        base.Complete();
        
        Context.Context.HintCooldown.Resume(this);
    }

    public override bool IsExecuteable()
    {
        return isSpawn && base.IsExecuteable();
    }
}