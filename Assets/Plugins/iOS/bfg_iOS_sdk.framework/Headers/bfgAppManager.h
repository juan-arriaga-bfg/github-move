///
/// \file bfgAppManager.h
/// \brief Installing and launching other apps
///
/// \since 4.6
///
// \author John Starin
// \date 10/15/2013
// \copyright (c) 2013 Big Fish Games, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

#pragma mark - Defines for App Detection

///
/// \details This notification is posted when an app has been detected to be installed after calling \b startDetectingAppWithIdentifier:
///
/// \since 4.6
///
#define BFG_NOTIFICATION_APP_DETECTED @"BFG_NOTIFICATION_APP_DETECTED"

///
/// \details This is the key used to retrieve the bundle identifier of the detected app from the userInfo dictionary of the BFG_NOTIFICATION_APP_DETECTED notification.
///
/// \since 4.6
///
#define BFG_BUNDLE_IDENTIFIER_KEY @"identifier"

///
/// \details Bundle identifier of the Big Fish Games App: @"com.bigfishgames.gamefinder"
///
/// \since 4.6
///
#define BFG_BIG_FISH_GAMES_BUNDLE_ID @"com.bigfishgames.gamefinder"


#pragma mark - Defines for App Store events

///
/// \details This notification is posted when the App Store application will be opened because
/// the in-game App Store was not available.
///
/// \since 4.6
///
#define BFG_NOTIFICATION_OPENING_APP_STORE @"BFG_NOTIFICATION_OPENING_APP_STORE"

///
/// \details This notification is posted when the App Store view controller will be presented
///
/// \since 4.6
///
#define BFG_NOTIFICATION_APP_STORE_WILL_PRESENT @"BFG_NOTIFICATION_APP_STORE_WILL_PRESENT"

///
/// \details This notification is posted when the App Store view controller has been presented (iOS 6+).
///
/// \since 4.6
///
#define BFG_NOTIFICATION_APP_STORE_PRESENTED @"BFG_NOTIFICATION_APP_STORE_PRESENTED"

///
/// \details This notification is posted when the App Store view controller has been dismissed (iOS 6+).
///
/// \since 4.6
///
#define BFG_NOTIFICATION_APP_STORE_DISMISSED @"BFG_NOTIFICATION_APP_STORE_DISMISSED"


#pragma mark - Defines for Referral URLs

///
/// \details This notification is posted when bfgAppManager has started to open a referral URL.
///
/// \since 4.6
///
#define BFG_NOTIFICATION_OPEN_REFERRAL_URL_STARTED @"BFG_NOTIFICATION_OPEN_REFERRAL_URL_STARTED"

///
/// \details This notification is posted when bfgAppManager was able to extract an app ID from a referral URL and will attempt to open that app in the App Store.
///
/// \since 4.6
///
#define BFG_NOTIFICATION_OPEN_REFERRAL_URL_SUCCEEDED @"BFG_NOTIFICATION_OPEN_REFERRAL_URL_SUCCEEDED"

///
/// \details This notification is posted when bfgAppManager is unable to extract an app ID from the referral URL.
///
/// \since 4.6
///
#define BFG_NOTIFICATION_OPEN_REFERRAL_URL_FAILED @"BFG_NOTIFICATION_OPEN_REFERRAL_URL_FAILED"

///
/// \details This is the key used to retrieve the NSError (if any) for a connection failure from the userInfo dictionary of the BFG_NOTIFICATION_OPEN_REFERRAL_URL_FAILED notification. The userInfo dictionary may be nil if there was not an error.
///
/// \since 4.6
///
#define BFG_OPEN_REFERAL_URL_ERROR_KEY @"error"


#pragma mark - bfgAppManager

///
/// \brief Installing and launching other apps.
///
@interface bfgAppManager : NSObject


#pragma mark - Launch Apps

///
/// Launches an installed app.
///
/// \details Will launch an app if it is installed on the device.
/// \since 4.6
///
/// \param bundleIdentifier Bundle identifier of app to launch. Example: \@"com.bigfishgames.gamefinder"
///
/// \retval YES if the app was successfully launched.
/// \retval NO if the app is not installed.
///
+ (BOOL)launchApp:(NSString *)bundleIdentifier;

///
/// Launches an installed app and passes a parameter string to the app.
///
/// \since 4.6
///
/// \param bundleIdentifier Bundle identifier of app to launch. Example: \@"com.bigfishgames.gamefinder"
/// \param parameterString Parameter string that is passed to the app being launched. Example \@"openGuide?index=5"
///
/// \retval YES if the app was successfully launched.
/// \retval NO if the app is not installed.
///
+ (BOOL)launchApp:(NSString *)bundleIdentifier withParams:(NSString * _Nullable)parameterString;


#pragma mark - Install Apps

