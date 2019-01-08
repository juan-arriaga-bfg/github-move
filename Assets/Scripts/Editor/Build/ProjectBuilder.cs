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
    
    public enum BuildPlatform
    {
        Unknown,
        Android,
        Amazon,
        Ios
    }
    
    private static ProjectBuilder actionsContext;

    private static List<IProjectBuildAction> buildActions = new List<IProjectBuildAction>();
    private static List<IProjectBuildAction> postBuildActions = new List<IProjectBuildAction>();

    public BuildPurpose CurrentBuildPurpose { get; private set; } = BuildPurpose.Unknown;
    public BuildType CurrentBuildType { get; private set; } = BuildType.Unknown;
    public BuildPlatform CurrentBuildPlatform { get; private set; } = BuildPlatform.Unknown;
    
    public Dictionary<string, string> CustomOptions { get; private set; } = new Dictionary<string, string>();
    
    public string BuildPath { get; private set; } = "BuildPath";
    
    public string BuildName { get; private set; } = "BuildName";
    
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
 
    public ProjectBuilder SetBuildTargetPlatform(BuildPlatform platform)
    {
        CurrentBuildPlatform = platform;
        return this;
    }
    
    public ProjectBuilder SetBuildPath(string path)
    {
        BuildPath = path;
        return this;
    }
    
    public ProjectBuilder SetBuildName(string name)
    {
        BuildName = name;
        return this;
    }
    
    public ProjectBuilder SetCustomOptions(Dictionary<string, string> options)
    {
        CustomOptions = options ?? CustomOptions;
        return this;
    }

    private static void ExecuteActions(List<IProjectBuildAction> actions)
    {
        for (var i = 0; i < actions.Count; i++)
        {
            var action = actions[i];
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

            //Debug.Log($"[ProjectBuilder] => action '{action.GetType()}' executed");
        }
    }

    public ProjectBuilder Execute()
    {
        ClearConsole();
        
        Debug.Log($"[ProjectBuilder] => Start!");
        
        if (CurrentBuildPurpose == BuildPurpose.Unknown)
        {
            throw new Exception("[ProjectBuilder] => Execute: BuildPurpose is not specified");
        }
        
        if (CurrentBuildPlatform == BuildPlatform.Unknown)
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

    private static void ClearConsole()
    {
        try
        {
            var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
 
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
 
            clearMethod.Invoke(null, null);
        }
        catch (Exception e)
        {
            //
        }
    }
    
    [PostProcessBuild(10000)]
    public static void OnPostprocessBuild(UnityEditor.BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log($"[ProjectBuilder] => OnPostprocessBuild");
        
        ExecuteActions(postBuildActions);
    }
}

#endif