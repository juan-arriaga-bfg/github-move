using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class FogSaveComponent : BaseSaveComponent, IECSSerializeable, IProfileSaveComponent
{
    public bool AllowDataCollect { get; set; }

    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private Dictionary<string, FogSaveItem> cache;

    [JsonProperty] public string CompleteFogs;

    [JsonProperty] public List<FogSaveItem> InprogressFogs;

    public List<string> CompleteFogIds;
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if (!AllowDataCollect)
        {
            return;
        }
        
        var data = GameDataService.Current.FogsManager;
        
        InprogressFogs = new List<FogSaveItem>();
        List<string> idsList = data.ClearedFogPositions.Values.Select(e => e.Uid).ToList();
        CompleteFogs = IdsToString(idsList);

        foreach (var pair in data.VisibleFogPositions)
        {
            BoardPosition key = pair.Key;
            FogDef def = pair.Value;
            
            if (data.FogObservers.TryGetValue(key, out var observer) == false
                || observer.IsRemoved
                || observer.RequiredConditionReached() == false
                || observer.AlreadyPaid.Amount == 0) continue;
            
            InprogressFogs.Add(new FogSaveItem {Uid = def.Uid, AlreadyPaid = observer.AlreadyPaid.Amount});
        }
    }

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        CompleteFogIds = StringToIds(CompleteFogs);
        UpdateCache();
    }

    public void UpdateCache()
    {
        cache = new Dictionary<string, FogSaveItem>();
        
        if (InprogressFogs != null)
        {
            foreach (var fog in InprogressFogs)
            {
                cache.Add(fog.Uid, fog);
            }
        }
    }
    
    public FogSaveItem GetRewardsSave(string uid)
    {
        if (cache == null || cache.TryGetValue(uid, out var item) == false) return null;

        cache.Remove(uid);
		
        return item;
    }
}