///
/// Checks if an app is installed.
///
/// \since 4.6
/// \updated 5.9
///
/// \param bundleIdentifier Bundle identifier of app to check. Example: \@"com.bigfishgames.gamefinder"
/// \param error Will return an error if the target app is not whitelisted. See LSApplicationQueriesSchemes
///
/// \retval YES if the app is installed.
/// \retval NO if the app is not installed.
///
+ (BOOL)isAppInstalled:(NSString *)bundleIdentifier error:(NSError * __autoreleasing * _Nullable)error;

///
/// Presents app in the App Store for user to install.
///
/// \details Presents a modal view of the App Store.
/// \since 4.6
///
/// \param appID App's iTunes identifier. This number can be found at http://linkmaker.itunes.apple.com and is a string of numbers. For example, the iTunes identifier for the iBooks app is 364709193.
+ (void)launchStoreWithApp:(NSString *)appID;



#pragma mark - Big Fish Games App

///
/// Determines if the Big Fish Games App is installed.
///
/// \since 4.6
/// \updated 5.9
///
/// \param error Will return an error if the target app is not whitelisted. See LSApplicationQueriesSchemes
///
/// \retval YES if the Big Fish Games App was detected.
/// \retval NO if app not installed.
///
+ (BOOL)isBigFishGamesAppInstalled:(NSError * __autoreleasing * _Nullable)error;

///
/// Presents the Big Fish Games App in an App Store view.
///
/// \details openReferralURL: is invoked with the correct URL for the user's language. @see openReferralURL: for details on expected behavior. This method does not automatically begin detecting if the Big Fish Games App has been installed.
/// \since 4.6
///
+ (BOOL)launchStoreWithBigFishGamesApp;

///
/// Launches or installs the Big Fish Games App.
///
/// \details If the Big Fish Games App is not installed, a system dialog is presented to the user asking them to download the app.
/// \since 4.9
/// \updated 5.9
///
/// \retval YES if app was launched.
/// \retval NO if app not installed.
///
+ (BOOL)launchOrInstallBigFishGamesApp;


///
/// Launches the Big Fish Games App and opens the strategy guide.
///
/// \details If the Big Fish Games App is not installed, a system dialog is presented to the user asking them to download the app.
/// \since 4.6
/// \updated 5.9
///
/// \param wrappingID Wrapping ID of the game.
///
/// \retval YES if app was launched.
/// \retval NO if app not installed.
///
+ (BOOL)launchBigFishGamesAppStrategyGuideWithWrappingID:(NSString *)wrappingID;

///
/// Launches the Big Fish Games App and opens the strategy guide to a specific chapter and page.
///
/// \details If the Big Fish Games App is not installed, an interstitial is presented to the user asking them to download the app.
/// \since 4.6
/// \updated 5.10
///
/// \param wrappingID Wrapping ID of the game.
/// \param chapterIndex Index of strategy guide chapter.
/// \param pageIndex Index of strategy guide page in chapter.
///
/// \retval YES if app was launched.
/// \retval NO if app not installed.
///
+ (BOOL)launchBigFishGamesAppStrategyGuideWithWrappingID:(NSString *)wrappingID chapterIndex:(NSUInteger)chapterIndex pageIndex:(NSUInteger)pageIndex;

///
/// \details  Launches the Big Fish Games App and opens the forums to the forum specified
///  in your settings json (URL encoded). Key: "forum_identifier"
///
/// \note Specify a tracking URL using key "forum_tracking_url" in your settings file if
/// your producer asks you to do game specific forums tracking.
///
/// \since 5.10
///
/// \retval YES if app was launched.
/// \retval NO if app not launched.
///
+ (BOOL)launchBigFishGamesAppWithForum;

///
/// \details  Launches the Big Fish Games App and opens the forums to a specific forum.
/// \since 5.10
///
/// \note Specify a tracking URL using key "forum_tracking_url" in your settings file if
/// your producer asks you to do game specific forums tracking.
///
/// \param forumId ID of forum to open in the Big Fish Games App (URL encoded)
///
/// \retval YES if app was launched.
/// \retval NO if app not launched.
///
+ (BOOL)launchBigFishGamesAppWithForum:(NSString *)forumId;


#pragma mark - Referral URLs

///
/// Opens a referral link and presents the App Store view (iOS 6+) or switches to the App Store (iOS 5).
///
/// \details A URL connection is made in the background and redirects are followed. When an App Store link is detected, the app ID is extracted and an App Store view is used to present the app in-game (iOS 6), or the user is switched to the App Store to view the app (iOS 5).
/// \since 4.6
///
/// \retval YES if the URL connection has started successfully.
/// \retval NO if the URL failed to start or the URL does not match one of the supported referral domains.
///
+ (BOOL)openReferralURL:(NSURL *)url;

///
/// Attempts to cancel the current referral URL that was started with \b openReferralURL:
///
/// \since 4.6
///
+ (void)cancelCurrentReferral;

@end

NS_ASSUME_NONNULL_END
