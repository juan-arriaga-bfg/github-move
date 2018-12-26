package com.bigfishgames.bfgunityandroid;

import java.util.List;
import java.util.Hashtable;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.concurrent.Semaphore;

import android.util.Log;

import com.bigfishgames.bfglib.bfgpurchase.PublicInventory;
import com.bigfishgames.bfglib.bfgpurchase.bfgPurchase;
import com.bigfishgames.bfglib.bfgpurchase.bfgPurchaseGoogle;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonParser;

class StringReturn {
	public String _val; 
}

class BoolReturn {
	public boolean _val; 
}

public class bfgPurchaseUnityWrapper {
	
	private static volatile boolean bIsMutexed;
	
	public static void aquireMutex(Semaphore mutex) {
		bIsMutexed = true;

		try {
			mutex.acquire();
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
	}
	
	public static void releaseMutex(Semaphore mutex) {
		mutex.release();
		bIsMutexed = false;
	}
	
	public static boolean isMutexed() { return bIsMutexed; }

	public static boolean acquireProductInformation() {

		final BoolReturn boolreturn = new BoolReturn();
		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.currentInstance().addProductId(BFGUnityPlayerNativeActivity.DEFAULT_PRODUCTID);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					boolean retval = bfgPurchase.sharedInstance().acquireProductInformation();
					boolreturn._val = retval;
					releaseMutex(mutex);
				}
			}
		);
		
		
		aquireMutex(mutex);
		
		return boolreturn._val;
	}
	
	public static boolean acquireProductInformation(final String productId) {

		final BoolReturn boolreturn = new BoolReturn();
		final Semaphore mutex = new Semaphore(0);
		
		// TODO Remove debugging statements below.
		Log.wtf("purchaseGoogle", "acquireProductInformation called for " + productId + ".");

		BFGUnityPlayerNativeActivity.currentInstance().addProductId(productId);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					boolean retval = bfgPurchase.sharedInstance().acquireProductInformation(productId);
					boolreturn._val = retval;
					releaseMutex(mutex);
				}
			}
		);
		
		aquireMutex(mutex);
		
		// TODO Remove debugging statements below.
		
		if (boolreturn._val)
			Log.wtf("purchaseGoogle", "acquireProductInformation returning true.");
		else
			Log.wtf("purchaseGoogle", "acquireProductInformation returning false.");
		
		return boolreturn._val;
	}
	
	public static boolean acquireProductInformationForProducts(String productids) {

		final List<String> listProductIds = new ArrayList<String>();
		final BoolReturn boolreturn = new BoolReturn();
		final Semaphore mutex = new Semaphore(0);
		
		// The "productids" argument represents a list of product Ids in the format of a JSON array value.
		// Parse it and add the product id strings to listProductIds.
		if (productids != null) {

			JsonElement rootElement = (new JsonParser()).parse(productids);
			if (rootElement != null) {

				JsonArray rootArray = rootElement.getAsJsonArray();
				if (rootArray != null) {
					int i, elementCount = rootArray.size();

					for (i = 0; i < elementCount; ++i) {
						String productId = (rootArray.get(i)).getAsString();
						if (productId != null)
							listProductIds.add(productId);

							BFGUnityPlayerNativeActivity.currentInstance().addProductId(productId);
					}
				}
			}
		}

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					boolean retval = bfgPurchase.sharedInstance().acquireProductInformation(listProductIds);
					boolreturn._val = retval;
					releaseMutex(mutex);
				}
			}
		);
		
		aquireMutex(mutex);

		return boolreturn._val;
	}

	/*
	 * TODO Find corresponding Android SDK method or remove altogether.
	public static boolean canStartPurchase() {
		// Not supported by Android SDK.
		return false;
	}
	*/
	
	public static boolean completePurchase(String productid, String additionalPayload) {
		if (bfgPurchase.sharedInstance() instanceof bfgPurchaseGoogle) {
			((bfgPurchaseGoogle) bfgPurchase.sharedInstance()).completePurchase(productid, additionalPayload);
			return true;
		}
		return false;
	}
	
	public static String getVolatileInventory() {
		if (bfgPurchase.sharedInstance() instanceof bfgPurchaseGoogle) {
			PublicInventory inventory = ((bfgPurchaseGoogle) bfgPurchase.sharedInstance()).getVolatileInventory();
            GsonBuilder gsonBuilder = new GsonBuilder();
            gsonBuilder.serializeSpecialFloatingPointValues();
            Gson gson = gsonBuilder.create();
			String inventoryString = gson.toJson(inventory);
			return inventoryString;
		}
		return "";
	}

	public static boolean canStartPurchase(final String productId) {

		final BoolReturn boolreturn = new BoolReturn();
		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					boolean retval = bfgPurchase.sharedInstance().canStartPurchase(productId);
					boolreturn._val = retval;
					releaseMutex(mutex);
				}
			}
		);
		
		aquireMutex(mutex);
		
		return boolreturn._val;
	}

	/*
	 * TODO Find corresponding Android SDK method or remove altogether.
	public static boolean isPurchaseActive() {
		// Not supported by Android SDK.
		return false;
	}
	*/

	/*
	 * TODO Find corresponding Android SDK method or remove altogether.
	public static boolean isPurchaseActive(String productId) {
		// Not supported by Android SDK.
		return false;
	}
	*/
	
	/*
	 * TODO Find corresponding Android SDK method or remove altogether.
	public static boolean isRestoreActive() {
		// Not supported by Android SDK.
		return false;
	}
	*/

	/*
	 * TODO Find corresponding Android SDK method or remove altogether.
	public static boolean purchaseActivityInProgress() {
		// Not supported by Android SDK.
		return false;
	}
	*/

	public static void startPurchase() {
		bfgPurchase.sharedInstance().beginPurchase();
	}
	
	public static void startPurchase(String productId) {
		bfgPurchase.sharedInstance().beginPurchase(productId);
	}

	/*
	 * TODO Find corresponding Android SDK method or remove altogether.
	public static void startPurchase(String productId, String details1, String details2, String details3, String additionalDetails) {
		// Not supported by Android SDK.
	}
	*/
	
	public static boolean startService() {
		return bfgPurchase.sharedInstance().startUsingService();
	}
	
	public static String getAppstoreName() {
		return bfgPurchase.sharedInstance().getAppstoreName();
	}

	public static void setupService() {
		
		final Semaphore mutex = new Semaphore(0);
		
		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					bfgPurchase.sharedInstance().setupService(BFGUnityPlayerNativeActivity.currentInstance());
					releaseMutex(mutex);
				}
			}
		);

		aquireMutex(mutex);
	}
	
	public static boolean startUsingService() {
		
		final BoolReturn boolreturn = new BoolReturn();
		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					boolean retval = bfgPurchase.sharedInstance().startUsingService();
					boolreturn._val = retval;
					releaseMutex(mutex);
				}
			}
		);
		
		aquireMutex(mutex);
		
		return boolreturn._val;
	}

	public static void resumeUsingService() {
		
		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					bfgPurchase.sharedInstance().resumeUsingService();
					releaseMutex(mutex);
				}
			}
		);

		aquireMutex(mutex);
	}
	
	public static String getCurrentUser() {
		
		final StringReturn stringreturn = new StringReturn();
		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					String retval = bfgPurchase.sharedInstance().getCurrentUser();
					stringreturn._val = retval;
					releaseMutex(mutex);
				}
			}
		);
		
		aquireMutex(mutex);
		
		return stringreturn._val;
	}

	public static void setCurrentUser(String userId) {
		bfgPurchase.sharedInstance().setCurrentUser(userId);
	}
	
	public static void stopUsingService() {
		bfgPurchase.sharedInstance().stopUsingService();
	}

	public static void cleanupService() {
		bfgPurchase.sharedInstance().cleanupService();
	}

	public static boolean beginPurchase() {
		return bfgPurchase.sharedInstance().beginPurchase();
	}

	public static boolean beginPurchase(final String productid) {

		final BoolReturn boolreturn = new BoolReturn();
		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					boolean retval = bfgPurchase.sharedInstance().beginPurchase(productid);
					boolreturn._val = retval;
					releaseMutex(mutex);
				}
			}
		);
		
		aquireMutex(mutex);
		
		return boolreturn._val;
	}

	public static void completePurchase(final String productid) {

		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					bfgPurchase.sharedInstance().completePurchase(productid);
					releaseMutex(mutex);
				}
			}
		);

		aquireMutex(mutex);
	}

	public static void restorePurchase() {

		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					bfgPurchase.sharedInstance().restorePurchase();
					releaseMutex(mutex);
				}
			}
		);

		aquireMutex(mutex);
	}

	public static void restorePurchase(final String productid) {

		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					bfgPurchase.sharedInstance().restorePurchase(productid);
					releaseMutex(mutex);
				}
			}
		);

		aquireMutex(mutex);
	}

	public static void consumePurchase(final String productid) {

		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					bfgPurchase.sharedInstance().consumePurchase(productid);
					releaseMutex(mutex);
				}
			}
		);

		aquireMutex(mutex);
	}

	public static String productInformation(String productid) {
		Hashtable<String, Object> productInformationDictionary =
		bfgPurchase.sharedInstance().productInformation(productid);
		Gson gson = new Gson();
		String productInformationString = gson.toJson(productInformationDictionary);
		return productInformationString;
	}

	public static String productInformation() {
		Hashtable<String, Object> productInformationDictionary =
		bfgPurchase.sharedInstance().productInformation();
		Gson gson = new Gson();
		String productInformationString = gson.toJson(productInformationDictionary);
		return productInformationString;

	}

	// Big Fish Android SDK 5.5

	public static boolean postCurrentInventory() {
		return bfgPurchase.sharedInstance().postCurrentInventory();
	}

	public static void defineConsumableSKUs(String jsonConsumableSKUs) {
		final HashSet<String> consumableSKUs = new HashSet<String>();
		final Semaphore mutex = new Semaphore(0);

		if (jsonConsumableSKUs != null) {

			JsonElement rootElement = (new JsonParser()).parse(jsonConsumableSKUs);
			if (rootElement != null) {

				JsonArray rootArray = rootElement.getAsJsonArray();
				if (rootArray != null) {
					int i, elementCount = rootArray.size();

					for (i = 0; i < elementCount; ++i) {
						String sku = (rootArray.get(i)).getAsString();
						if (sku != null) {
							consumableSKUs.add(sku);
						}
					}
				}
			}
		}

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					bfgPurchase.sharedInstance().defineConsumableSKUs(consumableSKUs);

					releaseMutex(mutex);
				}
			}
		);

		aquireMutex(mutex);
	}

	public static void finishPurchase(final String sku) {
		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
			new Runnable()
			{
				@Override
				public void run()
				{
					bfgPurchase.sharedInstance().finishPurchase(sku);

					releaseMutex(mutex);
				}
			}
		);

		aquireMutex(mutex);
	}
}
