//
/// \file bfgGameReporting.h
/// \brief Used by game to report significant events for analytics and placements.
///
/// \details Logging a significant event in your game helps us understand how well the players
///   are progressing. We may also provide advertising content, called "placements", after
///   these events occur.
///
/// \note This class contains keys for debugging placements in your app. Refer to the SDK
///   documentation for more information.
///
//  bfg_iOS_sdk
//
// \author Created by Michelle McKelvey on 3/5/12.
// \author Updated by Craig Thompson on 4/11/14.
// \author Updated by Craig Thompson on 7/14/15.
// \author Updated by Benjamin Flynn on 11/25/15
// \copyright Copyright (c) 2013 Big Fish Games, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <bfg_iOS_sdk/bfgReporting.h>

@protocol bfgDeferredDeepLinkDelegate;
@protocol bfgPlacementDelegate;

typedef NS_OPTIONS(NSUInteger, bfgUATrackerId) {
    bfgUATrackerIdTune = 1 << 0,
    bfgUATrackerIdBigFish = 1 << 1,
    bfgUATrackerIdAppsFlyer = 1 << 2,
    bfgUATrackerIdAll = bfgUATrackerIdTune | bfgUATrackerIdBigFish | bfgUATrackerIdAppsFlyer,
};


#pragma mark - bfgGameReporting
// ***********************************************************************************************
// * GAME REPORTING

///
///
/// Required game logging APIs.
///
///

/// \details This class is used for reporting all game events.
///
@interface bfgGameReporting : NSObject


///
/// \details Called each time the Main Menu is shown.
///
+ (void)logMainMenuShown;


///
/// \details Called each time Rate from the Main Menu is canceled.
///
+ (void)logRateMainMenuCanceled;


///
/// \details Called each time the Options Menu is shown.
///
+ (void)logOptionsShown;


///
/// \details Called each time purchase from the Main Menu is shown.
///
+ (void)logPurchaseMainMenuShown __deprecated_msg("Unused since 6.4 [Big Fish Premium]");

///
/// \details Called each time purchase from the Main Menu is closed.
///
+ (void)logPurchaseMainMenuClosed __deprecated_msg("Unused since 6.4 [Big Fish Premium]");

///
/// \details Called each time a non-mainmenu purchase paywall is shown with an identifier for the paywall.
///
+ (void)logPurchasePayWallShown:(NSString * _Nullable)paywallID __deprecated_msg("Unused since 6.4 [Big Fish Premium]");


///
/// \details Called each time a non-mainmenu purchase paywall is closed with an identifier for the paywall.
///
+ (void)logPurchasePayWallClosed:(NSString * _Nullable)paywallID __deprecated_msg("Unused since 6.4 [Big Fish Premium]");

///
/// \details Called each time a level has started.
///
+ (void)logLevelStart:(NSString * _Nonnull)levelID;


///
/// \details Called each time a level is finished.
///
+ (void)logLevelFinished:(NSString * _Nonnull)levelID;


///
/// \details Called each time a mini game is started.
///
+ (void)logMiniGameStart:(NSString * _Nonnull)miniGameID;


///
/// \details Called each time a mini game is skipped.
///
+ (void)logMiniGameSkipped:(NSString * _Nonnull)miniGameID;


///
/// \details Called each time a mini game is finished.
///
+ (void)logMiniGameFinished:(NSString * _Nonnull)miniGameID;


///
/// \details Called each time an achievement is earned.
///
+ (void)logAchievementEarned:(NSString * _Nonnull)achievementID;

///
/// \details Called each time a hint is requested.
///
+ (void)logGameHintRequested;

//
// \details  Calls the In-App Purchase screen when the purchase button is selected.
//
// purchaseButton can be one of the following values:
// BFG_PURCHASE_BUTTON_BUY,
// BFG_PURCHASE_BUTTON_RESTORE,
// BFG_PURCHASE_BUTTON_LATER,
// BFG_PURCHASE_BUTTON_CLOSE
//
//  Removed Flurry Events
// + (void)logIAPButtonTapped:(BFG_PURCHASE_BUTTON)purchaseButton;


///
/// \details Called when the game is completed.
///
+ (void)logGameCompleted;

