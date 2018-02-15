public class DefaultProfileBuilder : IProfileBuilder 
{
    public UserProfile Create()
    {
        var profile = new UserProfile();

        SetupComponents(profile);
        SetDefaultSettings(profile);
        
        return profile;
    }

    public void SetupComponents(UserProfile profile)
    {
//        profile.RegisterComponent(new LevelProgress());
    }

    public void SetDefaultSettings(UserProfile profile)
    {
//        profile.GetStorageItem(Currency.Coins).Amount = 60;
    }
}
