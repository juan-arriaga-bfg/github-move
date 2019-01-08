#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

public class BuildActionCopyGoogleServicesJson : BuildActionCopyFilesBase
{
    protected override string SrcDir => Application.dataPath.Replace("/Assets", $"/Misc/BFG/{context.CurrentBuildPlatform.ToString()}/GoogleServicesJson/");
    protected override string DstDir => context.BuildPath + "/";

    protected override List<string> Files => new List<string>
    {
        "google-services.json"
    };
}

#endif