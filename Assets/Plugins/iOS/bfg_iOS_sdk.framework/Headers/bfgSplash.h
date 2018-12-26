///  \file bfgSplash.h
///  \brief Displays Big Fish iSplash Newsletter.
///
// \author Created by Sean Hummel on 12/2/10.
// \author Updated by Craig Thompson on 7/14/15.
// \copyright Copyright 2013 Big Fish Games. All rights reserved.

#import <bfg_iOS_sdk/bfglibPrefix.h>

///
/// \brief Notification that is posted when BFGSplash mail has completed with success/failure.
///
/// \details Notification has "object" of type NSNumber which wraps a BOOL - "NO" if mail could not
/// be sent.
/// 
#define BFGSPLASH_NOTIFICATION_COMPLETED @"BFGSPLASH_NOTIFICATION_COMPLETED"

///
/// \brief Notification that is posted when the iSplash Main Menu image has been updated.
///
/// \details The iSplash menu button should be updated with the images for normal and selected
/// states.
///
#define BFGSPLASH_NOTIFICATION_IMAGE_UPDATED @"BFGSPLASH_NOTIFICATION_IMAGE_UPDATED"

///
/// API for iSplash Newsletter functionality.
/// \details Display the iSplash Newsletter for the user to send.
///
/// If you are using these APIs, you can become an observer 
/// for the following events:
///
///  - \ref BFGSPLASH_NOTIFICATION_COMPLETED
///
@interface bfgSplash: NSObject

/// \details Displays the newsletter UI that allows the user to send the iSplash email.
///
/// \param
/// parentController Parent view controller for the iSplash mail controller view.
///
+ (void)displayNewsletter:(UIViewController *)parentController;


/// \details Sets the user setting that the iSplash Newsletter has been sent. 
/// This value is updated on the first successful connection to the Big Fish server
/// on the initial launch of the game. After that, the client can update the value locally.
///
+ (void)setNewsletterSent:(BOOL)sent;


/// \details Gets the user setting that the iSplash Newsletter has been sent.
/// This value is updated on the first successful connection to the Big Fish server
/// on the initial launch of the game. After that, the client can update the value locally.
+ (BOOL)getNewsletterSent;

/// \details Gets the localized "normal" iSplash button image.
+ (UIImage *)splashMenuImageNormal;

/// \details Gets the localized "highlighted" iSplash button image.
+ (UIImage *)splashMenuImageHighlighted;

/// \details Recommended method for creating the iSplash button to display on your Main Menu.
/// Create the iSplash button as a UIButton at the specified location.
/// The button's view is added as a subview to the parentViewController
/// associated with the bfgManager.
+ (UIButton *)createSplashButton;

@end
