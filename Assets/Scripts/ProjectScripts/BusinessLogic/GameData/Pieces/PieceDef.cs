using System.Collections.Generic;

public class PieceDef
{
    public string Uid { get; set; }
    public int Delay { get; set; }
    
    public string SpawnPiece { get; set; }
    public string UpgradeTargetPiece { get; set; }
    
    public int SpawnAmount { get; set; }
    public int SpawnCapacity { get; set; }
    public bool IsFilledInStart { get; set; }
    
    public int Limit { get; set; }
    public CurrencyPair StepReward { get; set; }
    public CurrencyPair Reproduction { get; set; }
    public CurrencyPair FastPrice { get; set; }
    
    public CurrencyPair SpawnResources { get; set; }
    public List<CurrencyPair> CreateRewards { get; set; }
    public List<CurrencyPair> UpgradePrices { get; set; }
    public List<CurrencyPair> UnlockBonus { get; set; }

    public string Name { get; set; }
     
    private CurrencyDef levelCurrencyDef;
    private CurrencyDef upgradeTargetCurrencyDef;

    private int upgradeTargetLevel;
    
    public int Piece => PieceType.Parse(Uid);
    public int SpawnPieceType => PieceType.Parse(SpawnPiece);

    public CurrencyDef UpgradeCurrency => levelCurrencyDef ?? (levelCurrencyDef = Currency.GetCurrencyDef($"Level{Uid.Substring(0, Uid.Length - 1)}"));

    public CurrencyDef UpgradeTargetCurrency
    {
        get
        {
            if (string.IsNullOrEmpty(UpgradeTargetPiece)) return null;

            if (upgradeTargetCurrencyDef != null) return upgradeTargetCurrencyDef;

            upgradeTargetCurrencyDef = Currency.GetCurrencyDef($"Level{UpgradeTargetPiece.Substring(0, UpgradeTargetPiece.Length - 1)}");
            
            upgradeTargetLevel = int.Parse(UpgradeTargetPiece.Substring(UpgradeTargetPiece.Length - 1, 1));
            
            return upgradeTargetCurrencyDef;
        }
    }

    public int CurrentLevel()
    {
        return ProfileService.Current.GetStorageItem(UpgradeCurrency.Name).Amount + 1;
    }

    public bool IsMaxLevel()
    {
        if (UpgradeTargetCurrency == null) return false;
        
        var targetLevel = ProfileService.Current.GetStorageItem(UpgradeTargetCurrency.Name).Amount + 1;
        
        return upgradeTargetLevel > targetLevel;
    }

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