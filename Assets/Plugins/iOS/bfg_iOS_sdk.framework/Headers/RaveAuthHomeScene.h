//
//  RaveAuthHomeScene.h
//  BigFishScenePack
//
//  Created by Rajesh Sabale on 5/23/17.
//  Copyright © 2017 Rave Social, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/RaveSocial.h>
#import "RaveSignUpEmailScene.h"
#import <bfg_iOS_sdk/RaveUtilities.h>
#import "AuthFlowScene.h"

@interface RaveAuthHomeScene : AuthFlowScene

/**
 *  Use this callback for login and details of account creation scenes
 */
@property (nonatomic, copy) BigFishSignUpCallback signUpCallback;

@end
