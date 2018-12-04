[TaskHighlight(typeof(HighlightTaskTree))]
[TaskHighlight(typeof(HighlightTaskNextFog))]
public class TaskHitTreeEntity : TaskEventCounterEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected override int EventCode => GameEventsCodes.StorageDamage;
    
    public override void OnBoardEvent(int code, object context)
    {
        if (!IsInProgress())
        {
            return;
        }

        if (code != EventCode)
        {
            return;
        }

        ObstacleLifeComponent lifeCmp = context as ObstacleLifeComponent;

        Piece piece = lifeCmp?.GetContext();
        if (piece == null)
        {
            return;
        }
        
        PieceTypeDef pieceTypeDef = PieceType.GetDefById(piece.PieceType);

        if (pieceTypeDef.Filter.Has(PieceTypeFilter.Tree))
        {
            CurrentValue += 1;
        }
    }
}