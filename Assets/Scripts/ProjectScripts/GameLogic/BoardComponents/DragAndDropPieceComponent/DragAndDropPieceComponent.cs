using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public class DragAndDropPieceComponent :  ECSEntity, IECSSystem 
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    
    public override int Guid => ComponentGuid;
    
    private int cachedPointerId = -2;
    
    private BoardLogicComponent context;
    
    private Vector3 lastRemoverWorldPosition;

    private BoardPosition lastRemoverBoardPosition = BoardPosition.Default();
    
    private bool isActive = false;

    public bool IsActive => isActive;
    
    private PieceBoardElementView dragableView;
    
    public override void OnRegisterEntity(ECSEntity entity)
    {
        base.OnRegisterEntity(entity);
        
        context = entity as BoardLogicComponent;
    }
    
    public virtual bool Begin(int pointerId)
    {
        if (isActive) return false;
        
        this.cachedPointerId = pointerId;
        
        return true;
    }

    protected virtual void BeginDragInternal()
    {
        isActive = true;
        
        // cachedRemoverView = context.Context.RendererContext.CreateBoardElement<PieceBoardElementView>((int) ViewType.PieceRemover);
        // cachedRemoverView.Init(context.Context.RendererContext);
        // cachedRemoverView.SyncRendererLayers(new BoardPosition(0, 0, 100));
        //
        // ToggleFilterPieces(true);
        //
        // if (OnBeginRemoverEvent != null)
        // {
        //     OnBeginRemoverEvent();
        // }
    }
    
    protected virtual void End()
    {
        this.cachedPointerId = -2;

        // if (cachedRemoverView != null)
        // {
        //     cachedRemoverView.ToggleSelection(false);
        //     
        //     context.Context.RendererContext.DestroyElement(cachedRemoverView);
        //     cachedRemoverView = null;
        // }
        //
        // ToggleFilterPieces(false);
        
        isActive = false;
        
        // if (OnEndRemoverEvent != null)
        // {
        //     OnEndRemoverEvent();
        // }
    }
    
    public bool IsExecuteable()
    {
        return cachedPointerId != -2;
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
        return false;
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
            if (TryApplyAt(lastRemoverWorldPosition) == false)
            {
                End(); 
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
