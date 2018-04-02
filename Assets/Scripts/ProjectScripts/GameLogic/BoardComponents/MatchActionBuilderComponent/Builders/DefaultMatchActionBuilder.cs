﻿using System.Collections.Generic;
using UnityEngine;

public class DefaultMatchActionBuilder : IMatchActionBuilder
{
    public List<int> GetKeys()
    {
        return new List<int>();
    }

    public IBoardAction Build(MatchDefinitionComponent definition, List<BoardPosition> matchField, int pieceType, BoardPosition position)
    {
        var nextType = definition.GetNext(pieceType);

        if (nextType == PieceType.None.Id) return null;

        var countForMatch = matchField.Count;
        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        if (countForMatch < countForMatchDefault) return null;

        var countForMatchBonus = countForMatchDefault * 2 - 1;

        var nextPieces = new List<int>();
        
        if (countForMatch % countForMatchBonus == 0)
        {
            nextPieces = Add((countForMatch / countForMatchBonus) * 2, nextType, nextPieces);
        }
        else
        {
            nextPieces = Add(countForMatch / countForMatchDefault, nextType, nextPieces);
            nextPieces = Add(countForMatch - (countForMatch / countForMatchDefault) * countForMatchDefault, pieceType, nextPieces);
        }
        
        var nextAction = new SpawnPiecesAction
        {
            IsCheckMatch = true,
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

    private void SpawnReward(BoardPosition position, int pieceType)
    {
        var def = GameDataService.Current.PiecesManager.GetPieceDef(pieceType);
        
        if(def == null) return;
        
        AddResourceView.Show(position, def.CreateReward);
    }
}