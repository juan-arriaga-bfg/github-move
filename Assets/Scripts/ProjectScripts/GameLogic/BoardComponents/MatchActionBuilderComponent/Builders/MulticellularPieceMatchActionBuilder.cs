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
        int nextType;
        
        if(Check(definition, matchField, pieceType, position, out nextType) == false) return null;
        
        return new ModificationPiecesAction
        {
            NextPieces = new List<int>{nextType},
            To = position,
            Positions = new List<BoardPosition>{position},
            OnSuccess = list => SpawnReward(list[0], nextType)
        };
    }
}