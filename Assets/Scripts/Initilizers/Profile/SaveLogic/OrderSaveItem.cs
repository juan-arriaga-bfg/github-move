using System;
using Newtonsoft.Json;

public class OrderSaveItemJsonConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof (OrderSaveItem);
	}
    
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		var targetValue = (OrderSaveItem) value;
        
		serializer.TypeNameHandling = TypeNameHandling.None;
		serializer.Serialize(writer, $"{targetValue.Id},{targetValue.Customer},{(int) targetValue.State},{(targetValue.IsStart ? 1 : 0)},{(targetValue.IsStartCooldown ? 1 : 0)},{targetValue.StartTime},{targetValue.CooldownTime}");
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var data = serializer.Deserialize<string>(reader);
		var dataArray = data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        
		var targetValue = new OrderSaveItem
		{
			Id = dataArray[0],
			Customer = int.Parse(dataArray[1]),
			State = (OrderState) int.Parse(dataArray[2]),
			IsStart = int.Parse(dataArray[3]) == 1,
			IsStartCooldown = int.Parse(dataArray[4]) == 1,
			StartTime = long.Parse(dataArray[5]),
			CooldownTime = long.Parse(dataArray[6])
		};
        
		return targetValue;
	}
}

[JsonConverter(typeof(OrderSaveItemJsonConverter))]
public class OrderSaveItem
{
	private string id;
	
	private int customer;

	private OrderState state;
	
	private bool isStart;
	private bool isStartCooldown;
	
	private long startTime;
	private long cooldownTime;
	
	public string Id
	{
		get { return id; }
		set { id = value; }
	}
	
	public int Customer
	{
		get { return customer; }
		set { customer = value; }
	}
	
	public OrderState State
	{
		get { return state; }
		set { state = value; }
	}
	
	public bool IsStart
	{
		get { return isStart; }
		set { isStart = value; }
	}
	
	public bool IsStartCooldown
	{
		get { return isStartCooldown; }
		set { isStartCooldown = value; }
	}
	
	public long StartTime
	{
		get { return startTime; }
		set { startTime = value; }
	}
	
	public long CooldownTime
	{
		get { return cooldownTime; }
		set { cooldownTime = value; }
	}
}