public interface IDamageable
{
    bool PerformHit(int damage, BoardPosition piecePosition, Piece attaker, BoardPosition attakerPosition);

    bool IsDamageable(BoardPosition at);
}