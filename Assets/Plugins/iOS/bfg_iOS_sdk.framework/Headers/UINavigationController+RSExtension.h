//
//  UINavigationController+RSExtension.h
//
//  RaveUI
//
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface UINavigationController (RSExtension)

/**
 You must use this method to create navigation conrollers, hosting RaveScenes.
 */
+ (UINavigationController*)rs_navigationControllerWithRootViewController:(UIViewController*)aController;
@end
