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

    private BoardElementView cachedRemoverView;

    private int cachedPointerId = -2;

    private Vector3 lastRemoverWorldPosition;

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

    public virtual bool BeginRemover(int pointerId)
    {
        Debug.LogWarning($"BeginRemover => pointerId:{pointerId}");

        this.cachedPointerId = pointerId;
        
        cachedRemoverView = context.Context.RendererContext.CreateBoardElement<BoardElementView>((int) ViewType.PieceRemover);
        cachedRemoverView.Init(context.Context.RendererContext);
        cachedRemoverView.SyncRendererLayers(new BoardPosition(0, 0, 100));

        if (OnBeginRemoverEvent != null)
        {
            OnBeginRemoverEvent();
        }

        return true;
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
        var isEmpty = context.IsEmpty(boardPosition);

        if (isEmpty)
        {
            Debug.LogWarning($"TryCollapsePieceAt => isEmpty at:{boardPosition}");
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

        DOTween.Kill(this);
        var sequence = DOTween.Sequence().SetId(this);
        sequence.AppendInterval(1f);
        sequence.OnComplete(() =>
        {
            EndRemover();
        });
    }

    protected virtual void EndRemover()
    {
        this.cachedPointerId = -2;

        if (cachedRemoverView != null)
        {
            context.Context.RendererContext.DestroyElement(cachedRemoverView);
            cachedRemoverView = null;
        }
        
        if (OnEndRemoverEvent != null)
        {
            OnEndRemoverEvent();
        }
    }

    public bool IsExecuteable()
    {
        return cachedPointerId != -2;
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
        
        // Debug.LogWarning($"ProcessRemover => isHas:{touchDef != null} touchCount:{touchCount}");

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
            
            cachedRemoverView.CachedTransform.position = targetPosition;
            cachedRemoverView.CachedTransform.localPosition = new Vector3(cachedRemoverView.CachedTransform.localPosition.x, cachedRemoverView.CachedTransform.localPosition.y, 0f);

            lastRemoverWorldPosition = targetPosition;
            
            var boardPosition = context.Context.BoardDef.GetSectorPosition(targetPosition);
            
            // Debug.LogWarning($"ProcessRemover => {boardPosition}");
        }

    }

    public object GetDependency()
    {
        return context;
    }
}
