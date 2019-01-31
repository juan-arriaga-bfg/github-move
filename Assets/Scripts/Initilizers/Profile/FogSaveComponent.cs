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
    
    [JsonProperty]
    public string CompleteFogs
    {
        get { return completeFogs; }
        set { completeFogs = value; }
    }
    
    public List<BoardPosition> CompleteFogPositions;
    
    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        completeFogs = PositionsToString(GameDataService.Current.FogsManager.ClearedFogPositions.Keys.ToList());
    }

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        CompleteFogPositions = StringToPositions(completeFogs);
    }
}
