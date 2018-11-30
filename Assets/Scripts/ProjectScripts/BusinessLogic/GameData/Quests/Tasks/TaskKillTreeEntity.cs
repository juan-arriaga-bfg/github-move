[TaskHighlight(typeof(HighlightTaskUsingObstaclePieceFilter))]
[TaskHighlight(typeof(HighlightTaskNextFog))]
public class TaskKillTreeEntity : TaskEventCounterEntity
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

        Piece piece = lifeCmp?.Context;
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