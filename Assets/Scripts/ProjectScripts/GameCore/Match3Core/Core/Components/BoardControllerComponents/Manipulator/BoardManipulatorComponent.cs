using UnityEngine;
using Lean.Touch;

public class BoardManipulatorComponent : ECSEntity,
    IECSSystem, ILockerComponent, ITouchRegionListener
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    protected BoardController context;

    protected LockerComponent locker;

    private Vector3 screenPositionDown;

    private BoardPosition selectedPoint;

    private CameraManipulator cameraManipulator;

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

        cachedViewForDrag.CachedTransform.position = new Vector3(pos.x, pos.y, cachedViewForDrag.CachedTransform.position.z);
        
        return true;
    }

    private BoardElementView cachedViewForDrag = null;
    
    public bool OnDown(Vector2 startPos, Vector2 pos)
    {
        if (cachedViewForDrag == null)
        {
            var piece = GetSelectedPiece();

            if (piece == null) return false;

            var boardPosition = piece.CachedPosition;
            
            var draggableComponent = piece.GetComponent<DraggablePieceComponent>(DraggablePieceComponent.ComponentGuid);

            if (draggableComponent == null || draggableComponent.IsDraggable(boardPosition) == false)
            {
                return false;
            }
            
            cachedViewForDrag = context.RendererContext.GetElementAt(boardPosition);

            if (cachedViewForDrag != null)
            {
                cachedViewForDrag.SyncRendererLayers(new BoardPosition(context.BoardDef.Width, context.BoardDef.Height,
                    context.BoardDef.Depth));
                cameraManipulator.CameraMove.Lock(this);
            }

            return true;
        }

        return false;
    }

    public bool OnUp(Vector2 startPos, Vector2 pos)
    {
        if (cachedViewForDrag != null)
        {
            BoardPosition fromPosition = context.RendererContext.GetBoardPosition(cachedViewForDrag);
            
            BoardPosition targetPosition = context.BoardDef.GetSectorPosition(new Vector3(pos.x, pos.y, 0));
            
            context.ActionExecutor.AddAction(new MovePieceFromToAction
            {
                From = fromPosition,
                To = targetPosition
            });
            
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