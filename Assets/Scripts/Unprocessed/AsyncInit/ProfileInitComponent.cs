public class ProfileInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        ProfileLoader.LoadProfile(ProfileLoader.DEFAULT_PATH, (profile, exists, error) =>
        {
            ProfileService.Instance.SetManager(profile);
            ProfileLoaded();
        });

    }

    private void ProfileLoaded()
    {
        isCompleted = true;
        OnComplete(this);
    }
}