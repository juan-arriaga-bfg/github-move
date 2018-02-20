public interface IPieceBoardObserver
{
    void OnAddToBoard(BoardPosition position, Piece context = null);

    void OnRemoveFromBoard(BoardPosition position, Piece context = null);
}