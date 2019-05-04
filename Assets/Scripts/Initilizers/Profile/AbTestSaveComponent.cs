using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

public interface IAbTestSaveComponent
{
    AbTestSaveComponent AbTestSave { get; }
}

public partial class UserProfile : IAbTestSaveComponent
{
    protected AbTestSaveComponent abTestSaveComponent;

    [JsonIgnore]
    public AbTestSaveComponent AbTestSave => abTestSaveComponent ?? (abTestSaveComponent = GetComponent<AbTestSaveComponent>(AbTestSaveComponent.ComponentGuid));
}

public class AbTestItemJsonConverter : JsonConverter
{
    private const string DELIMITER_PROPERTY = ";";

    private readonly string[] DELIMITER_SEMICOLON = {DELIMITER_PROPERTY};

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (DailyRewardSaveItem);
    }
    
    // "AbTestSaveComponent": {
    //     "Tests" : [
    //         "Name;Group"
    //     ]
    // }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (AbTestItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;

        StringBuilder sb = new StringBuilder();
        sb.Append(targetValue.TestName);
        sb.Append(DELIMITER_PROPERTY);
        sb.Append(targetValue.UserGroup);

        serializer.Serialize(writer, sb.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var targetValue = new AbTestItem();

        var data = serializer.Deserialize<string>(reader);

        var dataArray = data.Split(DELIMITER_SEMICOLON, StringSplitOptions.RemoveEmptyEntries);

        targetValue.TestName = dataArray[0];
        targetValue.UserGroup = dataArray[1];

        return targetValue;
    }
}

[JsonConverter(typeof(AbTestItemJsonConverter))]
public class AbTestItem
{
    public int GroupsCount;
    public string UserGroup;
    public string TestName;
}

[JsonObject(MemberSerialization.OptIn)]
public class AbTestSaveComponent : IECSComponent, IECSSerializeable
{
    public static int ComponentGuid = ECSManager.GetNextGuid();

    public int Guid => ComponentGuid;

    public void OnRegisterEntity(ECSEntity entity) {}

    public void OnUnRegisterEntity(ECSEntity entity) {}

    [JsonProperty] 
    public List<AbTestItem> Tests;

    [OnSerializing]
    internal void OnSerialization(StreamingContext context)
    {
        Tests = GameDataService.Current.AbTestManager.Tests.Values.ToList();
    }
}