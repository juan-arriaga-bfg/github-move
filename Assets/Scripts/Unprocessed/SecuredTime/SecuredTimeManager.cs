// #define DEBUG_FORCE_USE_STANDARD_DATETIME // Uncomment to replace return values for Now and UTCNow with standard DateTime.Now and DateTime.UTCNow calls
using Debug = IW.Logger;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.ObscuredTypes;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SecuredTimeManager : ISecuredTimeManager
{
    public enum SyncState
    {
        NotSynced,
        SyncOk,
        SyncInProgress,
        SyncFailed
    }

    public SyncState State { get; private set; } = SyncState.NotSynced;

    private List<IServerTimeProvider> timeProviders = new List<IServerTimeProvider>();
    
#if DEBUG
    /// <summary>
    /// Use only for debug purpose. This flag will disable server connection, the same as DEBUG_FORCE_USE_STANDARD_DATETIME define
    /// </summary>
    public bool ForceUseStandardDateTime
    {
        get
        {
            #if UNITY_EDITOR
            return !EditorPrefs.GetBool("DEBUG_SECURE_TIMER", true);
            #endif
            return !ObscuredPrefs.GetBool("DEBUG_SECURE_TIMER", true);
        }
    }
#endif
        
    private const string SAVE_KEY_MONOTONIC_TIME = "SecuredTime_MonotonicTime";
    private const string SAVE_KEY_SERVER_TIME    = "SecuredTime_ServerTime";
    private const string SAVE_KEY_LOCAL_TIME     = "SecuredTime_LocalTime";

    public ISecuredTimeManager AddServerTimeProvider(IServerTimeProvider provider)
    {
        timeProviders.Add(provider);
        return this;
    }
    
    public SecuredTimeManager()
    {
        if (!IsSavedDataExists())
        {
            State = SyncState.NotSynced;
            ClearSave();
            return;
        }
        
        try
        {
            securedMonotonicTime = long.Parse(ObscuredPrefs.GetString(SAVE_KEY_MONOTONIC_TIME));
            securedServerTime    = long.Parse(ObscuredPrefs.GetString(SAVE_KEY_SERVER_TIME));
            securedLocalTime     = long.Parse(ObscuredPrefs.GetString(SAVE_KEY_LOCAL_TIME));

            Init(null, false);
            
            Debug.Log("SecuredTime: Loaded from Prefs");
        }
        catch (Exception e)
        {
            State = SyncState.NotSynced;
            ClearSave();
            Debug.LogError("SecuredTime: Can't load settings from Prefs: " + e.Message);
        }
    }

    private bool IsSavedDataExists()
    {
        if (!ObscuredPrefs.HasKey(SAVE_KEY_MONOTONIC_TIME)
         || !ObscuredPrefs.HasKey(SAVE_KEY_SERVER_TIME)
         || !ObscuredPrefs.HasKey(SAVE_KEY_LOCAL_TIME)
        )
        {
            return false;
        }

        return true;
    }

    private void Save()
    {
        ObscuredPrefs.SetString(SAVE_KEY_MONOTONIC_TIME, securedMonotonicTime.ToString());
        ObscuredPrefs.SetString(SAVE_KEY_SERVER_TIME,    securedServerTime.ToString());
        ObscuredPrefs.SetString(SAVE_KEY_LOCAL_TIME,     securedLocalTime.ToString());
        ObscuredPrefs.Save();
    }
    
    private void ClearSave()
    {
        ObscuredPrefs.DeleteKey(SAVE_KEY_MONOTONIC_TIME);
        ObscuredPrefs.DeleteKey(SAVE_KEY_SERVER_TIME);
        ObscuredPrefs.DeleteKey(SAVE_KEY_LOCAL_TIME);
        ObscuredPrefs.Save();
    }
    
#if UNITY_IOS || UNITY_EDITOR_OSX
	[DllImport("__Internal")]
    private static extern long SecuredTime_GetMonotonicTime();
#endif

    private long securedMonotonicTime;
    private long securedServerTime;
    private long securedLocalTime;

    public bool IsSyncedWithServer => State == SyncState.SyncOk;

    public void Init(Action<bool> onComplete, bool shouldSyncWithServer)
    {
        if (shouldSyncWithServer)
        {
            SyncWithServer(onComplete);
            return;
        }

        bool isOk = true;

        do
        {
            if (!IsSavedDataExists())
            {
                Debug.Log("No saved data, sync with server is required");
                isOk = false;
                break;
            }
            
            long nowMonotonic = GetMonotonicTime();
            long diffMonotonic = nowMonotonic - securedMonotonicTime;

            DateTime localDateTime = DateTime.UtcNow;
            
            if (localDateTime.Year < 2000 || localDateTime.Year > 2100)
            {
                Debug.LogError("SecuredTime: Init: localDateTime.Year < 2000 || localDateTime.Year > 2100");
                isOk = false;
                break;
            }
            
            long localTime = UnixTimeHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);
            long diffLocalTime = localTime - securedLocalTime;
            
            long diffLocalTimeAndMonotonic = (long) Mathf.Abs(diffLocalTime - diffMonotonic / 1000);

            var MAX_DIFF_SECONDS = 60;

            Debug.Log($"SecuredTime: Init:\n" +
                      $"securedMonotonicTime: {securedMonotonicTime}\n" +
                      $"nowMonotonic        : {nowMonotonic}\n" +
                      $"diffMonotonic (ms)  : {diffMonotonic}\n\n" +
                      $"securedLocalTime    : {securedLocalTime}\n" +
                      $"localTime           : {localTime}\n" +
                      $"diffLocalTime (s)   : {diffLocalTime}\n\n" +
                      $"diffLocalTimeAndMonotonic: {diffLocalTimeAndMonotonic}\n" +
                      $"MAX_DIFF_SECONDS         : {MAX_DIFF_SECONDS}\n"
                );
            
            if (diffMonotonic < 0)
            {
                Debug.LogError("SecuredTime: Init: diffMonotonic < 0");
                isOk = false;
                break;
            }

            if (diffLocalTime < 0)
            {
                Debug.LogError("SecuredTime: Init: diffTime < 0");
                isOk = false;
                break;
            }

            if (diffLocalTimeAndMonotonic > MAX_DIFF_SECONDS)
            {
                Debug.LogError($"SecuredTime: Init: diffTimeAndMonotonic > MAX_DIFF ({MAX_DIFF_SECONDS})");
                isOk = false;
                break;
            }

        } while (false);

        if (isOk)
        {
            State = SyncState.SyncOk;
        }
        else
        {
            ClearSave();
            State = SyncState.NotSynced;
        }
        
        onComplete?.Invoke(isOk);
    }
    
    private void SyncWithServer(Action<bool> onComplete, int providerIndex = 0)
    {
        State = SyncState.SyncInProgress;

        var provider = providerIndex < timeProviders.Count ? timeProviders[providerIndex] : null;
        if (provider == null)
        {
            State = SyncState.SyncFailed;
            onComplete(false);
            return;
        }
        
        provider.GetServerTime((isOk, unixTimeOnServer) =>
        {
            if (isOk)
            {
                securedMonotonicTime = GetMonotonicTime();
                securedServerTime = unixTimeOnServer;
                securedLocalTime = UnixTimeHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);
                
                State = SyncState.SyncOk;
                
                Save();
                
                onComplete(true);
                return;
            }

            // Try with the next provider
            SyncWithServer(onComplete, providerIndex + 1);
        });
    }

    private long GetMonotonicTime()
    {
#if UNITY_EDITOR
        return GetMonotonicTimeEditor();
#elif UNITY_ANDROID
        return GetMonotonicTimeAndroid();
#elif UNITY_IOS
        return GetMonotonicTimeIos();
#else
        throw new NotImplementedException();
#endif
    }
   
