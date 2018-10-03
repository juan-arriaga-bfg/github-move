using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class TaskDummyEntity : TaskEntity
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    protected override bool Check()
    {
        return false;
    }

    public override string ToString()
    {
        string ret = $"{GetType()} [{Id}], State: {State}, Progress: N/A";
        return ret;
    }
}