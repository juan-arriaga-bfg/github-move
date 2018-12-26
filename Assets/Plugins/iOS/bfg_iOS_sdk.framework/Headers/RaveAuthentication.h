//
//  RaveAuthentication.h
//  RaveSocial
//
//  Created by gwilliams on 6/3/15.
//  Copyright (c) 2015 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <bfg_iOS_sdk/RaveAuthenticationManager.h>

/**
 *  This manager class owns and utilizes the RaveConnectPlugins.  It unifies the plugins with REST methods
 *  to manage authentication through the Rave backend as well as other services. Typically it will not be
 *  called directly by a Rave integrator but rather by other layers of the system.
 */

@interface RaveAuthentication : NSObject
/**
 *  Plugin management methods
 */

/**
 *  Convenience method to access all registered plugins
 *
 *  @return A container of the registered plugins
 */
+ (NSArray *)connectPlugins;

/**
 *  This method will return a connect plugin instance referred to by pluginKeyName
 *
 *  @param pluginKeyName The pluginKeyName key for the plugin used when registering
 *
 *  @return The plugin registered with the system or nil
 */
+ (id)getConnectPlugin:(NSString *)pluginKeyName;

/**
 *  Register a connect plugin object, it will assume the pluginKey as key
 *
 *  @param plugin The connect plugin to register with the manager
 */
+ (void)registerConnectPlugin:(id<RaveConnectPlugin>)plugin;

/**
 *  Register a built-in connect plugin by registration name
 *
 *  @param pluginKeyName The name of the built-in plugin to register, e.g. "facebook"
 */
+ (void)registerConnectPluginByName:(NSString *)pluginKeyName;

/**
 *  Authentication and connection management methods
 */

/**
 *  Log in to Rave anomymously
 *
 *  @param callback Completion callback for operations success
 */
+ (void)loginAsGuest:(RaveCompletionCallback)callback;

/**
 *  Login to Rave using the plugin specified
 *
 *  Authenticate with Rave using access token data for the specified plugin
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *  @param callback   A login callback or nil if no notification is desired
 */
+ (void)loginWith:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback;

/**
 *  Connect a plugin account to Rave with the plugin specified
 *
 *  Connect and authenticate with Rave using the access token data for the specified plugin
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *  @param callback   A connect callback or nil if no notification is desired
 */
+ (void)connectTo:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback;

/**
 *  Forcefully connect a plugin account to Rave with the plugin specified
 *
 *  Connect and authenticate with Rave using the access token data for the specified plugin
 *  ForceConnect will attempt to forceably link the plugin account with the current Rave account
 *  regardless of whether there's already another account of this type connected
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *  @param callback   A connect callback or nil if no notification is desired
 */
+ (void)forceConnectTo:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback;

/**
 *  Disconnect the plugin account from your current Rave account
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *  @param callback   A disconnect callback or nil if no notification is desired
 */
+ (void)disconnectFrom:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback;

/**
 *  Readiness management methods
 */

/**
 *  The last known readiness of the specified plugin
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *
 *  @return YES if the plugin is ready, otherwise NO
 */
+ (BOOL)lastKnownReadinessOf:(NSString *)pluginKeyName;

/**
 *  Check for the current readiness state of the plugin, it may have changed since the last time readiness was cached
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *  @param callback   A readiness callback, or nil if no notification is desired
 */
+ (void)checkReadinessOf:(NSString *)pluginKeyName callback:(RaveReadinessCallback)callback;

/**
 *  Token management methods
 */

/**
 *  The current access token for the plugin
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *
 *  @return The current access token or nil
 */
+ (NSString *)getCurrentTokenFrom:(NSString *)pluginKeyName;

/**
 *  The current token for the plugin and token type specified
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *  @param tokenType  The token type to get (e.g. access token or token secret)
 *
 *  @return The requested token or nil
 */
+ (NSString *)getCurrentTokenFrom:(NSString *)pluginKeyName tokenType:(RaveConnectPluginTokenType)tokenType;

/**
 *  This method fetches the connected state for the specified plugin from the Rave backend where applicable
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *  @param callback   A connect state callback or nil if no notification is desired
 */
+ (void)fetchConnectStateOf:(NSString *)pluginKeyName callback:(RaveConnectStateCallback)callback;

/**
 *  Log out of the current Rave account clearing all cached tokens in plugins.  If RaveSettings.General.AutoGuestLogin is enabled an anonymous guest user will automatically be created
 */
+ (void)logOut;

/**
 *  A method to explicitly set a token for a plugin to use to connect to Rave
 *
 *  This method is typically used if a token has been acquired in an app prior to integrating Rave.  Usually after that point the token should be cleared as you won't need to upgrade again.  In addition if you use the normal method of token storage for the given plugin this method doesn't need to be called at all.
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *  @param token      The token to give to Rave for automatic authentication attempts
 */
+ (void)useTokenForRaveUpgradeWith:(NSString *)pluginKeyName token:(NSString *)token;

/**
 *  Policy management
 */

/**
 *  Method to set the disconnect policy for all plugins.  Will override any policy previously set with setDisconnectPolicyFor
 *
 *  @param policy The disconnect policy to be used for each plugin
 */
+ (void)setDefaultDisconnectPolicy:(id<RaveDisconnectPolicy>)policy;

/**
 *  Method to set a the disconnect policy for a specific plugin.  Will override any policy previously set for this plugin
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *  @param policy     The disconnect policy to be used for the given plugin
 */
+ (void)setDisconnectPolicyFor:(NSString *)pluginKeyName policy:(id<RaveDisconnectPolicy>)policy;

/**
 *  Method to set conflict resolution policy for all plugins.  Will override any policy previously set with setConflictResolutionPolicyFor
 *
 *  @param policy The conflict resolution to be used for each plugin
 */
+ (void)setDefaultConflictResolutionPolicy:(id<RaveConflictResolutionPolicy>)policy;

/**
 *  Method to set the conflict resolution policy for a given plugin.  Will override any policy previously set for this plugin
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *  @param policy     The conflict resolution policy to be used for the given plugin
 */
+ (void)setConflictResolutionPolicyFor:(NSString *)pluginKeyName policy:(id<RaveConflictResolutionPolicy>)policy;

/**
 *  Method to set the default invalid token policy for all plugins.  Will override any policy previously set for a given plugin
 *
 *  @param policy The invalid token policy to be used for each plugin
 */
+ (void)setDefaultInvalidTokenPolicy:(id<RaveInvalidTokenPolicy>)policy;

/**
 *  Method to set an invalid token policy for a given plugin
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *  @param policy     The invalid token policy to be used for the given plugin
 */
+ (void)setInvalidTokenPolicyFor:(NSString *)pluginKeyName policy:(id<RaveInvalidTokenPolicy>)policy;

/**
 *  Method to set the default token import policy for all plugins.  Will override any policy previously set for a given plugin
 *
 *  @param policy The token import policy to be used for each plugin
 */
+ (void)setDefaultTokenImportPolicy:(id<RaveTokenImportPolicy>)policy;

/**
 *  Method to set the token import policy for for a given plugin
 *
 *  @param pluginKeyName The named key for the plugin to use, e.g. "facebook"
 *  @param policy The token import policy to be used for each plugin
 */
+ (void)setTokenImportPolicyFor:(NSString *)pluginKeyName policy:(id<RaveTokenImportPolicy>)policy;

+ (void)fetchThirdPartyInfo:(NSString *)source callback:(RaveConnectStateCallback)callback;
@end
