using System.Collections.Generic;

public class PartPieceBoardObserver : IECSComponent, IPieceBoardObserver
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid => ComponentGuid;

    protected Piece contextPiece;
    private MatchActionBuilderComponent matchActionBuilder;
    
    public virtual void OnRegisterEntity(ECSEntity entity)
    {
        contextPiece = entity as Piece;
        matchActionBuilder = contextPiece.Context.BoardLogic.MatchActionBuilder;
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public virtual void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        AddBubble(position, contextPiece.PieceType);
    }

    public void OnMovedFromToStart(BoardPosition from, BoardPosition to, Piece context = null)
    {
        RemoveBubble(from);
    }

    public void OnMovedFromToFinish(BoardPosition from, BoardPosition to, Piece context = null)
    {
        AddBubble(to, contextPiece.PieceType);
    }
    
    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        RemoveBubble(position);
    }

    public virtual void AddBubble(BoardPosition position, int pieceType)
    {
        var positions = new List<BoardPosition>();

        if (matchActionBuilder.CheckMatch(positions, pieceType, position, out _) == false) return;

        while (positions.Count >= 4)
        {
            var pattern = positions.GetRange(0, 4);
            
            positions.RemoveRange(0, 4);
            contextPiece.Context.PartPiecesLogic.Add(pattern);
        }
    }

    private void RemoveBubble(BoardPosition position)
    {
        contextPiece.Context.PartPiecesLogic.Remove(position);
    }
}