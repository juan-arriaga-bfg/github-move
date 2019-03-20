using Debug = IW.Logger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProductInfo
{
	public string title;
	public string description;
	public string price;
	public string priceMicros;
	public string currency;

	public static ProductInfo GetFromJson (string json)
	{
        try
        {
            return JsonUtility.FromJson<ProductInfo> (json);
        }
        catch (Exception e)
        {
            Debug.LogError($"ProductInfo GetFromJson: Can't parse json: \n{json}");
            return null;
        }
		
	}

	public override string ToString ()
	{
		string prodInfoString = String.Format ("Title: {0}\nDescription: {1}\nPrice: {2}\nPriceMicros: {3}\nCurrency: {4}", this.title, this.description, this.price, this.priceMicros, this.currency);
		return prodInfoString;
	}
}

public abstract class APurchaseController
{
	public abstract List<string> ProductIds { get; }

	public abstract List<string> ConsumableProductIds { get; }

	public abstract List<string> NonConsumableProductIds { get; }

	//Static instance of of either GooglePurchaseController, AmazonPurchaseController, or IOSPurchaseController
	public static APurchaseController instance = null;
	// The methods within this region are not intended to be overridden by it's derived classes.
	// These methods are important for the subclasses to use so they can raise the various PurchaseController events

	#region overridable methods

	protected void OnPurchaseSucceeded (string productID, ProductInfo productInfo)
	{
		PurchaseController.Instance.OnPurchaseSucceeded (productID, productInfo);
	}

	protected void OnPurchaseFailed (string productID)
	{
		PurchaseController.Instance.OnPurchaseFailed (productID);
	}

	protected void OnRestoreSucceeded (string productID)
	{
		PurchaseController.Instance.OnRestoreSucceeded (productID);
	}

	protected void OnRestoreFailed (string productID)
	{
		PurchaseController.Instance.OnRestoreFailed (productID);
	}

	#endregion

	/// <summary>
	/// Awake an instance of either GooglePurchaseController, AmazonPurchaseController, or IOSPurchaseController.
	/// This allows the developer to interface with PurchaseController instead of any of the subclasses of PurchaseController.
	/// </summary>
	public static APurchaseController Initialize ()
	{
		//Check if instance already exists
		if (instance == null) {
			#if UNITY_ANDROID
			if (bfgSettings.getString("app_store", "google") == "google") {
				instance = new GooglePurchaseController ();
				Debug.Log ("Google Purchase Controller Initialized");
			} else {
				instance = new AmazonPurchaseController ();
				Debug.Log ("Amazon Purchase Controller Initialized");
			}
			#elif UNITY_IPHONE || UNITY_IOS
				instance = new IOSPurchaseController();
				Debug.Log ("iOS Purchase Controller Initialized");
			#else
			instance = new GooglePurchaseController ();
			#endif
		}
		return instance;
	}

	/// <summary>
	/// Starts the service.
	/// Developer will need to subscribe for the OnPurchaseSuccess, OnPurchaseFailed, OnRestoreSucceeded and OnRestoreFailed delegates themselves.
	/// Starts up the purchasing service based on either Google, Amazon, or iOS store
	/// </summary>
	public abstract void StartService ();

	/// <summary>
	/// Purchase the specified productID.
	/// </summary>
	/// <returns>False if the product can't be purchased. True if it can be.</returns>
	/// <param name="productID">Product I.</param>
	public abstract bool Purchase (string productID);

	/// <summary>
	/// Restores the purchase. The instances of Purchase Controller will automatically handle this
	/// </summary>
	/// <param name="productIDs">Product Ids.</param>
	public abstract void RestorePurchase (List<string> productIDs);

	/// <summary>
	/// Consumes the purchase.
	/// NOTE: The Android and IOS purchase controllers are set up for automatic consumption, so calling this yourself is unnecessary unless you have a non-consumable SKU that you all of a sudden want to consume.
	/// </summary>
	/// <param name="productId">Product identifier.</param>
	public abstract void ConsumePurchase (string productId);

	/// <summary>
	/// Gets the product info for the specified productID. This will return an instance of the ProductInfo object.
	/// </summary>
	/// <returns>The product info.</returns>
	/// <param name="productID">Product I.</param>
	public abstract ProductInfo GetProductInfo (string productID);

	public abstract void Destroy ();

	public abstract void ApplicationPauseEvent (bool isPaused);
}
