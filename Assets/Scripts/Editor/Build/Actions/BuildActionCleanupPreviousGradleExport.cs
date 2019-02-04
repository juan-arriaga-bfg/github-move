using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR

public class BuildActionCleanupPreviousGradleExport : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        string dir1 = context.BuildPath;
        string dir2 = Application.dataPath.Replace("/Assets", "/TEMP_DIR");

        List<string> dirs = new List<string>
        {
            dir1,
            dir2
        };

        foreach (var dir in dirs)
        {
            if (Directory.Exists(dir))
            {
                try
                {
                    ProjectBuildFileUtils.DeleteDirectory(dir);
                }
                catch (Exception e)
                {
                    throw new Exception($"[BuildActionCleanupPreviousGradleExport] => Execute: Can't remove previous build artifact: '{dir}' with exception: {e.Message}. Ensure that dir is not locked by Unity, Android Studio, or command line window.");
                } 
            }
        }
    }
}

#endif