using UnityEngine;

public class PartPieceBoardObserver : IECSComponent, IPieceBoardObserver
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public int Guid { get { return ComponentGuid; } }
    
    public void OnRegisterEntity(ECSEntity entity)
    {
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
    }
    
    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        Debug.LogError("OnAddToBoard " + CheckСompositeMatch(position, context));
    }

    public void OnMovedFromTo(BoardPosition @from, BoardPosition to, Piece context = null)
    {
        Debug.LogError("OnMovedFromTo " + CheckСompositeMatch(to, context));
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        Debug.LogError("OnRemoveFromBoard " + CheckСompositeMatch(position, context));
    }
    
    private bool CheckСompositeMatch(BoardPosition position, Piece piece)
    {
        if (piece == null) return false;
        
        var logic = piece.Context.BoardLogic;

        if (!piece.Matchable.IsMatchable()
            || logic.MatchDefinition.GetNext(piece.PieceType) != PieceType.MegaZord.Id)
        {
            return false;
        }
		
        return logic.MatchActionBuilder.GetMatchAction(null, piece.PieceType, position) != null;
    }
}