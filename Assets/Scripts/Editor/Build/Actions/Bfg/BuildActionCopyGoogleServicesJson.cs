#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public class BuildActionCopyGoogleServicesJson : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        string srcDir = Application.dataPath.Replace("/Assets", $"/Misc/BFG/{context.CurrentBuildPlatform.ToString()}/GoogleServicesJson/");
        string dstDir = Application.dataPath.Replace("/Assets", "/GradleProject/bfgunityandroid/");

        var files = new List<string>
        {
            "google-services.json"
        };

        foreach (var file in files)
        {
            try
            {
                JSONNode node = JSONNode.Parse(file);
            }
            catch (Exception e)
            {
                throw new Exception($"Can't parse json '{file}' with exception {e.Message}");
            }

            File.Copy(srcDir + file, dstDir + file, true);
        }
    }
}

#endif