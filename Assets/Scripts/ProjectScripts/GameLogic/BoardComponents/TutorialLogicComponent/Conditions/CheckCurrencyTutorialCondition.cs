using UnityEngine;

public class CheckCurrencyTutorialCondition : BaseTutorialCondition
{
    public string Currensy;
    public int Target;
    
    private int current;

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        ShopService.Current.OnPurchasedEvent += Update;
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
                if(price.Currency != Currensy) continue;
                
                current -= price.DefaultPriceAmount;
            } 
            
            return;
        }

        if (shopItem.ItemUid != Currensy) return;
        
        current += shopItem.Amount;
    }
    
    public override bool Check()
    {
        return Mathf.Abs(Target) <= Mathf.Abs(current);
    }
}