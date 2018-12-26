///
/// \file bfgManager.h
/// \brief Interface for performing core Big Fish SDK tasks
///
// \author Created by Sean Hummel on 10/30/10.
// \author Updated by Ben Flynn 12/18/12.
// \author Updated by Ben Flynn 7/13/15.
// \copyright Copyright 2013 Big Fish Games, Inc. All rights reserved.
///

#import <bfg_iOS_sdk/bfglibPrefix.h>

/*
 NOTIFICATIONS
 */

/// Sent after GDN play button is selected.
#define    BFGPROMODASHBOARD_NOTIFICATION_COLDSTART            @"BFGPROMODASHBOARD_NOTIFICATION_CONTINUE"

/// Sent after GDN resume button is selected.
#define BFGPROMODASHBOARD_NOTIFICATION_WARMSTART            @"BFGPROMODASHBOARD_NOTIFICATION_APPLICATION_RESUMED"

/// An internal web browser has closed.
#define BFGPROMODASHBOARD_NOTIFICATION_WEBBROWSER_CLOSED    @"BFGPROMODASHBOARD_NOTIFICATION_WEBBROWSER_CLOSED"

/*
 Settings
 */

#define BFGDASH_UI_TYPE_NONE_STRING                     @"no"
#define BFGDASH_UI_TYPE_DASHFULL_STRING                 @"fs"
#define BFGDASH_UI_TYPE_ADS_STRING                      @"ad"

/// Whether the SDK is displaying UI and what UI is displayed.
typedef enum
{
    BFGDASH_UI_TYPE_NONE = 0, /**< The SDK is not displaying any UI. */
    BFGDASH_UI_TYPE_DASHFULL = 1, /**< The full Dashboard is displayed. */
    BFGDASH_UI_TYPE_DASHWIN __attribute__((deprecated)) = BFGDASH_UI_TYPE_DASHFULL /**< No longer possible */
}
BFGDASH_UI_TYPE;

typedef NS_ENUM(NSUInteger, bfgAnchorLocation)
{
    TOP     = 0,
    CENTER  = 1,
    BOTTOM  = 2,
    LEFT    = 3,
    RIGHT   = 4
};

///
/// \brief Control indicating third party opt-in
///
/// \since 6.7
///
FOUNDATION_EXPORT NSString * _Nonnull const BFGPolicyControlThirdPartyTargetedAdvertising;
///
/// \brief Completion block for the policy screen showing
///
/// \since 6.7
///
typedef void (^bfgManagerWillShowPoliciesBlock)(void);
///
/// \brief Completion block for clearing the policy UI
///
/// \since 6.7
///
typedef void (^bfgManagerOnPoliciesCompletedBlock)(void);

@protocol bfgManagerPauseResumeDelegate;

/**
 @brief Initialize the Big Fish Game Components.

 Initialize bfgManager to enable the Big Fish Game Discovery Network (GDN).

 You should initialize the bfgManager when your application finishes launching with either your root view controller
 or your main window.  If your main window is ready inside the AppDelegate, then you will use this call:

 \code
 - (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
 {
 ...
 [bfgManager startWithLaunchOptions:launchOptions parentViewController:myRootViewController];
 ...
 }

 \endcode

 If your main window will not be ready until later, then you will do the following.  In the AppDelegate:

 \code
 - (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
 {
 ...
 [bfgManager initWithLaunchOptions:launchOptions];
 ...
 }

 \endcode

 Then, whenever your main window is ready:

 \code
 - (void)myMainWindowReady
 {
 ...
 [bfgManager startWithParentViewController:myMainViewController];
 ...
 }

 \endcode


 If you are using these APIs, you should become an observer for the following events:

 <UL>
 <LI>BFGPROMODASHBOARD_NOTIFICATION_COLDSTART - Sent after the GDN play button is selected.
 <LI>BFGPROMODASHBOARD_NOTIFICATION_WARMSTART - Sent after the GDN resume button is selected.
 </UL>

 Lastly, if you are interested in pausing and resuming your game when the SDK is doing something that will
 obscure your game window (e.g., presenting an upsell), you can become a delegate for these events by implementing
 the bfgManagerPauseResumeDelegate protocol.

 */
