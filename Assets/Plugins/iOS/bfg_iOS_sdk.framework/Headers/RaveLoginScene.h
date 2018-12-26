//
//  RaveLoginScene.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <bfg_iOS_sdk/RaveUtilities.h>
#import "RaveSignUpEmailScene.h"
#import "AuthFlowScene.h"

/**
 *  Show this scene to ask a user to login in, or perhaps to create a new account
 *  -  Also may be shown indirectly through RaveSignUpEmailScene
 */
@interface RaveLoginScene : AuthFlowScene

/**
 *  Use this callback to check for login and details of account creation
 */
@property (nonatomic, copy) BigFishSignUpCallback signUpCallback;
/**
 *  Set to any predetermined email address, defaults to nil
 */
@property (nonatomic, copy) NSString* email;

@end
