using System.Collections.Generic;

public class DefaultMatchActionBuilder
{
    protected IBoardAction CreateAction(List<int> pieces, List<BoardPosition> positions, BoardPosition to, int ignoreType, bool ignoreBoost = false)
    {
        var rewardTransactions = CreateTransaction(pieces, ignoreType, ignoreBoost);
        
        return new ModificationPiecesAction
        {
            NextPieces = pieces,
            To = to,
            Positions = positions,
            OnSuccess = list =>
            {
                for (var i = 0; i < list.Count; i++)
                {
                    if (rewardTransactions.TryGetValue(i, out var targetTransactions) == false) continue;

                    for (var j = 0; j < targetTransactions.Count; j++)
                    {
                        var transaction = targetTransactions[j];
                        SpawnReward(list[i], transaction, 0.5f * j);
                    }
                }
            }
        };
    }
    
    private Dictionary<int, List<ShopItemTransaction>> CreateTransaction(List<int> nextPieces, int ignoreType, bool ignoreBoost)
    {
        var ignore = ignoreBoost ? nextPieces.FindLastIndex(id => id == PieceType.Boost_CR.Id) : -1;
        var rewardTransactions = new Dictionary<int, List<ShopItemTransaction>>();
        
        for (var i = 0; i < nextPieces.Count; i++)
        {
            var targetPieceType = nextPieces[i];
            
            if (targetPieceType == ignoreType || i == ignore) continue;
            
            var def = GameDataService.Current.PiecesManager.GetPieceDef(targetPieceType);

            if (def?.CreateRewards == null) continue;
            
            foreach (var reward in def.CreateRewards)
            {
                var transaction = CurrencyHelper.PurchaseAsync(reward);

                if (rewardTransactions.ContainsKey(i) == false) rewardTransactions.Add(i, new List<ShopItemTransaction>());
        
                rewardTransactions[i].Add(transaction);
            }
        }
        
        return rewardTransactions;
    }
    
    private void SpawnReward(BoardPosition position, ShopItemTransaction transaction, float delay)
    {
        var board = BoardService.Current.FirstBoard;
        var from = board.BoardDef.GetPiecePosition(position.X, position.Y);
        var flayPoint = board.BoardDef.ViewCamera.WorldToScreenPoint(from);
        
        CurrencyHelper.CurrencyFly(flayPoint, new CurrencyPair{Currency = transaction.ShopItem.ItemUid, Amount = transaction.ShopItem.Amount}, null, delay);
        AddResourceView.Show(position, transaction, delay);
    }
}