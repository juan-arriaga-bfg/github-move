[TaskHighlight(typeof(HighlightTaskNextFog))]
public class TaskKillFieldEntity : TaskEventCounterEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected override int EventCode => GameEventsCodes.ObstacleKilled;
    
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

        // Skip fake
        if (pieceTypeDef.Filter.Has(PieceTypeFilter.Reproduction))
        {
            CurrentValue += 1;
        }
    }
}