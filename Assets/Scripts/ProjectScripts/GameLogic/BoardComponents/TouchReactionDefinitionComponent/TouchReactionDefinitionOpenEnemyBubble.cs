using System;
using System.Collections.Generic;

public class TouchReactionDefinitionOpenEnemyBubble : TouchReactionDefinitionComponent
{
    public ViewType ViewId;
    public Action<bool> OnChange;
	
    private ViewDefinitionComponent viewDef;

    private BubbleView view;
    private EnemyDef enemyDef;
    
    public override bool IsViewShow(ViewDefinitionComponent viewDefinition)
    {
        return viewDefinition != null && viewDefinition.AddView(ViewId).IsShow;
    }
    
    public override bool Make(BoardPosition position, Piece piece)
    {
        piece.Context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, position);

        if (viewDef == null)
        {
            viewDef = piece.GetComponent<ViewDefinitionComponent>(ViewDefinitionComponent.ComponentGuid);

            if (viewDef == null) return false;
        }

        view = viewDef.AddView(ViewId) as BubbleView;

        enemyDef = GameDataService.Current.EnemiesManager.GetEnemyDefById(piece.PieceType);

        // ReSharper disable once PossibleNullReferenceException
        view.SetData($"{enemyDef.Name}", $"Erradicate <sprite name={enemyDef.Price.Currency}> {enemyDef.Price.Amount}", OnClick, true, false);

        OnChange?.Invoke(!view.IsShow);
        view.Change(!view.IsShow);

        return true;

    }
    
    private void OnClick(Piece piece)
    {
        view.OnHide = () =>
        {
            CurrencyHellper.Purchase(Currency.Damage.Name, 1, enemyDef.Price, success =>
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
            });
        };
        
        view.Change(false);
    }
}