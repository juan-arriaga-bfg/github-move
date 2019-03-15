using Debug = IW.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using Newtonsoft.Json;
using UnityEngine;

public class IapManager : ECSEntity, IIapManager
{
    private const string IAP_PRICE_CACHE_KEY = "IAP_PRICE_CACHE_KEY";

    public static readonly int ComponentGuid = ECSManager.GetNextGuid();

    public override int Guid => ComponentGuid;

    private IapProvider iapProvider;

    private readonly List<IIapValidator> validators = new List<IIapValidator>();

    private readonly List<Action> validationCompleteListners = new List<Action>();
    private readonly HashSet<string>validationInProgressIds = new HashSet<string>();
    
    public IapManager AddIapValidator(IIapValidator validator)
    {
        validators.Add(validator);
        return this;
    }

    private bool treatValidateConnectionErrorsAsOk = true;
    
    /// <summary>
    /// In case when ANY validator throws exception or can execute check (e.g. no internet for server side validation)
    /// if FALSE you got OnPurchaseFailed - more efficient against hackers but may be annoying for fair players
    /// if TRUE you got OnPurchaseSuccess - be loyal both to fair players and hacker 
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IapManager TreatValidateConnectionErrorsAsOk(bool value)
    {
        treatValidateConnectionErrorsAsOk = value;
        return this;
    }

    private Dictionary<string, string> iapPriceCache;

    private Dictionary<string, PendingIap> pendingIaps = new Dictionary<string, PendingIap>();
    public Dictionary<string, PendingIap> PendingIaps => pendingIaps;

    public IapCollection IapCollection => iapProvider?.IapCollection; 
    
    public bool IsInitialized { get; private set; }
    
    public IapErrorCode LastInitError { get; private set; }

    public delegate void OnPurchaseOkDelegate(string productId, string receipt, bool restore);

    public OnPurchaseOkDelegate OnPurchaseOK;

    public delegate void OnPurchaseFailDelegate(string productId, IapErrorCode error);

    public OnPurchaseFailDelegate OnPurchaseFail;
    
    public delegate void OnRestoreCompletedDelegate(bool isOK);

    public OnRestoreCompletedDelegate OnRestoreCompleted;

    private bool providerInitInProgress;
    
    /// <summary>
    /// Use this to add your analytics send method.
    /// Note that this will be raised even when receipt validation was not completed due to error.
    /// Check validated flag if it is important
    /// </summary>
    /// <param name="productId">purchased product</param>
    /// <param name="validated">is </param>
    public delegate void OnAnalyticsTimeDelegate(string productId, bool validated);

    public OnAnalyticsTimeDelegate OnAnalyticsTime;

    public Action OnInitialized;

    private bool isDestroyed;
    
    public IapManager SetIapProvider(IapProvider iapProviderInstance)
    {
        iapProvider = iapProviderInstance;

        iapProvider.OnPurchaseFail += OnPurchaseFailCallback;
        iapProvider.OnPurchaseOK += OnPurchaseOkCallback;
        iapProvider.OnRestoreCompleted += OnRestoreCompletedCallback;

        return this;
    }

    private void OnPurchaseOkCallback(string productId, string receipt, bool restore)
    {
        Debug.Log($"[IapManager] => OnPurchaseOkCallback: productId: '{productId}', restore: {restore}, with receipt:\n{receipt}");
        
        string storeId = iapProvider.IapCollection.GetStoreId(productId);

        Validate(storeId, receipt, (result) =>
        {
            HashSet<IapValidationResult> okResults = new HashSet<IapValidationResult>
            {
                IapValidationResult.Genuine,
                IapValidationResult.ValidationError,
            };

            HashSet<IapValidationResult> cheaterResults = new HashSet<IapValidationResult>
            {
                IapValidationResult.Fake,
            };

            HashSet<IapValidationResult> requireRevalidationResults = new HashSet<IapValidationResult>
            {

            };

            if (treatValidateConnectionErrorsAsOk)
            {
                okResults.Add(IapValidationResult.ConnectionError);
            }
            else
            {
                requireRevalidationResults.Add(IapValidationResult.ConnectionError);
            }

            var pendingIap = new PendingIap
            {
                Receipt = receipt,
                StoreId = storeId,
                Uid = new Guid().ToString()
            };

            // PROVIDE
            if (okResults.Contains(result))
            {
                pendingIap.Validated = true;
                pendingIaps.Add(productId, pendingIap);

                OnPurchaseOK.Invoke(productId, receipt, restore);
                return;
            }

            // HACKER
            if (cheaterResults.Contains(result))
            {
                OnPurchaseFail.Invoke(productId, IapErrorCode.ServerValidationFailCheatingDetected);
                return;
            }
            
            // REVALIDATE
            if (requireRevalidationResults.Contains(result))
            {
                pendingIap.Validated = false;
                pendingIaps.Add(productId, pendingIap);
                OnPurchaseFail.Invoke(productId, IapErrorCode.ServerValidationFailConnectionDown);
                return;
            }
            
            throw new ArgumentOutOfRangeException(nameof(result), result, null);
        });
    }
    private void OnPurchaseFailCallback(string productId, IapErrorCode error)
    {
        Debug.Log($"[IapManager] => OnPurchaseFailCallback: productId: '{productId}' with error: {error}");
        OnPurchaseFail?.Invoke(productId, error);
    }
    
