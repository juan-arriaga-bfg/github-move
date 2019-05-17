public class MatchDescription
{
    public int SourcePieceType;
    public int SourcePiecesCount;
    public int CreatedPieceType;
    public int CreatedPiecesCount;

    public override string ToString()
    {
        string src = PieceType.Parse(SourcePieceType);
        string dst = PieceType.Parse(CreatedPieceType);
        
        return $"{src} x {SourcePiecesCount} => {dst} x {CreatedPiecesCount}";
    }
}