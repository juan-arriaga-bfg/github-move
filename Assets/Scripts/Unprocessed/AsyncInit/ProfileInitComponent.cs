using System.IO;
using UnityEngine;

public class ProfileInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        //init profile 
        var profileManager = new ProfileManager<UserProfile> { SystemVersion = IWVersion.Get.BuildNumber };
        
#if UNITY_EDITOR
        var dataMapper = new ResourceConfigDataMapper<UserProfile>("configs/profile.data", false);
#else
        var dataMapper = new StoragePlayerPrefsDataMapper<UserProfile>("user.profile");
#endif

        profileManager.Init(dataMapper, new DefaultProfileBuilder(), new DefaultProfileMigration());

        ProfileService.Instance.SetManager(profileManager);
        
        // load local base profile
        ProfileService.Instance.Manager.LoadBaseProfile((baseProfile, errorBase) =>
        {
            // condition to reset profile
            if (baseProfile == null || profileManager.SystemVersion > baseProfile.SystemVersion)
            {
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
                ProfileService.Instance.Manager.ReplaceProfile(profileBuilder.Create());

                ProfileLoaded();
            }
            else
            {
                // load local profile
                ProfileService.Instance.Manager.LoadCurrentProfile((profile, errorCurrent) =>
                {
                    if (profile == null)
                    {
#if DEBUG
                        PrintProfileText(dataMapper);
#endif
                        
                        Debug.LogWarning($"[Reset progress] reset progress by: profile == null");

                        var profileBuilder = new DefaultProfileBuilder();
                        ProfileService.Instance.Manager.ReplaceProfile(profileBuilder.Create());
                    }
                    else
                    {
                        new DefaultProfileBuilder().SetupComponents(profile);
                        ProfileService.Instance.Manager.CheckMigration();
                    }
                    
                    ProfileLoaded();
                });
            }
        });
    }

#if DEBUG
    private void PrintProfileText(IJsonDataMapper<UserProfile> dataMapper)
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
    
    private void ProfileLoaded()
    {
        isCompleted = true;
        OnComplete(this);
    }
    
    private void LogLongString(string text)
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
}