    private void OnRestoreCompletedCallback(bool isOk)
    {
        OnRestoreCompleted?.Invoke(isOk);
    }

    public void Init()
    {
        if (ProfileService.Current == null)
        {
            throw new Exception("[IapManager] => Init: ProfileService.Current is null. Please init IapManager after ProfileService");
        }
        
        if (iapProvider == null)
        {
            throw new Exception("[IapManager] => InitProvider: IapProvider is not set.");
        }
        
        LoadPriceCache();
        LoadPendingIaps();
        
        InitProvider();
    }

    private void LoadPendingIaps()
    {
        var save = ProfileService.Current.GetComponent<PendingIapSaveComponent>(PendingIapSaveComponent.ComponentGuid);
        pendingIaps = save?.PendingIaps ?? new Dictionary<string, PendingIap>();
        
        Debug.Log($"[IapManager] => LoadPendingIaps: {(pendingIaps.Count > 0 ? string.Join(" | ", pendingIaps.Keys.ToList()) : "No pending iaps")}");
    }

    private void InitProvider(Action<IapErrorCode> onComplete = null)
    {
        if (IsInitialized)
        {
            onComplete?.Invoke(IapErrorCode.NoError);
            return;
        }

        Debug.Log($"[IapManager] => InitProvider...");

        providerInitInProgress = true;
        
        iapProvider.Init((error) =>
        {
            providerInitInProgress = false;
            
            LastInitError = error;
            
            if (error != IapErrorCode.NoError)
            {
                Debug.LogError($"[IapManager] => InitProvider failed with error: {error.ToString()}");

                onComplete?.Invoke(error);
                return;
            }

            InitPriceCache();

            Debug.Log($"[IapManager] => Inited!");
            
            IsInitialized = true;

            LastInitError = IapErrorCode.NoError;
            
            OnInitialized?.Invoke();

            onComplete?.Invoke(IapErrorCode.NoError);
        });
    }

    private void InitPriceCache()
    {
        iapPriceCache = new Dictionary<string, string>();

        foreach (var item in iapProvider.IapCollection.Defs)
        {
            var priceStr = iapProvider.GetLocalizedPriceStr(item.Id);
            if (string.IsNullOrEmpty(priceStr))
            {
#if !UNITY_EDITOR
                Debug.LogError($"[IapManager] => InitPriceCache: price is null for {item.Id}");
#endif
                continue;
            }

            iapPriceCache.Add(item.Id, priceStr);
        }

        SavePriceCache();
    }

    private void LoadPriceCache()
    {
        iapPriceCache = new Dictionary<string, string>();

        var iapPriceCacheStr = ObscuredPrefs.GetString(IAP_PRICE_CACHE_KEY, null);
        if (string.IsNullOrEmpty(iapPriceCacheStr))
        {
            return;
        }

        try
        {
            iapPriceCache = JsonConvert.DeserializeObject<Dictionary<string, string>>(iapPriceCacheStr);
        }
        catch (Exception e)
        {
            Debug.LogError($"[IapManager] => LoadPriceCache FAIL: {e.Message}, cache content: {iapPriceCacheStr}");
        }
    }

    private void SavePriceCache()
    {
        if (iapPriceCache == null || iapPriceCache.Count == 0)
        {
            ObscuredPrefs.DeleteKey(IAP_PRICE_CACHE_KEY);
            return;
        }

        string serialized = JsonConvert.SerializeObject(iapPriceCache);
        ObscuredPrefs.SetString(IAP_PRICE_CACHE_KEY, serialized);
    }

