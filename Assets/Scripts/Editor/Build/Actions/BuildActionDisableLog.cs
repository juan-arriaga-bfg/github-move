#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using IW;
using UnityEditor;

public class BuildActionDisableLogger : IProjectBuildAction
{    
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
        
        // Defines from platform settings
        List<string> existingDefines = GetDefinesFromPlatformSettings(buildTarget);
        foreach (var define in existingDefines)
        {
            if (define == Logger.ENABLE_DEFINE)
            {
                continue;
            }
            
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

#endif