using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class TutorialSaveComponent : ECSEntity, IECSSerializeable
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;
	
	private List<int> complete;
	
	[JsonProperty]
	public List<int> Complete
	{
		get { return complete; }
		set { complete = value; }
	}
	
	[OnSerializing]
	internal void OnSerialization(StreamingContext context)
	{
		if(BoardService.Current == null || complete != null) return;

		complete = BoardService.Current.FirstBoard.TutorialLogic.Save;
	}
    
	[OnDeserialized]
	internal void OnDeserialized(StreamingContext context)
	{
	}
}
