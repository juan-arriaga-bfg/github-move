//
//  RaveConnectPlugin.h
//  RaveSocial
//
//  Created by gwilliams on 9/29/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <bfg_iOS_sdk/RaveSocial.h>

/** This protocol should be implemented by any third party connect plugin
 
 Generally speaking Rave integrators don't need to use this interface directly.  Wherever possible it will be called by various Rave sub-systems for you.  Most of the methods will be used indirectly through the RaveAuthenticationManager and other Rave manager objects.
 
 */

@protocol RaveConnectPlugin;

/**
 *  This enum describes the different types of tokens expected to be cached in a connect plugin
 */
typedef NS_ENUM(NSInteger, RaveConnectPluginTokenType) {
    /**
     *  Access token type, the default token type
     */
    RaveConnectPluginTokenTypeAccessToken,
    /**
     *  Secret token type, will be nil for most connect plugins
     */
    RaveConnectPluginTokenTypeSecret,
};

/**
 *  This enum describes different possible states for friends syncing for a given plugin
 */
typedef NS_ENUM(NSInteger, RaveFriendsSyncState) {
    /**
     *  Friends synced when plugin is "ready" and AutoSyncFriends is enabled for given plugin
     */
    RaveFriendsSyncStateDefault,
    /**
     *  User has opted in to contacts syncing for given plugin
     */
    RaveFriendsSyncStateEnabled,
    /**
     *  User has opted out of contacts syncing for given plugin
     */
    RaveFriendsSyncStateDisabled,
};

/**
 *  Callback used when attempting to migrate a token
 *
 *  @param result Describes success or failure of acquiring a token by migration
 *  @param error         Error for inferal attempt or nil
 */
typedef void (^RaveMigrateTokenCallback)(RaveCallbackResult result, NSError * error);

/**
 *  Callback that an integrator will call if implementing a custom disconnect policy
 *
 *  @param disconnectAllowed YES if disconnect is allowed, otherwise NO
 *  @param error             An error if appropriate, or nil
 */
typedef void (^RaveCanDisconnectCallback)(BOOL disconnectAllowed, NSError * error);
/**
 *  Protocol for the RaveConnectPlugin Disconnect policy
 *
 *  Implement this policy if you want to override the default behavior when a user attempts to disconnect a plugin from their account
 */
@protocol RaveDisconnectPolicy <NSObject>
- (void)canDisconnect:(NSString *)plugin callback:(RaveCanDisconnectCallback)callback;
@end

@interface RaveDefaultDisconnectPolicy : NSObject<RaveDisconnectPolicy>
@end

/**
 *  Enum describing different conflict types Rave may encounter during connect attempts
 */
typedef NS_ENUM(NSInteger, RaveConflictType) {
    /**
     *  This conflict type indicates that the current Rave account already has another provider account connected to it.
     *  The only way to resolve this conflict is to forceConnect using the current access token
     */
    RaveConflictTypeProviderAccountAlreadyAssociatedWithRaveAccount,
    /**
     *  This conflict type indicates the provider account is already in use with another Rave account.
     *  If your current Rave account is authenticated the only way to resolve this conflict is to first disconnect the provider account from your other Rave account.
     *  If your current Rave account is anonymous you can instead login as the other Rave user and Rave will merge the anonymous user into that account
     */
    RaveConflictTypeProviderAccountAlreadyInUse,
};
/**
 *  This callback is to indicate to Rave whether the conflict has been resolved and the user is connected
 *
 *  @param resolved YES if the conflict was resolved and the user is connected, otherwise NO
 */
typedef void (^RaveConflictResolvedCallback)(BOOL resolved, NSError * error);

/**
 *  Protocol for the RaveConnectPlugin Conflict Resolution policy
 *
 *  Implement this policy if you want to customize the response to conflicts that occur when connecting plugin accounts
 */
@protocol RaveConnectPlugin;
@protocol RaveConflictResolutionPolicy <NSObject>
/**
 *  This method is called when a conflict occurs when connecting a plugin to a Rave account
 *
 *  @param conflictType       This enum describes the type of comment that occurred
 *  @param resolutionCallback Fire this callback after the conflict has been resolved
 */
- (void)conflictOccurred:(RaveConflictType)conflictType plugin:(id<RaveConnectPlugin>)plugin resolutionCallback:(RaveConflictResolvedCallback)resolutionCallback;
@end

