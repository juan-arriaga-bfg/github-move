using Newtonsoft.Json;

/// <summary>
/// Always returns True on check
/// </summary>
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