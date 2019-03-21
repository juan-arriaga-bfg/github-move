using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class FogSaveComponent : BaseSaveComponent, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private Dictionary<string, FogSaveItem> fogSave;

    [JsonProperty] public string CompleteFogs;

    [JsonProperty] public List<FogSaveItem> InprogressFogs;

    public List<string> CompleteFogIds;
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
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
        
        fogSave = new Dictionary<string, FogSaveItem>();
        
        if (InprogressFogs != null)
        {
            foreach (var fog in InprogressFogs)
            {
                fogSave.Add(fog.Uid, fog);
            }
        }
    }
    
    public FogSaveItem GetRewardsSave(string uid)
    {
        if (fogSave == null || fogSave.TryGetValue(uid, out var item) == false) return null;

        fogSave.Remove(uid);
		
        return item;
    }
}
