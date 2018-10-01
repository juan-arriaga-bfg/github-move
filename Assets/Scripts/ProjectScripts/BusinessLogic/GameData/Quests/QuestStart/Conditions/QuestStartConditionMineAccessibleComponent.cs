using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public sealed class QuestStartConditionMineAccessibleComponent : QuestStartConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    [JsonProperty] public int MineId { get; protected set; }

    public override bool Check()
    {
        var board = BoardService.Current.FirstBoard;
        var position = board.BoardLogic.PositionsCache.GetPiecePositionsByType(MineId);

        return false;
    }
}