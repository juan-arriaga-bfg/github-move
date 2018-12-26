//
//  RaveNewsletterScene.h
//
//  RaveUI
//  Copyright (c) 2017 Rave, Inc. All rights reserved.
//

/**
 *  Sign up data for use in callbacks
 */

#import "RaveSignUpEmailScene.h"
#import "AuthFlowScene.h"

/**
 *  Show this scene if you explicitly want to ask the user to create a BigFish account without password.
 *  This will sign them up for newsletter
 */
@interface RaveNewsletterScene : AuthFlowScene

/**
 *  Callback that will include sign up data if the user did sign up.  On cancel expect sign up
 *  data and error to be nil
 */
@property (nonatomic, copy) BigFishSignUpCallback signUpCallback;

/**
 *  Set to any predetermined email address, defaults to nil
 */
@property (nonatomic, copy) NSString* email;

@end
