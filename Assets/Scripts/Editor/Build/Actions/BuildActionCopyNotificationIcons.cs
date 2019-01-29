#if UNITY_EDITOR

using System;
using System.IO;
using UnityEngine;

public class BuildActionCopyNotificationIcons : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        string src = Application.dataPath.Replace("/Assets", "/Misc/BFG/Android/NotificationsIcons");
        string dst = context.BuildPath + "/src/main/res";;

        if (!Directory.Exists(src))
        {
            throw new Exception($"Can't found notiications  at {src}");
        }
        
        ProjectBuildFileUtils.CopyDirectory(src, dst, false);
    }
}

#endif