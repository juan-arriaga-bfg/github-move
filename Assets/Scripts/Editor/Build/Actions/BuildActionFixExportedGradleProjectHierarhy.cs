using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR

public class BuildActionFixExportedGradleProjectHierarhy : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        string dst = Application.dataPath.Replace("/Assets", "/GradleProject");
        string src = Path.Combine(dst, Application.productName);
        string tmp = Application.dataPath.Replace("/Assets", "/TEMP_DIR");
        
        if (!Directory.Exists(src))
        {
            throw new Exception($"[BuildActionFixExportedGradleProjectHierarhy] => Execute: Directory NOT exist: '{src}'. Skip step!");
        }

        if (Directory.Exists(tmp))
        {
            ProjectBuildFileUtils.DeleteDirectory(tmp);
        }
        
        Directory.Move(src, tmp);
        
        if (Directory.Exists(dst))
        {
            ProjectBuildFileUtils.DeleteDirectory(dst);
        }

        Directory.Move(tmp, dst);
    }
}

#endif