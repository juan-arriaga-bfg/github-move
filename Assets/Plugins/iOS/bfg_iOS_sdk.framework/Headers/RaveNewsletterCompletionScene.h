//
//  RaveNewsletterCompletionSceneViewController.h
//  BigFishScenePack
//
//  Created by Rajesh Sabale on 6/8/17.
//  Copyright © 2017 Rave Social, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/RaveSocial.h>
#import "AuthFlowScene.h"

@interface RaveNewsletterCompletionScene : AuthFlowScene
@property (assign, nonatomic) BOOL isSuccessScreen;
@property (assign, nonatomic) BOOL COPPAError;
@property (assign, nonatomic) BOOL CASLOptOut;
@end
