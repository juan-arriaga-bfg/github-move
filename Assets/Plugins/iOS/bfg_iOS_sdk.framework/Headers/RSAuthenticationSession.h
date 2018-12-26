//
//  RSAuthenticationSession.h
//
//  RaveSocial
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <Accounts/ACAccount.h>
#import "RaveConnectPlugin.h"

@protocol RSModel;
typedef void (^RSASLoginCompletionCallback)(id<RSModel> aModel);

typedef enum
{
    kRSUnknownState,
    kRSFailedState,
    kRSSuccessState,
} RSAuthenticationSessionState;

@protocol RSReadOnlyUserProfile;
@protocol RSOperation;
@interface RSAuthenticationSession : NSObject

@property (nonatomic, assign) RSAuthenticationSessionState state;

/**
 Possible error. See RSError values.
*/
@property (nonatomic, retain, readonly) NSError* error;

/**
 User's email
*/
@property (nonatomic, retain) NSString* email;

/**
 The user's name as displayed on leaderboards.
*/
@property (nonatomic, retain) NSString* userName;

/**
 The user's date of birth.
*/
@property (nonatomic, retain) NSDate* birthdate;

/**
 A user's password.
 */
@property (nonatomic, retain) NSString* password;

/**
 Uses email property to find RSReadOnlyUserProfile objects known in system. If there
 is no Internet connection, local user information is returned, otherwise we make
 a call to the server to fetch data.
*/
- (void)findAllUserProfilesByEmail:(void (^)(NSArray* anUserProfiles))aCompletion;

@end


@interface RSAuthenticationSession (SocialExtension)

// Twitter

/**
 Fetches an array of twitter ACAccount objects. Can lead to:
 kRSTwitterAccessDeniedError in case user denied twitter access.
 If RSTwitterSettings's forceAccountCreation
 flag is set to YES, then this call will present web view to sign in with twitter for user 
 in case no account was found. Otherwise kRSTwitterNoAccountsFoundError will be set.
 If user will cancel twitter sign in flow kRSOperationCancelledError is returned. 
 anAccounts array is never nil.
*/
- (void)fetchTwitterAccountsWithCompletion:(void (^)(NSArray* anAccounts , NSError* error))aCallback;

/*
 Can lead to:
 kRSTwitterAuthenticationDataInvalidError in case twittter credentials
 have been expired and user was redirected to go to settings.
 kRSUserDoesNotExistError in case user with provided Twitter credentials is
 not known to Rave.
 */
- (void)signInWithTwitterAccount:(ACAccount*)twAccount completion:(RSASLoginCompletionCallback)aCallback;

/*
 User is already logged in on successfull call.
 Can lead to:
 kRSTwitterAuthenticationDataInvalidError in case twittter credentials
 have been expired and user was redirected to go to settings.
 Can lead to kRSUserExistsError in case user with provided Twitter credentials is
 already known to Rave.
 */
- (void)signUpWithTwitterAccount:(ACAccount*)twAccount completion:(RSASLoginCompletionCallback)aCallback;


@end

