using System.Collections.Generic;

public class DefaultMatchActionBuilder : IMatchActionBuilder
{
    public IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        var nextType = definition.GetNext(pieceType);

        if (nextType == PieceType.None.Id) return null;

        var countForMatch = matchField.Count;
        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        if (countForMatch < countForMatchDefault) return null;

        var countForMatchBonus = countForMatchDefault * 2 - 1;

        var nextPieces = new List<int>();
        
        if (countForMatch % countForMatchBonus == 0)
        {
            nextPieces = Add((countForMatch / countForMatchBonus) * 2, nextType, nextPieces);
        }
        else
        {
            nextPieces = Add(countForMatch / countForMatchDefault, nextType, nextPieces);
            nextPieces = Add(countForMatch - (countForMatch / countForMatchDefault) * countForMatchDefault, pieceType, nextPieces);
        }
        
        var nextAction = new SpawnPiecesAction
        {
            IsCheckMatch = true,
            At = position,
            Pieces = nextPieces
        };
        
        return new CollapsePieceToAction
        {
            To = position,
            Positions = matchField,
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