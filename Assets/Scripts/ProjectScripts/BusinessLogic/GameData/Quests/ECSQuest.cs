

// public class SubtaskCurrencyCounterEntity : SubtaskCounterEntity
// {
//     public override int Guid => ComponentGuid;
//
//     public void OnRegisterEntity(ECSEntity entity)
//     {
//          throw new NotImplementedException();
//     }
//
//     public void OnUnRegisterEntity(ECSEntity entity)
//     {
//         
//     }
// }

using System.Collections.Generic;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;

public class asdf
{
    
}

[JsonObject(MemberSerialization.OptIn)]
public abstract class QuestStartConditionComponent : IECSComponent, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public int Guid => ComponentGuid;
    
    public void OnRegisterEntity(ECSEntity entity)
    {

    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {

    }

    public abstract bool Check();
}

[JsonObject(MemberSerialization.OptIn)]
public class QuestStarterEntity : ECSEntity, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    
    public override int Guid => ComponentGuid;

    private List<QuestStartConditionComponent> conditions = new List<QuestStartConditionComponent>();
    
    [JsonProperty] public string TaskToStartId { get; protected set; }
    
    public override ECSEntity RegisterComponent(IECSComponent component, bool isCollection = false)
    {
        var conditionComponent = component as QuestStartConditionComponent;
        if (conditionComponent != null)
        {
            conditions.Add(conditionComponent);
        }
        
        return base.RegisterComponent(component, isCollection);
    }
    
    public void Check()
    {
        foreach (var condition in conditions)
        {
            if (!condition.Check())
            {
                return;
            }
        }
        
        StartQuest();
    }

    public void StartQuest()
    {
        
    }
}