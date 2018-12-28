#if UNITY_EDITOR

using UnityEditor;

public static class ProjectBuilderAndroid
{
    private static void LocalBuild(IProjectBuildAction buildPlayerAction, 
                           ProjectBuilder.BuildPlatform platform,
                           ProjectBuilder.BuildPurpose purpose,
                           ProjectBuilder.BuildType buildType)
    {
        ProjectBuilder.Create()
                      .SetBuildType(buildType)
                      .SetBuildTargetPlatform(platform)
                      .SetBuildPurpose(purpose)
                       
                      .AddBuildAction(new BuildActionCleanupPreviousGradleExport())
                      .AddBuildAction(new BuildActionPrepare())
                      .AddBuildAction(new BuildActionEncryptConfigs())
                      .AddBuildAction(new BuildActionIncrementVersion())
                      .AddBuildAction(new BuildActionBuildAssetBundles())
                      .AddBuildAction(new BuildActionDisableScenes())

                      .AddBuildAction(buildPlayerAction)

                      .AddPostBuildAction(new BuildActionFixExportedGradleProjectHierarhy())
                      .AddPostBuildAction(new BuildActionCopyBfgSettingsJson())
                      .AddPostBuildAction(new BuildActionCopyGoogleServicesJson())
                      
                       // GDPR DEBUG// Uncomment to show sample gdpr policy
                      .AddPostBuildAction(new BuildActionCopySampleGdprPolicyJson())
                      .AddPostBuildAction(new BuildActionReplaceBfgLibWithDebugVersion())
                       // END GDPR DEBUG
                       
                      .AddPostBuildAction(new BuildActionInstallGradleWrapper())
                      .AddPostBuildAction(new BuildActionReset())
                       
                      .AddPostBuildAction(new BuildActionPrintToConsole().SetMessage("BUILD COMPLETE!"))
                       
                      .Execute(); 
    }
    
    [MenuItem("Build/Android/Qa")]
    public static void RunExportGradleQa()
    {
        var buildPlayerAction = new BuildActionGradleExport();
        LocalBuild(buildPlayerAction, ProjectBuilder.BuildPlatform.Android, ProjectBuilder.BuildPurpose.Qa, ProjectBuilder.BuildType.Development);
    }
    
    [MenuItem("Build/Android/Stage")]
    public static void RunExportGradleStage()
    {
        var buildPlayerAction = new BuildActionGradleExport();
        LocalBuild(buildPlayerAction, ProjectBuilder.BuildPlatform.Android, ProjectBuilder.BuildPurpose.Stage, ProjectBuilder.BuildType.Release);
    }
    
        
    [MenuItem("Build/Android/Prod")]
    public static void RunExportGradleProd()
    {
        var buildPlayerAction = new BuildActionGradleExport();
        LocalBuild(buildPlayerAction, ProjectBuilder.BuildPlatform.Android, ProjectBuilder.BuildPurpose.Prod, ProjectBuilder.BuildType.Release);
    }
}

#endif