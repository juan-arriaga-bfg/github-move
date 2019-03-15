using Debug = IW.Logger;
public class DefaultProfileMigration : IProfileMigration 
{
    public void Migrate(int clientVersion, UserProfile profile)
    {
        if (profile.SystemVersion < clientVersion)
        {
            //UnityEngine.Debug.Log(string.Format("[Profile]: migrate {0} => {1}", profile.SystemVersion, clientVersion));
            
            profile.SystemVersion++;
            Migrate(clientVersion, profile);
        }
    }
}
