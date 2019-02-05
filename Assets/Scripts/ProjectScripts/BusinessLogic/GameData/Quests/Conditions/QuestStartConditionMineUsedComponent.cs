using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Will check if mine with MineUid used by a player at least once.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public sealed class QuestStartConditionMineUsedComponent : QuestStartConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    [JsonProperty] public string MineUid { get; protected set; }

    private int pieceId;
    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        MineUid = Value;
        pieceId = PieceType.Parse(MineUid);
    }

    public override bool Check()
    {
        var board = BoardService.Current.FirstBoard;
        var positions = board.BoardLogic.PositionsCache.GetPiecePositionsByType(pieceId);

        if (positions.Count == 0) return false;
        
        var piece = board.BoardLogic.GetPieceAt(positions[0]);
        if (piece == null)
        {
            // Debug.LogError($"[QuestStartConditionMineUsedComponent] => Check: Piece instance not found for MineUid {MineUid} at {pos}");
            return false;
        }
        
        var life = piece.GetComponent<MineLifeComponent>(LifeComponent.ComponentGuid);
        if (life == null)
        {
            // Debug.LogError($"[QuestStartConditionMineUsedComponent] => Check: LifeComponent not found for MineUid {MineUid} at {pos}");
            return false;
        }

        var isDamaged = life.Current > 0;
        // Debug.Log($"[QuestStartConditionMineUsedComponent] => Check: {MineUid} health: {life.HP - life.Current}/{life.HP}, return {isDamaged}");
        
        return isDamaged;
    }
}