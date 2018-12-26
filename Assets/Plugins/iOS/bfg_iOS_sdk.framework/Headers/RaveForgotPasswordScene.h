//
//  RaveForgotPasswordScene.h
//  BigFishScenePack
//
//  Created by Anton Rivera on 5/24/17.
//  Copyright © 2017 Rave Social, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/RaveSocial.h>
#import "RaveSignUpEmailScene.h"
#import "AuthFlowScene.h"

@interface RaveForgotPasswordScene : AuthFlowScene

/**
 *  Use this callback to check for login and details of account creation
 */
@property (nonatomic, copy) BigFishSignUpCallback signUpCallback;
/**
 *  Set to any predetermined email address, defaults to nil
 */
@property (nonatomic, copy) NSString* email;

@end