    public void Purchase(string productId)
    {
        Debug.Log($"[IapManager] => Purchase: '{productId}'...");
        
        if (isDestroyed)
        {
            Debug.Log($"[IapManager] => Purchase: Can't start: isDestroyed: {isDestroyed}");
            OnPurchaseFail?.Invoke(productId, IapErrorCode.PurchaseFailIapPrviderNotInitialized);
            return;
        }
        
        if (providerInitInProgress)
        {
            Debug.Log($"[IapManager] => Purchase: Can't start: providerInitInProgress: {providerInitInProgress}");
            OnPurchaseFail?.Invoke(productId, IapErrorCode.PurchaseFailIapPrviderNotInitialized);
            return;
        }
        
        if (!IsInitialized)
        {
            Debug.Log($"[IapManager] => Purchase: Can't start: IsInitialized: {IsInitialized}");
            OnPurchaseFail?.Invoke(productId, IapErrorCode.PurchaseFailIapPrviderNotInitialized);
            return;
        }

        InitProvider((error) =>// todo: no sense to call this until we check IsInitialized above
        {
            if (error == IapErrorCode.NoError)
            {
                // This product already purchased
                if (pendingIaps.ContainsKey(productId))
                {
                    Debug.Log($"[IapManager] => Purchase: '{productId}' is in pendingIaps list");
                    ProcessPendingIapInsteadOfPurchase(productId);
                }
                else
                {
                    Debug.Log($"[IapManager] => Purchase: Call iapProvider");
                    iapProvider.Purchase(productId);
                }
            }
            else
            {
                Debug.Log($"[IapManager] => Purchase: OnPurchaseFail: {productId} with {error}");
                OnPurchaseFail?.Invoke(productId, error);
            }
        });
    }

    public void RestorePurchases()
    {
        Debug.Log($"[IapManager] => RestorePurchases...");
        
        if (!IsInitialized || isDestroyed)
        {
            Debug.Log($"[IapManager] => RestorePurchases failed: !IsInitialized || isDestroyed");
            OnRestoreCompleted?.Invoke(false);
            return;
        }
        
        iapProvider.RestorePurchases();
    }

    private void ProcessPendingIapInsteadOfPurchase(string productId)
    {
        Debug.Log($"[IapManager] => ProcessPendingIapInsteadOfPurchase: {productId}");
        
        var pendingIap = pendingIaps[productId];
        if (pendingIap.Validated)
        {
            OnPurchaseOK?.Invoke(productId, pendingIap.Receipt, false);
            return;
        }

        // ...  but not validated yet. Do not start new purchase flow, just revalidate
        Debug.Log($"[IapManager] => Iap {productId} already purchased but not validated. Retry validation...");

        ValidatePendingIaps(() =>
        {
            // Iap was deleted as FAKE
            if (!pendingIaps.TryGetValue(productId, out PendingIap validatedPendingIap))
            {
                OnPurchaseFail?.Invoke(productId, IapErrorCode.ServerValidationFailCheatingDetected);
            }
            else
            {
                if (validatedPendingIap.Validated)
                {
                    OnPurchaseOK?.Invoke(productId, validatedPendingIap.Receipt, false);
                }
                else
                {
                    OnPurchaseFail?.Invoke(productId, IapErrorCode.ServerValidationFailConnectionDown);
                }
            }
        });
    }

