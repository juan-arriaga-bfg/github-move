public class ServerSideConfigService : IWService<ServerSideConfigService, ServerSideConfigsManager>
{
    public static ServerSideConfigsManager Current => Instance.Manager;
}