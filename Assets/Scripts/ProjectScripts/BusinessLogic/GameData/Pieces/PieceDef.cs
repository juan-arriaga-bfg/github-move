using System.Collections.Generic;

public class PieceDef
{
    public string Uid { get; set; }
    public int Delay { get; set; }
    
    public string SpawnPiece { get; set; }
    public string UpgradeTargetPiece { get; set; }
    
    public int ChestLevel { get; set; }
    
    public int SpawnAmount { get; set; }
    public int SpawnCapacity { get; set; }
    public bool IsFilledInStart { get; set; }
    
    public CurrencyPair Reproduction { get; set; }
    public CurrencyPair SpawnResources { get; set; }
    public List<CurrencyPair> CreateRewards { get; set; }
    public CurrencyPair UpgradePrice { get; set; }
    
    private CurrencyDef levelCurrencyDef;
    private CurrencyDef upgradeTargetCurrencyDef;
    
    public int Piece
    {
        get { return PieceType.Parse(Uid); }
    }
    
    public int SpawnPieceType
    {
        get { return PieceType.Parse(SpawnPiece); }
    }
    
    public CurrencyDef UpgradeCurrency
    {
        get {return levelCurrencyDef ?? (levelCurrencyDef = Currency.GetCurrencyDef(string.Format("Level{0}", Uid.Substring(0, Uid.Length - 1))));}
    }

    public CurrencyDef UpgradeTargetCurrency
    {
        get
        {
            if (string.IsNullOrEmpty(UpgradeTargetPiece)) return null;
            
            return upgradeTargetCurrencyDef ?? (upgradeTargetCurrencyDef = Currency.GetCurrencyDef(string.Format("Level{0}", UpgradeTargetPiece)));
        }
    }

    public int CurrentLevel()
    {
        return ProfileService.Current.GetStorageItem(UpgradeCurrency.Name).Amount + 1;
    }

    public bool IsMaxLevel()
    {
        if (UpgradeTargetCurrency == null) return false;
        
        var targetLevel = ProfileService.Current.GetStorageItem(UpgradeTargetCurrency.Name).Amount;
        
        return CurrentLevel() > targetLevel;
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