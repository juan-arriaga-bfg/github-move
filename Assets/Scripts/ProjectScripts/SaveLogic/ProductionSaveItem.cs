using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

public class ProductionSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (ProductionSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (ProductionSaveItem) value;
        
        var str = new StringBuilder();

        foreach (var item in targetValue.Storage)
        {
            str.Append(item.Key);
            str.Append(",");
            str.Append(item.Value);
            str.Append(";");
        }
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        
        serializer.Serialize(writer,
            string.Format("{0},{1},{2},{3}:{4}",
                targetValue.Id,
                targetValue.Position.ToSaveString(),
                (int) targetValue.State,
                targetValue.StartTime,
                str));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var targetValue = new ProductionSaveItem{Storage = new Dictionary<int, int>()};

        var data = serializer.Deserialize<string>(reader);
        var array = data.Split(new string[] {":"}, StringSplitOptions.RemoveEmptyEntries);
        var dataArray = array[0].Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
        var storageArray = array[1].Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries);
        
        targetValue.Id = int.Parse(dataArray[0]);
        targetValue.Position = new BoardPosition(int.Parse(dataArray[1]), int.Parse(dataArray[2]), int.Parse(dataArray[3]));
        targetValue.State = (ProductionState) int.Parse(dataArray[4]);
        targetValue.StartTime = long.Parse(dataArray[5]);
        
        foreach (var str in storageArray)
        {
            var strArray = str.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
            
            targetValue.Storage.Add(int.Parse(strArray[0]), int.Parse(strArray[1]));
        }
        
        return targetValue;
    }
}

[JsonConverter(typeof(ProductionSaveItemJsonConverter))]
public class ProductionSaveItem
{
    private int id;
    private BoardPosition position;
    private ProductionState state;
    private long startTime;
    private Dictionary<int, int> storage;

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

    public ProductionState State
    {
        get { return state; }
        set { state = value; }
    }

    public long StartTime
    {
        get { return startTime; }
        set { startTime = value; }
    }

    public Dictionary<int, int> Storage
    {
        get { return storage; }
        set { storage = value; }
    }
}