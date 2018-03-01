using System.Collections.Generic;

public class TouchReactionDefinitionCollectResource : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var storage = piece.GetComponent<ResourceStorageComponent>(ResourceStorageComponent.ComponentGuid);

        if (storage == null || storage.Resources == null || storage.Resources.Amount == 0) return false;
        
        var shopItem = new ShopItem
        {
            Uid = string.Format("add.test.{0}.{1}", storage.Resources.Currency, storage.Resources.Amount), 
            ItemUid = storage.Resources.Currency, 
            Amount = storage.Resources.Amount,
            CurrentPrices = new List<Price>
            {
                new Price{Currency = Currency.Cash.Name, DefaultPriceAmount = 0}
            }
        };
        
        ShopService.Current.PurchaseItem
        ( 
            shopItem,
            (item, s) =>
            {
                // on purchase success
                piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
                {
                    To = position,
                    Positions = new List<BoardPosition>{position}
                });
            },
            item =>
            {
                // on purchase failed (not enough cash)
            }
        );
        
        return true;
    }
}