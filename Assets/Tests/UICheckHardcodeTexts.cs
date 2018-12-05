using System;
using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

namespace UTIgnored
{
    public class UICheckHardcodeTexts
    {

        private List<string> pathToCheck = new List<string> {"Assets/Scripts"};

        private List<string> ignoredFileNames = new List<string>
        {
            "R",
            "ConfigsGoogleLoader",
            "TextMeshSpriteAssetHelper",
            "WebRequestHelper",
            "MainSceneInitilizer",
            "GoogleLoaderSettings",
            "DevTools",
            "UIWindowType",
            "Currency",
            "TutorialBuilder",
        };
        
        private string searchFilter = "t:Script";

        [Test]
        public void UICheckHardcodeTextsSimplePasses()
        {

            // Use the Assert class to test conditions.
            var filteredAssets = GetFilteredAssets(pathToCheck, ignoredFileNames, searchFilter);

            var resultMessage = new StringBuilder();

            bool isValidated = true;
            foreach (var filteredAssetDef in filteredAssets)
            {
                var fileName = filteredAssetDef.Key;
                var fileLocalPath = filteredAssetDef.Value.Key;
                var fileAbsolutePath = filteredAssetDef.Value.Value;

                var scriptText = new StringBuilder();
                scriptText.Append(File.ReadAllText(fileAbsolutePath));

                IsValidScript(scriptText, result =>
                {
                    if (string.IsNullOrEmpty(result))
                    {

                    }
                    else
                    {
                        resultMessage.AppendLine($"{fileName} => {result}");
                        isValidated = false;
                    }
                });
            }

            if (isValidated)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail(resultMessage.ToString());
            }
        }

        public void IsValidScript(StringBuilder scriptText, Action<string> result)
        {
            string findHardcodeStringsPattern = ".+\\s*=\\s*(\").+";
            var findHardcodeStringsMatches = Regex.Matches(scriptText.ToString(), findHardcodeStringsPattern);

            var resultMessage = new StringBuilder();
            if (findHardcodeStringsMatches.Count > 0)
            {
                foreach (Match match in findHardcodeStringsMatches)
                {
                    var val = match.Value;
                    
                    resultMessage.AppendLine(val);
                }
            }
            
            if (result != null)
            {
                result(resultMessage.ToString());
            }
        }

        public static Dictionary<string, KeyValuePair<string, string>> GetFilteredAssets(List<string> pathToCheck, List<string> ignoredFileNames, string searchFilter = "")
        {
            var filesGuids = AssetDatabase.FindAssets(searchFilter, pathToCheck.ToArray());
            var filteredScriptAbsolutePath = new Dictionary<string, string>();
            var fileNamesCache = new Dictionary<string, string>();
            var processedGuids = new Dictionary<string, string>();

            var filteredFiles = new Dictionary<string, KeyValuePair<string, string>>();

            for (int i = 0; i < filesGuids.Length; i++)
            {
                var fileGuid = filesGuids[i];
                if (processedGuids.ContainsKey(fileGuid)) continue;

                var filePath = AssetDatabase.GUIDToAssetPath(fileGuid);
                if (filePath.Contains(".meta")) continue;

                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var absoluteFilePath = Path.Combine(Application.dataPath, filePath.Replace($"Assets/", ""));

                // skip directory
                if (File.GetAttributes(absoluteFilePath).HasFlag(FileAttributes.Directory))
                {
                    continue;
                }

                processedGuids.Add(fileGuid, fileName);

                if (ignoredFileNames.Contains(fileName)) continue;

                if (fileNamesCache.ContainsKey(fileName)) continue;

                fileNamesCache.Add(fileName, filePath);
                filteredScriptAbsolutePath.Add(fileName, absoluteFilePath);

                filteredFiles.Add(fileName, new KeyValuePair<string, string>(filePath, absoluteFilePath));
            }

            return filteredFiles;
        }


    }
}