using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

public class QuestManager : ECSEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private TaskEntity task;
    
    public void Init()
    {
        task = new TaskEntity {Id = "SomeTaskId"};
        task.RegisterComponent(new QuestDescriptionComponent());
        task.RegisterComponent(new QuestRewardComponent());
                
        SubtaskMatchCounterEntity subtaskMatch = new SubtaskMatchCounterEntity {Id = "MatchSubtaskId"};
        subtaskMatch.RegisterComponent(new QuestDescriptionComponent());
                
        SubtaskCreatePieceCounterEntity subtaskBuild = new SubtaskCreatePieceCounterEntity {Id = "CreateSubtaskId"};
        subtaskBuild.RegisterComponent(new QuestDescriptionComponent());

        task.RegisterComponent(subtaskMatch);
        task.RegisterComponent(subtaskBuild);
    }

    public void ConnectToBoard()
    {
        foreach (var cacheItem in task.ComponentsCache)
        {
            var cmp = cacheItem.Value;
            (cmp as IConnectedToBoardEvent)?.ConnectToBoard();
        }
        
        JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
        };
        
        var text = JsonConvert.SerializeObject(task, serializerSettings);
        // Debug.LogWarning(text);
        //
        var item = JsonConvert.DeserializeObject<ECSEntity>(text, serializerSettings);
        int i    = 0;
    }
}