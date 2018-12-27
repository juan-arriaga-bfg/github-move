using UnityEditor;

public static class ProjectBuilderAndroid
{
    public static void Run(IProjectBuildAction buildPlayerAction, 
                           BuildTarget target,
                           ProjectBuilder.BuildPurpose purpose,
                           ProjectBuilder.BuildType buildType)
    {
        ProjectBuilder.Create()
                      .SetBuildType(buildType)
                      .SetBuildTargetPlatform(target)
                      .SetBuildPurpose(purpose)
                       
                      .AddBuildAction(new BuildActionPrepare())
                      .AddBuildAction(new BuildActionEncryptConfigs())
                      .AddBuildAction(new BuildActionIncrementVersion())
                      .AddBuildAction(new BuildActionBuildAssetBundles())
                      .AddBuildAction(new BuildActionDisableScenes())
                       
                      .AddBuildAction(buildPlayerAction)
                       
                      .AddPostBuildAction(new BuildActionReset())
                       
                      .Execute(); 
    }
    
    [MenuItem("Build/Android/Qa")]
    public static void RunExportGradleQa()
    {
        var buildPlayerAction = new BuildActionGradleExport();
        Run(buildPlayerAction, BuildTarget.Android, ProjectBuilder.BuildPurpose.Qa, ProjectBuilder.BuildType.Development);
    }
    
    [MenuItem("Build/Android/Stage")]
    public static void RunExportGradleStage()
    {
        var buildPlayerAction = new BuildActionGradleExport();
        Run(buildPlayerAction, BuildTarget.Android, ProjectBuilder.BuildPurpose.Stage, ProjectBuilder.BuildType.Release);
    }
    
        
    [MenuItem("Build/Android/Prod")]
    public static void RunExportGradleProd()
    {
        var buildPlayerAction = new BuildActionGradleExport();
        Run(buildPlayerAction, BuildTarget.Android, ProjectBuilder.BuildPurpose.Prod, ProjectBuilder.BuildType.Release);
    }
}