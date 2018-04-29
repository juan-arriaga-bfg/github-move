﻿using System.Collections.Generic;
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
    
    public void Go(FogDef fog)
    {
        this.fog = fog;
        
        var next = new BoardPosition(fog.Position.X + Random.Range(0, fog.Size.X), fog.Position.Y + Random.Range(0, fog.Size.Y));
        var position = Context.Context.BoardDef.GetSectorCenterWorldPosition(next.X, next.Y, 0);
        var duration = BoardPosition.SqrMagnitude(target, next) * 2;
        
        target = next;
        
        DOTween.Kill(CachedTransform);
        
        var sequence = DOTween.Sequence().SetId(CachedTransform).SetEase(Ease.Linear);

        sequence.Append(CachedTransform.DOMove(position, duration));
        sequence.InsertCallback(duration - 0.1f, () => Go(this.fog));
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
    
    public static void Show(Enemy enemy)
    {
        var board = BoardService.Current.GetBoardById(0);
        
        var kingPosition = GameDataService.Current.PiecesManager.KingPosition;
        var kingWorldPos = board.BoardDef.GetSectorCenterWorldPosition(kingPosition.X, kingPosition.Y, 0);
		
        var distances = new List<KeyValuePair<float, FogDef>>();

        foreach (var def in GameDataService.Current.FogsManager.FogPositions.Values)
        {
            distances.Add(new KeyValuePair<float, FogDef>(Vector2.Distance(kingWorldPos, def.GetCenter(board)), def));
        }

        distances.Sort((a, b) => a.Key.CompareTo(b.Key));
		
        if(distances.Count == 0) return;

        var nearest = distances[Random.Range(0, distances.Count < 3 ? distances.Count : 3)].Value;
        var start = new BoardPosition(nearest.Position.X + Random.Range(0, nearest.Size.X), nearest.Position.Y + Random.Range(0, nearest.Size.Y));
        var view = board.RendererContext.CreateBoardElementAt<EnemyView>(R.EnemyView, start);
        
        view.target = start;
        view.enemy = enemy;
        view.Go(nearest);
    }
}