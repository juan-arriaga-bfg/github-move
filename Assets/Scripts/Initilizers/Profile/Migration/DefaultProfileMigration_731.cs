using System.Collections.Generic;
using System.Linq;

public partial class DefaultProfileMigration
{
    private static void Migrate731(int clientVersion, UserProfile profile)
    {
        var ids = new Dictionary<int, int>
        {
            {5001, 5010}, // MN_B
            {5002, 5020}, // MN_C
            {5003, 5030}, // MN_E
            {5004, 5040}, // MN_F
            {5005, 5050}, // MN_H
            {5006, 5060}, // MN_I
        };
        
        MigrationUtils.ReplacePieceIdFromTo(profile, ids);
        MigrationUtils.ResetLife(profile, ids.Values.ToList());
    }
}