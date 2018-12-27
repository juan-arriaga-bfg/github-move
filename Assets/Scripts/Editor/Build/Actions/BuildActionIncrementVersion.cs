using UnityEditor;

public class BuildActionIncrementVersion : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        IWProjectVersionSettings.Instance.BuildNumber++;
        IWProjectVerisonsEditor.GetCurrentVersion();
        EditorUtility.SetDirty(IWProjectVersionSettings.Instance);
        AssetDatabase.SaveAssets();
    }
}