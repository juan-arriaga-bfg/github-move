using System;
using System.Collections.Generic;
using Backend;
using BestHTTP;
using BestHTTP.Forms;
using UnityEngine;
using BFGSDK;

public struct BackendSettings
{
    public string Host;
    public string Protocol;
    public string ClientPlatform;
}

public class NetworkUtils
{
    private static NetworkUtils s_instance;

    BackendSettings backendSettings;
    
    private NetworkUtils()
    {
        BestHTTP.HTTPManager.Setup();

        backendSettings = new BackendSettings
        {
            ClientPlatform = Application.platform.ToString(),
            Protocol = "v3",
            
#if BUILD_QA
            Host = "https://f2p-qa.bigfishgames.com/nodejs101",
#elif BUILD_STAGE
            Host = "https://f2p-st.bigfishgames.com/nodejs10",
#elif BUILD_PROD
            Host = "https://f2p.bigfishgames.com/nodejs10",
#endif
        };

        // Fallback for Unity Editor and for debug overriding
        if (backendSettings.Host == null)
        // if (true)
        {
            backendSettings.Host = "http://127.0.0.1:8080/nodejs101";              // Localhost
            // backendSettings.Host = "http://134.17.4.143/nodejs101";             // Neskinsoft
            // backendSettings.Host = "https://f2p-qa.bigfishgames.com/nodejs101"; // Bfg QA
        }
    }

    public string GetHostUrl()
    {
        return backendSettings.Host;
    }

    public static NetworkUtils Instance => s_instance ?? (s_instance = new NetworkUtils());

    public delegate void PostToBackendCallback(BackendResponse response);

    /// <summary>
    /// Send request to ur backend
    ///  Warning! In case of file sending use HTTPFormUsage.Multipart
    /// </summary>
    /// <param name="endpoint">Wil be used as part of url: http://Host/Protocol/Path  Do not add / at the beginning </param>
    /// <param name="body">Post body</param>
    /// <param name="queryParams">Query string items</param>
    /// <param name="callback">Will be called on completion</param>
    /// <param name="timeOut">Whatchdog time</param>
    /// <param name="method">POST/GET/etc</param>
    public void RequestToBackend(string endpoint, string body, Dictionary<string, string> queryParams, PostToBackendCallback callback, int timeOut = 30, HTTPMethods method = HTTPMethods.Post)
    {
        RequestToBackend(endpoint, body, null, queryParams, callback, timeOut, method);
    }
    
    public void RequestToBackend(string endpoint, byte[] rawData, Dictionary<string, string> queryParams, PostToBackendCallback callback, int timeOut = 30, HTTPMethods method = HTTPMethods.Post)
    {
        RequestToBackend(endpoint, null, rawData, queryParams, callback, timeOut, method);
    }
    
    public void PostToBackend(string endpoint, Dictionary<string, string> queryParams, PostToBackendCallback callback, int timeOut = 30)
    {
        RequestToBackend(endpoint, null, null, queryParams, callback, timeOut);
    }
    
    private void RequestToBackend(string endpoint, string body, byte[] rawData, Dictionary<string, string> queryParams, PostToBackendCallback callback, int timeOut = 30, HTTPMethods method = HTTPMethods.Post)
    {
        if (body != null && rawData != null)
        {
            Debug.LogError($"RequestToBackend: Do not specify both body and rawData");
            return;
        }

        string url = $"{backendSettings.Host}/{backendSettings.Protocol}/{(endpoint.StartsWith("/") ? endpoint.Remove(0, 1) : endpoint)}";
        HTTPRequest request = new HTTPRequest(new Uri(url), method,
            (req, res) =>
            {
                BackendResponse br = ResponseParser.Parse(req, res);
                callback(br);
            });

        var queryParamsAsString = "";

        if (queryParams != null)
        {
            //var encoding = new UTF8Encoding(true);
            foreach (var param in queryParams)
            {
                if (param.Value != null)
                {
                    request.AddField(param.Key, param.Value);
                    queryParamsAsString += (param.Key + "=" + param.Value + "&");
                }
                else
                {
                    IW.Logger.LogWarningFormat("RequestToBackend field {0} == null", param.Key);
                }
            }
            if (queryParamsAsString.EndsWith("&"))
            {
                queryParamsAsString = queryParamsAsString.Substring(0, queryParamsAsString.Length - 1);
            }
        }

        if (body != null)
        {
            request.AddField("body", body);
            request.FormUsage = HTTPFormUsage.Multipart;
        }
        else if (rawData != null)
        {
            request.AddBinaryData("raw", rawData);
        }
        else
        {
            request.FormUsage = HTTPFormUsage.UrlEncoded;
        }

        request.AddHeader("X-Client-Version", IWVersion.Get.CurrentVersion);
        request.AddHeader("X-Client-Protocol", backendSettings.Protocol);
        request.AddHeader("X-Client-Platform", backendSettings.ClientPlatform);

        if (SocialUtils.IsLoggedInRave())
        {
            request.AddHeader("X-User-Id", SocialUtils.SessionUser.UserId);
            request.AddHeader("X-User-Token", SocialUtils.SessionUser.BackendToken ?? "");
        }

        request.Timeout = TimeSpan.FromSeconds(timeOut);
        //request.Proxy = new HTTPProxy(new Uri("http://localhost:8888"));

        IW.Logger.LogFormat("NetworkUtils: RequestToBackend: url: '{0}' params: {1}", url, queryParamsAsString);

        request.Send();
    }
    
    public static bool CheckInternetConnection(bool showErrorDialogAutomatically = false)
    {
#if !UNITY_EDITOR
        if (!bfgManager.checkForInternetConnectionAndAlert(false))
#else
        if (Application.internetReachability == NetworkReachability.NotReachable)
#endif
        {
            if (showErrorDialogAutomatically)
            {
                UIMessageWindowController.CreateNoInternetMessage();
            }

            return false;
        }

        return true;
    } 
}