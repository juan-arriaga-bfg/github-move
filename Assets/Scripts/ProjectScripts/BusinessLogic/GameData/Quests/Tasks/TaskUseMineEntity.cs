using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[TaskHighlight(typeof(HighlightTaskUseMine))]
[TaskHighlight(typeof(HighlightTaskNextFog))]
public class TaskUseMineEntity : TaskEventCounterEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected override int EventCode => GameEventsCodes.MineUsed;

    public int PieceId { get; protected set; } = -1;

    private List<int> chain;
    
    public List<int> Chain => chain ?? FillChain();

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
        if (string.IsNullOrEmpty(PieceUid))
        {
            return;
        }
        
        PieceId = PieceType.Parse(PieceUid);
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

        if (PieceId <= 0)
        {
            CurrentValue += 1;
            return;
        }

        if (context is MineLifeComponent lifeComponent)
        {
            int id = lifeComponent.Context.PieceType;
            if (IsPieceInChain(id))
            {
                CurrentValue += 1;
            }
        }
    }

    private List<int> FillChain()
    {
        if (chain == null)
        {
            chain = GameDataService.Current.MatchDefinition.GetChain(PieceId);
            for (int i = chain.Count - 1; i >= 0; i--)
            {
                PieceTypeDef def = PieceType.GetDefById(chain[i]);
                if (!def.Filter.Has(PieceTypeFilter.Mine)/* || def.Filter.Has(PieceTypeFilter.Fake)*/)
                {
                    chain.RemoveAt(i); 
                }
            }
        }

        return chain;
    }
    
    private bool IsPieceInChain(int id)
    {
        return Chain.Contains(id);
    }

    public override string GetIco()
    {
        var baseIco = base.GetIco();
        
        if (!string.IsNullOrEmpty(baseIco) || PieceId == PieceType.None.Id || PieceId == PieceType.Empty.Id || Chain.Count == 0)
        {
            return baseIco;
        }
        
        return PieceType.Parse(Chain[1]);
    }
}