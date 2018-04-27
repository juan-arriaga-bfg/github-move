using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyView : BoardElementView
{
    [SerializeField] private Image back;

    private Enemy enemy;
    private float start = 0.36f;
    
    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        enemy = null;
    }

    public void UpdateFill()
    {
        if(enemy == null) return;

        back.fillAmount = Mathf.Clamp(start + (1 - start) * (1 - enemy.Progress), start, 1f);
    }

    public void SpawnReward(int reward)
    {
        var positions = new List<BoardPosition>();
        var from = Context.Context.BoardDef.GetSectorPosition(CachedTransform.position);
        
        from.Z = Context.Context.BoardDef.PieceLayer;

        if (!Context.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(@from, positions, 1)) return;
        
        Context.Context.ActionExecutor.AddAction(new ReproductionPieceAction
        {
            BoardElement = this,
            From = @from,
            Piece = reward,
            Positions = positions,
            OnComplete = () =>
            {
                if(enemy.IsComplete == false) return;
        
                DestroyOnBoard();
            }
        });
    }

    public void OnClick()
    {
        var model = UIService.Get.GetCachedModel<UIRobberyWindowModel>(UIWindowType.RobberyWindow);

        model.Enemy = enemy;
        model.View = this;
        
        UIService.Get.ShowWindow(UIWindowType.RobberyWindow);
    }
    
    public static void Show(BoardPosition position, Enemy enemy)
    {
        var board = BoardService.Current.GetBoardById(0);
        var view = board.RendererContext.CreateBoardElementAt<EnemyView>(R.EnemyView, position);

        view.enemy = enemy;
    }
}