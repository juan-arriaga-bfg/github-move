#if UNITY_EDITOR
using System;
using Debug = IW.Logger;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BestHTTP;

namespace Dws
{
    public struct WebRequestData
    {
        public readonly string Url;
        public readonly string[] Headers;
        public readonly string Method;
        public readonly string Body;

        public WebRequestData(string url, string method = "GET", string[] headers = null, string body = null)
        {
            Url = url;
            Body = body;
            Method = method;
            Headers = headers;
        }
    }

    public struct WebResponseData
    {
        public readonly HttpStatusCode? StatusCode;
        public readonly string Result;
        public readonly string Error;
        public readonly WebExceptionStatus WebExceptionStatus;

        public WebResponseData(HttpStatusCode statusCode, string result, string error)
        {
            StatusCode = statusCode;
            Result = result;
            Error = error;
            WebExceptionStatus = WebExceptionStatus.Success;
        }

        public WebResponseData(string result, string error)
        {
            StatusCode = null;
            Result = result;
            Error = error;
            WebExceptionStatus = WebExceptionStatus.Success;
        }

        public WebResponseData(WebExceptionStatus status)
        {
            StatusCode = null;
            Result = null;
            Error = status.ToString();
            WebExceptionStatus = status;
        }

        public bool IsOk { get { return string.IsNullOrEmpty(Error); } }
    }

    /// <summary>
    /// Async http requests wrapper
    /// </summary>
    public static class WebHelper
    {
        static WebHelper()
        {
            HTTPManager.Setup();
        }
        
        public delegate void WebRequestCallback(WebResponseData response);

        /// <summary>
        /// Make http request within the Editor and only in edit mode. Do not use in a Play mode!
        /// </summary>
        /// <param name="requestData"></param>
        /// <param name="callback"></param>
        public static void MakeRequest(WebRequestData requestData, WebRequestCallback callback)
        {
            //todo: check that method executed only in editor and edit mode! Warning: app.isPlaying will fail here in case of redirect (method will be called recursively from non-ui thread).

            Debug.Log( "WebHelper: MakeRequest: " + requestData.Url);

            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

            Thread t = new Thread(() =>
            {
                try
                {
                    HTTPRequest httpRequest = new HTTPRequest(new Uri(requestData.Url), (request, response) =>
                    {
                        switch (request.State)
                        {
                            case HTTPRequestStates.Finished:
                                // Redirect!
                                // todo: handle redirection loop
                                if ((int) response.StatusCode >= 300 && (int) response.StatusCode <= 399)
                                {
                                    var redirectUrl = response.Headers["Location"];
                                    WebRequestData newRequestData = new WebRequestData(redirectUrl.First(), requestData.Method);
                                    MakeRequest(newRequestData, callback);
                                    return;
                                }

                                var statusCode = (HttpStatusCode)response.StatusCode;

                                EditorMainThreadSync.Execute(() =>
                                {
                                    string resultText = response.DataAsText;
                                    Debug.Log("WebHelper: MakeRequest: Return: " + resultText);
                                    callback(new WebResponseData(statusCode, resultText, null));
                                });
                                break;
                            case HTTPRequestStates.Error:
                                Debug.LogError("Request Finished with Error! " +
                                               (request.Exception != null ?
                                                   (request.Exception.Message + "\n" + request.Exception.StackTrace) :
                                                   "No Exception"));
                                
                                callback(new WebResponseData(null, request.Exception?.Message));
                                break;
                            case HTTPRequestStates.Aborted:
                                Debug.LogWarning("Request Aborted!");
                                callback(new WebResponseData(null, request.State.ToString()));
                                break;
                            case HTTPRequestStates.ConnectionTimedOut:
                                Debug.LogError("Connection Timed Out!");
                                callback(new WebResponseData(null, request.State.ToString()));
                                break;
                            case HTTPRequestStates.TimedOut:
                                Debug.LogError("Processing the request Timed Out!");
                                callback(new WebResponseData(null, request.State.ToString()));
                                break;
                        }
                    });
                    httpRequest.Timeout = TimeSpan.FromSeconds(120);
                    httpRequest.MethodType = HTTPMethods.Get;
                    httpRequest.Send();
                    
                    if (requestData.Headers != null && requestData.Headers.Length > 0)
                    {
                        foreach (var header in requestData.Headers)
                        {
                            if (!header.Contains(":"))
                            {
                                throw new Exception(string.Format("WebHelper: MakeRequest: Header '{0}' should contains ':'", header));
                            }

                            var splitHeader = header.Split(':');
                            httpRequest.AddHeader(splitHeader[0], splitHeader[1]);
                        }
                    }

//                    if (!string.IsNullOrEmpty(requestData.Body))
//                    {
//                        request.ContentType = "application/x-www-form-urlencoded";
//
//                        request.
//                        byte[] postData = Encoding.UTF8.GetBytes(requestData.Body);
//                        request.ContentLength = postData.Length;
//
//                        Stream requestStream = request.GetRequestStream();
//                        requestStream.Write(postData, 0, postData.Length);
//                        requestStream.Close();
//                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.Success)
                    {
                        // Server returns some http error code. Try to read message before fail
                        try
                        {
                            using (var stream = ex.Response.GetResponseStream())
                            using (var reader = new StreamReader(stream))
                            {
                                callback(new WebResponseData(null, reader.ReadToEnd()));
                            }
                        }
                        catch (Exception e)
                        {
                            callback(new WebResponseData(null, ex.Message));
                        }
                    }
                    // Connection failed. Internet down?
                    else
                    {
                        callback(new WebResponseData(ex.Status));
                    }
                }
                catch (Exception e)
                {
                    callback(new WebResponseData(null, e.Message));
                }
            });

            t.Start();
        }

        /// <summary>
        /// Print headers to console. For debug purposes only
        /// </summary>
        /// <param name="response"></param>
        private static void PrintHeadersToLog(HttpWebResponse response)
        {
            var headersAsList = "";
            foreach (var header in response.Headers)
            {
                headersAsList += string.Format("{0} : {1}\n", header, response.Headers[(string) header]);
            }

            Debug.Log(headersAsList);
        }

        /// <summary>
        /// Handle Mono's issue: TlsException: Invalid certificate received from server. http://answers.unity3d.com/questions/792342/how-to-validate-ssl-certificates-when-using-httpwe.html
        /// </summary>
        private static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2) certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }


        /// <summary>
        /// Extention for WEB FORM build
        /// </summary>
        public static void AppendUrlEncoded(this StringBuilder sb, string name, string value)
        {
            if (sb.Length != 0)
            {
                sb.Append("&");
            }
            sb.Append(WWW.EscapeURL(name));
            sb.Append("=");
            sb.Append(WWW.EscapeURL(value));
        }
    }
}


#endif