///
/// \details logCustomEvent logs a custom event.
///
/// @param
/// name The name of the event at its lowest level of detail.
/// @param
/// value The value of this parameter is dependent on eventDetails1.
/// @param
/// level This parameter can be used to track game levels where custom events occur.
/// If it is not applicable to your event, you should use 1.
/// @param
/// details1 This field should be outlined in a document provided by your producer.
/// @param
/// details2 Typically reflects the "type" of an event occurring within details1.
/// @param
/// details3 Typically describes the method or location of an event.
/// @param
/// additionalDetails For reporting purposes, any additional data pertaining to the event
/// can be added as a dictionary. This *must* be
/// a flat dictionary containing only NSString * as keys and values.
/// \return
/// \retval YES if the custom event was added to the event queue.
/// \retval NO if otherwise.
+ (BOOL)logCustomEvent:(NSString * _Nonnull)name
                 value:(NSInteger)value
                 level:(NSInteger)level
              details1:(NSString * _Nullable)details1
              details2:(NSString * _Nullable)details2
              details3:(NSString * _Nullable)details3
     additionalDetails:(NSDictionary * _Nullable)additionalDetails;

///
/// \details logCustomEvent logs a custom event.
///
/// \param
/// name The name of the event at its lowest level of detail.
/// \param
/// value The value of this parameter is dependent on eventDetails1.
/// \param
/// level This parameter can be used to track game levels where custom events occur.
/// If it is not applicable to your event, you should use 1.
/// \param
/// details1 This field should be outlined in a document provided by your producer.
/// \param
/// details2 Typically reflects the "type" of an event occurring within details1.
/// \param
/// details3 Typically describes the method or location of an event.
/// \param
/// additionalDetails For reporting purposes, any additional data pertaining to the event
/// can be added as a dictionary. This *must* be
/// a flat dictionary containing only NSString * as keys and values.
/// \param
/// error Used for indicating KT is disabled or that the payload is invalid for any of these three reasons:
///       1) Payload is too big, over 2000 characters.
///       2) Use of invalid characters (like spaces) in name, details1, details2 or details3.
///       3) details2 is set but there is no details1 or details3 is set but there is no details2.
///
/// \since 5.11
/// \return
/// \retval YES if the custom event was added to reporting queues as appropriate (Kontagent / Big Fish).
/// \retval NO if otherwise.
///
+ (BOOL)logCustomEvent:(NSString * _Nonnull)name
                 value:(NSInteger)value
                 level:(NSInteger)level
              details1:(NSString * _Nullable)details1
              details2:(NSString * _Nullable)details2
              details3:(NSString * _Nullable)details3
     additionalDetails:(NSDictionary * _Nullable)additionalDetails
                 error:(NSError * _Nullable __autoreleasing * _Nullable)error;

///
/// \details Logs a custom event and immediately delivers it.
///
/// \note Do not use this method unless you absolutely have to as it undermines the queuing of
/// events.
///
/// @param
/// name The name of the event at its lowest level of detail.
/// @param
/// value The value of this parameter is dependent on eventDetails1.
/// @param
/// level This parameter can be used to track game levels where custom events occur.
/// If it is not applicable to your event, you should use 1.
/// @param
/// details1 This field should be outlined in a document provided by your producer.
/// @param
/// details2 Typically reflects the "type" of an event occurring within details1.
/// @param
/// details3 Typically describes the method or location of an event.
/// @param
/// additionalDetails For reporting purposes, any additional data pertaining to the event
/// can be added as a dictionary. This *must* be
/// a flat dictionary containing only NSString * as keys and values.
///
/// \since 5.7
/// \return
/// \retval YES if the custom event was logged.
/// \retval NO if otherwise.
///
+ (BOOL)logCustomEventImmediately:(NSString * _Nonnull)name
                            value:(NSInteger)value
                            level:(NSInteger)level
                         details1:(NSString * _Nullable)details1
                         details2:(NSString * _Nullable)details2
                         details3:(NSString * _Nullable)details3
                additionalDetails:(NSDictionary * _Nullable)additionalDetails;

