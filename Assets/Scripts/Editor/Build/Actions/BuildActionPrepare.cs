using IWEditor;

public class BuildActionPrepare : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        IWAssetBundleManagerEditorUtils.RenameBundlePathesWithResources(IWAssetBundleManagerEditorUtils.MaskFolderName, IWAssetBundleManagerEditorUtils.TargetMaskFolderName);
    }
}