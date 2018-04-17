using System.Collections.Generic;

public class MulticellularPieceMatchActionBuilder : IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>
        {
            PieceType.Mine1.Id,
            PieceType.Mine2.Id,
            PieceType.Mine3.Id,
            PieceType.Mine4.Id,
            PieceType.Mine5.Id,
            PieceType.Mine6.Id,
            PieceType.Mine7.Id,
            
            PieceType.Sawmill1.Id,
            PieceType.Sawmill2.Id,
            PieceType.Sawmill3.Id,
            PieceType.Sawmill4.Id,
            PieceType.Sawmill5.Id,
            PieceType.Sawmill6.Id,
            PieceType.Sawmill7.Id,
            
            PieceType.Sheepfold1.Id,
            PieceType.Sheepfold2.Id,
            PieceType.Sheepfold3.Id,
            PieceType.Sheepfold4.Id,
            PieceType.Sheepfold5.Id,
            PieceType.Sheepfold6.Id,
            PieceType.Sheepfold7.Id,
            
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
        
        if (countForMatchDefault == -1 || countForMatch < countForMatchDefault) return null;
        
        var nextAction = new SpawnPieceAtAction
        {
            IsCheckMatch = false,
            At = position,
            PieceTypeId = nextType
        };
        
        return new CollapsePieceToAction
        {
            To = position,
            Positions = new List<BoardPosition>{position},
            OnCompleteAction = nextAction
        };
    }
}