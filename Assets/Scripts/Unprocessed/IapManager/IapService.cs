public class IapService : IWService<IapService, IapManager>
{
    public static IapManager Current => Instance.Manager;
}