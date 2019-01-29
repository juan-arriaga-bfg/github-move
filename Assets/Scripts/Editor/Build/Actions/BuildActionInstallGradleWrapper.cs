#if UNITY_EDITOR

using System;
using System.IO;
using UnityEngine;

public class BuildActionInstallGradleWrapper : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        string dst = context.BuildPath;
        
#if UNITY_EDITOR_WIN
        string src = Application.dataPath.Replace("/Assets", "/Misc/BFG/Android/GradleWrapper/Win");
#endif
        
#if UNITY_EDITOR_OSX
        string src = Application.dataPath.Replace("/Assets", "/Misc/BFG/Android/GradleWrapper/OSX");
#endif

        if (!Directory.Exists(src))
        {
            throw new Exception($"Can't found gradle wrapper files at {src}");
        }
        
        ProjectBuildFileUtils.CopyDirectory(src, dst, false);
    }
}

#endif