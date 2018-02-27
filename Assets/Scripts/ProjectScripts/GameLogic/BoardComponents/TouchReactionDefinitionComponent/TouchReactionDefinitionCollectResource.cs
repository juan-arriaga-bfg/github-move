using System.Collections.Generic;

public class TouchReactionDefinitionCollectResource : TouchReactionDefinitionComponent
{
    public int Amount;
    public string CurrencyName;
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        var shopItem = new ShopItem
        {
            Uid = string.Format("add.test.{0}.{1}", CurrencyName, this.Amount), 
            ItemUid = CurrencyName, 
            Amount = this.Amount,
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
                piece.Context.ActionExecutor.PerformAction(new CollapsePieceToAction
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