using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class FogSaveComponent : BaseSaveComponent, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    private string completeFogs;
    private List<FogSaveItem> inprogressFogs;
    
    private Dictionary<BoardPosition, FogSaveItem> fogSave;
    
    [JsonProperty]
    public string CompleteFogs
    {
        get { return completeFogs; }
        set { completeFogs = value; }
    }
    
    [JsonProperty]
    public List<FogSaveItem> InprogressFogs
    {
        get { return inprogressFogs; }
        set { inprogressFogs = value; }
    }
    
    public List<BoardPosition> CompleteFogPositions;
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        var data = GameDataService.Current.FogsManager;
        
        inprogressFogs = new List<FogSaveItem>();
        completeFogs = PositionsToString(data.ClearedFogPositions.Keys.ToList());

        foreach (var key in data.VisibleFogPositions.Keys)
        {
            if (data.FogObservers.TryGetValue(key, out var observer) == false
                || observer.IsRemoved
                || observer.RequiredLevelReached() == false
                || observer.AlreadyPaid.Amount == 0) continue;
            
            inprogressFogs.Add(new FogSaveItem {Position = key, AlreadyPaid = observer.AlreadyPaid.Amount});
        }
    }

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        CompleteFogPositions = StringToPositions(completeFogs);
        
        fogSave = new Dictionary<BoardPosition, FogSaveItem>();
        
        if (inprogressFogs != null)
        {
            foreach (var fog in inprogressFogs)
            {
                fogSave.Add(fog.Position, fog);
            }
        }
    }
    
    public FogSaveItem GetRewardsSave(BoardPosition position)
    {
        if (fogSave == null || fogSave.TryGetValue(position, out var item) == false) return null;

        fogSave.Remove(position);
		
        return item;
    }
}
