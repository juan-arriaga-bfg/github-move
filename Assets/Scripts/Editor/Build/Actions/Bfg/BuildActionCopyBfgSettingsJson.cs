#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

public class BuildActionCopyBfgSettingsJson : BuildActionCopyFilesBase
{
    protected override string SrcDir => Application.dataPath.Replace("/Assets", $"/Misc/BFG/{context.CurrentBuildPlatform.ToString()}/SettingsJson/{context.CurrentBuildType.ToString()}/");
    protected override string DstDir => context.BuildPath + "/bfgunityandroid/res/raw/";

    protected override List<string> Files => new List<string>
    {
        "bfg_first_launch_settings.json",
        "bfg_upgrade_settings.json"
    };
}

#endif