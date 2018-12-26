//
/// \file bfgsettings.h
/// \brief A mechanism for persisting key-value pairs
//
// \author Created by Sean Hummel on 10/2/10.
// \copyright Copyright (c) 2013 Big Fish Games, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/bfglibPrefix.h>

typedef int bfgSettingsStorageType;

/// \brief Used to represent the plist storage
///
/// \note Default value is 0
FOUNDATION_EXPORT bfgSettingsStorageType const kBfgSettingsStorageTypePlist;

/// \brief Used to represent the local keychain storage
///
/// \note Default value is 1
///
/// \since 6.0
FOUNDATION_EXPORT bfgSettingsStorageType const kBfgSettingsStorageTypeLocalKeychain;

/// \brief Used to represent the shared keychain storage
///
/// \note Default value is 2
///
/// \since 6.0
FOUNDATION_EXPORT bfgSettingsStorageType const kBfgSettingsStorageTypeSharedKeychain;

/// \brief Used to represent the coded storage
///
/// \note Default value is 3
FOUNDATION_EXPORT bfgSettingsStorageType const kBfgSettingsStorageTypeCoded;

/// \brief  Used to represent the disk mapped store.
///         Files are marked to _not_ be backed up.
///
/// \note Values of this class are stored on disk and not retained. Meant for heavy-weight objects. Default value is 4
///
/// \since 5.4
FOUNDATION_EXPORT bfgSettingsStorageType const kBfgSettingsStorageTypeDiskMapped;

/// \details Value is "start_datetime". Associate with dates in format: 'yyyy-MM-dd HH:mm:ss zzz'
///
/// \since 5.10
FOUNDATION_EXPORT NSString * const kBFGSettings_StartDate;

/// \details Value is "end_datetime". Associate with dates in format: 'yyyy-MM-dd HH:mm:ss zzz'
///
/// \since 5.10
FOUNDATION_EXPORT NSString * const kBFGSettings_EndDate;

///
///
/// \since 6.4
FOUNDATION_EXPORT NSString * const kMGCUpsellBackgroundImage;

// SDK Settings -- Many or all of these macros will likely become private in the future

///
/// \brief Stores settings to disk automatically so that other libraries can use keys to retrieve them.
///
/// Developers can use this class to store settings used in their own libraries.
///
@interface bfgSettings : NSObject


///
/// \return Is this storage type available for use?
/// \retval YES Storage type is available for use.
/// \retval NO Storage type is not available for use.
///
/// \since 4.7
///
+ (BOOL)canUseStorageType:(bfgSettingsStorageType)storageType;


///
/// \details Serializes the settings plist to disk.
///
/// \return
/// \retval YES if serialization completed successfully.
/// \retval NO if serialization did not complete.
///
/// \since 2.2
///
+ (BOOL)write;


///
/// \details Persists the relevant settings to the requested storage type.
///
/// \return
/// \retval YES if serialization completed successfully.
/// \retval NO if serialization did not complete.
///
/// \since 4.7
///
+ (BOOL)writeStorageType:(bfgSettingsStorageType)storageType;


///
/// \details Sets a settings value for a key for the settings plist.
///
/// \param key The key to write the value to.
/// \param value The value to write.
///
/// \since 2.2
///
+ (void)set:(NSString*)key value:(id)value;


///
/// \details Sets a settings value for a key for the requested storage type.
///
/// \param key The key to write the value to.
/// \param value The value to write.
/// \param storageType The medium on which to persist the values (plist, local keychain, etc.).
///
/// \since 4.7
///
+ (void)set:(NSString*)key value:(id)value storageType:(bfgSettingsStorageType)storageType;


///
/// \details Gets a setting value for a key from the settings plist.
///
/// \param key The key for the value to be retrieved.
/// \return The value from the settings.
///
/// \since 2.2
///
+ (id)get:(NSString*)key;


///
/// \details Gets a setting value for a key from the requested storage type.
///
/// \param key The key for the value to be retrieved.
/// \param storageType The medium on which to persist the values (plist, local keychain, etc.).
/// \return The value from the settings.
///
/// \since 4.7
///
+ (id)get:(NSString *)key storageType:(bfgSettingsStorageType)storageType;


///
/// \details Gets a setting value for a key from the settings plist, and allows for default if the key does not exist.
///
/// \param key The key for the value to be retrieved.
/// \param defaultValue The default value to be returned when no value is set for the key.
/// \return The value from settings, or withDefault if nil.
///
/// \since 2.2
///
+ (id)get:(NSString*)key withDefault:(id)defaultValue;


