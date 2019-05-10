using BFGSDK;
using Debug = IW.Logger;

using UnityEngine;

public static class TackleBoxEvents
{
    private static void Send(string id)
    {
        Debug.Log($"[TackleBoxEvents] => Send: {id}");
        
#if !UNITY_EDITOR
        bfgGameReporting.logCustomPlacement(id);  
#endif  
    }

    public static void SendCollectionOpen()
    {
        Send("collection_open");
    }

    public static void SendCollectionClosed()
    {
        Send("collection_closed");
    }

    public static void SendEnergyOpen()
    {
        Send("energy_open");
    }

    public static void SendEnergyClosed()
    {
        Send("energy_closed");
    }

    public static void SendGameResumed()
    {
        Send("game_resumed");
    }
    
    public static void SendLevelUp()
    {
        Send("level_up");
    }

    public static void SendMarketOpen()
    {
        Send("market_open");
    }

    public static void SendMarketClosed()
    {
        Send("market_closed");
    }

    public static void SendOrdersOpen()
    {
        Send("orders_open");
    }

    public static void SendOrdersClosed()
    {
        Send("orders_closed");
    }

    public static void SendSettingsOpen()
    {
        Send("settings_open");
    }

    public static void SendSettingsClosed()
    {
        Send("settings_closed");
    }

    public static void SendQuestComplete()
    {
        Send("quest_complete");
    }

    public static void SendShopClose()
    {
        Send("shop_close");
    }

    public static void SendShopEnter()
    {
        Send("shop_enter");
    }
}