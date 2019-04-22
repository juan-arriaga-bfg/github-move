using Debug = IW.Logger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using BFGSDK;

public class IOSPurchaseController : APurchaseController
{
	private List<string> iOSProductIDs;
	private List<string> iOSConsumableProductIds;
	private List<string> iOSNonConsumableProductIds;

	#region implemented abstract members of PurchaseController

	public override List<string> ProductIds {
		get {
			return iOSProductIDs;
		}
	}

	public override List<string> ConsumableProductIds {
		get {
			return iOSConsumableProductIds;
		}
	}

	public override List<string> NonConsumableProductIds {
		get {
			return iOSNonConsumableProductIds;
		}
	}

	public override void StartService ()
	{
		iOSConsumableProductIds = PurchaseControllerProductIds.consumableIOSProductIds;
		iOSNonConsumableProductIds = PurchaseControllerProductIds.nonConsumableIOSProductIds;
		iOSProductIDs = new List<string> ();
		iOSProductIDs.AddRange (iOSNonConsumableProductIds);
		iOSProductIDs.AddRange (iOSConsumableProductIds);

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

		if (!NotificationCenter.Instance.HandlerSetHasObserver (notification_product_information, bfgCommon.NOTIFICATION_PURCHASE_PRODUCTINFORMATION)) {
			NotificationCenter.Instance.AddObserver (notification_product_information, bfgCommon.NOTIFICATION_PURCHASE_PRODUCTINFORMATION);
		}

		bfgPurchase.startService ();
		bfgPurchase.acquireProductInformationForProducts (iOSProductIDs);

	}

	public override bool Purchase (string productID)
	{
		if (bfgPurchase.canStartPurchase (productID)) {
			bfgPurchase.startPurchase (productID);
			return true;
		}
		return false;
	}

	public override void RestorePurchase (List<string> productIDs)
	{
		bfgPurchase.restorePurchases ();
	}

	public override void ConsumePurchase (string productId)
	{
		//Not valid in iOS
	}

	public override ProductInfo GetProductInfo (string productID)
	{
		string productInfoJson = "";
		bool hasProdInfo = bfgPurchase.productInformation (productID, ref productInfoJson);
		if (!hasProdInfo) {
			return null;
		}
		ProductInfo productInfo = ProductInfo.GetFromJson (productInfoJson);
		return productInfo;
	}

	public override void Destroy ()
	{
		//Not valid in iOS
	}

	public override void ApplicationPauseEvent (bool isPaused)
	{
		//Not valid in iOS
	}
	#endregion

	void purchase_succeeded (string notification)
	{

		Debug.Log ("notification_purchase_succeeded: notification=" + notification);
		string productId = "";

		//Parse root node of notification JSON
		JSONNode root_node = JSON.Parse (notification);
		//Extract productID
		productId = root_node ["BFG_PURCHASE_OBJECT_USER_INFO_KEY"]["productInfo"]["productID"];

		if (String.IsNullOrEmpty(productId)) {
			Debug.Log("notification_purchase_succeeded: unable to retrieve product ID: " + notification);
			return;
		}

		//Extract restore value and productTitle
		bool isRestore = root_node ["BFG_PURCHASE_OBJECT_USER_INFO_KEY"]["restore"].AsBool;
		string productTitle = root_node ["BFG_PURCHASE_OBJECT_USER_INFO_KEY"]["productInfo"]["title"];

		bfgPurchase.finishPurchase(productId);

		if (isRestore) {
				Debug.Log("Your purchase has been restored.");
		} else {
				Debug.Log("Thank you for your purchase.");
		}
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

	void restore_failed (string productId)
	{
		Debug.Log ("Restore failed for product: " + productId);
		OnRestoreFailed (productId);
	}

	void notification_product_information (string notification)
	{
		JSONNode root_node = JSON.Parse (notification);
		JSONArray successes = root_node ["BFG_ACQUIRE_PRODUCT_INFO_SUCCESSES_KEY"].AsArray;
		JSONArray failures = root_node ["BFG_ACQUIRE_PRODUCT_INFO_FAILURES_KEY"].AsArray;

		if (failures.Count > 0)
		{
			Debug.Log("Unexpected failures acquiring product info for the following products: " + failures.ToString());
			return;
		}

		string successesString = successes.ToString();
		Debug.Log("Acquired product info for: " + successesString);

	}
}
