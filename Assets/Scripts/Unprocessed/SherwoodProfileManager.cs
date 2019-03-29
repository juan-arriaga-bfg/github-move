public class SherwoodProfileManager<T>: ProfileManager<T>  where T : UserProfile, new()
{
    protected override void OnUploadProfileEventHandler(string error, string data)
    {
        base.OnUploadProfileEventHandler(error, data);
        
        if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(data))
        {
            return;
        }
        
        SocialUtils.SendProgress(data);
    }
}