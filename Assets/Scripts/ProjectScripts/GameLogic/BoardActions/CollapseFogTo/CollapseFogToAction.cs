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
        if (FogObserver == null || FogObserver.Def == null)
        {
            return true;
        }

        GameDataService.Current.FogsManager.RemoveFog(FogObserver.Key);
        
        AddResourceView.Show(FogObserver.Def.GetCenter(gameBoardController), FogObserver.Def.Reward, 0.5f);

        
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
                    
                    gameBoardController.ActionExecutor.PerformAction(new CreatePieceAtAction
                    {
                        At = pos,
                        PieceTypeId = pieceId
                    });
                }
            }
        }
        
        var weights = FogObserver.Def.PieceWeights == null || FogObserver.Def.PieceWeights.Count == 0
            ? GameDataService.Current.FogsManager.DefaultPieceWeights
            : FogObserver.Def.PieceWeights;
        
        foreach (var point in FogObserver.Mask)
        {
            var piece = ItemWeight.GetRandomItem(weights).Piece;
            
            if(piece == PieceType.Empty.Id) continue;
            
            gameBoardController.ActionExecutor.PerformAction(new CreatePieceAtAction
            {
                At = point,
                PieceTypeId = piece
            });
        }

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