@interface RaveDefaultConflictResolutionPolicy : NSObject<RaveConflictResolutionPolicy>
@end

/**
 *  This enum describes options to resolve an invalid access token
 */
typedef NS_ENUM(NSInteger, RaveInvalidTokenResolution) {
    /**
     *  Inform Rave that a new token has been independently acquired
     */
    RaveInvalidTokenResolutionTokenAcquired,
    /**
     *  Ask Rave to retry token acquisition potentially allowing popover UI
     */
    RaveInvalidTokenResolutionRetryWithUI,
    /**
     *  Give up on acquiring a new token at this time
     */
    RaveInvalidTokenResolutionGiveUp,
};

/**
 *  Use this callback to tell Rave what steps to take to generate a new token
 *
 *  @param resolution The resolution action for Rave
 */
typedef void (^RaveInvalidTokenResolutionCallback)(RaveInvalidTokenResolution resolution);

/**
 *  Protocol for the RaveConnectPlugin Invalid Token policy
 *
 *  Implement this policy if you want to customize the default behavior when a plugin's access token is invalid.
 *  Rave will typically attempt to acquire a new token automatically without the use of UI before calling this method.
 */
@protocol RaveInvalidTokenPolicy <NSObject>
- (void)tokenInvalidated:(RaveInvalidTokenResolutionCallback)resolutionCallback;
@end

@interface RaveDefaultInvalidTokenPolicy : NSObject<RaveInvalidTokenPolicy>
@end

/**
 *  Protocol for determining when an automatic pre-existing token migration for authentication should be attempted.  If the result is YES Rave will automatically attempt authenticating using the token
 */
@protocol RaveTokenImportPolicy <NSObject>
- (BOOL)shouldImportToken:(NSString *)pluginKeyName;
- (void)performPostImportAction:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback;
@end

/**
 *  The default token migration policy is to automatically attempt authentication using a pre-existing token for any non-authenticated user
 */
@interface RaveDefaultTokenImportPolicy : NSObject<RaveTokenImportPolicy>
@end

/**
 *  This callback embodies the connected state for a plugin
 *
 *  @param isConnected YES if the plugin is connected to a Rave account, otherwise NO
 *  @param userId      A userid for the plugin account or nil
 *  @param error       An error, or nil if none occurred
 */
typedef void (^RaveConnectStateCallback)(BOOL isConnected, NSString * userId, NSError * error);

@class RSRestSocialMergeRequest;
@protocol RaveConnectPlugin <NSObject>
/**---------------------------------------------------------------------------------------
 * @name Token data
 *  ---------------------------------------------------------------------------------------
 */

/**
 *  This method returns the name the key the plugin is registered with
 *
 *  @return A string containing registered name for the plugin
 */
- (NSString *)keyName;

/**
 *  This method returns the display name for the plugin
 *
 *  @return The human readable name of the plugin
 */
- (NSString *)displayName;

/** This method returns the current access token for this plugin
 
 @return Returns the current access token or nil if none is present
 */
- (NSString *)currentToken;

/** This method returns the current token specified by the passed enum

 @param tokenType The type of token (access or secret) to be retrieved
 
 @return Returns the cached token of the specified type
 */
- (NSString *)getCurrentTokenByType:(RaveConnectPluginTokenType)tokenType;

/** This method is intended to trigger authentication via the plugin and to update the cached token on completion.
 
 @param callback If the operation is successful the completion callback will be passed nil for the error parameter
 */
- (void)getNewToken:(BOOL)allowUI callback:(RaveCompletionCallback)callback;

/**
 *  Since readiness may have changed since the plugin was last accessed, this explicitly is the last known state
 *
 *  A ready plugin will have all the cached information necessary to perform most of it's other operations without user interaction
 *
 *  @return Return the last known readiness state
 */
- (BOOL)lastKnownReadiness;

/**
 *  This method checks the readiness of the plugin
 *
 *  @param callback The readiness callback.  It will pass YES if ready or NO and an error if not
 */
- (void)checkReadiness:(RaveReadinessCallback)callback;

/**---------------------------------------------------------------------------------------
 * @name Log out cleanup
 *  ---------------------------------------------------------------------------------------
 */

