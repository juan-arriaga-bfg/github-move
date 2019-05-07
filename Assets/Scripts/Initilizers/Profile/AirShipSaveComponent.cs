using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class AirShipSaveComponent : ECSEntity, IECSSerializeable, IProfileSaveComponent
{
    public bool AllowDataCollect { get; set; }
    
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid => ComponentGuid;

    [JsonProperty("AirShips")] 
    public List<AirShipSaveItem> Items;

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if (!AllowDataCollect)  
        {
            return;
        }
        
        Items = BoardService.Current.FirstBoard.BoardLogic.AirShipLogic.Save();
    }
}