using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

public class TaskSaveItemJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (TaskSaveItem);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (TaskSaveItem) value;

        var pricesStr = new StringBuilder();
        var rewardsStr = new StringBuilder();

        foreach (var price in targetValue.Prices)
        {
            var save = price.ToSaveString();
            
            if(string.IsNullOrEmpty(save)) continue;
            
            pricesStr.Append(save);
            pricesStr.Append(";");
        }

        foreach (var reward in targetValue.Rewards)
        {
            rewardsStr.Append(reward.Key);
            rewardsStr.Append(",");
            rewardsStr.Append(reward.Value);
            rewardsStr.Append(";");
        }
        
        serializer.TypeNameHandling = TypeNameHandling.None;
        serializer.Serialize(writer, string.Format("{0}:{1}:{2}", targetValue.Result, pricesStr, rewardsStr));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize<string>(reader);
        var dataArray = data.Split(new string[] {":"}, StringSplitOptions.RemoveEmptyEntries);
        var pricesArray = dataArray[1].Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries);
        var rewardsArray = dataArray[2].Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries);

        var prices = new List<CurrencyPair>();
        var rewards = new Dictionary<int, int>();

        foreach (var str in pricesArray)
        {
            prices.Add(CurrencyPair.Parse(str));
        }

        foreach (var str in rewardsArray)
        {
            var strArr = str.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
            rewards.Add(int.Parse(strArr[0]), int.Parse(strArr[1]));
        }
        
        var targetValue = new TaskSaveItem
        {
            Result = int.Parse(dataArray[0]),
            Prices = prices,
            Rewards = rewards
        };
        
        return targetValue;
    }
}

[JsonConverter(typeof(TaskSaveItemJsonConverter))]
public class TaskSaveItem
{
    private int result;
    private List<CurrencyPair> prices;
    private Dictionary<int, int> rewards;
    
    public int Result
    {
        get { return result; }
        set { result = value; }
    }
    
    public List<CurrencyPair> Prices
    {
        get { return prices; }
        set { prices = value; }
    }

    public Dictionary<int, int> Rewards
    {
        get { return rewards; }
        set { rewards = value; }
    }
}