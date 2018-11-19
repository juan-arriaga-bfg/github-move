using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIOrdersSelectItem : UISimpleScrollItem
{
    [SerializeField] private List<UISimpleScrollItem> priceItems;
    
    [SerializeField] protected Image iconOrder;
    
    [SerializeField] protected NSText timer;
    
    [SerializeField] protected NSText btnCompleteLabel;
    [SerializeField] protected NSText btnBuyLabel;
    
    [SerializeField] protected GameObject goTimer;
    
    [SerializeField] protected GameObject btnComplete;
    [SerializeField] protected GameObject btnBuy;
    [SerializeField] protected GameObject shine;

    private Order order;
    private CustomerComponent customer;
    private bool isComplete;
    
    public void Init(Order order)
    {
        RemoveListeners();
        
        this.order = order;
        isComplete = false;
        
        Init(PieceType.Parse(order.Customer), string.Format(LocalizationService.Instance.Manager.GetTextByUid("common.message.reward", "Reward:{0}"), $"{Order.Separator}{order.Reward}"));
        
        iconOrder.sprite = IconService.Current.GetSpriteById(order.Def.Uid);
        
        for (var i = 0; i < priceItems.Count; i++)
        {
            var isExcess = i >= order.Def.Prices.Count;
            var item = priceItems[i];
            
            item.SetActive(!isExcess);
            
            if(isExcess) continue;
            
            var price = order.Def.Prices[i];
            var current = ProfileService.Current.GetStorageItem(price.Currency).Amount;
            var color = current == price.Amount ? "FFFFFF" : (current > price.Amount ? "28EC6D" : "EC5928"); 
            
            item.Init(price.Currency, $"<color=#{color}>{current}</color><size=45>/{price.Amount}</size>");
        }
        
        var board = BoardService.Current.GetBoardById(0);
        var position = board.BoardLogic.PositionsCache.GetRandomPositions(order.Customer, 1)[0];
        
        customer = board.BoardLogic.GetPieceAt(position)?.GetComponent<CustomerComponent>(CustomerComponent.ComponentGuid);
        
        if (customer.Timer != null)
        {
            customer.Timer.OnExecute += UpdateTimer;
            customer.Timer.OnComplete += UpdateState;
        }

        order.OnStateChange += UpdateState;
        UpdateState();
    }

    public override void OnViewCloseCompleted()
    {
        RemoveListeners();

        if (isComplete == false)
        {
            customer = null;
            return;
        }

        isComplete = false;
        customer.GetReward();
        customer = null;
    }

    private void RemoveListeners()
    {
        if (customer?.Timer != null)
        {
            customer.Timer.OnExecute -= UpdateTimer;
            customer.Timer.OnComplete -= UpdateState;
        }
        
        if(order != null) order.OnStateChange -= UpdateState;
    }

    private void UpdateTimer()
    {
        timer.Text = customer.Timer.CompleteTime.GetTimeLeftText();
        btnBuyLabel.Text = customer.Timer.GetPrise().ToStringIcon(false);
    }
    
    private void UpdateState()
    {
        btnCompleteLabel.Text = order.State != OrderState.Complete
            ? LocalizationService.Instance.Manager.GetTextByUid("common.button.produce", "Produce")
            : LocalizationService.Instance.Manager.GetTextByUid("common.button.claim", "Claim!");
        
        goTimer.SetActive(order.State == OrderState.InProgress);
        btnBuy.SetActive(order.State == OrderState.InProgress);
        btnComplete.SetActive(order.State != OrderState.InProgress);
        shine.SetActive(order.State == OrderState.Complete);
        
        for (var i = 0; i < priceItems.Count; i++)
        {
            var isExcess = i >= order.Def.Prices.Count;
            
            if(isExcess) continue;

            var item = priceItems[i];
            var price = order.Def.Prices[i];

            if (order.State == OrderState.InProgress || order.State == OrderState.Complete)
            {
                item.Init(price.Currency, "");
                item.Alpha = 0.7f;
                continue;
            }
            
            var current = ProfileService.Current.GetStorageItem(price.Currency).Amount;
            
            if (current >= price.Amount)
            {
                item.Init(price.Currency, $"<sprite name={OrderState.Complete}>");
            }
        }
        
        (context as UIOrdersWindowView).UpdateOrders();
    }

    public void OnClickBuy()
    {
        customer.Timer.FastComplete();
    }

    public void OnClick()
    {
        if (order.State == OrderState.Waiting)
        {
            customer.Exchange();
            return;
        }

        if (order.State == OrderState.Enough)
        {
            customer.Buy();
            return;
        }

        if (order.State == OrderState.Complete)
        {
            var board = BoardService.Current.GetBoardById(0);
            var position = board.BoardLogic.PositionsCache.GetRandomPositions(order.Customer, 1)[0];
            
            if(!board.BoardLogic.EmptyCellsFinder.CheckFreeSpaceNearPosition(position, order.PiecesReward.Sum(e => e.Value)))
            {
                UIErrorWindowController.AddError(LocalizationService.Instance.Manager.GetTextByUid("message.error.freeSpace", "Free space not found!"));
                return;
            }
            
            isComplete = true;
            context.Controller.CloseCurrentWindow();
        }
    }
}