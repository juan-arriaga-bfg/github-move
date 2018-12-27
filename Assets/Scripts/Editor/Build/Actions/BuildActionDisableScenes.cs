#if UNITY_EDITOR

using UnityEditor;

public class BuildActionDisableScenes : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        var scenes = EditorBuildSettings.scenes;

        for (var i = 1; i < scenes.Length; i++)
            scenes[i].enabled = false;

        EditorBuildSettings.scenes = scenes;
    }
}

#endif