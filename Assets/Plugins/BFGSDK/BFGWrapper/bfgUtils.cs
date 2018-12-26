using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public class bfgUtils
{
	#if UNITY_EDITOR
	// Nothing to see here.
	#elif UNITY_IOS || UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void __bfgutils__bfgUDID( StringBuilder returnBFGUDID, int size );

	// Big Fish iOS SDK 5.8
	[DllImport("__Internal")]
	private static extern int __bfgutils__bfgIDFV( StringBuilder returnIDFV, int size );
#endif

	public static string bfgUDID ()
	{
		#if UNITY_EDITOR
		return null;
		#elif UNITY_IOS || UNITY_IPHONE
			StringBuilder sb = new StringBuilder(41);
			__bfgutils__bfgUDID(sb, sb.Capacity);
			return sb.ToString();
		#elif UNITY_ANDROID
			string result;
			using (AndroidJavaClass ajc = new AndroidJavaClass("com.bigfishgames.bfgunityandroid.bfgUtilsUnityWrapper")) {result = ajc.CallStatic<string>("bfgUDID");}
			return result;
		#else
			return null;
		#endif
	}

	// Big Fish iOS SDK 5.8

	public static string bfgIDFV ()
	{
		#if UNITY_EDITOR
		return null;
		#elif UNITY_IOS || UNITY_IPHONE
			int sbSize = 41;
			StringBuilder sb = new StringBuilder(sbSize);

			int requiredSize = __bfgutils__bfgIDFV(sb, sbSize);
			if (sbSize <= requiredSize)
			{
				sbSize = requiredSize + 1;
				sb = new StringBuilder(sbSize);
				requiredSize = __bfgutils__bfgIDFV(sb, sbSize);
			}

			if (sbSize <= requiredSize)
			{
				return null;
			}

			return sb.ToString();
		#elif UNITY_ANDROID
			throw new NotImplementedException();
		#else
			return null;
		#endif
	}

	public enum bfgAnchorLocation
	{
		TOP,
		CENTER,
		BOTTOM,
		LEFT,
		RIGHT
	}

	public class bfgRect
	{
		public float x;
		public float y;
		public float w;
		public float h;

		public bfgRect (float x, float y, float w, float h)
		{
			this.x = x;
			this.y = y;
			this.w = w;
			this.h = h;
		}
	}

	public interface bfgPolicyListener
	{
		void willShowPolicies ();

		void onPoliciesCompleted ();
	}
}
