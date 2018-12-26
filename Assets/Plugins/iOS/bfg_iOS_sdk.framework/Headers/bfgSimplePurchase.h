///
///  \file bfgSimplePurchase.h
///  \brief In-App Purchase handling for Big Fish Premium titles.
///
//  Created by Benjamin Flynn on 12/16/15.
//  Copyright © 2015 Big Fish Games, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

///
/// \brief Whether the paywall is being triggered from the game's main .enu or at the end of the game's gameplay
///
/// \since 6.4
///
typedef NS_ENUM(NSUInteger, BFGPaywallOrigin)
{
    BFGPaywallOriginMainMenu,       ///< Arriving at paywall from main menu
    BFGPaywallOriginEndOfGameplay   ///< Arriving at paywall from end of free content
};

/// \brief Size, in points, for rendering the subscription profile button.
///
/// \details Values hardcoded to { .width = 260.f, .height =  102.f }
///
/// \since 6.4
///
FOUNDATION_EXTERN CGSize const kBFGSubscriptionProfileButtonSize;

/// \brief Notification sent when bfgSimplePurchase and all dependents are ready to be used.
///
/// \details bfgSimplePurchase relies on two seperate internal services, productInfo, and a valid receipt.
//  This notificatin is sent once all of bfgSimplePurchase's dependent services have been started, product
/// info has been retreived from Apple, and a valid receipt has been detected.  It is recommended for your
/// game to listen for this notificaion and only use bfgSimplePurchase once it has been received.
///
/// \since 6.6
///
FOUNDATION_EXPORT NSString * const kNotificationSimplePurchaseStartupComplete;

///
/// \details Sets one of your classes as a delegate of the bfgSimplePurchase class. You
/// must set this delegate and implement the required dismissPaywallAndUnlock: method in order
/// to be notified when the Big Fish Premium Paywall is dismissed.
///
/// \since 5.10
/// \updated 6.4
///
@protocol bfgSimplePurchaseDelegate <NSObject>

@required

///
/// \details If you receive this call, the user is done with the paywall and you should dismiss it.
/// The 'unlock' parameter is passed as a convenience to determine whether the user purchased your
/// game or not.
///
/// \param unlock If true, the game has been purchased. If false, the game has not been purchased.
///
/// \since 6.4
///
- (void)dismissPaywallAndUnlock:(BOOL)unlock;

@end

///
/// \brief Provides the interface for showing the Big Fish Premium paywall, subscription profile, and customizing
/// the profile button's appearance.
///
/// \details Premium games will need to use bfgSimplePurchase to facilitate purchasing the Big Fish Premium Subscription
/// and full unlock for Premium games.  Be aware that you must declare the base name of the In-App Purchase in your
/// bfg_first_launch_settings.json -- use the key "iap_default_product_id". For more information on the settings json files,
/// see the Big Fish documentation.
///
/// This class also offers interface methods for showing a users subscription profile as well as customizing the appearance
/// of the profile/account button that will appear in each game's Main Menu.
///
/// \since 5.10
/// \updated 6.4
///
@interface bfgSimplePurchase : NSObject

///
/// \details Call this as early as possible, generally right after you initialize bfgManager. Must be called for
///  purchase to work.
/// \param delegate A class that implements bfgSimplePurchaseDelegate and will receive purchase events.
///
/// \since 5.10
///
+ (void)startServiceWithDelegate:(id<bfgSimplePurchaseDelegate>)delegate;


///
/// \details Convenience method to determine purchased state.
/// \return YES if the In-App Purchase has been purchased, otherwise NO.
///
/// \since 5.10
///
+ (BOOL)isPurchased;

///
/// \details Shows the paywall UI on top of the game screen. It exposes UI to unlock the game
///  and to subscribe to Big Fish Game club.
/// \param origin The originator of the paywall, i.e., from where is the paywall being invoked, it can either be the
///   Main Menu or the end of gameplay trigger.
///
/// \see BFGPaywallOrigin
///
/// \since 6.4
///
+ (void)showPaywallUIFrom:(BFGPaywallOrigin)origin;

///
/// \brief Open the user's subscription details screen
///
/// \details If you render the subscription profile button yourself, when the user taps on the button you should
///   call this method to show information about the user's subscription. Provides the opportunity to log in if
///   the user is not currently logged in.
///
/// \since 6.4
///
+ (void)showSubscriptionProfile;


/// \brief Localized native subscription profile button
///
/// \details The UIButton is delivered at a fixed height and width.  Add this button as a subview of the desired view,
///   then set its frame to place it in its desired location.
///
/// \see kBFGSubscriptionProfileButtonSize
///
/// \return A localized iOS native button wired to show the user's subscription profile.
///
/// \since 6.4
///
+ (UIButton *)subscriptionProfileButton;


/// \details Used to custom render the subscription profile button. For a common conversion see
///   UIImagePNGRepresentation. The image contains an alpha channel.
///
/// \return UIImage of the subscription profile button in its non-highlighted state.
///
/// \since 6.4
///
+ (UIImage *)subscriptionProfileButtonImageNormal;


/// \details Used to custom render the subscription profile button. For a common conversion see
///   UIImagePNGRepresentation. The image contains an alpha channel.
///
/// \return UIImage of the subscription profile button in its highlighted state.
///
/// \since 6.4
///
+ (UIImage *)subscriptionProfileButtonImageHighlighted;

@end
