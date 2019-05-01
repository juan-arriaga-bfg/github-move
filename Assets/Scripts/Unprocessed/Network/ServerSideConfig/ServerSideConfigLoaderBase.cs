using System;
using CodeStage.AntiCheat.ObscuredTypes;
using IW.SimpleJSON;

public abstract class ServerSideConfigLoaderBase: IECSComponent
{
    public abstract int Guid { get; }

#if DEBUG
    private int checkInterval = 30;
#else
    private int checkInterval = 60 * 60 * 1;
#endif

    private string url;

    private ServerSideConfigsManager context;

    private bool destroyed;

    private string timeKey;

    private bool isUpdateInProgress;
    
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
                ObscuredPrefs.SetString(timeKey, null);
            }
            else
            {
                IW.Logger.Log($"ServerSideLoaderBase: Update: Skipped by interval: Now: {now}, Saved: {now}, Diff: {diff.TotalSeconds}s, CheckInterval: {checkInterval}");
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
        });
    }

    private delegate void RequestCallback(string error, object data);
    private void SendRequestAsync(RequestCallback callback)
    {
        IW.Logger.Log("ServerSideLoaderBase: SendRequestAsync...");

        if (destroyed)
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

                        callback(null, data);
                        return;
                    }
                    catch (Exception e)
                    {
                        IW.Logger.Log("ServerSideLoaderBase: SendRequestAsync: Error: " + e.GetType() + " " + e.Message);
                    }
                }
                else if (result.IsConnectionError)
                {
                    IW.Logger.Log("ServerSideLoaderBase: SendRequestAsync: Connection error");
                }
                else
                {
                    IW.Logger.Log("ServerSideLoaderBase: SendRequestAsync: Error: " + result.ErrorAsText);
                }
                
                callback("fail", null);
            }
        );
    }

    protected abstract object ParseResponse(JSONNode data);
}