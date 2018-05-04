using System;
using System.Collections.Generic;
using UnityEngine;

public class TouchReactionDefinitionSimpleObstacle : TouchReactionDefinitionComponent
{
    public bool isOpen;
    private bool isClear;
    
    public int Steps { get; set; }
    private int current;
    
    public float GetProgress
    {
        get { return 1 - current/(float)Steps; }
    }
    
    public float GetProgressFake
    {
        get { return 1 - (current+1)/(float)Steps; }
    }
    
    public string GetProgressText
    {
        get { return string.Format("{0}/{1}", current, Steps); }
    }

    public CurrencyPair Price { get; private set; }
    
    public Action OnClick { get; set; }
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        if (isClear) return false;
        
        Price = GameDataService.Current.SimpleObstaclesManager.PriceForPiece(piece, current);
        
        if (OnClick != null) OnClick();
        
        var hint = piece.Context.GetComponent<HintCooldownComponent>(HintCooldownComponent.ComponentGuid);
        
        if(hint == null) return true;
        
        hint.Step(HintType.Obstacle);
        
        return true;
    }
    
    public void Clear(Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, this);

        CurrencyHellper.Purchase(Currency.Obstacle.Name, 1, Price, success =>
        {
            if(success == false) return;

            var position = piece.CachedPosition;
            
            current++;
            AddResourceView.Show(position, new CurrencyPair{Currency = "Life", Amount = -1});

            if (current != Steps)
            {
                piece.Context.ActionExecutor.AddAction(new EjectionPieceAction()
                {
                    From = position,
                    Pieces = GameDataService.Current.SimpleObstaclesManager.RewardForPiece(piece.PieceType, current)
                });
                
                return;
            }
                
            isClear = true;

            var chest = GameDataService.Current.SimpleObstaclesManager.ChestForPiece(piece.PieceType);
                
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
        });
    }
}