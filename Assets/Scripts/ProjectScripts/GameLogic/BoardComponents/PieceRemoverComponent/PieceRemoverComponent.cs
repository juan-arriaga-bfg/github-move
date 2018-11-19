using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;

public interface IPieceRemoverComponent
{
    PieceRemoverComponent Remover { get; }
}

public partial class BoardLogicComponent : IPieceRemoverComponent
{
    private PieceRemoverComponent remover;
    
    public PieceRemoverComponent Remover => remover ?? (remover = GetComponent<PieceRemoverComponent>(PieceRemoverComponent.ComponentGuid));
}

public class PieceRemoverComponent : ECSEntity, IECSSystem
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public Action OnBeginRemoverEvent { get; set; }

    public Action OnEndRemoverEvent { get; set; }

    private BoardLogicComponent context;

    private PieceBoardElementView cachedRemoverView;

    private int cachedPointerId = -2;

    private Vector3 lastRemoverWorldPosition;

    private BoardPosition lastRemoverBoardPosition = BoardPosition.Default();

    private int cachedApplyAnimationId = Animator.StringToHash("Apply");

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        context = entity as BoardLogicComponent;
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        base.OnUnRegisterEntity(entity);
        
    }

    protected virtual void ToggleFilterPieces(bool state)
    {
        if (state)
        {
            var points = context.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Default);
            var filteredIds = PieceType.GetIdsByFilter(PieceTypeFilter.Removable);

            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var pieceEntity = context.GetPieceAt(point);
                if (pieceEntity == null) continue;
                if (pieceEntity.PieceType == PieceType.Fog.Id) continue;
                
                var pieceView = context.Context.RendererContext.GetElementAt(point) as PieceBoardElementView;
                
                if (filteredIds.Contains(pieceEntity.PieceType) == false)
                {
                    if (pieceView != null) pieceView.SetFade(0.5f);
                }
                else
                {
                    if (pieceView != null) pieceView.SetHighlight(true);
                }

            }
        }
        else
        {
            var points = context.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Default);
            var filteredIds = PieceType.GetIdsByFilter(PieceTypeFilter.Removable);

            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var pieceEntity = context.GetPieceAt(point);
                if (pieceEntity == null) continue;
                if (pieceEntity.PieceType == PieceType.Fog.Id) continue;
                
                // if (filteredIds.Contains(pieceEntity.PieceType)) continue; 
                
                var pieceView = context.Context.RendererContext.GetElementAt(point) as PieceBoardElementView;
                
                if (pieceView != null) pieceView.ResetDefaultMaterial();
            }
        }
    }

    public virtual bool BeginRemover(int pointerId)
    {
        this.cachedPointerId = pointerId;
        
        cachedRemoverView = context.Context.RendererContext.CreateBoardElement<PieceBoardElementView>((int) ViewType.PieceRemover);
        cachedRemoverView.Init(context.Context.RendererContext);
        cachedRemoverView.SyncRendererLayers(new BoardPosition(0, 0, 100));

        ToggleFilterPieces(true);

        if (OnBeginRemoverEvent != null)
        {
            OnBeginRemoverEvent();
        }

        return true;
    }
    
    protected virtual void EndRemover()
    {
        this.cachedPointerId = -2;

        if (cachedRemoverView != null)
        {
            cachedRemoverView.ToggleSelection(false);
            
            context.Context.RendererContext.DestroyElement(cachedRemoverView);
            cachedRemoverView = null;
        }
        
        ToggleFilterPieces(false);
        
        if (OnEndRemoverEvent != null)
        {
            OnEndRemoverEvent();
        }
    }

    public virtual void CollapsePieceAt(BoardPosition position)
    {
        context.Context.ActionExecutor.AddAction(new CollapsePieceToAction
        {
            Positions = new List<BoardPosition> {position},
            To = position,
            OnCompleteAction = null
        });
    }

    protected virtual bool TryCollapsePieceAt(Vector3 worldPosition)
    {
        var boardPosition = context.Context.BoardDef.GetSectorPosition(worldPosition);
        boardPosition = new BoardPosition(boardPosition.X, boardPosition.Y, context.Context.BoardDef.PieceLayer);

        return TryCollapsePieceAt(boardPosition);
    }

    protected virtual bool TryCollapsePieceAt(BoardPosition boardPosition)
    {
        var isValid = IsValidPoint(boardPosition);

        if (isValid == false)
        {
            return false;
        }
        
        var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);
        
        model.Title = LocalizationService.Get("window.remove.title","Remove");
        model.Message = LocalizationService.Get("window.remove.message", "Are you sure that you want to remove the figure from the field?");
        model.CancelLabel = LocalizationService.Get("common.button.yes", "Yes");
        model.AcceptLabel = LocalizationService.Get("common.button.no", "No");
        
        model.OnAccept = () =>
        {
            EndRemover();
        };
        
        model.OnCancel = () =>
        {
            Confirm(boardPosition);
        };

        UIService.Get.ShowWindow(UIWindowType.MessageWindow);

        return true;
    }

    protected virtual void Confirm(BoardPosition position)
    {
        CollapsePieceAt(position);

        if (cachedRemoverView != null) cachedRemoverView.Animator.SetTrigger(cachedApplyAnimationId);
        
        if (cachedRemoverView != null) cachedRemoverView.ToggleSelection(false);
        
        ToggleFilterPieces(false);

        DOTween.Kill(this);
        var sequence = DOTween.Sequence().SetId(this);
        sequence.AppendInterval(2f);
        sequence.OnComplete(() =>
        {
            EndRemover();
        });
    }

    public bool IsExecuteable()
    {
        return cachedPointerId != -2;
    }

    public bool IsValidPoint(BoardPosition boardPosition)
    {
        var pieceEntity = context.GetPieceAt(boardPosition);

        if (pieceEntity == null) return false;

        var ids = PieceType.GetIdsByFilter(PieceTypeFilter.Removable);
        
        return ids.Contains(pieceEntity.PieceType);
    }

    public void Execute()
    {
        if (cachedRemoverView == null) return;

        int touchCount = LeanTouch.Fingers.Count; // Input.touchCount;
        LeanFinger touchDef = null;
        for (int i = 0; i < touchCount; i++)
        {
            var touch = LeanTouch.Fingers[i]; // Input.GetTouch(i);

            if (touch.Index == cachedPointerId)
            {
                touchDef = touch;
            }
            else
            {
                Debug.LogWarning($"Finger => id:{touch.Index}");
            }
        }
        
        if (touchDef == null)
        {
            if (TryCollapsePieceAt(lastRemoverWorldPosition) == false)
            {
                EndRemover(); 
            }
            else
            {
                this.cachedPointerId = -2;
            }
        }
        else
        {
            Vector3 screenPosition = new Vector3(touchDef.ScreenPosition.x, touchDef.ScreenPosition.y, context.Context.BoardDef.ViewCamera.transform.position.z * -1);
            var targetPosition = context.Context.BoardDef.ViewCamera.ScreenToWorldPoint(screenPosition);

            lastRemoverWorldPosition = targetPosition;

            var normalPosition = new Vector3(targetPosition.x, targetPosition.y, -context.Context.BoardDef.ViewCamera.transform.localPosition.z);
            var boardPosition = context.Context.BoardDef.GetSectorPosition(targetPosition);
            boardPosition = new BoardPosition(boardPosition.X, boardPosition.Y, context.Context.BoardDef.PieceLayer);
            
            if (boardPosition.Equals(lastRemoverBoardPosition) == false)
            {
                lastRemoverBoardPosition = boardPosition;
                var targetCellPosition = context.Context.BoardDef.GetWorldPosition(boardPosition.X, boardPosition.Y);

                cachedRemoverView.CachedTransform.position = targetCellPosition;
                cachedRemoverView.CachedTransform.localPosition = new Vector3(cachedRemoverView.CachedTransform.localPosition.x, cachedRemoverView.CachedTransform.localPosition.y, 0f);

                bool isValidPoint = IsValidPoint(boardPosition);
                cachedRemoverView.ToggleSelection(true, isValidPoint);
            }
        }

    }

    public object GetDependency()
    {
        return context;
    }
}
