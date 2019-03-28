using System;
using System.Collections.Generic;

public class TouchReactionDefinitionOpenEnemyBubble : TouchReactionDefinitionComponent
{
    public Action<bool> OnChange;
    
    private BubbleView view;
    private EnemyDef enemyDef;
    
    public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
    {
        return viewDefinition != null && viewDefinition.AddView(ViewType.Bubble).IsShow;
    }
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, position);

        if (piece.ViewDefinition == null) return false;

        view = piece.ViewDefinition.AddView(ViewType.Bubble) as BubbleView;

        enemyDef = GameDataService.Current.EnemiesManager.GetEnemyDefById(piece.PieceType);
        
        var button = string.Format(LocalizationService.Get("gameboard.bubble.button.eradicate", "gameboard.bubble.button.eradicate {0}"), enemyDef.Price.ToStringIcon());
        
        view.SetData($"{enemyDef.Name}", button, OnClick, true, false);

        OnChange?.Invoke(!view.IsShow);
        view.Change(!view.IsShow);

        if (view.IsShow)
        {
            view.FitToScreen();
        }
        
        return true;
    }
    
    private void OnClick(Piece piece)
    {
        view.OnHide = () =>
        {
            CurrencyHelper.Purchase(Currency.Damage.Name, 1, enemyDef.Price, success =>
            {
                if (success == false)
                {
                    return;
                }
			
                piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
                {
                    To = piece.CachedPosition,
                    Positions = new List<BoardPosition>{piece.CachedPosition}
                });

                ProvideReward(piece);
            });
        };
        
        view.Change(false);
    }

    private void ProvideReward(Piece piece)
    {
        AddResourceView.Show(piece.CachedPosition, enemyDef.Reward, 0.7f);
    }
}