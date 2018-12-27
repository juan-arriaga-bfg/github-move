using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildActionGradleExport : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        string path = Application.dataPath.Replace("/Assets", "/GradleProject");

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
        }

        buildOptions = buildOptions | BuildOptions.AcceptExternalModificationsToPlayer;

        var scenes = EditorBuildSettings.scenes.ToList();
        string[] scenePaths = scenes.Select(e => e.path).ToArray();
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            locationPathName = path,
            scenes = scenePaths,
            target = context.CurrentBuildTarget,
            targetGroup = BuildTargetGroup.Android,
            options = buildOptions
        };

        BuildPipeline.BuildPlayer (buildPlayerOptions);
    }
}