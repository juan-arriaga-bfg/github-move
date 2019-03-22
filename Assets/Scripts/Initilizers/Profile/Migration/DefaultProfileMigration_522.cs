using System.Collections.Generic;

public partial class DefaultProfileMigration
{
    private static void Migrate522(int clientVersion, UserProfile profile)
    {
        MigrationUtils.ReplacePieceIdFromTo(profile, new Dictionary<int, int>
        {
            {2001, 1000001}, // CR1
            {2002, 1000002}, // CR2
            {2003, 1000003}, // CR3
            {2004, 1000004}, // CR
            {2100, 1000100}, // WR
        });
    }
}