public class HighlightFogTutorialStep : LoopFingerTutorialStep
{
    private BoardPosition Target = new BoardPosition(20, 10, BoardLayer.Piece.Layer);
    
    public override void Perform()
    {
        if (IsPerform) return;
        
        base.Perform();

        Context.Context.HintCooldown.Pause(this);
    }
    
    public override void Execute()
    {
        var positions = Context.Context.BoardLogic.PositionsCache.GetPiecePositionsByType(PieceType.Mana1.Id);
        
        from = positions[0];
        to = Target;
        
        base.Execute();
    }
}