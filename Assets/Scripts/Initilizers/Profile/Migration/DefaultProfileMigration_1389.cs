using System.Collections.Generic;

public partial class DefaultProfileMigration
{
    private static void Migrate1389(int clientVersion, UserProfile profile)
    {
        if (profile.CodexSave != null && profile.CodexSave.Data != null)
        {
            var idsForChange = new List<KeyValuePair<int, int>>
            {
                new KeyValuePair<int, int>(9000201, 9000200), //Boost_WR
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