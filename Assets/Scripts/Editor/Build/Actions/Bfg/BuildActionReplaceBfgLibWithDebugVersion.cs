#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuildActionReplaceBfgLibWithDebugVersion : IProjectBuildAction
{
    public virtual void Execute(ProjectBuilder context)
    {
        string srcFile = Application.dataPath.Replace("/Assets", $"/Misc/BFG/{context.CurrentBuildPlatform.ToString()}/BfgLibDebug/bfgLib-debug.aar");

        List<string> targets = new List<string>
        {
            context.BuildPath + "/bfgunityandroid/libs/bfgLib-release.aar",
            context.BuildPath + "/libs/bfgLib-release.aar"
        };
        
        foreach (var target in targets)
        {
            File.Copy(srcFile, target, true);
        }
    }
}

#endif