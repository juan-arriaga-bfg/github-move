using System;
using System.Collections.Generic;
using Debug = IW.Logger;


public partial class DefaultProfileMigration : IProfileMigration
{
    public readonly Dictionary<int, Action<int, UserProfile>> migrationDef = new Dictionary<int, Action<int, UserProfile>>
    {
        // Market slots
        // CR and WR ids change
        {490, Migrate484},
        
        // CR and WR ids change
        {522, Migrate522},
        
        // Fog centers        
        {560, Migrate560},
    };
    
    public void Migrate(int clientVersion, UserProfile profile)
    {
        var currentVersion = profile.SystemVersion;
        
        // Handle new profile
        if (currentVersion == 0)
        {
            profile.SystemVersion = clientVersion;
            return;
        }

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
