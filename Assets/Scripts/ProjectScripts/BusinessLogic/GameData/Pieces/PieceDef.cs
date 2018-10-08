using System.Collections.Generic;

public class PieceDef : SimplePieceDef
{
    public PiecesReproductionDef ReproductionDef;
    public PiecesMatchConditionsDef MatchConditionsDef;
    public PiecesMakingDef MakingDef;
    
    public string Name { get; set; }
    
    public CurrencyPair SpawnResources { get; set; }
    public CurrencyPair ExchangePrice { get; set; }
    
    public List<CurrencyPair> CreateRewards { get; set; }
    public List<CurrencyPair> UnlockBonus { get; set; }
    
    public static PieceDef Default()
    {
        return new PieceDef
        {
            Uid = PieceType.None.Abbreviations[0],
            Delay = 1,
            SpawnResources = new CurrencyPair{Currency = PieceType.Chest1.Abbreviations[0], Amount = 1}
        };
    }
}