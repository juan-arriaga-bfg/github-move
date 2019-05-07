using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

public interface ICodexSaveComponent
{
    CodexSaveComponent CodexSave { get; }
}

public partial class UserProfile : ICodexSaveComponent
{
    protected CodexSaveComponent codexSaveComponent;

    [JsonIgnore]
    public CodexSaveComponent CodexSave
    {
        get
        {
            if (codexSaveComponent == null)
            {
                codexSaveComponent = GetComponent<CodexSaveComponent>(CodexSaveComponent.ComponentGuid);
            }

            return codexSaveComponent;
        }
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class CodexSaveComponent : ECSEntity, IECSSerializeable, IProfileSaveComponent
{
    public bool AllowDataCollect { get; set; }

    public static int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid => ComponentGuid;

    [JsonProperty("Data")]
    public Dictionary<int, CodexChainState> Data;
    
    [JsonProperty("State")]
    public CodexState State;

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if (!AllowDataCollect)
        {
            return;
        }
        
        var codexManager = GameDataService.Current?.CodexManager;
        if (codexManager == null)
        {
            return;
        }
        
        var items = codexManager.Items ?? new Dictionary<int, CodexChainState>();
        Data = items;

        State = codexManager.CodexState;
    }
    
    //
    // [OnDeserialized]
    // internal void OnDeserialized(StreamingContext context)
    // {
    // }
}