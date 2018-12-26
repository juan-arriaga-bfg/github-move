//
//  RaveSocial.h
//
//  RaveSocial
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

/**
 Login status states
 */
typedef NS_ENUM(NSUInteger, RaveLoginStatus) {
    RaveLoggedIn,
    RaveLoggedOut,
    RaveLoginError,
};

/**
 *  Login status callback tracks when authentication state changes
 *
 *  @param status Status value, see RaveLoginStatus enum
 *  @param error  Error or nil if successful
 */
typedef void (^RaveLoginStatusCallback)(RaveLoginStatus status, NSError * error);

#import <bfg_iOS_sdk/RaveObject.h>
#import <bfg_iOS_sdk/RaveErrors.h>
#import <bfg_iOS_sdk/RaveSettings.h>
#import <bfg_iOS_sdk/RaveProgressDelegate.h>
#import <bfg_iOS_sdk/RaveErrorDelegate.h>
#import <bfg_iOS_sdk/RaveUI.h>
#import <bfg_iOS_sdk/RaveScene.h>
#import <bfg_iOS_sdk/RaveToast.h>
#import <bfg_iOS_sdk/RaveUtilities.h>
#import <bfg_iOS_sdk/RaveAuthenticationManager.h>
#import <bfg_iOS_sdk/RaveUsersManager.h>
#import <bfg_iOS_sdk/RaveContactsManager.h>
#import <bfg_iOS_sdk/RaveLocale.h>
#import <bfg_iOS_sdk/RaveMergePolicy.h>

//  Extensions
#import <bfg_iOS_sdk/UIView+Additions.h>
#import <bfg_iOS_sdk/NSString+Additions.h>
#import <bfg_iOS_sdk/NSRegularExpression+RSExtenstion.h>

/**
 Important: all objects must be accessed only from the main thread.
*/

/**
 *  Important forwarded objects
 */
@protocol RaveContactsManager;
@protocol RaveUsersManager;
@protocol RaveProgressDelegate;
@protocol RaveErrorDelegate;
@protocol RaveBusyDelegate;
@protocol RaveGiftsManager;
@protocol RaveLeaderboardManager;
@protocol RaveAchievementsManager;
@protocol RavePromotionsManager;
@protocol RaveSharingManager;
@protocol RaveResourcesProvider;
@protocol RaveAuthenticationManager;
@protocol RaveAppDataKeysManager;

/**
 RaveSocial interface
     Initialization methods, common accessors for Rave managers, and convenience methods for important functions
 */
@interface RaveSocial : NSObject
/**
 *  **New method**
 *  Call to initialize Rave when using FBSDKCoreKit for Facebook and call from your UIApplicationDelegate application:didFinishLaunchingWithOptions:
 *
 *  Will attempt to load config JSON from application bundle
 *
 *  @param launchOptions Launch options forwarded from UIApplication delegate, may be nil though that may introduce incorrect behavior from FBSDKCoreKit
 *  @param readyCallback Callback when Rave is ready after initializing, which may include some asynchronous operations
 */
+ (void)initializeRaveWithLaunchOptions:(NSDictionary *)launchOptions withReadyCallback:(RaveCompletionCallback)readyCallback;

/**
 *  **New method**
 *  Call to initialize Rave when using FBSDKCoreKit for Facebook and call from your UIApplicationDelegate application:didFinishLaunchingWithOptions:
 *
 *  @param configURL     URL for alternate path to JSON configuration for Rave settings, may be nil
 *  @param launchOptions Launch options forwarded from UIApplication delegate, may be nil though that may introduce incorrect behavior from FBSDKCoreKit
 *  @param readyCallback Callback when Rave is ready after initializing, which may include some asynchronous operations
 */
+ (void)initializeRaveWithConfig:(NSURL *)configURL withLaunchOptions:(NSDictionary *)launchOptions withReadyCallback:(RaveCompletionCallback)readyCallback;

/**
 *  Call to initialize Rave
 *
 *  Will attempt to load config JSON from application bundle
 *
 *  @param readyCallback Callback when Rave is ready after initializing, which may include some asynchronous operations
 */
+ (void)initializeRaveWithReadyCallback:(RaveCompletionCallback)readyCallback;

/**
 *  Call to initialize Rave
 *
 *  @param configURL     URL for alternate path to JSON configuration for Rave settings, may be nil
 *  @param readyCallback Callback when Rave is ready after initializing, which may include some asynchronous operations
 */
+ (void)initializeRaveWithConfig:(NSURL *)configURL withReadyCallback:(RaveCompletionCallback)readyCallback;

/**
 *  Call to initialize Rave
 *
 *  Will attempt to load config JSON from application bundle
 *
 */
+ (void)initializeRave;

