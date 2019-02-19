using System.Collections.Generic;

public class CharacterMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>
        {
            PieceType.NPC_B8.Id,
            PieceType.NPC_C8.Id,
            PieceType.NPC_D8.Id,
            PieceType.NPC_E8.Id,
            PieceType.NPC_F8.Id
        };
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
        
        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);

        if (countForMatchDefault == -1 || matchField.Count < countForMatchDefault) return null;
        
        matchField = new List<BoardPosition>();
        var chain = definition.GetChain(nextType);

        foreach (var id in chain)
        {
            matchField.AddRange(definition.Context.PositionsCache.GetPiecePositionsByType(id));
        }
        
        GameDataService.Current.CharactersManager.UnlockNewCharacter(nextType);
        
        // collect and purchase rewards before action
        return CreateAction(new List<int>{nextType}, matchField, position, pieceType);
    }
}