using System.Collections.Generic;

public class ConversationScenarioEntity : ECSEntity, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
   
    private readonly List<ConversationActionEntity> actions = new List<ConversationActionEntity>();

    private int index;

    public override ECSEntity RegisterComponent(IECSComponent component, bool isCollection = false)
    {       
        ConversationActionEntity action = component as ConversationActionEntity;
        if (action != null)
        {
            actions.Add(action);
        }
        
        return base.RegisterComponent(component, isCollection);
    }

    public ConversationActionEntity GetNextAction()
    {
        if (actions == null || index >= actions.Count)
        {
            return null;
        }

        var ret = actions[index];
        index++;

        return ret;
    }
}