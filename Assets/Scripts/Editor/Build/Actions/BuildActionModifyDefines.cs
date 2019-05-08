using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class BuildActionModifyDefines : IProjectBuildAction
{
    public virtual HashSet<string> definesToAdd { get; protected set; }
    public virtual HashSet<string> definesToRemove { get; protected set; }

    public void Execute(ProjectBuilder context)
    {
        // Lets collect all defines
        HashSet<string> resultDefines = new HashSet<string>();

        BuildTargetGroup buildTarget;
        switch (context.CurrentBuildPlatform)
        {
            case ProjectBuilder.BuildPlatform.Android:
                buildTarget = BuildTargetGroup.Android;
                break;
            
            case ProjectBuilder.BuildPlatform.Ios:
                buildTarget = BuildTargetGroup.iOS;
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        var toAdd = definesToAdd ?? new HashSet<string>();
        var toRemove = definesToRemove ?? new HashSet<string>();
        
        // Defines from platform settings
        List<string> existingDefines = GetDefinesFromPlatformSettings(buildTarget);
        foreach (var define in existingDefines)
        {
            if (toRemove.Contains(define) || toAdd.Contains(define))
            {
                continue;
            }
            
            resultDefines.Add(define); 
        }
        
        foreach (var define in toAdd)
        {
            resultDefines.Add(define); 
        }
        
        string resultDefinesStr = "";
        foreach (var define in resultDefines)
        {
            resultDefinesStr = resultDefinesStr + define + ";";
        }
        
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, resultDefinesStr);
    }
    private static List<string> GetDefinesFromPlatformSettings(BuildTargetGroup targetGroup)
    {
        string definesStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        if (string.IsNullOrEmpty(definesStr))
        {
            return new List<string>();
        }

        List<string> defines = definesStr.Split(';').ToList();
        return defines;
    }
}