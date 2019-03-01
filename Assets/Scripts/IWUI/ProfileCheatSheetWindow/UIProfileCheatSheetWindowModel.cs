using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIProfileCheatSheetSlotData
{
    public int SlotId;
    public string SlotPath;
    public ProfileManager<UserProfile> Profile;
    /// <summary>
    /// Means that we can't use profile for some reason and default profile created
    /// </summary>
    public bool IsDefault;

    public string Error;
}

public class UIProfileCheatSheetWindowModel : IWWindowModel
{
    public string Title => "Profile slots";

    public const int MAX_SLOTS = 20;
    
    public void GetExistingProfiles(Action<List<UIProfileCheatSheetSlotData>> onComplete)
    {
        List<UIProfileCheatSheetSlotData> ret = new List<UIProfileCheatSheetSlotData>();
        
        List<string> paths = new List<string>();
        paths.Add(ProfileLoader.DEFAULT_PATH);

        for (int i = 1; i < MAX_SLOTS; i++)
        {
            string slotPath = $"{ProfileLoader.DEFAULT_PATH}_{i}";
            paths.Add(slotPath);
        }

        for (var i = 0; i < paths.Count; i++)
        {
            var slotPath = paths[i];
#if UNITY_EDITOR
            var dataMapper = new ResourceConfigDataMapper<UserProfile>(slotPath, false);
#else
            var dataMapper = new StoragePlayerPrefsDataMapper<UserProfile>(slotPath);
#endif

            if (dataMapper.IsDataExists())
            {
                var index = i; //closure
                Load(slotPath, data =>
                {
                    ret.Add(data);

                    if (index == paths.Count - 1)
                    {
                        onComplete.Invoke(ret);
                    }
                });
            }
            else
            {
                if (i == paths.Count - 1)
                {
                    onComplete.Invoke(ret);
                }
            }
        }
    }

    private void Load(string path, Action<UIProfileCheatSheetSlotData> onComplete)
    {
        ProfileLoader.LoadProfile(path, (manager, dataExistsOnPath, error) =>
        {
            UIProfileCheatSheetSlotData data = new UIProfileCheatSheetSlotData();
            data.SlotId = 0;
            data.Profile = manager;
            data.SlotPath = ProfileLoader.DEFAULT_PATH;
            data.IsDefault = false;
            data.Error = error;
            
            onComplete(data);
        });
    }
}