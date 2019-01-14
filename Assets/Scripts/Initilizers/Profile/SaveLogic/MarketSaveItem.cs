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
        serializer.Serialize(writer, $"{targetValue.Index},{targetValue.ItemIndex},{(int)targetValue.State},{targetValue.Piece},{targetValue.Amount}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
        var targetValue = new MarketSaveItem
        {
            Index = int.Parse(dataArray[0]),
            ItemIndex = int.Parse(dataArray[1]),
            State = (MarketItemState)int.Parse(dataArray[2]),
            Piece = int.Parse(dataArray[3]),
            Amount = int.Parse(dataArray[4])
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(MarketSaveItemJsonConverter))]
public class MarketSaveItem
{
    private int index;
    private int itemIndex;
    private MarketItemState state;
    private int piece;
    private int amount;

    public int Index
    {
        get { return index; }
        set { index = value; }
    }

    public int ItemIndex
    {
        get { return itemIndex; }
        set { itemIndex = value; }
    }

    public MarketItemState State
    {
        get { return state; }
        set
        {
            if (value == MarketItemState.Purchased) value = MarketItemState.Saved;
            
            state = value;
        }
    }

    public int Piece
    {
        get { return piece; }
        set { piece = value; }
    }

    public int Amount
    {
        get { return amount; }
        set { amount = value; }
    }
}
