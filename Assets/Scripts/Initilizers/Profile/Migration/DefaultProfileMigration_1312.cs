using System.Collections.Generic;

public partial class DefaultProfileMigration
{
    private static void Migrate1312(int clientVersion, UserProfile profile)
    {
        if (profile.CodexSave != null && profile.CodexSave.Data != null)
        {
            var idsForChange = new List<KeyValuePair<int, int>>
            {
                new KeyValuePair<int, int>(5100101, 5100103), //SK3_PR
                new KeyValuePair<int, int>(9000101, 9000100), //Boost_CR
                new KeyValuePair<int, int>(7200101, 7200105), //Soft5
                new KeyValuePair<int, int>(7200101, 7200106), //Soft6
            };
            
            
            foreach (var pair in idsForChange)
            {
                var startKey = pair.Key;
                var pieceId = pair.Value;

                if (profile.CodexSave.Data.ContainsKey(startKey) == false)
                {
                    continue;
                }
                
                var chainState = profile.CodexSave.Data[startKey];
                
                if (chainState.PendingReward.Contains(pieceId))
                {
                    continue;
                }
                
                if (chainState.Unlocked.Contains(pieceId))
                {
                    chainState.Unlocked.Remove(pieceId);
                }
            }
        }
    }
}