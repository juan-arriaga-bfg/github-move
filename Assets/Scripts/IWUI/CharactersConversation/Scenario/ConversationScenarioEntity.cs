using System.Collections.Generic;

public class ConversationScenarioEntity : ECSEntity, IECSSerializeable
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    public ConversationScenarioEntity PreviousScenario { get; set; }
    
    private readonly List<ConversationActionEntity> actions = new List<ConversationActionEntity>();

    public List<ConversationActionEntity> Actions => actions; 
    
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

    public ConversationActionEntity GetFirstAction()
    {
        return actions[0];
    }

    public ConversationActionBubbleEntity GetLastBubbleAction()
    {
        for (var i = actions.Count - 1; i >= 0; i--)
        {
            var action = actions[i];
            if (action is ConversationActionBubbleEntity bubbleAction)
            {
                return bubbleAction;
            }
        }

        return null;
    }
}