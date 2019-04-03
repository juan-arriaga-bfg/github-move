public class SherwoodProfileManager<T>: ProfileManager<T>  where T : UserProfile, new()
{
    protected override void UploadToBackend(string data)
    {
        SocialUtils.ArchiveAndSend(data);
    }
}