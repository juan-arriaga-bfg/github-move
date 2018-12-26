package com.bigfishgames.bfgunityandroid;

import android.util.Log;

import java.util.Hashtable;
import com.bigfishgames.bfglib.bfgreporting.bfgGameReporting;
import com.bigfishgames.bfglib.bfgreporting.bfgHasOffers;
import com.google.gson.Gson;

public class bfgGameReportingUnityWrapper {

	public static void logMainMenuShown() {
		bfgGameReporting.sharedInstance().logMainMenuShown();
	}

	public static void logOptionsShown() {
		bfgGameReporting.sharedInstance().logOptionsShown();
	}

	public static void logPurchaseMainMenuShown() {
		bfgGameReporting.sharedInstance().logPurchaseMainMenuShown();
	}

	public static void logPurchaseMainMenuClosed() {
		bfgGameReporting.sharedInstance().logPurchaseMainMenuClosed();
	}

	public static void logPurchasePayWallShown(String paywallID) {
		bfgGameReporting.sharedInstance().logPurchasePayWallShown(paywallID);
	}

	public static void logPurchasePayWallClosed(String paywallID) {
		bfgGameReporting.sharedInstance().logPurchasePayWallClosed(paywallID);
	}

	public static void logIAPbuttontapped(int purchaseButton) {
		bfgGameReporting.sharedInstance().logIAPButtonTapped(purchaseButton);
	}

	public static void logLevelStart(String levelID) {
		bfgGameReporting.sharedInstance().logLevelStart(levelID);
	}

	public static void logLevelFinished(String levelID) {
		bfgGameReporting.sharedInstance().logLevelFinished(levelID);
	}

	public static void logMiniGameStart(String miniGameID) {
		bfgGameReporting.sharedInstance().logMiniGameStart(miniGameID);
	}

	public static void logMiniGameSkipped(String miniGameID) {
		bfgGameReporting.sharedInstance().logMiniGameSkipped(miniGameID);
	}

	public static void logMiniGameFinished(String miniGameID) {
		bfgGameReporting.sharedInstance().logMiniGameFinished(miniGameID);
	}

	public static void logAchievementEarned(String achievementID) {
		bfgGameReporting.sharedInstance().logAchievementEarned(achievementID);
	}

	public static void logGameHintRequested() {
		bfgGameReporting.sharedInstance().logGameHintRequested();
	}

	public static void logGameCompleted() {
		bfgGameReporting.sharedInstance().logGameCompleted();
	}

	public static boolean logCustomEvent(String name, long _value, long level, String details1, String details2, String details3, String additionalDetailsJSON) {
		Hashtable<String, Object> additionalDetailsFromJSON = new Hashtable<String, Object>();
		// Parse JSON to populate hash table.
		Gson gson = new Gson();
		additionalDetailsFromJSON = gson.fromJson(additionalDetailsJSON, additionalDetailsFromJSON.getClass());
		return bfgGameReporting.sharedInstance().logCustomEvent(name, (int)_value, (int)level, details1, details2, details3, additionalDetailsFromJSON);
	}

	public static void logCustomPlacement(String placementName) {
		bfgGameReporting.sharedInstance().logCustomPlacement(placementName);
	}

	public static void logMobileAppTrackingCustomEvent(String name) {
		bfgHasOffers.sharedInstance().logMobileAppTrackingCustomEvent(name);
	}

	public static void logRewardedVideoSeenWithProvider(String videoProvider,String videoLocation)
	{
		bfgGameReporting.sharedInstance().logRewardedVideoSeenWithProvider(videoProvider, videoLocation);
	}

	public static void logRewardedVideoSeenWithProvider(String videoProvider)
	{
		bfgGameReporting.sharedInstance().logRewardedVideoSeenWithProvider(videoProvider);
	}
    
    public static void setPlayerSpend(float playerSpend)
    {
        bfgGameReporting.sharedInstance().setPlayerSpend(playerSpend);
    }
    
    public  static void setLastLevelPlayed(String lastLevelPlayed)
    {
        bfgGameReporting.sharedInstance().setLastLevelPlayed(lastLevelPlayed);
    }

	/*
	 * TODO Find corresponding Android SDK method or remove altogether.
	public static void preloadCustomPlacement(String placementName) {
		// Note: No corresponding method in Android SDK?
	}
	*/
}
