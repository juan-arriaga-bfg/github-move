#if UNITY_EDITOR

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class BuildActionUpdateAndroidVersionCode : IProjectBuildAction
{
    private readonly Regex regex = new Regex(@"(?<=versionCode )[0-9]*", RegexOptions.Singleline | RegexOptions.Compiled);
    
    public void Execute(ProjectBuilder context)
    {
        if (context.CurrentBuildPlatform != ProjectBuilder.BuildPlatform.Amazon && context.CurrentBuildPlatform != ProjectBuilder.BuildPlatform.Android)
        {
            return;
        }
        
        string path = Application.dataPath + "/Plugins/Android/mainTemplate.gradle";

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"[BuildActionUpdateAndroidVersionCode] => file: '{path}' not exists");
        }

        string text = File.ReadAllText(path);
        
        int version = IWProjectVersionSettings.Instance.BuildNumber;
        
        Debug.Log($"[BuildActionUpdateAndroidVersionCode] => file: '{path}', version: {version}");

        string fixedText = regex.Replace(text, version.ToString());
        
        File.WriteAllText(path, fixedText, new UTF8Encoding(false));
    }
}

#endif