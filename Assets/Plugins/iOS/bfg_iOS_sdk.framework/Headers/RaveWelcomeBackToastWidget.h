//
//  RSWelcomeBackToastWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <bfg_iOS_sdk/RaveWidget.h>
#import <bfg_iOS_sdk/RaveToastWidget.h>

@interface RaveWelcomeBackToastWidget : RaveToastWidget

+ (void)showWelcome:(BOOL)isNewUser withCompletion:(dispatch_block_t)aCompletion;
- (void)presentWithMessage:(NSString*)aMessage completion:(dispatch_block_t)aCompletion;

@end
