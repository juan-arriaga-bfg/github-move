public class SimplePieceDef
{
    public string Uid;

    private int id = -1;
    public int Id => id > 0 ? id : (id = PieceType.Parse(Uid));

    public int Delay;
    public CurrencyPair FastPrice;
}