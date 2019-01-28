using System.Collections.Generic;

public class MulticellularPieceMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>();
    }

    public bool Check(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position, out int next)
    {
        next = definition.GetNext(pieceType, false);
        matchField = new List<BoardPosition>{position};
        
        if (next == PieceType.None.Id) return false;

        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        return countForMatchDefault != -1 && 1 >= countForMatchDefault;
    }

    public IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        return Check(definition, matchField, pieceType, position, out var nextType) == false
            ? null
            : CreateAction(new List<int> {nextType}, new List<BoardPosition> {position}, position, pieceType);
    }
}