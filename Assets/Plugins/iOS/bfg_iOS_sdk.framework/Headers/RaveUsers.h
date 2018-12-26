//
//  RaveUsers.h
//  RaveSocial
//
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <bfg_iOS_sdk/RaveUser.h>
#import <bfg_iOS_sdk/RaveUsersManager.h>

@interface RaveUsers : NSObject
/**
 *  Retreive a user by Rave id
 *
 *  @param raveId The id for the user to retrieve
 *
 *  @return The user object requested or nil if not present
 */
+ (id<RaveUser>)getUserById:(NSString *)raveId;

/**
 *  Fetch a user by reference
 *
 *  @param reference Details to search for the user by
 *  @param callback  Callback providing the found user or potentially an error
 */
+ (void)fetchUser:(RaveUserReference *)reference callback:(RaveUserCallback)callback;

/**
 *  Request the user specified be updated from the Rave server, potentially filling out missing details
 *
 *  @param raveId   The id for the user to update
 *  @param callback    A callback indicating whether or not an error occured
 */
+ (void)updateUserById:(NSString *)raveId withCallback:(RaveCompletionCallback)callback;

/**
 *  Register an observer for the current user, multiple concurrent users may be registered at once
 *
 *  @param observer Observer for changes to the current user
 */
+ (void)addCurrentUserObserver:(id<RaveCurrentUserObserver>)observer;

/**
 *  Unregister an observer for the current user, multiple concurrent users may be registered at once
 *
 *  @param observer Observer for changes to the current user
 */
+ (void)removeCurrentUserObserver:(id<RaveCurrentUserObserver>)observer;

/**
 *  Accessor for the current user of the system
 */
+ (id<RaveUser>) current;

/**
 * Files a moderation request for a given user
 *
 * @param raveId UUID for user being reported
 * @param data Custom data to be associated to a report. Can be to up to 64 characters
 * @param callback A callback indicating whether or not an error occured
 */
+ (void)reportUser:(NSString *)raveId data:(NSString *)data callback:(RaveCompletionCallback)callback;

/**
 * Files a moderation request for a given user
 *
 * @param raveId UUID for user being reported
 * @param callback A callback indication whether or not an error occured
 */
+ (void)reportUser:(NSString *)raveId callback:(RaveCompletionCallback)callback;

/**
 *  Request the current user be updated from the Rave server, potentially filling out missing details
 *
 *  @param callback    A callback indicating whether or not an error occured
 */

+ (void)updateCurrent:(RaveCompletionCallback)callback;

/**
 *  Push changes to the current user to the Rave backend.  User state notifications will be triggered if successful
 *
 *  @param userChanges The set of changes to the current user (see RaveUserChanges for more details)
 *  @param callback    A callback indicating whether or not an error occured
 */
+ (void)pushUserChanges:(RaveUserChanges *)userChanges callback:(RaveCompletionCallback)callback;

/**
 *  Push the binary data for a local image to the Rave backend for a user's profile picture
 *
 *  @param data     May be of types NSString, NSData, UIImage, or NSURL
 *  @param callback A callback indicating whether an error occurred
 */
+ (void)pushProfilePicture:(id)data callback:(RaveCompletionCallback)callback;

/**
 *  Push the binary data for a local image to the Rave backend for a users profile picture
 *
 *  @param url      The URL specifying the local image to upload
 *  @param callback    A callback indicating whether or not an error occured
 */
+ (void)pushProfilePicture:(NSString*)url withCallback:(RaveCompletionCallback)callback;

/**
 *  Check to see if a Rave account exists for the specified email address
 *
 *  @param email    The email to search for
 *  @param callback A callback indicating whether such a user exists or an error
 */
+ (void)checkAccountExists:(NSString *)email withCallback:(RaveAccountExistsCallback)callback;

/**
 *  Check to see if a Rave account authenticated by third party using the specified email
 *
 *  @param email    The email to search for
 *  @param callback A callback indicating whether such a user exists or an error
 */
+ (void)checkThirdPartyAccountExists:(NSString *)email withCallback:(RaveAccountExistsCallback)callback;

/**
 *  Fetch an access token from the Rave backend for the current user
 *
 *  @param callback Callback providing the fetched token or an error
 */
+ (void)fetchAccessToken:(RaveAccessTokenCallback)callback;

/**
 *  Fetch a random assortment of users from the current application
 *
 *  @param callback A set of users or an error
 */
+ (void)fetchRandomUsersForApplication:(RaveUsersCallback)callback;

/**
 *  Fetch a random assortment of users from the specified application
 *
 *  @param appUuid  Uuid for the application to query
 *  @param callback A set of users or an error
 */
+ (void)fetchRandomUsersForApplication:(NSString *)appUuid callback:(RaveUsersCallback)callback;

/**
 *  Fetch a random assortment of users from the specified application, narrowed by whether they should
 *  also be contacts
 *
 *  @param appUuid         Uuid for the application to query
 *  @param excludeContacts Boolean indicating whether to exclude contacts
 *  @param limit           Value indicating the limit on number of users to fetch
 *  @param callback        A set of users or an error
 */
+ (void)fetchRandomUsersForApplication:(NSString *)appUuid excludeContacts:(NSNumber *)excludeContacts withLimit:(NSNumber *)limit callback:(RaveUsersCallback)callback;

/**
 *  If a Rave account has been merged it will potentially have multiple user uuid over the lifetime of the
 *  account.  This method will fetch the complete list of identities that have been associated with the
 *  current account.
 *
 *  @param callback A list of identities or an error
 */
+ (void)fetchAllIdentities:(RaveIdentitiesCallback)callback;

/**
 *  If a Rave account has been merged it will potentially have multiple user uuid over the lifetime of the
 *  account.  This method will fetch the list of identities for the current application that have been
 *  associated with the current account.
 *
 *  @param callback A list of identities or an error
 */
+ (void)fetchIdentitiesForApplication:(RaveIdentitiesCallback)callback;

/**
 *  Deprecated push changes to the current user to the Rave backend
 *
 *  @param callback Callback indicating whether an error occurred or not
 */
+ (void)pushCurrent:(RaveCompletionCallback)callback;
@end
