using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public enum OrderState
{
    Init,
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

    private Dictionary<int, int> piecesReward;
    public Dictionary<int, int> PiecesReward
    {
        get
        {
            if (piecesReward == null)
            {
                InitReward();
            }
            
            return piecesReward;
        }
    }
    
    private List<CurrencyPair> currencysReward;
    public List<CurrencyPair> CurrenciesReward
    {
        get
        {
            if (currencysReward == null)
            {
                InitReward();
            }
            
            return currencysReward;
        }
    }

    private void InitReward()
    {
        piecesReward = CurrencyHelper.FiltrationRewards(Def.Rewards, out currencysReward);
    }
    
    private string reward;
    public string Reward
    {
        get
        {
            if (string.IsNullOrEmpty(reward) == false) return reward;
            
            reward = CurrencyHelper.RewardsToString(Separator, PiecesReward, CurrenciesReward);
            return reward;
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