using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BFGSDK
{
    public class bfgManager
    {

        [Serializable]
        private class JsonList
        {
            public static JsonList FromJson(string json)
            {
                return JsonUtility.FromJson<JsonList>(json);
            }
        }

#if UNITY_EDITOR
        // Nothing to see here.
#elif UNITY_IOS || UNITY_IPHONE

	[System.Serializable]
	private class ButtonBounds
	{
	public int x;
	public int y;
	public int width;
	public int height;

	public ButtonBounds (int x, int y, int width, int height)
	{
	this.x = x;
	this.y = y;
	this.width = width;
	this.height = height;
	}

	public string SaveToString ()
	{
	return JsonUtility.ToJson (this);
	}

	public static ButtonBounds CreateFromJSON (string jsonString)
	{
	return JsonUtility.FromJson<ButtonBounds> (jsonString);
	}
	}
	[DllImport("__Internal")]
	private static extern void _setParentViewController();

	[DllImport("__Internal")]
	private static extern long _userID();

	[DllImport("__Internal")]
	private static extern void _setUserID(long userID);

	[DllImport("__Internal")]
	private static extern bool _launchSDKByURLScheme(string urlScheme);

	[DllImport("__Internal")]
	private static extern long _sessionCount();

	[DllImport("__Internal")]
	private static extern bool _isInitialLaunch();

	[DllImport("__Internal")]
	private static extern bool _isFirstTime();

	[DllImport("__Internal")]
	private static extern bool _isInitialized();

	[DllImport("__Internal")]
	private static extern void _showMoreGames();

	[DllImport("__Internal")]
	private static extern void _showSupport();

	[DllImport("__Internal")]
	private static extern void _showPrivacy();

	[DllImport("__Internal")]
	private static extern void _showTerms();

	[DllImport("__Internal")]
	private static extern void _showWebBrowser(string startPage);

	[DllImport("__Internal")]
	private static extern void _removeWebBrowser();

	[DllImport("__Internal")]
	private static extern bool _checkForInternetConnection();

	[DllImport("__Internal")]
	private static extern bool _checkForInternetConnectionAndAlert(bool displayAlert);

	[DllImport("__Internal")]
	private static extern bool _startBranding();

	[DllImport("__Internal")]
	private static extern void _stopBranding();

	[DllImport("__Internal")]
	private static extern void _addPauseResumeDelegate();

	[DllImport("__Internal")]
	private static extern void _removePauseResumeDelegate();

	[DllImport("__Internal")]
	private static extern bool _isPaused();

	[DllImport("__Internal")]
	private static extern bool _createCCSButtonBounds(float widthPercent, string horizontalAnchor, string verticalAnchor, StringBuilder returnButtonJson);

	[DllImport("__Internal")]
	private static extern void _hideCCSButton();

	[DllImport("__Internal")]
	private static extern void _showCcsButtonLocation(string buttonBoundsJson, string gameLocation);

	[DllImport("__Internal")]
	private static extern void _showCcsButton(string buttonBoundsJson);

	[DllImport("__Internal")]
	private static extern bool _isShowingCCSButton();

    [DllImport("__Internal")]
    private static extern void _showCcsButtonLocationWithPercent(float xPercent, float yPercent, float widthPercent, float heightPercent, string gameLocation);

    [DllImport("__Internal")]
    private static extern void _showCcsButtonLocationWithPixels(int x, int y, int width, int height, string gameLocation);

	[DllImport("__Internal")]
    private static extern int _getDebugDictionary(StringBuilder jsonDebugDictionary, int size);

	[DllImport("__Internal")]
    private static extern void _setDebugDictionary(string jsonDebugDictionary);

//6.7 GDPR

	[DllImport("__Internal")]
	private static extern void _removePolicyListener();

	[DllImport("__Internal")]
	private static extern bool _didAcceptPolicyControl(string policyControl);

	[DllImport("__Internal")]
	private static extern void _setLimitEventAndDataUsage(bool limitData);

#endif

        public static void setParentViewController()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			_setParentViewController();
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return;
#endif
        }

        //  Deprecated in  Big Fish iOS SDK 5.10
        /*
        public static bool coppaOptOut()
        {
            #if UNITY_EDITOR
                return false;
            #elif UNITY_IOS || UNITY_IPHONE
                return _coppaOptOut();
            #elif UNITY_ANDROID
                throw new NotImplementedException();
            #else
                return false;
            #endif
        }

        public static void setCoppaOptOut(bool yesOrNo)
        {
            #if UNITY_EDITOR
                return;
            #elif UNITY_IOS || UNITY_IPHONE
                _setCoppaOptOut(yesOrNo);
            #elif UNITY_ANDROID
                throw new NotImplementedException();
            #else
                return;
            #endif
        }
    */

        public static long userID()
        {
#if UNITY_EDITOR
            return 0;
#elif UNITY_IOS || UNITY_IPHONE
			return _userID();
#elif UNITY_ANDROID
			long result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {result = ajc.CallStatic<long>("userID");}
			return result;
#else
			return 0;
#endif
        }

        public static void setUserID(long userID)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			_setUserID(userID);
#elif UNITY_ANDROID
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) { ajc.CallStatic ("setUserID", userID);}
#else
			return;
