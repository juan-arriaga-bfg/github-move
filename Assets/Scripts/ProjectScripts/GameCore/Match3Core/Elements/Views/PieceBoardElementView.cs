public class PieceBoardElementView : BoardElementView
{
    public Piece Piece { get; set; }
    
    public virtual void Init(BoardRenderer context, Piece piece)
    {
        base.Init(context);

        Piece = piece;
        Piece.ActorView = this;
    }

}