@interface bfgManager : NSObject


///
/// \details Number of session for the application. Incremented on each UIApplicationDidBecomeActiveNotification.
///
+ (NSInteger)sessionCount;


///
/// \details It is the initial launch or "install" of the application.
///
+ (BOOL)isInitialLaunch;


///
/// \details Application has been started from a cold-started.
///
+ (BOOL)isFirstTime;


///
/// \return
/// \retval YES if bfgManger initialized.
/// \retval NO if bfgManger did not initialize.
///
+ (BOOL)isInitialized;


///
/// \details Gets the unique integer identifying this user. Used in all reporting calls to services.
/// \return Obtains the GameUID, and defaults to UID if it does not exist.
/// If UID does not exist, it defaults to zero.
///
+ (NSNumber * _Nonnull)userID;


///
/// \details Sets gameUID, the unique integer identifying this user. Used in all reporting calls to services.
///
/// \param userID The unique integer identifying this user. If nil is passed, GameUID will be set back to the current UID value.
+ (void)setUserID:(NSNumber * _Nullable)userID;


///
/// \details This method tries to open the Big Fish Games App to allow the user to browse other Big Fish games.
/// If the app is not installed, it will prompt the user to install it. Your app will be backgrounded in order
/// to show the Big Fish Games App, if it is installed.
///
/// \updated 5.0
///
+ (void)showMoreGames;


///
/// \details Shows Help Desk Support using Zendesk SDK.
///
/// \updated 6.5
///
+ (void)showSupport;


///
/// \details Shows the Privacy page in Mobile Safari.
///
+ (void)showPrivacy;


///
/// \details Shows the Terms of Use page in Mobile Safari.
///
+ (void)showTerms;


///
/// \details Shows an in-game browser displaying startPage.
///
/// \return Web browser can be created.
///
+ (BOOL)showWebBrowser:(NSString * _Nullable)startPage;


///
/// \details Removes the in-game browser shown via showWebBrowser:
///
+ (void)removeWebBrowser;


///
///
///
+ (UIInterfaceOrientation)currentOrientation;


///
/// \return
/// \retval YES if there is a connection to the Internet.
/// \retval NO if there is not a connection to the Internet, and displays alert.
///
+ (BOOL)checkForInternetConnection;


///
/// \param displayAlert If YES, displays a UIAlert that it cannot connect to the Internet.
/// \return
/// \retval YES if can connect to the Internet.
/// \retval NO if cannot connect to the Internet.
///
+ (BOOL)checkForInternetConnectionAndAlert:(BOOL)displayAlert;


///
/// \details The view controller to use when showing the GDN,
/// for both startup and resume UI. If you change your root ViewController
/// for your application, you should update bfgManager.
///
+ (void)setParentViewController:(UIViewController * _Nonnull)parent;
+ (UIViewController * _Nonnull)getParentViewController __deprecated;
+ (UIViewController * _Nonnull)parentViewController;


///
/// \details Starts branding animation running based on contents of bfgbranding_resources.zip.
///
+ (BOOL)startBranding;


///
/// \details Stops the branding animation. The BFGBRANDING_NOTIFICATION_COMPLETED notification will be sent.
///
+ (void)stopBranding;


///
/// \details Returns enum value for the current UI displayed by the SDK.
/// \return A number between 0 and 3
/// \ref BFGDASH_UI_TYPE
///
///    \ref BFGDASH_UI_TYPE_NONE = 0
///
///    \retVal BFGDASH_UI_TYPE_DASHFULL = 1
///
///    \retVal BFGDASH_UI_TYPE_DASHWIN = 2
///
///    \retVal BFGDASH_UI_TYPE_MOREGAMES = 3

+ (BFGDASH_UI_TYPE)currentUIType __deprecated_msg("Since 6.4 -- no public use for this value");


#define BFG_LAUNCH_ISPLASH                      @"bfg_launch_isplash"
#define BFG_LAUNCH_MORE_GAMES                   @"bfg_launch_more_games"
#define BFG_LAUNCH_LOGIN                        @"bfg_launch_login"
#define BFG_LAUNCH_WEBBROWSER                   @"bfg_launch_webbrowser"

