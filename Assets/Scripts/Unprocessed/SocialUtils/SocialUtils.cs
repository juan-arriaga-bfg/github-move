using System;
using System.Collections.Generic;

public static class SocialUtils
{
    public static bool IsLoggedInRave()
    {
        return false;
    }

    public static class SessionUser
    {
        public static string UserId => ProfileService.Current.ClientID;
        public static string BackendToken { get; set; }
    }

    public static void SendProgress(string data)
    {
        // bool force = true;
        //
        // var prms = new Dictionary<string, string>
        // {
        //     {"install_id", ProfileService.Current.ClientID },
        //     {"force", force.ToString().ToLower()}
        // };
        //
        // m_lastUploadTime = DateTime.UtcNow;
        //
        // NetworkUtils.Instance.RequestToBackend("cloud-string/set",
        //     data,
        //     prms,
        //     (result) =>
        //     {
        //         if (result.IsOk)
        //         {
        //             try
        //             {
        //                 var resultStr = result.ResultAsJson["status"].Value;
        //
        //                 SyncControllerUploadRequestStatus status;
        //                 switch (resultStr)
        //                 {
        //                     case "ok created":
        //                     case "ok updated":
        //                     {
        //                         status = SyncControllerUploadRequestStatus.Ok;
        //                         break;
        //                     }
        //                     case "conflict":
        //                     {
        //                         status = SyncControllerUploadRequestStatus.Conflict;
        //                         FillPendingProcessFromResponse(result, ref pendingProgress);
        //                         break;
        //                     }
        //                     default:
        //                         status = SyncControllerUploadRequestStatus.Fail;
        //                         break;
        //                 }
        //
        //                 m_lastError = null;
        //                 callback(status, pendingProgress);
        //             }
        //             catch (Exception e)
        //             {
        //                 Neskinsoft.Core.Logger.Log("SyncController: UploadProgress Error: " + e.GetType() + " " + e.Message);
        //                 callback(SyncControllerUploadRequestStatus.Fail, pendingProgress);
        //                 m_lastError = "sync.error.wrong.server.response";
        //             }
        //         }
        //         else if (result.IsConnectionError)
        //         {
        //             Neskinsoft.Core.Logger.Log("SyncController: UploadProgress Connection error");
        //             callback(SyncControllerUploadRequestStatus.Fail, pendingProgress);
        //             m_lastError = "sync.network.error.message";
        //         }
        //         else
        //         {
        //             Neskinsoft.Core.Logger.Log("SyncController: UploadProgress Error: " + result.ErrorAsText);
        //             callback(SyncControllerUploadRequestStatus.Fail, pendingProgress);
        //             m_lastError = "sync.network.error.message";
        //         }
        //     }
        // );
    }
}