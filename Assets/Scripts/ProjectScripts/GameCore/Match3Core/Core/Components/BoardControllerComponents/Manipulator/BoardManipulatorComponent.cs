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

    private BoardPosition lastResetPosition;

    private PieceBoardElementView lastResetPieceView;

    private readonly ViewAnimationUid dragAnimationId = new ViewAnimationUid();
    
    private readonly ViewAnimationUid dragTresholdId = new ViewAnimationUid();

    private float dragDuration = 0.25f;

    private Vector2 prevDragPos;

    private bool isDragLip = false;

    private bool isTouch = true;

    private bool? isDragInternal;

    private LayerMask cachedGUILayerMask;
    
    private bool isCachedLayerMash = false;

    public static float DragTreshold = 0.25f;

    private bool? isDrag
    {
        get { return isDragInternal; }
        set
        {
            if (value.HasValue == false)
            {
                if (isCachedLayerMash)
                {
                    LeanTouch.Instance.GuiLayers = cachedGUILayerMask;
                }
            }
            else
            {
                if ((isDragInternal.HasValue == false && value.Value == true) || (isDragInternal.HasValue && isDragInternal.Value != value.Value && value.Value == true))
                {
                    cachedGUILayerMask = LeanTouch.Instance.GuiLayers;
                    LeanTouch.Instance.GuiLayers = LayerMask.GetMask("Nothing");
                    isCachedLayerMash = true;
                }
                else
                {
                    if (isCachedLayerMash)
                    {
                        LeanTouch.Instance.GuiLayers = cachedGUILayerMask;
                    }
                }
            }

            isDragInternal = value;
        }
    }
    
    private bool isFullPass = false;
    
    private BoardElementView cachedViewForDrag = null;

    private BoardElementView cachedViewOnDown = null;
    
    private Vector2 cachedDragDownPos = Vector2.zero;

    private CallbackAction cachedTutorialStateUpdateAction = null;

    public CameraManipulator CameraManipulator => cameraManipulator;

    public override int Guid => ComponentGuid;

    private ITouchableBoardObjectLogic[] touchableBoardObjectLogics;
    private ITouchableBoardObjectLogic[] TouchableBoardObjectLogics =>
        touchableBoardObjectLogics ?? (touchableBoardObjectLogics = new ITouchableBoardObjectLogic[]
        {
            context.BoardLogic.FireflyLogic,
            context.BoardLogic.AirShipLogic,
            context.BoardLogic.VIPIslandLogic,
        });

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

    public virtual void StopDragAnimation(BoardPosition at)
    {
        if (at.Equals(lastResetPosition))
        {
            StopDragAnimationInternal();
        }
    }

    protected void StopDragAnimationInternal()
    {
        DOTween.Kill(dragAnimationId);

        if (lastResetPieceView != null)
        {
            context.RendererContext.ResetBoardElement(lastResetPieceView, lastResetPieceView.Piece.CachedPosition);
        }

        lastResetPosition = BoardPosition.Default();
        lastResetPieceView = null;
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

        foreach (var touchableObjectLogic in TouchableBoardObjectLogics)
        {
            if (touchableObjectLogic.OnClick(selectedView)) return true;
        }

        var pieceView = selectedView as PieceBoardElementView;
        if (pieceView != null && context.BoardLogic.IsLockedCell(pieceView.Piece.CachedPosition) == false && pieceView.Piece.CachedPosition.Equals(BoardPosition.Zero()) == false)
        {
            pieceView.OnTap(pieceView.Piece.CachedPosition, pos);
            
            if (pieceView.AvailiableLockTouchMessage() && !context.PathfindLocker.HasPath(pieceView.Piece))
            { 
                ShowHintForLockObject(pieceView.Piece);
            }

            if (pieceView.Piece.TouchReaction != null)
            {
                var isPieceTouch = pieceView.Piece.TouchReaction.Touch(pieceView.Piece.CachedPosition);

                if (isPieceTouch) context.TutorialLogic.Update();
                
                return isPieceTouch;
            }
        }
        
        context.BoardEvents.RaiseEvent(GameEventsCodes.ClosePieceUI, this);
        return false;
    }

    private void ShowHintForLockObject(Piece lockedObject)
    {
        Piece bestTarget = null;
        int bestCurrentPrice = int.MaxValue;
        FindTargetFor(lockedObject, ref bestTarget, ref bestCurrentPrice, 0, new HashSet<BoardPosition> {lockedObject.CachedPosition});
        if (bestCurrentPrice == int.MaxValue)
        {
            IW.Logger.LogError("[BoardManipulatorComponent] => target for hint not found");
            return;
        }
        
        HintArrowView.Show(bestTarget.CachedPosition);
    }

    private void FindTargetFor(Piece currentPiece, ref Piece currentBestPiece, ref int minCost, int currentCost, HashSet<BoardPosition> checkedPositions)
    {
        var obstacleLife = currentPiece?.GetComponent<ObstacleLifeComponent>(LifeComponent.ComponentGuid);
        
        if (obstacleLife != null)
        {
            currentCost += obstacleLife.CalculateResultPrice();
            if (currentCost >= minCost)
            {
                return;
            }
        }
        
        if (context.PathfindLocker.HasPath(currentPiece))
        {
            currentBestPiece = currentPiece;
            minCost = currentCost;
            return;
        }

        var checkedPositionsNow = new HashSet<BoardPosition>(checkedPositions);
        var blockPositions = context.PathfindLocker.GetBlockPathPositions(currentPiece);
        foreach (var blockPos in blockPositions)
        {
            if (checkedPositionsNow.Contains(blockPos))
            {
                continue;
            }

            checkedPositionsNow.Add(blockPos);
            
            var piece = context.BoardLogic.GetPieceAt(blockPos);
            var obstacleLifeTarget = piece?.GetComponent<ObstacleLifeComponent>(LifeComponent.ComponentGuid);
            if (obstacleLifeTarget != null)
            {
                FindTargetFor(piece, ref currentBestPiece, ref minCost, currentCost, checkedPositionsNow);
            }
        }
    }

    private bool CheckDrag(Vector2 startPos, Vector2 pos, float duration)
    {
        bool state = cachedViewForDrag == null && isDrag == null;
              
        return state;
    }

    private void BeginDrag(Vector2 startPos, Vector2 pos, BoardElementView selectedView)
    {
        isDrag = false;
        
        var start = context.BoardDef.GetSectorPosition(startPos);
        var current = context.BoardDef.GetSectorPosition(pos);
        
        if (start.Equals(current) == false) return;
        if (cachedViewForDrag != null) return;
        if (selectedView == null) return;
            
        if (selectedView is PieceBoardElementView pieceView)
        {
            if (pieceView.Piece.CachedPosition.Equals(BoardPosition.Zero())
                || pieceView.Piece.Draggable == null
                || pieceView.Piece.Draggable.IsDraggable(pieceView.Piece.CachedPosition) == false)
            {
                return;
            }
                
            pieceView.OnDragStart(context.BoardDef.GetSectorPosition(pos), pos);
        }
        else
        {
            foreach (var touchableObjectLogic in TouchableBoardObjectLogics)
            {
                if (touchableObjectLogic.OnDragStart(selectedView) == false) continue;
            
                break;
            }
        }
        
        cachedViewForDrag = selectedView;
        cachedDragDownPos = pos + Vector2.up * 0.5f;
        cameraManipulator.CameraMove.Lock(this);
        isDrag = true;
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

        if (CheckDrag(startPos, pos, duration))
        {
            if (cameraManipulator.CameraMove.IsLockedBy(dragTresholdId))
            {
                if ((startPos - pos).magnitude > DragTreshold)
                {
                    BeginDrag(startPos, startPos, cachedViewOnDown);
                }
                else
                {
                    return true;
                }
            }
        }
        
        if ((pos - startPos).sqrMagnitude <= 0.01f || cachedViewForDrag == null) return false;

        pos = pos + Vector2.up * 0.5f;

        var targetPos = new Vector3(pos.x, pos.y, cachedViewForDrag.CachedTransform.position.z);

        foreach (var touchableObjectLogic in TouchableBoardObjectLogics)
        {
            if (touchableObjectLogic.Check(cachedViewForDrag) == false) continue;
            if (touchableObjectLogic.IsDraggable) cachedViewForDrag.CachedTransform.localPosition = pos;
            
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
                StopDragAnimationInternal();
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
                StopDragAnimationInternal();
                var sequence = DOTween.Sequence().SetId(dragAnimationId);
                sequence.Append(cachedViewForDrag.CachedTransform.DOLocalMove(targetCellPos, dragDuration).SetEase(Ease.Linear)); 
            }

            isFullPass = false;
        }
        
        return true;
    }
    
    public bool OnDown(Vector2 startPos, Vector2 pos)
    {
        context.BoardLogic.FireflyLogic.ResetSession();
        
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
            
            var selectedView = GetSelectedBoardElementView(0);
            if (selectedView != null && selectedView is PieceBoardElementView)
            {
                var pieceView = selectedView as PieceBoardElementView;
                var pathfindLockListenerComponent = pieceView.Piece?.GetComponent<PathfindLockListenerComponent>(PathfindLockListenerComponent.ComponentGuid);
                
                if (pieceView != null && (pieceView.Piece?.Draggable != null && (pathfindLockListenerComponent != null && pathfindLockListenerComponent.IsHasPath() == true)))
                {
                    cameraManipulator.CameraMove.Lock(dragTresholdId, true);

                    cachedViewOnDown = selectedView;
                }
                return true;
            }

            foreach (var touchableObjectLogic in TouchableBoardObjectLogics)
            {
                if (touchableObjectLogic.Check(selectedView) == false) continue;
                if (touchableObjectLogic.IsDraggable) cameraManipulator.CameraMove.Lock(dragTresholdId, true);

                cachedViewOnDown = selectedView;
                break;
            }

            return true;
        }
        
        return false;
    }

    public bool OnUp(Vector2 startPos, Vector2 pos)
    {
        isDrag = false;
        
        cameraManipulator.CameraMove.UnLock(dragTresholdId);
        cachedViewOnDown = null;
        
        if (cachedViewForDrag != null)
        {
            pos = pos + Vector2.up * 0.5f;

            StopDragAnimationInternal();

            foreach (var touchableObjectLogic in TouchableBoardObjectLogics)
            {
                if (touchableObjectLogic.OnDragEnd(cachedViewForDrag) == false) continue;
                
                cachedViewForDrag = null;
                cameraManipulator.CameraMove.UnLock(this);
                context.TutorialLogic.Pause(false);
                context.TutorialLogic.Update();
                return true;
            }

            if (cachedViewForDrag is PieceBoardElementView pieceView)
            {
                var boardPos = context.BoardDef.GetSectorPosition(pos);
                var fromPosition = context.RendererContext.GetBoardPosition(pieceView);
                
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

                    lastResetPosition = new BoardPosition(boardPos.X, boardPos.Y, BoardLayer.Piece.Layer);
                    lastResetPieceView = pieceView;
                    
                    cachedViewForDrag.CachedTransform.DOLocalMove(targetPos, duration).OnComplete(() =>
                    {
                        lastResetPosition = BoardPosition.Default();
                        lastResetPieceView = null;
                        
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
    
    /// <summary>
    /// Get selected view using filter: -1 (all);  0 (only draggable objects + firefly)
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    protected virtual BoardElementView GetSelectedBoardElementView(int filter = -1)
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

            if (filter == 0)
            {
                if (touchableObject is UIBoardView)
                {
                    coef = int.MinValue;
                }
            }

            foreach (var touchableObjectLogic in TouchableBoardObjectLogics)
            {
                if (touchableObjectLogic.Check(touchableObject as BoardElementView) == false) continue;
                
                coef = int.MaxValue;
                break;
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