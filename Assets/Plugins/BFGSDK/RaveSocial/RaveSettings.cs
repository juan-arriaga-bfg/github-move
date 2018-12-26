using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class RaveSettings {
	public class General {
		public static string ApplicationID { get { return GetSetting("RaveSettings.General.ApplicationID"); } set { SetSetting("RaveSettings.General.ApplicationID", value); } }
		public static string ServerURL { get { return GetSetting("RaveSettings.General.ServerURL"); } set { SetSetting("RaveSettings.General.ServerURL", value); } }
		public static string AutoSyncInterval { get { return GetSetting("RaveSettings.General.AutoSyncInterval"); } set { SetSetting("RaveSettings.General.AutoSyncInterval", value); } }
		public static string AutoGuestLogin { get { return GetSetting("RaveSettings.General.AutoGuestLogin"); } set { SetSetting("RaveSettings.General.AutoGuestLogin", value); } }
		public static string AutoCrossAppLogin { get { return GetSetting("RaveSettings.General.AutoCrossAppLogin"); } set { SetSetting("RaveSettings.General.AutoCrossAppLogin", value); } }
		public static string AutoInstallConfigUpdates { get { return GetSetting("RaveSettings.General.AutoInstallConfigUpdates"); } set { SetSetting("RaveSettings.General.AutoInstallConfigUpdates", value); } }
		public static string LogLevel { get { return GetSetting("RaveSettings.General.LogLevel"); } set { SetSetting("RaveSettings.General.LogLevel", value); } }
		public static string AllowForceDisconnect { get { return GetSetting("RaveSettings.General.AllowForceDisconnect"); } set { SetSetting("RaveSettings.General.AllowForceDisconnect", value); } }
		public static string NetworkTimeout { get { return GetSetting("RaveSettings.General.NetworkTimeout"); } set { SetSetting("RaveSettings.General.NetworkTimeout", value); } }
		public static string PhoneContactsUpdateInterval { get { return GetSetting("RaveSettings.General.PhoneContactsUpdateInterval"); } set { SetSetting("RaveSettings.General.PhoneContactsUpdateInterval", value); } }
		public static string ConfigUpdateInterval { get { return GetSetting("RaveSettings.General.ConfigUpdateInterval"); } set { SetSetting("RaveSettings.General.ConfigUpdateInterval", value); } }
		public static string DefaultNewUserName { get { return GetSetting("RaveSettings.General.DefaultNewUserName"); } set { SetSetting("RaveSettings.General.DefaultNewUserName", value); } }
		public static string DefaultResourcesPath { get { return GetSetting("RaveSettings.General.DefaultResourcesPath"); } set { SetSetting("RaveSettings.General.DefaultResourcesPath", value); } }
		public static string ThirdPartySource { get { return GetSetting("RaveSettings.General.ThirdPartySource"); } set { SetSetting("RaveSettings.General.ThirdPartySource", value); } }
		public static string AutoSendGiftOnGrantedRequest { get { return GetSetting("RaveSettings.General.AutoSendGiftOnGrantedRequest"); } set { SetSetting("RaveSettings.General.AutoSendGiftOnGrantedRequest", value); } }
		public static string SceneServerHost { get { return GetSetting("RaveSettings.General.SceneServerHost"); } set { SetSetting("RaveSettings.General.SceneServerHost", value); } }
	}

	public class WelcomeBackToast {
		public static string ShowOnAppStartup { get { return GetSetting("RaveSettings.WelcomeBackToast.ShowOnAppStartup"); } set { SetSetting("RaveSettings.WelcomeBackToast.ShowOnAppStartup", value); } }
		public static string ShowOnAuthentication { get { return GetSetting("RaveSettings.WelcomeBackToast.ShowOnAuthentication"); } set { SetSetting("RaveSettings.WelcomeBackToast.ShowOnAuthentication", value); } }
		public static string ShowOnPersonalization { get { return GetSetting("RaveSettings.WelcomeBackToast.ShowOnPersonalization"); } set { SetSetting("RaveSettings.WelcomeBackToast.ShowOnPersonalization", value); } }
	}

	public class Facebook {
		public static string AutoUpdateContactsOnConnect { get { return GetSetting("RaveSettings.Facebook.AutoUpdateContactsOnConnect"); } set { SetSetting("RaveSettings.Facebook.AutoUpdateContactsOnConnect", value); } }
		public static string ContactsUpdateInterval { get { return GetSetting("RaveSettings.Facebook.ContactsUpdateInterval"); } set { SetSetting("RaveSettings.Facebook.ContactsUpdateInterval", value); } }
		public static string ApplicationId { get { return GetSetting("RaveSettings.Facebook.ApplicationId"); } set { SetSetting("RaveSettings.Facebook.ApplicationId", value); } }
		public static string ReadPermissions { get { return GetSetting("RaveSettings.Facebook.ReadPermissions"); } set { SetSetting("RaveSettings.Facebook.ReadPermissions", value); } }
		public static string AlwaysUseLiveContacts { get { return GetSetting("RaveSettings.Facebook.AlwaysUseLiveContacts"); } set { SetSetting("RaveSettings.Facebook.AlwaysUseLiveContacts", value); } }
		public static string UseGraphAPIv1 { get { return GetSetting("RaveSettings.Facebook.UseGraphAPIv1"); } set { SetSetting("RaveSettings.Facebook.UseGraphAPIv1", value); } }
	}

	public class Twitter {
		public static string ConsumerKey { get { return GetSetting("RaveSettings.Twitter.ConsumerKey"); } set { SetSetting("RaveSettings.Twitter.ConsumerKey", value); } }
		public static string ConsumerSecret { get { return GetSetting("RaveSettings.Twitter.ConsumerSecret"); } set { SetSetting("RaveSettings.Twitter.ConsumerSecret", value); } }
		public static string CallbackURL { get { return GetSetting("RaveSettings.Twitter.CallbackURL"); } set { SetSetting("RaveSettings.Twitter.CallbackURL", value); } }
		public static string AllowWebAuthFallback { get { return GetSetting("RaveSettings.Twitter.AllowWebAuthFallback"); } set { SetSetting("RaveSettings.Twitter.AllowWebAuthFallback", value); } }
	}

	public class GooglePlus {
		public static string AutoUpdateContactsOnConnect { get { return GetSetting("RaveSettings.GooglePlus.AutoUpdateContactsOnConnect"); } set { SetSetting("RaveSettings.GooglePlus.AutoUpdateContactsOnConnect", value); } }
		public static string ContactsUpdateInterval { get { return GetSetting("RaveSettings.GooglePlus.ContactsUpdateInterval"); } set { SetSetting("RaveSettings.GooglePlus.ContactsUpdateInterval", value); } }
		public static string ClientID { get { return GetSetting("RaveSettings.GooglePlus.ClientID"); } set { SetSetting("RaveSettings.GooglePlus.ClientID", value); } }
		public static string ShareContentURL { get { return GetSetting("RaveSettings.GooglePlus.ShareContentURL"); } set { SetSetting("RaveSettings.GooglePlus.ShareContentURL", value); } }
	}

	public class IOS {
		public static string InitGameCenterOnStartUp { get { return GetSetting("RaveSettings.IOS.InitGameCenterOnStartUp"); } set { SetSetting("RaveSettings.IOS.InitGameCenterOnStartUp", value); } }
		public static string BundleName { get { return GetSetting("RaveSettings.IOS.BundleName"); } set { SetSetting("RaveSettings.IOS.BundleName", value); } }
		public static string UseIDFAForDeviceID { get { return GetSetting("RaveSettings.IOS.UseIDFAForDeviceID"); } set { SetSetting("RaveSettings.IOS.UseIDFAForDeviceID", value); } }
	}
	
	public static string GetSetting(string settingName) {
		return RaveSettingsGetSetting(settingName);
	}
	#if UNITY_EDITOR
	private static string RaveSettingsGetSetting(string settingName) {return "";}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern string RaveSettingsGetSetting(string settingName);
	#elif UNITY_ANDROID
	private static string RaveSettingsGetSetting(string settingName) {
		object[] args = new object[1];
		args[0] = settingName;

		AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics");
		return rsClass.CallStatic<string>("RaveSettingsGetSetting",args);
	}
	#endif

	public static void SetSetting(string settingName, string value) {
		RaveSettingsSetSetting(settingName, value);
	}
	#if UNITY_EDITOR
	private static void RaveSettingsSetSetting(string settingName,string value) {}
	#elif UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void RaveSettingsSetSetting(string settingName,string value);
	#elif UNITY_ANDROID
	private static void RaveSettingsSetSetting(string settingName,string value) {
		object[] args = new object[2];
		args[0] = settingName;
		args[1] = value;

		AndroidJavaClass rsClass = new AndroidJavaClass("co.ravesocial.unity.RaveSocialStatics");
		rsClass.CallStatic("RaveSettingsSetSetting",args);
	}
	#endif
}