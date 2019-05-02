using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class TutorialSaveComponent : ECSEntity, IECSSerializeable
{
	public static int ComponentGuid = ECSManager.GetNextGuid();
	public override int Guid => ComponentGuid;
	
    [JsonProperty] public HashSet<int> Complete;
    [JsonProperty] public HashSet<int> Started;
	
	[OnSerializing]
	internal void OnSerialization(StreamingContext context)
	{
		if(BoardService.Current == null || Complete != null) return;

        Complete = GameDataService.Current.TutorialDataManager.Completed;
        Started  = GameDataService.Current.TutorialDataManager.Started;
	}
    
	[OnDeserialized]
	internal void OnDeserialized(StreamingContext context)
	{
	}
}
