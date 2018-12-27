#if UNITY_EDITOR

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public static class ProjectBuildFileUtils
{
    public static void DeleteDirectory(string dir)
    {
        Debug.LogFormat("[ProjectBuildFileUtils] => DeleteDirectory: [{0}]", dir);

        // Made two attempts, sometimes helps to handle exceptions on windows system when target directory is opened by Explorer or other tool
        const int ATTEMPTS = 5;
        for (int i = 1; i <= ATTEMPTS; i++)
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
                Debug.LogError($"[ProjectBuildFileUtils] => DeleteDirectory: Attempt {i}/{ATTEMPTS} failed with: {e.Message}");
                Thread.Sleep(1000);
            }
        }
    }
    
    /// <summary>
    /// Copies and replace directory utility method.
    /// </summary>
    /// <param name="srcPath">Source path.</param>
    /// <param name="dstPath">Dst path.</param>
    /// <param name="replaceDstDir">If TRUE dst dir will be deleted before copyng</param>
    /// <param name="excludeRegex">Matched files wil be ignored</param>
    public static void CopyDirectory(string srcPath, string dstPath, bool replaceDstDir, string excludeRegex = null)
    {
        Debug.LogFormat("[ProjectBuildFileUtils] => CopyDirectory: [{0}] at {1} (exclude: {2})", srcPath, dstPath, excludeRegex );

        if (replaceDstDir)
        {
            if (Directory.Exists(dstPath))
            {
                DeleteDirectory(dstPath);
            }
        }

        if (File.Exists(dstPath))
        {
            File.Delete(dstPath);
        }

        if (!Directory.Exists(dstPath))
        {
            Directory.CreateDirectory(dstPath);
        }

        foreach (var file in Directory.GetFiles(srcPath))
        {
            if (excludeRegex != null)
            {
                Regex r = new Regex(excludeRegex);
                if (r.Match(file).Success)
                {
                    continue;
                }
            }

            var targetPath = Path.Combine(dstPath, Path.GetFileName(file));
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            
            File.Copy(file, targetPath);
        }

        foreach (var dir in Directory.GetDirectories(srcPath))
        {
            CopyDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)), replaceDstDir, excludeRegex);
        }
    }
}

#endif