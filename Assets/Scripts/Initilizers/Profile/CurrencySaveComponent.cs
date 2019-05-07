using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class CurrencySaveComponent : ECSEntity, IECSSerializeable, IProfileSaveComponent
{
    public bool AllowDataCollect { get; set; }

    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    [JsonProperty]
    public long EnergyLastUpdate;
    
    [JsonProperty]
    public string WorkerUnlockDelay;

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if (!AllowDataCollect)
        {
            return;
        }
        
        if(BoardService.Current == null) return;
        
        var board = BoardService.Current.FirstBoard;
        var energyLogic = board.GetComponent<EnergyCurrencyLogicComponent>(EnergyCurrencyLogicComponent.ComponentGuid);
        var workerLogic = board.GetComponent<WorkerCurrencyLogicComponent>(WorkerCurrencyLogicComponent.ComponentGuid);

        EnergyLastUpdate = energyLogic.LastUpdate;
        WorkerUnlockDelay = workerLogic.Save();
    }

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
    }
}