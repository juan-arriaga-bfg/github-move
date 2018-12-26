//
//  RaveProgressDelegate.h
//  RaveUI
//
//  Created by gwilliams on 12/6/13.
//  Copyright (c) 2013 Rave Social, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol RaveProgressDelegate <NSObject>
@property (nonatomic, readonly) BOOL isShowing;
- (void)showWithMessage:(NSString *)message andCancelCallback:(dispatch_block_t)callback;
- (void)showInView:(UIView *)view withMessage:(NSString *)message andCancelCallback:(dispatch_block_t)callback;
- (void)dismiss;
@end
