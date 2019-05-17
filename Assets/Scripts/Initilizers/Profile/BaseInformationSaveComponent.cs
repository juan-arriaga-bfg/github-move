using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using IW.SimpleJSON;
using Newtonsoft.Json;
using UnityEngine;

public interface IBaseInformationSaveComponent
{
    BaseInformationSaveComponent BaseInformation { get; }
}

public partial class UserProfile : IBaseInformationSaveComponent
{
    protected BaseInformationSaveComponent baseInformationSaveComponent;

    [JsonIgnore]
    public BaseInformationSaveComponent BaseInformation => baseInformationSaveComponent ?? (baseInformationSaveComponent = GetComponent<BaseInformationSaveComponent>(BaseInformationSaveComponent.ComponentGuid));
}

public class MatchesCounterJsonConverter : JsonConverter
{
    private const string DELIMITER_PROPERTY = ";";

    private readonly string[] DELIMITER_SEMICOLON = {DELIMITER_PROPERTY};

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (DailyRewardSaveItem);
    }
    
    // "BaseInformationSaveComponent": {
    //     "MatchesCounter" : "TotalCount;EffectiveCount"
    // }
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (MatchesCounter) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;

        StringBuilder sb = new StringBuilder();
        sb.Append(targetValue.TotalMatchesCount);
        sb.Append(DELIMITER_PROPERTY);
        sb.Append(targetValue.EffectiveMatchesCount);

        serializer.Serialize(writer, sb.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);

        var dataArray = data.Split(DELIMITER_SEMICOLON, StringSplitOptions.RemoveEmptyEntries);

        var targetValue = new MatchesCounter(int.Parse(dataArray[0]), int.Parse(dataArray[1]));

        return targetValue;
    }
}

[JsonConverter(typeof(MatchesCounterJsonConverter))]
public class MatchesCounter
{
    public int TotalMatchesCount { get; private set; }
    
    public int EffectiveMatchesCount { get; private set; }

    public double Effectiveness => TotalMatchesCount != 0 ? Math.Round(EffectiveMatchesCount / (double)TotalMatchesCount, 3) : -1;

    public MatchesCounter(int totalCount, int effectiveCount)
    {
        TotalMatchesCount = totalCount;
        EffectiveMatchesCount = effectiveCount;
    }
    
    public void AddMatch(MatchDescription matchDescription)
    {
        int matchedCount = matchDescription.SourcePiecesCount;
        bool isEffective = matchedCount >= 15 || (matchedCount > 0 && matchedCount % 5 == 0);
        
        TotalMatchesCount++;
        if (isEffective)
        {
            EffectiveMatchesCount++;
        }

        IW.Logger.Log($"[MatchesCounter] => AddMatch({matchDescription}, effective: {isEffective}): Effectiveness: {TotalMatchesCount}/{EffectiveMatchesCount} = {Effectiveness}");
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class BaseInformationSaveComponent : ECSEntity, IECSSerializeable, IProfileSaveComponent
{
    public bool AllowDataCollect { get; set; }

    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
	
    /// <summary>
    /// Time when player install the game from a Store
    /// </summary>
    [JsonProperty]
    public long CreationTimestamp = UnixTimeHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);
    
    /// <summary>
    /// Time when player download and install a new client version
    /// </summary>
    [JsonProperty]
    public long UpdateTimestamp;

    [JsonProperty]
    public bool IsPayer;
    
    [JsonProperty]
    public List<string> IapsList = new List<string>();

    [JsonProperty]
    public MatchesCounter MatchesCounter = new MatchesCounter(0 ,0);

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        if (!AllowDataCollect)
        {
            return;
        }
        
        
    }

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
    }
}