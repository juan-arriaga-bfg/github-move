//
//  BigFishScenePack.h
//
//  Copyright (c) 2013 Gorilla Graph, Inc. All rights reserved.
//

#import "AuthFlowScene.h"

/**
 *  Default BigFish merge policy, not set by default. Set an instance to RaveSocial.mergePolicy to enable authenticated merging
 */
@protocol RaveMergePolicy;
@interface BigFishMergePolicy : NSObject<RaveMergePolicy>
@end

@protocol BigFishScenePackAuthFlowDelegate <NSObject>

@required
- (void)logShownForAuthFlowScene:(AuthFlowScene *)scene;
- (void)authFlowScene:(AuthFlowScene *)scene logClickedHotspot:(AuthFlowHotSpot)hotspot;

@end

/**
 *  New setting for BigFishScene pack available:
 *
 *  BigFishSettings.General.NewsletterName is the key
 *  -  set the name to the value that should be sent to the BigFish server during user creation
 *  -  may be set programatically, in plist, or json by the calling application
 */

extern NSString * const  BigFishSettingsGeneralNewsletterName;
extern NSString * const  optInEndpoint;

/**
 *  BigFishScenePack used to prepare scenes for use
 *  -  Call one of the initalizeScenePack methods, call intead of [RaveSocial intitializeRave*] if you
 *     intend to use the scene pack
 */
@interface BigFishScenePack : NSObject
/**
 *  The assigned delegate for handling auth flow events
 *
 *  @return The auth flow delegate or nil if there is none
 */
+ (id<BigFishScenePackAuthFlowDelegate>)authFlowDelegate;

/**
 *  Sets the auth flow delegate
 */
+ (void)setAuthFlowDelegate:(id<BigFishScenePackAuthFlowDelegate>)authFlowDelegate;

/**
 *  Validates that the scene pack is ready for use
 *
 *  @return Yes or No depending on readiness
 */
+ (BOOL)isScenePackInitialized;

/**
 *  Automatically validates the scene pack and will display an error if not ready
 */
+ (void)validateScenePack;

/**
 *  **New Method**
 *
 *  When using FBSDKCoreKit for Facebook integration call from UIAppliationDelegate application:didFinishLaunchingWithOptions: where possible
 *
 *  Initialize the Scene Pack as well as chaining to [RaveSocial initializeRaveWithLaunchOptions:withReadyCallback:]
 *
 *  @param launchOptions  Launch options to forward to RaveSocial initialization
 *  @param readyCallback Ready callback to forward to RaveSocial initialization
 */
+ (void)initializeScenePackWithLaunchOptions:(NSDictionary *)launchOptions withReadyCallback:(RaveCompletionCallback)readyCallback;

/**
 *  **New Method**
 *
 *  When using FBSDKCoreKit for Facebook integration call from UIAppliationDelegate application:didFinishLaunchingWithOptions: where possible
 *
 *  Initialize Scene Pack as well as chaining to [RaveSocial initializeRaveWithConfig:withLaunchOptions:withReadyCallback]
 *
 *  @param configURL Config JSON URL to forward to RaveSocial initialization
 *  @param launchOptions  Launch options to forward to RaveSocial initialization
 *  @param readyCallback Ready callback to forward to RaveSocial initialization
 */
+ (void)initializeScenePackWithConfig:(NSURL *)configURL withLaunchOptions:(NSDictionary *)launchOptions withReadyCallback:(RaveCompletionCallback)readyCallback;

/**
 *  Initializes the scene pack and RaveSocial
 */
+ (void)initializeScenePack;

/**
 *  Initalizes the scene pack and RaveSocial
 *
 *  @param configURL Custom URL to json configuration file
 */
+ (void)initializeScenePackWithConfig:(NSURL *)configURL;

/**
 *  Intializes scene pack and RaveSocial
 *
 *  @param readyCallback Custom callback for RaveSocial readiness notification
 */
+ (void)initializeScenePackWithReadyCallback:(RaveCompletionCallback)readyCallback;

/**
 *  Initializes scene pack and RaveSocial
 *
 *  @param configURL     Custom URL to json configuration file
 *  @param readyCallback Custom callback for RaveSocial readiness notification
 */
+ (void)initializeScenePackWithConfig:(NSURL *)configURL withReadyCallback:(RaveCompletionCallback)readyCallback;

/**
 *  Sanity check to ensure asserts are disabled
 */
+ (void)testAsserts;

/**
 *  Send Update user's newsletter opt-in status
 **/
+ (void)updateNewsletterOptInStatusTo:(BOOL)optIn;
@end





/**
 *  Legacy constants
 */
extern NSString * const RaveConnectPluginBigFish;

typedef void (^RaveBigFishAuthorizationCallback)(BOOL createdNewAccount, NSString* anEmail, NSString* anUserToken, NSError * error);

