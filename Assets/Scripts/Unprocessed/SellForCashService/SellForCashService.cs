public class SellForCashService : IWService<SellForCashService, SellForCashManager>
{
    public static SellForCashManager Current => Instance.Manager;
}