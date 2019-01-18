using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PurchaseController
/// Developers call PurchaseController.StartService() to initialize everything. This will create a non destroyable game object with this script attached as a component. 
/// After that, developers can use PurchaseController.Instance.StartService(), PurchaseController.Instance.Purchase() and PurchaseController.Instance.GetProductInfo() freely.
/// Developers can also add event subscribers for the following events (if they want to handle them): PurchaseSucceededEvent, PurchaseFailedEvent, RestoreSucceededEvent and RestoreFailedEvent.
/// They must add these event subscribers AFTER an Instance of PurchaseController is available. 
/// </summary>
public class PurchaseController : MonoBehaviour
{
	public static PurchaseController Instance = null;

	public List<string> ProductIds { get { return APurchaseController.instance.ProductIds; } }

	public List<string> ConsumableProductIds { get { return APurchaseController.instance.ConsumableProductIds; } }

	public List<string> NonComsumableProductIds { get { return APurchaseController.instance.NonConsumableProductIds; } }

	// Delegate callbacks for purchase lifecycle events
	public delegate void PurchaseFailedDelegate (string productID);

	public delegate void PurchaseSuccessDelegate (string productID,ProductInfo productInfo);

	public delegate void RestoreDelegate (string productID);

	// Events that will be raised when important notifications that come from the BFG SDK are fired
	public event PurchaseSuccessDelegate PurchaseSucceededEvent;
	public event PurchaseFailedDelegate PurchaseFailedEvent;
	public event RestoreDelegate RestoreSucceededEvent;
	public event RestoreDelegate RestoreFailedEvent;

	// These methods allow the APurchaseController (and its subclasses) to raise the purchase events listed above.

	#region purchase event raising methods

	public void OnPurchaseSucceeded (string productID, ProductInfo productInfo)
	{		
		// Make a temporary copy of the event to avoid possibility of
		// a race condition if the last subscriber unsubscribes
		// immediately after the null check and before the event is raised.
		PurchaseSuccessDelegate handler = PurchaseSucceededEvent;
		if (handler != null) {
			handler (productID, productInfo);
		}
	}

	public void OnPurchaseFailed (string productID)
	{
		// Make a temporary copy of the event to avoid possibility of
		// a race condition if the last subscriber unsubscribes
		// immediately after the null check and before the event is raised.
		PurchaseFailedDelegate handler = PurchaseFailedEvent;
		if (handler != null) {
			handler (productID);
		}
	}

	public void OnRestoreSucceeded (string productID)
	{
		// Make a temporary copy of the event to avoid possibility of
		// a race condition if the last subscriber unsubscribes
		// immediately after the null check and before the event is raised.
		RestoreDelegate handler = RestoreSucceededEvent;
		if (handler != null) {
			handler (productID);
		}
	}

	public void OnRestoreFailed (string productID)
	{
		// Make a temporary copy of the event to avoid possibility of
		// a race condition if the last subscriber unsubscribes
		// immediately after the null check and before the event is raised.
		RestoreDelegate handler = RestoreFailedEvent;
		if (handler != null) {
			handler (productID);
		}
	}

	#endregion

	#region Unity lifecycle events

	void Awake ()
	{
		//Check if instance already exists
		if (Instance == null) {			
			Instance = this;
		} else if (Instance != this) {
			//This enforces our singleton pattern, meaning there can only ever be one instance of AbstractPurchaseController.
			Destroy (gameObject);    
		}

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad (gameObject);

		// Initialize one of the subclass instances of APurchaseController. 
		// After this, just call APurchaseController.instance, and it'll already have the correct instance you need
		APurchaseController.Initialize ();
	}

	void OnDestroy ()
	{		
		APurchaseController.instance.Destroy ();
	}

	void OnApplicationPause (bool isPaused)
	{                 
		APurchaseController.instance.ApplicationPauseEvent (isPaused);
	}

	#endregion

	#region methods to interface with

	/// <summary>
	/// Will create a game object and attach this script as a component to it, making the game object not destroyable on load. 
	/// Then it starts up the purchasing service based on either Google, Amazon, or iOS store
	/// Developers will need to add listeners for the following events if they want to handle them: OnPurchaseSuccess, OnPurchaseFailed, OnRestoreSucceeded and OnRestoreFailed.
	/// </summary>
	public static void StartService ()
	{
		// If Instance is null, we know the dev hasn't already added this class as a component to a gameobject. Let's generate one automatically, then start the service. 
		if (Instance == null) {
			GameObject purchaseControllerObject = new GameObject ("BFGUnityWrapperPurchaseController");
			purchaseControllerObject.AddComponent <PurchaseController> ();
		}

		APurchaseController.instance.StartService ();
	}

	/// <summary>
	/// Purchase the specified productID.
	/// </summary>
	/// <returns> False if the productId can't be purchased, true if it can.</returns>
	/// <param name="productID">Product I.</param>
	public bool Purchase (string productID)
	{
		return APurchaseController.instance.Purchase (productID);
	}

	/// <summary>
	/// Gets the product info for the specified productID. This will return an instance of the ProductInfo object. 
	/// </summary>
	/// <returns>The product info. Null if there is no product info</returns>
	/// <param name="productID">Product I.</param>
	public ProductInfo GetProductInfo (string productID)
	{
		ProductInfo pi = APurchaseController.instance.GetProductInfo (productID);
		if (pi == null) {
			pi = new ProductInfo ();
			pi.title = "Title Unavailable";
			pi.description = "Description Unavailable";
			pi.price = "Price Unavailable";
			pi.priceMicros = "PriceMicros Unavailable";
			pi.currency = "Currency Unavailable";
		}
		return pi;
	}

	/// <summary>
	/// Consumes the product.
	/// Since automatic purchase consumption is already enabled, this method should only be used for testing purposes.
	/// </summary>
	/// <param name="productID">Product ID.</param>
	public void ConsumePurchase (string productID)
	{
		APurchaseController.instance.ConsumePurchase (productID);
	}

	#endregion




}