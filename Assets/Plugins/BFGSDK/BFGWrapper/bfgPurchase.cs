using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SimpleJSON;

namespace BFGSDK
{
    public class bfgPurchase
    {
#if UNITY_EDITOR
        // Nothing to see here.

#elif UNITY_IOS || UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern bool __bfgPurchase__acquireProductInformation();

	[DllImport("__Internal")]
	private static extern bool __bfgPurchase__acquireProductInformation2(string productId);

	// acquireProductInfotmationForProducts:(NSSet *)productIds
	[DllImport("__Internal")]
	private static extern bool __bfgPurchase__acquireProductInformationForProducts(string arrayOfProductIds);

	[DllImport("__Internal")]
	private static extern bool __bfgPurchase__canStartPurchase();

	[DllImport("__Internal")]
	private static extern bool __bfgPurchase__canStartPurchase2(string productId);

	[DllImport("__Internal")]
	private static extern void __bfgPurchase__finishPurchase();

	[DllImport("__Internal")]
	private static extern void __bfgPurchase__finishPurchase2(string productId);

	[DllImport("__Internal")]
	private static extern bool __bfgPurchase__isPurchaseActive();

	[DllImport("__Internal")]
	private static extern bool __bfgPurchase__isPurchaseActive2(string productId);

	[DllImport("__Internal")]
	private static extern bool __bfgPurchase__isRestoreActive();

	// (NSDictionary *)productInformation
	[DllImport("__Internal")]
	private static extern int __bfgPurchase__productInformation(StringBuilder returnDictionary, int size);

	// (NSDictionary *)productInformation:(NSString *)productId
	[DllImport("__Internal")]
	private static extern int __bfgPurchase__productInformation2(string productId, StringBuilder returnDictionary, int size);

	[DllImport("__Internal")]
	private static extern bool __bfgPurchase__purchaseActivityInProgress();

	[DllImport("__Internal")]
	private static extern void __bfgPurchase__restorePurchases();

	[DllImport("__Internal")]
	private static extern void __bfgPurchase__startPurchase();

	[DllImport("__Internal")]
	private static extern void __bfgPurchase__startPurchase2(string productId);

	// startPurchase:(NSString *)productId details1:(NSString *)details1 details2:(NSString *)details2 details3:(NSString *)details3 additionalDetails:(NSDictinoary *)addtionalDetails
	[DllImport("__Internal")]
	private static extern void __bfgPurchase__startPurchase3(string productId, string details1, string details2, string details3, string additionalDetails);

	[DllImport("__Internal")]
	private static extern bool __bfgPurchase__startService();

	[DllImport("__Internal")]
	private static extern bool __bfgPurchase__startService2(StringBuilder error, int size);

    // Big Fish iOS SDK 5.9

	[DllImport("__Internal")]
	private static extern int __bfgPurchase__deliverablePurchases(StringBuilder deliverablePurchasesJson, int size);

#endif

        ///////////////////////


        public static bool acquireProductInformation()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgPurchase__acquireProductInformation();
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("acquireProductInformation");}
			return result;
#else
			return false;
#endif
        }

        public static bool acquireProductInformation(string productId)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgPurchase__acquireProductInformation2(productId);
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("acquireProductInformation", productId);}
			return result;
#else
			return false;
#endif
        }

        public static bool acquireProductInformationForProducts(List<string> productIds)
        {
            string productIdsArrayString = BfgUtilities.ConvertArrayOfStringsToJsonString(productIds);
            if (productIdsArrayString == null)
                productIdsArrayString = "[]";

#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgPurchase__acquireProductInformationForProducts(productIdsArrayString);
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("acquireProductInformationForProducts", productIdsArrayString);}
			return result;
#else
			return false;
#endif
        }

        public static bool canStartPurchase()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgPurchase__canStartPurchase();
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("canStartPurchase");}
			return result;
#else
			return false;
#endif
        }

        public static bool canStartPurchase(string productId)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgPurchase__canStartPurchase2(productId);
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("canStartPurchase", productId);}
			return result;
#else
			return false;
#endif
        }

        public static void finishPurchase()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgPurchase__finishPurchase();
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return;
#endif
        }

        public static void finishPurchase(string productId)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgPurchase__finishPurchase2(productId);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("finishPurchase", productId);}
#else
			return;
#endif
        }

        public static bool isPurchaseActive()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgPurchase__isPurchaseActive();
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("isPurchaseActive");}
			return result;
