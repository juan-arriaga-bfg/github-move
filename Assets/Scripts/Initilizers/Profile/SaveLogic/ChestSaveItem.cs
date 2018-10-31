using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public static class DictionaryStringConverter
{
    public static string ToSaveString<TKey, TValue>(this Dictionary<TKey, TValue> target)
    {
        StringBuilder strBuilder = new StringBuilder();

        foreach (var elem in target)
        {
            strBuilder.Append(elem.Key);
            strBuilder.Append(",");
            strBuilder.Append(elem.Value);
            strBuilder.Append(",");
        }

        if (target.Count > 0)
            strBuilder.Remove(strBuilder.Length - 1, 1);

        return strBuilder.ToString();
    }

    public static Dictionary<TKey, TValue> FromDataArray<TKey, TValue>(string[] dataArray, Func<string, TKey> parseKey, Func<string, TValue> parseValue, 
                                                                       int beginPosition, int endPosition = -1)
    {
        if (endPosition < 0)
            endPosition = dataArray.Length;
        Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
        for (int i = beginPosition; i < endPosition; i+=2)
        {
            dict.Add(parseKey(dataArray[i]), parseValue(dataArray[i+1]));
        }

        return dict;
    }
}

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
        serializer.Serialize(writer, $"{targetValue.Position.ToSaveString()}, {targetValue.RewardAmount}, {targetValue.Reward.ToSaveString()},");
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