///
///
/// \details Trims URL Scheme and launches by keyword.
+(BOOL)launchSDKByURLScheme:(NSString * _Nullable)urlScheme;

///
/// \details Certain aspects of the SDK can be flagged for debugging either in
/// bfgSettings or with a bfgDebug.json file placed in the Documents folder. To
/// programmatically set flags, you can make changes directly to this dictionary.
///
/// \returns A mutable dictionary of debug settings.
///
/// \since 5.7
+ (NSMutableDictionary * _Nullable)debugDictionary;

#pragma mark - UIApplicationDelegate Wrappers

///
/// \details Call this method from your app delegate at the beginning of
/// application:didReceiveLocalNotification:
///
/// \since 5.8
+ (void)applicationDidReceiveLocalNotification;

///
/// \brief Call this method from your app delegate at the beginning of
/// application:openURL:sourceApplication:annotation
///
/// \details The app delegate may be asked to open a resource via a URL.
/// Some URLs can be handled by the Big Fish SDK. This method exists to
/// support versions of iOS pre-iOS 9.
///
/// \param url From the delegate call.
/// \param sourceApplication From the delegate call.
/// \param annotation From the delegate call.
///
/// \note A known issue is causing this method always to return YES. Always inspect the
///   URL to see if you need to handle it.
///
/// \retval YES The SDK acted upon the App Link.
/// \retval NO The SDK did not act upon the App Link.
///
/// \since 5.10
+ (BOOL)applicationOpenURL:(NSURL * _Nonnull)url sourceApplication:(NSString * _Nullable)sourceApplication annotation:(id _Nullable)annotation __deprecated_msg("Deprecated since iOS SDK 6.9");

///
/// \brief Call this method from your app delegate at the beginning of
/// application:openURL:sourceApplication:annotation
///
/// \details The app delegate may be asked to open a resource via a URL.
/// Some URLs can be handled by the Big Fish SDK. This method supports
/// versions of iOS 9+.
///
/// \param url From the delegate call.
/// \param options From the delegate call.
///
/// \note A known issue is causing this method always to return YES. Always inspect the
///   URL to see if you need to handle it.
///
/// \retval YES The SDK acted upon the App Link.
/// \retval NO The SDK did not act upon the App Link.
///
/// \since 5.10
+ (BOOL)applicationOpenURL:(NSURL * _Nonnull)url options:(NSDictionary<NSString *,id> * _Nullable)options;

/// \brief Call this method from your app delegate at the beginning of
/// application:continueUserActivity:userActivity restorationHandler
///
/// \details Universal deeplinks trigger this delegate method instead of
/// openURL.
///
/// \param userActivity From the delegate call.
/// \param restorationHandler From the delegate call.
///
/// \retval YES The SDK acted upon the App Link.
/// \retval NO The SDK did not act upon the App Link.
///
/// \note A known issue is causing this method always to return YES. Always inspect the
///   activity if you might need to handle it.
///
/// \since 5.11
+ (BOOL)applicationContinueUserActivity:(NSUserActivity * _Nonnull)userActivity restorationHandler:(void (^ _Nullable )(NSArray * _Nullable restorableObjects))restorationHandler;


#pragma mark Remote notifications

///
/// \brief Call this method from your app delegate at the beginning of
/// application:didRegisterForRemoteNotificationsWithDeviceToken:
///
/// \details This method is critical for Big Fish to set up push notifications
/// for your app.
///
/// \param deviceToken From the delegate call.
///
/// \since 5.10
+ (void)applicationDidRegisterForRemoteNotificationsWithDeviceToken:(NSData * _Nonnull)deviceToken;

/// \brief Call this method from your app delegate at the beginning of
/// application:didFailToRegisterForRemoteNotificationsWithError:
///
/// \param error From the delegate call.
///
/// \since 5.10
+ (void)applicationDidFailToRegisterForRemoteNotificationsWithError:(NSError * _Nonnull)error;

