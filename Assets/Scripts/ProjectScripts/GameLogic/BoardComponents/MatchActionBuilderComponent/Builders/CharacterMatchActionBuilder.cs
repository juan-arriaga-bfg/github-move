﻿using System.Collections.Generic;

public class CharacterMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>
        {
            PieceType.NPC_B3.Id,
            PieceType.NPC_C5.Id,
            PieceType.NPC_D6.Id,
            PieceType.NPC_E6.Id,
            PieceType.NPC_F6.Id,
            PieceType.NPC_G6.Id,
            PieceType.NPC_H6.Id,
            PieceType.NPC_I6.Id,
            PieceType.NPC_J6.Id,
            PieceType.NPC_K6.Id,
            PieceType.NPC_L6.Id,
            PieceType.NPC_M6.Id,
            PieceType.NPC_N6.Id,
            PieceType.NPC_O6.Id,
            PieceType.NPC_P6.Id,
            PieceType.NPC_Q6.Id,
            PieceType.NPC_R6.Id,
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
        
        matchField = matchField.FindAll(boardPosition => definition.Context.GetPieceAt(boardPosition)?.PieceType == PieceType.Boost_CR.Id);
        var chain = definition.GetChain(nextType);

        foreach (var id in chain)
        {
            matchField.AddRange(definition.Context.PositionsCache.GetPiecePositionsByType(id));
        }
        
        // collect and purchase rewards before action
        return CreateAction(new List<int>{nextType}, matchField, position, pieceType);
    }
}