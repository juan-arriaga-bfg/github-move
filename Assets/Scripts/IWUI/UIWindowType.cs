﻿using System.Collections.Generic;

public class UIWindowType
{
    public const string LauncherWindow = "LauncherWindow";
    
    public const string SampleWindow = "SampleWindow";
    
    public const string MainWindow = "MainWindow";
    
    public const string MessageWindow = "MessageWindow";
    
    public const string ChestMessage = "ChestMessageWindow";
    
    public const string QuestWindow = "QuestWindow";
    
    public const string ErrorWindow = "ErrorWindow";
    
    public const string ChestsShopWindow = "ChestsShopWindow";
    
    public const string EnergyShopWindow = "EnergyShopWindow";
    
    public const string CodexWindow = "CodexWindow";
    
    public const string CurrencyCheatSheetWindow = "CurrencyCheatSheetWindow";
    
    public const string PiecesCheatSheetWindow = "PiecesCheatSheetWindow";
    
    public const string ExchangeWindow = "ExchangeWindow";
    
    public const string OrdersWindow = "OrdersWindow";
    
    public const string QuestStartWindow = "QuestStartWindow";

    public const string NextLevelWindow = "NextLevelWindow";

    public const string ConfirmRemoverMessageWindow = "ConfirmRemoverMessageWindow";

    public static readonly HashSet<string> IgnoredWindows = new HashSet<string>
    {
        MainWindow,
        ErrorWindow
    };
    
    public static bool IsIgnore(string name)
    {
        return IgnoredWindows.Contains(name);
    }
}