using System.Collections.Generic;

public class MineMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>
        {
            PieceType.MN_B1.Id,
            PieceType.MN_B2Fake.Id,
            PieceType.MN_B2.Id,
            PieceType.MN_B3Fake.Id,
            PieceType.MN_B3.Id,
            
            PieceType.MN_C1.Id,
            PieceType.MN_C2Fake.Id,
            PieceType.MN_C2.Id,
            PieceType.MN_C3Fake.Id,
            PieceType.MN_C3.Id,
            
            PieceType.MN_E1.Id,
            PieceType.MN_E2Fake.Id,
            PieceType.MN_E2.Id,
            PieceType.MN_E3Fake.Id,
            PieceType.MN_E3.Id,
            
            PieceType.MN_F1.Id,
            PieceType.MN_F2Fake.Id,
            PieceType.MN_F2.Id,
            PieceType.MN_F3Fake.Id,
            PieceType.MN_F3.Id,
            
            PieceType.MN_H1.Id,
            PieceType.MN_H2Fake.Id,
            PieceType.MN_H2.Id,
            PieceType.MN_H3Fake.Id,
            PieceType.MN_H3.Id,
            
            PieceType.MN_I1.Id,
            PieceType.MN_I2Fake.Id,
            PieceType.MN_I2.Id,
            PieceType.MN_I3Fake.Id,
            PieceType.MN_I3.Id,
        };
    }
    
    public bool Check(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position, out int next)
    {
        next = definition.GetNext(pieceType, false);
        
        if (next == PieceType.None.Id) return false;
        
        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        return countForMatchDefault != -1;
    }
    
    public IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        var nextType = definition.GetNext(pieceType, false);

        if (nextType == PieceType.None.Id) return null;
        
        var nextPieces = Calculation(definition, pieceType, nextType);

        // collect and purchase rewards before action
        return CreateAction(nextPieces, matchField, position, pieceType);
    }

    private List<int> Calculation(MatchDefinitionComponent definition, int currentType, int nextType)
    {
        var result = new List<int> {nextType};
        var isFake = PieceType.GetDefById(currentType).Filter.Has(PieceTypeFilter.Fake);

        if (isFake == false) return result;

        var data = GameDataService.Current.PiecesManager.GetComponent<PiecesMineDataManager>(PiecesMineDataManager.ComponentGuid);
        var previousType = definition.GetPrevious(currentType);

        return data.GetLoop(previousType) == 0 ? result : new List<int> {previousType};
    }
}