/// \brief Call this method from your app delegate at the beginning of
/// application:didReceiveRemoteNotification:
///
/// \param userInfo From the delegate call.
///
/// \since 5.10
+ (void)applicationDidReceiveRemoteNotification:(NSDictionary * _Nonnull)userInfo;

/// \brief Call this method from your app delegate at the beginning of
/// application:didRegisterUserNotificationSettings:
///
/// \param notificationSettings From the delegate call.
///
/// \since 5.10
+ (void)applicationDidRegisterUserNotificationSettings:(UIUserNotificationSettings * _Nonnull)notificationSettings;

/// \details Call this method to add a delegate to listen for pause/resume events from the SDK.
///
/// \param delegate The object that will act as a delegate for pause/resume events.  Needs to conform to the bfgManagerPauseResumeDelegate protocol.
///
/// \since 6.0
+ (void)addPauseResumeDelegate:(id<bfgManagerPauseResumeDelegate> _Nonnull)delegate;

/// \details Call this method to remove a pause/resume event delegate.
///
/// \param delegate The delegate you wish to remove.  Needs to conform to the bfgManagerPauseResumeDelegate protocol.
///
/// \since 6.0
+ (void)removePauseResumeDelegate:(id<bfgManagerPauseResumeDelegate> _Nonnull)delegate;


/// \details Test to see if game should be paused.
///
/// \retval YES The game should be paused.
/// \retval NO The game should not be paused.
///
/// \since 6.0
+ (BOOL)isPaused;

///
/// \details Call this method from your app delegate in
/// application:didFinishLaunchingWithOption: when the view controller for the SDK is available.
///
/// \param launchOptions The launch options sent to the delegate call.
/// \param parentViewController The view controller the SDK should use.
///
/// \since 6.0
+ (BOOL)startWithLaunchOptions:(NSDictionary * _Nonnull)launchOptions parentViewController:(UIViewController * _Nonnull)parentViewController;

///
/// \details Call this method from your app delegate in
/// application:didFinishLaunchingWithOption: when your SDK view controller IS NOT available.
///
/// \param launchOptions The launch options sent to the delegate call.
///
/// \since 6.0
+ (BOOL)initWithLaunchOptions:(NSDictionary * _Nonnull)launchOptions;

///
/// \details Call this method when your view controller was not ready in
/// application:didFinishLaunchingWithOption: and it has finally been initialized.
///
/// \param parentViewController The view controller the SDK should use.
///
/// \since 6.0
+ (BOOL)startWithParentViewController:(UIViewController * _Nonnull)parentViewController;

#pragma mark - bfgCCSManager
///
/// Shows a CCS button at a given frame.
///
/// \details When the app requests to show a CCS button at the point for location, the SDK verifies the remote mapping configuration to verify if the button can be shown. If yes, the button is added at the given point on the parent view.
///
/// \param frame CGRect where button will be shown.
/// \param rotationHandler A block that will be called when the bfgCCSManager detects an orientation change has occurred.
///   This block should calculate and return the CCS button's new frame (either manually calculated by your game or by calling the createCcsButtonBounds utility method).
/// \since 6.6
///
+ (void)showCCSButtonForFrame:(CGRect)frame withRotationHandler:(CGRect (^ _Nullable )(void))rotationHandler;

///
/// Shows a CCS button at a given frame for game location.
///
/// \details When the app requests to show a CCS button at the point for location, the SDK verifies the remote mapping configuration to verify if the button can be shown. If yes, the button is added at the given point on the parent view.
///
/// \param frame CGRect where button will be shown.
/// \param gameLocation location in the game. E.g., Menu scene, Rewards scene, etc.
/// \param rotationHandler A block that will be called when the bfgCCSManager detects an orientation change has occurred.
///   This block should calculate and return the CCS button's new frame (either manually calculated by your game or by calling the createCcsButtonBounds utility method).
///
/// \since 6.6
///
+ (void)showCCSButtonForFrame:(CGRect)frame gameLocation:(NSString * _Nullable)gameLocation withRotationHandler:(CGRect (^ _Nullable )(void))rotationHandler;

