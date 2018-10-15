using System;
using System.Collections.Generic;
using System.Text;

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
        }
    }
    
    public void Check()
    {
        if (state != OrderState.Waiting && state != OrderState.Enough) return;
        
        State = CurrencyHellper.IsCanPurchase(Def.Prices) ? OrderState.Enough : OrderState.Waiting;
    }
}