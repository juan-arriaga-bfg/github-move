using Debug = IW.Logger;
using System.Collections.Generic;

public static class MigrationUtils
{
    public static void ReplacePieceIdFromTo(UserProfile profile, Dictionary<int, int> fromToDef)
    {
        if (profile.FieldDef == null) return;
        
        for (int i = 0; i < profile.FieldDef.Pieces.Count; i++)
        {
            var pieceSaveItem = profile.FieldDef.Pieces[i];

            int targetId = -1;
            if (fromToDef.TryGetValue(pieceSaveItem.Id, out targetId))
            {
                Debug.LogWarning(string.Format("[MigrationUtils]: replace piece id {0} => {1}", pieceSaveItem.Id, targetId));
                pieceSaveItem.Id = targetId;
            }
        }
    }
}