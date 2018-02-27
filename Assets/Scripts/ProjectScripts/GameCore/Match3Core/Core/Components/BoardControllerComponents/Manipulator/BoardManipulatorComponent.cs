using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Profiling;
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
        
        var boardPosition = context.BoardDef.GetSectorPosition(new Vector3(pos.x, pos.y, 0));

        boardPosition = new BoardPosition(boardPosition.X, boardPosition.Y, context.BoardDef.PieceLayer);

        if (context.BoardLogic.IsEmpty(boardPosition)) return true;
            
        var piece = context.BoardLogic.GetPieceAt(boardPosition);
            
        var touchReaction = piece.GetComponent<TouchReactionComponent>(TouchReactionComponent.ComponentGuid);

        if (touchReaction != null) return touchReaction.Touch(boardPosition);
        
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
            var boardPosition = context.BoardDef.GetSectorPosition(new Vector3(pos.x, pos.y, 0));

            boardPosition = new BoardPosition(boardPosition.X, boardPosition.Y, context.BoardDef.PieceLayer);

            if (context.BoardLogic.IsEmpty(boardPosition)) return false;
            
            var piece = context.BoardLogic.GetPieceAt(boardPosition);
            
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
}