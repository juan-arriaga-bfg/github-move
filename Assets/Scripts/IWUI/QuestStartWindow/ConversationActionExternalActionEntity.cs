public class ConversationActionExternalActionEntity : ConversationActionEntity
{    
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
}