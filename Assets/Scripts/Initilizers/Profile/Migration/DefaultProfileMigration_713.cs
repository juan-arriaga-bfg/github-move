using System.Collections.Generic;
using System.Linq;

public partial class DefaultProfileMigration
{
    private static void Migrate713(int clientVersion, UserProfile profile)
    {
        var ids = new Dictionary<int, int>
        {
            {5001, 5011}, // MN_B -> MN_B1
            {5002, 5021}, // MN_C -> MN_C1
            {5003, 5031}, // MN_E -> MN_E1
            {5004, 5041}, // MN_F -> MN_F1
            {5005, 5051}, // MN_H -> MN_H1
            {5006, 5061}, // MN_I -> MN_I1
        };
        
        MigrationUtils.ReplacePieceIdFromTo(profile, ids);
        MigrationUtils.ResetLife(profile, ids.Values.ToList());
    }
}