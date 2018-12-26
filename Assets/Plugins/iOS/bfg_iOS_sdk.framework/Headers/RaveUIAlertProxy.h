//
//  RaveUIAlertProxy.h
//  RaveUI
//
//  Copyright (c) 2013 Gorilla Graph, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

typedef void (^UIAlertProxyConfirmationCallback)(NSInteger buttonIndex);

/**
 *  Proxy interface as a stand-in for UIAlertView (deprecated in iOS8) and UIAlertController (new in iOS9)
 */

typedef void (^RaveUIAlertProxyEditTextCallback)(NSString * text);

@interface RaveUIAlertProxy : NSObject

/**
 Show a message in system dialog

 @param message String message
 */
+ (void)showMessage:(NSString*)message;

/**
 Show an error in a system dialog

 @param error Error to display
 */
+ (void)showError:(NSError*)error;

/**
 Show message with title in system dialog

 @param title Title string
 @param message Message string
 */
+ (void)showTitle:(NSString *)title message:(NSString *)message;

/**
 Show a message with title, user defined buttons, and button handler

 @param title Title string
 @param message Message string
 @param buttons Array of button titles for possible actions
 @param callback Handler for button chosen by user
 */
+ (void)showTitle:(NSString*)title message:(NSString*)message
    buttonsTitles:(NSArray*)buttons
         callback:(UIAlertProxyConfirmationCallback)callback;

/**
 show a message with title for edit box with placeholder text and handler for when editing is finished (tapping OK or Cancel)

 @param title Title string
 @param message Message string
 @param placeholder Placeholder string
 @param callback Callback for responding to editing being finished by pressing button
 */
+ (void)showEditTextTitle:(NSString *)title message:(NSString *)message placeholder:(NSString *)placeholder callback:(RaveUIAlertProxyEditTextCallback)callback;
@end
