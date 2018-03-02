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

        var piece = GetSelectedPiece();

        if (piece == null) return false;
            
        var touchReaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);

        if (touchReaction != null) return touchReaction.Touch(piece.CachedPosition);
        
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
            var pieceBoardElementView = cachedViewForDrag as PieceBoardElementView;
            var boardPos = context.BoardDef.GetSectorPosition(targetPos);

            pieceBoardElementView.OnDrag(boardPos, pos);

            if ((prevDragPos - pos).sqrMagnitude > 0.01f)
            {
                DOTween.Kill(dragAnimationId);
                cachedViewForDrag.CachedTransform.localPosition = pos;

                isDragLip = false;
            }
            
            prevDragPos = pos;
            
            var targetCellPos = context.BoardDef.GetPiecePosition(boardPos.X, boardPos.Y);
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
//                sequence.AppendInterval(0.05f);
                sequence.Append(cachedViewForDrag.CachedTransform.DOLocalMove(targetCellPos, dragDuration).SetEase(Ease.Linear));
            }

            
//            }
//            else
//            {
//                prevDragPos = pos;
//                
////                var targetCellPos = context.BoardDef.GetPiecePosition(boardPos.X, boardPos.Y);
////                targetCellPos = new Vector3(targetCellPos.x, targetCellPos.y, 0f);
//                DOTween.Kill(dragAnimationId);
//                cachedViewForDrag.CachedTransform.localPosition = pos;
//            }
        }
        
       
        
        return true;
    }

    
    public bool OnDown(Vector2 startPos, Vector2 pos)
    {
        if (cachedViewForDrag == null)
        {
            var piece = GetSelectedPiece();
        
            if (piece == null) return false;
            
            var boardPosition = piece.CachedPosition;
            
            var selectedView = context.RendererContext.GetElementAt(boardPosition);
            
            var draggableComponent = piece.GetComponent<DraggablePieceComponent>(DraggablePieceComponent.ComponentGuid);

            if (draggableComponent == null || draggableComponent.IsDraggable(boardPosition) == false)
            {
                
                if (selectedView is PieceBoardElementView)
                {
                    var pieceBoardElementView = selectedView as PieceBoardElementView;
                    var boardPos = context.BoardDef.GetSectorPosition(pos);
                    pieceBoardElementView.OnDragStart(boardPos, pos);
                }
                
                return false;
            }

            cachedViewForDrag = selectedView;
            cachedDragDownPos = pos + Vector2.up * 0.5f;

            if (cachedViewForDrag != null)
            {
                
                cameraManipulator.CameraMove.Lock(this);
                
                if (selectedView is PieceBoardElementView)
                {
                    var pieceBoardElementView = selectedView as PieceBoardElementView;
                    var boardPos = context.BoardDef.GetSectorPosition(pos);
                    pieceBoardElementView.OnDragStart(boardPos, pos);
                }
            }

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
            
            if ((cachedDragDownPos - pos).sqrMagnitude > 0.01f)
            {
                BoardPosition fromPosition = context.RendererContext.GetBoardPosition(cachedViewForDrag);
                BoardPosition targetPosition = context.BoardDef.GetSectorPosition(new Vector3(pos.x, pos.y, 0));

                context.ActionExecutor.AddAction(new MovePieceFromToAction
                {
                    From = fromPosition,
                    To = targetPosition
                });
            }
            
            
            
            if (cachedViewForDrag is PieceBoardElementView)
            {
                var pieceBoardElementView = cachedViewForDrag as PieceBoardElementView;
                var boardPos = context.BoardDef.GetSectorPosition(pos);
                pieceBoardElementView.OnDragEnd(boardPos, pos);
                
                cachedViewForDrag.SyncRendererLayers(new BoardPosition(boardPos.X, boardPos.Y, pieceBoardElementView.Piece.Layer.Index));
            }

            cachedViewForDrag = null;

            cameraManipulator.CameraMove.UnLock(this);

            return true;
        }

        return false;
    }

    public virtual Piece GetSelectedPiece()
    {
        var selectedBoardElementView = GetSelectedBoardElementView();

        if (selectedBoardElementView == null) return null;

        return selectedBoardElementView.Piece;
    }

    public virtual PieceBoardElementView GetSelectedBoardElementView()
    {
        var touchableObjects = cameraManipulator.GetTouchable();

        PieceBoardElementView selectedBoardElement = null;

        int maxCoef = int.MinValue;

        for (int i = 0; i < touchableObjects.size; i++)
        {
            var touchableObject = touchableObjects[i].GetContext();
            if (touchableObject != null && touchableObject is PieceBoardElementView)
            {
                var boardElementView = touchableObject as PieceBoardElementView;

                var boardPosition = boardElementView.Piece.CachedPosition;

                var coef = boardPosition.X * context.BoardDef.Width - boardPosition.Y + boardPosition.Z;
                if (coef > maxCoef)
                {
                    maxCoef = coef;
                    selectedBoardElement = boardElementView;
                }
            }
        }

        return selectedBoardElement;
    }
}