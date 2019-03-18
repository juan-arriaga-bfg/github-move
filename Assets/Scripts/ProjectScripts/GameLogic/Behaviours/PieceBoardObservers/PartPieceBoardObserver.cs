using System.Collections.Generic;

public class PartPieceBoardObserver : IECSComponent, IPieceBoardObserver
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    private MatchActionBuilderComponent matchActionBuilder;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        matchActionBuilder = (entity as Piece)?.Context.BoardLogic.MatchActionBuilder;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        if (context == null) return;

        var positions = new List<BoardPosition>();

        if (matchActionBuilder.CheckMatch(positions, context.PieceType, position, out _) == false) return;
        
        context.Context.PartPiecesLogic.Add(positions);
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
        if (context == null) return;

        var positions = new List<BoardPosition>();

        if (matchActionBuilder.CheckMatch(positions, context.PieceType, @from, out _) == false) return;
        
        context.Context.PartPiecesLogic.Remove(positions);
    }

    public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
        if (context == null) return;

        var positions = new List<BoardPosition>();

        if (matchActionBuilder.CheckMatch(positions, context.PieceType, to, out _) == false) return;
        
        context.Context.PartPiecesLogic.Add(positions);
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
    }
}