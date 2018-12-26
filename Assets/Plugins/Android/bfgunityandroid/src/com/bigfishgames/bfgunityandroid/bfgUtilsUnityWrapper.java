package com.bigfishgames.bfgunityandroid;

import java.util.concurrent.Semaphore;

import com.bigfishgames.bfglib.bfgutils.bfgUtils;

public class bfgUtilsUnityWrapper {

	public static void acquireMutex(Semaphore mutex) {
		try {
			mutex.acquire();
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
	}

	public static void releaseMutex(Semaphore mutex) {
		mutex.release();
	}

	public static String bfgUDID() {
		
		final Semaphore mutex = new Semaphore(0);
		final StringReturn stringreturn = new StringReturn();
		
		BFGUnityPlayerNativeActivity.bfgRunOnUIThread(
			new Runnable() {
				@Override
				public void run() {
					stringreturn._val = bfgUtils.bfgUDID();
					releaseMutex(mutex);
				}
			}
		);

		acquireMutex(mutex);
		return stringreturn._val;
	}
}
