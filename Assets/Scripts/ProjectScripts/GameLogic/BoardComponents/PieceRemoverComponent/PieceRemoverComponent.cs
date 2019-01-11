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

    private bool isActive = false;

    public bool IsActive => isActive;

    private PieceTypeFilter Filter
    {
        get
        {
            // Debug hack: press LSHIFT to allow to remove any piece
#if UNITY_EDITOR
            return Input.GetKey(KeyCode.LeftShift) ? PieceTypeFilter.Default : PieceTypeFilter.Removable;
#else
             return PieceTypeFilter.Removable;
#endif
        }
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        context = entity as BoardLogicComponent;
    }

    protected virtual void ToggleFilterPieces(bool state)
    {
        if (state)
        {
            var points = context.PositionsCache.GetPiecePositionsByFilter(PieceTypeFilter.Default);

            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var pieceEntity = context.GetPieceAt(point);
                if (pieceEntity == null) continue;
                if (pieceEntity.PieceType == PieceType.Fog.Id) continue;
                
                var pieceView = context.Context.RendererContext.GetElementAt(point) as PieceBoardElementView;
                
                if (IsValidPoint(point) == false)
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
            
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var pieceEntity = context.GetPieceAt(point);
                if (pieceEntity == null) continue;
                if (pieceEntity.PieceType == PieceType.Fog.Id) continue;
                
                var pieceView = context.Context.RendererContext.GetElementAt(point) as PieceBoardElementView;
                
                if (pieceView != null) pieceView.ResetDefaultMaterial();
            }
        }
    }

    public virtual bool BeginRemover(int pointerId)
    {
        if (isActive) return false;
        
        this.cachedPointerId = pointerId;
        
        return true;
    }

    protected virtual void BeginRemoverInternal()
    {
        isActive = true;
        
        cachedRemoverView = context.Context.RendererContext.CreateBoardElement<PieceBoardElementView>((int) ViewType.PieceRemover);
        cachedRemoverView.Init(context.Context.RendererContext);
        cachedRemoverView.SyncRendererLayers(new BoardPosition(0, 0, BoardLayer.MAX.Layer));
        
        ToggleFilterPieces(true);

        if (OnBeginRemoverEvent != null)
        {
            OnBeginRemoverEvent();
        }
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
        
        isActive = false;
        
        if (OnEndRemoverEvent != null)
        {
            OnEndRemoverEvent();
        }
    }

    protected virtual void CollapsePieceAt(BoardPosition position)
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
        boardPosition = new BoardPosition(boardPosition.X, boardPosition.Y, BoardLayer.Piece.Layer);

        return TryCollapsePieceAt(boardPosition);
    }

    protected virtual bool TryCollapsePieceAt(BoardPosition boardPosition)
    {
        if (IsActive == false)
        {
            UIMessageWindowController.CreatePrefabMessage(
                LocalizationService.Get("window.remove.hint.title",   "window.remove.hint.title"),
                UIMessageWindowModel.HintType.RemoverHint.ToString(),
                LocalizationService.Get("window.remove.hint.message", "window.remove.hint.message"));
            
            return false;
        }
        
        if (IsValidPoint(boardPosition))
        {
            var pieceEntity = context.GetPieceAt(boardPosition);
            
            var model = UIService.Get.GetCachedModel<UIMessageWindowModel>(UIWindowType.MessageWindow);

            model.Prefab = PieceType.Parse(pieceEntity.PieceType);
            model.Title = LocalizationService.Get("window.remove.title", "window.remove.title");
            model.Message = LocalizationService.Get("window.remove.message", "window.remove.message");
            model.AcceptLabel = LocalizationService.Get("common.button.yes", "common.button.yes");
            model.CancelLabel = LocalizationService.Get("common.button.no", "common.button.no");
            
            model.OnAccept = () => { Confirm(boardPosition); };
            model.OnCancel = EndRemover;
            model.OnClose = EndRemover;

            model.AcceptColor = UIMessageWindowModel.ButtonColor.Red;
            model.CancelColor = UIMessageWindowModel.ButtonColor.Green;

            model.IsAcceptLeft = true;
            
            UIService.Get.ShowWindow(UIWindowType.MessageWindow);
            return true;
        }

        return false;
    }

    protected virtual void Confirm(BoardPosition position)
    {
        if (cachedRemoverView != null) cachedRemoverView.Animator.SetTrigger(cachedApplyAnimationId);
        
        if (cachedRemoverView != null) cachedRemoverView.ToggleSelection(false);
        
        ToggleFilterPieces(false);

        DOTween.Kill(this);
        var sequence = DOTween.Sequence().SetId(this);
        
        NSAudioService.Current.Play(SoundId.remover_use);
        
        sequence.InsertCallback(1f, () =>
        {
            CollapsePieceAt(position);
        });
        sequence.AppendInterval(1.6f);
        sequence.OnComplete(EndRemover);
    }

    public bool IsExecuteable()
    {
        return cachedPointerId != -2;
    }

    public bool IsValidPoint(BoardPosition boardPosition)
    {
        var pieceEntity = context.GetPieceAt(boardPosition);

        if (pieceEntity == null) return false;

        var lockersComponents = pieceEntity.GetComponentsBy<ILockerComponent>();
        bool isLocked = true;
        foreach (var lockerComponent in lockersComponents)
        {
            if (lockerComponent.Locker != null && lockerComponent.Locker.IsLocked == false)
            {
                isLocked = false;
            }
        }

        if (isLocked && Filter != PieceTypeFilter.Default) return false;

        var ids = PieceType.GetIdsByFilter(Filter);
        
        return ids.Contains(pieceEntity.PieceType);
    }

    public void Execute()
    {
        int touchCount = LeanTouch.Fingers.Count; // Input.touchCount;
        LeanFinger touchDef = null;
        for (int i = 0; i < touchCount; i++)
        {
            var touch = LeanTouch.Fingers[i]; // Input.GetTouch(i);

            if (touch.Index == cachedPointerId)
            {
                touchDef = touch;
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
            if (IsActive)
            {
                Vector3 screenPosition = new Vector3(touchDef.ScreenPosition.x, touchDef.ScreenPosition.y, context.Context.BoardDef.ViewCamera.transform.position.z * -1);
                var targetPosition = context.Context.BoardDef.ViewCamera.ScreenToWorldPoint(screenPosition);

                lastRemoverWorldPosition = targetPosition;

                var normalPosition = new Vector3(targetPosition.x, targetPosition.y, -context.Context.BoardDef.ViewCamera.transform.localPosition.z);
                var boardPosition = context.Context.BoardDef.GetSectorPosition(targetPosition);
                boardPosition = new BoardPosition(boardPosition.X, boardPosition.Y, BoardLayer.Piece.Layer);

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
            else
            {
                if (touchDef.SwipeScaledDelta.sqrMagnitude > 25f)
                {
                    BeginRemoverInternal();
                }
            }
        }
    }

    public object GetDependency()
    {
        return context;
    }
}
