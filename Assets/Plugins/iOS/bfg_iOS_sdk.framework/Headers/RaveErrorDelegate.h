//
//  RaveErrorDelegate.h
//  RaveSocial
//
//  Created by gwilliams on 12/7/13.
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef void(^RaveConfirmationCallback)(BOOL confirmed);

@protocol RaveErrorDelegate <NSObject>
- (void)showError:(NSError *)error;
- (void)showMessages:(NSArray *)messages;
- (void)showStyledMessage:(NSString *)message withCallback:(dispatch_block_t)callback;
- (void)showConfirmationMessage:(NSString *)message withCallback:(RaveConfirmationCallback)callback;
- (void)showConfirmationMessage:(NSString *)message buttonTitles:(NSArray *)buttonTitles callback:(RaveConfirmationCallback)callback;
@end
