package com.bigfishgames.bfgunityandroid;

import com.bigfishgames.bfglib.bfgSettings;

public class bfgSettingsUnityWrapper {

	public static boolean getBoolean(String key, boolean withDefault) {
		return bfgSettings.getBoolean(key, withDefault);
	}

	public static double getDouble(String key, double withDefault) {
		return bfgSettings.getDouble(key, withDefault);
	}

	public static int getInteger(String key, int withDefault) {
		return bfgSettings.getInteger(key, withDefault);
	}

	public static long getLong(String key, long withDefault) {
		return bfgSettings.getLong(key, withDefault);
	}

	public static String getString(String key, String withDefault) {
		return bfgSettings.getString(key, withDefault);
	}
}
