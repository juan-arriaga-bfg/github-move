//
//  RaveWelcomeSceneViewController.h
//  BigFishScenePack
//
//  Created by Rajesh Sabale on 6/15/17.
//  Copyright © 2017 Rave Social, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/RaveSocial.h>
#import "RaveSignUpEmailScene.h"

@interface RaveWelcomeScene : AuthFlowScene

/**
 *  Use this callback to check for login and details of account creation
 */
@property (nonatomic, copy) BigFishSignUpCallback signUpCallback;
@property (nonatomic, strong) BigFishSignUpData *signupData;

@end
