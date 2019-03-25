using System.Runtime.Serialization;
using Newtonsoft.Json;
using Debug = IW.Logger;

[TaskHighlight(typeof(HighlightTaskTreeBranch))]
[TaskHighlight(typeof(HighlightTaskAnyTree))]
[TaskHighlight(typeof(HighlightTaskNextFog))]
public class TaskHitTreeEntity : TaskEventCounterEntity, IHavePieceId
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected override int EventCode => GameEventsCodes.StorageDamage;
    
    public int PieceId { get; protected set; }

    private string targetBranch;
    
#region Serialization

    [JsonProperty] public string PieceUid;
    
    public bool ShouldSerializePieceId()
    {
        return false;
    }    
    
    public bool ShouldSerializePieceUid()
    {
        return false;
    }
    
    [OnDeserialized]
    protected void OnDeserializedTaskCounterAboutPiece(StreamingContext context)
    {
        if (!string.IsNullOrEmpty(PieceUid))
        {
            PieceId = PieceType.Parse(PieceUid);
            targetBranch = HighlightTaskPointToPieceSourceHelper.PieceBranchRegexComplex.Match(PieceUid).Value;

            // fallback for tutorial pieces
            if (targetBranch == "TT")
            {
                targetBranch = "A";
            }
            
            if (string.IsNullOrEmpty(targetBranch))
            {
                Debug.LogError($"[TaskHitTreeEntity] => OnDeserializedTaskCounterAboutPiece: Branch for {PieceUid} is empty!");
            }
        }
    }
    
#endregion

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
            // No target defined
            if (PieceId == PieceType.None.Id || PieceId == PieceType.Empty.Id)
            {
                CurrentValue += 1;
                return;
            }
            
            string branch = HighlightTaskPointToPieceSourceHelper.PieceBranchRegexComplex.Match(pieceTypeDef.Abbreviations[0]).Value;
            if (string.IsNullOrEmpty(branch))
            {
                Debug.LogError($"[TaskHitTreeEntity] => OnBoardEvent: Branch for {pieceTypeDef.Abbreviations[0]} is empty!");
                return;
            }

            if (branch == targetBranch)
            {
                CurrentValue += 1; 
            }
        }
    }
}