namespace BfgAnalytics
{
    public static class Analytics
    {
        // Fields naming
        // st1 - details1 - category
        // st2 - details2 - type
        // st3 - details3 - name
        // n   - name     - action
        // l   - level    - placeholder
        // v   - value    - value

        public static JsonDataGroup AllJsonDataExceptTransaction()
        {
            return JsonDataGroup.Standart | JsonDataGroup.Userstats | JsonDataGroup.Balances | JsonDataGroup.Flags | JsonDataGroup.Story;
        }
        
        public static JsonDataGroup AllJsonData()
        {
            return JsonDataGroup.Standart | JsonDataGroup.Userstats | JsonDataGroup.Balances | JsonDataGroup.Flags | JsonDataGroup.Story | JsonDataGroup.Transaction;
        }
        
        public static void SendQuestStartEvent(string questId)
        {
            AnalyticsService.Current?.Event("progress", "quest", questId, "start", AllJsonDataExceptTransaction());
        }
        
        public static void SendQuestCompletedEvent(string id)
        {
            AnalyticsService.Current?.Event("progress", "quest", id, "start", AllJsonData());
        }
        
        public static void SendPieceUnlockedEvent(string id)
        {
            AnalyticsService.Current?.Event("progress", "piece", id, "unlock", AllJsonDataExceptTransaction());
        }
        
        public static void SendCharUnlockedEvent(string id)
        {
            AnalyticsService.Current?.Event("progress", "character", id, "unlock", AllJsonDataExceptTransaction());
        }
        
        public static void SendFogClearedEvent(string id)
        {
            AnalyticsService.Current?.Event("progress", "fog", id, "unlock", AllJsonData());
        }
        
        public static void SendLevelReachedEvent(int level)
        {
            AnalyticsService.Current?.Event("progress", "level", level.ToString(), null, AllJsonData());
        }
    }
}