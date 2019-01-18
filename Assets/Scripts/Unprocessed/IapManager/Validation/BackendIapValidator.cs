using System;
using System.Net.Sockets;
using BestHTTP;
using BestHTTP.Forms;
using UnityEngine;

public class BackendIapValidator : IIapValidator
{
    const string RESPONSE_GENUINE = "genuine";
    const string RESPONSE_FAKE = "fake";
    const string RESPONSE_ERROR = "error";

    const string PLATFORM_ANDROID = "android";
    const string PLATFORM_IOS = "ios";
    const string PLATFORM_EDITOR = "editor";

    public string Endpoint { get; private set; }

    public BackendIapValidator SetEndpoint(string endpoint)
    {
        Endpoint = endpoint;
        return this;
    }

    private string GetPlatform()
    {
#if UNITY_EDITOR
      return PLATFORM_EDITOR;
#endif
#if UNITY_IOS
      return PLATFORM_IOS;
#endif
#if UNITY_ANDROID
      return PLATFORM_ANDROID;
#endif
    }

    private void LogRequestError(string text)
    {
        Debug.LogWarning($"[BackendIapValidator] => Validate: HTTPRequest response parsing: {text}");
    }
    
    public void Validate(string productId, string receipt, Action<IapValidationResult> onComplete)
    {
        string url = $"{Endpoint}?productid={productId}&platform={GetPlatform()}";
        
        HTTPRequest request = new HTTPRequest(new Uri(url), HTTPMethods.Post, (req, res) => ParseResponse(req, res, onComplete));
        request.Timeout = TimeSpan.FromSeconds(30);
        
        request.AddField("receipt", receipt);
        request.FormUsage = HTTPFormUsage.Multipart;
        
        request.Send();
    }

    private static (bool isOk, bool isConnectionError, string errorText) ValidateResponse(HTTPRequest req, HTTPResponse res)
    {
        bool isOk = false;
        bool isConnectionError = false;
        string errorText;

        try
        {
            // VALIDATE REQUEST

            // Request should exists
            if (req == null)
            {
                throw new Exception("Request is null");
            }

            // Request should be completed without exceptions
            if (req.Exception != null)
            {
                if (req.Exception is TimeoutException || req.Exception is SocketException)
                {
                    isConnectionError = true;
                }

                throw req.Exception;
            }

            // Request should be finished
            if (req.State != HTTPRequestStates.Finished)
            {
                isConnectionError = true;
                throw new Exception("Http request failed: " + req.State);
            }

            // VALIDATE RESPONSE

            if (res == null)
            {
                isConnectionError = true;
                throw new Exception("No response");
            }

            if (!res.IsSuccess)
            {
                isConnectionError = true;
                throw new Exception($"Response code: {res.StatusCode}");
            }

            // Cool! All is ok!
            isOk = true;
            errorText = null;
        }
        catch (Exception e)
        {
            errorText = e.Message;
        }

        return (isOk, isConnectionError, errorText);
    }
     
    private void ParseResponse(HTTPRequest req, HTTPResponse res, Action<IapValidationResult> onComplete)
    {
        try
        {
            (bool isOk, bool isConnectionError, string errorText) = ValidateResponse(req, res);
            if (isConnectionError)
            {
                LogRequestError(errorText);
                onComplete(IapValidationResult.ConnectionError);
                return;
            }
            
            if (!isOk)
            {
                LogRequestError(errorText);
                onComplete(IapValidationResult.ValidationError);
                return;
            }
            
            string responseStr = res.DataAsText;
            switch (responseStr)
            {
                case RESPONSE_GENUINE:
                    onComplete(IapValidationResult.Genuine);
                    return;

                case RESPONSE_ERROR:
                    onComplete(IapValidationResult.ValidationError);
                    return;

                case RESPONSE_FAKE:
                    onComplete(IapValidationResult.Fake);
                    return;

                default:
                    throw new Exception($"Unknown response: {responseStr}");
            }
        }
        catch (Exception e)
        {
            LogRequestError(e.Message);
            onComplete(IapValidationResult.ValidationError);
            return;
        }
    }
}