/** This method triggers cleanup typically due to the user logging out of the system
 */
- (void)logOut;

/**---------------------------------------------------------------------------------------
 * @name Inferred login handling
 *  ---------------------------------------------------------------------------------------
 */

/** This method indicates whether a plugin supports inferred token acquisition
 @returns YES if inferred login is supported, otherwise NO
 */

- (BOOL)supportsTokenMigration;

/** This method is to cache a token that has been previously stored in a way that Rave can't automatically infer it.  Normally it's use would be optional.
 @param token The access token that you want Rave to use when attempting to automatically infer login using this plugin
 */
- (void)useTokenForRaveUpgrade:(NSString *)token;

/** This method indicates whether there is data that could potentially be used for an inferred login attempt
 @return YES if inferred login data is available otherwise NO
 */
- (BOOL)hasMigratableTokenData;

/** This method attempts inferred login during Rave initialization
 @param callback The callback will be used once the inferred login attempt has resolved.  An error will be set if the attempt fails, otherwise you should check to see if RaveSocial.isAuthenticated is YES.
 */
- (void)attemptMigratingToken:(RaveMigrateTokenCallback)callback;

/**---------------------------------------------------------------------------------------
 * @name Authentication key and dictionary for use with Rave server
 *  ---------------------------------------------------------------------------------------
 */

/** This method returns a dictionary of JSON mappable data to be sent for authentication to the Rave server.  Typically this will return null unless a customization has been made by the Rave Server team.
 @ returns JSON mappable authentication data for the Rave server or nil
 */
- (NSDictionary *)raveAuthDictionary;

/** This method returns a string key to be used in various Rave Server calls
 @return String authentication key used for Rave Server calls
 */
- (NSString *)raveAuthKey;

/**---------------------------------------------------------------------------------------
 * @name Achievement mirroring
 *  ---------------------------------------------------------------------------------------
 */

/** This method let's you set a dictionary of keys to map between Rave achievement keys and plugin specific keys.
 
 The map should map Rave achievement keys to plugin specific achievement keys.  It is purely optional.  If no map is set the Rave achievement key will be forwarded to the plugin service.
 
 @property achievementKeyMap Dictionary mapping Rave achievement keys to plugin specific keys, or nil
 */
@property (nonatomic, copy) NSDictionary * achievementKeyMap;

/** This method mirrors unlocking an achievement to the plugin service

 Mirroring will automatically be attempted if RaveSettings.DataMirror.Achievements includes the provider name of the plugin.  It is up to the plugin internally to interface with the plugin service achievement mechanism.
 
 @param achievementKey The Rave achievement key for the achievement to be unlocked, it will be mapped automatically if the achievementKeyMap is set
 @param callback Mirror may be asynchronous for this plugin.  The callback for the mirroring operation will occur when the mirroring operation is complete with either an error or nil.
 */
- (void)mirrorUnlockAchievement:(NSString *)achievementKey callback:(RaveCompletionCallback)callback;

/**---------------------------------------------------------------------------------------
 * @name Leaderboard mirroring
 *  ---------------------------------------------------------------------------------------
 */

/** This method let's you set a dictionary of keys to map between Rave leaderboard keys and plugin specific keys.
 
 The map should map Rave leaderboard keys to plugin specific leaderboard keys.  It is purely optional.  If no map is set the Rave leaderboard key will be forwarded to the plugin service.
 
 @property leaderboardKeyMap Dictionary mapping Rave leaderboard keys to plugin specific keys, or nil
 */
@property (nonatomic, copy) NSDictionary * leaderboardKeyMap;

/** This method mirrors a new score to the plugin service
 
 Mirroring will automatically be attempted if RaveSettings.DataMirror.Leaderboards includes the provider name of the plugin.  It is up to the plugin internally to interface with the plugin service leaderboard mechanism.
 
 @param leaderboardKey The Rave achievement key for the leaderboard to be unlocked, it will be mapped automatically if the leaderboardKeyMap is set
 @param callback Mirror may be asynchronous for this plugin.  The callback for the mirroring operation will occur when the mirroring operation is complete with either an error or nil.
 */
- (void)mirrorSubmitScore:(NSString *)leaderboardKey score:(NSNumber *)score callback:(RaveCompletionCallback)callback;

/**---------------------------------------------------------------------------------------
 * @name Posts
 *  ---------------------------------------------------------------------------------------
 */

