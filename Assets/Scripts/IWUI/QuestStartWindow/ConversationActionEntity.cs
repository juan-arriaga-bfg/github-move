public abstract class ConversationActionEntity : ECSEntity, IECSSerializeable
{    
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
}