#endif
        }

        public static long sessionCount()
        {
#if UNITY_EDITOR
            return 0;
#elif UNITY_IOS || UNITY_IPHONE
			return _sessionCount();
#elif UNITY_ANDROID
			long result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {result = ajc.CallStatic<long>("sessionCount");}
			return result;
#else
			return 0;
#endif
        }

        public static bool isInitialLaunch()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return _isInitialLaunch();
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {result = ajc.CallStatic<bool>("isInitialLaunch");}
			return result;
#else
			return false;
#endif
        }

        public static bool isFirstTime()
        {
#if UNITY_EDITOR
            return true;
#elif UNITY_IOS || UNITY_IPHONE
			return _isFirstTime();
#elif UNITY_ANDROID
		//warning Not implemented as of BFG SDK 6.0
            throw new NotImplementedException();
#else
			return false;
#endif
        }

        public static bool isInitialized()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return _isInitialized();
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {result = ajc.CallStatic<bool>("isInitialized");}
			return result;
#else
			return false;
#endif
        }

        public static void showMoreGames()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			_showMoreGames();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ajc.CallStatic("showMoreGames");}
#else
			return;
#endif
        }

        public static void showSupport()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			_showSupport();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ajc.CallStatic("showSupport");}
#else
			return;
#endif
        }

        public static void showPrivacy()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			_showPrivacy();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ajc.CallStatic("showPrivacy");}
#else
			return;
#endif
        }

        public static void showTerms()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			_showTerms();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ajc.CallStatic("showTerms");}
#else
			return;
#endif
        }

        public static void showWebBrowser(string startPage)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			_showWebBrowser(startPage);
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ajc.CallStatic("showWebBrowser", startPage);}
#else
			return;
#endif
        }

        public static void removeWebBrowser()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			_removeWebBrowser();
#elif UNITY_ANDROID
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ajc.CallStatic("removeWebBrowser");}
#else
			return;
#endif
        }

        public static bool checkForInternetConnection()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return _checkForInternetConnection();
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {result = ajc.CallStatic<bool>("checkForInternetConnection");}
			return result;
#else
			return false;
#endif
        }

        public static bool checkForInternetConnectionAndAlert(bool displayAlert)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return _checkForInternetConnectionAndAlert(displayAlert);
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {result = ajc.CallStatic<bool>("checkForInternetConnectionAndAlert", displayAlert);}
			return result;
#else
			return false;
#endif
        }

        public static bool startBranding()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return _startBranding();
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {result = ajc.CallStatic<bool>("startBranding");}
			return result;
#else
			return false;
#endif
        }

        public static void stopBranding()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			_stopBranding();