///
/// \details Gets a settings value for a key from the requested storage type,
/// and allows for default if the key does not exist.
///
/// \param key The key for the value to be retrieved.
/// \param defaultValue The default value to be returned when no value is set for the key.
/// \param storageType The medium on which to persist the values (plist, local keychain, etc.).
/// \return The value from settings, or withDefault if nil.
///
/// \since 4.7
///
+ (id)get:(NSString*)key withDefault:(id)defaultValue storageType:(bfgSettingsStorageType)storageType;


///
/// \details Not all objects are serializable. Perform a quick check on a value
/// to determine its eligibility.
///
/// \param value A value we wish to write to the storage medium.
/// \param storageType The medium on which to persist the values (plist, local keychain, etc.).
///
/// \retval YES value can be written to specified storage type.
/// \retval NO value cannot be written to specified storage type.
///
/// \since 4.7
///
+ (BOOL)isPersistable:(id)value storageType:(bfgSettingsStorageType)storageType;


///
/// \details Gets a settings value for a key and checks the type of the retrieved value
/// against the passed in type parameter.
///
/// \param key The key for the value to be retrieved.
/// \param type The desired object type of the retrieved value.
///
/// \return The value from settings, or nil if the object type of the retreived value and
/// passed in type parameter do not match.
///
+ (id)get:(NSString*)key ofType:(Class)type;

///
/// \details Gets a settings value for a key and checks the type of the retrieved value
/// against the passed in type parameter.  Allows for a default value if the key does not
/// exist.
///
/// \param key The key for the value to be retrieved.
/// \param type The desired object type of the retrieved value.
/// \param defaultValue The default value to be returned when no value is set for the key
/// or the retreived values' object type does not match the passed in type parameter.
///
/// \return The value from settings, or defaultValue if either the object type of the retreived
/// value and type parameter do not match or if no value is set for the key.
///
/// \since 6.0
///
+ (id)get:(NSString*)key ofType:(Class)type withDefault:(id)defaultValue;

///
/// \details Gets a settings value for a key from the requested store and checks
/// the type of the retrieved value against the passed in type parameter.  Allows
/// for a default value if the key does not exist.
///
/// \param key The key for the value to be retrieved.
/// \param type The desired object type of the retrieved value.
/// \param defaultValue The default value to be returned when no value is set for the key
/// or the retreived value's object type does not match the passed in type parameter.
/// \param storageType The medium on which to persist the values (plist, local keychain, etc.).
///
/// \return The value from settings, or defaultValue if either the object type of the retreived
/// value and type parameter do not match or if no value is set for the key.
///
/// \since 6.0
///
+ (id)get:(NSString*)key ofType:(Class)type withDefault:(id)defaultValue storageType:(bfgSettingsStorageType)storageType;

///
/// \details Gets a settings value for a key from the requested store and checks the
/// type of the retrieved value against the passed in type parameter.
///
/// \param key The key for the value to be retrieved.
/// \param type The desired object type of the retrieved value.
/// \param storageType The medium on which to persist the values (plist, local keychain, etc.).
///
/// \return The value from settings, or nil if the object type of the retreived value and
/// the passed in type parameter do not match.
///
/// \since 6.0
///
+ (id)get:(NSString *)key ofType:(Class)type storageType:(bfgSettingsStorageType)storageType;


///
/// \details Gets a settings value for a key from the requested store, checks the
/// type of the retrieved value against the passed in type parameter, and allows for a
/// default value if the key does not exist in the store.  In additin, this method sets
/// the passed in NSError** when there is a type mismatch.
///
/// \param key The key for the value to be retrieved.
/// \param type The desired object type of the retrieved value.
/// \param defaultValue the default value to be returned when no value is set for the key
/// or the retreived value's object type does not match the passed in type parameter.
/// \param storageType The medium on which to persist the value.
/// \param error The NSError object that will be populated if the a type mismatch is
/// detected.
+ (id)get:(NSString *)key ofType:(Class)type withDefault:(id)defaultValue storageType:(bfgSettingsStorageType)storageType error:(NSError * __autoreleasing * _Nullable)error;

/// \details Gets the file path for a resource persisted through bfgDiskMappedPersister
///
/// \param key The key used to write and retreive the value from the persisted store.
/// \return The file path for the resource or nil if the resouce cannot be found.
///
/// \sincd 6.6
+ (NSString*)filePathforDiskMappedResource:(NSString*)key;

@end


// ******************************************************************************************

#pragma mark - Game settings

/// \brief Unique Hashed Bundle ID. Supplied by your Big Fish Producer
#define BFG_HASHED_BUNDLED_ID_KEY                       @"hbi"

