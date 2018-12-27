using System;
using System.IO;
using System.Threading;
using UnityEngine;

#if UNITY_EDITOR

public static class ProjectBuildFileUtils
{
    public static void DeleteDirectory(string dir)
    {
        Debug.LogFormat("OptimizedBuildTool: DeleteDirectory: [{0}]", dir);

        // Made two attempts, sometimes helps to handle exceptions on windows system when target directory is opened by Explorer or other tool
        for (int i = 0; i < 2; i++)
        {
            try
            {
                var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    File.Delete(file);
                }
                Directory.Delete(dir, true);
                
                break;
            }
            catch (Exception e)
            {
                Debug.LogFormat("OptimizedBuildTool: DeleteDirectory: Attempt failed with: {0}", e.Message);
                Thread.Sleep(1000);
            }
        }
    }
}

#endif