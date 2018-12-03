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
    
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        MineUid = Value;
    }

    public override bool Check()
    {
        BoardPosition pos;
        bool isOk = GameDataService.Current.MinesManager.GetMinePositionByUid(MineUid, out pos);
        if (!isOk)
        {
            // Assume that mine was removed
            
            // Debug.Log($"[QuestStartConditionMineUsedComponent] => Check: {MineUid} removed, return true");
            return true;
        }
        
        var board = BoardService.Current.FirstBoard;
        
        pos.Z = BoardLayer.Piece.Layer;

        var piece = board.BoardLogic.GetPieceAt(pos);
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

        bool isDamaged = life.Current > 0;
        // Debug.Log($"[QuestStartConditionMineUsedComponent] => Check: {MineUid} health: {life.HP - life.Current}/{life.HP}, return {isDamaged}");
        
        return isDamaged;
    }
}