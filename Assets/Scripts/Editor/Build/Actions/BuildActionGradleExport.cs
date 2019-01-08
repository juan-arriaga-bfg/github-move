#if UNITY_EDITOR

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildActionGradleExport : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        string path = context.BuildPath;

        if (Directory.Exists(path))
        {
            ProjectBuildFileUtils.DeleteDirectory(path);
        }
        
        Directory.CreateDirectory(path);

        IWProjectVerisonsEditor.GetCurrentVersion();

        BuildOptions buildOptions = BuildOptions.None;

        if (context.CurrentBuildType == ProjectBuilder.BuildType.Development)
        {
            buildOptions = buildOptions | BuildOptions.Development;
            buildOptions = buildOptions | BuildOptions.AllowDebugging;
        }

        buildOptions = buildOptions | BuildOptions.AcceptExternalModificationsToPlayer;

        var scenes = EditorBuildSettings.scenes.ToList();
        string[] scenePaths = scenes.Select(e => e.path).ToArray();

        var buildTarget = context.CurrentBuildPlatform == ProjectBuilder.BuildPlatform.Amazon || context.CurrentBuildPlatform == ProjectBuilder.BuildPlatform.Android ? BuildTarget.Android : BuildTarget.iOS;
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            locationPathName = path,
            scenes = scenePaths,
            target = buildTarget,
            targetGroup = BuildTargetGroup.Android,
            options = buildOptions
        };

        BuildPipeline.BuildPlayer (buildPlayerOptions);
    }
}

#endif