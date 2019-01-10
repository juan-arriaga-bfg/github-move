using System;
using Newtonsoft.Json;

public class MarketSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (MarketSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (MarketSaveItem) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        serializer.Serialize(writer, $"{targetValue.Index},{targetValue.ItemIndex},{(targetValue.IsPurchased ? 1 : 0)},{targetValue.Piece},{targetValue.Amount}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new MarketSaveItem
        {
            Index = int.Parse(dataArray[0]),
            ItemIndex = int.Parse(dataArray[1]),
            IsPurchased = int.Parse(dataArray[2]) == 1,
            Piece = int.Parse(dataArray[3]),
            Amount = int.Parse(dataArray[4])
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(MarketSaveItemJsonConverter))]
public class MarketSaveItem
{
    public int Index;
    public int ItemIndex;
    public bool IsPurchased;
    public int Piece;
    public int Amount;
}
