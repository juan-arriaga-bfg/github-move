using System;
using System.Collections.Generic;
using UnityEngine;

public class TouchReactionDefinitionSimpleObstacle : TouchReactionDefinitionComponent
{
    public bool isOpen;
    private bool isClear;
    
    private int max;
    private int current = -1;
    
    public float GetProgress
    {
        get { return 1 - current/(float)max; }
    }
    
    public string GetProgressText
    {
        get { return string.Format("{0}/{1}", current, max); }
    }

    public CurrencyPair Price { get; private set; }
    
    public Action OnClick { get; set; }
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        if (current == -1) Init(piece);
        
        if (isClear) return false;
        
        Price = GameDataService.Current.SimpleObstaclesManager.PriceForPiece(piece, current);
        
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
        
        var shopItem = new ShopItem
        {
            Uid = string.Format("purchase.test.{0}.10", Currency.Obstacle.Name), 
            ItemUid = Currency.Obstacle.Name, 
            Amount = 1,
            CurrentPrices = new List<Price>
            {
                new Price{Currency = Price.Currency, DefaultPriceAmount = Price.Amount}
            }
        };
        
        ShopService.Current.PurchaseItem
        (
            shopItem,
            (item, s) =>
            {
                // on purchase success
                
                current++;
                HitboxDamageView.Show(position, 1);
                
                if (current != max) return;
                
                isClear = true;

                var chest = GameDataService.Current.SimpleObstaclesManager.RewardForPiece(piece.PieceType);
                
                if(chest == PieceType.None.Id) return;
                
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
                            PieceTypeId = chest
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

    private void Init(Piece piece)
    {
        max = piece.Context.BoardLogic.MatchDefinition.GetIndexInChain(piece.PieceType);
        current = 0;
    }
}