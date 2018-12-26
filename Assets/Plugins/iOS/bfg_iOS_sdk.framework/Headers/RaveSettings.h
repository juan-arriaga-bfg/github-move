//
//  RaveSettings.h
//  RaveSocial
//
//  Created by dsalced on 12/12/13.
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef enum {
    SETTING_BOOLEAN,
    SETTING_INTEGER,
    SETTING_STRING,
} RaveSettingsType;

extern NSString* RaveFBAutoUpdateContactsOnConnect; //  deprecated
extern NSString* RaveFBContactsUpdateInterval; //  deprecated
extern NSString* RaveFBApplicationId; //  deprecated
extern NSString* RaveFBReadPermissions; //  deprecated
extern NSString* RaveFBAlwaysUseLiveContacts; //  deprecated
extern NSString* RaveTWConsumerKey; //  deprecated
extern NSString* RaveTWConsumerSecret; //  deprecated
extern NSString* RaveTWCallbackURL; //  deprecated
extern NSString* RaveTWAllowWebAuthFallback; //  deprecated
extern NSString* RaveGPlusAutoUpdateContactsOnConnect; //  deprecated
extern NSString* RaveGplusContactsUpdateInterval; //  deprecated
extern NSString* RaveGplusClientID; //  deprecated
extern NSString* RaveGPlusShareContentURL; //  deprecated
extern NSString* RaveApplicationID; //  deprecated
extern NSString* RaveServerURL; //  deprecated
extern NSString* RaveAutoSyncInterval; //  deprecated
extern NSString* RaveAutoGuestLogin; //  deprecated
extern NSString* RaveAutoCrossAppLogin; //  deprecated
extern NSString* RaveInitGameCenterOnStartUp; //  deprecated
extern NSString* RaveAutoInstallConfigUpdates; //  deprecated
extern NSString* RaveLogLevel; //  deprecated
extern NSString* RaveAllowForceDisconnect; //  deprecated
extern NSString* RaveNetworkTimeout; //  deprecated
extern NSString* RavePhoneContactsUpdateInterval; //  deprecated
extern NSString* RaveConfigUpdateInterval; //  deprecated
extern NSString* RaveDefaultNewUserName; //  deprecated
extern NSString* RaveDefaultResourcesPath; //  deprecated
extern NSString* RaveThirdPartySource; //  deprecated
extern NSString* RaveAutoSendGiftOnGrantedRequest; //  deprecated
extern NSString* RaveFBUseGraphAPIv1; //  deprecated
extern NSString* RaveIOSBundleName; //  deprecated
extern NSString* RaveIOSUseIDFAForDeviceID; //  deprecated
extern NSString* RaveSceneServerHost; //  deprecated

@interface RaveSettingsMapSelectorsToKeys : NSObject
@end

@interface RaveSettingsGeneralKeys : RaveSettingsMapSelectorsToKeys
- (NSString *) ApplicationID;
- (NSString *) ServerURL;
- (NSString *) AutoSyncInterval;
- (NSString *) AutoGuestLogin;
- (NSString *) AutoCrossAppLogin;
- (NSString *) AutoMergeOnConnect;
- (NSString *) AutoInstallConfigUpdates;
- (NSString *) LogLevel;
- (NSString *) AllowForceDisconnect;
- (NSString *) NetworkTimeout;
- (NSString *) PhoneContactsUpdateInterval;
- (NSString *) ConfigUpdateInterval;
- (NSString *) DefaultNewUserName;
- (NSString *) DefaultResourcesPath;
- (NSString *) ThirdPartySource;
- (NSString *) AutoSendGiftOnGrantedRequest;
- (NSString *) SceneServerHost;
- (NSString *) AutoSyncFriends;
- (NSString *) ContactsUpdateInterval;
@end

@interface RaveSettingsFacebookKeys : RaveSettingsMapSelectorsToKeys
- (NSString *) ApplicationId;
- (NSString *) ReadPermissions;
- (NSString *) AlwaysUseLiveContacts;
- (NSString *) AppInviteURL;
- (NSString *) AppEventsEnabled;

- (NSString *) ContactsUpdateInterval;  //  deprecated
- (NSString *) UseGraphAPIv1;  // unsupported
- (NSString *) AutoUpdateContactsOnConnect;  // deprecated
@end

@interface RaveSettingsGooglePlusKeys : RaveSettingsMapSelectorsToKeys
- (NSString *) ClientID;
- (NSString *) ShareContentURL;

- (NSString *) ContactsUpdateInterval;  //  deprecated
- (NSString *) AutoUpdateContactsOnConnect;  // deprecated
@end

@interface RaveSettingsIOSKeys : RaveSettingsMapSelectorsToKeys
- (NSString *) InitGameCenterOnStartUp;
- (NSString *) BundleName;
- (NSString *) UseIDFAForDeviceID;
- (NSString *) ApplicationGroupIdentifier;
@end

@interface RaveSettingsDataMirrorKeys : RaveSettingsMapSelectorsToKeys
- (NSString *) Achievements;
- (NSString *) Leaderboards;
@end

@protocol RaveSettingValidator <NSObject>
-(void) validate:(NSString*)name value:(NSString*)value;
@end

@interface RaveSetting : NSObject
+(id)stringSettingWithDefault:(NSString *)defaultValue withValidators:(NSArray *)validators;
+(id)requiredStringSettingWithDefault:(NSString *)defaultValue withValidators:(NSArray *)validators;

+ (id)booleanSettingWithDefault:(NSString *)defaultValue withValidators:(NSArray *)validators;
+ (id)requiredBooleanSettingWithDefault:(NSString *)defaultValue withValidators:(NSArray *)validators;

+ (id)integerSettingWithDefault:(NSString *)defaultValue withValidators:(NSArray *)validators;
+ (id)requiredIntegerSettingWithDefault:(NSString *)defaultValue withValidators:(NSArray *)validators;
@end

@protocol RaveSettingListener <NSObject>
@required
-(void)onSettingChanged:(NSString*)name withValue:(NSString*)value;
@end

@interface RaveSettings : NSObject
+ (void) addSetting:(RaveSetting *)setting key:(NSString *)key fallbackKey:(NSString *)fallbackKey;

+ (RaveSettings*)sharedInstance;

+ (RaveSettingsGeneralKeys *)General;
+ (RaveSettingsFacebookKeys *)Facebook;
+ (RaveSettingsGooglePlusKeys *)GooglePlus;
+ (RaveSettingsDataMirrorKeys *)DataMirror;
+ (RaveSettingsIOSKeys *)IOS;

- (void) set:(NSString*)name toString:(NSString*)value;
- (void) set:(NSString*)name toBoolean:(BOOL)value;
- (void) set:(NSString*)name toInteger:(int)value;

- (NSString*) get:(NSString*)name;
- (BOOL) getAsBoolean:(NSString*)name;
- (int) getAsInteger:(NSString*)name;
- (NSArray *) getAsArray:(NSString*)name;

- (void) addSettingListener:(id<RaveSettingListener>)listener withSetting:(NSString*)name;
- (void) removeSettingListener:(NSString*)name  withSetting:(id<RaveSettingListener>)listener;

- (void)loadUnsetFromJSON:(NSURL*)configURL;
- (void)loadUnsetFromPlist;
@end
