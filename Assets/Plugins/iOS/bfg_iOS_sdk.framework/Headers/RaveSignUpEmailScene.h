//
//  RSSignUpEmailScene.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import "AuthFlowScene.h"

/**
 *  Sign up data for use in callbacks
 */
@interface BigFishSignUpData : NSObject
/**
 *  Accepted newsleter indicates whether or not the user accepted offer for newsletter
 *  -  Will always be NO if the user was not asked or cancelled
 */
@property (nonatomic, assign) BOOL acceptedNewsletter;

/**
 *  Indicates whether the user passed the COPPA check
 *  -  Will always be NO if the user was not asked or cancelled
 */
@property (nonatomic, assign) BOOL passedCoppaCheck;

/**
 *  Birth year indicates the birth year specified by the user, if asked
 *  -  Will be nil if the user was not asked or cancelled
 */
@property (nonatomic, copy) NSString * birthYear;
@end

/**
 *  Callback that provides sign up data for new users
 *  @param result     See RaveCallbackResult in RaveSocial/RaveUtilities.h for more details
 *  @param signUpData This value will be non-nil if they created an account, nil otherwise
 *  @param error      This value will only be non-nil if there was an error
 */
typedef void (^BigFishSignUpCallback)(RaveCallbackResult result, BigFishSignUpData *signUpData, NSError *error);

/**
 *  Show this scene if you explicitly want to ask the user to create a BigFish account
 *  -  Is also indirectly accessible through RaveLoginScene
 */
@interface RaveSignUpEmailScene : AuthFlowScene

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
