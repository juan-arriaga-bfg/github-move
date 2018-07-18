public class FieldDef
{
    public string Uid { get; set; }
    public BoardPosition Position { get; set; }
    
    public int Id
    {
        get { return PieceType.Parse(Uid); }
    }
}