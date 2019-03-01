public class ProfileInitComponent : AsyncInitComponentBase
{   
    public override void Execute()
    {
        string path = ProfileSlots.ActiveSlot;
        
        ProfileSlots.Load(path, (profile, exists, error) =>
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