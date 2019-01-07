using System;
using System.Globalization;
using BestHTTP;
using DG.Tweening;
using SimpleJSON;
using UnityEngine;

public class BfgServerTimeProvider : IServerTimeProvider
{
    public const float TIMEOUT_SECONDS = 5f;
    
    public string Url { get; private set; }

    public IServerTimeProvider SetUrl(string url)
    {
        Url = url;
        return this;
    }
    
    public void GetServerTime(Action<bool, long> onComplete)
    {
        Debug.Log($"[BfgServerTimeProvider] => GetServerTime...");

        // Uncomment to debug FAIL case
        // DOTween.Sequence()
        //        .AppendInterval(2)
        //        .AppendCallback(() => { onComplete(false, 0); });
        // return;
        
        HTTPRequest r = new HTTPRequest(new Uri(Url), (request, response) =>
        {
            long unixTime = 0;
            bool isOk = true;
            
            try
            {
                Debug.Log($"[BfgServerTimeProvider] => GetServerTime: Request completed with code: {(response != null ? response.StatusCode.ToString() : "No response")}");
                Debug.Log($"[BfgServerTimeProvider] => GetServerTime: Response: {response.DataAsText.ToString()}");
                
                //{"result":{"code":0,"message":"2018-12-20T14:24:48.694Z"}}
                JSONNode n = JSONNode.Parse(response.DataAsText.ToString());
                string dateStr = n["result"]["message"];
                DateTime dateParsed = DateTime.Parse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                unixTime = UnixTimeHelper.DateTimeToUnixTimestamp(dateParsed);
            }
            catch (Exception e)
            {
                Debug.LogError("[BfgServerTimeProvider] => GetServerTime: Can't parse time from server: " + e.Message);
                isOk = false;
            }

            // Debug.Log(unixTime.ToString());

            onComplete(isOk, unixTime);
        });
        r.Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
        r.Send();
    }
}