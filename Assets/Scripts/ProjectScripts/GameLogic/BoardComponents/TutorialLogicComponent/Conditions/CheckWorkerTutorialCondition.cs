using System.Collections.Generic;

public class CheckWorkerTutorialCondition : BaseTutorialCondition
{
    private bool isCheck;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        ShopService.Current.OnPurchasedEvent += Update;
    }
    
    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        ShopService.Current.OnPurchasedEvent -= Update;
        base.OnUnRegisterEntity(entity);
    }
    
    private void Update(IPurchaseableItem purchasableItem, IShopItem shopItem)
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
        var board = context.Context.Context;
        
        if(isCheck || board.WorkerLogic.Last == null) return;
        
        var ids = (context as SelectStorageTutorialStep).Targets;
        var targets = new List<BoardPosition>();
        
        foreach (var id in ids)
        {
            targets.AddRange(board.BoardLogic.PositionsCache.GetUnlockedPiecePositionsByType(id));
        }
        
        foreach (var price in shopItem.CurrentPrices)
        {
            if(price.Currency != Currency.Worker.Name || targets.Contains(board.WorkerLogic.Last.Value) == false) continue;

            isCheck = true;
            return;
        }
    }
    
    public override bool Check()
    {
        return isCheck;
    }
}
