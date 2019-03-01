using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIProfileCheatSheetWindowModel : IWWindowModel
{
    public string Title => "Saves";

    public const int MAX_SLOTS = 20;
    
    public void GetExistingProfiles(Action<List<UIProfileCheatSheetSlotData>> onComplete)
    {
        List<UIProfileCheatSheetSlotData> ret = new List<UIProfileCheatSheetSlotData>();
        
        List<string> paths = new List<string>();

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            string slotPath = GetSlotPathByIndex(i);
            paths.Add(slotPath);
        }

        for (var i = 0; i < paths.Count; i++)
        {
            var slotPath = paths[i];

            if (IsSlotUsed(slotPath))
            {
                var index = i; //closure
                Load(slotPath, data =>
                {
                    data.SlotIndex = index;
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

    public static string GetSlotPathByIndex(int index)
    {
        return index == 0 ? ProfileSlots.DEFAULT_SLOT_PATH : $"{ProfileSlots.DEFAULT_SLOT_PATH}_{index}";
    }

    private void Load(string path, Action<UIProfileCheatSheetSlotData> onComplete)
    {
        ProfileSlots.Load(path, (manager, dataExistsOnPath, error) =>
        {
            UIProfileCheatSheetSlotData data = new UIProfileCheatSheetSlotData();
            data.SlotIndex = 0;
            data.Profile = manager;
            data.SlotPath = path;
            data.IsDefault = false;
            data.Error = error;
            
            onComplete(data);
        });
    }

    public static string GetFreeSlotPath()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            string slotPath = GetSlotPathByIndex(i);
            if (!IsSlotUsed(slotPath))
            {
                return slotPath;
            }
        }

        return null;
    }

    public static bool IsSlotUsed(string slotPath)
    {
        var dataMapper = ProfileSlots.GetDataMapper(slotPath);
        return dataMapper.IsDataExists();
    }
}