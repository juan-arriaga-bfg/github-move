﻿using System;
using Newtonsoft.Json;

public class RandomSaveItemJsonConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof (RandomSaveItem);
	}
    
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		var targetValue = (RandomSaveItem) value;
        
		serializer.TypeNameHandling = TypeNameHandling.None;
		serializer.Serialize(writer, $"{targetValue.Uid},{targetValue.Seed},{targetValue.Count}");
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var data = serializer.Deserialize<string>(reader);
		var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
		var targetValue = new RandomSaveItem
		{
			Uid = dataArray[0],
			Seed = int.Parse(dataArray[1]),
			Count = int.Parse(dataArray[2]),
		};
        
		return targetValue;
	}
}

[JsonConverter(typeof(RandomSaveItemJsonConverter))]
public class RandomSaveItem
{
	private string uid;
	private int seed;
	private int _count;
	
	public string Uid
	{
		get { return uid; }
		set { uid = value; }
	}

	public int Seed
	{
		get { return seed; }
		set { seed = value; }
	}

	public int Count
	{
		get { return _count; }
		set { _count = value; }
	}
}