///
/// \details Logs a custom event and immediately delivers it.
///
/// \param
/// name The name of the event at its lowest level of detail.
/// \param
/// value The value of this parameter is dependent on eventDetails1.
/// \param
/// level This parameter can be used to track game levels where custom events occur.
/// If it is not applicable to your event, you should use 1.
/// \param
/// details1 This field should be outlined in a document provided by your producer.
/// \param
/// details2 Typically reflects the "type" of an event occurring within details1.
/// \param
/// details3 Typically describes the method or location of an event.
/// \param
/// additionalDetails For reporting purposes, any additional data pertaining to the event
/// can be added as a dictionary. This *must* be
/// a flat dictionary containing only NSString * as keys and values.
/// \param
/// error Used for indicating KT is disabled or that the payload is invalid for any of these 3 reasons:
///       1) Payload is too big, over 2000 characters.
///       2) Use of invalid characters (like spaces) in name, details1, details2 or details3.
///       3) details2 is set but there is no details1 or details3 is set but there is no details2.
///
/// \since 5.11
/// \return
/// \retval YES if the custom event was added to immediate reporting queues as appropriate (Kontagent / Big Fish).
/// \retval NO if otherwise.
///
+ (BOOL)logCustomEventImmediately:(NSString * _Nonnull)name
                            value:(NSInteger)value
                            level:(NSInteger)level
                         details1:(NSString * _Nullable)details1
                         details2:(NSString * _Nullable)details2
                         details3:(NSString * _Nullable)details3
                additionalDetails:(NSDictionary * _Nullable)additionalDetails
                            error:(NSError * _Nullable __autoreleasing * _Nullable)error;

///
/// \details logCustomPlacement logs a custom placement.
///
/// @param
/// placementName The name of the custom placement that has occurred.
///
+ (void)logCustomPlacement:(NSString * _Nonnull)placementName;

///
/// \details preloadCustomPlacement Pre-load the content of a custom event.
///
/// @param
/// placementName The name of the placement to be custom loaded.
///
/// Note: Frequency capping will not be respected for placements that are custom loaded.
///
+ (void)preloadCustomPlacement:(NSString * _Nullable)placementName __deprecated_msg("Deprecated since iOS SDK 6.8");

///
/// \details User is presented with a content gate.
/// \note This is not for prompting the user to install Big Fish Games App for strategy guides --
/// use logBigFishGamesAppOverlayShown for that.
///
/// \since 5.0
///
+ (void)logContentGateShown;

///
/// \details If you have a specific need to send custom events to MobileAppTracking (HasOffers),
/// you can use this method.
///
/// \param name An event identifier, such as "FinishedTutorial"
///
/// \since 5.7
///
+ (void)logMobileAppTrackingCustomEvent:(NSString * _Nonnull)name;

///
/// \details Placements will make calls to their delegate in addition to posting notifications. If you prefer
/// using delegation to NSNotifications, implement the delegate calls and set your delegate here.
///
/// \param delegate An implementer of bfgPlacementDelegate
///
/// \since 5.7
///
+ (void)setPlacementDelegate:(id<bfgPlacementDelegate> _Nonnull)delegate;

///
/// \details Calling this method will trigger a call to for a placement to present a user with a survey.
///
/// \note The placement associated with this event is called 'survey_time'.
///
/// \since 5.7
///
+ (void)logSurveyEvent;

/// \details If it would be inappropriate for a placement ad to appear over the game in the moment,
///   set suppressPlacements to YES. As soon as it is ok to show placements again, be sure to set
///   it back to no.
///
/// \since 5.10
///
+ (void)setSuppressPlacement:(BOOL)suppressPlacements;

/// \details If a placement is showing, it is automatically and instantly dismissed. If no placement
///  is showing, does nothing.
///
/// \since 5.10
///
+ (void)dismissVisiblePlacement;

/// \details Set this before initializing bfgManager to receive notifications if you received a deferred
/// deep link.
///
/// \param delegate The delegate is informed when deferred deep links succeed or fail.
///
/// \since 5.10
/// \deprecated Deprecated since iOS SDK 6.9
/// \note This method has been deprecated but the replacement method will not be implemented until SDK 6.9.1.  Please continue using this method until SDK 6.9.1 is released.
+ (void)setDeferredDeepLinkDelegate:(id<bfgDeferredDeepLinkDelegate> _Nonnull)delegate __deprecated_msg("Deprecated since iOS SDK 6.9.");

/// \details Calling this method will report that a rewarded video has been fully watched.
///
/// \param provider The name of the ad provider, example "vungle".
/// \param videoLocation Location in the game where the user tapped to view the video, example "in-game-store".
///
/// \since 6.1
///
+ (void)logRewardedVideoSeenWithProvider:(NSString * _Nullable)provider videoLocation:(NSString * _Nullable)videoLocation;

/// \details Calling this method will report that a rewarded video has been fully watched.
///
/// \param provider The name of the ad provider, example "vungle".
///
/// \since 6.1
///
+ (void)logRewardedVideoSeenWithProvider:(NSString * _Nullable)provider;

