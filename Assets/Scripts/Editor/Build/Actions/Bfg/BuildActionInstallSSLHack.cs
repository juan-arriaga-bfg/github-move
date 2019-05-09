#if UNITY_EDITOR

using UnityEngine;

// ReSharper disable once InconsistentNaming
public class BuildActionInstallSSLHack: IProjectBuildAction
{
    private ProjectBuilder context;
    
    private string SrcDir => Application.dataPath.Replace("/Assets", $"/Misc/BFG/{context.CurrentBuildPlatform.ToString()}/HackSSL/");
    private string DstDir => context.BuildPath + "/src/debug";
    // private override string DstDir => context.BuildPath + "/src/release";
    
    public void Execute(ProjectBuilder context)
    {
        this.context = context;
        
        ProjectBuildFileUtils.CopyDirectory(SrcDir, DstDir, true);
    }
}

#endif