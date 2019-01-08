#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

public class BuildActionCopySampleGdprPolicyJson : BuildActionCopyFilesBase
{
    protected override string SrcDir => Application.dataPath.Replace("/Assets", $"/Misc/BFG/{context.CurrentBuildPlatform.ToString()}/SamplePoliciesJson/");
    protected override string DstDir => context.BuildPath + "/bfgunityandroid/res/raw/";

    protected override List<string> Files => new List<string>
    {
        "sample_policies.json"
    };
}

#endif