/// \brief Optional game specific data (LTV) that can be added to Zendesk tickets.  This value must be a float.
///
/// \param playerSpend The value of the user's current spend to be reported to Zendesk.
///
/// \since 6.5
///
+ (void)setPlayerSpend:(float)playerSpend;

/// \brief Getter for the playerSpend value.
///
/// \return The value currently stored in playerSpend.  This will either be the last value the game set, using setPlayerSpend, or nil.
///
/// \since 6.5
///
+ (float)playerSpend;

/// \brief Optional level name to be added to Zendesk tickets.
///
/// \param lastLevelPlayed The last level played by the user to be reported to Zendesk.
///
/// \since 6.5
///
+ (void)setLastLevelPlayed:(NSString * _Nullable)lastLevelPlayed;

/// \brief Getter for the lastLevelPlayed value.
///
/// \return The values currently stored in lastLevelPlayed.  This will either be the last value the game set, using setLastLevelPlayed, or nil.
///
/// \since 6.5
///
+ (NSString * _Nullable)lastLevelPlayed;

@end


#pragma mark - bfgDeferredDeepLinkDelegate
// ***********************************************************************************************
// * DEFERRED DEEPLINK DELEGATION

/// \since 5.10
/// \deprecated Deprecated since iOS SDK 6.9
/// \note This protocol has been deprecated but the replacement protocol will not be introduced until SDK 6.9.1.  Please continue using this protocol until SDK 6.9.1 is released.
__deprecated_msg("Deprecated since iOS SDK 6.9.")
@protocol bfgDeferredDeepLinkDelegate <NSObject>

@optional

/// \details Called when an attempt to receive a deferred deep link completes.
///
/// \param deepLinkString Stringified deep link URL. nil on error or if none present.
/// \param error Error retrieving deep link. May be nil if there simply is no deep link.
/// \param provider The service that is reporting the deep link.
/// \param timeSinceLaunch Elapsed seconds between the app launching and deferred deep
/// link arriving.
///
/// \since 5.10
/// \deprecated Deprecated since iOS SDK 6.9
/// \note This method has been deprecated but the replacement method will not be introduced until SDK 6.9.1.  Please continue to use this method until SDK 6.9.1 is released.
- (void)didReceiveDeferredDeepLink:(NSString * _Nullable)deepLinkString error:(NSError * _Nullable)error provider:(bfgUATrackerId)provider timeSinceLaunch:(NSTimeInterval)timeSinceLaunch __deprecated_msg("Deprecated since iOS SDK 6.9.");

@end


#pragma mark - bfgPlacementDelegate
// ***********************************************************************************************
// * PLACEMENT DELEGATION

/// \brief Notifies when placement activities occur.
///
/// \see bfgManagerPauseResumeDelegate
///
/// \since 5.10
///
@protocol bfgPlacementDelegate <NSObject>

@optional

/// \brief A placement failed to load and will not display.
///
/// \since 5.7
///
- (void)bfgPlacementContentError:(NSString * _Nullable)contentName error:(NSError * _Nullable)error;

/// \brief The placement would like to reward the player
///
/// \details To function safely, the actual rewarding logic should occur on the
///   game's server, and the placement would simply notify the user that the
///   reward was available. A client should never reward purely on the basis of
///   receiving this call, as there are not safeguards in place to prevent fraud.
///
/// \note This is only triggered if the placementDebugMode is enabled in first_launch_settings.json
///     and this is a debug build. This won't be triggered on a production build regardless of the
///     setting being enabled on first_launch_settings.json.
///
/// \param placementKey Name of the placement.
/// \param rewardName An identifier for the reward
/// \param rewardQuantity A quantity such as @(100) associated with the reward
///
/// \since 5.10
///
- (void)bfgPlacementRewardReceived:(NSString * _Nullable)placementKey
                        rewardName:(NSString * _Nonnull)rewardName
                    rewardQuantity:(NSNumber * _Nullable)rewardQuantity;

/// \brief A purchase has been requested. Your game should start the purchase.
///
/// \param placementKey Name of the placement.
/// \param productId The Apple product identifier.
/// \param productQuantity The number of products being requested for purchase (should always be 1).
///
/// \note This is only triggered if the placementDebugMode is enabled in first_launch_settings.json
///     and this is a debug build. This won't be triggered on a production build regardless of the
///     setting being enabled on first_launch_settings.json.
///
/// \return YES if your delegate will start the purchase flow. If NO, the SDK will start the purchase.
///
/// \since 5.10
///
- (BOOL)bfgPlacementStartPurchase:(NSString * _Nullable)placementKey
                        productId:(NSString * _Nonnull)productId
                  productQuantity:(NSNumber * _Nullable)productQuantity;

