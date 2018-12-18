﻿using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SimpleMatchActionBuilder : DefaultMatchActionBuilder, IMatchActionBuilder
{
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

        var countForMatch = matchField.Count;
        var countForMatchDefault = definition.GetPieceCountForMatch(pieceType);
        
        if (countForMatchDefault == -1 || countForMatch < countForMatchDefault) return null;
        
        var nextPieces = Add(Mathf.RoundToInt(countForMatch / (float)countForMatchDefault), nextType, new List<int>());
        
        if(countForMatch % countForMatchDefault == 1) nextPieces = Add(1, pieceType, nextPieces);
        
        var matchDescription = new MatchDescription
        {
            SourcePieceType = pieceType,
            MatchedPiecesCount = matchField.Count,
            CreatedPieceType = nextType,
        };
            
        BoardService.Current.FirstBoard.BoardEvents.RaiseEvent(GameEventsCodes.Match, matchDescription);
        
        // collect and purchase rewards before action
        var rewardTransactions = new Dictionary<int, List<ShopItemTransaction>>();
        for (var i = 0; i < nextPieces.Count; i++)
        {
            var targetPieceType = nextPieces[i];
            
            if (targetPieceType == pieceType) continue;
            
            var def = GameDataService.Current.PiecesManager.GetPieceDef(targetPieceType);
        
            if(def?.CreateRewards == null) continue;
            
            for (var j = 0; j < def.CreateRewards.Count; j++)
            {
                var reward = def.CreateRewards[j];
        
                if (reward.Currency == Currency.Coins.Name
                 || reward.Currency == Currency.Crystals.Name
                 || reward.Currency == Currency.Energy.Name
                 || reward.Currency == Currency.Mana.Name
                 || reward.Currency == Currency.Worker.Name
                 || reward.Currency == Currency.Experience.Name)
                {
                    var transaction = CurrencyHellper.PurchaseAsync
                    (
                        reward
                    );
        
                    if (rewardTransactions.ContainsKey(i) == false)
                    {
                        rewardTransactions.Add(i, new List<ShopItemTransaction>());
                    }
        
                    rewardTransactions[i].Add(transaction);
                }
            }
        } 

        return new ModificationPiecesAction
        {
            NextPieces = nextPieces,
            To = position,
            Positions = matchField,
            OnSuccess = list =>
            {
                for (var i = 0; i < list.Count; i++)
                {
                    if (nextPieces[i] == pieceType) continue;

                    if (rewardTransactions.ContainsKey(i))
                    {
                        var targetTransactions = rewardTransactions[i];
                        var flyPosition = list[i];
                    
                        var sequence = DOTween.Sequence();
                        for (int j = 0; j < targetTransactions.Count; j++)
                        {
                            var targetTransaction = targetTransactions[j];                            
                            var from = BoardService.Current.FirstBoard.BoardDef.GetPiecePosition(flyPosition.X, flyPosition.Y);
                    
                            sequence.InsertCallback(0.5f * j, () =>
                            {
                                CurrencyHellper.CurrencyFly
                                (
                                    BoardService.Current.FirstBoard.BoardDef.ViewCamera.WorldToScreenPoint(from), 
                                    new CurrencyPair{ Currency = targetTransaction.ShopItem.ItemUid, Amount = targetTransaction.ShopItem.Amount }
                                );
                            });
                        }
                    }

                    // SpawnReward(list[i], nextPieces[i]);
                }
            }
        };
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