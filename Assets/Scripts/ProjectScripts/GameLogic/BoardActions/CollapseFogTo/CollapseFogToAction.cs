using System;
using System.Collections.Generic;
using System.Linq;
using BfgAnalytics;

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
        var context = FogObserver.Context;
        var posByMask = new List<BoardPosition>();
        foreach (var boardPosition in FogObserver.Mask)
        {
            posByMask.Add(FogObserver.GetPointInMask(context.CachedPosition, boardPosition));
        }
        
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
        
        if (FogObserver.Def.Pieces != null)
        {
            foreach (var piece in FogObserver.Def.Pieces)
            {
                foreach (var pos in piece.Value)
                {
                    var pieceId = PieceType.Parse(piece.Key);

                    if (pieceId == PieceType.Empty.Id || pieceId == PieceType.None.Id) pieceId = PieceType.LockedEmpty.Id;
                    
                    addedPieces.Add(new BoardPosition(pos.X, pos.Y, FogObserver.Context.Layer.Index), pieceId);
                }
            }
        }

        SendAnalytics(addedPieces, FogObserver.Def.Uid);
        
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
                
                gameBoardController.PathfindLocker.OnAddComplete(posByMask);
                
                gameBoardController.PathfindLocker?.RecalcCacheOnPieceRemoved(context);
                var emptyCells = gameBoardController.PathfindLocker.CollectUnlockedEmptyCells();
                foreach (var emptyCell in emptyCells)
                {
                    var hasPath = gameBoardController.PathfindLocker.HasPath(emptyCell);
                    if (hasPath && pieces.ContainsKey(emptyCell.CachedPosition))
                    {
                        pieces.Remove(emptyCell.CachedPosition);    
                        gameBoardController.BoardLogic.RemovePieceAt(emptyCell.CachedPosition);    
                    }
                    else if(hasPath)
                    {
                        gameBoardController.ActionExecutor.PerformAction(new CollapsePieceToAction()
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
    
    private void SendAnalytics(Dictionary<BoardPosition, int> data, string fog)
    {
        var boosters = new Dictionary<int, CurrencyPair>
        {
            {PieceType.Boost_CR.Id, new CurrencyPair{Currency = PieceType.Boost_CR.Abbreviations[0]}},
            {PieceType.Boost_WR.Id, new CurrencyPair{Currency = PieceType.Boost_WR.Abbreviations[0]}}
        };
        
        foreach (var item in data)
        {
            if (boosters.TryGetValue(item.Value, out var pair) == false) continue;

            pair.Amount += 1;
        }

        var collect = boosters.Values.ToList().FindAll(pair => pair.Amount > 0);

        if (collect.Count == 0) return;
        
        Analytics.SendPurchase("board_main", fog, null, collect, false, false);
    }
}