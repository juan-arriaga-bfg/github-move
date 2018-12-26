//
//  RaveMGCAccountInfoSceneViewController.h
//  BigFishScenePack
//
//  Created by Rajesh Sabale on 7/7/17.
//  Copyright © 2017 Rave Social, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/RaveSocial.h>
#import "RaveSignUpEmailScene.h"
#import "AuthFlowScene.h"

@interface RaveMGCAccountInfoScene : AuthFlowScene

@property (nonatomic, assign) BOOL                  isSubscriber;
@property (nonatomic, assign) NSUInteger            numberOfCredits;
@property (nonatomic, copy) BigFishSignUpCallback   signUpCallback;

@end
