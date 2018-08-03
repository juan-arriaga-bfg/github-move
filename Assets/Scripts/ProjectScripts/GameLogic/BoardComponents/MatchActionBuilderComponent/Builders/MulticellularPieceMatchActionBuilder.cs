using System.Collections.Generic;

public class MulticellularPieceMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
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
            PieceType.Castle9.Id
        };
    }

    public IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        var nextType = definition.GetNext(pieceType, false);

        if (nextType == PieceType.None.Id) return null;

        var countForMatch = 1;
        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        if (countForMatchDefault == -1 || countForMatch < countForMatchDefault) return null;
        
        var nextAction = new SpawnPieceAtAction
        {
            IsCheckMatch = false,
            At = position,
            PieceTypeId = nextType,
            OnSuccessEvent = pos => SpawnReward(pos, nextType)
        };
        
        return new CollapsePieceToAction
        {
            To = position,
            Positions = new List<BoardPosition>{position},
            OnCompleteAction = nextAction
        };
    }
}