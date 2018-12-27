#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildActionCreateApk : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        string path = Application.dataPath.Replace("/Assets", "/Builds");
        string buildName = PlayerSettings.productName.ToLower() + "_"
                                                                + EditorUserBuildSettings.activeBuildTarget.ToString().ToLower() + "_"
                                                                + IWProjectVersionSettings.Instance.CurrentVersion.ToString().ToLower();

        buildName = buildName.Replace(".", "_").Replace(" ", "_");
        buildName = buildName + ".apk";

        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        IWProjectVerisonsEditor.GetCurrentVersion();

        BuildOptions options = BuildOptions.None;
        //		 options = options | BuildOptions.Development | BuildOptions.AutoRunPlayer;
        options = options | BuildOptions.AutoRunPlayer;

        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path + "/" + buildName, EditorUserBuildSettings.activeBuildTarget, options);
    }
}

#endif