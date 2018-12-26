//
//  RaveAppDataKeysManager.h
//  RaveSocial
//
//  Copyright (c) 2015 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@class RaveAppDataKeyUserPair;
/**
 *  Implement this observer protocol when using the app data keys system to track save keys for cloud storage
 */
@protocol RaveAppDataKeysStateObserver <NSObject>
@required

/**
 *  This method will be called after init to establish your current identity or if you are currently in an unresolved state.
 *  It will also be called when the system has detected that, after a merge, the current user has no selected app data
 *  key. This means that both users in the merge had a selected key and a determination needs to be made about which merged
 *  key should become the selected key. See RaveAppDataKeys(Manager) for details on retrieving the unresolved keys and
 *  selecting the valid key.
 *
 *  @param selectedKey    The key selected for this application and user
 *  @param unresolvedKeys Keys that need to be resolved, there will always be zero or two or more
 */
- (void)appDataKeyStateChanged:(NSString *)selectedKey unresolvedKeys:(NSArray *)unresolvedKeys;
@end

/**
 *  Callback to provide all possible details for application data keys for this user and application
 *
 *  @param selectedKey    The key selected for this application and user
 *  @param rejectedKeys   Keys that have been rejected for use as the selected key
 *  @param unresolvedKeys Keys that need to be resolved, there will always be zero or two or more
 *  @param error          An error or nil
 */
typedef void (^RaveAppDataKeysStateCallback)(NSString * selectedKey, NSArray * rejectedKeys, NSArray * unresolvedKeys, NSError * error);

/**
 *  Callback to fetch a subset of the app data keys information
 *
 *  @param keys  The array of requested keys
 *  @param error An error or nil
 */
typedef void (^RaveAppDataKeysCallback)(NSArray * keys, NSError * error);

/**
 *  Callback to fetch the selected key
 *
 *  @param selectedKey The app data key selected for the current user
 *  @param error       An error or nil
 */
typedef void (^RaveAppDataKeyCallback)(NSString * selectedKey, NSError * error);

/**
 *  Callback to fetch an array of keys.
 *
 *  @param selectedKeys The array of selected app data keys
 *  @param error        An error or nil
 */
typedef void (^RaveAppDataKeySetCallback)(NSArray <RaveAppDataKeyUserPair *> * userKeyPairs, NSError * error);

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
 */
@protocol RaveAppDataKeysManager <NSObject>
/**
 *  Set an instance of the appDataKeysStatusObserver
 *
 *  @param stateObserver Observer for changes in status
 */
- (void)setStateObserver:(id<RaveAppDataKeysStateObserver>)stateObserver;

/**
 *  Cached last selected key, if any. When possible prefer key from state observer
 *
 *  @return Last selected app key or nil
 */
- (NSString *)lastSelectedKey;

/**
 *  Fetch the selected key, rejected keys, and unresolved keys for this application
 *
 *  @param callback Provides information about data keys for the current application or an error
 */
- (void)fetchCurrentState:(RaveAppDataKeysStateCallback)callback;

/**
 *  Fetch the set of unresolved keys, will either have a count of zero or two or more
 *
 *  @param callback Callback supplying the keys or an error
 */
- (void)fetchUnresolved:(RaveAppDataKeysCallback)callback;

/**
 *  Fetch the currently selected key for the user.  Will be nil unless a key has been selected
 *
 *  @param callback Callback supplying the selected key or an error
 */
- (void)fetchSelected:(RaveAppDataKeyCallback)callback;

/**
 *  Fetch values available to be used as keys
 *
 *  This list will included every Rave identity associated with the current account
 *
 *  @param callback Callback supplying the requested keys or an error
 */
- (void)fetchAvailable:(RaveAppDataKeysCallback)callback;

/**
 *  Change the selected key to the specified value
 *
 *  @param selectedKey The desired value of the new selected key
 *  @param callback    Callback suppyling an error or nil on success
 */
- (void)selectKey:(NSString *)selectedKey callback:(RaveCompletionCallback)callback;

/**
 * Deactivates a key
 *
 * Requires active network and only works for keys which are selected, rejected or unresolved
 *
 *  @param key The key to deactivate, must be 32 character hex format
 *  @param callback Callback suppyling an error or nil on success
 */
- (void)deactivateKey:(NSString *)key callback:(RaveCompletionCallback)callback;

/**
 *  Fetch the currently selected key for a specified user.
 *
 *  Requires active network and only works for keys which are selected
 *
 *  @param raveId   The user raveId to access their key
 *  @param callback Callback supplying the selected key or an error
 */
- (void)fetchUserKey:(NSString *)raveId callback:(RaveAppDataKeyCallback)callback;

/**
 *  Fetch a set of keys given an array of RaveIDs.
 *
 *  Requires an active network and only works for keys which are selected.
 *
 *  @param raveIds  The list of desired RaveIDs.
 *  @param callback Callback supplying a list of AppDataKeys or an error
 */
- (void)fetchUserKeySet:(NSArray <NSString *>*)raveIds callback:(RaveAppDataKeySetCallback)callback;

/**
 *  Fetch the current user's contacts app data keys.
 *
 *  @param callback Callback supplying a list of app data keys or an error.
 */
- (void)fetchUserKeySetForContacts:(RaveAppDataKeySetCallback)callback;
@end
