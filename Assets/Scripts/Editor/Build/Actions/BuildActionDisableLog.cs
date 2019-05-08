#if UNITY_EDITOR
using System.Collections.Generic;
using IW;

public class BuildActionDisableLogger : BuildActionModifyDefines
{ 
    public override void Execute(ProjectBuilder context)
    {
        definesToRemove = new HashSet<string> {Logger.ENABLE_DEFINE};
        
        base.Execute(context);
    }
}

#endif