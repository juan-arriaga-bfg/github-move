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
        profile.RegisterComponent(new QuestSaveComponent());
//        profile.RegisterComponent(new CharacterSaveComponent());
    }
    
    public void SetDefaultSettings(UserProfile profile)
    {
        profile.GetStorageItem(Currency.Level.Name).Amount = 1;
        profile.GetStorageItem(Currency.Worker.Name).Amount = 2;
        profile.GetStorageItem(Currency.WorkerLimit.Name).Amount = 2;
        profile.GetStorageItem(Currency.Energy.Name).Amount = 100;
        profile.GetStorageItem(Currency.EnergyLimit.Name).Amount = 100;
    }
}