#elif UNITY_ANDROID
		//warning Removed in Big Fish Android SDK 5.6
            throw new NotImplementedException();
#else
			return;
#endif
        }

        public static void addPauseResumeDelegate()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			_addPauseResumeDelegate();
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return;
#endif
        }

        public static void removePauseResumeDelegate()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			_removePauseResumeDelegate();
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return;
#endif
        }

        public static bool isPaused()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return _isPaused();
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return false;
#endif
        }

        //  Deprecated in Big Fish iOS SDK 5.10 and Android SDK 5.6
        /*
        public static bool adsRunning()
        {
            #if UNITY_EDITOR
                return false;
            #elif UNITY_IOS || UNITY_IPHONE
                return _adsRunning();
            #elif UNITY_ANDROID
                bool result;
                using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {result = ajc.CallStatic<bool>("adsRunning");}
                return result;
            #else
                return false;
            #endif
        }

        public static bool startAds()
        {
            #if UNITY_EDITOR
                return false;
            #elif UNITY_IOS || UNITY_IPHONE
                return _startAds();
            #elif UNITY_ANDROID
                bool result;
                using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {result = ajc.CallStatic<bool>("startAds");}
                return result;
            #else
                return false;
            #endif
        }

        public static void stopAds()
        {
            #if UNITY_EDITOR
                return;
            #elif UNITY_IOS || UNITY_IPHONE
                _stopAds();
            #elif UNITY_ANDROID
                using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ajc.CallStatic("stopAds");}
            #else
                return;
            #endif
        }
    */

        public static bool launchSDKByURLScheme(string urlScheme)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			return _launchSDKByURLScheme(urlScheme);
#elif UNITY_ANDROID
			bool result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {result = ajc.CallStatic<bool>("launchSDKByURLScheme", urlScheme);}
			return result;
#else
			return false;
#endif
        }

        // Big Fish iOS SDK 5.7

        public static bool getDebugDictionary(ref string jsonDebugDictionary)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
			int sbSize = 1024;
			StringBuilder sb = new StringBuilder(sbSize);

			int requiredSize = _getDebugDictionary(sb, sbSize);
			if (sbSize <= requiredSize)
			{
				sbSize = requiredSize + 1;
				sb = new StringBuilder(sbSize);
				requiredSize = _getDebugDictionary(sb, sbSize);
			}

			if (sbSize <= requiredSize)
			{
				return false;
			}

			jsonDebugDictionary = sb.ToString();
			return true;
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return false;
#endif
        }

        public static void setDebugDictionary(string jsonDebugDictionary)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			_setDebugDictionary(jsonDebugDictionary);
#elif UNITY_ANDROID
			throw new NotImplementedException();
#else
			return;
#endif
        }

        public static bfgUtils.bfgRect createCcsButtonBounds(float widthPercentage, bfgUtils.bfgAnchorLocation horizontalAnchor, bfgUtils.bfgAnchorLocation verticalAnchor)
        {
#if UNITY_EDITOR
            return null;
#elif UNITY_IOS || UNITY_IPHONE
		StringBuilder btnBoundsJson = new StringBuilder (500);
		ButtonBounds buttonBounds;
		if (_createCCSButtonBounds (widthPercentage, horizontalAnchor.ToString (), verticalAnchor.ToString (), btnBoundsJson))
		{
			string btnBoundsJsonString = btnBoundsJson.ToString ();
			Debug.Log ("createCcsButtonBounds for btnBoundsJsonString: " + btnBoundsJsonString);
			buttonBounds = ButtonBounds.CreateFromJSON (btnBoundsJsonString);
		}
		else
		{
			throw new Exception ("ERROR creating the ccsRectBoundsArray. Either the horizontalAnchor or verticalAnchor are no longer valid with this iOS SDK version.\nhorizontalAnchor param: " + horizontalAnchor.ToString () + "\nverticalAnchor param: " + verticalAnchor.ToString ());
		}
		return new bfgUtils.bfgRect (buttonBounds.x, buttonBounds.y, buttonBounds.width, buttonBounds.height);
#elif UNITY_ANDROID
		float[] ccsRectBoundsArray;
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ccsRectBoundsArray = ajc.CallStatic<float[]>("createCcsButtonBounds", widthPercentage, horizontalAnchor.ToString(), verticalAnchor.ToString());}
		return new bfgUtils.bfgRect(ccsRectBoundsArray[0], ccsRectBoundsArray[1], ccsRectBoundsArray[2], ccsRectBoundsArray[3]);
