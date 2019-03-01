using System;
using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;

#if UNITY_EDITOR
    using UnityEditor;
#endif

using UnityEngine;

public static class ProfileSlots
{
    private const string KEY = "ActiveSlot";
    
#if UNITY_EDITOR
    public const string DEFAULT_SLOT_PATH = "configs/profile.data";
#else
    public const string DEFAULT_PATH = "user.profile";
#endif

    public static string ActiveSlot
    {
        get
        {
            var activeSlot = ObscuredPrefs.GetString(KEY, null);
            if (string.IsNullOrEmpty(activeSlot))
            {
                ObscuredPrefs.SetString(KEY, DEFAULT_SLOT_PATH);
                return DEFAULT_SLOT_PATH;
            }

            return activeSlot;
        }
        set => ObscuredPrefs.SetString(KEY, value);
    }

    public static IJsonDataMapper<UserProfile> GetDataMapper(string path)
    {
#if UNITY_EDITOR
        IJsonDataMapper<UserProfile> dataMapper = new ResourceConfigDataMapper<UserProfile>(path, false);
#else
        IJsonDataMapper<UserProfile> dataMapper = new StoragePlayerPrefsDataMapper<UserProfile>(path);
#endif
        return dataMapper;
    }
    
    public delegate void LoadCallback(ProfileManager<UserProfile> profileManager, bool dataExistsOnPath, string error);
    public static void Load(string path, LoadCallback onComplete)
    {
        //init profile 
        ProfileManager<UserProfile> profileManager = new ProfileManager<UserProfile> {SystemVersion = IWVersion.Get.BuildNumber};

        var dataMapper = GetDataMapper(path);

        bool dataExists = dataMapper.IsDataExists();
        
        profileManager.Init(dataMapper, new DefaultProfileBuilder(), new DefaultProfileMigration());

        // load local base profile
        profileManager.LoadBaseProfile((baseProfile, baseError) =>
        {
            // condition to reset profile
            if (baseProfile == null || profileManager.SystemVersion > baseProfile.SystemVersion)
            {
                var error = baseError ?? (baseProfile == null ? "Can't load base profile" : null);
                
                if (baseProfile == null)
                {
#if DEBUG
                    PrintProfileText(dataMapper);
#endif

                    Debug.LogWarning($"[Reset progress] reset progress by: baseProfile == null");
                }
                else
                {
                    Debug.LogWarning($"[Reset progress] profileManager.SystemVersion > baseProfile.SystemVersion:{profileManager.SystemVersion > baseProfile.SystemVersion}");
                }

                var profileBuilder = new DefaultProfileBuilder();
                profileManager.ReplaceProfile(profileBuilder.Create());

                onComplete(profileManager, dataExists, error);
            }
            else
            {
                // load local profile
                profileManager.LoadCurrentProfile((profile, currentError) =>
                {
                    var error = currentError ?? (profile == null ? "Can't load profile" : null);
                    
                    if (profile == null)
                    {
#if DEBUG
                        PrintProfileText(dataMapper);
#endif

                        Debug.LogWarning($"[Reset progress] reset progress by: profile == null");

                        var profileBuilder = new DefaultProfileBuilder();
                        profileManager.ReplaceProfile(profileBuilder.Create());
                    }
                    else
                    {
                        new DefaultProfileBuilder().SetupComponents(profile);
                        profileManager.CheckMigration();
                    }

                    onComplete(profileManager, dataExists, error);
                });
            }
        });
    }

#if DEBUG
    private static void PrintProfileText(IJsonDataMapper<UserProfile> dataMapper)
    {
        var profileText = dataMapper.GetJsonDataAsString();
        if (!string.IsNullOrEmpty(profileText))
        {
            LogLongString(profileText);
        }
        else
        {
            Debug.LogWarning("[ProfileInitComponent] => PrintProfileText: profile text is null or empty");
        }
    }
#endif
    
    private static void LogLongString(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        using (StringReader reader = new StringReader(text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Debug.Log(line);
            }
        }
    }

    public static void Delete(string path)
    {
        var dataMapper = GetDataMapper(path);

        switch (dataMapper)
        {
            case ResourceConfigDataMapper<UserProfile> filesystem:
                var fileName = $"{Application.dataPath}/Resources/{path}.txt";
                File.Delete(fileName);
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
                break;
               
            case StoragePlayerPrefsDataMapper<UserProfile> prefs:
                ObscuredPrefs.DeleteKey(path);
                break; 
        }
    }
}