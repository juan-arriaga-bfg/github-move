#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ProjectBuilderAndroid
{
    private static void LocalBuild(IProjectBuildAction buildPlayerAction, 
                           ProjectBuilder.BuildPlatform platform,
                           ProjectBuilder.BuildPurpose purpose,
                           ProjectBuilder.BuildType buildType, Dictionary<string, string> customOptions = null)
    {
        string path = Application.dataPath.Replace("/Assets", "/GradleProject");

        ProjectBuilder.Create()
            .SetBuildType(buildType)
            .SetBuildTargetPlatform(platform)
            .SetBuildPurpose(purpose)
            .SetBuildPath(path)
            .SetCustomOptions(customOptions)
             
            .AddBuildAction(new BuildActionCleanupPreviousGradleExport())
            .AddBuildAction(new BuildActionPrepare())
            .AddBuildAction(new BuildActionEncryptConfigs())
            .AddBuildAction(new BuildActionIncrementVersion())
            .AddBuildAction(new BuildActionBuildAssetBundles())
            .AddBuildAction(new BuildActionDisableScenes())
            .AddBuildAction(new BuildActionSetScriptingBackendMono2x())
             
            .AddBuildAction(buildPlayerAction)
             
            .AddPostBuildAction(new BuildActionFixExportedGradleProjectHierarhy())
            .AddPostBuildAction(new BuildActionCopyBfgSettingsJson())
            .AddPostBuildAction(new BuildActionCopyGoogleServicesJson())

            .AddPostBuildAction(new BuildActionCopySampleGdprPolicyJson())      // optional
            .AddPostBuildAction(new BuildActionReplaceBfgLibWithDebugVersion()) // optional
                      
            .AddPostBuildAction(new BuildActionInstallGradleWrapper())
            .AddPostBuildAction(new BuildActionReset())
             
            .AddPostBuildAction(new BuildActionPrintToConsole().SetMessage("BUILD COMPLETE!"))
             
            .Execute(); 
    }
    
    [MenuItem("Build/Android/Qa", false, 1000)]
    public static void RunExportGradleQa()
    {
        var buildPlayerAction = new BuildActionGradleExport();
        LocalBuild(buildPlayerAction, ProjectBuilder.BuildPlatform.Android, ProjectBuilder.BuildPurpose.Qa, ProjectBuilder.BuildType.Development);
    }
    
    [MenuItem("Build/Android/Stage", false, 1000)]
    public static void RunExportGradleStage()
    {
        var buildPlayerAction = new BuildActionGradleExport();
        LocalBuild(buildPlayerAction, ProjectBuilder.BuildPlatform.Android, ProjectBuilder.BuildPurpose.Stage, ProjectBuilder.BuildType.Release);
    }
    
        
    [MenuItem("Build/Android/Prod", false, 1000)]
    public static void RunExportGradleProd()
    {
        var buildPlayerAction = new BuildActionGradleExport();
        LocalBuild(buildPlayerAction, ProjectBuilder.BuildPlatform.Android, ProjectBuilder.BuildPurpose.Prod, ProjectBuilder.BuildType.Release);
    }

    // ReSharper disable once InconsistentNaming
    [MenuItem("Build/Android/Custom/GDPR Test", false, 2000)]
    public static void RunExportGradleGDPRTest()
    {
        var customOptions = new Dictionary<string, string>
        {
            {BuildActionCopySampleGdprPolicyJson.OPTION_ENABLE, null},
            {BuildActionReplaceBfgLibWithDebugVersion.OPTION_ENABLE, null},
        };
        
        var buildPlayerAction = new BuildActionGradleExport();
        LocalBuild(buildPlayerAction, ProjectBuilder.BuildPlatform.Android, ProjectBuilder.BuildPurpose.Qa, ProjectBuilder.BuildType.Development, customOptions);
    }
}

#endif