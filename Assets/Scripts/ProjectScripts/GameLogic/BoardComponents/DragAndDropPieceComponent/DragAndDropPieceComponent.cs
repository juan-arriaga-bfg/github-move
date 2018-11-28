using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public interface IDragAndDropPieceComponent
{
    DragAndDropPieceComponent DragAndDrop { get; }
}

public partial class BoardLogicComponent : IDragAndDropPieceComponent
{
    private DragAndDropPieceComponent dragAndDrop;
    
    public DragAndDropPieceComponent DragAndDrop => dragAndDrop ?? (dragAndDrop = GetComponent<DragAndDropPieceComponent>(DragAndDropPieceComponent.ComponentGuid));
}

public class DragAndDropPieceComponent :  ECSEntity, IECSSystem 
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    
    public override int Guid => ComponentGuid;
    
    private int cachedPointerId = -2;

    private int targetPieceId;
    
    private BoardLogicComponent context;
    
    private Vector3 lastRemoverWorldPosition;

    private BoardPosition lastRemoverBoardPosition = BoardPosition.Default();
    
    private bool isActive = false;

    public bool IsActive => isActive;
    
    public Action OnBeginDragAndDropEvent { get; set; }

    public Action OnEndDragAndDropEvent { get; set; }
    
    private PieceBoardElementView dragableView;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        context = entity as BoardLogicComponent;
    }
    
    public virtual bool Begin(int pointerId, int targetPieceId)
    {
        if (isActive) return false;
        
        this.cachedPointerId = pointerId;
        this.targetPieceId = targetPieceId;
        
        return true;
    }

    protected virtual void BeginDragInternal()
    {
        isActive = true;
        
        var fakePiece = context.Context.CreatePieceFromType(targetPieceId);
        var fakeBoardElement = context.Context.RendererContext.CreatePieceAt(fakePiece, new BoardPosition(0, 0, 3));
        dragableView = fakeBoardElement;
        dragableView.SyncRendererLayers(new BoardPosition(0, 0, 100));
        
 
        if (OnBeginDragAndDropEvent != null)
        {
            OnBeginDragAndDropEvent();
        }
    }
    
    protected virtual void End()
    {
        this.cachedPointerId = -2;

        if (dragableView != null)
        {
            dragableView.ToggleSelection(false);
            
            context.Context.RendererContext.RemoveElement(dragableView);
            dragableView = null;
        }
        
        isActive = false;
        
        if (OnEndDragAndDropEvent != null)
        {
            OnEndDragAndDropEvent();
        }
    }
    
    public bool IsExecuteable()
    {
        return cachedPointerId != -2;
    }
    
    public virtual PieceBoardElementView CreateFakePieceAt(int targetPieceId, BoardPosition fakePosition)
    {
        var fakePiece = context.Context.CreatePieceFromType(targetPieceId);
        var fakeBoardElement = context.Context.RendererContext.CreateElementAt(fakePiece.PieceType, fakePosition) as PieceBoardElementView;

        if (fakeBoardElement != null)
        {
            fakeBoardElement.Init(context.Context.RendererContext, fakePiece);

            var particleSystems = fakeBoardElement.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particleSystems.Length; i++)
            {
                var particleSystem = particleSystems[i];
                var emission = particleSystem.emission;
                emission.enabled = false;
            }
        }

        return fakeBoardElement;
    }

    public virtual PieceBoardElementView CreateFakePiece(int targetPieceId, BoardPosition fakePosition)
    {
        var fakePiece = context.Context.CreatePieceFromType(targetPieceId);
        var fakeBoardElement = context.Context.RendererContext.CreatePieceAt(fakePiece, fakePosition);
        fakeBoardElement.SyncRendererLayers(new BoardPosition(0, 0, 3));
        fakeBoardElement.ToggleSelection(true);

        return fakeBoardElement;
    }

    public virtual void DestroyFakePiece(PieceBoardElementView fakePieceView)
    {
        var particleSystems = fakePieceView.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < particleSystems.Length; i++)
        {
            var particleSystem = particleSystems[i];
            var emission = particleSystem.emission;
            emission.enabled = true;
        }
        
        fakePieceView.ToggleSelection(false);  
        context.Context.RendererContext.RemoveElement(fakePieceView);
    }

    public bool IsValidPoint(BoardPosition boardPosition)
    {
        var pieceEntity = context.GetPieceAt(boardPosition);

        if (pieceEntity != null) return false;
        
        return true;
    }
    
    protected virtual bool TryApplyAt(Vector3 worldPosition)
    {
        var boardPosition = context.Context.BoardDef.GetSectorPosition(worldPosition);
        boardPosition = new BoardPosition(boardPosition.X, boardPosition.Y, context.Context.BoardDef.PieceLayer);

        return TryApplyAt(boardPosition);
    }

    protected virtual bool TryApplyAt(BoardPosition boardPosition)
    {
        if (IsValidPoint(boardPosition))
        {
            BoardService.Current.GetBoardById(0).ActionExecutor.AddAction(new CreatePieceAtAction
            {
                At = boardPosition,
                PieceTypeId = targetPieceId
            });

            return true;
        }
        return false;
    }

    public void Execute()
    {
        int touchCount = LeanTouch.Fingers.Count;
        LeanFinger touchDef = null;
        for (int i = 0; i < touchCount; i++)
        {
            var touch = LeanTouch.Fingers[i];

            if (touch.Index == cachedPointerId)
            {
                touchDef = touch;
            }
        }
        
        if (touchDef == null)
        {
            if (TryApplyAt(lastRemoverWorldPosition) == false)
            {
                this.cachedPointerId = -2;
                End(); 
            }
            else
            {
                this.cachedPointerId = -2;
                End();
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
                boardPosition = new BoardPosition(boardPosition.X, boardPosition.Y, context.Context.BoardDef.PieceLayer);

                if (boardPosition.Equals(lastRemoverBoardPosition) == false)
                {
                    lastRemoverBoardPosition = boardPosition;
                    var targetCellPosition = context.Context.BoardDef.GetWorldPosition(boardPosition.X, boardPosition.Y);

                    dragableView.CachedTransform.position = targetCellPosition;
                    dragableView.CachedTransform.localPosition = new Vector3(dragableView.CachedTransform.localPosition.x, dragableView.CachedTransform.localPosition.y, 0f);

                    bool isValidPoint = IsValidPoint(boardPosition);
                    dragableView.ToggleSelection(true, isValidPoint);
                }
            }
            else
            {
                if (touchDef.SwipeScaledDelta.sqrMagnitude > 25f)
                {
                    BeginDragInternal();
                }
            }
        }
    }

    public object GetDependency()
    {
        return context;
    }
}