/// \brief An interstitial has delivered a data payload to your game.
///
/// \param dataString the data content in string format
/// \param scenarioId the interstitial id that delivered the data
///
/// \return YES if your delegate handled the data payload, NO otherwise
///
/// \since 6.1
///
- (BOOL)bfgPlacementDidReceiveDataString:(NSString * _Nullable)dataString
                      scenarioId:(NSString * _Nullable)scenarioId;

/// \brief A placement will be opening in front of the game.
///
/// \since 5.10
///
- (void)bfgPlacementContentWillOpen:(NSString * _Nullable)placementKey __deprecated_msg("Use bfgManagerPauseResumeDelegate");

/// \brief A placement has closed and is no longer displaying.
///
/// \since 5.10
///
- (void)bfgPlacementContentDidDismiss:(NSString * _Nullable)placementKey __deprecated_msg("Use bfgManagerPauseResumeDelegate");

/// \brief A placement has been opened and is displaying in front of the game.
///
/// \since 5.10
///
- (void)bfgPlacementContentDidOpen:(NSString * _Nullable)placementKey __deprecated_msg("Use bfgManagerPauseResumeDelegate");

/// \brief A placement will close.
///
/// \since 5.10
///
- (void)bfgPlacementContentWillDismiss:(NSString * _Nullable)placementKey __deprecated_msg("Use bfgManagerPauseResumeDelegate");

@end

#pragma mark - Placement Debug

// ***********************************************************************************************
// * PLACEMENT DEBUG

/// \brief Set to true to enable placement debugging. Value: @"placementDebugMode"
/// \since 5.10
///
FOUNDATION_EXPORT NSString * const _Nonnull kBFGPlacementDebug_DebugMode;

/// \brief Set to an in-app purchase product ID to test a placement sellling that product.
///   Value: @"placementDebugProductId"
/// \since 5.10
///
FOUNDATION_EXPORT NSString * const _Nonnull kBFGPlacementDebug_ProductId;

/// \brief Set to the ID of an item you could reward in the game. Value: @"placementDebugRewardId"
/// \since 5.10
///
FOUNDATION_EXPORT NSString * const _Nonnull kBFGPlacementDebug_RewardId;


// Just to be more explicit
typedef NSString BFGPlacementDebugPlacementKey;

/// \brief Setting this key to one of the below values will cause every placement triggered to
///   be of the specified type. See the BFGPlacementDebugPlacementKeys. Value: @"placementDebugPlacementKey"
/// \since 5.10
///
FOUNDATION_EXPORT NSString * const _Nonnull kBFGPlacementDebug_PlacementKey;

/// \brief A placement that opens the external AppStore. Value: @"placementDebugExternalStorePlacement"
/// \since 5.10
///
FOUNDATION_EXPORT BFGPlacementDebugPlacementKey * const _Nonnull kBFGPlacementDebug_ExternalStore_PlacementKey;

/// \brief A placement that opens the in-game AppStore. Value: @"placementDebugInternalStorePlacement"
/// \since 5.10
///
FOUNDATION_EXPORT BFGPlacementDebugPlacementKey * const _Nonnull kBFGPlacementDebug_InternalStore_PlacementKey;

/// \brief A placement that triggers and in-app purchase. Uses the placement debug product ID, or
///   otherwise the debug. Value: @"placementDebugInAppPurchasePlacement"
/// \since 5.10
///
FOUNDATION_EXPORT BFGPlacementDebugPlacementKey * const _Nonnull kBFGPlacementDebug_InAppPurchase_PlacementKey;

/// \brief A placement that opens a page in Safari. Value: @"placementDebugSafariPlacement"
/// \since 5.10
///
FOUNDATION_EXPORT BFGPlacementDebugPlacementKey * const _Nonnull kBFGPlacementDebug_Safari_PlacementKey;

/// \brief A placement that triggers a reward. Value: @"placementDebugInAppRewardPlacement"
/// \since 5.10
///
FOUNDATION_EXPORT BFGPlacementDebugPlacementKey * const _Nonnull kBFGPlacementDebug_InAppReward_PlacementKey;
