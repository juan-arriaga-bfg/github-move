﻿using System.Collections.Generic;

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