using System;
using System.Collections.Generic;

public static class SocialUtils
{
    public static bool IsLoggedInRave()
    {
        return true;
    }

    public static class SessionUser
    {
        public static string UserId => ProfileService.Current.ClientID.Replace("-", "");
        public static string BackendToken { get; set; }
    }

    public static void SendProgress(string data)
    {
        bool force = true;
        
        var prms = new Dictionary<string, string>
        {
            {"install_id", ProfileService.Current.ClientID.Replace("-", "") },
            {"force", force.ToString().ToLower()}
        };

        NetworkUtils.Instance.RequestToBackend("cloud-string/set",
            data,
            prms,
            (result) =>
            {
                if (result.IsOk)
                {
                    try
                    {
                        var resultStr = result.ResultAsJson["status"].Value;

                        IW.Logger.Log($"[SocialUtils] => SendProgress: {resultStr}");
                    }
                    catch (Exception e)
                    {
                        IW.Logger.LogError($"[SocialUtils] => SendProgress: {e.GetType()} {e.Message}");
                    }
                }
                else if (result.IsConnectionError)
                {
                    IW.Logger.LogError($"[SocialUtils] => SendProgress: Connection error");
                }
                else
                {
                    IW.Logger.LogError($"[SocialUtils] => SendProgress: {result.ErrorAsText}");
                }
            }
        );
    }
}