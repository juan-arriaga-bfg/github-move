using System;
using System.Collections.Generic;

public static partial class CurrencyHelper
{
    public static void PurchaseAndProvideEjection(List<CurrencyPair> products, CurrencyPair price = null, BoardPosition? position = null, Action onComplete = null)
    {
        var piecesReward = FiltrationRewards(products, out var currenciesReward);
        
        PurchaseAndProvideEjection(piecesReward, currenciesReward, price, position, onComplete);
    }
    
    public static void PurchaseAndProvideEjection(Dictionary<int, int> piecesReward, List<CurrencyPair> currenciesReward, CurrencyPair price = null, BoardPosition? position = null, Action onComplete = null)
    {
        if (GetAllData(currenciesReward, price, position, out var point, out var transactions) == false) return;
        
        var board = BoardService.Current.FirstBoard;
        
        board.ActionExecutor.AddAction(new EjectionPieceAction
        {
            From = point,
            Pieces = piecesReward,
            OnComplete = () => { OnCompleteAction(point, transactions, onComplete); }
        });
    }

    public static void PurchaseAndProvideSpawn(List<CurrencyPair> products, CurrencyPair price = null, BoardPosition? position = null, Action onComplete = null, bool topHighlight = false, bool bottomHighlight = false)
    {
        var piecesReward = FiltrationRewards(products, out var currenciesReward);
        PurchaseAndProvideSpawn(piecesReward, currenciesReward, price, position, onComplete, topHighlight, bottomHighlight);
    }

    public static void PurchaseAndProvideSpawn(Dictionary<int, int> piecesReward, List<CurrencyPair> currenciesReward,
        CurrencyPair price = null, BoardPosition? position = null, Action onComplete = null, bool topHighlight = false, bool bottomHighlight = false)
    {
        if (GetAllData(currenciesReward, price, position, out var point, out var transactions) == false) return;
        
        var board = BoardService.Current.FirstBoard;
        
        board.ActionExecutor.AddAction(new SpawnRewardPiecesAction
        {
            From = point,
            Pieces = piecesReward,
            EnabledTopHighlight = topHighlight,
            EnabledBottomHighlight = bottomHighlight,
            OnComplete = () => { OnCompleteAction(point, transactions, onComplete); }
        });
    }

    private static bool GetAllData(List<CurrencyPair> currenciesReward, CurrencyPair price, BoardPosition? position, out BoardPosition point, out List<ShopItemTransaction> transactions)
    {
        var board = BoardService.Current.FirstBoard;
        
        point = BoardPosition.Default();
        transactions = new List<ShopItemTransaction>();

        if (position == null)
        {
            var positions = board.BoardLogic.PositionsCache.GetRandomPositions(PieceTypeFilter.Character, 1);
            
            if (positions.Count == 0) return false;
            
            point = positions[0];
        }
        else
        {
            point = position.Value;
        }
        
        var from = board.BoardDef.GetPiecePosition(point.X, point.Y);
        var flayPoint = board.BoardDef.ViewCamera.WorldToScreenPoint(from);
        
        for (var i = 0; i < currenciesReward.Count; i++)
        {
            var product = currenciesReward[i];

            if (i != 0 || price == null) price = new CurrencyPair {Currency = Currency.Cash.Name, Amount = 0};
            
            var transaction = PurchaseAsync(product, price, null, flayPoint);

            if (transaction == null)
            {
                if(i == 0) return false;
                
                continue;
            }
            
            transactions.Add(transaction);
        }
        
        return true;
    }

    private static void OnCompleteAction(BoardPosition point, List<ShopItemTransaction> transactions, Action onComplete)
    {
        var board = BoardService.Current.FirstBoard;
        var view = board.RendererContext.GetElementAt(point) as CharacterPieceView;
                
        if(view != null) view.StartRewardAnimation();

        for (var i = 0; i < transactions.Count; i++)
        {
            var transaction = transactions[i];
                    
            AddResourceView.Show(point, transaction, 1.5f * i);
        }
                
        onComplete?.Invoke();
    }
}