/**
 *  Call to initialize Rave
 *
 *  @param configURL     URL for alternate path to JSON configuration for Rave settings, may be nil
 */
+ (void)initializeRaveWithConfig:(NSURL *)configURL;

/**
 *  **New Method**
 *
 *  Integration method to forward URL requests from the UIApplicationDelegate for internal Rave usage
 *
 *  **Typically must be called for proper Rave functioning (exceptions documented where warranted)**
 *
 *  @param url               URL forwarded from application delegate
 *  @param sourceApplication Source application forwarded from application delegate
 *  @param annotation        Annotation forwarded from application delegate
 *
 *  @return YES if Rave handled the URL request otherwise NO, typically should be returned from the UIApplicationDelegate
 */
+ (BOOL)handleURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;

/**
 *  Check if Rave is initialized after calling initializeRave
 *
 *  @return YES or NO depending on Rave's initialization status
 */
+ (BOOL)isInitialized;

/**
 *  Method accessing the version of the SDK
 *
 *  @return SDK version in the form V.X.Y-Z
 */
+ (NSString *)SDKVersion;

/**
 *  The device ID that Rave has assumed for the current device
 *
 *  @return String device ID
 */
+ (NSString *)deviceId;

/**
 *  Convenience method to check the most recently determined network state
 *
 *  @return YES or NO depending on network (WIFI or cell) availability
 */
+ (BOOL)isNetworkAvailable;

/**
 *  Convenience method to set a merge policy
 *
 *  This policy enables Rave's authenticated user merging feature. Without an implemented policy
 *  authenticated merging will not take place.  See RaveIdentiyPolicy.h for more details.
 *
 *  Suggested that this policy is set prior to calling initializeRave as it may be triggered
 *  during initialization
 *
 *  @param mergePolicy The object implimenting the RaveMergePolicy protocol.
 */
+ (void)setMergePolicy:(id<RaveMergePolicy>)mergePolicy;

/**
 *  Method for a rotation work-around for iOS8, typically will be utilized by Rave in scene packs
 *
 *  @return YES or NO based on whether a potential app switch may occur during the current operation
 */
+ (BOOL)expectingApplicationSwitch;

/**
 *  Convenience method to determine if there is a session for the current user
 *
 *  @return YES or NO depending on presence of session
 */
+ (BOOL)isLoggedIn;

/**
 *  Convenience method to determin if hte current user is a guest (e.g. anonymous)
 *
 *  @return YES if the current user is anonymous otherwise NO
 */
+ (BOOL)isLoggedInAsGuest;

/**
 *  Convenience method to determin if hte current user is a guest (e.g. anonymous)
 *
 *  @return YES if the current user is anonymous otherwise NO
 */
+ (BOOL)isAnonymous;
/**
 *  Convenience method to test if the current user is personalized (e.g. some user data has been customized but user is not authenticated)
 *
 *  @return YES if the user is personalized or NO
 */
+ (BOOL)isPersonalized;

/**
 *  Convenience method to test if the current user has been authenticated through some third party service (e.g. Facebook or Google+)
 *
 *  @return YES if the current user has been authenticated otherwise NO
 */
+ (BOOL)isAuthenticated;

/**
 * Method to get volatile Rave ID, will always return a UUID though the value may change after initialization completes
 */
+ (NSString *)volatileRaveId;

/**
 *  Convenience method to access the current user, e.g. RaveSocial.usersManager.currentUser
 *
 *  @return The current user
 */
+ (id<RaveUser>)currentUser;

/**
 *  Convenience method to login as an anonymous user, e.g. RaveSocial.authenticationManager.loginAsGuest
 *
 *  Do not call if RaveSettings.General.AutoGuestLogin is set to YES
 *
 *  @param callback Callback indicating success of operation
 */
+ (void)loginAsGuest:(RaveCompletionCallback)callback;

/**
 *  Convenience method to log out from Rave, typically only necessary if the current user is authenticated or personalized (though a personalized user will be abandoned)
 *
 *  @param callback Callback indicating that asynchronous access for log out has finished, typically other UI should be disabled when calling until the completion callback has been called. May be nil.
 */
+ (void)logOut:(RaveCompletionCallback)callback;

/**
 *  Set the global login status callback, allows you to track login status of the system across different API access that may happen directly or through scenes
 *
 *  @param callback Callback will indicate success or failure of log in and log out attempts
 */
+ (void)setLoginStatusCallback:(RaveLoginStatusCallback)callback;

/**
 *  Conveneience method to login to a Rave account authenticated by a third party network (e.g. Facebook or Google+)
 *
 *  Do not call if already authenticated as this will cause a change in Rave accounts
 *
 *  @param pluginKeyName One of several possibilities (see RaveUtilities.h for built-in constants, or Scene Pack for custom constants)
 *  @param callback     Callback indicating success or failure
 */
