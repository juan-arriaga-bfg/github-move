#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public interface IProjectBuildAction
{
    void Execute(ProjectBuilder context);
}

public class ProjectBuilder
{
    public enum BuildPurpose
    {
        Unknown,
        Qa,
        Stage,
        Prod
    }
    
    public enum BuildType
    {
        Unknown,
        Development,
        Release
    }
    
    private static ProjectBuilder actionsContext;

    private static List<IProjectBuildAction> buildActions = new List<IProjectBuildAction>();
    private static List<IProjectBuildAction> postBuildActions = new List<IProjectBuildAction>();

    public BuildPurpose CurrentBuildPurpose { get; private set; } = BuildPurpose.Unknown;
    public BuildType CurrentBuildType { get; private set; } = BuildType.Unknown;
    public BuildTarget CurrentBuildTarget { get; private set; } = BuildTarget.NoTarget;
    
    public static ProjectBuilder Create()
    {
        return new ProjectBuilder();
    }

    private ProjectBuilder()
    {
        Cleanup();
        actionsContext = this;
    }

    private static void Cleanup()
    {
        buildActions.Clear();
        postBuildActions.Clear();
        actionsContext = null;
    }

    public ProjectBuilder AddBuildAction(IProjectBuildAction action)
    {
        buildActions.Add(action);
        return this;
    }

    public ProjectBuilder AddPostBuildAction(IProjectBuildAction action)
    {
        postBuildActions.Add(action);
        return this;
    }
    
    public ProjectBuilder SetBuildPurpose(BuildPurpose purpose)
    {
        CurrentBuildPurpose = purpose;
        return this;
    }
    
        
    public ProjectBuilder SetBuildType(BuildType type)
    {
        CurrentBuildType = type;
        return this;
    }
 
    public ProjectBuilder SetBuildTargetPlatform(BuildTarget target)
    {
        CurrentBuildTarget = target;
        return this;
    }

    private static void ExecuteActions(List<IProjectBuildAction> actions)
    {
        foreach (var action in actions)
        {
            Debug.Log($"[ProjectBuilder] => EXECUTE ACTION '{action.GetType()}'...");
            
            try
            {
                action.Execute(actionsContext);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ProjectBuilder] => Action '{action.GetType()}' FAILED");

                Cleanup();
                throw;
            }
            
            Debug.Log($"[ProjectBuilder] => action '{action.GetType()}' executed");
        }
    }

    public ProjectBuilder Execute()
    {
        if (CurrentBuildPurpose == BuildPurpose.Unknown)
        {
            throw new Exception("[ProjectBuilder] => Execute: BuildPurpose is not specified");
        }
        
        if (CurrentBuildTarget == BuildTarget.NoTarget)
        {
            throw new Exception("[ProjectBuilder] => Execute: BuildTarget is not specified");
        }
        
        if (CurrentBuildType == BuildType.Unknown)
        {
            throw new Exception("[ProjectBuilder] => Execute: BuildType is not specified");
        }
        
        ExecuteActions(buildActions);
        return this;
    }

    [PostProcessBuild(10000)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        ExecuteActions(postBuildActions);
        Cleanup();
    }
}

#endif