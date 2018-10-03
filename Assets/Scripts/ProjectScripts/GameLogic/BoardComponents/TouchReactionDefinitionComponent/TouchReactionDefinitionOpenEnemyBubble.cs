using System;
using System.Collections.Generic;
using DG.Tweening;

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

                ProvideReward(piece);
            });
        };
        
        view.Change(false);
    }

    private void ProvideReward(Piece piece)
    {
        DOTween.Sequence()
               .AppendInterval(0.7f)
               .AppendCallback(() =>
                {
                    AddResourceView.Show(piece.CachedPosition, enemyDef.Reward);
                });
    }
}