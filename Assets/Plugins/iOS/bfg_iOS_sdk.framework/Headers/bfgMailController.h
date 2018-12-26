///
/// \file bfgMailController.h
/// \brief Helper class for presenting a mail composer
///
// \author Created by John Starin on 6/4/13.
// \author Updated by Ben Flynn 12/18/12.
// \author Updated by Craig Thompson on 10/1/13.
// \author Updated by Craig Thompson on 7/14/15.
// \copyright Copyright 2013 Big Fish Games, Inc. All rights reserved.
///

#import <Foundation/Foundation.h>
#import <MessageUI/MessageUI.h>

/// Helper class for presenting a mail composer.
@interface bfgMailController : NSObject

@property (nonatomic, readonly, weak) UIViewController *parentViewController;
@property (nonatomic, readonly, strong) NSString *completionNotificationName;

/// \brief Initialization method for mail controller.
/// \param viewController Parent view controller that should display email composer.
/// \param notification Name of notification that will be sent when the bfgMailController is finished.
///
- (id)initWithViewController:(UIViewController *)viewController completionNotification:(NSString *)notification;

///
/// \details Presents the user with an email composer filled out and ready to be sent.
/// \return YES if email was presented to the user; NO if the device is not configured to send emails.
/// \param image An optional image that is inserted into the top of the email body.
/// \param emailAddress Recipient email address.
/// \param subject Subject line of the email.
/// \param body Body of the email.
/// \param reporting Block to handle reporting or logging based on the email composer result.
///
- (BOOL)presentEmailWithImage:(UIImage *)image emailAddress:(NSString *)emailAddress subject:(NSString *)subject body:(NSString *)body reporting:(void(^)(MFMailComposeResult result))reporting;

@end
