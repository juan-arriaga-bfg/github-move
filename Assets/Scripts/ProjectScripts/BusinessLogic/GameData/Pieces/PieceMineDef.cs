using System.Collections.Generic;

public class PieceMineDef : SimplePieceDef
{
    public int Cooldown;
    
    public int Size;
    
    public CurrencyPair Price;
    public CurrencyPair Reward;
    
    public List<CurrencyPair> StepRewards;
    public List<CurrencyPair> LastRewards;
}