public class PrLockTutorialStep : BaseTutorialStep
{
    protected override void Complete()
    {
        base.Complete();
        
        var logic = BoardService.Current.FirstBoard.BoardLogic;
        var positions = logic.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.ProductionField, PieceTypeFilter.Obstacle);
        
        foreach (var pos in positions)
        {
            logic.GetPieceAt(pos)?.TutorialLocker?.Complete();
        }
    }
}