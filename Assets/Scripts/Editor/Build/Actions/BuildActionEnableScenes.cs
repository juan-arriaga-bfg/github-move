#if UNITY_EDITOR

using UnityEditor;

public class BuildActionEnableScenes : IProjectBuildAction
{
    public void Execute(ProjectBuilder context)
    {
        var scenes = EditorBuildSettings.scenes;

        for (var i = 1; i < scenes.Length; i++)
            scenes[i].enabled = true;

        EditorBuildSettings.scenes = scenes;
    }
}

#endif