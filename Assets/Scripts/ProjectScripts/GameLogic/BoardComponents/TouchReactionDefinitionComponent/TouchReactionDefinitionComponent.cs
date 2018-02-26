using System.Collections.Generic;

public class TouchReactionDefinitionComponent : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid { get { return ComponentGuid; } }
    
    public virtual bool Make(BoardPosition position, Piece piece)
    {
        piece.Context.ActionExecutor.AddAction(new SpawnPiecesAction()
        {
            At = position,
            Pieces = new List<int> {PieceType.B1.Id}
        });
        
        return true;
    }
}