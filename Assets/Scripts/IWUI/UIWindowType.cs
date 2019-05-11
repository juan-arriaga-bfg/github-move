using System.Collections.Generic;

public class UIWindowType
{
    public const string LauncherWindow = "LauncherWindow";
    
    public const string MainWindow = "MainWindow";
    
    public const string MessageWindow = "MessageWindow";
    
    public const string ChestMessage = "ChestMessageWindow";
    
    public const string QuestWindow = "QuestWindow";
    
    public const string ErrorWindow = "ErrorWindow";
    
    public const string CodexWindow = "CodexWindow";
    
    public const string CurrencyCheatSheetWindow = "CurrencyCheatSheetWindow";
    
    public const string PiecesCheatSheetWindow = "PiecesCheatSheetWindow";
    
    public const string ExchangeWindow = "ExchangeWindow";
    
    public const string OrdersWindow = "OrdersWindow";
    
    public const string QuestStartWindow = "QuestStartWindow";

    public const string NextLevelWindow = "NextLevelWindow";
    
    public const string DailyQuestWindow = "DailyQuestWindow";
    
    public const string DailyRewardWindow = "DailyRewardWindow";
    
    public const string ResourcePanelWindow = "ResourcePanelWindow";
    
    public const string SettingsWindow = "SettingsWindow";
    
    public const string HardShopWindow = "HardShopWindow";
    
    public const string MarketWindow = "MarketWindow";
    
    public const string TimeSyncWindow = "TimeSyncWindow";
    
    public const string ConfirmationWindow = "ConfirmationWindow";

    public const string WaitWindow = "WaitWindow";

    public const string CreditsWindow = "CreditsWindow";
    
    public const string QuestCheatSheetWindow = "QuestCheatSheetWindow";
    
    public const string EnergyShopWindow = "EnergyShopWindow";
    
    public const string ProfileCheatSheetWindow = "ProfileCheatSheetWindow";
    
    public const string OfferWindow = "OfferWindow";

    public const string CharacterUnlockedWindow = "CharacterUnlockedWindow";

    public const string SystemDevParamsWindowWindow = "SystemDevParamsWindowWindow";
    
    public const string AbTestCheatSheetWindow = "AbTestCheatSheetWindow";

    public const string EventPreviewWindow = "EventPreviewWindow";
    
    public const string EventSubscriptionWindow = "EventSubscriptionWindow";
    
    public const string EventAlmostWindow = "EventAlmostWindow";
    
    public const string EventWindow = "EventWindow";
    
    public const string LootBoxWindow = "LootBoxWindow";

    public static readonly HashSet<string> IgnoredWindows = new HashSet<string>
    {
        MainWindow,
        ErrorWindow,
        ResourcePanelWindow
    };
    
    public static bool IsIgnore(string name)
    {
        return IgnoredWindows.Contains(name);
    }
}