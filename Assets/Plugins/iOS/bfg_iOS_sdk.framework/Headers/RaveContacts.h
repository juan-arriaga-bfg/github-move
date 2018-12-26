//
//  RaveContacts.h
//  RaveSocial
//
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <bfg_iOS_sdk/RaveContact.h>

/**
 *  RaveContacts Manager convenience interface
 */
@interface RaveContacts : NSObject
/**
 *  getContactByRaveId
 *
 *  @param raveId RaveUser's raveId (uuid) identifying the desired user
 *
 *  @return The requested user or nil
 */
+ (id<RaveContact>)getContactByRaveId:(NSString *)raveId;

/**
 *  Factory method for creating an external contact instance
 *
 *  Called internally, typically only called when reverse mapping contacts from a language binding
 *  or when implementing a custom RaveConnectPlugin
 *
 *  @param source      Source for contact (e.g. Facebook)
 *  @param externalId  Id for external source (e.g. Facebook id)
 *  @param displayName Display name for contact
 *
 *  @return Returns an instance of an external contact with the given details
 */
+(id<RaveContact>)createContactInstance:(NSString *)source externalId:(NSString *)externalId displayName:(NSString *)displayName;

/**
 *  Factory method for creating an external contact instance
 *
 *  Called internally, typically only called when reverse mapping contacts from a language binding
 *  or when implementing a custom RaveConnectPlugin
 *
 *  @param source      Source for contact (e.g. Facebook)
 *  @param externalId  Id for external source (e.g. Facebook id)
 *  @param displayName Display name for contact
 *  @param pictureUrl  Profile picture URL for contact
 *
 *  @return Returns an instance of an external contact with the given details
 */
+(id<RaveContact>)createContactInstance:(NSString *)source externalId:(NSString *)externalId displayName:(NSString *)displayName pictureUrl:(NSString *)pictureUrl;

/**
 *  Cache accessor for unfiltered contacts
 */
+ (NSOrderedSet *)all;

/**
 *  Method to update unfiltered contacts cache
 *
 *  @param callback Callback will return nil on success otherwise an error
 */
+ (void)updateAll:(RaveCompletionCallback)callback;

/**
 *  Cache accessor for contacts using this application
 */
+ (NSOrderedSet *)allUsingThisApplication;

/**
 *  Method to update cache for contacts using using this applciation
 *
 *  @param callback Callback will return nil on success otherwise an error
 */
+ (void)updateAllUsingThisApplication:(RaveCompletionCallback)callback;

/**
 * Cache accessor for contacts using Facebook
 */
+ (NSOrderedSet *)facebook;

/**
 *  Method to update cache for contacts using facebook
 *
 *  @param callback Callback will return nil on success otherwise an error
 */
+ (void)updateFacebook:(RaveCompletionCallback)callback;

/**
 *  Method to verify that users with Facebook ids are contacts friends
 *
 *  @param fbIds    Facebook ids for users to verify
 *  @param callback Will provide an array of verified contacts or an error
 */
+ (void)verifyFacebookFriendsWithIds:(NSArray *)fbIds withCallback:(void (^)(NSArray * verified, NSError * error))callback;

/**
 *  Method to add an array of contacts by username, they will be RaveSocial sourced
 *
 *  @param usernames Array of usernames to add
 *  @param callback Callback will return nil on success otherwise an error
 */
+ (void) addContactsByUsername:(NSArray*)usernames withCallback:(RaveCompletionCallback)callback;

/**
 *  Remove a contact by Rave id (uuid)
 *
 *  @param userUuid The Rave id (uuid) of the user to remove from contacts
 *  @param callback Callback will return nil on success otherwise an error
 */
+ (void) deleteContact:(NSString *)userUuid withCallback:(RaveCompletionCallback)callback;

/**
 *  Cache accessor for contacts using a specfied application
 *
 *  @param appId Application uuid for the contacts to track
 *
 *  @return A set of contacts for the application specified or nil
 */
+ (NSOrderedSet *)getAllUsingApplication:(NSString *)appId;

/**
 *  Method to update cache for contacts using a specified application
 *
 *  @param appId Application uuid for the contacts to track
 *  @param callback Callback will return nil on success otherwise an error
 */
+ (void)updateAllUsingApplication:(NSString *)appId withCallback:(RaveCompletionCallback)callback;

/**
 *  Method to start updating Rave contacts using the service specified
 *
 *  This typically will require the user to authenticate using that service
 *
 *  Will enable future updates to happen automatically based on RaveSettings.General.ContactsUpdateInterval
 *  Will automatically be called by +[RaveSocial connectTo] if RaveSettings.General.AutoSyncFriends includes a key matching pluginKeyName
 *
 *  @param pluginKeyName The key for the plugin to sync friends with
 *  @param callback Callback will return nil on success otherwise an error
 */
+ (void)startFriendsSyncFor:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback;

/**
 *  Method to stop Rave from automatically syncing the contacts for the associated plugin
 *
 *  @param pluginKeyName The key for the plugin to sync friends with
 */
+ (void)stopFriendsSyncFor:(NSString *)pluginKeyName;

/**
 *  Method to fetch external contacts from all plugins. An external contact is a contact associated with an external service that isn't necessarily associated with Rave
 *
 *  @param filter   Filter to specify which external contacts should be fetched
 *  @param callback A container of external contacts or an error
 */
+ (void)fetchAllExternal:(RaveContactsFilter)filter withCallback:(RaveContactsCallback)callback;

/**
 *  Method to fetch external contacts from the specified plugin. An external contact is a contact associated with an external service that isn't necessarily associated with Rave
 *
 *  @param pluginKeyName Key name for the plugin to use to fetch external contacts
 *  @param filter        Filter to specify which external contacts should be fetched
 *  @param callback      A container of external contacts or an error
 */
+ (void)fetchExternalFrom:(NSString *)pluginKeyName filter:(RaveContactsFilter)filter withCallback:(RaveContactsCallback)callback;

/**
 *  Consider deprecated, internally calls RaveSharingManager shareWith:viaPlugin...
 */
+ (void)inviteToAppVia:(NSString *)pluginKeyName withContacts:(NSArray *)contacts withMessage:(NSString *)invitation withCallback:(RaveCompletionCallback)callback;

@end
