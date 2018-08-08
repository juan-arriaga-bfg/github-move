public interface IPieceBoardObserver
{
    void OnAddToBoard(BoardPosition position, Piece context = null);
    
    void OnMovedFromToStart(BoardPosition from, BoardPosition to, Piece context = null);
    void OnMovedFromToFinish(BoardPosition from, BoardPosition to, Piece context = null);

    void OnRemoveFromBoard(BoardPosition position, Piece context = null);
}