/** This method indicates whether posting is supported by this plugin
 
 Posting is used for generic data publication, sometimes with an image
 
 @return YES if supported otherwise NO
 */

- (BOOL)supportsPost;
/** This method publishes a post to the plugin service
 @param subject The subject text of the post, or nil
 @param message The message text of the post, or nil
 @param imageData The image data to accompany the post.  Objects of types NSString, NSURL, UIImage, NSData, and nil are all acceptable values
 @param callback The callback will occur when the request is completed, passing an error or nil if successful
 */
- (void)post:(NSString *)subject message:(NSString *)message imageData:(id)imageData callback:(RaveCompletionCallback)callback;

/**---------------------------------------------------------------------------------------
 * @name External Contacts
 *  ---------------------------------------------------------------------------------------
 */

/** This method indicates whether external contacts are supported by this plugin
 
 External contacts are typically used for invitations, app requests, and gifting
 
 @return YES if supported otherwise NO
 */
- (BOOL)supportsExternalContacts;

/** This method fetches an array of external contacts on callback completion
 
 You can create an external contact by calling [RaveSocial.contactsManager createContactInstance:externalId:displayName:pictureUrl:]
 
 @param callback The fetch callback will either have an array of external contacts for the plugin or an error
 */
- (void)fetchExternalContacts:(RaveContactsCallback)callback;


/**---------------------------------------------------------------------------------------
 * @name Sharing
 *  ---------------------------------------------------------------------------------------
 */

/** This method indicates whether sharing is supported by this plugin
 
 Share is typically used for invitations and app requests
 
 @return YES if supported otherwise NO
 */
- (BOOL)supportsShare;

/** This method executes a share for the plugin
 @param externalIds The list of external ids that should be shared with or nil for everyone
 @param subject The subject text for the share post
 @param message The message text for the share post
 @param callback The callback indicates when the share request has been completed.  On success it should return a completed RaveShareRequest object, otherwse an error
 */
- (void)share:(NSArray *)externalIds subject:(NSString *)subject message:(NSString *)message callback:(RaveProviderShareRequestCallback)callback;

/**---------------------------------------------------------------------------------------
 * @name Share Install Ids
 *  ---------------------------------------------------------------------------------------
 */

/** This method indicates whether share install ids are supported by this plugin
 
 Share Install Ids are used for app requests that result in an app install to reward app installs
 
 @return YES if supported otherwise NO
 */
- (BOOL)supportsShareInstallId;

/** This method extracts a share install id from source data, typically a URL
 @return The share install id extracted or nil
 */
- (NSString *)extractShareInstallId:(NSString *)sourceData;

/**
 *  Accessor for an object implementing the RaveDisconnectPolicy protocol
 */
@property (nonatomic, retain) id<RaveDisconnectPolicy> disconnectPolicy;

/**
 *  Accessor for an object implementing the RaveConflictResolutionPolicy protocol
 */
@property (nonatomic, retain) id<RaveConflictResolutionPolicy> conflictResolutionPolicy;

/**
 *  Accessor for an object implementing the RaveInvalidTokenPolicy protocol
 */
@property (nonatomic, retain) id<RaveInvalidTokenPolicy> invalidTokenPolicy;

/**
 *  Accessor for an object implementing the RaveTokenImportPolicy protocol
 */
@property (nonatomic, retain) id<RaveTokenImportPolicy> tokenImportPolicy;

/**
 *  Provide hook to allow forwarding of system notification to underlying plugin SDK
 */
- (void)appDidBecomeActive;

/** This method is to forward URL's from the application delegate as needed.  No need to call directly, the authentication manager will cover it for you
 @param url The URL from the application delegate
 @param sourceApplication The source application from the application delegate
 @param annotation The annotation from the application delegate
 */
- (BOOL)handleURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;

/**
 *  This method returns the sync state for the given plugin, used to determine whether to sync friends
 *
 *  @return Current friends sync state, see RaveFriendsSyncState for more details
 */
- (RaveFriendsSyncState)friendsSyncState;

/**
 *  This method will cause the plugin to sync friends, usually with the Rave back-end
 *
 *  @param callback Callback indicating error or nil on success
 */
- (void)syncFriends:(RaveCompletionCallback)callback;
@end
