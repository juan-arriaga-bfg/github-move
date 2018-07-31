using System;

public class MineDef
{
    private int id = -1;
    public int Id
    {
        get
        {
            if (id == -1)
            {
                id = int.Parse(Uid.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)[1]);
            }
            
            return id;
        }
    }

    public string Uid { get; set; }
    public string Skin { get; set; }
    public int Delay { get; set; }
    public int Size { get; set; }
    public BoardPosition Position { get; set; }
    public CurrencyPair Reward { get; set; }
    public CurrencyPair StepReward { get; set; }
    public CurrencyPair Price { get; set; }
    public CurrencyPair FastPrice { get; set; }
}