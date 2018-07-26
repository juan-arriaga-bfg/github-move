﻿using System;
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

        serializer.Serialize(writer, string.Format("{0},{1},{2},{3},{4}",
            targetValue.Id,
            (int) targetValue.State,
            targetValue.StartTime.ConvertToUnixTime(),
            targetValue.Position.ToSaveString(),
            targetValue.Reward.ToSaveString()));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new ChestSaveItem
        {
            Id = int.Parse(dataArray[0]),
            State = (ChestState) int.Parse(dataArray[1]),
            StartTime = DateTimeExtension.UnixTimeToDateTime(long.Parse(dataArray[2])),
            Position = new BoardPosition(int.Parse(dataArray[3]), int.Parse(dataArray[4]), int.Parse(dataArray[5])),
            Reward = DictionaryStringConverter.FromDataArray(dataArray, int.Parse, int.Parse, 6)
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(ChestSaveItemJsonConverter))]
public class ChestSaveItem
{
    private int id;
    private BoardPosition position;
    private ChestState state;
    private DateTime startTime;
    private Dictionary<int, int> reward;
    
    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    
    public BoardPosition Position
    {
        get { return position; }
        set { position = value; }
    }

    public ChestState State
    {
        get { return state; }
        set { state = value; }
    }

    public DateTime StartTime
    {
        get { return startTime; }
        set { startTime = value; }
    }

    public Dictionary<int, int> Reward
    {
        get { return reward; }
        set { reward = value; }
    }
}