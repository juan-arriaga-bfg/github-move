using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyView : BoardElementView
{
    [SerializeField] private Image back;

    private Enemy enemy;
    private float start = 0.36f;

    private FogDef fog;
    private BoardPosition target;
    
    public void Go(FogDef fog, bool isFast = false)
    {
        DOTween.Kill(CachedTransform);
        
        this.fog = fog;
        
        var next = new BoardPosition(fog.Position.X + Random.Range(0, fog.Size.X), fog.Position.Y + Random.Range(0, fog.Size.Y));

        if (target.Equals(next))
        {
            Go(this.fog);
            return;
        }
        
        var position = Context.Context.BoardDef.GetSectorCenterWorldPosition(next.X, next.Y, 0);
        var duration = isFast ? 0.2f : BoardPosition.SqrMagnitude(target, next) * 2;
        
        target = next;
        
        var sequence = DOTween.Sequence().SetId(CachedTransform).SetEase(Ease.Linear);

        sequence.Append(CachedTransform.DOMove(position, duration));
        sequence.OnUpdate(GoOther);
        sequence.InsertCallback(duration - 0.1f, () => Go(this.fog));
    }

    private void GoOther()
    {
        if (GameDataService.Current.FogsManager.GetDef(fog.Position) != null)
        {
            return;
        }
        
        var board = BoardService.Current.GetBoardById(0);
        var position = Context.Context.BoardDef.GetSectorCenterWorldPosition(target.X, target.Y, 0);
        
        var distances = new List<KeyValuePair<float, FogDef>>();

        foreach (var def in GameDataService.Current.FogsManager.FogPositions.Values)
        {
            distances.Add(new KeyValuePair<float, FogDef>(Vector2.Distance(position, def.GetCenter(board)), def));
        }

        distances.Sort((a, b) => a.Key.CompareTo(b.Key));
        
        Go(distances[0].Value, true);
    }

    public override void ResetViewOnDestroy()
    {
        base.ResetViewOnDestroy();
        enemy = null;
        DOTween.Kill(CachedTransform);
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

        if (enemy.IsComplete)
        {
            GameDataService.Current.EnemiesManager.Kill(enemy.Def.Uid);
        }
        
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
        
        Context.Context.Manipulator.CameraManipulator.ZoomTo(0.6f, CachedTransform.position);
    }
    
    public void OnClick()
    {
        var model = UIService.Get.GetCachedModel<UIRobberyWindowModel>(UIWindowType.RobberyWindow);

        model.Enemy = enemy;
        model.View = this;
        
        UIService.Get.ShowWindow(UIWindowType.RobberyWindow);
    }
    
    public static void Show(Enemy enemy)
    {
        var board = BoardService.Current.GetBoardById(0);
        
        var castlePosition = GameDataService.Current.PiecesManager.CastlePosition;
        var castleWorldPos = board.BoardDef.GetSectorCenterWorldPosition(castlePosition.X, castlePosition.Y, 0);
        
        var distances = new List<KeyValuePair<float, FogDef>>();

        foreach (var def in GameDataService.Current.FogsManager.FogPositions.Values)
        {
            distances.Add(new KeyValuePair<float, FogDef>(Vector2.Distance(castleWorldPos, def.GetCenter(board)), def));
        }

        distances.Sort((a, b) => a.Key.CompareTo(b.Key));
		
        if(distances.Count == 0) return;

        var nearest = distances[Random.Range(0, distances.Count < 3 ? distances.Count : 3)].Value;
        var start = new BoardPosition(nearest.Position.X + Random.Range(0, nearest.Size.X), nearest.Position.Y + Random.Range(0, nearest.Size.Y));
        var view = board.RendererContext.CreateBoardElementAt<EnemyView>(R.EnemyView, start);
        
        var position = board.BoardDef.GetSectorCenterWorldPosition(start.X, start.Y, start.Z);
        
        board.Manipulator.CameraManipulator.ZoomTo(0.6f, position);
        
        view.target = start;
        view.enemy = enemy;
        view.Go(nearest);
    }
}