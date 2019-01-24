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
    
    [MenuItem("Open Scene/Text Styles", false, 20)]
    private static void TextStyles()
    {
        OpenIf("Assets/Scenes/TextStyles.unity");
    }
    
    [MenuItem("Open Scene/Reload", false, 20)]
    private static void Reload()
    {
        OpenIf("Assets/Scenes/Reload.unity");
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
