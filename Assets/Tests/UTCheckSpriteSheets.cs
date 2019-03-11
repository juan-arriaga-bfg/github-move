using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace UT
{
    public class UTCheckSpriteSheets
    {
        [Test]
        public void UTCheckSpriteSheetsSettings()
        {
            
            var scmanager = SpriteSheetsEditorUtils.LoadCurrentSpriteSheetManager();
    
            var isValid = true;
            for (var i = 0; i < scmanager.SpriteSheetsData.Count; i++)
            {
                var scdata = scmanager.SpriteSheetsData[i];

                bool isValidSpriteSheet = SpriteSheetsEditorUtils.IsValidSpriteSheetsContent(scdata);
                if (isValidSpriteSheet == false)
                {
                    isValid = isValidSpriteSheet;
                }
            }
    
            Assert.IsTrue(isValid);
        }
    
        [Test]
        public void UTCheckSpriteSheetsImport()
        {
            var scmanager = SpriteSheetsEditorUtils.LoadCurrentSpriteSheetManager();
    
            var isValid = true;
            for (var i = 0; i < scmanager.SpriteSheetsData.Count; i++)
            {
                var scdata = scmanager.SpriteSheetsData[i];
                // var spriteSheetTexture = SpriteSheetsEditorUtils.LoadSpriteSheetTexture(scdata);
    
                // var importer = (TextureImporter) AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(spriteSheetTexture));
                // var textureImporterSettings = new TextureImporterSettings();
                // importer.ReadTextureSettings(textureImporterSettings);
    
                // if (textureImporterSettings.spritePixelsPerUnit != 2)
                // {
                //     Debug.LogWarning(string.Format("TextureSettings for: {0} -> wrong spritePixelsPerUnit", spriteSheetTexture.name));
                //     isValid = false;
                // }
    
                // if (textureImporterSettings.mipmapEnabled)
                // {
                //     Debug.LogWarning(string.Format("TextureSettings for: {0} -> wrong mipmapEnabled", spriteSheetTexture.name));
                //     isValid = false;
                // }
    
                // if (textureImporterSettings.filterMode != FilterMode.Bilinear)
                // {
                //     Debug.LogWarning(string.Format("TextureSettings for: {0} -> wrong filterMode", spriteSheetTexture.name));
                //     isValid = false;
                // }
    // #if UNITY_ANDROID
    //             TextureImporterPlatformSettings platformSettings = importer.GetPlatformTextureSettings(GetActivePlatform());
    //             if (platformSettings.format != TextureImporterFormat.RGBA32 
    //                 && platformSettings.format != TextureImporterFormat.ETC2_RGB4
    //                 && platformSettings.format != TextureImporterFormat.ETC2_RGBA8)
    //             {
    //                 Debug.LogWarning(string.Format("TextureSettings for: {0} -> wrong textureFormat", spriteSheetTexture.name));
    //                 isValid = false;
    //             }
    // #elif UNITY_IOS
    //             var platformSettings = importer.GetPlatformTextureSettings(GetActivePlatform());
    //             if (platformSettings.format != TextureImporterFormat.RGBA32 && platformSettings.format != TextureImporterFormat.PVRTC_RGB4)
    //             {
    //                 Debug.LogWarning(string.Format("TextureSettings for: {0} -> wrong textureFormat", spriteSheetTexture.name));
    //                 isValid = false;
    //             }
    // #endif
            }
    
            Assert.IsTrue(isValid);
        }
    
        public static string GetActivePlatform()
        {
            var platformString = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS ? "iPhone" : EditorUserBuildSettings.activeBuildTarget.ToString();
    
            return platformString;
        }
    }
}