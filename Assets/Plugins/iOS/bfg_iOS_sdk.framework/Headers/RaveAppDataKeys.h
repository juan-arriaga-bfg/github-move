//
//  RaveAppDataKeys.h
//  RaveSocial
//
//  Copyright (c) 2015 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "RaveAppDataKeysManager.h"

/**
 * Manages app data keys
 *
 * App data keys can be used to map cloud or other back-end storage data to a user account per-application.  Keys are based off of RaveIDs.
 * By default a user doesn't have a selected key.  Push to select, typically the current Rave id. If an instance of the state observer is set Rave will automatically push the current Rave id as the selected key.
 * When a user is merged and both the source and target user of the merge have an active key for a game, the keys will enter
 * an unresolved state and no key will be selected.  This API provides the unresolved keys so that an application can provide
 * a selection, either automatically or via user input.  Once a key is selected, the remaining unresolved keys will become rejected.
 * Rejected keys can be selected at any time.
 *
 * To assist with migration from mapping RaveID directly as an app data key, pushIdentityAsKey can be used.
 *
 * Any key in the list of all available can be selected at any time.
 *
 */@interface RaveAppDataKeys : NSObject
/**
 *  Set an instance of the appDataKeysStatusObserver.  When this policy is set Rave will automatically set the selected key to the current Rave id
 *
 *  @param stateObserver Observer for changes in status
 */
+ (void)setStateObserver:(id<RaveAppDataKeysStateObserver>)stateObserver;

/**
 *  Cached last selected key, if any. When possible prefer key from state observer
 *
 *  @return Last selected app key
 */
+ (NSString *)lastSelectedKey;

/**
 *  Fetch all information about data keys for this application
 *
 *  @param callback Provides information about data keys for the current application or an error
 */
+ (void)fetchCurrentState:(RaveAppDataKeysStateCallback)callback;

/**
 *  Fetch the set of unresolved keys, will either have a count of zero or two or more
 *
 *  @param callback Callback supplying the keys or an error
 */
+ (void)fetchUnresolved:(RaveAppDataKeysCallback)callback;

/**
 *  Fetch the currently selected key for the user.  Will be nil unless a key has been selected
 *
 *  @param callback Callback supplying the selected key or an error
 */
+ (void)fetchSelected:(RaveAppDataKeyCallback)callback;

/**
 *  Fetch values available to be used as keys
 *  
 *  This list will included every Rave identity associated with the current account
 *
 *  @param callback Callback supplying the requested keys or an error
 */
+ (void)fetchAvailable:(RaveAppDataKeysCallback)callback;

/**
 *  Change the selected key to the specified value
 *
 *  @param selectedKey The desired value of the new selected key
 *  @param callback    Callback suppyling an error or nil on success
 */
+ (void)selectKey:(NSString *)selectedKey callback:(RaveCompletionCallback)callback;

/**
 *  Deactivate an identity key
 *
 *  @param key The key to delete
 *  @param callback Callback suppyling an error or nil on success
 */
+ (void)deactivateKey:(NSString *)key callback:(RaveCompletionCallback)callback;

/**
 * Fetch a key for a given user by their raveId
 *
 *  @param raveId   The raveId of the user find an available active key
 *  @param callback Callback supplying the requested keys or an error
 */
+ (void)fetchUserKey:(NSString *)raveId callback:(RaveAppDataKeyCallback)callback;

/**
 *  Fetch a set of keys given an array of RaveIDs.
 * 
 *  @param raveIds  The list of desired RaveIDs.
 *  @param callback Callback supplying a list of app data keys or an error
 */
+ (void)fetchUserKeySet:(NSArray <NSString *>*)raveIds callback:(RaveAppDataKeySetCallback)callback;

/**
 *  Fetch the current user's contacts app data keys.
 *
 *  @param callback Callback supplying a list of app data keys or an error.
 */
+ (void)fetchUserKeySetForContacts:(RaveAppDataKeySetCallback)callback;
@end
