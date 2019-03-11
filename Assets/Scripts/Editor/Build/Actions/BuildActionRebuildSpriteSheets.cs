#if UNITY_EDITOR

using IWEditor;

public class BuildActionRebuildSpriteSheets : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        SpriteSheetsEditorUtils.RebuildContentAll();
    }
}

#endif
