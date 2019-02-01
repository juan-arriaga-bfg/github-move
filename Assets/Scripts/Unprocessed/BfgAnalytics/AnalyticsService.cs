namespace BfgAnalytics
{
    public class AnalyticsService : IWService<AnalyticsService, IAnalyticsManager> 
    {
        public static IAnalyticsManager Current => Instance.Manager;
    }
}