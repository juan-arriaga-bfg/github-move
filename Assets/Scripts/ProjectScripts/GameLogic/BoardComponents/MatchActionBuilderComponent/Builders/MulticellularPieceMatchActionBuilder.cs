using System.Collections.Generic;

public class MulticellularPieceMatchActionBuilder : IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>
        {
            PieceType.Castle1.Id,
            PieceType.Castle2.Id,
            PieceType.Castle3.Id,
            PieceType.Castle4.Id,
            PieceType.Castle5.Id,
            PieceType.Castle6.Id,
            PieceType.Castle7.Id,
            PieceType.Castle8.Id,
            PieceType.Castle9.Id,
            
            PieceType.Tavern1.Id,
            PieceType.Tavern2.Id,
            PieceType.Tavern3.Id,
            PieceType.Tavern4.Id,
            PieceType.Tavern5.Id,
            PieceType.Tavern6.Id,
            PieceType.Tavern7.Id,
            PieceType.Tavern8.Id,
            PieceType.Tavern9.Id,
        };
    }

    public IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        var nextType = definition.GetNext(pieceType);

        if (nextType == PieceType.None.Id) return null;

        var countForMatch = 1;
        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        if (countForMatch < countForMatchDefault) return null;
        
        var nextAction = new SpawnPiecesAction
        {
            IsCheckMatch = false,
            At = position,
            Pieces = new List<int> {nextType}
        };
        
        return new CollapsePieceToAction
        {
            To = position,
            Positions = new List<BoardPosition>{position},
            OnCompleteAction = nextAction
        };
    }

    private List<int> Add(int count, int piece, List<int> pieces)
    {
        for (int i = 0; i < count; i++)
        {
            pieces.Add(piece);
        }
        
        return pieces;
    }
}