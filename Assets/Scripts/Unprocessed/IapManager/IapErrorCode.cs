public enum IapErrorCode
{
    NoError                                   = -1, 
    
    // Initialization errors
    InitFailReasonPurchasingUnavailable       = 1000,// Purchases disabled on this device or user not logged into the store
    InitFailReasonNoProductsAvailable         = 1001,
    InitFailReasonUnknown                     = 1002,
    
    // Purchasing errors
    PurchaseFailReasonPurchasingUnavailable   = 2000,
    PurchaseFailReasonExistingPurchasePending = 2001,
    PurchaseFailReasonProductUnavailable      = 2002,
    PurchaseFailReasonSignatureInvalid        = 2003,
    PurchaseFailReasonUserCancelled           = 2004,// NOT AN ERROR! User just cancelled purchase flow. Must be processed silently.
    PurchaseFailReasonPaymentDeclined         = 2005,
    PurchaseFailReasonDuplicateTransaction    = 2006,
    PurchaseFailReasonUnknown                 = 2007,
    
    PurchaseFailSomePurchaseAlreadyInProgress = 3000,
    PurchaseFailIapPrviderNotInitialized      = 3001,
    PurchaseFailNoProductWithIdDefined        = 3002,
    
    ServerValidationFailCheatingDetected      = 4000,//Cheating detected. purchase should be blocked
    ServerValidationFailConnectionDown        = 4001,// Request failed due to connection issue. Should be retried after fixing Internet connectivity.
}