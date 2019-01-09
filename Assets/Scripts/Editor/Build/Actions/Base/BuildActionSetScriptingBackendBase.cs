#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public abstract class BuildActionSetScriptingBackendBase : IProjectBuildAction
{
    protected abstract ScriptingImplementation ScriptingImplementation { get; }
    
    public void Execute(ProjectBuilder context)
    {
        Debug.Log($"[BuildActionSetScriptingBackend] => Backend set to: {ScriptingImplementation}");
        
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