using System;
using CodeStage.AntiCheat.ObscuredTypes;
using IW.SimpleJSON;
using Newtonsoft.Json;

public enum ServerSideConfigLoaderCacheMode
{
    /// <summary>
    /// No caching. Always ask server for data.
    /// </summary>
    Disabled,
    
    /// <summary>
    /// No request after we got data from the server once
    /// </summary>
    Cache,
    
    /// <summary>
    /// Always ask server for data but return last successful response in case of request fail
    /// </summary>
    Fallback
}

public class ServerSideConfigLoaderCacheItem
{
    public long Timestamp;
    public string Data;
}

public abstract class ServerSideConfigLoaderBase: IECSComponent
{
    public abstract int Guid { get; }

#if DEBUG
    private int checkInterval = 30;
#else
    private int checkInterval = 60 * 60;
#endif

#if DEBUG
    private int cacheLifetime = 30;
#else
    private int cacheLifetime = 60 * 60;
#endif
    
    private string url;

    private ServerSideConfigsManager context;

    private bool destroyed;

    private string timeKey;
    private string cacheKey;

    private bool isUpdateInProgress;

    private ServerSideConfigLoaderCacheMode cacheMode;
    
    public ServerSideConfigLoaderBase SetCacheMode(ServerSideConfigLoaderCacheMode cacheMode)
    {
        this.cacheMode = cacheMode;
        return this;
    }
    
    public ServerSideConfigLoaderBase SetCacheLifetime(int seconds)
    {
        cacheLifetime = seconds;
        return this;
    }
    
    public ServerSideConfigLoaderBase SetCheckInterval(int seconds)
    {
        checkInterval = seconds;
        return this;
    }
    
    public ServerSideConfigLoaderBase SetUrl(string url)
    {
        this.url = url;
        return this;
    }

    public void OnRegisterEntity(ECSEntity entity)
    {
        context = (ServerSideConfigsManager)entity;
        timeKey = $"{GetType()}_timestamp";
        cacheKey = $"{GetType()}_cache";
    }

    public void OnUnRegisterEntity(ECSEntity entity)
    {
        context = null;
        Cleanup();
    }

    private void Cleanup()
    {
        destroyed = true;
    }

    private void SaveToCache(string data)
    {
        var cache = new ServerSideConfigLoaderCacheItem
        {
            Timestamp = UnixTimeHelper.DateTimeToUnixTimestamp(DateTime.UtcNow),
            Data = data
        };

        string serialized = JsonConvert.SerializeObject(cache);
        
        ObscuredPrefs.SetString(cacheKey, serialized);
    }

    private ServerSideConfigLoaderCacheItem LoadFromCache()
    {
        var cacheStr = ObscuredPrefs.GetString(cacheKey, null);
        if (string.IsNullOrEmpty(cacheStr))
        {
            return null;
        }

        try
        {
            ServerSideConfigLoaderCacheItem item = JsonConvert.DeserializeObject<ServerSideConfigLoaderCacheItem>(cacheStr);

            if (cacheLifetime < 0)
            {
                return item;
            }

            long now = UnixTimeHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);
            long timestamp = item.Timestamp;
            long dt = now - timestamp;

            if (Math.Abs(dt) > cacheLifetime)
            {
                IW.Logger.Log($"[{GetType()}] => LoadFromCache: Expired: now: {now}, timestamp: {timestamp}, cacheLifetime: {cacheLifetime}, dt: {dt}");
                ClearCache();
                return null;
            }

            return item;
        }
        catch (Exception e)
        {
            ClearCache();
            IW.Logger.LogError($"[{GetType()}] => LoadFromCache FAIL: {e.Message}");
        }

        return null;
    }

    private void ClearCache()
    {
        ObscuredPrefs.DeleteKey(cacheKey);
    }
    
    public void Update()
    {
        var savedTime = ObscuredPrefs.GetString(timeKey, null);
        
        if (!string.IsNullOrEmpty(savedTime) && long.TryParse(savedTime, out long savedTimeLong))
        {
            var now = DateTime.UtcNow;
            var saved = UnixTimeHelper.UnixTimestampToDateTime(savedTimeLong);
            var diff = now - saved;
            if (diff.TotalSeconds < 0 || diff.TotalSeconds > checkInterval)
            {
                ObscuredPrefs.DeleteKey(timeKey);
            }
            else
            {
                IW.Logger.Log($"[{GetType()}] => Update: Request skipped by interval: Now: {now}, Saved: {now}, Waiting: {(long)(diff.TotalSeconds)}/{checkInterval} seconds");
                
                if (cacheMode == ServerSideConfigLoaderCacheMode.Cache || cacheMode == ServerSideConfigLoaderCacheMode.Fallback)
                {
                    ProvideFromCache((error, data) =>
                    {
                        if (string.IsNullOrEmpty(error))
                        {
                            context.DataReceived(this, data);
                        }
                        else
                        {
                            context.DataRequestFailed(this, error);
                        }
                    });
                }

                return;
            }
        }

        SendRequestAsync((error, data) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                context.DataReceived(this, data);
                ObscuredPrefs.SetString(timeKey, UnixTimeHelper.DateTimeToUnixTimestamp(DateTime.UtcNow).ToString());
            }
            else
            {
                context.DataRequestFailed(this, error);
            }
        });
    }

    private bool ProvideFromCache(RequestCallback callback)
    {
        if (cacheMode == ServerSideConfigLoaderCacheMode.Cache || cacheMode == ServerSideConfigLoaderCacheMode.Fallback)
        {
            ServerSideConfigLoaderCacheItem item = LoadFromCache();
            if (item != null)
            {
                try
                {
                    JSONNode node = JSONNode.Parse(item.Data);
                    object data = ParseResponse(node);
   
                    IW.Logger.Log($"[{GetType()}] => ProvideFromCache: Mode: {cacheMode}");
                    
                    callback(null, data);
                    return true;
                }
                catch (Exception e)
                {
                    ClearCache();
                    IW.Logger.Log($"[{GetType()}] => ProvideFromCache: Parsing error: " + e.GetType() + " " + e.Message);
                }
            }
        }

        return false;
    }

    private delegate void RequestCallback(string error, object data);
    private void SendRequestAsync(RequestCallback callback)
    {
        IW.Logger.Log($"[{GetType()}] => SendRequestAsync...");

        if (destroyed)
        {
            return;
        }

        if (cacheMode == ServerSideConfigLoaderCacheMode.Cache && ProvideFromCache(callback))
        {
            return;
        }
        
        NetworkUtils.Instance.PostToBackend(url,
            null,
            (result) =>
            {
                if (destroyed)
                {
                    return;
                }
                
                if (result.IsOk)
                {
                    try
                    {
                        object data = ParseResponse(result.ResultAsJson);
                       
                        SaveToCache(result.ResultAsText);
                        
                        callback(null, data);
                        return;
                    }
                    catch (Exception e)
                    {
                        IW.Logger.Log($"[{GetType()}] => SendRequestAsync: Error: " + e.GetType() + " " + e.Message);
                    }
                }
                else if (result.IsConnectionError)
                {
                    IW.Logger.Log($"[{GetType()}] => SendRequestAsync: Connection error");
                }
                else
                {
                    IW.Logger.Log($"[{GetType()}] => SendRequestAsync: Error: " + result.ErrorAsText);
                }
                
                if (cacheMode == ServerSideConfigLoaderCacheMode.Fallback && ProvideFromCache(callback))
                {
                    return;
                }
                
                callback("fail", null);
            }
        );
    }

    protected abstract object ParseResponse(JSONNode data);
}