public class SilentUpdateService : IWService<SilentUpdateService, SilentUpdateManager>
{
    public static SilentUpdateManager Current => Instance.Manager;
}