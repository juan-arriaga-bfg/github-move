public class Notifier
{
    public TimerComponent Timer;
    public NotifyType NotifyType;

    public Notifier(TimerComponent timer, NotifyType notifyType)
    {
        Timer = timer;
        NotifyType = notifyType;
    }
}