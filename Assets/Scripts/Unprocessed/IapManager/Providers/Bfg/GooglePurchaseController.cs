using Debug = IW.Logger;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GooglePurchaseController : APurchaseController
{
	private List<string> consumableGoogleProductIds;
	private List<string> nonConsumableGoogleProductIds;
	private List<string> googleProductIds;

	#region implemented abstract  members of PurchaseController

	public override List<string> ProductIds {
		get {
			return googleProductIds;
		}
	}

	public override List<string> ConsumableProductIds {
		get {
			return consumableGoogleProductIds;
		}
	}

	public override List<string> NonConsumableProductIds {
		get {
			return nonConsumableGoogleProductIds;
		}
	}

	public override void StartService ()
	{
        Debug.Log($"GooglePurchaseController: StartService");
        
		consumableGoogleProductIds = PurchaseControllerProductIds.consumableGoogleProductIds;
		nonConsumableGoogleProductIds = PurchaseControllerProductIds.nonConsumableGoogleProductIds;
		googleProductIds = new List<string> ();
		googleProductIds.AddRange (consumableGoogleProductIds);
		googleProductIds.AddRange (nonConsumableGoogleProductIds);

		bfgPurchaseAndroid.defineConsumableSKUs(consumableGoogleProductIds);
		
		// Only add each notification observer once...in case the dev calls StartService multiple times.
		// Shared notifications between all platforms
		if (!NotificationCenter.Instance.HandlerSetHasObserver (purchase_succeeded, bfgCommon.NOTIFICATION_PURCHASE_SUCCEEDED)) {
			NotificationCenter.Instance.AddObserver (purchase_succeeded, bfgCommon.NOTIFICATION_PURCHASE_SUCCEEDED);
		}
		if (!NotificationCenter.Instance.HandlerSetHasObserver (purchase_failed, bfgCommon.NOTIFICATION_PURCHASE_FAILED)) {
			NotificationCenter.Instance.AddObserver (purchase_failed, bfgCommon.NOTIFICATION_PURCHASE_FAILED);
		}
		if (!NotificationCenter.Instance.HandlerSetHasObserver (restore_succeeded, bfgCommon.NOTIFICATION_RESTORE_SUCCEEDED)) {
			NotificationCenter.Instance.AddObserver (restore_succeeded, bfgCommon.NOTIFICATION_RESTORE_SUCCEEDED);
		}
		if (!NotificationCenter.Instance.HandlerSetHasObserver (restore_failed, bfgCommon.NOTIFICATION_RESTORE_FAILED)) {
			NotificationCenter.Instance.AddObserver (restore_failed, bfgCommon.NOTIFICATION_RESTORE_FAILED);
		}

		// Android specific notifications
		if (!NotificationCenter.Instance.HandlerSetHasObserver (billing_init_succeeded, bfgPurchaseAndroid.NOTIFICATION_BILLING_INITIALIZE_SUCCEEDED)) {
			NotificationCenter.Instance.AddObserver (billing_init_succeeded, bfgPurchaseAndroid.NOTIFICATION_BILLING_INITIALIZE_SUCCEEDED);
		}
		if (!NotificationCenter.Instance.HandlerSetHasObserver (notification_product_information, bfgCommon.NOTIFICATION_PURCHASE_PRODUCTINFORMATION)) {
			NotificationCenter.Instance.AddObserver (notification_product_information, bfgCommon.NOTIFICATION_PURCHASE_PRODUCTINFORMATION);
		}
		if (!NotificationCenter.Instance.HandlerSetHasObserver (notification_ask_user, bfgPurchaseAndroid.NOTIFICATION_PURCHASE_ASKUSER)) {
			NotificationCenter.Instance.AddObserver (notification_ask_user, bfgPurchaseAndroid.NOTIFICATION_PURCHASE_ASKUSER);
		}

		bfgPurchaseAndroid.setupService ();
		bfgPurchaseAndroid.startUsingService ();
	}

	public override bool Purchase (string productID)
	{
        Debug.Log($"GooglePurchaseController: Purchase: {productID}");
        
		if (bfgPurchase.canStartPurchase (productID)) {
			bfgPurchase.startPurchase (productID);
			return true;
		}
		return false;
	}

	public override void RestorePurchase (List<string> productIDs)
	{
		// Restore the entire list at once. We will need to confirm that the purchase restore notification will send a valid string[] of all the product ids that have been restored.
		bfgPurchaseAndroid.restorePurchase (productIDs);
	}

	public override void ConsumePurchase (string productId)
	{
		bfgPurchaseAndroid.consumePurchase (productId);
	}

	public override ProductInfo GetProductInfo (string productID)
	{
        Debug.Log($"GooglePurchaseController: GetProductInfo: {productID}");
        
		string productInfoJson = "";
		bool hasProdInfo = bfgPurchase.productInformation (productID, ref productInfoJson);

		// return null if there is no product info for the productID.
		// PurchaseController class has logic to handle null and return a ProductInfo object with dummy values.
		if (!hasProdInfo) {
			return null;
		}
		ProductInfo productInfo = ProductInfo.GetFromJson (productInfoJson);
		return productInfo;
	}

	public override void ApplicationPauseEvent (bool isPaused)
	{
		if (isPaused) {
			Debug.Log ("Google Purchase stop called.");
			bfgPurchaseAndroid.stopUsingService ();
		} else {
			Debug.Log ("Google Purchase resume called.");
			bfgPurchaseAndroid.resumeUsingService ();
		}
	}

	public override void Destroy ()
	{
		bfgPurchaseAndroid.cleanupService ();
	}

	#endregion

	void purchase_succeeded (string productId)
	{
        Debug.Log($"GooglePurchaseController: purchase_succeeded: {productId}");
        
		// consume the purchase if it is marked as a consumable product id
		if (consumableGoogleProductIds.Contains (productId)) {
			bfgPurchaseAndroid.finishPurchase(productId);
		}
		ProductInfo pi = GetProductInfo (productId);
		OnPurchaseSucceeded (productId, pi);
	}

	void purchase_failed (string productId)
	{
        Debug.Log($"GooglePurchaseController: purchase_failed: {productId}");
		OnPurchaseFailed (productId);
	}

	void restore_succeeded (string productId)
	{
        Debug.Log($"GooglePurchaseController: restore_succeeded: {productId}");
		OnRestoreSucceeded (productId);
	}

	void restore_failed (string productId)
	{
		Debug.Log ($"GooglePurchaseController: restore_failed for product: {productId}");
		OnRestoreFailed (productId);
	}

	void billing_init_succeeded (string notification)
	{
        Debug.Log($"GooglePurchaseController: billing_init_succeeded: {notification}");
		bfgPurchase.acquireProductInformationForProducts (ProductIds);
	}

	void notification_product_information (string notification)
	{
        Debug.Log($"GooglePurchaseController: notification_product_information: {notification}");
        
		// Consume all consumable google product IDs after getting product information
		foreach (string productId in consumableGoogleProductIds) {
			bfgPurchaseAndroid.consumePurchase (productId);
		}
		RestorePurchase (ProductIds);
	}

	void notification_ask_user (string productID)
	{
        Debug.Log($"GooglePurchaseController: notification_ask_user: {productID}");
        
		bfgPurchaseAndroid.completePurchase (productID);
	}
}