#else
		return null;
#endif
        }

        public static void hideCcsButton()
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
		_hideCCSButton();
#elif UNITY_ANDROID
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ajc.CallStatic("hideCcsButton");}
#else
		return;
#endif
        }

        public static bool isShowingCcsButton()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
		bool showing = _isShowingCCSButton();
		return showing;
#elif UNITY_ANDROID
		bool showing;
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {showing = ajc.CallStatic<bool>("isShowingCcsButton");}
		return showing;
#else
		return false;
#endif
        }

        public static void showCcsButton(bfgUtils.bfgRect buttonBounds)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
		ButtonBounds buttonBoundsObject = new ButtonBounds ((int)buttonBounds.x, (int)buttonBounds.y, (int)buttonBounds.w, (int)buttonBounds.h);
		_showCcsButton(buttonBoundsObject.SaveToString ());
#elif UNITY_ANDROID
		float[] buttonBoundsArray = {buttonBounds.x, buttonBounds.y, buttonBounds.w, buttonBounds.h};
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ajc.CallStatic("showCcsButton", buttonBoundsArray);}
#else
		return;
#endif
        }

        public static void showCcsButton(bfgUtils.bfgRect buttonBounds, string gameLocation)
        {

#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
		ButtonBounds buttonBoundsObject = new ButtonBounds ((int)buttonBounds.x, (int)buttonBounds.y, (int)buttonBounds.w, (int)buttonBounds.h);
		_showCcsButtonLocation(buttonBoundsObject.SaveToString (), gameLocation);
#elif UNITY_ANDROID
		float[] buttonBoundsArray = {buttonBounds.x, buttonBounds.y, buttonBounds.w, buttonBounds.h};
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ajc.CallStatic("showCcsButton", buttonBoundsArray, gameLocation);}
#else
		return;
