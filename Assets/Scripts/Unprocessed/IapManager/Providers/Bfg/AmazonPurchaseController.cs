using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AmazonPurchaseController : APurchaseController
{
	private List<string> consumableAmazonProductIds;
	private List<string> nonConsumableAmazonProductIds;
	private List<string> amazonProductIds;

	bool hasStarted = false;

	#region implemented abstract members of PurchaseController

	public override List<string> ProductIds {
		get {
			return amazonProductIds;
		}
	}

	public override List<string> ConsumableProductIds {
		get {
			return consumableAmazonProductIds;
		}
	}

	public override List<string> NonConsumableProductIds {
		get {
			return nonConsumableAmazonProductIds;
		}
	}

	public override void StartService ()
	{
		consumableAmazonProductIds = PurchaseControllerProductIds.consumableAmazonProductIds;
		nonConsumableAmazonProductIds = PurchaseControllerProductIds.nonConsumableAmazonProductIds;
		amazonProductIds = new List<string> ();
		amazonProductIds.AddRange (consumableAmazonProductIds);
		amazonProductIds.AddRange (nonConsumableAmazonProductIds);

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

		//Amazon specific notifications
		if (!NotificationCenter.Instance.HandlerSetHasObserver (notification_ask_user, bfgPurchaseAndroid.NOTIFICATION_PURCHASE_REVOKED)) {
			NotificationCenter.Instance.AddObserver (notification_purchase_revoked, bfgPurchaseAndroid.NOTIFICATION_PURCHASE_REVOKED);
		}

		bfgPurchaseAndroid.setupService ();
		hasStarted = bfgPurchaseAndroid.startUsingService ();


	}

	public override bool Purchase (string productID)
	{
		if (bfgPurchase.canStartPurchase (productID)) {
			bfgPurchase.startPurchase (productID);
			return true;
		}
		return false;
	}

	public override ProductInfo GetProductInfo (string productID)
	{
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

	public override void RestorePurchase (List<string> productIDs)
	{
		bfgPurchaseAndroid.restorePurchase (productIDs);
	}

	public override void ConsumePurchase (string productId)
	{
		bfgPurchaseAndroid.consumePurchase (productId);
	}

	public override void ApplicationPauseEvent (bool isPaused)
	{
		if (isPaused && hasStarted) {
			Debug.Log ("Purchase Service stopping...");
			bfgPurchaseAndroid.stopUsingService ();
		} else if (hasStarted) {
			Debug.Log ("Purchase Service resuming...");
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
		// consume the purchase if it is marked as a consumable product id
		if (consumableAmazonProductIds.Contains (productId)) {
			bfgPurchaseAndroid.consumePurchase (productId);
		}
		ProductInfo pi = GetProductInfo (productId);
		OnPurchaseSucceeded (productId, pi);
	}

	void purchase_failed (string productId)
	{
		OnPurchaseFailed (productId);
	}

	void restore_succeeded (string productId)
	{
		Debug.Log ("Restore success for product: " + productId);
		OnRestoreSucceeded (productId);
	}

	void notification_purchase_revoked (string productId)
	{
		Debug.Log ("Revoked product: " + productId);
	}

	void restore_failed (string productId)
	{
		Debug.Log ("Restore failed for product: " + productId);
		OnRestoreFailed (productId);
	}
	void billing_init_succeeded (string notification)
	{
		bfgPurchase.acquireProductInformationForProducts (amazonProductIds);
	}

	void notification_product_information (string notification)
	{
		// Consume all consumable google product IDs after getting product information
		foreach (string productId in consumableAmazonProductIds) {
			bfgPurchaseAndroid.consumePurchase (productId);
		}
		RestorePurchase (ProductIds);
	}

	void notification_ask_user (string productID)
	{
		bfgPurchaseAndroid.completePurchase (productID);
	}
}
