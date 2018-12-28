#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;

public abstract class BuildActionCopyFilesBase : IProjectBuildAction
{
    protected abstract string SrcDir { get;}
    protected abstract string DstDir { get;}
    protected abstract List<string> Files { get;}

    protected ProjectBuilder context;
    
    public virtual void Execute(ProjectBuilder context)
    {
        this.context = context;
        
        string srcDir = SrcDir;
        string dstDir = DstDir;
        var files = Files;

        if (string.IsNullOrEmpty(srcDir))
        {
            throw new Exception("[BuildActionCopyFilesBase] => Execute: SrcDir is null or empty");
        }
        
        if (string.IsNullOrEmpty(dstDir))
        {
            throw new Exception("[BuildActionCopyFilesBase] => Execute: DstDir is null or empty");
        }
        
        if (files == null || files.Count == 0)
        {
            throw new Exception("[BuildActionCopyFilesBase] => Execute: Files is null or empty");
        }

        foreach (var file in files)
        {
            File.Copy(srcDir + file, dstDir + file, true);
        }
    }
}

#endif