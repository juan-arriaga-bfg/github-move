using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerComponent : ECSEntity, IPieceBoardObserver
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    public Order Order;
    
    public TimerComponent Timer { get; private set; }
    public TimerComponent Cooldown { get; private set; }
    
    private Piece pieceContext;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        pieceContext = entity as Piece;
        
        Timer = pieceContext.GetComponent<TimerComponent>(TimerComponent.ComponentGuid);
        Timer.OnComplete += UpdateView;
        Timer.OnComplete += () => UpdateState(OrderState.Complete);
        
        int delay = GameDataService.Current.LevelsManager.OrdersDelay;
        
        Cooldown = new TimerComponent {Delay = delay};
        Cooldown.OnComplete += CreateOrder;

        Debug.Log($"[CustomerComponent] => OnRegisterEntity: Start cooldown: delay: {delay}");
        
        RegisterComponent(Cooldown);
    }

    public void OnAddToBoard(BoardPosition position, Piece context = null)
    {
        InitInSave();
    }

    public void OnMovedFromToStart(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnMovedFromToFinish(BoardPosition @from, BoardPosition to, Piece context = null)
    {
    }

    public void OnRemoveFromBoard(BoardPosition position, Piece context = null)
    {
        Timer.OnComplete = null;
        Cooldown.OnComplete = null;
    }

    private void InitInSave()
    {
        var save = ProfileService.Current.GetComponent<OrdersSaveComponent>(OrdersSaveComponent.ComponentGuid);
        var item = save?.Orders?.Find(o => o.Customer == pieceContext.PieceType);

        if (item == null)
        {
            if(GameDataService.Current.OrdersManager.CheckStart()) Cooldown.Start();
            return;
        }
        
        Order = GameDataService.Current.OrdersManager.Orders.Find(o => o.Customer == pieceContext.PieceType);
        
        Timer.Delay = Order.Def.Delay;
        
        if(item.IsStart) Timer.Start(item.StartTime);
        if(item.IsStartCooldown) Cooldown.Start(item.CooldownTime);

        Order.State = item.State;
        
        UpdateView();
        
        save.Orders.Remove(item);
        
        if(save.Orders.Count > 0) return;
        
        save.Orders = null;
    }
    
    private void UpdateView()
    {
        if(Order == null || pieceContext.ViewDefinition == null) return;
        
        var view = pieceContext.ViewDefinition.AddView(ViewType.OrderBubble) as OrderBubbleView;
        var isShow = Order.State != OrderState.Reward;
        
        view.UpdateIcon();
        
        if(isShow && view.IsShow) return;
        
        view.Priority = isShow ? -1 : 1;
        view.Change(isShow);
        
        if (isShow)
        {
            pieceContext.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
            pieceContext.Context.HintCooldown.AddView(view);
            return;
        }
        
        pieceContext.Context.HintCooldown.RemoweView(view);
    }
    
    private void CreateOrder()
    {
        NSAudioService.Current.Play(SoundId.OrderAppear);
        
        Order = GameDataService.Current.OrdersManager.GetOrder(pieceContext.PieceType);
        
        if(Order == null) return;
        
        Timer.Delay = Order.Def.Delay;
        UpdateView();
    }

    public void GetReward()
    {
        CurrencyHelper.PurchaseAndProvideEjection(Order.PiecesReward, Order.CurrenciesReward, null, pieceContext.CachedPosition, () => { Order = null; });
        GameDataService.Current.OrdersManager.RemoveOrder(Order, pieceContext.Context.BoardLogic);
        
        Order.State = OrderState.Reward;
        UpdateView();
    }

    public void UpdateState(OrderState state)
    {
        if(Order == null) return;
        
        if(Order.State == OrderState.Init && state == OrderState.Waiting) Order.State = CurrencyHelper.IsCanPurchase(Order.Def.Prices) ? OrderState.Enough : OrderState.Waiting;
        if(Order.State == OrderState.InProgress && state == OrderState.Complete) Order.State = state;
    }
    
    public void Buy()
    {
        CurrencyHelper.Purchase(new CurrencyPair{Currency = Currency.Order.Name, Amount = 1}, Order.Def.Prices,
            success =>
            {
                GameDataService.Current.OrdersManager.UpdateOrders();
                Order.State = OrderState.InProgress;
                Timer?.Start();
            });
    }
    
    public void Exchange()
    {
        
        CurrencyPair price = null;
        List<CurrencyPair> diff;
        
        CurrencyHelper.IsCanPurchase(Order.Def.Prices, out diff);
        
        foreach (var pair in diff)
        {
            var def = GameDataService.Current.PiecesManager.GetPieceDef(PieceType.Parse(pair.Currency));
            
            if(def?.ExchangePrice == null) continue;
            if (price == null) price = new CurrencyPair {Currency = def.ExchangePrice.Currency};

            price.Amount += pair.Amount * def.ExchangePrice.Amount;
        }
        
        var model = UIService.Get.GetCachedModel<UIExchangeWindowModel>(UIWindowType.ExchangeWindow);
        
        model.Products = diff;
        model.Price = price;
        model.OnClick = () =>
        {
            NSAudioService.Current.Play(SoundId.TimeBoost);
            Buy();
        };
        
        UIService.Get.ShowWindow(UIWindowType.ExchangeWindow);
    }

    public void RestartCooldown()
    {
        int delay = GameDataService.Current.LevelsManager.OrdersDelay;
        Cooldown.Delay = delay;
        Cooldown.Start();
        
        Debug.Log($"[CustomerComponent] => RestartCooldown: delay: {delay}");
    }
}