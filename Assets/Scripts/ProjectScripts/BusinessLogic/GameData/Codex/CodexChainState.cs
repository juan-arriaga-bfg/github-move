using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

public class CodexChainStateJsonConverter : JsonConverter
{
    private readonly string[] DELIMITER_SEMICOLON = {";"};
    private readonly string[] DELIMITER_COLON = {":"};
    
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (CodexChainState);
    }
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var targetValue = (CodexChainState) value;
        
        serializer.TypeNameHandling = TypeNameHandling.None;

        StringBuilder sb = new StringBuilder();
        foreach (int item in targetValue.Unlocked)
        {
            string pendingReward = targetValue.PendingReward.Count == 0 || !targetValue.PendingReward.Contains(item) ? "0" : "1";
            string text = item + ":" + pendingReward + ";"; 
            sb.Append(text);
        }

        serializer.Serialize(writer, sb.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var targetValue = new CodexChainState();

        var data = serializer.Deserialize<string>(reader);
        
        var dataArray = data.Split(DELIMITER_SEMICOLON, StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < dataArray.Length; i++)
        {
            var itemStr   = dataArray[i];
            var itemArray = itemStr.Split(DELIMITER_COLON, StringSplitOptions.RemoveEmptyEntries);
            int pieceId   = int.Parse(itemArray[0]);
            
            targetValue.Unlocked.Add(pieceId);
            
            if (itemArray[1] != "0")
            {
                targetValue.PendingReward.Add(pieceId);
            }
        }

        return targetValue;
    }
}

[JsonConverter(typeof(CodexChainStateJsonConverter))]
public class CodexChainState
{
    public HashSet<int> Unlocked = new HashSet<int>();
    public HashSet<int> PendingReward = new HashSet<int>(); 
}