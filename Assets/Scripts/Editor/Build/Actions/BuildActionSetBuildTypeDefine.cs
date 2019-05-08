#if UNITY_EDITOR

using System;
using System.Collections.Generic;

public class BuildActionSetBuildTypeDefine : BuildActionModifyDefines
{
    public static string BUILD_QA    = "BUILD_QA";
    public static string BUILD_PROD  = "BUILD_PROD";
    public static string BUILD_STAGE = "BUILD_STAGE";
    
    public override void Execute(ProjectBuilder context)
    {
        switch (context.CurrentBuildPurpose)
        {
            case ProjectBuilder.BuildPurpose.Qa:
                definesToRemove = new HashSet<string> {BUILD_QA};
                break;
            
            case ProjectBuilder.BuildPurpose.Stage:
                definesToRemove = new HashSet<string> {BUILD_STAGE};
                break;
            
            case ProjectBuilder.BuildPurpose.Prod:
                definesToRemove = new HashSet<string> {BUILD_PROD};
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        base.Execute(context);
    }
}

#endif