
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

public interface ICodexDailyRewardSaveComponent
{
    DailyRewardSaveComponent DailyRewardSave { get; }
}

public partial class UserProfile : ICodexDailyRewardSaveComponent
{
    protected DailyRewardSaveComponent dailyRewardSaveComponent;

    [JsonIgnore]
    public DailyRewardSaveComponent DailyRewardSave
    {
        get
        {
            if (dailyRewardSaveComponent == null)
            {
                dailyRewardSaveComponent = GetComponent<DailyRewardSaveComponent>(DailyRewardSaveComponent.ComponentGuid);
            }

            return dailyRewardSaveComponent;
        }
    }
}

public class DailyRewardSaveItemJsonConverter : JsonConverter
{
    private const string DELIMITER_PROPERTY = ";";

    private readonly string[] DELIMITER_SEMICOLON = {DELIMITER_PROPERTY};

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (DailyRewardSaveItem);
    }
    
    // "DailyRewardSaveComponent": {
    //     "Data" : "Activated;Day;Timer"
    // }
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (DailyRewardSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;

        StringBuilder sb = new StringBuilder();
        sb.Append(targetValue.IsActivated ? "1" : "0");
        sb.Append(DELIMITER_PROPERTY);
        sb.Append(targetValue.Day);
        sb.Append(DELIMITER_PROPERTY);
        sb.Append(targetValue.TimerStart);

        serializer.Serialize(writer, sb.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var targetValue = new DailyRewardSaveItem();

        var data = serializer.Deserialize<string>(reader);

        var dataArray = data.Split(DELIMITER_SEMICOLON, StringSplitOptions.RemoveEmptyEntries);

        targetValue.IsActivated = dataArray[0] == "1";
        targetValue.Day = int.Parse(dataArray[1]);
        targetValue.TimerStart = long.Parse(dataArray[2]);

        return targetValue;
    }
}

[JsonConverter(typeof(DailyRewardSaveItemJsonConverter))]
public class DailyRewardSaveItem
{
    public long TimerStart;

    public int Day;

    public bool IsActivated;
}

// [JsonConverter(typeof(DailyRewardSaveComponentJsonConverter))]
[JsonObject(MemberSerialization.OptIn)]
public class DailyRewardSaveComponent : IECSComponent, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid => ComponentGuid;
    
    public void OnRegisterEntity(ECSEntity entity)
    {
        
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        
    }

    [JsonProperty]
    public DailyRewardSaveItem Data = new DailyRewardSaveItem();

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if (Data == null)
        {
            Data = new DailyRewardSaveItem();
        }
        
        var manager = GameDataService.Current.DailyRewardManager;
        Data.TimerStart = manager.Timer?.StartTimeLong ?? 0;
        Data.Day = manager.Day;
        Data.IsActivated = manager.IsActivated;
    }
}