    public string GetLocalizedPriceStr(string productId)
    {
        // Directly from store
        string ret = null;

        if (IsInitialized)
        {
            try
            {
                ret = iapProvider.GetLocalizedPriceStr(productId);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        // Else try get from the cache
        if (ret == null)
        {
            iapPriceCache?.TryGetValue(productId, out ret);
        }

        // Else use default value
        if (ret == null)
        {
            var item = iapProvider.IapCollection.Defs.FirstOrDefault(e => e.Id == productId);
            if (item != null)
            {
                ret = item.DefaultPrice;
            }
        }

        return ret;
    }
    
    public long GetPriceAsNumber(string productId)
    {
        // todo: add to cache
        // Directly from store
        if (IsInitialized)
        {
            return iapProvider.GetPriceAsNumber(productId);
        }

        return -1;
    }

    /// <summary>
    /// Remove iap from pending iaps list
    /// </summary>
    /// <param name="id"></param>
    public void IapProvidedToPlayer(string id)
    {
        pendingIaps.Remove(id);
    }

    /// <summary>
    /// Validate iaps (it's validation were failed recently)
    /// </summary>
    /// <param name="onComplete">Callback</param>
    public void ValidatePendingIaps(Action onComplete)
    {
        if (!IsInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        var ids = pendingIaps
                  .Where(e => e.Value.Validated == false && !validationInProgressIds.Contains(e.Key))
                  .Select(e => e.Key).ToList();

        // Nothing to check and no validation in progress, immediate exit!
        if (ids.Count == 0 && validationInProgressIds.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }
        
        validationCompleteListners.Add(onComplete);
        
        // Nothing new to check. Just wait for complete!
        if (ids.Count == 0)
        {
            return;
        }

#if DEBUG
        Debug.Log($"[ValidatePendingIaps] => Schedule validation: {string.Join("\n", ids)}");
#endif

        foreach (var productId in ids)
        {
            validationInProgressIds.Add(productId);
        }

        foreach (var productId in ids)
        {
            PendingIap pendingIap = pendingIaps[productId];

            Validate(pendingIap.StoreId, pendingIap.Receipt, result =>
            {
                string id = productId;

                validationInProgressIds.Remove(id);

                if (!pendingIaps.TryGetValue(id, out PendingIap validatedPendingIap))
                {
                    Debug.LogWarning($"[ValidatePendingIaps] => Product id {id} removed from pending iaps list while validation was in progress");
                }
                else
                {
                    switch (result)
                    {
                        case IapValidationResult.Fake:
                            Debug.LogWarning($"[ValidatePendingIaps] => Product id {id} is FAKE. Removing from pending iaps list.");
                            pendingIaps.Remove(id);
                            break;

                        case IapValidationResult.Genuine:
                        case IapValidationResult.ValidationError:
                            validatedPendingIap.Validated = true;
                            Debug.Log($"[ValidatePendingIaps] => Product id {id} is GENUINE with server response: {result}");
                            break;

                        case IapValidationResult.ConnectionError:
                            // Try again later...
                            Debug.LogWarning($"[ValidatePendingIaps] => Product id {id} validation failed because of connection error.");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(result), result, null);
                    }
                }

                Debug.Log($"[ValidatePendingIaps] => Remaining items: {validationInProgressIds.Count}");

                if (validationInProgressIds.Count == 0)
                {
                    foreach (var callback in validationCompleteListners)
                    {
                        try
                        {
                            callback.Invoke();
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[ValidatePendingIaps] => Exception on executing callback: {e.Message}");
                        }
                    }

                    validationCompleteListners.Clear();
                }
            });
        }
    }

    private void ValidateUsingSpecificValidator(int validatorIndex, string productId, string receipt, Action<IapValidationResult> onComplete)
    {
        IIapValidator validator = validators[validatorIndex];

        Debug.LogWarning($"[IapManager] => ValidateUsingSpecificValidator: {validator.GetType()}...");

        validator.Validate(productId, receipt, result =>
        {
            Debug.LogWarning($"[IapManager] => ValidateUsingSpecificValidator: {validator.GetType()} for {productId} returns {result.ToString()}");
            
            switch (result)
            {
                case IapValidationResult.Genuine:
                case IapValidationResult.ValidationError:
                    validatorIndex++;
                    if (validatorIndex >= validators.Count)
                    {
                        onComplete(result);
                    }
                    else
                    {
                        ValidateUsingSpecificValidator(validatorIndex, productId, receipt, onComplete);
                    }

                    break;

                case IapValidationResult.Fake:
                case IapValidationResult.ConnectionError:
                    onComplete(result);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        });
    }

    private void Validate(string productId, string receipt, Action<IapValidationResult> onComplete)
    {
        Debug.Log($"[IapManager] => Validate: productId: '{productId}'");
        
        if (validators == null || validators.Count == 0)
        {
            Debug.LogWarning("[IapManager] => Validate: No validators set");
            onComplete(IapValidationResult.Genuine);
            return;
        }

        ValidateUsingSpecificValidator(0, productId, receipt, onComplete);
    }

    public void CleanUp()
    {
        // todo: break all async operations like validation

        isDestroyed = true;
        
        if (iapProvider != null)
        {
            iapProvider.OnPurchaseFail -= OnPurchaseFailCallback;
            iapProvider.OnPurchaseOK -= OnPurchaseOkCallback;
            iapProvider.OnRestoreCompleted -= OnRestoreCompletedCallback;
            iapProvider.CleanUp();
        }
    }
}