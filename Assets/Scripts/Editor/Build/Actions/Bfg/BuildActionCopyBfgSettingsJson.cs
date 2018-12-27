#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public class BuildActionCopyBfgSettingsJson : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        string srcDir = Application.dataPath.Replace("/Assets", $"/Misc/BFG/{context.CurrentBuildPlatform.ToString()}/SettingsJson/{context.CurrentBuildType.ToString()}/");
        string dstDir = Application.dataPath.Replace("/Assets", "/GradleProject/bfgunityandroid/res/raw/");

        var files = new List<string>
        {
            "bfg_first_launch_settings.json",
            "bfg_upgrade_settings.json"
        };

        foreach (var file in files)
        {
            try
            {
                string text = File.ReadAllText(srcDir + file);
                JSONNode node = JSONNode.Parse(text);
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