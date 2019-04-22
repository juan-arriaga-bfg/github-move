///
/// \file bfgLegacyDeviceManager.h
/// \brief Utility for determining whether current device meets recommended hardware specifications for the app.
/// \since 6.0
///
// \author Anton Rivera 8/11/16
// \copyright Copyright 2016 Big Fish Games, Inc. All rights reserved.
///

#import <Foundation/Foundation.h>

/// List of identifiers of iOS devices
/// Format for each entry is "BFGDeviceIdentifier_<DeviceName>_<HardwareString>"
/// Thanks https://www.theiphonewiki.com/wiki/List_of_iPhones and https://www.theiphonewiki.com/wiki/List_of_iPads

typedef NS_ENUM(NSUInteger, BFGDeviceIdentifier)
{
        BFGDeviceIdentifier_Unknown,
    
        // iPhone
        BFGDeviceIdentifier_iPhone4S_41,
    
        BFGDeviceIdentifier_iPhone5_51,
        BFGDeviceIdentifier_iPhone5_52,
        BFGDeviceIdentifier_iPhone5_All,
    
        BFGDeviceIdentifier_iPhone5C_53,
        BFGDeviceIdentifier_iPhone5C_54,
        BFGDeviceIdentifier_iPhone5C_All,
    
        BFGDeviceIdentifier_iPhone5S_61,
        BFGDeviceIdentifier_iPhone5S_62,
        BFGDeviceIdentifier_iPhone5S_All,
    
        BFGDeviceIdentifier_iPhone6Plus_71,
    
        BFGDeviceIdentifier_iPhone6_72,
    
        BFGDeviceIdentifier_iPhone6S_81,
    
        BFGDeviceIdentifier_iPhone6SPlus_82,
    
        BFGDeviceIdentifier_iPhoneSE_84,
    
        BFGDeviceIdentifier_iPhone7_91,
        BFGDeviceIdentifier_iPhone7_93,
        BFGDeviceIdentifier_iPhone7_All,
    
        BFGDeviceIdentifier_iPhone7Plus_92,
        BFGDeviceIdentifier_iPhone7Plus_94,
        BFGDeviceIdentifier_iPhone7Plus_All,
    
        BFGDeviceIdentifier_iPhone8_101,
        BFGDeviceIdentifier_iPhone8_104,
        BFGDeviceIdentifier_iPhone8_All,
    
        BFGDeviceIdentifier_iPhone8Plus_102,
        BFGDeviceIdentifier_iPhone8Plus_105,
        BFGDeviceIdentifier_iPhone8Plus_All,
    
        BFGDeviceIdentifier_iPhoneX_103,
        BFGDeviceIdentifier_iPhoneX_106,
        BFGDeviceIdentifier_iPhoneX_All,
    
        BFGDeviceIdentifier_iPhoneXS_112,
    
        BFGDeviceIdentifier_iPhoneXSMax_114,
        BFGDeviceIdentifier_iPhoneXSMax_116,
        BFGDeviceIdentifier_iPhoneXSMax_All,
    
        BFGDeviceIdentifier_iPhoneXR,
    
        // iPod
        BFGDeviceIdentifier_iPodTouch5G_51,
        BFGDeviceIdentifier_iPodTouch6G_71,
    
        // iPad
        BFGDeviceIdentifier_iPad2_21,
        BFGDeviceIdentifier_iPad2_22,
        BFGDeviceIdentifier_iPad2_23,
        BFGDeviceIdentifier_iPad2_24,
        BFGDeviceIdentifier_iPad2_All,
    
        BFGDeviceIdentifier_iPad3_31,
        BFGDeviceIdentifier_iPad3_32,
        BFGDeviceIdentifier_iPad3_33,
        BFGDeviceIdentifier_iPad3_All,
    
        BFGDeviceIdentifier_iPad4_34,
        BFGDeviceIdentifier_iPad4_35,
        BFGDeviceIdentifier_iPad4_36,
        BFGDeviceIdentifier_iPad4_All,
    
        // iPad Air
        BFGDeviceIdentifier_iPadAir_41,
        BFGDeviceIdentifier_iPadAir_42,
        BFGDeviceIdentifier_iPadAir_43,
        BFGDeviceIdentifier_iPadAir_All,
    
        BFGDeviceIdentifier_iPadAir2_53,
        BFGDeviceIdentifier_iPadAir2_54,
        BFGDeviceIdentifier_iPadAir2_All,
    
        // iPad Pro
        BFGDeviceIdentifier_iPad2017_611,
        BFGDeviceIdentifier_iPad2017_612,
        BFGDeviceIdentifier_iPad2017_All,
    
        BFGDeviceIdentifier_iPad2018_75,
        BFGDeviceIdentifier_iPad2018_76,
        BFGDeviceIdentifier_iPad2018_All,
    
        BFGDeviceIdentifier_iPadPro12in_67,
        BFGDeviceIdentifier_iPadPro12in_68,
        BFGDeviceIdentifier_iPadPro12in_All,
    
        BFGDeviceIdentifier_iPadPro212in_71,
        BFGDeviceIdentifier_iPadPro212in_72,
        BFGDeviceIdentifier_iPadPro212in_All,
    
        BFGDeviceIdentifier_iPadPro312in_85,
        BFGDeviceIdentifier_iPadPro312in_86,
        BFGDeviceIdentifier_iPadPro312in_87,
        BFGDeviceIdentifier_iPadPro312in_88,
        BFGDeviceIdentifier_iPadPro312in_All,
    
        BFGDeviceIdentifier_iPadPro9in_63,
        BFGDeviceIdentifier_iPadPro9in_64,
        BFGDeviceIdentifier_iPadPro9in_All,
    
        BFGDeviceIdentifier_iPadPro10in_73,
        BFGDeviceIdentifier_iPadPro10in_74,
        BFGDeviceIdentifier_iPadPro10in_All,
    
        BFGDeviceIdentifier_iPadPro11in_81,
        BFGDeviceIdentifier_iPadPro11in_82,
        BFGDeviceIdentifier_iPadPro11in_83,
        BFGDeviceIdentifier_iPadPro11in_84,
        BFGDeviceIdentifier_iPadPro11in_All,
    
        // iPad Mini
        BFGDeviceIdentifier_iPadMini_25,
        BFGDeviceIdentifier_iPadMini_26,
        BFGDeviceIdentifier_iPadMini_27,
        BFGDeviceIdentifier_iPadMini_All,
    
        BFGDeviceIdentifier_iPadMini2_44,
        BFGDeviceIdentifier_iPadMini2_45,
        BFGDeviceIdentifier_iPadMini2_46,
        BFGDeviceIdentifier_iPadMini2_All,
    
        BFGDeviceIdentifier_iPadMini3_47,
        BFGDeviceIdentifier_iPadMini3_48,
        BFGDeviceIdentifier_iPadMini3_49,
        BFGDeviceIdentifier_iPadMini3_All,
    
        BFGDeviceIdentifier_iPadMini4_51,
        BFGDeviceIdentifier_iPadMini4_52,
        BFGDeviceIdentifier_iPadMini4_All
} __deprecated_msg("Deprecated in 6.9.1");

