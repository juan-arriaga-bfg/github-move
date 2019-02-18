using System.Collections.Generic;

public class SimpleMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
{
    private const int amountBonus = 5;
    
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

        var nextPieces = Calculation(pieceType, nextType, definition.GetPieceCountForMatch(pieceType), matchField.Count);

        if (nextPieces.Count == 0) return null;
        
        nextPieces.Sort();
        
        var matchDescription = new MatchDescription
        {
            SourcePieceType = pieceType,
            MatchedPiecesCount = matchField.Count,
            CreatedPieceType = nextType,
        };
            
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.Match, matchDescription);
        
        // collect and purchase rewards before action
        return CreateAction(nextPieces, matchField, position, pieceType);
    }

    private List<int> Calculation(int currentType, int nextType, int amountDefault, int amountCurrent)
    {
        var nextPieces = new List<int>();
        
        if (amountDefault == -1 || amountCurrent < amountDefault) return nextPieces;
        
        if (amountCurrent < amountBonus)
        {
            nextPieces = Add(1, nextType, nextPieces);
            if (amountCurrent == amountBonus - 1) nextPieces = Add(1, currentType, nextPieces);
            
            return nextPieces;
        }

        var amount = amountCurrent / amountBonus * 2;
        var remainder = amountCurrent % amountBonus;

        switch (remainder)
        {
            case 0:
            case 1:
                nextPieces = Add(amount, nextType, nextPieces);
                break;
            case 2:
                nextPieces = Add(amount, nextType, nextPieces);
                nextPieces = Add(1, currentType, nextPieces);
                break;
            case 3:
            case 4:
                nextPieces = Add(amount + 1, nextType, nextPieces);
                break;
        }
        
        return nextPieces;
    }
    
    private List<int> Add(int count, int piece, List<int> pieces)
    {
        for (var i = 0; i < count; i++)
        {
            pieces.Add(piece);
        }

        return pieces;
    }
}