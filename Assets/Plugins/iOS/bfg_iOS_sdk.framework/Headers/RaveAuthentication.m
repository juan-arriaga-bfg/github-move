//
//  RaveAuthentication.m
//  RaveSocial
//
//  Created by gwilliams on 6/3/15.
//  Copyright (c) 2015 Rave, Inc. All rights reserved.
//

#import "RaveAuthentication.h"

@implementation RaveAuthentication
+ (NSArray *)connectPlugins {
    return [RaveSocial.authenticationManager connectPlugins];
}

+ (id)getConnectPlugin:(NSString *)pluginKeyName {
    return [RaveSocial.authenticationManager getConnectPlugin:pluginKeyName];
}

+ (void)registerConnectPlugin:(id<RaveConnectPlugin>)plugin {
    [RaveSocial.authenticationManager registerConnectPlugin:plugin];
}

+ (void)registerConnectPluginByName:(NSString *)pluginKeyName {
    [RaveSocial.authenticationManager registerConnectPluginByName:pluginKeyName];
}

+ (void)loginAsGuest:(RaveCompletionCallback)callback {
    [RaveSocial.authenticationManager loginAsGuest:callback];
}

+ (void)loginWith:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback {
    [RaveSocial.authenticationManager loginWith:pluginKeyName callback:callback];
}

+ (void)connectTo:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback {
    [RaveSocial.authenticationManager connectTo:pluginKeyName callback:callback];
}

+ (void)forceConnectTo:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback {
    [RaveSocial.authenticationManager forceConnectTo:pluginKeyName callback:callback];
}

+ (void)disconnectFrom:(NSString *)pluginKeyName callback:(RaveCompletionCallback)callback {
    [RaveSocial.authenticationManager disconnectFrom:pluginKeyName callback:callback];
}

+ (BOOL)lastKnownReadinessOf:(NSString *)pluginKeyName {
    return [RaveSocial.authenticationManager lastKnownReadinessOf:pluginKeyName];
}

+ (void)checkReadinessOf:(NSString *)pluginKeyName callback:(RaveReadinessCallback)callback {
    [RaveSocial.authenticationManager checkReadinessOf:pluginKeyName callback:callback];
}

+ (NSString *)getCurrentTokenFrom:(NSString *)pluginKeyName {
    return [RaveSocial.authenticationManager getCurrentTokenFrom:pluginKeyName];
}

+ (NSString *)getCurrentTokenFrom:(NSString *)pluginKeyName tokenType:(RaveConnectPluginTokenType)tokenType {
    return [RaveSocial.authenticationManager getCurrentTokenFrom:pluginKeyName tokenType:tokenType];
}

+ (void)fetchConnectStateOf:(NSString *)pluginKeyName callback:(RaveConnectStateCallback)callback {
    [RaveSocial.authenticationManager fetchConnectStateOf:pluginKeyName callback:callback];
}

+ (void)logOut {
    [RaveSocial.authenticationManager logOut];
}

+ (void)useTokenForRaveUpgradeWith:(NSString *)pluginKeyName token:(NSString *)token {
    [RaveSocial.authenticationManager useTokenForRaveUpgradeWith:pluginKeyName token:token];
}

+ (void)setDefaultDisconnectPolicy:(id<RaveDisconnectPolicy>)policy {
    [RaveSocial.authenticationManager setDefaultDisconnectPolicy:policy];
}

+ (void)setDisconnectPolicyFor:(NSString *)pluginKeyName policy:(id<RaveDisconnectPolicy>)policy {
    [RaveSocial.authenticationManager setDisconnectPolicyFor:pluginKeyName policy:policy];
}

+ (void)setDefaultConflictResolutionPolicy:(id<RaveConflictResolutionPolicy>)policy {
    [RaveSocial.authenticationManager setDefaultConflictResolutionPolicy:policy];
}

+ (void)setConflictResolutionPolicyFor:(NSString *)pluginKeyName policy:(id<RaveConflictResolutionPolicy>)policy {
    [RaveSocial.authenticationManager setConflictResolutionPolicyFor:pluginKeyName policy:policy];
}

+ (void)setDefaultInvalidTokenPolicy:(id<RaveInvalidTokenPolicy>)policy {
    [RaveSocial.authenticationManager setDefaultInvalidTokenPolicy:policy];
}

+ (void)setInvalidTokenPolicyFor:(NSString *)pluginKeyName policy:(id<RaveInvalidTokenPolicy>)policy {
    [RaveSocial.authenticationManager setInvalidTokenPolicyFor:pluginKeyName policy:policy];
}

+ (void)setDefaultTokenImportPolicy:(id<RaveTokenImportPolicy>)policy {
    [RaveSocial.authenticationManager setDefaultTokenImportPolicy:policy];
}

+ (void)setTokenImportPolicyFor:(NSString *)pluginKeyName policy:(id<RaveTokenImportPolicy>)policy {
    [RaveSocial.authenticationManager setTokenImportPolicyFor:pluginKeyName policy:policy];
}

+ (void)fetchThirdPartyInfo:(NSString *)source callback:(RaveConnectStateCallback)callback {
    [RaveSocial.authenticationManager fetchThirdPartyInfo:source callback:callback];
}
@end
