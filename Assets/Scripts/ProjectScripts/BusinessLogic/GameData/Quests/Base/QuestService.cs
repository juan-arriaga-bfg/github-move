public class QuestService : IWService<QuestService, QuestManager>
{
    public static QuestManager Current
    {
        get { return Instance.Manager; }
    }
}