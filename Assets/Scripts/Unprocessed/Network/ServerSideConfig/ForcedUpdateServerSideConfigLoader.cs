using IW.SimpleJSON;

// {"min_version":"0","last_version":"0","soft_message":"","forced_message":""}
public class ForcedUpdateServerConfig
{
    public string ForceVersion;  // min_version  - if lower, force market
    public string NotifyVersion; // last_version - if lower, show suggestion
}

public class ForcedUpdateServerSideConfigLoader : ServerSideConfigLoaderBase
{
    public static readonly int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;
    
    protected override object ParseResponse(JSONNode data)
    {
        ForcedUpdateServerConfig ret = new ForcedUpdateServerConfig
        {
            NotifyVersion = data["last_version"], 
            ForceVersion = data["min_version"]
        };

        return ret;
    }
}