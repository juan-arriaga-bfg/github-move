public class BfgSdkService : IWService<BfgSdkService, IBfgSdkManager> 
{
    public static BfgSdkManager Current => Instance.Manager as BfgSdkManager;
}