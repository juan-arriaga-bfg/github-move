///  \file bfgPushNotificationManager.h
///  \brief bfgPushNotificationManager header file.
///
//  \author Created by Michelle McKelvey on 3/18/14.
//  \author Edited by Craig Thompson on 5/2/14.
//  \copyright Copyright (c) 2014 Big Fish Games, Inc. All rights reserved.
//


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <UserNotifications/UserNotifications.h>

//MOB-15481: Removed BFGPushProviderUpsight = 1 << 0 as part of Upsight removal
typedef NS_OPTIONS(NSUInteger, BFGPushProvider) {
    BFGPushProviderNone         = 0 << 0,
    BFGPushProviderBigFish      = 1 << 1,
    BFGPushProviderAll          = BFGPushProviderBigFish,
};


/// \brief Gives app the opportunity to handle a remote (push) notification.
///
/// \since 5.10
///
@protocol bfgPushNotificationDelegate <NSObject>

@optional

/// \brief A remote (push) notification has been received.
///
/// \note A push received when the app is in the foreground is not shown to the user. There is generally nothing
/// the app needs to do in these circumstances, but the functionality is available on an as-needed basis.
///
/// \param payload A dictionary sent with the notification
/// \param appInForeground YES if app in foreground when the push notification was received
///
/// \since 5.10
////
- (void)didReceivePushNotificationWithPayload:(NSDictionary * _Nullable)payload whileAppInForeground:(BOOL)appInForeground;

@end


/// \brief Enables Push Notifications.
///
/// \details Class to implement Push Notifications in a game. You must change your application delegate methods
///  to the bfgPushNotificationManager methods in order to implement Push Notifications.
///
@interface bfgPushNotificationManager : NSObject <UNUserNotificationCenterDelegate>

///
/// \brief Registers with all providers to receive push notifications.
///
/// \since 5.10
///
+ (void)registerForPushNotifications;

///
/// \brief Registers with all providers to receive push notifications.
///
/// \param categories (optional) If your game uses categories, which let users take custom actions when responding
///   to the notification without opening the app, you can specify them here.
///
/// \since 5.10
///
+ (void)registerForPushNotificationsWithCategories:(NSArray<UIUserNotificationCategory*> * _Nullable)categories;

/// \brief Sets the badge number for the app, visible from the iOS Springboard
///
/// \param badgeNumber The number to display in a red circle on an app. Use 0 to unset the badge.
///
+ (void)setIconBadgeNumber:(NSUInteger)badgeNumber;

/// \brief The delegate will be called when a push notification arrives.
///
/// \param delegate Your delegate implementation.
///
/// \since 5.10
///
+ (void)setPushNotificationDelegate:(id<bfgPushNotificationDelegate> _Nullable)delegate;

@end
