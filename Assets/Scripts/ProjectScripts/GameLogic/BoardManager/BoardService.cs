public class BoardService : IWService<BoardService, BoardManager> 
{
    public static BoardManager Current
    {
        get { return Instance.Manager; }
    }
}
