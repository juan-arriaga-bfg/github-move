using System;
using System.Collections.Generic;
using Debug = IW.Logger;


public class DefaultProfileMigration : IProfileMigration
{
    public Dictionary<int, Action<int, UserProfile>> migrationDef = new Dictionary<int, Action<int, UserProfile>>
    {
        {490, Migrate484}
    };

    private static void Migrate484(int clientVersion, UserProfile profile)
    {
        
    }
    
    public void Migrate(int clientVersion, UserProfile profile)
    {
        Action<int, UserProfile> migrationDelegate = null;
        if (migrationDef.TryGetValue(profile.SystemVersion, out migrationDelegate))
        {
            Debug.LogWarning(string.Format("[Profile]: migrate {0} => {1}", profile.SystemVersion, clientVersion));
            
            migrationDelegate(clientVersion, profile);
        }
        
        if (profile.SystemVersion < clientVersion)
        {
            // UnityEngine.Debug.Log(string.Format("[Profile]: migrate {0} => {1}", profile.SystemVersion, clientVersion));
            
            profile.SystemVersion++;
            Migrate(clientVersion, profile);
        }
    }
}
