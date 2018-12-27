using IWEditor;

public class BuildActionBuildAssetBundles : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        IWAssetBundleManagerEditorUtils.GenerateCurrentBundlesToStreaming();
    }
}