#if UNITY_EDITOR

using System;
using System.Collections.Generic;

public class BuildActionSetBuildPurposeDefine : BuildActionModifyDefines
{
    public const string BUILD_QA    = "BUILD_QA";
    public const string BUILD_PROD  = "BUILD_PROD";
    public const string BUILD_STAGE = "BUILD_STAGE";
    
    public override void Execute(ProjectBuilder context)
    {
        switch (context.CurrentBuildPurpose)
        {
            case ProjectBuilder.BuildPurpose.Qa:
                definesToAdd = new HashSet<string> {BUILD_QA};
                break;
            
            case ProjectBuilder.BuildPurpose.Stage:
                definesToAdd = new HashSet<string> {BUILD_STAGE};
                break;
            
            case ProjectBuilder.BuildPurpose.Prod:
                definesToAdd = new HashSet<string> {BUILD_PROD};
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        base.Execute(context);
    }
}

#endif