using UnityEngine;

public class ProfileInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        //init profile 
        var profileManager = new ProfileManager<UserProfile> { SystemVersion = IWVersion.Get.BuildNumber };
#if UNITY_EDITOR
        profileManager.Init(new ResourceConfigDataMapper<UserProfile>("configs/profile.data", false), new DefaultProfileBuilder(), new DefaultProfileMigration());
#else
        profileManager.Init(new StoragePlayerPrefsDataMapper<UserProfile>("user.profile"), new DefaultProfileBuilder(), new DefaultProfileMigration());
#endif
        ProfileService.Instance.SetManager(profileManager);
        
        // load local base profile
        ProfileService.Instance.Manager.LoadBaseProfile((baseProfile) =>
        {
            // condition to reset profile
            if (baseProfile == null || profileManager.SystemVersion > baseProfile.SystemVersion)
            {
                if (baseProfile == null)
                {
                    Debug.LogWarning($"[Reset progress] reset progress by: baseProfile == null:{baseProfile == null}");
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
                ProfileService.Instance.Manager.LoadCurrentProfile((profile) =>
                {
                    if (profile == null)
                    {
                        Debug.LogWarning($"[Reset progress] reset progress by: profile == null:{profile == null}");

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

    private void ProfileLoaded()
    {
        isCompleted = true;
        OnComplete(this);
    }
}