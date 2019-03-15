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

    private bool isTouch = true;
    private bool? isDrag = false;
    private bool isFullPass = false;
    
    private BoardElementView cachedViewForDrag = null;
    
    private Vector2 cachedDragDownPos = Vector2.zero;

    private CallbackAction cachedTutorialStateUpdateAction = null;

    public CameraManipulator CameraManipulator => cameraManipulator;

    public override int Guid => ComponentGuid;

    public bool IsExecuteable()
    {
        return true;
    }

    public void Execute()
    {
        if (Locker.IsLocked) return;
    }

    public object GetDependency()
    {
        return null;
    }
    
    public virtual LockerComponent Locker => locker ?? (locker = GetComponent<LockerComponent>(LockerComponent.ComponentGuid));

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
        if (isTouch == false) return false;
        
        isDrag = false;
        
        if (cachedViewForDrag != null)
        {
            cameraManipulator.CameraMove.UnLock(this);
        }

        var selectedView = GetSelectedBoardElementView();

        if (selectedView == null)
        {
            context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
            return false;
        }

        if (context.BoardLogic.FireflyLogic.OnClick(selectedView)) return true;

        var pieceView = selectedView as PieceBoardElementView;
        if (pieceView != null && context.BoardLogic.IsLockedCell(pieceView.Piece.CachedPosition) == false && pieceView.Piece.CachedPosition.Equals(BoardPosition.Zero()) == false)
        {
            pieceView.OnTap(pieceView.Piece.CachedPosition, pos);
            
            if (pieceView.AvailiableLockTouchMessage() && !context.PathfindLocker.HasPath(pieceView.Piece))
            {
                
                UIErrorWindowController.AddError(LocalizationService.Get("message.error.pieceLock"));  
            }
            
            if (pieceView.Piece.TouchReaction != null) return pieceView.Piece.TouchReaction.Touch(pieceView.Piece.CachedPosition);
        }
        
        context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
        return false;
    }

    private bool CheckDrag(float duration)
    {
        return cachedViewForDrag == null && isDrag == null;
    }

    private void BeginDrag(Vector2 startPos, Vector2 pos)
    {
        isDrag = false;
        
        var start = context.BoardDef.GetSectorPosition(startPos);
        var current = context.BoardDef.GetSectorPosition(pos);
        
        if (!start.Equals(current)) return;
           
        if (cachedViewForDrag == null)
        {
            var selectedView = GetSelectedBoardElementView();
            
            if (selectedView == null) return;
            
            var pieceView = selectedView as PieceBoardElementView;
            
            if (context.BoardLogic.FireflyLogic.OnDragStart(selectedView) == false && pieceView != null)
            {
                if (pieceView.Piece.CachedPosition.Equals(BoardPosition.Zero()) || pieceView.Piece.Draggable == null || pieceView.Piece.Draggable.IsDraggable(pieceView.Piece.CachedPosition) == false)
                {
                    return;
                }
                
                var boardPos = context.BoardDef.GetSectorPosition(pos);
                pieceView.OnDragStart(boardPos, pos);
            }
            
            cachedViewForDrag = selectedView;
            cachedDragDownPos = pos + Vector2.up * 0.5f;
            cameraManipulator.CameraMove.Lock(this);
            isDrag = true;
            
            
        }
    }

    public bool OnSet(Vector2 startPos, Vector2 pos, float duration)
    {
        if (LeanTouch.Fingers.Count > 1)
        {
            isTouch = false;
            return false;
        }

        var start = context.BoardDef.GetSectorPosition(startPos);
        var current = context.BoardDef.GetSectorPosition(pos);
        if (!start.Equals(current)) isTouch = false;
        
        if(CheckDrag(duration)) BeginDrag(startPos, pos);
        
        if ((pos - startPos).sqrMagnitude <= 0.01f || cachedViewForDrag == null) return false;

        pos = pos + Vector2.up * 0.5f;

        var targetPos = new Vector3(pos.x, pos.y, cachedViewForDrag.CachedTransform.position.z);

        if (context.BoardLogic.FireflyLogic.Check(cachedViewForDrag))
        {
            cachedViewForDrag.CachedTransform.localPosition = pos;
            return true;
        }

        if (cachedViewForDrag is PieceBoardElementView)
        {
            var pieceView = cachedViewForDrag as PieceBoardElementView;
            
            var boardPos = context.BoardDef.GetSectorPosition(targetPos);
            boardPos.Z = BoardLayer.Piece.Layer;
            pieceView.OnDrag(boardPos, pos);
            pieceView.Piece.ViewDefinition?.OnDrag(false);
            
            if ((prevDragPos - pos).sqrMagnitude > 0.01f || isFullPass)
            {
                DOTween.Kill(dragAnimationId);
                cachedViewForDrag.CachedTransform.localPosition = pos;
                
                isDragLip = false;
            }
            
            prevDragPos = pos;
            
            var targetCellPos = context.BoardDef.GetPiecePosition(boardPos.X, boardPos.Y);
            targetCellPos = new Vector3(targetCellPos.x, targetCellPos.y, 0f);
            
            if (lastCachedDragPosition.Equals(boardPos) == false || isFullPass)
            {
                cachedViewForDrag.SyncRendererLayers(context.BoardDef.MaxPoit);
                lastCachedDragPosition = boardPos;
                
                isDragLip = false;
            }
            
            bool isPointValid = context.BoardLogic.IsPointValid(boardPos);
            if (isPointValid && isDragLip == false && (targetCellPos - cachedViewForDrag.CachedTransform.localPosition).sqrMagnitude > 0.01f || isFullPass)
            {
                isDragLip = true;
                DOTween.Kill(dragAnimationId);
                var sequence = DOTween.Sequence().SetId(dragAnimationId);
                sequence.Append(cachedViewForDrag.CachedTransform.DOLocalMove(targetCellPos, dragDuration).SetEase(Ease.Linear)); 
            }

            isFullPass = false;
        }
        
        return true;
    }
    
    public bool OnDown(Vector2 startPos, Vector2 pos)
    {
        // cancel tutorial pause (false) delayed action
        if (cachedTutorialStateUpdateAction != null)
        {
            cachedTutorialStateUpdateAction.CancelAction();
            cachedTutorialStateUpdateAction = null;
        }
        
        context.TutorialLogic.Pause(true);
        
        if (cachedViewForDrag == null)
        {
            isDrag = null;
            isTouch = true;
            isFullPass = true;
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

            if (context.BoardLogic.FireflyLogic.OnDragEnd(cachedViewForDrag))
            {
                cachedViewForDrag = null;
                cameraManipulator.CameraMove.UnLock(this);
                context.TutorialLogic.Pause(false);
                context.TutorialLogic.Update();
                return true;
            }
            
            if (cachedViewForDrag is PieceBoardElementView)
            {
                var pieceView = cachedViewForDrag as PieceBoardElementView;
                var boardPos = context.BoardDef.GetSectorPosition(pos);
                var fromPosition = context.RendererContext.GetBoardPosition(cachedViewForDrag);
                
                pieceView.OnDragEnd(boardPos, pos);

                if ((cachedDragDownPos - pos).sqrMagnitude > 0.01f && !isTouch)
                {
                    var targetPosition = context.BoardDef.GetSectorPosition(new Vector3(pos.x, pos.y, 0));
                    var toPosition = new BoardPosition(targetPosition.X, targetPosition.Y, fromPosition.Z);
                    
                    if (context.BoardLogic.IsEmpty(toPosition) == false && context.WorkerLogic.SetExtra(pieceView.Piece, toPosition))
                    {
                        context.ActionExecutor.AddAction(new CollapsePieceToAction
                        {
                            To = targetPosition,
                            Positions = new List<BoardPosition> {pieceView.Piece.CachedPosition}
                        });
                    }
                    else if(context.BoardLogic.IsEmpty(toPosition) == false && GameDataService.Current.FogsManager.SetMana(pieceView.Piece, toPosition))
                    {
                    }
                    else
                    {
                        context.ActionExecutor.AddAction(new DragAndCheckMatchAction
                        {
                            From = fromPosition,
                            To = toPosition
                        });
                    }
                    
                    cachedViewForDrag = null;
                    cameraManipulator.CameraMove.UnLock(this);
                }
                else
                {
                    var currentPos = pieceView.CachedTransform.localPosition;
                    var targetPos = context.BoardDef.GetSectorCenterWorldPosition(fromPosition.X, fromPosition.Y, 0);
                    var distance = Vector2.Distance(currentPos, targetPos);
                    var duration = dragDuration * (distance < 1 ? distance : 1);
                    
                    if (pieceView.Piece != null)
                    {
                        pieceView.SyncRendererLayers(pieceView.Piece.CachedPosition);
                    }
                    else
                    {
                       if (cachedViewForDrag != null) cachedViewForDrag.SyncRendererLayers(new BoardPosition(boardPos.X, boardPos.Y, BoardLayer.Piece.Layer));
                    }
                    
                    cachedViewForDrag.CachedTransform.DOLocalMove(targetPos, duration).OnComplete(() =>
                        {
                            pieceView.Piece?.ViewDefinition?.OnDrag(true);
                        })
                        .SetId(dragAnimationId);
                    
                    cachedViewForDrag = null;
                    cameraManipulator.CameraMove.UnLock(this);
                }
            }

            cachedTutorialStateUpdateAction = new CallbackAction
            {
                Delay = 0.5f,
                Callback = controller =>
                {
                    context.TutorialLogic.Pause(false);
                    context.TutorialLogic.Update();
                }
            };
            context.ActionExecutor.AddAction(cachedTutorialStateUpdateAction);
            
            return true;
        }
        
        context.TutorialLogic.Pause(false);
        context.TutorialLogic.Update();
        return false;
    }

    protected virtual BoardElementView GetSelectedBoardElementView()
    {
        var touchableObjects = cameraManipulator.GetTouchable();
        
        BoardElementView selectedBoardElement = null;
        
        var maxCoef = int.MinValue;
        
        for (var i = 0; i < touchableObjects.size; i++)
        {
            int? coef = null;
            var touchableObject = touchableObjects[i].GetContext();
            
            if (touchableObject is PieceBoardElementView)
            {
                var view = touchableObject as PieceBoardElementView;
                
                if (view.Piece == null) continue;
                
                var position = view.Piece.CachedPosition;
                
                if(view.Piece.Context.BoardLogic.IsLockedCell(position)) continue;
                
                coef = position.X * context.BoardDef.Width - position.Y + position.Z;
            }

            if (context.BoardLogic.FireflyLogic.Check(touchableObject as BoardElementView))
            {
                coef = int.MaxValue;
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