using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DG.Tweening;

public class BoardManipulatorComponent : ECSEntity,
    IECSSystem, ILockerComponent, ITouchRegionListener
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    protected BoardController context;

    protected LockerComponent locker;

    private Vector3 screenPositionDown;

    private BoardPosition selectedPoint;

    private CameraManipulator cameraManipulator;
    
    private BoardPosition lastCachedDragPosition;

    private readonly ViewAnimationUid dragAnimationId = new ViewAnimationUid();

    private float dragDuration = 0.25f;

    private Vector2 prevDragPos;

    private bool isDragLip = false;
    
    
    private BoardElementView cachedViewForDrag = null;
    
    private Vector2 cachedDragDownPos = Vector2.zero;

    public CameraManipulator CameraManipulator
    {
        get { return cameraManipulator; }
    }

    public override int Guid
    {
        get { return ComponentGuid; }
    }

    public bool IsExecuteable()
    {
        return true;
    }
    
    public bool IsPersistence
    {
        get { return false; }
    }

    public void Execute()
    {
        if (Locker.IsLocked) return;
    }

    public virtual LockerComponent Locker
    {
        get
        {
            if (locker == null)
            {
                locker = GetComponent<LockerComponent>(LockerComponent.ComponentGuid);
            }
            
            return locker;
        }
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity as BoardController;

        cameraManipulator = GameObject.FindObjectOfType<CameraManipulator>();
        
        if (cameraManipulator != null)
        {
            cameraManipulator.RegisterTouchListener(this);
        }
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        if (cameraManipulator != null)
        {
            cameraManipulator.UnRegisterTouchListener(this);
        }
    }

    public bool OnTap(Vector2 startPos, Vector2 pos, int tapCount)
    {
        if (cachedViewForDrag != null)
        {
            cachedViewForDrag = null;

            cameraManipulator.CameraMove.UnLock(this);
        }
        
        var selectedView = GetSelectedBoardElementView();
        
        if (selectedView == null)
        {
            context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, this);
            return true;
        }

        if (selectedView is ResourceView) return true;

        if (selectedView is PieceBoardElementView)
        {
            var pieceView = selectedView as PieceBoardElementView;
            var touchReaction = pieceView.Piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);
            
            var boardPos = context.BoardDef.GetSectorPosition(pos);
            pieceView.OnTap(boardPos, pos);

            if (touchReaction != null) return touchReaction.Touch(pieceView.Piece.CachedPosition);
            
            context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceMenu, this);
        }
        
        return false;
    }

    public bool OnSet(Vector2 startPos, Vector2 pos, float duration)
    {
        if ((pos - startPos).sqrMagnitude <= 0.01f || cachedViewForDrag == null) return false;

        if (LeanTouch.Fingers.Count > 1) return false;

        pos = pos + Vector2.up * 0.5f;

        var targetPos = new Vector3(pos.x, pos.y, cachedViewForDrag.CachedTransform.position.z);

        if (cachedViewForDrag is PieceBoardElementView)
        {
            var pieceView = cachedViewForDrag as PieceBoardElementView;
            var boardPos = context.BoardDef.GetSectorPosition(targetPos);

            pieceView.OnDrag(boardPos, pos);
            
            if ((prevDragPos - pos).sqrMagnitude > 0.01f)
            {
//                DOTween.Kill(dragAnimationId);
//                cachedViewForDrag.CachedTransform.localPosition = pos;
//
//                isDragLip = false;
            }
            else
            {
                context.ProductionLogic.Check(pieceView.Piece.PieceType);
            }
            
            prevDragPos = pos;
            
            /*var targetCellPos = context.BoardDef.GetPiecePosition(boardPos.X, boardPos.Y);
            targetCellPos = new Vector3(targetCellPos.x, targetCellPos.y, 0f);
            
            if (lastCachedDragPosition.Equals(boardPos) == false)
            {
                cachedViewForDrag.SyncRendererLayers(new BoardPosition(context.BoardDef.Width,context.BoardDef.Height, context.BoardDef.Depth));
                lastCachedDragPosition = boardPos;
                
                isDragLip = false;
            }
            
            bool isPointValid = context.BoardLogic.IsPointValid(boardPos);
            if (isPointValid && isDragLip == false && (targetCellPos - cachedViewForDrag.CachedTransform.localPosition).sqrMagnitude > 0.01f)
            {
                isDragLip = true;
                DOTween.Kill(dragAnimationId);
                var sequence = DOTween.Sequence().SetId(dragAnimationId);
                sequence.Append(cachedViewForDrag.CachedTransform.DOLocalMove(targetCellPos, dragDuration).SetEase(Ease.Linear));
            }*/
        }
        
        return true;
    }
    
    public bool OnDown(Vector2 startPos, Vector2 pos)
    {
        if (cachedViewForDrag == null)
        {
            var selectedView = GetSelectedBoardElementView();
            
            if (selectedView == null) return false;
            
            if (selectedView is ResourceView)
            {
                var resourceView = selectedView as ResourceView;
                resourceView.Collect();
                return true;
            }
            
            if (selectedView is PieceBoardElementView)
            {
                var pieceView = selectedView as PieceBoardElementView;
                var draggableComponent = pieceView.Piece.GetComponent<DraggablePieceComponent>(DraggablePieceComponent.ComponentGuid);
                
                if (draggableComponent == null || draggableComponent.IsDraggable(pieceView.Piece.CachedPosition) == false)
                {
                    return false;
                }
                
                var boardPos = context.BoardDef.GetSectorPosition(pos);
                pieceView.OnDragStart(boardPos, pos);
            }
            
            cachedViewForDrag = selectedView;
            cachedDragDownPos = pos + Vector2.up * 0.5f;

            cameraManipulator.CameraMove.Lock(this);
            
            return true;
        }

        return false;
    }

    public bool OnUp(Vector2 startPos, Vector2 pos)
    {
        if (cachedViewForDrag != null)
        {
            pos = pos + Vector2.up * 0.5f;

            DOTween.Kill(dragAnimationId);
            
            if (cachedViewForDrag is PieceBoardElementView)
            {
                var pieceView = cachedViewForDrag as PieceBoardElementView;
                
                var boardPos = context.BoardDef.GetSectorPosition(pos);
                pieceView.OnDragEnd(boardPos, pos);

                if (pieceView.Drop(pos) == false)
                {
                    if ((cachedDragDownPos - pos).sqrMagnitude > 0.01f)
                    {
                        BoardPosition fromPosition = context.RendererContext.GetBoardPosition(cachedViewForDrag);
                        BoardPosition targetPosition = context.BoardDef.GetSectorPosition(new Vector3(pos.x, pos.y, 0));
                        BoardPosition toPosition = new BoardPosition(targetPosition.X, targetPosition.Y, fromPosition.Z);
                    
                        if (context.ProductionLogic.Hide(pieceView.Piece.PieceType, toPosition))
                        {
                            context.ActionExecutor.AddAction(new CollapsePieceToAction
                            {
                                To = toPosition,
                                Positions = new List<BoardPosition>{fromPosition}
                            });
                        }
                        else
                        {
                            context.ActionExecutor.AddAction(new DragAndCheckMatchAction
                            {
                                From = fromPosition,
                                To = toPosition
                            });
                        }
                        
                        UIService.Get.GetShowedView<UIProductionWindowView>(UIWindowType.ProductionWindow).Change(false);
                    }
                
                    cachedViewForDrag.SyncRendererLayers(new BoardPosition(boardPos.X, boardPos.Y, pieceView.Piece.Layer.Index));
                }
            }

            cachedViewForDrag = null;

            cameraManipulator.CameraMove.UnLock(this);

            return true;
        }

        return false;
    }
    
    public virtual BoardElementView GetSelectedBoardElementView()
    {
        var touchableObjects = cameraManipulator.GetTouchable();

        BoardElementView selectedBoardElement = null;

        var maxCoef = int.MinValue;

        for (var i = 0; i < touchableObjects.size; i++)
        {
            int? coef = null;
            var touchableObject = touchableObjects[i].GetContext();

            if (touchableObject is ResourceView)
            {
                var view = touchableObject as ResourceView;

                var position = view.CachedTransform.position * 100;
                
                coef = (int)position.x * context.BoardDef.Width - (int)position.y + (int)position.z;
            }
            else if (touchableObject is PieceBoardElementView)
            {
                var view = touchableObject as PieceBoardElementView;

                var position = view.Piece.CachedPosition;
                
                if(view.Piece.Context.BoardLogic.IsLockedCell(position)) continue;
                
                coef = position.X * context.BoardDef.Width - position.Y + position.Z;
            }

            if (coef != null && coef > maxCoef)
            {
                maxCoef = coef.Value;
                selectedBoardElement = touchableObject as BoardElementView;
            }
        }

        return selectedBoardElement;
    }
}