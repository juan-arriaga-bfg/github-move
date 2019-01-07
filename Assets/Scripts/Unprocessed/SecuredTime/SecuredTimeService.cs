public class SecuredTimeService : IWService<SecuredTimeService, ISecuredTimeManager> 
{
    public static SecuredTimeManager Current => Instance.Manager as SecuredTimeManager;
}