using System.Collections.Generic;
using DG.Tweening;
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

        DOTween.Kill(back);
        
        var value = Mathf.Clamp(start + (1 - start) * (1 - enemy.Progress), start, 1f);
        DOTween.To(() => back.fillAmount, (v) => { back.fillAmount = v; }, value, 0.5f).SetId(back);
    }
    
    public void SpawnReward()
    {
        var positions = new List<BoardPosition>();
        var from = Context.Context.BoardDef.GetSectorPosition(CachedTransform.position);
        
        from.Z = Context.Context.BoardDef.PieceLayer;

        if (!Context.Context.BoardLogic.EmptyCellsFinder.FindRandomNearWithPointInCenter(from, positions, 1)) return;
        
        Context.Context.ActionExecutor.AddAction(new ReproductionPieceAction
        {
            BoardElement = this,
            From = from,
            Piece = enemy.ActiveReward,
            Positions = positions,
            OnComplete = () =>
            {
                if(enemy.IsComplete == false) return;
        
                DestroyOnBoard();
            }
        });

        enemy.ActiveReward = PieceType.None.Id;
        
        Context.Context.Manipulator.CameraManipulator.ZoomTo(0.3f, CachedTransform.position);
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