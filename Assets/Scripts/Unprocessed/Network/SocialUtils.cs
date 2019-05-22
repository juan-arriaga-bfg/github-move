using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BFGSDK;
using CodeStage.AntiCheat.ObscuredTypes;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

public static class SocialUtils
{
    public enum SyncControllerDownloadRequestStatus
    {
        Unknown,
        Same,
        Fail,
        Empty,
        Available,
        Cancelled
    }

    private const string BACKEND_TOKEN = "BACKEND_TOKEN";
    private const string BACKEND_SUPPORT_ID = "BACKEND_SUPPORT_ID";
    private const string BACKEND_USER_ID = "BACKEND_USER_ID";
    private const string BACKEND_IS_LOGGED_IN = "BACKEND_IS_LOGGED_IN";

    private static volatile bool IsRequestInProgress;
    
    public static bool IsLoggedInRave()
    {
        return ObscuredPrefs.GetBool(BACKEND_IS_LOGGED_IN, false);
    }

    public static string GetInstallId()
    {
        return ProfileService.Current.ClientID.Replace("-", "");
    }

    public static class SessionUser
    {
        public static string UserId => IsLoggedInRave() ? ObscuredPrefs.GetString(BACKEND_USER_ID, null) : null;
        public static string BackendToken => IsLoggedInRave() ? ObscuredPrefs.GetString(BACKEND_TOKEN, null) : null;
        public static string SupportId => IsLoggedInRave() ? ObscuredPrefs.GetString(BACKEND_SUPPORT_ID, null) : null;
        public static bool IsLoggedInRave => IsLoggedInRave();
    }

