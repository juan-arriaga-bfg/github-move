#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuildActionReplaceBfgLibWithDebugVersion : IProjectBuildAction
{
    public const string OPTION_ENABLE = "BUILD_OPTION_COPY_DEBUG_BFG_LIB";
    
    public virtual void Execute(ProjectBuilder context)
    {
        if (!context.CustomOptions.ContainsKey(OPTION_ENABLE))
        {
            return;
        }
        
        if (context.CurrentBuildPurpose == ProjectBuilder.BuildPurpose.Prod)
        {
            throw new Exception("Not allowed for PROD builds!");
        }
        
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