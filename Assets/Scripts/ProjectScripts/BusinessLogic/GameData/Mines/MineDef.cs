using System;
using System.Collections.Generic;

public class MineDef
{
    private int id = -1;
    public int Id => id == -1 ? (id = int.Parse(Uid.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)[1])) : id;

    public string Uid { get; set; }
    public string Skin { get; set; }
    public int Delay { get; set; }
    public int Size { get; set; }
    
    public CurrencyPair Price { get; set; }
    
    public BoardPosition Position { get; set; }
    
    public CurrencyPair Reward { get; set; }
    public List<CurrencyPair> StepRewards { get; set; }
    
    public int PieceAmount { get; set; }
    public List<ItemWeight> PieceWeights { get; set; }
}