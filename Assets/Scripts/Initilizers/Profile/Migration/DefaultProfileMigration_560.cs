using System.Collections.Generic;

public partial class DefaultProfileMigration
{
    private static void Migrate560(int clientVersion, UserProfile profile)
    {
        Dictionary<string, string> replacements = new Dictionary<string, string>
        {
            {"22,11,0", "Fog 1"},
            {"23,8,0",  "Fog 2"},
            {"26,8,0",  "Fog 3"},
            {"19,8,0",  "Fog 4"},
            {"19,4,0",  "Fog 5"},
            {"14,7,0",  "Fog 6"},
            {"11,9,0",  "Fog 7"},
            {"17,10,0", "Fog 8"},
            {"15,14,0", "Fog 9"},
            {"16,15,0", "Fog 10"},
            {"21,16,0", "Fog 11"},
            {"26,12,0", "Fog 12"},
            {"24,16,0", "Fog 13"},
            {"20,18,0", "Fog 14"},
        };
        
        var fogSave = profile.GetComponent<FogSaveComponent>(FogSaveComponent.ComponentGuid);

        if (fogSave.CompleteFogIds != null)
        {
            for (var i = 0; i < fogSave.CompleteFogIds.Count; i++)
            {
                var id = fogSave.CompleteFogIds[i];
                fogSave.CompleteFogIds[i] = replacements[id];
            }
        }

        if (fogSave.InprogressFogs != null)
        {
            for (var i = 0; i < fogSave.InprogressFogs.Count; i++)
            {
                var uid = fogSave.InprogressFogs[i].Uid;
                fogSave.InprogressFogs[i].Uid = replacements[uid];
            }

            fogSave.UpdateCache();
        }
    }
}