#else
			return false;
#endif
        }

        public static bool isPurchaseActive(string productId)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgPurchase__isPurchaseActive2(productId);
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("isPurchaseActive", productId);}
			return result;
#else
			return false;
#endif
        }

        public static bool isRestoreActive()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgPurchase__isRestoreActive();
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("isRestoreActive");}
			return result;
#else
			return false;
#endif
        }

        public static bool productInformation(ref string prodInfoJson)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			int sbSize = 1024;
			StringBuilder sb = new StringBuilder(sbSize);

			int requiredSize = __bfgPurchase__productInformation(sb, sbSize);
			if (sbSize <= requiredSize)
			{
				sbSize = requiredSize + 1;
				sb = new StringBuilder(sbSize);
				requiredSize = __bfgPurchase__productInformation(sb, sbSize);
			}

			if (sbSize <= requiredSize)
			{
				return false;
			}

			prodInfoJson = sb.ToString();
			return true;
#elif UNITY_ANDROID
			string result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<string>("productInformation");}
			prodInfoJson = result;
			return true;
#else
			return false;
#endif
        }

        public static bool productInformation(string productId, ref string prodInfoJson)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			int sbSize = 1024;
			StringBuilder sb = new StringBuilder(sbSize);

			int requiredSize = __bfgPurchase__productInformation2(productId, sb, sbSize);
			if (sbSize <= requiredSize)
			{
				sbSize = requiredSize + 1;
				sb = new StringBuilder(sbSize);
				requiredSize = __bfgPurchase__productInformation2(productId, sb, sbSize);
			}

			if (sbSize <= requiredSize)
			{
				return false;
			}

			prodInfoJson = sb.ToString();
			return true;
#elif UNITY_ANDROID
			string result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<string>("productInformation", productId);}
			prodInfoJson = result;
			return true;
#else
			return false;
#endif
        }

        public static bool purchaseActivityInProgress()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgPurchase__purchaseActivityInProgress();
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("purchaseActivityInProgress");}
			return result;
#else
			return false;
#endif
        }

        public static void restorePurchases()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgPurchase__restorePurchases();
#elif UNITY_ANDROID
				//warning This is a deprecated method. Please refer to bfgPurchaseAndroid restorePurchases(), restorePurchases(string sku), or restorePurchases(List<string> skuList) 
                throw new NotImplementedException();
#else
			return;
#endif
        }

        public static void startPurchase()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgPurchase__startPurchase();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("startPurchase");}
#else
			return;
#endif
        }

        public static void startPurchase(string productId)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgPurchase__startPurchase2(productId);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("startPurchase", productId);}
#else
			return;
#endif
        }

        public static void startPurchase(string productId, string details1, string details2, string details3, Dictionary<string, string> additionalDetails)
        {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            string additionalDetailsJsonString = BfgUtilities.ConvertDictionaryOfStringsToJsonString(additionalDetails);
#endif

#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			__bfgPurchase__startPurchase3(productId, details1, details2, details3, additionalDetailsJsonString);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("startPurchase", productId, details1, details2, details3, additionalDetailsJsonString);}
#else
			return;
#endif
        }

        public static bool startService()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgPurchase__startService();
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("startService");}
			return result;
#else
			return false;
#endif
        }

        public static bool startService(ref string error)
        {
            bool success;

#if UNITY_EDITOR
            success = false;
#elif UNITY_IOS || UNITY_IPHONE
       		StringBuilder sb = new StringBuilder(256);
			success = __bfgPurchase__startService2(sb, sb.Capacity);
            error = sb.ToString();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {success = ajc.CallStatic<bool>("startService");}
#else
			success = false;
#endif

            return success;
        }

        // Big Fish iOS SDK 5.9

        public static bool deliverablePurchases(ref string deliverablePurchasesJson)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			int sbSize = 1024;
			StringBuilder sb = new StringBuilder(sbSize);
			int requiredSize = __bfgPurchase__deliverablePurchases(sb, sbSize);
			if (sbSize <= requiredSize)
			{
				sbSize = requiredSize + 1;
				sb = new StringBuilder(sbSize);
				requiredSize = __bfgPurchase__deliverablePurchases(sb, sbSize);
			}

			if (sbSize <= requiredSize)
			{
				return false;
			}

			deliverablePurchasesJson = sb.ToString();
			return true;
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return false;
#endif
        }
    }
}