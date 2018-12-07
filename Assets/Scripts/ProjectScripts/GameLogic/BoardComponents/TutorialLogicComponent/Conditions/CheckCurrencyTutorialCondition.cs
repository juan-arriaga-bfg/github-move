using System.Collections.Generic;
using UnityEngine;

public class CheckCurrencyTutorialCondition : BaseTutorialCondition
{
    public List<string> Currency;
    public int Target;
    
    private int current;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        ShopService.Current.OnPurchasedEvent += Update;

        if (Target < 0) return;
        
        foreach (var name in Currency)
        {
            current += ProfileService.Current.GetStorageItem(name).Amount;
        }
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        base.OnUnRegisterEntity(entity);
        ShopService.Current.OnPurchasedEvent -= Update;
    }

    private void Update(IPurchaseableItem purchaseableItem, IShopItem shopItem)
    {
        if (ConditionType == TutorialConditionType.Start)
        {
            Increment(shopItem);
            return;
        }
        
        if(context.IsPerform == false) return;
        
        Increment(shopItem);
    }

    private void Increment(IShopItem shopItem)
    {
        if (Target < 0)
        {
            foreach (var price in shopItem.CurrentPrices)
            {
                if(Currency.Contains(price.Currency) == false) continue;
                
                current -= price.DefaultPriceAmount;
            } 
            
            return;
        }
        
        if(Currency.Contains(shopItem.ItemUid) == false) return;
        
        current += shopItem.Amount;
    }
    
    public override bool Check()
    {
        return Mathf.Abs(Target) <= Mathf.Abs(current);
    }
}