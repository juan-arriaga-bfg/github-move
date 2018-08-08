using System.Collections.Generic;

public class SimpleMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>();
    }

    public bool Check(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position, out int next)
    {
        next = definition.GetNext(pieceType, false);
        
        if (next == PieceType.None.Id) return false;
        
        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        return countForMatchDefault != -1 && matchField.Count >= countForMatchDefault;
    }
    
    public IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        var nextType = definition.GetNext(pieceType, false);

        if (nextType == PieceType.None.Id) return null;

        var countForMatch = matchField.Count;
        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        if (countForMatchDefault == -1 || countForMatch < countForMatchDefault) return null;

        var countForMatchBonus = countForMatchDefault * 2 - 1;

        var nextPieces = new List<int>();

        if (countForMatch % countForMatchBonus == 0)
        {
            nextPieces = Add((countForMatch / countForMatchBonus) * 2, nextType, nextPieces);
        }
        else
        {
            nextPieces = Add(countForMatch / countForMatchDefault, nextType, nextPieces);
            nextPieces = Add(countForMatch - (countForMatch / countForMatchDefault) * countForMatchDefault, pieceType,
                nextPieces);
        }

        var nextAction = new SpawnPiecesAction
        {
            IsCheckMatch = false,
            At = position,
            Pieces = nextPieces,
            OnSuccessEvent = list =>
            {
                for (int i = 0; i < list.Count; i++)
                {
                    SpawnReward(list[i], nextPieces[i]);
                }
            }
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