#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class ProjectBuilderAndroid
{
    private static readonly string LOCAL_BUILD_PATH = Application.dataPath.Replace("/Assets", "/GradleProject");
    
    private static void Build( 
                           ProjectBuilder.BuildPlatform platform,
                           ProjectBuilder.BuildPurpose purpose,
                           ProjectBuilder.BuildType buildType, 
                           string exportPath,
                           bool useIL2CPP = false,
                           Dictionary<string, string> customOptions = null)
    {
        ProjectBuilder.Create()
            .SetBuildType(buildType)
            .SetBuildTargetPlatform(platform)
            .SetBuildPurpose(purpose)
            .SetBuildPath(exportPath)
            .SetCustomOptions(customOptions)
             
            .AddBuildAction(new BuildActionCleanupPreviousGradleExport())
            .AddBuildAction(new BuildActionPrepare())
            .AddBuildAction(new BuildActionEncryptConfigs())
            .AddBuildAction(new BuildActionIncrementVersion())
            .AddBuildAction(new BuildActionBuildAssetBundles())
            .AddBuildAction(new BuildActionDisableScenes())
            .AddBuildAction(new BuildActionSetScriptingBackend()
                //.SetType(useIL2CPP ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x)
                )
             
            .AddBuildAction(new BuildActionGradleExport())
             
            .AddPostBuildAction(new BuildActionFixExportedGradleProjectHierarhy())
            .AddPostBuildAction(new BuildActionCopyBfgSettingsJson())
            .AddPostBuildAction(new BuildActionCopyGoogleServicesJson())

            .AddPostBuildAction(new BuildActionCopySampleGdprPolicyJson())      // optional
            .AddPostBuildAction(new BuildActionReplaceBfgLibWithDebugVersion()) // optional
                      
            .AddPostBuildAction(new BuildActionInstallGradleWrapper())
            .AddPostBuildAction(new BuildActionReset())
             
            .AddPostBuildAction(new BuildActionPrintToConsole()
                .SetMessage("BUILD COMPLETE!"))
             
            .Execute(); 
    }
    
    [UsedImplicitly]
    public static void AutoBuild()
    {
        var customOptions = new Dictionary<string, string>();

        List<string> parameters = ProjectBuilder.GetCommandLineParameters();

        string path = parameters.First(e => e.StartsWith("exportpath")).Split(';')[1];
        ProjectBuilder.BuildType buildType = parameters.Contains("dev") ? ProjectBuilder.BuildType.Development : ProjectBuilder.BuildType.Release;
        ProjectBuilder.BuildPlatform platform = ProjectBuilder.BuildPlatform.Android;
        ProjectBuilder.BuildPurpose purpose = ProjectBuilder.BuildPurpose.Qa;

        bool useIL2CPP = parameters.Contains("cpp");
        
        Build(platform, purpose, buildType, path, useIL2CPP, customOptions);

    }
    
#region Unity Menu -> Android build
    
    [MenuItem("Build/Android/Qa", false, 1000)]
    public static void RunExportGradleQa()
    {
        Build(ProjectBuilder.BuildPlatform.Android, ProjectBuilder.BuildPurpose.Qa, ProjectBuilder.BuildType.Development, LOCAL_BUILD_PATH);
    }
    
    [MenuItem("Build/Android/Stage", false, 1000)]
    public static void RunExportGradleStage()
    {
        Build(ProjectBuilder.BuildPlatform.Android, ProjectBuilder.BuildPurpose.Stage, ProjectBuilder.BuildType.Release, LOCAL_BUILD_PATH);
    }
    
        
    [MenuItem("Build/Android/Prod", false, 1000)]
    public static void RunExportGradleProd()
    {
        Build(ProjectBuilder.BuildPlatform.Android, ProjectBuilder.BuildPurpose.Prod, ProjectBuilder.BuildType.Release, LOCAL_BUILD_PATH);
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
        
        Build(ProjectBuilder.BuildPlatform.Android, ProjectBuilder.BuildPurpose.Qa, ProjectBuilder.BuildType.Development, LOCAL_BUILD_PATH, false, customOptions);
    }
    
#endregion
    
}

#endif