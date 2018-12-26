using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SimpleJSON;


public class bfgPurchaseAndroid
{

	// Types

	public enum bfgStoreType
	{

		google,
		amazon}

	;

	// Constants

	public const string	BFGSETTING_FULLGAMEPURCHASE_KEY = "BFGSETTING_FULLGAMEPURCHASE_KEY";

	public const string	NOTIFICATION_PURCHASE_ASKUSER = "NOTIFICATION_PURCHASE_ASKUSER";
	// User info contains a dictionary with description, and pricing.
	public const string	NOTIFICATION_PURCHASE_NOTALLOWED = "NOTIFICATION_PURCHASE_NOTALLOWED";
	// This user is not allowed to make purchases on this phone.
	public const string	NOTIFICATION_PURCHASE_STARTED = "NOTIFICATION_PURCHASE_STARTED";
	/// SKU Revoked
	public const string	NOTIFICATION_PURCHASE_REVOKED = "NOTIFICATION_PURCHASE_REVOKED";
	/// Restore completed
	public const string NOTIFICATION_RESTORE_COMPLETED = "NOTIFICATION_RESTORE_COMPLETED";
	public const string	NOTIFICATION_PURCHASE_CANCELLED = "NOTIFICATION_PURCHASE_CANCELLED";
	public const string NOTIFICATION_PURCHASE_GETUSERID_SUCCEEDED = "NOTIFICATION_PURCHASE_GETUSERID_SUCCEEDED";

	// New Google IABv3 events
	public const string NOTIFICATION_BILLING_INITIALIZE_SUCCEEDED = "NOTIFICATION_BILLING_INITIALIZE_SUCCEEDED";
	public const string NOTIFICATION_BILLING_INITIALIZE_FAILED = "NOTIFICATION_BILLING_INITIALIZE_FAILED";
	public const string NOTIFICATION_PURCHASE_PRODUCTINFORMATION_FAILED	= "NOTIFICATION_PURCHASE_PRODUCTINFORMATION_FAILED";
	public const string NOTIFICATION_PURCHASE_CONSUMPTION_SUCCEEDED = "NOTIFICATION_PURCHASE_CONSUMPTION_SUCCEEDED";
	public const string NOTIFICATION_PURCHASE_CONSUMPTION_FAILED = "NOTIFICATION_PURCHASE_CONSUMPTION_FAILED";
	public const string NOTIFICATION_PURCHASE_SUCCEEDED_WITH_RECEIPT	= "NOTIFICATION_PURCHASE_SUCCEEDED_WITH_RECEIPT";

	public const string NOTIFICATION_POST_CURRENT_INVENTORY_SUCCEEDED	= "NOTIFICATION_POST_CURRENT_INVENTORY_SUCCEEDED";
	public const string NOTIFICATION_POST_CURRENT_INVENTORY_FAILED = "NOTIFICATION_POST_CURRENT_INVENTORY_FAILED";


	// Misc.
	public const string kPurchaseKeyPrefix = "PURCHASEKEY-";
	public const string kPurchaseMapName = "PurchaseMap";
	public const string kSkuMapName = "SkuMap";


	// Static Methods

