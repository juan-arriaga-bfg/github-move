public class Notifier
{
    public ITimerComponent Timer;
    public NotifyType NotifyType;

    public Notifier(ITimerComponent timer, NotifyType notifyType)
    {
        Timer = timer;
        NotifyType = notifyType;
    }
}