+ (void)loginWith:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback;

/**
 *  Convenience method to connect the current Rave account to an authentication network (e.g. Facebook or Google+)
 *
 *  May fail if attempting to connect an authentication source that is already associated with another Rave account
 *
 *  @param pluginKeyName One of several possibilities (see RaveUtilities.h for built-in constants, or Scene Pack for custom constants)
 *  @param callback     Callback indicating success or failure
 */
+ (void)connectTo:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback;

/**
 *  Convenience method to the cached readiness of the specified plugin.  A provider that is ready has already authenticated this account and can be used to post to the associated social network through Rave's own methods or through custom use of the associated network's own SDK
 *
 *  @param pluginKeyName One of several possibilities (see RaveUtilities.h for built-in constants, or Scene Pack for custom constants)
 *
 *  @return YES if the provider is ready otherwise NO
 */
+ (BOOL)isPluginReady:(NSString *)pluginKeyName;

/**
 *  Convenience method to recheck readiness of the associated plugin.  A provider that is ready has already authenticated this account and can be used to post to the associated social network through Rave's own methods or through custom use of the associated network's own SDK
 *
 *  @param pluginKeyName One of several possibilities (see RaveUtilities.h for built-in constants, or Scene Pack for custom constants)
 *  @param callback     Callback indicating whether a provider is ready and possibly an error
 */
+ (void)checkReadinessOf:(NSString *)pluginKeyName callback:(RaveReadinessCallback)callback;

/**
 *  Convenience method to disconnect an authentication source (e.g. Facebook or Google+) from the current account
 *
 *  Will not change the authentication state of this account, though if the current user disconnects all authentication sources they will not be able to access this account through another device.
 *
 *  @param pluginKeyName One of several possibilities (see RaveUtilities.h for built-in constants, or Scene Pack for custom constants)
 *  @param callback     Callback indicating whether the disconnect succeeded, ifluenced by the current disconnect policy (see RaveAuthenticationManager or your scene pack for more details on the RaveDisconnectPolicy)
 */
+ (void)disconnectFrom:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback;

/**
 *  Convenience method to access the current locale
 *
 *  @return The current locale
 */
+ (id<RaveLocale>)locale;

/**
 *  Rave manager accessors
 */
+ (id<RaveContactsManager>)contactsManager;

+ (id<RaveUsersManager>)usersManager;

+ (id<RaveGiftsManager>)giftsManager;

+ (id<RaveLeaderboardManager>)leaderboardsManager;

+ (id<RavePromotionsManager>)promotionsManager;

+ (id<RaveAchievementsManager>)achievementsManager;

+ (id<RaveSharingManager>)sharingManager;

+ (id<RaveAuthenticationManager>)authenticationManager;

+ (id<RaveAppDataKeysManager>)appDataKeysManager;

/**
 *  Accessor for the current progress delegate, will be used automatically by various API's when set
 *
 *  If you're integrated with a Scene Pack a default instance will be set
 *
 *  @return The current progress delegate instance or nil
 */
+ (id<RaveProgressDelegate>)progressDelegate;

/**
 *  Setter for the current progress delegate, will be used automatically by various API's when set
 *
 *  @param progressDelegate Progress delegate instance or nil
 */
+ (void)setProgressDelegate:(id<RaveProgressDelegate>)progressDelegate;

/**
 *  Accessor for the current error delegate, will be used automatically by various API's when set
 *
 *  If you're integrated with a Scene Pack a default instance will be set
 *
 *  @return The current error delegate or nil
 */
+ (id<RaveErrorDelegate>)errorDelegate;

/**
 *  Setter for the current error delegate, will be used automatically by various API's when set
 *
 *  @param errorDelegate Error delegate instance or nil
 */
+ (void)setErrorDelegate:(id<RaveErrorDelegate>)errorDelegate;

/**
 *  Used by Scene Pack to prevent concurrent method calls during some operations triggered by UI
 *
 *  @return The current busy delegate instance
 */
+ (id<RaveBusyDelegate>)busyDelegate;

/**
 *  Typically only used by the Scene Pack to access bundled resources used for scenes
 *
 *  @return The current resources provider
 */
+ (id<RaveResourcesProvider>)resourcesProvider;

/**
 *  Test methods
 */
/**
 *  Method to delete CAL data as well as data caches, application restart required
 */
+ (void)testDeleteAllData;

/**
 *  Method to delete CAL data, application restart suggested
 */
+ (void)testDeleteCALData;

/**
 *  Sanity check to validate asserts are off
 */
+ (void)testAsserts;
@end
