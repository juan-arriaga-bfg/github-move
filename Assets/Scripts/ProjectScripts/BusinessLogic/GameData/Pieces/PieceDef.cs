using System.Collections.Generic;

public class PieceDef : SimplePieceDef
{
    public PiecesReproductionDef ReproductionDef;
    public PiecesMakingDef MakingDef;
    
    public string Name { get; set; }
    
    public CurrencyPair SpawnResources { get; set; }
    public CurrencyPair ExchangePrice { get; set; }
    
    public List<CurrencyPair> CreateRewards { get; set; }
    public List<CurrencyPair> UnlockBonus { get; set; }
}