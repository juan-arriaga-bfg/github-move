using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BFGSDK
{
    public class bfgSettings
    {
#if (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern bool __bfgSettings__getBoolean(string key, bool withDefault);
    [DllImport("__Internal")]
    private static extern double __bfgSettings__getDouble(string key, double withDefault);
    [DllImport("__Internal")]
    private static extern int __bfgSettings__getInteger(string key, int withDefault);
    [DllImport("__Internal")]
    private static extern long __bfgSettings__getLong(string key, long withDefault);
    [DllImport("__Internal")]
    private static extern void __bfgSettings__getString(string key, string withDefault, StringBuilder returnGetString);

    private static readonly int BFG_SETTING = 300;
#endif

        //
        // ---------------------------------------
        //

        public static bool getBoolean(string key, bool withDefault)
        {
#if UNITY_EDITOR
            return true;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgSettings__getBoolean(key, withDefault);
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgSettingsUnityWrapper")) {result = ajc.CallStatic<bool>("getBoolean", key, withDefault);}
			return result;
#else
			return false;
#endif
        }

        public static double getDouble(string key, double withDefault)
        {
#if UNITY_EDITOR
            return 0;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgSettings__getDouble(key, withDefault);
#elif UNITY_ANDROID
			double result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgSettingsUnityWrapper")) {result = ajc.CallStatic<double>("getDouble", key, withDefault);}
			return result;
#else
			return 0;
#endif
        }

        public static int getInteger(string key, int withDefault)
        {
#if UNITY_EDITOR
            return 0;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgSettings__getInteger(key, withDefault);
#elif UNITY_ANDROID
			int result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgSettingsUnityWrapper")) {result = ajc.CallStatic<int>("getInteger", key, withDefault);}
			return result;
#else
			return 0;
#endif
        }

        public static long getLong(string key, long withDefault)
        {
#if UNITY_EDITOR
            return 0;
#elif UNITY_IOS || UNITY_IPHONE
			return __bfgSettings__getLong(key, withDefault);
#elif UNITY_ANDROID
			long result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgSettingsUnityWrapper")) {result = ajc.CallStatic<long>("getLong", key, withDefault);}
			return result;
#else
			return 0;
#endif
        }

        public static string getString(string key, string withDefault)
        {
#if UNITY_EDITOR
            return withDefault;
#elif UNITY_IOS || UNITY_IPHONE
      StringBuilder returnSetting = new StringBuilder(bfgSettings.BFG_SETTING);
			__bfgSettings__getString(key, withDefault, returnSetting);
      return returnSetting.ToString();
#elif UNITY_ANDROID
			string result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgSettingsUnityWrapper")) {result = ajc.CallStatic<string>("getString", key, withDefault);}
			return result;
#else
			return null;
#endif
        }
    }
}