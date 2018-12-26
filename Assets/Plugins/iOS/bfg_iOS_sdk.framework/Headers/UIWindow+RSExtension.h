//
//  UIWindow+RSExtension.h
//  RaveSocial
//
//  Created by Iaroslav Pavlov on 11/1/13.
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface UIWindow (RSExtension)

+ (UIWindow*)rs_topWindow;
+ (UIWindow*)rs_createAlertWindow;
+ (UIWindow*)rs_createTemporaryWindow;
@end
