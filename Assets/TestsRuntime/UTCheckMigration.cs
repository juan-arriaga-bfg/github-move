using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace UTRuntime
{
    public class UTCheckMigration
    {
        private List<string> pathToFolderSaves = new List<string> {"Assets/Tests/MigrationSaves"};
        
        private string searchFilter = "t:TextAsset";

        private static string FixWinSlash(string path)
        {
            return path.Replace("\\", "/");
        }
        
        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        [Timeout(100000000)]
        public IEnumerator UTCheckMigrationPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
            
            var filesGuids = AssetDatabase.FindAssets(searchFilter, pathToFolderSaves.ToArray());
            var filteredScriptAbsolutePath = new Dictionary<string, string>();
            var fileNamesCache = new Dictionary<string, string>();
            var processedGuids = new Dictionary<string, string>();

            for (int i = 0; i < filesGuids.Length; i++)
            {
                var fileGuid = filesGuids[i];
                if (processedGuids.ContainsKey(fileGuid))
                {
                    continue;
                }

                var filePath = AssetDatabase.GUIDToAssetPath(fileGuid);
                var dataPath = FixWinSlash(Application.dataPath);
                var finalPath = FixWinSlash(Path.Combine(dataPath, filePath));
                
                Debug.LogWarning(filePath);
            }

            // reset progress in editor
            #if UNITY_EDITOR
            var progressPath = Application.dataPath + "/Resources/configs/profile.data.txt";
            File.Delete(progressPath);
            File.Delete(progressPath + ".meta");
            
            // File.Replace(progressPath, );
            AssetDatabase.Refresh();
            #endif

            SceneManager.LoadScene("Assets/Scenes/Launcher.unity", LoadSceneMode.Single);

            while (BoardService.Current == null || BoardService.Current.FirstBoard == null || UIService.Get.IsComplete == false)
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(1f);

            Assert.Pass();
        }
    }
}