//
//
//  RaveUser.h
//  RaveSocial
//
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 *  Enumeration describing RaveUser state
 */
typedef NS_ENUM(NSInteger, RaveUserState) {
    /**
     *  No user state, typical for users for contacts or from leaderboards
     */
    RaveUserStateNone,
    /**
     *  An anonymous user, typically only the current user
     */
    RaveUserStateAnonymous,
    /**
     *  A personalized user, not authenticated but with some changes. Typically only the current user
     */
    RaveUserStatePersonalized,
    /**
     *  An authenticated user, verified by external authentication at some point during the life of the user
     */
    RaveUserStateAuthenticated
};

/**
 *  Inteface for a RaveUser
 *
 *  Most commonly the current user, but also for Rave backed contacts and leaderboards
 */
@class RaveUserReference;
@protocol RaveUser <NSObject>
/**
 *  Convenience method returning RaveUserReference for this user
 *
 *  @return User reference
 */
- (RaveUserReference *)reference;

/**
 *  YES if the current user is anonymous, otherwise NO
 */
@property (nonatomic, assign, readonly) BOOL isGuest;

/**
 *  The user's display name or nil
 */
@property (nonatomic, retain) NSString * displayName;

/**
 *  The user's real name or nil
 */
@property (nonatomic, retain) NSString * realName;

/**
 *  The user's username, should never be nil
 */
@property (nonatomic, retain) NSString * username;

/**
 *  The user's email or nil
 */
@property (nonatomic, retain) NSString * email;

/**
 *  The user's birthdate or nil
 */
@property (nonatomic, retain) NSDate * birthdate;

/**
 *  The user's raveId or uuid, should never be nil
 */
@property (nonatomic, retain) NSString * raveId;

/**
 *  The user's Facebook id or nil
 *
 *  This field will only be set for the current user if they've authenticated using Facebook for the current application
 */
@property (nonatomic, retain) NSString * facebookId;

/**
 *  The user's Google+ id or nil
 *
 *  This field will only be set for the current user if they've authenticated using Google+
 */
@property (nonatomic, retain) NSString * googlePlusId;

/**
 *  The user's third party id or nil
 *
 *  This field will only be set for the current user if they've authenticated using a third party source
 */
@property (nonatomic, retain) NSString * thirdPartyId;

/**
 *  The user's gender or nil
 */
@property (nonatomic, retain) NSString * gender;

/**
 *  The user's profile picture URL or nil
 */
@property (nonatomic, retain) NSString * pictureURL;

/**
 *  The user's profile picture image or nil
 *
 *  Will be nil until updatePictureImage has been called and pictureURL is non-nil
 */
@property (nonatomic, retain) UIImage * pictureImage;

/**
 *  The user's account state, see RaveUserState for more details
 */
@property (nonatomic, assign, readonly) RaveUserState accountState;

/**
 *  Convenience method to update the cached profile picture image
 *
 *  @param callback Will return nil on success otherwise an error
 */
- (void)updatePictureImage:(RaveCompletionCallback)callback;
@end
