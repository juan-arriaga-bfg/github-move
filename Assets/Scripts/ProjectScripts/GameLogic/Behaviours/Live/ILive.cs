public interface ILive
{
    int HitPoints { get; set; }
    bool IsLive(BoardPosition at);
}