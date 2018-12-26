/// \file bfgutils.h
/// \brief BFG utilities
///
// \author Big Fish Games
// \copyright Copyright 2013 Big Fish Games. All rights reserved.
/// \details Helpful utility methods.

#import <bfg_iOS_sdk/bfglibPrefix.h>


/// \brief bfgutils class
@interface bfgutils : NSObject

/// \return
/// \retval YES if the current iOS version is greater than or equal to versionString.
/// \retval NO if otherwise.
+ (BOOL)systemVersionGreaterThanOrEqualTo:(NSString *)versionString;

/// \return A Big Fish identifier for the user. NOT Apple's UDID.
+ (NSString *)bfgUDID;

/// \return
/// \retval YES if IFA tracking has been enabled.
/// \retval NO if otherwise.
+ (BOOL)bfgIFAEnabled; // Return if IFA tracking has been enabled/disabled

/// \return
/// \retval IFA for device if IFA tracking has been enabled
/// \retval nil if IFA tracking is disabled
+ (NSString *)bfgIFA; // Return the IFA for device

/// \return IDFV for device, can be nil in rare circumstances.
/// \since 5.8
+ (NSString *)bfgIDFV;

/// \return Country code of the user's device.
+ (NSString *)userCountryCode;

/// \return Preferred language of the user's device.
+ (NSString *)userPreferredLanguage;

@end
