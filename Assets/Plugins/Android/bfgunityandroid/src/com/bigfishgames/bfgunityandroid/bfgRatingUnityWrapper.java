package com.bigfishgames.bfgunityandroid;

import com.bigfishgames.bfglib.bfgManager;
import com.bigfishgames.bfglib.bfgRating;
import java.util.concurrent.Semaphore;

public class bfgRatingUnityWrapper {

	public static boolean canShowMainMenuRateButton() {
		final BoolReturn boolreturn = new BoolReturn();
		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
						new Runnable()
						{
										@Override
										public void run()
										{
														boolean retval = bfgRating.sharedInstance().canShowMainMenuRateButton();
														boolreturn._val = retval;
														bfgUtilsUnityWrapper.releaseMutex(mutex);
										}
						}
		);


		bfgUtilsUnityWrapper.acquireMutex(mutex);

		return boolreturn._val;
	}

	public static void mainMenuGiveFeedback() {
		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
						new Runnable()
						{
										@Override
										public void run()
										{
											      bfgRating.sharedInstance().mainMenuGiveFeedback(bfgManager.getParentViewController());
														bfgUtilsUnityWrapper.releaseMutex(mutex);
										}
						}
		);

		bfgUtilsUnityWrapper.acquireMutex(mutex);
	}

	public static void mainMenuRateApp() {
		bfgRating.sharedInstance().mainMenuRateApp();
	}

	public static void userDidSignificantEvent() {
		final Semaphore mutex = new Semaphore(0);

		BFGUnityPlayerNativeActivity.bfgRunOnUIThread
		(
						new Runnable()
						{
										@Override
										public void run()
										{
											      bfgRating.sharedInstance().userDidSignificantEvent(bfgManager.getParentViewController());
														bfgUtilsUnityWrapper.releaseMutex(mutex);
										}
						}
		);

		bfgUtilsUnityWrapper.acquireMutex(mutex);
	}

	public static void enableRatingsPrompt () {
		bfgRating.sharedInstance().enableRatingsPrompt ();
	}

	public static void disableRatingsPrompt() {
		bfgRating.sharedInstance().disableRatingsPrompt();
	}

}
