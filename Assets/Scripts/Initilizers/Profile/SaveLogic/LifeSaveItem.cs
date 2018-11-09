﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class LifeSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (LifeSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (LifeSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        serializer.Serialize(writer, $"{targetValue.Step},{targetValue.StorageSpawnPiece},{targetValue.StartTime},{(targetValue.IsStart ? 1 : 0)},{targetValue.Position.ToSaveString()},{targetValue.Reward.ToSaveString()}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new LifeSaveItem
        {
            Step = int.Parse(dataArray[0]),
            StorageSpawnPiece = int.Parse(dataArray[1]),
            StartTime = long.Parse(dataArray[2]),
            IsStart = int.Parse(dataArray[3]) == 1,
            Position = new BoardPosition(int.Parse(dataArray[4]), int.Parse(dataArray[5]), int.Parse(dataArray[6])),
            Reward = DictionaryStringConverter.FromDataArray(dataArray, int.Parse, int.Parse, 7)
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(LifeSaveItemJsonConverter))]
public class LifeSaveItem
{
    private int step;
    private int storageSpawnPiece;
    private long startTime;
    private BoardPosition position;
    private bool isStart;
    private Dictionary<int, int> reward;
    
    public int Step
    {
        get { return step; }
        set { step = value; }
    }
    
    public int StorageSpawnPiece
    {
        get { return storageSpawnPiece; }
        set { storageSpawnPiece = value; }
    }
    
    public long StartTime
    {
        get { return startTime; }
        set { startTime = value; }
    }
    
    public BoardPosition Position
    {
        get { return position; }
        set { position = value; }
    }
    
    public bool IsStart
    {
        get { return isStart; }
        set { isStart = value; }
    }

    public Dictionary<int, int> Reward
    {
        get { return reward; }
        set { reward = value; }
    }
}