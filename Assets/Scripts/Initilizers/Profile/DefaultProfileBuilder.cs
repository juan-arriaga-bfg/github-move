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
        profile.RegisterComponent(new UserPurchasesComponent());
        profile.RegisterComponent(new UserSettingsComponent());
        profile.RegisterComponent(new FieldDefComponent());
    }

    public void SetDefaultSettings(UserProfile profile)
    {
        profile.GetStorageItem(Currency.Level.Name).Amount = 1;
    }
}
