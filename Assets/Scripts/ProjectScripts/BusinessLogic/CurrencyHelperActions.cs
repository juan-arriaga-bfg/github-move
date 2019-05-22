using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class CurrencyHelper
{
    public static Vector3 FlyPosition;
    
    public static void PurchaseAsyncOnlyCurrency(CurrencyPair product, CurrencyPair price, Vector3 flyPosition, Action<bool> onSuccess)
    {
        var transaction = PurchaseAsync(product, price, success =>
        {
            if (success) PlaySoundOnPurchase(new List<CurrencyPair> {product});
            
            onSuccess?.Invoke(success);
        }, flyPosition);

        transaction.Complete();
    }

    public static void PurchaseAsyncOnlyCurrency(List<CurrencyPair> products, CurrencyPair price, Vector3 flyPosition, Action<bool> onSuccess)
    {
        if (price != null) Purchase(new CurrencyPair {Currency = Currency.Cash.Name, Amount = 0}, price);
        PurchaseAsyncOnlyCurrency(products, flyPosition, onSuccess);
    }
    
    public static void PurchaseAsyncOnlyCurrency(List<CurrencyPair> products, Vector3 flyPosition, Action<bool> onSuccess)
    {
        var transactions = PurchaseAsync(products, success =>
        {
            if (success) PlaySoundOnPurchase(products);
            
            onSuccess?.Invoke(success);
        }, flyPosition);

        foreach (var transaction in transactions)
        {
            transaction.Complete();
        }
    }
    
    private static void PlaySoundOnPurchase(List<CurrencyPair> products)
    {
        foreach (var product in products)
        {
            if(product.Currency == Currency.Energy.Name)
                NSAudioService.Current.Play(SoundId.BuyEnergy, false, 1);
            if(product.Currency == Currency.Coins.Name)
                NSAudioService.Current.Play(SoundId.BuySoftCurr, false, 1);    
        }
    }
    
    public static void PurchaseAndProvideEjection(List<CurrencyPair> products, CurrencyPair price = null, BoardPosition? position = null, Action onComplete = null)
    {
        var piecesReward = FiltrationRewards(products, out var currenciesReward);
        
        PurchaseAndProvideEjection(piecesReward, currenciesReward, price, position, onComplete, null);
    }
    
    public static void PurchaseAndProvideEjection(Dictionary<int, int> piecesReward, List<CurrencyPair> currenciesReward, CurrencyPair price = null, BoardPosition? position = null, Action onComplete = null, Action<List<BoardPosition>> OnSuccess = null)
    {
        if (GetAllData(currenciesReward, price, position, out var point, out var transactions) == false) return;
        
        var board = BoardService.Current.FirstBoard;
        
        board.ActionExecutor.AddAction(new EjectionPieceAction
        {
            From = point,
            Pieces = piecesReward,
            OnComplete = () => { OnCompleteAction(point, transactions, onComplete); },
            OnSuccess = OnSuccess
        });
    }

    public static void PurchaseAndProvideSpawn(List<CurrencyPair> products, CurrencyPair price, BoardPosition? position, Vector3 flyPosition, Action onComplete = null, bool topHighlight = false, bool bottomHighlight = false)
    {
        var piecesReward = FiltrationRewards(products, out var currenciesReward);
        
        PurchaseAndProvideSpawn(piecesReward, currenciesReward, price, position, flyPosition, onComplete, topHighlight, bottomHighlight);
    }
    
    public static void PurchaseAndProvideSpawn(Dictionary<int, int> piecesReward, List<CurrencyPair> currenciesReward, CurrencyPair price, BoardPosition? position, Vector3 flyPosition, Action onComplete = null, bool topHighlight = false, bool bottomHighlight = false)
    {
        if (piecesReward.Count == 0)
        {
            PurchaseAsyncOnlyCurrency(currenciesReward, price, flyPosition, success => onComplete?.Invoke());
            return;
        }
        
        var board = BoardService.Current.FirstBoard;
        var isDataFalse = GetAllData(currenciesReward, price, position, out var point, out var transactions) == false;
        
        var amount = piecesReward.Sum(pair => pair.Value);
        var free = new List<BoardPosition>();

        if (isDataFalse || board.BoardLogic.EmptyCellsFinder.CheckInFrontOrFindRandomNear(point, free, amount) == false || free.Count < amount)
        {
            board.BoardLogic.AirShipLogic.Add(new Dictionary<int, int>(piecesReward));
            PurchaseAsyncOnlyCurrency(currenciesReward, price, flyPosition, success => onComplete?.Invoke());
        }
        
        if (isDataFalse) return;
        
        board.ActionExecutor.PerformAction(new SpawnRewardPiecesAction
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

        if (currenciesReward.Count == 0)
        {
            if (price != null) Purchase(new CurrencyPair {Currency = Currency.Cash.Name, Amount = 0}, price);
            return true;
        }
        
        for (var i = 0; i < currenciesReward.Count; i++)
        {
            var product = currenciesReward[i];

            if (i != 0 || price == null) price = new CurrencyPair {Currency = Currency.Cash.Name, Amount = 0};
            
            var transaction = PurchaseAsync(product, price, null, flayPoint);

            if (transaction == null)
            {
                if (i == 0) return false;
                
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
