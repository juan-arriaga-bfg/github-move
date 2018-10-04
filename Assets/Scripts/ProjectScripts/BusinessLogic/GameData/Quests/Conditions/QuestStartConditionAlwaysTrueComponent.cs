using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public sealed class QuestStartConditionAlwaysTrueComponent : QuestStartConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public override bool Check()
    {
        return true;
    }
}