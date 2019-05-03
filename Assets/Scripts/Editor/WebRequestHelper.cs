
using System.Net;
using System.Net.Sockets;
#if UNITY_EDITOR
using System;
using Debug = IW.Logger;
using BestHTTP;

namespace Dws
{
    /// <summary>
    /// Async http requests wrapper
    /// </summary>
    public static class WebHelper
    {
        static WebHelper()
        {
            HTTPManager.Setup();
        }

        public delegate void WebRequestCallback(string error, string response);

        /// <summary>
        /// Make http request within the Editor and only in edit mode. Do not use in a Play mode!
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="callback"></param>
        public static void MakeRequest(string url, WebRequestCallback callback)
        {
            Debug.Log("WebHelper: MakeRequest: " + url);

            HTTPRequest httpRequest = new HTTPRequest(new Uri(url), (req, res) =>
            {
                bool IsConnectionError = false;

                try
                {
                    if (res == null)
                    {
                        IsConnectionError = true;
                        throw new WebException("No response received");
                    }
                    
                    if (req.Exception != null)
                    {
                        if (req.Exception is TimeoutException || req.Exception is SocketException)
                        {
                            IsConnectionError = true;
                            throw req.Exception;
                        }

                        throw req.Exception;
                    }

                    if (req.State != HTTPRequestStates.Finished)
                    {
                        IsConnectionError = true;
                        throw new Exception("Http request failed: " + req.State);
                    }
                    
                    if (!res.IsSuccess)
                    {
                        throw new Exception("Http request failed: " + res.StatusCode);
                    }

                    string result = res.DataAsText;
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new Exception("Http request failed: response as text is null or empty");
                    }
                    
                    // Debug.Log("WebHelper: MakeRequest: Return: " + resultText);

                    callback(null, result);
                }
                catch (Exception e)
                {
                    Debug.LogError(IsConnectionError 
                        ? $"Request failed: Connection issue: {e.Message}" 
                        : $"Request failed: {e.Message}");

                    callback("Failed", null);
                }
            });

            httpRequest.Timeout = TimeSpan.FromSeconds(120);
            httpRequest.MethodType = HTTPMethods.Get;
            httpRequest.Send();
        }
    }
}


#endif