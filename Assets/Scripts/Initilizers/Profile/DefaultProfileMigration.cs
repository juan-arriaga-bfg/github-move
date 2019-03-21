using System;
using System.Collections.Generic;
using Debug = IW.Logger;


public class DefaultProfileMigration : IProfileMigration
{
    public readonly Dictionary<int, Action<int, UserProfile>> migrationDef = new Dictionary<int, Action<int, UserProfile>>
    {
        {490, Migrate484},
        {522, Migrate522}
    };
    
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

    private static void Migrate484(int clientVersion, UserProfile profile)
    {
        MigrationUtils.ReplacePieceIdFromTo(profile, new Dictionary<int, int>
        {
            {2001, 1000001}, // CR1
            {2002, 1000002}, // CR2
            {2003, 1000003}, // CR3
            {2004, 1000004}, // CR
            {2100, 1000100}, // WR
        });

        if (profile.MarketSave.Slots == null) return;
        
        // add slot Uid
        foreach (var slot in profile.MarketSave.Slots)
        {
            slot.Index += 1;
        }
    }
    
    public void Migrate(int clientVersion, UserProfile profile)
    {
        var currentVersion = profile.SystemVersion;

        MigrateIteration(clientVersion, profile);
        
        if (currentVersion != clientVersion)
        {
            OnCompleteMigration(profile);
        }
    }

    private void MigrateIteration(int clientVersion, UserProfile profile)
    {
        if (migrationDef.TryGetValue(profile.SystemVersion, out var migrationDelegate))
        {
            Debug.LogWarning($"[Profile]: migrate {profile.SystemVersion} => {clientVersion}");
            
            migrationDelegate(clientVersion, profile);
        }
        
        if (profile.SystemVersion < clientVersion)
        {
            // UnityEngine.Debug.Log(string.Format("[Profile]: migrate {0} => {1}", profile.SystemVersion, clientVersion));
            
            profile.SystemVersion++;
            MigrateIteration(clientVersion, profile);
        }
    }

    public void OnCompleteMigration(UserProfile profile)
    {
        
    }
}
