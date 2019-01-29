#if UNITY_EDITOR

using System;
using System.IO;
using UnityEngine;

public class BuildActionCopyNotificationIcons : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        string dst = context.BuildPath + "/src/res";;
        string src = Application.dataPath.Replace("/Assets", "/Misc/BFG/Android/NotificationsIcons");

        if (!Directory.Exists(src))
        {
            throw new Exception($"Can't found notiications  at {src}");
        }
        
        ProjectBuildFileUtils.CopyDirectory(src, dst, false);
    }
}

#endif