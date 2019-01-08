#if UNITY_EDITOR

using UnityEditor;

public abstract class BuildActionSetScriptingBackendBase : IProjectBuildAction
{
    public abstract ScriptingImplementation ScriptingImplementation { get; }
    
    public void Execute(ProjectBuilder context)
    {
        BuildTarget target = BuildTarget.NoTarget;
        
        switch (context.CurrentBuildPlatform)
        {
            case ProjectBuilder.BuildPlatform.Android:
            case ProjectBuilder.BuildPlatform.Amazon:
                target = BuildTarget.Android;
                break;
            case ProjectBuilder.BuildPlatform.Ios:
                target = BuildTarget.iOS;
                break;
        }
        
        PlayerSettings.SetScriptingBackend(BuildPipeline.GetBuildTargetGroup(target), ScriptingImplementation);
    }
}

#endif