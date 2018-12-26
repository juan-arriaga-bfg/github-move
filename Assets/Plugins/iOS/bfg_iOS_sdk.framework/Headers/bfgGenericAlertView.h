///
/// \file bfgGenericAlertView.h
/// \brief Interface for performing core Big Fish SDK tasks
///
/// \details A limited implementation of UIAlertView-like functionality. Uses
/// UIAlertControllers on iOS 8+, UIAlertViews on iOS < 8. Uses delegation in
/// either case.
///
/// \note Will NOT call <tt>alertViewShouldEnableFirstOtherButton:</tt> on delegate.
///
// \author Created by Benjamin Flynn on 12/10/14.
// \copyright Copyright 2015 Big Fish Games, Inc. All rights reserved.
///

#import <UIKit/UIKit.h>

__deprecated_msg("This feature will continue to function for SDK 6.9 but will be replaced in the next SDK release.")
@interface bfgGenericAlertView : NSObject

///
/// \returns In iOS 8+, a dummy alert view for use when handling the delegate calls.
/// In iOS < 8, the actual UIAlertView shown.
///
@property (nonatomic, readonly) UIAlertView             *alertView;

///
/// \returns In iOS 8+, the UIAlertController. In iOS < 8, nil.
///
@property (nonatomic, readonly) UIAlertController       *alertController;

///
/// \details A user-defined dictionary retained by the alert object.
///
@property (nonatomic, strong) NSDictionary              *userInfo;

///
/// \details Factory method for creating a bfgGenericAlertView. 
///
+ (instancetype)alertViewWithTitle:(NSString *)title
                           message:(NSString *)message
                          delegate:(id<UIAlertViewDelegate>)delegate
                 cancelButtonTitle:(NSString *)cancelButtonTitle
                 otherButtonTitles:(NSArray *)otherButtonTitles;

///
/// \details Factory method for creating a bfgGenericAlertView.
///
/// \since 5.7
///
+ (instancetype)alertViewWithTitle:(NSString *)title
                           message:(NSString *)message
                          delegate:(id<UIAlertViewDelegate>)delegate
                 cancelButtonTitle:(NSString *)cancelButtonTitle
                 otherButtonTitles:(NSArray *)otherButtonTitles
                          userInfo:(NSDictionary *)userInfo;

///
/// \details Presents the UIAlertView or UIAlertControllers
///
- (void)show;

///
/// \details Programmaticaly dismisses the alert as if a button had been 'clicked'.
///
- (void)dismissWithClickedButtonIndex:(NSInteger)buttonIndex animated:(BOOL)animated;

@end