#if UNITY_EDITOR
    private long GetMonotonicTimeEditor()
    {
        return Environment.TickCount;
    }
#else
    #if UNITY_ANDROID
    private long GetMonotonicTimeAndroid()
    {
        long result;
	    
        using (AndroidJavaClass cls = new AndroidJavaClass("com.neskinsoft.securedtime.SecuredTime"))
        {
            result = cls.CallStatic<long>("GetMonotonicTime"); 
        }

        return result;
    }
    #endif

    #if UNITY_IOS
    private long GetMonotonicTimeIos()
    {
        return SecuredTime_GetMonotonicTime();
    }
    #endif 
#endif

    public DateTime UtcNow
    {
        get
        {
            {
#if DEBUG_FORCE_USE_STANDARD_DATETIME
                return DateTime.UtcNow;
#endif
#if DEBUG
                if (ForceUseStandardDateTime)
                {
                    return DateTime.UtcNow; 
                }
#endif                
                long currentTicks = GetMonotonicTime();
                long elapsedTicks = currentTicks - securedMonotonicTime;

                DateTime securedDateTime = UnixTimeHelper.UnixTimestampToDateTime(securedServerTime);

                DateTime utcNow = securedDateTime.AddMilliseconds(elapsedTicks);
                return utcNow;
            }
        }
    }
    
    public DateTime Now
    {
        get
        {
            {
#if DEBUG_FORCE_USE_STANDARD_DATETIME
                return DateTime.Now;
#endif
#if DEBUG
                if (ForceUseStandardDateTime)
                {
                    return DateTime.Now; 
                }
#endif     
                var utcNow = UtcNow;
                var localNow = utcNow.ToLocalTime();
                return localNow;
            }
        }
    }
}