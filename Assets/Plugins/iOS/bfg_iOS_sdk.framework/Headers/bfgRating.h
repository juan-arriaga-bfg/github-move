///
/// \file bfgRating.h
/// \brief Manages prompting for game rating
///
//  bfg_iOS_sdk
//
// \author Created by Arash Payan on 9/5/09.
// \author Updated by Sean Hummel.
// \author Updated by Craig Thompson on 10/1/13.
// \author Updated by Benjamin Flynn on 2/28/15.
// \author Updated by Craig Thompson on 6/29/15.
/// \copyright Copyright (c) 2013 Big Fish Games. All rights reserved.
//

#import <Foundation/Foundation.h>


/// Notification that rating alert is being shown. Game should pause.
#define BFGRATING_NOTIFICATION_RATING_ALERT_OPENED        @"BFGRATING_NOTIFICATION_RATING_ALERT_OPENED"

/// Notification that rating alert has been dismissed.
#define BFGRATING_NOTIFICATION_RATING_ALERT_CLOSED        @"BFGRATING_NOTIFICATION_RATING_ALERT_CLOSED"

///
/// \brief Enables users to rate games.
/// \details If you are using these APIs, you can become an observer 
/// for the following events:
///
/// - BFGRATING_NOTIFICATION_RATING_ALERT_OPENED - The "Rate this Game" dialog has been shown.
/// - BFGRATING_NOTIFICATION_RATING_ALERT_CLOSED - The "Rate this Game" dialog has been closed.
///
/// User will be prompted to rate at each significant event, presuming they have an Internet connection.
///
@interface bfgRating : NSObject

///
/// \details Call this at each point in the game that would be appropriate to prompt the
/// user to rate the game.
///
/// \note Will post BFGRATING_NOTIFICATION_RATING_ALERT_OPENED if we are able to show UI and
/// later BFGRATING_NOTIFICATION_RATING_ALERT_CLOSED when that UI is dismissed.
///
/// \returns NO if UI will not be shown. YES if UI will be shown.
///
+ (BOOL)userDidSignificantEvent;

///
/// \details Used to navigate directly to the AppStore's rating page.
/// Should be called when users clicks on "Rate App" from the Main Menu.
/// Rating will take the user out of the app and into the AppStore.
///
/// \note Check that [bfgRating canShowMainMenuRateButton] returns YES before
/// displaying the button connected to this method.
///
+ (void)mainMenuRateApp;

///
/// \details Used to give the opportunity for the user to provide us negative
/// feedback about our game, rather than having them rate us in the AppStore.
/// Should be called when users taps "No, here's my feedback."
///
/// \note Check that [bfgRating canShowMainMenuRateButton] returns YES before
/// displaying the button connected to this method.
///
/// \note Will post BFGRATING_NOTIFICATION_RATING_ALERT_OPENED and
/// BFGRATING_NOTIFICATION_RATING_ALERT_CLOSED
///
/// \since 5.6
///
+ (void)mainMenuGiveFeedback;

///
/// \details Should your game display the "Rate Me!" button on the Main Menu?
///
/// \return
/// \retval YES if the user has not already rated the app, and if the URL
/// to rate the app has been downloaded from the servers and the user is
/// Internet connected.
/// \retval NO if the user has rated the app.
///
+ (BOOL)canShowMainMenuRateButton;

@end