using Newtonsoft.Json;

/// <summary>
/// Always returns false on check
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public sealed class QuestStartConditionAlwaysFalseComponent : QuestStartConditionComponent
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    public override bool Check()
    {
        return false;
    }
}