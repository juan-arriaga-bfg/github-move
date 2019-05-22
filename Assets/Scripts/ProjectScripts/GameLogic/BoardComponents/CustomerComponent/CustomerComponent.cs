using Debug = IW.Logger;
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
        
        var delay = GameDataService.Current.LevelsManager.OrdersDelay;
        
        Cooldown = new TimerComponent {Delay = delay};
        Cooldown.OnComplete += CreateOrder;

        Debug.Log($"[CustomerComponent] => OnRegisterEntity: Start cooldown: delay: {delay}");
        
        RegisterComponent(Cooldown);
        
        LocalNotificationsService.Current.RegisterNotifier(new Notifier(Timer, NotifyType.OrderComplete));
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        Timer.OnComplete = null;
        Cooldown.OnComplete = null;
        
        base.OnUnRegisterEntity(entity);
        
        LocalNotificationsService.Current.UnRegisterNotifier(Timer);
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
        Order.Check();
        
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

        if (!isShow) return;
        pieceContext.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
    }
    
    private void CreateOrder()
    {
        if (GameDataService.Current.OrdersManager.GetOrder(pieceContext.PieceType, out Order)) RestartCooldown();
        if (Order == null) return;
        
        NSAudioService.Current.Play(SoundId.OrderAppear);
        
        Timer.Delay = Order.Def.Delay;
        UpdateState(OrderState.Waiting);
        UpdateView();
    }

    public void GetReward()
    {
        CurrencyHelper.PurchaseAndProvideEjection(Order.Result, new List<CurrencyPair>(), null, pieceContext.CachedPosition, () => { Order = null; }, SelectReward);
        
        Order.State = OrderState.Reward;
        
        GameDataService.Current.OrdersManager.RemoveOrder(Order, pieceContext.Context.BoardLogic);
        
        UpdateView();
    }

    private void SelectReward(List<BoardPosition> positions)
    {
        if(positions == null || positions.Count == 0 || pieceContext.Context.Manipulator.CameraManipulator.CameraMove.IsLocked) return;
        
        var position = positions[0];
        var target = pieceContext.Context.BoardDef.GetSectorCenterWorldPosition(position.X, position.Up.Y, position.Z);
        
        pieceContext.Context.Manipulator.CameraManipulator.MoveTo(target);
    }

    public void UpdateState(OrderState state)
    {
        if(Order == null) return;
        
        if(Order.State == OrderState.Waiting && state == OrderState.Waiting) Order.State = CurrencyHelper.IsCanPurchase(Order.Def.Prices) ? OrderState.Enough : OrderState.Waiting;
        if(Order.State == OrderState.InProgress && state == OrderState.Complete) Order.State = state;
    }
    
    public void Buy()
    {
        CurrencyHelper.Purchase(new CurrencyPair{Currency = Currency.Order.Name, Amount = 1}, Order.Def.Prices,
            success =>
            {
                GameDataService.Current.OrdersManager.UpdateOrders();
                Timer?.Start();
                Order.State = OrderState.InProgress;
                
                ProfileService.Instance.Manager.UploadCurrentProfile(false);
            });
    }
    
    public bool Exchange()
    {
        CurrencyPair price = null;
        List<CurrencyPair> diff;
        
        if (CurrencyHelper.IsCanPurchase(Order.Def.Prices, out diff)) return false;
        
        foreach (var pair in diff)
        {
            var def = GameDataService.Current.PiecesManager.GetPieceDef(PieceType.Parse(pair.Currency));

            if (def?.ExchangePrice == null) continue;
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
        return true;
    }

    public void RestartCooldown()
    {
        var delay = GameDataService.Current.LevelsManager.OrdersDelay;
        
        Cooldown.Delay = delay;
        Cooldown.Start();
        
        Debug.Log($"[CustomerComponent] => RestartCooldown: delay: {delay}");
    }
}