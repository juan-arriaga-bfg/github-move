public class AsyncInitService : IWService<AsyncInitService, IAsyncInitManager> 
{
    public static AsyncInitManager Current => Instance.Manager as AsyncInitManager;
}