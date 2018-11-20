#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenSceneMenu : Editor
{
    [MenuItem("Open Scene/Launcher", false, 20)]
    private static void Launcher()
    {
        OpenIf("Assets/Scenes/Launcher.unity");
    }

    [MenuItem("Open Scene/Main", false, 20)]
    private static void Main()
    {
        OpenIf("Assets/Scenes/Main.unity");
    }

    private static void OpenIf(string level)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(level);
        }
    }
}

#endif
