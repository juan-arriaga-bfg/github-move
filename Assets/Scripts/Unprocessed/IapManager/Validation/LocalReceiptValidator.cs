#if UNITY_PURCHASES
using Debug = IW.Logger;
using System;
using UnityEngine;
using UnityEngine.Purchasing.Security;

public class LocalReceiptValidator : IIapValidator
{
    public void Validate(string productId, string receipt, Action<IapValidationResult> onComplete)
    {
        if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(receipt))
        {
            Debug.LogWarning($"[LocalReceiptValidator] => Validate failed because of null or empty params: productId: '{productId}' and/or receipt: {receipt}");
            onComplete(IapValidationResult.ValidationError);
            return;
        }
        
        bool validPurchase = true; // Presume valid for platforms with no R.V.

        // Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
        // Prepare the validator with the secrets we prepared in the Editor
        // obfuscation window.
        
        // If you got compilation error here, in the Unity menu bar, go to Window > Unity IAP > IAP Receipt Validation Obfuscator.
        // Docs: https: //docs.unity3d.com/Manual/UnityIAPValidatingReceipts.html 
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),AppleTangle.Data(), Application.identifier);

        try {
            // On Google Play, result has a single product ID.
            // On Apple stores, receipts contain multiple products.
            IPurchaseReceipt[] result = validator.Validate(receipt);
            
            // For informational purposes, we list the receipt(s)
            Debug.Log($"[LocalReceiptValidator] => Validate {productId}: Receipt is valid with content: ");

            bool isProductIdFound = false;
            foreach (IPurchaseReceipt productReceipt in result) 
            {
                if (productReceipt.productID == productId)
                {
                    isProductIdFound = true;
                }
                
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }

            if (!isProductIdFound)
            {
                Debug.Log($"[LocalReceiptValidator] => Validate {productId}: Receipt is valid but NOT contains requested product id.");
                validPurchase = false;
            }
            
        } catch (IAPSecurityException) {
            Debug.LogWarning($"[LocalReceiptValidator] => Validate {productId}: Receipt INVALID");
            validPurchase = false;
        }
#endif

        onComplete(validPurchase ? IapValidationResult.Genuine : IapValidationResult.Fake);
    }
}

#endif