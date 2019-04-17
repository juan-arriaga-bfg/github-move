using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class AirShipSaveComponent : ECSEntity, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid => ComponentGuid;

    [JsonProperty("AirShips")] 
    public List<AirShipSaveItem> Items;

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        Items = BoardService.Current.FirstBoard.BoardLogic.AirShipLogic.Save();
    }
    
    //
    // [OnDeserialized]
    // internal void OnDeserialized(StreamingContext context)
    // {
    // }
}