	public static string getAppstoreName ()
	{

		#if UNITY_EDITOR
		return null;
		#elif UNITY_ANDROID
			string result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<string>("getAppstoreName");}
			return result;
		#else
			return null;
		#endif
	}

	public static void setupService ()
	{

		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("setupService");}
		#else
			return;
		#endif
	}

	public static bool startUsingService ()
	{

		#if UNITY_EDITOR
		return false;
		#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("startUsingService");}
			return result;
		#else
			return false;
		#endif
	}

	public static void resumeUsingService ()
	{

		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("resumeUsingService");}
		#else
			return;
		#endif
	}


	public static void reportPurchaseCompletedSuccessfully (string receipt)
	{

		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("reportPurchaseCompletedSuccessfully", receipt);}
		#else
			return;
		#endif
	}

	public static string getCurrentUser ()
	{

		#if UNITY_EDITOR
		return null;
		#elif UNITY_ANDROID
	string result;
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<string>("getCurrentUser");}
		return result;
		#else
			return null;
		#endif
	}

	public static void setCurrentUser (string userId)
	{

		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
	using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("setCurrentUser", userId);}
		#else
			return;
		#endif
	}

	public static void stopUsingService ()
	{

		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
	using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("stopUsingService");}
		#else
			return;
		#endif
	}

	public static void cleanupService ()
	{

		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
	using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("cleanupService");}
		#else
			return;
		#endif
	}

	public static bool beginPurchase ()
	{

		#if UNITY_EDITOR
		return false;
		#elif UNITY_ANDROID
	bool result;
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("beginPurchase");}
		return result;
		#else
		return false;
		#endif
	}

	public static bool beginPurchase (string productid)
	{

		#if UNITY_EDITOR
		return false;
		#elif UNITY_ANDROID
	bool result;
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("beginPurchase", productid);}
		return result;
		#else
		return false;
		#endif
	}

	public static void completePurchase (string productid)
	{

		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
	using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("completePurchase", productid);}
		#else
			return;
		#endif
	}

	public static bool completePurchase (string productid, string additionalPayload)
	{

		#if UNITY_EDITOR
		return false;
		#elif UNITY_ANDROID
	bool isSuccessful;
		string[] args = new string[] { productid, additionalPayload };
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {isSuccessful = ajc.CallStatic<bool>("completePurchase", args); }
		return isSuccessful;
		#else
		return false;
		#endif
	}

	public static void restorePurchase ()
	{

		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
	        using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("restorePurchase");}
		#else
			return;
		#endif
	}

	public static void restorePurchase (string productid)
	{

		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
	        using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("restorePurchase", productid);}
		#else
			return;
		#endif
	}

	public static void restorePurchase (List<string> productids)
	{
	
		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
		foreach (string productid in productids)
		{
			using (AndroidJavaClass ajc = new AndroidJavaClass ("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")){ajc.CallStatic ("restorePurchase", productid);}
		}            
		#else
            return;
		#endif
	}

	public static void consumePurchase (string productid)
	{

		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
	using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("consumePurchase", productid);}
		#else
			return;
		#endif
	}

	public static Dictionary<string, Dictionary<string,Dictionary<string,string>>> getVolatileInventory ()
	{

		#if UNITY_EDITOR
		return null;
		#elif UNITY_ANDROID
	Dictionary<string, Dictionary<string,Dictionary<string,string>>> returnDictionary = new Dictionary<string, Dictionary<string,Dictionary<string,string>>>();
		Dictionary<string, Dictionary<string,string>> purchaseDictionary = new Dictionary<string, Dictionary<string,string>>();
		Dictionary<string, Dictionary<string,string>> skuDictionary = new Dictionary<string, Dictionary<string,string>>();

		string inventoryJSON;
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {inventoryJSON =  ajc.CallStatic<string>("getVolatileInventory"); }
		JSONNode rootNode = JSON.Parse(inventoryJSON);

		if (inventoryJSON == null)
			return null;

		JSONNode rootPurchase = rootNode ["mPurchaseMap"];
		JSONNode rootSku = rootNode ["mSkuMap"];

		// purchase map
		foreach (KeyValuePair<string, JSONNode> objectAttribute in rootPurchase.AsObject) {

		string key = objectAttribute.Key;
			Dictionary<string,string> purchase_dictionary = new Dictionary<string,string>();
			// each purchase
			foreach (KeyValuePair<string, JSONNode> childObjecttAttribute in objectAttribute.Value.AsObject) {


			purchase_dictionary.Add(childObjecttAttribute.Key, childObjecttAttribute.Value);
			}
			purchaseDictionary.Add (key, purchase_dictionary);
		}

		// sku map
		foreach (KeyValuePair<string, JSONNode> objectAttribute in rootSku.AsObject) {

		string key = objectAttribute.Key;
			Dictionary<string,string> sku_dictionary = new Dictionary<string,string>();
			// each purchase
			foreach (KeyValuePair<string, JSONNode> childObjecttAttribute in objectAttribute.Value.AsObject) {

			sku_dictionary.Add(childObjecttAttribute.Key, childObjecttAttribute.Value);
			}
			skuDictionary.Add (key, sku_dictionary);
		}

		returnDictionary.Add (kPurchaseMapName, purchaseDictionary);
		returnDictionary.Add (kSkuMapName, skuDictionary);

		return returnDictionary;
		#else
		return null;
		#endif
	}

	public static Dictionary<string, string> productInformation (string productid)
	{

		#if UNITY_EDITOR
		return null;
		#elif UNITY_ANDROID
	Dictionary<string, string> returnDictionary = new Dictionary<string, string>();

		string productInformationJson;
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {productInformationJson = ajc.CallStatic<string>("productInformation", productid);}
		JSONNode rootNode = JSON.Parse (productInformationJson);
		foreach (KeyValuePair<string, JSONNode> objectAttribute in rootNode.AsObject) {

		string key = objectAttribute.Key;
			string val = objectAttribute.Value.Value;
			returnDictionary.Add(key, val);
		}

		return returnDictionary;
		#else
		return null;
		#endif
	}

	public static Dictionary<string, string> productInformation ()
	{

		#if UNITY_EDITOR
		return null;
		#elif UNITY_ANDROID
	Dictionary<string, string> returnDictionary = new Dictionary<string, string> ();
		string productInformationJson;
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {productInformationJson = ajc.CallStatic<string>("productInformation");}
		JSONNode rootNode = JSON.Parse (productInformationJson);
		foreach (KeyValuePair<string, JSONNode> objectAttribute in rootNode.AsObject) {

		string key = objectAttribute.Key;
			string val = objectAttribute.Value.Value;
			returnDictionary.Add(key, val);
		}
		return returnDictionary;
		#else
		return null;
		#endif
	}

	// Big Fish Android SDK 5.5

	public static bool postCurrentInventory ()
	{

		#if UNITY_EDITOR
		return false;
		#elif UNITY_ANDROID
	bool result;
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {result = ajc.CallStatic<bool>("postCurrentInventory");}
		return result;
		#else
		return false;
		#endif
	}

	public static void defineConsumableSKUs (List<string> skus)
	{

		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
	string jsonConsumableSKUs = BfgUtilities.ConvertArrayOfStringsToJsonString(skus);
		if (jsonConsumableSKUs == null)
			jsonConsumableSKUs = "[]";

		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("defineConsumableSKUs", jsonConsumableSKUs);}
		#else
			return;
		#endif
	}

	public static void finishPurchase (string sku)
	{

		#if UNITY_EDITOR
		return;
		#elif UNITY_ANDROID
	using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgPurchaseUnityWrapper")) {ajc.CallStatic("finishPurchase", sku);}
		#else
			return;
		#endif
	}
}
