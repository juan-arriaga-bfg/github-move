using System.Collections.Generic;

public class TouchReactionDefinitionObstacleComponent : TouchReactionDefinitionSpawnInStorage
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var storageLife = piece.GetComponent<StorageLifeComponent>(StorageLifeComponent.ComponentGuid);
        var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        
        if (storageLife.Current == storageLife.HP)
        {
            int tmp;
            if (storage.Scatter(out tmp, IsAutoStart) == false)
            {
                UIErrorWindowController.AddError("Production of the resource is not complete!");
                return false;
            }
            
            piece.Context.ActionExecutor.AddAction(new CollapsePieceToAction
            {
                Positions = new List<BoardPosition> {position},
                To = position,
                OnCompleteAction = new CreatePieceAtAction
                {
                    At = position,
                    PieceTypeId = GameDataService.Current.ObstaclesManager.GetReward(piece.PieceType),
                    OnComplete = () =>
                    {
                        if (storage.OnScatter != null) storage.OnScatter();
                    }
                }
            });
            return true;
        }

        return base.Make(position, piece);
    }
}