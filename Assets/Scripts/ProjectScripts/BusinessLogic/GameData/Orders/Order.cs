using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public enum OrderState
{
    Waiting,
    Enough,
    InProgress,
    Complete,
    Reward
}

public class Order
{
    public const string Separator = "  ";
    public OrderDef Def;
    public int Customer;
    
    public Action OnStateChange;

    public Action<Order, OrderState, OrderState> OnStageChangeFromTo;

    public int GetAmountOfResult()
    {
        return 1;
    }
    
    private Dictionary<int, int> result;
    public Dictionary<int, int> Result
    {
        get
        {
            if (result == null)
            {
                result = new Dictionary<int, int>();
                result.Add(PieceType.Parse(Def.Uid), 1);
            }
            
            return result;
        }
    }
    
    public string Reward
    {
        get
        {
            var piecesReward = CurrencyHelper.FiltrationRewards(Def.Rewards, out var currenciesReward);
            
            return CurrencyHelper.RewardsToString(Separator, piecesReward, currenciesReward);;
        }
    }

    private OrderState state;
    public OrderState State
    {
        get { return state; }
        set
        {
            var prevState = state;
            state = value;
            OnStateChange?.Invoke();
            OnStageChangeFromTo?.Invoke(this, prevState, state);

            ResourcesViewManager.Instance.GetFirstViewById(Currency.Order.Name)?.UpdateResource(0);
        }
    }

    public void SetMark(Image mark, GameObject clock = null)
    {
        switch (state)
        {
            case OrderState.Enough:
                mark.gameObject.SetActive(true);
                mark.sprite = IconService.Current.GetSpriteById("icon_Warning");
                break;
            case OrderState.Complete:
                mark.gameObject.SetActive(true);
                mark.sprite = IconService.Current.GetSpriteById("icon_Complete");
                break;
            default:
                mark.gameObject.SetActive(false);
                break;
        }

        if (clock == null) return;
        
        clock.SetActive(state == OrderState.InProgress);
    }
    
    public void Check()
    {
        if (state != OrderState.Waiting && state != OrderState.Enough) return;
        
        State = CurrencyHelper.IsCanPurchase(Def.Prices) ? OrderState.Enough : OrderState.Waiting;
    }
}