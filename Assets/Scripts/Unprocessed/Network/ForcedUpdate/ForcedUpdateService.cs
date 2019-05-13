public class ForcedUpdateService : IWService<ForcedUpdateService, ForcedUpdateManager>
{
    public static ForcedUpdateManager Current => Instance.Manager;
}