///
/// Shows CCS button at a given frame.
///
/// \details When app requests to show CCS button at a point for location, SDK verifies remote mapping configuration to verify if button can be shown. If yes, button is added at given point on parent view.
///
/// \param frame CGRect where button will be shown.
/// \since 6.6
///
+ (void)showCCSButtonForFrame:(CGRect)frame;

///
/// Shows CCS button at a given frame for game location.
///
/// \details When app requests to show CCS button at a point for location, SDK verifies remote mapping configuration to verify if button can be shown. If yes, button is added at given point on parent view.
///
/// \param frame CGRect where button will be shown.
/// \param gameLocation location in the game. E.g., Menu scene, Rewards scene, etc.
///
/// \since 6.6
///
+ (void)showCCSButtonForFrame:(CGRect)frame gameLocation:(NSString * _Nullable)gameLocation;

///
/// Hides CCS Button.
///
/// \details Hide the CCS button if found on current view.
/// \since 6.6
///
+ (void)hideCCSButton;

///
/// Determine if the CCS button is being displayed.
///
/// \details Determines if any CCS button is displayed currently.
/// \since 6.6
///
+ (BOOL)isShowingCCSButton;

///
/// Creates a bfgRect that describes a space that is widthPercent percent of the screen width and positioned
/// according to verticalAnchor and horizontalAnchor.
///
/// \param widthPercent     The percent of the screen width to use as the button width.
/// \param horizontalAnchor The horizontal position of the frame: left, center, or right.
/// \param verticalAnchor   The vertical position of the frame: top, center, or bottom.
///
/// \retval A CGRect to position and size the CCS button for the given parameters.
///
/// \since 6.6
///
+ (CGRect)createCcsButtonBounds:(CGFloat)widthPercent horizontalAnchor:(bfgAnchorLocation)horizontalAnchor verticalAnchor:(bfgAnchorLocation)verticalAnchor;

#pragma mark - bfgConsentManager
///
/// \details Respond to the manager showing the screen or perform actions when the UI is cleared.  Can only have one set of blocks per listener.  Care should be exercised in the completion blocks not to create strong references to the listener, as this can lead to a retain cycle where you may believe you no longer have a reference to the listener, but the completion blocks stored by the manager do.  If you need to reference listener inside a block, then use a weak reference such as "__weak __typeof(self) weakSelf = self", then use "weakSelf" inside the block.
///
/// \param listener Should always be self.
/// \param willShowPolicies The code block you want executed if the manager is going to present the UI.  Always called on the main thread.
/// \param onPoliciesCompleted The code block you want executed whenever the manager is done.  This can be called either because the user accepted all policies or because the user was not required to accept any policies.  Will be executed immediately upon adding if policies have already been cleared.  Always called on the main thread.
///
/// \since 6.7
///
+ (void)addPolicyListener:(nonnull id)listener willShowPolicies:(nullable bfgManagerWillShowPoliciesBlock)willShowPolicies onPoliciesCompleted:(nullable bfgManagerOnPoliciesCompletedBlock)onPoliciesCompleted;
///
/// \details Removes the listener.
///
/// \param listener The listener to remove.
///
/// \since 6.7
///
+ (void)removePolicyListener:(nonnull id)listener;

///
/// \details Check if the user has accepted a specific control.
///
/// \param policyControl The control you are interested in checking.
///
/// \returns YES if the user has accepted the control, NO if declined or there is no record of the user accepting it.
///
/// \since 6.7
///
+ (BOOL)didAcceptPolicyControl:(nonnull NSString *)policyControl;

@end

#pragma mark - bfgManagerPauseResumeDelegate
/// \brief Gives app the opportunity to handle pause and resume caused by the SDK.
///
/// \since 6.0
///
@protocol bfgManagerPauseResumeDelegate <NSObject>

@optional

///
/// \details Received by the delegate when the SDK is about to do something that requires a pause in the game.
///
/// \since 6.0
///
- (void)bfgManagerShouldPauseGame;

///
/// \details Received by the delegate when it is okay to resume gameplay.
///
/// \since 6.0
///
- (void)bfgManagerShouldResumeGame;

@end
