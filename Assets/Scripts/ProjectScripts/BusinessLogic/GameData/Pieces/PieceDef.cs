using System.Collections.Generic;

public class PieceDef : SimplePieceDef
{
    public PieceReproductionDef ReproductionDef;
    public PieceMakingDef MakingDef;
    public PieceMineDef MineDef;
    
    public string Name => LocalizationService.Get($"piece.name.{Uid}", $"piece.name.{Uid}");
    
    public CurrencyPair SpawnResources { get; set; }
    public CurrencyPair ExchangePrice { get; set; }
    
    public List<CurrencyPair> CreateRewards { get; set; }
    public List<CurrencyPair> UnlockBonus { get; set; }
}