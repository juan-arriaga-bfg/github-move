public class TouchReactionDefinitionObstacleComponent:TouchReactionDefinitionSpawnInStorage
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        var storageLife = piece.GetComponent<StorageLifeComponent>(StorageLifeComponent.ComponentGuid);
        var storage = piece.GetComponent<StorageComponent>(StorageComponent.ComponentGuid);
        var health = storageLife.HP;
        if (health == 1)
        {
            int tmp;
            if (storage.Scatter(out tmp, IsAutoStart) == false)
            {
                UIErrorWindowController.AddError("Production of the resource is not complete!");
                return false;
            }
            
            piece.Context.ActionExecutor.AddAction(new ChangePieceAction()
            {
                Position = position,
                TargetPieceId = storage.SpawnPiece
            });
            return true;
        }

        return base.Make(position, piece);
    }
}