using System;
using System.Collections.Generic;

public class TouchReactionDefinitionSimpleObstacle : TouchReactionDefinitionComponent
{
    public bool isOpen;
    private bool isClear;
    
    public Action OnClick { get; set; }
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        if (isClear) return false;
        
        if (isOpen)
        {
            Clear(position, piece);
            return true;
        }
        
        if (OnClick != null) OnClick();
        return true;
    }
    
    private void Clear(BoardPosition position, Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, this);
        
        var price = GameDataService.Current.ObstaclesManager.SimpleObstaclePrice;
        
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", Currency.Obstacle.Name), 
            ItemUid = Currency.Obstacle.Name, 
            Amount = 1,
            CurrentPrices = new List<Price>
            {
                new Price{Currency = price.Currency, DefaultPriceAmount = price.Amount}
            }
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success
                
                isClear = true;
                
                piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
                {
                    To = position,
                    Positions = new List<BoardPosition>{position},
                    OnComplete = () =>
                    {
                        piece.Context.ActionExecutor.AddAction(new SpawnPieceAtAction()
                        {
                            IsCheckMatch = false,
                            At = position,
                            PieceTypeId = PieceType.Chest1.Id
                        });
                    }
                });
            },
            item =>
            {
                // on purchase failed (not enough cash)
                UIMessageWindowController.CreateDefaultMessage("Not enough coins!");
            }
        );
    }
}