#if UNITY_EDITOR

using IWEditor;

public class BuildActionReset : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        IWAssetBundleManagerEditorUtils.RenameBundlePathesWithResources(IWAssetBundleManagerEditorUtils.TargetMaskFolderName, IWAssetBundleManagerEditorUtils.MaskFolderName);
        new BuildActionEnableScenes().Execute(null);
    }
}

#endif