using System;
using System.Collections.Generic;
using System.Text;
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

    private string reward;
    public string Reward
    {
        get
        {
            if (string.IsNullOrEmpty(reward) == false) return reward;

            var str = new StringBuilder();
            
            for (var i = 0; i < Def.Rewards.Count; i++)
            {
                if (i != 0) str.Append(Separator);
                str.Append(Def.Rewards[i].ToStringIcon(false));
            }
            
            reward = str.ToString();
            return reward;
        }
    }

    private OrderState state;
    public OrderState State
    {
        get { return state; }
        set
        {
            state = value;
            OnStateChange?.Invoke();

            ResourcesViewManager.Instance.GetFirstViewById(Currency.Order.Name)?.UpdateResource(0);
        }
    }

    public void SetMark(Image mark)
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
    }
    
    public void Check()
    {
        if (state != OrderState.Waiting && state != OrderState.Enough) return;
        
        State = CurrencyHellper.IsCanPurchase(Def.Prices) ? OrderState.Enough : OrderState.Waiting;
    }
}