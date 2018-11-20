using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditor;

public class UTUniqueResourceName {

    [Test]
    public void UTUniqueResourceNameSimplePasses() {
        // Use the Assert class to test conditions.
        // var scriptsGuids = AssetDatabase.FindAssets("", new string[]{});
        // var filteredScriptPath = new List<string>();
        // var filteredScriptAbsolutePath = new List<string>();
        // for (int i = 0; i < scriptsGuids.Length; i++)
        // {
        //     var scriptGuid = scriptsGuids[i];
        //     var scriptPath = AssetDatabase.GUIDToAssetPath(scriptGuid);
        //     if ((scriptPath.Contains($"{Path.DirectorySeparatorChar}Editor{Path.DirectorySeparatorChar}")) && scriptPath.Contains(".cs"))
        //     {
        //         filteredScriptPath.Add(scriptPath);
        //         filteredScriptAbsolutePath.Add(Path.Combine(Application.dataPath, scriptPath.Replace($"Assets{Path.DirectorySeparatorChar}", "")));
        //         
        //     }
        // }
        //
        // filteredScriptAbsolutePath.ForEach((path) => { Debug.LogWarning($"{path}");});
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator UTUniqueResourceNameWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
