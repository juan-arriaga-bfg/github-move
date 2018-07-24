using System;
using Newtonsoft.Json;

public class EnemySaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (EnemySaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (EnemySaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;

        serializer.Serialize(writer, string.Format("{0},{1},{2}",
            targetValue.Damage,
            targetValue.Reward,
            targetValue.Step));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new EnemySaveItem
        {
            Damage = int.Parse(dataArray[0]),
            Reward = int.Parse(dataArray[1]),
            Step = int.Parse(dataArray[2]),
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(EnemySaveItemJsonConverter))]
public class EnemySaveItem
{
    private int damage;
    private int reward = -1;
    private int step;
    
    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public int Reward
    {
        get { return reward; }
        set { reward = value; }
    }

    public int Step
    {
        get { return step; }
        set { step = value; }
    }
}