using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

public class PieceSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (PieceSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (PieceSaveItem) value;
        var str = new StringBuilder();

        foreach (var position in targetValue.Positions)
        {
            str.Append(position.ToSaveString());
            str.Append(";");
        }
        
        serializer.TypeNameHandling = TypeNameHandling.None;

        serializer.Serialize(writer, string.Format("{0}:{1}", targetValue.Id, str));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var targetValue = new PieceSaveItem{Positions = new List<BoardPosition>()};

        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new string[] {":"}, StringSplitOptions.RemoveEmptyEntries);
        var posArray = dataArray[1].Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries);
        
        targetValue.Id = int.Parse(dataArray[0]);
        
        foreach (var str in posArray)
        {
            targetValue.Positions.Add(BoardPosition.Parse(str));
        }
        
        return targetValue;
    }
}

[JsonConverter(typeof(PieceSaveItemJsonConverter))]
public class PieceSaveItem
{
    private int id;
    
    private List<BoardPosition> positions;
    
    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    
    public List<BoardPosition> Positions
    {
        get { return positions; }
        set { positions = value; }
    }
}