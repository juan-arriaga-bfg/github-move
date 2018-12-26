using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class bfgSimplePurchase
{
	public enum BFGPaywallOrigin
	{
		BFGPaywallOriginMainMenu,
		BFGPaywallOriginEndOfGameplay
	}
	#if UNITY_EDITOR
	// Nothing to see here.
	#elif UNITY_IOS || UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern bool __bfgSimplePurchase__isPurchased();

	[DllImport("__Internal")]
	private static extern void __bfgSimplePurchase__startServiceWithDelegate();

	[DllImport("__Internal")]
	private static extern void __bfgSimplePurchase__showPaywallUIFrom(int origin);

	[DllImport("__Internal")]
	private static extern void __bfgSimplePurchase__showsubscriptionProfile();

	[DllImport("__Internal")]
	private static extern void __bfgSimplePurchase__subscriptionProfileButton();

	[DllImport("__Internal")]
	private static extern void __bfgSimplePurchase__subscriptionProfileButtonImageNormal ();

	[DllImport("__Internal")]
	private static extern void __bfgSimplePurchase__subscriptionProfileButtonImageHighlighted ();

#endif

	public static bool isPurchased ()
	{
		#if UNITY_EDITOR
		return false;
		#elif UNITY_IOS || UNITY_IPHONE
			return __bfgSimplePurchase__isPurchased();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return false;
		#endif
	}

	// Registers predefined wrapper bfgSimplePurchase delegate
	public static void startServiceWithDelegate ()
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
			// Observer bfgCommon.NOTIFICATION_SIMPLEPURCHASE_... notifications for forwarded delegate method calls

			__bfgSimplePurchase__startServiceWithDelegate();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return;
		#endif
	}

	public static void showPaywallUIFrom (BFGPaywallOrigin origin)
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
		if (origin == BFGPaywallOrigin.BFGPaywallOriginEndOfGameplay)
		{
			__bfgSimplePurchase__showPaywallUIFrom(1);
		}else{
			__bfgSimplePurchase__showPaywallUIFrom(0);
		}
		#elif UNITY_ANDROID
		throw new NotImplementedException();
		#else
		return;
		#endif
	}

	public static void showSubscriptionProfile ()
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
		__bfgSimplePurchase__showsubscriptionProfile();
		#elif UNITY_ANDROID
		throw new NotImplementedException();
		#else
		return;
		#endif
	}

	public static void subscriptionProfileButton ()
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
		__bfgSimplePurchase__subscriptionProfileButton();
		#elif UNITY_ANDROID
		throw new NotImplementedException();
		#else
		return;
		#endif
	}

	public static void subscriptionProfileButtonImageNormal ()
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
		__bfgSimplePurchase__subscriptionProfileButtonImageNormal();
		#elif UNITY_ANDROID
		throw new NotImplementedException();
		#else
		return;
		#endif
	}

	public static void subscriptionProfileButtonImageHighlighted ()
	{
		#if UNITY_EDITOR
		return;
		#elif UNITY_IOS || UNITY_IPHONE
		__bfgSimplePurchase__subscriptionProfileButtonImageHighlighted ();
		#elif UNITY_ANDROID
		throw new NotImplementedException();
		#else
		return;
		#endif
	}

}
