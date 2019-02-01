using System;
using System.Collections.Generic;

public class CollapseFogToAction : IBoardAction
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public virtual int Guid => ComponentGuid;

    public BoardPosition To { get; set; }

    public FogObserver FogObserver { get; set; }

    public List<BoardPosition> Positions { get; set; }
	
    public IBoardAction OnCompleteAction;

    public Action OnComplete;

    public bool IsIgnoreSpawn { get; set; }

    public bool PerformAction(BoardController gameBoardController)
    {
        if (Positions == null || Positions.Count == 0) return false;
		
        // gameBoardController.BoardLogic.LockCells(Positions, this);
        gameBoardController.BoardLogic.RemovePiecesAt(Positions);
        
        var animation = new CollapseFogToAnimation
        {
            Action = this
        };
        
        animation.PrepareAnimation(gameBoardController.RendererContext);
		
        animation.OnCompleteEvent += (_) =>
        {
            // gameBoardController.BoardLogic.UnlockCells(Positions, this);
            if (OnCompleteAction != null) gameBoardController.ActionExecutor.AddAction(OnCompleteAction);
            
            OnComplete?.Invoke();
            
            gameBoardController.ActionExecutor.AddAction(new CallbackAction
            {
                Callback = controller => controller.TutorialLogic.Update()
            });
        };
		
        gameBoardController.RendererContext.AddAnimationToQueue(animation);
        
        // spawn pieces
        if (FogObserver?.Def == null) return true;

        GameDataService.Current.FogsManager.RemoveFog(FogObserver.Key);

        if (IsIgnoreSpawn) return true;
        
        var addedPieces = new Dictionary<BoardPosition, int>();
        
        if(FogObserver.Def.Pieces != null)
        {
            foreach (var piece in FogObserver.Def.Pieces)
            {
                foreach (var pos in piece.Value)
                {
                    var pieceId = GameDataService.Current.MinesManager.GetMineTypeById(piece.Key);

                    if (pieceId == PieceType.None.Id)
                    {
                        pieceId = PieceType.Parse(piece.Key);
                    }

                    if (pieceId == PieceType.Empty.Id || pieceId == PieceType.None.Id)
                        pieceId = PieceType.LockedEmpty.Id;
                    
                    addedPieces.Add(new BoardPosition(pos.X, pos.Y, FogObserver.Context.Layer.Index), pieceId);
                }
            }
        }
        
        foreach (var point in FogObserver.Mask)
        {
            var position = new BoardPosition(point.X, point.Y, BoardLayer.Piece.Layer);

            if (addedPieces.ContainsKey(position)) continue;
            
            addedPieces.Add(position, PieceType.LockedEmpty.Id);
        }
        
        gameBoardController.ActionExecutor.PerformAction(new CreateGroupPieces()
        {
            Pieces = addedPieces,
            LogicCallback = (pieces) =>
            {
                var Context = FogObserver.Context;
                var board = Context.Context;
                var posByMask = new List<BoardPosition>();
                foreach (var boardPosition in FogObserver.Mask)
                {
                    posByMask.Add(FogObserver.GetPointInMask(Context.CachedPosition, boardPosition));
                }
                board.PathfindLocker.OnAddComplete(posByMask);
                
                Context.PathfindLockObserver.RemoveRecalculate(Context.CachedPosition);
                var emptyCells = board.PathfindLocker.CollectUnlockedEmptyCells();
                foreach (var emptyCell in emptyCells)
                {
                    var hasPath = board.PathfindLocker.HasPath(emptyCell);
                    if (hasPath && pieces.ContainsKey(emptyCell.CachedPosition))
                    {
                        pieces.Remove(emptyCell.CachedPosition);    
                        board.BoardLogic.RemovePieceAt(emptyCell.CachedPosition);    
                    }
                    else if(hasPath)
                    {
                        board.ActionExecutor.PerformAction(new CollapsePieceToAction()
                        {
                            IsMatch = false,
                            Positions = new List<BoardPosition>() {emptyCell.CachedPosition},
                            To = emptyCell.CachedPosition
                        });
                    }
                }
            },
            OnSuccessEvent = () =>
            {     
                var views = ResourcesViewManager.Instance.GetViewsById(Currency.Level.Name);

                if (views == null) return;
        
                foreach (var view in views)
                {
                    view.UpdateResource(0);
                }
            }
        });

        // add delay to spawn animations
        var animationsQueue = gameBoardController.RendererContext.GetAnimationsQueue();
        for (int i = 0; i < animationsQueue.Count; i++)
        {
            var spawnAnimation = animationsQueue[i] as SpawnPieceAtAnimation;
            
            if (spawnAnimation == null) continue;

            spawnAnimation.Delay = 0.5f;
        }
 
        return true;
    }
}
