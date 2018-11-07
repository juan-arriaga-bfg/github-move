using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ChestSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (ChestSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (ChestSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        serializer.Serialize(writer, $"{targetValue.Position.ToSaveString()},{targetValue.RewardAmount},{targetValue.Reward.ToSaveString()},");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new ChestSaveItem
        {
            Position = new BoardPosition(int.Parse(dataArray[0]), int.Parse(dataArray[1]), int.Parse(dataArray[2])),
            RewardAmount = int.Parse(dataArray[3]),
            Reward = DictionaryStringConverter.FromDataArray(dataArray, int.Parse, int.Parse, 4)
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(ChestSaveItemJsonConverter))]
public class ChestSaveItem
{
    private BoardPosition position;
    private Dictionary<int, int> reward;
    private int rewardAmount;
    
    public BoardPosition Position
    {
        get { return position; }
        set { position = value; }
    }

    public Dictionary<int, int> Reward
    {
        get { return reward; }
        set { reward = value; }
    }

    public int RewardAmount
    {
        get { return rewardAmount; }
        set { rewardAmount = value; }
    }
}