#endif
        }

        public static void showCcsButtonWithPercent(float xPercent, float yPercent, float widthPercent, float heightPercent, string location)
        {

#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
            if ((xPercent > 1 || xPercent < 0) || 
            (yPercent > 1 || yPercent < 0) ||
            (widthPercent > 1 || widthPercent < 0) ||
            (heightPercent > 1 || heightPercent < 0))
            {
                throw new Exception("percentages must be between 0-1");
            }
            _showCcsButtonLocationWithPercent(xPercent, yPercent, widthPercent, heightPercent, location);
#elif UNITY_ANDROID
            if ((xPercent > 1 || xPercent < 0) || 
            (yPercent > 1 || yPercent < 0) ||
            (widthPercent > 1 || widthPercent < 0) ||
            (heightPercent > 1 || heightPercent < 0))
            {
                throw new Exception("percentages must be between 0-1");
            }
            int deviceWidth = Screen.width;
            int deviceHeight = Screen.height;       
            bfgUtils.bfgRect buttonBounds = new bfgUtils.bfgRect(deviceWidth * xPercent, deviceHeight * yPercent, deviceWidth * widthPercent, deviceHeight * heightPercent);            
            showCcsButton(buttonBounds, "test");
#else
            return;
#endif
        }

        public static void showCcsButtonWithPixels(int x, int y, int width, int height, string location)
        {

#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
            _showCcsButtonLocationWithPixels(x, y, width, height, location);
#elif UNITY_ANDROID
            bfgUtils.bfgRect rect = new bfgUtils.bfgRect(x, y, width, height);
            showCcsButton(rect, location);
#else
            return;
#endif
        }


        /// <summary>
        /// Adds the policy listener. This method internally creates a notification observer for bfgCommon.BFG_POLICY_LISTENER_WILLSHOWPOLICIES and bfgCommon.BFG_POLICY_LISTENER_ONPOLICIESCOMPLETED.
        /// To remove the policy listeners, please call removePolicyListener with the notification observers you have created.
        /// </summary>
        /// <param name="willShowPolicies">The listener for willShowPolicies.</param>
        /// <param name="onPoliciesCompleted">The listener for onPoliciesCompleted.</param>
        public static void addPolicyListener(NotificationHandler willShowPolicies, NotificationHandler onPoliciesCompleted)
        {
            NotificationCenter.Instance.AddObserver(willShowPolicies, bfgCommon.BFG_POLICY_LISTENER_WILLSHOWPOLICIES);
            NotificationCenter.Instance.AddObserver(onPoliciesCompleted, bfgCommon.BFG_POLICY_LISTENER_ONPOLICIESCOMPLETED);
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
			// To complete adding the policy listener, call BfgPolicyListenerWrapper__setBfgPolicyListener(); in application:didFinishLaunchingWithOptions: of UnityAppController.mm
#elif UNITY_ANDROID
			// No extra code required to add the policy listener. BFGUnityPlayerNativeActivity.java does this automatically
#else
			return;
#endif
        }

        /// <summary>
        /// Removes the policy listener.
        /// </summary>
        /// <param name="willShowPolicies">The listener for willShowPolicies.</param>
        /// <param name="onPoliciesCompleted">The listener for onPoliciesCompleted.</param>
        public static void removePolicyListener(NotificationHandler willShowPolicies, NotificationHandler onPoliciesCompleted)
        {
            if (NotificationCenter.Instance.HandlerSetHasObserver(willShowPolicies, bfgCommon.BFG_POLICY_LISTENER_WILLSHOWPOLICIES))
            {
                NotificationCenter.Instance.RemoveObserver(willShowPolicies, bfgCommon.BFG_POLICY_LISTENER_WILLSHOWPOLICIES);
            }
            else
            {
                Debug.Log("No instance of willShowPolicies in the NotificationHandlerSet for: " + bfgCommon.BFG_POLICY_LISTENER_WILLSHOWPOLICIES);
            }
            if (NotificationCenter.Instance.HandlerSetHasObserver(onPoliciesCompleted, bfgCommon.BFG_POLICY_LISTENER_ONPOLICIESCOMPLETED))
            {
                NotificationCenter.Instance.RemoveObserver(onPoliciesCompleted, bfgCommon.BFG_POLICY_LISTENER_ONPOLICIESCOMPLETED);
            }
            else
            {
                Debug.Log("No instance of onPoliciesCompleted in the NotificationHandlerSet for: " + bfgCommon.BFG_POLICY_LISTENER_ONPOLICIESCOMPLETED);
            }
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
		_removePolicyListener();
#elif UNITY_ANDROID
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ajc.CallStatic("removePolicyListener");}
#else
		return;
#endif
        }

        public static bool didAcceptPolicyControl(string policyControl)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS || UNITY_IPHONE
		return _didAcceptPolicyControl(policyControl);
#elif UNITY_ANDROID
		bool didAcceptPC;
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {didAcceptPC = ajc.CallStatic<bool>("didAcceptPolicyControl", policyControl);}
		return didAcceptPC;
#else
		return false;
#endif
        }


        public static void setLimitEventAndDataUsage(bool limitData)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_IOS || UNITY_IPHONE
		_setLimitEventAndDataUsage(limitData);
#elif UNITY_ANDROID
		using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgManagerUnityWrapper")) {ajc.CallStatic("setLimitEventAndDataUsage", limitData);}
#else
		return;
#endif
        }
    }
}