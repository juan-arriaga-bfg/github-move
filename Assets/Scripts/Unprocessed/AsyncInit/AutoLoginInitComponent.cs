public class AutoLoginInitComponent : AsyncInitComponentBase
{
    public override void Execute()
    {
        SocialUtils.LoginAsync((error, token, id) =>
        {
            // Do not wait for login at this time
        });

        isCompleted = true;
        OnComplete(this);
    }
}