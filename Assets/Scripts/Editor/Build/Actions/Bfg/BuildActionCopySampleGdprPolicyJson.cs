#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

public class BuildActionCopySampleGdprPolicyJson : BuildActionCopyFilesBase
{
    public const string OPTION_ENABLE = "BUILD_OPTION_COPY_SAMPLE_GDPR_POLICY";
    
    protected override string SrcDir => Application.dataPath.Replace("/Assets", $"/Misc/BFG/{context.CurrentBuildPlatform.ToString()}/SamplePoliciesJson/");
    protected override string DstDir => context.BuildPath + "/bfgunityandroid/res/raw/";

    protected override List<string> Files => new List<string>
    {
        "sample_policies.json"
    };

    public override void Execute(ProjectBuilder context)
    {
        if (context.CustomOptions.ContainsKey(OPTION_ENABLE))
        {
            base.Execute(context);
        } 
    }
}

#endif