    public static void ArchiveAndSend(string data)
    {
        IW.Logger.Log($"[SocialUtils] => ArchiveAndSend: {data.Length} bytes...");
        
        if (IsRequestInProgress)
        {
            IW.Logger.LogWarning($"SendProgress: ArchiveAndSend: Cancelled by IsRequestInProgress == true");
            return;
        }
        
        if (!IsLoggedInRave())
        {
            LoginAsync((error, token, id) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    ArchiveAndSend(data);
                }
            });
            return;
        }

        IsRequestInProgress = true;

        bool force = true;
        
        var prms = new Dictionary<string, string>
        {
            {"install_id", GetInstallId() },
            {"force", force.ToString().ToLower()}
        };
        
        IW.Logger.Log($"[SocialUtils] => ArchiveAndSend: Running async task...");
        
        byte[] bytes = null;
        bytes = Archive(data);
        SendProgress(bytes, prms);
        
        // todo: async not working on the device if going to background
        
        // Task.Run(() =>
        // {
        //     bytes = Archive(data); // Parallel thread
        // })
        // .GetAwaiter().OnCompleted(() =>
        // {
        //     SendProgress(bytes, prms); // Main thread
        // });

    }

    private static void SendProgress(byte[] bytes, Dictionary<string, string> prms)
    {
        IW.Logger.Log($"[SocialUtils] => SendProgress: RequestToBackend");
        
        NetworkUtils.Instance.RequestToBackend("user-progress/set", bytes, prms, (result) =>
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

            IsRequestInProgress = false;
        }); 
    }

    private static byte[] Archive(string data)
    {
        IW.Logger.Log($"[SocialUtils] => Archiving...");
        
#if DEBUG
        var sw = new Stopwatch();
        sw.Start();
#endif 
        MemoryStream dataStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        MemoryStream zipped = CreateZipToMemoryStream(dataStream, "profile.data.txt");
        var ret = zipped.ToArray();
        
#if DEBUG
        sw.Stop();
        IW.Logger.Log($"[SocialUtils] => SendProgress: Archive: Done in {sw.ElapsedMilliseconds}ms");
#endif

        return ret;
    }

    public delegate void DownloadProgressCallback(SyncControllerDownloadRequestStatus status, string data);
    public static void GetProgress(DownloadProgressCallback callback)
    {
        var prms = new Dictionary<string, string>
        {
            {"install_id", "123"},
            {"key", "progress"}
        };

        NetworkUtils.Instance.PostToBackend("user-progress/get",
            prms,
            (result) =>
            {
                    if (result.IsOk)
                    {
                        try
                        {
                            var statusStr = result.ResultAsJson["status"].Value;
                            switch (statusStr)
                            {
                                case "ok empty":
                                    callback(SyncControllerDownloadRequestStatus.Empty, null);
                                    break;
                                case "ok same":
                                    callback(SyncControllerDownloadRequestStatus.Same, null);
                                    break;
                                case "ok available":
                                    var str = result.ResultAsJson["data"].Value;

                                    byte[] bytes = Convert.FromBase64String(str);

                                    using (var ms = new MemoryStream(bytes))
                                    {
                                        string data = ExtractZipFile(ms);
                                        callback(SyncControllerDownloadRequestStatus.Available, data);
                                    }

                                    break;
                                default:
                                    throw new IndexOutOfRangeException();
                            }
                        }
                        catch (Exception e)
                        {
                            IW.Logger.Log("SyncController: Error: " + e.GetType() + " " + e.Message);
                            callback(SyncControllerDownloadRequestStatus.Fail, null);
                        }
                    }
                    else if (result.IsConnectionError)
                    {
                        IW.Logger.Log("SyncController: Connection error");
                        callback(SyncControllerDownloadRequestStatus.Fail, null);
                    }
                    else
                    {
                        IW.Logger.Log("SyncController: Error: " + result.ErrorAsText);
                        callback(SyncControllerDownloadRequestStatus.Fail, null);
                    }
            });
    }
    
    public static MemoryStream CreateZipToMemoryStream(MemoryStream memStreamIn, string zipEntryName) {

        MemoryStream outputMemStream = new MemoryStream();
        ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

        zipStream.SetLevel(9); //0-9, 9 being the highest level of compression

        ZipEntry newEntry = new ZipEntry(zipEntryName);
        newEntry.DateTime = DateTime.Now;

        zipStream.PutNextEntry(newEntry);

        StreamUtils.Copy(memStreamIn, zipStream, new byte[4096]);
        zipStream.CloseEntry();

        zipStream.IsStreamOwner = false; // False stops the Close also Closing the underlying stream.
        zipStream.Close();               // Must finish the ZipOutputStream before using outputMemStream.

        outputMemStream.Position = 0;
        return outputMemStream;
    }

    public static string ExtractZipFile(MemoryStream ms)
    {
        string ret = null;

        ZipFile zf = null;
        try
        {
            zf = new ZipFile(ms);

            foreach (ZipEntry zipEntry in zf)
            {
                if (!zipEntry.IsFile)
                {
                    continue; // Ignore directories
                }

                // String entryFileName = zipEntry.Name;

                byte[] buffer = new byte[4096]; // 4K is optimum
                Stream zipStream = zf.GetInputStream(zipEntry);

                using (var outputStream = new MemoryStream())
                {
                    StreamUtils.Copy(zipStream, outputStream, buffer);
                    ret = new UTF8Encoding(false).GetString(outputStream.ToArray());
                }
            }
        }
        finally
        {
            if (zf != null)
            {
                zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                zf.Close();              // Ensure we release resources
            }
        }

        return ret;
    }
    
    public delegate void LoginAsyncCallback(string error, string token, string supportId);
    public static void LoginAsync(LoginAsyncCallback callback)
    {
        IW.Logger.Log("LoginRequestAsync...");

        if (IsLoggedInRave())
        {
            IW.Logger.LogWarning($"LoginAsync: Cancelled by IsLoggedInRave == true");
            return;
        }

        if (IsRequestInProgress)
        {
            IW.Logger.LogWarning($"LoginAsync: Cancelled by IsRequestInProgress == true");
            return;
        }

        IsRequestInProgress = true;
        
        string installId = GetInstallId();
        
        var prms = new Dictionary<string, string>
        {
            {"user_id", installId},
            {"social_id", null},
            {"social_token", null},
#if UNITY_EDITOR
            {"name", Environment.MachineName},
#else 
            {"name", SystemInfo.deviceModel},
#endif
            {"email", null},
            {"social_network", null},
            {"platform", null}
        };

        NetworkUtils.Instance.PostToBackend("user/create-or-update",
            prms,
            (result) =>
            {
                IsRequestInProgress = false;
                
                if (result.IsOk)
                {
                    try
                    {
                        var root = result.ResultAsJson;

                        ObscuredPrefs.SetString(BACKEND_TOKEN, root["backendToken"]);

                        string supportId = root["supportId"];
                        ObscuredPrefs.SetString(BACKEND_SUPPORT_ID, root["supportId"]);
                        ObscuredPrefs.SetString(BACKEND_USER_ID, installId);
                        ObscuredPrefs.SetBool(BACKEND_IS_LOGGED_IN, true);

                        if (long.TryParse(supportId, out long supportIdAsLong))
                        {
                            bfgManager.setUserID(supportIdAsLong);
                        }
                        
                        callback(null, root["backendToken"], root["supportId"]);
                        return;
                        
                    }
                    catch (Exception e)
                    {
                        IW.Logger.Log("LoginAsync: Error: " + e.GetType() + " " + e.Message);
                    }
                }
                else if (result.IsConnectionError)
                {
                    IW.Logger.Log("LoginAsync: Connection error");
                }
                else
                {
                    IW.Logger.Log("LoginAsync: Error: " + result.ErrorAsText);
                }

                ObscuredPrefs.DeleteKey(BACKEND_TOKEN);
                ObscuredPrefs.DeleteKey(BACKEND_SUPPORT_ID);
                ObscuredPrefs.DeleteKey(BACKEND_USER_ID);
                ObscuredPrefs.DeleteKey(BACKEND_IS_LOGGED_IN);
                
                callback("fail", null, null);
            },
            10
        );
    }
    
    public delegate void GetGameEventsCallback(string error, object events);
    public static void GetGameEventsAsync(GetGameEventsCallback callback)
    {
        IW.Logger.Log("GetGameEventsAsync...");

        NetworkUtils.Instance.PostToBackend("game-event/get",
            null,
            (result) =>
            {
                if (result.IsOk)
                {
                    try
                    {
                        var root = result.ResultAsJson;

                        callback(null, result.ResultAsText);
                        return;
                        
                    }
                    catch (Exception e)
                    {
                        IW.Logger.Log("GetGameEventsAsync: Error: " + e.GetType() + " " + e.Message);
                    }
                }
                else if (result.IsConnectionError)
                {
                    IW.Logger.Log("GetGameEventsAsync: Connection error");
                }
                else
                {
                    IW.Logger.Log("GetGameEventsAsync: Error: " + result.ErrorAsText);
                }
                
                callback("fail", null);
            }
        );
    }
}