/// List of identifiers of unsupported iOS devices (max iOS 7.0 and below)
/// This value is only for completeness.  It should not be used.
typedef NS_ENUM(unsigned long long, BFGDeviceIdentifier_pre_minimum_ios_supported)
{
    BFGDeviceIdentifier_pre_minimum_iPhone,
    BFGDeviceIdentifier_pre_minimum_iPhone3G,
    BFGDeviceIdentifier_pre_minimum_iPhone3GS,
    BFGDeviceIdentifier_pre_minimum_iPhone4,

    BFGDeviceIdentifier_pre_minimum_iPad,

    BFGDeviceIdentifier_pre_minimum_iPodTouch,
    BFGDeviceIdentifier_pre_minimum_iPodTouch2G,
    BFGDeviceIdentifier_pre_minimum_iPodTouch3G,
    BFGDeviceIdentifier_pre_minimum_iPodTouch4G
} __deprecated_msg("Deprecated in 6.9.1");

///
/// \brief Utility for determining whether current device meets recommended hardware specifications for the app.
/// \since 6.0
///
/// \deprecated Deprecated as of 6.9.1.  This functionality will be removed in a future release.
///
__deprecated_msg("This feature will continue to function for SDK 6.9.1 but will be removed in a future SDK release.")
@interface bfgLegacyDeviceManager : NSObject

///
/// \brief Identify whether the current device matches the deviceIdentifier(s). Might be used to
/// tailor game experience or warn user that their experience might not be great on their current
/// hardware.
///
/// \param deviceIdentifier Device identifier(s) to be checked against.
/// \return
/// \retval YES if deviceIdentifier matches current device.
/// \retval NO if deviceIdentifier does not match current device.
/// \since 6.0
///
/// \deprecated Deprecatd in 6.9.1.
///
+ (BOOL)checkForDeviceGeneration:(BFGDeviceIdentifier)deviceIdentifier  __deprecated_msg("Deprecated in 6.9.1");

///
/// \brief Identify whether the current device matches the deviceIdentifier(s). Might be used to
/// tailor game experience or warn user that their experience might not be great on their current
/// hardware.
///
/// \param deviceIdentifier Device identifier(s) to be checked against.
/// \param displayAlert If device is too old, display an alert.
/// \param title Title of alert for device being too old.
/// \param message If device too old, show an alert with this message.
/// \param dismissButtonTitle If device too old, show an alert with this dismiss button title.
/// \return
/// \retval YES if deviceIdentifier matches current device.
/// \retval NO if deviceIdentifier does not match current device.
/// \since 6.0
///
/// \deprecated Deprecatd in 6.9.1.
///
+ (BOOL)checkForDeviceGeneration:(BFGDeviceIdentifier)deviceIdentifier andAlert:(BOOL)displayAlert withTitle:(NSString *)title message:(NSString *)message dismissButtonTitle:(NSString *)dismissButtonTitle    __deprecated_msg("Deprecated in 6.9.1");


///
/// \return The common name (such as iPhone4 or iPhone6Plus) of a device.
/// \since 6.0
///
/// \deprecated Deprecatd in 6.9.1.
///
+ (NSString *)commonDeviceNameForIdentifier:(BFGDeviceIdentifier)deviceIdentifer    __deprecated_msg("Deprecated in 6.9.1");

///
/// \return The models of the device currently running the app.
/// \since 6.0
///
/// \deprecated Deprecatd in 6.9.1.
///
+ (BFGDeviceIdentifier)currentDeviceIdentifier  __deprecated_msg("Deprecated in 6.9.1");

@end