/// \brief The app's ID with Apple
#define BFG_SETTING_APPLE_APP_ID                        @"app_store_id"

// User Acquisition (UA) Tracking

/// \brief This key is for a dictionary of settings for "Tune" configuration, which used to be called "HasOffers". This is a user-acquisition tool.
#define BFG_SETTING_HASOFFERS                           @"hasoffers"


/// \brief This key is for a dictionary of settings for "Upsight Analytics", which used to be called "Kontagent". This is a business-intelligence tool.
#define BFG_SETTING_KONTAGENT                           @"kontagent"

/// \brief This key is for a bool to enable / disable Fabric (Crash Analytics). This is crash monitoring tool.
#define BFG_SETTING_CRASH_ANALYTICS                     @"crash_analytics"

/// \brief UID set by the game through setUserID
///
/// \since 5.11
#define BFG_SETTING_GAME_UID                            @"game_set_uid"

/// \brief Used to determine if non-KPI events should be throttled (dropped)
///
/// \details If the value is different than 100 the non-KPI events will be throttled.
///          This value defaults to 100.
///
/// \since 6.0
#define BFG_SETTING_REPORTING_PERCENTAGE                @"reporting_percentage"

/// \brief This key is for a dictionary of "Rave" settings. Rave manages user authetication.
///
/// \note You *MUST* have Rave settings in both your bfg_first_launch_settings.json and bfg_upgrade_settings.json.
///
/// \since 6.0
#define BFG_SETTING_RAVE                                @"rave"

/// \since 6.3
#define BFG_SETTING_TOAST_ENABLED                       @"bfg_toast_enabled"


// *** DEBUG SECTION ***
#ifdef BFGLIB_DEBUG

/// \note See the documentation for information about debugging purchase testing.
///
/// \since 5.7
#define BFG_SETTING_PURCHASE_DEBUG_MODE                 @"purchaseDebugMode"

/// \since 5.7
#define BFG_SETTING_PURCHASE_DEBUG_PRODUCT_ID           @"purchaseDebugProductId"

/// \since 5.7
#define BFG_SETTING_PURCHASE_DEBUG_CAN_START_PURCHASE   @"purchaseDebugCanStartPurchase"

/// \since 5.7
#define BFG_SETTING_PURCHASE_RESTORABLE_PRODUCTS        @"purchaseDebugRestorableProducts"

#endif
// *** ***


// ******************************************************************************************

#pragma mark - Premium specific settings

/// \brief Used to specify GDN background image behavior.
///
/// 0 == gdn background is clear.
/// 1 == always show background image.
/// 2 == show background image on first GDN presentation only.
///
#define BFG_SETTING_GDN_BACKGROUND                      @"gdn_background"

/// \brief Used to specify wether to display or not the ratings prompt
///
/// \note By default the ratings prompt will be displayed
#define BFG_SETTING_RATING_ENABLED                      @"ratings_prompt"

/// \brief  Used to specif wether to display or not the dashboard
///
/// \note   The "dashboard" is the old name for the GDN (Game Discovery Network)
#define BFGPROMODASHBOARD_SETTING_ENABLED               @"dashboard_active"

/// \brief Wait at least this long before potentially showing dashboard
#define BFGPROMODASHBOARD_SETTING_TIMEOUT               @"dashboard_timeout"

/// \brief Ignore showing the dashboard while this is greater than zero.
///
/// \note Decremented on each attempt to show dashboard.
#define BFGPROMODASHBOARD_SETTING_TRIGGER_COUNTDOWN     @"dashboard_show_after_this_many_runs"

/// \brief How long to wait when loading the GDN from the disk cache
///
/// \note The GDN will likely look for this value before the startup call has completed, so the
/// value could lag until the next foregrounding of the app.
///
/// \since 5.5
#define BFG_SETTING_GDN_CACHE_MAX_WAIT                  @"gdn_cache_max_wait"

/// \brief Mobile Game Club active for this game
///
/// \since 6.4
#define BFG_SETTING_SUBSCRIPTION_MODE                   @"premiumSubscriptionMode"

/// \brief Game specific Zendesk url provided by game.
///
/// \since 6.5
#define BFG_SETTING_ZENDESK_URL                         @"zendesk_url"

/// \brief Game specific Zendesk app identifier provided by game.
///
/// \since 6.5
#define BFG_SETTING_ZENDESK_APP_IDENTIFIER              @"zendesk_app_identifier"

/// \since 6.9
#define BFG_SETTING_APPSFLYER                           @"appsflyer"

