//
//  RaveSignOutScene.h
//  BigFishScenePack
//
//  Created by Anton Rivera on 6/6/17.
//  Copyright © 2017 Rave Social, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <bfg_iOS_sdk/RaveUtilities.h>
#import "RaveSignUpEmailScene.h"
#import "AuthFlowScene.h"

@interface RaveSignOutScene : AuthFlowScene

/**
 *  Use this callback to check for login and details of account creation
 */
@property (nonatomic, copy) BigFishSignUpCallback signUpCallback;

@end
