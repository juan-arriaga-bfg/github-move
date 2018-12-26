//
//  BfgLegacyDeviceManagerWrapper.mm
//  Unity-iPhone
//
//  Created by Michael Molien on 01/04/17.
//  Copyright 2017 Big Fish Games, Inc. All rights reserved.
//


#import <bfg_iOS_sdk/bfgLegacyDeviceManager.h>
#import "UnityWrapperUtilities.h"

extern "C"
{
//        + (BOOL)checkForDeviceGeneration:(BFGDeviceIdentifier)deviceIdentifier;
//        + (BOOL)checkForDeviceGeneration:(BFGDeviceIdentifier)deviceIdentifier andAlert:(BOOL)displayAlert withTitle:(NSString *)title message:(NSString *)message dismissButtonTitle:(NSString *)dismissButtonTitle;
//        + (NSString *)commonDeviceNameForIdentifier:(BFGDeviceIdentifier)deviceIdentifer;
//        + (BFGDeviceIdentifier)currentDeviceIdentifier;

    BOOL __bfgLegacyDeviceManager__checkForDeviceGeneration()
    {
        BOOL result =[bfgLegacyDeviceManager checkForDeviceGeneration:[bfgLegacyDeviceManager currentDeviceIdentifier]];
        return result;
    }

    BOOL __bfgLegacyDeviceManager__checkForDeviceGenerationDisplayAlert(
     const BOOL displayAlert,
     const char* title,
     const char* message,
     const char* dismissButtonTitle )
    {
        BOOL result = [bfgLegacyDeviceManager checkForDeviceGeneration:[bfgLegacyDeviceManager currentDeviceIdentifier]
                                    andAlert:displayAlert
                                    withTitle:[NSString stringWithUTF8String:title]
                                    message:[NSString stringWithUTF8String:message]
                                    dismissButtonTitle:[NSString stringWithUTF8String:dismissButtonTitle]];
        return result;
    }

    int __bfgLegacyDeviceManager__commonDeviceNameForIdentifier(char* returnDeviceName, int bufferSize)
    {
    	NSString *_deviceIdentifier = [bfgLegacyDeviceManager commonDeviceNameForIdentifier:[bfgLegacyDeviceManager currentDeviceIdentifier]];
        NSUInteger length = [_deviceIdentifier lengthOfBytesUsingEncoding:NSUTF8StringEncoding];
        if (bufferSize > length)
        {
            copyStringToBuffer( _deviceIdentifier, returnDeviceName, bufferSize );
        }
        return (int) length;
    }
}
